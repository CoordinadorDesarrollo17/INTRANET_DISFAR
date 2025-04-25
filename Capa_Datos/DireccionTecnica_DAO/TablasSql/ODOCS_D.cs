using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.DireccionTecnica_ENT.TablasSql;
using Capa_Entidad.TablasSql;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace Capa_Datos.DireccionTecnica_DAO.TablasSql
{
    public class ODOCS_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<ODOCS_E> ListarInternamientos(string condicion, Dictionary<string, object> parametros)
        {
            List<ODOCS_E> lista = new List<ODOCS_E>();
            var lookup = new Dictionary<int, ODOCS_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.CadSql3))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT DOC.Id, DOC.TipoDocumento, DOC.DocEntry, DOC.DocNum,");
                    sb.AppendLine("CASE WHEN LEFT(DOC.CardCode, 1) = 'P' THEN SUBSTRING(DOC.CardCode, 2, LEN(DOC.CardCode)) ELSE DOC.CardCode END AS CardCodeFormat,");
                    sb.AppendLine("DOC.CardName, DOC.Guia, DOC.ComprobanteVinculado,");
                    sb.AppendLine("CONVERT(varchar, DOC.FechaContabilizacion, 103) AS FechaContabilizacion,");
                    sb.AppendLine("CONVERT(varchar, DOC.FechaInicioTraslado, 103) AS FechaInicioTraslado,");
                    sb.AppendLine("DOC.Estado,");
                    sb.AppendLine("DET.Id, DET.ItemCode, DET.ItemName, DET.Lote,");
                    sb.AppendLine("CONVERT(varchar, DET.FechaVencimiento, 103) AS FechaVencimiento,");
                    sb.AppendLine("DET.RegistroSanitario, DET.Fabricante, DET.CondicionAlmTrans,");
                    sb.AppendLine("DET.Almacen, DET.CertificadoAnalisis, DET.ComentarioOrganoleptico,");
                    sb.AppendLine("DET.CantidadAprobados, DET.CantidadBaja, DET.CantidadDevolucion,");
                    sb.AppendLine("DET.CantidadTotal, DET.Liberado, DET.Transferido,");

                    // Devuelve el estado actual de la solicitud de reversión asociada al DET.Id, si existe.
                    // Esto permite mostrar en la interfaz el estado textual actual ('Pendiente', 'Aprobada', 'Rechazada', etc.),
                    // y controlar dinámicamente la lógica del botón de envío o la visualización del estado actual de la solicitud.
                    sb.AppendLine("(");
                    sb.AppendLine("    SELECT TOP 1 SR.Estado");
                    sb.AppendLine("    FROM SolicitudesReversion_DOCS1 SR");
                    sb.AppendLine("    WHERE SR.DOCS1Id = DET.Id");
                    sb.AppendLine("    ORDER BY SR.Id DESC"); 
                    sb.AppendLine(") AS EstadoSolicitudReversion");

                    sb.AppendLine("FROM ODOCS DOC");
                    sb.AppendLine("INNER JOIN DOCS1 DET ON DOC.Id = DET.ODOCSId");
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
                                int id = dr.GetInt32(0);

                                if (!lookup.TryGetValue(id, out var obj))
                                {
                                    obj = new ODOCS_E
                                    {
                                        Id = id,
                                        TipoDocumento = dr.IsDBNull(1) ? null : dr.GetString(1),
                                        DocEntry = dr.IsDBNull(2) ? 0 : dr.GetInt64(2),
                                        DocNum = dr.IsDBNull(3) ? 0 : dr.GetInt64(3),
                                        CardCode = dr.IsDBNull(4) ? null : dr.GetString(4),
                                        CardName = dr.IsDBNull(5) ? null : dr.GetString(5),
                                        Guia = dr.IsDBNull(6) ? null : dr.GetString(6),
                                        ComprobanteVinculado = dr.IsDBNull(7) ? null : dr.GetString(7),
                                        FechaContabilizacion = dr.IsDBNull(8) ? null : dr.GetString(8),
                                        FechaInicioTraslado = dr.IsDBNull(9) ? null : dr.GetString(9),
                                        Estado = dr.IsDBNull(10) ? null : dr.GetString(10),
                                        Detalle = new List<DOCS1_E>()
                                    };

                                    lookup[id] = obj;
                                    lista.Add(obj);
                                }

                                // Agregar detalle
                                var detalle = new DOCS1_E();
                                detalle.Id = dr.IsDBNull(11) ? 0 : dr.GetInt32(11);
                                detalle.ItemCode = dr.IsDBNull(12) ? null : dr.GetString(12);
                                detalle.ItemName = dr.IsDBNull(13) ? null : dr.GetString(13);
                                detalle.Lote = dr.IsDBNull(14) ? null : dr.GetString(14);
                                detalle.FechaVencimiento = dr.IsDBNull(15) ? null : dr.GetString(15);
                                detalle.RegistroSanitario = dr.IsDBNull(16) ? null : dr.GetString(16);
                                detalle.Fabricante = dr.IsDBNull(17) ? null : dr.GetString(17);
                                detalle.CondicionAlmTrans = dr.IsDBNull(18) ? null : dr.GetString(18);
                                detalle.Almacen = dr.IsDBNull(19) ? null : dr.GetString(19);
                                detalle.CertificadoAnalisis = dr.IsDBNull(20) ? null : dr.GetString(20);
                                detalle.ComentarioOrganoleptico = dr.IsDBNull(21) ? null : dr.GetString(21);
                                detalle.CantidadAprobados = dr.IsDBNull(22) ? 0 : dr.GetInt32(22);
                                detalle.CantidadBaja = dr.IsDBNull(23) ? 0 : dr.GetInt32(23);
                                detalle.CantidadDevolucion = dr.IsDBNull(24) ? 0 : dr.GetInt32(24);
                                detalle.CantidadTotal = dr.IsDBNull(25) ? 0 : dr.GetInt32(25);
                                detalle.Liberado = dr.IsDBNull(26) ? 0 : dr.GetInt32(26);
                                detalle.Transferido = dr.IsDBNull(27) ? 0 : dr.GetInt32(27);
                                detalle.EstadoSolicitudReversion = dr.IsDBNull(28) ? null : dr.GetString(28);

                                // Asignación de archivos adjuntos
                                string baseRuta = uti.directorioDocumentosRegulatorios;
                                string rutaDirectorio = Path.Combine(baseRuta, "Documentos");
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

                                string rutaOrganoleptico = Path.Combine(rutaDirectorio, carpeta, $"{detalle.Lote}.pdf").Replace("\\", "/");
                                if (System.IO.File.Exists(rutaOrganoleptico))
                                {
                                    byte[] contenido3 = System.IO.File.ReadAllBytes(rutaOrganoleptico);
                                    detalle.DescargarArchivoOrganoleptico = Convert.ToBase64String(contenido3);
                                }

                                obj.Detalle.Add(detalle);
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

        public List<ODOCS_E> ListarTransferencias(string condicion, Dictionary<string, object> parametros)
        {
            List<ODOCS_E> lista = new List<ODOCS_E>();
            var lookup = new Dictionary<int, ODOCS_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.CadSql3))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT");
                    sb.AppendLine("DOC.Id, DOC.TipoDocumento, DOC.DocEntry, DOC.DocNum,");
                    sb.AppendLine("CASE WHEN LEFT(DOC.CardCode, 1) = 'P' THEN SUBSTRING(DOC.CardCode, 2, LEN(DOC.CardCode)) ELSE DOC.CardCode END AS CardCodeFormat,");
                    sb.AppendLine("DOC.CardName, DOC.Guia, DOC.ComprobanteVinculado,");
                    sb.AppendLine("CONVERT(varchar, DOC.FechaContabilizacion, 103) AS FechaContabilizacion,");
                    sb.AppendLine("CONVERT(varchar, DOC.FechaInicioTraslado, 103) AS FechaInicioTraslado,");
                    sb.AppendLine("DOC.Estado,");

                    sb.AppendLine("DET.Id, DET.ItemCode, DET.ItemName, DET.Lote,");
                    sb.AppendLine("CONVERT(varchar, DET.FechaVencimiento, 103) AS FechaVencimiento,");
                    sb.AppendLine("DET.RegistroSanitario, DET.Fabricante, DET.CondicionAlmTrans,");
                    sb.AppendLine("DET.Almacen, DET.CertificadoAnalisis, DET.ComentarioOrganoleptico,");
                    sb.AppendLine("DET.CantidadAprobados, DET.CantidadBaja, DET.CantidadDevolucion, DET.CantidadTotal,");
                    sb.AppendLine("DET.Liberado, DET.Transferido,");

                    // Aprobados
                    sb.AppendLine("AT_AP.Area, AT_AP.Atendido AS AtendidoAprobados, AT_AP.AtendidoPor AS AtendidoPor,");
                    sb.AppendLine("CONVERT(varchar, AT_AP.Fecha, 103) AS FechaAprobados, CONVERT(varchar, AT_AP.Hora , 8) AS HoraAprobados,");

                    // Baja
                    sb.AppendLine("AT_BA.Area, AT_BA.Atendido AS AtendidoBaja, AT_BA.AtendidoPor AS AtendidoPor,");
                    sb.AppendLine("CONVERT(varchar, AT_BA.Fecha, 103) AS FechaBaja, CONVERT(varchar, AT_BA.Hora , 8) AS HoraBaja,");

                    // Devolución
                    sb.AppendLine("AT_DE.Area, AT_DE.Atendido AS AtendidoDevolucion, AT_DE.AtendidoPor AS AtendidoPor,");
                    sb.AppendLine("CONVERT(varchar, AT_DE.Fecha, 103) AS FechaDevolucion, CONVERT(varchar, AT_DE.Hora , 8) AS HoraDevolucion");

                    sb.AppendLine("FROM ODOCS DOC");
                    sb.AppendLine("INNER JOIN DOCS1 DET ON DOC.Id = DET.ODOCSId");

                    sb.AppendLine("OUTER APPLY (");
                    sb.AppendLine("    SELECT TOP 1 Area, Atendido, AtendidoPor, CONVERT(varchar, Fecha, 103) AS Fecha, CONVERT(varchar, Hora , 8) AS Hora FROM AT_DOCS1");
                    sb.AppendLine("    WHERE DOCS1Id = DET.Id AND Area = 'Aprobados'");
                    sb.AppendLine("    ORDER BY Fecha DESC, Hora DESC");
                    sb.AppendLine(") AT_AP");

                    sb.AppendLine("OUTER APPLY (");
                    sb.AppendLine("    SELECT TOP 1 Area, Atendido, AtendidoPor, CONVERT(varchar, Fecha, 103) AS Fecha, CONVERT(varchar, Hora , 8) AS Hora FROM AT_DOCS1");
                    sb.AppendLine("    WHERE DOCS1Id = DET.Id AND Area = 'Baja'");
                    sb.AppendLine("    ORDER BY Fecha DESC, Hora DESC");
                    sb.AppendLine(") AT_BA");

                    sb.AppendLine("OUTER APPLY (");
                    sb.AppendLine("    SELECT TOP 1 Area, Atendido, AtendidoPor, CONVERT(varchar, Fecha, 103) AS Fecha, CONVERT(varchar, Hora , 8) AS Hora FROM AT_DOCS1");
                    sb.AppendLine("    WHERE DOCS1Id = DET.Id AND Area = 'Devolucion'");
                    sb.AppendLine("    ORDER BY Fecha DESC, Hora DESC");
                    sb.AppendLine(") AT_DE");

                    sb.AppendLine("WHERE DET.Liberado = 1");
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
                                int id = dr.GetInt32(0);

                                if (!lookup.TryGetValue(id, out var obj))
                                {
                                    obj = new ODOCS_E
                                    {
                                        Id = id,
                                        TipoDocumento = dr.IsDBNull(1) ? null : dr.GetString(1),
                                        DocEntry = dr.IsDBNull(2) ? 0 : dr.GetInt64(2),
                                        DocNum = dr.IsDBNull(3) ? 0 : dr.GetInt64(3),
                                        CardCode = dr.IsDBNull(4) ? null : dr.GetString(4),
                                        CardName = dr.IsDBNull(5) ? null : dr.GetString(5),
                                        Guia = dr.IsDBNull(6) ? null : dr.GetString(6),
                                        ComprobanteVinculado = dr.IsDBNull(7) ? null : dr.GetString(7),
                                        FechaContabilizacion = dr.IsDBNull(8) ? null : dr.GetString(8),
                                        FechaInicioTraslado = dr.IsDBNull(9) ? null : dr.GetString(9),
                                        Estado = dr.IsDBNull(10) ? null : dr.GetString(10),
                                        Detalle = new List<DOCS1_E>()
                                    };

                                    lookup[id] = obj;
                                    lista.Add(obj);
                                }

                                // Agregar detalle
                                var detalle = new DOCS1_E();
                                detalle.Id = dr.IsDBNull(11) ? 0 : dr.GetInt32(11);
                                detalle.ItemCode = dr.IsDBNull(12) ? null : dr.GetString(12);
                                detalle.ItemName = dr.IsDBNull(13) ? null : dr.GetString(13);
                                detalle.Lote = dr.IsDBNull(14) ? null : dr.GetString(14);
                                detalle.FechaVencimiento = dr.IsDBNull(15) ? null : dr.GetString(15);
                                detalle.RegistroSanitario = dr.IsDBNull(16) ? null : dr.GetString(16);
                                detalle.Fabricante = dr.IsDBNull(17) ? null : dr.GetString(17);
                                detalle.CondicionAlmTrans = dr.IsDBNull(18) ? null : dr.GetString(18);
                                detalle.Almacen = dr.IsDBNull(19) ? null : dr.GetString(19);
                                detalle.CertificadoAnalisis = dr.IsDBNull(20) ? null : dr.GetString(20);
                                detalle.ComentarioOrganoleptico = dr.IsDBNull(21) ? null : dr.GetString(21);
                                detalle.CantidadAprobados = dr.IsDBNull(22) ? 0 : dr.GetInt32(22);
                                detalle.CantidadBaja = dr.IsDBNull(23) ? 0 : dr.GetInt32(23);
                                detalle.CantidadDevolucion = dr.IsDBNull(24) ? 0 : dr.GetInt32(24);
                                detalle.CantidadTotal = dr.IsDBNull(25) ? 0 : dr.GetInt32(25);
                                detalle.Liberado = dr.IsDBNull(26) ? 0 : dr.GetInt32(26);
                                detalle.Transferido = dr.IsDBNull(27) ? 0 : dr.GetInt32(27);

                                detalle.AtencionAprobados = new AT_DOCS1_E
                                {
                                    Area = dr.IsDBNull(28) ? null : dr.GetString(28),
                                    Atendido = dr.IsDBNull(29) ? 0 : dr.GetInt32(29),
                                    AtendidoPor = dr.IsDBNull(30) ? null : dr.GetString(30),
                                    Fecha = dr.IsDBNull(31) ? null : dr.GetString(31),
                                    Hora = dr.IsDBNull(32) ? null : dr.GetString(32)
                                };

                                detalle.AtencionBaja = new AT_DOCS1_E
                                {
                                    Area = dr.IsDBNull(33) ? null : dr.GetString(33),
                                    Atendido = dr.IsDBNull(34) ? 0 : dr.GetInt32(34),
                                    AtendidoPor = dr.IsDBNull(35) ? null : dr.GetString(35),
                                    Fecha = dr.IsDBNull(36) ? null : dr.GetString(36),
                                    Hora = dr.IsDBNull(37) ? null : dr.GetString(37)
                                };

                                detalle.AtencionDevolucion = new AT_DOCS1_E
                                {
                                    Area = dr.IsDBNull(38) ? null : dr.GetString(38),
                                    Atendido = dr.IsDBNull(39) ? 0 : dr.GetInt32(39),
                                    AtendidoPor = dr.IsDBNull(40) ? null : dr.GetString(40),
                                    Fecha = dr.IsDBNull(41) ? null : dr.GetString(41),
                                    Hora = dr.IsDBNull(42) ? null : dr.GetString(42)
                                };

                                // Asignación de archivos adjuntos
                                string baseRuta = uti.directorioDocumentosRegulatorios;
                                string rutaDirectorio = Path.Combine(baseRuta, "Documentos");
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

                                obj.Detalle.Add(detalle);
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

                            // Proceso para cargar archivo
                            string baseRuta = uti.directorioDocumentosRegulatorios;

                            foreach (var item in datos.Detalle)
                            {
                                if (item.ArchivoET == null && item.ArchivoProtocolo == null)
                                    continue;

                                string rutaDirectorio = Path.Combine(baseRuta, "Documentos", item.ItemCode);

                                if (!Directory.Exists(rutaDirectorio))
                                    Directory.CreateDirectory(rutaDirectorio);

                                if (item.ArchivoET != null)
                                {
                                    string extension = Path.GetExtension(item.ArchivoET.FileName)?.ToLower();
                                    if (extension != ".pdf")
                                        continue;

                                    string rutaCompleta = Path.Combine(rutaDirectorio, "ET" + extension);
                                    item.ArchivoET.SaveAs(rutaCompleta);
                                }

                                if (item.ArchivoProtocolo != null)
                                {
                                    string extension = Path.GetExtension(item.ArchivoProtocolo.FileName)?.ToLower();
                                    if (extension != ".pdf")
                                        continue;

                                    string rutaCompleta = Path.Combine(rutaDirectorio, item.Lote + extension);
                                    item.ArchivoProtocolo.SaveAs(rutaCompleta);
                                }
                            }

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

        public Helper_E CancelarDocumento(int id, string usuarioRegistro)
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

                            cmd.Parameters.AddWithValue("@Operacion", "CANCELAR");
                            cmd.Parameters.AddWithValue("@Id", id);

                            // Para [CC_ODOCS]
                            cmd.Parameters.AddWithValue("@UsuarioRegistro", usuarioRegistro);
                            cmd.Parameters.AddWithValue("@TipoOperacion", "CANCELAR");

                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();

                        result.Titulo = "Acción completada";
                        result.Mensajes.Add("Documento cancelado correctamente.");
                        result.Icono = "success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogHelper.RegistrarError(ex, "Error inesperado en ODOCS_D - CancelarDocumento()");

                        result.Titulo = "Error";
                        result.Mensajes.Add("Ocurrió un error al cancelar el documento.");
                        result.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                        result.Icono = "error";
                    }
                }
            }

            return result;
        }

        public Helper_E CancelarTransferencia(int id, string usuarioRegistro)
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

                            cmd.Parameters.AddWithValue("@Operacion", "CANCELAR_TRANSFERENCIA");
                            cmd.Parameters.AddWithValue("@Id", id);

                            // Para [CC_ODOCS]
                            cmd.Parameters.AddWithValue("@UsuarioRegistro", usuarioRegistro);
                            cmd.Parameters.AddWithValue("@TipoOperacion", "CANCELAR_TRANSFERENCIA");

                            cmd.ExecuteNonQuery();
                        }
                        transaction.Commit();

                        result.Titulo = "Acción completada";
                        result.Mensajes.Add("Transferencia cancelada correctamente.");
                        result.Icono = "success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogHelper.RegistrarError(ex, "Error inesperado en ODOCS_D - CancelarTransferencia()");

                        result.Titulo = "Error";
                        result.Mensajes.Add("Ocurrió un error al cancelar la transferencia del documento.");
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