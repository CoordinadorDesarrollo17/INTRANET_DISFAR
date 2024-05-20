using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.ReportesDigemid_ENT
{
    public class OrdenDeVenta_E
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string RucCliente { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Almacen { get; set; }
        public string Comentarios { get; set; }
        public string Fecha { get; set; }
        public string NombreBd { get; set; }
        public decimal DocTotal { get; set; }
        public string SlpName { get; set; }//VENDEDOR
        public string LugarDeEntrega { get; set; }
        public string Producto { get; set; }
        public string FechaVenc { get; set; }
        public string Laboratorio { get; set; }
        public string Lote { get; set; }
        public string RegSanit { get; set; }
        public decimal NumUnidVend { get; set; }
        public decimal PrecioProdIgvVend { get; set; }
        public decimal TotalProdIgvVend { get; set; }
        public string UniMedidVend { get; set; }
        //agregado
        public string AlmacenSalida { get; set; }
    }
}
