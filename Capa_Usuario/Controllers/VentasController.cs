using Capa_Datos;
using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.ComprobantesContables_ENT;
using Capa_Entidad.ReportesDigemid_ENT;
using Capa_Entidad.ReportesDigemid_ENT.Reportes;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.Reportes;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasSql;
using Capa_Negocio.Almacen_NEG.Tablas;
using Capa_Negocio.AtencionCliente_NEG.TablasSql;
using Capa_Negocio.ComprobantesContables_NEG;
using Capa_Negocio.General_NEG.TablasSql;
using Capa_Negocio.Operaciones_NEG.TablasSql;
using Capa_Negocio.Rutas_NEG.TablasSql;
using Capa_Negocio.Seguridad_NEG;
using Capa_Negocio.Seguridad_NEG.TablasSql;
using Capa_Negocio.SocioNegocios_NEG.TablasExternas;
using Capa_Negocio.Ventas_NEG.Tablas;
using Capa_Negocio.Ventas_NEG.TablasSql;
using Capa_Usuario.Helpers;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DocumentFormat.OpenXml.EMMA;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
namespace Capa_Usuario.Controllers
{
    public class VentasController : Controller
    {
        Usuario_N _usuarioN = new Usuario_N();
        ORTV_N _ticketN = new ORTV_N();
        OLDS_N _libroDeSaldoN = new OLDS_N();
        CC_ORTV_N ccORTV_N = new CC_ORTV_N();
        UbicacionesLotes_N _ubicacionesLotesN = new UbicacionesLotes_N();
        private readonly OUSR_OPE_N _usuarioOperacionN = new OUSR_OPE_N();
        private readonly Capa_Negocio.General_NEG.Tablas.OWHS_N _owhsSapN = new Capa_Negocio.General_NEG.Tablas.OWHS_N();
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
        protected string ResaltarTicket(string LugarDestino = "")
        {
            string colorTicket = "#FFFFFF";
            if (!LugarDestino.Equals(""))
            {
                Dictionary<string, string> opcionesColor = new Dictionary<string, string>()
                {
                    { "Arriola", "#FF6800" }, { "Centro", "#0AC5EA" }, { "Domicilio", "#F7F30E" }, { "Agencia", "#F7F30E" }, { "Agencia Courier", "#F7F30E" },
                };
                colorTicket = opcionesColor[LugarDestino];
            }
            return colorTicket;
        }
        public ActionResult ListadoTicketsVenta(int DocNum = 0, ORTV_E t = null, string mensaje = null, int idOperation = 501)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                ViewBag.DocEntryUsuario = user.DocEntry;
                ViewBag.IdRol = user.IdRol;
                ViewBag.ListaTicketsSeparados = _ticketN.listarTicketsSeparados(user.CodigoSap);
                ViewBag.FaltaRegularizar = _ticketN.ListarTicketsPorRegularizarContraEntrega().Count();
                ViewBag.DocNum = DocNum;
                ViewBag.Ortv = t;
                ViewBag.Vendedores = _usuarioN.listaUsuariosPermisos(new Usuario_E { Activo = 1 }, 6);        // Usado como Filtro en el botón AnVentas (Reporte Analítico Ventas)
                ViewBag.CodSapVendedor = user != null && user.IdRol != 6 ? user.CodigoSap : 0;
                if (mensaje != null) { ViewBag.Mensaje = mensaje; }
                return View(_ticketN.ListarTicketsAreaVenta(user, t));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ListadoTicketsAutorizacionRegularizar(string mensaje = null, int idOperation = 501)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (!string.IsNullOrWhiteSpace(mensaje))
                {
                    ViewBag.Mensaje = mensaje;
                }
                // Retorna la lista de objetos dinámicos como modelo
                var listaTickets = _ticketN.ListarTicketsPorRegularizarContraEntrega();
                return View(listaTickets);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //Actualiza Fecha Hora y Operario de Autorizacion en contraentrega fuera de horario.
        public JsonResult RegularizarAutorizacion(int docNum, int idOTC)
        {
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            var operario = $"{usu.Nombres} {usu.Apellidos}";
            var result = new OTC_N().RegularizarAutorizacion(docNum, idOTC, operario);
            return Json(new { Mensaje = result });
        }
        public JsonResult ObtenerDatosTicket(int docEntry)
        {
            try
            {
                ORTV_N ortvN = new ORTV_N();
                var result = ortvN.ObtenerDatosCompletosTicket(docEntry);
                return Json(new { Ticket = result });
            }
            catch (Exception e)
            {
                return Json(new { Titulo = e.Message });
            }
        }
        public JsonResult buscarTicketAVincular(int DocNum = 0)
        {
            ORTV_N ortvN = new ORTV_N();
            ORTV_E ticket = ortvN.ObtenerTicketVenta(DocNum);
            if (ticket != null)
            {
                List<RTV2_E> nuevoDet2 = new List<RTV2_E>();
                foreach (RTV2_E t in ticket.Det2)
                {
                    if (t.AlmacenSalida != "06" && t.AlmacenSalida != "09" && t.Monto > 0)
                    {
                        nuevoDet2.Add(t);
                    }
                }
                if (nuevoDet2.Count > 0)
                {
                    ticket.Det2 = nuevoDet2;
                }
                else { ticket.Det2 = null; }
            }
            return Json(ticket);
        }
        public ActionResult CreaTicketVenta(int DocEntry = 0, Usuario_E u = null, int idOperation = 502)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                UBIG_N ubigN = new UBIG_N(); OCRD_N oN = new OCRD_N(); OUR1_N ofiN = new OUR1_N();
                COUR_N couN = new COUR_N();
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                ViewBag.Mensaje = "";
                ViewBag.ProveedoresConContactos = oN.listarSociosConContactos();
                ViewBag.Ubigeos = ubigN.Listar(null);
                ViewBag.Oficinas = ofiN.Listar();
                ViewBag.Agencias = couN.Listar();
                ViewBag.Usuario = $"{user.Prefijo}{user.Id}";
                if (DocEntry > 0) { return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry)); }
                else
                {
                    //Si usuario entidad llega con data al GET se entiende que el ticket esta siendo separado por un vendedor de reemplazo.
                    if (u != null && u.CodigoSap > 0 && !string.IsNullOrWhiteSpace(u.Nombres) && !string.IsNullOrWhiteSpace(u.Apellidos) && user.IdRol == 12)
                    {
                        return View(_ticketN.Separar(u));
                    }
                    else
                    {
                        return View(_ticketN.Separar(user));
                    }
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult CreaTicketVenta(ORTV_E ticket, string SolicitudesReclamoSeleccionadas, int idOperation = 502)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    ticket.OpRegistro = $"{user.Nombres} {user.Apellidos}";
                    ticket.WhsCodeLog = $"{user.WhsCode}";
                    int DocNum = _ticketN.Registrar(ticket);
                    if (!string.IsNullOrEmpty(SolicitudesReclamoSeleccionadas))
                    {
                        var listaSolicitudes = SolicitudesReclamoSeleccionadas.Split(',').ToList();
                        new Capa_Negocio.AtencionCliente_NEG.TablasSql.OSAT_N().ActualizarTicketSolucion(listaSolicitudes, DocNum.ToString());
                    }
                    return RedirectToAction("ListadoTicketsVenta", new { DocNum = DocNum });
                }
                catch (Exception e)
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    UBIG_N ubigN = new UBIG_N(); OCRD_N oN = new OCRD_N(); OUR1_N ofiN = new OUR1_N();
                    COUR_N couN = new COUR_N();
                    ViewBag.Mensaje = e.Message;
                    ViewBag.ProveedoresConContactos = oN.listarSociosConContactos();
                    ViewBag.Agencias = couN.Listar();
                    ViewBag.Ubigeos = ubigN.Listar(null);
                    ViewBag.Oficinas = ofiN.Listar();
                    ViewBag.Usuario = $"{user.Prefijo}{user.Id}";
                    return View(ticket);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EditarTicketVenta(int DocEntry, int idOperation = 503)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                UBIG_N ubigN = new UBIG_N(); OUR1_N ofiN = new OUR1_N(); OCRD_N oN = new OCRD_N();
                OCLR_N oclrN = new OCLR_N(); COUR_N couN = new COUR_N();
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                ViewBag.ProveedoresConContactos = oN.listarSociosConContactos();
                ORTV_E t = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                ViewBag.Mensaje = "";
                if (t.EstadoPago != null)
                {
                    if (t.EstadoPago.Equals("PAGADO"))
                    {
                        ViewBag.Mensaje = "El ticket se encuentra PAGADO, anular pago para Editar Ticket";
                    }
                }
                ViewBag.ClienteRegalo = oclrN.buscarClienteRegalo(t.CardCode);
                ViewBag.Ubigeos = ubigN.Listar(null);
                ViewBag.Oficinas = ofiN.Listar();
                ViewBag.Agencias = couN.Listar();
                ViewBag.IdRol = user.IdRol;
                ViewBag.Usuario = $"{user.Prefijo}{user.Id}";
                // --- INICIO: OBTENER RECLAMOS CON TICKETSOLUCION ---
                var osatN = new Capa_Negocio.AtencionCliente_NEG.TablasSql.OSAT_N();
                var filtro = new Capa_Entidad.AtencionCliente_ENT.TablasSql.OSAT_E
                {
                    DetORTV = new Dictionary<string, string> { { "CardCode", t.CardCode } },
                    TicketSolucion = t.DocNum.ToString()
                };
                var reclamos = osatN.ListarSolicitudes(filtro, false, false);
                // Filtrar los que tienen TicketSolucion asignado (no null ni vacío)
                var reclamosAplicados = reclamos
                .Where(x => !string.IsNullOrEmpty(x.TicketSolucion) && x.TicketSolucion == t.DocNum.ToString())
                .Select(x => x.DocNum.ToString())
                .ToList();
                ViewBag.ReclamosAplicados = string.Join(",", reclamosAplicados);

                // --- FIN: OBTENER RECLAMOS CON TICKETSOLUCION ---
                if (t.Estado.Equals("SEPARADO")) { return RedirectToAction("CreaTicketVenta", new { DocEntry = t.DocEntry }); }
                else { return View(t); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult EditarTicketVenta(int DocEntry, ORTV_E t, string SolicitudesReclamoSeleccionadas, int idOperation = 503)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                try
                {
                    t.Vendedor = $"{user.Nombres} {user.Apellidos}";     // Seteamos el usuario Propietario con el nombre del usuario en sesiòn
                    t.OpRegistro = $"{user.Nombres} {user.Apellidos}";     // Seteamos el valor de OpRegistro para grabarlo en la transaccion de regalo si lo tuviera.
                    t.WhsCodeLog = $"{user.WhsCode}";
                    _ticketN.Editar(DocEntry, t);
                    int DocNum = t.DocNum;
                    // --- Actualización de OSAT ---
                    if (!string.IsNullOrEmpty(SolicitudesReclamoSeleccionadas))
                    {
                        var listaSolicitudes = SolicitudesReclamoSeleccionadas.Split(',').ToList();
                        new Capa_Negocio.AtencionCliente_NEG.TablasSql.OSAT_N().ActualizarTicketSolucion(listaSolicitudes, DocNum.ToString());
                    }
                    else
                    {
                        new Capa_Negocio.AtencionCliente_NEG.TablasSql.OSAT_N().ActualizarTicketSolucion(new List<string>(), DocNum.ToString());
                    }
                    return RedirectToAction("ListadoTicketsVenta", new { DocNum = t.DocNum });
                }
                catch (Exception e)
                {
                    UBIG_N ubigN = new UBIG_N(); OCRD_N oN = new OCRD_N(); OCLR_N oclrN = new OCLR_N();
                    OUR1_N ofiN = new OUR1_N(); COUR_N couN = new COUR_N();
                    ViewBag.ProveedoresConContactos = oN.listarSociosConContactos();
                    ViewBag.Mensaje = e.Message;
                    ViewBag.Ubigeos = ubigN.Listar(null);
                    ViewBag.Oficinas = ofiN.Listar(); ViewBag.Agencias = couN.Listar();
                    ViewBag.ClienteRegalo = oclrN.buscarClienteRegalo(t.CardCode);
                    ViewBag.Usuario = $"{user.Prefijo}{user.Id}";
                    return View(t);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public void VerificarOpSeguimiento(Dictionary<string, Object> datos, string Request)
        {
            int Op = 0;
            if (datos["accion"].Equals("RECIBIDO")) { Op = 508; }
            if (datos["accion"].Equals("ANULARRECIBIDO")) { Op = 509; }
            if (datos["accion"].Equals("EMPACADO")) { Op = 512; }
            if (datos["accion"].Equals("ANULAREMPACADO")) { Op = 513; }
            if (datos["accion"].Equals("ENVIADO")) { Op = 514; }
            if (datos["accion"].Equals("ANULARENVIADO")) { Op = 515; }
            if (datos["accion"].Equals("ENTREGADO")) { Op = 516; }
            if (datos["accion"].Equals("ANULARENTREGADO")) { Op = 517; }
            //cambiar datos de NroMesa y Cajas
            if (datos["accion"].Equals("UPDATEEMP")) { Op = 599; }

            string acceso = AccesoHelper.VerificarAccesos(Op, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), "", "");
            if (acceso == "C_Access")
            {
                _ticketN.EditarTicketDesdeSeguimiento(datos, Request);
            }
            else
            {
                throw new Exception("Error Ud. no tiene permiso esta operacion.");
            }
        }
        public ActionResult SeguimientoDeTicket(int DocEntry, string Mensaje, int idOperation = 507)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ORRU_N orruN = new ORRU_N();
                try
                {
                    ViewBag.BtnNuevaSolicitud = false;
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    if (usu.IdRol == 1 || usu.IdRol == 11)
                    { ViewBag.BtnNuevaSolicitud = true; }
                    RTV6_N rtv6_N = new RTV6_N();
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    ticket.orru = orruN.obtenerOrdenDeRutaTicket(DocEntry);
                    ViewBag.flujoEstadosTicket = ccORTV_N.ListarCC_FlujoEstados(DocEntry);
                    //consulta referencia para los estados, acopla los nuevos datos sin perder lo anterior consultado.
                    _ticketN.ObtenerReferenciaEstadosTicket(ticket);
                    /**************peso total******************/
                    if (ticket.Det6 != null && ticket.Det6.Count >= 1)
                    { ViewBag.pesoTotal = rtv6_N.ObtenerPesoTotal(DocEntry); }
                    ViewBag.Mensaje = Mensaje;
                    ViewBag.NameBotonEstado = "";
                    ViewBag.ValueBotonEstado = "";
                    ViewBag.MostrarBotonCambiarEstado = false;
                    ViewBag.BtnAnularRecibido = "";

                    if (ticket.Estado != null)
                    {
                        bool permisoAlm = false;
                        if (usu.IdRol.Equals(1) || usu.IdRol.Equals(4) || usu.IdRol.Equals(6) || usu.IdRol.Equals(50) || usu.IdRol.Equals(51))
                        {   // MANAGER, SALM, SVENTAS, PIK,ALM
                            permisoAlm = true;
                        }
                        if (ticket.Estado.Equals("ABIERTO") && (usu.IdRol.Equals(1) || usu.IdRol.Equals(5)))
                        {   // MANAGER, RECEP
                            ViewBag.NameBotonEstado = "RECIBIDO";
                            ViewBag.ValueBotonEstado = "CAMBIAR A RECIBIDO";
                            ViewBag.MostrarBotonCambiarEstado = true;
                        }
                        else if (ticket.Estado.Equals("RECIBIDO") && permisoAlm == true)
                        {
                            ViewBag.BtnAnularRecibido = "<input class='btn btn-sm btn-danger my-1' type='submit' name='ANULARRECIBIDO' value='ANULAR RECIBIDO' />";
                        }
                    }
                    ViewBag.IdRol = usu.IdRol;
                    ViewBag.permisoCajas = new OUSR_OPE_N().VerificarAccesoOperacion(new OUSR_OPE_E { UsrDocEntry = usu.DocEntry, OpeID = 2024 });
                    return View(ticket);
                }
                catch
                {
                    return RedirectToAction("ListadoTicketsVenta");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult SeguimientoDeTicket(int DocEntry, ORTV_E t, int idOperation = 507)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ORRU_N orruN = new ORRU_N();
                try
                {
                    ViewBag.Mensaje = string.Empty;
                    RTV6_N rtv6_N = new RTV6_N();
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    tc.orru = orruN.obtenerOrdenDeRutaTicket(DocEntry);


                    // Creamos la estructura de parámetros para el método
                    Dictionary<string, Object> datos = new Dictionary<string, Object>()
                    {
                        ["docEntryTicket"] = DocEntry,
                        ["docNumTicket"] = tc.DocNum,
                        ["estadoTicket"] = tc.Estado,
                        ["opRegistro"] = $"{usu.Nombres} {usu.Apellidos}"
                    };
                    if (Request.Form["RECIBIDO"] != null)
                    {
                        try
                        {
                            datos.Add("accion", "RECIBIDO");
                            datos.Add("tipoMantenimiento", "USRE");
                            VerificarOpSeguimiento(datos, Request.Form["RECIBIDO"]);
                            ViewBag.Mensaje = "Ticket recibido correctamente";
                        }
                        catch (Exception e) { ViewBag.Mensaje = e.Message; }
                    }
                    else if (Request.Form["ANULARRECIBIDO"] != null)
                    {
                        try
                        {
                            datos.Add("accion", "ANULARRECIBIDO");
                            datos.Add("tipoMantenimiento", "USAR");
                            VerificarOpSeguimiento(datos, Request.Form["ANULARRECIBIDO"]);
                            ViewBag.Mensaje = "Ha anulado Recibido el ticket, se regreso al estado anterior";
                        }
                        catch (Exception e) { ViewBag.Mensaje = e.Message; }
                    }
                    else if (Request.Form["UPDATEPROD"] != null)
                    {
                        _ticketN.EditarProductosPendientesTicket(DocEntry);
                    }
                    else if (Request.Form["UPDATEEMP"] != null)
                    {
                        try
                        {
                            datos.Add("accion", "UPDATEEMP");
                            datos.Add("tipoMantenimiento", "UDEMP");
                            datos.Add("NroMesa", t.NroMesaNuevo);
                            datos.Add("Cajas", t.CajasNuevo);
                            VerificarOpSeguimiento(datos, Request.Form["UPDATEEMP"]);
                            ViewBag.Mensaje = "Se envio los datos correctamente";
                        }
                        catch (Exception e) { ViewBag.Mensaje = e.Message; }
                    }
                    else if (Request.Form["ENTREGADO"] != null)
                    {
                        try
                        {
                            datos.Add("accion", "ENTREGADO");
                            datos.Add("tipoMantenimiento", "USET");
                            VerificarOpSeguimiento(datos, Request.Form["ENTREGADO"]);
                            ViewBag.Mensaje = "En estado Entregado el ticket correctamente";
                        }
                        catch (Exception e) { ViewBag.Mensaje = e.Message; }
                    }
                    else if (Request.Form["ANULARENTREGADO"] != null)
                    {
                        try
                        {
                            datos.Add("accion", "ANULARENTREGADO");
                            datos.Add("tipoMantenimiento", "USAT");
                            datos.Add("LugarDestino", tc.LugarDestino);
                            VerificarOpSeguimiento(datos, Request.Form["ANULARENTREGADO"]);
                            ViewBag.Mensaje = "Ha anuladoEntregado el ticket, se regreso al estado anterior";
                        }
                        catch (Exception e) { ViewBag.Mensaje = e.Message; }
                    }
                    t = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    t.orru = orruN.obtenerOrdenDeRutaTicket(DocEntry);
                    ViewBag.flujoEstadosTicket = ccORTV_N.ListarCC_FlujoEstados(DocEntry);
                    ViewBag.ultimoEstadoTicket = ccORTV_N.UltimoEstadoCC_ORTV(DocEntry);
                    ViewBag.pesoTotal = rtv6_N.ObtenerPesoTotal(DocEntry);
                    ViewBag.IdRol = usu.IdRol;
                    return RedirectToAction("SeguimientoDeTicket", new { DocEntry = DocEntry, Mensaje = ViewBag.Mensaje });
                }
                catch
                {
                    return RedirectToAction("ListadoTicketsVenta");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult CancelarTicket(int DocEntry, string vista, int idOperation = 518)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ORTV_N ortvN = new ORTV_N();
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    string Operario = $"{usu.Nombres} {usu.Apellidos}";
                    int DocNum = ortvN.Cancelar(DocEntry, Operario, usu.IdRol);
                    var osatN = new Capa_Negocio.AtencionCliente_NEG.TablasSql.OSAT_N();
                    osatN.ActualizarTicketSolucion(new List<string>(), DocNum.ToString());

                    return RedirectToAction(vista, new { DocNum });
                }
                catch (Exception e)
                {
                    return RedirectToAction(vista, new { Mensaje = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        /*************************** P E D I D O S  O N L I N E ***************************/
        [HttpGet]
        public ActionResult PedidosOnline(Capa_Entidad.Ventas_ENT.TablasSql.ORDR_E datos, int idOperation = 1326)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N pedidoN = new Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N();
                Capa_Negocio.Almacen_NEG.TablasSql.OIBT_N oibtN = new Capa_Negocio.Almacen_NEG.TablasSql.OIBT_N();
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                ViewBag.RolUsuario = usu.IdRol;
                oibtN.TemporizarMigrarArticulos();
                // 7: Op Ventas
                if (usu.IdRol.Equals(7))
                {
                    datos.Vendedor = $"{usu.Nombres} {usu.Apellidos}";
                }
                return View(pedidoN.ListarPedidosOnline(datos));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public JsonResult BuscarCliente(OCRD_E cliente)
        {
            if (!cliente.CardName.Equals(""))
            {
                Capa_Negocio.SocioNegocios_NEG.TablasSql.OCRD_N ocrdN = new Capa_Negocio.SocioNegocios_NEG.TablasSql.OCRD_N();
                var datalist = "<datalist id='ListaClientes'>";
                var listaClientes = ocrdN.BuscarCliente(cliente);
                if (listaClientes.Count >= 1)
                {
                    foreach (var c in listaClientes)
                    {
                        datalist += $"<option CardCode='{c.CardCode}' value='{c.CardName}'></option>";
                    }
                }
                datalist += "</datalist>";
                return Json(datalist);
            }
            else
            {
                return null;
            }
        }
        public JsonResult MigrarArticulos()
        {
            Capa_Negocio.Almacen_NEG.TablasSql.OIBT_N oibtN = new Capa_Negocio.Almacen_NEG.TablasSql.OIBT_N();
            oibtN.MigrarArticulos();
            return Json("Migración realizada");
        }
        public JsonResult VerificarMigracionArticulos()
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            Capa_Negocio.Almacen_NEG.TablasSql.OIBT_N oibtN = new Capa_Negocio.Almacen_NEG.TablasSql.OIBT_N();
            var result = oibtN.VerificarMigracionArticulos();
            return Json(result);
        }
        public JsonResult BuscarArticulo(Capa_Entidad.Almacen_ENT.TablasSql.OIBT_E articulo)
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            if (!string.IsNullOrWhiteSpace(articulo.ItemName) || !string.IsNullOrWhiteSpace(articulo.PrincActivo))
            {
                Capa_Negocio.Almacen_NEG.TablasSql.OIBT_N oibtN = new Capa_Negocio.Almacen_NEG.TablasSql.OIBT_N();
                var tbody = string.Empty;
                var listaArticulos = oibtN.BuscarArticulo(articulo);
                foreach (var art in listaArticulos)
                {
                    tbody += "<tr>" +
                                    $"<td><button type=\"button\" class=\"btn btn-warning\" onclick=\"agregarDetallePedido('{art.ItemCode}', '{art.ItemName}', '{art.NumInBuy}', '{art.BuyUnitMsr}', '{art.SalUnitMsr}', '{art.BatchNum}', '{art.PorVender}', '{art.ExpDate}', {art.Price}, {art.Id});\"><i class='icon-plus'></i> </button></td>" +
                                    $"<td><span class='text-danger font-weight-bold'>{art.ItemCode}</span></br>{art.ItemName}</td>" +
                                    $"<td class='text-center'>{art.Price}</td>" +
                                    $"<td class='text-center'>{art.PrecioxCaja}</td>" +
                                    $"<td class='text-center'>{art.PorVender}</td>" +
                                    $"<td class='text-center'>{art.ExpDate}</td>" +
                                    $"<td class='text-center'>{art.PrincActivo}</td>" +
                                    $"<td class='text-center'>{art.Observacion}</td>" +
                                    $"<td class='text-center'>{art.CajonM}</td>" +
                                    //$"<td><div class=\"d-flex justify-content-center\"><select id='undmed_{art.Id}' class='form-control bg-cobefar text-white border-sucess' style='width: 100px'><option value=''>Selec.</option><option value='PZA'>PZA</option><option value='CAJA'>CAJA</option></select></div></td>" +
                                    //$"<td class='text-center'><input type='hidden' id='stock_disponible_{art.Id}' value='{Convert.ToInt32(art.Quantity)}' /> {Convert.ToInt32(art.Quantity)}</td>" +
                                    //$"<td><input type='number' id='cantidad_{art.Id}' class='form-control border-success' /></td>" +
                                    "<tr>";
                }
                return Json(tbody);
            }
            else
            {
                return null;
            }
        }
        public JsonResult VerificarCliente(int DocEntry, string CardCode)
        {
            // verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            ORTV_N ortvN = new ORTV_N();
            var result = ortvN.ObtenerDatosCompletosTicket(DocEntry);
            string msj = string.Empty;
            if (CardCode != result.CardCode)
            {
                msj = "El cliente no debe ser distinto del creado en PedidosOnline";
            }
            return Json(msj);
        }
        [HttpGet]
        public ActionResult CrearPedidoOnline(Capa_Entidad.Ventas_ENT.TablasSql.ORDR_E Pedido, int idOperation = 1327)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View(Pedido);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult CrearPedidoOnline(Capa_Entidad.Ventas_ENT.TablasSql.ORDR_E Pedido, List<Capa_Entidad.Almacen_ENT.TablasSql.OIBT_E> DetallePedido, int idOperation = 1327)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N ordrN = new Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N();
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                try
                {
                    Pedido.CodSapVendedor = usu.CodigoSap;
                    Pedido.Vendedor = $"{usu.Nombres} {usu.Apellidos}";
                    ordrN.RegistrarPedidoOnline(Pedido, DetallePedido);
                    return RedirectToAction("PedidosOnline", new { Id = Pedido.Id });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return View(Pedido);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpGet]
        public ActionResult VerPedidoOnline(Capa_Entidad.Ventas_ENT.TablasSql.ORDR_E pedido, int idOperation = 1327)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N ordrN = new Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N();
                List<Capa_Entidad.Ventas_ENT.TablasSql.ORDR_E> datos = ordrN.ListarPedidosOnline(pedido);
                return View(datos);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult EditarPedidoOnline(int idORDR, List<Capa_Entidad.Almacen_ENT.TablasSql.OIBT_E> DetallePedido, int idOperation = 1327)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N ordrN = new Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N();
                try
                {
                    ordrN.EditarPedidoOnline(idORDR, DetallePedido);
                    return RedirectToAction("PedidosOnline");
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return RedirectToAction("EditarPedidoOnline");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        protected string CargarListaPedidosOnline()
        {
            Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N pedidoN = new Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N();
            var listaPedidosOnline = pedidoN.ListarPedidosOnline(null);
            string lista = string.Empty;
            foreach (var ped in listaPedidosOnline)
            {
                string btnCancelado = "btn-danger";
                string btnRecibido = "btn-success";
                string btnTicketVenta = "btn-secondary disabled";
                string disabled = string.Empty;
                string disabledRecibido = string.Empty;
                string btnUrlTicket = "#";
                if (ped.Estado.Equals("CANCELADO") || ped.Estado.Equals("BORRADOR"))
                {
                    disabled = "disabled"; disabledRecibido = "disabled";
                    btnCancelado = "btn-secondary"; btnRecibido = "btn-secondary";
                }
                else if (ped.Estado.Equals("RECIBIDO"))
                {
                    btnRecibido = "btn-secondary disabled";
                    btnTicketVenta = "btn-warning";
                    btnUrlTicket = $"/Ventas/CreaTicketVenta?DocEntry={ped.DocEntryTicket}&Tipo=PedidoOnline";
                    disabledRecibido = "disabled";
                }
                lista += "<tr>" +
                                $"<td class=\"text-center\">{ped.CardCode}</td>" +
                                $"<td class=\"text -center\">{ped.CardName}</td>" +
                                $"<td class=\"text-center\" >{ped.FechaCreacion}</td>" +
                                $"<td class=\"text-center\">{ped.Vendedor}</td>" +
                                $"<td class=\"text-center\">{ped.Estado}</td>" +
                                $"<td class=\"text-center\">{ped.DocNumTicket}</td>" +
                                "<td class=\"text-center\">" +
                                    $"<a href =\"/Ventas/VerPedidoOnline?Id={ped.Id}\" class=\"btn btn-sm btn-info mx-1 my-lg-0 my-sm-1\" title=\"Recibir Pedido\">" +
                                        "<i class=\"icon-eye\" title=\"Ver Pedido\"></i>" +
                                    "</a>" +
                                    $"<button class=\"btn btn-sm {btnRecibido} mx-1 my-lg-0 my-sm-1\" title=\"Recibir Pedido\" onclick=\"enviarDatosPedido({ped.Id}, {ped.DocEntryTicket}, 'REC')\" {disabledRecibido}>" +
                                        "<i class=\"icon icon-checkmark\" title=\"Recibir Pedido\"></i>" +
                                    "</button>" +
                                    $"<button type='button' class=\"btn btn-sm {btnCancelado} mx-1 my-lg-0 my-sm-1\" title=\"Cancelar Pedido\" onclick=\"enviarDatosPedido({ped.Id}, {ped.DocEntryTicket}, 'CAN')\" {disabled}>" +
                                        "<i class=\"icon icon-blocked\" title=\"Cancelar Pedido\"></i>" +
                                    "</button>" +
                                "</td>" +
                                "<td class=\"text-center\">" +
                                    $"<a href=\"/Ventas/ExportarDetallePedidoOnline?Id={ped.Id}&CardCode={ped.CardCode}\" class=\"btn btn-sm btn-cobefar font-weight-bold my-lg-0 my-sm-1\" title=\"Descargar Excel Detalle Pedido\"><i class=\"icon icon-file-excel\"></i> Detalle Pedido</a>" +
                                    $"<a href=\"{btnUrlTicket}\" class=\"btn btn-sm {btnTicketVenta} my-lg-0 my-sm-1\" title=\"Ticket Venta\"><i class=\"icon-redo2\"></i> Ticket Venta</a>" +
                                "</td>" +
                            "</tr>";
            }
            return lista;
        }
        public JsonResult CambiarEstadoPedidoOnline(Capa_Entidad.Ventas_ENT.TablasSql.ORDR_E Pedido, string Accion)
        {
            if (Pedido.Id >= 1)
            {
                Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N pedidoN = new Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N();
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                if (Accion.Equals("REC"))
                {
                    Pedido.VendedorRecibido = $"{usu.Nombres} {usu.Apellidos}";
                }
                else if (Accion.Equals("CAN"))
                {
                    Pedido.VendedorCancelado = $"{usu.Nombres} {usu.Apellidos}";
                }
                var result = pedidoN.CambiarEstadoPedidoOnline(Pedido, Accion);
                Dictionary<string, string> data = new Dictionary<string, string>{
                    { "Lista", CargarListaPedidosOnline() }, { "Mensaje", result },
                };
                return Json(data);
            }
            else
            {
                return null;
            }
        }
        public ActionResult ExportarDetallePedidoOnline(Capa_Entidad.Ventas_ENT.TablasSql.ORDR_E Pedido, int idOperation = 1327)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                RDR1_N detPedidoN = new RDR1_N();
                string nombreArchivo = $"DetallePedido_{Pedido.CardCode}.xlsx";
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var solicitudes = detPedidoN.ExportarDetallePedidosOnline(Pedido.Id);
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("DetallePedido");
                    worksheet.Cells["A1"].LoadFromCollection(solicitudes, PrintHeaders: true);
                    if (solicitudes != null)
                    {
                        if (solicitudes.Count >= 1)
                        {
                            for (var col = 1; col <= 4; col++)
                            {
                                worksheet.Column(col).AutoFit();
                            }
                            var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: solicitudes.Count + 1, toColumn: 4), "DetallePedido");
                            tabla.ShowHeader = true;
                            tabla.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;
                        }
                    }
                    return File(libro.GetAsByteArray(), excelContentType, nombreArchivo);
                }
            }
            else { return null; }
        }
        public ActionResult ListadoTicketsFacturacion(int DocNum = 0, ORTV_E ticket = null, string Mensaje = "", int idOperation = 601, int SoloConObservacion = 0)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                ORTV_N tkN = new ORTV_N();
                ViewBag.IdRol = user.IdRol;
                ViewBag.DocNum = DocNum;
                //Si el filtro DocNum es diferente a 0 todos los datos necesarios del ticket se llenan en ViewBag.Ortv (para que muestre en el filtro)
                if (DocNum > 0)
                {
                    DocNum = tkN.DocNumTicketLike(DocNum);
                    var DocEntry = tkN.DocEntryTicket(DocNum);
                    var ticketUnico = tkN.ObtenerTicketFacturacion(DocEntry);
                    ticket.LugarDestino = ticketUnico.LugarDestino;
                    ticket.EstadoFacturacion = ticketUnico.EstadoFacturacion;
                    ticket.Estado = ticketUnico.Estado;
                    ticket.DocNum = DocNum;
                    ViewBag.Ortv = ticket;
                    ViewBag.DocNum = DocNum;
                }
                else { ViewBag.Ortv = ticket; }
                ViewBag.Mensaje = Mensaje;
                var lista = tkN.ListarTicketsAreaFacturacion(user, ticket, SoloConObservacion);
                ViewBag.CP = _ticketN.CantidadTicketsFacturacion("PENDIENTE");
                ViewBag.CG = _ticketN.CantidadTicketsFacturacion("GRE EMITIDA");
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EmitirGuiasTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 2802)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ORTV_E datos = new ORTV_E
                {
                    EstadoPago = ticketPost.EstadoPago,
                    TipoVenta = ticketPost.TipoVenta,
                    TiempoEntrega = ticketPost.TiempoEntrega,
                    Estado = ticketPost.Estado,
                    FechaSapTicket = ticketPost.FechaSapTicket,
                    CardName = ticketPost.CardName,
                    Vendedor = ticketPost.Vendedor,
                    Flete = ticketPost.Flete,
                    DescuentoNC = ticketPost.DescuentoNC,
                    EstadoFacturacion = ticketPost.EstadoFacturacion,
                    LugarDestino = ticketPost.LugarDestino,
                    Zona = ticketPost.Zona,
                    zonaDistinta = ticketPost.zonaDistinta,
                    Mensaje = string.Empty
                };
                bool hayFinVerificar = false;
                int DocNum = 0;
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    List<CC_ORTV_E> ticketFinVerificar = ccORTV_N.ListarCC_ORTV(DocEntry, "FIN VERIFICAR");
                    List<CC_ORTV_E> ticketAnularFinVerificar = ccORTV_N.ListarCC_ORTV(DocEntry, "ANULAR FIN VERIFICAR");
                    List<CC_ORTV_E> listaCC = new List<CC_ORTV_E>() { ticketFinVerificar[0], ticketAnularFinVerificar[0] }.OrderByDescending(x => x.Id).ToList();
                    if (listaCC.FirstOrDefault().Operacion == "FIN VERIFICAR") { hayFinVerificar = true; }

                    if (hayFinVerificar)
                    {
                        ORTV_N negtik = new ORTV_N();
                        ORTV_E ticket = negtik.ObtenerDatosCompletosTicket(DocEntry);
                        string Guias = "";
                        if (ticket.LugarDestino.Equals("Arriola") || ticket.LugarDestino.Equals("Centro"))
                        {
                            string WhsCode = string.Empty;
                            Capa_Negocio.Almacen_NEG.Tablas.OWTR_N owtrN = new Capa_Negocio.Almacen_NEG.Tablas.OWTR_N();
                            if (ticket.LugarDestino.Equals("Centro")) { WhsCode = "01"; }
                            else if (ticket.LugarDestino.Equals("Arriola")) { WhsCode = "09"; }
                            Guias = owtrN.GuiasTicketTransferencia(ticket.DocNum, WhsCode, ticket.CardCode);
                        }
                        else
                        {
                            Guias = _ticketN.GuiasTicket(DocEntry);
                        }
                        //verificamos guias existentes desde SAP
                        if (!string.IsNullOrWhiteSpace(Guias) && Guias.Length > 6)
                        {
                            //pasa EstadoFacturacion a GRE EMITIDA
                            DocNum = _ticketN.emitirGuia(DocEntry, u);
                        }
                        else { throw new Exception("El ticket " + DocEntry + " no tiene guías en SAP."); }
                    }
                    else { throw new Exception("El ticket " + DocEntry + " no ha sido verificado."); }
                    return RedirectToAction("ListadoTicketsFacturacion", datos);
                }
                catch (Exception e)
                {
                    datos.Mensaje = e.Message;
                    return RedirectToAction("ListadoTicketsFacturacion", datos);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RevertirEmitirGuiasTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 2803)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ORTV_E datos = new ORTV_E
                {
                    EstadoPago = ticketPost.EstadoPago,
                    TipoVenta = ticketPost.TipoVenta,
                    TiempoEntrega = ticketPost.TiempoEntrega,
                    Estado = ticketPost.Estado,
                    FechaSapTicket = ticketPost.FechaSapTicket,
                    CardName = ticketPost.CardName,
                    Vendedor = ticketPost.Vendedor,
                    Flete = ticketPost.Flete,
                    DescuentoNC = ticketPost.DescuentoNC,
                    EstadoFacturacion = ticketPost.EstadoFacturacion,
                    LugarDestino = ticketPost.LugarDestino,
                    Zona = ticketPost.Zona,
                    zonaDistinta = ticketPost.zonaDistinta,
                    Mensaje = string.Empty
                };
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    String operario = $"{usu.Nombres} {usu.Apellidos}";
                    int DocNum = _ticketN.revertirGuiasTicket(DocEntry, operario);
                    return RedirectToAction("ListadoTicketsFacturacion", datos);
                }
                catch (Exception e)
                {
                    datos.Mensaje = e.Message;
                    return RedirectToAction("ListadoTicketsFacturacion", datos);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult FacturarTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 602)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ORTV_E datos = new ORTV_E
                {
                    EstadoPago = ticketPost.EstadoPago,
                    TipoVenta = ticketPost.TipoVenta,
                    TiempoEntrega = ticketPost.TiempoEntrega,
                    Estado = ticketPost.Estado,
                    FechaSapTicket = ticketPost.FechaSapTicket,
                    CardName = ticketPost.CardName,
                    Vendedor = ticketPost.Vendedor,
                    Flete = ticketPost.Flete,
                    DescuentoNC = ticketPost.DescuentoNC,
                    EstadoFacturacion = ticketPost.EstadoFacturacion,
                    LugarDestino = ticketPost.LugarDestino,
                    Zona = ticketPost.Zona,
                    zonaDistinta = ticketPost.zonaDistinta,
                    Mensaje = string.Empty
                };
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    int DocNum = _ticketN.facturarTicket(DocEntry, u);
                    // Envío de correo automático en caso de clientes específicos
                    var ticketFacturado = new ORTV_N().ObtenerDatosTicketParaDocumentos(DocEntry);
                    var correosClientes = new Dictionary<string, string>
                    {
                        { "C20609641500", "Juancarloshuapayalazarte@gmail.com" },
                        { "C20557398628", "distribuidoravgfarma@hotmail.com" },
                        { "C20600765044", "eucelsrl@gmail.com" }
                    };
                    // Verificar si el CardCode existe en el diccionario
                    if (DocNum > 0 && correosClientes.TryGetValue(ticketFacturado.CardCode, out string correoCliente))
                    {
                        EnviarCorreo(ticketFacturado.DocEntry, correoCliente);
                    }
                    return RedirectToAction("ListadoTicketsFacturacion", datos);
                }
                catch (Exception e)
                {
                    datos.Mensaje = e.Message;
                    return RedirectToAction("ListadoTicketsFacturacion", datos);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        /*******************************************************************************************************************************************************/
        //MODIFICAR POR SERVICES
        public void EnviarCorreo(int docEntry, string correoCliente)
        {
            Utilitarios uti = new Utilitarios();

            string destinatario = correoCliente;
            string remitente = "facturacion@cobefar.com.pe";
            string claveRemitente = "jnrfpqmjzbngkzrv"; // ⚠️ cámbiala por la real

            string asunto = "COBEFAR SAC - DOCUMENTOS ELECTRÓNICOS";
            string cuerpo = @"
                            <html>
                            <body>
                                <h3 style='color:green;'>Gracias por su compra - COBEFAR SAC</h3>
                                <p style='font-size:16px;font-weight:bold'>
                                    Estimado cliente,<br>
                                    Adjuntamos sus comprobantes electrónicos.
                                </p>
                                <span>Área Comercial - COBEFAR SAC</span>
                            </body>
                            </html>";

            // Crear mensaje
            MailMessage ms = new MailMessage();
            ms.From = new MailAddress(remitente, "COBEFAR SAC");
            ms.To.Add(destinatario);
            ms.Subject = asunto;
            ms.Body = cuerpo;
            ms.IsBodyHtml = true;

            // Generar y adjuntar PDF
            string filePath = CrearYObtenerDocumento(docEntry, "F");

            try
            {
                if (!string.IsNullOrWhiteSpace(filePath) && System.IO.File.Exists(filePath))
                {
                    ms.Attachments.Add(new Attachment(filePath));
                }
                else
                {
                    throw new FileNotFoundException("No se encontró el archivo PDF para adjuntar.");
                }

                // 🔹 SOLO aquí se usa Office 365
                using (SmtpClient smtp = new SmtpClient("smtp.office365.com", 587))
                {
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(remitente, claveRemitente);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Requerido por Microsoft (TLS 1.2)
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    smtp.Send(ms);
                    Console.WriteLine("✅ Correo enviado exitosamente a " + destinatario);
                }
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"❌ Error SMTP: {ex.StatusCode} - {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error general: {ex.Message}");
            }
            finally
            {
                // Eliminar PDF temporal
                if (!string.IsNullOrWhiteSpace(filePath) && System.IO.File.Exists(filePath))
                {
                    try
                    {
                        System.IO.File.Delete(filePath); 
                        Console.WriteLine("Archivo PDF eliminado después del envío del correo.");
                    }
                    catch (Exception deleteEx)
                    {
                        Console.WriteLine($"Error al eliminar el archivo PDF: {deleteEx.Message}");
                    }
                }
            }
        }


        private List<Comprobante_E> ObtenerEncabezados(List<int> listDocEntrySap, ORTV_E obj, string Tipo)
        {
            Comprobante_N compN = new Comprobante_N();
            List<Comprobante_E> documentos = new List<Comprobante_E>();
            // Obtener los documentos basados en el tipo proporcionado
            switch (Tipo)
            {
                case "F":
                    foreach (var docEntryOrden in listDocEntrySap)
                    {
                        documentos.AddRange(compN.ObtenerEncabezadoFacturas(docEntryOrden, obj.LugarDestino));
                    }
                    break;
                case "G":
                    if (obj.LugarDestino.Equals("Domicilio") || obj.LugarDestino.Equals("Agencia"))
                    {
                        documentos.AddRange(compN.ObtenerEncabezadoGuiasPorEntrega(listDocEntrySap));
                    }
                    else
                    {
                        documentos.AddRange(compN.ObtenerEncabezadoGuiasTransferencia(obj));
                    }
                    break;
                case "NC":
                    List<Comprobante_E> Facturas = new List<Comprobante_E>();
                    foreach (var docEntryOrden in listDocEntrySap)
                    {
                        Facturas.Add(compN.ObtenerEncabezadoFacturas(docEntryOrden, obj.LugarDestino).FirstOrDefault());
                    }
                    string FacturasConcatenadas = string.Join(", ", Facturas.Select(x => $"'{x.U_SYP_MDTD}-{x.U_SYP_MDSD}-{x.U_SYP_MDCD}'"));
                    documentos.AddRange(compN.ObtenerEncabezadoNotaCredito(obj.Det4, FacturasConcatenadas));
                    break;
                case "ND":
                    List<Comprobante_E> FacturasParaNotaDébito = new List<Comprobante_E>();
                    foreach (var docEntryOrden in listDocEntrySap)
                    {
                        FacturasParaNotaDébito.Add(compN.ObtenerEncabezadoFacturas(docEntryOrden, obj.LugarDestino).FirstOrDefault());
                    }
                    string FacturasConcatenadasParaNotaDébito = string.Join(", ", FacturasParaNotaDébito.Select(x => $"'{x.U_SYP_MDTD}-{x.U_SYP_MDSD}-{x.U_SYP_MDCD}'"));
                    documentos.AddRange(compN.ObtenerEncabezadoNotaDebito(FacturasConcatenadasParaNotaDébito));
                    break;
            }
            // Filtrar documentos con U_SYP_MDCD no vacío y eliminar duplicados
            var documentosFiltrados = documentos
                .Where(d => !string.IsNullOrWhiteSpace(d.U_SYP_MDCD))
                .GroupBy(d => d.U_SYP_MDCD)
                .Select(g => g.First())
                .ToList();
            return documentosFiltrados;
        }
        public string CrearYObtenerDocumento(int DocEntry, string Tipo)
        {
            Utilitarios uti = new Utilitarios();
            Comprobante_N compN = new Comprobante_N();
            ORTV_E ortvE = _ticketN.ObtenerDatosTicketParaDocumentos(DocEntry);
            List<int> listDocEntryOrdenesVenta = compN.ObtenerDocEntryOV(ortvE.Det2, false);
            if (ortvE.Estado.Equals("ANULADO") || ortvE.Estado.Equals("CANCELADO"))
            {
                throw new InvalidOperationException("Ticket en un estado no válido para la descarga de documentos.");
            }
            else if (ortvE.Det2 == null || ortvE.Det2.Count == 0 || listDocEntryOrdenesVenta == null || listDocEntryOrdenesVenta.Count == 0)
            {
                throw new InvalidOperationException("No se encontraron órdenes SAP activas.");
            }
            List<Comprobante_E> documentos = ObtenerEncabezados(listDocEntryOrdenesVenta, ortvE, Tipo);
            string fileName = string.Empty;
            switch (Tipo)
            {
                case "F": fileName = $"Facturas_{ortvE.DocNum}.pdf"; break;
                case "ND": fileName = $"NotasDebito_{ortvE.DocNum}.pdf"; break;
                case "NC": fileName = $"NotasCredito_{ortvE.DocNum}.pdf"; break;
                case "G": fileName = $"Guias_{ortvE.DocNum}.pdf"; break;
                default: throw new InvalidOperationException("Tipo del documento no reconocido.");
            }
            GeneracionDocumentoPDF(documentos, ortvE.DocNum, Tipo, fileName);
            string filePath = Path.Combine(uti.directorioFileServer, "Comprobantes", fileName);
            return filePath;
        }
        private void GeneracionDocumentoPDF(List<Comprobante_E> documentosDistinct, int DocNum, string Tipo, string fileName)
        {
            Utilitarios uti = new Utilitarios();
            //agrupa todos los documentos del mismo tipo en un solo pdf
            string filePath = Path.Combine(uti.directorioFileServer, "Comprobantes", fileName);
            using (MemoryStream combinedPdfStream = new MemoryStream())
            {
                using (Document document = new Document())
                {
                    PdfCopy copy = new PdfCopy(document, combinedPdfStream);
                    document.Open();
                    foreach (var f in documentosDistinct)
                    {
                        AgruparPdfSegunTipo(f, DocNum, copy, Tipo);
                    }
                    document.Close();
                }
                System.IO.File.WriteAllBytes(filePath, combinedPdfStream.ToArray());
            }
        }
        private void AgruparPdfSegunTipo(Comprobante_E documento, int docNum, PdfCopy copy, string Tipo)
        {
            var pdfResult = new ActionAsPdf(null);
            string NumAtCard = $"{documento.U_SYP_MDTD}-{documento.U_SYP_MDSD}-{documento.U_SYP_MDCD}";
            string fileName = $"{documento.U_SYP_MDTD}_{documento.U_SYP_MDSD}_{documento.U_SYP_MDCD}.pdf";
            //contemplar un caso de layout por cada tipo de documentos:
            //Factura,
            //Boleta,
            //Guia,
            //Nota credito,
            //Nota debito 
            switch (Tipo)
            {
                case "F":
                    var parametrosFactura = new
                    {
                        NumAtCard = NumAtCard,
                        Tipo = documento.U_SYP_MDTD.Equals("01") ? "F" : "B",
                        DocNumTicket = docNum
                    };
                    string _headerUrlFactura = Url.Action("LayoutFactura_header", "ComprobantesContables", parametrosFactura, "http");
                    pdfResult = new ActionAsPdf("LayoutFactura", new { NumAtCard = parametrosFactura.NumAtCard })
                    {
                        FileName = fileName,
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        CustomSwitches = "--header-html " + _headerUrlFactura + " --header-spacing 0 ",
                        PageSize = Rotativa.Options.Size.A4,
                        PageMargins = new Rotativa.Options.Margins(65, 10, 20, 10)
                    };
                    break;
                case "ND":
                case "NC":
                    var parametrosNotaCredito = new
                    {
                        NumAtCard = NumAtCard,
                        DocNumTicket = docNum
                    };
                    string _headerUrlNotaCredito = Url.Action("LayoutNotaCreditoDebito_header", "ComprobantesContables", parametrosNotaCredito, "http");
                    pdfResult = new ActionAsPdf("LayoutNotaCreditoDebito", new { NumAtCard = NumAtCard })
                    {
                        FileName = fileName,
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        CustomSwitches = "--header-html " + _headerUrlNotaCredito + " --header-spacing 0 ",
                        PageSize = Rotativa.Options.Size.A4,
                        PageMargins = new Rotativa.Options.Margins(65, 10, 20, 10)
                    };
                    break;
                case "G":
                    var parametrosGuia = new
                    {
                        NumAtCard = NumAtCard,
                        DocNumTicket = docNum,
                        Tabla = documento.TablaSAP
                    };
                    string _headerUrlGuia = Url.Action("LayoutGuia_header", "ComprobantesContables", parametrosGuia, "http");
                    pdfResult = new ActionAsPdf("LayoutGuia", parametrosGuia)
                    {
                        FileName = fileName,
                        PageOrientation = Rotativa.Options.Orientation.Portrait,
                        CustomSwitches = "--header-html " + _headerUrlGuia + " --header-spacing 0 ",
                        PageSize = Rotativa.Options.Size.A4,
                        PageMargins = new Rotativa.Options.Margins(70, 10, 20, 10)
                    };
                    break;
            }
            var pdfBytes = pdfResult.BuildFile(ControllerContext);
            using (var pdfStream = new MemoryStream(pdfBytes))
            {
                using (var pdfReader = new PdfReader(pdfStream))
                {
                    // Aplicar paginación al PDF antes de agregarlo al documento combinado
                    using (MemoryStream paginatedPdfStream = new MemoryStream())
                    {
                        using (PdfStamper stamper = new PdfStamper(pdfReader, paginatedPdfStream))
                        {
                            int totalPages = pdfReader.NumberOfPages;
                            for (int i = 1; i <= totalPages; i++)
                            {
                                PdfContentByte content = stamper.GetUnderContent(i);
                                iTextSharp.text.Font font = FontFactory.GetFont("Helvetica", BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
                                Phrase phrase = new Phrase($"Página {i} de {totalPages}", font);
                                ColumnText.ShowTextAligned(content, Element.ALIGN_CENTER, phrase, 300, 30, 0);
                            }
                        }
                        using (var paginatedPdfReader = new PdfReader(paginatedPdfStream.ToArray()))
                        {
                            // Agregar el PDF paginado al documento combinado
                            copy.AddDocument(paginatedPdfReader);
                        }
                    }
                }
            }
        }
        public ActionResult LayoutFactura(string NumAtCard)
        {
            var factura = ObtenerDetalleFactura(NumAtCard);
            return View(factura);
        }
        private List<ComprobanteDePago_E> ObtenerDetalleFactura(string numAtCard)
        {
            return new Comprobante_N().ObtenerDetalleFactura(numAtCard);
        }
        /*******************************************************************************************************************/
        public ActionResult AnularFacturarTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 603)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ORTV_E datos = new ORTV_E
                {
                    EstadoPago = ticketPost.EstadoPago,
                    TipoVenta = ticketPost.TipoVenta,
                    TiempoEntrega = ticketPost.TiempoEntrega,
                    Estado = ticketPost.Estado,
                    FechaSapTicket = ticketPost.FechaSapTicket,
                    CardName = ticketPost.CardName,
                    Vendedor = ticketPost.Vendedor,
                    Flete = ticketPost.Flete,
                    DescuentoNC = ticketPost.DescuentoNC,
                    EstadoFacturacion = ticketPost.EstadoFacturacion,
                    LugarDestino = ticketPost.LugarDestino,
                    Zona = ticketPost.Zona,
                    zonaDistinta = ticketPost.zonaDistinta,
                    Mensaje = string.Empty
                };
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    String operario = $"{usu.Nombres} {usu.Apellidos}";
                    int DocNum = _ticketN.revertirFacturarTicket(DocEntry, operario);
                    return RedirectToAction("ListadoTicketsFacturacion", datos);
                }
                catch (Exception e)
                {
                    datos.Mensaje = e.Message;
                    return RedirectToAction("ListadoTicketsFacturacion", datos);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        /*
         * 
         * Buscar Facturas y Boletas relacionadas al ticket
         * 
         */
        public JsonResult buscarFacturasyBoletas(int DocEntry)
        {
            Capa_Negocio.Ventas_NEG.Tablas.ORDR_N ordrN = new Capa_Negocio.Ventas_NEG.Tablas.ORDR_N(); ORTV_N negtik = new ORTV_N(); OINV_N oinvNeg = new OINV_N();
            ORTV_E obj = negtik.ObtenerDatosCompletosTicket(DocEntry);
            List<string> FB = new List<string>();
            foreach (var orden in obj.Det2)
            {
                List<Capa_Entidad.Ventas_ENT.Tablas.ORDR_E> Ordenes = ordrN.listadoOrdenesDeVenta(new Capa_Entidad.Ventas_ENT.Tablas.ORDR_E { DocNum = orden.NroSap }, true);
                if (Ordenes.Count > 0)
                {
                    foreach (var x in Ordenes[0].ComprobantesVinculados)
                    {
                        FB.Add(x.NumAtCard);
                    }
                }
            }
            List<OINV_E> lista = new List<OINV_E>();
            foreach (var o in FB)
            {
                if (!string.IsNullOrWhiteSpace(o) && o.Contains("F"))
                {
                    lista.Add(oinvNeg.listadoFacturasDeVenta(new OINV_E { NumAtCard = o }).FirstOrDefault());
                }
                else if (!string.IsNullOrWhiteSpace(o) && o.Contains("B"))
                {
                    lista.Add(oinvNeg.listadoBoletasDeVenta(new OINV_E { NumAtCard = o }).FirstOrDefault());
                }
            }
            return Json(
                    lista
                        .Where(x => x != null && !string.IsNullOrWhiteSpace(x.NumAtCard))
                        .GroupBy(x => x.NumAtCard)
                        .Select(g => g.First())
                        .ToList()
                );
        }
        public JsonResult buscarGuias(int DocEntry)
        {
            var lista = new List<Guia_Remision_E>();
            var ticket = new ORTV_N().ObtenerDatosCompletosTicket(DocEntry);
            string Guias = string.Empty;
            if (ticket.LugarDestino.Equals("Arriola") || ticket.LugarDestino.Equals("Centro"))
            {
                string WhsCode = string.Empty;
                if (ticket.LugarDestino.Equals("Centro")) { WhsCode = "01"; }
                else if (ticket.LugarDestino.Equals("Arriola")) { WhsCode = "09"; }
                Guias = new Capa_Negocio.Almacen_NEG.Tablas.OWTR_N().GuiasTicketTransferencia(ticket.DocNum, WhsCode, ticket.CardCode);
            }
            else
            {
                Guias = _ticketN.GuiasTicket(DocEntry);
            }
            //separar las guias concatenadas desde Guias string
            Guias = Guias.Trim(); string[] guiasSeparadas = Guias.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < guiasSeparadas.Count(); i++)
            {
                ODLN_N odlN = new ODLN_N(); OWTR_N owtrN = new OWTR_N();
                List<Guia_Remision_E> resultGuias = odlN.buscarGuiaRemisionSap(guiasSeparadas[i]);
                if (resultGuias != null && resultGuias.Count() > 0)
                {
                    if (!string.IsNullOrWhiteSpace(resultGuias[0].NumAtCard)) { lista.Add(odlN.buscarGuiaRemisionSap(guiasSeparadas[i])[0]); }
                }
                else { lista.Add(owtrN.buscarGuiaRemisionSap(guiasSeparadas[i])[0]); }
            }
            return Json(lista);
        }
        public JsonResult buscarNotaCreditoVenta(int DocEntry)
        {
            ORTV_N ortvN = new ORTV_N();
            List<RTV4_E> nc = ortvN.obtenerDet4Ticket(DocEntry);
            return Json(nc);
        }
        //RECEPCION
        public ActionResult ListadoTicketsRecepcion(int DocNum = 0, ORTV_E ticket = null, string Mensaje = "", int idOperation = 701)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                ViewBag.DocNum = DocNum;
                //if (user.WhsCode != null && (user.IdRol == 5 || user.IdRol == 4 || user.IdRol == 51))
                //{
                //    if (user.WhsCode.Equals("07"))
                //    {
                //        ticket.AlmProcedencia = "ALM07";
                //    }
                //}
                if (DocNum > 0)
                {
                    ticket.DocNum = DocNum;
                    ViewBag.Ortv = ticket;
                    ViewBag.DocNum = DocNum;
                }
                else { ViewBag.Ortv = ticket; }
                ViewBag.Mensaje = Mensaje;
                ViewBag.IdRol = user.IdRol;
                return View(_ticketN.ListarTicketsAreaRecepcion(user, ticket));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RecibirTicketVenta(int DocEntry, int idOperation = 702)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Capa_Negocio.Ventas_NEG.Tablas.ORDR_N ordrN = new Capa_Negocio.Ventas_NEG.Tablas.ORDR_N();
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E t = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    t.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    //validar que las ordenes de ventas esten vigentes, sino no puede recibir
                    bool todasvigentes = true; int DocNum = 0; int num = 0;
                    foreach (var ordenForEach in t.Det2)
                    {
                        var orden = ordrN.obtenerOrdenDeVenta(ordenForEach.NroSap);
                        if (orden.CANCELED.Equals("Y") || string.IsNullOrWhiteSpace(orden.CANCELED)) { todasvigentes = false; num = ordenForEach.NroSap; break; }
                    }
                    if (todasvigentes)
                    {
                        DocNum = _ticketN.recibirTicket(DocEntry, t);
                        return RedirectToAction("ListadoTicketsRecepcion", new { DocNum = DocNum, Mensaje = "Se ha recibido correctamente", DescargarPDF = 1 });
                    }
                    else
                    {
                        return RedirectToAction("ListadoTicketsRecepcion", new { DocNum = t.DocNum, Mensaje = "El ticket tiene la orden de venta " + num + " que ya no esta vigente.", DescargarPDF = 0 });
                    }
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoTicketsRecepcion", new { Mensaje = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AnularRecibirTicketVenta(int DocEntry, int idOperation = 703)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E t = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    t.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    int DocNum = _ticketN.anularRecibirTicket(DocEntry, t);
                    return RedirectToAction("ListadoTicketsRecepcion", new { DocNum = DocNum, Mensaje = "Se anuló el ticket recibido correctamente" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoTicketsRecepcion", new { Mensaje = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //PICKING PACKING
        public ActionResult ListadoTicketsAlmacen(int DocNum = 0, ORTV_E t = null, string Mensaje = "", int idOperation = 801)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];

                ViewBag.PermisosUsuario = new Dictionary<string, bool>
                {
                    {"PackingTicket",  (_usuarioOperacionN.VerificarAccesoOperacion(new OUSR_OPE_E { UsrDocEntry = user.DocEntry, OpeID = 804 })) > 0 },
                    {"CancelarTicket",  (_usuarioOperacionN.VerificarAccesoOperacion(new OUSR_OPE_E { UsrDocEntry = user.DocEntry, OpeID = 518 })) > 0 },
                    {"AnularPickingTicket",  (_usuarioOperacionN.VerificarAccesoOperacion(new OUSR_OPE_E { UsrDocEntry = user.DocEntry, OpeID = 803 })) > 0 },
                    {"AnularPackingTicket",  (_usuarioOperacionN.VerificarAccesoOperacion(new OUSR_OPE_E { UsrDocEntry = user.DocEntry, OpeID = 805 })) > 0 },
                    {"AnularPesadoTicket",  (_usuarioOperacionN.VerificarAccesoOperacion(new OUSR_OPE_E { UsrDocEntry = user.DocEntry, OpeID = 807 })) > 0 },
                    {"AnularVerificacionTicket",  (_usuarioOperacionN.VerificarAccesoOperacion(new OUSR_OPE_E { UsrDocEntry = user.DocEntry, OpeID = 809 })) > 0 }
                };

                ViewBag.RolSupervisor = user.IdRol;
                ViewBag.AlmUsuario = user.WhsCode;
                ViewBag.DocNum = DocNum;
                ViewBag.Ortv = t;
                ViewBag.Mensaje = Mensaje;
                ViewBag.ProductosPendientes = _ticketN.CantidadTicketsProductosPendientes();
                return View(_ticketN.ListarTicketsAreaAlmacén(user, t));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult IniciarSacandoTicketVenta(int DocEntry, int idOperation = 802)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    tc.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    int DocNum = _ticketN.editarSeguimientoTicket("INICIO PICKING", DocEntry, tc);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Ticket se está pickeando" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoTicketsAlmacen", new { Mensaje = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AnularIniciarSacandoTicket(int DocEntry, int idOperation = 803)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ticket.RolSupervisor = usu.IdRol;
                    ticket.DocEntryOpRegistro = usu.DocEntry;
                    int DocNum = _ticketN.editarSeguimientoTicket("ANULAR INICIO PICKING", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Se ha anulado el proceso de iniciar picking" });
                }
                catch (Exception e) { return RedirectToAction("ListadoTicketsAlmacen", new { Mensaje = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult SacandoTicketVenta(int DocEntry, int idOperation = 802)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E u = (Usuario_E)Session["UsuarioId"];
                try
                {
                    ViewBag.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    var listaUsuarios = _usuarioN.ListaUsuarios(new Usuario_E { IdRol = 4 });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    ViewBag.Mensaje = string.Empty;
                    return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry));
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    var listaUsuarios = _usuarioN.ListaUsuarios(new Usuario_E { IdRol = 4 });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    ViewBag.OpRegistro = $"{u.Nombres} {u.Apellidos}"; return View();
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult SacandoTicketVenta(int DocEntry, ORTV_E t, int idOperation = 802)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    tc.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    tc.Det11 = t.Det11;                                                 // OpSacador 2, OpSacador 3 y OpSacador 4
                    tc.Det11[0].Operario = tc.OpRegistro;                   // Seteamos elOpSacando quién es el usuario en sesión
                    ViewBag.datosSacador = ccORTV_N.ListarCC_ORTV(DocEntry, "FIN PICKING");
                    int DocNum = _ticketN.editarSeguimientoTicket("FIN PICKING", DocEntry, tc);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Ticket ha sido pickeado" });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    var listaUsuarios = _usuarioN.ListaUsuarios(new Usuario_E { IdRol = 4 });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry));
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AnularSacandoTicket(int DocEntry, int idOperation = 803)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ViewBag.Mensaje = string.Empty;
                    return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult AnularSacandoTicket(int DocEntry, ORTV_E t, int idOperation = 803)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ViewBag.Mensaje = "Se anulo proceso de FIN PICKING al ticket";
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ticket.RolSupervisor = usu.IdRol;
                    ticket.DocEntryOpRegistro = usu.DocEntry;
                    int DocNum = _ticketN.editarSeguimientoTicket("ANULAR FIN PICKING", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum });
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry)); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult IniciarVerificandoTicketVenta(int DocEntry, int idOperation = 808)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    tc.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    int DocNum = _ticketN.editarSeguimientoTicket("INICIO VERIFICAR", DocEntry, tc);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Verificando el Ticket" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoTicketsAlmacen", new { Mensaje = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AnularIniciarVerificandoTicket(int DocEntry, int idOperation = 809)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ticket.RolSupervisor = usu.IdRol;
                    ticket.DocEntryOpRegistro = usu.DocEntry;
                    int DocNum = _ticketN.editarSeguimientoTicket("ANULAR INICIO VERIFICAR", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Inicio de verificación ANULADO" });
                }
                catch (Exception e) { return RedirectToAction("ListadoTicketsAlmacen", new { Mensaje = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult VerificadoTicketVenta(int DocEntry, int idOperation = 808)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    OTEP_N otepN = new OTEP_N();
                    RTV11_N rtv11N = new RTV11_N();
                    OMRC_N omrcN = new OMRC_N();
                    SAT1_N sat1N = new SAT1_N();
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ViewBag.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ViewBag.Mensaje = string.Empty;
                    var listaUsuarios = _usuarioN.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    List<CC_ORTV_E> ticketVerificado = new List<CC_ORTV_E>();
                    ViewBag.ErroresPicking = otepN.ListarTiposErroresPicking();
                    ViewBag.Pickers = rtv11N.ObtenerPickers(DocEntry);
                    ViewBag.Laboratorios = omrcN.listarFabricantes();
                    ViewBag.ListaArticulosTicket = sat1N.BuscarCodProductosTicket(ticket.DocNum);
                    if (ticketVerificado != null && ticketVerificado.Count >= 1)
                    {
                        ticket.OpVerificado = ticketVerificado[0].Operario;
                        ticket.FechaVerificado = ticketVerificado[0].FechaOperacion;
                        ticket.HoraVerificado = ticketVerificado[0].HoraOperacion;
                    }
                    return View(ticket);
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    var listaUsuarios = _usuarioN.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    return View();
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult VerificadoTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 808)

        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    ticket.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    ticket.Det12 = ticketPost.Det12;        // OpVerificador 2 y OpVerificador 3
                    ticket.ProductoPendiente = ticketPost.ProductoPendiente;  // IMPORTANTE: Se asigna el valor

                    var listaUsuarios = _usuarioN.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;

                    // Registrar el ticket independientemente del valor de ProductoPendiente
                    int DocNum = _ticketN.editarSeguimientoTicket("FIN VERIFICAR", DocEntry, ticket);

                    // Mensaje diferente según si hay productos pendientes o no
                    string mensaje = ticket.ProductoPendiente == 1
                        ? $"Ticket {DocNum} verificado con productos pendientes"
                        : $"Ticket {DocNum} verificado correctamente";

                    return Json(new { DocNum, Mensaje = mensaje });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    var listaUsuarios = _usuarioN.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry));
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public ActionResult AnularVerificadoTicket(int DocEntry, int idOperation = 809)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ViewBag.Mensaje = string.Empty;
                    return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult AnularVerificadoTicket(int DocEntry, ORTV_E t, int idOperation = 809)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ViewBag.Mensaje = "Se anuló el proceso de FIN VERIFICAR";
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ticket.RolSupervisor = usu.IdRol;
                    ticket.DocEntryOpRegistro = usu.DocEntry;
                    int DocNum = _ticketN.editarSeguimientoTicket("ANULAR FIN VERIFICAR", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum });
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry)); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult IniciarEmpacandoTicketVenta(int DocEntry, int idOperation = 804)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    tc.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    int DocNum = _ticketN.editarSeguimientoTicket("INICIO EMPACAR", DocEntry, tc);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Empacando Ticket" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoTicketsAlmacen", new { Mensaje = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AnularIniciarEmpacandoTicket(int DocEntry, int idOperation = 805)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ticket.RolSupervisor = usu.IdRol;
                    ticket.DocEntryOpRegistro = usu.DocEntry;
                    int DocNum = _ticketN.editarSeguimientoTicket("ANULAR INICIO EMPACAR", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Inicio de packing anulado" });
                }
                catch (Exception e) { return RedirectToAction("ListadoTicketsAlmacen", new { Mensaje = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EmpacadoTicketVenta(int DocEntry, int idOperation = 804)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ViewBag.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ViewBag.Mensaje = string.Empty;
                    var listaUsuarios = _usuarioN.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    return View(ticket);
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    var listaUsuarios = _usuarioN.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    return View();
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult EmpacadoTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 804)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E usuario = (Usuario_E)Session["UsuarioId"];
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    ticket.OpRegistro = $"{usuario.Nombres} {usuario.Apellidos}";
                    ticket.Cajas = ticketPost.Cajas;
                    ticket.NroMesa = ticketPost.NroMesa;
                    ticket.Operario = usuario.WhsCode;      //envia el dato de WhsCode del usuario
                    ticket.Det13 = ticketPost.Det13;        // OpEmpacador 2 y OpEmpacador 3

                    int DocNum = _ticketN.editarSeguimientoTicket("FIN EMPACAR", DocEntry, ticket);
                    var listaUsuarios = _usuarioN.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    var mensaje = "Ticket terminó packing";
                    if (ticket.LugarDestino.Equals("Agencia"))
                    {
                        return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = mensaje, DescargarPDF = 0, NumTicket = DocEntry, LugarDestino = ticketPost.LugarDestino });
                    }
                    else
                    {
                        return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = mensaje, DescargarPDF = 1, NumTicket = DocEntry, LugarDestino = ticketPost.LugarDestino });
                    }
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    var listaUsuarios = _usuarioN.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry));
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AnularEmpacadoTicket(int DocEntry, string Mensaje, int idOperation = 805)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ViewBag.Mensaje = Mensaje;
                    return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult AnularEmpacadoTicket(int DocEntry, ORTV_E t, int idOperation = 805)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ViewBag.Mensaje = "Se anuló termino de packing";
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ticket.RolSupervisor = usu.IdRol;
                    ticket.Operario = usu.WhsCode;
                    ticket.DocEntryOpRegistro = usu.DocEntry;
                    int DocNum = _ticketN.editarSeguimientoTicket("ANULAR FIN EMPACAR", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum });
                }
                catch (Exception e) { return RedirectToAction("AnularEmpacadoTicket", new { DocEntry, Mensaje = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //DESPACHO
        public ActionResult ListadoTicketsDespacho(int DocNum = 0, ORTV_E ticket = null, string Mensaje = "", int idOperation = 901)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                ViewBag.DocNum = DocNum;
                ViewBag.almacenUsuario = user.WhsCode;
                ViewBag.idRolUsuario = user.IdRol;
                ViewBag.Mensaje = Mensaje;
                if (user.WhsCode != null)
                {
                    if (user.WhsCode.Equals("01"))
                    {
                        ticket.LugarDestino = "Centro";
                    }
                    else if (user.WhsCode.Equals("06"))
                    {
                        ticket.LugarDestino = "Arriola";
                    }
                }
                var lista = _ticketN.ListarTicketsAreaDespacho(user, ticket);
                if (user.IdRol == 53)
                {
                    lista = lista.OrderBy(x =>
                    {
                        if (x.Estado == "PICKEANDO")
                            return 0;
                        if (x.Estado == "VERIFICANDO")
                            return 1;
                        if (x.Estado == "EMPACANDO")
                            return 2;
                        if (x.Estado == "EMPACADO")
                            return 3;
                        if (x.Estado == "PESADO")
                            return 4;
                        if (x.Estado == "PREENVIO")
                            return 5;
                        if (x.Estado == "ENVIADO")
                            return 6;
                        if (x.Estado == "ENTREGADO")
                            return 7;
                        return 8;
                    }).ToList();
                }
                ViewBag.Ortv = ticket;
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EntregadoTicketVenta(int DocEntry, int idOperation = 902)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    ViewBag.SelectedRegEstado = "";
                    ViewBag.DescripcionRegalo = "";
                    ViewBag.cantRegalo = 0;
                    if (ticket.Det5 != null && ticket.Det5.Count() >= 1)
                    {
                        if (ticket.Det5[0].RegEstado.Equals("Entregado"))
                        {
                            ViewBag.SelectedRegEstado = "selected";
                        }
                        ViewBag.DescripcionRegalo = $"{ticket.Det5[0].RegCate} - {ticket.Det5[0].RegTipo}";
                        ViewBag.CantRegalo = Decimal.Round(ticket.Det5[0].RegCant, 0);
                    }
                    ViewBag.Mensaje = string.Empty;
                    return View(ticket);
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult EntregadoTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 902)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    if (!String.IsNullOrWhiteSpace(ticketPost.Det5[0].RegEstado))
                    {
                        ticket.Det5[0].RegEstado = ticketPost.Det5[0].RegEstado;
                    }
                    ViewBag.Mensaje = "Entregado Correctamente";
                    int DocNum = _ticketN.Entregar(ticket);
                    return RedirectToAction("ListadoTicketsDespacho", new { DocNum = DocNum });
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry)); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AnularEntregadoTicket(int DocEntry, int idOperation = 903)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ViewBag.Mensaje = "";
                    return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult AnularEntregadoTicket(int DocEntry, ORTV_E t, int idOperation = 903)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                var ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                ViewBag.Mensaje = "AnularEntregado Correctamente";
                try
                {
                    int DocNum = _ticketN.editarSeguimientoTicket("ANULARENTREGADO", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsDespacho", new { DocNum = DocNum });
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(ticket); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RptFormatoAgencia(int idOperation = 520)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ReportClass rc = new ReportClass();
                rc.FileName = Server.MapPath("/Reportes/RptVentas/RptFormatoAgencia.rpt");
                Capa_Negocio.Utilitarios_N utiN = new Capa_Negocio.Utilitarios_N();
                var coninfo = utiN.getConexion();
                TableLogOnInfo logoninfo = new TableLogOnInfo();
                CrystalDecisions.CrystalReports.Engine.Tables tables;
                tables = rc.Database.Tables;
                foreach (CrystalDecisions.CrystalReports.Engine.Table item in tables)
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
                return File(stream, "application/pdf", "FormatoAgencia.pdf");
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RptFormatoAgenciaExcel(int idOperation = 521)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                ORTV_N ortvN = new ORTV_N();
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var formatoAgencia = ortvN.listarTicketsAgencia();
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("FormatoAgencia");
                    worksheet.Cells["A1"].LoadFromCollection(formatoAgencia, PrintHeaders: true);
                    for (var col = 1; col <= 13; col++)
                    {
                        worksheet.Column(col).AutoFit();
                    }
                    var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: formatoAgencia.Count + 1, toColumn: 13), "FormatoAgencia");
                    tabla.ShowHeader = true;
                    tabla.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;
                    return File(libro.GetAsByteArray(), excelContentType, "FormatoAgencia.xlsx");
                }
            }
            else { return null; }
        }
        public JsonResult CalcularPesoTotal(ORTV_E t)
        {
            return Json(_ticketN.CalcularPesoTotal(t));
        }
        public ActionResult PesadoTicketVenta(int DocEntry, int idOperation = 806)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ViewBag.Mensaje = "¿Está seguro(a) de cambiar el estado a PESADO?";
                    return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult PesadoTicketVenta(int DocEntry, ORTV_E t, int idOperation = 806)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    tc.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    if (t.Det6 != null && t.Det6.Count > 0)
                    {
                        for (int i = 0; i > t.Det6.Count; i++)
                        {
                            t.Det6[i].UniMed = "KG";
                        }
                    }
                    tc.Det6 = t.Det6;
                    int DocNum = _ticketN.editarSeguimientoTicket("PESADO", DocEntry, tc);
                    return RedirectToAction("ListadoTicketsDespacho", new { DocNum = DocNum, Mensaje = "Pesado Correctamente" });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry));
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpGet]
        public ActionResult AnularPesadoTicket(int DocEntry, int idOperation = 807)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ViewBag.Mensaje = "¿Está seguro(a) de ANULAR PESADO?";
                    return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult AnularPesadoTicket(int DocEntry, ORTV_E nulo, int idOperation = 807)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    int DocNum = _ticketN.editarSeguimientoTicket("ANULARPESADO", DocEntry, ticket);
                    ViewBag.Mensaje = "Proceso de pesado anulado correctamente";
                    return RedirectToAction("ListadoTicketsDespacho", new { DocNum = DocNum });
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(_ticketN.ObtenerDatosCompletosTicket(DocEntry)); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //LIBROS SALDOS
        public ActionResult ListadoLibrosSaldo(OLDS_E li = null, int idOperation = 1301)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ViewBag.Mensaje = "";
                    ViewBag.Olds = li;
                    return View(_libroDeSaldoN.listarLibrosSaldo(li));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(_libroDeSaldoN.listarLibrosSaldo(li)); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult CrearLibroSaldo(int idOperation = 1302)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OCRD_N ocrdN = new OCRD_N();
                ViewBag.Mensaje = "";
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                return View(new OLDS_E());
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult CrearLibroSaldo(OLDS_E l, int idOperation = 1302)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OCRD_N ocrdN = new OCRD_N();
                ViewBag.Mensaje = "";
                try
                {
                    ViewBag.Mensaje = "LibroCreado";
                    _libroDeSaldoN.crearLibroSaldo(l);
                    return RedirectToAction("ListadoLibrosSaldo");
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; }
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                return View(l);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ListarDetLibroSaldo(string CardCode, int idOperation = 1303)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ViewBag.LibroSaldo = _libroDeSaldoN.obtenerLibroSaldo(CardCode);
                    ViewBag.Mensaje = "";
                    return View(_libroDeSaldoN.obtenerDetLibroSaldo(CardCode));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AgregarDetLibroSaldo(string CardCode, int idOperation = 1304)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ViewBag.LibroSaldo = _libroDeSaldoN.obtenerLibroSaldo(CardCode);
                    ViewBag.Mensaje = "";
                    LDS1_E d = new LDS1_E();
                    d.CardCode = CardCode;
                    return View(d);
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult AgregarDetLibroSaldo(LDS1_E d, int idOperation = 1304)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ViewBag.Mensaje = "";
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    d.OperarioReg = u.Nombres + " " + u.Apellidos;
                    if (ModelState.IsValid)
                    {
                        _libroDeSaldoN.agregarDetLibroSaldo(d);
                        return RedirectToAction("ListarDetLibroSaldo", new { CardCode = d.CardCode });
                    }
                    return View(d);
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; ViewBag.LibroSaldo = _libroDeSaldoN.obtenerLibroSaldo(d.CardCode); return View(d); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ReporteLibroSaldo(string CardCode, int idOperation = 1305)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ViewBag.Mensaje = "";
                    return View(_libroDeSaldoN.obtenerLibroSaldo(CardCode));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //REPORTES
        public ActionResult ReportesVentas(int idOperation = 1306)
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
        public ActionResult infoAuditVtsCli(int idOperation = 1307)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.General_NEG.Tablas.OSLP_N oslpN = new Capa_Negocio.General_NEG.Tablas.OSLP_N();
                OCRD_N ocrdN = new OCRD_N();
                Capa_Negocio.General_NEG.TablasSql.OWHS_N owhsN = new Capa_Negocio.General_NEG.TablasSql.OWHS_N();
                Capa_Negocio.Almacen_NEG.Tablas.OMRC_N omrcN = new Capa_Negocio.Almacen_NEG.Tablas.OMRC_N();
                ViewBag.Vendedores = new SelectList(oslpN.listadoOslp("VENTA"), "SlpCode", "SlpName");
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                ViewBag.Laboratorios = new SelectList(omrcN.listarFabricantes(), "FirmCode", "U_SYP_DESC");
                ViewBag.Almacenes = owhsN.listarAlmacenes();
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult tbReporteAuditVtsCli(string FecIni, string FecFin, string CardCode, int FirmCode = 0, int SlpCode = 0, int idOperation = 1307)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Ventas_NEG.ReportesHana.AnlVent1_N anlVent1N = new Capa_Negocio.Ventas_NEG.ReportesHana.AnlVent1_N();
                ReportViewer rp = new ReportViewer();
                ViewBag.Mensaje = "";
                try
                {
                    rp.ProcessingMode = ProcessingMode.Local;
                    rp.SizeToReportContent = true;
                    rp.LocalReport.ReportPath =
                        Request.MapPath(Request.ApplicationPath) + @"Reportes\RptVentas\RptAudiVtsCli.rdlc";
                    rp.LocalReport.DataSources.Add(new ReportDataSource("DS_AudiVtsCli", anlVent1N.tbRptAnlVent1(FecIni, FecFin,
                        SlpCode, CardCode, FirmCode)));
                    ViewBag.REPORTE = rp;
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                }
                return View("reporteViewer");
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult infoAnalisisTickets(int idOperation = 1308)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Almacenes = _owhsSapN.ListarAlmacenes("todos");
                ViewBag.Clientes = new Capa_Negocio.SocioNegocios_NEG.TablasExternas.OCRD_N().listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                ViewBag.Operarios = new Capa_Negocio.Seguridad_NEG.Usuario_N().ListaUsuarios(null);
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RptAnalisisTickets(RptFiltrosAnalisisTickets_E frm, int idOperation = 1308)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ORTV_N ortvN = new ORTV_N();
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var analisisTickets = ortvN.ListarRptAnalisisTickets(frm);
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("Analisis Tickets");
                    worksheet.Cells["A1"].LoadFromCollection(analisisTickets, PrintHeaders: true);
                    if (analisisTickets != null)
                    {
                        if (analisisTickets.Count >= 1)
                        {
                            for (var col = 1; col <= 71; col++)
                            {
                                worksheet.Column(col).AutoFit();
                            }
                            var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: analisisTickets.Count + 1, toColumn: 71), "RptAnalisisTickets");
                            tabla.ShowHeader = true;
                            tabla.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;
                        }
                    }
                    return File(libro.GetAsByteArray(), excelContentType, "ReporteAnalisisTickets.xlsx");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult infoAnalisisVentas(int idOperation = 1309)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Almacenes = new Capa_Negocio.General_NEG.TablasSql.OWHS_N().listarAlmacenes();
                ViewBag.Clientes = new Capa_Negocio.SocioNegocios_NEG.TablasExternas.OCRD_N().listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                ViewBag.Operarios = new Capa_Negocio.Seguridad_NEG.Usuario_N().ListaUsuarios(null);
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult tbReporteAnalisisVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnalisisVentas_E obj, int idOperation = 1309)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ReportViewer rp = new ReportViewer();
                ViewBag.Mensaje = "";
                try
                {
                    rp.ProcessingMode = ProcessingMode.Local;
                    rp.SizeToReportContent = true;
                    rp.LocalReport.ReportPath =
                        Request.MapPath(Request.ApplicationPath) + @"Reportes\RptVentas\RptAnalisisVentas.rdlc";
                    rp.LocalReport.DataSources.Add(new ReportDataSource("DS_AnalisisVentas", _ticketN.tbRptAnalisisVentas(obj)));
                    ViewBag.REPORTE = rp;
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                }
                return View("reporteViewer");
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //REGALOS
        public ActionResult GestionRegalos(OREG_E filtro, string mensaje, int idOperation = 1310)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Regalos = filtro;
                ViewBag.Mensaje = mensaje;
                return View(new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N().listaRegalos(filtro));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult NuevoRegalo(int idOperation = 1311)
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
        [HttpPost]
        public ActionResult NuevoRegalo(OREG_E obj, int idOperation = 1311)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OREG_N oregN = new OREG_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.OpRegistro = $"{user.Nombres} {user.Apellidos}";
                    obj.StockDisp = obj.StockTotal;
                    oregN.registrarNuevoRegalo(obj);
                    return RedirectToAction("GestionRegalos", new { mensaje = "Se registró regalo exitosamente." }); ;
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
        public ActionResult GestionarStock(int id, int idOperation = 1312)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Regalo = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N().buscarRegalo(id);
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult GestionarStock(OTRC_E obj, int idOperation = 1312)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    obj.Operario = $"{u.Nombres} {u.Apellidos}";
                    new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N().validarGestionStock(new OREG_E() { Id = obj.IdReg }, obj);
                    if (obj.Sentido == "Salida") { obj.Cantidad = -1 * obj.Cantidad; }
                    new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N().RegistrarGestionStock(new OREG_E() { Id = obj.IdReg, StockDisp = obj.Cantidad }, obj);
                    return RedirectToAction("GestionRegalos");
                }
                catch
                {
                    ViewBag.Regalo = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N().buscarRegalo(obj.IdReg);
                    return View();
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult InactivarRegalo(int id, int idOperation = 1313)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View(new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N().buscarRegalo(id));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult InactivarRegaloPost(OREG_E obj, int idOperation = 1313)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N().inactivarRegalo(obj);
                    return RedirectToAction("GestionRegalos", new { Id = obj.Id });
                }
                catch (Exception e)
                { return RedirectToAction("InactivarRegalo", new { msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RevertirInactivarRegalo(OREG_E obj, int idOperation = 1313)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N().revertirInactivarRegalo(obj);
                    return RedirectToAction("GestionRegalos", new { Id = obj.Id });
                }
                catch (Exception e)
                { return RedirectToAction("InactivarRegalo", new { msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult TransaccionesRegalo(int id, int idOperation = 1314)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Regalo = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N().buscarRegalo(id);
                return View(new Capa_Negocio.Ventas_NEG.TablasSql.OTRC_N().listarTransacciones(new OTRC_E() { IdReg = id }));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ExportarExcelTransReg(OTRC_E o, int idOperation = 1314)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                ViewBag.Regalo = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N().buscarRegalo(o.IdReg);
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var transacciones = new Capa_Negocio.Ventas_NEG.TablasSql.OTRC_N().listarTransacciones(o);
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("Transacciones");
                    worksheet.Cells["A1"].LoadFromCollection(transacciones, PrintHeaders: true);
                    for (var col = 1; col <= 12; col++)
                    {
                        worksheet.Column(col).AutoFit();
                    }
                    var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: transacciones.Count + 1, toColumn: 12), "Transacciones");
                    tabla.ShowHeader = true;
                    return File(libro.GetAsByteArray(), excelContentType, "TransaccionesRegalos.xlsx");
                }
            }
            else { return null; }
        }
        public JsonResult VerificarExistenciaDatos(string fechaTicketDesde, string fechaTicketHasta, string estadoTicket, string estadoRegalo)
        {
            var result = new Capa_Negocio.Ventas_NEG.TablasSql.ORTV_N().listarTicketsRegalo(fechaTicketDesde, fechaTicketHasta, estadoTicket, estadoRegalo);
            if (result != null && result.Count() > 0)
            {
                return Json(new { Titulo = "" });
            }
            else
            {
                return Json(new { Titulo = "Sin Datos" });
            }
        }
        public ActionResult ExporteReporteGeneralTicketsRegalos(string fechaTicketDesde, string fechaTicketHasta, string estadoTicket, string estadoRegalo, int idOperation = 524)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation,
                (Usuario_E)Session["UsuarioId"],
                this.ControllerContext.RouteData.Values["action"].ToString(),
                Request.UserHostAddress,
                Request.UserHostName);
            if (acceso == "C_Access")
            {
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var result = new Capa_Negocio.Ventas_NEG.TablasSql.ORTV_N().listarTicketsRegalo(fechaTicketDesde, fechaTicketHasta, estadoTicket, estadoRegalo);
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("ReporteRegalosEntregados");
                    worksheet.Cells["A1"].LoadFromCollection(result, PrintHeaders: true);
                    for (var col = 1; col <= 11; col++)
                    {
                        worksheet.Column(col).AutoFit();
                    }
                    var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: result.Count + 1, toColumn: 11), "ReporteRegalosEntregados");
                    tabla.ShowHeader = true;
                    return File(libro.GetAsByteArray(), excelContentType, "ReporteRegalosTickets.xlsx");
                }
            }
            else { return null; }
        }
        public ActionResult ReporteClienteRegalos(string CardCode, int idOperation = 522)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OCLR_N oclrN = new OCLR_N();
                try
                {
                    ViewBag.Mensaje = "";
                    return View(oclrN.buscarClienteRegalo(CardCode));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult GestionClienteRegalos(OCLR_E filtro, int idOperation = 1315)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    ViewBag.CliReg = filtro;
                    // Obtener listas necesarias para el modal de Nuevo Cliente Regalo
                    OCLR_N oclrN = new OCLR_N();
                    OREG_N oregN = new OREG_N();
                    OCRD_N ocrdN = new OCRD_N();
                    OREG_E filtroRegalo = null;
                    ViewBag.Regalos = oregN.listaRegalos(filtroRegalo);
                    ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                    return View(oclrN.listadoRegaloCliente(filtro));
                }
                catch (Exception e)
                {
                    return RedirectToAction("GestionClienteRegalos", new { msj = e.Message });
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult NuevoClienteRegalo(int idOperation = 1316)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OREG_N oregN = new OREG_N(); OCRD_N ocrdN = new OCRD_N();
                OREG_E filtro = null;
                ViewBag.Regalos = oregN.listaRegalos(filtro);
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult NuevoClienteRegalo(OCLR_E obj, int idOperation = 1316)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    new Capa_Negocio.Ventas_NEG.TablasSql.OCLR_N().registrarClienteRegalo(obj);
                    return RedirectToAction("GestionClienteRegalos");
                }
                catch (Exception e)
                { return RedirectToAction("NuevoClienteRegalo", new { msj = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult EditarClienteRegalos(string CardCode, int idOperation = 1317)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                OREG_N oregN = new OREG_N();
                OCLR_N oclrN = new OCLR_N();
                ViewBag.Regalos = oregN.listaRegalos(null);
                return View(oclrN.buscarClienteRegalo(CardCode));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult EditarClienteRegalos(OCLR_E obj, int idOperation = 1317)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    new Capa_Negocio.Ventas_NEG.TablasSql.OCLR_N().editarClienteRegalo(obj);
                    return RedirectToAction("GestionClienteRegalos", new { CardCode = obj.CardCode });
                }
                catch (Exception e)
                {
                    ViewBag.Regalos = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N().listaRegalos(null);
                    ViewBag.Mensaje = e.Message;
                    return View(obj);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ReporteReclamosCliente(string CardCode, int? DocNumTicket = null, int idOperation = 523)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.AtencionCliente_NEG.TablasSql.OSAT_N osatN = new Capa_Negocio.AtencionCliente_NEG.TablasSql.OSAT_N();
                try
                {
                    Dictionary<string, string> ortv = new Dictionary<string, string> { { "CardCode", CardCode } };
                    ViewBag.Mensaje = "";
                    var filtro = new Capa_Entidad.AtencionCliente_ENT.TablasSql.OSAT_E()
                    {
                        DetORTV = ortv,
                        TipoSolicitudCreaTicketVenta = "('Reclamo','Devolucion')",      // TipoSolicitudCreaTicketVenta: Filtro para el botón Reclamos Crea Ticket Venta
                        TipoSolucionCreaTicketVenta = "('Regalo')",
                        Estado = "Atendido",
                        SoloSinTicketSolucion = true,
                        TicketSolucion = DocNumTicket?.ToString()
                    };
                    return View(osatN.ListarSolicitudes(filtro, false, false));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RptAnCtVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnCtVentas_E obj, int idOperation = 1318)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Ventas_NEG.ReportesHana.AnCtVentas_N anCtVentasN = new Capa_Negocio.Ventas_NEG.ReportesHana.AnCtVentas_N();
                ReportViewer rp = new ReportViewer();
                ViewBag.Mensaje = "";
                try
                {
                    List<Capa_Entidad.Ventas_ENT.ReportesHana.AnCtVentas_E> listaR = anCtVentasN.rptAnCtVentas(obj);
                    rp.ProcessingMode = ProcessingMode.Local;
                    rp.SizeToReportContent = true;
                    rp.LocalReport.ReportPath =
                        Request.MapPath(Request.ApplicationPath) + @"Reportes\RptVentas\RptAnCtVentas.rdlc";
                    rp.LocalReport.DataSources.Add(new ReportDataSource("DS_AnCtVentas", listaR));
                    ViewBag.REPORTE = rp;
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                }
                return View("reporteViewer");
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RptAnCtVentas2(Capa_Entidad.Ventas_ENT.Formularios.FrmAnCtVentas_E obj, int idOperation = 1318)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Ventas_NEG.ReportesHana.AnCtVentas_N anCtVentasN = new Capa_Negocio.Ventas_NEG.ReportesHana.AnCtVentas_N();
                ReportViewer rp = new ReportViewer();
                ViewBag.Mensaje = "";
                try
                {
                    List<Capa_Entidad.Ventas_ENT.ReportesHana.AnCtVentas_E> listaR = anCtVentasN.rptAnCtVentas(obj);
                    rp.ProcessingMode = ProcessingMode.Local;
                    rp.SizeToReportContent = true;
                    rp.LocalReport.ReportPath =
                        Request.MapPath(Request.ApplicationPath) + @"Reportes\RptVentas\RptAnCtVentas2.rdlc";
                    rp.LocalReport.DataSources.Add(new ReportDataSource("DS_AnCtVentas2", listaR));
                    ViewBag.REPORTE = rp;
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                }
                return View("reporteViewer");
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult AnCtVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnCtVentas_E obj, int idOperation = 1318)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Seguridad_NEG.TablasSql.USR2_N usr2N = new Capa_Negocio.Seguridad_NEG.TablasSql.USR2_N();
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    obj.user = user;
                    ViewBag.frm = obj;
                    if (!(user.IdRol == 1 || user.IdRol == 6 || user.IdRol == 7)) { usr2N.IntentosUsoOp(user.DocEntry, idOperation); }
                    return View();
                }
                catch { return RedirectToAction("Error", "Index"); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult GestionVendedores(USR1_E fil, int idOperation = 1319)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                USR1_N usr1N = new USR1_N();
                if (fil == null) { ViewBag.fil = new USR1_E(); }
                else { ViewBag.fil = fil; }
                return View(usr1N.listarVenUltCuotas(fil));
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult GestionCuotas(int DocEntry, string Mensaje = "", int idOperation = 1320)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                USR1_N usr1N = new USR1_N();
                ViewBag.CuotasUser = usr1N.listarUsrCuotas(DocEntry);
                ViewBag.User = _usuarioN.buscarUsuario(DocEntry);
                ViewBag.Mensaje = Mensaje;
                DateTime ahora = DateTime.Now;
                return View(new USR1_E() { YearU = ahora.Year, MonthU = ahora.Month });
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult GestionCuotas(USR1_E obj, int idOperation = 1320)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                USR1_N usr1N = new USR1_N();
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                obj.OpRegistro = $"{user.Nombres} {user.Apellidos}";
                try
                {
                    usr1N.registrarUsr1(obj);
                    return RedirectToAction("GestionCuotas", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e) { return RedirectToAction("GestionCuotas", new { DocEntry = obj.DocEntry, Mensaje = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult BorrarCuotaUsr(USR1_E obj, int idOperation = 1321)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                USR1_N usr1N = new USR1_N();
                try
                {
                    usr1N.borrarUsr1(obj);
                    return RedirectToAction("GestionCuotas", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e) { return RedirectToAction("GestionCuotas", new { DocEntry = obj.DocEntry, Mensaje = e.Message }); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult infoVentasClienteDias(int idOperation = 1322)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.SocioNegocios_NEG.TablasExternas.OCRD_N ocrdN = new Capa_Negocio.SocioNegocios_NEG.TablasExternas.OCRD_N();
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RptVentasClienteDias(string CardCodeIni, string CardCodeFin, string Fecha, int idOperation = 1322)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Ventas_NEG.ReportesHana.VentCliDias_N oN = new Capa_Negocio.Ventas_NEG.ReportesHana.VentCliDias_N();
                ReportViewer rp = new ReportViewer();
                ViewBag.Mensaje = "";
                try
                {
                    List<Capa_Entidad.Ventas_ENT.ReportesHana.VentCliDias_E> listaR = oN.RptVentCliDias(DateTime.Parse(Fecha), CardCodeIni, CardCodeFin);
                    rp.ProcessingMode = ProcessingMode.Local;
                    rp.SizeToReportContent = true;
                    rp.LocalReport.ReportPath =
                        Request.MapPath(Request.ApplicationPath) + @"Reportes\RptVentas\RptVentCliDias.rdlc";
                    rp.LocalReport.DataSources.Add(new ReportDataSource("DS_VentCliDias", listaR));
                    ViewBag.REPORTE = rp;
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                }
                return View("reporteViewer");
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult infoVentasVendedorDias(int idOperation = 1323)
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
        public ActionResult RptVentVendDias(string Fecha, int idOperation = 1323)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Ventas_NEG.ReportesHana.VentVendDias_N oN = new Capa_Negocio.Ventas_NEG.ReportesHana.VentVendDias_N();
                ReportViewer rp = new ReportViewer();
                ViewBag.Mensaje = "";
                try
                {
                    List<Capa_Entidad.Ventas_ENT.ReportesHana.VentVendDias_E> listaR = oN.RptVentVendDias(DateTime.Parse(Fecha));
                    rp.ProcessingMode = ProcessingMode.Local;
                    rp.SizeToReportContent = true;
                    rp.LocalReport.ReportPath =
                        Request.MapPath(Request.ApplicationPath) + @"Reportes\RptVentas\RptVentVendDias.rdlc";
                    rp.LocalReport.DataSources.Add(new ReportDataSource("DS_VentVendDias", listaR));
                    ViewBag.REPORTE = rp;
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                }
                return View("reporteViewer");
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult infoVentasSkuDias(int idOperation = 1324)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Almacen_NEG.Tablas.OITM_N oitmN = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N();
                ViewBag.ListaProductos = oitmN.Listar(null);
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RptVentasSkuDias(string ItemCodeIni, string ItemCodeFin, string Fecha, int idOperation = 1324)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Ventas_NEG.ReportesHana.VentSkuDias_N oN = new Capa_Negocio.Ventas_NEG.ReportesHana.VentSkuDias_N();
                ReportViewer rp = new ReportViewer();
                ViewBag.Mensaje = "";
                try
                {
                    List<Capa_Entidad.Ventas_ENT.ReportesHana.VentSkuDias_E> listaR = oN.RptVentSkuDias(DateTime.Parse(Fecha), ItemCodeIni, ItemCodeFin);
                    rp.ProcessingMode = ProcessingMode.Local;
                    rp.SizeToReportContent = true;
                    rp.LocalReport.ReportPath =
                        Request.MapPath(Request.ApplicationPath) + @"Reportes\RptVentas\RptVentasSkuDias.rdlc";
                    rp.LocalReport.DataSources.Add(new ReportDataSource("DS_VentasSkuDias", listaR));
                    ViewBag.REPORTE = rp;
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                }
                return View("reporteViewer");
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult infoVentasSkuCliDias(int idOperation = 1325)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Almacen_NEG.Tablas.OITM_N oitmN = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N();
                Capa_Negocio.SocioNegocios_NEG.TablasExternas.OCRD_N ocrdN = new Capa_Negocio.SocioNegocios_NEG.TablasExternas.OCRD_N();
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                ViewBag.ListaProductos = oitmN.Listar(null);
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult RptVentasSkuCliDias(string ItemCodeIni, string ItemCodeFin, string CardCodeIni, string CardCodeFin, string Fecha, int idOperation = 1325)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Capa_Negocio.Ventas_NEG.ReportesHana.VentSkuCliDias_N oN = new Capa_Negocio.Ventas_NEG.ReportesHana.VentSkuCliDias_N();
                ReportViewer rp = new ReportViewer();
                ViewBag.Mensaje = "";
                try
                {
                    List<Capa_Entidad.Ventas_ENT.ReportesHana.VentSkuCliDias_E> listaR = oN.RptVentSkuCliDias(DateTime.Parse(Fecha)
                        , ItemCodeIni, ItemCodeFin, CardCodeIni, CardCodeFin);
                    rp.ProcessingMode = ProcessingMode.Local;
                    rp.SizeToReportContent = true;
                    rp.LocalReport.ReportPath =
                        Request.MapPath(Request.ApplicationPath) + @"Reportes\RptVentas\RptVentSkuCliDias.rdlc";
                    rp.LocalReport.DataSources.Add(new ReportDataSource("DS_VentSkuCliDias", listaR));
                    ViewBag.REPORTE = rp;
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                }
                return View("reporteViewer");
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //metodos asincronos
        public JsonResult buscarClienteRegalo(string CardCode)
        {
            Capa_Negocio.Ventas_NEG.TablasSql.OCLR_N oclrN = new Capa_Negocio.Ventas_NEG.TablasSql.OCLR_N();
            return Json(oclrN.buscarClienteRegalo(CardCode));
        }
        /**********************************************************************************************************/
        public ActionResult reporteViewer(ReportViewer rp)
        {
            ViewBag.Mensaje = "";
            ViewBag.REPORTE = rp;
            return View();
        }
        public ActionResult PdfTicketVenta(int DocEntry)
        {
            //verificacionAccesos(0);
            try
            {
                ORTV_E t = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                try
                {
                    if (!String.IsNullOrWhiteSpace(t.LugarDestino))
                    {
                        if (t.LugarDestino == "Centro")
                        {
                            t.TiempoEntrega = Convert.ToDateTime(t.TiempoEntrega).AddMinutes(120);
                        }
                        else if (t.LugarDestino == "Arriola")
                        {
                            t.TiempoEntrega = Convert.ToDateTime(t.TiempoEntrega).AddMinutes(30);
                        }
                        else if (t.LugarDestino == "Domicilio" || t.LugarDestino == "Agencia")
                        {
                            t.TiempoEntrega = Convert.ToDateTime(t.TiempoEntrega).AddMinutes(15);
                        }
                    }
                    ViewBag.ColorTicket = ResaltarTicket(t.LugarDestino);
                }
                catch { }
                return View(t);
            }
            catch
            {
                return View();
            }
        }
        public ActionResult GenerarPDF(int DocEntry)
        {
            return RedirectToAction("PdfTicketVenta", new { DocEntry = DocEntry });
        }
        /***************** Formulario de ticket de venta *****************/
        public JsonResult infoContactosVentasSocio(string CardCode)
        {
            Capa_Negocio.SocioNegocios_NEG.TablasExternas.OCPR_N oN = new Capa_Negocio.SocioNegocios_NEG.TablasExternas.OCPR_N();
            return Json(oN.listarContactosVentasSocio(CardCode));
        }
        public ActionResult infoListaClientes(string Fecha)
        {
            return Content(_ticketN.generaInfoListaClientes(Fecha));
        }
        public ActionResult infoDirDestino(string CardCode)
        {
            return Content(_ticketN.generaInfoListaDirDestinos(CardCode));
        }
        public ActionResult infoListaOrdenesDeVenta(string Fecha, string CardCode, int DocNum)
        {
            var (htmlContent, tipoVenta) = _ticketN.generaInfoListaOrdenesDeVenta(Fecha, CardCode, DocNum);
            var response = new
            {
                HtmlContent = htmlContent,
                TipoVenta = tipoVenta
            };
            return Json(response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult infoListaNotasDeCreditoV(string CardCode)
        {
            return Content(_ticketN.generaInfoListaNotasDeCreditoV(CardCode));
        }
        /**************Calculos y validaciones ,objetos*****************/
        public ActionResult validarEditarClienteRegalo(OCLR_E obj)
        {
            string status = "true";
            OCLR_N oclrN = new OCLR_N();
            try
            {
                oclrN.validarEditarClienteRegalo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarGestionarStock(OREG_E obj1, OTRC_E obj2)
        {
            string status = "true";
            Capa_Negocio.Ventas_NEG.TablasSql.OREG_N oregN = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N();
            try
            {
                oregN.validarGestionStock(obj1, obj2);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarNuevoRegalo(OREG_E obj)
        {
            string status = "true";
            Capa_Negocio.Ventas_NEG.TablasSql.OREG_N oregN = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N();
            try
            {
                oregN.validarNuevoRegalo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarInactivarRegalo(int id)
        {
            string status = "true";
            Capa_Negocio.Ventas_NEG.TablasSql.OREG_N oregN = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N();
            try
            {
                oregN.validarInactivarRegalo(id);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarRevertirInactivar(int id)
        {
            string status = "true";
            Capa_Negocio.Ventas_NEG.TablasSql.OREG_N oregN = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N();
            try
            {
                oregN.validarRevertirInactivar(id);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult validarClienteRegalo(OCLR_E obj)
        {
            string status = "true";
            Capa_Negocio.Ventas_NEG.TablasSql.OCLR_N oclrN = new Capa_Negocio.Ventas_NEG.TablasSql.OCLR_N();
            try
            {
                oclrN.validarNuevoClienteRegalo(obj);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public JsonResult ObtieneDeudasSaldos(string CardCode)
        {
            return Json(_libroDeSaldoN.obtenerLibroSaldo(CardCode));
        }
        public JsonResult comprobarReclamosCliente(string CardCode, int? DocNumTicket = null)
        {
            Capa_Negocio.AtencionCliente_NEG.TablasSql.OSAT_N osatN = new Capa_Negocio.AtencionCliente_NEG.TablasSql.OSAT_N();
            Dictionary<string, string> ortv = new Dictionary<string, string> { { "CardCode", CardCode } };
            var filtro = new Capa_Entidad.AtencionCliente_ENT.TablasSql.OSAT_E()
            {
                DetORTV = ortv,
                TipoSolicitudCreaTicketVenta = "('Reclamo','Devolucion')",      // TipoSolicitudCreaTicketVenta: Filtro para el botón Reclamos Crea Ticket Venta
                TipoSolucionCreaTicketVenta = "('Regalo')",
                SoloSinTicketSolucion = true,
                Estado = "Atendido",
                TicketSolucion = DocNumTicket?.ToString()
            };
            return Json(osatN.ListarSolicitudes(filtro, false, false));
        }
        public JsonResult CalcularMontos(ORTV_E t)
        {
            return Json(_ticketN.CalcularMontos(t));
        }
        public ActionResult ValidarDatosTicket(ORTV_E t) //llamada desde ajax por formularios de ticket
        {
            string status = "true";
            try
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                t.Vendedor = $"{user.Nombres} {user.Apellidos}";
                t.WhsCodeLog = $"{user.WhsCode}";
                _ticketN.ValidarDatosTicket(t, 0);
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }
        }
        public ActionResult ExportarExcelArticulos(int DocEntry)
        {
            Capa_Negocio.Ventas_NEG.Tablas.ORDR_N ordrN = new Capa_Negocio.Ventas_NEG.Tablas.ORDR_N();
            ORTV_N tkN = new ORTV_N();
            string nombreArchivo = "ReporteArticulos.xlsx";
            string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            List<RTV2_E> listaOrdenes = tkN.obtenerDet2Ticket(DocEntry);
            List<int> listaDocNums = listaOrdenes.Select(item => item.NroSap).ToList();
            var detalleOrdenes = ordrN.listadoDetalleOrdenesDeVenta(listaDocNums);
            using (var libro = new ExcelPackage())
            {
                var worksheet = libro.Workbook.Worksheets.Add("ReporteArticulos");
                // Cargar datos en la hoja.
                worksheet.Cells["A1"].LoadFromCollection(detalleOrdenes, PrintHeaders: true);
                using (var headerRange = worksheet.Cells[1, 1, 1, detalleOrdenes.First().GetType().GetProperties().Length])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    headerRange.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                }
                for (var col = 1; col <= detalleOrdenes.First().GetType().GetProperties().Length; col++)
                {
                    worksheet.Column(col).AutoFit();
                }
                worksheet.Column(3).Style.Numberformat.Format = "dd/MM/yyyy";
                var allCells = worksheet.Cells[worksheet.Dimension.Address];
                allCells.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                allCells.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                allCells.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                allCells.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                if (detalleOrdenes != null && detalleOrdenes.Count >= 1)
                {
                    var range = new ExcelAddressBase(1, 1, detalleOrdenes.Count + 1, detalleOrdenes.First().GetType().GetProperties().Length);
                    var tabla = worksheet.Tables.Add(range, "ReporteArticulos");
                    tabla.ShowHeader = true;
                    tabla.TableStyle = OfficeOpenXml.Table.TableStyles.Medium4;
                }
                return File(libro.GetAsByteArray(), excelContentType, nombreArchivo);
            }
        }
        /**********Documentos imprimibles para el proceso de tickets (OPERACIONES) **************/
        public ActionResult PdfTacoComentarios(int DocEntry)
        {
            ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
            var agenciaStr = ticket.Agencia ?? string.Empty;
            int agenciaFill = _ticketN.AgenciaFill(agenciaStr);
            if (agenciaFill < 0) agenciaFill = 0;

            List<CC_ORTV_E> ticketAbierto = ccORTV_N.ListarCC_ORTV(DocEntry, "REGISTRAR");
            // Si el ticket no está ABIERTO y en el control de cambios nunca hubo un movimiento
            if (ticket.Estado != "ABIERTO" && ticketAbierto[0].FechaOperacion == "")
            {
                return RedirectToAction("ListadoTicketsRecepcion", new { DocNum = ticket.DocNum, Mensaje = "El ticket debe estar ABIERTO" });
            }
            return new ActionAsPdf("TacoComentarios", new { DocEntry = DocEntry, agencia = agenciaFill }) { FileName = "TacoComentario" + DocEntry + ".pdf", PageOrientation = Rotativa.Options.Orientation.Portrait, PageSize = Rotativa.Options.Size.A6 };
        }
        public ActionResult TacoComentarios(int DocEntry, int agencia)
        {
            try
            {
                ORTV_E t = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                t.isFill = agencia;
                if (t.TiempoEntrega != null)
                {
                    try
                    {
                        DateTime dt = Convert.ToDateTime(t.TiempoEntrega);
                        dt = dt.AddMinutes(-70);
                        t.TiempoEntrega = Convert.ToDateTime(dt.ToString("dd/MM/yyyy hh:mm tt"));
                    }
                    catch { }
                }
                return View(t);
            }
            catch { return View(new ORTV_E()); }
        }
        public ActionResult PdfRotuladoTicket(int DocEntry)
        {
            var pdfResult = new ActionAsPdf("RotuladoTicket", new { DocEntry = DocEntry })
            {
                FileName = "RotuladoTicket" + DocEntry + ".pdf",
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.A4
            };
            var pdfResponse = pdfResult.BuildFile(ControllerContext);
            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", "inline; filename=RotuladoTicket" + DocEntry + ".pdf");
            Response.BinaryWrite(pdfResponse);
            Response.End();
            return new EmptyResult();
        }
        public ActionResult RotuladoTicket(int DocEntry)
        {
            ORTV_N ortvN = new ORTV_N();
            object obj = null;
            try
            {
                obj = ortvN.ObtenerTicketRotulado(DocEntry);
            }
            catch { }
            return View(obj);
        }
        public ActionResult PdfTacoEmpaque(int DocEntry)
        {
            //verificacionAccesos(0);
            return new ActionAsPdf("TacoEmpaque", new { DocEntry = DocEntry }) { FileName = "PdfTacoEmpaque.pdf", PageOrientation = Rotativa.Options.Orientation.Portrait, PageSize = Rotativa.Options.Size.A6 };
        }
        public ActionResult TacoEmpaque(int DocEntry)
        {
            try
            {
                //verificacionAccesos(0);
                ORTV_N ortvN = new ORTV_N();
                var ticket = ortvN.ObtenerDatosCompletosTicket(DocEntry);
                if (EsEstadoEmpacado(ticket.Estado))
                {
                    ObtenerOperariosVerificacion(ticket, DocEntry);
                    ObtenerOperariosEmpacado(ticket, DocEntry);
                    AsignarGuias(ticket);
                    AsignarFechaHoraEmpacado(ticket);
                    AjustarTiempoEntrega(ticket);
                }
                else
                {
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = ticket.DocNum, Mensaje = "El ticket debe estar EMPACADO" });
                }
                ViewBag.Letra = 4;
                ViewBag.ColorTicket = ResaltarTicket(ticket.LugarDestino);
                return View(ticket);
            }
            catch
            {
                return View(new ORTV_E());
            }
        }
        private bool EsEstadoEmpacado(string estado)
        {
            return estado.Equals("EMPACADO") || estado.Equals("PREENVIO") || estado.Equals("ENVIADO") || estado.Equals("ENTREGADO");
        }
        private void ObtenerOperariosVerificacion(ORTV_E ticket, int docEntry)
        {
            var ticketVerificando = ccORTV_N.ListarCC_ORTV(docEntry, "FIN VERIFICAR");
            if (ticketVerificando.Any())
            {
                ticket.OpVerificado = ticketVerificando[0].Operario;
                var operariosChequeando = new RTV12_D().BuscarOperariosChequeando(ticket.DocEntry);
                if (operariosChequeando != null)
                {
                    ticket.OpVerificadoApoyo = operariosChequeando;
                }
            }
        }
        private void ObtenerOperariosEmpacado(ORTV_E ticket, int docEntry)
        {
            if (ticket.Cajas >= 1)
            {
                var ticketEmpacado = ccORTV_N.ListarCC_ORTV(docEntry, "FIN EMPACAR");
                var operariosEmpacando = new RTV13_D().BuscarOperariosEmpacando(docEntry);
                if (operariosEmpacando != null)
                {
                    ticket.OpEmpacadoApoyo = operariosEmpacando;
                }
            }
        }
        private void AsignarGuias(ORTV_E ticket)
        {
            if (ticket.LugarDestino.Equals("Arriola") || ticket.LugarDestino.Equals("Centro"))
            {
                var owtrN = new Capa_Negocio.Almacen_NEG.Tablas.OWTR_N();
                string whsCode = ticket.LugarDestino.Equals("Centro") ? "01" : "09";
                ViewBag.Guias = owtrN.GuiasTicketTransferencia(ticket.DocNum, whsCode, ticket.CardCode);
            }
            else
            {
                ViewBag.Guias = _ticketN.GuiasTicket(ticket.DocEntry);
            }
        }
        private void AsignarFechaHoraEmpacado(ORTV_E ticket)
        {
            var ticketEmpacado = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "FIN EMPACAR");
            if (ticketEmpacado.Any())
            {
                DateTime dt;
                if (DateTime.TryParse(ticketEmpacado[0].FechaOperacion, out dt))
                {
                    ticket.FechaEmpacado = dt.ToString("dd/MM/yyyy");
                    ticket.HoraEmpacado = ticketEmpacado[0].HoraOperacion;
                    ticket.OpEmpacado = ticketEmpacado[0].Operario;
                }
            }
        }
        private void AjustarTiempoEntrega(ORTV_E ticket)
        {
            if (ticket.TiempoEntrega != null)
            {
                DateTime dt = Convert.ToDateTime(ticket.TiempoEntrega);
                dt = dt.AddMinutes(-70);
                ticket.TiempoEntrega = Convert.ToDateTime(dt.ToString("dd/MM/yyyy hh:mm tt"));
            }
        }
        public ActionResult OrdenDeVenta(int DocNum)
        {
            try
            {
                ViewBag.Letra = 4;
                return View(_ticketN.obtenerOrdenDeVenta(DocNum));
            }
            catch { return View(); }
        }
        /****************************** E R R O R E S   P I C K I N G ******************************/
        public JsonResult RegistrarErroresPicking(OEP_E datos, List<EP1_E> detalleErroresPicking)
        {
            try
            {
                // Registrar el usuario que realiza el registro de errores
                Usuario_E u = (Usuario_E)Session["UsuarioId"];
                datos.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                // Llamar al método para registrar los errores de picking
                var result = new OEP_N().RegistrarErroresPicking(datos, detalleErroresPicking);
                return Json(new { DocNum = datos.DocNumTicket, Mensaje = result });                 // Devolver un JsonResult con el número de documento y un mensaje de resultado
            }
            catch (Exception e)
            {
                // En caso de error, devolver un JsonResult con un mensaje de error
                return Json(new { Titulo = e.Message });
            }
        }
        public ActionResult ExportarReporteErroresPicking(RptFiltrosErroresPicking_E filtros, int idOperation = 810)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);
            if (acceso == "C_Access")
            {
                OEP_N oepN = new OEP_N();
                string nombreArchivo = "ReporteErroresPicking.xlsx";
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var erroresPicking = oepN.ExportarReporteErroresPicking(filtros);
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("ReporteErroresPicking");
                    worksheet.Cells["A1"].LoadFromCollection(erroresPicking, PrintHeaders: true);
                    for (var col = 1; col <= 11; col++)
                    {
                        worksheet.Column(col).AutoFit();
                    }
                    if (erroresPicking != null)
                    {
                        if (erroresPicking.Count >= 1)
                        {
                            var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: erroresPicking.Count + 1, toColumn: 11), "ReporteErroresPicking");
                            tabla.ShowHeader = true;
                            tabla.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;
                        }
                    }
                    return File(libro.GetAsByteArray(), excelContentType, nombreArchivo);
                }
            }
            else { return null; }
        }
        /***************************** P A G O   C O N T R A E N T R E G A ************************/
        [HttpGet]
        public ActionResult AutorizarTicketReparto(int docEntry, int idOTC, string mensaje = null, int idOperation = 504)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                ORTV_E ticket = _ticketN.ObtenerDatosCompletosTicket(docEntry);
                ticket.MontoRecibido = ticket.MontoFinal;
                OTC_N otcN = new OTC_N();
                var result = otcN.ObtenerDatosTicketACuadrar(docEntry, idOTC);
                ViewBag.IdOTC = (result != null) ? result.IdOTC : 0;
                ViewBag.MontoRecibidoEfectivo = (result != null) ? result.MontoRecibidoEfectivo : 0;
                ViewBag.MontoRecibidoDeposito = (result != null) ? result.MontoRecibidoDeposito : 0;
                ViewBag.TipoPago = (result != null) ? result.TipoPago : string.Empty;
                ViewBag.DescTipoPago = (result != null) ? result.DescTipoPago : string.Empty;
                ViewBag.EstadoContraEntrega = (result != null) ? result.Estado : string.Empty;
                ViewBag.FechaCompromisoPago = (result != null) ? result.FechaCompromisoPago : string.Empty;
                ViewBag.SaldoAFavor = (result != null) ? result.SaldoAFavor : string.Empty;
                ViewBag.ComentarioCaja = (result != null) ? result.ComentarioCaja : string.Empty;
                ViewBag.ComentarioVentas = (result != null) ? result.ComentarioVentas : string.Empty;
                ViewBag.IdRol = usu.IdRol;
                CC_OTC_N cc_otcN = new CC_OTC_N();
                var datosCC = (result != null) ? cc_otcN.ObtenerDatosCC_OTC(result.IdOTC, "REGISTRAR") : null;
                ViewBag.Comprobantes = (datosCC != null) ? otcN.ObtenerComprobantePagoEfectivo((int)result.DocNumTicket, datosCC.FechaOperacion) : new List<string>();
                Capa_Negocio.Caja_NEG.OPP_N oppN = new Capa_Negocio.Caja_NEG.OPP_N();
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
        /****************************** E N T R E G A  M A S I V A ********************************/
        [HttpPost]
        public JsonResult gestionarEntregadoMasivo(int[] ticketsMasivo, int entregadoConRegalo)
        {
            //verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            ORTV_N ortvN = new ORTV_N();
            Usuario_E user = (Usuario_E)Session["UsuarioId"];
            List<Tickets> lista = new List<Tickets>();
            foreach (int DocEntry in ticketsMasivo)
            {
                string OpEntrega = user.Nombres + " " + user.Apellidos;
                Tickets result = new Tickets();
                try
                {
                    result = ortvN.entregarMasivoTicket(DocEntry, OpEntrega, entregadoConRegalo);
                }
                catch (Exception e)
                {
                    result.Mensaje = e.Message;
                }
                lista.Add(result);
            }
            return Json(lista);
        }
        [HttpPost]
        public JsonResult verTicketsNoEntregados(int[] arrTickets)
        {
            return Json(_ticketN.BuscarVariosTickets(arrTickets));
        }
        public ActionResult EditarTicketVentaSup(int DocEntry, int idOperation = 503)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                UBIG_N ubigN = new UBIG_N(); OUR1_N ofiN = new OUR1_N(); COUR_N couN = new COUR_N();
                ViewBag.Mensaje = "";
                ORTV_E t = _ticketN.ObtenerDatosCompletosTicket(DocEntry);
                ViewBag.Ubigeos = ubigN.Listar(null);
                ViewBag.Oficinas = ofiN.Listar();
                ViewBag.Agencias = couN.Listar();
                if (t.Estado.Equals("SEPARADO")) { return RedirectToAction("CreaTicketVenta", new { DocEntry = t.DocEntry }); }
                else { return View(t); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public ActionResult EditarTicketVentaSup(int DocEntry, ORTV_E t, int idOperation = 503)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    t.OpRegistro = $"{user.Nombres} {user.Apellidos}";
                    t.WhsCodeLog = $"{user.WhsCode}";
                    _ticketN.editarTicketSup(DocEntry, user.IdRol, t);
                    return RedirectToAction("ListadoTicketsVenta", new { DocNum = t.DocNum });
                }
                catch (Exception e)
                {
                    UBIG_N ubigN = new UBIG_N(); OUR1_N ofiN = new OUR1_N(); COUR_N couN = new COUR_N();
                    ViewBag.Mensaje = e.Message;
                    ViewBag.Ubigeos = ubigN.Listar(null);
                    ViewBag.Oficinas = ofiN.Listar();
                    ViewBag.Agencias = couN.Listar();
                    return View(t);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpPost]
        public JsonResult buscarOficinas(string nombreAgencia)
        {
            OUR1_N oN = new OUR1_N();
            //verificacionAccesos(0);
            return Json(oN.Listar().Where(x => x.NombreAgencia == nombreAgencia));
        }
        //Metodo para listar tickets pagados en caja para ventas
        public JsonResult ListarTicketsPresupuestoPagados(int DocEntryUsuario)
        {
            ORTV_N ortvN = new ORTV_N(); Usuario_N usuN = new Usuario_N(); Usuario_E u = usuN.buscarUsuario(DocEntryUsuario);
            var result = ortvN.ListarTicketsAreaVenta(u, new ORTV_E { Estado = "ABIERTO" }).Where(x => x.Presupuesto == "SI" && x.EstadoPago == "PAGADO").OrderBy(x => x.FechaPago + " " + x.HoraPago).ToList();
            return Json(new { Datos = result });
        }
        //Metodo que permite visibilidad a Recepcion
        public JsonResult CambiarPresupuestoTicket(int DocEntry)
        {
            //verificacionAccesos(0);
            ORTV_N ortvN = new ORTV_N(); var result = ortvN.EditarPresupuestoTicket(DocEntry);
            return Json(new { Datos = result });
        }
        public JsonResult CambiarVisibilidadTicket(int docEntry, string proceso)
        {
            //Se ejecuta desde la impresion de layout de un ticket de venta en ListadoTicketsVenta
            Usuario_E user = (Usuario_E)Session["UsuarioId"];
            var opImpresion = $"{user.Nombres} {user.Apellidos}";
            var result = new Capa_Negocio.Ventas_NEG.TablasSql.ORTV_N().EditarVisibilidadTicket(docEntry, opImpresion, proceso);
            return Json(new { NroTicket = result });
        }
        //Registra impresion de documentos de un ticket para despacho (centro y arriola)
        public JsonResult RegistrarImpresion(int docEntry, string area)
        {
            Usuario_E user = (Usuario_E)Session["UsuarioId"];
            var operario = $"{user.Nombres} {user.Apellidos}";
            ORTV_N ortvN = new ORTV_N();
            var result = ortvN.RegistrarImpresionTicket(docEntry, operario, area);
            return Json(new { Datos = result });
        }
        public void PreliminarLayoutOV_Ticket(int docEntry)
        {
            var ticket = new Capa_Negocio.Ventas_NEG.TablasSql.ORTV_N().ObtenerDatosTicketParaDocumentos(docEntry);
            // Crear un MemoryStream para el PDF combinado
            using (MemoryStream combinedPdfStream = new MemoryStream())
            {
                using (Document document = new Document())
                {
                    PdfCopy copy = new PdfCopy(document, combinedPdfStream);
                    document.Open();
                    // Generar PDF para cada orden de venta
                    foreach (var orden in ticket.Det2)
                    {
                        string fileName = $"OrdenDeVenta_{orden.NroSap}.pdf";
                        var pdfResult = GenerarPdfParaOrden(orden.NroSap, fileName, ticket.AlmProcedencia);
                        // Leer el PDF generado
                        using (var pdfReader = new PdfReader(pdfResult))
                        {
                            // Crear un MemoryStream para agregar la paginación
                            using (MemoryStream paginatedPdfStream = new MemoryStream())
                            {
                                using (PdfStamper stamper = new PdfStamper(pdfReader, paginatedPdfStream))
                                {
                                    int totalPages = pdfReader.NumberOfPages;
                                    // Agregar paginación a cada página
                                    for (int i = 1; i <= totalPages; i++)
                                    {
                                        PdfContentByte content = stamper.GetUnderContent(i);
                                        iTextSharp.text.Font font = FontFactory.GetFont("Arial", BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 10, iTextSharp.text.Font.BOLD);
                                        Phrase phrase = new Phrase($"Página {i} de {totalPages}", font);
                                        Phrase fecha = new Phrase($"{DateTime.Now}", font);
                                        Phrase docNumPhrase = new Phrase($"Nro Ticket: {ticket.DocNum}", font);
                                        ColumnText.ShowTextAligned(content, Element.ALIGN_LEFT, fecha, 30, 810, 0);
                                        ColumnText.ShowTextAligned(content, Element.ALIGN_CENTER, phrase, 300, 810, 0);
                                        ColumnText.ShowTextAligned(content, Element.ALIGN_RIGHT, docNumPhrase, 570, 810, 0);
                                    }
                                }
                                // Agregar el PDF paginado al documento combinado
                                using (var paginatedPdfReader = new PdfReader(paginatedPdfStream.ToArray()))
                                {
                                    copy.AddDocument(paginatedPdfReader);
                                }
                            }
                        }
                    }
                    document.Close();
                }
                // Guardar el PDF combinado en un archivo o devolverlo directamente
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "inline; filename=OrdenesDeVentaPreliminar.pdf");
                Response.BinaryWrite(combinedPdfStream.ToArray());
                Response.End();
            }
        }
        private byte[] GenerarPdfParaOrden(int NroSap, string fileName, string almProcedencia)
        {
            var pdfResult = new ActionAsPdf("PDF_OrdenesDeVentas", new { docNum = NroSap, almProcedencia = almProcedencia })
            {
                FileName = fileName,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = new Rotativa.Options.Margins(20, 10, 30, 10)
            };
            return pdfResult.BuildFile(ControllerContext);
        }
        public ActionResult PDF_OrdenesDeVentas(int docNum, string almProcedencia)
        {
            var lista = new ORTV_N().obtenerOrdenDeVenta(docNum);

            if (lista == null || !lista.Any())
                return Content("No se encontró la orden.");

            // Depende de LugarDestino del ticket
            if (lista != null && lista.Count > 0 && lista[0].Almacen != "ALM07" && (string.IsNullOrEmpty(almProcedencia) || almProcedencia == "16"))
            {
                var ubicacionesPorItem = new Dictionary<string, string[]>(); // Caché

                foreach (var ordr in lista)
                {
                    if (!ubicacionesPorItem.ContainsKey(ordr.ItemCode))
                    {
                        var ubicaciones = _ubicacionesLotesN.ListarUbicaciones(new Capa_Entidad.AbastecimientoInterno_ENT.TablasSql.UbicacionesLotes_E
                        {
                            ItemCode = ordr.ItemCode,
                            Almacen = "PICKING"
                        })
                        .Select(u => u.CodigoUbicacion)
                        .Where(c => !string.IsNullOrWhiteSpace(c))
                        .ToArray();

                        ubicacionesPorItem[ordr.ItemCode] = ubicaciones;
                    }

                    // Siempre asignar (cada línea debe tener su copia)
                    ordr.Ubicaciones = ubicacionesPorItem[ordr.ItemCode];

                    // Calcular la suma de la unidades vendidas por itemcode
                    ordr.TotalNumUnidVend = lista
                        .Where(x => x.ItemCode == ordr.ItemCode)
                        .Sum(x => x.NumUnidVend);
                }
            }

            lista = lista
                .OrderBy(x => x.Ubicaciones != null && x.Ubicaciones.Length > 0 ? x.Ubicaciones[0] : string.Empty)
                .ToList();

            List<OrdenDeVentaAgrupada_E> resultado = lista
                .GroupBy(x => new { x.Almacen, x.DocNum, x.NombreBd, x.Fecha, x.CardName, x.RucCliente, x.SlpName, x.DocTotal })
                .Select(doc => new OrdenDeVentaAgrupada_E
                {
                    Almacen = doc.Key.Almacen,
                    DocNum = doc.Key.DocNum,
                    NombreBd = doc.Key.NombreBd,
                    Fecha = Convert.ToDateTime(doc.Key.Fecha).ToString("dd/MM/yyyy"),
                    CardName = doc.Key.CardName,
                    RucCliente = doc.Key.RucCliente,
                    SlpName = doc.Key.SlpName,
                    DocTotal = doc.Key.DocTotal,

                    ItemCodeDetalle = doc
                        .Select((x, idx) => new
                        {
                            x.ItemCode,
                            x.UniMedidVend,
                            x.NumUnid,
                            x.Producto,
                            x.Laboratorio,
                            x.Comentarios,
                            x.Ubicaciones,
                            x.Lote,
                            x.FechaVenc,
                            x.NumUnidVend,
                            x.PrecioProdIgvVend,
                            x.TotalProdIgvVend,
                            x.CantidadSolicitadaVenta,
                            Grupo = x.UniMedidVend?.Trim().ToUpper() == "F" ? $"{x.ItemCode}_idx{idx}" : x.ItemCode,            // Agrupador por F o por código
                            x.TotalNumUnidVend
                        })
                        .ToList()
                        .GroupBy(x => x.Grupo)
                        .Select(g => new OVItemCodeDetalle_E
                        {

                            Codigo = g.First().ItemCode,
                            Producto = g.First().Producto,
                            Laboratorio = g.First().Laboratorio,
                            Comentarios = g.First().Comentarios,
                            Ubicaciones = g.First().Ubicaciones,
                            TotalUnidadesVendidas = g.First().TotalNumUnidVend,

                            LoteDetalle = g
                                    .GroupBy(l => l.Lote)
                                    .Select(l => new OVLoteDetalle_E
                                    {
                                        Lote = l.Key,
                                        FechaVenc = l.First().FechaVenc,
                                        NumUnidVend = l.Sum(x => x.NumUnidVend),
                                        PrecioProdIgvVend = l.First().PrecioProdIgvVend,
                                        TotalProdIgvVend = l.First().TotalProdIgvVend,
                                        UniMedidVend = l.First().UniMedidVend,
                                        CantidadSolicitadaVenta = l.First().CantidadSolicitadaVenta
                                    }).ToList()
                        }).ToList()
                }).ToList();

            ViewBag.AlmProcedencia = almProcedencia;

            return View("~/Views/Ventas/PDF/PDF_OrdenesDeVentasSophos.cshtml", resultado);
        }
        [HttpPost]
        public JsonResult GuardarObservacion(int docEntry, string observacion)
        {
            try
            {
                var exito = _ticketN.GuardarComentario(docEntry, observacion);
                if (exito)
                    return Json(new { success = true, message = "Observación guardada correctamente." });
                else
                    return Json(new { success = false, message = "No se pudo guardar la observación." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public JsonResult LeerObservacion(int docEntry)
        {
            try
            {
                var comentario = _ticketN.LeerComentario(docEntry);
                return Json(new { success = true, comentario = comentario ?? "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, comentario = "", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult quitarRegalo(int docEntry)
        {
            try
            {
                var ticket = _ticketN.ObtenerDatosCompletosTicket(docEntry);
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                var operario = $"{usu.Nombres} {usu.Apellidos}";
                if (ticket == null)
                    return Json(new { success = false, mensaje = "Ticket no encontrado" });
                ticket.Operario = operario;
                if (ticket.Det5 != null && ticket.Det5.Count > 0)
                {
                    // Reset regalo
                    ticket.Det5[0].IdReg = 0;
                    ticket.Det5[0].RegCant = 0;
                    ticket.Det5[0].RegCate = null;
                    ticket.Det5[0].RegTipo = null;
                }

                _ticketN.EditarRegalo(docEntry, ticket);

                return Json(new { success = true, mensaje = "Regalo retirado del ticket" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

    }
}
