using Capa_Datos.Ventas_DAO.Tablas;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.Tablas;
using System.Collections.Generic;

namespace Capa_Negocio.Ventas_NEG.Tablas
{
    public class ORIN_N
    {
        ORIN_D orinD = new ORIN_D();
        public List<NotaCreditoDebito_E> buscarNotaCreditoSapServicio(string NumAtCard)
        { return orinD.buscarNotaCreditoSapServicio(NumAtCard); }
        public List<NotaCreditoDebito_E> buscarNotaCreditoSapArticulo(string NumAtCard)
        { return orinD.buscarNotaCreditoSapArticulo(NumAtCard); }

        public List<ORIN_E> listarNotasDeCredito(ORIN_E fil)
        {
            return orinD.listarNotasDeCredito(fil);
        }
        public TEMP_RRU01_E NotaCreditoSap(int DocNum)
        {
            return orinD.NotaCreditoSap(DocNum);
        }


    }
}
