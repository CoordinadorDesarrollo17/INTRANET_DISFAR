using Capa_Datos.Almacen_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;

namespace Capa_Negocio.Almacen_NEG.TablasSql
{
	public class CC_OIPE_N
	{
		CC_OIPE_D ccOIPE = new CC_OIPE_D();

		public List<CC_OIPE_E> ListarCC_OIPE(int DocEntry, string operacion)
		{
			return ccOIPE.ListarCC_OIPE(DocEntry, operacion);
		}

		public string UltimoEstadoCC_OIPE(int DocEntry)
		{
			return ccOIPE.UltimoEstadoCC_OIPE(DocEntry);
		}
	}
}
