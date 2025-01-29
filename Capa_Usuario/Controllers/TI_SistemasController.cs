using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Capa_Negocio.Seguridad_NEG;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.General_NEG.TablasSql;
using Capa_Negocio.Seguridad_NEG.TablasSql;
using Capa_Negocio.RecursosHumanos_NEG.TablasSQL;
using Capa_Usuario.Helpers;
using System.Linq;

namespace Capa_Usuario.Controllers
{
    public class TI_SistemasController : Controller
    {
        Usuario_N ousrN = new Usuario_N();
        Orol_N orolN = new Orol_N();

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

        public ActionResult GestionPermisos(Usuario_E filtro, string Mensaje = "", int idOperation = 1502)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var usuarioSesion = (Usuario_E)Session["UsuarioId"];

                ViewBag.Mensaje = Mensaje;
                ViewBag.Usuario = filtro;
                ViewBag.Roles = orolN.listarRoles(usuarioSesion.IdRol).Where(x => x.Id!=1);

                return View(ousrN.listaUsuariosPermisos(filtro, usuarioSesion.IdRol).OrderByDescending(x => x.DocEntry).ToList());
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public ActionResult CrearUsuario(int idOperation = 1503)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                Usuario_E usuarioSesion = (Usuario_E)Session["UsuarioId"];
                ViewBag.Empleados = new OEMPL_N().ListarEmpleadosConDatosLaborales(new Capa_Entidad.RecursosHumanos_ENT.TablasSQL.OEMPL_E { Estado = "1" }, null);
                ViewBag.Roles = orolN.listarRoles(usuarioSesion.IdRol);
                ViewBag.Sedes = new SEDE_N().ListarSedesParaCrearUsuario(null);

