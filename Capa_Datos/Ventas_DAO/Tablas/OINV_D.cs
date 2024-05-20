using Capa_Entidad.ReportesDigemid_ENT.Reportes;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.Tablas;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Capa_Datos.Ventas_DAO.Tablas
{
    public class OINV_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();

        public List<OINV_E> listadoFacturasDeVenta(OINV_E fil)
        {
            List<OINV_E> lista = new List<OINV_E>();
            string filtros = "";
            if (fil != null)
            {
                if (fil.DocNum > 0) { filtros += " and \"DocNum\" like '%" + fil.DocNum + "'"; }
                if (fil.U_BPP_FECINITRA != null) { filtros += " and \"U_BPP_FECINITRA\"='" + fil.U_BPP_FECINITRA + "'"; }
                if (fil.DocDate != null) { filtros += " and \"DocDate\"='" + fil.DocDate + "'"; }
                if (fil.CardName != null) { filtros += " and UPPER(\"CardName\") like UPPER('%" + fil.CardName + "%')"; }
                if (fil.NumAtCard != null) { filtros += " and UPPER(\"NumAtCard\") like UPPER('%" + fil.NumAtCard + "')"; }
                if (fil.U_COB_CORDOC != null) { filtros += " and UPPER(\"U_COB_CORDOC\") like UPPER('%" + fil.U_COB_CORDOC + "')"; }
                if (fil.DocTotal > 0.00M) { filtros += " and \"DocTotal\" like '%" + fil.DocTotal + "%'"; }
                if (fil.U_SYP_STATUS != null) { filtros += " and UPPER(\"U_SYP_STATUS\")=UPPER('" + fil.U_SYP_STATUS + "')"; }
                if (fil.U_COB_LUGAREN != null) { filtros += " and \"U_COB_LUGAREN\"='" + fil.U_COB_LUGAREN + "'"; }
            }

            string query = "select top 50 \"DocEntry\",\"DocNum\",\"CANCELED\",\"DocDate\",\"CardName\",\"NumAtCard\",\"DocTotal\"" +
                                    ",\"U_SYP_STATUS\",\"U_COB_LUGAREN\",\"U_COB_TIPODOC\",\"U_COB_SERIE\",\"U_COB_CORDOC\", \"U_BPP_FECINITRA\"  from " + uti.schemaHana + "OINV " +
                                    "where \"DocEntry\">0 and \"Series\" in(select \"Series\" from " + uti.schemaHana + "nnm1 where \"SeriesName\" like 'FV%')" +
            filtros + " order by 1 desc";


            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    OINV_E f = new OINV_E();
                    f.DocEntry = hdr.GetInt32(0);
                    if (!hdr.IsDBNull(1)) { f.DocNum = hdr.GetInt32(1); }
                    if (!hdr.IsDBNull(2)) { f.CANCELED = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { f.DocDate = hdr.GetDateTime(3).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(4)) { f.CardName = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { f.NumAtCard = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { f.DocTotal = hdr.GetDecimal(6); }
                    if (!hdr.IsDBNull(7)) { f.U_SYP_STATUS = hdr.GetString(7); }
                    if (!hdr.IsDBNull(8)) { f.U_COB_LUGAREN = hdr.GetString(8); }
                    if (!hdr.IsDBNull(9)) { f.U_COB_TIPODOC = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { f.U_COB_SERIE = hdr.GetString(10); }
                    if (!hdr.IsDBNull(11)) { f.U_COB_CORDOC = hdr.GetString(11); }
                    if (!hdr.IsDBNull(12))
                    {
                        f.U_BPP_FECINITRA = hdr.GetDateTime(12).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        f.U_BPP_FECINITRA = f.DocDate;
                    }

                    lista.Add(f);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public List<OINV_E> listadoBoletasDeVenta(OINV_E fil)
        {
            List<OINV_E> lista = new List<OINV_E>();
            string filtros = "";
            if (fil != null)
            {
                if (fil.DocNum > 0) { filtros += " and \"DocNum\" like '%" + fil.DocNum + "'"; }
                if (fil.U_BPP_FECINITRA != null) { filtros += " and \"U_BPP_FECINITRA\"='" + fil.U_BPP_FECINITRA + "'"; }
                if (fil.DocDate != null) { filtros += " and \"DocDate\"='" + fil.DocDate + "'"; }
                if (fil.CardName != null) { filtros += " and UPPER(\"CardName\") like UPPER('%" + fil.CardName + "%')"; }
                if (fil.NumAtCard != null) { filtros += " and UPPER(\"NumAtCard\") like UPPER('%" + fil.NumAtCard + "')"; }
                if (fil.U_COB_CORDOC != null) { filtros += " and UPPER(\"U_COB_CORDOC\") like UPPER('%" + fil.U_COB_CORDOC + "')"; }
                if (fil.DocTotal > 0.00M) { filtros += " and \"DocTotal\" like '%" + fil.DocTotal + "%'"; }
                if (fil.U_SYP_STATUS != null) { filtros += " and UPPER(\"U_SYP_STATUS\")=UPPER('" + fil.U_SYP_STATUS + "')"; }
                if (fil.U_COB_LUGAREN != null) { filtros += " and \"U_COB_LUGAREN\"='" + fil.U_COB_LUGAREN + "'"; }
            }

            string query = "select top 50 \"DocEntry\",\"DocNum\",\"CANCELED\",\"DocDate\",\"CardName\",\"NumAtCard\",\"DocTotal\"" +
                ",\"U_SYP_STATUS\",\"U_COB_LUGAREN\",\"U_COB_TIPODOC\",\"U_COB_SERIE\",\"U_COB_CORDOC\", \"U_BPP_FECINITRA\"  from " + uti.schemaHana + "OINV " +
                "where \"DocEntry\">0 and \"Series\" in(select \"Series\" from " + uti.schemaHana + "nnm1 where \"SeriesName\" like 'BV%')" +
                filtros + " order by 1 desc";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    OINV_E f = new OINV_E();
                    f.DocEntry = hdr.GetInt32(0);
                    if (!hdr.IsDBNull(1)) { f.DocNum = hdr.GetInt32(1); }
                    if (!hdr.IsDBNull(2)) { f.CANCELED = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { f.DocDate = hdr.GetDateTime(3).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(4)) { f.CardName = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { f.NumAtCard = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { f.DocTotal = hdr.GetDecimal(6); }
                    if (!hdr.IsDBNull(7)) { f.U_SYP_STATUS = hdr.GetString(7); }
                    if (!hdr.IsDBNull(8)) { f.U_COB_LUGAREN = hdr.GetString(8); }
                    if (!hdr.IsDBNull(9)) { f.U_COB_TIPODOC = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { f.U_COB_SERIE = hdr.GetString(10); }
                    if (!hdr.IsDBNull(11)) { f.U_COB_CORDOC = hdr.GetString(11); }
                    if (!hdr.IsDBNull(12))
                    {
                        f.U_BPP_FECINITRA = hdr.GetDateTime(12).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        f.U_BPP_FECINITRA = f.DocDate;
                    }
                    lista.Add(f);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public List<OINV_E> listadoComprobantesPorOrdr(int DocEntryOrden)
        {
            List<OINV_E> lista = new List<OINV_E>();
            string query1 = "SELECT T0.\"DocEntry\",T0.\"DocNum\",T0.\"NumAtCard\" FROM " + uti.schemaHana + "OINV T0 " +
                " INNER JOIN " + uti.schemaHana + "INV1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\"" +
                " INNER JOIN " + uti.schemaHana + "RDR1 T2 ON T2.\"DocEntry\" = T1.\"BaseEntry\" AND T2.\"ObjType\" = T1.\"BaseType\"" +
                                                    " AND T2.\"LineNum\" = T1.\"BaseLine\" AND T2.\"DocEntry\" =" + DocEntryOrden +
                " WHERE T0.\"CANCELED\" = 'N' GROUP BY T0.\"DocEntry\",T0.\"DocNum\",T0.\"NumAtCard\"";
            string query2 = "SELECT T4.\"DocEntry\",T4.\"DocNum\",T4.\"NumAtCard\" FROM " + uti.schemaHana + "ODLN T0 " +
                " INNER JOIN " + uti.schemaHana + "DLN1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\"" +
                " INNER JOIN " + uti.schemaHana + "RDR1 T2 ON T2.\"DocEntry\" = T1.\"BaseEntry\" AND T2.\"ObjType\" = T1.\"BaseType\"" +
                                                    " AND T2.\"LineNum\" = T1.\"BaseLine\" AND T2.\"DocEntry\" = " + DocEntryOrden +
                " INNER JOIN " + uti.schemaHana + "INV1 T3 ON T3.\"BaseEntry\" = T1.\"DocEntry\" AND T3.\"BaseType\" = T1.\"ObjType\"" +
                                                    " AND T3.\"BaseLine\" = T1.\"LineNum\" " +
                " INNER JOIN " + uti.schemaHana + "OINV T4 ON T4.\"DocEntry\" = T3.\"DocEntry\" AND T4.\"CANCELED\" = 'N' " +
                " WHERE T0.\"CANCELED\" = 'N' GROUP BY T4.\"DocEntry\",T4.\"DocNum\",T4.\"NumAtCard\"";

            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query1);
                while (hdr.Read())
                {
                    OINV_E o = new OINV_E();
                    if (!hdr.IsDBNull(0)) { o.DocEntry = hdr.GetInt32(0); }
                    if (!hdr.IsDBNull(1)) { o.DocNum = hdr.GetInt32(1); }
                    if (!hdr.IsDBNull(2)) { o.NumAtCard = hdr.GetString(2); }
                    lista.Add(o);
                }
                hdr.Close();
                HanaDataReader hdr2 = db.HanaExecuteReaderNoSp(query2);
                while (hdr2.Read())
                {
                    OINV_E o = new OINV_E();
                    if (!hdr2.IsDBNull(0)) { o.DocEntry = hdr2.GetInt32(0); }
                    if (!hdr2.IsDBNull(1)) { o.DocNum = hdr2.GetInt32(1); }
                    if (!hdr2.IsDBNull(2)) { o.NumAtCard = hdr2.GetString(2); }
                    lista.Add(o);
                }
                hdr2.Close();
            }
            catch { }
            return lista;
        }
        public List<OINV_E> listadoComprobantesPorOrdrArticulo(int DocEntryOrden, string ItemCode)
        {
            List<OINV_E> lista = new List<OINV_E>();
            string query1 = "SELECT T0.\"DocEntry\",T0.\"DocNum\",T0.\"NumAtCard\" FROM " + uti.schemaHana + "OINV T0 " +
                " INNER JOIN " + uti.schemaHana + "INV1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\"" +
                " INNER JOIN " + uti.schemaHana + "RDR1 T2 ON T2.\"DocEntry\" = T1.\"BaseEntry\" AND T2.\"ObjType\" = T1.\"BaseType\"" +
                                                    " AND T2.\"LineNum\" = T1.\"BaseLine\" AND T2.\"DocEntry\" =" + DocEntryOrden +
                                                    " AND T2.\"ItemCode\"=T1.\"ItemCode\" AND T2.\"ItemCode\"='" + ItemCode + "'" +
                " WHERE T0.\"CANCELED\" = 'N' GROUP BY T0.\"DocEntry\",T0.\"DocNum\",T0.\"NumAtCard\"";
            string query2 = "SELECT T4.\"DocEntry\",T4.\"DocNum\",T4.\"NumAtCard\" FROM " + uti.schemaHana + "ODLN T0 " +
                " INNER JOIN " + uti.schemaHana + "DLN1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\"" +
                " INNER JOIN " + uti.schemaHana + "RDR1 T2 ON T2.\"DocEntry\" = T1.\"BaseEntry\" AND T2.\"ObjType\" = T1.\"BaseType\"" +
                                                    " AND T2.\"LineNum\" = T1.\"BaseLine\" AND T2.\"DocEntry\" = " + DocEntryOrden +
                                                    " AND T2.\"ItemCode\"=T1.\"ItemCode\" AND T2.\"ItemCode\"='" + ItemCode + "'" +
                " INNER JOIN " + uti.schemaHana + "INV1 T3 ON T3.\"BaseEntry\" = T1.\"DocEntry\" AND T3.\"BaseType\" = T1.\"ObjType\"" +
                                                    " AND T3.\"BaseLine\" = T1.\"LineNum\" AND T3.\"ItemCode\" = T1.\"ItemCode\"" +
                " INNER JOIN " + uti.schemaHana + "OINV T4 ON T4.\"DocEntry\" = T3.\"DocEntry\" AND T4.\"CANCELED\" = 'N' " +
                " WHERE T0.\"CANCELED\" = 'N' GROUP BY T4.\"DocEntry\",T4.\"DocNum\",T4.\"NumAtCard\"";

            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query1);
                while (hdr.Read())
                {
                    OINV_E o = new OINV_E();
                    if (!hdr.IsDBNull(0)) { o.DocEntry = hdr.GetInt32(0); }
                    if (!hdr.IsDBNull(1)) { o.DocNum = hdr.GetInt32(1); }
                    if (!hdr.IsDBNull(2)) { o.NumAtCard = hdr.GetString(2); }
                    lista.Add(o);
                }
                hdr.Close();
                HanaDataReader hdr2 = db.HanaExecuteReaderNoSp(query2);
                while (hdr2.Read())
                {
                    OINV_E o = new OINV_E();
                    if (!hdr2.IsDBNull(0)) { o.DocEntry = hdr2.GetInt32(0); }
                    if (!hdr2.IsDBNull(1)) { o.DocNum = hdr2.GetInt32(1); }
                    if (!hdr2.IsDBNull(2)) { o.NumAtCard = hdr2.GetString(2); }
                    lista.Add(o);
                }
                hdr2.Close();
            }
            catch { }
            return lista;
        }
        public string buscarGuiasTrasladoSinEnt(int DocEntryVenta)
        {
            string guias = "";
            string query = "select IFNULL(t0.\"U_COB_TIPODOC\"||'-','')||IFNULL(t0.\"U_COB_SERIE\" || '-','')||IFNULL(t0.\"U_COB_CORDOC\",'') " +
                            " from " + uti.schemaHana + "oinv t0 " +
                            " inner join " + uti.schemaHana + "inv1 t1 on t1.\"DocEntry\"=t0.\"DocEntry\"" +
                            " inner join " + uti.schemaHana + "rdr1 t2 on t2.\"DocEntry\"=t1.\"BaseEntry\" and t2.\"ObjType\"=t1.\"BaseType\" " +
                                                    " and t2.\"ItemCode\"=t1.\"ItemCode\" where t0.\"CANCELED\"='N' and t2.\"DocEntry\"=" + DocEntryVenta
                                                    + " group by IFNULL(t0.\"U_COB_TIPODOC\"||'-','')||IFNULL(t0.\"U_COB_SERIE\" || '-','')||IFNULL(t0.\"U_COB_CORDOC\",'')";
            HanaConnection cn = new HanaConnection(uti.cadHana);
            try
            {
                cn.Open();
                HanaCommand cmd = new HanaCommand(query, cn);
                cmd.CommandType = System.Data.CommandType.Text;
                HanaDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0)) { guias += dr.GetString(0) + ","; }
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return guias;
        }
        public string buscarGuiasTrasladoConEnt(int DocEntryEnt)
        {
            string guias = "";
            string query = "select IFNULL(t0.\"U_COB_TIPODOC\"||'-','')||IFNULL(t0.\"U_COB_SERIE\" || '-','')||IFNULL(t0.\"U_COB_CORDOC\",'') " +
                            " from " + uti.schemaHana + "oinv t0 " +
                            " inner join " + uti.schemaHana + "inv1 t1 on t1.\"DocEntry\"=t0.\"DocEntry\"" +
                            " inner join " + uti.schemaHana + "dln1 t2 on t2.\"DocEntry\"=t1.\"BaseEntry\" and t2.\"ObjType\"=t1.\"BaseType\" " +
                                                    " and t2.\"ItemCode\"=t1.\"ItemCode\" where t0.\"CANCELED\"='N' and t2.\"DocEntry\"=" + DocEntryEnt
                                                    + " group by IFNULL(t0.\"U_COB_TIPODOC\"||'-','')||IFNULL(t0.\"U_COB_SERIE\" || '-','')||IFNULL(t0.\"U_COB_CORDOC\",'')";
            HanaConnection cn = new HanaConnection(uti.cadHana);
            try
            {
                cn.Open();
                HanaCommand cmd = new HanaCommand(query, cn);
                cmd.CommandType = System.Data.CommandType.Text;
                HanaDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0)) { guias += dr.GetString(0) + ","; }
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return guias;
        }
        public string CalcularPdfsActaDespachoOINV(string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN, string TipoComprobante)
        {
            string doc = string.Empty;
            string filtros = string.Empty;

            if (!string.IsNullOrEmpty(Fecha))
            {
                filtros += " and (\"DocDate\"='" + Fecha + "' or \"U_BPP_FECINITRA\"='" + Fecha + "')";
            }
            if (!string.IsNullOrEmpty(U_SYP_STATUS))
            {
                if (U_SYP_STATUS == "V") { filtros += " and \"CANCELED\"='N'"; }
                filtros += " and \"U_SYP_STATUS\"='" + U_SYP_STATUS + "'";
            }
            if (!string.IsNullOrEmpty(U_COB_LUGAREN))
            {
                filtros += " and \"U_COB_LUGAREN\"='" + U_COB_LUGAREN + "'";
            }
            /*
            if(TieneGuia!=null && TieneGuia!="")
            {
                if (TieneGuia.Equals("Si")) { filtros += " and \"U_COB_CORDOC\" is not null"; }
                if (TieneGuia.Equals("No")) { filtros += " and \"U_COB_CORDOC\" is null"; }
            }*/
            if (!string.IsNullOrEmpty(TipoComprobante))
            {
                filtros += " and \"Series\" in(select \"Series\" from " + uti.schemaHana + "nnm1 where \"SeriesName\" like '" + TipoComprobante + "V%')";
            }
            string query = "select \"DocDate\", \"U_BPP_FECINITRA\"  from " + uti.schemaHana + " OINV where \"DocEntry\">0 " + filtros;
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                List<OINV_E> lis = new List<OINV_E>();
                while (hdr.Read())
                {
                    OINV_E o = new OINV_E();
                    o.DocDate = hdr.GetDateTime(0).ToString("yyyy-MM-dd");
                    if (!hdr.IsDBNull(1)) { o.U_BPP_FECINITRA = hdr.GetDateTime(1).ToString("yyyy-MM-dd"); }
                    lis.Add(o);
                }
                int total = 0;
                if (lis.Count > 0)
                {
                    foreach (var item in lis)
                    { if (string.IsNullOrEmpty(item.U_BPP_FECINITRA)) { item.U_BPP_FECINITRA = Fecha; } }
                }
                total = lis.Where(x => x.U_BPP_FECINITRA == Fecha).Count();
                doc = "Son " + total + " Documentos";
                hdr.Close();
            }
            catch { }
            return doc;
        }
        public string CalcularPdfsDescarga(string DocDate, string U_SYP_STATUS, string U_COB_LUGAREN, string TieneGuia, string TipoComprobante)
        {
            string doc = "";
            string filtros = "";
            if (DocDate != null && DocDate != "")
            {
                filtros += " and \"DocDate\"='" + DocDate + "'";
            }
            if (U_SYP_STATUS != null && U_SYP_STATUS != "")
            {
                filtros += " and \"U_SYP_STATUS\"='" + U_SYP_STATUS + "'";
            }
            if (U_COB_LUGAREN != null && U_COB_LUGAREN != "")
            {
                filtros += " and \"U_COB_LUGAREN\"='" + U_COB_LUGAREN + "'";
            }
            if (TieneGuia != null && TieneGuia != "")
            {
                if (TieneGuia.Equals("Si")) { filtros += " and \"U_COB_CORDOC\" is not null"; }
                if (TieneGuia.Equals("No")) { filtros += " and \"U_COB_CORDOC\" is null"; }
            }
            if (TipoComprobante != null && TipoComprobante != "")
            {
                filtros += " and \"Series\" in(select \"Series\" from " + uti.schemaHana + "nnm1 where \"SeriesName\" like '" + TipoComprobante + "V%')";
            }
            string query = "select count(*) from " + uti.schemaHana + "oinv where \"DocEntry\">0 " + filtros;
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                hdr.Read();
                doc = "Son " + hdr.GetInt32(0) + " Documentos";
                hdr.Close();
            }
            catch { }
            return doc;
        }
        public List<ComprobanteDePago_E> buscarFacturaBoletaSap(string NumAtCard)
        {
            List<ComprobanteDePago_E> lista = new List<ComprobanteDePago_E>();
            int DocEntry = 0;
            //busca DocEntry de NumAtCard en OINV
            string queryDE = $"SELECT  \"DocEntry\" FROM {uti.schemaHana}OINV  WHERE \"U_SYP_MDTD\" || '-' ||\"U_SYP_MDSD\" || '-' || \"U_SYP_MDCD\" = '{NumAtCard}'";
            HanaConnection cn = new HanaConnection(uti.cadHana);
            try
            {
                cn.Open();
                HanaCommand cmd = new HanaCommand(queryDE, cn);
                cmd.CommandType = System.Data.CommandType.Text;
                HanaDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0)) { DocEntry = dr.GetInt32(0); }
                }
                dr.Close(); cn.Close();
            }
            catch { return lista; }

            if (DocEntry > 0)
            {
                string query = $"CALL {uti.schemaHana} DIEGO_LYT_FV({DocEntry})";
                try
                {
                    HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                    if (hdr.HasRows)
                    {
                        while (hdr.Read())
                        {
                            ComprobanteDePago_E c = new ComprobanteDePago_E();
                            if (!hdr.IsDBNull(0)) { c.DocEntry = hdr.GetInt32(0); }
                            if (!hdr.IsDBNull(1)) { c.DocNum = hdr.GetInt32(1); }
                            if (!hdr.IsDBNull(2)) { c.ElaboradoPor = hdr.GetString(2); }
                            if (!hdr.IsDBNull(3)) { c.TipoDoc = hdr.GetString(3); }
                            if (!hdr.IsDBNull(4)) { c.SerieDoc = hdr.GetString(4); }
                            if (!hdr.IsDBNull(5)) { c.CorreDoc = hdr.GetString(5); }
                            if (!hdr.IsDBNull(6)) { c.NumGuias = hdr.GetString(6); }
                            if (!hdr.IsDBNull(7)) { c.NombreSocio = hdr.GetString(7); }
                            if (!hdr.IsDBNull(8)) { c.DirPagar = hdr.GetString(8); }
                            if (!hdr.IsDBNull(9)) { c.Ruc = hdr.GetString(9); }
                            if (!hdr.IsDBNull(10)) { c.Fecha = hdr.GetDateTime(10).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(11)) { c.FechaVencimiento = hdr.GetDateTime(11).ToLongDateString(); }
                            if (!hdr.IsDBNull(12)) { c.MonedaLetras = hdr.GetString(12); }
                            if (!hdr.IsDBNull(13)) { c.ItemCode = hdr.GetString(13); }
                            if (!hdr.IsDBNull(14)) { c.Descripcion = hdr.GetString(14); }
                            if (!hdr.IsDBNull(15)) { c.DocNumTicket = hdr.GetString(15); }
                            if (!hdr.IsDBNull(16)) { c.Um = hdr.GetString(16); }
                            if (!hdr.IsDBNull(17)) { c.Cantidad = Math.Round(hdr.GetDecimal(17), 0); }
                            if (!hdr.IsDBNull(18)) { c.PreUnitSinIgv = hdr.GetDecimal(18); }
                            if (!hdr.IsDBNull(19)) { c.Descuento = hdr.GetDecimal(19); }
                            if (!hdr.IsDBNull(20)) { c.PreVentaNeto = hdr.GetDecimal(20); }
                            if (!hdr.IsDBNull(21)) { c.PrecioVenta = hdr.GetDecimal(21); }
                            if (!hdr.IsDBNull(22)) { c.ItemPrecio = hdr.GetDecimal(22); }
                            if (!hdr.IsDBNull(23)) { c.ItemTotal = hdr.GetDecimal(23); }
                            if (!hdr.IsDBNull(24)) { c.FechaEntrega = hdr.GetDateTime(24).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(25)) { c.Impuesto = hdr.GetDecimal(25); }
                            if (!hdr.IsDBNull(26)) { c.DocTotal = hdr.GetDecimal(26); }
                            if (!hdr.IsDBNull(27)) { c.PorcenImpto = hdr.GetDecimal(27); }
                            if (!hdr.IsDBNull(28)) { c.LoteNum = hdr.GetString(28); }
                            if (!hdr.IsDBNull(29)) { c.CantidadL = hdr.GetDecimal(29); }
                            if (!hdr.IsDBNull(30)) { c.TieneAnticipo = hdr.GetInt32(30); }
                            if (!hdr.IsDBNull(31)) { c.Laboratorio = hdr.GetString(31); }
                            if (!hdr.IsDBNull(32)) { c.VctoLote = hdr.GetDateTime(32).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(33)) { c.QUMVta = Math.Round(hdr.GetDecimal(33), 0); }
                            if (!hdr.IsDBNull(34)) { c.CondPago = hdr.GetString(34); }
                            if (!hdr.IsDBNull(35)) { c.NroOrdVenta = hdr.GetInt32(35); }
                            if (!hdr.IsDBNull(36)) { c.CodImpuesto = hdr.GetString(36); }
                            if (!hdr.IsDBNull(37)) { c.Almacen = hdr.GetString(37); }
                            if (!hdr.IsDBNull(38)) { c.PtoPartida = hdr.GetString(38); }
                            if (!hdr.IsDBNull(39)) { c.DirEnvio = hdr.GetString(39); }
                            lista.Add(c);
                        }
                    }
                    hdr.Close();
                }
                catch (Exception e) { throw new Exception(e.Message); }
            }
            return lista;
        }
        public List<NotaCreditoDebito_E> buscarNotaDebitoSap(string NumAtCard)
        {
            List<NotaCreditoDebito_E> lista = new List<NotaCreditoDebito_E>();
            int DocEntry = 0;
            string queryDE = $"SELECT  \"DocEntry\" FROM {uti.schemaHana}OINV  WHERE \"U_SYP_MDTD\" || '-' ||\"U_SYP_MDSD\" || '-' || \"U_SYP_MDCD\" = '{NumAtCard}'";
            HanaConnection cn = new HanaConnection(uti.cadHana);
            try
            {
                cn.Open();
                HanaCommand cmd = new HanaCommand(queryDE, cn);
                cmd.CommandType = System.Data.CommandType.Text;
                HanaDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0)) { DocEntry = dr.GetInt32(0); }
                }
                dr.Close(); cn.Close();
            }
            catch { return lista; }

            if (DocEntry > 0)
            {
                string query = $"CALL {uti.schemaHana}SYP_LYT_ND_BRISMAR_C({DocEntry})";
                try
                {
                    HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                    if (hdr.HasRows)
                    {
                        while (hdr.Read())
                        {
                            NotaCreditoDebito_E c = new NotaCreditoDebito_E();
                            if (!hdr.IsDBNull(0)) { c.DocEntry = hdr.GetInt32(0); }
                            if (!hdr.IsDBNull(4)) { c.ElaboradoPor = hdr.GetString(4); }
                            if (!hdr.IsDBNull(5)) { c.NombreBD = hdr.GetString(5); }
                            if (!hdr.IsDBNull(6)) { c.DireccionBD = hdr.GetString(6); }
                            if (!hdr.IsDBNull(7)) { c.RucBD = hdr.GetString(7); }
                            if (!hdr.IsDBNull(9)) { c.TipoDoc = hdr.GetString(9); }
                            if (!hdr.IsDBNull(10)) { c.SerieDoc = hdr.GetString(10); }
                            if (!hdr.IsDBNull(11)) { c.CorreDoc = hdr.GetString(11); }
                            if (!hdr.IsDBNull(12)) { c.TipoDocRel = hdr.GetString(12); }
                            if (!hdr.IsDBNull(13)) { c.SerieDocRel = hdr.GetString(13); }
                            if (!hdr.IsDBNull(14)) { c.CorreDocRel = hdr.GetString(14); }
                            if (!hdr.IsDBNull(15)) { c.DocDateRel = hdr.GetDateTime(15).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(16)) { c.Motivo = hdr.GetString(16); }
                            if (!hdr.IsDBNull(18)) { c.DocNum = hdr.GetInt32(18); }
                            if (!hdr.IsDBNull(19)) { c.SerieSap = hdr.GetInt32(19); }
                            if (!hdr.IsDBNull(20)) { c.NombreSocio = hdr.GetString(20); }
                            if (!hdr.IsDBNull(22)) { c.DirDestino = hdr.GetString(22); }
                            if (!hdr.IsDBNull(23)) { c.DirPagar = hdr.GetString(23); }
                            if (!hdr.IsDBNull(24)) { c.DistritoCli = hdr.GetString(24); }
                            if (!hdr.IsDBNull(25)) { c.Ruc = hdr.GetString(25); }
                            if (!hdr.IsDBNull(27)) { c.Email = hdr.GetString(27); }
                            if (!hdr.IsDBNull(28)) { c.DocDate = Convert.ToDateTime(hdr.GetString(28)).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(29)) { c.Moneda = hdr.GetString(29); }
                            if (!hdr.IsDBNull(30)) { c.MonedaLetras = hdr.GetString(30); }
                            if (!hdr.IsDBNull(31)) { c.Telefonos = hdr.GetString(31); }
                            if (!hdr.IsDBNull(43)) { c.DescripcionLinea = hdr.GetString(43); }
                            if (!hdr.IsDBNull(44)) { c.Um = hdr.GetString(44); }
                            if (!hdr.IsDBNull(45)) { c.Cantidad = Math.Round(hdr.GetDecimal(45), 2); }
                            if (!hdr.IsDBNull(49)) { c.PrecioLinea = Math.Round(hdr.GetDecimal(49), 2); }
                            if (!hdr.IsDBNull(61)) { c.FechaEntrega = Convert.ToDateTime(hdr.GetString(61)).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(62)) { c.LugarEntrega = hdr.GetString(62); }
                            if (!hdr.IsDBNull(63)) { c.CondicionPago = hdr.GetString(63); }
                            if (!hdr.IsDBNull(65)) { c.SubTotal = Math.Round(hdr.GetDecimal(65), 2); }
                            if (!hdr.IsDBNull(67)) { c.Impuesto = Math.Round(hdr.GetDecimal(67), 2); }
                            if (!hdr.IsDBNull(68)) { c.DocTotal = Math.Round(hdr.GetDecimal(68), 2); }
                            if (!hdr.IsDBNull(72)) { c.ImpuestoPorcentaje = Math.Round(hdr.GetDecimal(72), 0); }
                            if (!hdr.IsDBNull(80)) { c.CodigoGasto = hdr.GetString(80); }
                            if (!hdr.IsDBNull(81)) { c.TipoDocumentoSAP = hdr.GetString(81); }

                            lista.Add(c);
                        }
                    }
                    hdr.Close();
                }
                catch (Exception e) { throw new Exception(e.Message); }
            }
            return lista;
        }
        public TEMP_RRU01_E obtenerGuiaRemision(string NumAtCard)
        {
            TEMP_RRU01_E obj = new TEMP_RRU01_E();
            string query = $"SELECT 'OINV', \"U_COB_TIPODOC\", \"U_COB_SERIE\", \"U_COB_CORDOC\", TO_CHAR(\"DocDate\", 'YYYY-MM-DD'), TO_CHAR(\"U_BPP_FECINITRA\", 'YYYY-MM-DD'), 'G' FROM {uti.schemaHana}OINV  WHERE \"U_COB_TIPODOC\"||'-'||\"U_COB_SERIE\" ||'-'||\"U_COB_CORDOC\" = '{NumAtCard}'";

            HanaConnection cn = new HanaConnection(uti.cadHana);
            try
            {
                cn.Open();
                HanaCommand cmd = new HanaCommand(query, cn);
                cmd.CommandType = System.Data.CommandType.Text;
                HanaDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0)) { obj.TablaSAP = dr.GetString(0); }
                    if (!dr.IsDBNull(1)) { obj.U_SYP_MDTD = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { obj.U_SYP_MDSD = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { obj.U_SYP_MDCD = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { obj.DocDate = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { obj.U_BPP_FECINITRA = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { obj.Identificador = dr.GetString(6); }
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return obj;
        }
        public List<TEMP_RRU01_E> FactBoletaSap(int DocEntryOrden)
        {
            List<TEMP_RRU01_E> lista = new List<TEMP_RRU01_E>();

            string query1 = "SELECT 'OINV',T0.\"U_SYP_MDTD\",T0.\"U_SYP_MDSD\",T0.\"U_SYP_MDCD\",to_char(T0.\"DocDate\",'YYYY-MM-DD'),to_char(T0.\"U_BPP_FECINITRA\",'YYYY-MM-DD')  FROM " + uti.schemaHana + "OINV T0 " +
                " INNER JOIN " + uti.schemaHana + "INV1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\"" +
                " INNER JOIN " + uti.schemaHana + "RDR1 T2 ON T2.\"DocEntry\" = T1.\"BaseEntry\" AND T2.\"ObjType\" = T1.\"BaseType\"" +
                                                    " AND T2.\"LineNum\" = T1.\"BaseLine\" AND T2.\"DocEntry\" =" + DocEntryOrden +
                " WHERE T0.\"CANCELED\" = 'N' GROUP BY T0.\"U_SYP_MDTD\",T0.\"U_SYP_MDSD\",T0.\"U_SYP_MDCD\",to_char(T0.\"DocDate\",'YYYY-MM-DD'),to_char(T0.\"U_BPP_FECINITRA\",'YYYY-MM-DD') ";

            string query2 = " SELECT 'OINV',T4.\"U_SYP_MDTD\",T4.\"U_SYP_MDSD\",T4.\"U_SYP_MDCD\",to_char(T4.\"DocDate\",'YYYY-MM-DD'),to_char(T4.\"U_BPP_FECINITRA\",'YYYY-MM-DD')  FROM " + uti.schemaHana + "ODLN T0 " +
                " INNER JOIN " + uti.schemaHana + "DLN1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\"" +
                " INNER JOIN " + uti.schemaHana + "RDR1 T2 ON T2.\"DocEntry\" = T1.\"BaseEntry\" AND T2.\"ObjType\" = T1.\"BaseType\"" +
                                                    " AND T2.\"LineNum\" = T1.\"BaseLine\" AND T2.\"DocEntry\" = " + DocEntryOrden +
                " INNER JOIN " + uti.schemaHana + "INV1 T3 ON T3.\"BaseEntry\" = T1.\"DocEntry\" AND T3.\"BaseType\" = T1.\"ObjType\"" +
                                                    " AND T3.\"BaseLine\" = T1.\"LineNum\" " +
                " INNER JOIN " + uti.schemaHana + "OINV T4 ON T4.\"DocEntry\" = T3.\"DocEntry\" AND T4.\"CANCELED\" = 'N' " +
                " WHERE T0.\"CANCELED\" = 'N' GROUP BY  T4.\"U_SYP_MDTD\",T4.\"U_SYP_MDSD\",T4.\"U_SYP_MDCD\",to_char(T4.\"DocDate\",'YYYY-MM-DD'),to_char(T4.\"U_BPP_FECINITRA\",'YYYY-MM-DD')";


            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query1);

                while (hdr.Read())
                {
                    TEMP_RRU01_E obj = new TEMP_RRU01_E();
                    if (!hdr.IsDBNull(0)) { obj.TablaSAP = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { obj.U_SYP_MDTD = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2))
                    {
                        obj.U_SYP_MDSD = hdr.GetString(2);
                        if (obj.U_SYP_MDSD.Contains("F"))
                        {
                            obj.Identificador = "F";
                        }
                        else if (obj.U_SYP_MDSD.Contains("B"))
                        {
                            obj.Identificador = "B";
                        }
                        if (!hdr.IsDBNull(3)) { obj.U_SYP_MDCD = hdr.GetString(3); }
                        if (!hdr.IsDBNull(4)) { obj.DocDate = hdr.GetString(4); }
                        if (!hdr.IsDBNull(5)) { obj.U_BPP_FECINITRA = hdr.GetString(5); }

                    }
                    lista.Add(obj);
                }
                hdr.Close();
                HanaDataReader hdr2 = db.HanaExecuteReaderNoSp(query2);
                while (hdr2.Read())
                {
                    TEMP_RRU01_E obj = new TEMP_RRU01_E();
                    if (!hdr2.IsDBNull(0)) { obj.TablaSAP = hdr2.GetString(0); }
                    if (!hdr2.IsDBNull(1)) { obj.U_SYP_MDTD = hdr2.GetString(1); }
                    if (!hdr2.IsDBNull(2))
                    {
                        obj.U_SYP_MDSD = hdr2.GetString(2);
                        if (obj.U_SYP_MDSD.Contains("F"))
                        {
                            obj.Identificador = "F";
                        }
                        else if (obj.U_SYP_MDSD.Contains("B"))
                        {
                            obj.Identificador = "B";
                        }
                        if (!hdr2.IsDBNull(3)) { obj.U_SYP_MDCD = hdr2.GetString(3); }
                        if (!hdr2.IsDBNull(4)) { obj.DocDate = hdr2.GetString(4); }
                        if (!hdr2.IsDBNull(5)) { obj.U_BPP_FECINITRA = hdr2.GetString(5); }

                    }
                    lista.Add(obj);
                }
                hdr2.Close();

            }
            catch { }
            return lista;
        }
        public List<TEMP_RRU01_E> NotasDebitoSap(string FBConcatenadas)
        {
            List<TEMP_RRU01_E> lista = new List<TEMP_RRU01_E>();
            string query = $"SELECT 'OINV', T0.\"U_SYP_MDTD\", T0.\"U_SYP_MDSD\", T0.\"U_SYP_MDCD\", to_char(T0.\"DocDate\", 'YYYY-MM-DD'), to_char(T0.\"U_BPP_FECINITRA\", 'YYYY-MM-DD'), 'ND' FROM {uti.schemaHana}OINV T0 WHERE T0.\"U_SYP_MDTD\" = '08' AND (T0.\"U_SYP_MDTO\" || '-' || T0.\"U_SYP_MDSO\" || '-' || T0.\"U_SYP_MDCO\") IN ('{FBConcatenadas}') ";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);

                while (hdr.Read())
                {
                    TEMP_RRU01_E obj = new TEMP_RRU01_E();
                    if (!hdr.IsDBNull(0)) { obj.TablaSAP = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { obj.U_SYP_MDTD = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { obj.U_SYP_MDSD = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { obj.U_SYP_MDCD = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { obj.DocDate = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { obj.U_BPP_FECINITRA = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { obj.Identificador = hdr.GetString(6); }
                    lista.Add(obj);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
    }
}
