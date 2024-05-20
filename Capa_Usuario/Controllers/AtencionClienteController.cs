using Capa_Entidad.AtencionCliente_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.AtencionCliente_NEG.TablasSql;
using Capa_Negocio.General_NEG.Tablas;
using Capa_Negocio.Seguridad_NEG;
using Capa_Negocio.Ventas_NEG.TablasSql;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Capa_Usuario.Controllers
{
    public class AtencionClienteController : Controller
    {
        Rol1_N rol1 = new Rol1_N(); int modulo = 7;
        OSAT_N osatN = new OSAT_N();
        SAT1_N sat1N = new SAT1_N();

        protected Dictionary<string, string> DatosSolicitud(string tipoVenta, string canalVenta, string errorAlm)
        {
            if (string.IsNullOrEmpty(tipoVenta)) { tipoVenta = ""; }
            if (string.IsNullOrEmpty(canalVenta)) { canalVenta = ""; }
            if (string.IsNullOrEmpty(errorAlm)) { errorAlm = ""; }

            Dictionary<string, string> opcionesTipoVenta = new Dictionary<string, string>
                {
                    { "", ""},
                    { "VCALLC", "Ventas Call Center"},
                    { "VHORIZ", "Ventas Horizontal"},
                    { "VESTRAT", "Ventas Estratégicas"}
                };

            Dictionary<string, string> opcionesCanalVenta = new Dictionary<string, string>
                {
                    { "", ""},
                    { "LIMA", "Lima"},
                    { "PROV", "Provincia"},
                    { "LIC", "Licitación"},
                    { "CAD", "Cadena"},
                    { "PROMLIMA", "Promotoría Lima"},
                    { "PROMPROV", "Promotoría Provincia"},
                    { "TELEV", "Televentas"},
                    { "CENTRO", "Centro"}
                };

            Dictionary<string, string> opcionesErrorAlmacen = new Dictionary<string, string>
                {
                    { "", ""},
                    { "ARECEP3", "Área de Recepción 3"},
                    { "ARECEP5", "Área de Recepción 5"},
                    { "ARECEP7", "Área de Recepción 7"},
                    { "ADESP", "Área de Despacho"},
                    { "AVERIF", "Área de Verificación"},
                    { "AEMB", "Área de Embalaje"},
                    { "APICK", "Área de Picking"},
                    { "AFACT", "Área de Facturación"},
                    { "AING", "Área de Ingreso"}
                };

            Dictionary<string, string> result = new Dictionary<string, string>
                {
                    {"TipoVenta", opcionesTipoVenta[tipoVenta]},
                    {"CanalVenta", opcionesCanalVenta[canalVenta]},
                    {"ErrorAlmacen", opcionesErrorAlmacen[errorAlm]}
                };

            return result;
        }

        public JsonResult ObtenerDetalleSolicitud(int DocEntry)
        {
            return Json(sat1N.BuscarDetallesSolicitud(DocEntry));
        }

        public ActionResult GestionSolicitud(OSAT_E filtro, string Mensaje = "", int idOperation = 2701)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ViewBag.Mensaje = Mensaje;
                ViewBag.Osat = filtro;

                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                if (user.IdRol == 54)
                {
                    return View(osatN.ListarSolicitudes(filtro, true));
                }
                else
                {
                    return View(osatN.ListarSolicitudes(filtro, false));
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }

        }
        public ActionResult ExportarExcel(OSAT_E filtro, int idOperation = 2701)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OSAT_N osatN = new OSAT_N();
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var solicitudes = osatN.ListarSolicitudesExcel(filtro);
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("Solicitudes");
                    worksheet.Cells["A1"].LoadFromCollection(solicitudes, PrintHeaders: true);

                    if (solicitudes != null && solicitudes.Count >= 1)
                    {
                        for (var col = 1; col <= 26; col++)
                        {
                            worksheet.Column(col).AutoFit();
                        }

                        var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: solicitudes.Count + 1, toColumn: 26), "Solicitudes");
                        tabla.ShowHeader = true;
                        tabla.TableStyle = TableStyles.Medium2;
                    }

                    return File(libro.GetAsByteArray(), excelContentType, "Solicitudes.xlsx");
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login") { return null; }
            else { return null; }
        }
        public ActionResult NuevaSolicitud(int idOperation = 2702)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ORTV_N ortvN = new ORTV_N();
                ViewBag.Tickets = ortvN.ListarTicketsParaAtencion();
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }

        }

        [HttpPost]
        public ActionResult NuevaSolicitud(OSAT_E obj, int idOperation = 2702)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.OpRegistro = user.Nombres + " " + user.Apellidos;
                    string DocNum = osatN.registrarNuevaSolicitud(obj);
                    return RedirectToAction("GestionSolicitud", new { DocNum = DocNum });
                }
                catch (Exception e)
                {
                    ORTV_N ortvN = new ORTV_N();
                    ViewBag.Mensaje = e.Message; ViewBag.Tickets = ortvN.ListarTicketsParaAtencion(); return View(obj);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }

        }
        public ActionResult DetallesSolicitud(int id, int idOperation = 2703)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    CC_OSAT_N ccOSAT_N = new CC_OSAT_N();
                    var datosProceso = ccOSAT_N.ListarCC_OSAT(id, "PROCESAR");
                    var datosAtencion = ccOSAT_N.ListarCC_OSAT(id, "ATENDER");
                    var datosCulminacion = ccOSAT_N.ListarCC_OSAT(id, "CULMINAR");
                    var adjuntos = osatN.BuscarAdjuntosOSAT(id, 0);

                    string errorAlm = string.Empty;
                    var result = osatN.buscarSolicitud(id);

                    // Solo cuando el Tipo de Error sea "ErrorAlmacen" mostrará el campo Error de almacén
                    if (result.Det != null && result.Det.Count >= 1)
                    {
                        errorAlm = result.Det[0].ErrorAlmacen;
                    }

                    var datos = DatosSolicitud(result.TipoVenta, result.CanalVenta, errorAlm);

                    ViewBag.ErrorAlmacen = errorAlm;
                    ViewBag.TipoVenta = datos["TipoVenta"];
                    ViewBag.CanalVenta = datos["CanalVenta"];
                    ViewBag.ErrorAlmacen = datos["ErrorAlmacen"];

                    ViewBag.Adjuntos = adjuntos;
                    ViewBag.CantidadAdjuntos = adjuntos.Count();
                    ViewBag.FechaProceso = (datosProceso != null && datosProceso[0].FechaOperacion != null) ? $"{datosProceso[0].FechaOperacion} {datosProceso[0].HoraOperacion}" : "";
                    ViewBag.FechaAtencion = (datosAtencion != null && datosAtencion[0].FechaOperacion != null) ? $"{datosAtencion[0].FechaOperacion} {datosAtencion[0].HoraOperacion}" : "";
                    ViewBag.FechaCulminacion = (datosCulminacion != null) ? $"{datosCulminacion[0].FechaOperacion} {datosCulminacion[0].HoraOperacion}" : "";
                    ViewBag.OpProceso = (datosProceso != null && datosProceso[0].Operario != null) ? datosProceso[0].Operario : "";
                    ViewBag.OpAtencion = (datosAtencion != null && datosAtencion[0].Operario != null) ? datosAtencion[0].Operario : "";
                    ViewBag.OpCulminacion = (datosCulminacion != null && datosCulminacion[0].Operario != null) ? datosCulminacion[0].Operario : "";

                    return View(result);
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message; return View(id);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }

        }
        public ActionResult EditarSolicitud(int id, int idOperation = 2704)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                var result = osatN.buscarSolicitud(id);
                ViewBag.Adjuntos = osatN.BuscarAdjuntosOSAT(id, 0);

                return View(result);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult EditarSolicitud(OSAT_E OSAT_Post, int idOperation = 2704)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    OSAT_Post.OpRegistro = $"{user.Nombres} {user.Apellidos}";
                    string DocNum = osatN.editarSolicitud(OSAT_Post);
                    return RedirectToAction("GestionSolicitud", new { DocNum = DocNum });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    ViewBag.Adjuntos = osatN.BuscarAdjuntosOSAT(OSAT_Post.DocEntry, 0);
                    return View(OSAT_Post);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AnularSolicitud(OSAT_E obj, int idOperation = 2705)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.OpRegistro = $"{user.Nombres} {user.Apellidos}";
                    OSAT_E detalles = osatN.buscarSolicitud(obj.DocEntry);
                    osatN.anularSolicitud(detalles);

                    return RedirectToAction("GestionSolicitud", new { DocNum = obj.DocNum });
                }
                catch (Exception e)
                {
                    return RedirectToAction("GestionSolicitud", new { Mensaje = e.Message });
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

        public ActionResult ProcesarSolicitud(int id, int idOperation = 2706)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_N ousrN = new Usuario_N();

                var result = osatN.buscarSolicitud(id);
                var datos = DatosSolicitud(result.TipoVenta, result.CanalVenta, "");

                ViewBag.TipoVenta = datos["TipoVenta"];
                ViewBag.CanalVenta = datos["CanalVenta"];
                ViewBag.Usuarios = ousrN.ListaUsuarios(null);
                ViewBag.Adjuntos = osatN.BuscarAdjuntosOSAT(id, 0);

                return View(result);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

        [HttpPost]
        public ActionResult ProcesarSolicitud(OSAT_E obj, int idOperation = 2706)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.OpRegistro = $"{user.Nombres} {user.Apellidos}";
                    string DocNum = osatN.procesarSolicitud(obj);
                    return RedirectToAction("GestionSolicitud", new { DocNum = DocNum });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return View(obj);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RevertirProcesarSolicitud(OSAT_E obj, int idOperation = 2706)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.Adjuntos = osatN.BuscarAdjuntosOSAT(obj.DocEntry, 0);
                    string DocNum = osatN.revertirProcesarSolicitud(obj);
                    return RedirectToAction("GestionSolicitud", new { DocNum = DocNum });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    ViewBag.Adjuntos = osatN.BuscarAdjuntosOSAT(obj.DocEntry, 0);
                    return RedirectToAction("ProcesarSolicitud", new { id = obj.DocEntry });
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AtenderSolicitud(int id, int idOperation = 2707)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OWHS_N owhsN = new OWHS_N();
                OREG_N oregN = new OREG_N();
                CC_OSAT_N ccOSAT_N = new CC_OSAT_N();

                string errorAlm = string.Empty;
                var result = osatN.buscarSolicitud(id);

                // Solo cuando el Tipo de Error sea "ErrorAlmacen" mostrará el campo Error de almacén
                if (result.Det != null && result.Det.Count >= 1)
                {
                    errorAlm = result.Det[0].ErrorAlmacen;
                }

                var datos = DatosSolicitud(result.TipoVenta, result.CanalVenta, errorAlm);

                ViewBag.ErrorAlmacen = errorAlm;
                ViewBag.TipoVenta = datos["TipoVenta"];
                ViewBag.CanalVenta = datos["CanalVenta"];
                ViewBag.ErrorAlmacen = datos["ErrorAlmacen"];
                ViewBag.Almacenes = owhsN.ListarAlmacenes("todos");
                ViewBag.Regalos = oregN.listaRegalos(null);
                ViewBag.DatosAtencion = ccOSAT_N.ListarCC_OSAT(id, "ATENDER");

                return View(result);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult AtenderSolicitud(OSAT_E obj, int idOperation = 2707)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OWHS_N owhsN = new OWHS_N();
                OREG_N oregN = new OREG_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.OpRegistro = $"{user.Nombres} {user.Apellidos}";

                    string DocNum = osatN.atenderSolicitud(obj);
                    return RedirectToAction("GestionSolicitud", new { DocNum = DocNum });

                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    ViewBag.Adjuntos = osatN.BuscarAdjuntosOSAT(obj.DocEntry, 0);
                    ViewBag.Almacenes = owhsN.ListarAlmacenes("todos");
                    ViewBag.Regalos = oregN.listaRegalos(null);
                    return View(obj);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RevertirAtenderSolicitud(OSAT_E obj, int idOperation = 2707)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.OpRegistro = $"{user.Nombres} {user.Apellidos}";
                    string DocNum = osatN.revertirAtenderSolicitud(obj);
                    return RedirectToAction("GestionSolicitud", new { DocNum = DocNum });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return RedirectToAction("AtenderSolicitud", new { id = obj.DocEntry });
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult CulminarSolicitud(int id, int idOperation = 2708)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                CC_OSAT_N ccOSAT_N = new CC_OSAT_N();
                ViewBag.DatosAtencion = ccOSAT_N.ListarCC_OSAT(id, "ATENDER");
                ViewBag.DatosCulminacion = ccOSAT_N.ListarCC_OSAT(id, "CULMINAR");

                string errorAlm = string.Empty;
                var result = osatN.buscarSolicitud(id);

                // Solo cuando el Tipo de Error sea "ErrorAlmacen" mostrará el campo Error de almacén
                if (result.Det != null && result.Det.Count >= 1)
                {
                    errorAlm = result.Det[0].ErrorAlmacen;
                }

                var datos = DatosSolicitud(result.TipoVenta, result.CanalVenta, errorAlm);

                ViewBag.ErrorAlmacen = errorAlm;
                ViewBag.TipoVenta = datos["TipoVenta"];
                ViewBag.CanalVenta = datos["CanalVenta"];
                ViewBag.ErrorAlmacen = datos["ErrorAlmacen"];

                return View(result);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

        [HttpPost]
        public ActionResult CulminarSolicitud(OSAT_E obj, int idOperation = 2708)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.OpRegistro = $"{user.Nombres} {user.Apellidos}";
                    string DocNum = osatN.culminarSolicitud(obj);

                    // Solicitud por Facturación, ya que siempre inician sus actividades con el filtro "Atendido"
                    return RedirectToAction("GestionSolicitud", new { Estado = "Atendido" });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return View(obj);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RevertirCulminarSolicitud(OSAT_E obj, int idOperation = 2708)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.OpRegistro = $"{user.Nombres} {user.Apellidos}";

                    string DocNum = osatN.revertirCulminarSolicitud(obj);
                    return RedirectToAction("GestionSolicitud", new { DocNum = obj.DocNum });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return RedirectToAction("CulminarSolicitud", new { id = obj.DocEntry });
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptDocEnvio(int id, int idOperation = 2709)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ReportClass rc = new ReportClass();
                rc.FileName = Server.MapPath("~/Reportes/RptAtencionCliente/RptDocEnvio.rpt");
                rc.SetParameterValue("@DocEntry", id);
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                rc.SetParameterValue("@Remitente", user.Nombres + " " + user.Apellidos);

                Capa_Negocio.Utilitarios_N utiN = new Capa_Negocio.Utilitarios_N();
                var coninfo = utiN.getConexion();
                TableLogOnInfo logoninfo = new TableLogOnInfo();
                Tables tables;
                tables = rc.Database.Tables;
                foreach (Table item in tables)
                {
                    logoninfo = item.LogOnInfo;
                    logoninfo.ConnectionInfo = coninfo;
                    item.ApplyLogOnInfo(logoninfo);
                }
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream stream = rc.ExportToStream(ExportFormatType.WordForWindows);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/msword", "DocEnvio.doc");
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptDocReg016(int id, int idOperation = 2709)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ReportClass rc = new ReportClass();
                rc.FileName = Server.MapPath("/Reportes/RptAtencionCliente/RptDocReg016.rpt");
                rc.SetParameterValue("@DocEntry", id);

                Capa_Negocio.Utilitarios_N utiN = new Capa_Negocio.Utilitarios_N();
                var coninfo = utiN.getConexion();
                TableLogOnInfo logoninfo = new TableLogOnInfo();
                Tables tables;
                tables = rc.Database.Tables;
                foreach (Table item in tables)
                {
                    logoninfo = item.LogOnInfo;
                    logoninfo.ConnectionInfo = coninfo;
                    item.ApplyLogOnInfo(logoninfo);
                }
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream stream = rc.ExportToStream(ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "RptDocReg016.pdf");
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptDocReg015(int id, int idOperation = 2709)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ReportClass rc = new ReportClass();
                rc.FileName = Server.MapPath("/Reportes/RptAtencionCliente/RptDocReg015.rpt");
                rc.SetParameterValue("@DocEntry", id);

                Capa_Negocio.Utilitarios_N utiN = new Capa_Negocio.Utilitarios_N();
                var coninfo = utiN.getConexion();
                TableLogOnInfo logoninfo = new TableLogOnInfo();
                Tables tables;
                tables = rc.Database.Tables;
                foreach (Table item in tables)
                {
                    logoninfo = item.LogOnInfo;
                    logoninfo.ConnectionInfo = coninfo;
                    item.ApplyLogOnInfo(logoninfo);
                }
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();
                Stream stream = rc.ExportToStream(ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "RptDocReg015.pdf");
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public FileResult ArchivoSolicitud(int id, int linea, int idOperation = 2710)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OSAT_N osatN = new OSAT_N();
                Dictionary<int, string> adjuntos = osatN.BuscarAdjuntosOSAT(id, linea);
                string contentType = "";
                foreach (KeyValuePair<int, string> adj in adjuntos)
                {
                    if (adj.Value.Contains(".pdf"))
                    {
                        contentType = "application/pdf";
                    }
                    else if (adj.Value.Contains(".png"))
                    {
                        contentType = "image/png";
                    }
                    else if (adj.Value.Contains(".jpeg"))
                    {
                        contentType = "image/jpeg";
                    }
                    else if (adj.Value.Contains(".jpg"))
                    {
                        contentType = "image/jpg";
                    }
                    else if (adj.Value.Contains(".docx"))
                    {
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    }
                    else if (adj.Value.Contains(".xlsx"))
                    {
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    }
                    else if (adj.Value.Contains(".zip"))
                    {
                        contentType = "application/x-zip-compressed";
                    }
                    else if (adj.Value.Contains(".rar"))
                    {
                        contentType = "application/octet-stream";
                    }
                }

                string rutaarchivo = "D:/COBEFARWEBFILES/AtencionAlCliente_2023/" + id + "/" + adjuntos[linea];

                return File(rutaarchivo, contentType, adjuntos[linea]);
            }
            else if (verificacionAccesos(idOperation) == "E_Login") { return null; }
            else { return null; }
        }

        //metodos ajax
        public JsonResult listarArticulosTicket(int DocNumTicket)
        {
            return Json(osatN.buscarDatosTicket(DocNumTicket));
        }
        public ActionResult obtenerNroSolicitud(string Tipo)
        {
            return Content(osatN.obtenerNroSolicitud(Tipo));
        }
        public ActionResult validarNuevaSolicitud(OSAT_E obj)
        {
            string status = "true";
            try
            {
                osatN.validarNuevaSolicitud(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarEditarSolicitud(OSAT_E obj)
        {
            string status = "true";
            try
            {
                osatN.validarEditarSolicitud(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarProcesarSolicitud(OSAT_E obj)
        {
            string status = "true";
            try
            {
                osatN.validarProcesarSolicitud(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarAtenderSolicitud(OSAT_E obj)
        {
            string status = "true";
            try
            {
                osatN.validarAtenderSolicitud(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarCulminarSolicitud(OSAT_E obj)
        {
            string status = "true";
            try
            {
                osatN.validarCulminarSolicitud(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        /************************************/
        private string verificacionAccesos(int ope)
        {
            string nombreOperacion = this.ControllerContext.RouteData.Values["action"].ToString();
            Usuario_E user = (Usuario_E)Session["UsuarioId"];
            if (user == null)
            { return "E_Login"; }
            else
            {
                if ((rol1.verificarAccesoOperacion(user.IdRol, ope, nombreOperacion, modulo) == 1) || (user.IdRol == 1))
                {
                    Capa_Negocio.Utilitarios_N utiN = new Capa_Negocio.Utilitarios_N();
                    utiN.registrarLog($"{user.Prefijo} {user.Id}", "intento de " + nombreOperacion, ope, Request.UserHostAddress, Request.UserHostName);
                    return "C_Access";
                }
                else
                { return "E_Access"; }
            }
        }
    }
}
