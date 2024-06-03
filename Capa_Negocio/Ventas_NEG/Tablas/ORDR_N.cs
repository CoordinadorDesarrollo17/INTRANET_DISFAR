using Capa_Datos.Ventas_DAO.Tablas;
using Capa_Entidad.Ventas_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.Tablas
{
    public class ORDR_N
    {
        ORDR_D oD = new ORDR_D();
        public List<ORDR_E> listadoOrdenesDeVenta(ORDR_E fo)
        {
            return oD.listadoOrdenesDeVenta(fo);
        }
        public ORDR_E obtenerOrdenDeVenta(int DocNum)
        {
            return oD.obtenerOrdenDeVenta(DocNum);
        }
        
    }
}
