using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
	public class CC_ORTV_E
	{
		public int Id { get; set; }
		public int DocEntry { get; set; }
		public string Operacion { get; set; }
		public string Operario { get; set; }
		public string FechaOperacion { get; set; }
		public string HoraOperacion { get; set; }
	}
	
}
