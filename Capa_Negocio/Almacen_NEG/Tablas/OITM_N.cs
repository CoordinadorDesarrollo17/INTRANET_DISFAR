using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Almacen_NEG.Tablas
{
    public class OITM_N
    {
        OITM_D oD = new OITM_D();

        public List<OITM_E> Listar(OITM_E fil)
        {
            return oD.Listar(fil);
        }
        public string datalistArticulosLabo(OITM_E fil)
        {
            return oD.datalistArticulosLabo(fil);
        }
        public OITM_E buscarArticulo(string ItemCode)
        {
            return oD.Obtener(ItemCode);
        }
        public string datalistArticulos(OITM_E fil)
        {
            return oD.datalistArticulos(fil);
        }
    }
}
