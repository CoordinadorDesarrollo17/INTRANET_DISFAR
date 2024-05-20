using Capa_Datos.General_DAO.Tablas;
using Capa_Entidad.General_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.Tablas
{
    public class OWHS_N
    {
        OWHS_D oD = new OWHS_D();
        public List<OWHS_E> ListarAlmacenes(string alms = null)
        {
            return oD.ListarAlmacenes(alms);
        }
        public OWHS_E buscarAlmacen(string WhsCode)
        {
            return oD.buscarAlmacen(WhsCode);
        }
    }
}
