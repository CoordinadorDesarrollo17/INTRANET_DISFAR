using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.AbastecimientoInterno_ENT.Reportes;

namespace Capa_Datos.AbastecimientoInterno_DAO.Reportes
{
    public class ReporteKardex_D
    {
        Utilitarios uti = new Utilitarios();

        public List<ReporteKardexIngreso_E> ListarKardexIngreso(string fechaInicio, string fechaFin)
        {
            List<ReporteKardexIngreso_E> resultado = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ReporteKardex", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();

                        cmd.Parameters.AddWithValue("@TipoReporte", "INGRESO");
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            resultado = new List<ReporteKardexIngreso_E>();

                            while (dr.Read())
                            {
                                var obj = new ReporteKardexIngreso_E();

                                if (!dr.IsDBNull(0)) obj.DocNumTransferencia = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) obj.Proceso = dr.GetString(1);
                                if (!dr.IsDBNull(2)) obj.FechaIngreso = dr.GetString(2);
                                if (!dr.IsDBNull(3)) obj.HoradeNumTransferencia = dr.GetString(3);
                                if (!dr.IsDBNull(4)) obj.HoraIngreso = dr.GetString(4);
                                if (!dr.IsDBNull(5)) obj.OperarioIngreso = dr.GetString(5);
                                if (!dr.IsDBNull(6)) obj.SKU = dr.GetString(6);
                                if (!dr.IsDBNull(7)) obj.DescripcionArticulo = dr.GetString(7);
                                if (!dr.IsDBNull(8)) obj.Lote = dr.GetString(8);
                                if (!dr.IsDBNull(9)) obj.CantidadMaster = dr.GetInt32(9);
                                if (!dr.IsDBNull(10)) obj.CantidadSaldo = dr.GetInt32(10);
                                if (!dr.IsDBNull(11)) obj.CantidadUnidadesCajas = dr.GetInt32(11);
                                if (!dr.IsDBNull(12)) obj.UbicacionRegistro = dr.GetString(12);
                                if (!dr.IsDBNull(13)) obj.UbicacionActual = dr.GetString(13);
                                
                                
                                resultado.Add(obj);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error inesperado en ReporteKardex_D - ListarKardexIngreso()");
            }

            return resultado;
        }

        public List<ReporteKardexSalida_E> ListarKardexSalida(string fechaInicio, string fechaFin)
        {
            List<ReporteKardexSalida_E> resultado = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_ReporteKardex", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();

                        cmd.Parameters.AddWithValue("@TipoReporte", "SALIDA");
                        cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                        cmd.Parameters.AddWithValue("@FechaFin", fechaFin);

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            resultado = new List<ReporteKardexSalida_E>();

                            while (dr.Read())
                            {
                                var obj = new ReporteKardexSalida_E();

                                if (!dr.IsDBNull(0)) obj.NroRequerimiento = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) obj.Proceso = dr.GetString(1);
                                if (!dr.IsDBNull(2)) obj.FechaSalida = dr.GetString(2);
                                if (!dr.IsDBNull(3)) obj.HoraSalida = dr.GetString(3);
                                // CREAR 
                                if (!dr.IsDBNull(4)) obj.OperarioSolicitud = dr.GetString(4);
                                if (!dr.IsDBNull(5)) obj.HoraSolicitud = dr.GetString(5);
                                //  ATD_RESERVA 
                                if (!dr.IsDBNull(6)) obj.OperarioReabastecimiento = dr.GetString(6);
                                if (!dr.IsDBNull(7)) obj.HoraReabastecimiento = dr.GetString(7);
                                // ATD_PICKING )
                                if (!dr.IsDBNull(8)) obj.OperarioPicking = dr.GetString(8);
                                if (!dr.IsDBNull(9)) obj.HoraPicking = dr.GetString(9);
                                if (!dr.IsDBNull(10)) obj.SKU = dr.GetString(10);
                                if (!dr.IsDBNull(11)) obj.DescripcionArticulo = dr.GetString(11);
                                if (!dr.IsDBNull(12)) obj.Lote = dr.GetString(12);
                                if (!dr.IsDBNull(13)) obj.CantidadMaster = dr.GetInt32(13);
                                if (!dr.IsDBNull(14)) obj.CantidadSaldo = dr.GetInt32(14);
                                if (!dr.IsDBNull(15)) obj.CantidadUnidadesCajas = dr.GetInt32(15);
                                if (!dr.IsDBNull(16)) obj.UbicacionOrigen = dr.GetString(16);
                                if (!dr.IsDBNull(17)) obj.UbicacionDestino = dr.GetString(17);
                                if (!dr.IsDBNull(18)) obj.UbicacionActual = dr.GetString(18);

                                resultado.Add(obj);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error inesperado en ReporteKardex_D - ListarKardexSalida()");
            }

            return resultado;
        }
    }
}
