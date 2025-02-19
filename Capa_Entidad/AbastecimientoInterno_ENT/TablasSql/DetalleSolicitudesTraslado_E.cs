namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class DetalleSolicitudesTraslado_E
    {
        public int Id {  get; set; }
        public int SolicitudesTrasladoId {  get; set; }
        public string ItemCode {  get; set; }
        public string ItemName {  get; set; }
        public string BatchNum {  get; set; }
        public decimal QuantityCajas {  get; set; }
        public string FromWhsCode {  get; set; }
        public string ToWhsCode {  get; set; }
        public string Estado {  get; set; }
    }
}