using Capa_Datos.Ventas_DAO.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
    public class RTV6_N
    {
        RTV6_D rtvD = new RTV6_D();
        public void AsignarPrecio(int DocEntry, int Linea, decimal Precio)
        {
            rtvD.AsignarPrecio(DocEntry, Linea, Precio);
        }

        public decimal ObtenerPesoTotal(int DocEntry)
        {
            return rtvD.ObtenerPesoTotal(DocEntry);
        }
    }
}
