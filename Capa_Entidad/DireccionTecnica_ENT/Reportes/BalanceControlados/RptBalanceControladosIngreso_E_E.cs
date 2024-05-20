using System;
using System.Collections.Generic;

namespace Capa_Entidad.DireccionTecnica_ENT.Reportes.BalanceControlados
{
	public class RptBalanceControladosIngreso_E
	{
		public string CodProducto { get; set; }
		public string NombreGenerico { get; set; }
		public string NombreComercial { get; set; }
		public string RegSanitario { get; set; }
		public string Concentracion { get; set; }
		public string FormaPresentacion { get; set; }
		public string FormaFamaceutica { get; set; }
		public string NroLote { get; set; }
		public decimal CantLote { get; set; }		
		public string Proveedor { get; set; }
		public string RucProveedor { get; set; }
		public string CalleJrAvN { get; set; }
		public string Distrito { get; set; }
		public string Provincia { get; set; }
		public string Departamento { get; set; }		
		public string NroFacturaNcredito { get; set; }		
		public string Fecha { get; set; }
		public string TipoControlado { get; set; }
	}
}