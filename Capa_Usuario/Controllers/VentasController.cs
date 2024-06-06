using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.AtencionCliente_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.Reportes;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Capa_Negocio.Almacen_NEG.Tablas;
using Capa_Negocio.AtencionCliente_NEG.TablasSql;
using Capa_Negocio.General_NEG.TablasSql;
using Capa_Negocio.Operaciones_NEG.TablasSql;
using Capa_Negocio.Rutas_NEG.TablasSql;
using Capa_Negocio.Seguridad_NEG;
using Capa_Negocio.SocioNegocios_NEG.Tablas;
using Capa_Negocio.Ventas_NEG.Tablas;
using Capa_Negocio.Ventas_NEG.TablasSql;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DocumentFormat.OpenXml.Drawing;

//using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Reporting.WebForms;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Table;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.util;
using System.Web;
using System.Web.Mvc;
using System.Windows;

namespace Capa_Usuario.Controllers
{
    public class VentasController : Controller
    {
        Usuario_N u_N = new Usuario_N(); Rol1_N rol1 = new Rol1_N(); int modulo = 5;
        ORTV_N ticketN = new ORTV_N(); OLDS_N lN = new OLDS_N();
        CC_ORTV_N ccORTV_N = new CC_ORTV_N();       // Control de cambios de estado de tickets	
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

        public JsonResult ObtenerDatosTicket(int docEntry)
        {
            verificacionAccesos(0);         // Validar sesion logueada, solo para ajax

            try
            {
                ORTV_N ortvN = new ORTV_N();
                var result = ortvN.obtenerTicket(docEntry);

                return Json(new { Ticket = result });
            }
            catch (Exception e)
            {
                return Json(new { Mensaje = e.Message });
            }
        }

        public ActionResult ListadoTickets(int DocNum = 0, ORTV_E t = null, string mensaje = null, int idOperation = 501)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                ViewBag.DocEntryUsuario = user.DocEntry;
                ViewBag.IdRol = user.IdRol;
                ViewBag.ListaTicketsSeparados = ticketN.listarTicketsSeparados(user.CodigoSap);
                ViewBag.DocNum = DocNum;
                ViewBag.Ortv = t;
                ViewBag.Vendedores = u_N.listaUsuariosPermisos(null, 6);        // Usado como Filtro en el botón AnVentas (Reporte Analítico Ventas)
                if (mensaje != null) { ViewBag.Mensaje = mensaje; }
                t.NombreVista = "ListadoTickets";

                return View(ticketN.listarTicketsVenta(user, t));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public JsonResult buscarTicket(int DocNum = 0)
        {
            Usuario_E user = (Usuario_E)Session["UsuarioId"];
            ORTV_N ortvN = new ORTV_N();
            ORTV_E ticket = ortvN.listarTicketsVenta(user, new ORTV_E { DocNum = DocNum }).FirstOrDefault();
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
        public ActionResult CreaTicketVenta(int DocEntry = 0, int idOperation = 502)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                UBIG_N ubigN = new UBIG_N(); OCRD_N oN = new OCRD_N(); OUR1_N ofiN = new OUR1_N();
                COUR_N couN = new COUR_N();
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                ViewBag.Mensaje = "";
                ViewBag.ProveedoresConContactos = oN.listarSociosConContactos();
                ViewBag.Ubigeos = ubigN.Listar();
                ViewBag.Oficinas = ofiN.Listar();
                ViewBag.Agencias = couN.Listar();
                ViewBag.Usuario = $"{user.Prefijo}{user.Id}";
                if (DocEntry > 0) { return View(ticketN.obtenerTicket(DocEntry)); }
                else { return View(ticketN.separarTicket(user)); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult CreaTicketVenta(ORTV_E ticket, int idOperation = 502)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    ticket.OpRegistro = $"{user.Nombres} {user.Apellidos}";
                    ticket.WhsCodeLog = $"{user.WhsCode}";
                    int DocNum = ticketN.registrarTicket(ticket);
                    return RedirectToAction("ListadoTickets", new { DocNum = DocNum });
                }
                catch (Exception e)
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    UBIG_N ubigN = new UBIG_N(); OCRD_N oN = new OCRD_N(); OUR1_N ofiN = new OUR1_N();
                    COUR_N couN = new COUR_N();
                    ViewBag.Mensaje = e.Message;
                    ViewBag.ProveedoresConContactos = oN.listarSociosConContactos();
                    ViewBag.Agencias = couN.Listar();
                    ViewBag.Ubigeos = ubigN.Listar();
                    ViewBag.Oficinas = ofiN.Listar();
                    ViewBag.Usuario = $"{user.Prefijo}{user.Id}";
                    return View(ticket);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult EditarTicketVenta(int DocEntry, int idOperation = 503)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                UBIG_N ubigN = new UBIG_N(); OUR1_N ofiN = new OUR1_N(); OCRD_N oN = new OCRD_N();
                OCLR_N oclrN = new OCLR_N(); COUR_N couN = new COUR_N();
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];

                ViewBag.ProveedoresConContactos = oN.listarSociosConContactos();
                ORTV_E t = ticketN.obtenerTicket(DocEntry);

                ViewBag.Mensaje = "";
                if (t.EstadoPago != null)
                {
                    if (t.EstadoPago.Equals("PAGADO"))
                    {
                        ViewBag.Mensaje = "El ticket se encuentra PAGADO, anular pago para Editar Ticket";
                    }
                }

                ViewBag.ClienteRegalo = oclrN.buscarClienteRegalo(t.CardCode);
                ViewBag.Ubigeos = ubigN.Listar();
                ViewBag.Oficinas = ofiN.Listar();
                ViewBag.Agencias = couN.Listar();
                ViewBag.IdRol = usu.IdRol;

