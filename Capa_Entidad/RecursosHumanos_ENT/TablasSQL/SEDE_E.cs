namespace Capa_Entidad.RecursosHumanos_ENT.TablasSQL
{
    public class SEDE_E
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int UbigeoID { get; set; }
        public string Estado { get; set; }

        // Campos que no son de la tabla
        public string DescripcionEstado { get; set; }
        public string IdAlterno { get; set; }
    }
}