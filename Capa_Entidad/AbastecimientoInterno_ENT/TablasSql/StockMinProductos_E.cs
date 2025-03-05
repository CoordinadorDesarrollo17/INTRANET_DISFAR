namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class StockMinProductos_E
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int StockMinAbastecimiento { get; set; }
        public int StockMinVenta { get; set; }
        public string Clasificacion { get; set; }

        // Campos que no son de la tabla
        public string NombreOperarioAccion { get; set; }
    }
}