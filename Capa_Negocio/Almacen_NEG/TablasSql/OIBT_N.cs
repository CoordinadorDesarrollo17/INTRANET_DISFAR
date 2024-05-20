using Capa_Datos.Almacen_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System.Collections.Generic;

namespace Capa_Negocio.Almacen_NEG.TablasSql
{
	public class OIBT_N
	{
		readonly OIBT_D oibtD = new OIBT_D();
		public List<OIBT_E> BuscarArticulo(OIBT_E articulo)
		{
			return oibtD.BuscarArticulo(articulo);
		}
		public List<OIBT_E> listarArticulosLotes(OIBT_E filtro = null){ return oibtD.ListarArticulosLotes(filtro); }
        public List<OIBT_E> ListarLaboratorios()
        {
            return oibtD.ListarLaboratorios();
        }
        public bool VerificarMigracionArticulos()
        {
            return oibtD.VerificarMigracionArticulos();
        }
        public int TemporizarMigrarArticulos() => oibtD.TemporizarMigrarArticulos();
        public void MigrarArticulos() => oibtD.Migrar();
    }
}
