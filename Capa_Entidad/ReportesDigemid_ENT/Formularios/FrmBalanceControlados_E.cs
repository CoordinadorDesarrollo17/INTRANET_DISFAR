using Capa_Entidad.DireccionTecnica_ENT.Reportes.BalanceControlados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.ReportesDigemid_ENT.Formularios
{
    public class FrmBalanceControlados_E
    {
        public string Informe { get; set; }
        public string FecIni { get; set; }
        public string FecFin { get; set; }
        public string TipoControlado { get; set; }
        /********************* C A M P O S  Q U E   N O   S O N   D E   L A   T A B L A *********************/
        public string DescTipoControlado { get; set; }
        public List<RptBalanceControladosIngreso_E> DetBalanceControladosIngreso { get; set; }
        public List<RptBalanceControladosEgreso_E> DetBalanceControladosEgreso { get; set; }
        public List<RptBalanceControladosConsolidado_E> DetBalanceControladosConsolidado { get; set; }
        public List<RptBalanceControladosLibroControlados_E> DetBalanceControladosLibroControlados { get; set; }
    }
}
