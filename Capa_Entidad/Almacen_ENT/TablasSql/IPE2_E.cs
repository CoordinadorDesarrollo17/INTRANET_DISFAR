using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.TablasSql
{
    public class IPE2_E
    {
        public int DocEntry { get; set; }
        public string ItemCode { get; set; }
        [DisplayName("Lote")]
        public string BatchNum { get; set; }
        [DisplayName("Almacen")]
        public string WhsCode { get; set; }
        [DisplayName("Descripcion")]
        public string ItemName { get; set; }
        [DisplayName("F.V.")]
        public DateTime ExpDate  { get; set; }
        [DisplayName("StockPza")]
        public decimal Quantity { get; set; }
        [DisplayName("U.m.")]
        public decimal NumInBuy { get; set; }
        [DisplayName("CostoIgv")]
        public decimal AvgPrice { get; set; }
        [DisplayName("GrupoArticulo")]
        public int ItmsGrpCod { get; set; }
        public int FirmCode { get; set; }
        //campos no de la tabla
        public string AuxExpDate { get; set; }
        //metodos
        public static DataTable tbDetalle(List<IPE2_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("ItemCode", typeof(string));
            tb.Columns.Add("BatchNum", typeof(string));
            tb.Columns.Add("WhsCode", typeof(string));
            tb.Columns.Add("ItemName", typeof(string));
            tb.Columns.Add("ExpDate", typeof(DateTime));
            tb.Columns.Add("Quantity", typeof(decimal));
            tb.Columns.Add("NumInBuy", typeof(decimal));
            tb.Columns.Add("AvgPrice", typeof(decimal));
            tb.Columns.Add("ItmsGrpCod", typeof(int));
            tb.Columns.Add("FirmCode", typeof(int));
            foreach (IPE2_E reg in dt)
            {
                tb.Rows.Add(reg.DocEntry, reg.ItemCode, reg.BatchNum,reg.WhsCode,reg.ItemName,reg.ExpDate,reg.Quantity
                    ,reg.NumInBuy,reg.AvgPrice,reg.ItmsGrpCod,reg.FirmCode);
            }
            return tb;
        }
        public decimal CostoIgv()
        {
            if(ItmsGrpCod!=102)
            {
                return AvgPrice * 1.18M;
            }
            return AvgPrice;
        }
    }
}
