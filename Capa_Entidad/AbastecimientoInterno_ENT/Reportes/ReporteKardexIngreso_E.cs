namespace Capa_Entidad.AbastecimientoInterno_ENT.Reportes
{
    public class ReporteKardexIngreso_E
    {
        public int DocNumTransferencia {  get; set; }
        public string Proceso {  get; set; }
        public string Comentario { get; set; }
        public string FechaIngreso {  get; set; }
        public string HoradeNumTransferencia { get; set; }
        public string ApilarIngreso { get; set; }
        public string HoraIngreso {  get; set; }
        public string Digitador {  get; set; }
        public string SKU {  get; set; }
        public string DescripcionArticulo {  get; set; }
        public string Lote {  get; set; }
        public string Master { get; set; }
        public int CantidadMaster {  get; set; }
        public int CantidadSaldo {  get; set; }
        public int CantidadUnidadesCajas {  get; set; }
        public string UbicacionRegistro {  get; set; }
        public string UbicacionActual {  get; set; }
    }
}