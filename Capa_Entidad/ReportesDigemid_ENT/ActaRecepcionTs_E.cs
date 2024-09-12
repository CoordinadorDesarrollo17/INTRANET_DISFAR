using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.ReportesDigemid_ENT
{
    public class ActaRecepcionTs_E
    {
        public string T1_DocDate{ get; set; }
        public string CodAlmacenEnvio { get; set; }
        public string CodAlmacenDestino { get; set; }
        public string NroDeGuia { get; set; }
        public string T2_ItemCode { get; set; }
        public string T2_Dscription { get; set; }
        public decimal T3_Quantity { get; set; }
        public string T8_FrgnName { get; set; }
        public string Concentracion { get; set; }
        public string FormaPresentacion { get; set; }
        public string FormaFarmaceutica { get; set; }
        public string Fabricante { get; set; }
        public string Lote { get; set; }
        public string FechaVenc { get; set; }
        public string RegistroSan { get; set; }
        public string CondAlmac { get; set; }
        public string TaxOfficeAlmacenEnvio { get; set; }
        public string TaxOfficeAlmacenDestino { get; set; }
        public string AlmacenEnvio { get; set; }
        public string AlmacenDestino { get; set; }
    }
}
