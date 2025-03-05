using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.Almacen_ENT.Tablas;
using Sap.Data.Hana;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class StockMinProductos_D
    {
        readonly Utilitarios uti = new Utilitarios();
        public StockMinProductos_E Obtener(string itemCode)
        {
             StockMinProductos_E obj = new StockMinProductos_E();

            string query = $@"SELECT Id, ItemCode, ItemName, StockMinAbastecimiento, StockMinVenta FROM StockMinProductos where ItemCode= {itemCode}";
            try
            {
                using (SqlConnection cn =  new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = query;

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
                LogHelper.RegistrarError(ex, "Error en Obtener");
            }
            return obj;
        }
        public Helper_E ActualizarStocksMinimos(StockMinProductos_E datos)
        {
            string mensaje, icono;

            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                cn.Open();
                SqlTransaction transaction = cn.BeginTransaction();

                try
                {
                    if (datos.StockMinAbastecimiento > 0 && datos.StockMinVenta > 0)
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_GestionarStockMinProductos", cn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@ItemCode", datos.ItemCode);
                            cmd.Parameters.AddWithValue("@ItemName", datos.ItemName);
                            cmd.Parameters.AddWithValue("@StockMinAbastecimiento", datos.StockMinAbastecimiento);
                            cmd.Parameters.AddWithValue("@StockMinVenta", datos.StockMinVenta);
                            cmd.Parameters.AddWithValue("@Clasificacion", datos.Clasificacion);
                            cmd.ExecuteNonQuery();

                            transaction.Commit();
                            mensaje = "Stocks mínimos  y/o clasificación establecidos correctamente.";
                            icono = "success";
                        }
                    }
                    else
                    {
                        mensaje = "Los datos no son válidos.";
                        icono = "error";
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogHelper.RegistrarError(ex, "StockMinProductos_D - ActualizarStocksMinimos");
                    mensaje = "Ocurrió un error al establecer stocks mínimos. Por favor, comuníquese con el área de Sistemas para más información.";
                    icono = "error";
                }
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }
    }
}
