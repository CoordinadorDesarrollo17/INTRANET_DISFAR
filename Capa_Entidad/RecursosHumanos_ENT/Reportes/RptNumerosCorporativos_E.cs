using System.ComponentModel;

namespace Capa_Entidad.RecursosHumanos_ENT.Reportes
{
    public class RptNumerosCorporativos_E
    {
        [DisplayName("NÚMERO CORPORATIVO")] public string NumeroCorporativo { get; set; }
        [DisplayName("OPERADOR")] public string Operador { get; set; }
        [DisplayName("ASIGNADO")] public string Asignado { get; set; }
        [DisplayName("DNI")] public string NroDocumento { get; set; }
        [DisplayName("NOMBRES Y APELLIDOS")] public string Empleado { get; set; }
        [DisplayName("NÚMERO PERSONAL")] public string Celular{ get; set; }
        [DisplayName("CARGO")] public string Cargo { get; set; }
        [DisplayName("SEDE")] public string Sede { get; set; }
        [DisplayName("ESTADO")] public string Estado { get; set; }
        [DisplayName("FECHA REGISTRO")] public string FechaRegistro { get; set; }
        [DisplayName("FECHA ÚLTIMA MODIFICACIÓN")] public string FechaModificacion { get; set; }
    }
}
