using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System.Drawing;
using System.Windows.Forms;
using Capa_Entidad;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class SolicitudesTraslado_D
    {
        readonly Utilitarios uti = new Utilitarios();
        public SolicitudesTraslado_E ObtenerSolicitudDeTraslado(int docNum)
        {
            SolicitudesTraslado_E solicitud = null;
            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                try
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_MantenimientoSolicitudTraslado", cn))
                    {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "GET");
                    cmd.Parameters.AddWithValue("@DocNum", docNum);
                     var outputId = new SqlParameter("@IdGenerado", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                     cmd.Parameters.Add(outputId);


                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read()) 
                            {
                                solicitud = new SolicitudesTraslado_E();
                                if (!dr.IsDBNull(0)) { solicitud.Id = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { solicitud.DocEntry = dr.GetInt32(1); }
                                if (!dr.IsDBNull(2)) { solicitud.DocNum = dr.GetInt32(2); }
                                if (!dr.IsDBNull(3)) { solicitud.DocDate = dr.GetDateTime(3).ToString("yyyy-MM-dd"); }
                                if (!dr.IsDBNull(4)) { solicitud.CardCode = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { solicitud.CardName = dr.GetString(5); }
                                if (!dr.IsDBNull(6)) { solicitud.NroGuia = dr.GetString(6); }
                                if (!dr.IsDBNull(7)) { solicitud.OperarioResponsableSAP = dr.GetString(7); }
                                if (!dr.IsDBNull(8)) { solicitud.MotivoTraslado = dr.GetString(8); }
                                if (!dr.IsDBNull(9)) { solicitud.Estado = dr.GetString(9); }
                                solicitud.Detalle = new List<DetalleSolicitudesTraslado_E>();
                            }

                            if (dr.NextResult() && solicitud != null)
                            {
                                while (dr.Read())
                                {
                                    var detalle = new DetalleSolicitudesTraslado_E();

                                    if (!dr.IsDBNull(0)) { detalle.Id = dr.GetInt32(0); }
                                    if (!dr.IsDBNull(1)) { detalle.SolicitudesTrasladoId = dr.GetInt32(1); }
                                    if (!dr.IsDBNull(2)) { detalle.ItemCode = dr.GetString(2); }
                                    if (!dr.IsDBNull(3)) { detalle.ItemName = dr.GetString(3); }
                                    if (!dr.IsDBNull(4)) { detalle.BatchNum = dr.GetString(4); }
                                    if (!dr.IsDBNull(5)) { detalle.QuantityCajas = dr.GetDecimal(5); }
                                    if (!dr.IsDBNull(6)) { detalle.FromWhsCode = dr.GetString(6); }
                                    if (!dr.IsDBNull(7)) { detalle.ToWhsCode = dr.GetString(7); }
                                    if (!dr.IsDBNull(8)) { detalle.Estado = dr.GetString(8); }
                                    if (!dr.IsDBNull(9)) { detalle.InDate = dr.GetDateTime(9).ToString("yyyy-MM-dd"); }
                                    if (!dr.IsDBNull(10)) { detalle.ExpDate = dr.GetDateTime(10).ToString("yyyy-MM-dd"); }

                                    solicitud.Detalle.Add(detalle);
                                }
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    LogHelper.RegistrarError(ex, "SolicitudesTraslado_D - ObtenerSolicitudDeTraslado");
                }
            }
            return solicitud;
        }
        public SolicitudesTraslado_E ImportarSolicitudDeTraslado(SolicitudesTraslado_E obj)
        {
            SolicitudesTraslado_E solicitud = null;

            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                cn.Open();
                using (SqlTransaction transaction = cn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_MantenimientoSolicitudTraslado", cn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Parámetros de la solicitud
                            cmd.Parameters.AddWithValue("@TipoMantenimiento", "INSERT");
                            cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry);
                            cmd.Parameters.AddWithValue("@DocNum", obj.DocNum);
                            cmd.Parameters.AddWithValue("@DocDate", obj.DocDate);
                            cmd.Parameters.AddWithValue("@NroGuia", obj.NroGuia);
                            cmd.Parameters.AddWithValue("@CardCode", obj.CardCode);
                            cmd.Parameters.AddWithValue("@CardName", obj.CardName);
                            cmd.Parameters.AddWithValue("@OperarioResponsableSAP", obj.OperarioResponsableSAP);
                            cmd.Parameters.AddWithValue("@MotivoTraslado", obj.MotivoTraslado);
                            cmd.Parameters.AddWithValue("@Estado", obj.Estado ?? "PENDIENTE");

                            // Parámetro de salida para capturar el ID generado
                            var outputId = new SqlParameter("@IdGenerado", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(outputId);

                            // Crear y llenar el DataTable solo si hay detalles
                            var table = new DataTable();
                            table.Columns.Add("SolicitudesTrasladoId", typeof(int));
                            table.Columns.Add("ItemCode", typeof(string));
                            table.Columns.Add("ItemName", typeof(string));
                            table.Columns.Add("BatchNum", typeof(string));
                            table.Columns.Add("QuantityCajas", typeof(decimal));
                            table.Columns.Add("FromWhsCode", typeof(string));
                            table.Columns.Add("ToWhsCode", typeof(string));
                            table.Columns.Add("Estado", typeof(string));

                            if (obj.Detalle != null)
                            {
                                foreach (var detalle in obj.Detalle)
                                {
                                    table.Rows.Add(
                                        0,
                                        detalle.ItemCode,
                                        detalle.ItemName,
                                        detalle.BatchNum,
                                        detalle.QuantityCajas,
                                        detalle.FromWhsCode,
                                        detalle.ToWhsCode,
                                        "PENDIENTE"
                                    );
                                }
                            }

                            // Agregar el parámetro de tipo tabla solo si hay datos
                            if (table.Rows.Count > 0)
                            {
                                var param = cmd.Parameters.AddWithValue("@Detalles", table);
                                param.SqlDbType = SqlDbType.Structured;
                                param.TypeName = "DetalleSolicitudesTrasladoType";
                            }
                            else
                            {
                                throw new Exception("La solicitud no contiene detalles de traslado.");
                            }

                            // Ejecutar el procedimiento almacenado
                            cmd.ExecuteNonQuery();

                            // Obtener el Id generado
                            obj.Id = (int)outputId.Value;

                            transaction.Commit();
                            solicitud = obj;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogHelper.RegistrarError(ex, "SolicitudTraslado_D - ImportarSolicitudDeTraslado");
                        throw new Exception($"Error al importar la solicitud de traslado: {ex.Message}");
                    }
                }
            }

            return solicitud;
        }
        public Helper_E DeleteSolicitudDeTraslado(int docNum, SqlConnection cn)
        {
            string mensaje, icono;
            try
            {
                // Verificar si la conexión está abierta
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoSolicitudTraslado ", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "DELETE");
                    cmd.Parameters.AddWithValue("@DocNum", docNum);

                    cmd.ExecuteNonQuery();
                }

                mensaje = "Solicitud de Traslado eliminada correctamente";
                icono = "success";
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "SolicitudTraslado_D - DeleteSolicitudDeTraslado");
                mensaje = "Ocurrió un error al eliminar la solicitud de traslado. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en DeleteSolicitudDeTraslado.", ex);
            }
            return new Helper_E { Mensaje = mensaje, IconoSweetAlert = icono };
        }
    }
}
