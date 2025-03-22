using Capa_Datos.General_DAO;
using Capa_Datos.RecursosHumanos_DAO;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using Capa_Negocio.RecursosHumanos_NEG.TablasSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class OCARGO_N
    {
        CARGO_D cargoD = new CARGO_D();
        private readonly Helpers helper = new Helpers();

        public List<string> ValidarRegistroCargo(CARGO_E cargo)
        {
            List<string> errores = new List<string>();

            // El cargo que viaja desde POST, debe estar ACTIVO y NO ASIGNADO
            if (!string.IsNullOrWhiteSpace(cargo.Nombre))
            {
                var cargoBuscado = cargoD.ListarCargos(new CARGO_E { Nombre = cargo.Nombre, Estado = "1" });

                if (cargoBuscado != null)
                {
                    errores.Add("El nombre del cargo ya se encuentra registrado.");
                    return errores;
                }
            }
            else
            {
                errores.Add("El nombre del cargo es obligatorio.");
            }

            if (cargo.Nombre != null && cargo.Nombre.Length > 100)
            {
                errores.Add("El nombre del cargo excede el límite de caracteres.");
            }

            // La validación fue exitosa, procede a registrar el cargo
            if (errores.Count == 0)
            {
                cargo.Nombre = helper.SanitizarTexto(cargo.Nombre);
                var mensajeRegistro = cargoD.RegistrarCargo(cargo);
                if (!string.IsNullOrWhiteSpace(mensajeRegistro)) { errores.Add(mensajeRegistro); }
            }

            // Devolver el mensaje de error como una cadena
            return errores;
        }

        public List<CARGO_E> ListarCargos(CARGO_E filtros)
        {
            return cargoD.ListarCargos(filtros);
        }

        public string EditarCargo(CARGO_E datosPOST)
        {
            var datosCargo = cargoD.ObtenerDatosCargo(datosPOST.Id);
            var datosEmpleado = new OEMPL_D().ListarEmpleados(new OEMPL_E { Estado = "1", DatosLaborales = new EMPL1_E { IdCargo = datosPOST.Id } },0);

            if (datosCargo == null)
            {
                return "No se encontró cargo a editar.";
            }

            if (datosEmpleado != null && datosEmpleado.Count >= 1 && datosPOST.Estado == "0")
            {
                return "Para inactivar el cargo, no debe tener ningún empleado relacionado.";
            }

            if (string.IsNullOrWhiteSpace(datosPOST.Estado))
            {
                datosPOST.Estado = datosCargo.Estado;
            }

            return cargoD.EditarCargo(datosPOST);
        }

        public string EliminarCargo(int idCargo)
        {
            var datosCargo = cargoD.ObtenerDatosCargo(idCargo);
            var datosEmpleado = new OEMPL_D().ListarEmpleados(new OEMPL_E { DatosLaborales = new EMPL1_E { IdEMPL1 = idCargo } }, 0);

            if (datosCargo != null)
            {
                if (datosEmpleado != null)
                {
                    return "Para eliminar el cargo, ningún empleado debe encontrarse relacionado.";
                }

                return cargoD.EliminarCargo(idCargo);
            }
            else
            {
                return "No se encontró cargo a eliminar.";
            }
        }
    }
}
