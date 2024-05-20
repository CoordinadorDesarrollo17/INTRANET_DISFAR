using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.Tablas
{
    public class OPDN_E
    {
        public int DocNum { get; set; }
        public string DocDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Address { get; set; }
        public string U_COB_LUGAREN { get; set; }
        public string FirmCode { get; set; }
        public string FirmName { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string BatchNum { get; set; }
        public string ExpDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal QuantityOIBT { get; set; }
        public decimal NumInBuy { get; set; }
        public string BuyUnitMsr { get; set; }
        public string NumAtCard { get; set; }
        public bool SinEM { get; set; }
        public bool RM { get; set; }

    }
}
