using System.Collections.Generic;

namespace Capa_Entidad.TablasSql
{
    public class ODOCS_E
    {
        public int Id { get; set; }
        public string TipoDocumento { get; set; }
        public long DocEntry { get; set; }
        public long DocNum { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Guia { get; set; }
        public string ComprobanteVinculado { get; set; }
        public string FechaContabilizacion { get; set; }
        public string FechaInicioTraslado { get; set; }
        public string Estado { get; set; }

        public List<DOCS1_E> Detalle { get; set; } = new List<DOCS1_E>();

        // Campos Externos
        public string UsuarioRegistro { get; set; }
    }
}