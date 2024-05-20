
using System.ComponentModel;

namespace Capa_Entidad.Rutas_ENT.ReportesSql
{
	public class RptPesaje_E
	{
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string CardCode { get; set; }                                // Usado como filtro

        [DisplayName("Cliente")]
        public string CardName { get; set; }
        public string Estado { get; set; }
        public string TipoVenta { get; set; }
        public string LugarDestino { get; set; }
        public string DirDestino { get; set; }
        public string Referencia { get; set; }
        public string Agencia { get; set; }
        public string EnvioAgencia { get; set; }
        public string Embalaje { get; set; }
        public int CodSapVendedor { get; set; }
        public string Vendedor { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal Flete { get; set; }
        public decimal GastoEnvio { get; set; }
        public string EstadoGasto { get; set; }
		public decimal PagoEnv { get; set; }
		public string ClaveEnv { get; set; }
		public string TiempoEntrega { get; set; }
		public decimal DescuentoNC { get; set; }
		public decimal DeudaCliente { get; set; }
		public decimal DeudaEmpresa { get; set; }
        public decimal MontoFinal { get; set; }
        public string FormaPago { get; set; }
        public decimal MontoRecibido { get; set; }
        [DisplayName("EstPago")] public string EstadoPago { get; set; }
        public string FechaPago { get; set; }
        public string HoraPago { get; set; }
        public int CodSapCajero { get; set; }
        public string Cajero { get; set; }
        public string Comentario { get; set; }
        public int Cajas { get; set; }
        public int NroMesa { get; set; }
        public string FechaNC { get; set; }
        public string EstadoFacturacion { get; set; }
        public string FechaFacturacion { get; set; }
        public string HoraFacturacion { get; set; }
        public string OpFacturacion { get; set; }
        public string Observaciones { get; set; }
        public string Observaciones2 { get; set; }
        public string Observaciones3 { get; set; }
        public string FechaSapTicket { get; set; }
        public string AlmProcedencia { get; set; }
        public int LineaPeso { get; set; }
        public decimal Peso { get; set; }
       
	}

	public class FiltroRptPesaje : RptPesaje_E
	{
		public string FecConIni { get; set; }
		public string FecConFin { get; set; }
	}
}