using Capa_Entidad.Ventas_ENT.Tablas;
using Sap.Data.Hana;
using System.Collections.Generic;

namespace Capa_Datos.Ventas_DAO.Tablas
{
    public class ORDR_D
    {
        DBHelper db = new DBHelper(); Utilitarios uti = new Utilitarios();
        OINV_D oinvD = new OINV_D();
        ODLN_D odlnD = new ODLN_D();
        public List<ORDR_E> listadoOrdenesDeVenta(ORDR_E fo)
        {
            List<ORDR_E> lista = new List<ORDR_E>();
            string filtros = "";
            if (fo != null)
            {
                if (fo.DocNum > 0) { filtros += " and \"DocNum\" =" + fo.DocNum; }
                if (fo.DocDate != null) { filtros += " and \"DocDate\"='" + fo.DocDate + "'"; }
                if (fo.CardName != null) { filtros += " and UPPER(\"CardName\") like UPPER('%" + fo.CardName + "%')"; }
                if (fo.CardCode != null) { filtros += " and UPPER(\"CardCode\") like UPPER('%" + fo.CardCode + "')"; }
                if (fo.SlpCode > 0) { filtros += " and \"SlpCode\"=" + fo.SlpCode; }
                if (fo.DocTotal > 0.00M) { filtros += " and \"DocTotal\" like '%" + fo.DocTotal + "%'"; }
                if (fo.Comments != null) { filtros += " and UPPER(\"Comments\") like UPPER('%" + fo.Comments + "%')"; }
                if (fo.U_SYP_STATUS != null) { filtros += " and UPPER(\"U_SYP_STATUS\")=UPPER('" + fo.U_SYP_STATUS + "')"; }
                if (fo.U_COB_LUGAREN != null) { filtros += " and \"U_COB_LUGAREN\"='" + fo.U_COB_LUGAREN + "'"; }
            }
            string query = "select top 50 \"DocEntry\",\"DocNum\",\"CANCELED\",\"DocDate\",\"CardCode\",\"CardName\",\"DocTotal\",\"SlpCode\",\"Comments\",\"U_SYP_STATUS\",\"U_COB_LUGAREN\"   from " + uti.schemaHana + "ordr where \"DocEntry\">0 " + filtros + " order by 1 desc";

            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    ORDR_E o = new ORDR_E();
                    o.DocEntry = hdr.GetInt32(0);
                    if (!hdr.IsDBNull(1)) { o.DocNum = hdr.GetInt32(1); }
                    if (!hdr.IsDBNull(2)) { o.CANCELED = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { o.DocDate = hdr.GetDateTime(3).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(4)) { o.CardCode = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { o.CardName = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { o.DocTotal = hdr.GetDecimal(6); }
                    if (!hdr.IsDBNull(7)) { o.SlpCode = hdr.GetInt32(7); }
                    if (!hdr.IsDBNull(8)) { o.Comments = hdr.GetString(8); }
                    if (!hdr.IsDBNull(9)) { o.U_SYP_STATUS = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { o.U_COB_LUGAREN = hdr.GetString(10); }
                    o.ComprobantesVinculados = oinvD.listadoComprobantesPorOrdr(o.DocEntry);
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public string guiasTraslado(int DocNum)
        {
            string guia = ""; int DocEntry = 0;
            string query = "select \"DocEntry\" from " + uti.schemaHana + "ordr where \"DocNum\"=" + DocNum + " and \"CANCELED\"='N'";
            HanaConnection cn = new HanaConnection(uti.cadHana);
            try
            {
                cn.Open();
                HanaCommand cmd = new HanaCommand(query, cn);
                cmd.CommandType = System.Data.CommandType.Text;
                HanaDataReader dr = cmd.ExecuteReader();
                dr.Read();
                if (!dr.IsDBNull(0)) { DocEntry = dr.GetInt32(0); }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); return guia; }

            string guiaRem = odlnD.buscarGuiasRemision(DocEntry);
            string guiasTras = oinvD.buscarGuiasTrasladoSinEnt(DocEntry);
            string guiasTrasEnt = string.Empty;
            foreach (ODLN_E o in odlnD.listarEntregasPorNroVenta(DocEntry))
            {
                guiasTrasEnt += oinvD.buscarGuiasTrasladoConEnt(o.DocEntry);
            }
            if (!string.IsNullOrEmpty(guiaRem)) { guia = guiaRem; }
            else { guia = guiasTras + guiasTrasEnt; }
            return guia;
        }
        public int buscarDocEntry(int DocNum)
        {
            int DocEntry = 0;
            string query = "select \"DocEntry\" from " + uti.schemaHana + "ordr where \"DocNum\"=" + DocNum;
            try
            {
                HanaDataReader dr = db.HanaExecuteReaderNoSp(query);
                dr.Read();
                if (!dr.IsDBNull(0)) { DocEntry = dr.GetInt32(0); }
                dr.Close();
            }
            catch { }
            return DocEntry;
        }
    }
}
