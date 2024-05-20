using Capa_Datos.Ventas_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.Tablas;
using System.Collections.Generic;

namespace Capa_Negocio.Ventas_NEG.Tablas
{
    public class ODLN_N
    {
        ODLN_D oD = new ODLN_D();
        public List<ODLN_E> listarEntregasVenta(ODLN_E fil)
        {
            return oD.listarEntregasVenta(fil);
        }
        public string CalcularPdfsActaDespachoODLN(string Fecha, string U_SYP_STATUS,
         string U_COB_LUGAREN, string TipoComprobante)
        {
            return oD.CalcularPdfsActaDespachoODLN(Fecha, U_SYP_STATUS, U_COB_LUGAREN, TipoComprobante);
        }
        public List<Guia_Remision_E> buscarGuiaRemisionSap(string NumAtCard)
        {
            return oD.buscarGuiaRemisionSap(NumAtCard);
        }
    }
}
