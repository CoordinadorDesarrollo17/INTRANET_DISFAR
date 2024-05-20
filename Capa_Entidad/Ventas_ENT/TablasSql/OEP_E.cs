using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
	public class OEP_E
	{
		public int IdOEP { get; set; }
		public int DocEntryTicket { get; set; }
		public int DocNumTicket { get; set; }
		public int IdTipoErrorPicking { get; set; }
		public string Estado { get; set; }
		public string OpRegistro{ get; set; }
		public string FechaRegistro { get; set; }
		public string HoraRegistro { get; set; }
	}
}
