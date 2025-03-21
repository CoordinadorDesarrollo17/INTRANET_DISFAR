using Capa_Datos.General_DAO;
using Capa_Datos.RecursosHumanos_DAO;
using Capa_Datos.RecursosHumanos_DAO.TablasSQL;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using Capa_Negocio.General_NEG.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebSockets;

namespace Capa_Negocio.RecursosHumanos_NEG.TablasSQL
{
    public class EMPL1_N
    {
        EMPL1_D empl1D = new EMPL1_D();
        private readonly Helpers helper = new Helpers();

        public List<string> ValidarDatosLaborales(EMPL1_E datosLaborales)
        {
            List<string> errores = new List<string>();
            var datosEmpleado = new OEMPL_D().ObtenerDatosEmpleado(datosLaborales.Id);

            if (string.IsNullOrWhiteSpace(datosLaborales.CondicionLaboral))
            {
                errores.Add("Debe seleccionar una condición laboral.");
            }

            if (datosLaborales.SedeID <= 0)
            {
                errores.Add("Debe seleccionar una sede.");
            }

            if (datosLaborales.DepartamentoID <= 0)
            {
                errores.Add("Debe seleccionar un departamento.");
            }

            if (datosLaborales.AreaID <= 0)
            {
                errores.Add("Debe seleccionar un área.");
            }

            if (datosLaborales.AnexoCorporativo != null && datosLaborales.AnexoCorporativo.Length > 3)
            {
                errores.Add("El anexo corporativo excede el límite de caracteres.");
            }

            if (datosLaborales.CorreoCorporativo != null)
            {
                if (datosLaborales.CorreoCorporativo.Length > 100) { errores.Add("El correo corporativo excede el límite de caracteres."); }
                if (helper.EsCorreo(datosLaborales.CorreoCorporativo) == false) { errores.Add("Ingresar un correo válido."); }
            }

            if (!string.IsNullOrWhiteSpace(datosLaborales.NumeroCorporativo) && string.IsNullOrWhiteSpace(datosLaborales.FechaCese))
            {
                if (datosEmpleado.Estado == "0")
                {
                    errores.Add("Solo puedes asignar un número corporativo a empleados activos.");
                }

                var numBuscado = new ONUM_N().ListarNumeros(new ONUM_E { IdNumero = datosLaborales.NumeroCorporativoID, Estado = "1", Asignado = "0" });
                if (numBuscado == null && datosLaborales.NumeroCorporativoID <= 0)
                {
                    errores.Add("El número escogido no es válido.");
                }
            }

            // Devolver el mensaje de error como una cadena
            return errores;
        }

        public EMPL1_E ObtenerDatosLaborales(int idOEMPL)
        {
            return empl1D.ObtenerDatosLaborales(idOEMPL);
        }

        public List<string> BuscarAnexoCorreoDuplicado(string anexo, string correoCorporativo, int id)
        {
            List<string> errores = new List<string>();
            var duplicados = empl1D.BuscarAnexoCorreoDuplicado(anexo, correoCorporativo, id);

            if (duplicados != null)
            {
                if (duplicados.ContainsKey("AnexoCorporativo") && !string.IsNullOrWhiteSpace(duplicados["AnexoCorporativo"]))
                {
                    errores.Add("El anexo ingresado ya se encuentra asignado a un empleado.");
                }

                if (duplicados.ContainsKey("CorreoCorporativo") && !string.IsNullOrWhiteSpace(duplicados["CorreoCorporativo"]))
                {
                    errores.Add("El correo corporativo ingresado ya se encuentra asignado a un empleado.");
                }
            }

            return errores;
        }
    }
}
