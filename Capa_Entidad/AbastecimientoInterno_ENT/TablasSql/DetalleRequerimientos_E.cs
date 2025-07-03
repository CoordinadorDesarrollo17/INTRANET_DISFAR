namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class DetalleRequerimientos_E
    {
        public int IdentificadorExcel { get; set; }
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
        public int UbicacionId { get; set; }            // Para UbicacionesLotes

        //campos que no son de la tabla, sirven para filtrar listado
        public  string RackBloque{ get; set; }
        public  string Posicion{ get; set; }
        public  string Nivel{ get; set; }
        public string Zona { get; set; }         // Zona solo cuando sea Venta Master
        public int Aprobado { get; set; }         // Campo de requerimiento automático
    }
}
