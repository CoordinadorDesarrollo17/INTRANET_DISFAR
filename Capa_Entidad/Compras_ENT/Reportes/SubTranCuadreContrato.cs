using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Compras_ENT.Reportes
{
    public class SubTranCuadreContrato
    {
        public string TaxDate { get; set; }
        public string NumAtCard { get; set; }
        public decimal DocTotal { get; set; }
        public int DocEntry { get; set; }
        public string SoloSuma { get; set; }
        public decimal Displays { get; set; }
    }
}
