
using System.ComponentModel;

namespace Capa_Entidad.Almacen_ENT.ReportesSql
{
    public class RptCorreoDevolucion_E
    {
        [DisplayName("SERIE")] public string SerieFactura { get; set; }
        [DisplayName("CORRELATIVO")] public string CorrelativoFactura { get; set; }
        [DisplayName("DESCRIPCIÓN DEL PRODUCTO")] public string ItemName { get; set; }
        [DisplayName("LOTE")] public string BatchNum { get; set; }
        [DisplayName("FECHA/V")] public string ExpDate { get; set; }
        [DisplayName("CANTIDAD")] public decimal Quantity { get; set; }
        [DisplayName("MOTIVO")] public string SubMotivoDescripcion { get; set; }

    }

}

