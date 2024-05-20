using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.ReportesHana
{
    public class AnCtVentas_E
    {
        //estructura analitica ventas de cuotas
        public int DocEntry { get; set; }
        public string nombre { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int Dia { get; set; }
        public decimal TotVendCliDia { get; set; }
        public decimal TotVendDia { get; set; }
        public decimal TotVendRg { get; set; }
        public int TotCliAtRg { get; set; }
        public decimal TotNcCliDia { get; set; }
        public decimal PromTicket { get; set; }
        public int TotPagTk { get; set; }
        public int TotAnTk { get; set; }
        public int TotPendTk { get; set; }
        public decimal Cuota { get; set; }

        // metodos

    }
}
