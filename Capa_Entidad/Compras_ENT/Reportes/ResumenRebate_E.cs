using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Compras_ENT.Reportes
{
    public class ResumenRebate_E
    {
        public List<string> Fabricantes { get; set; }
        public List<string> Proveedores { get; set; }
        public List<string> Descripciones { get; set; }
        public List<string> SubTipos { get; set; }
        public List<decimal> Rebates { get; set; }
        public List<string> PerInis { get; set; }
        public List<string> PerFins { get; set; }
        public List<decimal> DisplayPactadas { get; set; }
        public List<decimal> DisplayActuales { get; set; }
        public List<decimal> CuotaPactadas { get; set; }
        public List<decimal> TotalComprados { get; set; }
        public List<decimal> Diferencias { get; set; }
        public List<string> Estados { get; set; }

        public ResumenRebate_E()
        {
            Fabricantes = new List<string>(); Proveedores = new List<string>();Descripciones = new List<string>();
            SubTipos = new List<string>(); Rebates = new List<decimal>(); PerInis = new List<string>();
            PerFins = new List<string>();DisplayPactadas = new List<decimal>(); DisplayActuales = new List<decimal>();
            CuotaPactadas = new List<decimal>(); TotalComprados = new List<decimal>(); Diferencias = new List<decimal>();
            Estados = new List<string>();
        }
    }
}
