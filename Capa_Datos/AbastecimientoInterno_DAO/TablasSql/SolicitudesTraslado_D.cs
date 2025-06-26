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
        public SolicitudesTraslado_E ObtenerSolicitudDeTraslado(int docNum, SqlConnection cn)
        {
            if (cn == null)
            {
                cn = new SqlConnection(uti.cadSql2);
            }

            SolicitudesTraslado_E solicitud = null;
            try
            {

                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }
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
                            solicitud.Detalle = new Dictionary<string, DetalleSolicitudesTraslado_E>();
                        }

                        if (dr.NextResult() && solicitud != null)
                        {
                            while (dr.Read())
                            {
                                var detalle = new DetalleSolicitudesTraslado_E();

                                string itemCode = dr.IsDBNull(2) ? "" : dr.GetString(2);
                                string batchNum = dr.IsDBNull(4) ? "" : dr.GetString(4); // Lote
                                string uniqueKey = $"{itemCode}_{batchNum}"; // Clave única combinada

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
                                //Averigua si el SKU ya ha sido atendido o validado en alguno de sus lotes o ubicaciones (Datos que vienen desde Transferencia)
                                if (!dr.IsDBNull(11)) { detalle.AtendidoReserva = dr.GetInt32(11); }
                                if (!dr.IsDBNull(12)) { detalle.Validado = dr.GetInt32(12); }
                                // Si la clave única no existe en el diccionario, agregar el detalle
                                if (!solicitud.Detalle.ContainsKey(uniqueKey))
                                {
                                    solicitud.Detalle[uniqueKey] = detalle;
                                }

                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "SolicitudesTraslado_D - ObtenerSolicitudDeTraslado");
            }
            return solicitud;
        }
        public Helper_E ImportarSolicitudDeTraslado(SolicitudesTraslado_E obj, SqlConnection cn)
        {
            string mensaje, icono;
            SolicitudesTraslado_E solicitud = null;
            int baseInicial = 0;

            try
            {
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                // Solo para tipo de documento EMI
                
                string queryMax = @"
                        SELECT ISNULL(MAX(DocNum), 0) 
                        FROM SolicitudesTraslado 
                        WHERE DocNum >= @BaseInicial AND DocNum < 250000000
                    ";

                using (SqlCommand cmdMax = new SqlCommand(queryMax, cn))
                {
                    cmdMax.Parameters.AddWithValue("@BaseInicial", 200000000);

                    object result = cmdMax.ExecuteScalar();
                    int maxActual = Convert.ToInt32(result);

                    if (maxActual == 0)
                        baseInicial = 200000000;
                    else
                        baseInicial = maxActual + 1;
                }


                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoSolicitudTraslado", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros de la solicitud
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "INSERT");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", obj.TipoDocumento == "EMI" ? baseInicial : obj.DocNum);
                    cmd.Parameters.AddWithValue("@DocDate", obj.TipoDocumento == "EMI" ? DateTime.Now.ToString("yyyy-MM-dd") : obj.DocDate);
                    cmd.Parameters.AddWithValue("@NroGuia", obj.NroGuia);
                    cmd.Parameters.AddWithValue("@CardCode", obj.CardCode);
                    cmd.Parameters.AddWithValue("@CardName", obj.CardName);
                    cmd.Parameters.AddWithValue("@OperarioResponsableSAP", obj.TipoDocumento == "EMI"? obj.OperarioRegistra : obj.OperarioResponsableSAP);
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
                                detalle.Value.ItemCode,
                                detalle.Value.ItemName,
                                detalle.Value.BatchNum,
                                detalle.Value.QuantityCajas,
                                detalle.Value.FromWhsCode,
                                detalle.Value.ToWhsCode,
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

                    solicitud = obj;
                    mensaje = "Solicitud de traslado importada correctamente";
                    icono = "success";
                }
            }
            catch (SqlException sqlEx)
            {

                if (sqlEx.Message.Contains("Documento ya existente"))
                {
                    mensaje = "Documento ya existente, verifique en su BD.";
                    icono = "error";
                }
                else
                {
                    mensaje = $"Error SQL al importar la solicitud.";
                    icono = "error";
                }

                LogHelper.RegistrarError(sqlEx, "SolicitudTraslado_D - ImportarSolicitudDeTraslado");
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "SolicitudTraslado_D - ImportarSolicitudDeTraslado");
                mensaje = $"Ocurrió un error al importar la solicitud de traslado. {ex.Message}";
                icono = "error";
                throw new Exception($"Error al importar la solicitud de traslado: {ex.Message}");
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, Icono = icono, Id = obj.Id, DocNum = baseInicial };
        }
        public Helper_E DeleteSolicitudDeTraslado(int docNum, SqlConnection cn)
        {
            string mensaje, icono;
            try
            {
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoSolicitudTraslado ", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "DELETE");
                    cmd.Parameters.AddWithValue("@DocNum", docNum);
                    SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idGeneradoParam);
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
            return new Helper_E { Mensajes = new List<string> { mensaje }, Icono = icono };
        }
        public Helper_E ActualizarEstado(int solicitudTrasladoId, List<DetalleTransferenciaReserva_E> detalleTransferencia, SqlConnection cn)
        {
            string mensaje, icono;
            try
            {
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                // Obtener los valores distintos de ItemCode
                var itemCodes = detalleTransferencia.Select(d => d.ItemCode).Distinct().ToList();

                foreach (var itemCode in itemCodes)
                {
                    using (SqlCommand cmd = new SqlCommand("sp_MantenimientoSolicitudTraslado", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "UPDATE");
                        cmd.Parameters.AddWithValue("@ItemCode", itemCode);
                        cmd.Parameters.AddWithValue("@Id", solicitudTrasladoId); // usa el parametro de la cabecera
                        SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(idGeneradoParam);

                        cmd.ExecuteNonQuery();
                    }
                }

                mensaje = "Estado de solicitud de traslado actualizado correctamente";
                icono = "success";
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "SolicitudTraslado_D - ActualizarEstado");
                mensaje = "Ocurrió un error al actualizar el estado de la solicitud de traslado. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en ActualizarEstado.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, Icono = icono };
        }
      
    }
}
