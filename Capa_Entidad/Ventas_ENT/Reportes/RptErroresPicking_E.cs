using Capa_Entidad.Almacen_ENT.ReportesSql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.Reportes
{
	public class RptErroresPicking_E
	{
		[DisplayName("NRO TICKET")] public int DocNumTicket { get; set; }
		[DisplayName("FECHA TICKET")] public string FechaTicket { get; set; }
		[DisplayName("ESTADO TICKET")] public string EstadoTicket { get; set; }
		[DisplayName("CÓDIGO PRODUCTO")] public string CodigoProducto { get; set; }
		[DisplayName("DESCRIPCIÓN PRODUCTO")] public string DescripcionProducto { get; set; }
		[DisplayName("TIPO ERROR")] public string TipoError { get; set; }
		[DisplayName("PICKER RESPONSABLE")] public string PickerResponsable { get; set; }
		[DisplayName("ESTADO")] public string Estado { get; set; }
		[DisplayName("OPERARIO REGISTRO")] public string OpRegistro { get; set; }
		[DisplayName("FECHA REGISTRO")] public string FechaRegistro { get; set; }
		[DisplayName("HORA REGISTRO")] public string HoraRegistro { get; set; }
	}

	public class RptFiltrosErroresPicking_E
	{
		public int DocNumTicket { get; set; }
		public string Estado { get; set; }
		public string FechaTicketDesde { get; set; }
		public string FechaTicketHasta { get; set; }
	}
}