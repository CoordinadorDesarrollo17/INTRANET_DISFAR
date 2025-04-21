namespace Capa_Entidad.DireccionTecnica_ENT.TablasSql
{
    public class SolicitudesReversion_DOCS1_E
    {
        public int Id { get; set; }
        public int DOCS1Id { get; set; }
        public string SolicitadoPor { get; set; }
        public string FechaSolicitud { get; set; }
        public string HoraSolicitud { get; set; }
        public string Estado { get; set; }
        public string ConfirmadoPor { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }

        // Campos Extras
        public long ODOCSId { get; set; }
    }
}