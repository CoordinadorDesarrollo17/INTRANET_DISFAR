using Capa_Datos.SocioNegocios_DAO.TablasExternas;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Capa_Negocio.SocioNegocios_NEG.TablasExternas
{
    public class OCRD_N
    {
		readonly OCRD_D _datos = new OCRD_D();
        public List<OCRD_E> listarSociosDeNegocios(OCRD_E filtro)
        {
            return _datos.listarSociosDeNegocios(filtro);
        }
        public List<OCRD_E> listarSociosConContactos()
        {
            return _datos.listarSociosConContactos();
        }
        public int Migrar()
        {
            return _datos.Migrar();
        }
    }
}
