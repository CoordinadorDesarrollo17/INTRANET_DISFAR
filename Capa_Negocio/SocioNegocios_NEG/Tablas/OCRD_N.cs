using Capa_Datos.SocioNegocios_DAO.Tablas;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.SocioNegocios_NEG.Tablas
{
    public class OCRD_N
    {
		readonly OCRD_D oD = new OCRD_D();
        public List<OCRD_E> listarSociosDeNegocios(OCRD_E filtro)
        {
            return oD.listarSociosDeNegocios(filtro);
        }
        public List<OCRD_E> listarSociosConContactos()
        {
            return oD.listarSociosConContactos();
        }

		public List<OCRD_E> BuscarCliente(OCRD_E cliente)
		{
			return oD.BuscarCliente(cliente);
		}
	}
}
