using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
    public class OTRC_N
    {
        OTRC_D otrcD = new OTRC_D();
        public List<OTRC_E.RptTransacciones> listarTransacciones(OTRC_E filtro)
        {
            return otrcD.listarTransacciones(filtro);
        }
    }
}
