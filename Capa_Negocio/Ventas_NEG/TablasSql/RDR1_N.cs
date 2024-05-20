using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
    public class RDR1_N
    {
        RDR1_D rdr1D = new RDR1_D();
        public List<ExcelDetallePedido> ExportarDetallePedidosOnline(int IdORDR)
        {
            return rdr1D.ExportarDetallePedidosOnline(IdORDR);
        }
    }
}
