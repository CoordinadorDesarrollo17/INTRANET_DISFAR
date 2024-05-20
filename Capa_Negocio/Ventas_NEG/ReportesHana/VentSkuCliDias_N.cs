using Capa_Datos.Ventas_DAO.ReportesHana;
using Capa_Entidad.Ventas_ENT.ReportesHana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.ReportesHana
{
    public class VentSkuCliDias_N
    {
        VentSkuCliDias_D oD = new VentSkuCliDias_D();
        public List<VentSkuCliDias_E> RptVentSkuCliDias(DateTime Fecha, string ItemCodeIni, string ItemCodeFin
            , string CardCodeIni, string CardCodeFin)
        {
            return oD.RptVentSkuCliDias(Fecha, ItemCodeIni, ItemCodeFin, CardCodeIni, CardCodeFin);
        }
    }
}
