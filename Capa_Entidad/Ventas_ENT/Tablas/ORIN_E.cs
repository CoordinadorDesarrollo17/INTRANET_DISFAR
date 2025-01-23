using System.ComponentModel;

namespace Capa_Entidad.Ventas_ENT.Tablas
{
    public class ORIN_E
    {
        // nota de credito cliente
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        [DisplayName("FechaCont")]
        public string DocDate { get; set; }
        public string CardCode { get; set; }
        [DisplayName("Nombre")]
        public string CardName { get; set; }
        [DisplayName("Comprobante")]
        public string NumAtCard { get; set; }
        [DisplayName("Total")]
        public decimal DocTotal { get; set; }
        [DisplayName("EstadoDoc")]
        public string U_SYP_STATUS { get; set; }
        public string RefFactura { get; set; }
        public string DocType { get; set; }
        //
        public string SerieDoc { get; set; }
        public string CorreDoc { get; set; }
        public string DirPagar { get; set; }
        public string Ruc { get; set; }
        public string MonedaLetras { get; set; }
    }
}
