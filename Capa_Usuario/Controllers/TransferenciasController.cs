using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Capa_Entidad.DireccionTecnica_ENT.Reportes.Liberaciones;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Entidad.TablasSql;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasSql;
using Capa_Negocio.DireccionTecnica_NEG.TablasSql;
using Capa_Negocio.SocioNegocios_NEG.TablasExternas;
using Capa_Usuario.Helpers;
using OfficeOpenXml;

namespace Capa_Usuario.Controllers
{
    public class TransferenciasController : Controller
    {
        private readonly Capa_Negocio.DireccionTecnica_NEG.TablasSql.ODOCS_N _docsN = new Capa_Negocio.DireccionTecnica_NEG.TablasSql.ODOCS_N();
        private readonly DOCS1_N _detalleDocN = new DOCS1_N();

        /************************* C O N F I G U R A C I Ó N *************************/
        private ActionResult VerificarPermiso(int idOperacion)
        {
            var accesoHelper = new Capa_Entidad.AccessoHelper_E
            {
                OpeID = idOperacion,
                usuario = (Usuario_E)Session["UsuarioId"],
                controllerDestino = this.ControllerContext.RouteData.Values["controller"].ToString(),
                action = this.ControllerContext.RouteData.Values["action"].ToString(),
                userHostAddress = Request.UserHostAddress,
                userHostName = Request.UserHostName
            };
            return AccesoHelper.GestionarAccesoController(this, accesoHelper);
        }
        /*******************************************************************/

        public ActionResult Index(int idOperacion = 0)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperacion);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _docsN.ListarTransferencias();
                ViewBag.ListaProveedores = new OCRD_N().listarSociosDeNegocios(new OCRD_E { CardType = "S" });     // Solo socios Proveedores

                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public ActionResult FiltrarListado(ODOCS_E filtros)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var lista = _docsN.ListarTransferencias(filtros);

            return PartialView("Transferencias/_ListadoTransferencias", lista);
        }

        public ActionResult VerDetalle(long id, int idOperacion = 0)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperacion);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _docsN.ListarTransferencias(new ODOCS_E { Id = id }).First();

                ViewBag.ListaAprobados = lista.Detalle.Where(x => x.CantidadAprobados > 0).ToList();
                ViewBag.ListaBaja = lista.Detalle.Where(x => x.CantidadBaja > 0).ToList();
                ViewBag.ListaDevolucion = lista.Detalle.Where(x => x.CantidadDevolucion > 0).ToList();

                return View("DetalleTransferencia", lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public JsonResult TransferirArticulo(int cantidad, string area, int id, int idOperacion = 0)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var usuarioRegistro = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";
            var result = _detalleDocN.TransferirArticulo(cantidad, area, id, usuarioRegistro);
            return Json(result);
        }

        public JsonResult RevertirTransferenciaArticulo(int id, string area, int idOperacion = 0)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var usuarioRegistro = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";
            var result = _detalleDocN.RevertirTransferenciaArticulo(id, area, usuarioRegistro);
            return Json(result);
        }

        public JsonResult CancelarTransferencia(int id)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var usuarioRegistro = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";

            var result = _docsN.CancelarTransferencia(id, usuarioRegistro);

            return Json(result);
        }

        public ActionResult DescargarExcelTransferencias(ODOCS_E filtros, int idOperation = 3000)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var listado = _docsN.ListarTransferencias(filtros);

                var tiposDocumento = new Dictionary<string, string>
                    {
                        { "OPDN", "Entrada de Mercancía" },
                        { "OWTR", "Transferencia" }
                    };

                if (listado != null && listado.Any())
                {
                    var exportList = listado
                        .Where(x => x.Detalle != null)
                        .SelectMany(x => x.Detalle.Select(d => new RptTransferencias_E
                        {
                            // Cabecera
                            TipoDocumento = tiposDocumento.ContainsKey(x.TipoDocumento) ? tiposDocumento[x.TipoDocumento] : x.TipoDocumento,
                            DocEntry = x.DocEntry,
                            DocNum = x.DocNum,
                            CardCode = x.CardCode,
                            CardName = x.CardName,
                            Guia = x.Guia,
                            ComprobanteVinculado = x.ComprobanteVinculado,
                            FechaContabilizacion = x.FechaContabilizacion,
                            FechaInicioTraslado = x.FechaInicioTraslado,
                            Estado = x.Estado,

                            // Detalle
                            ItemCode = d.ItemCode,
                            ItemName = d.ItemName,
                            Lote = d.Lote,
                            FechaVencimiento = d.FechaVencimiento,
                            RegistroSanitario = d.RegistroSanitario,
                            Fabricante = d.Fabricante,
                            CondicionAlmTrans = d.CondicionAlmTrans,
                            Almacen = d.Almacen,
                            CertificadoAnalisis = d.CertificadoAnalisis,
                            ComentarioOrganoleptico = d.ComentarioOrganoleptico,
                            CantidadAprobados = d.CantidadAprobados,
                            CantidadBaja = d.CantidadBaja,
                            CantidadDevolucion = d.CantidadDevolucion,
                            CantidadTotal = d.CantidadTotal,
                            Liberado = d.Liberado == 1 ? "SI" : "NO",
                            Transferido = d.Transferido == 1 ? "SI" : "NO"
                        }))
                        .ToList();

                    using (var libro = new ExcelPackage())
                    {
                        var worksheet = libro.Workbook.Worksheets.Add("ReporteTransferencias");

                        // Cargamos los datos desde fila 1, columna 1
                        worksheet.Cells["A1"].LoadFromCollection(exportList, true);

                        // Obtenemos dimensión real luego del Load
                        int totalFilas = exportList.Count + 1; // +1 por el header
                        int totalColumnas = worksheet.Dimension.End.Column;

                        // Ajustamos columnas
                        for (int col = 1; col <= totalColumnas; col++)
                            worksheet.Column(col).AutoFit();

                        // Añadimos tabla asegurando que se incluyan todas las filas
                        var tabla = worksheet.Tables.Add(
                            worksheet.Cells[1, 1, totalFilas, totalColumnas],
                            "ReporteTransferencias"
                        );

                        tabla.ShowHeader = true;
                        tabla.TableStyle = OfficeOpenXml.Table.TableStyles.Medium2;

                        string excelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        return File(libro.GetAsByteArray(), excelContentType, "ReporteTransferencias.xlsx");
                    }

                }
                else
                {
                    return Content("No hay datos para exportar");
                }
            }
            else
            {
                return resultadoAcceso;
            }

        }

    }
}