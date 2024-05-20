using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Compras_ENT.Tablas
{
	public class SQL_OPDN_E
	{
		public int DocEntry { get; set; }
		public int ObjType { get; set; }
		public string Estado { get; set; }
		public string FechaRealizacion { get; set; }
		public string HoraRealizacion { get; set; }
		public string OpRealizacion { get; set; }
		public string Observaciones { get; set; }

		// metodos
		public static string SqlTextIn(List<SQL_OPDN_E> lista)
		{
			string SqlIn = "";
			foreach(SQL_OPDN_E s in lista)
			{
				SqlIn += s.DocEntry + ",";
			}
			SqlIn += 0;
			return SqlIn;
		}
	}
}
