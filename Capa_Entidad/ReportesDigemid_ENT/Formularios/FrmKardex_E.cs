using Capa_Entidad.DireccionTecnica_ENT.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.ReportesDigemid_ENT.Formularios
{
    public class FrmKardex_E
    {
        public string WhsCode { get; set; }
        public string FecIni { get; set; }
        public string FecFin { get; set; }
        public int FirmCode { get; set; }
        public string ItemCode { get; set; }
        public string Lote { get; set; }
        /********************* C A M P O S  Q U E   N O   S O N   D E   L A   T A B L A *********************/
        public string WhsName { get; set; }
        public string ItemName { get; set; }
        public List<RptKardexAlmacenes_E> DetKardexAlmacenes { get; set; }
    }
}
