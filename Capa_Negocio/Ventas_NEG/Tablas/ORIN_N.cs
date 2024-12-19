using Capa_Datos.Ventas_DAO.Tablas;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.Tablas;
using System.Collections.Generic;

namespace Capa_Negocio.Ventas_NEG.Tablas
{
    public class ORIN_N
    {
        ORIN_D orinD = new ORIN_D();
        public List<ORIN_E> Listar(ORIN_E fil)
        {
            return orinD.Listar(fil);
        }
        public ORIN_E ObtenerCabecera(int docEntry = 0, string numAtCard = "")
        {
            return orinD.ObtenerCabecera(docEntry, numAtCard);
        }
        public List<NotaCreditoDebito_E> ObtenerDetalleNotaCreditoServicio(string numAtCard)
        { return orinD.ObtenerDetalleNotaCreditoServicio(numAtCard); }
        public List<NotaCreditoDebito_E> ObtenerDetalleNotaCreditoArticulo(string numAtCard)
        { return orinD.ObtenerDetalleNotaCreditoArticulo(numAtCard); }

        
        


    }
}
