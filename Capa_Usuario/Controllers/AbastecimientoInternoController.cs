using Capa_Datos;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasExternas;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasSql;
using Capa_Usuario.Helpers;
using DocumentFormat.OpenXml.EMMA;
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
        private readonly Ubicaciones_N _ubicacionesN = new Ubicaciones_N();
        private readonly Productos_N _productosN = new Productos_N();
        private readonly OWTQ_N _solicitudTrasladoHanaN = new OWTQ_N();
        private readonly StockMinProductos_N _stockMinProdN = new StockMinProductos_N();
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
        public ActionResult ListarUbicacionesPicking(Ubicaciones_E filtros)
        {
            //Por defecto Picking
            filtros.Almacen = "PICKING";

            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new
                {
                    Titulo = "No se pudo completar la acción",
                    Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }
                ,
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);

            var listaAgrupada = _ubicacionesN.ListarUbicaciones(filtros)
                .GroupBy(u => new { u.ItemCode, u.ItemName, u.StockMinAbastecimiento, u.StockMinVenta })
                .Select(grupo => new Ubicaciones_E
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
        public JsonResult RegistrarUbicacionPicking(Ubicaciones_E form)
        {
            //Por defecto Picking
            form.Almacen = "PICKING";
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new
                {
                    Titulo = "No se pudo completar la acción",
                    Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" },
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);

            form.NombreOperarioAccion = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";
            var result = _ubicacionesN.RegistrarUbicacion(form);
            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            return Json(new
            {
                Titulo = tituloSweetAlert,
                result.Mensajes,
                Icono = result.IconoSweetAlert
            });
        }
        public JsonResult EliminarUbicacionPicking(int id)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new
                {
                    Titulo = "No se pudo completar la acción",
                    Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" },
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);

            var result = _ubicacionesN.EliminarUbicacion(id);
            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            return Json(new
            {
                Titulo = tituloSweetAlert,
                result.Mensajes,
                Icono = result.IconoSweetAlert
            });
        }
        public JsonResult ActualizarStocksMinimos(StockMinProductos_E form)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);
            form.NombreOperarioAccion = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";
            var result = _stockMinProdN.ActualizarStocksMinimos(form);
            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            return Json(new
            {
                Titulo = tituloSweetAlert,
                result.Mensajes,
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
        public ActionResult ListarUbicacionesReserva(Ubicaciones_E filtros)
        {
            filtros.Almacen = "RESERVA";
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new
                {
                    Titulo = "No se pudo completar la acción",
                    Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" },
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);
            var listaAgrupada = _ubicacionesN.ListarUbicaciones(filtros).GroupBy(u => new { u.ItemCode, u.ItemName })
                .Select(grupo => new Ubicaciones_E
                {
                    ItemCode = grupo.Key.ItemCode,
                    ItemName = grupo.Key.ItemName,
                    CantidadUbicaciones = grupo.Count(),
                    Ubicaciones = grupo.Select(u => (Ubicaciones_E)u).ToList()
                })
                .ToDictionary(x => x.ItemCode);

            return PartialView("AbastecimientoInterno/_ListadoUbicacionesReserva", listaAgrupada);
        }
        public JsonResult RegistrarUbicacionReserva(Ubicaciones_E form)
        {
            //Por defecto Reserva
            form.Almacen = "RESERVA";
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new
                {
                    Titulo = "No se pudo completar la acción",
                    Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" },
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);
            form.NombreOperarioAccion = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";
            var result = _ubicacionesN.RegistrarUbicacion(form);
            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            return Json(new
            {
                Titulo = tituloSweetAlert,
                result.Mensajes,
                Icono = result.IconoSweetAlert
            });
        }
        public JsonResult EliminarUbicacionReserva(int id)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new
                {
                    Titulo = "No se pudo completar la acción",
                    Mensajes = "Inicia sesión nuevamente para continuar",
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);
            var result = _ubicacionesN.EliminarUbicacion(id);
            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            return Json(new
            {
                Titulo = tituloSweetAlert,
                result.Mensajes,
                Icono = result.IconoSweetAlert
            });
        }
        public JsonResult EliminarUbicacionGeneral(string codigoUbicacion)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Comentario = "Inicia sesión nuevamente para continuar", Icono = "error" }, JsonRequestBehavior.AllowGet);
            var result = _ubicacionesN.EliminarUbicacionGeneral(codigoUbicacion);
            string tituloSweetAlert = result.IconoSweetAlert.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            return Json(new
            {
                Titulo = tituloSweetAlert,
                result.Mensajes,
                Icono = result.IconoSweetAlert
            });
        }
        /************************* S O L I C I T U D   D E   T R A S L A D O *************************/
        public JsonResult BuscarSolicitudDeTraslado(int docNum)
        {
            SolicitudesTraslado_E traslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(docNum)
                        ?? _solicitudTrasladoHanaN.BuscarSolicitudDeTraslado(docNum);

            if (traslado == null)
            {
                var tituloSweetAlert = "No se pudo completar la acción";
                var icono = "error";
                var mensaje = "No existe ningun resultado";
                return Json(new { Titulo = tituloSweetAlert, Mensajes = new List<string> { mensaje }, Icono = icono });
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
                    return Json(new { Titulo = tituloSweetAlert, Mensajes = new List<string> { mensaje }, Icono = icono });
                }

                //Asignar la ubicacion ideal segun UbicacionesLotesMaster
                foreach (var item in traslado.Detalle)
                {
                    var resultados = _ubicacionesLotesMasterN.BuscarUnidadAlm(new UbicacionesLotesMaster_E { Almacen = "RESERVA", ItemCode = item.Value.ItemCode, BatchNum = item.Value.BatchNum });
                    if (resultados != null && resultados.Count == 1) { item.Value.UnidadAlmSugerido = resultados.First(); }
                }
            }
            return Json(new { traslado, transferencia });
        }
        public JsonResult BuscarUbicaciones(string almacen, string itemCode)
        {
            var resultUbicaciones = _ubicacionesN.BuscarUbicaciones(almacen, itemCode);
            var listaUbicacionesLote = _ubicacionesLotesN.Obtener(itemCode);
            string resultUbicacionesLote = null;

            if (listaUbicacionesLote != null && listaUbicacionesLote.Count == 1) { resultUbicacionesLote = listaUbicacionesLote.First().CodigoUbicacion; }

            return Json(new { resultUbicaciones, resultUbicacionesLote });
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
                    if (solicitudTraslado == null || solicitudTraslado.DocNum == 0)
                        solicitudTraslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(transferenciaPost.SolicitudTrasladoDocNum);

                    // Validar o inserta los lotes de registro sanitario (fuera de la transacción)
                    _lotesRegistroSanitarioN.ValidarLotesRegistroSanitario(solicitudTraslado.Detalle);

                    // Importa a las tablas internas solo si no existe previamente el DocNum
                    var traslado = (solicitudTraslado == null || solicitudTraslado.Id == 0) ? _solicitudTrasladoN.ImportarSolicitudDeTraslado(solicitudTraslado) : solicitudTraslado;

                    // Validar si la importación fue exitosa
                    if (traslado == null || traslado.Id == 0)
                    {
                        return Json(new
                        {
                            Titulo = "No se pudo completar la acción",
                            Mensajes = new List<string> { "No se importó la Solicitud de Traslado" },
                            Icono = "error"
                        });
                    }

                    // Obtener usuario de sesión (fuera de la transacción)
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    if (user == null)
                    {
                        return Json(new
                        {
                            Titulo = "Error en la operación",
                            Mensajes = new List<string> { "No existe usuario logueado, se terminó la sesión." },
                            Icono = "error"
                        });
                    }

                    // Asignar datos de traslado a la transferencia, preparando para registrar  agregar lineas a la transferencia
                    transferenciaPost.OperarioRegistra = $"{user.Nombres} {user.Apellidos}";
                    transferenciaPost.SolicitudTrasladoId = traslado.Id;
                    transferenciaPost.SolicitudTrasladoDocNum = traslado.DocNum;

                    Utilitarios uti = new Utilitarios();
                    // Iniciar la transacción global para las operaciones críticas
                    using (var scope = new TransactionScope(TransactionScopeOption.Required,
                       new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                       TransactionScopeAsyncFlowOption.Enabled))
                    {
                        using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                        {
                            cn.Open();
                            // Registrar  o agrega mas lineas al detalle de la transferencia de reserva
                            var resultTransferenciaGet = _transferenciaReservaN.RegistrarTransferenciaReserva(transferenciaPost, cn);
                            if (resultTransferenciaGet == null || resultTransferenciaGet.Id == 0)
                            {
                                if (resultTransferenciaGet.IconoSweetAlert.Equals("error"))
                                {
                                    // Validar y eliminar la solicitud de traslado si en caso se importo a la tabla interna pero no se ha encontrado una transferencia
                                    _solicitudTrasladoN.DeleteSolicitudDeTraslado(traslado.DocNum, cn);
                                    return Json(new
                                    {
                                        Titulo = "No se pudo completar la acción",
                                        resultTransferenciaGet.Mensajes,
                                        Icono = resultTransferenciaGet.IconoSweetAlert
                                    });

                                }
                            }
                            TransferenciaReserva_E transferencia = _transferenciaReservaN.ObtenerTransferenciaReserva(transferenciaPost.SolicitudTrasladoDocNum);
                            //Actualiza a TRANSFERIDO en el DetalleDeSolicitudTraslado los ItemCode(s) que se hayan enviado para TransferenciaReserva
                            var resultActualizarEstado = _solicitudTrasladoN.ActualizarEstado(transferencia.SolicitudTrasladoId, transferencia.Detalle, cn);
                            if (resultActualizarEstado.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Titulo = "No se pudo completar la acción",
                                    resultActualizarEstado.Mensajes,
                                    Icono = resultActualizarEstado.IconoSweetAlert
                                });
                            }

                            // Registrar la(s) operación(es) de ingreso(s) en KardexAbastecimiento - Los datos a insertar son los del detalle en transferencia
                            // TransferenciaGet ya tiene los datos limpios por enviar hacia el kardex y la suma de stocks
                            var resultKardex = _kardexAbastecimientoN.InsertarTransaccionIngresoKardex(transferencia, cn);
                            if (resultKardex.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Titulo = "No se pudo completar la acción",
                                    resultKardex.Mensajes,
                                    Icono = resultKardex.IconoSweetAlert
                                });
                            }

                            // Sumar y/o Registrar QuantityUnidadesCajas en la tabla UbicacionesLotes
                            var resultUbicacionesLotes = new Helper_E();

                            foreach (var item in transferencia.Detalle)
                            {
                                resultUbicacionesLotes = _ubicacionesLotesN.Ingreso(item, cn);
                                if (resultUbicacionesLotes.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Titulo = "No se pudo completar la acción",
                                        resultUbicacionesLotes.Mensajes,
                                        Icono = resultUbicacionesLotes.IconoSweetAlert
                                    });
                                }

                                var ubicacionLoteId = resultUbicacionesLotes.Id;
                                // Sumar y/o Registrar en la tabla UbicacionesLotesMaster
                                //Se envia el parametro de UbicacionesLoteId si en caso es una nueva UbicacionLote creada
                                var resultUbicacionesLotesMaster = _ubicacionesLotesMasterN.Ingreso(Convert.ToInt32(ubicacionLoteId), item, cn);
                                if (resultUbicacionesLotesMaster.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Titulo = "No se pudo completar la acción",
                                        resultUbicacionesLotesMaster.Mensajes,
                                        Icono = resultUbicacionesLotesMaster.IconoSweetAlert
                                    });
                                }
                            }

                            scope.Complete();
                        }
                    }
                    // Devolver respuesta exitosa
                    return Json(new
                    {
                        Titulo = "Acción completada exitosamente",
                        Mensajes = new List<string> { "Se registró la Transferencia Reserva correctamente." },
                        Icono = "success"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Titulo = "Error en la operación",
                        Mensajes = new List<string> { "El documento que trata de registrar no tiene una transferencia realizandose." },
                        Icono = "error"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { ex.Message },
                    Icono = "error"
                });
            }
        }
        public JsonResult CancelarTransferenciaYTraslado(int docNum) //recibe el docnum de la solicitud de traslado
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
                            Titulo = "No se pudo completar la acción",
                            Mensajes = new List<string> { "No se encontró transferencia de reserva relacionada." },
                            Icono = "error"
                        });
                    }
                    // El elemento de transferencia trae los datos a revertir en las transacciones.
                    //Se eliminara tambien el DetalleSolicitudTraslado asi como la SolicitudTraslado
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
                                    Titulo = "No se pudo completar la acción",
                                    resultUbicacionesLotesMaster.Mensajes,
                                    Icono = resultUbicacionesLotesMaster.IconoSweetAlert
                                });
                            }
                            // Restar y/o eliminar Quantity en Cajas en la tabla UbicacionesLotes
                            var resultUbicacionesLotes = _ubicacionesLotesN.RevertirIngreso(transferenciaGet, cn);
                            if (resultUbicacionesLotes.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Titulo = "No se pudo completar la acción",
                                    resultUbicacionesLotes.Mensajes,
                                    Icono = resultUbicacionesLotes.IconoSweetAlert
                                });
                            }
                            // Eliminar la(s) operación(es) de ingreso(s) en KardexAbastecimiento - Los datos a eliminar son los del detalle en transferencia
                            var resultKardex = _kardexAbastecimientoN.EliminarTotalTransaccionesIngresoKardex(docNum, cn);
                            if (resultKardex.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Titulo = "No se pudo completar la acción",
                                    resultKardex.Mensajes,
                                    Icono = resultKardex.IconoSweetAlert
                                });
                            }
                            // Eliminar Detalle y Cabecera de Transferencia de Reserva
                            var resultTransferencia = _transferenciaReservaN.DeleteTransferenciaReserva(docNum, cn);
                            if (resultTransferencia.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Titulo = "No se pudo completar la acción",
                                    resultTransferencia.Mensajes,
                                    Icono = resultTransferencia.IconoSweetAlert
                                });
                            }
                            //Eliminar Detalle y Cabecera de Solicitud de Traslado
                            var resultSolicitudTraslado = _solicitudTrasladoN.DeleteSolicitudDeTraslado(docNum, cn);
                            if (resultSolicitudTraslado.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Titulo = "No se pudo completar la acción",
                                    resultTransferencia.Mensajes,
                                    Icono = resultSolicitudTraslado.IconoSweetAlert
                                });
                            }

                            scope.Complete();
                        }
                    }
                    return Json(new
                    {
                        Titulo = "Acción completada exitosamente",
                        Mensajes = new List<string> { "Se canceló la Transferencia Reserva y Solicitud de Traslado correctamente." },
                        Icono = "success"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Titulo = "Error en la operación",
                        Mensajes = new List<string> { "El docNum es invalido." },
                        Icono = "error"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { ex.Message },
                    Icono = "error"
                });
            }

        }
        public JsonResult RevertirTransferenciaReservaPorItem(int docNum, string itemCode) //recibe el docnum de la solicitud de traslado y el ItemCode a revertir
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
                            Titulo = "No se pudo completar la acción",
                            Mensajes = new List<string> { "No se encontró transferencia de reserva relacionada." },
                            Icono = "error"
                        });
                    }

                    // Identificar solo el itemcode, reduciendo mi Detalle
                    if (transferenciaGet.Detalle != null)
                    {
                        transferenciaGet.Detalle = transferenciaGet.Detalle.Where(x => x.ItemCode == itemCode).ToList();
                    }

                    // Validar que los ids en cuanto a la suma de QuantityUnidadesCajas es igual a Quantity de DetSolicitudDeTraslado respecto a ese ItemCode
                    if (traslado.Detalle != null && transferenciaGet.Detalle != null)
                    {
                        traslado.Detalle = traslado.Detalle
                        .Where(kv => kv.Value.ItemCode == itemCode)
                        .ToDictionary(kv => kv.Key, kv => kv.Value);

                        // Toma el primer elemento del diccionario
                        var primerDetalle = traslado.Detalle.First().Value;

                        //Si las cantidades no coinciden quiere decir que no se ha pasado el grupo completo de los ids correspondientes a un ItemCode en la solicitud de traslado, muestra error
                        if (primerDetalle.QuantityCajas != transferenciaGet.Detalle.Where(x => x.ItemCode == itemCode).Sum(x => x.QuantityUnidadesCajas))
                        {
                            return Json(new
                            {
                                Mensaje = "No se pudo completar la acción",
                                Comentario = new List<string> { $"La suma de cantidades a revertir no coincide con el total en la solicitud de traslado para el SKU:{transferenciaGet.Detalle[0].ItemCode} " },
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
                                        Titulo = "No se pudo completar la acción",
                                        resultUbicacionesLotesMaster.Mensajes,
                                        Icono = resultUbicacionesLotesMaster.IconoSweetAlert
                                    });
                                }

                                // Restar y/o eliminar Quantity en Cajas en la tabla UbicacionesLotes
                                var resultUbicacionesLotes = _ubicacionesLotesN.RevertirIngreso(transferenciaGet, cn);
                                if (resultUbicacionesLotes.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Titulo = "No se pudo completar la acción",
                                        resultUbicacionesLotes.Mensajes,
                                        Icono = resultUbicacionesLotes.IconoSweetAlert
                                    });
                                }

                                // Eliminar la operación de ingreso en KardexAbastecimiento que pertenece a dicho ItemCode - Los datos a eliminar son los del detalle en transferencia
                                var resultKardex = _kardexAbastecimientoN.EliminarPorItemCodeTransaccionIngresoKardex(docNum, transferenciaGet.Detalle[0].ItemCode, cn);
                                if (resultKardex.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Titulo = "No se pudo completar la acción",
                                        resultKardex.Mensajes,
                                        Icono = resultKardex.IconoSweetAlert
                                    });
                                }

                                //Eliminar los items de Detalle de Transferencia de Reserva 'REVERT' que sean del ItemCode
                                var resultTransferencia = _transferenciaReservaN.DeleteDetalleItemTransferenciaReserva(transferenciaGet.Detalle, primerDetalle, cn);
                                if (resultTransferencia.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Titulo = "No se pudo completar la acción",
                                        resultTransferencia.Mensajes,
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
                            using (var scope = new TransactionScope(TransactionScopeOption.Required,
                                 new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                                 TransactionScopeAsyncFlowOption.Enabled))
                            {
                                Utilitarios uti = new Utilitarios();
                                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                                {
                                    cn.Open();

                                    //Eliminar la transferencia 
                                    var resultEliminarTransferencia = _transferenciaReservaN.DeleteTransferenciaReserva(docNum, cn);
                                    if (resultEliminarTransferencia.IconoSweetAlert.Equals("error"))
                                    {
                                        return Json(new
                                        {
                                            Titulo = "No se pudo completar la acción",
                                            resultEliminarTransferencia.Mensajes,
                                            Icono = resultEliminarTransferencia.IconoSweetAlert
                                        });
                                    }

                                    //Luego Eliminar Detalle y Cabecera de Solicitud de Traslado solo si la transferencia se quedo sin elementos en su detalle, genera una nueva connection
                                    var resultSolicitudTraslado = _solicitudTrasladoN.DeleteSolicitudDeTraslado(docNum, cn);
                                    if (resultSolicitudTraslado.IconoSweetAlert.Equals("error"))
                                    {
                                        return Json(new
                                        {
                                            Titulo = "No se pudo completar la acción",
                                            resultSolicitudTraslado.Mensajes,
                                            Icono = resultSolicitudTraslado.IconoSweetAlert
                                        });
                                    }

                                    scope.Complete();
                                }
                            }
                        }   

                    }

                    return Json(new
                    {
                        Titulo = "Acción completada exitosamente",
                        Mensajes = new List<string> { "Se canceló la Transferencia Reserva y Solicitud de Traslado correctamente." },
                        Icono = "success"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Titulo = "Error en la operación",
                        Mensajes = new List<string> { "El docNum es invalido." },
                        Icono = "error"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { ex.Message },
                    Icono = "error"
                });
            }

        }
        /****************************** R E Q U E R I M I E N T O S ****************************/
        public ActionResult Requerimientos(int idOperation = 3300)
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
        [HttpGet]
        public ActionResult ListarArticulos(string tipoAbastecimiento, string itemCode, int cantidadSolicitada)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            List<UbicacionesLotesMaster_E> lista = _ubicacionesLotesMasterN.BuscarArticulos(new UbicacionesLotesMaster_E { ItemCode = itemCode })
                .OrderBy(a => DateTime.Parse(a.InDate)) // Ordena por fecha de vencimiento (asc)
                .ThenBy(a => DateTime.Parse(a.ExpDate)) // Luego por fecha de admisión (asc)
                .ThenByDescending(a => a.QuantityUnidadesCajas) // Luego por cantidad en unidades (desc)
                .ToList();

            switch (tipoAbastecimiento)
            {
                case "Picking":
                    return PartialView("AbastecimientoInterno/_ListadoArticulosPicking", lista);

                case "Venta":
                    return PartialView("AbastecimientoInterno/_ListadoArticulosVenta", lista);

                default:
                    return HttpNotFound("No se encontró la vista para el tipo de abastecimiento especificado.");
            }
        }
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
                            Titulo = "Error en la operación",
                            Mensajes = new List<string> { "No existe usuario logueado, se terminó la sesión." },
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
                                    Titulo = "No se pudo completar la acción",
                                    Mensajes = new List<string> { "No se completo el registro del requerimiento" },
                                    Icono = "error"
                                });
                            }

                            // Registrar la(s) operación(es) de imputado(s) en KardexAbastecimiento - Los datos a insertar son los del detalle en requerimiento, RequerimientoGet ya tiene los datos limpios por enviar hacia el kardex como imputado, previamente validados
                            var resultKardexImputar = _kardexAbastecimientoN.InsertarTransaccionImputadoKardex(requerimientoGet, cn);
                            if (resultKardexImputar.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Titulo = "No se pudo completar la acción",
                                    resultKardexImputar.Mensajes,
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
                        Titulo = "Acción completada exitosamente",
                        Mensajes = new List<string> { "Se registró el requerimiento correctamente." },
                        Icono = "success"
                    });
                }
                else
                {
                    return Json(new
                    {
                        Titulo = "Error en la operación",
                        Mensajes = new List<string> { "Envie un documento de requerimiento válido" },
                        Icono = "error"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { ex.Message },
                    Icono = "error"
                });
            }
        }
        //Listado de pendientes para apiladores con 4 posibles filtros
        public ActionResult ListarRequerimientosReserva(string nivel = "", string posicion = "", string rackBloque = "", string itemCode = "", int idOperation = 3300)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _requerimientosN.ListarDetalles("", "ListarApiladores")
                .Where(x =>
                    (string.IsNullOrEmpty(nivel) || x.Nivel == nivel) &&
                    (string.IsNullOrEmpty(posicion) || x.Posicion == posicion) &&
                    (string.IsNullOrEmpty(rackBloque) || x.RackBloque == rackBloque) &&
                    (string.IsNullOrEmpty(itemCode) || x.ItemCode == itemCode)
                ).ToList();
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //Listado de pendientes para picking con 1 posible filtros
        public ActionResult ListarRequerimientosPicking(string itemCode = "", int idOperation = 3300)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _requerimientosN.ListarDetalles("", "ListarPicking")
                .Where(x =>
                    (string.IsNullOrEmpty(itemCode) || x.ItemCode == itemCode)
                ).ToList();
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //Atendido de apiladores (Solo cambia el AtendidoReserva a 1)
        public ActionResult ListarProductosConUrgencia(string itemCode = "", int idOperation = 3300)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _requerimientosN.ListarDetalles("", "ListarPicking")
                .Where(x =>
                    (string.IsNullOrEmpty(itemCode) || x.ItemCode == itemCode)
                ).ToList();
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //Atendido de apiladores (Solo cambia el AtendidoReserva a 1)
        public JsonResult AtenderReservaRequerimiento(int id)
        {
            try
            {
                //Actualizar a AtendidoReserva 1 solo la linea de detalle enviada
                var resultAtender = _requerimientosN.AtenderReserva(id);

                return Json(new
                {
                    Titulo = "Acción completada exitosamente",
                    resultAtender.Mensajes,
                    Icono = resultAtender.IconoSweetAlert
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { ex.Message },
                    Icono = "error"
                });
            }
        }
        //Atendido de Picking (Cambia el AtendidoPicking a 1 y valida si todo se completo respecto al sku del requerimiento para generar una salida en el Kardex
        public JsonResult AtenderPickingRequerimiento(DetalleRequerimientos_E detalle)
        {
            try
            {
                if (detalle != null)
                {
                    //Actualizar a AtendidoPicking 1 solo la linea de detalle enviada
                    var resultAtender = _requerimientosN.AtenderPicking(detalle.Id);

                    if (resultAtender != null && resultAtender.IconoSweetAlert.Equals("success"))
                    {
                        //Valida que todos los items del requerimiento con el mismo Sku esten en AtendidoPicking 1, de ser asi se genera kardex de salida
                        bool listoKardexSalida = false;
                        listoKardexSalida = _requerimientosN.ValidarSkuParaKardexSalida(detalle);

                        if (listoKardexSalida)
                        {
                            Usuario_E user = (Usuario_E)Session["UsuarioId"];
                            if (user == null)
                            {
                                return Json(new
                                {
                                    Titulo = "Error en la operación",
                                    Mensajes = new List<string> { "No existe usuario logueado, se terminó la sesión." },
                                    Icono = "error"
                                });
                            }
                            // Asignar datos de operario para el kardex de salida 
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

                                    // Registrar la operacion de salida en KardexAbastecimiento 
                                    // Busca el requerimiento y calcula cantidad global es la suma de todas las cantidades del itemCode
                                    var requerimientoGet = _requerimientosN.ObtenerRequerimiento(detalle.RequerimientoId);
                                    var requerimientoPorSku = requerimientoGet.Detalle.Where(x => x.ItemCode == detalle.ItemCode).ToList();
                                    int cantidadGlobal = Convert.ToInt32(requerimientoPorSku.Sum(x => x.QuantityUnidadesCajas));

                                    var resultKardex = _kardexAbastecimientoN.InsertarTransaccionSalidaKardex(detalle.ItemCode, detalle.ItemName, cantidadGlobal, operarioRegistra, detalle.RequerimientoId, cn);

                                    if (resultKardex.IconoSweetAlert.Equals("error"))
                                    {
                                        return Json(new
                                        {
                                            Titulo = "No se pudo completar la acción",
                                            resultKardex.Mensajes,
                                            Icono = resultKardex.IconoSweetAlert
                                        });
                                    }

                                    // Restar y/o registrar Quantity en Cajas en la tabla UbicacionesLotes
                                    var resultUbicacionesLotes = _ubicacionesLotesN.Salida(requerimientoPorSku, cn);
                                    if (resultUbicacionesLotes.IconoSweetAlert.Equals("error"))
                                    {
                                        return Json(new
                                        {
                                            Titulo = "No se pudo completar la acción",
                                            resultUbicacionesLotes.Mensajes,
                                            Icono = resultUbicacionesLotes.IconoSweetAlert
                                        });
                                    }

                                    //Restar y/o Registrar en la tabla UbicacionesLotesMaster
                                    var resultUbicacionesLotesMaster = _ubicacionesLotesMasterN.Salida(requerimientoPorSku, cn);
                                    if (resultUbicacionesLotesMaster.IconoSweetAlert.Equals("error"))
                                    {
                                        return Json(new
                                        {
                                            Titulo = "No se pudo completar la acción",
                                            resultUbicacionesLotesMaster.Mensajes,
                                            Icono = resultUbicacionesLotesMaster.IconoSweetAlert
                                        });
                                    }

                                    scope.Complete();
                                }
                            }

                            // Devolver respuesta exitosa
                            return Json(new
                            {
                                Titulo = "Acción completada exitosamente",
                                Mensajes = new List<string> { "Se atendió el SKU correctamente." },
                                Icono = "success"
                            });
                        }

                        // Devolver respuesta exitosa
                        return Json(new
                        {
                            Titulo = "Acción completada exitosamente",
                            Mensajes = new List<string> { "Se atendió el SKU correctamente." },
                            Icono = "success"
                        });
                    }

                    return Json(new
                    {
                        Titulo = "Error en la operación",
                        resultAtender.Mensajes,
                        Icono = resultAtender.IconoSweetAlert
                    });
                }
                else
                {
                    return Json(new
                    {
                        Titulo = "Error en la operación",
                        Mensajes = new List<string> { "Los datos enviados son inválidos." },
                        Icono = "error"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { ex.Message },
                    Icono = "error"
                });
            }
        }
        public JsonResult CalcularCantidadSolicitada(string tipoAbastecimiento, string itemCode)
        {
            int cantidadSolicitada = 0;
            if (tipoAbastecimiento != null && tipoAbastecimiento.Equals("Picking") && itemCode != null)
            {
                //Calcular desde SAP (Stock Total - Stock Comprometido)  en Almacen 16 por defecto
                int stockLibreEnAlmacen16 = Convert.ToInt32(new Capa_Negocio.Almacen_NEG.Tablas.OITW_N().ListarDetArticulosInv(new OITW_E { ItemCode = itemCode, WhsCode = "16" }).DefaultIfEmpty(new OITW_E { }).First().StockLibre);

                int stockDeAlmReserva = _ubicacionesLotesN.Obtener(itemCode).Sum(u => u.QuantityUnidadesCajas) -
                    Convert.ToInt32(_requerimientosN.ListarDetalles(itemCode, "CantidadSolicitada").Sum(r => r.QuantityUnidadesCajas)); //resta de lo que esta por entrar a Picking Atendido=0

                int stockEnPicking = stockLibreEnAlmacen16 - stockDeAlmReserva;

                int stockMinimoParaLaVenta = _stockMinProdN.Obtener(itemCode).StockMinVenta;

                cantidadSolicitada = stockMinimoParaLaVenta - stockEnPicking;

                if (cantidadSolicitada < 0) { cantidadSolicitada = 0; }
            }
            else
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { "Los datos enviados son inválidos." },
                    Icono = "error"
                });
            }
            return Json(new
            {
                Titulo = "Acción completada exitosamente",
                Mensajes = new List<string> { Convert.ToString(cantidadSolicitada) },
                Icono = "success"
            });
        }
    }
}