using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System.Collections.Generic;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
	public class RTV11_N
	{
		RTV11_D rtv11D = new RTV11_D();

		public List<RTV11_E> ObtenerPickers(int docEntry)
		{
			return rtv11D.ObtenerPickers(docEntry);
		}
	}
}
