using System.Collections.Generic;
using System.Web.Mvc;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.DireccionTecnica_NEG.TablasSql;
using Capa_Usuario.Helpers;

namespace Capa_Usuario.Controllers
{
    public class SolicitudesReversion_DOCS1Controller : Controller
    {
        private readonly SolicitudesReversion_DOCS1_N _solicitadesReversion = new SolicitudesReversion_DOCS1_N();
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
                var lista = _solicitadesReversion.ListarSolicitudesReversion();
                return View(lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public JsonResult SolicitarReversionLiberacionArticulo(int id, int idOperacion = 0)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var usuarioRegistro = $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}";
            var result = _solicitadesReversion.SolicitarReversionLiberacionArticulo(id, usuarioRegistro);
            return Json(result);
        }
    }
}