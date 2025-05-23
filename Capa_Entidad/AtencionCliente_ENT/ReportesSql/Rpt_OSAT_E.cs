using System.ComponentModel;

namespace Capa_Entidad.AtencionCliente_ENT.ReportesSql
{
    public class Rpt_OSAT_E
    {
        [DisplayName("N° RECLAMO")] public string DocNum { get; set; }
        [DisplayName("N° TICKET")] public int DocNumTicket { get; set; }
        [DisplayName("FECHA DE RECLAMO")] public string FechaRegistro { get; set; }
        [DisplayName("CLIENTE")] public string CardName { get; set; }
        [DisplayName("FECHA DE COMPRA")] public string FechaFacturacion { get; set; }
        [DisplayName("RESULTADO")] public string Resultado { get; set; }
        [DisplayName("TIPO DE VENTA")] public string TipoVenta { get; set; }
        [DisplayName("CANAL DE VENTA")] public string CanalVenta { get; set; }
        [DisplayName("ESTADO")] public string Estado { get; set; }
        [DisplayName("FECHA DE ATENCION")] public string FechaAtencion { get; set; }
        [DisplayName("TIPO DE ERROR")] public string TipoError { get; set; }
        [DisplayName("PROBLEMA")] public string Problema { get; set; }
        [DisplayName("ERROR DE ALMACEN")] public string ErrorAlmacen { get; set; }
        [DisplayName("COMENTARIO")] public string Comentario { get; set; }
        [DisplayName("SOLUCIÓN")] public string Solucion { get; set; }
        [DisplayName("FACTOR")] public string Factor { get; set; }
        [DisplayName("RESPONSABLE")] public string OpResponsable { get; set; }
        [DisplayName("LUGAR DESTINO")] public string LugarDestino { get; set; }
        [DisplayName("NC SAP")] public int? NCSAP { get; set; }
        [DisplayName("CODPROD")] public string ItemCode { get; set; }
        [DisplayName("DESCRIPCION")] public string Dscription { get; set; }
        [DisplayName("LOTE")] public string BatchNum { get; set; }
        [DisplayName("FECHA VENC")] public string ExpDate { get; set; }
        [DisplayName("UNIDAD")] public string UnitMsrF { get; set; }
        [DisplayName("CANTIDAD")] public decimal QuantityF { get; set; }
        [DisplayName("TOTAL")] public decimal Total { get; set; }
        [DisplayName("FECHA PROCESO")] public string FechaProceso { get; set; } 
        [DisplayName("FECHA CULMINADO")] public string FechaCulminado { get; set; }
        [DisplayName("TIPO SOLUCION")] public string TipoSolucion { get; set; }
    }
}