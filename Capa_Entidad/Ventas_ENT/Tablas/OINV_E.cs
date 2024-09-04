using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.Tablas
{
    public class OINV_E
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string CANCELED { get; set; }
        [DisplayName("FechaCont")]
        public string DocDate { get; set; }
        [DisplayName("Nombre")]
        public string CardName { get; set; }
        [DisplayName("Comprobante")]
        public string NumAtCard { get; set; }
        [DisplayName("Total")]
        public decimal DocTotal { get; set; }
        public decimal Max1099 { get; set; }
        [DisplayName("Estado")]
        public string U_SYP_STATUS { get; set; }
        [DisplayName("LugarDeEntrega")]
        public string U_COB_LUGAREN { get; set; }
        public string U_COB_TIPODOC { get; set; }
        public string U_COB_SERIE { get; set; }
        public string U_COB_CORDOC { get; set; }

		[DisplayName("Fecha Traslado")]
		public string U_BPP_FECINITRA { get; set; }
	}
}
