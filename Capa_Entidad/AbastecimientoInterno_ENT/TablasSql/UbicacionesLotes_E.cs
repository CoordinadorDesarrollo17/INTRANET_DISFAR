namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class UbicacionesLotes_E
    {
        public int Id { get; set; }
        public int UbicacionId { get; set; }
        public string Almacen { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CodigoUbicacion { get; set; }
        public string BatchNum { get; set; }
        public int QuantityUnidadesCajas { get; set; }
    }
}
