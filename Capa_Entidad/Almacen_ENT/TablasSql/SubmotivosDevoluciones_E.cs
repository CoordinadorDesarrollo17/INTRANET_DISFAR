using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.TablasSql
{
    public class SubmotivosDevoluciones_E
    {
        public int IdSubmotivo { get; set; }
        public int IdMotivo { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public string DescEstado { get; set; }
        public string FechaRegistro { get; set; }
        public string Operario { get; set; }
        public string DescMotivo { get; set; }          // Campo INNER JOIN [al].[MotivosDevoluciones]
    }
}