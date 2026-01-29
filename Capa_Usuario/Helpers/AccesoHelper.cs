using Capa_Entidad;
using Capa_Entidad.Seguridad_ENT;
using Capa_Negocio.Seguridad_NEG;
using Capa_Negocio.Seguridad_NEG.TablasSql;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Capa_Usuario.Helpers
{
    public static class AccesoHelper
    {
        public static int ObtenerDesignacionModulos(string nombreControlador)
        {
            Dictionary<string, int> designacionModulos = new Dictionary<string, int>
            {
                { "Almacen", 2 },
                { "AtencionCliente", 7 },
                { "Caja", 8 },
                { "Compras", 5 },
                { "DireccionTecnica", 3 },
                { "ReportesDigemid", 3 },
                { "Rutas", 2 },
                { "TI_Sistemas", 1 },
                { "Ventas", 5 },
            };

            // Utiliza TryGetValue para intentar obtener el valor asociado a la clave
            if (designacionModulos.TryGetValue(nombreControlador, out int valor))
            {
                return valor;
            }

            // Devuelve 0 si la clave no se encuentra en el diccionario
            return 0;
        }

        public static ActionResult GestionarAccesoIndex(Controller controller, AccessoHelper_E accesoHelper)
        {
            string accesoResultado = VerificarAccesos(accesoHelper.OpeID, accesoHelper.usuario, accesoHelper.controllerDestino, accesoHelper.userHostAddress, accesoHelper.userHostName);

            switch (accesoResultado)
            {
                case "C_Access":
                    return Redirect(controller, accesoHelper.action, accesoHelper.controllerDestino);

                case "E_Login":
                    return Redirect(controller, "Index", "Index");

                default:
                    return Redirect(controller, "Error", "Index");
            }
        }

        public static ActionResult GestionarAccesoController(Controller controller, AccessoHelper_E accesoHelper)
        {
            string accesoResultado = VerificarAccesos(accesoHelper.OpeID, accesoHelper.usuario, accesoHelper.controllerDestino, accesoHelper.userHostAddress, accesoHelper.userHostName);

            switch (accesoResultado)
            {
                case "C_Access":
                    return new HttpStatusCodeResult(200); // Indicador de acceso permitido

                case "E_Login":
                    return Redirect(controller, "Index", "Index");

                default:
                    return Redirect(controller, "Error", "Index");
            }
        }

        internal static string VerificarAccesos(int ope, Usuario_E usu, string nombreOperacion, string userHostAddress, string userHostName)
        {
            if (usu == null)
            {
                return "E_Login";
            }

            var accesoRol = new ROL_OPE_N().VerificarAccesoOperacion(usu.IdRol, ope);
            var accesoPersonalizado = new OUSR_OPE_N().VerificarAccesoOperacion(new OUSR_OPE_E { OpeID = ope, UsrDocEntry = usu.DocEntry });

            //int modulo = ObtenerDesignacionModulos(nombreOperacion);
            if (usu.IdRol == 1 || accesoRol == 1 || accesoPersonalizado == 1)
            {
                new Capa_Negocio.Utilitarios_N().RegistrarLog($"{usu.Prefijo} {usu.Id}", $"intento de {nombreOperacion}", ope, userHostAddress, userHostName);
                return "C_Access";
            }
            else
            {
                return "E_Access";
            }
        }

        
        public static string MostrarBotonSiTienePermiso(int opeID, Usuario_E usuario, string botonHtml)
        {
            if (usuario == null)
            {
                return string.Empty;
            }

            // Verificar si el usuario tiene acceso mediante su rol o permisos personalizados
            var accesoRol = new ROL_OPE_N().VerificarAccesoOperacion(usuario.IdRol, opeID);
            var accesoPersonalizado = new OUSR_OPE_N().VerificarAccesoOperacion(new OUSR_OPE_E { OpeID = opeID, UsrDocEntry = usuario.DocEntry });

            // Si es administrador (IdRol == 1) o tiene permiso por rol o personalizado
            if (usuario.IdRol == 1 || accesoRol == 1 || accesoPersonalizado == 1)
            {
                return botonHtml;
            }

            return string.Empty;
        }

        private static ActionResult Redirect(Controller controller, string action, string controllerName)
        {
            string url = new UrlHelper(controller.Request.RequestContext).Action(action, controllerName);
            controller.Response.Redirect(url);
            return new EmptyResult();
        }
    }
}