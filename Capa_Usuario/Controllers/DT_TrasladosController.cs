using System.Collections.Generic;
using System.Web.Mvc;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.TablasSql;
using Capa_Negocio.DireccionTecnica_NEG.TablasSql;
using Capa_Usuario.Helpers;

namespace Capa_Usuario.Controllers
{
    public class DT_TrasladosController : Controller
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

        public ActionResult Index(int idOperation = 0)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                return View("~/Views/DireccionTecnica/Liberaciones/Traslados.cshtml");
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public ActionResult VerDetalle(long id, int idOperation = 0)
        {
            var usuarioSesion = Session["UsuarioId"] as Usuario_E;
            if (usuarioSesion == null)
                return Json(new { Titulo = "No se pudo completar la acción", Mensajes = new List<string> { "Inicia sesión nuevamente para continuar" }, Icono = "error" }, JsonRequestBehavior.AllowGet);

            var resultadoAcceso = VerificarPermiso(idOperation);
            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var lista = _docsN.ListarTraslados(new ODOCS_E { Id = id });

                return View("~/Views/DireccionTecnica/Liberaciones/DetalleTraslado.cshtml", lista);
            }
            else
            {
                return resultadoAcceso;
            }
        }
    }
}