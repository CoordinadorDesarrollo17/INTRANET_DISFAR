using Capa_Datos.Ventas_DAO.ReportesHana;
using Capa_Entidad.Ventas_ENT.ReportesHana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.ReportesHana
{
    public class VentVendDias_N
    {
        VentVendDias_D oD = new VentVendDias_D();
        public List<VentVendDias_E> RptVentVendDias(DateTime Fecha)
        {
            return oD.RptVentVendDias(Fecha);
        }
    }
}
