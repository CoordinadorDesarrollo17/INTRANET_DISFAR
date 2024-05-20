using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.ReportesDigemid_ENT.Reportes
{
    public class OperacionesLotes_E
    {
        [DisplayName("Número de Artículo")]
        public string ItemCode { get; set; }
        [DisplayName("Descripción")]
        public string ItemName { get; set; }
        [DisplayName("Lote")]
        public string DistNumber { get; set; }
        [DisplayName("Almacén")]
        public string WhsCode { get; set; }
        [DisplayName("Nombre de Almacén")]
        public string WhsName { get; set; }
        [DisplayName("Cantidad")]
        public decimal QuantityTotal { get; set; }
        public decimal ImputadoTotal { get; set; }
        [DisplayName("Registro Sanitario")]
        public string MnfSerial { get; set; }
        [DisplayName("Fecha Vencimiento")]
        public string ExpDate { get; set; }
        public string FechaAdmision { get; set; }
        public string Temperatura { get; set; }
        //detalles
        [DisplayName("Documento")]
        public int DocNum { get; set; }
        [DisplayName("Fecha")]
        public string DocDate { get; set; }
        [DisplayName("Nombre de Proveedor")]
        public string CardName { get; set; }
        [DisplayName("Cantidad")]
        public decimal Quantity { get; set; }
        public decimal Imputado { get; set; }
        public string Sentido { get; set; }
        public string Abreviatura { get; set; }
    }
}
