using System.ComponentModel;

namespace Capa_Entidad.Ventas_ENT.Tablas
{
    public class ODLN_E
    {
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
        [DisplayName("LugarEntrega")]
        public string U_COB_LUGAREN { get; set; }

        [DisplayName("Fecha Traslado")]
        public string U_BPP_FECINITRA { get; set; }
    }
}
