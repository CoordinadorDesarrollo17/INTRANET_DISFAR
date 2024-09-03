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
        public List<ORDR_E> listadoOrdenesDeVenta(ORDR_E fo, bool mostrarCompVinculados)
        {
            return oD.listadoOrdenesDeVenta(fo, mostrarCompVinculados);
        }
        public List<ORDR_E.DetalleOrdenDeVenta> listadoDetalleOrdenesDeVenta(List<int> docNums)
        {
            return oD.listadoDetalleOrdenesDeVenta(docNums);
        }
        public ORDR_E obtenerOrdenDeVenta(int DocNum)
        {
            return oD.obtenerOrdenDeVenta(DocNum);
        }

    }
}
