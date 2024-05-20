using Capa_Datos.DireccionTecnica_DAO.TablasSql;
using Capa_Entidad.DireccionTecnica_ENT.TablasSql;

namespace Capa_Negocio.DireccionTecnica_NEG.TablasSql
{
	public class SQL_OWTR_N
	{
		SQL_OWTR_D owtrD = new SQL_OWTR_D();

		public SQL_OWTR_E ObtenerOWTR(int DocNumSAP)
		{
			return owtrD.ObtenerOWTR(DocNumSAP);
		}

		public int CambiarEstadoOWTR(SQL_OWTR_E datos, string accion)
		{
			return owtrD.CambiarEstadoOWTR(datos, accion);
		}
	}
}
