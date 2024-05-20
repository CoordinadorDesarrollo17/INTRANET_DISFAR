using Capa_Datos.AtencionCliente_DAO.TablasSql;
using Capa_Entidad.AtencionCliente_ENT.TablasSql;
using System;
using System.Collections.Generic;

namespace Capa_Negocio.AtencionCliente_NEG.TablasSql
{
	public class CC_OSAT_N
	{
		CC_OSAT_D ccOSAT = new CC_OSAT_D();

		public List<CC_OSAT_E> ListarCC_OSAT(int DocEntry, string operacion)
		{
			return ccOSAT.ListarCC_OSAT(DocEntry, operacion);
		}

		public string UltimoEstadoCC_OSAT(int DocEntry)
		{
			return ccOSAT.UltimoEstadoCC_OSAT(DocEntry);
		}
	}
}
