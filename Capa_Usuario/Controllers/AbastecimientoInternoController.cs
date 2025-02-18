using Capa_Entidad.AbastecimientoInterno_ENT.Interfaces;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasExternas;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasExternas;
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
        private readonly OWTQ_N _solicitudTrasladoHanaN = new OWTQ_N();
        private readonly StockMinProductos_N _stockMinProdN = new StockMinProductos_N();
        private readonly UbicacionesReserva_N _ubicacionReservaN = new UbicacionesReserva_N();
        private readonly TransferenciaStock_N _transferenciaStockN = new TransferenciaStock_N();
        private readonly LotesRegistroSanitario_N _lotesRegistroSanitarioN = new LotesRegistroSanitario_N();
        private readonly SolicitudTraslado_N _solicitudTrasladoN = new SolicitudTraslado_N();

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
                .GroupBy(u => new { u.ItemCode, u.ItemName })
                .Select(grupo => new UbicacionesReserva_E
                {
                    ItemCode = grupo.Key.ItemCode,
                    ItemName = grupo.Key.ItemName,
                    CantidadUbicaciones = grupo.Count(),
                    Ubicaciones = grupo.ToList()
                })
                .ToDictionary(x => x.ItemCode);

            return PartialView("AbastecimientoInterno/_ListadoUbicacionesReserva", listaAgrupada);
        }

        public JsonResult RegistrarUbicacionReserva(UbicacionesReserva_E form)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Mensaje = "No se pudo completar la acción", Comentario = "Inicia sesión nuevamente para continuar", Icono = "error" });

            form.NombreOperarioAccion = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";
            var result = _ubicacionReservaN.RegistrarUbicacionReserva(form);
            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";

            return Json(new { Mensaje = tituloSweetAlert, Comentario = result.Mensajes, Icono = result.IconoSweetAlert });
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

        public JsonResult EliminarUbicacionGeneral(string codigoUbicacion)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Mensaje = "No se pudo completar la acción", Comentario = "Inicia sesión nuevamente para continuar", Icono = "error" });

            var result = _ubicacionReservaN.EliminarUbicacionGeneral(codigoUbicacion);
            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";

            return Json(new { Mensaje = tituloSweetAlert, Comentario = new List<string> { result.Mensaje }, Icono = result.IconoSweetAlert });
        }

        /************************* S O L I C I T U D   D E   T R A S L A D O *************************/
        public JsonResult BuscarSolicitudDeTraslado(int DocNum)
        {
            // Buscar en mis tablas internas
            var trasladoInterno = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(DocNum);

            var trasladoHana = trasladoInterno == null ? _solicitudTrasladoHanaN.BuscarSolicitudDeTraslado(DocNum) : null;

            // Si no hay resultado en ninguna fuente
            if (trasladoInterno == null && trasladoHana == null)
            {
                return Json(new { Mensaje = "Error", Comentario = new List<string> { "No existe ningún resultado" }, Icono = "error" });
            }
            ITraslado traslado = trasladoInterno as ITraslado ?? trasladoHana as ITraslado;

            if (traslado?.Id > 0)
            {
                // Código cuando traslado.Id > 0 quiere decir que vino la informacion de tabla interna, buscar lo insertado en comparacion con transferencia
            }
            return Json(traslado);
        }


        public JsonResult RegistrarTransferenciaDeStock(SolicitudTraslado_E obj, TransferenciaStock_E transferencia)
        {
            //buscar si la solicitud de traslado ya estaba previamente importada
            var traslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(obj.DocNum);
            if (traslado == null) { 
                //Importa solo si no existe previamente el DocNum
                traslado = _solicitudTrasladoN.ImportarSolicitudDeTraslado(obj);

            }
            //Si no hay traslado valido no registra la transferencia
            if (traslado.Id == 0) {
                return Json(new { Mensaje = "No se pudo completar la acción", Comentario = new List<string> { "No se encuentra Solicitud de traslado válida o relacionada" }, Icono = "error" }); 
            }
            //Revisar LotesRegistroSanitario, si hay uno nuevo se inserta
            _lotesRegistroSanitarioN.ValidarLotesRegistroSanitario(transferencia.Detalle);

            transferencia.SolicitudTrasladoId = traslado.Id;
            var result = _transferenciaStockN.RegistrarTransferenciaDeStock(transferencia);

            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            return Json(new { Mensaje = tituloSweetAlert, Comentario = new List<string> { result.Mensaje }, Icono = result.IconoSweetAlert });
        }

        
    }
}