using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Almacen_NEG.Tablas
{
    public class OITW_N
    {
        OITW_D oD = new OITW_D();
        public List<OITW_E> listarDetArticulosInv(OITW_E obj)
        {
            return oD.listarDetArticulosInv(obj);
        }
    }
}
