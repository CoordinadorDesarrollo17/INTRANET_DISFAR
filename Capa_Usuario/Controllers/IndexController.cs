using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio;
using Capa_Negocio.Seguridad_NEG;
using Capa_Negocio.Ventas_NEG.TablasSql;
using System;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Capa_Usuario.Controllers
{
    public class IndexController : Controller
    {
        //operacion 1--99
        int modulo = 1;
        Rol1_N rol1 = new Rol1_N();
        BaseDeDatos_N bd_N = new BaseDeDatos_N();
        Utilitarios_N utiN = new Utilitarios_N();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Sesion()
        {
            Usuario_E user = (Usuario_E)Session["UsuarioId"];
            if (user != null)
            {
                Capa_Negocio.Seguridad_NEG.TablasSql.AREA_N areaN = new Capa_Negocio.Seguridad_NEG.TablasSql.AREA_N();
                Capa_Negocio.Seguridad_NEG.TablasSql.REA1_N rea1N = new Capa_Negocio.Seguridad_NEG.TablasSql.REA1_N();
                ViewBag.AREAS = areaN.listarAreas();
                ViewBag.AREASFC = rea1N.listarAreasFc();
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
                Usuario_E usuario = new Usuario_N().buscarUsuarioSesion(user, pass);

                if (usuario != null)
                {
                    Session["UsuarioId"] = usuario;
                    utiN.registrarLog(user, " inicio Sesion", 0, Request.UserHostAddress, Request.UserHostName);

                    // No está claro para qué se usa neg aquí, pero lo he dejado en caso de que sea necesario
                    ORTV_N neg = new ORTV_N();

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
        public ActionResult ALM_Devoluciones(int idOperation = 100)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "DevolucionMercancias", "Almacen", "Error");
        }
        public ActionResult ALM_Repartos(int idOperation = 200)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoRepartos", "Rutas", "Error");
        }
        public ActionResult ALM_Rutas(int idOperation = 200)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoRutas", "Rutas", "Error");
        }
        public ActionResult RUT_ReportesRutas(int idOperation = 206)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ReportesRutas", "Rutas", "Error");
        }
        public ActionResult RH_Control_Asistencias(int idOperation = 400)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "GestionColaboradores", "RecursosHumanos", "Error");
        }
        public ActionResult VT_Tickets(int idOperation = 500)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoTicketsVenta", "Ventas", "Error");
        }
        public ActionResult VT_TicketsFacturacion(int idOperation = 600)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoTicketsFacturacion", "Ventas", "Error");
        }
        public ActionResult VT_TicketsRecepcion(int idOperation = 700)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoTicketsRecepcion", "Ventas", "Error");
        }
        public ActionResult VT_TicketsAlmacen(int idOperation = 800)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoTicketsAlmacen", "Ventas", "Error");
        }
        public ActionResult VT_TicketsDespacho(int idOperation = 900)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoTicketsDespacho", "Ventas", "Error");
        }
        public ActionResult VT_TicketsProtocolos(int idOperation = 1000)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoTicketsProtocolos", "Ventas", "Error");
        }
        public ActionResult CP_LineasProduccion(int idOperation = 1100)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoLineasProduccion", "Compras", "Error");
        }
        public ActionResult CP_ContratosRebate(int idOperation = 1200)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoContratosRebate", "Compras", "Error");
        }
        public ActionResult VT_LibrosSaldos(int idOperation = 1300)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoLibrosSaldo", "Ventas", "Error");
        }
        public ActionResult VT_ReportesVentas(int idOperation = 1306)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ReportesVentas", "Ventas", "Error");
        }
        public ActionResult VT_Regalos(int idOperation = 1310)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "GestionRegalos", "Ventas", "Error");
        }
        public ActionResult VT_PedidosOnline(int idOperation = 1326)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "PedidosOnline", "Ventas", "Error");
        }

        /******************************** M Ó D U L O   C A J A ********************************/
        public ActionResult CJ_ListadoTickets(int idOperation = 3000)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "Index", "Caja", "Error");
        }
        /****************************************************************************************/

        public ActionResult CP_ResumenRebate(int idOperation = 1400)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ResumenRebate", "Compras", "Error");
        }
        public ActionResult TI_Permisos(int idOperation = 1501)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "GestionPermisos", "TI_Sistemas", "Error");
        }
        public ActionResult TI_Soporte(int idOperation = 1508)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "GestionTicketsSoporte", "TI_Sistemas", "Error");
        }
        public ActionResult INV_Inventario(int idOperation = 1600)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ContabilizacionInventario", "Almacen", "Error");
        }
        public ActionResult DT_OrdenesDeVenta(int idOperation = 1700)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoOrdenesDeVenta", "DireccionTecnica", "Error");
        }
        public ActionResult DT_FacturasDeVenta(int idOperation = 1800)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoFacturasDeVenta", "DireccionTecnica", "Error");
        }
        public ActionResult DT_CalculadoraPdf(int idOperation = 1805)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "CalcularPdfsDescarga", "DireccionTecnica", "Error");
        }
        public ActionResult DT_BoletasDeVenta(int idOperation = 1900)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoBoletasDeVenta", "DireccionTecnica", "Error");
        }
        public ActionResult DT_EntregasDeVenta(int idOperation = 2000)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoEntregasDeVenta", "DireccionTecnica", "Error");
        }
        public ActionResult DT_NotasDeCreditoVenta(int idOperation = 2100)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoNotasDeCreditoVenta", "DireccionTecnica", "Error");
        }
        public ActionResult DT_TransferenciasDeStock(int idOperation = 2200)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoTransferenciasDeStock", "DireccionTecnica", "Error");
        }
        public ActionResult DT_EntradasDeMercancias(int idOperation = 2300)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoEntradasDeMercancias", "DireccionTecnica", "Error");
        }
        public ActionResult DT_FacturasDeProveedores(int idOperation = 2400)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoFacturasDeProveedores", "DireccionTecnica", "Error");
        }
        public ActionResult DT_MaestroDeArticulos(int idOperation = 2500)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoMaestroDeArticulos", "DireccionTecnica", "Error");
        }
        public ActionResult DT_Reportes(int idOperation = 2600)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "Reportes", "DireccionTecnica", "Error");
        }

        public ActionResult DT_RegistrosSanitarios(int idOperation = 2900)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "RegistrosSanitarios", "DireccionTecnica", "Error");
        }

        public ActionResult ATC_Solicitud(int idOperation = 2700)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "GestionSolicitud", "AtencionCliente", "Error");
        }
        public ActionResult VT_TicketsGuias(int idOperation = 2800)
        {
            return validaAccesos((Usuario_E)Session["UsuarioId"], idOperation, "Index", "ListadoTicketsGuiasRemision", "Ventas", "Error");
        }

        /////////////////////////////////////////////////////////////////////////////
        private ActionResult validaAccesos(Usuario_E user, int ope, string ActionE_Login, string ActionC_Acccess, string ActionAccessController, string ActionE_Access)
        {
            string action = "";
            string nombreOperacion = this.ControllerContext.RouteData.Values["action"].ToString();
            if (user == null)
            { action = ActionE_Login; return RedirectToAction(action); }
            else
            {
                if ((rol1.verificarAccesoOperacion(user.IdRol, ope, nombreOperacion, modulo) == 1) || (user.IdRol == 1))
                { action = ActionC_Acccess; return RedirectToAction(action, ActionAccessController); }
                else
                { action = ActionE_Access; return RedirectToAction(action); }
            }
        }


    }
}
