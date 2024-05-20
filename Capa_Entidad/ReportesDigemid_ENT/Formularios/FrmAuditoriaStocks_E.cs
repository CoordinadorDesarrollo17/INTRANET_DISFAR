using Capa_Entidad.General_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.ReportesDigemid_ENT.Formularios
{
    public class FrmAuditoriaStocks_E
    {
        public string TipoFecha { get; set; }
        public string FecIni { get; set; }
        public string FecFin { get; set; }
        public string ArtIni { get; set; }
        public string ArtFin { get; set; }
        public int ItmsGrpCod { get; set; }
        public List<OWHS_E> Almacenes { get; set; }
}
}
