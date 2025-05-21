using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Capa_Entidad.Almacen_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Entidad.TablasSql;
using Capa_Negocio.DireccionTecnica_NEG.TablasSql;
using Capa_Negocio.SocioNegocios_NEG.TablasExternas;
using Capa_Usuario.Helpers;
using DocumentFormat.OpenXml.Office2013.Drawing.Chart;

namespace Capa_Usuario.Controllers
{
    public class InternamientosController : Controller
    {
        private readonly Capa_Negocio.DireccionTecnica_NEG.TablasSql.ODOCS_N _docsN = new Capa_Negocio.DireccionTecnica_NEG.TablasSql.ODOCS_N();
        private readonly DOCS1_N _detalleDocN = new DOCS1_N();
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
        /*******************************************************************/

        public ActionResult Index(int idOperacion = 6000)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperacion);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _docsN.ListarInternamientos();
                ViewBag.ListaProveedores = new OCRD_N().listarSociosDeNegocios(new OCRD_E { CardType = "S" });     // Solo socios Proveedores

                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public ActionResult Internamiento(int idOperacion = 6000)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperacion);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View();
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public JsonResult BuscarDocumento(long docNum, string guia, string tipoDocumento)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            if (string.IsNullOrWhiteSpace(tipoDocumento) || !new List<string> { "OPDN", "OWTQ" }.Contains(tipoDocumento))
                return Json(new { Titulo = "Error al buscar documento", Mensajes = new List<string> { "Tipo de documento inválido." }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            if (docNum <= 0 && string.IsNullOrWhiteSpace(guia))
            {
                return Json(new
                {
                    Titulo = "Validación requerida",
                    Mensajes = new List<string> { "Debe ingresar al menos el DocNum mayor a cero o la Guía no vacía." },
                    Icono = "error"
                }, JsonRequestBehavior.AllowGet);
            }

            // Si se envió solo el docnum y es válido
            if (docNum > 0)
            {
                if (_docsN.ListarInternamientos(new ODOCS_E { DocNum = docNum }).Any(x => x.Estado != "Cancelado"))
                    return Json(_helper.CrearAlertaUI(new List<string> { "El documento ingresado ya se encuentra registrado." }, "error"), JsonRequestBehavior.AllowGet);

                Capa_Negocio.DireccionTecnica_NEG.TablasExternas.ODOCS_SAP_N _internamientoSap = new Capa_Negocio.DireccionTecnica_NEG.TablasExternas.ODOCS_SAP_N();
                var result = tipoDocumento == "OPDN"
                    ? _internamientoSap.BuscarDocEntradaMercaderia(docNum, "")
                    : _internamientoSap.BuscarDocSolicitudTraslado(docNum, "");

                return Json(new { result.Item1.Titulo, result.Item1.Mensajes, result.Item1.Icono, Documento = result.Item2 });
            }

            // Si se envió solo la guía y es válida
            if (!string.IsNullOrWhiteSpace(guia))
            {
                if (_docsN.ListarInternamientos(new ODOCS_E { Guia = guia }).Any(x => x.Estado != "Cancelado"))
                    return Json(_helper.CrearAlertaUI(new List<string> { "La guía ingresada ya se encuentra registrada." }, "error"), JsonRequestBehavior.AllowGet);

                Capa_Negocio.DireccionTecnica_NEG.TablasExternas.ODOCS_SAP_N _internamientoSap = new Capa_Negocio.DireccionTecnica_NEG.TablasExternas.ODOCS_SAP_N();
                var result2 = tipoDocumento == "OPDN"
                    ? _internamientoSap.BuscarDocEntradaMercaderia(0, guia)
                    : _internamientoSap.BuscarDocSolicitudTraslado(0, guia);

                return Json(new { result2.Item1.Titulo, result2.Item1.Mensajes, result2.Item1.Icono, Documento = result2.Item2 });
            }

            return Json(new { Titulo = "Error inesperado", Mensajes = new List<string> { "No se pudo procesar la solicitud." }, Icono = "error" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FiltrarListado(ODOCS_E filtros)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var lista = _docsN.ListarInternamientos(filtros);

            return PartialView("Internamientos/_ListadoInternamientos", lista);
        }

        public ActionResult VerDetalle(long id, int idOperation = 6000)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _docsN.ListarInternamientos(new ODOCS_E { Id = id });
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;
                ViewBag.JsonModel = serializer.Serialize(lista);

                return View("DetalleInternamiento", lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public JsonResult RegistrarDocumento(ODOCS_E docPost, int idOperacion = 6001)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperacion);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                return Json(new { Titulo = "Acceso Denegado", Mensajes = new List<string> { "No tienes permisos para registrar documento" }, Icono = "warning" }, JsonRequestBehavior.AllowGet);

            docPost.UsuarioRegistro = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";

            var result = _docsN.RegistrarDocumento(docPost);

            return Json(result);
        }

        public JsonResult EditarItemDetalleDoc(DOCS1_E detallePost, int idOperacion = 6002)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperacion);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                return Json(new { Titulo = "Acceso Denegado", Mensajes = new List<string> { "No tienes permisos para editar el detalle del documento" }, Icono = "warning" }, JsonRequestBehavior.AllowGet);

            var usuarioRegistro = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";
            var result = _detalleDocN.EditarItemDetalleDoc(detallePost, usuarioRegistro);
            return Json(result);
        }

        public JsonResult LiberarArticulos(List<int> ids, int idOperacion = 6003)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperacion);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                return Json(new { Titulo = "Acceso Denegado", Mensajes = new List<string> { "No tienes permisos para editar el detalle del documento" }, Icono = "warning" }, JsonRequestBehavior.AllowGet);

            var usuarioRegistro = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";
            var result = _detalleDocN.LiberarArticulos(ids, usuarioRegistro);
            return Json(result);
        }

        public JsonResult RevertirLiberacionArticulo(int id, string estado, int idOperacion = 6004)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperacion);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                return Json(new { Titulo = "Acceso Denegado", Mensajes = new List<string> { "No tienes permisos para revertir la liberación del artículo" }, Icono = "warning" }, JsonRequestBehavior.AllowGet);

            var usuarioRegistro = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";
            var result = _detalleDocN.RevertirLiberacionArticulo(id, estado, usuarioRegistro);
            return Json(result);
        }

        public JsonResult CancelarDocumento(int id, int idOperacion = 6005)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperacion);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                return Json(new { Titulo = "Acceso Denegado", Mensajes = new List<string> { "No tienes permisos para cancelar el documento" }, Icono = "warning" }, JsonRequestBehavior.AllowGet);

            var usuarioRegistro = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";

            var result = _docsN.CancelarDocumento(id, usuarioRegistro);

            return Json(result);
        }

        public JsonResult CrearDevolucion(long id, int idOperacion = 0)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperacion);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode != 200)
                return Json(new { Titulo = "Acceso Denegado", Mensajes = new List<string> { "No tienes permisos para cancelar el documento" }, Icono = "warning" }, JsonRequestBehavior.AllowGet);

            var lista = _docsN.ListarInternamientos(new ODOCS_E { Id = id });
            var internamiento = lista != null ? lista.First() : new ODOCS_E();

            var devolucion = new ORPD_E();
            devolucion.WhsCode = internamiento.Detalle != null && internamiento.Detalle.Any() ? internamiento.Detalle.First().Almacen : "";
            devolucion.CardCode = internamiento.CardCode;
            devolucion.CardName = internamiento.CardName;
            devolucion.RetiroMercado = false;
            devolucion.SinEM = false;
            devolucion.Operario = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";

            int linea = 1;
            var detalleDevolucion = new List<RPD1_E>();
            foreach (var item in internamiento.Detalle)
            {
                DateTime fechaConvertida = DateTime.ParseExact(item.FechaVencimiento, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                var obj = new RPD1_E();
                obj.Linea = linea;
                obj.ItemCode = item.ItemCode;
                obj.ItemName = item.ItemName;
                obj.FirmCode = 0;
                obj.BatchNum = item.Lote;
                obj.ExpDate = fechaConvertida.ToString("yyyy-MM-dd");
                obj.Quantity = item.CantidadDevolucion;
                obj.NumInBuy = 0;
                obj.BuyUnitMsr = "";
                obj.Motivo = 11;        // 11: Devolución
                obj.RefFactura = internamiento.ComprobanteVinculado;
                obj.Observacion = null;
                obj.MaxQuantity = 0;
                obj.Submotivo = 0;
                obj.MaxQuantityOIBT = 0;
                obj.NumInBuyKey = 0;

                detalleDevolucion.Add(obj);
                ++linea;
            }
            ;
            var result = new Capa_Negocio.Almacen_NEG.TablasSql.ORPD_N().RegistrarDevolucion(devolucion, detalleDevolucion);

            return Json(result);

        }
    }
}