using Capa_Entidad.AbastecimientoInterno_ENT.Interfaces;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasExternas;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasSql;
using Capa_Usuario.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
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
        private readonly Ubicaciones_N _ubicacionesN = new Ubicaciones_N();
        private readonly TransferenciaReserva_N _transferenciaReservaN = new TransferenciaReserva_N();
        private readonly LotesRegistroSanitario_N _lotesRegistroSanitarioN = new LotesRegistroSanitario_N();
        private readonly SolicitudesTraslado_N _solicitudTrasladoN = new SolicitudesTraslado_N();
        private readonly Masters_N _masterN = new Masters_N();
        private readonly KardexAbastecimiento_N _kardexAbastecimientoN = new KardexAbastecimiento_N();

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
                    Ubicaciones = grupo.Select(u => (Ubicaciones_E)u).ToList(),
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
                    Ubicaciones = grupo.Select(u => (Ubicaciones_E)u).ToList()
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

            var result = _ubicacionesN.EliminarUbicacionGeneral(codigoUbicacion);
            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";

            return Json(new { Mensaje = tituloSweetAlert, Comentario = new List<string> { result.Mensaje }, Icono = result.IconoSweetAlert });
        }

        /************************* S O L I C I T U D   D E   T R A S L A D O *************************/
        public JsonResult BuscarSolicitudDeTraslado(int docNum)
        {
            var trasladoInterno = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(docNum);
            var trasladoHana = trasladoInterno == null ? _solicitudTrasladoHanaN.BuscarSolicitudDeTraslado(docNum) : null;

            if (trasladoInterno == null && trasladoHana == null)
            {
                var tituloSweetAlert = "No se pudo completar la acción";
                var icono = "error";
                var mensaje = "No existe ningun resultado";

                return Json(new { Mensaje = tituloSweetAlert, Comentario = new List<string> { mensaje }, Icono = icono });
            }

            ITraslado traslado = trasladoInterno as ITraslado ?? trasladoHana as ITraslado;
            TransferenciaReserva_E transferencia = null;
            if (traslado?.Id > 0)
            {
                // Código cuando traslado.Id > 0 quiere decir que vino la informacion de tabla interna, buscar lo insertado en comparacion con transferencia
                transferencia = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum);
            }

            return Json(new { traslado, transferencia });
        }

        public JsonResult BuscarUbicaciones(string almacen, string itemCode)
        {
            var result = _ubicacionesN.BuscarUbicaciones(almacen, itemCode);

            return Json(result);
        }

        public ActionResult SolicitudesTraslado(int idOperation = 3300)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Masters = _masterN.ListarMasters();

                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public JsonResult RegistrarTransferenciaDeStock(SolicitudesTraslado_E solicitudTraslado, TransferenciaReserva_E transferenciaGet)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required,
                   new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                try
                {
                    // Buscar si la solicitud de traslado ya fue importada previamente
                    var traslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(solicitudTraslado.DocNum);

                    // Importa solo si no existe previamente el DocNum
                    if (traslado == null)
                    {
                        traslado = _solicitudTrasladoN.ImportarSolicitudDeTraslado(solicitudTraslado);
                    }

                    // Validar si la importación de la solicitud de traslado fue exitosa
                    if (traslado == null || traslado.Id == 0)
                    {
                        return Json(new
                        {
                            Mensaje = "No se pudo completar la acción",
                            Comentario = new List<string> { "No se importó la Solicitud de Traslado" },
                            Icono = "error"
                        });
                    }

                    // Revisar y validar los lotes de registro sanitario
                    _lotesRegistroSanitarioN.ValidarLotesRegistroSanitario(solicitudTraslado.Detalle);

                    Usuario_E user = (Usuario_E)Session["UsuarioId"];

                    // Asignar el ID de la solicitud de traslado a la transferencia
                    transferenciaGet.SolicitudTrasladoId = traslado.Id;
                    transferenciaGet.SolicitudTrasladoDocNum = traslado.DocNum;
                    transferenciaGet.NroGuia = traslado.NroGuia;
                    transferenciaGet.OperarioRegistra = $"{user.Nombres} {user.Apellidos}";

                    if (transferenciaGet.OperarioRegistra != null)
                    {
                        // Registrar la transferencia de reserva
                        var transferenciaPost = _transferenciaReservaN.RegistrarTransferenciaReserva(transferenciaGet);

                        // Validar si la transferencia se registró correctamente
                        if (transferenciaPost == null || transferenciaPost.Id == 0)
                        {
                            return Json(new
                            {
                                Mensaje = "No se pudo completar la acción",
                                Comentario = new List<string> { "No se registró la Transferencia Reserva" },
                                Icono = "error"
                            });
                        }
                        else
                        {
                            //Registrar operacion de Ingreso (siempre positivo) en KardexAbastecimiento
                            var kardexAbastecimientoIngreso = _kardexAbastecimientoN.InsertarTransaccionIngresoKardex(transferenciaGet);
                            // Id,ItemCode,ItemName,Almacen,RucProveedor,NombreProveedor,Sentido,Tabla,Referencia,Cantidad,Imputado,Operario,TiempoRegistro

                            //Sumar y/o Registrar en la tabla UbicacionesLotesMaster

                            //Sumar y/o Registrar Quantity en Cajas en la tabla UbicacionesLotes
                        }

                        // Confirmar la transacción
                        scope.Complete();

                        // Devolver respuesta exitosa
                        return Json(new
                        {
                            Mensaje = "Acción completada exitosamente",
                            Comentario = new List<string> { "Se registró la Transferencia Reserva correctamente" },
                            Icono = "success"
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Mensaje = "Error en la operación",
                            Comentario = new List<string> { "No existe usuario logueado, se terminó la sesión." },
                            Icono = "error"
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        Mensaje = "Error en la operación",
                        Comentario = new List<string> { ex.Message },
                        Icono = "error"
                    });
                }
            }
        }

    }
}