using Capa_Datos.RecursosHumanos_DAO;
using Capa_Datos.RecursosHumanos_DAO.TablasSQL;
using Capa_Entidad.RecursosHumanos_ENT.Reportes;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.RecursosHumanos_NEG.TablasSQL
{
    public class ONUM_N
    {
        ONUM_D numD = new ONUM_D();

        public List<string> ValidarRegistroNumero(ONUM_E numero)
        {
            List<string> errores = new List<string>();

            // El numero que viaja desde POST, debe estar ACTIVO y NO ASIGNADO
            if (!string.IsNullOrWhiteSpace(numero.NumeroCorporativo))
            {
                var numBuscado = new ONUM_N().ListarNumeros(new ONUM_E { NumeroCorporativo = numero.NumeroCorporativo, Estado = "1", Asignado = "0" });

                if (numBuscado != null)
                {
                    errores.Add("El número ingresado ya se encuentra registrado.");
                    return errores;
                }
            }
            else
            {
                errores.Add("El número corporativo es obligatorio.");
            }

            if (numero.NumeroCorporativo != null && numero.NumeroCorporativo.Length > 9)
            {
                errores.Add("El número corporativo excede el límite de caracteres.");
            }

            if (string.IsNullOrWhiteSpace(numero.Operador))
            {
                errores.Add("El operador es obligatorio.");
            }

            // La validación fue exitosa, procede a registrar el número
            if (errores.Count == 0)
            {
                var mensajeRegistro = numD.RegistrarNumero(numero);
                if (!string.IsNullOrWhiteSpace(mensajeRegistro)) { errores.Add(mensajeRegistro); }
            }

            // Devolver el mensaje de error como una cadena
            return errores;
        }

        public List<ONUM_E> ListarNumeros(ONUM_E filtros)
        {
            return numD.ListarNumeros(filtros);
        }

        public string EditarNumero(ONUM_E datosPOST)
        {
            var datosNumero = numD.ObtenerDatosNumero(datosPOST.IdNumero);

            if (datosNumero == null)
            {
                return "No se encontró el número a editar.";
            }

            if (datosNumero != null && datosPOST.Estado == "0" && datosNumero.Asignado == "1")
            {
                return "Para inactivar el número corporativo, no debe encontrarse asignado a un empleado.";
            }

            if (string.IsNullOrWhiteSpace(datosPOST.Operador))
            {
                datosPOST.Operador = datosNumero.Operador;
            }

            if (string.IsNullOrWhiteSpace(datosPOST.Estado))
            {
                datosPOST.Estado = datosNumero.Estado;
            }

            return numD.EditarNumero(datosPOST);
        }

        public string EliminarNumero(int idNumero)
        {
            var datosNumero = numD.ObtenerDatosNumero(idNumero);

            if (datosNumero != null)
            {
                // false: no es reasignación desde registro empleado
                if (datosNumero.Asignado == "1")
                {
                    return "Para eliminar el número corporativo, no debe encontrarse asignado a un empleado.";
                }

                return numD.EliminarNumero(idNumero);
            }
            else
            {
                return "No se encontró el número a eliminar.";
            }
        }

        public ONUM_E ObtenerDatosNumero(int idNumero)
        {
            return numD.ObtenerDatosNumero(idNumero);
        }

        public string LiberarNumero(int idNumero, string nroDocumento, string origen = "")
        {
            var datosNumero = numD.ObtenerDatosNumero(idNumero);
            var datosEmpleado = new OEMPL_N().ListarEmpleados(new OEMPL_E { NroDocumento = nroDocumento },0);
            bool accionEditarEmpleado = (origen == "EditarEmpleado");

            if (datosEmpleado == null)
            {
                return "No se encontró el número asignado al empleado seleccionadodo.";
            }

            if (datosNumero == null)
            {
                return "No se encontró el número a liberar.";
            }

            if (datosNumero.Asignado == "0" && accionEditarEmpleado == false)
            {
                return "El número corporativo ya se encuentra liberado. Actualizar el listado.";
            }

            return numD.LiberarNumero(idNumero, datosEmpleado[0].Id);
        }

        public List<RptNumerosCorporativos_E> ExportarListaNumeros(RptNumerosCorporativos_E filtros)
        {
            return numD.ExportarListaNumeros(filtros);
        }
    }
}
