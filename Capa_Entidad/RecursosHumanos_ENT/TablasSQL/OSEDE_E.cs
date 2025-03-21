namespace Capa_Entidad.RecursosHumanos_ENT.TablasSQL
{
    public class OSEDE_E
    {
        public int IdSede { get; set; }
        public string Nombre { get; set; }
        public int IdUbig { get; set; }
        public string Estado { get; set; }
        public string FechaRegistro { get; set; }
        public string FechaModificacion { get; set; }

        // Campos que no son de la tabla
        public string DescripcionEstado { get; set; }
    }
}