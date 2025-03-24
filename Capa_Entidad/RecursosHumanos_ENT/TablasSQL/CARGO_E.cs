namespace Capa_Entidad.RecursosHumanos_ENT.TablasSQL
{
    public class CARGO_E
    {
        public int Id { get; set; }
        public int RolID { get; set; }
        public string Nombre { get; set; }
        public string Estado { get; set; }
        public string UsuarioOperacion { get; set; }

        // Campos que no son de la tabla
        public string DescripcionEstado { get; set; }
    }
}