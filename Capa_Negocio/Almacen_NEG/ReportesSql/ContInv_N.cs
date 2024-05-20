using Capa_Datos.Almacen_DAO.ReportesSql;
using Capa_Entidad.Almacen_ENT.ReportesSql;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Almacen_NEG.ReportesSql
{
    public class ContInv_N
    {
        ContInv_D contInvD = new ContInv_D();
        public DataTable tbRptContInv(OIAR_E o)
        {
            return contInvD.tbRptContInv(o);
        }
    }
}
