using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.Tablas
{
    public class OWTQ_E // solicitud de traslado
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string NumAtCard { get; set; }
        public string SlpName { get; set; }
    }
}
