using Aspose.Pdf.Operators;
using Capa_Datos;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.Reportes;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.General_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.AbastecimientoInterno_NEG.Reportes;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasExternas;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasSql;
using Capa_Negocio.General_NEG.TablasSql;
using Capa_Usuario.Helpers;
using DocumentFormat.OpenXml.EMMA;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Transactions;
using System.util;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
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
        private readonly ReporteStockPicking_N _reporteStockPicking = new ReporteStockPicking_N();
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
        public ActionResult ListarUbicacionesPicking(Ubicaciones_E filtros, int idOperation = 3100)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
                    .GroupBy(u => new { u.ItemCode, u.ItemName, u.StockMinAbastecimiento, u.StockMinVenta, u.Clasificacion })
                    .Select(grupo => new Ubicaciones_E
                    {
                        ItemCode = grupo.Key.ItemCode,
                        ItemName = grupo.Key.ItemName,
                        CantidadUbicaciones = grupo.Count(),
                        Ubicaciones = grupo.Select(u => (Ubicaciones_E)u).ToList(),
                        StockMinAbastecimiento = grupo.Key.StockMinAbastecimiento,
                        StockMinVenta = grupo.Key.StockMinVenta,
                        Clasificacion = grupo.Key.Clasificacion
                    })
                    .ToDictionary(x => x.ItemCode);
                return PartialView("AbastecimientoInterno/_ListadoUbicacionesPicking", listaAgrupada);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public JsonResult RegistrarUbicacionPicking(Ubicaciones_E form, int idOperation = 3101)
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
        public JsonResult EliminarUbicacionPicking(int id, int idOperation = 3102)
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
        public JsonResult ActualizarStocksMinimos(StockMinProductos_E form, int idOperation = 3103)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var usuarioSesion = Session["UsuarioId"] as Usuario_E;
                if (usuarioSesion == null)
                    return Json(new
                    {
                        Titulo = "No se pudo completar la acción",
                        Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" },
                        Icono = "error"
                    }, JsonRequestBehavior.AllowGet);

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
            else
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { "Sin accesos." },
                    Icono = "error"
                });
            }
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
            Utilitarios uti = new Utilitarios();
            SolicitudesTraslado_E traslado = null;
            TransferenciaReserva_E transferencia = null;
            traslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(docNum, null)
                       ?? _solicitudTrasladoHanaN.BuscarSolicitudDeTraslado(docNum);
            // Iniciar la transacción global para las operaciones críticas
            using (var scope = new TransactionScope(TransactionScopeOption.Required,
               new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
               TransactionScopeAsyncFlowOption.Enabled))
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();
                    

                    if (traslado == null)
                    {
                        var tituloSweetAlert = "No se pudo completar la acción";
                        var icono = "error";
                        var mensaje = "No existe ningun resultado";
                        return Json(new { Titulo = tituloSweetAlert, Mensajes = new List<string> { mensaje }, Icono = icono });
                    }

                    if (traslado?.Id > 0)
                    {
                        if (traslado.Detalle != null && traslado.Detalle.All(item => item.Value.Estado == "TRANSFERIDO"))
                        {
                            return Json(new
                            {
                                Titulo = "Error en la operación",
                                Mensajes = new List<string> { "La solicitud de traslado ya ha sido TRANSFERIDA en su totalidad al sistema." },
                                Icono = "warning"
                            });
                        }

                        // Código cuando traslado.Id > 0 quiere decir que vino la informacion de tabla interna, buscar lo insertado en comparacion con transferencia
                        transferencia = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum, cn);
                        if (transferencia == null)
                        {
                            var tituloSweetAlert = "No se pudo completar la acción";
                            var icono = "error";
                            var mensaje = "No existe ningun resultado de transferencia relacionada a la solicitud de traslado que ya esta registrada.";
                            return Json(new
                            {
                                Titulo = tituloSweetAlert,
                                Mensajes = new List<string> { mensaje },
                                Icono = icono
                            });
                        }

                        //Asignar la ubicacion ideal segun UbicacionesLotesMaster
                        foreach (var item in traslado.Detalle)
                        {
                            var resultados = _ubicacionesLotesMasterN.BuscarUnidadAlm(cn, new UbicacionesLotesMaster_E { Almacen = "RESERVA", ItemCode = item.Value.ItemCode, BatchNum = item.Value.BatchNum });
                            if (resultados != null && resultados.Count == 1) { item.Value.UnidadAlmSugerido = resultados.First(); }
                        }
                    }
                    scope.Complete();
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
        public JsonResult ImportarTransferenciaDeStock(HttpPostedFileBase file, int idOperation = 3301)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
            {
                return Json(new
                {
                    Titulo = "No se pudo completar la acción",
                    Mensajes = new List<string> { "Error de accesos." },
                    Icono = "error"
                });
            }

            if (file == null || file.ContentLength == 0)
            {
                return Json(new
                {
                    Titulo = "No se pudo completar la acción",
                    Mensajes = new List<string> { "No se ha seleccionado un archivo válido." },
                    Icono = "error"
                });
            }

            try
            {
                using (var stream = file.InputStream)
                {
                    SLDocument sld = new SLDocument(stream);

                    if (!sld.GetSheetNames().Contains("CABECERA"))
                    {
                        return Json(new
                        {
                            Titulo = "No se pudo completar la acción",
                            Mensajes = new List<string> { "La hoja 'CABECERA' no existe en el archivo." },
                            Icono = "error"
                        });
                    }
                    sld.SelectWorksheet("CABECERA");

                    int iRow = 10;
                    if (string.IsNullOrWhiteSpace(sld.GetCellValueAsString(iRow, 1)))
                    {
                        return Json(new
                        {
                            Titulo = "No se pudo completar la acción",
                            Mensajes = new List<string> { "No hay datos en el archivo." },
                            Icono = "error"
                        });
                    }

                    SolicitudesTraslado_E solicitudTraslado = new SolicitudesTraslado_E
                    {
                        DocEntry = sld.GetCellValueAsInt32(iRow, 1),
                        DocNum = sld.GetCellValueAsInt32(iRow, 2),
                        DocDate = sld.GetCellValueAsDateTime(iRow, 3).ToString("yyyy-MM-dd"),
                        CardCode = sld.GetCellValueAsString(iRow, 4),
                        CardName = sld.GetCellValueAsString(iRow, 5),
                        NroGuia = string.IsNullOrWhiteSpace(sld.GetCellValueAsString(iRow, 6)) ? null : sld.GetCellValueAsString(iRow, 6),
                        OperarioResponsableSAP = sld.GetCellValueAsString(iRow, 7),
                        MotivoTraslado = sld.GetCellValueAsString(iRow, 8),
                        Estado = "TRANSFERIDO",
                        Detalle = new Dictionary<string, DetalleSolicitudesTraslado_E>()
                    };

                    TransferenciaReserva_E transferencia = new TransferenciaReserva_E
                    {
                        CardCode = sld.GetCellValueAsString(iRow, 4),
                        CardName = sld.GetCellValueAsString(iRow, 5),
                        NroGuia = string.IsNullOrWhiteSpace(sld.GetCellValueAsString(iRow, 6)) ? null : sld.GetCellValueAsString(iRow, 6),
                        Detalle = new List<DetalleTransferenciaReserva_E>()
                    };

                    if (!sld.GetSheetNames().Contains("CUERPO"))
                    {
                        return Json(new
                        {
                            Titulo = "No se pudo completar la acción",
                            Mensajes = new List<string> { "La hoja 'CUERPO' no existe en el archivo." },
                            Icono = "error"
                        });
                    }
                    sld.SelectWorksheet("CUERPO");

                    iRow = 10;
                    while (!string.IsNullOrWhiteSpace(sld.GetCellValueAsString(iRow, 1)))
                    {
                        var detalleSolicitudTraslado = new DetalleSolicitudesTraslado_E
                        {
                            ItemCode = sld.GetCellValueAsString(iRow, 2),
                            ItemName = sld.GetCellValueAsString(iRow, 3),
                            BatchNum = sld.GetCellValueAsString(iRow, 4),
                            FromWhsCode = sld.GetCellValueAsString(iRow, 5),
                            ToWhsCode = sld.GetCellValueAsString(iRow, 6),
                            ExpDate = sld.GetCellValueAsDateTime(iRow, 8).ToString("yyyy-MM-dd"),
                            InDate = sld.GetCellValueAsDateTime(iRow, 9).ToString("yyyy-MM-dd"),
                            QuantityCajas = Convert.ToDecimal((sld.GetCellValueAsInt32(iRow, 11) * sld.GetCellValueAsInt32(iRow, 12)) + sld.GetCellValueAsInt32(iRow, 13))
                        };

                        var detalleTransferencia = new DetalleTransferenciaReserva_E
                        {
                            ItemCode = sld.GetCellValueAsString(iRow, 2),
                            ItemName = sld.GetCellValueAsString(iRow, 3),
                            BatchNum = sld.GetCellValueAsString(iRow, 4),
                            CodigoUbicacion = sld.GetCellValueAsString(iRow, 7),
                            ExpDate = sld.GetCellValueAsDateTime(iRow, 8).ToString("yyyy-MM-dd"),
                            InDate = sld.GetCellValueAsDateTime(iRow, 9).ToString("yyyy-MM-dd"),
                            UmAlm = sld.GetCellValueAsString(iRow, 10),
                            ValorUmAlm = sld.GetCellValueAsInt32(iRow, 11),
                            QuantityMaster = sld.GetCellValueAsInt32(iRow, 12),
                            QuantitySaldo = sld.GetCellValueAsInt32(iRow, 13),
                            QuantityUnidadesCajas = (sld.GetCellValueAsInt32(iRow, 11) * sld.GetCellValueAsInt32(iRow, 12)) + sld.GetCellValueAsInt32(iRow, 13)
                        };

                        string uniqueKey = detalleSolicitudTraslado.ItemCode;
                        if (!solicitudTraslado.Detalle.ContainsKey(uniqueKey))
                        {
                            solicitudTraslado.Detalle[uniqueKey] = detalleSolicitudTraslado;
                        }

                        solicitudTraslado.Detalle[detalleSolicitudTraslado.ItemCode] = detalleSolicitudTraslado;
                        transferencia.Detalle.Add(detalleTransferencia);
                        iRow++;
                    }

                    var resultado = RegistrarTransferenciaDeStock(solicitudTraslado, transferencia);

                    // Verificar si el resultado es un JsonResult válido
                    if (resultado is JsonResult jsonResultado)
                    {
                        return jsonResultado;
                    }
                    else
                    {
                        return Json(new
                        {
                            Titulo = "Error en la transferencia",
                            Mensajes = new List<string> { "Respuesta inesperada al registrar la transferencia de stock." },
                            Icono = "error"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error en la importación",
                    Mensajes = new List<string> { "Ocurrió un error al procesar la transferencia de stock.", ex.Message },
                    Icono = "error"
                });
            }
        }
        //   public JsonResult RegistrarTransferenciaDeStock(SolicitudesTraslado_E solicitudTraslado, TransferenciaReserva_E transferenciaPost, int idOperation = 3302)
        //   {
        //       var resultadoAcceso = VerificarPermiso(idOperation);
        //       if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
        //       {
        //           return Json(new
        //           {
        //               Titulo = "No se pudo completar la acción",
        //               Mensajes = new List<string> { "Error de accesos." },
        //               Icono = "error"
        //           });
        //       }
        //       try
        //       {
        //           if (transferenciaPost == null)
        //           {
        //               return Json(new
        //               {
        //                   Titulo = "Error en la operación",
        //                   Mensajes = new List<string> { "El documento que trata de registrar no tiene una transferencia realizándose." },
        //                   Icono = "error"
        //               });
        //           }

        //           if (solicitudTraslado == null || solicitudTraslado.DocNum == 0)
        //               solicitudTraslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(transferenciaPost.SolicitudTrasladoDocNum);

        //           var resultLotes = _lotesRegistroSanitarioN.ValidarLotesRegistroSanitario(solicitudTraslado.Detalle);
        //           if (resultLotes.IconoSweetAlert.Equals("error"))
        //           {
        //               return Json(new { Titulo = "No se pudo completar la acción", resultLotes.Mensajes, Icono = resultLotes.IconoSweetAlert });
        //           }

        //           if (solicitudTraslado == null || solicitudTraslado.Id == 0)
        //           {
        //               var resultSolicitud = _solicitudTrasladoN.ImportarSolicitudDeTraslado(solicitudTraslado);
        //               if (resultSolicitud.IconoSweetAlert.Equals("error") || resultSolicitud.Id == 0)
        //               {
        //                   return Json(new { Titulo = "No se pudo completar la acción", resultSolicitud.Mensajes, Icono = "error" });
        //               }
        //               solicitudTraslado.Id = resultSolicitud.Id;
        //           }

        //           Usuario_E user = (Usuario_E)Session["UsuarioId"];
        //           if (user == null)
        //           {
        //               return Json(new
        //               {
        //                   Titulo = "Error en la operación",
        //                   Mensajes = new List<string> { "No existe usuario logueado, se terminó la sesión." },
        //                   Icono = "error"
        //               });
        //           }

        //           transferenciaPost.OperarioRegistra = $"{user.Nombres} {user.Apellidos}";
        //           transferenciaPost.SolicitudTrasladoId = solicitudTraslado.Id;
        //           transferenciaPost.SolicitudTrasladoDocNum = solicitudTraslado.DocNum;

        //           Utilitarios uti = new Utilitarios();

        //           using (var scope = new TransactionScope(TransactionScopeOption.Required,
        //              new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
        //              TransactionScopeAsyncFlowOption.Enabled))
        //           {
        //               using (SqlConnection cn = new SqlConnection(uti.cadSql2))
        //               {
        //                   cn.Open();

        //                   var resultRegistroTransferencia = _transferenciaReservaN.RegistrarTransferenciaReserva(transferenciaPost, cn);
        //                   if (resultRegistroTransferencia.IconoSweetAlert.Equals("error"))
        //                   {
        //                       throw new Exception(string.Join(", ", resultRegistroTransferencia.Mensajes));
        //                   }

        //                   TransferenciaReserva_E transferenciaActualizada = _transferenciaReservaN.ObtenerTransferenciaReserva(transferenciaPost.SolicitudTrasladoDocNum);
        //                   var resultActualizarEstado = _solicitudTrasladoN.ActualizarEstado(transferenciaActualizada.SolicitudTrasladoId, transferenciaActualizada.Detalle, cn);
        //                   if (resultActualizarEstado.IconoSweetAlert.Equals("error"))
        //                   {
        //                       throw new Exception(string.Join(", ", resultActualizarEstado.Mensajes));
        //                   }

        //                   foreach (var item in transferenciaActualizada.Detalle)
        //                   {
        //                       var resultUbicacionesLotes = _ubicacionesLotesN.Ingreso(item, cn);
        //                       if (resultUbicacionesLotes.IconoSweetAlert.Equals("error"))
        //                       {
        //                           throw new Exception(string.Join(", ", resultUbicacionesLotes.Mensajes));
        //                       }

        //                       int ubicacionLoteId = resultUbicacionesLotes.Id;
        //                       var resultUbicacionesLotesMaster = _ubicacionesLotesMasterN.Ingreso(ubicacionLoteId, item, cn);
        //                       if (resultUbicacionesLotesMaster.IconoSweetAlert.Equals("error"))
        //                       {
        //                           throw new Exception(string.Join(", ", resultUbicacionesLotesMaster.Mensajes));
        //                       }
        //                   }

        //                   var resultKardex = _kardexAbastecimientoN.InsertarTransaccionIngresoKardex(transferenciaActualizada, cn);
        //                   if (resultKardex.IconoSweetAlert.Equals("error"))
        //                   {
        //                       throw new Exception(string.Join(", ", resultKardex.Mensajes));
        //                   }

        //                   scope.Complete();
        //               }

        //           }
        //           return Json(new { Titulo = "Acción completada exitosamente", Mensajes = new List<string> { "Se registró la Transferencia Reserva correctamente." }, Icono = "success" });
        //       }
        //       catch (Exception ex)
        //       {
        //           using (var scope = new TransactionScope(TransactionScopeOption.Required,
        //new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
        //TransactionScopeAsyncFlowOption.Enabled))
        //           {
        //               Utilitarios uti = new Utilitarios();
        //               using (SqlConnection cn = new SqlConnection(uti.cadSql2))
        //               {
        //                   cn.Open();
        //                   TransferenciaReserva_E transferenciaExistente = _transferenciaReservaN.ObtenerTransferenciaReserva(transferenciaPost.SolicitudTrasladoDocNum);
        //                   if (transferenciaExistente == null || transferenciaExistente.Detalle.Count == 0)
        //                   {
        //                       var resultDeleteSolicitudTraslado = _solicitudTrasladoN.DeleteSolicitudDeTraslado(solicitudTraslado.DocNum, cn);
        //                       if (resultDeleteSolicitudTraslado.IconoSweetAlert.Equals("error"))
        //                       {
        //                           throw new Exception(string.Join(", ", resultDeleteSolicitudTraslado.Mensajes));
        //                       }
        //                   }
        //                   scope.Complete();
        //               }
        //           }

        //           return Json(new
        //           {
        //               Titulo = "Error en la operación",
        //               Mensajes = new List<string> { ex.Message }, // Excepción ya tiene los mensajes concatenados
        //               Icono = "error"
        //           });
        //       }

        //   }

        //public JsonResult RegistrarTransferenciaDeStock(SolicitudesTraslado_E solicitudTraslado, TransferenciaReserva_E transferenciaPost, int idOperation = 3302)
        //{
        //    try
        //    {
        //        if (transferenciaPost == null)
        //        {
        //            return Json(new
        //            {
        //                Titulo = "Error en la operación",
        //                Mensajes = new List<string> { "El documento que trata de registrar no tiene una transferencia realizándose." },
        //                Icono = "error"
        //            });
        //        }

        //        if (solicitudTraslado == null || solicitudTraslado.DocNum == 0)
        //            solicitudTraslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(transferenciaPost.SolicitudTrasladoDocNum);

        //        var resultLotes = _lotesRegistroSanitarioN.ValidarLotesRegistroSanitario(solicitudTraslado.Detalle);
        //        if (resultLotes.IconoSweetAlert.Equals("error"))
        //        {
        //            return Json(new { Titulo = "No se pudo completar la acción", resultLotes.Mensajes, Icono = resultLotes.IconoSweetAlert });
        //        }

        //        if (solicitudTraslado == null || solicitudTraslado.Id == 0)
        //        {
        //            var resultSolicitud = _solicitudTrasladoN.ImportarSolicitudDeTraslado(solicitudTraslado);
        //            if (resultSolicitud.IconoSweetAlert.Equals("error") || resultSolicitud.Id == 0)
        //            {
        //                return Json(new { Titulo = "No se pudo completar la acción", resultSolicitud.Mensajes, Icono = "error" });
        //            }
        //            solicitudTraslado.Id = resultSolicitud.Id;
        //        }

        //        Usuario_E user = (Usuario_E)Session["UsuarioId"];
        //        if (user == null)
        //        {
        //            return Json(new
        //            {
        //                Titulo = "Error en la operación",
        //                Mensajes = new List<string> { "No existe usuario logueado, se terminó la sesión." },
        //                Icono = "error"
        //            });
        //        }

        //        transferenciaPost.OperarioRegistra = $"{user.Nombres} {user.Apellidos}";
        //        transferenciaPost.SolicitudTrasladoId = solicitudTraslado.Id;
        //        transferenciaPost.SolicitudTrasladoDocNum = solicitudTraslado.DocNum;

        //        Utilitarios uti = new Utilitarios();

        //        using (var scope = new TransactionScope(TransactionScopeOption.Required,
        //           new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
        //           TransactionScopeAsyncFlowOption.Enabled))
        //        {
        //            try
        //            {
        //                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
        //                {
        //                    cn.Open();
        //                    var resultRegistroTransferencia = _transferenciaReservaN.RegistrarTransferenciaReserva(transferenciaPost, cn);
        //                     if (resultRegistroTransferencia.IconoSweetAlert.Equals("error")) {
        //                        //Validar si no existe algunos items de transferencia sobre esta solicitudTraslado
        //                       TransferenciaReserva_E transferenciaExistente = _transferenciaReservaN.ObtenerTransferenciaReserva(transferenciaPost.SolicitudTrasladoDocNum);
        //                        if (transferenciaExistente == null || transferenciaExistente.Detalle.Count == 0)
        //                        {
        //                            // Eliminar la solicitud de traslado si en caso se importo a la tabla interna pero no se ha registrado un item por lo menos de transferencia
        //                            _solicitudTrasladoN.DeleteSolicitudDeTraslado(solicitudTraslado.DocNum, cn);
        //                        }
        //                        throw new Exception("No se pudo registrar la transferencia de reserva.");
        //                    }
        //                    else { 
        //                        TransferenciaReserva_E transferenciaActualizada = _transferenciaReservaN.ObtenerTransferenciaReserva(transferenciaPost.SolicitudTrasladoDocNum);
        //                        var resultActualizarEstado = _solicitudTrasladoN.ActualizarEstado(transferenciaActualizada.SolicitudTrasladoId, transferenciaActualizada.Detalle, cn);
        //                        if (resultActualizarEstado.IconoSweetAlert.Equals("error"))
        //                            throw new Exception("No se pudo actualizar el estado de los items en solicitud de traslado.");

        //                        foreach (var item in transferenciaActualizada.Detalle)
        //                        {
        //                            var resultUbicacionesLotes = _ubicacionesLotesN.Ingreso(item, cn);
        //                            if (resultUbicacionesLotes.IconoSweetAlert.Equals("error"))
        //                                throw new Exception("No se pudo registrar en Ubicaciones Lotes.");

        //                            int ubicacionLoteId = resultUbicacionesLotes.Id;
        //                            var resultUbicacionesLotesMaster = _ubicacionesLotesMasterN.Ingreso(ubicacionLoteId, item, cn);
        //                            if (resultUbicacionesLotesMaster.IconoSweetAlert.Equals("error"))
        //                                throw new Exception("No se pudo registrar en Ubicaciones Lotes Master.");
        //                        }

        //                        var resultKardex = _kardexAbastecimientoN.InsertarTransaccionIngresoKardex(transferenciaActualizada, cn);
        //                        if (resultKardex.IconoSweetAlert.Equals("error"))
        //                            throw new Exception("No se pudo registrar la transacción en Kardex.");
        //                    }
        //                }

        //                scope.Complete();
        //                return Json(new { Titulo = "Acción completada exitosamente", Mensajes = new List<string> { "Se registró la Transferencia Reserva correctamente." }, Icono = "success" });
        //            }
        //            catch (Exception)
        //            {
        //                scope.Dispose(); 
        //                throw; // Relanza la excepción para ser capturada en el catch externo, aqui se transforma en Json
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { $"Ocurrió un error al registrar la transferencia: {ex.Message}" }, Icono = "error" });
        //    }
        //}

        public JsonResult RegistrarTransferenciaDeStock(SolicitudesTraslado_E solicitudTraslado, TransferenciaReserva_E transferenciaPost, int idOperation = 3302)
        {
            try
            {
                // Obtener usuario de sesión
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

                if (transferenciaPost != null)
                {
                    Utilitarios uti = new Utilitarios();
                    // Iniciar la transacción global para las operaciones críticas
                    using (var scope = new TransactionScope(TransactionScopeOption.Required,
                       new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                       TransactionScopeAsyncFlowOption.Enabled))
                    {
                        using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                        {
                            cn.Open();
                            //Es exclusivo para la continuacion de transferencia en una solicitud de traslado.
                            if (solicitudTraslado == null || solicitudTraslado.DocNum == 0)
                                solicitudTraslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(transferenciaPost.SolicitudTrasladoDocNum, cn);

                            if (solicitudTraslado == null || solicitudTraslado.Id == 0)
                            {
                                // Importa a las tablas internas solo si no existe previamente el DocNum
                                var resultImportarSolicitud = _solicitudTrasladoN.ImportarSolicitudDeTraslado(solicitudTraslado, cn);

                                // Validar si la importación fue exitosa
                                if (resultImportarSolicitud.IconoSweetAlert.Equals("error") || resultImportarSolicitud.Id == 0)
                                {
                                    return Json(new
                                    {
                                        Titulo = "No se pudo completar la acción",
                                        resultImportarSolicitud.Mensajes,
                                        Icono = "error"
                                    });
                                }
                                //Asigna su Id porque ya fue insertado
                                solicitudTraslado.Id = resultImportarSolicitud.Id;
                            }

                            // Validar o inserta los lotes de registro sanitario (fuera de la transacción)
                            var resultLotes = _lotesRegistroSanitarioN.ValidarLotesRegistroSanitario(solicitudTraslado.Detalle, cn);
                            if (resultLotes.IconoSweetAlert.Equals("error"))
                            {
                                return Json(new
                                {
                                    Titulo = "No se pudo completar la acción",
                                    resultLotes.Mensajes,
                                    Icono = resultLotes.IconoSweetAlert
                                });
                            }

                            // Asignar datos de traslado a la transferencia, preparando para registrar  agregar lineas a la transferencia
                            transferenciaPost.OperarioRegistra = $"{user.Nombres} {user.Apellidos}";
                            transferenciaPost.SolicitudTrasladoId = solicitudTraslado.Id;
                            transferenciaPost.SolicitudTrasladoDocNum = solicitudTraslado.DocNum;


                            // Registrar  o agrega mas lineas al detalle de la transferencia de reserva
                            var resultTransferenciaGet = _transferenciaReservaN.RegistrarTransferenciaReserva(transferenciaPost, cn);
                            if (resultTransferenciaGet == null || resultTransferenciaGet.Id == 0)
                            {
                                if (resultTransferenciaGet.IconoSweetAlert.Equals("error"))
                                {
                                    // Validar y eliminar la solicitud de traslado si en caso se importo a la tabla interna pero no se ha encontrado una transferencia
                                    _solicitudTrasladoN.DeleteSolicitudDeTraslado(solicitudTraslado.DocNum, cn);
                                    return Json(new
                                    {
                                        Titulo = "No se pudo completar la acción",
                                        resultTransferenciaGet.Mensajes,
                                        Icono = resultTransferenciaGet.IconoSweetAlert
                                    });

                                }
                            }

                            TransferenciaReserva_E transferencia = _transferenciaReservaN.ObtenerTransferenciaReserva(transferenciaPost.SolicitudTrasladoDocNum, cn);

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
                           
                            int ubicacionLoteId = 0;
                            foreach (var item in transferencia.Detalle)
                            {
                                var resultUbicacionesLotes = _ubicacionesLotesN.Ingreso(item, cn);
                                if (resultUbicacionesLotes.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Titulo = "No se pudo completar la acción",
                                        resultUbicacionesLotes.Mensajes,
                                        Icono = resultUbicacionesLotes.IconoSweetAlert
                                    });
                                }
                                ubicacionLoteId = resultUbicacionesLotes.Id;
                                // Sumar y/o Registrar en la tabla UbicacionesLotesMaster
                                //Se envia el parametro de UbicacionesLoteId si en caso es una nueva UbicacionLote creada
                                var resultUbicacionesLotesMaster = _ubicacionesLotesMasterN.Ingreso(ubicacionLoteId, item, cn);
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
                    Mensajes = new List<string> { $"Ocurrió un error al registrar la transferencia: {ex.Message}" },
                    Icono = "error"
                });
            }
        }
        public JsonResult CancelarTransferenciaYTraslado(int docNum, int idOperation = 3303) //recibe el docnum de la solicitud de traslado
        {
            try
            {
                if (docNum > 0)
                {
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
                            var transferenciaGet = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum, cn);
                            if (transferenciaGet == null || transferenciaGet.Id == 0)
                            {
                                return Json(new
                                {
                                    Titulo = "No se pudo completar la acción",
                                    Mensajes = new List<string> { "No se encontró transferencia de reserva relacionada." },
                                    Icono = "error"
                                });
                            }
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
        public JsonResult RevertirTransferenciaReservaPorItem(int docNum, string itemCode, int idOperation = 3304) //recibe el docnum de la solicitud de traslado y el ItemCode a revertir
        {
            try
            {
                if (docNum > 0)
                {
                    //Una vez validado los ids enviarlo a la transaccion unica para la reversion de todo el grupo segun ItemCode
                    using (var scope = new TransactionScope(TransactionScopeOption.Required,
                             new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                             TransactionScopeAsyncFlowOption.Enabled))
                    {
                        Utilitarios uti = new Utilitarios();
                        using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                        {
                            cn.Open();
                            var transferenciaGet = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum, cn);
                            if (transferenciaGet == null || transferenciaGet.Id == 0)
                            {
                                return Json(new
                                {
                                    Titulo = "No se pudo completar la acción",
                                    Mensajes = new List<string> { "No se encontró transferencia de reserva relacionada." },
                                    Icono = "error"
                                });
                            }
                            var traslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(docNum, cn);

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


                                int quantityCajasItemCode = Convert.ToInt32(traslado.Detalle.Sum(x=>x.Value.QuantityCajas));

                                //Si las cantidades no coinciden quiere decir que no se ha pasado el grupo completo de los ids correspondientes a un ItemCode en la solicitud de traslado, muestra error
                                if (quantityCajasItemCode != transferenciaGet.Detalle.Where(x => x.ItemCode == itemCode).Sum(x => x.QuantityUnidadesCajas))
                                {
                                    return Json(new
                                    {
                                        Titulo = "No se pudo completar la acción",
                                        Mensajes = new List<string> { $"La suma de cantidades a revertir no coincide con el total en la solicitud de traslado para el SKU:{transferenciaGet.Detalle[0].ItemCode} " },
                                        Icono = "error"
                                    });
                                }



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
                                var resultTransferencia = _transferenciaReservaN.DeleteDetalleItemTransferenciaReserva(transferenciaGet.Detalle, traslado.Detalle, cn);
                                if (resultTransferencia.IconoSweetAlert.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Titulo = "No se pudo completar la acción",
                                        resultTransferencia.Mensajes,
                                        Icono = resultTransferencia.IconoSweetAlert
                                    });
                                }

                                //Verificar si la TransferenciaReserva se quedo sin elementos 
                                var transferenciaPostReversion = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum, cn);

                                if (transferenciaPostReversion != null && transferenciaPostReversion.Detalle.Count() == 0)
                                {
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

                                    //Luego Eliminar Detalle y Cabecera de Solicitud de Traslado solo si la transferencia ya se elimino
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

                                }

                                scope.Complete();
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
        public ActionResult Requerimientos(int idOperation = 3400)
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

            // Orden: próxima fecha de vencimiento, primera fecha de admisión registrada, la menor cantidad en unidades
            List<UbicacionesLotesMaster_E> lista = _ubicacionesLotesMasterN.BuscarArticulos(new UbicacionesLotesMaster_E { ItemCode = itemCode })
                .OrderBy(a => DateTime.Parse(a.ExpDate))
                .ThenBy(a => DateTime.Parse(a.InDate))
                .ThenBy(a => a.QuantityUnidadesCajas)
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
        public JsonResult RegistrarRequerimiento(Requerimientos_E requerimiento, int idOperation = 3401)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { "Sin accesos." },
                    Icono = "error"
                });
            }
        }
        public JsonResult CalcularCantidadSolicitada(string tipoAbastecimiento, string itemCode, int idOperation = 3402)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                int cantidadSolicitada = 0;
                if (tipoAbastecimiento != null && tipoAbastecimiento.Equals("Picking") && itemCode != null)
                {
                    //Calcular desde SAP (Stock Total - Stock Comprometido)  en Almacen 16 por defecto
                    int stockLibreEnAlmacen16 = Convert.ToInt32(new Capa_Negocio.Almacen_NEG.Tablas.OITW_N().ListarDetArticulosInv(new OITW_E { ItemCode = itemCode, WhsCode = "16" }).DefaultIfEmpty(new OITW_E { }).First().StockLibre);

                    List<DetalleRequerimientos_E> resultDetReq = _requerimientosN.ListarDetalles(itemCode, "CantidadSolicitada");
                    int quantityReq = 0;
                    if (resultDetReq!= null) { quantityReq=Convert.ToInt32(resultDetReq.Sum(r => r.QuantityUnidadesCajas)); }

                    List<UbicacionesLotes_E> resultUbicacionesLotes = _ubicacionesLotesN.Obtener(itemCode);
                    int quantityUbicacionesLote = 0; 
                    if (resultUbicacionesLotes != null) { quantityUbicacionesLote=resultUbicacionesLotes.Sum(r => r.QuantityUnidadesCajas); }

                    int stockDeAlmReserva = quantityUbicacionesLote - quantityReq; //resta de lo que esta por entrar a Picking Atendido=0

                    int stockEnPicking = stockLibreEnAlmacen16 - stockDeAlmReserva;

                    int stockMinimoParaLaVenta = _stockMinProdN.Obtener(itemCode).StockMinVenta;

                    cantidadSolicitada = stockMinimoParaLaVenta - stockEnPicking;

                    if (cantidadSolicitada < 0) { cantidadSolicitada = 0; }
                }

                return Json(new { cantidadSolicitada = Convert.ToString(cantidadSolicitada) });
            }
            else
            {
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Sin accesos." }, Icono = "error" });
            }
        }
        //Listado de pendientes para apiladores con 4 posibles filtros
        public ActionResult ListarRequerimientosReserva(int idOperation = 3500)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _requerimientosN.ListarDetalles("", "ListarApiladores");
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //Listado de pendientes para picking con 1 posible filtros
        public ActionResult ListarRequerimientosPicking(int idOperation = 3600)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _requerimientosN.ListarDetalles("", "ListarPicking");
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //Atendido de apiladores (Solo cambia el AtendidoReserva a 1)
        public ActionResult ListarControlStockInterno(int idOperation = 3700)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _reporteStockPicking.ControlStockInternoPicking();//.Where(x=>x.StockActual <= x.StockMinAbastecimiento);
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //Atendido de apiladores (Solo cambia el AtendidoReserva a 1)
        public JsonResult AtenderReservaRequerimiento(int id, int idOperation = 3501)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
            else
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { "Sin accesos." },
                    Icono = "error"
                });
            }
        }
        //Atendido de Picking (Cambia el AtendidoPicking a 1 y valida si todo se completo respecto al sku del requerimiento para generar una salida en el Kardex
        public JsonResult AtenderPickingRequerimiento(int id, int requerimientoId, string itemCode, string itemName, int idOperation = 3601)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    if (id > 0 && requerimientoId > 0 && !string.IsNullOrEmpty(itemCode) && !string.IsNullOrEmpty(itemName))
                    {
                        //Actualizar a AtendidoPicking 1 solo la linea de detalle enviada
                        var resultAtender = _requerimientosN.AtenderPicking(id);

                        if (resultAtender != null && resultAtender.IconoSweetAlert.Equals("success"))
                        {
                            var requerimiento = _requerimientosN.ObtenerRequerimiento(requerimientoId);
                            //Valida que todos los items del requerimiento con el mismo Sku esten en AtendidoPicking 1, de ser asi se genera kardex de salida
                            bool listoKardexSalida = false;
                            listoKardexSalida = _requerimientosN.ValidarSkuParaKardexSalida(requerimientoId, itemCode, requerimiento);

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
                                    // Registrar la operacion de salida en KardexAbastecimiento 
                                    using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                                    {
                                        cn.Open();

                                        // Con el requerimiento obtenido y calcula cantidad global es la suma de todas las cantidades del itemCode
                                        var requerimientoPorSku = requerimiento.Detalle.Where(x => x.ItemCode == itemCode).ToList();
                                        int cantidadGlobal = Convert.ToInt32(requerimientoPorSku.Sum(x => x.QuantityUnidadesCajas));

                                        var resultKardex = _kardexAbastecimientoN.InsertarTransaccionSalidaKardex(itemCode, itemName, cantidadGlobal, operarioRegistra, requerimientoId, cn);

                                        if (resultKardex.IconoSweetAlert.Equals("error"))
                                        {
                                            return Json(new
                                            {
                                                Titulo = "No se pudo completar la acción",
                                                resultKardex.Mensajes,
                                                Icono = resultKardex.IconoSweetAlert
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

                                        scope.Complete();
                                    }
                                }

                                // Devolver respuesta exitosa
                                return Json(new
                                {
                                    Titulo = "Acción completada exitosamente",
                                    Mensajes = new List<string> { "Se atendió el SKU correctamente y se generó kardex por salida." },
                                    Icono = "success"
                                });
                            }

                            // Devolver respuesta exitosa
                            return Json(new
                            {
                                Titulo = "Acción completada exitosamente",
                                Mensajes = new List<string> { "Se atendió el SKU correctamente. " },
                                Icono = "success"
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                Titulo = "Error en la operación",
                                resultAtender.Mensajes,
                                Icono = resultAtender.IconoSweetAlert
                            });
                        }
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
            else
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { "Sin accesos." },
                    Icono = "error"
                });
            }
        }

    }
}