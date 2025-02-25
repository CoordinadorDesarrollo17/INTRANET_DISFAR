namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class DetalleRequerimientos_E
    {
        public int Id { get; set; }
        public int RequerimientoId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string BatchNum { get; set; }
        public string CodigoUbicacionOrigen { get; set; }
        public string CodigoUbicacionDestino { get; set; }
        public string UmAlm { get; set; }
        public int ValorUmAlm { get; set; }
        public int? QuantityMaster { get; set; }
        public int? QuantitySaldo { get; set; }
        public int? QuantityUnidadesCajas { get; set; }
        public int AtendidoReserva { get; set; }
        public int AtendidoPicking { get; set; }
    }
}
