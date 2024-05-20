using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.ReportesDigemid_ENT.Reportes
{
    public class AuditoriaStocks_E
    {
        public string DeFecha { get; set; }
        public string HastaFecha { get; set; }
        public string Cuentas { get; set; }
        public string Moneda { get; set; }
        public string CuentaMayor { get; set; }
        [DisplayName("Fecha del Sistema")]
        public string CreateDate { get; set; }
        [DisplayName("Fecha de Contabilizacion")]
        public string DocDate { get; set; }
        public string Abreviatura { get; set; }
        [DisplayName("Documento")]
        public int DocNum { get; set; }
        [DisplayName("Número de Artículo")]
        public string ItemCode { get; set; }
        [DisplayName("Descripción del Artículo")]
        public string ItemName { get; set; }
        [DisplayName("Almacén")]
        public string WhsCode { get; set; }
        [DisplayName("Cantidad")]
        public decimal Quantity { get; set; }
        public decimal Costos { get; set; }
        public decimal ValorTrans { get; set; }
        public decimal ValorAcumulado { get; set; }
        public decimal ValorAcumuladoTotal { get; set; }
        public string CardName { get; set; }
        public decimal CantidadAcumulada { get; set; }
    }
}
