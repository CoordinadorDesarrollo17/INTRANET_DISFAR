using Capa_Datos.General_DAO;
using Capa_Datos.RecursosHumanos_DAO;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class SEDE_N
    {
        SEDE_D sedeD = new SEDE_D();
        private readonly Helpers helper = new Helpers();

        public List<string> ValidarRegistroSede(SEDE_E sede)
        {
            List<string> errores = new List<string>();

            // La sede que viaja desde POST, debe estar ACTIVO y NO ASIGNADO
            if (!string.IsNullOrWhiteSpace(sede.Nombre))
            {
                var sedeBuscada = sedeD.ListarSedes(new SEDE_E { Nombre = sede.Nombre, Estado = "1" });

                if (sedeBuscada != null)
                {
                    errores.Add("El nombre de la sede ya se encuentra registrado.");
                    return errores;
                }
            }
            else
            {
                errores.Add("El nombre de la sede es obligatorio.");
            }

            if (sede.Nombre != null && sede.Nombre.Length > 100)
            {
                errores.Add("El nombre de la sede excede el límite de caracteres.");
            }

            // Validar que la dirección de la sede no esté vacía
            if (sede.UbigeoID <= 0)
            {
                errores.Add("El ubigeo de la sede es obligatorio.");
            }

            // La validación fue exitosa, procede a registrar la sede
            if (errores.Count == 0)
            {
                sede.Nombre = helper.SanitizarTexto(sede.Nombre);
                var mensajeRegistro = sedeD.RegistrarSede(sede);
                if (!string.IsNullOrWhiteSpace(mensajeRegistro)) { errores.Add(mensajeRegistro); }
            }

            // Devolver el mensaje de error como una cadena
            return errores;
        }

        public List<SEDE_E> ListarSedes(SEDE_E filtros)
        {
            return sedeD.ListarSedes(filtros);
        }

        public List<SEDE_E> ListarSedesParaCrearUsuario(SEDE_E filtros)
        {
            List<SEDE_E> sedes = sedeD.ListarSedes(filtros);

            // Excluir todos los elementos que no tienen Id 1, 2, 4, o 5
            sedes.RemoveAll(s => !(s.Id == 1 || s.Id == 2 || s.Id == 4 || s.Id == 5));

            // Actualizar los IDs de acuerdo con la antigua estructura del campo "Almacén" en "TI_Sistemas/CrearUsuario"
            foreach (var sede in sedes)
            {
                switch (sede.Id)
                {
                    case 1:
                        sede.IdAlterno = "01";
                        break;
                    case 2:
                        sede.IdAlterno = "03";
                        break;
                    case 4:
                        sede.IdAlterno = "06";
                        break;
                    case 5:
                        sede.IdAlterno = "07";
                        break;
                }
            }

            return sedes;
        }

        public string EditarSede(SEDE_E datosPOST)
        {
            var datosSede = sedeD.ObtenerDatosSede(datosPOST.Id);
            var datosEmpleado = new OEMPL_D().ListarEmpleados(new OEMPL_E { Estado = "1", DatosLaborales = new EMPL1_E { SedeID = datosPOST.Id } },null);

            if (datosSede == null)
            {
                return "No se encontró sede a editar.";
            }

            if (datosEmpleado != null && datosEmpleado.Count >= 1 && datosPOST.Estado == "0")
            {
                return "Para inactivar la sede, no debe tener ningún empleado relacionado.";
            }

            if (string.IsNullOrWhiteSpace(datosPOST.Estado))
            {
                datosPOST.Estado = datosSede.Estado;
            }

            return sedeD.EditarSede(datosPOST);
        }

        //public string EliminarSede(int idSede)
        //{
        //    var datosSede = sedeD.ObtenerDatosSede(idSede);
        //    var datosEmpleado = new OEMPL_D().ListarEmpleados(new OEMPL_E { DatosLaborales = new EMPL1_E { IdSede = idSede } });

        //    if (datosSede != null)
        //    {
        //        if (datosEmpleado != null)
        //        {
        //            return "Para eliminar la sede, ningún empleado debe encontrarse relacionado.";
        //        }

        //        return sedeD.EliminarSede(idSede);
        //    }
        //    else
        //    {
        //        return "No se encontró sede a eliminar.";
        //    }
        //}
    }
}
