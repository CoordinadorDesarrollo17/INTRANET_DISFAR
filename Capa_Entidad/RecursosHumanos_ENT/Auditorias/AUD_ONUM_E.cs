namespace Capa_Entidad.RecursosHumanos_ENT.Auditorias
{
    public class AUD_ONUM_E
    {
        public int IdAUD_ONUM { get; set; }
        public int IdNumero { get; set; }
        public string Campo { get; set; }
        public string ValorAnterior { get; set; }
        public string ValorActual { get; set; }
        public int RegistradoPor { get; set; }
        public string FechaRegistro { get; set; }
        public string HoraRegistro { get; set; }

        // Campos que no son de la tabla
        public string NomApeRegistradoPor { get; set; }
    }
}
