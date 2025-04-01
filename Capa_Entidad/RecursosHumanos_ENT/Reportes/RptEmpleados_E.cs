using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.RecursosHumanos_ENT.Reportes
{
    public class RptEmpleados_E
    {
        [DisplayName("Apellidos y Nombres")] public string ApellidosNombres { get; set; }
        public string Sede { get; set; }
        public string Departamento { get; set; }
        [DisplayName("Área")] public string Area { get; set; }
        public string Cargo { get; set; }
        [DisplayName("Cel. Corporativo")] public string NumeroCorporativo { get; set; }
        [DisplayName("Anexo")] public string AnexoCorporativo { get; set; }
        [DisplayName("Correo Corporativo")] public string CorreoCorporativo { get; set; }
    }
}
