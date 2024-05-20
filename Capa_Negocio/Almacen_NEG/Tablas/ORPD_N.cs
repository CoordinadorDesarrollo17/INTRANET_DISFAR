using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Almacen_NEG.Tablas
{
    public class ORPD_N
    {
        ORPD_D orpdHana = new ORPD_D();
       
        public List<ORPD_E> BuscarDevolucion(Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E Dev,string FechaFormateada,string CardCode, string RefFactura=null)
        {
            var listaEncontrada =  orpdHana.BuscarDevolucion(FechaFormateada,  CardCode, RefFactura);
            // Agrupar por DocNum
            var gruposDocNum = listaEncontrada.GroupBy(x => x.DocNum);

            // Iterar sobre los grupos
            foreach (var grupo in gruposDocNum)
            {
                // Ordenar la lista por ItemCode y luego por BatchNum
               var listaOrdenadaSql = Dev.DetalleDevolucion.OrderBy(x => x.ItemCode)
                                                  .ThenBy(x => x.BatchNum)
                                                  .ToList();
                var listaOrdenadaHana = grupo.OrderBy(x => x.ItemCode)
                                                  .ThenBy(x => x.BatchNum)
                                                  .ToList();

                //comparar las dos listas ordenadas SQL y HANA
                bool sonIguales = ComparativoListas(listaOrdenadaSql, listaOrdenadaHana);
                if (sonIguales) { TablasSql.ORPD_N orpdNSql = new TablasSql.ORPD_N();
                    orpdNSql.CambiarEstadoDevolucion(Dev, "NC");
                }
            }
            return null;
         }
        static bool ComparativoListas(List<Capa_Entidad.Almacen_ENT.TablasSql.RPD1_E> lista1, IEnumerable<ORPD_E> lista2)
        {
            // Comparar las listas utilizando SequenceEqual y proyectando las propiedades a una tupla
            return lista1.Select(objeto => (objeto.ItemCode, objeto.BatchNum, objeto.Quantity,objeto.NumInBuy,objeto.BuyUnitMsr))
                         .SequenceEqual(lista2.Select(objeto => (objeto.ItemCode, objeto.BatchNum, objeto.Quantity,objeto.NumInBuy, objeto.BuyUnitMsr)));
        }

    }
}
