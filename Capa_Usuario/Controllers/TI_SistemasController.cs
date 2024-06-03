
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Capa_Entidad;
using Capa_Negocio.Seguridad_NEG;
using Capa_Entidad.Seguridad_ENT;
using System.Collections;
using Capa_Negocio.TI_Sistemas_NEG;
using Capa_Entidad.TI_Sistemas_ENT;
using Capa_Negocio.General_NEG.TablasSql;

namespace Capa_Usuario.Controllers
{
    public class TI_SistemasController : Controller
    {

        int modulo = 1;
        Rol1_N rol1N = new Rol1_N();
        Usuario_N uN = new Usuario_N(); Orol_N orolN = new Orol_N(); OTSO_N otsoN = new OTSO_N();
        private string verificacionAccesos(int ope)
        {
            string nombreOperacion = this.ControllerContext.RouteData.Values["action"].ToString();
            Usuario_E user = (Usuario_E)Session["UsuarioId"];
            if (user == null)
            { return "E_Login"; }
            else
            {
                if ((rol1N.verificarAccesoOperacion(user.IdRol, ope, nombreOperacion, modulo) == 1) || (user.IdRol == 1))
                {
                    Capa_Negocio.Utilitarios_N utiN = new Capa_Negocio.Utilitarios_N();
                    utiN.registrarLog($"{user.Prefijo}{user.Id}", "intento de " + nombreOperacion, ope, Request.UserHostAddress, Request.UserHostName);
                    return "C_Access";
                }
                else
                { return "E_Access"; }
            }
        }

        public ActionResult GestionPermisos(Usuario_E filtro, string Mensaje = "", int idOperation = 1502)
        {
            switch (verificacionAccesos(idOperation))
            {
                case "C_Access":
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    ViewBag.Mensaje = Mensaje;
                    ViewBag.Usuario = filtro;
                    return View(uN.listaUsuariosPermisos(filtro, user.IdRol));

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
            }
        }

        public ActionResult CrearUsuario(int idOperation = 1503)
        {
            switch (verificacionAccesos(idOperation))
            {
                case "C_Access":
                    Usuario_E user = (Usuario_E)Session["UsuarioId"];
                    ViewBag.Rol = user.IdRol;
                    ViewBag.Roles = new SelectList(orolN.listarRoles(user.IdRol), "id", "nombre");
                    return View(new Usuario_E());

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
            }
        }

        [HttpPost]
        public ActionResult CrearUsuario(Usuario_E usuarioPost, string password2, int idOperation = 1503)
        {
            switch (verificacionAccesos(idOperation))
            {
                case "C_Access":
                    Usuario_E usuarioSesion = (Usuario_E)Session["UsuarioId"];
                    ViewBag.Rol = usuarioSesion.IdRol;
                    try
                    {
                        if (usuarioPost.Password != null && usuarioPost.Password.Equals(password2))
                        {
                            string msj = uN.crearUsuario(usuarioPost, $"{usuarioSesion.Nombres} {usuarioSesion.Apellidos}");
                            return RedirectToAction("GestionPermisos", new { Mensaje = msj });
                        }
                        else
                        {
                            ViewBag.Roles = new SelectList(orolN.listarRoles(usuarioSesion.IdRol), "id", "nombre");
                            ViewBag.Mensaje = "Las contraseñas no coinciden.";
                            return View(usuarioPost);
                        }

                    }
                    catch (Exception e)
                    {
                        ViewBag.Mensaje = e.Message;
                        ViewBag.Roles = new SelectList(orolN.listarRoles(usuarioSesion.IdRol), "id", "nombre");
                        return View(usuarioPost);
                    }

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
            }
        }

        public ActionResult EditarUsuario(int DocEntry, int idOperation = 1504)
        {
            switch (verificacionAccesos(idOperation))
            {
                case "C_Access":
                    var datosUsuario = uN.buscarUsuario(DocEntry);
                    ViewBag.RolUsuario = orolN.ObtenerRol(datosUsuario.IdRol);

                    return View(datosUsuario);

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
            }
        }

        [HttpPost]
        public ActionResult EditarUsuario(Usuario_E u, int idOperation = 1504)
        {
            switch (verificacionAccesos(idOperation))
            {
                case "C_Access":
                    try
                    {
                        uN.editarUsuario(u);
                        // OLD--- return RedirectToAction("GestionPermisos", new { id = $"{u.Prefijo}{u.Id}"});
                        return RedirectToAction("GestionPermisos");
                    }
                    catch (Exception e)
                    {
                        ViewBag.Mensaje = e.Message;
                        return View(u);
                    }

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
            }
        }

