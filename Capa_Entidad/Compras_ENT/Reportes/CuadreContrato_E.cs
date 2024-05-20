using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Compras_ENT.Reportes
{
    public class CuadreContrato_E
    {
        public string Tipo { get; set; }
        public string SocioDesc { get; set; }
        public string PerIni { get; set; }
        public string PerFin { get; set; }
        public string Estado { get; set; }
        public List<DetCuadreContrato_E> Det { get; set; }

    }
}
