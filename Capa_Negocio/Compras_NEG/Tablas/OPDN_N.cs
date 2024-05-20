using Capa_Datos.Compras_DAO.Tablas;
using Capa_Entidad.Compras_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Compras_NEG.Tablas
{
    public class OPDN_N
    {
        OPDN_D oD = new OPDN_D();
        public List<OPDN_E> listadoEntradaMercancias(OPDN_E fil)
        {
            return oD.listadoEntradaMercancias(fil);
        }
        public OPDN_E buscarEntradaMercancias(int DocEntry)
        {
            return oD.buscarEntradaMercancias(DocEntry);
        }
    }
}
