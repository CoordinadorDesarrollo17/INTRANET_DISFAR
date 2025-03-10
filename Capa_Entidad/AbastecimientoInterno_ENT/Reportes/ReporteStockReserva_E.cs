using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.AbastecimientoInterno_ENT.Reportes
{
    public class ReporteStockReserva_E
    {
        public int UbicacionId { get; set; }
        public int UbicacionLoteId { get; set; }
        public int UbicacionLoteMasterId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CodigoUbicacion { get; set; }
        public string BatchNum { get; set; }
        public string UmAlm { get; set; }
        public int ValorUmAlm { get; set; }
        public int QuantityMaster { get; set; }
        public int QuantitySaldo { get; set; }
        public int QuantityUnidadesCajas { get; set; }
    }
}
