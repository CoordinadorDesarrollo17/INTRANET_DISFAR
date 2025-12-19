using Capa_Entidad.AtencionCliente_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio;
using Capa_Negocio.AtencionCliente_NEG.TablasSql;
using Capa_Negocio.General_NEG.Tablas;
using Capa_Negocio.Seguridad_NEG;
using Capa_Negocio.Ventas_NEG.TablasSql;
using Capa_Usuario.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace Capa_Usuario.Controllers
{
    public class AtencionClienteController : Controller
    {
        OSAT_N osatN = new OSAT_N();
        SAT1_N sat1N = new SAT1_N();
        /************************* C O N F I G U R A C I Ó N *************************/
        private ActionResult VerificarPermiso(int idOperation)
        {
            var accesoHelper = new Capa_Entidad.AccessoHelper_E
            {
                OpeID = idOperation,
                usuario = (Usuario_E)Session["UsuarioId"],
                controllerDestino = this.ControllerContext.RouteData.Values["controller"].ToString(),
                action = this.ControllerContext.RouteData.Values["action"].ToString(),
                userHostAddress = Request.UserHostAddress,
                userHostName = Request.UserHostName
            };
            return AccesoHelper.GestionarAccesoController(this, accesoHelper);
        }
        /********************************************************************/
        /************************** R E C L A M O S **************************/
        protected Dictionary<string, string> DatosSolicitud(string tipoVenta, string canalVenta, string errorAlm)
        {
            if (string.IsNullOrWhiteSpace(tipoVenta)) { tipoVenta = ""; }
            if (string.IsNullOrWhiteSpace(canalVenta)) { canalVenta = ""; }
            if (string.IsNullOrWhiteSpace(errorAlm)) { errorAlm = ""; }
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
                    { "ARECEP6", "Área de Recepción 6"},
                    { "ARECEP7", "Área de Recepción 7"},
                    { "ARECEP8", "Área de Recepción 8"},
                    { "ADESP", "Área de Despacho"},
                    { "AVERIF", "Área de Verificación"},
                    { "AEMB", "Área de Embalaje"},
                    { "APICK", "Área de Picking"},
                    { "AFACT", "Área de Facturación"},
                    { "AING", "Área de Ingreso"},
                    { "OTROS", "OTROS"},
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
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];

                var mostrarTodos = (user.IdRol == 54) ? true : false;       // IdRol: 54 (operario alm facturador)
                var lista = osatN.ListarSolicitudes(filtro, mostrarTodos, mostrarTodos);

                //var RoFac = (user.IdRol == 54) ? true : false;       // IdRol: 54 (operario alm facturador)
                //var lista = osatN.ListarSolicitudes(filtro, false, RoFac);

                ViewBag.ContadorCriticos = osatN.ListarSolicitudes(filtro, true, false).Count(x => x.DiasRetraso > 2);
                ViewBag.Mensaje = Mensaje;
                ViewBag.Osat = filtro;

                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ExportarExcel(OSAT_E filtro, int idOperation = 2701)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
                        for (var col = 1; col <= 29; col++)
                        {
                            worksheet.Column(col).AutoFit();
                        }
                        var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: solicitudes.Count + 1, toColumn: 29), "Solicitudes");
                        tabla.ShowHeader = true;
                        tabla.TableStyle = TableStyles.Medium2;
                    }
                    return File(libro.GetAsByteArray(), excelContentType, "Solicitudes.xlsx");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult NuevaSolicitud(int idOperation = 2702)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ORTV_N ortvN = new ORTV_N();
                ViewBag.Tickets = ortvN.ListarTicketsParaAtencion();
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult NuevaSolicitud(OSAT_E obj, int idOperation = 2702)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult DetallesSolicitud(int id, int idOperation = 2703)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EditarSolicitud(int id, int idOperation = 2704)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var result = osatN.buscarSolicitud(id);
                ViewBag.Adjuntos = osatN.BuscarAdjuntosOSAT(id, 0);
                return View(result);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult EditarSolicitud(OSAT_E OSAT_Post, int idOperation = 2704)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AnularSolicitud(OSAT_E obj, int idOperation = 2705)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ProcesarSolicitud(int id, int idOperation = 2706)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult ProcesarSolicitud(OSAT_E obj, int idOperation = 2706)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult RevertirProcesarSolicitud(OSAT_E obj, int idOperation = 2706)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AtenderSolicitud(int id, int idOperation = 2707)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult AtenderSolicitud(OSAT_E obj, int idOperation = 2707)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return resultadoAcceso;
            }
        }

        [HttpPost]
        public ActionResult RevertirAtenderSolicitud(OSAT_E obj, int idOperation = 2707)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult CulminarSolicitud(int id, int idOperation = 2708)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult CulminarSolicitud(OSAT_E obj, int idOperation = 2708)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult RevertirCulminarSolicitud(OSAT_E obj, int idOperation = 2708)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return resultadoAcceso;
            }
        }
        public FileResult ArchivoSolicitud(int id, int linea, int idOperation = 2710)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                OSAT_N osatN = new OSAT_N();
                Utilitarios_N utilitarios = new Utilitarios_N();
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
                string rutaarchivo = utilitarios.directorioFileServer + "AtencionAlCliente_2023/" + id + "/" + adjuntos[linea];
                return File(rutaarchivo, contentType, adjuntos[linea]);
            }
            else
            {
                return null;
            }
        }
        public JsonResult ListarArticulosTicket(int DocNumTicket)
        {
            return Json(osatN.BuscarDatosTicket(DocNumTicket));
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
        public JsonResult ObtenerNotificadoCliente(int idOperation = 2711)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var data = osatN.obtenerNotificadoCliente();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = "Acceso denegado" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ObtenerNotificadoClienteDetalle(string CardCode, int idOperation = 2711)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var data = osatN.obtenerNotificadoClienteDetalle(CardCode);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = "Acceso denegado" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult RegalosAplicados(OSAT_E filtro = null, string Mensaje = "", int idOperation = 2713)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Mensaje = Mensaje;
                ViewBag.Osat = filtro ?? new OSAT_E();
                // Llama a un nuevo método que solo trae los OSAT con TicketSolucion no nulo
                var lista = osatN.ListarRegalosAplicados();
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public ActionResult ExportarExcelRegalos(int idOperation = 2713)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OSAT_N osatN = new OSAT_N();
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                // Llama al método que retorna List<Rpt_Regalos>
                var lista = osatN.ListarRegalosAplicados();

                using (var libro = new OfficeOpenXml.ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("RegalosAplicados");
                    worksheet.Cells["A1"].LoadFromCollection(lista, PrintHeaders: true);

                    if (lista != null && lista.Count >= 1)
                    {
                        for (var col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            worksheet.Column(col).AutoFit();
                        }
                        var tabla = worksheet.Tables.Add(
                            new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: lista.Count + 1, toColumn: worksheet.Dimension.End.Column),
                            "RegalosAplicados"
                        );
                        tabla.ShowHeader = true;
                        tabla.TableStyle = TableStyles.Medium2;
                    }
                    return File(libro.GetAsByteArray(), excelContentType, "RegalosAplicados.xlsx");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
    }
}
