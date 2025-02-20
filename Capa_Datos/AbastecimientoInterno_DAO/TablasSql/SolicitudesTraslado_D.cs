using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class SolicitudesTraslado_D
    {
        readonly Utilitarios uti = new Utilitarios();
        readonly DBHelper db = new DBHelper();
        public SolicitudesTraslado_E ObtenerSolicitudDeTraslado(int DocNum)
        {
            SolicitudesTraslado_E solicitud = null;
            SqlConnection cn = new SqlConnection(uti.cadSql2);
            try
            {
                cn.Open();
                string query = $"SELECT Id FROM SolicitudesTraslado where DocNum ={DocNum}";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    solicitud = new SolicitudesTraslado_E();
                    solicitud.Id = dr.GetInt32(0);
                }

                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
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
    }
}
