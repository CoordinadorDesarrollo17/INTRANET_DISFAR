namespace Capa_Entidad.AbastecimientoInterno_ENT.Reportes
{
    public class ReporteStockPicking_E
    {        
        public string ItemCode { get; set; }
        public string  ItemName { get; set; }
        public string Clasificacion { get; set; }
        public int StockActualPiezas { get; set; }
        public decimal StockActualUnidades { get; set; }
        public int StockMinAbastecimiento { get; set; }
        public string Almacen { get; set; }

        // Para Stock Picking
        public int UbicacionId { get; set; }
        public int UbicacionLoteId { get; set; }
        public string CodigoUbicacion { get; set; }
        public string BatchNum { get; set; }
    }
}
