namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class DetalleTransferenciaReserva_E
    {
        public int Id { get; set; }
        public int TransferenciaReservaId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string BatchNum { get; set; }
        public string FechaAdmision { get; set; }
        public string FechaVencimiento { get; set; }
        public string CodigoUbicacion { get; set; }
        public string UmAlm { get; set; }
        public string ValorUmAlm { get; set; }
        public decimal QuantityMaster { get; set; }
        public decimal QuantitySaldo { get; set; }
        public decimal QuantityUnidades { get; set; }
    }
}
