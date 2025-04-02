using Capa_Datos.RecursosHumanos_DAO.TablasSQL;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.RecursosHumanos_NEG.TablasSQL
{
    public class ODPTO_N
    {
        ODPTO_D dptoD = new ODPTO_D();
        private readonly Helpers helper = new Helpers();

        //public List<string> ValidarRegistroDepartamento(ODPTO_E departamento)
        //{
        //    List<string> errores = new List<string>();

        //    // Validar que el nombre del departamento no esté vacío
        //    if (string.IsNullOrWhiteSpace(departamento.Nombre))
        //    {
        //        errores.Add("El nombre del departamento es obligatorio.");
        //    }

        //    if (departamento.Nombre != null && departamento.Nombre.Length > 100)
        //    {
        //        errores.Add("El nombre del departamento excede el límite de caracteres.");
        //    }

        //    // La validación fue exitosa, procede a registrar el departamento
        //    if (errores.Count == 0)
        //    {
        //        departamento.Nombre = helper.SanitizarTexto(departamento.Nombre);
        //        var mensajeRegistro = dptoD.RegistrarDepartamento(departamento);
        //        if (!string.IsNullOrWhiteSpace(mensajeRegistro)) { errores.Add(mensajeRegistro); }
        //    }

        //    // Devolver el mensaje de error como una cadena
        //    return errores;
        //}

        public List<ODPTO_E> ListarDepartamentos(ODPTO_E filtros)
        {
            return dptoD.ListarDepartamentos(filtros);
        }

        //public string EditarDepartamento(ODPTO_E datosPOST)
        //{
        //    var datosDepartamento = dptoD.ObtenerDatosDepartamento(datosPOST.IdDepartamento);

        //    if (datosDepartamento == null)
        //    {
        //        return "No se encontró departamento a editar.";
        //    }

        //    if (string.IsNullOrWhiteSpace(datosPOST.Estado))
        //    {
        //        return "Seleccionar el estado al que deseas cambiar.";
        //    }

        //    return dptoD.EditarDepartamento(datosPOST);
        //}

        //public string EliminarDepartamento(int idDepartamento)
        //{
        //    var datosDepartamento = dptoD.ObtenerDatosDepartamento(idDepartamento);

        //    if (datosDepartamento != null)
        //    {
        //        return dptoD.EliminarDepartamento(idDepartamento);
        //    }
        //    else
        //    {
        //        return "No se encontró departamento a eliminar.";
        //    }
        //}

    }
}
