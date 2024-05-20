using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Rutas_ENT.ReportesSql
{
	public class Rpt_TicketVenta_E
	{
		[DisplayName("ALMACEN")]
		public string Almacen { get; set; }
		[Key]
		[DisplayName("TICKET")]
		public int DocEntry { get; set; }
		[DisplayName("OBSERVACIONES")]
		public string Observaciones2 { get; set; }
		[DisplayName("RAZON SOCIAL")]
		public string CardName { get; set; }
		[DisplayName("PERSONA DE RECOJO")]
		public string NombrePer1 { get; set; }
		[DisplayName("DNI")]
		public string DocPer1 { get; set; }
		[DisplayName("CELULAR")]
		public string TelfPer1 { get; set; }
		[DisplayName("VENDEDOR")]
		public string PropietarioDesc { get; set; }
		[DisplayName("AGENCIA")]
		public string Agencia { get; set; }
		[DisplayName("DIRECCION 1")]
		public string DirDestino1 { get; set; }
		[DisplayName("DIRECCION 2")]
		public string DirDestino2 { get; set; }
		[DisplayName("CAJAS")]
		public int Cajas { get; set; }
		[DisplayName("GASTO DE ENVIO")]
		public decimal GastoEnvio { get; set; }
	}
}

