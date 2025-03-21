namespace Capa_Entidad.RecursosHumanos_ENT.TablasSQL
{
    public class ONUM_E
    {
        public int IdNumero { get; set; }
        public string NumeroCorporativo { get; set; }
        public string Operador { get; set; }
        public string Asignado { get; set; }
        public string NroDocumento { get; set; }
        public string Estado { get; set; }
        public string FechaRegistro { get; set; }
        public string FechaModificacion { get; set; }

        // Campos que no son de la tabla
        public int RegistradoPor { get; set; }
    }
}
