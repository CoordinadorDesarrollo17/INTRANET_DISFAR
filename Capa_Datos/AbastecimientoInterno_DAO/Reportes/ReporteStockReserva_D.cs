using Capa_Entidad.AbastecimientoInterno_ENT.Reportes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.AbastecimientoInterno_DAO.Reportes
{
    public class ReporteStockReserva_D
    {
        Utilitarios uti = new Utilitarios();
        DBHelper dbHelper = new DBHelper();
        public List<ReporteStockReserva_E> Listar()
        {
            List<ReporteStockReserva_E> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ListarStockReserva", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            lista = new List<ReporteStockReserva_E>();
                            while (dr.Read())
                            {
                                ReporteStockReserva_E reporte = new ReporteStockReserva_E();

                                if (!dr.IsDBNull(0)) reporte.UbicacionId = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) reporte.UbicacionLoteId = dr.GetInt32(1);
                                if (!dr.IsDBNull(2)) reporte.UbicacionLoteMasterId = dr.GetInt32(2);
                                if (!dr.IsDBNull(3)) reporte.ItemCode = dr.GetString(3);
                                if (!dr.IsDBNull(4)) reporte.ItemName = dr.GetString(4);
                                if (!dr.IsDBNull(5)) reporte.CodigoUbicacion = dr.GetString(5);
                                if (!dr.IsDBNull(6)) reporte.BatchNum = dr.GetString(6);
                                if (!dr.IsDBNull(7)) reporte.UmAlm = dr.GetString(7);
                                if (!dr.IsDBNull(8)) reporte.ValorUmAlm = dr.GetInt32(8);
                                if (!dr.IsDBNull(9)) reporte.QuantityMaster = dr.GetInt32(9);
                                if (!dr.IsDBNull(10)) reporte.QuantitySaldo = dr.GetInt32(10);
                                if (!dr.IsDBNull(11)) reporte.QuantityUnidadesCajas= dr.GetInt32(11);

                                lista.Add(reporte);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "ReporteStockReserva_D - Listar");
                throw new Exception("Error en ReporteStockReserva_D.", ex);
            }

            return lista;
        }
    }
}
