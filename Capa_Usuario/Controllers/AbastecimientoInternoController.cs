using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasSql;
using Capa_Usuario.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Capa_Usuario.Controllers
{
    public class AbastecimientoInternoController : Controller
    {
        private readonly UbicacionesPicking_N _ubicacionPickingN = new UbicacionesPicking_N();
        private readonly Productos_N _productosN = new Productos_N();
        private readonly StockMinProductos_N _stockMinProdN = new StockMinProductos_N();
        private readonly UbicacionesReserva_N _ubicacionReservaN = new UbicacionesReserva_N();

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

        /************************* U B I C A C I O N E S   P I C K I N G *************************/
        public ActionResult UbicacionesPicking(int idOperation = 3100)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Productos = _productosN.ListarProductos();

                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }

        [HttpGet]
        public ActionResult ListarUbicacionesPicking(UbicacionesPicking_E filtros)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Mensaje = "No se pudo completar la acción", Comentario = "Inicia sesión nuevamente para continuar", Icono = "error" });

            var listaAgrupada = _ubicacionPickingN.ListarUbicacionesPicking(filtros)
                .GroupBy(u => new { u.ItemCode, u.ItemName, u.StockMinAbastecimiento, u.StockMinVenta })
                .Select(grupo => new UbicacionesPicking_E
                {
                    ItemCode = grupo.Key.ItemCode,
                    ItemName = grupo.Key.ItemName,
                    CantidadUbicaciones = grupo.Count(),
                    Ubicaciones = grupo.ToList(),
                    StockMinAbastecimiento = grupo.Key.StockMinAbastecimiento,
                    StockMinVenta = grupo.Key.StockMinVenta,
                })
                .ToDictionary(x => x.ItemCode);


            return PartialView("AbastecimientoInterno/_ListadoUbicacionesPicking", listaAgrupada);
        }

        public JsonResult RegistrarUbicacionPicking(UbicacionesPicking_E form)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Mensaje = "No se pudo completar la acción", Comentario = "Inicia sesión nuevamente para continuar", Icono = "error" });

            form.NombreOperarioAccion = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";
            var result = _ubicacionPickingN.RegistrarUbicacionPicking(form);
            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";

            return Json(new { Mensaje = tituloSweetAlert, Comentario = result.Mensajes, Icono = result.IconoSweetAlert });
        }

        public JsonResult EliminarUbicacionPicking(int id)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Mensaje = "No se pudo completar la acción", Comentario = "Inicia sesión nuevamente para continuar", Icono = "error" });

            var result = _ubicacionPickingN.EliminarUbicacionPicking(id);
            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";

            return Json(new { Mensaje = tituloSweetAlert, Comentario = new List<string> { result.Mensaje }, Icono = result.IconoSweetAlert });
        }

        public JsonResult ActualizarStocksMinimos(StockMinProductos_E form)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Mensaje = "No se pudo completar la acción", Comentario = "Inicia sesión nuevamente para continuar", Icono = "error" });

            form.NombreOperarioAccion = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";
            var result = _stockMinProdN.ActualizarStocksMinimos(form);
            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";

            return Json(new
            {
                Mensaje = tituloSweetAlert,
                Comentario = result.Mensajes,
                Icono = result.IconoSweetAlert
            });
        }

        /************************* U B I C A C I O N E S   R E S E R V A *************************/
        public ActionResult UbicacionesReserva(int idOperation = 3200)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Productos = _productosN.ListarProductos();

                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }

        [HttpGet]
        public ActionResult ListarUbicacionesReserva(UbicacionesReserva_E filtros)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Mensaje = "No se pudo completar la acción", Comentario = "Inicia sesión nuevamente para continuar", Icono = "error" });

            var listaAgrupada = _ubicacionReservaN.ListarUbicacionesReserva(filtros)
                .GroupBy(u => new { u.ItemCode, u.ItemName, u.StockMinAbastecimiento, u.StockMinVenta })
                .Select(grupo => new UbicacionesReserva_E
                {
                    ItemCode = grupo.Key.ItemCode,
                    ItemName = grupo.Key.ItemName,
                    CantidadUbicaciones = grupo.Count(),
                    Ubicaciones = grupo.ToList(),
                    StockMinAbastecimiento = grupo.Key.StockMinAbastecimiento,
                    StockMinVenta = grupo.Key.StockMinVenta,
                })
                .ToDictionary(x => x.ItemCode);

            return PartialView("AbastecimientoInterno/_ListadoUbicacionesReserva", listaAgrupada);
        }

        public JsonResult EliminarUbicacionReserva(int id)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Mensaje = "No se pudo completar la acción", Comentario = "Inicia sesión nuevamente para continuar", Icono = "error" });

            var result = _ubicacionReservaN.EliminarUbicacionReserva(id);
            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";

            return Json(new { Mensaje = tituloSweetAlert, Comentario = new List<string> { result.Mensaje }, Icono = result.IconoSweetAlert });
        }
    }
}