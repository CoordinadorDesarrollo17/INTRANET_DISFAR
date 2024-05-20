using System.ComponentModel;

namespace Capa_Entidad.Ventas_ENT.Reportes
{
	public class RptTicketACuadrar_E
	{
		[DisplayName("N° TICKET")] public int DocNumTicket { get; set; }
		[DisplayName("FECHA TICKET")] public string FechaSapTicket { get; set; }
		[DisplayName("RUC EMPRESA")] public string CardCode { get; set; }
		[DisplayName("CLIENTE")] public string CardName { get; set; }
		[DisplayName("TIPO DE VENTA")] public string TipoVenta { get; set; }
		[DisplayName("VENDEDOR")] public string Vendedor { get; set; }
		[DisplayName("FORMA DE PAGO")] public string FormaPago { get; set; }
		[DisplayName("ESTADO DE PAGO")] public string EstadoPago { get; set; }
		[DisplayName("TIPO DE PAGO REPARTO")] public string TipoPago { get; set; }
		[DisplayName("ESTADO DE CUADRE")] public string EstadoCuadre { get; set; }
		[DisplayName("FECHA COBRO")] public string FechaCobro { get; set; }
		[DisplayName("FECHA CUADRE")] public string FechaCuadre { get; set; }
		[DisplayName("PER. RECEPCIONA")] public string PersonaRecepciona { get; set; }
		[DisplayName("PER. ENTREGA")] public string PersonaEntrega { get; set; }
		[DisplayName("PAGO EFECTIVO")] public decimal MontoRecibidoEfectivo { get; set; }
		[DisplayName("PAGO DEPÓSITO")] public decimal MontoRecibidoDeposito { get; set; }
		[DisplayName("TOTAL TICKET")] public decimal MontoFinal { get; set; }
		[DisplayName("COMPORTAMIENTO PAGO")] public string ComportamientoPago { get; set; }
	}
}
