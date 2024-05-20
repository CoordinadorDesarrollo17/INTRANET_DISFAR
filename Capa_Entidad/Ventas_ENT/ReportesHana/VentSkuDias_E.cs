using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.ReportesHana
{
    public class VentSkuDias_E
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Total { get; set; }
        public decimal TotalCant { get; set; }
        public decimal TotDia1 { get; set; }
        public decimal TotDia2 { get; set; }
        public decimal TotDia3 { get; set; }
        public decimal TotDia4 { get; set; }
        public decimal TotDia5 { get; set; }
        public decimal TotDia6 { get; set; }
        public decimal TotDia7 { get; set; }
        public decimal CantDia1 { get; set; }
        public decimal CantDia2 { get; set; }
        public decimal CantDia3 { get; set; }
        public decimal CantDia4 { get; set; }
        public decimal CantDia5 { get; set; }
        public decimal CantDia6 { get; set; }
        public decimal CantDia7 { get; set; }
        public void CalcTotal()
        {
            Total = TotDia1 + TotDia2 + TotDia3 + TotDia4 + TotDia5 + TotDia6 + TotDia7;
            TotalCant = CantDia1 + CantDia2 + CantDia3 + CantDia4 + CantDia5 + CantDia6 + CantDia7;
        }
    }
}
