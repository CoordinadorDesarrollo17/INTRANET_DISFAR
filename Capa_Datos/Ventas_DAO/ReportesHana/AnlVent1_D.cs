using Capa_Entidad.Ventas_ENT.ReportesHana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sap.Data.Hana;
using System.Data;

namespace Capa_Datos.Ventas_DAO.ReportesHana
{
    public class AnlVent1_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public List<AnlVent1_E> RptAnlVent1(string FecIni,string FecFin,int SlpCode,string CardCode,int FirmCode)
        {
            List<AnlVent1_E> lista = new List<AnlVent1_E>();
            string query = "call "+uti.schemaHana+"DIEGO_RPT_ANLVENT('"+FecIni+"','"+FecFin+"',"+SlpCode+",'"+CardCode+"',"+FirmCode+")";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while(hdr.Read())
                {
                    AnlVent1_E obj = new AnlVent1_E();
                    if (!hdr.IsDBNull(0)) { obj.CardCode = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { obj.RazonSocial = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { obj.ImporteVentas = hdr.GetDecimal(2); }
                    if (!hdr.IsDBNull(3)) { obj.FirmName = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { obj.ItemCode = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { obj.Articulo = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { obj.CantidadCj = Math.Truncate(hdr.GetDecimal(6)); }
                    if (!hdr.IsDBNull(7)) { obj.PrecioIgvCj = hdr.GetDecimal(7); }
                    if (!hdr.IsDBNull(8)) { obj.Departamento = hdr.GetString(8); }
                    if (!hdr.IsDBNull(9)) { obj.Vendedor = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { obj.Ene = Math.Truncate(hdr.GetDecimal(10)); }
                    if (!hdr.IsDBNull(11)) { obj.Feb = Math.Truncate(hdr.GetDecimal(11)); }
                    if (!hdr.IsDBNull(12)) { obj.Mar = Math.Truncate(hdr.GetDecimal(12)); }
                    if (!hdr.IsDBNull(13)) { obj.Abr = Math.Truncate(hdr.GetDecimal(13)); }
                    if (!hdr.IsDBNull(14)) { obj.May = Math.Truncate(hdr.GetDecimal(14)); }
                    if (!hdr.IsDBNull(15)) { obj.Jun = Math.Truncate(hdr.GetDecimal(15)); }
                    if (!hdr.IsDBNull(16)) { obj.Jul = Math.Truncate(hdr.GetDecimal(16)); }
                    if (!hdr.IsDBNull(17)) { obj.Ago = Math.Truncate(hdr.GetDecimal(17)); }
                    if (!hdr.IsDBNull(18)) { obj.Set = Math.Truncate(hdr.GetDecimal(18)); }
                    if (!hdr.IsDBNull(19)) { obj.Oct = Math.Truncate(hdr.GetDecimal(19)); }
                    if (!hdr.IsDBNull(20)) { obj.Nov = Math.Truncate(hdr.GetDecimal(20)); }
                    if (!hdr.IsDBNull(21)) { obj.Dic = Math.Truncate(hdr.GetDecimal(21)); }
                    lista.Add(obj);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        DataTable definirTabla(List<string> campos, List<Type> tipos, string nombre)
        {
            DataTable tb = new DataTable(nombre);
            int i = 0;
            foreach (string campo in campos)
            {
                DataColumn dc = new DataColumn(campo, tipos[i]);
                dc.ReadOnly = true;
                tb.Columns.Add(dc);
                i++;
            }
            return tb;
        }
        public DataTable tbRptAnlVent1(string FecIni, string FecFin, int SlpCode, string CardCode, int FirmCode)
        {
            List<string> campos = new List<string>();
            List<Type> tipos = new List<Type>();
            campos.Add("Orden"); tipos.Add(typeof(string));
            campos.Add("CardCode"); tipos.Add(typeof(string));
            campos.Add("RazonSocial"); tipos.Add(typeof(string));
            campos.Add("ImporteVentas"); tipos.Add(typeof(string));
            campos.Add("FirmName"); tipos.Add(typeof(string));
            campos.Add("ItemCode"); tipos.Add(typeof(string));
            campos.Add("Articulo"); tipos.Add(typeof(string));
            campos.Add("CantidadCj"); tipos.Add(typeof(string));
            campos.Add("PrecioIgvCj"); tipos.Add(typeof(string));
            campos.Add("Departamento"); tipos.Add(typeof(string));
            campos.Add("Vendedor"); tipos.Add(typeof(string));
            campos.Add("Ene"); tipos.Add(typeof(string));
            campos.Add("Feb"); tipos.Add(typeof(string));
            campos.Add("Mar"); tipos.Add(typeof(string));
            campos.Add("Abr"); tipos.Add(typeof(string));
            campos.Add("May"); tipos.Add(typeof(string));
            campos.Add("Jun"); tipos.Add(typeof(string));
            campos.Add("Jul"); tipos.Add(typeof(string));
            campos.Add("Ago"); tipos.Add(typeof(string));
            campos.Add("Set"); tipos.Add(typeof(string));
            campos.Add("Oct"); tipos.Add(typeof(string));
            campos.Add("Nov"); tipos.Add(typeof(string));
            campos.Add("Dic"); tipos.Add(typeof(string));
            
            DataTable tb = definirTabla(campos, tipos, "DataTableReporteAnlVent1");
            int i = 0;
            foreach (AnlVent1_E p in RptAnlVent1(FecIni,FecFin,SlpCode,CardCode,FirmCode))
            {
                DataRow row = tb.NewRow();
                row["Orden"] = i++;
                row["CardCode"] = p.CardCode;
                row["RazonSocial"] = p.RazonSocial;
                row["ImporteVentas"] = p.ImporteVentas;
                row["FirmName"] = p.FirmName;
                row["ItemCode"] = p.ItemCode;
                row["Articulo"] = p.Articulo;
                row["CantidadCj"] = p.CantidadCj;
                row["PrecioIgvCj"] = p.PrecioIgvCj;
                row["Departamento"] = p.Departamento;
                row["Vendedor"] = p.Vendedor;
                row["Ene"] = p.Ene;
                row["Feb"] = p.Feb;
                row["Mar"] = p.Mar;
                row["Abr"] = p.Abr;
                row["May"] = p.May;
                row["Jun"] = p.Jun;
                row["Jul"] = p.Jul;
                row["Ago"] = p.Ago;
                row["Set"] = p.Set;
                row["Oct"] = p.Oct;
                row["Nov"] = p.Nov;
                row["Dic"] = p.Dic;
                tb.Rows.Add(row);
            }
            return tb;
        }
    }
}
