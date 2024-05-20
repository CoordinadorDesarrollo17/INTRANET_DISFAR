using Capa_Datos.Compras_DAO.TablasSql;
using Capa_Entidad.Compras_ENT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Compras_NEG.TablasSql
{
    public class LNPV_N
    {
        LNPV_D pDao = new LNPV_D();
        public List<ProveedorListaNegra_E> listarProveedoresListaNegra()
        {
            return pDao.listarProveedoresListaNegra();
        }
        public int agregarProveedorListaNegra(ProveedorListaNegra_E p)
        {
            return pDao.agregarProveedorListaNegra(p);
        }
        public int retirarProveedorListaNegra(string CardCode)
        {
            return pDao.retirarProveedorListaNegra(CardCode);
        }
    }
}
