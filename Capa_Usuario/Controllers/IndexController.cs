using Capa_Entidad;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio;
using Capa_Negocio.Seguridad_NEG;
using Capa_Usuario.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Capa_Usuario.Controllers
{
    public class IndexController : Controller
    {
        Rol1_N rol1 = new Rol1_N();
        BaseDeDatos_N bd_N = new BaseDeDatos_N();
        Utilitarios_N utiN = new Utilitarios_N();

        private string ObtenerIPCliente()
        {
            string clientIp = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(clientIp))
            {
                clientIp = Request.ServerVariables["REMOTE_ADDR"];
            }
            return clientIp;

        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sesion()
        {
            //ejecutar este sql para tener una lista de dicha lista de resultados insertar en la tabla vt.otrc tal cual los mismos datos por cada registro pero convirtiendo el valor de imputado en negativo





            Usuario_E user = (Usuario_E)Session["UsuarioId"];

            if (user != null)
            {
                ViewBag.AREAS = new Capa_Negocio.Seguridad_NEG.TablasSql.AREA_N().listarAreas();
                ViewBag.AREASFC = new Capa_Negocio.Seguridad_NEG.TablasSql.REA1_N().listarAreasFc();
                ViewBag.NOMBRE = $"{user.Nombres} {user.Apellidos}";
                ViewBag.IDROL = user.IdRol;
                return View();
            }
            else
            {
                return RedirectToAction("Sesion");
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string user, string pass)
        {
            try
            {
                //string direccionIP = ObtenerIPCliente();
                //string[] segmentos = direccionIP != "::1" ? direccionIP.Split('.') : null;

                //// Prohibido el acceso del segmento 3 y 9 por solicitud de María
                //if (segmentos != null && new string[] { "3", "9" }.Contains(segmentos[2]))
                //{
                //    TempData["Mensaje"] = "Acceso restringido";
                //    TempData.Keep("Mensaje");

                //    return RedirectToAction("Index");
                //}

                Usuario_E usuario = new Usuario_N().buscarUsuarioSesion(user, pass);

                if (usuario != null)
                {
                    string equipo = Dns.GetHostEntry(Dns.GetHostName()).HostName;
                    string ip = Request.UserHostAddress == "::1" ? "127.0.0.1" : Request.UserHostAddress;
                    Session["UsuarioId"] = usuario;
                    utiN.RegistrarLog(user, "Inicio de Sesión", 0, ip, equipo);

                    return RedirectToAction("Sesion");
                }
                else
                {
                    TempData["Mensaje"] = "Usuario o contraseña inválidos.";
                    TempData.Keep("Mensaje");

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                rol1 = null;
                TempData["Mensaje"] = ex.Message;
                TempData.Keep("Mensaje");

                return RedirectToAction("Index");
            }
        }
        public ActionResult LogOut()
        {
            Session["UsuarioId"] = null;
            return RedirectToAction("Index");
        }
        public ActionResult Error()
        {
            return View();
        }
        public ActionResult statusBD()
        {
            return Content(bd_N.statusBD()); ;
        }

        ///******************************** M Ó D U L O   A L M A C É N ********************************/
        public ActionResult ALM_Devoluciones(int idOperation = 100)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "DevolucionMercancias", controllerDestino = "Almacen", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult ALM_Repartos(int idOperation = 200)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoRepartos", controllerDestino = "Rutas", usuario = (Usuario_E)Session["UsuarioId"] });

        }
        public ActionResult ALM_Rutas(int idOperation = 200)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoRutas", controllerDestino = "Rutas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult RUT_ReportesRutas(int idOperation = 206)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ReportesRutas", controllerDestino = "Rutas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult RH_Control_Asistencias(int idOperation = 400)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "GestionColaboradores", controllerDestino = "RecursosHumanos", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult VT_Tickets(int idOperation = 500)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoTicketsVenta", controllerDestino = "Ventas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult VT_TicketsFacturacion(int idOperation = 600)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoTicketsFacturacion", controllerDestino = "Ventas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult VT_TicketsRecepcion(int idOperation = 700)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoTicketsRecepcion", controllerDestino = "Ventas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult VT_TicketsAlmacen(int idOperation = 800)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoTicketsAlmacen", controllerDestino = "Ventas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult VT_TicketsDespacho(int idOperation = 900)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoTicketsDespacho", controllerDestino = "Ventas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult VT_TicketsProtocolos(int idOperation = 1000)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoTicketsProtocolos", controllerDestino = "Ventas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult CP_LineasProduccion(int idOperation = 1100)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoLineasProduccion", controllerDestino = "Compras", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult CP_ContratosRebate(int idOperation = 1200)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoContratosRebate", controllerDestino = "Compras", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult VT_LibrosSaldos(int idOperation = 1300)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoLibrosSaldo", controllerDestino = "Ventas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult VT_ReportesVentas(int idOperation = 1306)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ReportesVentas", controllerDestino = "Ventas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult VT_Regalos(int idOperation = 1310)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "GestionRegalos", controllerDestino = "Ventas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult VT_PedidosOnline(int idOperation = 1326)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "PedidosOnline", controllerDestino = "Ventas", usuario = (Usuario_E)Session["UsuarioId"] });
        }

        ///******************************** M Ó D U L O   C A J A ********************************/
        public ActionResult CJ_ListadoTickets(int idOperation = 3000)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "Index", controllerDestino = "Caja", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        ///****************************************************************************************/

        public ActionResult CP_ResumenRebate(int idOperation = 1400)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ResumenRebate", controllerDestino = "Compras", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult TI_Permisos(int idOperation = 1501)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "GestionPermisos", controllerDestino = "TI_Sistemas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult TI_Soporte(int idOperation = 1508)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "GestionTicketsSoporte", controllerDestino = "TI_Sistemas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult INV_Inventario(int idOperation = 1600)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ContabilizacionInventario", controllerDestino = "Almacen", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult DT_OrdenesDeVenta(int idOperation = 1700)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoOrdenesDeVenta", controllerDestino = "DireccionTecnica", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult DT_FacturasDeVenta(int idOperation = 1800)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoFacturasDeVenta", controllerDestino = "DireccionTecnica", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult DT_CalculadoraPdf(int idOperation = 1805)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "CalcularPdfsDescarga", controllerDestino = "DireccionTecnica", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult DT_BoletasDeVenta(int idOperation = 1900)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoBoletasDeVenta", controllerDestino = "DireccionTecnica", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult DT_EntregasDeVenta(int idOperation = 2000)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoEntregasDeVenta", controllerDestino = "DireccionTecnica", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult DT_NotasDeCreditoVenta(int idOperation = 2100)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoNotasDeCreditoVenta", controllerDestino = "DireccionTecnica", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult DT_TransferenciasDeStock(int idOperation = 2200)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoTransferenciasDeStock", controllerDestino = "DireccionTecnica", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult DT_EntradasDeMercancias(int idOperation = 2300)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoEntradasDeMercancias", controllerDestino = "DireccionTecnica", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult DT_FacturasDeProveedores(int idOperation = 2400)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoFacturasDeProveedores", controllerDestino = "DireccionTecnica", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult DT_MaestroDeArticulos(int idOperation = 2500)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoMaestroDeArticulos", controllerDestino = "DireccionTecnica", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult DT_Reportes(int idOperation = 2600)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "Reportes", controllerDestino = "DireccionTecnica", usuario = (Usuario_E)Session["UsuarioId"] });
        }

        public ActionResult DT_RegistrosSanitarios(int idOperation = 2900)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "RegistrosSanitarios", controllerDestino = "DireccionTecnica", usuario = (Usuario_E)Session["UsuarioId"] });
        }

        public ActionResult ATC_Solicitud(int idOperation = 2700)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "GestionSolicitud", controllerDestino = "AtencionCliente", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult VT_TicketsGuias(int idOperation = 2800)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoTicketsGuiasRemision", controllerDestino = "Ventas", usuario = (Usuario_E)Session["UsuarioId"] });
        }

    }
}