namespace Capa_Entidad.AbastecimientoInterno_ENT.Reportes
{
    public class ReporteKardexSalida_E
    {
        public int NroRequerimiento { get; set; }
        public string Proceso { get; set; }
        public string FechaSalida { get; set; }
        
        // CREAR
        public string OperarioSolicitud { get; set; }
        public string HoraSolicitud { get; set; }

        // ATD_RESERVA
        public string OperarioApilador { get; set; }
        //public string HoraReabastecimiento { get; set; }
        public string HoraSalida { get; set; }
        // ATD_PICKING
        public string OperarioReabastecimiento { get; set; }
        public string HoraPicking { get; set; }
        public string SKU { get; set; }
        public string DescripcionArticulo { get; set; }
        public string Lote { get; set; }
        public string Master { get; set; }
        public int CantidadMaster { get; set; }
        public int CantidadSaldo { get; set; }
        public int CantidadUnidadesCajas { get; set; }
        public string UbicacionOrigen { get; set; }
        public string UbicacionDestino { get; set; }
        public string UbicacionActual { get; set; }
    }
}