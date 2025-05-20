using Capa_Datos;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.TablasSql;
using Capa_Negocio.AbastecimientoInterno_NEG.Reportes;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasExternas;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasSql;
using Capa_Negocio.DireccionTecnica_NEG.TablasSql;
using Capa_Usuario.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using IsolationLevel = System.Transactions.IsolationLevel;

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
        private readonly ReporteStockReserva_N _reporteStockReserva = new ReporteStockReserva_N();
        private readonly ProductosDisponiblesReserva_N _productosDisponiblesReserva = new ProductosDisponiblesReserva_N();
        private readonly Capa_Negocio.Helpers _helper = new Capa_Negocio.Helpers();
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
                var listaULM = _ubicacionesLotesN.ListarUbicaciones(new UbicacionesLotes_E { Almacen = "PICKING" });

                ViewBag.UbicacionesLotes = listaULM.Select(u => u.CodigoUbicacion).Distinct().ToList();
                ViewBag.ItemsCodes = listaULM.Select(u => u.ItemCode).Distinct().ToList();
                ViewBag.ItemsNames = listaULM.Select(u => u.ItemName).Distinct().ToList();

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
                    return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

                // Inicializar con valores vacíos
                string itemCode = string.Empty;
                string itemName = string.Empty;

                // Verificar si se envió alguna búsqueda por UbicacionesLotes
                if (filtros.UbicacionesLotes != null && filtros.UbicacionesLotes.Any())
                {
                    var primerLote = filtros.UbicacionesLotes[0];

                    if (!string.IsNullOrEmpty(primerLote.ItemCode))
                        itemCode = primerLote.ItemCode;

                    if (!string.IsNullOrEmpty(primerLote.ItemName))
                        itemName = primerLote.ItemName;
                }

                var listaULM = _ubicacionesLotesN.ListarUbicaciones(new UbicacionesLotes_E
                {
                    Almacen = "PICKING",
                    ItemCode = itemCode,
                    ItemName = itemName
                });

                // Agrupar listaULM por CodigoUbicacion
                var cantidadPorUbicacion = listaULM
                    .GroupBy(u => u.CodigoUbicacion)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(u => u.ItemCode).Distinct().Count()
                    );

                // Obtener solo las ubicaciones donde se encontraron lotes (listaULM)
                var codigosUbicacionesConProducto = listaULM
                    .Select(x => x.CodigoUbicacion)
                    .Distinct()
                    .ToList();

                // Filtrar ubicaciones completas solo por las que tienen productos encontrados
                var listaU = _ubicacionesN.ListarUbicaciones(filtros)
                    .Where(u => codigosUbicacionesConProducto.Contains(u.CodigoUbicacion))
                    .ToList();

                // Asignar cantidad de productos a cada ubicación
                foreach (var ubicacion in listaU)
                {
                    ubicacion.CantidadProductos = cantidadPorUbicacion.TryGetValue(ubicacion.CodigoUbicacion, out int cantidad)
                        ? cantidad
                        : 0;
                }

                ViewBag.ListaFiltradaUbicacionesLotes = listaULM;

                return PartialView("AbastecimientoInterno/_ListadoUbicacionesPicking", listaU);
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
            string tituloSweetAlert = result.Icono.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            return Json(new
            {
                Titulo = tituloSweetAlert,
                result.Mensajes,
                Icono = result.Icono
            });
        }
        public JsonResult EliminarUbicacionPicking(string codigoUbicacion, int idOperation = 3102)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new
                {
                    Titulo = "No se pudo completar la acción",
                    Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" },
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);
            var result = _ubicacionesN.EliminarUbicacion(codigoUbicacion);
            string tituloSweetAlert = result.Icono.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            return Json(new
            {
                Titulo = tituloSweetAlert,
                result.Mensajes,
                Icono = result.Icono
            });
        }
        public JsonResult ActualizarStockMinimoPicking(StockMinProductos_E form, int idOperation = 3103)
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
                var result = _stockMinProdN.ActualizarStockMinimo(form);
                string tituloSweetAlert = result.Icono.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
                return Json(new
                {
                    Titulo = tituloSweetAlert,
                    result.Mensajes,
                    Icono = result.Icono
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
        public ActionResult StockMinimoPicking(int idOperation = 3104)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _stockMinProdN.ListarStockMinProductos();
                var productos = _productosN.ListarProductos();
                // Excluir los ItemCode que están en "lista"
                var itemCodesAExcluir = lista.Select(s => s.ItemCode).ToHashSet(); // Usar HashSet para mejor rendimiento
                var productosFiltrados = productos.Where(p => !itemCodesAExcluir.Contains(p.ItemCode)).ToList();
                ViewBag.Productos = productosFiltrados;
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public JsonResult RegistrarStockMinimoPicking(StockMinProductos_E form, int idOperation = 3103)
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
            var result = _stockMinProdN.RegistrarStockMinimo(form);
            string tituloSweetAlert = result.Icono.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            return Json(new
            {
                Titulo = tituloSweetAlert,
                result.Mensajes,
                Icono = result.Icono
            });
        }
        public JsonResult EliminarArticuloPicking(string itemCode, string codigoUbicacion, int idOperation = 3105)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);
            var result = _ubicacionesLotesN.EliminarArticulo(itemCode, codigoUbicacion);
            string tituloSweetAlert = result.Icono.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            return Json(new { Titulo = tituloSweetAlert, result.Mensajes, Icono = result.Icono });
        }
        public ActionResult ExportarExcelUbicacionesPicking(int idOperation = 3106)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                int columnas = 4;
                var listado = _ubicacionesN.ListarUbicaciones(new Ubicaciones_E { Almacen = "PICKING" });
                var listaULM = _ubicacionesLotesN.ListarUbicaciones(new UbicacionesLotes_E { Almacen = "PICKING" });
                var codigoU = listaULM
                    .GroupBy(u => u.CodigoUbicacion)
                    .ToDictionary(g => g.Key, g => g.ToList());
                foreach (var item in listado)
                {
                    item.UbicacionesLotes = codigoU.ContainsKey(item.CodigoUbicacion)
                        ? codigoU[item.CodigoUbicacion] // Si hay lotes, asignarlos
                        : new List<UbicacionesLotes_E>(); // Si no hay lotes, asignar lista vacía
                }
                var nuevaLista = listado.GroupJoin(
                                                    listaULM,
                                                    ub => ub.CodigoUbicacion,  // Clave en listado
                                                    ulm => ulm.CodigoUbicacion, // Clave en listaULM
                                                    (ub, ulmGroup) => new
                                                    {
                                                        CodigoUbicacion = ub.CodigoUbicacion,
                                                        Lotes = ulmGroup.Any() ? ulmGroup : new List<UbicacionesLotes_E> { new UbicacionesLotes_E() } // Si no tiene lotes, agrega una fila vacía
                                                    }
                                                )
                                                .SelectMany(grupo => grupo.Lotes.Select(ulm => new
                                                {
                                                    CodigoUbicacion = grupo.CodigoUbicacion,
                                                    CodigoArticulo = ulm.ItemCode ?? "",
                                                    Descripcion = ulm.ItemName ?? "",
                                                    Lote = ulm.BatchNum ?? ""
                                                }))
                                                .OrderByDescending(x => !string.IsNullOrEmpty(x.Lote))  // Primero los que tienen Lote
                                                .ThenBy(x => x.CodigoUbicacion)   // Luego ordena alfabéticamente por CódigoUbicacion
                                                .ToList();
                if (nuevaLista != null && nuevaLista.Any())
                {
                    using (var libro = new ExcelPackage())
                    {
                        var worksheet = libro.Workbook.Worksheets.Add("ReporteUbicaciones_PICKING");
                        worksheet.Cells["A1"].LoadFromCollection(nuevaLista, PrintHeaders: true);
                        for (var col = 1; col <= columnas; col++)
                        {
                            worksheet.Column(col).AutoFit();
                        }
                        var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: nuevaLista.Count() + 1, toColumn: columnas), "ReporteUbicaciones_PICKING");
                        tabla.ShowHeader = true;
                        tabla.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;
                        string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(libro.GetAsByteArray(), excelContentType, "ReporteUbicaciones_PICKING.xlsx");
                    }
                }
                else { return Content("No hay datos para exportar"); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        /************************* U B I C A C I O N E S   R E S E R V A *************************/
        public ActionResult UbicacionesReserva(int idOperation = 3200)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var listaULM = _ubicacionesLotesN.ListarUbicaciones(new UbicacionesLotes_E { Almacen = "RESERVA" });

                ViewBag.UbicacionesLotes = listaULM.Select(u => u.CodigoUbicacion).Distinct().ToList();
                ViewBag.ItemsCodes = listaULM.Select(u => u.ItemCode).Distinct().ToList();
                ViewBag.ItemsNames = listaULM.Select(u => u.ItemName).Distinct().ToList();

                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }
        [HttpGet]
        public ActionResult ListarUbicacionesReserva(Ubicaciones_E filtros, int idOperation = 3201)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                filtros.Almacen = "RESERVA";

                var usuarioSesion = Session["UsuarioId"] as Usuario_E;
                if (usuarioSesion == null)
                    return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

                // Inicializar con valores vacíos
                string itemCode = string.Empty;
                string itemName = string.Empty;

                // Verificar si se envió alguna búsqueda por UbicacionesLotes
                if (filtros.UbicacionesLotes != null && filtros.UbicacionesLotes.Any())
                {
                    var primerLote = filtros.UbicacionesLotes[0];

                    if (!string.IsNullOrEmpty(primerLote.ItemCode))
                        itemCode = primerLote.ItemCode;

                    if (!string.IsNullOrEmpty(primerLote.ItemName))
                        itemName = primerLote.ItemName;
                }

                var listaULM = _ubicacionesLotesN.ListarUbicaciones(new UbicacionesLotes_E
                {
                    Almacen = "RESERVA",
                    ItemCode = itemCode,
                    ItemName = itemName
                });

                // Agrupar listaULM por CodigoUbicacion
                var cantidadPorUbicacion = listaULM
                    .GroupBy(u => u.CodigoUbicacion)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(u => u.ItemCode).Distinct().Count()
                    );

                // Obtener solo las ubicaciones donde se encontraron lotes (listaULM)
                var codigosUbicacionesConProducto = listaULM
                    .Select(x => x.CodigoUbicacion)
                    .Distinct()
                    .ToList();

                // Filtrar ubicaciones completas solo por las que tienen productos encontrados
                var listaU = _ubicacionesN.ListarUbicaciones(filtros)
                    .Where(u => codigosUbicacionesConProducto.Contains(u.CodigoUbicacion))
                    .ToList();

                // Asignar cantidad de productos a cada ubicación
                foreach (var ubicacion in listaU)
                {
                    ubicacion.CantidadProductos = cantidadPorUbicacion.TryGetValue(ubicacion.CodigoUbicacion, out int cantidad)
                        ? cantidad
                        : 0;
                }

                ViewBag.ListaFiltradaUbicacionesLotes = listaULM;

                return PartialView("AbastecimientoInterno/_ListadoUbicacionesReserva", listaU);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public JsonResult RegistrarUbicacionReserva(Ubicaciones_E form, int idOperation = 3202)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
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
                string tituloSweetAlert = result.Icono.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
                return Json(new
                {
                    Titulo = tituloSweetAlert,
                    result.Mensajes,
                    Icono = result.Icono
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
        public JsonResult EliminarUbicacionReserva(string codigoUbicacion, int idOperation = 3203)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var usuarioSesion = Session["UsuarioId"] as Usuario_E;
                if (usuarioSesion == null)
                    return Json(new { Titulo = "No se pudo completar la acción", Comentario = "Inicia sesión nuevamente para continuar", Icono = "error" }, JsonRequestBehavior.AllowGet);
                var result = _ubicacionesN.EliminarUbicacion(codigoUbicacion);
                string tituloSweetAlert = result.Icono.Equals("success") ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
                return Json(new { Titulo = tituloSweetAlert, result.Mensajes, Icono = result.Icono });
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
        public ActionResult StockReserva(int idOperation = 3204)
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
        public ActionResult ListarStockReserva(Ubicaciones_E filtros, int idOperation = 3204)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var usuarioSesion = Session["UsuarioId"] as Usuario_E;
                if (usuarioSesion == null)
                    return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);
                var lista = _reporteStockReserva.Listar();
                return PartialView("AbastecimientoInterno/_ListadoStockReserva", lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public ActionResult StockPicking(int idOperation = 3209)
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
        public ActionResult ListarStockPicking(Ubicaciones_E filtros, int idOperation = 3209)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var usuarioSesion = Session["UsuarioId"] as Usuario_E;
                if (usuarioSesion == null)
                    return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

                var lista = _reporteStockPicking.ListarStockPicking();

                return PartialView("AbastecimientoInterno/_ListadoStockPicking", lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public JsonResult CambiarUbicacionPicking(string nuevoCodigoUbicacion, int ubicacionLoteId, int idOperation = 3210)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                if (string.IsNullOrWhiteSpace(nuevoCodigoUbicacion) || ubicacionLoteId <= 0)
                    return Json(_helper.CrearAlertaUI(new List<string> { "Verificar datos enviados." }, "warning"));

                return Json(_ubicacionesLotesN.CambiarUbicacionPicking(nuevoCodigoUbicacion, ubicacionLoteId));
            }
            else
            {
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Sin accesos." }, Icono = "error" });
            }
        }

        public JsonResult CambioUbicacionReserva(string nuevoCodigoUbicacion, int idUbicacionLoteMaster, int idOperation = 3205)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    if (idUbicacionLoteMaster > 0 && !string.IsNullOrEmpty(nuevoCodigoUbicacion))
                    {
                        //Obtener todo el registro de ubicacionLoteMaster que esta mudando
                        var obj = _ubicacionesLotesMasterN.Obtener(idUbicacionLoteMaster);
                        //Validar que desde requerimientos el producto ubicacion y lote no tiene Imputados
                        bool resultValidarSku = _requerimientosN.ValidarSkuParaCambioUbicacion(obj.ItemCode, obj.BatchNum, obj.CodigoUbicacion);
                        if (!resultValidarSku)
                        {
                            return Json(new
                            {
                                Titulo = "Error en la operación",
                                Mensajes = new List<string> { "Existe comprometidos en proceso para Sku, Lote y Ubicación. Puede revisarlo en Módulo Picking" },
                                Icono = "error"
                            });
                        }
                        //Construir un objeto para enviar a la salida del Sku en la antigua ubicacion y a su vez al ingreso en la nueva ubicacion
                        List<DetalleRequerimientos_E> listaEnvioDatos = new List<DetalleRequerimientos_E> { new DetalleRequerimientos_E {
                            ItemCode=obj.ItemCode,
                            ItemName=obj.ItemName,
                            BatchNum=obj.BatchNum,
                            UmAlm=obj.UmAlm,
                            ValorUmAlm=obj.ValorUmAlm,
                            QuantityMaster=obj.QuantityMaster,
                            QuantitySaldo = obj.QuantitySaldo,
                            QuantityUnidadesCajas= obj.QuantityUnidadesCajas,
                            CodigoUbicacionOrigen=obj.CodigoUbicacion
                        } };
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
                                var resultSalidaUbicacionesLotesMaster = _ubicacionesLotesMasterN.Salida(listaEnvioDatos, cn);
                                if (resultSalidaUbicacionesLotesMaster.Icono.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Titulo = "Error en la operación",
                                        resultSalidaUbicacionesLotesMaster.Mensajes,
                                        Icono = resultSalidaUbicacionesLotesMaster.Icono
                                    });
                                }
                                var resultSalidaUbicacionesLotes = _ubicacionesLotesN.Salida(listaEnvioDatos, cn);
                                if (resultSalidaUbicacionesLotes.Icono.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Titulo = "Error en la operación",
                                        resultSalidaUbicacionesLotes.Mensajes,
                                        Icono = resultSalidaUbicacionesLotes.Icono
                                    });
                                }
                                var resultIngresoUbicacionesLotes = _ubicacionesLotesN.Ingreso(new DetalleTransferenciaReserva_E
                                {
                                    ItemCode = obj.ItemCode,
                                    ItemName = obj.ItemName,
                                    BatchNum = obj.BatchNum,
                                    CodigoUbicacion = nuevoCodigoUbicacion,
                                    QuantityUnidadesCajas = Convert.ToInt32(obj.QuantityUnidadesCajas)
                                }, cn);
                                if (resultSalidaUbicacionesLotes.Icono.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Titulo = "Error en la operación",
                                        resultIngresoUbicacionesLotes.Mensajes,
                                        Icono = resultIngresoUbicacionesLotes.Icono
                                    });
                                }
                                var resultIngresoUbicacionesLotesMaster = _ubicacionesLotesMasterN.Ingreso(resultIngresoUbicacionesLotes.Id, new DetalleTransferenciaReserva_E
                                {
                                    ItemCode = obj.ItemCode,
                                    ItemName = obj.ItemName,
                                    BatchNum = obj.BatchNum,
                                    CodigoUbicacion = nuevoCodigoUbicacion,
                                    QuantityMaster = obj.QuantityMaster,
                                    QuantitySaldo = obj.QuantitySaldo,
                                    UmAlm = obj.UmAlm,
                                    ValorUmAlm = obj.ValorUmAlm
                                }, cn);
                                if (resultIngresoUbicacionesLotesMaster.Icono.Equals("error"))
                                {
                                    return Json(new
                                    {
                                        Titulo = "Error en la operación",
                                        resultIngresoUbicacionesLotesMaster.Mensajes,
                                        Icono = resultIngresoUbicacionesLotesMaster.Icono
                                    });
                                }
                                scope.Complete();
                            }
                        }
                        return Json(new
                        {
                            Titulo = "Acción completada exitosamente",
                            Mensajes = new List<string> { "Se realizó correctamente el cambio de ubicación" },
                            Icono = "success"
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Titulo = "Error en la operación",
                            Mensajes = new List<string> { "La nueva ubicacion es invalida." },
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
        public ActionResult ExportarExcelUbicacionesReserva(int idOperation = 3206)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                int columnas = 7;
                var listado = _ubicacionesN.ListarUbicaciones(new Ubicaciones_E { Almacen = "RESERVA" });
                var listaULM = _ubicacionesLotesMasterN.ListarUbicaciones(new UbicacionesLotesMaster_E { Almacen = "RESERVA" });
                var codigoU = listaULM
                    .GroupBy(u => u.CodigoUbicacion)
                    .ToDictionary(g => g.Key, g => g.ToList());

                foreach (var item in listado)
                {
                    item.UbicacionesLotes = codigoU.ContainsKey(item.CodigoUbicacion)
                        ? codigoU[item.CodigoUbicacion]
                             .Select(m => new UbicacionesLotes_E
                             {
                                 CodigoUbicacion = m.CodigoUbicacion,
                                 ItemCode = m.ItemCode,
                                 ItemName = m.ItemName,
                                 BatchNum = m.BatchNum,
                                 QuantityUnidadesCajas = m.QuantityUnidadesCajas
                             }).ToList()
                        : new List<UbicacionesLotes_E>(); // Si no hay lotes, asignar lista vacía
                }

                var nuevaLista = listado.GroupJoin(
                                                    listaULM,
                                                    ub => ub.CodigoUbicacion,  // Clave en listado
                                                    ulm => ulm.CodigoUbicacion, // Clave en listaULM
                                                    (ub, ulmGroup) => new
                                                    {
                                                        CodigoUbicacion = ub.CodigoUbicacion,
                                                        Lotes = ulmGroup.Any() ? ulmGroup : new List<UbicacionesLotesMaster_E> { new UbicacionesLotesMaster_E() } // Si no tiene lotes, agrega una fila vacía
                                                    }
                                                )
                                                .SelectMany(grupo => grupo.Lotes.Select(ulm => new
                                                {
                                                    CodigoUbicacion = grupo.CodigoUbicacion,
                                                    CodigoArticulo = ulm.ItemCode ?? "",
                                                    Descripcion = ulm.ItemName ?? "",
                                                    Lote = ulm.BatchNum ?? "",
                                                    QuantityMaster = ulm?.QuantityMaster ?? 0,
                                                    QuantitySaldo = ulm?.QuantitySaldo ?? 0,
                                                    CantidadUnidadesCajas = ulm.QuantityUnidadesCajas
                                                }))
                                                .OrderByDescending(x => !string.IsNullOrEmpty(x.Lote))  // Primero los que tienen Lote
                                                .ThenBy(x => x.CodigoUbicacion)   // Luego ordena alfabéticamente por CódigoUbicacion
                                                .ToList();

                if (nuevaLista != null && nuevaLista.Any())
                {
                    using (var libro = new ExcelPackage())
                    {
                        var worksheet = libro.Workbook.Worksheets.Add("ReporteUbicaciones_RESERVA");
                        worksheet.Cells["A1"].LoadFromCollection(nuevaLista, PrintHeaders: true);
                        for (var col = 1; col <= columnas; col++)
                        {
                            worksheet.Column(col).AutoFit();
                        }
                        var tabla = worksheet.Tables.Add(new ExcelAddressBase(fromRow: 1, fromCol: 1, toRow: nuevaLista.Count() + 1, toColumn: columnas), "ReporteUbicaciones_RESERVA");
                        tabla.ShowHeader = true;
                        tabla.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;
                        string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(libro.GetAsByteArray(), excelContentType, "ReporteUbicaciones_RESERVA.xlsx");
                    }
                }
                else { return Content("No hay datos para exportar"); }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        /************************* T R A N S F E R E N C I A S *************************/
        public ActionResult Transferencias(int idOperation = 3300)
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
        public JsonResult BuscarSolicitudDeTraslado(int docNum, int idOperation = 3301)
        {
            try
            {
                var resultadoAcceso = VerificarPermiso(idOperation);
                if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                {
                    return Json(new
                    {
                        Titulo = "Sesión expirada o acceso denegado",
                        Mensajes = new List<string> { "Debe iniciar sesión nuevamente o no tiene permisos." },
                        Icono = "error"
                    }, JsonRequestBehavior.AllowGet);
                }

                Utilitarios uti = new Utilitarios();
                SolicitudesTraslado_E traslado = null;
                TransferenciaReserva_E transferencia = null;

                traslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(docNum, null)
                    ?? _solicitudTrasladoHanaN.BuscarSolicitudDeTraslado(docNum);

                using (var scope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                    {
                        cn.Open();
                        if (traslado == null)
                        {
                            return Json(new
                            {
                                Titulo = "No se pudo completar la acción",
                                Mensajes = new List<string> { "No existe ningún resultado." },
                                Icono = "error"
                            }, JsonRequestBehavior.AllowGet);
                        }

                        if (traslado?.Id > 0)
                        {
                            if (traslado.Detalle != null)
                            {
                                transferencia = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum, cn);
                                if (transferencia == null)
                                {
                                    return Json(new
                                    {
                                        Titulo = "No se pudo completar la acción",
                                        Mensajes = new List<string> { "No existe resultado de transferencia relacionado a esta solicitud." },
                                        Icono = "error"
                                    }, JsonRequestBehavior.AllowGet);
                                }
                                traslado.Detalle = traslado.Detalle.OrderBy(d => d.Value.Estado != "PENDIENTE")
                                                                     .ToDictionary(d => d.Key, d => d.Value);
                            }
                        }
                        scope.Complete();
                    }
                }

                return Json(new { traslado, transferencia }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error inesperado",
                    Mensajes = new List<string> { ex.Message },
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult BuscarUbicaciones(string almacen, string itemCode = "", int idOperation = 3302)
        {
            try
            {
                var resultadoAcceso = VerificarPermiso(idOperation);
                if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                {
                    return Json(new
                    {
                        Titulo = "Acceso denegado",
                        Mensajes = new List<string> { "Debe iniciar sesión nuevamente o no tiene permisos." },
                        Icono = "error"
                    }, JsonRequestBehavior.AllowGet);
                }

                var listadoString = new List<string>();
                var listadoUbicaciones = _ubicacionesN.ListarUbicaciones(new Ubicaciones_E { Almacen = almacen });
                foreach (var i in listadoUbicaciones)
                {
                    listadoString.Add(i.CodigoUbicacion);
                }

                var ubicacionesDefault = _ubicacionesLotesN.Obtener(itemCode)
                    ?.Where(u => u.Almacen == "PICKING")
                    .ToList() ?? new List<UbicacionesLotes_E>();

                return Json(new
                {
                    resultUbicaciones = listadoString,
                    ubicacionesDefault
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error inesperado",
                    Mensajes = new List<string> { ex.Message },
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ImportarTransferenciaDeStock(HttpPostedFileBase file, int idOperation = 3303)
        {
            try
            {
                var resultadoAcceso = VerificarPermiso(idOperation);
                if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                {
                    return Json(new
                    {
                        Titulo = "Sesión expirada o acceso denegado",
                        Mensajes = new List<string> { "Debe iniciar sesión nuevamente o no tiene permisos." },
                        Icono = "error"
                    }, JsonRequestBehavior.AllowGet);
                }

                if (file == null || file.ContentLength == 0)
                {
                    return Json(new
                    {
                        Titulo = "Archivo inválido",
                        Mensajes = new List<string> { "No se ha seleccionado un archivo válido." },
                        Icono = "error"
                    }, JsonRequestBehavior.AllowGet);
                }


                Utilitarios uti = new Utilitarios();
                string rutaRespaldo = Path.Combine(uti.directorioFileServer, "ImportacionTransferencias");

                //Respaldar transferencias
                if (!Directory.Exists(rutaRespaldo))
                    Directory.CreateDirectory(rutaRespaldo);

                // Nombre único para evitar sobreescritura, por ejemplo con fecha y hora
                string nombreArchivo = $"TransferenciaStock_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}_{Path.GetFileName(file.FileName)}";

                // Ruta completa donde se guardará el archivo
                string rutaCompletaArchivo = Path.Combine(rutaRespaldo, nombreArchivo);
                using (var stream = file.InputStream)
                {
                    SLDocument sld = new SLDocument(stream);
                    if (!sld.GetSheetNames().Contains("CABECERA"))
                    {
                        return Json(new
                        {
                            Titulo = "Formato inválido",
                            Mensajes = new List<string> { "La hoja 'CABECERA' no existe en el archivo." },
                            Icono = "error"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    sld.SelectWorksheet("CABECERA");
                    // Leer todos los identificadores de CABECERA
                    int iRow = 10;
                    List<SolicitudesTraslado_E> solicitudesTraslado = new List<SolicitudesTraslado_E>();
                    List<TransferenciaReserva_E> transferencias = new List<TransferenciaReserva_E>();
                    while (!string.IsNullOrWhiteSpace(sld.GetCellValueAsString(iRow, 2)))
                    {
                        solicitudesTraslado.Add(new SolicitudesTraslado_E
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
                        });
                        transferencias.Add(new TransferenciaReserva_E
                        {
                            IdentificadorExcel = sld.GetCellValueAsInt32(iRow, 2),
                            CardCode = sld.GetCellValueAsString(iRow, 4),
                            CardName = sld.GetCellValueAsString(iRow, 5),
                            NroGuia = string.IsNullOrWhiteSpace(sld.GetCellValueAsString(iRow, 6)) ? null : sld.GetCellValueAsString(iRow, 6),
                            Detalle = new List<DetalleTransferenciaReserva_E>()
                        });
                        iRow++;
                    }
                    if (!sld.GetSheetNames().Contains("CUERPO"))
                    {
                        return Json(new
                        {
                            Titulo = "Formato inválido",
                            Mensajes = new List<string> { "La hoja 'CUERPO' no existe en el archivo." },
                            Icono = "error"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    sld.SelectWorksheet("CUERPO");
                    iRow = 10;
                    while (!string.IsNullOrWhiteSpace(sld.GetCellValueAsString(iRow, 1)))
                    {
                        int idDetalle = sld.GetCellValueAsInt32(iRow, 1);
                        if (idDetalle == 0)
                        {
                            iRow++;
                            continue; // Saltar filas inválidas
                        }
                        var solicitudTraslado = solicitudesTraslado.FirstOrDefault(r => r.DocNum == idDetalle);
                        var transferencia = transferencias.FirstOrDefault(r => r.IdentificadorExcel == idDetalle);
                        if (solicitudTraslado != null)
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
                            string uniqueKey = $"{detalleSolicitudTraslado.ItemCode}_{detalleSolicitudTraslado.BatchNum}";
                            if (!solicitudTraslado.Detalle.ContainsKey(uniqueKey))
                            {
                                solicitudTraslado.Detalle[uniqueKey] = detalleSolicitudTraslado;
                            }
                            else
                            {
                                solicitudTraslado.Detalle[uniqueKey].QuantityCajas += detalleSolicitudTraslado.QuantityCajas;
                            }
                        }
                        if (transferencia != null)
                        {
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
                            transferencia.Detalle.Add(detalleTransferencia);
                        }
                        iRow++;
                    }
                    // Registrar cada transferencia de forma independiente
                    List<string> errores = new List<string>();
                    for (int i = 0; i < solicitudesTraslado.Count; i++)
                    {
                        var solicitudTraslado = solicitudesTraslado[i];
                        var transferencia = transferencias[i];
                        var resultado = RegistrarTransferenciaDeStock(solicitudTraslado, transferencia);
                        if (resultado is JsonResult jsonResultado)
                        {
                            var data = jsonResultado.Data as dynamic;
                            if (data != null && data.Icono == "error")
                            {
                                errores.Add($"Error  al registrar Documento {solicitudTraslado.DocNum}: {data.Mensajes[0]}");
                            }
                        }
                        else
                        {
                            errores.Add($"Error inesperado al registrar Documento {solicitudTraslado.DocNum}.");
                        }
                    }
                    if (errores.Count == 0)
                        file.SaveAs(rutaCompletaArchivo);

                    // Responder según el resultado
                    return errores.Count > 0
                        ? Json(new { Titulo = "Errores en la importación", Mensajes = errores, Icono = "warning" }, JsonRequestBehavior.AllowGet)
                        : Json(new { Titulo = "Importación exitosa", Mensajes = new List<string> { "Todos los traslados fueron importados correctamente." }, Icono = "success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error en la importación",
                    Mensajes = new List<string> { ex.Message },
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult RegistrarTransferenciaDeStock(SolicitudesTraslado_E solicitudTraslado, TransferenciaReserva_E transferenciaPost, int idOperation = 3304)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Error de accesos." }, Icono = "error" });

            try
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                if (user == null)
                    return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "No existe usuario logueado, se terminó la sesión." }, Icono = "error" });

                if (transferenciaPost != null)
                {
                    if (transferenciaPost.Detalle == null)
                        return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Verificar el detalle del documento." }, Icono = "error" });

                    var ubicacionesReserva = transferenciaPost.Detalle
                        .SelectMany(d => new[] { d.CodigoUbicacion })
                        .Distinct()
                        .ToList();

                    foreach (var u in ubicacionesReserva)
                    {
                        bool resultValidarUbicaciones = _ubicacionesN.BuscarUbicacion("RESERVA", u);
                        if (!resultValidarUbicaciones)
                            return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { $"Revise que exista previamente la ubicación en Reserva {u}" }, Icono = "error" });
                    }

                    Utilitarios uti = new Utilitarios();

                    // Iniciar la transacción global para las operaciones críticas
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
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
                                if (resultImportarSolicitud.Icono.Equals("error") || resultImportarSolicitud.Id == 0)
                                    return Json(new { Titulo = "No se pudo completar la acción", resultImportarSolicitud.Mensajes, Icono = "error" });

                                //Asigna su Id porque ya fue insertado
                                solicitudTraslado.Id = resultImportarSolicitud.Id;
                            }

                            // Validar o inserta los lotes de registro sanitario (fuera de la transacción)
                            var resultLotes = _lotesRegistroSanitarioN.ValidarLotesRegistroSanitario(solicitudTraslado.Detalle, cn);
                            if (resultLotes.Icono.Equals("error"))
                                return Json(new { Titulo = "No se pudo completar la acción", resultLotes.Mensajes, Icono = resultLotes.Icono });

                            // Asignar datos de traslado a la transferencia, preparando para registrar  agregar lineas a la transferencia
                            transferenciaPost.OperarioRegistra = $"{user.Nombres} {user.Apellidos}";
                            transferenciaPost.SolicitudTrasladoId = solicitudTraslado.Id;
                            transferenciaPost.SolicitudTrasladoDocNum = solicitudTraslado.DocNum;

                            // Registrar  o agrega mas lineas al detalle de la transferencia de reserva
                            var resultTransferenciaGet = _transferenciaReservaN.RegistrarTransferenciaReserva(transferenciaPost, cn);
                            if (resultTransferenciaGet == null || resultTransferenciaGet.Id == 0)
                            {
                                if (resultTransferenciaGet.Icono.Equals("error"))
                                {
                                    // Validar y eliminar la solicitud de traslado si en caso se importo a la tabla interna pero no se ha encontrado una transferencia
                                    _solicitudTrasladoN.DeleteSolicitudDeTraslado(solicitudTraslado.DocNum, cn);
                                    return Json(new { Titulo = "No se pudo completar la acción", resultTransferenciaGet.Mensajes, Icono = resultTransferenciaGet.Icono });
                                }
                            }

                            //Actualiza a TRANSFERIDO en el DetalleDeSolicitudTraslado los ItemCode(s) que se hayan enviado para TransferenciaReserva
                            TransferenciaReserva_E transferencia = _transferenciaReservaN.ObtenerTransferenciaReserva(transferenciaPost.SolicitudTrasladoDocNum, cn);

                            var resultActualizarEstado = _solicitudTrasladoN.ActualizarEstado(transferencia.SolicitudTrasladoId, transferencia.Detalle, cn);
                            if (resultActualizarEstado.Icono.Equals("error"))
                                return Json(new { Titulo = "No se pudo completar la acción", resultActualizarEstado.Mensajes, Icono = resultActualizarEstado.Icono });

                            scope.Complete();
                        }
                    }
                    // Devolver respuesta exitosa
                    return Json(new { Titulo = "Acción completada exitosamente", Mensajes = new List<string> { "Se registró la Transferencia Reserva correctamente." }, Icono = "success" });
                }
                else
                {
                    return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "El documento que trata de registrar no tiene una transferencia realizandose." }, Icono = "error" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { $"Ocurrió un error al registrar la transferencia: {ex.Message}" }, Icono = "error" });
            }
        }
        public JsonResult CancelarTransferenciaYTraslado(int docNum, int idOperation = 3305) //recibe el docnum de la solicitud de traslado
        {
            try
            {
                var resultadoAcceso = VerificarPermiso(idOperation);
                if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                {
                    return Json(new
                    {
                        Titulo = "Sesión expirada o acceso denegado",
                        Mensajes = new List<string> { "Debe iniciar sesión nuevamente o no tiene permisos suficientes." },
                        Icono = "error"
                    }, JsonRequestBehavior.AllowGet);
                }

                if (docNum <= 0)
                {
                    return Json(new
                    {
                        Titulo = "Error en la operación",
                        Mensajes = new List<string> { "El docNum es inválido." },
                        Icono = "error"
                    }, JsonRequestBehavior.AllowGet);
                }

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
                            }, JsonRequestBehavior.AllowGet);
                        }

                        var transferenciaSinKardex = transferenciaGet.Detalle.Where(x => x.AtendidoReserva == 0).ToList();
                        var transferenciaConKardex = new TransferenciaReserva_E
                        {
                            Detalle = transferenciaGet.Detalle.Where(x => x.AtendidoReserva == 1 && x.Validado == 1).ToList()
                        };

                        var resultEliminarItems = _transferenciaReservaN.DeleteDetalleItemTransferenciaReserva(transferenciaSinKardex, cn);
                        if (resultEliminarItems.Icono.Equals("error"))
                            return Json(resultEliminarItems, JsonRequestBehavior.AllowGet);

                        var resultUbicacionesLotesMaster = _ubicacionesLotesMasterN.RevertirIngreso(transferenciaConKardex, cn);
                        if (resultUbicacionesLotesMaster.Icono.Equals("error"))
                            return Json(resultUbicacionesLotesMaster, JsonRequestBehavior.AllowGet);

                        var resultUbicacionesLotes = _ubicacionesLotesN.RevertirIngreso(transferenciaConKardex, cn);
                        if (resultUbicacionesLotes.Icono.Equals("error"))
                            return Json(resultUbicacionesLotes, JsonRequestBehavior.AllowGet);

                        var resultKardex = _kardexAbastecimientoN.EliminarTotalTransaccionesIngresoKardex(docNum, cn);
                        if (resultKardex.Icono.Equals("error"))
                            return Json(resultKardex, JsonRequestBehavior.AllowGet);

                        var resultTransferencia = _transferenciaReservaN.DeleteTransferenciaReserva(docNum, cn);
                        if (resultTransferencia.Icono.Equals("error"))
                            return Json(resultTransferencia, JsonRequestBehavior.AllowGet);

                        var resultSolicitudTraslado = _solicitudTrasladoN.DeleteSolicitudDeTraslado(docNum, cn);
                        if (resultSolicitudTraslado.Icono.Equals("error"))
                            return Json(resultSolicitudTraslado, JsonRequestBehavior.AllowGet);

                        cn.Close();
                    }

                    scope.Complete();
                }

                return Json(new
                {
                    Titulo = "Acción completada exitosamente",
                    Mensajes = new List<string> { "Se canceló la Transferencia Reserva y Solicitud de Traslado correctamente." },
                    Icono = "success"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { ex.Message },
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult RevertirTransferenciaReservaPorItem(int docNum, string itemCode, int idOperation = 3306) //recibe el docnum de la solicitud de traslado y el ItemCode a revertir
        {
            try
            {
                var resultadoAcceso = VerificarPermiso(idOperation);
                if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                {
                    return Json(new
                    {
                        Titulo = "Sesión expirada o acceso denegado",
                        Mensajes = new List<string> { "Debe iniciar sesión nuevamente o no tiene permisos suficientes." },
                        Icono = "error"
                    }, JsonRequestBehavior.AllowGet);
                }

                if (docNum <= 0)
                {
                    return Json(new
                    {
                        Titulo = "Error en la operación",
                        Mensajes = new List<string> { "El docNum es inválido." },
                        Icono = "error"
                    }, JsonRequestBehavior.AllowGet);
                }

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
                            }, JsonRequestBehavior.AllowGet);
                        }

                        var traslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(docNum, cn);

                        if (transferenciaGet.Detalle != null)
                        {
                            transferenciaGet.Detalle = transferenciaGet.Detalle
                                .Where(x => x.ItemCode == itemCode)
                                .ToList();
                        }

                        if (traslado.Detalle != null && transferenciaGet.Detalle != null)
                        {
                            traslado.Detalle = traslado.Detalle
                                .Where(kv => kv.Value.ItemCode == itemCode)
                                .ToDictionary(kv => kv.Key, kv => kv.Value);

                            int quantityCajasItemCode = Convert.ToInt32(traslado.Detalle.Sum(x => x.Value.QuantityCajas));

                            if (quantityCajasItemCode != transferenciaGet.Detalle.Where(x => x.ItemCode == itemCode).Sum(x => x.QuantityUnidadesCajas))
                            {
                                return Json(new
                                {
                                    Titulo = "No se pudo completar la acción",
                                    Mensajes = new List<string> { $"La suma de cantidades a revertir no coincide con el total en la solicitud de traslado para el SKU: {transferenciaGet.Detalle[0].ItemCode}" },
                                    Icono = "error"
                                }, JsonRequestBehavior.AllowGet);
                            }

                            var resultUbicacionesLotesMaster = _ubicacionesLotesMasterN.RevertirIngreso(transferenciaGet, cn);
                            if (resultUbicacionesLotesMaster.Icono.Equals("error"))
                            {
                                return Json(resultUbicacionesLotesMaster, JsonRequestBehavior.AllowGet);
                            }

                            var resultUbicacionesLotes = _ubicacionesLotesN.RevertirIngreso(transferenciaGet, cn);
                            if (resultUbicacionesLotes.Icono.Equals("error"))
                            {
                                return Json(resultUbicacionesLotes, JsonRequestBehavior.AllowGet);
                            }

                            var resultKardex = _kardexAbastecimientoN.EliminarPorItemCodeTransaccionIngresoKardex(docNum, transferenciaGet.Detalle[0].ItemCode, cn);
                            if (resultKardex.Icono.Equals("error"))
                            {
                                return Json(resultKardex, JsonRequestBehavior.AllowGet);
                            }

                            var resultTransferencia = _transferenciaReservaN.DeleteDetalleItemTransferenciaReserva(transferenciaGet.Detalle, cn);
                            if (resultTransferencia.Icono.Equals("error"))
                            {
                                return Json(resultTransferencia, JsonRequestBehavior.AllowGet);
                            }

                            var transferenciaPostReversion = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum, cn);
                            if (transferenciaPostReversion != null && transferenciaPostReversion.Detalle.Count() == 0)
                            {
                                var resultEliminarTransferencia = _transferenciaReservaN.DeleteTransferenciaReserva(docNum, cn);
                                if (resultEliminarTransferencia.Icono.Equals("error"))
                                {
                                    return Json(resultEliminarTransferencia, JsonRequestBehavior.AllowGet);
                                }

                                var resultSolicitudTraslado = _solicitudTrasladoN.DeleteSolicitudDeTraslado(docNum, cn);
                                if (resultSolicitudTraslado.Icono.Equals("error"))
                                {
                                    return Json(resultSolicitudTraslado, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }

                        cn.Close();
                    }

                    scope.Complete();
                }

                return Json(new
                {
                    Titulo = "Acción completada exitosamente",
                    Mensajes = new List<string> { "Se canceló la Transferencia Reserva y Solicitud de Traslado correctamente." },
                    Icono = "success"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { ex.Message },
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteItemTransferencia(int docNum, string itemCode, int idOperation = 3307) //recibe el docnum de la solicitud de traslado y el ItemCode a eliminar
        {
            try
            {
                var resultadoAcceso = VerificarPermiso(idOperation);
                if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                {
                    return Json(new
                    {
                        Titulo = "Sesión expirada o acceso denegado",
                        Mensajes = new List<string> { "Debe iniciar sesión nuevamente o no tiene permisos suficientes." },
                        Icono = "error"
                    }, JsonRequestBehavior.AllowGet);
                }

                if (docNum <= 0)
                {
                    return Json(new
                    {
                        Titulo = "Error en la operación",
                        Mensajes = new List<string> { "El docNum es inválido." },
                        Icono = "error"
                    }, JsonRequestBehavior.AllowGet);
                }

                using (var scope = new TransactionScope(TransactionScopeOption.Required,
                         new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                         TransactionScopeAsyncFlowOption.Enabled))
                {
                    Utilitarios uti = new Utilitarios();
                    using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                    {
                        cn.Open();

                        var traslado = _solicitudTrasladoN.ObtenerSolicitudDeTraslado(docNum, cn).Detalle
                            .Where(kv => kv.Value.ItemCode == itemCode)
                            .ToDictionary(kv => kv.Key, kv => kv.Value);

                        if (traslado == null || traslado.Count == 0)
                        {
                            return Json(new
                            {
                                Titulo = "No se pudo completar la acción",
                                Mensajes = new List<string> { "No se encontró la solicitud de traslado original." },
                                Icono = "error"
                            }, JsonRequestBehavior.AllowGet);
                        }

                        var transferencia = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum, cn).Detalle
                            .Where(x => x.ItemCode == itemCode).ToList();

                        if (transferencia == null || transferencia.Count == 0)
                        {
                            return Json(new
                            {
                                Titulo = "No se pudo completar la acción",
                                Mensajes = new List<string> { "No se encontró transferencia de reserva relacionada." },
                                Icono = "error"
                            }, JsonRequestBehavior.AllowGet);
                        }

                        var resultTransferencia = _transferenciaReservaN.DeleteDetalleItemTransferenciaReserva(transferencia, cn);
                        if (resultTransferencia.Icono.Equals("error"))
                        {
                            return Json(new
                            {
                                Titulo = "No se pudo completar la acción",
                                resultTransferencia.Mensajes,
                                Icono = resultTransferencia.Icono
                            }, JsonRequestBehavior.AllowGet);
                        }

                        var transferenciaPostReversion = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum, cn);
                        if (transferenciaPostReversion != null && transferenciaPostReversion.Detalle.Count == 0)
                        {
                            var resultEliminarTransferencia = _transferenciaReservaN.DeleteTransferenciaReserva(docNum, cn);
                            if (resultEliminarTransferencia.Icono.Equals("error"))
                            {
                                return Json(new
                                {
                                    Titulo = "No se pudo completar la acción",
                                    resultEliminarTransferencia.Mensajes,
                                    Icono = resultEliminarTransferencia.Icono
                                }, JsonRequestBehavior.AllowGet);
                            }

                            var resultSolicitudTraslado = _solicitudTrasladoN.DeleteSolicitudDeTraslado(docNum, cn);
                            if (resultSolicitudTraslado.Icono.Equals("error"))
                            {
                                return Json(new
                                {
                                    Titulo = "No se pudo completar la acción",
                                    resultSolicitudTraslado.Mensajes,
                                    Icono = resultSolicitudTraslado.Icono
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        cn.Close();
                    }

                    scope.Complete();
                }

                return Json(new
                {
                    Titulo = "Acción completada exitosamente",
                    Mensajes = new List<string> { "Se restablecieron los datos en la Transferencia Reserva." },
                    Icono = "success"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error en la operación",
                    Mensajes = new List<string> { ex.Message },
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ValidarItemParaApilar(int docNum, string itemCode, int idOperation = 3308) //recibe el docnum de la solicitud de traslado y el ItemCode a validar
        {
            try
            {
                var resultadoAcceso = VerificarPermiso(idOperation);
                if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                    return Json(new { Titulo = "Sesión expirada o acceso denegado", Mensajes = new List<string> { "Debe iniciar sesión nuevamente o no tiene permisos suficientes." }, Icono = "error" }, JsonRequestBehavior.AllowGet);

                if (docNum <= 0)
                    return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "El docNum es inválido." }, Icono = "error" }, JsonRequestBehavior.AllowGet);

                var transferenciaGet = new TransferenciaReserva_E();
                Utilitarios uti = new Utilitarios();

                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();
                    transferenciaGet = _transferenciaReservaN.ObtenerTransferenciaReserva(docNum, cn);
                    if (transferenciaGet == null || transferenciaGet.Id == 0)
                        return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "No se encontró transferencia de reserva relacionada." }, Icono = "error" }, JsonRequestBehavior.AllowGet);

                    var resultValidacion = _transferenciaReservaN.ValidarSkuParaApilar(transferenciaGet.Id, itemCode, cn);
                    if (resultValidacion.Icono.Equals("error"))
                        return Json(new { Titulo = "No se pudo completar la acción", resultValidacion.Mensajes, Icono = resultValidacion.Icono }, JsonRequestBehavior.AllowGet);

                    cn.Close();
                }

                if (transferenciaGet.Detalle?.Any(t => t.ItemCode == itemCode) == true)
                {
                    var detalleFiltrado = transferenciaGet.Detalle.Where(b => b.ItemCode == itemCode && b.CodigoUbicacion == "RESERVA-UBI-SISTEMA" && b.Validado == 0).ToList();

                    foreach (var det in detalleFiltrado)
                    {
                        var resultadoAtender = AtenderReservaTransferencia(det.Id, det.DocNumSolicitudTraslado, det.ItemCode, det.ItemName, det.CodigoUbicacion);
                        if (resultadoAtender is JsonResult jsonResultado)
                        {
                            var data = jsonResultado.Data as dynamic;
                            if (data != null && data.Icono == "error")
                            {
                                var resultRevertir = _transferenciaReservaN.RevertirValidarSkuParaApilar(transferenciaGet.Id, itemCode);
                                if (resultRevertir != null && resultRevertir.Icono.Equals("error"))
                                    return Json(resultRevertir, JsonRequestBehavior.AllowGet);

                                return Json(new { Titulo = "Error", Mensajes = new List<string> { $"{data.Mensajes[0]}" }, Icono = "error" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }

                return Json(new { Titulo = "Acción completada exitosamente", Mensajes = new List<string> { "Se validó correctamente detalle de transferencia para apilar." }, Icono = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { ex.Message }, Icono = "error" }, JsonRequestBehavior.AllowGet);
            }
        }
        /****************************** R E Q U E R I M I E N T O S ****************************/
        /****************************** A P I L A D O R E S ****************************/
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
        public ActionResult ListarArticulos(string tipoAbastecimiento, string itemCode, int cantidadSolicitada, int idOperation = 3401)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var usuarioSesion = Session["UsuarioId"] as Usuario_E;
                if (usuarioSesion == null)
                    return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);
                // Orden: próxima fecha de vencimiento, primera fecha de admisión registrada, la menor cantidad en unidades
                List<UbicacionesLotesMaster_E> lista = _ubicacionesLotesMasterN.BuscarArticulos(new UbicacionesLotesMaster_E { ItemCode = itemCode }) ?? new List<UbicacionesLotesMaster_E>();
                if (lista.Any())
                {
                    // Verificar si todas las fechas ExpDate e InDate son iguales
                    bool fechasIguales = lista.All(a => a.ExpDate == lista.First().ExpDate) && lista.All(a => a.InDate == lista.First().InDate);
                    // Aplicar el ordenamiento según la condición
                    lista = fechasIguales
                        ? lista.OrderBy(a => a.CodigoUbicacion != "RESERVA-UBI-SISTEMA").ThenBy(a => a.QuantityUnidadesCajas).ThenBy(a => a.CodigoUbicacion).ToList() // Ordenar por CodigoUbicacion si las fechas son iguales
                        : lista.OrderBy(a => DateTime.Parse(a.ExpDate))
                               .ThenBy(a => DateTime.Parse(a.InDate))
                               .ThenBy(a => a.QuantityUnidadesCajas)
                               .ToList();
                }
                // Limpiamos el espacio para mejor validación
                tipoAbastecimiento = tipoAbastecimiento.Replace(" ", "");
                switch (tipoAbastecimiento)
                {
                    case "Picking":
                        return PartialView("AbastecimientoInterno/_ListadoArticulosPicking", lista);
                    case "VentaMaster":
                        return PartialView("AbastecimientoInterno/_ListadoArticulosVenta", lista);
                    case "SalidaporAlmacen":
                        return PartialView("AbastecimientoInterno/_ListadoArticulosSalidaporAlmacen", lista);
                    default:
                        return HttpNotFound("No se encontró la vista para el tipo de abastecimiento especificado.");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public JsonResult CalcularCantidadSolicitada(string tipoAbastecimiento, string itemCode, int idOperation = 3402)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                long cantidadSolicitada = 0;
                bool stockMin = _stockMinProdN.Obtener(itemCode)?.Id > 0;

                if (tipoAbastecimiento != null && tipoAbastecimiento.Equals("Picking") && itemCode != null)
                {
                    //Calcular desde SAP (Stock Total - Stock Comprometido)  en Almacen 16 por defecto
                    int stockLibreEnAlmacen16 = Convert.ToInt32(new Capa_Negocio.Almacen_NEG.Tablas.OITW_N().ListarDetArticulosInv(new OITW_E { ItemCode = itemCode, WhsCode = "16" }).DefaultIfEmpty(new OITW_E { }).First().StockLibreUnidades);
                    if (stockLibreEnAlmacen16 > 0)
                    {
                        // Para ver los imputados
                        List<DetalleRequerimientos_E> resultDetReq = _requerimientosN.ListarDetalles(itemCode, "CantidadSolicitada");
                        int quantityReq = 0;
                        if (resultDetReq != null) { quantityReq = Convert.ToInt32(resultDetReq.Sum(r => r.QuantityUnidadesCajas)); }
                        List<UbicacionesLotes_E> resultUbicacionesLotes = _ubicacionesLotesN.Obtener(itemCode).Where(x => x.Almacen.Equals("RESERVA")).ToList();
                        int quantityUbicacionesLote = 0;
                        if (resultUbicacionesLotes != null) { quantityUbicacionesLote = resultUbicacionesLotes.Sum(r => r.QuantityUnidadesCajas); }
                        int stockDeAlmReserva = quantityUbicacionesLote - quantityReq; //resta de lo que esta por entrar a Picking Atendido=0
                        long stockEnPicking = stockLibreEnAlmacen16 - stockDeAlmReserva;
                        long stockMinimoParaLaVenta = _stockMinProdN.Obtener(itemCode).StockMinAbastecimiento;
                        cantidadSolicitada = stockMinimoParaLaVenta - stockEnPicking;

                        if (cantidadSolicitada < 0 || (stockEnPicking <= 0 && stockMinimoParaLaVenta <= 0)) cantidadSolicitada = 0;
                    }
                }
                return Json(new { cantidadSolicitada = Convert.ToString(cantidadSolicitada), tieneStockMin = stockMin == true ? "Y" : "N" });
            }
            else
            {
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Sin accesos." }, Icono = "error" });
            }
        }
        public JsonResult RegistrarRequerimiento(Requerimientos_E requerimiento, int idOperation = 3403)
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
                            return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "No existe usuario logueado, se terminó la sesión." }, Icono = "error" });

                        var listaProductosDisponibles = _productosDisponiblesReserva.ObtenerProductosDisponiblesReserva();
                        List<string> listMensajes = new List<string>();
                        foreach (var u in requerimiento.Detalle)
                        {
                            var productoDisp = listaProductosDisponibles.Where(x =>
                                x.ValorUmAlm > 0 && u.ValorUmAlm > 0 && x.ValorUmAlm == u.ValorUmAlm &&
                                x.ItemCode != null && u.ItemCode != null && x.ItemCode == u.ItemCode &&
                                x.CodigoUbicacionOrigen != null && u.CodigoUbicacionOrigen != null && x.CodigoUbicacionOrigen == u.CodigoUbicacionOrigen &&
                                x.BatchNum != null && u.BatchNum != null && x.BatchNum == u.BatchNum
                            );

                            if (productoDisp.Any())
                            {
                                if (u.QuantityMaster > productoDisp.First().DisponibleMaster || u.QuantitySaldo > productoDisp.First().DisponibleSaldo)
                                {
                                    listMensajes.Add($"{u.ItemCode} {u.BatchNum} {u.ValorUmAlm}  en {u.CodigoUbicacionOrigen}");
                                    listMensajes.Add($"Disponible: Master({productoDisp.First().DisponibleMaster}) y Saldo({productoDisp.First().DisponibleSaldo})");
                                }
                            }
                        }
                        if (listMensajes.Any())
                            return Json(new { Titulo = "No se pudo completar la acción", Mensajes = listMensajes, Icono = "error" });

                        //Validar que las ubicacion origen existan
                        var ubicacionesReserva = requerimiento.Detalle
                           .SelectMany(d => new[] { d.CodigoUbicacionOrigen })
                           .Distinct()
                           .ToList();

                        foreach (var u in ubicacionesReserva)
                        {
                            bool resultValidarUbicaciones = _ubicacionesN.BuscarUbicacion("RESERVA", u);
                            if (!resultValidarUbicaciones)
                                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { $"Revise que exista previamente la ubicación en Reserva {u}" }, Icono = "error" });

                        }
                        // Solo se requiere validar la ubicación PICKING para requerimientos de tipo abastecimiento: Picking y Salida por Almacen
                        if (requerimiento.TipoAbastecimiento == "Picking")
                        {
                            int nulos = requerimiento.Detalle.Where(d => d?.CodigoUbicacionDestino == null).ToList().Count;
                            if (nulos > 0)
                                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Las ubicaciones Picking deben estar definidas." }, Icono = "error" });


                            //Validar que las ubicacion destino insertadas, existan
                            var ubicacionesPicking = requerimiento.Detalle
                                .SelectMany(d => d.CodigoUbicacionDestino.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                .Select(ubicacion => ubicacion.Trim()) // Para eliminar espacios en blanco
                                .Distinct()
                                .ToList();

                            foreach (var u in ubicacionesPicking)
                            {
                                bool resultValidarUbicaciones = _ubicacionesN.BuscarUbicacion("PICKING", u);
                                if (!resultValidarUbicaciones)
                                    return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { $"Revise que exista previamente la ubicación en Picking {u}" }, Icono = "error" });
                            }
                        }
                        // Asignar datos de operario en el requerimiento
                        requerimiento.OperarioRegistra = $"{user.Nombres} {user.Apellidos}";
                        Utilitarios uti = new Utilitarios();

                        // Iniciar la transacción global para las operaciones críticas
                        using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                        {
                            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                            {
                                cn.Open();
                                var requerimientoGet = _requerimientosN.RegistrarRequerimiento(requerimiento, cn);
                                if (requerimientoGet == null || requerimientoGet.Id == 0)
                                    return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "No se completo el registro del requerimiento" }, Icono = "error" });

                                if (requerimiento.TipoAbastecimiento == "Picking")
                                {
                                    var resultCodUbiPicking = _ubicacionesLotesN.RegistrarCodigoUbicacionPicking(requerimientoGet.Detalle, cn);
                                    if (resultCodUbiPicking.Icono.Equals("error"))
                                        return Json(new { Titulo = "No se pudo completar la acción", resultCodUbiPicking.Mensajes, Icono = resultCodUbiPicking.Icono });
                                }

                                // Registrar la(s) operación(es) de imputado(s) en KardexAbastecimiento - Los datos a insertar son los del detalle en requerimiento, RequerimientoGet ya tiene los datos limpios por enviar hacia el kardex como imputado, previamente validados
                                var resultKardexImputar = _kardexAbastecimientoN.InsertarTransaccionImputadoKardex(requerimientoGet, cn);
                                if (resultKardexImputar.Icono.Equals("error"))
                                    return Json(new { Titulo = "No se pudo completar la acción", resultKardexImputar.Mensajes, Icono = resultKardexImputar.Icono });

                                // Apilamos automáticamente a SKUs que tiene como CodigoUbicacionDestino = "PICKING-UBI-SISTEMA" y los que faltan confirmar por el área
                                if (requerimientoGet.Detalle != null && requerimientoGet.Detalle.Any())
                                {
                                    foreach (var det in requerimientoGet.Detalle)
                                    {
                                        var buscarDetReq = _requerimientosN
                                            .ObtenerRequerimiento(det.RequerimientoId, cn)
                                            .Detalle
                                            .First(d => d.Id == det.Id);

                                        if (buscarDetReq != null)
                                        {
                                            // Apilamiento automático solo cuando la ubicación origen sea: RESERVA-UBI-SISTEMA y aúno no ha sido apilado ni confirmado por picking
                                            if (det.CodigoUbicacionOrigen == "RESERVA-UBI-SISTEMA" && buscarDetReq.AtendidoReserva == 0 && buscarDetReq.AtendidoPicking == 0)
                                            {
                                                var resultadoAtender = AtenderReservaRequerimiento(det);
                                                if (resultadoAtender is JsonResult jsonResultado)
                                                {
                                                    var data = jsonResultado.Data as dynamic;
                                                    if (data != null && data.Icono == "error")
                                                        return Json(new { Titulo = "Error", Mensajes = new List<string> { $"{data.Mensajes[0]}" }, Icono = "error" }, JsonRequestBehavior.AllowGet);
                                                }

                                                // Reabastecimiento automático solo cuando la ubicación destino sea: PICKING-UBI-SISTEMA
                                                if (det.CodigoUbicacionDestino == "PICKING-UBI-SISTEMA")
                                                {
                                                    var resultadoReabastecer = AtenderPickingRequerimiento(det.Id, requerimientoGet.Id, det.ItemCode, det.ItemName);
                                                    if (resultadoReabastecer is JsonResult jsonResultado2)
                                                    {
                                                        var data2 = jsonResultado2.Data as dynamic;
                                                        if (data2 != null && data2.Icono == "error")
                                                            return Json(new { Titulo = "Error", Mensajes = new List<string> { $"{data2.Mensajes[0]}" }, Icono = "error" }, JsonRequestBehavior.AllowGet);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            return Json(new { Titulo = "Error", Mensajes = new List<string> { "Verificar datos enviados." }, Icono = "error" }, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                }

                                // Confirmar la transacción
                                scope.Complete();
                            }
                        }
                        // Devolver respuesta exitosa
                        return Json(new { Titulo = "Acción completada exitosamente", Mensajes = new List<string> { "Se registró el requerimiento correctamente." }, Icono = "success" });
                    }
                    else
                    {
                        return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Envie un documento de requerimiento válido" }, Icono = "error" });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { ex.Message }, Icono = "error" });
                }
            }
            else
            {
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Sin accesos." }, Icono = "error" });
            }
        }
        public JsonResult ImportarRequerimiento(HttpPostedFileBase file, int idOperation = 3404)
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
                Utilitarios uti = new Utilitarios();
                string rutaRespaldo = Path.Combine(uti.directorioFileServer, "ImportacionRequerimientos");
                //Respaldar transferencias
                if (!Directory.Exists(rutaRespaldo))
                {
                    Directory.CreateDirectory(rutaRespaldo);
                }
                // Nombre único para evitar sobreescritura, por ejemplo con fecha y hora
                string nombreArchivo = $"Requerimiento_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}_{Path.GetFileName(file.FileName)}";
                // Ruta completa donde se guardará el archivo
                string rutaCompletaArchivo = Path.Combine(rutaRespaldo, nombreArchivo);
                using (var stream = file.InputStream)
                {
                    SLDocument sld = new SLDocument(stream);
                    // Validar existencia de la hoja CABECERA
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
                    // Leer todos los identificadores de CABECERA
                    int iRow = 10;
                    List<Requerimientos_E> requerimientos = new List<Requerimientos_E>();
                    while (!string.IsNullOrWhiteSpace(sld.GetCellValueAsString(iRow, 1)))
                    {
                        requerimientos.Add(new Requerimientos_E
                        {
                            IdentificadorExcel = sld.GetCellValueAsInt32(iRow, 1),
                            Origen = sld.GetCellValueAsString(iRow, 2),
                            Destino = sld.GetCellValueAsString(iRow, 3),
                            TipoAbastecimiento = sld.GetCellValueAsString(iRow, 4),
                            Detalle = new List<DetalleRequerimientos_E>()
                        });
                        iRow++;
                    }
                    // Validar existencia de la hoja CUERPO
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
                    // Leer y asignar detalles a los requerimientos según el identificador
                    iRow = 10;
                    while (!string.IsNullOrWhiteSpace(sld.GetCellValueAsString(iRow, 1)))
                    {
                        int idDetalle = sld.GetCellValueAsInt32(iRow, 1);
                        if (idDetalle == 0)
                        {
                            iRow++;
                            continue; // Saltar filas inválidas
                        }
                        var requerimiento = requerimientos.FirstOrDefault(r => r.IdentificadorExcel == idDetalle);
                        if (requerimiento != null)
                        {
                            var valorUmAlm = sld.GetCellValueAsInt32(iRow, 8);
                            var quantityMaster = sld.GetCellValueAsInt32(iRow, 9);
                            var quantitySaldo = sld.GetCellValueAsInt32(iRow, 10);
                            var detalleRequerimiento = new DetalleRequerimientos_E
                            {
                                IdentificadorExcel = idDetalle,
                                ItemCode = sld.GetCellValueAsString(iRow, 2),
                                ItemName = sld.GetCellValueAsString(iRow, 3),
                                BatchNum = sld.GetCellValueAsString(iRow, 4),
                                CodigoUbicacionOrigen = sld.GetCellValueAsString(iRow, 5),
                                CodigoUbicacionDestino = sld.GetCellValueAsString(iRow, 6),
                                UmAlm = sld.GetCellValueAsString(iRow, 7),
                                ValorUmAlm = valorUmAlm,
                                QuantityMaster = quantityMaster,
                                QuantitySaldo = quantitySaldo,
                                QuantityUnidadesCajas = (valorUmAlm * quantityMaster) + quantitySaldo
                            };
                            requerimiento.Detalle.Add(detalleRequerimiento);
                        }
                        iRow++;
                    }
                    // Registrar cada requerimiento de forma independiente
                    List<string> errores = new List<string>();
                    foreach (var req in requerimientos)
                    {
                        var resultado = RegistrarRequerimiento(req);
                        if (resultado is JsonResult jsonResultado)
                        {
                            var data = jsonResultado.Data as dynamic;
                            if (data != null && data.Icono == "error")
                            {
                                errores.Add($"Error en Requerimiento {req.IdentificadorExcel}: {data.Mensajes[0]}");
                            }
                        }
                        else
                        {
                            errores.Add($"Error inesperado al registrar Requerimiento {req.IdentificadorExcel}.");
                        }
                    }
                    if (errores.Count == 0)
                        file.SaveAs(rutaCompletaArchivo);
                    // Responder según el resultado
                    return errores.Count > 0
                        ? Json(new
                        {
                            Titulo = "Errores en la importación",
                            Mensajes = errores,
                            Icono = "warning"
                        })
                        : Json(new
                        {
                            Titulo = "Importación exitosa",
                            Mensajes = new List<string> { "Todos los requerimientos fueron importados correctamente." },
                            Icono = "success"
                        });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error en la importación",
                    Mensajes = new List<string> { "Ocurrió un error al procesar el requerimiento.", ex.Message },
                    Icono = "error"
                });
            }
        }
        public ActionResult ApilarRequerimientos(int idOperation = 3500)
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
        //Atendido de apiladores (Solo cambia el AtendidoReserva a 1), en caso el tipo de requerimiento sea "Venta Master" o "Salida Almacen", hara la modificacion de Kardex
        public JsonResult AtenderReservaRequerimiento(DetalleRequerimientos_E detalleRequerimiento, int idOperation = 3501)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                if (user == null)
                    return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "No existe usuario logueado, se terminó la sesión." }, Icono = "error" });

                if (detalleRequerimiento.Id <= 0 || detalleRequerimiento.RequerimientoId <= 0 || string.IsNullOrEmpty(detalleRequerimiento.ItemCode) || string.IsNullOrEmpty(detalleRequerimiento.ItemName))
                    return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Los datos enviados son inválidos." }, Icono = "error" });

                try
                {
                    Utilitarios uti = new Utilitarios();
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                        {
                            cn.Open();

                            string operarioRegistra = $"{user.Nombres} {user.Apellidos}";

                            var requerimiento = _requerimientosN.ObtenerRequerimiento(detalleRequerimiento.RequerimientoId, cn);
                            if (requerimiento == null || requerimiento.Detalle == null || !requerimiento.Detalle.Any())
                                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Se atendió el SKU correctamente y se generó kardex por salida." }, Icono = "error" });

                            var requerimientoPorSku = requerimiento.Detalle.Where(x => x.Id == detalleRequerimiento.Id && x.ItemCode == detalleRequerimiento.ItemCode && x.AtendidoReserva == 0).ToList();
                            int cantidadGlobal = requerimientoPorSku.Sum(x => x.QuantityUnidadesCajas ?? 0);

                            // Actualizar a AtendidoReserva 1 solo la linea de detalle enviada
                            var resultAtender = _requerimientosN.AtenderReserva(detalleRequerimiento.Id);
                            if (resultAtender.Icono.Equals("error"))
                                return Json(new { Titulo = "No se pudo completar la acción", resultAtender.Mensajes, Icono = resultAtender.Icono });

                            //// Si tiene más de un elemento con el mismo itemCode, agrupamos
                            //if (requerimientoPorSku != null && requerimientoPorSku.Count > 1)
                            //{
                            //    requerimientoPorSku.GroupBy(x => new { x.ItemCode })
                            //        .Select(g => new DetalleRequerimientos_E
                            //        {
                            //            ItemCode = g.Key.ItemCode,
                            //            CodigoUbicacionDestino = g.First().CodigoUbicacionDestino,
                            //            QuantityMaster = g.Sum(x => x.QuantityMaster ?? 0),
                            //            QuantitySaldo = g.Sum(x => x.QuantitySaldo ?? 0),
                            //            QuantityUnidadesCajas = g.Sum(x => x.QuantityUnidadesCajas ?? 0),
                            //            ItemName = g.First().ItemName,
                            //            UmAlm = g.First().UmAlm,
                            //            ValorUmAlm = g.First().ValorUmAlm,
                            //            BatchNum = g.First().BatchNum,
                            //            CodigoUbicacionOrigen = g.First().CodigoUbicacionOrigen,
                            //            AtendidoReserva = g.First().AtendidoReserva,
                            //            AtendidoPicking = g.First().AtendidoPicking,
                            //            RequerimientoId = g.First().RequerimientoId
                            //        });
                            //}

                            // Actualizar Ubicaciones Lotes Master
                            var resultUbicacionesLotesMaster = _ubicacionesLotesMasterN.Salida(requerimientoPorSku, cn);
                            if (resultUbicacionesLotesMaster.Icono.Equals("error"))
                                return Json(new { Titulo = "No se pudo completar la acción", resultUbicacionesLotesMaster.Mensajes, Icono = resultUbicacionesLotesMaster.Icono });

                            // Actualizar Ubicaciones Lotes
                            var resultUbicacionesLotes = _ubicacionesLotesN.Salida(requerimientoPorSku, cn);
                            if (resultUbicacionesLotes.Icono.Equals("error"))
                                return Json(new { Titulo = "No se pudo completar la acción", resultUbicacionesLotes.Mensajes, Icono = resultUbicacionesLotes.Icono });

                            // Registrar Kardex Salida
                            var resultKardex = _kardexAbastecimientoN.InsertarTransaccionSalidaKardex(detalleRequerimiento.ItemCode, detalleRequerimiento.ItemName, cantidadGlobal, operarioRegistra, detalleRequerimiento.RequerimientoId, cn);
                            if (resultKardex.Icono.Equals("error"))
                                return Json(new { Titulo = "No se pudo completar la acción", resultKardex.Mensajes, Icono = resultKardex.Icono });

                            // Confirmar transacción
                            scope.Complete();

                            return Json(new { Titulo = "Acción completada exitosamente", Mensajes = new List<string> { "Se atendió el SKU correctamente y se generó kardex por salida." }, Icono = "success" });
                        }
                    }

                    // 9. Retornar éxito solo con atención de SKU
                    return Json(new { Titulo = "Acción completada exitosamente", Mensajes = new List<string> { "Se atendió el SKU correctamente." }, Icono = "success" });
                }
                catch (Exception ex)
                {
                    return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { ex.Message }, Icono = "error" });
                }
            }
            else
            {
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Sin accesos." }, Icono = "error" });
            }
        }
        public ActionResult ApilarIngreso(int idOperation = 3502)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _transferenciaReservaN.ListarDetalles();
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        //Atendido de apiladores (Solo cambia el AtendidoReserva a 1) en detalle de Transferencia
        public JsonResult AtenderReservaTransferencia(int detalleId, int docNumSolicitudTraslado, string itemCode, string itemName, string codigoUbicacion, int idOperation = 3503)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Sin accesos." }, Icono = "error" });

            if (detalleId <= 0 || docNumSolicitudTraslado <= 0 || string.IsNullOrEmpty(itemCode) || string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(codigoUbicacion))
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Los datos enviados son inválidos." }, Icono = "error" });

            try
            {
                var usuario = (Usuario_E)Session["UsuarioId"];
                if (usuario == null)
                    return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "No existe usuario logueado, se terminó la sesión." }, Icono = "error" });

                var listaUbicaciones = _ubicacionesN.ListarUbicaciones(new Ubicaciones_E { Almacen = "RESERVA" });
                if (!listaUbicaciones.Any(u => u.CodigoUbicacion == codigoUbicacion))
                    return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Revise que la ubicación exista en el sistema." }, Icono = "error" });

                Utilitarios uti = new Utilitarios();
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                    {
                        cn.Open();

                        // Actualizar AtendidoReserva=1
                        var resultAtender = _transferenciaReservaN.AtenderReserva(detalleId, cn);
                        if (resultAtender == null || !resultAtender.Icono.Equals("success"))
                            return Json(new { Titulo = "Error al atender reserva", resultAtender?.Mensajes, Icono = resultAtender?.Icono ?? "error" });

                        // Obtener la transferencia
                        var transferencia = _transferenciaReservaN.ObtenerTransferenciaReserva(docNumSolicitudTraslado, cn);

                        // Validar si todos los items del SKU están atendidos
                        bool listoKardexIngreso = _transferenciaReservaN.ValidarSkuParaKardexIngreso(docNumSolicitudTraslado, itemCode, transferencia);
                        if (listoKardexIngreso)
                        {
                            var operarioRegistra = $"{usuario.Nombres} {usuario.Apellidos}";
                            var transferenciaPorSku = transferencia.Detalle.Where(x => x.ItemCode == itemCode).ToList();
                            int cantidadGlobal = Convert.ToInt32(transferenciaPorSku.Sum(x => x.QuantityUnidadesCajas));

                            // Registrar Kardex Ingreso
                            var resultKardex = _kardexAbastecimientoN.InsertarTransaccionIngresoKardex(itemCode, itemName, cantidadGlobal, operarioRegistra, docNumSolicitudTraslado, transferencia.CardCode, transferencia.CardName, cn);

                            if (!resultKardex.Icono.Equals("success"))
                                return Json(new { Titulo = "No se pudo completar la acción", resultKardex.Mensajes, Icono = resultKardex.Icono });

                            // Registrar en UbicacionesLotes y UbicacionesLotesMaster
                            foreach (var item in transferenciaPorSku)
                            {
                                var resultUbicacionesLotes = _ubicacionesLotesN.Ingreso(item, cn);
                                if (!resultUbicacionesLotes.Icono.Equals("success"))
                                    return Json(new { Titulo = "No se pudo completar la acción", resultUbicacionesLotes.Mensajes, Icono = resultUbicacionesLotes.Icono });

                                var resultUbicacionesLotesMaster = _ubicacionesLotesMasterN.Ingreso(resultUbicacionesLotes.Id, item, cn);
                                if (!resultUbicacionesLotesMaster.Icono.Equals("success"))
                                    return Json(new { Titulo = "No se pudo completar la acción", resultUbicacionesLotesMaster.Mensajes, Icono = resultUbicacionesLotesMaster.Icono });
                            }

                            // Confirmar transacción con todo y Kardex
                            scope.Complete();
                            return Json(new { Titulo = "Acción completada exitosamente", Mensajes = new List<string> { "Se atendió el SKU correctamente y se generó kardex por ingreso." }, Icono = "success" });
                        }

                        // Confirmar transacción sin Kardex
                        scope.Complete();
                    }
                }
                return Json(new { Titulo = "Acción completada exitosamente", Mensajes = new List<string> { "Se atendió el SKU correctamente." }, Icono = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { ex.Message }, Icono = "error" });
            }
        }

        [HttpPost]
        public JsonResult AtenderReservaMasiva(List<ReservaTransferenciaMasiva_E> solicitudes, int idOperation = 3503)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
            {
                return Json(new
                {
                    Titulo = "Acceso denegado",
                    Mensajes = new List<string> { "No tienes permisos para realizar esta operación." },
                    Icono = "error"
                });
            }

            if (solicitudes == null || !solicitudes.Any())
            {
                return Json(new
                {
                    Titulo = "Datos inválidos",
                    Mensajes = new List<string> { "No se encontraron solicitudes a procesar." },
                    Icono = "error"
                });
            }

            try
            {
                var usuario = (Usuario_E)Session["UsuarioId"];
                if (usuario == null)
                {
                    return Json(new
                    {
                        Titulo = "Sesión finalizada",
                        Mensajes = new List<string> { "Debes iniciar sesión nuevamente." },
                        Icono = "error"
                    });
                }

                var listaUbicaciones = _ubicacionesN.ListarUbicaciones(new Ubicaciones_E { Almacen = "RESERVA" });
                Utilitarios uti = new Utilitarios();

                using (var scope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                    TransactionScopeAsyncFlowOption.Enabled))
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();

                    foreach (var solicitud in solicitudes)
                    {
                        if (solicitud?.Detalles == null || !solicitud.Detalles.Any())
                        {
                            return Json(new
                            {
                                Titulo = "Datos inválidos",
                                Mensajes = new List<string> { "Una de las solicitudes no tiene detalles válidos." },
                                Icono = "error"
                            });
                        }

                        foreach (var detalle in solicitud.Detalles)
                        {
                            if (detalle.DetalleId <= 0 || string.IsNullOrEmpty(detalle.ItemCode) ||
                                string.IsNullOrEmpty(detalle.ItemName) || string.IsNullOrEmpty(detalle.CodigoUbicacion))
                            {
                                return Json(new
                                {
                                    Titulo = "Error en datos",
                                    Mensajes = new List<string> { "Uno de los ítems contiene información inválida." },
                                    Icono = "error"
                                });
                            }

                            if (!listaUbicaciones.Any(u => u.CodigoUbicacion == detalle.CodigoUbicacion))
                            {
                                return Json(new
                                {
                                    Titulo = "Ubicación inválida",
                                    Mensajes = new List<string> { $"La ubicación {detalle.CodigoUbicacion} no existe." },
                                    Icono = "error"
                                });
                            }

                            var resultAtender = _transferenciaReservaN.AtenderReserva(detalle.DetalleId, cn);
                            if (resultAtender == null || resultAtender.Icono != "success")
                            {
                                return Json(new
                                {
                                    Titulo = "Error al atender reserva",
                                    resultAtender?.Mensajes,
                                    Icono = resultAtender?.Icono ?? "error"
                                });
                            }
                        }

                        var transferencia = _transferenciaReservaN.ObtenerTransferenciaReserva(solicitud.DocNumSolicitudTraslado, cn);
                        var itemCodes = solicitud.Detalles.Select(d => d.ItemCode).Distinct();

                        foreach (var itemCode in itemCodes)
                        {
                            if (_transferenciaReservaN.ValidarSkuParaKardexIngreso(solicitud.DocNumSolicitudTraslado, itemCode, transferencia))
                            {
                                var operario = $"{usuario.Nombres} {usuario.Apellidos}";
                                var transferenciaPorSku = transferencia.Detalle.Where(d => d.ItemCode == itemCode).ToList();
                                int cantidadTotal = Convert.ToInt32(transferenciaPorSku.Sum(d => d.QuantityUnidadesCajas));

                                var resultKardex = _kardexAbastecimientoN.InsertarTransaccionIngresoKardex(
                                    itemCode, transferenciaPorSku.First().ItemName, cantidadTotal, operario,
                                    solicitud.DocNumSolicitudTraslado, transferencia.CardCode, transferencia.CardName, cn);

                                if (resultKardex.Icono != "success")
                                    return Json(new { Titulo = "Error al registrar Kardex", resultKardex.Mensajes, Icono = resultKardex.Icono });

                                foreach (var item in transferenciaPorSku)
                                {
                                    var resultUbicacionesLotes = _ubicacionesLotesN.Ingreso(item, cn);
                                    if (resultUbicacionesLotes.Icono != "success")
                                        return Json(new { Titulo = "Error en ingreso de lotes", resultUbicacionesLotes.Mensajes, Icono = resultUbicacionesLotes.Icono });

                                    var resultMaster = _ubicacionesLotesMasterN.Ingreso(resultUbicacionesLotes.Id, item, cn);
                                    if (resultMaster.Icono != "success")
                                        return Json(new { Titulo = "Error en ingreso a lotes master", resultMaster.Mensajes, Icono = resultMaster.Icono });
                                }
                            }
                        }
                    }

                    scope.Complete();
                }

                return Json(new
                {
                    Titulo = "Reservas atendidas",
                    Mensajes = new List<string> { "Todas las solicitudes fueron procesadas correctamente." },
                    Icono = "success"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Titulo = "Error inesperado",
                    Mensajes = new List<string> { ex.Message },
                    Icono = "error"
                });
            }
        }


        /**************** R E A B A S T E C I M I E N T O ****************/
        //Listado de detalle solicitudes de traslado Transferido y atendidoReserva=0 
        public ActionResult Reabastecimiento(int idOperation = 3600)
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
        //Atendido de Picking (Cambia el AtendidoPicking a 1 y valida si todo se completo respecto al sku del requerimiento para generar una salida en el Kardex
        public JsonResult AtenderPickingRequerimiento(int id, int requerimientoId, string itemCode, string itemName, int idOperation = 3601)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Sin accesos." }, Icono = "error" });

            var user = Session["UsuarioId"] as Usuario_E;
            if (user == null)
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "No existe usuario logueado, se terminó la sesión." }, Icono = "error" });

            if (id <= 0 || requerimientoId <= 0 || string.IsNullOrEmpty(itemCode) || string.IsNullOrEmpty(itemName))
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Los datos enviados son inválidos." }, Icono = "error" });

            try
            {
                Utilitarios uti = new Utilitarios();
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                    {
                        cn.Open();

                        // 1. Atender Picking
                        var resultAtender = _requerimientosN.AtenderPicking(id, cn);
                        if (resultAtender == null || !resultAtender.Icono.Equals("success"))
                            return Json(new { Titulo = "Error en la operación", resultAtender?.Mensajes, Icono = resultAtender?.Icono ?? "error" });

                        scope.Complete();

                        // 9. Retornar éxito solo con atención de SKU
                        return Json(new { Titulo = "Acción completada exitosamente", Mensajes = new List<string> { "Se atendió el SKU correctamente." }, Icono = "success" });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { ex.Message }, Icono = "error" });
            }
        }
        //Atendido de apiladores (Solo cambia el AtendidoReserva a 1)
        public ActionResult ControlStockPicking(int idOperation = 3700)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _reporteStockPicking.ControlStockInternoPicking();
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public JsonResult ConsultarUbicacionesSkuPicking(string itemCode, int idOperation = 3701)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                List<UbicacionesLotes_E> resultUbicacionesLotes = _ubicacionesLotesN.Obtener(itemCode).Where(x => x.Almacen.Equals("RESERVA")).ToList();
                return Json(new { resultUbicacionesLotes });
            }
            else
            {
                return Json(new { Titulo = "Error en la operación", Mensajes = new List<string> { "Sin accesos." }, Icono = "error" });
            }
        }
    }
}