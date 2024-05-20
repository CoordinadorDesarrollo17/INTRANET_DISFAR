using Capa_Datos.Ventas_DAO.ReportesHana;
using Capa_Entidad.Ventas_ENT.ReportesHana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.ReportesHana
{
    public class VentCliDias_N
    {
        VentCliDias_D oD = new VentCliDias_D();
        public List<VentCliDias_E> RptVentCliDias(DateTime Fecha, string CardCodeIni, string CardCodeFin)
        {
            return oD.RptVentCliDias(Fecha, CardCodeIni, CardCodeFin);
        }
    }
}
