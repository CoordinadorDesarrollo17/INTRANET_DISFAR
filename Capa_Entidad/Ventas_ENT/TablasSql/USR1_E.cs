using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class USR1_E
    {
        public int DocEntry { get; set; }
        public string Usuario { get; set; }
		//cuotas de venta de usuario
		public int YearU { get; set; }
        public int MonthU { get; set; }
        public decimal Cuota { get; set; }
        public string FechaRegistro { get; set; }
        public string HoraRegistro { get; set; }
        public string OpRegistro { get; set; }
        // parametro no de tabla
        public string Nombres { get; set; }
    }
}
