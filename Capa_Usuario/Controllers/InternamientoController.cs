using System.Collections.Generic;
using System.Web.Mvc;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.TablasSql;
using Capa_Usuario.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Capa_Usuario.Controllers
{
    public class InternamientoController : Controller
    {
        private readonly Capa_Negocio.DireccionTecnica_NEG.TablasSql.ODOCS_N _docsN = new Capa_Negocio.DireccionTecnica_NEG.TablasSql.ODOCS_N();

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

        [HttpGet]
        public ActionResult Index(int idOperation = 0)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View("~/Views/DireccionTecnica/Liberaciones/Internamiento.cshtml");
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public JsonResult BuscarDocumento(long docNum, string tipoDocumento)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            if (string.IsNullOrWhiteSpace(tipoDocumento) || !new List<string> { "OPDN", "OWTR" }.Contains(tipoDocumento))
                return Json(new { Titulo = "Error al buscar documento", Mensajes = new List<string> { "Tipo de documento inválido." }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            if (docNum <= 0)
                return Json(new { Titulo = "Error al buscar documento", Mensajes = new List<string> { "DocNum inválido." }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            Capa_Negocio.DireccionTecnica_NEG.TablasExternas.ODOCS_SAP_N _internamientoSap = new Capa_Negocio.DireccionTecnica_NEG.TablasExternas.ODOCS_SAP_N();
            var result = tipoDocumento == "OPDN" ? _internamientoSap.BuscarDocEntradaMercaderia(docNum) : _internamientoSap.BuscarDocTransferencias(docNum);

            return Json(new { result.Item1.Titulo, result.Item1.Mensajes, result.Item1.Icono, Documento = result.Item2 });
        }

        public ActionResult FiltrarListado(ODOCS_E filtros)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var lista = _docsN.ListarInternamientos(filtros);

            return PartialView("~/Views/Shared/DireccionTecnica/Liberaciones/_ListadoInternamientos.cshtml", lista);
        }

        public ActionResult VerDetalle(long id, int idOperation = 0)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _docsN.ListarInternamientos(new ODOCS_E { Id = id });

                return View("~/Views/DireccionTecnica/Liberaciones/DetalleInternamiento.cshtml", lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public JsonResult RegistrarDocumento(ODOCS_E docPost)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            docPost.UsuarioRegistro = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";
            var result = _docsN.RegistrarDocumento(docPost);

            return Json(result);
        }
    }
}