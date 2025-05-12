using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;
using Capa_Entidad.DireccionTecnica_ENT.TablasSql;
using Capa_Entidad.TablasSql;

namespace Capa_Datos.DireccionTecnica_DAO.TablasSql
{
    public class SolicitudesReversion_DOCS1_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<SolicitudesReversion_DOCS1_E> ListarSolicitudesReversion(string condicion, Dictionary<string, object> parametros)
        {
            List<SolicitudesReversion_DOCS1_E> lista = new List<SolicitudesReversion_DOCS1_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.CadSql3))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT SOL.Id, SOL.DOCS1Id, SOL.SolicitadoPor, convert(varchar, SOL.FechaSolicitud, 103), convert(varchar, SOL.HoraSolicitud, 8), SOL.Estado, SOL.ConfirmadoPor,");
                    sb.AppendLine("CONVERT(varchar, SOL.Fecha, 103), CONVERT(varchar, SOL.Hora, 8), DET.ODOCSId AS 'IdDocumento'");
                    sb.AppendLine("FROM SolicitudesReversion_DOCS1 SOL");
                    sb.AppendLine("OUTER APPLY (");
                    sb.AppendLine("SELECT TOP 1 ODOCSId FROM DOCS1");
                    sb.AppendLine("WHERE Id = SOL.DOCS1Id");
                    sb.AppendLine(") DET");
                    sb.AppendLine("WHERE 1=1");
                    sb.AppendLine(condicion?.ToString().Trim());

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
                                // Agregar detalle
                                var obj = new SolicitudesReversion_DOCS1_E();
                                obj.Id = dr.IsDBNull(0) ? 0 : dr.GetInt32(0);
                                obj.DOCS1Id = dr.IsDBNull(1) ? 0 : dr.GetInt32(1);
                                obj.SolicitadoPor = dr.IsDBNull(2) ? string.Empty : dr.GetString(2);
                                obj.FechaSolicitud = dr.IsDBNull(3) ? null : dr.GetString(3);
                                obj.HoraSolicitud = dr.IsDBNull(4) ? null : dr.GetString(4);
                                obj.Estado = dr.IsDBNull(5) ? null : dr.GetString(5);
                                obj.ConfirmadoPor = dr.IsDBNull(6) ? null : dr.GetString(6);
                                obj.Fecha = dr.IsDBNull(7) ? null : dr.GetString(7);
                                obj.Hora = dr.IsDBNull(8) ? null : dr.GetString(8);
                                obj.ODOCSId = dr.IsDBNull(9) ? 0 : dr.GetInt32(9);

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error inesperado en ODOCS_D - ListarSolicitudesReversion()");
            }

            return lista;
        }

        public Helper_E SolicitarReversionLiberacionArticulo(int id, string usuarioRegistro)
        {
            Helper_E result = new Helper_E();

            using (SqlConnection cn = new SqlConnection(uti.CadSql3))
            {
                cn.Open();
                using (SqlTransaction transaction = cn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_GestionarDocumentos", cn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Operacion", "SOLICITUD_REVERTIR_LIBERACION");
                            cmd.Parameters.AddWithValue("@Id", id);

                            // Para [CC_ODOCS]
                            cmd.Parameters.AddWithValue("@UsuarioRegistro", usuarioRegistro);
                            cmd.Parameters.AddWithValue("@TipoOperacion", "SOLICITUD_REVERTIR_LIBERACION");

                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();

                        result.Titulo = "Acción completada";
                        result.Mensajes.Add("Se envió la solicitud para revertir la liberación de este artículo correctamente.");
                        result.Icono = "success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogHelper.RegistrarError(ex, "Error inesperado en SolicitudesReversion_DOCS1_D - SolicitarReversionLiberacionArticulo()");

                        result.Titulo = "Error";
                        result.Mensajes.Add("Ocurrió un error al enviar la solicitud para revertir la liberación del artículo.");
                        result.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                        result.Icono = "error";
                    }
                }
            }

            return result;
        }
    }
}
