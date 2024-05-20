using Capa_Entidad.Almacen_ENT.ReportesSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System.Data;

namespace Capa_Datos.Almacen_DAO.ReportesSql
{
    public class ContInv_D
    {
        DBHelper db = new DBHelper();
        public List<ContInv_E> RptContInv(OIAR_E o)
        {
            List<ContInv_E> lista = new List<ContInv_E>();
            try
            {
                SqlDataReader dr = db.ExecuteReader("al.RptContInv", o.DocEntryPer, o.ItemCode, o.Fase);
                while (dr.Read())
                {
                    ContInv_E bean = new ContInv_E();
                    if (!dr.IsDBNull(0)) { bean.DocEntryPer = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { bean.DescripcionPer = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { bean.FecIniPer = dr.GetDateTime(2).ToString("dd/MM/yyyy"); }
                    if (!dr.IsDBNull(3)) { bean.FecFinPer = dr.GetDateTime(3).ToString("dd/MM/yyyy"); }
                    if (!dr.IsDBNull(4)) { bean.Fase = dr.GetInt32(4); }
                    if (!dr.IsDBNull(5)) { bean.NombreFase = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { bean.WhsCode = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { bean.ItemCode = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { bean.ItemName = dr.GetString(8); }
                    if (!dr.IsDBNull(9)) { bean.QuantityCajas = dr.GetDecimal(9); }
                    if (!dr.IsDBNull(10)) { bean.QuantityPiezas = dr.GetDecimal(10); }
                    if (!dr.IsDBNull(11)) { bean.Equipos = dr.GetString(11); }
                    if (!dr.IsDBNull(12)) { bean.NumInBuy = dr.GetDecimal(12); }
                    if (!dr.IsDBNull(13)) { bean.QuantityTotalPzSist = dr.GetDecimal(13); }
                    if (bean.Fase == 3)
                    {
                        if (!dr.IsDBNull(15)) { bean.ParticipantesC = dr.GetString(15); }
                        bean.inicializarConteo();
                    }
                    else if (bean.Fase == 5)
                    {
                        if (!dr.IsDBNull(14)) { bean.DifConteo = dr.GetDecimal(14); }
                        if (!dr.IsDBNull(15)) { bean.ParticipantesR = dr.GetString(15); }
                        bean.inicializarReconteo();
                    }
                    else if (bean.Fase == 7)
                    {
                        if (!dr.IsDBNull(14)) { bean.DifConteo = dr.GetDecimal(14); }
                        if (!dr.IsDBNull(15)) { bean.DifReConteo = dr.GetDecimal(15); }
                        if (!dr.IsDBNull(16)) { bean.DifAnalisis = dr.GetDecimal(16); }
                        if (!dr.IsDBNull(17)) { bean.AvgPrice = dr.GetDecimal(17); }
                        if (!dr.IsDBNull(18)) { bean.ObsLoteC = dr.GetString(18); }
                        if (!dr.IsDBNull(19)) { bean.ObsLoteR = dr.GetString(19); }
                        if (!dr.IsDBNull(20)) { bean.ObsLoteA = dr.GetString(20); }
                        if (!dr.IsDBNull(21)) { bean.BatchNum = dr.GetString(21); }
                        if (!dr.IsDBNull(22)) { bean.ExpDate = dr.GetDateTime(22).ToString("dd/MM/yyyy"); }
                        if (!dr.IsDBNull(23)) { bean.Pisos = dr.GetString(23); }
                        if (!dr.IsDBNull(24)) { bean.ParticipantesC = dr.GetString(24); }
                        if (!dr.IsDBNull(25)) { bean.ParticipantesR = dr.GetString(25); }
                        if (!dr.IsDBNull(26)) { bean.ParticipantesA = dr.GetString(26); }
                        bean.inicializarAnalisis();
                    }
                    lista.Add(bean);
                }
                dr.Close();
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
        public DataTable tbRptContInv(OIAR_E o)
        {
            List<string> campos = new List<string>();
            List<Type> tipos = new List<Type>();
            campos.Add("Orden"); tipos.Add(typeof(string));
            campos.Add("DocEntryPer"); tipos.Add(typeof(string));
            campos.Add("DescripcionPer"); tipos.Add(typeof(string));
            campos.Add("FecIniPer"); tipos.Add(typeof(string));
            campos.Add("FecFinPer"); tipos.Add(typeof(string));
            campos.Add("Fase"); tipos.Add(typeof(string));
            campos.Add("NombreFase"); tipos.Add(typeof(string));
            campos.Add("WhsCode"); tipos.Add(typeof(string));
            campos.Add("ItemCode"); tipos.Add(typeof(string));
            campos.Add("ItemName"); tipos.Add(typeof(string));
            campos.Add("BatchNum"); tipos.Add(typeof(string));
            campos.Add("ExpDate"); tipos.Add(typeof(string));
            campos.Add("Pisos"); tipos.Add(typeof(string));
            campos.Add("QuantityCajas"); tipos.Add(typeof(string));
            campos.Add("QuantityPiezas"); tipos.Add(typeof(string));
            campos.Add("QuantityTotalPzCont"); tipos.Add(typeof(string));
            campos.Add("QuantityTotalPzSist"); tipos.Add(typeof(string));
            campos.Add("DifConteo"); tipos.Add(typeof(string));
            campos.Add("DifReConteo"); tipos.Add(typeof(string));
            campos.Add("DifAnalisis"); tipos.Add(typeof(string));
            campos.Add("DifDif"); tipos.Add(typeof(string));
            campos.Add("Resultado"); tipos.Add(typeof(string));
            campos.Add("Equipos"); tipos.Add(typeof(string));
            campos.Add("NumInBuy"); tipos.Add(typeof(string));
            campos.Add("AvgPrice"); tipos.Add(typeof(string));
            campos.Add("ObsLoteC"); tipos.Add(typeof(string));
            campos.Add("ObsLoteR"); tipos.Add(typeof(string));
            campos.Add("ObsLoteA"); tipos.Add(typeof(string));
            campos.Add("ParticipantesC"); tipos.Add(typeof(string));
            campos.Add("ParticipantesR"); tipos.Add(typeof(string));
            campos.Add("ParticipantesA"); tipos.Add(typeof(string));
            DataTable tb = definirTabla(campos, tipos, "DataTableReporteContInv");
            int i = 0;
            foreach (ContInv_E p in RptContInv(o))
            {
                DataRow row = tb.NewRow();
                row["Orden"] = i++;
                row["DocEntryPer"] = p.DocEntryPer;
                row["DescripcionPer"] = p.DescripcionPer;
                row["FecIniPer"] = p.FecIniPer;
                row["FecFinPer"] = p.FecFinPer;
                row["Fase"] = p.Fase;
                row["NombreFase"] = p.NombreFase;
                row["WhsCode"] = p.WhsCode;
                row["ItemCode"] = p.ItemCode;
                row["ItemName"] = p.ItemName;
                row["BatchNum"] = p.BatchNum;
                row["ExpDate"] = p.ExpDate;
                row["Pisos"] = p.Pisos;
                row["QuantityCajas"] = p.QuantityCajas;
                row["QuantityPiezas"] = p.QuantityPiezas;
                row["QuantityTotalPzCont"] = p.QuantityTotalPzCont;
                row["QuantityTotalPzSist"] = p.QuantityTotalPzSist;
                row["DifConteo"] = p.DifConteo;
                row["DifReConteo"] = p.DifReConteo;
                row["DifAnalisis"] = p.DifAnalisis;
                row["DifDif"] = p.DifDif;
                row["Resultado"] = p.Resultado;
                row["Equipos"] = p.Equipos;
                row["NumInBuy"] = p.NumInBuy;
                row["AvgPrice"] = p.AvgPrice;
                row["ObsLoteC"] = p.ObsLoteC;
                row["ObsLoteR"] = p.ObsLoteR;
                row["ObsLoteA"] = p.ObsLoteA;
                row["ParticipantesC"] = p.ParticipantesC;
                row["ParticipantesR"] = p.ParticipantesR;
                row["ParticipantesA"] = p.ParticipantesA;
                tb.Rows.Add(row);
            }
            return tb;
        }
    }
}