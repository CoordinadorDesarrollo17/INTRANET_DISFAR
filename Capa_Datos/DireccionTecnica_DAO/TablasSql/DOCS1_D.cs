using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.TablasSql;
using Capa_Entidad;

namespace Capa_Datos.DireccionTecnica_DAO.TablasSql
{
    public class DOCS1_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public Helper_E TransferirArticulo(string area, int id, string usuarioRegistro)
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

                            cmd.Parameters.AddWithValue("@Operacion", "TRANSFERIR");
                            cmd.Parameters.AddWithValue("@Id", id);

                            // Para [AT_DOCS1]
                            cmd.Parameters.AddWithValue("@Area", System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(area.ToLower()));

                            // Para [CC_ODOCS]
                            cmd.Parameters.AddWithValue("@UsuarioRegistro", usuarioRegistro);
                            cmd.Parameters.AddWithValue("@TipoOperacion", "TRANSFERIR");

                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();

                        result.Titulo = "Acción completada";
                        result.Mensajes.Add("Artículo transferido correctamente.");
                        result.Icono = "success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogHelper.RegistrarError(ex, "Error inesperado en DOCS1_D - TransferirArticulo()");

                        result.Titulo = "Error";
                        result.Mensajes.Add("Ocurrió un error al transferir artículo.");
                        result.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                        result.Icono = "error";
                    }
                }
            }

            return result;
        }

        public Helper_E RevertirTransferenciaArticulo(int id, string area, string usuarioRegistro)
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

                            cmd.Parameters.AddWithValue("@Operacion", "REVERTIR_TRANSFERENCIA");
                            cmd.Parameters.AddWithValue("@Id", id);

                            // Para [AT_DOCS1]
                            cmd.Parameters.AddWithValue("@Area", System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(area.ToLower()));

                            // Para [CC_ODOCS]
                            cmd.Parameters.AddWithValue("@UsuarioRegistro", usuarioRegistro);
                            cmd.Parameters.AddWithValue("@TipoOperacion", "REVERTIR_TRANSFERENCIA");

                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();

                        result.Titulo = "Acción completada";
                        result.Mensajes.Add("Se revirtió la transferencia del artículo correctamente.");
                        result.Icono = "success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogHelper.RegistrarError(ex, "Error inesperado en DOCS1_D - RevertirTransferenciaArticulo()");

                        result.Titulo = "Error";
                        result.Mensajes.Add("Ocurrió un error al revertir trasnsferencia del artículo.");
                        result.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                        result.Icono = "error";
                    }
                }
            }

            return result;
        }

        public Helper_E LiberarArticulo(int id, string usuarioRegistro)
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

                            cmd.Parameters.AddWithValue("@Operacion", "LIBERAR");
                            cmd.Parameters.AddWithValue("@Id", id);

                            // Para [CC_ODOCS]
                            cmd.Parameters.AddWithValue("@UsuarioRegistro", usuarioRegistro);
                            cmd.Parameters.AddWithValue("@TipoOperacion", "LIBERAR");

                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();

                        result.Titulo = "Acción completada";
                        result.Mensajes.Add("Artículo liberado correctamente.");
                        result.Icono = "success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogHelper.RegistrarError(ex, "Error inesperado en DOCS1_D - LiberarItemDetalleDoc()");

                        result.Titulo = "Error";
                        result.Mensajes.Add("Ocurrió un error al liberar artículo.");
                        result.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                        result.Icono = "error";
                    }
                }
            }

            return result;
        }        

        public Helper_E RevertirLiberacionArticulo(int id, string estado, string usuarioRegistro)
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

                            cmd.Parameters.AddWithValue("@Operacion", "REVERTIR_LIBERACION");
                            cmd.Parameters.AddWithValue("@Id", id);

                            // Para: [SolicitudesReversion_DOCS1]
                            cmd.Parameters.AddWithValue("@Estado", estado);

                            // Para: [CC_ODOCS]
                            cmd.Parameters.AddWithValue("@UsuarioRegistro", usuarioRegistro);
                            cmd.Parameters.AddWithValue("@TipoOperacion", "REVERTIR_LIBERACION");

                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();

                        result.Titulo = "Acción completada";
                        result.Mensajes.Add("Se revirtió la liberación de este artículo correctamente.");
                        result.Icono = "success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogHelper.RegistrarError(ex, "Error inesperado en DOCS1_D - RevertirLiberacionArticulo()");

                        result.Titulo = "Error";
                        result.Mensajes.Add("Ocurrió un error al revertir la liberación del artículo.");
                        result.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                        result.Icono = "error";
                    }
                }
            }

            return result;
        }

        public List<DOCS1_E> ListarDetalleDocumento(string condicion, Dictionary<string, object> parametros, bool traerTodos)
        {
            List<DOCS1_E> lista = new List<DOCS1_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.CadSql3))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT Id, ODOCSId, ItemCode, ItemName, Lote, CONVERT(varchar, FechaVencimiento, 103), RegistroSanitario, Fabricante, CondicionAlmTrans, Almacen, CertificadoAnalisis, ComentarioOrganoleptico,");
                    sb.AppendLine("CantidadAprobados, CantidadBaja, CantidadDevolucion, CantidadTotal, Liberado, Transferido");
                    sb.AppendLine("FROM DOCS1");

                    if (traerTodos)
                    {
                        sb.AppendLine("WHERE ODOCId = (SELECT ODOCId FROM DOCS1 WHERE Id = @Id)");
                    }
                    else
                    {
                        sb.AppendLine("WHERE Id = @Id");
                    }

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
                                var detalle = new DOCS1_E();
                                detalle.Id = dr.IsDBNull(0) ? 0 : dr.GetInt32(0);
                                detalle.ODOCSId = dr.IsDBNull(1) ? 0 : dr.GetInt32(1);
                                detalle.ItemCode = dr.IsDBNull(2) ? null : dr.GetString(2);
                                detalle.ItemName = dr.IsDBNull(3) ? null : dr.GetString(3);
                                detalle.Lote = dr.IsDBNull(4) ? null : dr.GetString(4);
                                detalle.FechaVencimiento = dr.IsDBNull(5) ? null : dr.GetString(5);
                                detalle.RegistroSanitario = dr.IsDBNull(6) ? null : dr.GetString(6);
                                detalle.Fabricante = dr.IsDBNull(7) ? null : dr.GetString(7);
                                detalle.CondicionAlmTrans = dr.IsDBNull(8) ? null : dr.GetString(8);
                                detalle.Almacen = dr.IsDBNull(9) ? null : dr.GetString(9);
                                detalle.CertificadoAnalisis = dr.IsDBNull(10) ? null : dr.GetString(10);
                                detalle.ComentarioOrganoleptico = dr.IsDBNull(11) ? null : dr.GetString(11);
                                detalle.CantidadAprobados = dr.IsDBNull(12) ? 0 : dr.GetInt32(12);
                                detalle.CantidadBaja = dr.IsDBNull(13) ? 0 : dr.GetInt32(13);
                                detalle.CantidadDevolucion = dr.IsDBNull(14) ? 0 : dr.GetInt32(14);
                                detalle.CantidadTotal = dr.IsDBNull(15) ? 0 : dr.GetInt32(15);
                                detalle.Liberado = dr.IsDBNull(16) ? 0 : dr.GetInt32(16);
                                detalle.Transferido = dr.IsDBNull(17) ? 0 : dr.GetInt32(17);

                                // Asignación de archivos adjuntos
                                string baseRuta = uti.directorioFileServer;
                                string rutaDirectorio = Path.Combine(baseRuta, "DireccionTecnica", "Internamiento");
                                string carpeta = detalle.ItemCode ?? "undefined";
                                string rutaET = Path.Combine(rutaDirectorio, carpeta, "ET.pdf").Replace("\\", "/");
                                if (System.IO.File.Exists(rutaET))
                                {
                                    byte[] contenido = System.IO.File.ReadAllBytes(rutaET);
                                    detalle.DescargarArchivoET = Convert.ToBase64String(contenido);
                                }


                                string rutaProtocolo = Path.Combine(rutaDirectorio, carpeta, $"{detalle.Lote}.pdf").Replace("\\", "/");
                                if (System.IO.File.Exists(rutaProtocolo))
                                {
                                    byte[] contenido2 = System.IO.File.ReadAllBytes(rutaProtocolo);
                                    detalle.DescargarArchivoProtocolo = Convert.ToBase64String(contenido2);
                                }

                                lista.Add(detalle);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error inesperado en DOCS1_D - ListarDetalleDocumento");
            }

            return lista;
        }

        public Helper_E EditarItemDetalleDoc(DOCS1_E datos, string usuarioRegistro)
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

                            cmd.Parameters.AddWithValue("@Operacion", "EDITAR");
                            cmd.Parameters.AddWithValue("@Id", datos.Id);
                            cmd.Parameters.AddWithValue("@CertificadoAnalisis", datos.CertificadoAnalisis);
                            cmd.Parameters.AddWithValue("@ComentarioOrganoleptico", datos.ComentarioOrganoleptico);
                            cmd.Parameters.AddWithValue("@CantidadAprobados", datos.CantidadAprobados);
                            cmd.Parameters.AddWithValue("@CantidadBaja", datos.CantidadBaja);
                            cmd.Parameters.AddWithValue("@CantidadDevolucion", datos.CantidadDevolucion);

                            // Para [CC_ODOCS]
                            cmd.Parameters.AddWithValue("@UsuarioRegistro", usuarioRegistro);
                            cmd.Parameters.AddWithValue("@TipoOperacion", "EDITAR");

                            cmd.ExecuteNonQuery();

                            // Proceso para cargar archivo
                            string baseRuta = uti.directorioFileServer;
                            string rutaDirectorio = Path.Combine(baseRuta, "DireccionTecnica", "Internamiento", datos.ItemCode);

                            if (!Directory.Exists(rutaDirectorio))
                                Directory.CreateDirectory(rutaDirectorio);

                            if (datos.ArchivoET != null)
                            {
                                string extension = Path.GetExtension(datos.ArchivoET.FileName)?.ToLower();
                                string rutaCompleta = Path.Combine(rutaDirectorio, "ET" + extension);
                                datos.ArchivoET.SaveAs(rutaCompleta);
                            }

                            if (datos.ArchivoProtocolo != null)
                            {
                                string extension = Path.GetExtension(datos.ArchivoProtocolo.FileName)?.ToLower();
                                string rutaCompleta = Path.Combine(rutaDirectorio, datos.Lote + extension);
                                datos.ArchivoProtocolo.SaveAs(rutaCompleta);
                            }

                        }
                        transaction.Commit();

                        result.Titulo = "Acción completada";
                        result.Mensajes.Add("Detalle editado correctamente.");
                        result.Icono = "success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogHelper.RegistrarError(ex, "Error inesperado en DOCS1_D - EditarItemDetalleDoc()");

                        result.Titulo = "Error";
                        result.Mensajes.Add("Ocurrió un error al editar el detalle.");
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
