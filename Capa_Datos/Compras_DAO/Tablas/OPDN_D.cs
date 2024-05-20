using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.Compras_ENT.Tablas;
using Sap.Data.Hana;

namespace Capa_Datos.Compras_DAO.Tablas
{
    public class OPDN_D
    {
        DBHelper db = new DBHelper();Utilitarios uti = new Utilitarios();
        SQL_OPDN_D sqlopdnD = new SQL_OPDN_D();
        public List<OPDN_E> listadoEntradaMercancias(OPDN_E fil)
        {
            List<OPDN_E> lista = new List<OPDN_E>();
            string filtros = "";
            if (fil != null)
            {
                if (fil.DocNum > 0) { filtros += " and \"DocNum\" like '%" + fil.DocNum + "'"; }
                if (fil.DocDate != null) { filtros += " and \"DocDate\"='" + fil.DocDate + "'"; }
                if (fil.CardName != null) { filtros += " and UPPER(\"CardName\") like UPPER('%" + fil.CardName + "%')"; }
                if (fil.NumAtCard != null) { filtros += " and UPPER(\"NumAtCard\") like UPPER('%" + fil.NumAtCard + "')"; }
                if (fil.DocTotal > 0.00M) { filtros += " and \"DocTotal\" like '%" + fil.DocTotal + "%'"; }
                if (fil.U_SYP_STATUS != null) { filtros += " and UPPER(\"U_SYP_STATUS\")=UPPER('" + fil.U_SYP_STATUS + "')"; }
                if (fil.Almacen != null) { filtros += " and (select x.\"WhsCode\" from " + uti.schemaHana + "pdn1 x where x.\"DocEntry\" = t0.\"DocEntry\" and x.\"LineNum\"=(select min(x2.\"LineNum\") from " + uti.schemaHana + "pdn1 x2 where x2.\"DocEntry\" = x.\"DocEntry\" )) = '" + fil.Almacen + "'"; }
                if (fil.sqlopdn != null)
                {
                    if (fil.sqlopdn.Estado != null && fil.sqlopdn.Estado == "REALIZADO") { filtros += " and \"DocEntry\" in (" + SQL_OPDN_E.SqlTextIn(sqlopdnD.listarSqlEntradaDeMercancias(fil.sqlopdn)) + ")"; }
                    if (fil.sqlopdn.Estado != null && fil.sqlopdn.Estado == "PENDIENTE") { filtros += $" and \"DocEntry\" not in ("+SQL_OPDN_E.SqlTextIn(sqlopdnD.listarSqlEntradaDeMercancias(fil.sqlopdn)) +")"; }
                }
            }
            string query = "select top 50 t0.\"DocEntry\",t0.\"DocNum\",t0.\"DocDate\",t0.\"CardName\"" +
                ",t0.\"NumAtCard\",t0.\"DocTotal\",t0.\"U_SYP_STATUS\"" +
                ",(select x.\"WhsCode\" from " + uti.schemaHana + "pdn1 x where x.\"DocEntry\" = t0.\"DocEntry\" and x.\"LineNum\" =(select min(x2.\"LineNum\") from " + uti.schemaHana + "pdn1 x2 where x2.\"DocEntry\" = x.\"DocEntry\" ))" +
                " as \"Almacen\" from " + uti.schemaHana + "opdn t0 where t0.\"DocEntry\">0 " + filtros + " order by 1 desc";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while(hdr.Read())
                {
                    OPDN_E o = new OPDN_E();
                    if (!hdr.IsDBNull(0)) { o.DocEntry = hdr.GetInt32(0); }
                    if (!hdr.IsDBNull(1)) { o.DocNum = hdr.GetInt32(1); }
                    if (!hdr.IsDBNull(2)) { o.DocDate = hdr.GetDateTime(2).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(3)) { o.CardName = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { o.NumAtCard = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { o.DocTotal = hdr.GetDecimal(5); }
                    if (!hdr.IsDBNull(6)) { o.U_SYP_STATUS = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { o.Almacen = hdr.GetString(7); }
                    o.sqlopdn = sqlopdnD.buscarSqlEntradaDeMercancias(o.DocEntry);
                    lista.Add(o);
                }
                hdr.Close();
            }catch { }
            return lista;
        }
        public OPDN_E buscarEntradaMercancias(int DocEntry)
        {
            OPDN_E o = new OPDN_E();
            string query = "select t0.\"DocEntry\",t0.\"DocNum\",t0.\"DocDate\",t0.\"CardName\"" +
                ",t0.\"NumAtCard\",t0.\"DocTotal\",t0.\"U_SYP_STATUS\"" +
                ",(select x.\"WhsCode\" from " + uti.schemaHana + "pdn1 x where x.\"DocEntry\" = t0.\"DocEntry\" and x.\"LineNum\"=0 ) as \"Almacen\" " +
                " from "+uti.schemaHana+"opdn t0 where t0.\"DocEntry\"=" + DocEntry;
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                hdr.Read();
                if (!hdr.IsDBNull(0)) { o.DocEntry = hdr.GetInt32(0); }
                if (!hdr.IsDBNull(1)) { o.DocNum = hdr.GetInt32(1); }
                if (!hdr.IsDBNull(2)) { o.DocDate = hdr.GetDateTime(2).ToString("dd/MM/yyyy"); }
                if (!hdr.IsDBNull(3)) { o.CardName = hdr.GetString(3); }
                if (!hdr.IsDBNull(4)) { o.NumAtCard = hdr.GetString(4); }
                if (!hdr.IsDBNull(5)) { o.DocTotal = hdr.GetDecimal(5); }
                if (!hdr.IsDBNull(6)) { o.U_SYP_STATUS = hdr.GetString(6); }
                if (!hdr.IsDBNull(7)) { o.Almacen = hdr.GetString(7); }
                o.sqlopdn = sqlopdnD.buscarSqlEntradaDeMercancias(o.DocEntry);
                hdr.Close();
            }
            catch { }
            return o;
        }
        public List<OPDN_E> entradasVinculadasFacturaProv(int DocEntry)
        {
            List<OPDN_E> lista = new List<OPDN_E>();
            // facturas normales
            string query = "SELECT T3.\"DocEntry\",T3.\"DocNum\" FROM "+uti.schemaHana+"OPCH T0 "+
                            " INNER JOIN " + uti.schemaHana + "PCH1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" " +
                            " INNER JOIN " + uti.schemaHana + "PDN1 T2 ON T2.\"DocEntry\" = T1.\"BaseEntry\" " +
                                                                " and T2.\"ObjType\" = T1.\"BaseType\" and T2.\"LineNum\" = T1.\"BaseLine\" "+
                            " INNER JOIN " + uti.schemaHana + "OPDN T3 ON T3.\"DocEntry\" = T2.\"DocEntry\" "+
                            " WHERE T0.\"DocEntry\" = "+DocEntry+
                            " GROUP BY T3.\"DocEntry\",T3.\"DocNum\"";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while(hdr.Read())
                {
                    OPDN_E o = new OPDN_E();
                    if (!hdr.IsDBNull(0)) { o.DocEntry = hdr.GetInt32(0); }
                    if (!hdr.IsDBNull(1)) { o.DocNum = hdr.GetInt32(1); }
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
    }
}
