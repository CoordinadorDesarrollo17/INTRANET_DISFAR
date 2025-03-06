namespace Capa_Entidad.AbastecimientoInterno_ENT.Reportes
{
    public class ReporteStockPicking_E
    {
        public string ItemCode { get; set; }
        public string  ItemName { get; set; }
        public string Clasificacion { get; set; }
        public int StockActual { get; set; }
        public int StockMinVenta { get; set; }
        public int StockMinAbastecimiento { get; set; }
        public string Almacen { get; set; }
    }
}
