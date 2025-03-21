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
    public class OSEDE_N
    {
        //OSEDE_D sedeD = new OSEDE_D();
        //private readonly Helpers helper = new Helpers();

        //public List<string> ValidarRegistroSede(OSEDE_E sede)
        //{
        //    List<string> errores = new List<string>();

        //    // La sede que viaja desde POST, debe estar ACTIVO y NO ASIGNADO
        //    if (!string.IsNullOrWhiteSpace(sede.Nombre))
        //    {
        //        var sedeBuscada = sedeD.ListarSedes(new OSEDE_E { Nombre = sede.Nombre, Estado = "1" });

        //        if (sedeBuscada != null)
        //        {
        //            errores.Add("El nombre de la sede ya se encuentra registrado.");
        //            return errores;
        //        }
        //    }
        //    else
        //    {
        //        errores.Add("El nombre de la sede es obligatorio.");
        //    }

        //    if (sede.Nombre != null && sede.Nombre.Length > 100)
        //    {
        //        errores.Add("El nombre de la sede excede el límite de caracteres.");
        //    }

        //    // Validar que la dirección de la sede no esté vacía
        //    if (sede.IdUbig <= 0)
        //    {
        //        errores.Add("El ubigeo de la sede es obligatorio.");
        //    }

        //    // La validación fue exitosa, procede a registrar la sede
        //    if (errores.Count == 0)
        //    {
        //        sede.Nombre = helper.SanitizarTexto(sede.Nombre);
        //        var mensajeRegistro = sedeD.RegistrarSede(sede);
        //        if (!string.IsNullOrWhiteSpace(mensajeRegistro)) { errores.Add(mensajeRegistro); }
        //    }

        //    // Devolver el mensaje de error como una cadena
        //    return errores;
        //}

        //public List<OSEDE_E> ListarSedes(OSEDE_E filtros)
        //{
        //    return sedeD.ListarSedes(filtros);
        //}

        //public string EditarSede(OSEDE_E datosPOST)
        //{
        //    var datosSede = sedeD.ObtenerDatosSede(datosPOST.IdSede);
        //    var datosEmpleado = new OEMPL_D().ListarEmpleados(new OEMPL_E { Estado = "1", DatosLaborales = new EMPL1_E { IdSede = datosPOST.IdSede } }, 0);

        //    if (datosSede == null)
        //    {
        //        return "No se encontró sede a editar.";
        //    }

        //    if (datosEmpleado != null && datosEmpleado.Count >= 1 && datosPOST.Estado == "0")
        //    {
        //        return "Para inactivar la sede, no debe tener ningún empleado relacionado.";
        //    }

        //    if (string.IsNullOrWhiteSpace(datosPOST.Estado))
        //    {
        //        datosPOST.Estado = datosSede.Estado;
        //    }

        //    return sedeD.EditarSede(datosPOST);
        //}

        //public string EliminarSede(int idSede)
        //{
        //    var datosSede = sedeD.ObtenerDatosSede(idSede);
        //    var datosEmpleado = new OEMPL_D().ListarEmpleados(new OEMPL_E { DatosLaborales = new EMPL1_E { IdSede = idSede } }, 0);

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
