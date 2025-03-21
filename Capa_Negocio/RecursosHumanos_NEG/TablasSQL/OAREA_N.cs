using Capa_Datos.RecursosHumanos_DAO.TablasSQL;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.RecursosHumanos_NEG.TablasSQL
{
    public class OAREA_N
    {
        OAREA_D areaD = new OAREA_D();
        private readonly Helpers helper = new Helpers();

        //public List<string> ValidarRegistroArea(OAREA_E area)
        //{
        //    List<string> errores = new List<string>();

        //    // Validar que el nombre del area no esté vacío
        //    if (string.IsNullOrWhiteSpace(area.Nombre))
        //    {
        //        errores.Add("El nombre del área es obligatorio.");
        //    }

        //    if (area.Nombre != null && area.Nombre.Length > 100)
        //    {
        //        errores.Add("El nombre del área excede el límite de caracteres.");
        //    }

        //    // La validación fue exitosa, procede a registrar el área
        //    if (errores.Count == 0)
        //    {
        //        area.Nombre = helper.SanitizarTexto(area.Nombre);
        //        var mensajeRegistro = areaD.RegistrarArea(area);
        //        if (!string.IsNullOrWhiteSpace(mensajeRegistro)) { errores.Add(mensajeRegistro); }
        //    }

        //    // Devolver el mensaje de error como una cadena
        //    return errores;
        //}

        public List<OAREA_E> ListarAreas(OAREA_E filtros)
        {
            return areaD.ListarAreas(filtros);
        }

        //public string EditarArea(OAREA_E datosPOST)
        //{
        //    var datosArea = areaD.ObtenerDatosArea(datosPOST.IdArea);

        //    if (datosArea == null)
        //    {
        //        return "No se encontró área a editar.";
        //    }

        //    if (string.IsNullOrWhiteSpace(datosPOST.Estado))
        //    {
        //        return "Seleccionar el estado al que deseas cambiar.";
        //    }

        //    return areaD.EditarArea(datosPOST);
        //}

        //public string EliminarArea(int idArea)
        //{
        //    var datosArea = areaD.ObtenerDatosArea(idArea);

        //    if (datosArea != null)
        //    {
        //        return areaD.EliminarArea(idArea);
        //    }
        //    else
        //    {
        //        return "No se encontró área a eliminar.";
        //    }
        //}
    }
}
