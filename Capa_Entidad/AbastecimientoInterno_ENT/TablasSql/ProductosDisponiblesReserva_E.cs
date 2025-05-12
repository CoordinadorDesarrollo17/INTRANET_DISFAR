namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class ProductosDisponiblesReserva_E
    {
        public string ItemCode { get; set; }
        public string BatchNum { get; set; }
        public string UmAlm { get; set; }
        public int ValorUmAlm { get; set; }
        public string CodigoUbicacionOrigen { get; set; }
        public int DisponibleMaster { get; set; }
        public int DisponibleSaldo { get; set; }     
    }
}