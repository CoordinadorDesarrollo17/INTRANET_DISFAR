using Capa_Datos.General_DAO.Tablas;
using Capa_Entidad.General_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.Tablas
{
    public class OSLP_N
    {
        OSLP_D osD = new OSLP_D();
        public List<OSLP_E> listadoOslp(string Memo)
        {
            return osD.listadoOslp(Memo);
        }
    }
}
