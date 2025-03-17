using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.Almacen_ENT.Tablas;
using Sap.Data.Hana;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class StockMinProductos_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<StockMinProductos_E> ListarStockMinProductos(string condicion, Dictionary<string, object> parametros)
        {
            List<StockMinProductos_E> lista = new List<StockMinProductos_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT SM.Id, SM.ItemCode, SM.ItemName, SM.StockMinAbastecimiento, SM.Clasificacion");
                    sb.AppendLine("FROM StockMinProductos SM");
                    sb.AppendLine("WHERE 1=1");
                    sb.AppendLine(condicion.ToString());

                    // Agregamos los parámetros dinámicamente
                    foreach (var prm in parametros)
                    {
                        cmd.Parameters.AddWithValue(prm.Key, prm.Value);
                    }

                    cmd.CommandText = sb.ToString();

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var obj = new StockMinProductos_E();

                                if (!dr.IsDBNull(0)) obj.Id = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) obj.ItemCode = dr.GetString(1);
                                if (!dr.IsDBNull(2)) obj.ItemName = dr.GetString(2);
                                if (!dr.IsDBNull(3)) obj.StockMinAbastecimiento = dr.GetInt32(3);
                                if (!dr.IsDBNull(4)) obj.Clasificacion = dr.GetString(4);

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "StockMinProductos_D - ListarStockMinProductos");
            }

            return lista;
        }

        public StockMinProductos_E Obtener(string itemCode)
        {
            StockMinProductos_E obj = new StockMinProductos_E();

            string query = $@"SELECT Id, ItemCode, ItemName, StockMinAbastecimiento FROM StockMinProductos where ItemCode= @ItemCode";
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@ItemCode", itemCode);

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                if (!dr.IsDBNull(0)) obj.Id = dr.GetInt32(0);
                                if (!dr.IsDBNull(4)) obj.StockMinAbastecimiento = dr.GetInt32(4);
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "StockMinProductos_D - Obtener");
            }
            return obj;
        }
        public Helper_E RegistrarStockMinimo(StockMinProductos_E datos)
        {
            string mensaje, icono;

            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                cn.Open();
                SqlTransaction transaction = cn.BeginTransaction();

                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GestionarStockMinProductos", cn, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Accion", "INS");
                        cmd.Parameters.AddWithValue("@ItemCode", datos.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemName", datos.ItemName);
                        cmd.Parameters.AddWithValue("@StockMinAbastecimiento", datos.StockMinAbastecimiento);
                        cmd.Parameters.AddWithValue("@Clasificacion", datos.Clasificacion);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                        mensaje = "Stocks mínimos  y/o clasificación establecidos correctamente.";
                        icono = "success";
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogHelper.RegistrarError(ex, "StockMinProductos_D - RegistrarStockMinimo");
                    mensaje = "Ocurrió un error al establecer stocks mínimos. Por favor, comuníquese con el área de Sistemas para más información.";
                    icono = "error";
                }
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }

        public Helper_E ActualizarStockMinimo(StockMinProductos_E datos)
        {
            string mensaje, icono;

            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                cn.Open();
                SqlTransaction transaction = cn.BeginTransaction();

                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GestionarStockMinProductos", cn, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Accion", "UPD");
                        cmd.Parameters.AddWithValue("@ItemCode", (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ItemName", (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@StockMinAbastecimiento", datos.StockMinAbastecimiento);
                        cmd.Parameters.AddWithValue("@Clasificacion", datos.Clasificacion);
                        cmd.Parameters.AddWithValue("@Id", datos.Id);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                        mensaje = "Stocks mínimos  y/o clasificación establecidos correctamente.";
                        icono = "success";
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogHelper.RegistrarError(ex, "StockMinProductos_D - ActualizarStockMinimo");
                    mensaje = "Ocurrió un error al establecer stocks mínimos. Por favor, comuníquese con el área de Sistemas para más información.";
                    icono = "error";
                }
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }
    }
}
