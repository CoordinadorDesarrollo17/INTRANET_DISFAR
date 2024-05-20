using Capa_Entidad.Almacen_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.Tablas
{
    //DEVOLUCION DE MERCANCIA DE HANA
    public class ORPD_E
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        /*******/
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string WhsCode { get; set; }
        public int FirmCode { get; set; }
        public string BatchNum { get; set; }
        public string ExpDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal NumInBuy { get; set; }
        public string BuyUnitMsr { get; set; }
        public string RefFactura { get; set; }
public List<RPD1_E> DetalleDevolucion { get; set; }                  
    }
}
