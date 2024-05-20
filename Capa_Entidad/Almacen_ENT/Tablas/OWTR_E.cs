using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.Tablas
{
    public class OWTR_E
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocDate { get; set; }
        [DisplayName("Origen")]
        public string Filler { get; set; }
        [DisplayName("Destino")]
        public string ToWhsCode { get; set; }
        [DisplayName("Operario")]
        public int SlpCode { get; set; }
        public string U_SYP_MDTD { get; set; }
        public string U_SYP_MDSD { get; set; }
        public string U_SYP_MDCD { get; set; }
        [DisplayName("EstadoDoc")]
        public string U_SYP_STATUS { get; set; }

		[DisplayName("Fecha Traslado")]
		public string U_BPP_FECINITRA { get; set; }
        /************************ C A M P O S   Q U E   N O   S O N   D E   L A   T A B L A ************************/
        public string Estado { get; set; }
    }
}
