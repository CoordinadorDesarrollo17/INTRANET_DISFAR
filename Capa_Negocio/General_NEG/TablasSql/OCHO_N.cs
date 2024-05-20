using Capa_Datos.General_DAO.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class OCHO_N
    {
        OCHO_D ochoD = new OCHO_D();
        public OCHO_E buscaChofer(string Id)
        {
            return ochoD.buscaChofer(Id);
        }
        public List<OCHO_E> listaChoferes(int Top, OCHO_E filtro)
        {
            return ochoD.listaChoferes(Top, filtro);
        }
        public void registrarChofer(OCHO_E o)
        {
            validarChofer(o);
            ochoD.registrarChofer(o);
        }
        public void eliminarChofer(string Code)
        {
            ochoD.eliminarChofer(Code);
        }
        public void validarChofer(OCHO_E o)
        {
            bool esVacio(string dato)
            {
                if (dato == null || dato.ToString().Equals("")) { return true; } else { return false; }
            }

            if (o != null)
            {
                OCHO_E o2 = buscaChofer(o.Code);
                if (o2.Code != null) { throw new Exception("Ya existe un chofer registrado con el ID seleccionado"); }
            }

            if (o.Code == null) { throw new Exception("Por favor, primero debe registrar un usuario REPA para el chofer en el módulo Gestión de Permisos."); }
            if (esVacio(o.Name) || o.Name.Length != 8) { throw new Exception("Verificar Dni"); }
            if (esVacio(o.U_SYP_CHLI)) { throw new Exception("No ingreso licencia"); }
            if (esVacio(o.U_SYP_CHNO)) { throw new Exception("No selecciono o ingreso nombre"); }

        }
    }
}
