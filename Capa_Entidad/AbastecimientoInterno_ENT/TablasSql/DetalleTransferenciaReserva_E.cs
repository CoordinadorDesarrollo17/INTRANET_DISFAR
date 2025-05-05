namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class DetalleTransferenciaReserva_E
    {
        public int Id { get; set; }
        public int TransferenciaReservaId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string BatchNum { get; set; }
        public string InDate { get; set; }
        public string ExpDate { get; set; }
        public string CodigoUbicacion { get; set; }
        public string UmAlm { get; set; }
        public int ValorUmAlm { get; set; }
        public int QuantityMaster { get; set; }
        public int QuantitySaldo { get; set; }
        public int QuantityUnidadesCajas { get; set; }
        public int AtendidoReserva { get; set; }
        public int Validado { get; set; }
        //Campo que no es de la tabla
        public int DocNumSolicitudTraslado { get; set; }
        public string RackBloque { get; set; }
        public string Posicion { get; set; }
        public string Nivel { get; set; }
    }
}
