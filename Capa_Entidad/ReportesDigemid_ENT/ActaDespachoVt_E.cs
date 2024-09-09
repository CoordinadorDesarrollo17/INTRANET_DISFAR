using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.ReportesDigemid_ENT
{
    public class ActaDespachoVt_E
    {
		public string T0_ObjType { get; set; }
		public int T0_DocEntry { get; set; }
		public int T0_DocNum { get; set; }
		public string T0_CardCode { get; set; }
		public string T0_CardName { get; set; }
		public string T0_DocDate { get; set; }
		public string T0_NumAtCard { get; set; }
		public string T0_TaxDate { get; set; }
		public int T1_LineNum { get; set; }
		public string T1_ItemCode { get; set; }
		public string T1_WhsCode { get; set; }
		public string T8_ItemName { get; set; }
		public string T8_FrgnName { get; set; }
		public string Concentracion { get; set; }
		public string FormaPresentacion { get; set; }
		public string FormaFarmaceutica { get; set; }
		public string Fabricante { get; set; }
		public int T4_AbsEntry { get; set; }
		public string T4_DistNumber { get; set; }
		public string T4_MnfSerial { get; set; }
		public string T4_ExpDate { get; set; }
		public decimal Quantity { get; set; }
		public string T7_Location { get; set; }
		public string T0_U_COB_TIPODOC { get; set; }
		public string T0_U_COB_SERIE { get; set; }
		public string T0_U_COB_CORDOC { get; set; }
        public string TaxOfficeAlmacen { get; set; }
        public string Almacen { get; set; }
    }
}
