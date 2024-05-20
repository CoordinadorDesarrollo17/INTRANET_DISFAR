using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.Tablas
{
    public class RPD1_E
    {
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string BatchNum { get; set; }
        public string ExpDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal NumInBuy { get; set; }
        public string BuyUnitMsr { get; set; }      // UomCode 
    }

}