                return View(new Usuario_E());
            }
            else
            {
                return resultadoAcceso;
            }
        }

        [HttpPost]
        public ActionResult CrearUsuario(Usuario_E datosPost, int idOperation = 1503)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var usuarioSesion = (Usuario_E)Session["UsuarioId"];
                ViewBag.Rol = usuarioSesion.IdRol;

                try
                {
                    var result = ousrN.crearUsuario(datosPost, $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}");

                    return RedirectToAction("GestionPermisos", new { result.Mensaje });
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    ViewBag.Empleados = new OEMPL_N().ListarEmpleadosConDatosLaborales(new Capa_Entidad.RecursosHumanos_ENT.TablasSQL.OEMPL_E { Estado = "1" }, null);
                    ViewBag.Roles = orolN.listarRoles(usuarioSesion.IdRol);
                    ViewBag.Sedes = new SEDE_N().ListarSedesParaCrearUsuario(null);

                    return View(datosPost);
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public ActionResult EditarUsuario(int DocEntry, int idOperation = 1504)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var usuario = ousrN.buscarUsuario(DocEntry);
                usuario.Password2 = usuario.Password;
                ViewBag.RolUsuario = orolN.ObtenerRol(usuario.IdRol);
                ViewBag.Sedes = new SEDE_N().ListarSedesParaCrearUsuario(null);

                return View(usuario);
            }
            else
            {
                return resultadoAcceso;
            }
        }

        [HttpPost]
        public ActionResult EditarUsuario(Usuario_E datosPost, int idOperation = 1504)
        {

            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                var mensaje = ousrN.EditarUsuario(datosPost);

                if (!string.IsNullOrWhiteSpace(mensaje))
                {
                    var usuario = ousrN.buscarUsuario(datosPost.DocEntry);
                    usuario.Email = datosPost.Email;
                    usuario.CodigoSap = datosPost.CodigoSap;
                    usuario.Password = datosPost.Password;
                    usuario.Password2 = datosPost.Password2;

                    ViewBag.RolUsuario = orolN.ObtenerRol(usuario.IdRol);
                    ViewBag.Sedes = new SEDE_N().ListarSedesParaCrearUsuario(null);
                    ViewBag.Mensaje = mensaje;

                    return View(usuario);
                }
                else
                {
                    return RedirectToAction("GestionPermisos");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }

        public ActionResult InactivarUsuario(int DocEntry, int idOperation = 1505)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E obj = ousrN.buscarUsuario(DocEntry);
                    ousrN.Inactivar(obj);
                    return RedirectToAction("GestionPermisos");
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return RedirectToAction("GestionPermisos");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        public ActionResult ActivarUsuario(int DocEntry, int idOperation = 1505)
        {
            var resultadoAcceso = VerificarPermiso(idOperation);

            if (resultadoAcceso is HttpStatusCodeResult statusCodeResult && statusCodeResult.StatusCode == 200)
            {
                try
                {
                    Usuario_E obj = ousrN.buscarUsuario(DocEntry);
                    ousrN.Activar(obj);
                    return RedirectToAction("GestionPermisos");
                }
                catch (Exception e)
                {
                    ViewBag.Mensaje = e.Message;
                    return RedirectToAction("GestionPermisos");
                }
            }
            else
            {
                return resultadoAcceso;
            }
        }
        
        [HttpGet]
        public ActionResult VisualizarPermisosPorRol(int rolID)
        {
            var roles = new OOPE_N().ListarOperaciones(null);
            ViewBag.Operaciones = new ROL_OPE_N().ObtenerOperacionesPorRol(rolID);

            return PartialView("TI_Sistemas/AsignacionPermisos", roles);
        }

        [HttpGet]
        public ActionResult VisualizarPermisosPorUsuario(int usrDocEntry)
        {
            var roles = new OOPE_N().ListarOperaciones(null);
            ViewBag.Operaciones = new OUSR_OPE_N().ObtenerOperacionesPorUsuario(usrDocEntry);

            return PartialView("TI_Sistemas/AsignacionPermisos", roles);
        }

        public JsonResult AsignarPermisosPorRol(List<int> operaciones, int rolID, int idOperation = 4001)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["controller"].ToString(), Request.UserHostAddress, Request.UserHostName);

            if (string.IsNullOrWhiteSpace(acceso) || !acceso.Equals("C_Access"))
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }

            if (operaciones == null || rolID <= 0)
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Recargar página e intentar de nuevo." }, Icono = "error" });
            }

            var mensajeError = new ROL_OPE_N().AsignarPermisosPorRol(operaciones, rolID);
            string mensaje = string.IsNullOrWhiteSpace(mensajeError) ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            string iconoMensaje = string.IsNullOrWhiteSpace(mensajeError) ? "success" : "warning";

            return Json(new { Mensaje = mensaje, Comentario = new List<string>() { mensajeError }, Icono = iconoMensaje });
        }

        public JsonResult AsignarPermisosPorUsuario(List<int> operaciones, int usrDocEntry, int idOperation = 4001)
        {
            string acceso = AccesoHelper.VerificarAccesos(idOperation, (Usuario_E)Session["UsuarioId"], this.ControllerContext.RouteData.Values["controller"].ToString(), Request.UserHostAddress, Request.UserHostName);

            if (string.IsNullOrWhiteSpace(acceso) || !acceso.Equals("C_Access"))
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            }

            if (operaciones == null || usrDocEntry <= 0)
            {
                return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Recargar página e intentar de nuevo." }, Icono = "error" });
            }

            var mensajeError = new OUSR_OPE_N().AsignarPermisosPorUsuario(operaciones, usrDocEntry);
            string mensaje = string.IsNullOrWhiteSpace(mensajeError) ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            string iconoMensaje = string.IsNullOrWhiteSpace(mensajeError) ? "success" : "warning";

            return Json(new { Mensaje = mensaje, Comentario = new List<string>() { mensajeError }, Icono = iconoMensaje });
        }

        // infos
        public JsonResult infoIdUsuario(int idRol)
        {
            return Json(ousrN.generarId(idRol));
        }

    }
}
