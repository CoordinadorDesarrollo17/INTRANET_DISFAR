using System.Collections.Generic;
using System.Data;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
	public class EP1_E
	{
		public int IdEP1 { get; set; }
		public int IdOEP { get; set; }
		public string CodigoProducto { get; set; }
		public string DescripcionProducto { get; set; }
		public int IdTipoErrorPicking { get; set; }
		public string PickerResponsable{ get; set; }

		public static List<EP1_E> Registros(List<EP1_E> dt, int IdOEP)
		{
			List<EP1_E> lista = new List<EP1_E>();
			int ln = 1;
			foreach (EP1_E d in dt)
			{
				d.IdOEP = IdOEP;
				lista.Add(d);
				ln++;
			}
			return lista;
		}

		public static DataTable TbDetalle(List<EP1_E> dt, int IdOEP)
		{
			DataTable tb = new DataTable();
			tb.Columns.Add("IdOEP", typeof(int));
			tb.Columns.Add("CodigoProducto", typeof(string));
			tb.Columns.Add("DescripcionProducto", typeof(string));
			tb.Columns.Add("IdTipoErrorPicking", typeof(int));
			tb.Columns.Add("PickerResponsable", typeof(string));

			foreach (EP1_E reg in Registros(dt, IdOEP))
			{
				tb.Rows.Add(reg.IdOEP, reg.CodigoProducto, reg.DescripcionProducto, reg.IdTipoErrorPicking, reg.PickerResponsable);
			}
			return tb;
		}
	}
}
