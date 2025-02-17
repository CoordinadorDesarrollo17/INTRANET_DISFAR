using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.AtencionCliente_ENT.TablasSql;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class UbicacionesReserva_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<UbicacionesReserva_E> ListarUbicacionesReserva(string condicion, Dictionary<string, object> parametrosCondicion)
        {
            List<UbicacionesReserva_E> lista = new List<UbicacionesReserva_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT UL.[Id], UL.[UbicacionId], UL.[Almacen], UL.[ItemCode], UL.[ItemName], UL.[CodigoUbicacion], UL.[BatchNum], UL.[Quantity],");
                    sb.AppendLine("SM.[StockMinAbastecimiento], SM.[StockMinVenta]");
                    sb.AppendLine("FROM [dbo].[UbicacionesLotes] UL");
                    sb.AppendLine("OUTER APPLY (SELECT TOP 1 SM.[StockMinAbastecimiento], SM.[StockMinVenta] FROM [dbo].[StockMinProductos] SM WHERE SM.[ItemCode] = UL.[ItemCode]) SM");
                    sb.AppendLine("WHERE UL.[Almacen] = 'RESERVA'");
                    sb.AppendLine(condicion);

                    // Agregamos los parámetros dinámicamente
                    foreach (var param in parametrosCondicion)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    cmd.CommandText = sb.ToString();

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {   
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var obj = new UbicacionesReserva_E();

                                if (!dr.IsDBNull(0)) obj.Id = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) obj.UbicacionId = dr.GetInt32(1);
                                if (!dr.IsDBNull(2)) obj.Almacen = dr.GetString(2);
                                if (!dr.IsDBNull(3)) obj.ItemCode = dr.GetString(3);
                                if (!dr.IsDBNull(4)) obj.ItemName = dr.GetString(4);
                                if (!dr.IsDBNull(5)) obj.CodigoUbicacion = dr.GetString(5);
                                if (!dr.IsDBNull(6)) obj.BatchNum = dr.GetString(6);
                                if (!dr.IsDBNull(7)) obj.Quantity= dr.GetDecimal(7);
                                if (!dr.IsDBNull(8)) obj.StockMinAbastecimiento = dr.GetInt32(8);
                                if (!dr.IsDBNull(9)) obj.StockMinVenta = dr.GetInt32(9);

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesReserva_D - ListarUbicacionesReserva");
            }

            return lista;
        }

        public Helper_E RegistrarUbicacionReserva(UbicacionesReserva_E datos)
        {
            string mensaje, icono;
            int id = 0;

            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                cn.Open();
                SqlTransaction transaction = cn.BeginTransaction();

                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AdministrarUbicaciones", cn, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Operacion", "INSERT");
                        cmd.Parameters.AddWithValue("@Almacen", "RESERVA");
                        cmd.Parameters.AddWithValue("@ItemCode", datos.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemName", datos.ItemName);
                        cmd.Parameters.AddWithValue("@CodigoUbicacion", datos.CodigoUbicacion);

                        // Agregar parámetro de salida para Id
                        SqlParameter outputId = new SqlParameter("@Id", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputId);

                        cmd.ExecuteNonQuery();
                        id = (int)outputId.Value;
                    }

                    // Ejecutar sp_GestionarStockMinProductos (Registra los stocks mínimos)
                    using (SqlCommand cmd = new SqlCommand("sp_GestionarStockMinProductos", cn, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ItemCode", datos.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemName", datos.ItemName);
                        cmd.Parameters.AddWithValue("@StockMinAbastecimiento", datos.StockMinAbastecimiento);
                        cmd.Parameters.AddWithValue("@StockMinVenta", datos.StockMinVenta);

                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    mensaje = "Los datos y stocks mínimos han sido registrados correctamente.";
                    icono = "success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogHelper.RegistrarError(ex, "UbicacionesReserva_D - RegistrarUbicacionReserva");
                    mensaje = "Ocurrió un error al registrar la ubicación reserva. Por favor, comuníquese con el área de Sistemas para más información.";
                    icono = "error";
                }
            }

            return new Helper_E { Id = id, Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }

        public Helper_E EliminarUbicacionReserva(int id)
        {
            string mensaje, icono;

            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                cn.Open();
                SqlTransaction transaction = cn.BeginTransaction();

                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AdministrarUbicaciones", cn, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Operacion", "DELETE");
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                        mensaje = "Ubicación eliminada correctamente.";
                        icono = "success";
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogHelper.RegistrarError(ex, "UbicacionesReserva_D - EliminarUbicacionReserva");
                    mensaje = "Ocurrió un error al eliminar la ubicación. Por favor, comuníquese con el área de Sistemas para más información.";
                    icono = "error";
                }
            }

            return new Helper_E { Id = id, Mensaje = mensaje, IconoSweetAlert = icono };
        }
    }
}