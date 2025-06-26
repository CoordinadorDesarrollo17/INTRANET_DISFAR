namespace Capa_Entidad.RecursosHumanos_ENT.TablasSQL
{
    public class ODPTO_E
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Estado { get; set; }
        public string FechaRegistro { get; set; }

        // Campos que no son de la tabla
        public string DescripcionEstado { get; set; }
    }
}