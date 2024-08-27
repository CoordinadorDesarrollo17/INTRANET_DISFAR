using Capa_Datos.General_DAO;
using Capa_Datos.RecursosHumanos_DAO;
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
            return emplD.ListarEmpleados(filtros);
        }

        public List<OEMPL_E> ListarEmpleadosConDatosLaborales(OEMPL_E filtrosEmpleados, EMPL1_E filtrosDatosLaborales)
        {
            // Obtener la lista de empleados
            var empleados = emplD.ListarEmpleados(filtrosEmpleados);

            // Obtener la lista de datos laborales
            var datosLaborales = empl1D.ListarDatosLaborales(filtrosDatosLaborales);

            // Combinar las listas usando LINQ
            var empleadosConDatosLaborales = empleados.Select(empleado =>
            {
                // Encontrar los datos laborales correspondientes al empleado
                var datosLaboralesEmpleado = datosLaborales.FirstOrDefault(d => d.EmpleadoID == empleado.Id);

                // Asignar los datos laborales al empleado
                empleado.DatosLaborales = datosLaboralesEmpleado;

                return empleado;
            }).ToList();

            return empleadosConDatosLaborales;
        }


    }
}