                if (t.Estado.Equals("SEPARADO")) { return RedirectToAction("CreaTicketVenta", new { DocEntry = t.DocEntry }); }
                else { return View(t); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }

        }
        [HttpPost]
        public ActionResult EditarTicketVenta(int DocEntry, ORTV_E t, int idOperation = 503)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    t.Vendedor = $"{user.Nombres} {user.Apellidos}";     // Seteamos el usuario Propietario con el nombre del usuario en sesiòn
                    t.WhsCodeLog = $"{user.WhsCode}";
                    ticketN.editarTicket(DocEntry, t);
                    return RedirectToAction("ListadoTickets", new { DocNum = t.DocNum });
                }
                catch (Exception e)
                {
                    UBIG_N ubigN = new UBIG_N(); OCRD_N oN = new OCRD_N(); OCLR_N oclrN = new OCLR_N();
                    OUR1_N ofiN = new OUR1_N(); COUR_N couN = new COUR_N();
                    ViewBag.ProveedoresConContactos = oN.listarSociosConContactos();
                    ViewBag.Mensaje = e.Message;
                    ViewBag.Ubigeos = ubigN.Listar();
                    ViewBag.Oficinas = ofiN.Listar(); ViewBag.Agencias = couN.Listar();
                    ViewBag.ClienteRegalo = oclrN.buscarClienteRegalo(t.CardCode);
                    return View(t);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

        public ActionResult SeguimientoDeTicket(int DocEntry, string Mensaje, int idOperation = 507)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ORRU_N orruN = new ORRU_N();
                try
                {
                    ViewBag.BtnNuevaSolicitud = false;
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    if (usu.IdRol == 1 || usu.IdRol == 11)
                    { ViewBag.BtnNuevaSolicitud = true; }

                    RTV6_N rtv6_N = new RTV6_N();
                    ORTV_E ticket = ticketN.obtenerTicket(DocEntry);
                    ticket.orru = orruN.obtenerOrdenDeRutaTicket(DocEntry);
                    ViewBag.flujoEstadosTicket = ccORTV_N.ListarCC_FlujoEstados(DocEntry);

                    ticket.hayIniPicking = false;
                    ticket.hayIniVerificar = false;
                    ticket.hayIniEmpacar = false;
                    ticket.hayFinPicking = false;
                    ticket.hayFinVerificar = false;
                    ticket.hayFinEmpacar = false;
                    ticket.hayRecibir = false; ticket.hayEnviar = true; ticket.hayEntregar = false;

                    // Revisamos si hay RECIBIR
                    List<CC_ORTV_E> tkRecibido = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "RECIBIR");
                    List<CC_ORTV_E> tkAnRecibido = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "ANULAR RECIBIR");
                    List<CC_ORTV_E> listRecibir = new List<CC_ORTV_E>() { tkRecibido[0], tkAnRecibido[0] };
                    var listRecibirOrd = listRecibir.OrderByDescending(x => x.Id);
                    if (listRecibirOrd.FirstOrDefault().Operacion == "RECIBIR") { ticket.hayRecibir = true; }
                    else if (listRecibirOrd.FirstOrDefault().Operacion == "ANULAR RECIBIR") { ticket.hayRecibir = false; }
                    // Revisamos si hay ENVIAR
                    List<CC_ORTV_E> tkEnviado = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "ENVIAR");
                    List<CC_ORTV_E> tkAnEnviado = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "ANULAR ENVIAR");
                    List<CC_ORTV_E> listEnviar = new List<CC_ORTV_E>() { tkEnviado[0], tkAnEnviado[0] };
                    var listEnviarOrd = listEnviar.OrderByDescending(x => x.Id);
                    if (listEnviarOrd.FirstOrDefault().Operacion == "ENVIAR") { ticket.hayEnviar = true; }
                    else if (listEnviarOrd.FirstOrDefault().Operacion == "ANULAR ENVIAR") { ticket.hayEnviar = false; }
                    // Revisamos si hay ENTREGAR
                    List<CC_ORTV_E> tkEntregar = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "ENTREGAR");
                    List<CC_ORTV_E> tkAnEntregar = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "ANULAR ENTREGAR");
                    List<CC_ORTV_E> listEntregar = new List<CC_ORTV_E>() { tkEntregar[0], tkAnEntregar[0] };
                    var listEntregarOrd = listEntregar.OrderByDescending(x => x.Id);
                    if (listEntregarOrd.FirstOrDefault().Operacion == "ENTREGAR") { ticket.hayEntregar = true; }
                    else if (listEntregarOrd.FirstOrDefault().Operacion == "ANULAR ENTREGAR") { ticket.hayEntregar = false; }

                    // Revisamos si hay INICIO PICKING
                    List<CC_ORTV_E> ticketIniPicking = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "INICIO PICKING");
                    // Revisamos si hay ANULAR INICIO PICKING
                    List<CC_ORTV_E> ticketAnularIniPicking = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "ANULAR INICIO PICKING");
                    List<CC_ORTV_E> listaIPick = new List<CC_ORTV_E>() { ticketIniPicking[0], ticketAnularIniPicking[0] };
                    var listaIPickOrd = listaIPick.OrderByDescending(x => x.Id);
                    if (listaIPickOrd.FirstOrDefault().Operacion == "INICIO PICKING") { ticket.hayIniPicking = true; }
                    else if (listaIPickOrd.FirstOrDefault().Operacion == "ANULAR INICIO PICKING") { ticket.hayIniPicking = false; }

                    // Revisamos si hay INICIO VERIFICAR
                    List<CC_ORTV_E> ticketIniVerificar = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "INICIO VERIFICAR");
                    // Revisamos si hay ANULAR INICIO VERIFICAR
                    List<CC_ORTV_E> ticketAnularIniVerificar = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "ANULAR INICIO VERIFICAR");
                    List<CC_ORTV_E> listaVerif = new List<CC_ORTV_E>() { ticketIniVerificar[0], ticketAnularIniVerificar[0] };
                    var listaVerifOrd = listaVerif.OrderByDescending(x => x.Id);
                    if (listaVerifOrd.FirstOrDefault().Operacion == "INICIO VERIFICAR") { ticket.aptoIniVerificar = false; ticket.hayIniVerificar = true; }
                    else if (listaVerifOrd.FirstOrDefault().Operacion == "ANULAR INICIO VERIFICAR") { ticket.aptoIniVerificar = true; ticket.hayIniVerificar = false; }

                    // Revisamos si hay INICIO EMPACAR
                    List<CC_ORTV_E> ticketIniEmpacar = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "INICIO EMPACAR");
                    // Revisamos si hay ANULAR INICIO EMPACAR
                    List<CC_ORTV_E> ticketAnularIniEmpacar = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "ANULAR INICIO EMPACAR");
                    List<CC_ORTV_E> listaEmp = new List<CC_ORTV_E>() { ticketIniEmpacar[0], ticketAnularIniEmpacar[0] };
                    var listaEmpOrd = listaEmp.OrderByDescending(x => x.Id);
                    if (listaEmpOrd.FirstOrDefault().Operacion == "INICIO EMPACAR") { ticket.hayIniEmpacar = true; }
                    else if (listaEmpOrd.FirstOrDefault().Operacion == "ANULAR INICIO EMPACAR") { ticket.hayIniEmpacar = false; }

                    // Revisamos si hay FIN PICKING
                    List<CC_ORTV_E> ticketFinPicking = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "FIN PICKING");
                    // Revisamos si hay ANULAR FIN PICKING
                    List<CC_ORTV_E> ticketAnularFinPicking = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "ANULAR FIN PICKING");
                    List<CC_ORTV_E> listaPicking = new List<CC_ORTV_E>() { ticketFinPicking[0], ticketAnularFinPicking[0] };
                    var listaPickingOrd = listaPicking.OrderByDescending(x => x.Id);
                    if (listaPickingOrd.FirstOrDefault().Operacion == "FIN PICKING") { ticket.hayFinPicking = true; }
                    else if (listaPickingOrd.FirstOrDefault().Operacion == "ANULAR FIN PICKING") { ticket.hayFinPicking = false; }

                    // Revisamos si hay FIN VERIFICAR
                    List<CC_ORTV_E> ticketFinVerificar = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "FIN VERIFICAR");
                    // Revisamos si hay ANULAR FIN VERIFICAR
                    List<CC_ORTV_E> ticketAnularFinVerificar = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "ANULAR FIN VERIFICAR");
                    List<CC_ORTV_E> listaFVerif = new List<CC_ORTV_E>() { ticketFinVerificar[0], ticketAnularFinVerificar[0] };
                    var listaFVerifOrd = listaFVerif.OrderByDescending(x => x.Id);
                    if (listaFVerifOrd.FirstOrDefault().Operacion == "FIN VERIFICAR") { ticket.hayFinVerificar = true; }
                    else if (listaFVerifOrd.FirstOrDefault().Operacion == "ANULAR FIN VERIFICAR") { ticket.hayFinVerificar = false; }

                    // Revisamos si hay FIN EMPACAR
                    List<CC_ORTV_E> ticketFinEmpacar = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "FIN EMPACAR");
                    // Revisamos si hay ANULAR FIN EMPACAR
                    List<CC_ORTV_E> ticketAnularFinEmpacar = ccORTV_N.ListarCC_ORTV(ticket.DocEntry, "ANULAR FIN EMPACAR");
                    List<CC_ORTV_E> listaFEmpac = new List<CC_ORTV_E>() { ticketFinEmpacar[0], ticketAnularFinEmpacar[0] };
                    var listaFEmpacOrd = listaFEmpac.OrderByDescending(x => x.Id);
                    if (listaFEmpacOrd.FirstOrDefault().Operacion == "FIN EMPACAR") { ticket.hayFinEmpacar = true; }
                    else if (listaFEmpacOrd.FirstOrDefault().Operacion == "ANULAR FIN EMPACAR") { ticket.hayFinEmpacar = false; }

                    /**************peso total******************/
                    if (ticket.Det6 != null && ticket.Det6.Count >= 1)
                    { ViewBag.pesoTotal = rtv6_N.ObtenerPesoTotal(DocEntry); }

                    ViewBag.Mensaje = Mensaje;
                    ViewBag.NameBotonEstado = "";
                    ViewBag.ValueBotonEstado = "";
                    ViewBag.MostrarBotonCambiarEstado = false;
                    ViewBag.BtnAnularRecibido = "";
                    //ViewBag.BtnAnularEntregado = "<button class=\"btn btn-sm btn-secondary\" disabled>ANULAR ENT.</button>";

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
                        /*else if (ticket.Estado.Equals("PESADO") || ticket.Estado.Equals("EMPACADO") && (permisoAlm || usu.IdRol.Equals(52)))
                        {
                            ViewBag.NameBotonEstado = "ENTREGADO";
                            ViewBag.ValueBotonEstado = "CAMBIAR A ENTREGADO";
                            ViewBag.MostrarBotonCambiarEstado = true;
                        }
                        else if (ticket.Estado.Equals("ENTREGADO") && (usu.IdRol.Equals(53) || usu.IdRol.Equals(1) || usu.IdRol.Equals(6)))
                        {
                            ViewBag.BtnAnularEntregado = "<input class=\"btn btn-sm btn-danger\" type=\"submit\" name=\"ANULARENTREGADO\" value=\"ANULAR ENTREGADO\" />";
                        }*/
                    }
                    ViewBag.IdRol = usu.IdRol;
                    return View(ticket);
                }
                catch
                {
                    return RedirectToAction("ListadoTickets");
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult SeguimientoDeTicket(int DocEntry, ORTV_E t, int idOperation = 507)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {

                ORRU_N orruN = new ORRU_N();
                try
                {
                    ViewBag.Mensaje = string.Empty;
                    RTV6_N rtv6_N = new RTV6_N();
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = ticketN.obtenerTicket(DocEntry);
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

                    t = ticketN.obtenerTicket(DocEntry);
                    t.orru = orruN.obtenerOrdenDeRutaTicket(DocEntry);
                    ViewBag.flujoEstadosTicket = ccORTV_N.ListarCC_FlujoEstados(DocEntry);
                    ViewBag.ultimoEstadoTicket = ccORTV_N.UltimoEstadoCC_ORTV(DocEntry);
                    ViewBag.pesoTotal = rtv6_N.ObtenerPesoTotal(DocEntry);
                    ViewBag.IdRol = usu.IdRol;

                    return RedirectToAction("SeguimientoDeTicket", new { DocEntry = DocEntry, Mensaje = ViewBag.Mensaje });
                }
                catch
                {
                    return RedirectToAction("ListadoTickets");
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        //op 508--517
        public ActionResult CancelarTicket(int DocEntry, string vista, int idOperation = 518)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ORTV_N ortvN = new ORTV_N();
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    string Operario = $"{usu.Nombres} {usu.Apellidos}";
                    int DocNum = ortvN.cancelarTicket(DocEntry, Operario, usu.IdRol);

                    return RedirectToAction(vista, new { DocNum = DocNum });

                }
                catch (Exception e)
                {
                    return RedirectToAction(vista, new { Mensaje = e.Message });
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        /*FORMATO DE AGENCIA*/
        public ActionResult RptFormatoAgencia(int idOperation = 520)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ReportClass rc = new ReportClass();
                rc.FileName = Server.MapPath("/Reportes/RptVentas/RptFormatoAgencia.rpt");

                Capa_Negocio.Utilitarios_N utiN = new Capa_Negocio.Utilitarios_N();
                var coninfo = utiN.getConexion();
                TableLogOnInfo logoninfo = new TableLogOnInfo();
                Tables tables;
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptFormatoAgenciaExcel(int idOperation = 521)

        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
                    tabla.TableStyle = TableStyles.Medium2;
                    return File(libro.GetAsByteArray(), excelContentType, "FormatoAgencia.xlsx");
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login") { return null; }
            else { return null; }
        }
        public void VerificarOpSeguimiento(/*string estado, int docEntry, int docNum, string tipoMantenimiento*/ Dictionary<string, Object> datos, string Request)
        {
            verificacionAccesos(0);
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
            if (verificacionAccesos(Op).Equals("C_Access"))
            {
                //ticketN.editarSeguimientoTicket(estado, DocEntry, tc);
                ticketN.EditarTicketDesdeSeguimiento(datos, Request);
            }
            else
            {
                throw new Exception("Error Ud. no tiene permiso esta operacion.");
            }

        }
        /*public ActionResult ListadoTicketsGuiasRemision(int DocNum = 0, ORTV_E ticket = null, string Mensaje = "", int idOperation = 2801)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                ViewBag.DocNum = DocNum;
                ViewBag.Ortv = ticket;
                if (string.IsNullOrEmpty(ticket.EstadoFacturacion)) { ticket.EstadoFacturacion = "PENDIENTE"; }
                ViewBag.Mensaje = Mensaje;
                return View(ticketN.listarTicketsVenta(user, ticket));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }*/
        public ActionResult ListadoTicketsFacturacion(int DocNum = 0, ORTV_E ticket = null, string Mensaje = "", int idOperation = 601)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"]; ORTV_N tkN = new ORTV_N();
                ViewBag.DocNum = DocNum;
                //Si el DocNum es diferente a 0 todos los datos necesarios del ticket se llenan en ViewBag.Ortv ( para que muestre en el filtro)
                if (DocNum > 0)
                {
                    var DocEntry = DocNum - 2000000000;
                    var ticketUnico = tkN.obtenerTicket(DocEntry);
                    ticket.LugarDestino = ticketUnico.LugarDestino;
                    ticket.Estado = ticketUnico.Estado;
                    ticket.EstadoFacturacion = ticketUnico.EstadoFacturacion;
                    ticket.FechaSapTicket = ticketUnico.FechaSapTicket;
                    ViewBag.Ortv = ticket;
                }
                else { ViewBag.Ortv = ticket; }
                ViewBag.Mensaje = Mensaje;
                return View(ticketN.listarTicketsVenta(user, ticket));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult EmitirGuiasTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 2802)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
                    Mensaje = string.Empty
                };
                bool hayFinVerificar = false; int DocNum = 0;
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    List<CC_ORTV_E> ticketFinVerificar = ccORTV_N.ListarCC_ORTV(DocEntry, "FIN VERIFICAR");
                    List<CC_ORTV_E> ticketAnularFinVerificar = ccORTV_N.ListarCC_ORTV(DocEntry, "ANULAR FIN VERIFICAR");
                    List<CC_ORTV_E> listaCC = new List<CC_ORTV_E>() { ticketFinVerificar[0], ticketAnularFinVerificar[0] }.OrderByDescending(x => x.Id).ToList();
                    if (listaCC.FirstOrDefault().Operacion == "FIN VERIFICAR") { hayFinVerificar = true; }
                    else if (listaCC.FirstOrDefault().Operacion == "ANULAR FIN VERIFICAR") { hayFinVerificar = false; }
                    if (hayFinVerificar)
                    {
                        ORTV_N negtik = new ORTV_N();
                        ORTV_E ticket = negtik.obtenerTicket(DocEntry); string Guias = "";
                        if (ticket.LugarDestino.Equals("Arriola") || ticket.LugarDestino.Equals("Centro"))
                        {
                            string WhsCode = string.Empty;
                            Capa_Negocio.Almacen_NEG.Tablas.OWTR_N owtrN = new Capa_Negocio.Almacen_NEG.Tablas.OWTR_N();
                            if (ticket.LugarDestino.Equals("Centro")) { WhsCode = "01"; }
                            else if (ticket.LugarDestino.Equals("Arriola")) { WhsCode = "09"; }

                            Guias = owtrN.GuiasTicketTransferencia(ticket.DocNum, WhsCode);
                        }
                        else
                        {
                            Guias = ticketN.GuiasTicket(DocEntry);
                        }
                        //verificamos guias existentes desde SAP
                        if (!string.IsNullOrEmpty(Guias) && Guias.Length > 6)
                        {
                            //pasa EstadoFacturacion a GRE EMITIDA
                            DocNum = ticketN.emitirGuia(DocEntry, u);
                            //datos.DocNum = DocNum;
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RevertirEmitirGuiasTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 2803)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
                    Mensaje = string.Empty
                };

                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    String operario = $"{usu.Nombres} {usu.Apellidos}";
                    int DocNum = ticketN.revertirGuiasTicket(DocEntry, operario);
                    return RedirectToAction("ListadoTicketsFacturacion", datos);
                }
                catch (Exception e)
                {
                    datos.Mensaje = e.Message;
                    return RedirectToAction("ListadoTicketsFacturacion", datos);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult FacturarTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 602)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
                    Mensaje = string.Empty
                };

                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    int DocNum = ticketN.facturarTicket(DocEntry, u);
                    //datos.DocNum = DocNum;
                    return RedirectToAction("ListadoTicketsFacturacion", datos);

                }
                catch (Exception e)
                {
                    datos.Mensaje = e.Message;
                    return RedirectToAction("ListadoTicketsFacturacion", datos);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AnularFacturarTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 603)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
                    Mensaje = string.Empty
                };

                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    String operario = $"{usu.Nombres} {usu.Apellidos}";
                    int DocNum = ticketN.revertirFacturarTicket(DocEntry, operario);
                    return RedirectToAction("ListadoTicketsFacturacion", datos);
                }
                catch (Exception e)
                {
                    datos.Mensaje = e.Message;
                    return RedirectToAction("ListadoTicketsFacturacion", datos);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

        /*
         * 
         * Buscar Facturas y Boletas relacionadas al ticket
         * 
         */
        public JsonResult buscarFacturasyBoletas(int DocEntry)
        {
            Capa_Negocio.Ventas_NEG.Tablas.ORDR_N ordrN = new Capa_Negocio.Ventas_NEG.Tablas.ORDR_N(); ORTV_N negtik = new ORTV_N(); OINV_N oinvNeg = new OINV_N();

            ORTV_E obj = negtik.obtenerTicket(DocEntry);
            List<string> FB = new List<string>();
            foreach (var orden in obj.Det2)
            {
                List<Capa_Entidad.Ventas_ENT.Tablas.ORDR_E> Ordenes = ordrN.listadoOrdenesDeVenta(new Capa_Entidad.Ventas_ENT.Tablas.ORDR_E { DocNum = orden.NroSap });
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
                if (!string.IsNullOrEmpty(o) && o.Contains("F"))
                {
                    lista.Add(oinvNeg.listadoFacturasDeVenta(new OINV_E { NumAtCard = o }).FirstOrDefault());
                }
                else if (!string.IsNullOrEmpty(o) && o.Contains("B"))
                {
                    lista.Add(oinvNeg.listadoBoletasDeVenta(new OINV_E { NumAtCard = o }).FirstOrDefault());
                }
            }
            return Json(lista);
        }
        public JsonResult buscarGuias(int DocEntry)
        {
            ORTV_N negtik = new ORTV_N();
            List<Guia_Remision_E> lista = new List<Guia_Remision_E>();
            ORTV_E ticket = negtik.obtenerTicket(DocEntry); string Guias;
            if (ticket.LugarDestino.Equals("Arriola") || ticket.LugarDestino.Equals("Centro"))
            {
                string WhsCode = string.Empty;
                Capa_Negocio.Almacen_NEG.Tablas.OWTR_N owtrN = new Capa_Negocio.Almacen_NEG.Tablas.OWTR_N();
                if (ticket.LugarDestino.Equals("Centro")) { WhsCode = "01"; }
                else if (ticket.LugarDestino.Equals("Arriola")) { WhsCode = "09"; }
                Guias = owtrN.GuiasTicketTransferencia(ticket.DocNum, WhsCode);
            }
            else
            {
                Guias = ticketN.GuiasTicket(DocEntry);
            }
            //separar las guias concatenadas desde Guias string
            Guias = Guias.Trim(); string[] guiasSeparadas = Guias.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < guiasSeparadas.Count(); i++)
            {
                ODLN_N odlN = new ODLN_N(); OWTR_N owtrN = new OWTR_N();
                List<Guia_Remision_E> resultGuias = odlN.buscarGuiaRemisionSap(guiasSeparadas[i]);
                if (resultGuias != null && resultGuias.Count() > 0)
                {
                    if (!string.IsNullOrEmpty(resultGuias[0].NumAtCard)) { lista.Add(odlN.buscarGuiaRemisionSap(guiasSeparadas[i])[0]); }
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

        public ActionResult ListadoTicketsRecepcion(int DocNum = 0, ORTV_E ticket = null, string Mensaje = "", int idOperation = 701)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                ViewBag.DocNum = DocNum;
                ViewBag.Ortv = ticket;
                ViewBag.Mensaje = Mensaje;
                ViewBag.IdRol = user.IdRol;
                return View(ticketN.listarTicketsVenta(user, ticket));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RecibirTicketVenta(int DocEntry, int idOperation = 702)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Capa_Negocio.Ventas_NEG.Tablas.ORDR_N ordrN = new Capa_Negocio.Ventas_NEG.Tablas.ORDR_N();
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E t = ticketN.obtenerTicket(DocEntry);
                    t.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    //validar que las ordenes de ventas esten vigentes, sino no puede recibir
                    bool todasvigentes = true; int DocNum = 0; int num = 0;
                    foreach (var ordenForEach in t.Det2)
                    {
                        var orden = ordrN.obtenerOrdenDeVenta(ordenForEach.NroSap);
                        if (orden.CANCELED.Equals("Y") || string.IsNullOrEmpty(orden.CANCELED)) { todasvigentes = false; num = ordenForEach.NroSap; break; }
                    }
                    if (todasvigentes)
                    {
                        DocNum = ticketN.recibirTicket(DocEntry, t);
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AnularRecibirTicketVenta(int DocEntry, int idOperation = 703)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E t = ticketN.obtenerTicket(DocEntry);
                    t.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    int DocNum = ticketN.anularRecibirTicket(DocEntry, t);
                    return RedirectToAction("ListadoTicketsRecepcion", new { DocNum = DocNum, Mensaje = "Se anuló el ticket recibido correctamente" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoTicketsRecepcion", new { Mensaje = e.Message });
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ListadoTicketsAlmacen(int DocNum = 0, ORTV_E t = null, string Mensaje = "", int idOperation = 801)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                ViewBag.RolSupervisor = user.IdRol;
                ViewBag.AlmUsuario = user.WhsCode;
                ViewBag.DocNum = DocNum;
                ViewBag.Ortv = t;
                ViewBag.Mensaje = Mensaje;

                //OOPE_N opeN = new OOPE_N();
                //var operaciones = opeN.listarOperacionesRolModulo(modulo, user.IdRol);

                //if (operaciones != null && operaciones.Count >= 1)
                //{
                //    foreach (var item in operaciones)
                //    {
                //        // Saber si el usuario en sesion tiene permiso para exportar excel - errores de picking
                //        if (item.id.Equals(810)) { ViewBag.ExportarExcelPicking = "SI"; } else { ViewBag.ExportarExcelPicking = "NO"; }
                //    }
                //}

                return View(ticketN.listarTicketsVenta(user, t));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult IniciarSacandoTicketVenta(int DocEntry, int idOperation = 802)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = ticketN.obtenerTicket(DocEntry);
                    tc.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    int DocNum = ticketN.editarSeguimientoTicket("INICIO PICKING", DocEntry, tc);

                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Ticket se está pickeando" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoTicketsAlmacen", new { Mensaje = e.Message });
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AnularIniciarSacandoTicket(int DocEntry, int idOperation = 803)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];

                    ORTV_E ticket = ticketN.obtenerTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ticket.RolSupervisor = usu.IdRol;
                    int DocNum = ticketN.editarSeguimientoTicket("ANULAR INICIO PICKING", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Se ha anulado el proceso de iniciar picking" });
                }
                catch (Exception e) { return RedirectToAction("ListadoTicketsAlmacen", new { Mensaje = e.Message }); }

            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult SacandoTicketVenta(int DocEntry, int idOperation = 802)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_E u = (Usuario_E)Session["UsuarioId"];
                try
                {
                    ViewBag.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    var listaUsuarios = u_N.ListaUsuarios(new Usuario_E { IdRol = 4 });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    ViewBag.Mensaje = string.Empty;

                    return View(ticketN.obtenerTicket(DocEntry));
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    var listaUsuarios = u_N.ListaUsuarios(new Usuario_E { IdRol = 4 });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    ViewBag.OpRegistro = $"{u.Nombres} {u.Apellidos}"; return View();
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult SacandoTicketVenta(int DocEntry, ORTV_E t, int idOperation = 802)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = ticketN.obtenerTicket(DocEntry);
                    tc.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    tc.Det11 = t.Det11;                                                 // OpSacador 2, OpSacador 3 y OpSacador 4
                    tc.Det11[0].Operario = tc.OpRegistro;                   // Seteamos elOpSacando quién es el usuario en sesión
                    ViewBag.datosSacador = ccORTV_N.ListarCC_ORTV(DocEntry, "FIN PICKING");
                    int DocNum = ticketN.editarSeguimientoTicket("FIN PICKING", DocEntry, tc);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Ticket ha sido pickeado" });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message; ViewBag.ListaUsuarios = u_N.ListaUsuarios(new Usuario_E { IdRol = 4 });
                    return View(ticketN.obtenerTicket(DocEntry));
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AnularSacandoTicket(int DocEntry, int idOperation = 803)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.Mensaje = string.Empty;
                    return View(ticketN.obtenerTicket(DocEntry));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult AnularSacandoTicket(int DocEntry, ORTV_E t, int idOperation = 803)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ViewBag.Mensaje = "Se anulo proceso de FIN PICKING al ticket";
                    ORTV_E ticket = ticketN.obtenerTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ticket.RolSupervisor = usu.IdRol;
                    int DocNum = ticketN.editarSeguimientoTicket("ANULAR FIN PICKING", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum });
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(ticketN.obtenerTicket(DocEntry)); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult IniciarVerificandoTicketVenta(int DocEntry, int idOperation = 808)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = ticketN.obtenerTicket(DocEntry);
                    tc.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    int DocNum = ticketN.editarSeguimientoTicket("INICIO VERIFICAR", DocEntry, tc);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Verificando el Ticket" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoTicketsAlmacen", new { Mensaje = e.Message });
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AnularIniciarVerificandoTicket(int DocEntry, int idOperation = 809)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ORTV_E ticket = ticketN.obtenerTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ticket.RolSupervisor = usu.IdRol;
                    int DocNum = ticketN.editarSeguimientoTicket("ANULAR INICIO VERIFICAR", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Inicio de verificación ANULADO" });
                }
                catch (Exception e) { return RedirectToAction("ListadoTicketsAlmacen", new { Mensaje = e.Message }); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult VerificadoTicketVenta(int DocEntry, int idOperation = 808)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
                    var listaUsuarios = u_N.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;

                    ORTV_E ticket = ticketN.obtenerTicket(DocEntry);
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
                    var listaUsuarios = u_N.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    return View();
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult VerificadoTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 808)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = ticketN.obtenerTicket(DocEntry);
                    tc.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    tc.Det12 = ticketPost.Det12;        // OpVerificador 2 y OpVerificador 3
                    int DocNum = ticketN.editarSeguimientoTicket("FIN VERIFICAR", DocEntry, tc);
                    var listaUsuarios = u_N.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    //return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum,Mensaje= "Ticket se ha verificado" });
                    return Json(new { DocNum = DocNum, Mensaje = $"Ticket {DocNum} verificado correctamente" });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    var listaUsuarios = u_N.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    return View(ticketN.obtenerTicket(DocEntry));
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AnularVerificadoTicket(int DocEntry, int idOperation = 809)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.Mensaje = string.Empty;
                    return View(ticketN.obtenerTicket(DocEntry));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult AnularVerificadoTicket(int DocEntry, ORTV_E t, int idOperation = 809)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ViewBag.Mensaje = "Se anuló el proceso de FIN VERIFICAR";
                    ORTV_E ticket = ticketN.obtenerTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ticket.RolSupervisor = usu.IdRol;
                    int DocNum = ticketN.editarSeguimientoTicket("ANULAR FIN VERIFICAR", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum });
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(ticketN.obtenerTicket(DocEntry)); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult IniciarEmpacandoTicketVenta(int DocEntry, int idOperation = 804)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = ticketN.obtenerTicket(DocEntry);
                    tc.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    int DocNum = ticketN.editarSeguimientoTicket("INICIO EMPACAR", DocEntry, tc);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Empacando Ticket" });
                }
                catch (Exception e)
                {
                    return RedirectToAction("ListadoTicketsAlmacen", new { Mensaje = e.Message });
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AnularIniciarEmpacandoTicket(int DocEntry, int idOperation = 805)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ORTV_E ticket = ticketN.obtenerTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ticket.RolSupervisor = usu.IdRol;
                    int DocNum = ticketN.editarSeguimientoTicket("ANULAR INICIO EMPACAR", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Inicio de empaque anulado" });
                }
                catch (Exception e) { return RedirectToAction("ListadoTicketsAlmacen", new { Mensaje = e.Message }); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult EmpacadoTicketVenta(int DocEntry, int idOperation = 804)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ViewBag.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ViewBag.Mensaje = string.Empty;
                    var listaUsuarios = u_N.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    ORTV_E ticket = ticketN.obtenerTicket(DocEntry);
                    /* al ser una vista restringida nunca se usara que muestre los operarios de este estado
					 List<CC_ORTV_E> ticketEmpacado = new List<CC_ORTV_E>();
					if (ticketEmpacado != null && ticketEmpacado.Count >= 1)
					{  ticket.OpEmpacado = ticketEmpacado[0].Operario;
						ticket.FechaEmpacado = ticketEmpacado[0].FechaOperacion;
						ticket.HoraEmpacado = ticketEmpacado[0].HoraOperacion;
					}*/

                    return View(ticket);
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    var listaUsuarios = u_N.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    return View();
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult EmpacadoTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 804)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = ticketN.obtenerTicket(DocEntry);
                    tc.OpRegistro = $"{u.Nombres} {u.Apellidos}";
                    tc.Cajas = ticketPost.Cajas;
                    tc.NroMesa = ticketPost.NroMesa;
                    tc.AlmProcedencia = ticketPost.AlmProcedencia;
                    tc.Operario = u.WhsCode;
                    tc.Det13 = ticketPost.Det13;        // OpEmpacador 2 y OpEmpacador 3
                                                        //envia el dato de WhsCode del usuario
                    tc.Operario = u.WhsCode;
                    int DocNum = ticketN.editarSeguimientoTicket("FIN EMPACAR", DocEntry, tc);
                    var listaUsuarios = u_N.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum, Mensaje = "Ticket empacado correctamente", DescargarPDF = 1, NumTicket = DocEntry, LugarDestino = ticketPost.LugarDestino });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    var listaUsuarios = u_N.ListaUsuarios(new Usuario_E() { Prefijo = "ALM" });
                    var usuariosDistinct = listaUsuarios.Select(x => $"{x.Nombres} {x.Apellidos}").Distinct().ToList();
                    ViewBag.ListaUsuarios = usuariosDistinct;
                    return View(ticketN.obtenerTicket(DocEntry));
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AnularEmpacadoTicket(int DocEntry, string Mensaje, int idOperation = 805)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.Mensaje = Mensaje;
                    return View(ticketN.obtenerTicket(DocEntry));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult AnularEmpacadoTicket(int DocEntry, ORTV_E t, int idOperation = 805)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ViewBag.Mensaje = "Proceso de FIN EMPACAR anulado";
                    ORTV_E ticket = ticketN.obtenerTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    ticket.RolSupervisor = usu.IdRol;
                    ticket.Operario = usu.WhsCode;
                    int DocNum = ticketN.editarSeguimientoTicket("ANULAR FIN EMPACAR", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = DocNum });
                }
                catch (Exception e) { return RedirectToAction("AnularEmpacadoTicket", new { DocEntry, Mensaje = e.Message }); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

        public ActionResult PesadoTicketVenta(int DocEntry, int idOperation = 806)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.Mensaje = "¿Está seguro(a) de cambiar el estado a PESADO?";
                    return View(ticketN.obtenerTicket(DocEntry));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult PesadoTicketVenta(int DocEntry, ORTV_E t, int idOperation = 806)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ORTV_E tc = ticketN.obtenerTicket(DocEntry);
                    tc.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    if (t.Det6 != null && t.Det6.Count > 0)
                    {
                        for (int i = 0; i > t.Det6.Count; i++)
                        {
                            t.Det6[i].UniMed = "KG";
                        }
                    }
                    tc.Det6 = t.Det6;
                    int DocNum = ticketN.editarSeguimientoTicket("PESADO", DocEntry, tc);

                    return RedirectToAction("ListadoTicketsDespacho", new { DocNum = DocNum, Mensaje = "Pesado Correctamente" });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return View(ticketN.obtenerTicket(DocEntry));
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpGet]
        public ActionResult AnularPesadoTicket(int DocEntry, int idOperation = 807)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.Mensaje = "¿Está seguro(a) de ANULAR PESADO?";
                    return View(ticketN.obtenerTicket(DocEntry));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult AnularPesadoTicket(int DocEntry, ORTV_E nulo, int idOperation = 807)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ORTV_E ticket = ticketN.obtenerTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                    int DocNum = ticketN.editarSeguimientoTicket("ANULARPESADO", DocEntry, ticket);
                    ViewBag.Mensaje = "Proceso de pesado anulado correctamente";
                    return RedirectToAction("ListadoTicketsDespacho", new { DocNum = DocNum });
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(ticketN.obtenerTicket(DocEntry)); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        //para despacho 901+
        public ActionResult ListadoTicketsDespacho(int DocNum = 0, ORTV_E ticket = null, string Mensaje = "", int idOperation = 901)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                ViewBag.DocNum = DocNum;
                ViewBag.almacenUsuario = user.WhsCode;
                ViewBag.idRolUsuario = user.IdRol;
                ViewBag.Mensaje = Mensaje;
                ticket.NombreVista = "ListadoTicketsDespacho";
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
                var lista = ticketN.listarTicketsVenta(user, ticket);

                if (user.IdRol == 53)
                {
                    lista = lista.OrderBy(x =>
                    {
                        if (x.Estado == "PICKEANDO")
                            return 0;
                        if (x.Estado == "EMPACADO")
                            return 1;
                        if (x.Estado == "PREENVIO")
                            return 2;
                        if (x.Estado == "ENVIADO")
                            return 3;
                        if (x.Estado == "ENTREGADO")
                            return 4;
                        return 5;
                    }).ToList();

                }
                ViewBag.Ortv = ticket;
                return View(lista);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult EntregadoTicketVenta(int DocEntry, int idOperation = 902)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ORTV_E ticket = ticketN.obtenerTicket(DocEntry);

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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult EntregadoTicketVenta(int DocEntry, ORTV_E ticketPost, int idOperation = 902)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                    ORTV_E ticket = ticketN.obtenerTicket(DocEntry);
                    ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";

                    if (!String.IsNullOrEmpty(ticketPost.Det5[0].RegEstado))
                    {
                        ticket.Det5[0].RegEstado = ticketPost.Det5[0].RegEstado;
                    }

                    ViewBag.Mensaje = "Entregado Correctamente";
                    int DocNum = ticketN.editarSeguimientoTicket("ENTREGADO", DocEntry, ticket);
                    return RedirectToAction("ListadoTicketsDespacho", new { DocNum = DocNum });
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(ticketN.obtenerTicket(DocEntry)); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AnularEntregadoTicket(int DocEntry, int idOperation = 903)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.Mensaje = "";
                    return View(ticketN.obtenerTicket(DocEntry));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult AnularEntregadoTicket(int DocEntry, ORTV_E t, int idOperation = 903)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                var ticket = ticketN.obtenerTicket(DocEntry);
                ticket.OpRegistro = $"{usu.Nombres} {usu.Apellidos}";
                ViewBag.Mensaje = "AnularEntregado Correctamente";

                try
                {
                    int DocNum = ticketN.editarSeguimientoTicket("ANULARENTREGADO", DocEntry, ticket);

                    return RedirectToAction("ListadoTicketsDespacho", new { DocNum = DocNum });
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(ticket); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ListadoLibrosSaldo(OLDS_E li = null, int idOperation = 1301)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.Mensaje = "";
                    ViewBag.Olds = li;
                    return View(lN.listarLibrosSaldo(li));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(lN.listarLibrosSaldo(li)); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult CrearLibroSaldo(int idOperation = 1302)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OCRD_N ocrdN = new OCRD_N();
                ViewBag.Mensaje = "";
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                return View(new OLDS_E());
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult CrearLibroSaldo(OLDS_E l, int idOperation = 1302)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OCRD_N ocrdN = new OCRD_N();
                ViewBag.Mensaje = "";
                try
                {
                    ViewBag.Mensaje = "LibroCreado";
                    lN.crearLibroSaldo(l);
                    return RedirectToAction("ListadoLibrosSaldo");
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; }
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                return View(l);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ListarDetLibroSaldo(string CardCode, int idOperation = 1303)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.LibroSaldo = lN.obtenerLibroSaldo(CardCode);
                    ViewBag.Mensaje = "";
                    return View(lN.obtenerDetLibroSaldo(CardCode));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AgregarDetLibroSaldo(string CardCode, int idOperation = 1304)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.LibroSaldo = lN.obtenerLibroSaldo(CardCode);
                    ViewBag.Mensaje = "";
                    LDS1_E d = new LDS1_E();
                    d.CardCode = CardCode;
                    return View(d);
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult AgregarDetLibroSaldo(LDS1_E d, int idOperation = 1304)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.Mensaje = "";
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    d.OperarioReg = u.Nombres + " " + u.Apellidos;
                    if (ModelState.IsValid)
                    {
                        lN.agregarDetLibroSaldo(d);
                        return RedirectToAction("ListarDetLibroSaldo", new { CardCode = d.CardCode });
                    }
                    return View(d);
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; ViewBag.LibroSaldo = lN.obtenerLibroSaldo(d.CardCode); return View(d); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ReporteLibroSaldo(string CardCode, int idOperation = 1305)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.Mensaje = "";
                    return View(lN.obtenerLibroSaldo(CardCode));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ReportesVentas(int idOperation = 1306)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult infoAuditVtsCli(int idOperation = 1307)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Capa_Negocio.General_NEG.Tablas.OSLP_N oslpN = new Capa_Negocio.General_NEG.Tablas.OSLP_N();
                OCRD_N ocrdN = new OCRD_N();
                OWHS_N owhsN = new OWHS_N();
                Capa_Negocio.Almacen_NEG.Tablas.OMRC_N omrcN = new Capa_Negocio.Almacen_NEG.Tablas.OMRC_N();
                ViewBag.Vendedores = new SelectList(oslpN.listadoOslp("VENTA"), "SlpCode", "SlpName");
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                ViewBag.Laboratorios = new SelectList(omrcN.listarFabricantes(), "FirmCode", "U_SYP_DESC");
                ViewBag.Almacenes = owhsN.listarAlmacenes();

                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult tbReporteAuditVtsCli(string FecIni, string FecFin, string CardCode, int FirmCode = 0, int SlpCode = 0, int idOperation = 1307)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }

        }
        public ActionResult infoAnalisisTickets(int idOperation = 1308)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OWHS_N owhsN = new OWHS_N();
                OCRD_N ocrdN = new OCRD_N();
                Usuario_N usuarioN = new Usuario_N();
                ViewBag.Almacenes = owhsN.listarAlmacenes();
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                ViewBag.Operarios = usuarioN.ListaUsuarios(null);
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptAnalisisTickets(RptFiltrosAnalisisTickets_E frm, int idOperation = 1308)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
                            for (var col = 1; col <= 60; col++)
                            {
                                worksheet.Column(col).AutoFit();
                            }

                            var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: analisisTickets.Count + 1, toColumn: 60), "AnalisisTickets");
                            tabla.ShowHeader = true;
                            tabla.TableStyle = TableStyles.Medium2;
                        }
                    }

                    return File(libro.GetAsByteArray(), excelContentType, "ReporteAnalisisTickets.xlsx");
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult infoAnalisisVentas(int idOperation = 1309)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OCRD_N ocrdN = new OCRD_N();
                Usuario_N usuarioN = new Usuario_N();
                OWHS_N owhsN = new OWHS_N();
                ViewBag.Almacenes = owhsN.listarAlmacenes();
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                ViewBag.Operarios = usuarioN.ListaUsuarios(null);
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult tbReporteAnalisisVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnalisisVentas_E obj, int idOperation = 1309)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {

                ReportViewer rp = new ReportViewer();
                ViewBag.Mensaje = "";
                try
                {
                    rp.ProcessingMode = ProcessingMode.Local;
                    rp.SizeToReportContent = true;
                    rp.LocalReport.ReportPath =
                        Request.MapPath(Request.ApplicationPath) + @"Reportes\RptVentas\RptAnalisisVentas.rdlc";
                    rp.LocalReport.DataSources.Add(new ReportDataSource("DS_AnalisisVentas", ticketN.tbRptAnalisisVentas(obj)));
                    ViewBag.REPORTE = rp;
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                }
                return View("reporteViewer");
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }

        }
        public ActionResult GestionRegalos(OREG_E filtro, string mensaje, int idOperation = 1310)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                ViewBag.Regalos = filtro; ViewBag.Mensaje = mensaje;
                OREG_N oregN = new OREG_N();
                return View(oregN.listaRegalos(filtro));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult NuevoRegalo(int idOperation = 1311)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }

        }
        [HttpPost]
        public ActionResult NuevoRegalo(OREG_E obj, int idOperation = 1311)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
                    ORTV_N ortvN = new ORTV_N();
                    ViewBag.Mensaje = e.Message; ViewBag.Tickets = ortvN.ListarTicketsParaAtencion(); return View(obj);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }

        }
        public ActionResult GestionarStock(int id, int idOperation = 1312)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Capa_Negocio.Ventas_NEG.TablasSql.OREG_N oregN = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N();
                ViewBag.Regalo = oregN.buscarRegalo(id);
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult GestionarStock(OTRC_E o2, int idOperation = 1312)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Capa_Negocio.Ventas_NEG.TablasSql.OREG_N oregN = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N();
                try
                {
                    Usuario_E u = (Usuario_E)Session["UsuarioId"];
                    o2.Operario = $"{u.Nombres} {u.Apellidos}";
                    OREG_E o1 = new OREG_E() { Id = o2.IdReg };
                    oregN.registrarGestionStock(o1, o2);
                    return RedirectToAction("GestionRegalos");
                }
                catch
                {
                    ViewBag.Regalo = oregN.buscarRegalo(o2.IdReg);
                    return View();
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult InactivarRegalo(int id, int idOperation = 1313)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Capa_Negocio.Ventas_NEG.TablasSql.OREG_N oregN = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N();
                return View(oregN.buscarRegalo(id));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }

        }
        public ActionResult InactivarRegaloPost(OREG_E obj, int idOperation = 1313)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Capa_Negocio.Ventas_NEG.TablasSql.OREG_N oregN = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N();
                try
                {
                    oregN.inactivarRegalo(obj);
                    return RedirectToAction("GestionRegalos", new { Id = obj.Id });
                }
                catch (Exception e)
                { return RedirectToAction("InactivarRegalo", new { msj = e.Message }); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RevertirInactivarRegalo(OREG_E obj, int idOperation = 1313)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Capa_Negocio.Ventas_NEG.TablasSql.OREG_N oregN = new Capa_Negocio.Ventas_NEG.TablasSql.OREG_N();
                try
                {
                    oregN.revertirInactivarRegalo(obj);
                    return RedirectToAction("GestionRegalos", new { Id = obj.Id });
                }
                catch (Exception e)
                { return RedirectToAction("InactivarRegalo", new { msj = e.Message }); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult TransaccionesRegalo(int id, int idOperation = 1314)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OREG_N oregN = new OREG_N(); OTRC_N otrcN = new OTRC_N();
                ViewBag.Regalo = oregN.buscarRegalo(id);
                return View(otrcN.listarTransacciones(new OTRC_E() { IdReg = id }));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ExportarExcelTransReg(OTRC_E o, int idOperation = 1314)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OREG_N oregN = new OREG_N(); OTRC_N otrcN = new OTRC_N();
                ViewBag.Regalo = oregN.buscarRegalo(o.IdReg);
                string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var transacciones = otrcN.listarTransacciones(o);
                using (var libro = new ExcelPackage())
                {
                    var worksheet = libro.Workbook.Worksheets.Add("transacciones");
                    worksheet.Cells["A1"].LoadFromCollection(transacciones, PrintHeaders: true);
                    for (var col = 1; col <= 12; col++)
                    {
                        worksheet.Column(col).AutoFit();
                    }
                    var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: transacciones.Count + 1, toColumn: 12), "transacciones");
                    tabla.ShowHeader = true;
                    return File(libro.GetAsByteArray(), excelContentType, "Transacciones.xlsx");
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login") { return null; }
            else { return null; }
        }
        public ActionResult ReporteClienteRegalos(string CardCode, int idOperation = 502)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OCLR_N oclrN = new OCLR_N();
                try
                {
                    ViewBag.Mensaje = "";
                    return View(oclrN.buscarClienteRegalo(CardCode));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult GestionClienteRegalos(OCLR_E filtro, int idOperation = 1315)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    ViewBag.CliReg = filtro;
                    OCLR_N oclrN = new OCLR_N();
                    return View(oclrN.listadoRegaloCliente(filtro));
                }
                catch (Exception e)
                { return RedirectToAction("GestionClienteRegalos", new { msj = e.Message }); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult NuevoClienteRegalo(int idOperation = 1316)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OREG_N oregN = new OREG_N(); OCRD_N ocrdN = new OCRD_N();
                OREG_E filtro = null;
                ViewBag.Regalos = oregN.listaRegalos(filtro);
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }

        }
        [HttpPost]
        public ActionResult NuevoClienteRegalo(OCLR_E obj, int idOperation = 1316)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    OCLR_N oclrN = new OCLR_N();
                    oclrN.registrarClienteRegalo(obj);
                    return RedirectToAction("GestionClienteRegalos");
                }
                catch (Exception e)
                { return RedirectToAction("NuevoClienteRegalo", new { msj = e.Message }); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult EditarClienteRegalos(string CardCode, int idOperation = 1317)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OREG_N oregN = new OREG_N();
                OCLR_N oclrN = new OCLR_N();
                ViewBag.Regalos = oregN.listaRegalos(null);
                return View(oclrN.buscarClienteRegalo(CardCode));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult EditarClienteRegalos(OCLR_E obj, int idOperation = 1317)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                OREG_N oregN = new OREG_N();
                OCLR_N oclrN = new OCLR_N();
                try
                {
                    oclrN.editarClienteRegalo(obj);
                    return RedirectToAction("GestionClienteRegalos", new { CardCode = obj.CardCode });
                }
                catch (Exception e)
                {
                    ViewBag.Regalos = oregN.listaRegalos(null);
                    ViewBag.Mensaje = e.Message;
                    return View(obj);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult ReporteReclamosCliente(string CardCode, int idOperation = 502)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
                        TipoSolucion = "('Regalo','Articulo','RegaloArticulo')",
                        Estado = "Atendido"
                    };

                    return View(osatN.ListarSolicitudes(filtro, false));
                }
                catch (Exception e) { ViewBag.Mensaje = e.Message; return View(); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptAnCtVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnCtVentas_E obj, int idOperation = 1318)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptAnCtVentas2(Capa_Entidad.Ventas_ENT.Formularios.FrmAnCtVentas_E obj, int idOperation = 1318)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult AnCtVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnCtVentas_E obj, int idOperation = 1318)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult GestionVendedores(USR1_E fil, int idOperation = 1319)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                USR1_N usr1N = new USR1_N();
                if (fil == null) { ViewBag.fil = new USR1_E(); }
                else { ViewBag.fil = fil; }
                return View(usr1N.listarVenUltCuotas(fil));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult GestionCuotas(int DocEntry, string Mensaje = "", int idOperation = 1320)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                USR1_N usr1N = new USR1_N();
                ViewBag.CuotasUser = usr1N.listarUsrCuotas(DocEntry);
                ViewBag.User = u_N.buscarUsuario(DocEntry);
                ViewBag.Mensaje = Mensaje;
                DateTime ahora = DateTime.Now;
                return View(new USR1_E() { YearU = ahora.Year, MonthU = ahora.Month }); ;
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult GestionCuotas(USR1_E obj, int idOperation = 1320)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult BorrarCuotaUsr(USR1_E obj, int idOperation = 1321)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                USR1_N usr1N = new USR1_N();
                try
                {
                    usr1N.borrarUsr1(obj);
                    return RedirectToAction("GestionCuotas", new { DocEntry = obj.DocEntry });
                }
                catch (Exception e) { return RedirectToAction("GestionCuotas", new { DocEntry = obj.DocEntry, Mensaje = e.Message }); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult infoVentasClienteDias(int idOperation = 1322)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Capa_Negocio.SocioNegocios_NEG.Tablas.OCRD_N ocrdN = new Capa_Negocio.SocioNegocios_NEG.Tablas.OCRD_N();
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptVentasClienteDias(string CardCodeIni, string CardCodeFin, string Fecha, int idOperation = 1322)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult infoVentasVendedorDias(int idOperation = 1323)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptVentVendDias(string Fecha, int idOperation = 1323)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult infoVentasSkuDias(int idOperation = 1324)
        {
            //string Fecha,string ItemCodeIni,string ItemCodeFin
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Capa_Negocio.Almacen_NEG.Tablas.OITM_N oitmN = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N();
                ViewBag.ListaProductos = oitmN.Listar(null);
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptVentasSkuDias(string ItemCodeIni, string ItemCodeFin, string Fecha, int idOperation = 1324)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult infoVentasSkuCliDias(int idOperation = 1325)
        {
            //string Fecha,string ItemCodeIni,string ItemCodeFin
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Capa_Negocio.Almacen_NEG.Tablas.OITM_N oitmN = new Capa_Negocio.Almacen_NEG.Tablas.OITM_N();
                Capa_Negocio.SocioNegocios_NEG.Tablas.OCRD_N ocrdN = new Capa_Negocio.SocioNegocios_NEG.Tablas.OCRD_N();
                ViewBag.Clientes = ocrdN.listarSociosDeNegocios(new OCRD_E { CardType = "C" });
                ViewBag.ListaProductos = oitmN.Listar(null);
                return View();
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public ActionResult RptVentasSkuCliDias(string ItemCodeIni, string ItemCodeFin, string CardCodeIni
            , string CardCodeFin, string Fecha, int idOperation = 1325)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        //metodos asincronos
        public JsonResult buscarClienteRegalo(string CardCode)
        {
            Capa_Negocio.Ventas_NEG.TablasSql.OCLR_N oclrN = new Capa_Negocio.Ventas_NEG.TablasSql.OCLR_N();
            return Json(oclrN.buscarClienteRegalo(CardCode));
        }
        public JsonResult CalcularPesoTotal(ORTV_E t)
        {
            return Json(ticketN.CalcularPesoTotal(t));
        }

        /*************************** P E D I D O S  O N L I N E ***************************/
        [HttpGet]
        public ActionResult PedidosOnline(Capa_Entidad.Ventas_ENT.TablasSql.ORDR_E datos, int idOperation = 1326)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        public JsonResult BuscarCliente(OCRD_E cliente)
        {
            if (!cliente.CardName.Equals(""))
            {
                OCRD_N ocrdN = new OCRD_N();
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
            verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            Capa_Negocio.Almacen_NEG.TablasSql.OIBT_N oibtN = new Capa_Negocio.Almacen_NEG.TablasSql.OIBT_N();
            var result = oibtN.VerificarMigracionArticulos();

            return Json(result);
        }
        public JsonResult BuscarArticulo(Capa_Entidad.Almacen_ENT.TablasSql.OIBT_E articulo)
        {
            verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            if (!string.IsNullOrEmpty(articulo.ItemName) || !string.IsNullOrEmpty(articulo.PrincActivo))
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
            verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            ORTV_N ortvN = new ORTV_N();
            var result = ortvN.obtenerTicket(DocEntry);
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
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                return View(Pedido);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult CrearPedidoOnline(Capa_Entidad.Ventas_ENT.TablasSql.ORDR_E Pedido, List<Capa_Entidad.Almacen_ENT.TablasSql.OIBT_E> DetallePedido, int idOperation = 1327)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpGet]
        public ActionResult VerPedidoOnline(Capa_Entidad.Ventas_ENT.TablasSql.ORDR_E pedido, int idOperation = 1327)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N ordrN = new Capa_Negocio.Ventas_NEG.TablasSql.ORDR_N();
                List<Capa_Entidad.Ventas_ENT.TablasSql.ORDR_E> datos = ordrN.ListarPedidosOnline(pedido);

                return View(datos);
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public ActionResult EditarPedidoOnline(int idORDR, List<Capa_Entidad.Almacen_ENT.TablasSql.OIBT_E> DetallePedido, int idOperation = 1327)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
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
            if (verificacionAccesos(idOperation) == "C_Access")
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
                            tabla.TableStyle = TableStyles.Medium2;
                        }
                    }

                    return File(libro.GetAsByteArray(), excelContentType, nombreArchivo);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login") { return null; }
            else { return null; }
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
            verificacionAccesos(0);
            try
            {
                ORTV_E t = ticketN.obtenerTicket(DocEntry);
                try
                {
                    if (!String.IsNullOrEmpty(t.LugarDestino))
                    {
                        if (t.LugarDestino == "Centro")
                        {
                            t.TiempoEntrega = Convert.ToDateTime(t.TiempoEntrega).AddMinutes(90);
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
            verificacionAccesos(0);
            return RedirectToAction("PdfTicketVenta", new { DocEntry = DocEntry });
        }
        public ActionResult RotuladoTicket(int DocEntry)
        {
            verificacionAccesos(0);
            ORTV_N ortvN = new ORTV_N(); object obj = null;
            try
            {
                obj = ortvN.obtenerTicket(DocEntry);
            }
            catch { }
            return View(obj);
        }
        public ActionResult PdfRotuladoTicket(int DocEntry)
        {
            verificacionAccesos(0);
            ORTV_E t = ticketN.obtenerTicket(DocEntry);
            if (!t.Estado.Equals("EMPACADO"))
            {
                return RedirectToAction("ListadoTicketsAlmacen", new { DocNum = t.DocNum, Mensaje = "El ticket debe estar EMPACADO" });
            }
            return new ActionAsPdf("RotuladoTicket", new { DocEntry = DocEntry }) { FileName = "RotuladoTicket" + DocEntry + ".pdf", PageOrientation = Rotativa.Options.Orientation.Landscape, PageSize = Rotativa.Options.Size.A6 };
        }
        /*******************datos de form*****************/
        public JsonResult infoContactosVentasSocio(string CardCode)
        {
            Capa_Negocio.SocioNegocios_NEG.Tablas.OCPR_N oN = new Capa_Negocio.SocioNegocios_NEG.Tablas.OCPR_N();
            return Json(oN.listarContactosVentasSocio(CardCode));
        }
        public ActionResult infoListaClientes(string Fecha)
        {
            return Content(ticketN.generaInfoListaClientes(Fecha));
        }
        public ActionResult infoDirDestino(string CardCode)
        {
            return Content(ticketN.generaInfoListaDirDestinos(CardCode));
        }
        public ActionResult infoListaOrdenesDeVenta(string Fecha, string CardCode, int DocNum)
        {
            return Content(ticketN.generaInfoListaOrdenesDeVenta(Fecha, CardCode, DocNum));
        }
        public ActionResult infoListaNotasDeCreditoV(string CardCode)
        {
            return Content(ticketN.generaInfoListaNotasDeCreditoV(CardCode));
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
            return Json(lN.obtenerLibroSaldo(CardCode));
        }
        public JsonResult comprobarReclamosCliente(string CardCode)
        {
            Capa_Negocio.AtencionCliente_NEG.TablasSql.OSAT_N osatN = new Capa_Negocio.AtencionCliente_NEG.TablasSql.OSAT_N();
            Dictionary<string, string> ortv = new Dictionary<string, string> { { "CardCode", CardCode } };

            var filtro = new Capa_Entidad.AtencionCliente_ENT.TablasSql.OSAT_E()
            {
                DetORTV = ortv,
                TipoSolicitudCreaTicketVenta = "('Reclamo','Devolucion')",      // TipoSolicitudCreaTicketVenta: Filtro para el botón Reclamos Crea Ticket Venta
                TipoSolucion = "('Regalo','Articulo','RegaloArticulo')",
                Estado = "Atendido"
            };

            return Json(osatN.ListarSolicitudes(filtro, false));
        }
        public JsonResult CalcularMontos(ORTV_E t)
        {
            return Json(ticketN.CalcularMontos(t));
        }
        public ActionResult ValidarDatosTicket(ORTV_E t)
        {
            string status = "true";
            try
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                t.Vendedor = $"{user.Nombres} {user.Apellidos}";
                t.WhsCodeLog = $"{user.WhsCode}";
                ticketN.validarDatosTicket(t, 0);
                if (t.Estado == "SEPARADO" && t.Observaciones2 == "SI")
                {
                    if (t.Det7 != null && t.Det7.Count > 0)
                    {
                        foreach (var det7 in t.Det7)
                        {
                            if (string.IsNullOrEmpty(det7.CardCode) || string.IsNullOrEmpty(det7.CardName) || det7.DocNumVinc == 0 || det7.MontoFinal == 0)
                            {
                                throw new Exception("El ticket vinculado en la linea " + det7.Linea + " no cumple con los datos requeridos.");
                            }
                        }
                    }
                    else { throw new Exception("Debe vincular tickets"); }
                }
                return Content(status);
            }
            catch (Exception e) { return Content(e.Message); }

        }

        /**********Documentos imprimibles para el proceso de tickets **************/
        public ActionResult TacoComentarios(int DocEntry)
        {
            verificacionAccesos(0);
            try
            {
                ORTV_E t = ticketN.obtenerTicket(DocEntry);
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
        public ActionResult PdfTacoComentarios(int DocEntry)
        {
            verificacionAccesos(0);
            ORTV_E ticket = ticketN.obtenerTicket(DocEntry);
            List<CC_ORTV_E> ticketAbierto = ccORTV_N.ListarCC_ORTV(DocEntry, "REGISTRAR");

            // Si el ticket no está ABIERTO y en el control de cambios nunca hubo un movimiento
            if (ticket.Estado != "ABIERTO" && ticketAbierto[0].FechaOperacion == "")
            {
                return RedirectToAction("ListadoTicketsRecepcion", new { DocNum = ticket.DocNum, Mensaje = "El ticket debe estar ABIERTO" });
            }

            return new ActionAsPdf("TacoComentarios", new { DocEntry = DocEntry }) { FileName = "TacoComentario" + DocEntry + ".pdf", PageOrientation = Rotativa.Options.Orientation.Portrait, PageSize = Rotativa.Options.Size.A6 };
        }
        public ActionResult TacoEmpaque(int DocEntry)
        {
            verificacionAccesos(0);
            try
            {
                RTV12_D datosRTV12 = new RTV12_D();
                RTV13_D datosRTV13 = new RTV13_D();
                List<CC_ORTV_E> ticketVerificando = new List<CC_ORTV_E>(); List<CC_ORTV_E> ticketEmpacado = new List<CC_ORTV_E>();
                ORTV_E ticket = ticketN.obtenerTicket(DocEntry);
                if (ticket.Estado.Equals("EMPACADO")
                    || ticket.Estado.Equals("PREENVIO")
                    || ticket.Estado.Equals("ENVIADO")
                    || ticket.Estado.Equals("ENTREGADO"))
                {
                    /* Trae el operario de verificacion principal */
                    ticketVerificando = ccORTV_N.ListarCC_ORTV(DocEntry, "FIN VERIFICAR");
                    ticket.OpVerificado = ticketVerificando[0].Operario;

                    /* Trae los operarios de verificado de apoyo*/
                    List<string> operariosChequeando = datosRTV12.BuscarOperariosChequeando(ticket.DocEntry);
                    if (operariosChequeando != null)
                    {
                        ticket.OpVerificadoApoyo = operariosChequeando;
                    }
                }

                if (ticket.Cajas >= 1)
                {
                    // Trae el operario de empacado principal
                    ticketEmpacado = ccORTV_N.ListarCC_ORTV(DocEntry, "FIN EMPACAR");
                    List<string> operariosEmpacando = datosRTV13.BuscarOperariosEmpacando(DocEntry);

                    // Trae los operarios de empacado de apoyo
                    if (operariosEmpacando != null) { ticket.OpEmpacadoApoyo = operariosEmpacando; }
                }
                //obtener guias
                if (ticket.LugarDestino.Equals("Arriola") || ticket.LugarDestino.Equals("Centro"))
                {
                    string WhsCode = string.Empty;
                    Capa_Negocio.Almacen_NEG.Tablas.OWTR_N owtrN = new Capa_Negocio.Almacen_NEG.Tablas.OWTR_N();
                    if (ticket.LugarDestino.Equals("Centro")) { WhsCode = "01"; }
                    else if (ticket.LugarDestino.Equals("Arriola")) { WhsCode = "09"; }

                    ViewBag.Guias = owtrN.GuiasTicketTransferencia(ticket.DocNum, WhsCode);
                }
                else { ViewBag.Guias = ticketN.GuiasTicket(DocEntry); }
                if (ticketVerificando.Count() > 0) { ticket.OpVerificado = ticketVerificando[0].Operario; }

                ViewBag.Letra = 4;
                ViewBag.ColorTicket = ResaltarTicket(ticket.LugarDestino);

                if (ticketEmpacado.Count() > 0)
                {
                    try
                    {
                        DateTime dt = Convert.ToDateTime(ticketEmpacado[0].FechaOperacion);
                        ticket.FechaEmpacado = dt.ToString("dd/MM/yyyy");
                        ticket.HoraEmpacado = ticketEmpacado[0].HoraOperacion;
                        ticket.OpEmpacado = ticketEmpacado[0].Operario;
                    }
                    catch { }
                }

                if (ticket.TiempoEntrega != null)
                {
                    try
                    {
                        DateTime dt = Convert.ToDateTime(ticket.TiempoEntrega);
                        dt = dt.AddMinutes(-70);
                        ticket.TiempoEntrega = Convert.ToDateTime(dt.ToString("dd/MM/yyyy hh:mm tt"));
                    }
                    catch { }

                }
                return View(ticket);
            }
            catch { return View(new ORTV_E()); }
        }
        public ActionResult PdfTacoEmpaque(int DocEntry)
        {
            verificacionAccesos(0);
            return new ActionAsPdf("TacoEmpaque", new { DocEntry = DocEntry }) { FileName = "PdfTacoEmpaque.pdf", PageOrientation = Rotativa.Options.Orientation.Portrait, PageSize = Rotativa.Options.Size.A6 };

        }
        public ActionResult OrdenDeVenta(int DocNum)
        {
            verificacionAccesos(0);
            try
            {
                ViewBag.Letra = 4;
                return View(ticketN.obtenerOrdenDeVenta(DocNum));
            }
            catch { return View(); }
        }

        /****************************** E R R O R E S   P I C K I N G ******************************/
        public JsonResult RegistrarErroresPicking(OEP_E datos, List<EP1_E> detalleErroresPicking)
        {
            verificacionAccesos(0);         // Verificar sesión logueada solo para solicitudes AJAX

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
                return Json(new { Mensaje = e.Message });
            }
        }
        public ActionResult ExportarReporteErroresPicking(RptFiltrosErroresPicking_E filtros, int idOperation = 810)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
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
                            tabla.TableStyle = TableStyles.Medium2;
                        }
                    }

                    return File(libro.GetAsByteArray(), excelContentType, nombreArchivo);
                }

            }
            else if (verificacionAccesos(idOperation) == "E_Login") { return null; }
            else { return null; }
        }
        /***********************************************/


        /****************************** P A G O   C O N T R A E N T R E G A ******************************/
        [HttpGet]
        public ActionResult AutorizarTicketReparto(int docEntry, int idOTC, string mensaje = null, int idOperation = 504)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_E usu = (Usuario_E)Session["UsuarioId"];
                ORTV_E ticket = ticketN.obtenerTicket(docEntry);
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
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

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
        /***entrega masiva antony*************/
        [HttpPost]
        public JsonResult gestionarEntregadoMasivo(int[] ticketsMasivo, int entregadoConRegalo)
        {
            verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
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
            verificacionAccesos(0);         // Validar sesion logueada, solo para ajax
            return Json(ticketN.buscarVariosTickets(arrTickets));
        }
        public ActionResult EditarTicketVentaSup(int DocEntry, int idOperation = 503)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {

                UBIG_N ubigN = new UBIG_N(); OUR1_N ofiN = new OUR1_N(); COUR_N couN = new COUR_N();
                ViewBag.Mensaje = "";
                ORTV_E t = ticketN.obtenerTicket(DocEntry);

                ViewBag.Ubigeos = ubigN.Listar();
                ViewBag.Oficinas = ofiN.Listar();
                ViewBag.Agencias = couN.Listar();
                if (t.Estado.Equals("SEPARADO")) { return RedirectToAction("CreaTicketVenta", new { DocEntry = t.DocEntry }); }
                else { return View(t); }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }

        }
        [HttpPost]
        public ActionResult EditarTicketVentaSup(int DocEntry, ORTV_E t, int idOperation = 503)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                try
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    t.OpRegistro = $"{user.Nombres} {user.Apellidos}";
                    t.WhsCodeLog = $"{user.WhsCode}";
                    ticketN.editarTicketSup(DocEntry, user.IdRol, t);
                    return RedirectToAction("ListadoTickets", new { DocNum = t.DocNum });
                }
                catch (Exception e)
                {
                    UBIG_N ubigN = new UBIG_N(); OUR1_N ofiN = new OUR1_N(); COUR_N couN = new COUR_N();
                    ViewBag.Mensaje = e.Message;
                    ViewBag.Ubigeos = ubigN.Listar();
                    ViewBag.Oficinas = ofiN.Listar();
                    ViewBag.Agencias = couN.Listar();
                    return View(t);
                }
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }
        [HttpPost]
        public JsonResult buscarOficinas(string nombreAgencia)
        {
            OUR1_N oN = new OUR1_N();
            verificacionAccesos(0);
            return Json(oN.Listar().Where(x => x.NombreAgencia == nombreAgencia));
        }
        public JsonResult ListarTicketsNoVisiblesPagados(int DocEntryUsuario)
        {
            verificacionAccesos(0); ORTV_N ortvN = new ORTV_N(); Usuario_N usuN = new Usuario_N(); Usuario_E u = usuN.buscarUsuario(DocEntryUsuario);
            var result = ortvN.listarTicketsVenta(u, new ORTV_E { Estado = "ABIERTO" }).Where(x => x.Visible == "NO" && x.EstadoPago == "PAGADO").OrderBy(x => x.FechaPago + " " + x.HoraPago).ToList();
            return Json(new { Datos = result });
        }
        public JsonResult CambiarVisibleTicket(int DocEntry)
        {
            verificacionAccesos(0); ORTV_N ortvN = new ORTV_N(); var result = ortvN.editarVisibilidadTicket(DocEntry);
            return Json(new { Datos = result }); 
        }
        public JsonResult RegistrarImpresion(int DocEntry)
        {
            verificacionAccesos(0);
            Usuario_E user = (Usuario_E)Session["UsuarioId"];
            var Operario = $"{user.Nombres} {user.Apellidos}";
            ORTV_N ortvN = new ORTV_N(); 
            var result = ortvN.registrarImpresionTicket(DocEntry, Operario);
            return Json(new { Datos = result });
        }
        


    }
}
