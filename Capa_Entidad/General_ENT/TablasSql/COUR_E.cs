using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.General_ENT.TablasSql
{
   public  class COUR_E
    {
        public int Id { get; set; }
        public string Ruc { get; set; }
        public string Nombre { get; set; }
        public string DireccionFiscal { get; set; }
        public decimal MinDomicilio { get; set; }
        public decimal MinAgencia { get; set; }
    }
}