        public ActionResult InactivarUsuario(int DocEntry, int idOperation = 1505)
        {
            switch (verificacionAccesos(idOperation))
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


        public ActionResult GestionRolOperacion(int idOperation = 1506)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                return View(orolN.listarRoles(user.IdRol));
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            {
                return RedirectToAction("Index", "Index");
            }
            else
            {
                return RedirectToAction("Error", "Index");
            }
        }
        public ActionResult AdministrarRolOperacion(int id, int idOperation = 1507)
        {
            switch (verificacionAccesos(idOperation))
            {
                case "C_Access":
                    OOPE_N opN = new OOPE_N();
                    ViewBag.idRol = id;

                    ViewBag.OpMod1 = opN.listarOperacionesRolModulo(1, id);
                    ViewBag.OpMod2 = opN.listarOperacionesRolModulo(2, id);
                    ViewBag.OpMod3 = opN.listarOperacionesRolModulo(3, id);
                    ViewBag.OpMod4 = opN.listarOperacionesRolModulo(4, id);
                    ViewBag.OpMod5 = opN.listarOperacionesRolModulo(5, id);
                    ViewBag.OpMod6 = opN.listarOperacionesRolModulo(6, id);
                    ViewBag.OpMod7 = opN.listarOperacionesRolModulo(7, id);
                    ViewBag.OpMod8 = opN.listarOperacionesRolModulo(8, id);

                    ViewBag.AllModulo1 = opN.listarOperacionesRolModulo(1, 0);
                    ViewBag.AllModulo2 = opN.listarOperacionesRolModulo(2, 0);
                    ViewBag.AllModulo3 = opN.listarOperacionesRolModulo(3, 0);
                    ViewBag.AllModulo4 = opN.listarOperacionesRolModulo(4, 0);
                    ViewBag.AllModulo5 = opN.listarOperacionesRolModulo(5, 0);
                    ViewBag.AllModulo6 = opN.listarOperacionesRolModulo(6, 0);
                    ViewBag.AllModulo7 = opN.listarOperacionesRolModulo(7, 0);
                    ViewBag.AllModulo8 = opN.listarOperacionesRolModulo(8, 0);

                    ViewBag.Mensaje = "Seleccione los módulos a los que desee que el operario de Rol tenga acceso, luego presione Guardar Cambios";
                    return View();

                case "E_Login":
                    return RedirectToAction("Index", "Index");

                default:
                    return RedirectToAction("Error", "Index");
            }
        }

        [HttpPost]
        public ActionResult AdministrarRolOperacion(int id, IEnumerable<int> idOperacion, int idOperation = 1507)
        {
            if (verificacionAccesos(idOperation) == "C_Access")
            {
                Usuario_E user = (Usuario_E)Session["UsuarioId"];
                int[] numeros = idOperacion.ToArray();
                OOPE_N opN = new OOPE_N(); Rol1_N rl1N = new Rol1_N();
                if (user.IdRol == 1)
                { rl1N.crudOperacion(id, numeros); }
                else
                {
                    ViewBag.Mensaje = "Error, no tiene accesos para alterar permisos, solo de visualizacion";
                    ViewBag.OpMod1 = opN.listarOperacionesRolModulo(1, id);
                    ViewBag.OpMod2 = opN.listarOperacionesRolModulo(2, id);
                    ViewBag.OpMod3 = opN.listarOperacionesRolModulo(3, id);
                    ViewBag.OpMod4 = opN.listarOperacionesRolModulo(4, id);
                    ViewBag.OpMod5 = opN.listarOperacionesRolModulo(5, id);
                    ViewBag.OpMod6 = opN.listarOperacionesRolModulo(6, id);
                    ViewBag.OpMod7 = opN.listarOperacionesRolModulo(7, id);

                    ViewBag.AllModulo1 = opN.listarOperacionesRolModulo(1, 0);
                    ViewBag.AllModulo2 = opN.listarOperacionesRolModulo(2, 0);
                    ViewBag.AllModulo3 = opN.listarOperacionesRolModulo(3, 0);
                    ViewBag.AllModulo4 = opN.listarOperacionesRolModulo(4, 0);
                    ViewBag.AllModulo5 = opN.listarOperacionesRolModulo(5, 0);
                    ViewBag.AllModulo6 = opN.listarOperacionesRolModulo(6, 0);
                    ViewBag.AllModulo7 = opN.listarOperacionesRolModulo(7, 0);
                    return View();
                }
                return RedirectToAction("GestionRolOperacion");
            }
            else if (verificacionAccesos(idOperation) == "E_Login")
            { return RedirectToAction("Index", "Index"); }
            else
            { return RedirectToAction("Error", "Index"); }
        }

        // infos
        public JsonResult infoIdUsuario(int idRol)
        {
            return Json(uN.generarId(idRol));
        }
    }
}
