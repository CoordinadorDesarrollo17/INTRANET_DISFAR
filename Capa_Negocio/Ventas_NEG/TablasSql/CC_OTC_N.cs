using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
	public class CC_OTC_N
	{
		CC_OTC_D otcD = new CC_OTC_D();

		public CC_OTC_E ObtenerDatosCC_OTC(int idOTC, string operacion)
		{
			return otcD.ObtenerDatosCC_OTC(idOTC, operacion);
		}
	}
}