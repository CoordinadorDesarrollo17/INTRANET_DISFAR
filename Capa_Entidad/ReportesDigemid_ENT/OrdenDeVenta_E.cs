using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.ReportesDigemid_ENT
{
    public class OrdenDeVenta_E
    {
        public int DocNum { get; set; }
        public string RucCliente { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Almacen { get; set; }
        public string Comentarios { get; set; }
        public string Fecha { get; set; }
        public string NombreBd { get; set; }
        public decimal DocTotal { get; set; }
        public string SlpName { get; set; }
        public string LugarDeEntrega { get; set; }
        public string Producto { get; set; }
        public string FechaVenc { get; set; }
        public string Laboratorio { get; set; }
        public string Lote { get; set; }
        public string RegSanit { get; set; }
        public decimal NumUnid { get; set; }
        public decimal NumUnidVend { get; set; }
        public decimal CantUnid { get; set; }
        public decimal PrecioProdIgvVend { get; set; }
        public decimal TotalProdIgvVend { get; set; }
        public string UniMedidVend { get; set; }
        public string AlmacenSalida { get; set; }
        public string TipoVenta { get; set; }
        public string ItemCode { get; set; }        
        public decimal CantidadSolicitadaVenta { get; set; }
        public string[] Ubicaciones { get; set; }

        // C A M P O S   E X T R A S
        public decimal TotalNumUnidVend { get; set; }
    }
}
