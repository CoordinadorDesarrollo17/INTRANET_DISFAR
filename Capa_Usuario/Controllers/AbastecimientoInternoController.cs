using Aspose.Pdf.Operators;
using Capa_Datos;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.Interfaces;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasExternas;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasSql;
using Capa_Usuario.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.util;
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
        private readonly UbicacionesLotesMaster_N _ubicacionesLotesMasterN = new UbicacionesLotesMaster_N();
        private readonly UbicacionesLotes_N _ubicacionesLotesN = new UbicacionesLotes_N();
        private readonly Requerimientos_N _requerimientosN = new Requerimientos_N();
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
            ITraslado traslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(docNum) as ITraslado
                        ?? _solicitudTrasladoHanaN.BuscarSolicitudDeTraslado(docNum) as ITraslado;
            if (traslado == null)
            {
                var tituloSweetAlert = "No se pudo completar la acción";
                var icono = "error";
                var mensaje = "No existe ningun resultado";
                return Json(new { Mensaje = tituloSweetAlert, Comentario = new List<string> { mensaje }, Icono = icono });
            }
            TransferenciaReserva_E transferencia = null;
            if (traslado?.Id > 0)
            {
                // Código cuando traslado.Id > 0 quiere decir que vino la informacion de tabla interna, buscar lo insertado en comparacion con transferencia
                transferencia = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum);
                if (transferencia == null)
                {
                    var tituloSweetAlert = "No se pudo completar la acción";
                    var icono = "error";
                    var mensaje = "No existe ningun resultado de transferencia relacionada a la solicitud de traslado que ya esta registrada.";
                    return Json(new { Mensaje = tituloSweetAlert, Comentario = new List<string> { mensaje }, Icono = icono });
                }
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
        public JsonResult RegistrarTransferenciaDeStock(SolicitudesTraslado_E solicitudTraslado, TransferenciaReserva_E transferenciaPost)
        {
            try
            {
                if (transferenciaPost != null)
                {
                    // 1. Obtener la solicitud de traslado
                    var traslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(solicitudTraslado.DocNum);
                    // 2. Importa solo si no existe previamente el DocNum
                    if (traslado == null)
                    {
                        traslado = _solicitudTrasladoN.ImportarSolicitudDeTraslado(solicitudTraslado);
                    }
                    // 3. Validar si la importación fue exitosa
                    if (traslado == null || traslado.Id == 0)
                    {
                        return Json(new
                        {
                            Mensaje = "No se pudo completar la acción",
                            Comentario = new List<string> { "No se importó la Solicitud de Traslado" },
                            Icono = "error"
                        });
                    }
                    // 4. Validar los lotes de registro sanitario (fuera de la transacción)
                    _lotesRegistroSanitarioN.ValidarLotesRegistroSanitario(solicitudTraslado.Detalle);
                    // 5. Obtener usuario de sesión (fuera de la transacción)
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    if (user == null)
                    {
                        return Json(new
                        {
                            Mensaje = "Error en la operación",
                            Comentario = new List<string> { "No existe usuario logueado, se terminó la sesión." },
                            Icono = "error"
                        });
                    }
                    // 6. Asignar datos de traslado a la transferencia
                    transferenciaPost.OperarioRegistra = $"{user.Nombres} {user.Apellidos}";
                    transferenciaPost.SolicitudTrasladoId = traslado.Id;
                    transferenciaPost.SolicitudTrasladoDocNum = traslado.DocNum;

                    Utilitarios uti = new Utilitarios();
                    // 7. Iniciar la transacción global para las operaciones críticas
                    using (var scope = new TransactionScope(TransactionScopeOption.Required,
                       new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                       TransactionScopeAsyncFlowOption.Enabled))
                    {
                        using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                        {
                            cn.Open();
                            // 7.1 Registrar la transferencia de reserva
                            var transferenciaGet = _transferenciaReservaN.RegistrarTransferenciaReserva(transferenciaPost, cn);
                            if (transferenciaGet == null || transferenciaGet.Id == 0)
                            {
                                // 7.1.1 Eliminar la solicitud de traslado si en caso se importo a la tabla interna pero no se ha encontrado una transferencia
                                var resultEliminacionTraslado = _solicitudTrasladoN.DeleteSolicitudDeTraslado(traslado.DocNum, cn);
                                if (resultEliminacionTraslado.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Mensaje = "No se pudo completar la acción",
                                        Comentario = new List<string> { resultEliminacionTraslado.Mensaje },
                                        Icono = resultEliminacionTraslado.IconoSweetAlert
                                    });
                                }
                            }
                            // 7.2 Registrar la(s) operación(es) de ingreso(s) en KardexAbastecimiento - Los datos a insertar son los del detalle en transferencia
                            // TransferenciaGet ya tiene los datos limpios por enviar hacia el kardex y la suma de stocks
                            var resultKardex = _kardexAbastecimientoN.InsertarTransaccionIngresoKardex(transferenciaGet, cn);
                            if (resultKardex.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { resultKardex.Mensaje },
                                    Icono = resultKardex.IconoSweetAlert
                                });
                            }
                            // 7.3 Sumar y/o Registrar Quantity en Cajas en la tabla UbicacionesLotes
                            var resultUbicacionesLotes = new Helper_E();

                            foreach (var item in transferenciaGet.Detalle) {
                                resultUbicacionesLotes = _ubicacionesLotesN.Ingreso(item, cn);
                                if (resultUbicacionesLotes.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Mensaje = "No se pudo completar la acción",
                                        Comentario = new List<string> { resultUbicacionesLotes.Mensaje },
                                        Icono = resultUbicacionesLotes.IconoSweetAlert
                                    });
                                }

                                var ubicacionLoteId = resultUbicacionesLotes.Mensaje;
                                // 7.4 Sumar y/o Registrar en la tabla UbicacionesLotesMaster
                                var resultUbicacionesLotesMaster = _ubicacionesLotesMasterN.Ingreso(Convert.ToInt32(ubicacionLoteId),item, cn);
                                if (resultUbicacionesLotesMaster.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Mensaje = "No se pudo completar la acción",
                                        Comentario = new List<string> { resultUbicacionesLotesMaster.Mensaje },
                                        Icono = resultUbicacionesLotesMaster.IconoSweetAlert
                                    });
                                }

                            }
                            
                            
                            // 8. Confirmar la transacción
                            scope.Complete();
                        }
                    }
                    // 9. Devolver respuesta exitosa
                    return Json(new
                    {
                        Mensaje = "Acción completada exitosamente",
                        Comentario = new List<string> { "Se registró la Transferencia Reserva correctamente." },
                        Icono = "success"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Mensaje = "Error en la operación",
                        Comentario = new List<string> { "El documento que trata de registrar no tiene una transferencia realizandose." },
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
        public JsonResult CancelarTrasladoConTransferenciaReserva(int docNum) //recibe el docnum de la solicitud de traslado
        {
            try
            {
                if (docNum > 0)
                {
                    var transferenciaGet = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum);
                    if (transferenciaGet == null || transferenciaGet.Id == 0)
                    {
                        return Json(new
                        {
                            Mensaje = "No se pudo completar la acción",
                            Comentario = new List<string> { "No se encontró transferencia de reserva relacionada." },
                            Icono = "error"
                        });
                    }
                    // El elemento de transferencia trae los datos a revertir, por ultimo se eliminara tambien el DetalleSolicitud Traslado asi como la SolicitudTraslado
                    using (var scope = new TransactionScope(TransactionScopeOption.Required,
                             new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                             TransactionScopeAsyncFlowOption.Enabled))
                    {
                        Utilitarios uti = new Utilitarios();
                        using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                        {
                            cn.Open();

                            //  Restar y/o eliminar en la tabla UbicacionesLotesMaster
                            var resultUbicacionesLotesMaster = _ubicacionesLotesMasterN.RevertirIngreso(transferenciaGet, cn);
                            if (resultUbicacionesLotesMaster.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { resultUbicacionesLotesMaster.Mensaje },
                                    Icono = resultUbicacionesLotesMaster.IconoSweetAlert
                                });
                            }
                            // Restar y/o eliminar Quantity en Cajas en la tabla UbicacionesLotes
                            var resultUbicacionesLotes = _ubicacionesLotesN.RevertirIngreso(transferenciaGet, cn);
                            if (resultUbicacionesLotes.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { resultUbicacionesLotes.Mensaje },
                                    Icono = resultUbicacionesLotes.IconoSweetAlert
                                });
                            }
                            // Eliminar la(s) operación(es) de ingreso(s) en KardexAbastecimiento - Los datos a eliminar son los del detalle en transferencia
                            var resultKardex = _kardexAbastecimientoN.EliminarTotalTransaccionesIngresoKardex(docNum, cn);
                            if (resultKardex.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { resultKardex.Mensaje },
                                    Icono = resultKardex.IconoSweetAlert
                                });
                            }
                            // Eliminar Detalle y Cabecera de Transferencia de Reserva
                            var resultTransferencia = _transferenciaReservaN.DeleteTransferenciaReserva(docNum, cn);
                            if (resultTransferencia.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { resultTransferencia.Mensaje },
                                    Icono = resultTransferencia.IconoSweetAlert
                                });
                            }
                            //Eliminar Detalle y Cabecera de Solicitud de Traslado
                            var resultSolicitudTraslado = _solicitudTrasladoN.DeleteSolicitudDeTraslado(docNum, cn);
                            if (resultSolicitudTraslado.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { resultTransferencia.Mensaje },
                                    Icono = resultSolicitudTraslado.IconoSweetAlert
                                });
                            }
                            // Confirmar la transacción
                            scope.Complete();
                        }
                    }
                    return Json(new
                    {
                        Mensaje = "Acción completada exitosamente",
                        Comentario = new List<string> { "Se canceló la Transferencia Reserva y Solicitud de Traslado correctamente." },
                        Icono = "success"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Mensaje = "Error en la operación",
                        Comentario = new List<string> { "El docNum es invalido." },
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
        public JsonResult RevertirTransferenciaReservaPorItem(int docNum,int [] ids) //recibe el docnum de la solicitud de traslado y el array de Ids del detalle transferencia reserva que son de un solo ItemCode
        {
            try
            {
                if (docNum > 0)
                {
                    var transferenciaGet = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum);
                    var traslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(docNum);
                    if (transferenciaGet == null || transferenciaGet.Id == 0)
                    {
                        return Json(new
                        {
                            Mensaje = "No se pudo completar la acción",
                            Comentario = new List<string> { "No se encontró transferencia de reserva relacionada." },
                            Icono = "error"
                        });
                    }
                    
                    //Identificar solo los id's segun el array, reduciendo mi Detalle
                    if (transferenciaGet.Detalle != null)
                    {
                        transferenciaGet.Detalle = transferenciaGet.Detalle.Where(x => ids.Contains(x.Id)).ToList();
                    }

                    //Validar que los ids en cuanto a la suma de QuantityUnidadesCajas es igual a Quantity de traslado respecto a ese ItemCode
                    if(traslado.Detalle != null && transferenciaGet.Detalle != null)
                    {
                        traslado.Detalle = traslado.Detalle.Where(x => x.ItemCode.Equals(transferenciaGet.Detalle[0].ItemCode)).ToList();

                        //Si las cantidades no coinciden quiere decir que no se ha pasado el grupo completo de los ids correspondientes a un ItemCode en la solicitud de traslado
                        if (traslado.Detalle[0].QuantityCajas != transferenciaGet.Detalle.Sum(x => x.QuantityUnidadesCajas)){
                            return Json(new
                            {
                                Mensaje = "No se pudo completar la acción",
                                Comentario = new List<string> { $"La suma de cantidades a revertir no coincide con el total en la solicitud de traslado para el SKU:{ transferenciaGet.Detalle[0].ItemCode } "},
                                Icono = "error"
                            });
                        }

                        //Una vez validado los ids enviarlo a la transaccion unica para la reversion de todo el grupo segun ItemCode
                        using (var scope = new TransactionScope(TransactionScopeOption.Required,
                                 new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                                 TransactionScopeAsyncFlowOption.Enabled))
                        {
                            Utilitarios uti = new Utilitarios();
                            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                            {
                                cn.Open();

                                //  Restar y/o eliminar en la tabla UbicacionesLotesMaster
                                var resultUbicacionesLotesMaster = _ubicacionesLotesMasterN.RevertirIngreso(transferenciaGet, cn);
                                if (resultUbicacionesLotesMaster.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Mensaje = "No se pudo completar la acción",
                                        Comentario = new List<string> { resultUbicacionesLotesMaster.Mensaje },
                                        Icono = resultUbicacionesLotesMaster.IconoSweetAlert
                                    });
                                }
                                // Restar y/o eliminar Quantity en Cajas en la tabla UbicacionesLotes
                                var resultUbicacionesLotes = _ubicacionesLotesN.RevertirIngreso(transferenciaGet, cn);
                                if (resultUbicacionesLotes.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Mensaje = "No se pudo completar la acción",
                                        Comentario = new List<string> { resultUbicacionesLotes.Mensaje },
                                        Icono = resultUbicacionesLotes.IconoSweetAlert
                                    });
                                }
                                // Eliminar la operación de ingreso en KardexAbastecimiento que pertenece a dicho ItemCode - Los datos a eliminar son los del detalle en transferencia
                                var resultKardex = _kardexAbastecimientoN.EliminarPorItemCodeTransaccionIngresoKardex(docNum, transferenciaGet.Detalle[0].ItemCode, cn);
                                if (resultKardex.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Mensaje = "No se pudo completar la acción",
                                        Comentario = new List<string> { resultKardex.Mensaje },
                                        Icono = resultKardex.IconoSweetAlert
                                    });
                                }

                                //Eliminar los items de Detalle de Transferencia de Reserva 'REVERT' que sean del ItemCode
                                var resultTransferencia = _transferenciaReservaN.DeleteDetalleItemTransferenciaReserva(transferenciaGet.Detalle, cn);
                                if (resultTransferencia.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Mensaje = "No se pudo completar la acción",
                                        Comentario = new List<string> { resultTransferencia.Mensaje },
                                        Icono = resultTransferencia.IconoSweetAlert
                                    });
                                }
                                
                                // Confirmar la transacción
                                scope.Complete();
                            }
                        }

                        //Verificar si la TransferenciaReserva se quedo sin elementos 
                        var transferenciaPostReversion = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum);
                        if (transferenciaPostReversion != null && transferenciaPostReversion.Detalle.Count() == 0) 
                        { 
                            //Eliminar la transferencia 
                            var resultEliminarTransferencia= _transferenciaReservaN.DeleteTransferenciaReserva(docNum,null);
                            if (resultEliminarTransferencia.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { resultEliminarTransferencia.Mensaje },
                                    Icono = resultEliminarTransferencia.IconoSweetAlert
                                });
                            }

                            //Luego Eliminar Detalle y Cabecera de Solicitud de Traslado solo si la transferencia se quedo sin elementos en su detalle, genera una nueva connection
                            var resultSolicitudTraslado = _solicitudTrasladoN.DeleteSolicitudDeTraslado(docNum, null);
                            if (resultSolicitudTraslado.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { resultSolicitudTraslado.Mensaje },
                                    Icono = resultSolicitudTraslado.IconoSweetAlert
                                });
                            }
                        }
                       

                    }

                    return Json(new
                    {
                        Mensaje = "Acción completada exitosamente",
                        Comentario = new List<string> { "Se canceló la Transferencia Reserva y Solicitud de Traslado correctamente." },
                        Icono = "success"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Mensaje = "Error en la operación",
                        Comentario = new List<string> { "El docNum es invalido." },
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
        /****************************** R E Q U E R I M I E N T O S ****************************/
        public JsonResult RegistrarRequerimiento(Requerimientos_E requerimiento)
        {
            try
            {
                if (requerimiento != null)
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    if (user == null)
                    {
                        return Json(new
                        {
                            Mensaje = "Error en la operación",
                            Comentario = new List<string> { "No existe usuario logueado, se terminó la sesión." },
                            Icono = "error"
                        });
                    }
                    // Asignar datos de operario en el requerimiento
                    requerimiento.OperarioRegistra = $"{user.Nombres} {user.Apellidos}";

                    Utilitarios uti = new Utilitarios();

                    // Iniciar la transacción global para las operaciones críticas
                    using (var scope = new TransactionScope(TransactionScopeOption.Required,
                       new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                       TransactionScopeAsyncFlowOption.Enabled))
                    {
                        using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                        {
                            cn.Open();
                            // Registrar el requerimiento de picking hacia reserva
                            var requerimientoGet = _requerimientosN.RegistrarRequerimiento(requerimiento, cn);
                            if (requerimientoGet == null || requerimientoGet.Id == 0)
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { "No se completo el registro del requerimiento" },
                                    Icono = "error"
                                });
                            }
                            //Validar que exista cantidad disponible de los solicitado en requerimiento, segun TB UbicacionesLotesMaster
                            var resultKardexValidar = _kardexAbastecimientoN.ValidarTransaccionImputadoKardex(requerimientoGet, cn);
                            if (resultKardexValidar.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { resultKardexValidar.Mensaje },
                                    Icono = resultKardexValidar.IconoSweetAlert
                                });
                            }

                            // Registrar la(s) operación(es) de imputado(s) en KardexAbastecimiento - Los datos a insertar son los del detalle en requerimiento, RequerimientoGet ya tiene los datos limpios por enviar hacia el kardex como imputado, previamente validados
                            var resultKardexImputar = _kardexAbastecimientoN.InsertarTransaccionImputadoKardex(requerimientoGet, cn);
                            if (resultKardexImputar.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { resultKardexImputar.Mensaje },
                                    Icono = resultKardexImputar.IconoSweetAlert
                                });
                            }
                           
                            // Confirmar la transacción
                            scope.Complete();
                        }
                    }
                    // Devolver respuesta exitosa
                    return Json(new
                    {
                        Mensaje = "Acción completada exitosamente",
                        Comentario = new List<string> { "Se registró el requerimiento correctamente." },
                        Icono = "success"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Mensaje = "Error en la operación",
                        Comentario = new List<string> { "Envie un documento de requerimiento válido" },
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
        public JsonResult AtenderRequerimiento(string itemCode, string itemName, int cantidadGlobal, int requerimientoId)
        {
            try
            {
                if (requerimientoId>0 && itemCode != null && itemName != null && cantidadGlobal > 0)
                {
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    if (user == null)
                    {
                        return Json(new
                        {
                            Mensaje = "Error en la operación",
                            Comentario = new List<string> { "No existe usuario logueado, se terminó la sesión." },
                            Icono = "error"
                        });
                    }
                    // Asignar datos de operario en el requerimiento
                    var operarioRegistra = $"{user.Nombres} {user.Apellidos}";

                    Utilitarios uti = new Utilitarios();

                    // Iniciar la transacción global para las operaciones críticas
                    using (var scope = new TransactionScope(TransactionScopeOption.Required,
                       new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                       TransactionScopeAsyncFlowOption.Enabled))
                    {
                        using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                        {
                            cn.Open();

                            // Registrar la(s) operación(es) de salida(s) en KardexAbastecimiento - Los datos a insertar son los del detalle en requerimiento, RequerimientoGet ya tiene los datos limpios por enviar hacia el kardex y la resta de stocks
                            //cantidad global es la suma de todas las cantidades del itemcode
                            var resultKardex = _kardexAbastecimientoN.InsertarTransaccionSalidaKardex(itemCode,itemName, cantidadGlobal, operarioRegistra, requerimientoId, cn);
                            if (resultKardex.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { resultKardex.Mensaje },
                                    Icono = resultKardex.IconoSweetAlert
                                });
                            }

                            var lista = _requerimientosN
                               .ObtenerRequerimiento(requerimientoId)
                               .Detalle
                               .Where(x => x.ItemCode == itemCode)
                               .ToList();

                            // Restar y/o registrar Quantity en Cajas en la tabla UbicacionesLotes
                            var resultUbicacionesLotes = _ubicacionesLotesN.Salida(lista, cn);
                            if (resultUbicacionesLotes.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { resultUbicacionesLotes.Mensaje },
                                    Icono = resultUbicacionesLotes.IconoSweetAlert
                                });
                            }

                            //Restar y/o Registrar en la tabla UbicacionesLotesMaster
                            var resultUbicacionesLotesMaster = _ubicacionesLotesMasterN.Salida(lista, cn);
                            if (resultUbicacionesLotesMaster.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Mensaje = "No se pudo completar la acción",
                                    Comentario = new List<string> { resultUbicacionesLotesMaster.Mensaje },
                                    Icono = resultUbicacionesLotesMaster.IconoSweetAlert
                                });
                            }
                            // Confirmar la transacción
                            scope.Complete();
                        }
                    }
                    // Devolver respuesta exitosa
                    return Json(new
                    {
                        Mensaje = "Acción completada exitosamente",
                        Comentario = new List<string> { "Se atendió el SKU correctamente." },
                        Icono = "success"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Mensaje = "Error en la operación",
                        Comentario = new List<string> { "Los datos enviados son inválidos." },
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
        public JsonResult CalcularCantidadSolicitada(string tipoAbastecimiento, string itemCode)
        {
            int cantidadSolicitada = 0;
            if (tipoAbastecimiento != null && tipoAbastecimiento.Equals("Picking") && itemCode!=null)
            {
                itemCode = "PORT0078";
                //Calcular desde SAP (Stock Total - Comprometido)  en Almacen 16 por defecto
                int busqProducto = Convert.ToInt32(new Capa_Negocio.Almacen_NEG.Tablas.OITW_N().ListarDetArticulosInv(new OITW_E { ItemCode = itemCode ,WhsCode="16"}).DefaultIfEmpty(new OITW_E { }).First().StockLibre);
                int stockMinimoAbastec = _stockMinProdN.Obtener(itemCode).StockMinAbastecimiento;
                int stockInternoPorSku = _ubicacionesLotesN.Obtener(itemCode).Sum(u => u.QuantityUnidadesCajas);
                cantidadSolicitada = busqProducto - stockInternoPorSku- stockMinimoAbastec;
            }
            else
            {
                return Json(new
                {
                    Mensaje = "Error en la operación",
                    Comentario = new List<string> { "Los datos enviados son inválidos." },
                    Icono = "error"
                });
            }
            return Json(new
            {
                Mensaje = "Acción completada exitosamente",
                Comentario = new List<string> { Convert.ToString(cantidadSolicitada) },
                Icono = "success"
            });
        }
    }
}