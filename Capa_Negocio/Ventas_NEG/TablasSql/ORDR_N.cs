using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
    public class ORDR_N
    {
        ORDR_D ordr_D = new ORDR_D();

        public List<ORDR_E> ListarPedidosOnline(ORDR_E Pedido)
        {
            return ordr_D.ListarPedidosOnline(Pedido);
        }

        public int RegistrarPedidoOnline(ORDR_E Pedido, List<Capa_Entidad.Almacen_ENT.TablasSql.OIBT_E> DetallePedido)
        {
            if (string.IsNullOrEmpty(Pedido.CardCode) || string.IsNullOrEmpty(Pedido.CardName) || string.IsNullOrEmpty(Pedido.WhsCode))
            {
                throw new Exception("El código de cliente, nombre de cliente y el almacén, son campos obligatorios.");
            }

            if (DetallePedido == null || DetallePedido.Count <= 0)
            {
                throw new Exception("El pedido online a crearse debe contener al menos 1 artículo en su detalle.");
            }

            return ordr_D.RegistrarPedidoOnline(Pedido, DetallePedido);
        }

        public int EditarPedidoOnline(int idORDR, List<OIBT_E> DetallePedido)
        {
            List<OIBT_E> ListaDetPed = new List<OIBT_E>();

            if (DetallePedido != null && DetallePedido.Count >= 1)
            {
                foreach (var det in DetallePedido)
                {
                    if (det.ItemCode != null && det.ItemName != null)
                    {
                        ListaDetPed.Add(det);
                    }
                }
            }

            return ordr_D.EditarPedidoOnline(idORDR, ListaDetPed);
        }

        public string CambiarEstadoPedidoOnline(ORDR_E Pedido, string Accion)
        {
            List<ORDR_E> result = ordr_D.ListarPedidosOnline(Pedido);

            if (result.Count >= 1)
            {
                Pedido.Estado = (result[0].Estado != null) ? result[0].Estado : "";
            }
            else
            {
                return null;
            }

            return ordr_D.CambiarEstadoPedidoOnline(Pedido, Accion);
        }
    }
}
