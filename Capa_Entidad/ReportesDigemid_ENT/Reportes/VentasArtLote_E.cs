using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.ReportesDigemid_ENT.Reportes
{
    public class VentasArtLote_E
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string BatchNum { get; set; }
        public DateTime DocDate { get; set; }
        public string Comprobante { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public decimal CantPza { get; set; }
        public decimal CantCja { get; set; }
        public string Vendedor { get; set; }
        public string Correo { get; set; }
    }
    
}
