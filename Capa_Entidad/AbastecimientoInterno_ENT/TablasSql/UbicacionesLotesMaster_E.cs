namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class UbicacionesLotesMaster_E
    {
        public int Id { get; set; }
        public int UbicacionLoteId { get; set; }
        public string Almacen { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CodigoUbicacion { get; set; }
        public string BatchNum { get; set; }
        public string UmAlm { get; set; }
        public int ValorUmAlm { get; set; }
        public int QuantityMaster { get; set; }
        public int QuantitySaldo { get; set; }
        public int QuantityUnidadesCajas { get; set; }
    }
}
