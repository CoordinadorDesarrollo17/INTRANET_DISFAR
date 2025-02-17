using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class StockMinProductos_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public Helper_E ActualizarStocksMinimos(StockMinProductos_E datos)
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

                        cmd.Parameters.AddWithValue("@ItemCode", datos.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemName", datos.ItemName);
                        cmd.Parameters.AddWithValue("@StockMinAbastecimiento", datos.StockMinAbastecimiento);
                        cmd.Parameters.AddWithValue("@StockMinVenta", datos.StockMinVenta);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                        mensaje = "Stocks mínimos establecidos correctamente.";
                        icono = "success";
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

            return new Helper_E { Mensaje = mensaje, IconoSweetAlert = icono };
        }
    }
}
