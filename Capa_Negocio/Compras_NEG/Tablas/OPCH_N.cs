using Capa_Datos.Compras_DAO.Tablas;
using Capa_Entidad.Compras_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Compras_NEG.Tablas
{
    public class OPCH_N
    {
        OPCH_D oD = new OPCH_D();
        public List<OPCH_E> listadoFacturasProveedores(OPCH_E fil)
        {
            return oD.listadoFacturasProveedores(fil);
        }
        public OPCH_E buscarFacturaProveedor(int DocEntry)
        {
            return oD.buscarFacturaProveedor(DocEntry);
        }
    }
}
