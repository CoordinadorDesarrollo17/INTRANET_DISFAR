using System.Collections.Generic;
using System.Web;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class OTC_E
    {
        public int IdOTC { get; set; }
        public int? DocEntryTicket { get; set; }
        public int? DocNumTicket { get; set; }
        public int? DocEntryORRU { get; set; }
        public decimal MontoRecibidoEfectivo { get; set; }
        public decimal MontoRecibidoDeposito { get; set; }
        public string TipoPago { get; set; }
        public string PersonaEntrega { get; set; }
        public string Estado { get; set; }
        public string AutorizacionExcepcional { get; set; }
        public string ComentarioCaja { get; set; }
        public string ComentarioVentas { get; set; }
        public string SaldoAFavor { get; set; }
        public string FechaCompromisoPago { get; set; }
        public string ComportamientoPago { get; set; }
        public string ComentarioAdjunto { get; set; }

        /*********** C A M P O S   Q U E   N O   S O N   D E   L A   T A B L A ***********/
        public string TipoVenta { get; set; }
        public List<HttpPostedFileBase> ImgAdjunta { get; set; }
        public string FechaSapTicket { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string DescTipoPago { get; set; }
        public string Vendedor { get; set; }
    }
}