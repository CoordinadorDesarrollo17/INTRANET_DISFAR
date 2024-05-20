using System;
using System.Collections.Generic;

namespace Capa_Entidad.DireccionTecnica_ENT.Reportes
{
	public class RptKardexAlmacenes_E
	{
		public string DescProducto { get; set; }
		public string CodProducto { get; set; }
		public string RegSanitario { get; set; }
		public string FechaCont { get; set; }
		public string FacturaGuiaBoleta { get; set; }
		public string ProvEstab { get; set; }
		public string Ruc { get; set; }
		public string Lote { get; set; }
		public decimal CantLoteIngreso { get; set; }
		public decimal CantLoteSalida { get; set; }
		public decimal CantIngreso { get; set; }
		public decimal CantSalida { get; set; }
		public decimal AcumuladoLote { get; set; }
		public decimal AcumuladoProducto { get; set; }
		public double BaseType { get; set; }
		public int Direction { get; set; }
		public string Warehouse { get; set; }
		public int CreatedBy { get; set; }
	}
}
