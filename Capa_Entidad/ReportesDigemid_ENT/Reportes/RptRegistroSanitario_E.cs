using System.ComponentModel;

namespace Capa_Entidad.ReportesDigemid_ENT.Reportes
{
    public class RptRegistroSanitario_E
    {
        [DisplayName("CÓDIGO ARTÍCULO")] public string CodArticulo { get; set; }
        [DisplayName("DESCRIPCIÓN ARTÍCULO")] public string DescArticulo { get; set; }
        [DisplayName("FECHA VENCIMIENTO")] public string FechaVenc { get; set; }
        [DisplayName("COMENTARIO")] public string Comentario { get; set; }
        [DisplayName("ESTADO")] public string Estado { get; set; }
        [DisplayName("REGISTRO SANITARIO")] public string RegistroSanitario { get; set; }
        [DisplayName("OBSERVACIÓN")] public string Descripcion { get; set; }
        [DisplayName("REGISTRADO POR")] public string RegistradoPor { get; set; }
        [DisplayName("FECHA REGISTRO")] public string FechaRegistro { get; set; }
        [DisplayName("HORA REGISTRO")] public string HoraRegistro { get; set; }
    }
}