using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Compras_ENT.Reportes
{
    public class BaseResumenRebate_E
    {
        public string Fabricante { get; set; }
        public string Proveedor { get; set; }
        public string Descripcion { get; set; }
        public string SubTipo { get; set; }
        public decimal Rebate { get; set; }
        public string PerIni { get; set; }
        public string PerFin { get; set; }
        public decimal DisplayPactada { get; set; }
        public decimal DisplayActual { get; set; }
        public decimal CuotaPactada { get; set; }
        public decimal TotalComprado { get; set; }
        public decimal Diferencia { get; set; }
        public string Estado { get; set; }
    }
}
