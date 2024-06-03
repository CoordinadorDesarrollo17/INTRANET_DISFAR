using System.ComponentModel;
using Capa_Entidad.Almacen_ENT.ReportesSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.ReportesSql
{
    public class RptHistoricoDevoluciones_E
    {
        [DisplayName("DOCNUM")] public int DocNum { get; set; }
        [DisplayName("N°")] public string Correlativo { get; set; }
        [DisplayName("FECHA CREACIÓN")] public string FechaCreacion { get; set; }
        [DisplayName("RUC")] public string CardCode { get; set; }
        [DisplayName("RAZÓN SOCIAL")] public string CardName { get; set; }
        [DisplayName("FECHA DEVOLUCIÓN")] public string FechaDevolucion { get; set; }
        [DisplayName("ESTADO")] public string Estado { get; set; }
        [DisplayName("CÓDIGO")] public string ItemCode { get; set; }
        [DisplayName("DESCRIPCIÓN DEL PRODUCTO")] public string ItemName { get; set; }
        [DisplayName("ALMACÉN")] public string WhsCode { get; set; }
        [DisplayName("LOTE")] public string BatchNum { get; set; }
        [DisplayName("FECHA VENC.")] public string ExpDate { get; set; }
        [DisplayName("UM")] public string BuyUnitMsr { get; set; }
        [DisplayName("CANTIDAD")] public decimal Quantity { get; set; }
        [DisplayName("REF. FACTURA")] public string RefFactura { get; set; }
        [DisplayName("MOTIVO")] public string Descripcion { get; set; }
        [DisplayName("SUBMOTIVO")] public string Submotivo { get; set; }
        [DisplayName("OBSERVACIONES")] public string Observacion { get; set; }
        [DisplayName("COMENTARIO ANULACIÓN")] public string Comentario { get; set; }
        [DisplayName("OP CREACIÓN")] public string OpCreacion { get; set; }

    }

    public class RptFiltrosHistoricoDevoluciones_E : RptHistoricoDevoluciones_E
    {
        public int DocEntry { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
    }
}