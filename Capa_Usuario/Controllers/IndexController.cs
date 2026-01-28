using Capa_Entidad;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio;
using Capa_Negocio.General_NEG.TablasSql;
using Capa_Negocio.Seguridad_NEG;
using Capa_Usuario.Helpers;
using System;
using System.Data.Entity.Core.Metadata.Edm;
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
        private readonly MenuSistema_N _menuSistema = new MenuSistema_N();

        private string ObtenerIPCliente()
        {
            string clientIp = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrWhiteSpace(clientIp))
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
            Usuario_E user = (Usuario_E)Session["UsuarioId"];
            if (user != null)
            {
                var result  = _menuSistema.ListarMenuSistema();
                var menu = result.Item2.Where(m => m.Nivel == 1 && m.SuperiorId == null)
                    .OrderBy(m => m.Orden)
                    .ToList();

                foreach (var sm2 in menu)
                {
                    var menuN2 = result.Item2
                        .Where(m => m.Nivel == 2 && m.SuperiorId == sm2.Id)
                        .OrderBy(m => m.Orden)
                        .ToList();

                    sm2.SubMenus.AddRange(menuN2);

                    foreach (var sm3 in menuN2)
                    {
                        var menuN3 = result.Item2
                            .Where(m => m.Nivel == 3 && m.SuperiorId == sm3.Id)
                            .OrderBy(m => m.Orden)
                            .ToList();

                        sm3.SubMenus.AddRange(menuN3);

                        foreach (var sm4 in menuN3)
                        {
                            var menuN4 = result.Item2
                                .Where(m => m.Nivel == 4 && m.SuperiorId == sm4.Id)
                                .OrderBy(m => m.Orden)
                                .ToList();

                            sm4.SubMenus.AddRange(menuN4);
                        }

                    }
                }

                ViewBag.Mensaje = result.Item1;
                ViewBag.NombreUsuario = $"{user.Nombres} {user.Apellidos}";
                ViewBag.IdRol = user.IdRol;

                return View(menu);
            }
            else
            {
                return RedirectToAction("Sesion");
            }
        }
        public ActionResult BuscarUsuarioLogueado()
        {
            Usuario_E user = (Usuario_E)Session["UsuarioId"];
            if (user != null)
            {
                return Content($"{user.Nombres} {user.Apellidos}");
            }
            else { return null; }
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

        ///******************************** M Ó D U L O  A B A S T E C I M I E N T O  I N T E R N O ********************************/
        public ActionResult AI_UbicacionesPicking(int idOperation = 3100)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "UbicacionesPicking", controllerDestino = "AbastecimientoInterno", usuario = (Usuario_E)Session["UsuarioId"] });
        }

        public ActionResult AI_UbicacionesReserva(int idOperation = 3200)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "UbicacionesReserva", controllerDestino = "AbastecimientoInterno", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        
        public ActionResult AI_Transferencias(int idOperation = 3300)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "Transferencias", controllerDestino = "AbastecimientoInterno", usuario = (Usuario_E)Session["UsuarioId"] });
        }

        public ActionResult AI_Requerimientos(int idOperation = 3400)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "Requerimientos", controllerDestino = "AbastecimientoInterno", usuario = (Usuario_E)Session["UsuarioId"] });
        }

        public ActionResult AI_ApilarRequerimientos(int idOperation = 3500)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ApilarRequerimientos", controllerDestino = "AbastecimientoInterno", usuario = (Usuario_E)Session["UsuarioId"] });
        }

        public ActionResult AI_ApilarIngreso(int idOperation = 3502)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ApilarIngreso", controllerDestino = "AbastecimientoInterno", usuario = (Usuario_E)Session["UsuarioId"] });
        }

        public ActionResult AI_Reabastecimiento(int idOperation = 3600)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "Reabastecimiento", controllerDestino = "AbastecimientoInterno", usuario = (Usuario_E)Session["UsuarioId"] });
        }

        public ActionResult AI_ControlStockPicking(int idOperation = 3700)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ControlStockPicking", controllerDestino = "AbastecimientoInterno", usuario = (Usuario_E)Session["UsuarioId"] });
        }

        public ActionResult AI_PackingList(int idOperation = 3800)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "PackingList", controllerDestino = "AbastecimientoInterno", usuario = (Usuario_E)Session["UsuarioId"] });
        }

        public ActionResult AI_ReportesExcel(int idOperation = 3801)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ReportesExcel", controllerDestino = "AbastecimientoInterno", usuario = (Usuario_E)Session["UsuarioId"] });
        }


        ///****************************************************************************************/
        /*********************************** M Ó D U L O   R E C U R S O S   H U M A N O S ***********************************/
        public ActionResult RRHH_AdministracionRRHH(int idOperation = 4000)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "Index", controllerDestino = "RecursosHumanos", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        /********************************************************************************************************/
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

        public ActionResult DT_Internamientos(int idOperation = 6000)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "Index", controllerDestino = "Internamientos", usuario = (Usuario_E)Session["UsuarioId"] });
        }

        public ActionResult DT_SolicitudesReversion_DOCS1(int idOperation = 6100)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "Index", controllerDestino = "SolicitudesReversion_DOCS1", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult ALM_Transferencias(int idOperation = 6200)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "Index", controllerDestino = "Transferencias", usuario = (Usuario_E)Session["UsuarioId"] });
        }

        public ActionResult ATC_Solicitud(int idOperation = 2700)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "GestionSolicitud", controllerDestino = "AtencionCliente", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult VT_TicketsGuias(int idOperation = 2800)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "ListadoTicketsGuiasRemision", controllerDestino = "Ventas", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        public ActionResult ATC_RegalosAplicados(int idOperation = 2712)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "RegalosAplicados", controllerDestino = "AtencionCliente", usuario = (Usuario_E)Session["UsuarioId"] });
        }
        
        public ActionResult ATC_HojaRuta(int idOperation = 2714)
        {
            return AccesoHelper.GestionarAccesoIndex(this, new AccessoHelper_E { OpeID = idOperation, action = "HojaRuta", controllerDestino = "AtencionCliente", usuario = (Usuario_E)Session["UsuarioId"] });
        }
    }
}