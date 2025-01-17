using Capa_Entidad.General_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Capa_Negocio.General_NEG.TablasSql;
using Capa_Usuario.Helpers;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capa_Usuario.Controllers
{
    public class RepartosController : Controller
    {
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
        public JsonResult buscarConductorYPlaca(string zona)
        {
            var (placa, conductor) = new Capa_Negocio.Repartos_NEG.TablasHana.SYP_VEHICU_N().buscarConductorYPlaca(zona);
            var response = new
            {
                Placa = placa,
                Conductor = conductor
            };

            return Json(response, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GestionarAgencias(OAGE_E o, string TipoRep, string msj, int idOperation = 5000)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                ViewBag.Mensaje = msj;
                ViewBag.TipoRep = TipoRep;
                return View(new Capa_Negocio.General_NEG.TablasSql.OAGE_N().Listar());
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public JsonResult RegistrarAgencia(Capa_Entidad.General_ENT.TablasSql.OAGE_E o, int idOperation = 5000)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["action"].ToString(), Request.UserHostAddress, Request.UserHostName);

            if (acceso == "C_Access")
            {
                return Json(new Capa_Negocio.General_NEG.TablasSql.OAGE_N().Registrar(o));
            }
            else
            { return null; }

        }
        public ActionResult EliminarAgencia(int Id, int idOperation = 5000)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    new Capa_Negocio.General_NEG.TablasSql.OAGE_N().Eliminar(Id);
                    return RedirectToAction("GestionarAgencias");
                }

                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return RedirectToAction("GestionarAgencias");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
    }
}
