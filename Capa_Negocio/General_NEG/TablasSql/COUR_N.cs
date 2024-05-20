using Capa_Datos.General_DAO.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.TablasSql
{
   public  class COUR_N
    {
        COUR_D cD = new COUR_D();
        public List<COUR_E> Listar()
        {
            return cD.Listar();
        }
        public COUR_E Obtener(string Nombre)
        {
            return cD.Obtener(Nombre);
        }
        public string Registrar(COUR_E bean)
        {
            if (string.IsNullOrEmpty(bean.Nombre)) { return "Debe ingresar nombre"; }
            if (string.IsNullOrEmpty(bean.Ruc) || bean.Ruc.Length!=11) { return "Debe ingresar ruc de 11 dígitos"; }
            if (string.IsNullOrEmpty(bean.DireccionFiscal)) { return "Debe ingresar dirección fiscal"; }
            return cD.Registrar(bean);
        }
    }
}
