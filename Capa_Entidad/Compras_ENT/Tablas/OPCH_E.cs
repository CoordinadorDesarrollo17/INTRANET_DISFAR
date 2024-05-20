using System.Collections.Generic;
using System.ComponentModel;

namespace Capa_Entidad.Compras_ENT.Tablas
{
    public class OPCH_E
    {
        // factura de proveedores
        public int DocEntry { get; set; }
        [DisplayName("NroDocumento")]
        public int DocNum { get; set; }
        [DisplayName("FechaCont")]
        public string DocDate { get; set; }
        [DisplayName("FechaDoc")]
        public string TaxDate { get; set; }
        public string CardCode { get; set; }
        [DisplayName("NombreProv.")]
        public string CardName { get; set; }
        [DisplayName("Comprobante")]
        public string NumAtCard { get; set; }
        [DisplayName("Total")]
        public decimal DocTotal { get; set; }
        [DisplayName("EstadoDoc")]
        public string U_SYP_STATUS { get; set; }
        [DisplayName("Comentario")]
        public string Comments { get; set; }
        // fuera de tabla
        public List<PCH1_E> det { get; set; }
        // metodos
        [DisplayName("Entradas Vinculadas")]
        public List<OPDN_E> entradasVinculadas { get; set; }
    }
}
