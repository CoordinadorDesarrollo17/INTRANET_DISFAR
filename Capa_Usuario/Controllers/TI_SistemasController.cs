using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Capa_Negocio.Seguridad_NEG;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.General_NEG.TablasSql;
using Capa_Negocio.Seguridad_NEG.TablasSql;
using Capa_Negocio.RecursosHumanos_NEG.TablasSQL;

namespace Capa_Usuario.Controllers
{
    public class TI_SistemasController : Controller
    {
        int modulo = 1;
        private readonly Capa_Negocio.Helpers helper = new Capa_Negocio.Helpers();
        Usuario_N uN = new Usuario_N();
        Orol_N orolN = new Orol_N();

        public ActionResult GestionPermisos(Usuario_E filtro, string Mensaje = "", int idOperation = 1502)
        {
            switch (VerificarAccesos(idOperation))
            {
                case "C_Access":
                    var usuarioSesion = (Usuario_E)Session["UsuarioId"];

                    ViewBag.Mensaje = Mensaje;
                    ViewBag.Usuario = filtro;
                    ViewBag.Roles = orolN.listarRoles(usuarioSesion.IdRol);

                    return View(uN.listaUsuariosPermisos(filtro, usuarioSesion.IdRol));

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
            }
        }

        public ActionResult CrearUsuario(int idOperation = 1503)
        {
            switch (VerificarAccesos(idOperation))
            {
                case "C_Access":
                    Usuario_E usuarioSesion = (Usuario_E)Session["UsuarioId"];
                    ViewBag.Empleados = new OEMPL_N().ListarEmpleados(null);
                    ViewBag.Roles = orolN.listarRoles(usuarioSesion.IdRol);
                    ViewBag.Sedes = new SEDE_N().ListarSedesParaCrearUsuario(null);

                    return View(new Usuario_E());

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
            }
        }

        [HttpPost]
        public ActionResult CrearUsuario(Usuario_E datosPost, int idOperation = 1503)
        {
            switch (VerificarAccesos(idOperation))
            {
                case "C_Access":
                    var usuarioSesion = (Usuario_E)Session["UsuarioId"];
                    ViewBag.Rol = usuarioSesion.IdRol;

                    try
                    {
                        var result = uN.crearUsuario(datosPost, $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}");

                        return RedirectToAction("GestionPermisos", new { result.Mensaje });
                    }
                    catch (Exception e)
                    {
                        ViewBag.Mensaje = e.Message;
                        ViewBag.Empleados = new OEMPL_N().ListarEmpleados(null);
                        ViewBag.Roles = orolN.listarRoles(usuarioSesion.IdRol);
                        return View(datosPost);
                    }

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
            }
        }

        public ActionResult EditarUsuario(int DocEntry, int idOperation = 1504)
        {
            switch (VerificarAccesos(idOperation))
            {
                case "C_Access":
                    var usuario = uN.buscarUsuario(DocEntry);
                    usuario.Password2 = usuario.Password;
                    ViewBag.RolUsuario = orolN.ObtenerRol(usuario.IdRol);
                    ViewBag.Sedes = new SEDE_N().ListarSedesParaCrearUsuario(null);

                    return View(usuario);

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
            }
        }

        [HttpPost]
        public ActionResult EditarUsuario(Usuario_E datosPost, int idOperation = 1504)
        {
            switch (VerificarAccesos(idOperation))
            {
                case "C_Access":
                    var mensaje = uN.EditarUsuario(datosPost);

                    if (!string.IsNullOrEmpty(mensaje))
                    {
                        var usuario = uN.buscarUsuario(datosPost.DocEntry);
                        usuario.Email = datosPost.Email;
                        usuario.CodigoSap = datosPost.CodigoSap;
                        usuario.Password = datosPost.Password;
                        usuario.Password2 = datosPost.Password2;

                        ViewBag.RolUsuario = orolN.ObtenerRol(usuario.IdRol);
                        ViewBag.Mensaje = mensaje;

                        return View(usuario);
                    }
                    else
                    {
                        return RedirectToAction("GestionPermisos");
                    }

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
            }
        }

        public ActionResult InactivarUsuario(int DocEntry, int idOperation = 1505)
        {
            switch (VerificarAccesos(idOperation))
            {
                case "C_Access":
                    try
                    {
                        Usuario_E obj = uN.buscarUsuario(DocEntry);
                        uN.Inactivar(obj);
                        return RedirectToAction("GestionPermisos");
                    }
                    catch (Exception e)
                    {
                        ViewBag.Mensaje = e.Message;
                        return RedirectToAction("GestionPermisos");
                    }

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
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
            string acceso = VerificarAccesos(idOperation);

            //if (string.IsNullOrEmpty(acceso) || !acceso.Equals("C_Access"))
            //{
            //    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            //}

            //if (form == null || (form != null && form.IdNumero <= 0))
            //{
            //    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Número no válido." }, Icono = "error" });
            //}

            var mensajeError = new ROL_OPE_N().AsignarPermisosPorRol(operaciones, rolID);
            string mensaje = string.IsNullOrEmpty(mensajeError) ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            string iconoMensaje = string.IsNullOrEmpty(mensajeError) ? "success" : "warning";

            return Json(new { Mensaje = mensaje, Comentario = new List<string>() { mensajeError }, Icono = iconoMensaje });
        }

        public JsonResult AsignarPermisosPorUsuario(List<int> operaciones, int usrDocEntry, int idOperation = 4001)
        {
            string acceso = VerificarAccesos(idOperation);

            //if (string.IsNullOrEmpty(acceso) || !acceso.Equals("C_Access"))
            //{
            //    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "No cuentas con permisos para realizar esta acción." }, Icono = "error" });
            //}

            //if (form == null || (form != null && form.IdNumero <= 0))
            //{
            //    return Json(new { Mensaje = "Error crítico", Comentario = new List<string>() { "Número no válido." }, Icono = "error" });
            //}

            var mensajeError = new OUSR_OPE_N().AsignarPermisosPorUsuario(operaciones, usrDocEntry);
            string mensaje = string.IsNullOrEmpty(mensajeError) ? "¡Acción realizada con éxito!" : "No se pudo completar la acción";
            string iconoMensaje = string.IsNullOrEmpty(mensajeError) ? "success" : "warning";

            return Json(new { Mensaje = mensaje, Comentario = new List<string>() { mensajeError }, Icono = iconoMensaje });
        }

        // infos
        public JsonResult infoIdUsuario(int idRol)
        {
            return Json(uN.generarId(idRol));
        }

        private string VerificarAccesos(int idOperation)
        {
            Usuario_E usu = (Usuario_E)Session["UsuarioId"];
            string nombreOperacion = this.ControllerContext.RouteData.Values["action"].ToString();

            return helper.VerificarAccesos(idOperation, usu, nombreOperacion, modulo, Request.UserHostAddress, Request.UserHostName);
        }
    }
}
