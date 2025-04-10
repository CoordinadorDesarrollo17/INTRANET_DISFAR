using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.TablasSql;

namespace Capa_Datos.DireccionTecnica_DAO.TablasSql
{
    public class ODOCS_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<ODOCS_E> ListarInternamientos(string condicion, Dictionary<string, object> parametros)
        {
            List<ODOCS_E> lista = new List<ODOCS_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.CadSql3))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT DOC.Id, DOC.TipoDocumento, DOC.DocEntry, DOC.DocNum, DOC.CardCode, DOC.CardName, DOC.Guia, DOC.ComprobanteVinculado, CONVERT(varchar, DOC.FechaContabilizacion, 103), CONVERT(varchar, DOC.FechaInicioTraslado, 103), DOC.Estado");
                    sb.AppendLine("FROM ODOCS DOC");
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
                                var obj = new ODOCS_E();

                                if (!dr.IsDBNull(0)) obj.Id = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) obj.TipoDocumento = dr.GetString(1);
                                if (!dr.IsDBNull(2)) obj.DocEntry = dr.GetInt32(2);
                                if (!dr.IsDBNull(3)) obj.DocNum = dr.GetInt32(3);
                                if (!dr.IsDBNull(4)) obj.CardCode = dr.GetString(4);
                                if (!dr.IsDBNull(5)) obj.CardName = dr.GetString(5);
                                if (!dr.IsDBNull(6)) obj.Guia = dr.GetString(6);
                                if (!dr.IsDBNull(7)) obj.ComprobanteVinculado = dr.GetString(7);
                                if (!dr.IsDBNull(8)) obj.FechaContabilizacion = dr.GetString(8);
                                if (!dr.IsDBNull(9)) obj.FechaInicioTraslado = dr.GetString(9);
                                if (!dr.IsDBNull(10)) obj.Estado = dr.GetString(10);

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error inesperado en ODOCS_D - ListarInternamientos");
            }

            return lista;
        }

        public Helper_E RegistrarDocumento(ODOCS_E datos)
        {
            Helper_E result = new Helper_E();
            int id = 0;

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

                            cmd.Parameters.AddWithValue("@Operacion", "REGISTRAR");
                            cmd.Parameters.AddWithValue("@TipoDocumento", datos.TipoDocumento);
                            cmd.Parameters.AddWithValue("@DocEntry", datos.DocEntry);
                            cmd.Parameters.AddWithValue("@DocNum", datos.DocNum);
                            cmd.Parameters.AddWithValue("@CardCode", datos.CardCode);
                            cmd.Parameters.AddWithValue("@CardName", datos.CardName);
                            cmd.Parameters.AddWithValue("@Guia", datos.Guia);
                            cmd.Parameters.AddWithValue("@ComprobanteVinculado", datos.ComprobanteVinculado);
                            cmd.Parameters.AddWithValue("@FechaContabilizacion", (object)datos.FechaContabilizacion ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@FechaInicioTraslado", (object)datos.FechaInicioTraslado ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Estado", "Pendiente");

                            // Para [CC_ODOCS]
                            cmd.Parameters.AddWithValue("@UsuarioRegistro", datos.UsuarioRegistro);
                            cmd.Parameters.AddWithValue("@TipoOperacion", "REGISTRAR");

                            // Agregar parámetro de salida para Id
                            SqlParameter outputId = new SqlParameter("@Id", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(outputId);

                            // Tabla de detalles
                            DataTable dtDetalle = ConvertirADatatable(datos.Detalle);
                            SqlParameter detallesParam = cmd.Parameters.AddWithValue("@Detalle", dtDetalle);
                            detallesParam.SqlDbType = SqlDbType.Structured;
                            detallesParam.TypeName = "dbo.DOCS1_Type";

                            cmd.ExecuteNonQuery();

                            if (outputId.Value != DBNull.Value)
                                id = (int)outputId.Value;
                        }
                        transaction.Commit();

                        result.Titulo = "Acción completada";
                        result.Mensajes.Add("Documento registrado correctamente.");
                        result.Icono = "success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogHelper.RegistrarError(ex, "Error inesperado en ODOCS_D - RegistrarDocumento");

                        result.Titulo = "Error";
                        result.Mensajes.Add("Ocurrió un error al registrar el documento.");
                        result.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                        result.Icono = "error";
                    }
                }
            }

            return result;
        }

        private DataTable ConvertirADatatable(List<DOCS1_E> detalles)
        {
            DataTable table = new DataTable();
            table.Columns.Add("ODCOSId", typeof(int));
            table.Columns.Add("ItemCode", typeof(string));
            table.Columns.Add("ItemName", typeof(string));
            table.Columns.Add("Lote", typeof(string));
            table.Columns.Add("FechaVencimiento", typeof(DateTime));
            table.Columns.Add("RegistroSanitario", typeof(string));
            table.Columns.Add("Fabricante", typeof(string));
            table.Columns.Add("CondicionAlmTrans", typeof(string));
            table.Columns.Add("Almacen", typeof(string));
            table.Columns.Add("CertificadoAnalisis", typeof(string));
            table.Columns.Add("ComentarioOrganoleptico", typeof(string));
            table.Columns.Add("CantidadAprobados", typeof(int));
            table.Columns.Add("CantidadBaja", typeof(int));
            table.Columns.Add("CantidadDevolucion", typeof(int));
            table.Columns.Add("CantidadTotal", typeof(int));
            table.Columns.Add("Liberado", typeof(int));
            table.Columns.Add("Transferido", typeof(int));

            foreach (var detalle in detalles)
            {
                table.Rows.Add(0, detalle.ItemCode, detalle.ItemName, detalle.Lote, detalle.FechaVencimiento, detalle.RegistroSanitario, detalle.Fabricante, detalle.CondicionAlmTrans,
                    detalle.Almacen, detalle.CertificadoAnalisis, detalle.ComentarioOrganoleptico, detalle.CantidadAprobados, detalle.CantidadBaja, detalle.CantidadDevolucion, detalle.CantidadTotal,
                    detalle.Liberado, detalle.Transferido);
            }
            return table;
        }
    }
}