using Capa_Datos.General_DAO.Tablas;
using Capa_Entidad.General_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.Tablas
{
    public class COB_LUG_ENTREGA_N
    {
        COB_LUG_ENTREGA_D cD = new COB_LUG_ENTREGA_D();
        public List<COB_LUG_ENTREGA_E> listadoLugaresDeEntrega()
        {
            return cD.listadoLugaresDeEntrega();
        }
    }
}
