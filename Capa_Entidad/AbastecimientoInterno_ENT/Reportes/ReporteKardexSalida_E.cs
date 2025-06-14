namespace Capa_Entidad.AbastecimientoInterno_ENT.Reportes
{
    public class ReporteKardexSalida_E
    {
        public int NroRequerimiento { get; set; }
        public string Proceso { get; set; }
        public string FechaSalida { get; set; }
        public string HoraSalida { get; set; }
        public string NombrePersonal { get; set; }
        public string SKU { get; set; }
        public string DescripcionArticulo { get; set; }
        public string Lote { get; set; }
        public int CantidadMaster { get; set; }
        public int CantidadSaldo { get; set; }
        public int CantidadUnidadesCajas { get; set; }
        public string UbicacionOrigen { get; set; }
        public string UbicacionDestino { get; set; }
        public string UbicacionActual { get; set; }
    }
}