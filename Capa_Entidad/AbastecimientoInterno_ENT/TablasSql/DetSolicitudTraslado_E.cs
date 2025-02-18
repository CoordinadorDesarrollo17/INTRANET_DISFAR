namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class DetSolicitudTraslado_E
    {
        public string AlmacenOrigen { get; set; }
        public string AlmacenDestino { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string BatchNum { get; set; }
        public decimal CantidadTotalPorSKU { get; set; }
        public decimal CantidadTotalPorSKUyLote { get; set; }
        public string FechaAdmision { get; set; }
        public string FechaVencimiento { get; set; }
    }
}
