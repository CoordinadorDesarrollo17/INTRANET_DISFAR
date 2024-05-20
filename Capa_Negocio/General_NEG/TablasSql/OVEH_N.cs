using Capa_Datos.General_DAO.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class OVEH_N
    {
        OVEH_D ovehD = new OVEH_D();
        public OVEH_E buscaVehiculo(string Id)
        {
            return ovehD.buscaVehiculo(Id);
        }
        public List<OVEH_E> listaVeh(int Top, OVEH_E filtro)
        {
            return ovehD.listaVeh(Top, filtro);
        }
        public void registrarVeh(OVEH_E o)
        {
            validarVehiculo(o);
            ovehD.registrarVeh(o);
        }
        public void eliminarVeh(string Code)
        {
            ovehD.eliminarVeh(Code);
        }
        public void validarVehiculo(OVEH_E o)
        {
            bool esVacio(string dato)
            {
                if (dato == null || dato.ToString().Equals("")) { return true; } else { return false; }
            }

            bool validarCode(string Code)
            {
                bool resultado;

                Regex regex = new Regex(@"^[A-z][^$%&@*|<>#]*$");
                Match match = regex.Match(Code);

                resultado = match.Success;
                return resultado;
            }
            if (o != null)
            {
                OVEH_E o2 = buscaVehiculo(o.Code);
                if (o2.Code != null) { throw new Exception("Ya existe un vehiculo registrado con el ID"); }
            }
            if (validarCode(o.Code) == false) { throw new Exception("Ingrese cadena de texto con letra al empezar, sin caracteres especiales"); }
            if (esVacio(o.Name)) { throw new Exception("No ingreso tamaño"); }
            if (esVacio(o.U_SYP_VEMA)) { throw new Exception("No ingreso marca"); }
            if (esVacio(o.U_SYP_VEMO)) { throw new Exception("No ingreso modelo"); }
            if (esVacio(o.U_SYP_VEPL)) { throw new Exception("No ingreso placa"); }
            if (esVacio(o.SerieT1)) { throw new Exception("No ingreso Serie T1"); }
            if (esVacio(o.SerieT2)) { throw new Exception("No ingreso Serie T2"); }
        }
    }
}
