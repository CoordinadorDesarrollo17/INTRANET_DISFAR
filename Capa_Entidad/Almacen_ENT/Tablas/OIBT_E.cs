using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.Tablas
{
    public class OIBT_E
    {
        //tablas stock articulos lotes
        public int Id { get; set; }
        public string Laboratorio { get; set; }
        public string ItemCode { get; set; }
        public string BatchNum { get; set; }
        public string WhsCode { get; set; }
        public string ItemName { get; set; }
        public int FirmCode { get; set; }
        public string FirmName { get; set; }
        public string ExpDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string BuyUnitMsr { get; set; }
        public string UMVenta { get; set; }
        public decimal NumInBuy { get; set; }
        public string PrincActivo { get; set; }
        public string Observacion { get; set; }
        public string CajonM { get; set; }
        public decimal PrecioxCaja { get; set; }
        public decimal OnHand { get; set; }
        public decimal IsCommited { get; set; }
        public decimal PorVender { get; set; }      // Usado para el cálculo PorVender de Sophos
        public string SalUnitMsr { get; set; }

        // Para vt.RDR1
        public string UndMed { get; set; }
        public int Cantidad { get; set; }
    }
}
