using Capa_Datos.Operaciones_DAO.TablasSql;
using Capa_Entidad.Operaciones_ENT.TablasSql;
using System.Collections.Generic;

namespace Capa_Negocio.Operaciones_NEG.TablasSql
{
	public class OTEP_N
	{
		OTEP_D otepD = new OTEP_D();

		public List<OTEP_E> ListarTiposErroresPicking()
		{
			return otepD.ListarTiposErroresPicking();
		}
	}
}
