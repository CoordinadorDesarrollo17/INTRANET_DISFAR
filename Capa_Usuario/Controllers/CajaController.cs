using Capa_Entidad.Caja_ENT;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Capa_Negocio.Caja_NEG;
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
    public class CajaController : Controller
    {
        private OTC_N otcN = new OTC_N();
        private static readonly object _lock = new object();
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
        // Vista general para el listado de tickets a cuadrar
        public ActionResult Index(int idOperation = 3000)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        // Listado interno dentro de TicketsACuadrar para ser reutilizable
        public ActionResult ListarTicketsAPagar(ORTV_E datos)
        {
            ViewBag.OTC = datos;
            return PartialView("Caja/ListadoTickets", otcN.ListarTicketsVenta(datos));
        }
        [HttpGet]
        public ActionResult PagarTicketVenta(int docEntry, int idOTC = 0, string mensaje = null, int idOperation = 504)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                ORTV_E ticket = new ORTV_N().ObtenerDatosCompletosTicket(docEntry);
                ticket.MontoRecibido = ticket.MontoFinal;
                var result = otcN.ObtenerDatosTicketACuadrar(docEntry, idOTC);
                ViewBag.IdOTC = result?.IdOTC ?? 0;
                ViewBag.MontoRecibidoEfectivo = result?.MontoRecibidoEfectivo ?? 0;
                ViewBag.MontoRecibidoDeposito = result?.MontoRecibidoDeposito ?? 0;
                ViewBag.TipoPago = result?.TipoPago ?? string.Empty;
                ViewBag.DescTipoPago = result?.DescTipoPago ?? string.Empty;
                ViewBag.EstadoContraEntrega = result?.Estado ?? string.Empty;
                ViewBag.FechaCompromisoPago = result?.FechaCompromisoPago ?? string.Empty;
                ViewBag.SaldoAFavor = result?.SaldoAFavor ?? string.Empty;
                ViewBag.ComentarioCaja = result?.ComentarioCaja ?? string.Empty;
                ViewBag.ComentarioVentas = result?.ComentarioVentas ?? string.Empty;
                ViewBag.ComentarioAdjunto = result?.ComentarioAdjunto ?? string.Empty;
                ViewBag.IdRol = usu.IdRol;
                var datosCC = (result != null) ? new CC_OTC_N().ObtenerDatosCC_OTC(result.IdOTC, "REGISTRAR") : null;
                if (datosCC != null)
                {
                    string fechaRegistro = datosCC.FechaOperacion + " " + datosCC.HoraOperacion;
                    ViewBag.DatosRegistro = fechaRegistro;
                    ViewBag.UsuarioValidacion = datosCC.Operario;
                }
                else
                {
                    ViewBag.DatosRegistro = "";
                    ViewBag.UsuarioValidacion = "";
                }
                ViewBag.Comprobantes = (datosCC != null) ? otcN.ObtenerComprobantePagoEfectivo((int)result.DocNumTicket, datosCC.FechaOperacion) : new List<string>();
                OPP_N oppN = new OPP_N();
                ViewBag.PagosParciales = (result != null) ? oppN.ObtenerDatosPagosParciales(result.IdOTC) : null;
                ViewBag.TotalPagosParciales = (result != null) ? oppN.ObtenerTotalPagos(result.IdOTC) : 0;
                if (mensaje != null) { ViewBag.Mensaje = mensaje; }
                return View(ticket);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult PagarTicketVenta(int DocEntryTicket, ORTV_E ticket, int idOTC = 0, int idOperation = 504)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    if (Request.Form["AnularPago"] != null) { return RedirectToAction("AnularPagoTicketVenta", new { DocEntry = DocEntryTicket }); }
                    var result = otcN.ObtenerDatosTicketACuadrar(DocEntryTicket, idOTC);
                    var montoRecibidoEfectivo = result?.MontoRecibidoEfectivo ?? 0;
                    var montoRecibidoDeposito = result?.MontoRecibidoDeposito ?? 0;
                    OPP_N oppN = new OPP_N();
                    var totalPagosParciales = (result != null) ? oppN.ObtenerTotalPagos(result.IdOTC) : 0;
                    ORTV_N ticketN = new ORTV_N();
                    var datosTicket = ticketN.ObtenerDatosCompletosTicket(DocEntryTicket);
                    if (datosTicket.TipoVenta.Equals("ContraEntrega") && datosTicket.EstadoPago.Equals("PENDIENTE") && result != null)
                    {
                        decimal montosRecibidos = Convert.ToDecimal(montoRecibidoEfectivo) + Convert.ToDecimal(montoRecibidoDeposito) + Convert.ToDecimal(totalPagosParciales);
                        if (montosRecibidos != datosTicket.MontoFinal)
                        {
                            ViewBag.Mensaje = "El ticket no pudo ser PAGADO. Verificar los montos ingresados.";
                        }
                    }
                    // Si no existe un mensaje de error, se procede con el pago del ticket
                    if (string.IsNullOrWhiteSpace(ViewBag.Mensaje))
                    {
                        Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                        ticket.CodSapCajero = usu.CodigoSap;
                        ticket.Cajero = $"{usu.Nombres} {usu.Apellidos}";     // Seteamos el usuario Propietario con el nombre del usuario en sesiòn
                        int docNumTicket = otcN.PagarTicket(DocEntryTicket, ticket, usu.IdRol, idOTC);
                        ViewBag.Mensaje = $"Ticket {docNumTicket} PAGADO correctamente";
                    }
                    return RedirectToAction("/", new { DocNum = ticket.DocNum, Mensaje = ViewBag.Mensaje });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return RedirectToAction("PagarTicketVenta", new { DocEntry = DocEntryTicket, Mensaje = ViewBag.Mensaje });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ConfGastEnvio(ORTV_E o, int DocEntryTicket, int idOTC = 0, int idOperation = 504)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E usu = Session["UsuarioId"] as Usuario_E;
                    if (usu != null)
                    {
                        o.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                        o.DocEntry = DocEntryTicket;
                        otcN.ConfGastEnvio(o);
                        return RedirectToAction("PagarTicketVenta", new { DocEntry = DocEntryTicket, IdOTC = idOTC });
                    }
                    else
                    {
                        // La sesión de usuario no está disponible
                        return RedirectToAction("Error", "Index");
                    }
                }
                catch (Exception e)
                {
                    return RedirectToAction("PagarTicketVenta", new { DocEntry = DocEntryTicket, IdOTC = idOTC, mensaje = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AnularPagoTicketVenta(int DocEntryTicket, int idOTC = 0, int idOperation = 505)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    int DocNum = otcN.AnularPagoTicket(DocEntryTicket);
                    return RedirectToAction("Index", new { DocNum = DocNum });
                }
                catch (Exception e)
                {
                    return RedirectToAction("PagarTicketVenta", new { DocEntry = DocEntryTicket, IdOTC = idOTC, mensaje = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        /********************************** P R O Y E C T O:   P A G O   E F E C T I V O **********************************/
        public ActionResult ExportarListadoTicketsACuadrar(ORTV_E filtro, int idOperation = 3000)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                int columnas = 18;
                var listado = otcN.ExportarExcelTicketsACuadrar(filtro);
                if (listado != null && listado.Count >= 1)
                {
                    using (var libro = new ExcelPackage())
                    {
                        var worksheet = libro.Workbook.Worksheets.Add("ListadoTickets_CAJA");
                        worksheet.Cells["A1"].LoadFromCollection(listado, PrintHeaders: true);
                        for (var col = 1; col <= columnas; col++)
                        {
                            worksheet.Column(col).AutoFit();
                        }
                        var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: listado.Count + 1, toColumn: columnas), "ListadoTickets_CAJA");
                        tabla.ShowHeader = true;
                        tabla.TableStyle = TableStyles.Medium2;
                        string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(libro.GetAsByteArray(), excelContentType, "ListadoTickets_CAJA.xlsx");
                    }
                }
                else { return Content("No hay datos para exportar"); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public JsonResult ValidarMontoFinalTicket(OTC_E tc, string lineaORRU, string tipoRepORRU)
        {
            var ticket = new ORTV_N().ObtenerDatosCompletosTicket((int)tc.DocEntryTicket);
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            tc.PersonaEntrega = $"{usu.Nombres} {usu.Apellidos}";
            string result;
            lock (_lock)
            {
                if (ticket.EstadoPago.Equals("PAGADO"))
                {
                    result = "El ticket ya se encuentra PAGADO. Actualizar el listado.";
                }
                else
                {
                    result = otcN.ValidarRegistroTC(tc);
                }
            }
            string proceso = result.Equals("Se solicitó la VALIDACIÓN") ? "OK" : string.Empty;
            return Json(new { Mensaje = result, DocEntry = tc.DocEntryORRU, TipoRep = tipoRepORRU, Linea = lineaORRU, TipoVenta = ticket.TipoVenta, Proceso = proceso });
        }
        public JsonResult ConsultarNuevasSolicitudesTC()
        {
            //VerificarAccesos(0);         // Validar sesion logueada, solo para ajax
            var result = otcN.ConsultarNuevasSolicitudesTC();
            return Json(new { Mensaje = result });
        }
        public JsonResult ObtenerSolicitudesAutorizar()
        {
            //VerificarAccesos(0);         // Validar sesion logueada, solo para ajax
            var result = otcN.ObtenerSolicitudesAutorizar();
            return Json(new { Datos = result });
        }
        public JsonResult ValidarTC(OTC_E tc)
        {
            //VerificarAccesos(0);    // Validar sesion logueada, solo para ajax
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            tc.PersonaEntrega = $"{usu.Nombres} {usu.Apellidos}";
            tc.Estado = "VALIDADO";
            string mensaje;
            var datosTicket = new ORTV_N().ObtenerDatosCompletosTicket((int)tc.DocEntryTicket);
            string[] opcionesValidas = { "SEPARADO", "ANULADO", "CANCELADO", "ENTREGADO" };
            if (!opcionesValidas.Contains(datosTicket.Estado))
            {
                var result = otcN.ObtenerDatosTicketACuadrar((int)tc.DocEntryTicket, (int)tc.IdOTC);
                mensaje = (result != null && result.Estado.Equals("VALIDADO")) ? "El ticket ya se encuentra VALIDADO" : otcN.CambiarEstadoTicketACuadrar(tc, "VALIDAR", "C");
            }
            else { mensaje = "El ticket no se puede validar"; }
            return Json(new { Mensaje = mensaje });
        }
        public JsonResult AutorizarTC(OTC_E tc)
        {
            //VerificarAccesos(0);         // Validar sesion logueada, solo para ajax
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            tc.PersonaEntrega = $"{usu.Nombres} {usu.Apellidos}";
            tc.Estado = "AUTORIZADO";
            string mensaje;
            var datosTicket = new ORTV_N().ObtenerDatosCompletosTicket((int)tc.DocEntryTicket);
            string[] opcionesValidas = { "SEPARADO", "ANULADO", "CANCELADO", "ENTREGADO" };
            if (!opcionesValidas.Contains(datosTicket.Estado))
            {
                var result = otcN.ObtenerDatosTicketACuadrar((int)tc.DocEntryTicket, (int)tc.IdOTC);
                mensaje = (result != null && result.Estado.Equals("AUTORIZADO")) ? "El ticket ya se encuentra AUTORIZADO" : otcN.CambiarEstadoTicketACuadrar(tc, "AUTORIZAR", "V");
            }
            else { mensaje = "El ticket no se puede autorizar"; }
            return Json(new { Mensaje = mensaje });
        }
        public JsonResult RechazarTC(OTC_E tc, string area)
        {
            //VerificarAccesos(0);    // Validar sesion logueada, solo para ajax
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            tc.PersonaEntrega = $"{usu.Nombres} {usu.Apellidos}";
            tc.Estado = "RECHAZADO";
            string mensaje;
            var datosTicket = new ORTV_N().ObtenerDatosCompletosTicket((int)tc.DocEntryTicket);
            string[] opcionesValidas = { "SEPARADO", "ANULADO", "CANCELADO", "ENTREGADO" };
            if (!opcionesValidas.Contains(datosTicket.Estado))
            {
                var result = otcN.ObtenerDatosTicketACuadrar((int)tc.DocEntryTicket, (int)tc.IdOTC);
                mensaje = (result != null && result.Estado.Equals("RECHAZADO")) ? "El ticket ya se encuentra RECHAZADO" : otcN.CambiarEstadoTicketACuadrar(tc, "RECHAZAR", area);
            }
            else { mensaje = "Este ticket no se puede validar"; }
            return Json(new { Mensaje = mensaje });
        }
        public JsonResult AgregarPagosParciales(List<decimal> pagos, List<string> tiposPagosParciales, int docEntryTicket, int idOTC)
        {
            //VerificarAccesos(0);    // Validar sesion logueada, solo para ajax
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            // Se valida si la lista de pagos es nula o vacía al principio del método para evitar iterar sobre ella si no hay pagos que procesar
            if (pagos == null || pagos.Count == 0)
            {
                return Json(new { Titulo = "No ha registrado pagos parciales." });
            }
            var tc = otcN.ObtenerDatosTicketACuadrar(docEntryTicket, idOTC);
            //var pagosParciales = pagos.Select(p => new OPP_E
            //{
            //    IdOTC = tc.IdOTC,
            //    Monto = p,
            //    RegistradoPor = $"{usu.Nombres} {usu.Apellidos}"
            //}).ToList();
            var pagosParciales = new List<OPP_E>();
            int j = 0;
            foreach (var p in pagos)
            {
                var pagoParcial = new OPP_E
                {
                    IdOTC = tc.IdOTC,
                    Monto = p,
                    Comentario = tiposPagosParciales[j],
                    RegistradoPor = $"{usu.Nombres} {usu.Apellidos}"
                };
                pagosParciales.Add(pagoParcial);
                ++j;
            }
            var result = otcN.AgregarPagosParciales(pagosParciales, docEntryTicket);
            return Json(new { Mensaje = result });
        }
        /*
         public JsonResult AgregarPagosParciales(List<decimal> pagos, int docEntryTicket)
        {
            VerificarAccesos(0);         // Validar sesion logueada, solo para ajax
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            var pagosParciales = new List<OPP_E>();
            var tc = otcN.ObtenerDatosTicketACuadrar((int)docEntryTicket);
            if (pagos != null && pagos.Count >= 1)
            {
                foreach (var p in pagos)
                {
                    OPP_E pagoParcial = new OPP_E
                    {
                        IdOTC = tc.IdOTC,
                        Monto = p,
                        RegistradoPor = $"{usu.Nombres} {usu.Apellidos}"
                    };
                    pagosParciales.Add(pagoParcial);
                }
            }
            var result = otcN.AgregarPagosParciales(pagosParciales, docEntryTicket);
            return Json(new { Mensaje = result });
        }
         */
        public JsonResult EliminarPagoParcial(int id)
        {
            //VerificarAccesos(0);    // Validar sesion logueada, solo para ajax
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            var datos = new OPP_E { IdOPP = id, RegistradoPor = $"{usu.Nombres} {usu.Apellidos}" };
            var result = new OPP_N().EliminarPagoParcial(datos);
            var mensaje = result.Equals(1) ? "Pago parcial eliminado satisfactoriamente" : "Error al eliminar el pago parcial";
            return Json(new { Mensaje = mensaje });
        }
        public JsonResult VerRespuestaSolicitud(int docEntryTicket, int idOTC)
        {
            //VerificarAccesos(0);    // Validar sesion logueada, solo para ajax
            string msj = "Solicitud enviada sin respuesta";
            string tieneRespuesta = "NO";
            var result = otcN.ObtenerDatosTicketACuadrar((int)docEntryTicket, (int)idOTC);
            if (result != null)
            {
                if (!string.IsNullOrWhiteSpace(result.ComentarioVentas) || !string.IsNullOrWhiteSpace(result.ComentarioCaja))
                {
                    msj = !string.IsNullOrWhiteSpace(result.ComentarioVentas) ? result.ComentarioVentas : result.ComentarioCaja;
                    tieneRespuesta = "SI";
                }
            }
            return Json(new { Mensaje = msj, Respuesta = tieneRespuesta });
        }
    }
}