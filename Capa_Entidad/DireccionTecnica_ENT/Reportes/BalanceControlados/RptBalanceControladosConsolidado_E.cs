using System;
using System.Collections.Generic;

namespace Capa_Entidad.DireccionTecnica_ENT.Reportes.BalanceControlados
{
	public class RptBalanceControladosConsolidado_E
	{
		public string RazonSocial { get; set; }
		public string NmComercial { get; set; }
		public string RucCob { get; set; }
		public string Direccion { get; set; }
		public string Telefono2 { get; set; }
		public string Quimico { get; set; }
		public string Correo { get; set; }
		public int Anio { get; set; }
		public string CodProducto { get; set; }
		public string NombreGenerico { get; set; }
		public string NombreComercial { get; set; }
		public string Concentracion { get; set; }
		public string FormaFamaceutica { get; set; }
		public decimal SaldoAnterior { get; set; }
		public decimal Compra { get; set; }
		public decimal Venta { get; set; }
		public decimal OtrosIngresosNC { get; set; }
		public decimal OtrosEgresosDEV { get; set; }
		public decimal OtrosEgresosSAM { get; set; }
		public int BaseType { get; set; }
		public int CreatedBy { get; set; }
		public string TipoControlado { get; set; }
		public string Trimestre { get; set; }
	}
}
