using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Compras_ENT.Reportes
{
    public class EspCuadreContrato_E
    {
        public string Rango { get; set; }
        public decimal Rebate { get; set; }
        public decimal CuotaMin { get; set; }
        public string RangoF { get; set; }
        public decimal Displays { get; set; }
        public List<TranCuadreContrato_E> TranProveedor { get; set; }
        public List<TranCuadreContrato_E> TranFabricante { get; set; }
    }
}
