
using System.ComponentModel;

namespace Capa_Entidad.ComprobantesContables_ENT
{
    public class Comprobante_E
    {
        public int Id { get; set; }
        public string TablaSAP { get; set; }
        public string Identificador { get; set; }
        public int DocEntryTicket { get; set; }
        [DisplayName("Tipo")]
        public string U_SYP_MDTD { get; set; }
        [DisplayName("Serie")]
        public string U_SYP_MDSD { get; set; }
        [DisplayName("Correlativo")]
        public string U_SYP_MDCD { get; set; }
        [DisplayName("Fecha creación")]
        public string DocDate { get; set; }
        [DisplayName("Fecha Traslado")]
        public string U_BPP_FECINITRA { get; set; }
        public int Impreso { get; set; }
        public string TipoDocumentoImpreso { get; set; }
        public string Operario { get; set; }
        public string FechaOperación { get; set; }
        public string HoraOperación { get; set; }
        public decimal DocTotal { get; set; }
        public decimal AnticipoBruto { get; set; }
    }
}
