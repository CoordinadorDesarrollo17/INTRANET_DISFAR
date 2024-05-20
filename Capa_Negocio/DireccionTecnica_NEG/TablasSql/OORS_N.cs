using Capa_Datos.DireccionTecnica_DAO.TablasSql;
using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.DireccionTecnica_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.DireccionTecnica_NEG.TablasSql
{
	public class OORS_N
	{
		OORS_D rsD = new OORS_D();
		public List<OORS_E> ListarRegistrosSanitarios(OORS_E filtros)
		{
			return rsD.ListarRegistrosSanitarios(filtros);
        }

        public string RegistrarObservacion(OORS_E rs)
        {
            return rsD.RegistrarObservacion(rs);
        }

        public List<OORS_E> ObtenerDatosObsRS(string registroSanitario, string codArticulo, string codLaboratorio)
        {
            return rsD.ObtenerDatosObsRS(registroSanitario, codArticulo, codLaboratorio);
        }

        public List<string> ConsultarRegistrosSanitariosExpirados()
        {
            return rsD.ConsultarRegistrosSanitariosExpirados();
        }

    }
}
