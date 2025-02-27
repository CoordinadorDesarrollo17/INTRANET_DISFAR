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
    public class UbicacionesPicking_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<UbicacionesPicking_E> ListarUbicacionesPicking(string condicion, Dictionary<string, object> parametrosCondicion)
        {
            List<UbicacionesPicking_E> lista = new List<UbicacionesPicking_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT UP.[Id], UP.[Almacen], UP.[ItemCode], UP.[ItemName], UP.[CodigoUbicacion], SM.[StockMinAbastecimiento], SM.[StockMinVenta]");
                    sb.AppendLine("FROM [dbo].[Ubicaciones] UP");
                    sb.AppendLine("OUTER APPLY (SELECT TOP 1 SM.[StockMinAbastecimiento], SM.[StockMinVenta] FROM [dbo].[StockMinProductos] SM WHERE SM.[ItemCode] = UP.[ItemCode]) SM");
                    sb.AppendLine("WHERE UP.[Almacen] = 'PICKING'");
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
                                var obj = new UbicacionesPicking_E();

                                if (!dr.IsDBNull(0)) obj.Id = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) obj.Almacen = dr.GetString(1);
                                if (!dr.IsDBNull(2)) obj.ItemCode = dr.GetString(2);
                                if (!dr.IsDBNull(3)) obj.ItemName = dr.GetString(3);
                                if (!dr.IsDBNull(4)) obj.CodigoUbicacion = dr.GetString(4);
                                if (!dr.IsDBNull(5)) obj.StockMinAbastecimiento = dr.GetInt32(5);
                                if (!dr.IsDBNull(6)) obj.StockMinVenta = dr.GetInt32(6);

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesPicking_D - ListarUbicacionesPicking");
            }

            return lista;
        }

        public Helper_E RegistrarUbicacionPicking(UbicacionesPicking_E datos)
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
                        cmd.Parameters.AddWithValue("@Almacen", "PICKING");
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
                   
                    transaction.Commit();
                    mensaje = "Los datos de nueva ubicación han sido registrados correctamente.";
                    icono = "success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogHelper.RegistrarError(ex, "UbicacionesPicking_D - RegistrarUbicacionPicking");
                    mensaje = "Ocurrió un error al registrar la ubicación picking. Por favor, comuníquese con el área de Sistemas para más información.";
                    icono = "error";
                }
            }

            return new Helper_E { Id = id, Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }

        public Helper_E EliminarUbicacionPicking(int id)
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
                    LogHelper.RegistrarError(ex, "UbicacionesPicking_D - EliminarUbicacionPicking");
                    mensaje = "Ocurrió un error al eliminar la ubicación. Por favor, comuníquese con el área de Sistemas para más información.";
                    icono = "error";
                }
            }

            return new Helper_E { Id = id,  Mensajes = new List<string> { mensaje } , IconoSweetAlert = icono };
        }
    }
}