using Capa_Datos.Almacen_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System.Collections.Generic;

namespace Capa_Negocio.Almacen_NEG.TablasSql
{
    public class MotivosDevoluciones_N
    {
        MotivosDevoluciones_D md = new MotivosDevoluciones_D();

        public List<MotivosDevoluciones_E> ListarMotivosDevoluciones(MotivosDevoluciones_E filtro)
        {
            return md.ListarMotivosDevoluciones(filtro);
        }

		public string RegistrarMotivoDevolucion(MotivosDevoluciones_E motivo)
		{
			return md.RegistrarMotivoDevolucion(motivo);
		}

		public string EditarMotivoDevolucion(MotivosDevoluciones_E motivo)
		{
			return md.EditarMotivoDevolucion(motivo);
		}
	}
}
