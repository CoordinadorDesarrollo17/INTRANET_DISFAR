using Capa_Datos.General_DAO;
using Capa_Datos.RecursosHumanos_DAO;
using Capa_Datos.RecursosHumanos_DAO.TablasSQL;
using Capa_Entidad.RecursosHumanos_ENT.Reportes;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using Capa_Negocio.General_NEG.TablasSql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.RecursosHumanos_NEG.TablasSQL
{
    public class OEMPL_N
    {
        OEMPL_D emplD = new OEMPL_D();
        EMPL1_D empl1D = new EMPL1_D();
        private readonly Helpers helper = new Helpers();
        public List<OEMPL_E> ListarEmpleados(OEMPL_E filtros)
        {
            return emplD.ListarEmpleados(filtros,null);
        }
        public List<OEMPL_E> ListarEmpleadosConDatosLaborales(OEMPL_E filtrosEmpleados, EMPL1_E filtrosDatosLaborales)
        {
            // Obtener la lista de empleados
            var empleados = emplD.ListarEmpleados(filtrosEmpleados,null);

            // Obtener la lista de datos laborales
            var datosLaborales = empl1D.ListarDatosLaborales(filtrosDatosLaborales);

            // Combinar las listas usando LINQ
            var empleadosConDatosLaborales = empleados.Select(empleado =>
            {
                // Encontrar los datos laborales correspondientes al empleado
                var datosLaboralesEmpleado = datosLaborales.FirstOrDefault(d => d.IdOEMPL == empleado.IdOEMPL);

                // Asignar los datos laborales al empleado
                empleado.DatosLaborales = datosLaboralesEmpleado;

                return empleado;
            }).ToList();

            return empleadosConDatosLaborales;
        }
        public List<string> ValidarRegistroEmpleado(OEMPL_E empleado, EMPL1_E datosLaborales)
        {
            List<string> errores = new List<string>();

            // campo DNI
            if (!string.IsNullOrWhiteSpace(empleado.NroDocumento))
            {
                if (empleado.NroDocumento.Length > 8) { errores.Add("El número de documento del empleado excede el límite de caracteres."); }
                if (helper.EsNumero(empleado.NroDocumento) == false) { errores.Add("El número de documento del empleado debe ser solo números."); }
            }
            else
            {
                errores.Add("El número de documento del empleado es obligatorio.");
            }

            // campo Celular
            if (!string.IsNullOrWhiteSpace(empleado.Celular))
            {
                if (helper.EsNumero(empleado.Celular) == false) { errores.Add("El celular del empleado debe ser solo números."); }
                if (empleado.Celular.Length > 9) { errores.Add("El celular del empleado excede el límite de caracteres."); }
            }

            // Validar que los nombres del empleado no se encuentren vacíos
            if (!string.IsNullOrWhiteSpace(empleado.Nombres))
            {
                if (empleado.Nombres.Length < 3) { errores.Add("El nombre del empleado no válido."); }
                if (empleado.Nombres.Length > 150) { errores.Add("El nombre del empleado excede el límite de caracteres."); }
            }
            else
            {
                errores.Add("El nombre del empleado es obligatorio.");
            }

            // Validar que los apellidos del empleado no se encuentren vacíos
            if (!string.IsNullOrWhiteSpace(empleado.Apellidos))
            {
                if (empleado.Apellidos.Length < 7) { errores.Add("Los apellidos del empleado no son válidos."); }
                if (empleado.Apellidos.Length > 150) { errores.Add("Los apellidos del empleado excede el límite de caracteres."); }
            }
            else
            {
                errores.Add("Los apellidos del empleado es obligatorio.");
            }

            // Validar datos laborales del empleado a registrar
            errores.AddRange(new EMPL1_N().ValidarDatosLaborales(datosLaborales));

            // La validación fue exitosa, procede a registrar el empleado
            if (errores.Count == 0)
            {
                var datosEmpleado = emplD.ListarEmpleados(new OEMPL_E { NroDocumento = empleado.NroDocumento }, 0);
                if (datosEmpleado != null && datosEmpleado.Count >= 1)
                {
                    errores.Add("El número de documento ingresado ya se encuentra registrado.");
                    return errores;
                }

                empleado.Nombres = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(helper.SanitizarTexto(empleado.Nombres).Trim().ToLower());
                empleado.Apellidos = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(helper.SanitizarTexto(empleado.Apellidos).Trim().ToLower());

                var mensajeRegistro = emplD.RegistrarEmpleado(empleado, datosLaborales);
                //if (!string.IsNullOrWhiteSpace(mensajeRegistro)) { errores.Add(mensajeRegistro); }
            }

            // Devolver el mensaje de error como una cadena
            return errores;
        }

        public List<OEMPL_E> ListarEmpleados(OEMPL_E filtros, int? IdRol)
        {
            return emplD.ListarEmpleados(filtros,IdRol);
        }

        public OEMPL_E ObtenerDatosEmpleado(int id, string nroDocumento = "")
        {
            return emplD.ObtenerDatosEmpleado(id, nroDocumento);
        }

        public List<string> EditarEmpleado(OEMPL_E empleadoPOST, EMPL1_E empl1POST)
        {
            List<string> errores = new List<string>();

            var datosEmpleado = emplD.ObtenerDatosEmpleado(empleadoPOST.IdOEMPL);
            var datosLaborales = empl1D.ObtenerDatosLaborales(empleadoPOST.IdOEMPL);

            if (datosEmpleado == null || datosLaborales == null)
            {
                errores.Add("No se encontró empleado a editar.");
                return errores;
            }

            if (!string.IsNullOrWhiteSpace(empleadoPOST.Estado))
            {
                // Para el caso de CESE de un empleado
                if (empleadoPOST.Estado == "0")
                {
                    empl1POST.FechaCese = DateTime.Now.ToString("yyyy-MM-dd");
                }
            }
            else
            {
                empleadoPOST.Estado = datosEmpleado.Estado;
            }

            // Validar datos laborales del empleado a editar
            errores.AddRange(new EMPL1_N().ValidarDatosLaborales(empl1POST));

            // Verificar si el cargo enviado desde POST está activo
            if (empl1POST.IdCargo > 0)
            {
                var cargoBuscado = new CARGO_N().ListarCargos(new CARGO_E { Id = empl1POST.IdCargo, Estado = "1" });

                if (cargoBuscado == null)
                {
                    errores.Add("El cargo escogido no es válido.");
                    return errores;
                }
            }

            // Verificar si se envía el campo cargo vacío pero ya se había registrado con un cargo
            // Esto es obligatorio si el empleado ya tenía un cargo asignado previamente
            if (empl1POST.IdCargo <= 0 && datosLaborales.IdCargo > 0)
            {
                errores.Add("Debe seleccionar un cargo.");
                return errores;
            }

            // La validación fue exitosa, procede a editar el empleado
            if (errores.Count == 0)
            {
                empleadoPOST.Nombres = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(helper.SanitizarTexto(empleadoPOST.Nombres).Trim().ToLower());
                empleadoPOST.Apellidos = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(helper.SanitizarTexto(empleadoPOST.Apellidos).Trim().ToLower());
                var mensajeRegistro = emplD.EditarEmpleado(empleadoPOST, empl1POST);
                if (!string.IsNullOrWhiteSpace(mensajeRegistro)) { errores.Add(mensajeRegistro); }
            }

            return errores;
        }

        public string EliminarEmpleado(int id)
        {
            var datosEmpleado = emplD.ObtenerDatosEmpleado(id);

            if (datosEmpleado != null)
            {
                return emplD.EliminarEmpleado(id);
            }
            else
            {
                return "No se encontró empleado a eliminar.";
            }
        }

        public List<RptEmpleados_E> ExportarListaEmpleados(OEMPL_E filtros)
        {
            return emplD.ExportarListaEmpleados(filtros);
        }
    }
}
