using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.ReportesDigemid_ENT
{
    public class OrganolepticoEm_E
    {
        public string T0_CardName { get; set; }
        public string T0_DocDate { get; set; }
        public decimal T1_NumPerMsr { get; set; }
        public decimal Canti_pza_lot { get; set; }
        public string Lote { get; set; }
        public string T3_ExpDate { get; set; }
        public string Registro { get; set; }
        public string T6_Location { get; set; }
        public string T7_FrgnName { get; set; }
        public string Concentracion { get; set; }
        public string FormaPresentacion { get; set; }
        public string FormaFarmaceutica { get; set; }
        public string Fabricante { get; set; }
        public string NroFactura { get; set; }
        public decimal XDevolucion { get; set; }

        // Campos extras
        public string ComentarioOrganoleptico { get; set; }

    }
}
