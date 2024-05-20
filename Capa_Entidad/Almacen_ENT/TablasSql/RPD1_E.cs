using System.Collections.Generic;
using System.Data;
namespace Capa_Entidad.Almacen_ENT.TablasSql
{
    public class RPD1_E
{
    public int DocEntry { get; set; }
    public int Linea { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public int FirmCode { get; set; }
    public string BatchNum { get; set; }
    public string ExpDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal MaxQuantity { get; set; }
    public decimal NumInBuy { get; set; }
    public string BuyUnitMsr { get; set; }       // UomCode 
    public int Motivo { get; set; }
    public string RefFactura { get; set; }
    public string Observacion { get; set; }
    public string ExpDateFormat { get; set; }
    public string DescMotivo { get; set; }          // Campo INNER JOIN -> al.MotivosDevoluciones
    public int Submotivo { get; set; }
    public string DescSubmotivo { get; set; }          // Campo LEFT JOIN -> al.SubmotivosDevoluciones
    public decimal MaxQuantityOIBT { get; set; }         
    public decimal NumInBuyKey { get; set; }         
    public static List<RPD1_E> Registros(List<RPD1_E> dt, int DocEntry)
    {
        List<RPD1_E> lista = new List<RPD1_E>();
        int ln = 1;
        foreach (RPD1_E d in dt)
        {
            d.DocEntry = DocEntry;
            d.Linea = ln;
            lista.Add(d);
            ln++;
        }
        return lista;
    }

    public static DataTable TbDetalle(List<RPD1_E> dt, int DocEntry)
    {
        DataTable tb = new DataTable();
        tb.Columns.Add("DocEntry", typeof(int));
        tb.Columns.Add("Linea", typeof(int));
        tb.Columns.Add("ItemCode", typeof(string));
        tb.Columns.Add("ItemName", typeof(string));
        tb.Columns.Add("FirmCode", typeof(int));
        tb.Columns.Add("BatchNum", typeof(string));
        tb.Columns.Add("ExpDate", typeof(string));
        tb.Columns.Add("Quantity", typeof(decimal));
        tb.Columns.Add("NumInBuy", typeof(decimal));
        tb.Columns.Add("BuyUnitMsr", typeof(string));
        tb.Columns.Add("Motivo", typeof(int));
        tb.Columns.Add("RefFactura", typeof(string));
        tb.Columns.Add("Observacion", typeof(string));
        tb.Columns.Add("MaxQuantity", typeof(decimal));
        tb.Columns.Add("Submotivo", typeof(int));
        tb.Columns.Add("MaxQuantityOIBT", typeof(decimal));
        tb.Columns.Add("NumInBuyKey", typeof(decimal));

        foreach (RPD1_E reg in Registros(dt, DocEntry))
        {
            tb.Rows.Add(reg.DocEntry, reg.Linea, reg.ItemCode, reg.ItemName, reg.FirmCode, reg.BatchNum, reg.ExpDate, reg.Quantity, reg.NumInBuy, reg.BuyUnitMsr, reg.Motivo, reg.RefFactura, reg.Observacion, reg.MaxQuantity, reg.Submotivo,reg.MaxQuantityOIBT,reg.NumInBuyKey);
        }
        return tb;
    }
    public static DataTable TbDetalleFU(List<RPD1_E> dt)
    {
        DataTable tb = new DataTable();
        tb.Columns.Add("ItemCode", typeof(string));
        tb.Columns.Add("Dscription", typeof(string));
        tb.Columns.Add("Quantity", typeof(decimal));
        tb.Columns.Add("BuyUnitMsr", typeof(string));

        foreach (RPD1_E reg in dt)
        {
            tb.Rows.Add(reg.ItemCode, reg.ItemName, reg.Quantity, reg.BuyUnitMsr);
        }
        return tb;
    }
    }
}