using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class RDR1_E
    {
        public int Id { get; set; }
        public int IdORDR { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string BatchNum { get; set; }
        public string ExpDate { get; set; }
        public string UMVenta { get; set; }
        public string UndMed { get; set; }
        public decimal Quantity { get; set; }
        public int Cantidad { get; set; }
        public decimal Price { get; set; }
        public decimal PrecioxCantidad { get; set; }        // VerPedidoOnline
    }

    public class ExcelDetallePedido
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UndMed { get; set; }
        public int Cantidad { get; set; }
    }
}
