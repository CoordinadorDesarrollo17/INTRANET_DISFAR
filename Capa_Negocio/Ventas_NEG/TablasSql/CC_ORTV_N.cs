using System;
using System.Collections.Generic;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System.Data;
using Capa_Datos;
using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Seguridad_ENT;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
	public class CC_ORTV_N
    {
		CC_ORTV_D ccORTV = new CC_ORTV_D();

        public List<CC_ORTV_E> ListarCC_ORTV(int DocEntry, string operacion)
        {
            return ccORTV.ListarCC_ORTV(DocEntry, operacion);
        }

        // Seguimiento de Tickets
        public Dictionary<string, CC_ORTV_E> ListarCC_FlujoEstados(int DocEntry)
        {
            return ccORTV.ListarCC_FlujoEstados(DocEntry);
        }

        public string UltimoEstadoCC_ORTV(int DocEntry)
        {
            return ccORTV.UltimoEstadoCC_ORTV(DocEntry);
        }

    }
}
