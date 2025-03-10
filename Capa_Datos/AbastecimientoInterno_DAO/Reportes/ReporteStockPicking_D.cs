using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System;
using Capa_Entidad.AbastecimientoInterno_ENT.Reportes;
using DocumentFormat.OpenXml.Office2013.Excel;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using System.Windows.Forms;

namespace Capa_Datos.AbastecimientoInterno_DAO.Reportes
{
    public class ReporteStockPicking_D
    {
        Utilitarios uti = new Utilitarios();
        DBHelper dbHelper = new DBHelper();
        public List<ReporteStockPicking_E> ControlStockInternoPicking()
        {
              List<ReporteStockPicking_E> lista  =null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_ControlHistoricoDeIngresosAPicking", cn))
                    {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@TipoMantenimiento", "RPT");
                        using (SqlDataReader dr = cmd.ExecuteReader()) 
                        {
                            lista = new List<ReporteStockPicking_E>();
                            while (dr.Read())
                            {
                                ReporteStockPicking_E reporte = new ReporteStockPicking_E();

                                if (!dr.IsDBNull(0)) reporte.ItemCode = dr.GetString(0);
                                if (!dr.IsDBNull(1)) reporte.ItemName = dr.GetString(1);
                                if (!dr.IsDBNull(2)) reporte.Clasificacion = dr.GetString(2);
                                if (!dr.IsDBNull(3)) reporte.StockActual = dr.GetInt32(3);
                                if (!dr.IsDBNull(4)) reporte.Almacen = dr.GetString(4);
                                if (!dr.IsDBNull(5)) reporte.StockMinVenta = dr.GetInt32(5);
                                if (!dr.IsDBNull(6)) reporte.StockMinAbastecimiento = dr.GetInt32(6);

                                lista.Add(reporte);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Reportes - ControlStockInternoPicking");
                throw new Exception("Error en ReporteStockPicking_D.", ex);
            }

            return lista;
        }
    }
}
