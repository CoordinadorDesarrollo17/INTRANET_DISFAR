namespace Capa_Entidad.DireccionTecnica_ENT.TablasSql
{
    public class OORS_E
    {
        public int IdOORS { get; set; }
        public int CodLaboratorio { get; set; }
        public string CodArticulo { get; set; }
        public string RegistroSanitario { get; set; }
        public string Descripcion { get; set; }
        public string RegistradoPor { get; set; }
        public string FechaRegistro { get; set; }
        public string HoraRegistro { get; set; }

        /**************** C A M P O S   Q U E   N O   S O N   D E   L A   T A B L A ****************/
        public string DescArticulo { get; set; }
        public string FechaVenc { get; set; }
        public string Comentario { get; set; }
        public string Estado { get; set; }
    }
}