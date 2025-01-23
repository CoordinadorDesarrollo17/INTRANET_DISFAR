using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.General_ENT.TablasSql
{
    public class UBICACIONES_E
    {
        public int Id { get; set; }
        public string CodigoProducto { get; set; }
        public string NombreProducto { get; set; }
        public string[] CodigoUbicacion { get; set; }
    }
}
