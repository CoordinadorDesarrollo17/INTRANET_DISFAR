using Capa_Datos.Ventas_DAO.ReportesHana;
using Capa_Entidad.Ventas_ENT.ReportesHana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.ReportesHana
{
    public class VentSkuDias_N
    {
        VentSkuDias_D oD = new VentSkuDias_D();
        public List<VentSkuDias_E> RptVentSkuDias(DateTime Fecha, string ItemCodeIni, string ItemCodeFin)
        {
            return oD.RptVentSkuDias(Fecha, ItemCodeIni, ItemCodeFin);
        }
    }
}
