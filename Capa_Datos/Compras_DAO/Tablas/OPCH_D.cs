using Capa_Entidad.Compras_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sap.Data.Hana;
using Capa_Entidad.Almacen_ENT.Tablas;

namespace Capa_Datos.Compras_DAO.Tablas
{
    //factura proveedores
    public class OPCH_D
    {
        DBHelper db = new DBHelper();Utilitarios uti = new Utilitarios();
        PCH1_D pch1D = new PCH1_D(); OPDN_D opdnD = new OPDN_D();
        public List<OPCH_E> listadoFacturasProveedores(OPCH_E fil)
        {
            List<OPCH_E> lista = new List<OPCH_E>();
            string filtros = "";
            if (fil != null)
            {
                if (fil.DocNum > 0) { filtros += " and \"DocNum\" like '%" + fil.DocNum + "'"; }
                if (fil.DocDate != null) { filtros += " and \"DocDate\"='" + fil.DocDate + "'"; }
                if (fil.TaxDate != null) { filtros += " and \"TaxDate\"='" + fil.TaxDate + "'"; }
                if (fil.CardName != null) { filtros += " and UPPER(\"CardName\") like UPPER('%" + fil.CardName + "%')"; }
                if (fil.NumAtCard != null) { filtros += " and UPPER(\"NumAtCard\") like UPPER('%" + fil.NumAtCard + "')"; }
                if (fil.DocTotal > 0.00M) { filtros += " and \"DocTotal\" like '%" + fil.DocTotal + "%'"; }
                if (fil.U_SYP_STATUS != null) { filtros += " and UPPER(\"U_SYP_STATUS\")=UPPER('" + fil.U_SYP_STATUS + "')"; }
            }
            string query = "select top 50 t0.\"DocEntry\",t0.\"DocNum\",t0.\"DocDate\",t0.\"CardName\"" +
                ",t0.\"NumAtCard\",t0.\"DocTotal\",t0.\"U_SYP_STATUS\",t0.\"Comments\", t0.\"TaxDate\" " +
                " from " + uti.schemaHana + "opch t0 where t0.\"CardCode\" in (select \"CardCode\" from " + uti.schemaHana + "ocrd where \"GroupCode\" in (106,107)) " +
                                                    " and t0.\"DocEntry\">0 " + filtros + " order by 1 desc";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    OPCH_E o = new OPCH_E();
                    if (!hdr.IsDBNull(0)) { o.DocEntry = hdr.GetInt32(0); }
                    if (!hdr.IsDBNull(1)) { o.DocNum = hdr.GetInt32(1); }
                    if (!hdr.IsDBNull(2)) { o.DocDate = hdr.GetDateTime(2).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(3)) { o.CardName = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { o.NumAtCard = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { o.DocTotal = hdr.GetDecimal(5); }
                    if (!hdr.IsDBNull(6)) { o.U_SYP_STATUS = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { o.Comments = hdr.GetString(7); }
                    if (!hdr.IsDBNull(8)) { o.TaxDate = hdr.GetDateTime(8).ToString("dd/MM/yyyy"); }
                    o.entradasVinculadas = opdnD.entradasVinculadasFacturaProv(o.DocEntry);
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public OPCH_E buscarFacturaProveedor(int DocEntry)
        {
            OPCH_E o = new OPCH_E();
            string query = "select \"DocEntry\",\"DocNum\",\"DocDate\",\"CardName\",\"NumAtCard\",\"DocTotal\",\"U_SYP_STATUS\",\"Comments\"," +
                "\"TaxDate\"  from " + uti.schemaHana + "opch where \"DocEntry\"=" + DocEntry;
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
                if (!hdr.IsDBNull(7)) { o.Comments = hdr.GetString(7); }
                if (!hdr.IsDBNull(8)) { o.TaxDate = hdr.GetDateTime(8).ToString("dd/MM/yyyy"); }
                o.det = pch1D.listarDetallesPch1(o.DocEntry);
                hdr.Close();
            }
            catch { }
            return o;
        }
        public List<OPCH_E> listarFacturasProveedoresContrato(string CardCode, string FecIni, string FecFin, List<OITM_E> articulos)
        {
            List<OPCH_E> lista = new List<OPCH_E>();
            string productos = "";
            foreach (OITM_E a in articulos)
            {
                if (a == articulos[0])
                {
                    productos += "'" + a.ItemCode + "'";
                }
                else
                {
                    productos += ",'" + a.ItemCode + "'";
                }
            }
            string query =
                            "SELECT T0.\"DocEntry\",T0.\"CardCode\",T0.\"NumAtCard\",T0.\"DocTotal\"" +
                                        " FROM " + uti.schemaHana + "OPCH T0" +
                                        " INNER JOIN " + uti.schemaHana + "PCH1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\"" +
                                        " INNER JOIN " + uti.schemaHana + "OITM T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" " +
                                                                             " AND T2.\"ItemCode\" in(" + productos + ")" +
                                        " WHERE T0.\"TaxDate\" BETWEEN '" + FecIni + "' AND '" + FecFin +
                                                 "' AND T0.\"CardCode\"='" + CardCode + "' " +
                                                 " AND T0.\"CANCELED\"='N'" +
                                        " group by T0.\"CardCode\",T0.\"CardName\",T0.\"NumAtCard\" ,T0.\"TaxDate\",T0.\"DocTotal\",T0.\"DocEntry\"" +
                                        " order by T0.\"CardCode\",T0.\"TaxDate\",T0.\"NumAtCard\" ";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    OPCH_E f = new OPCH_E()
                    {
                        DocEntry = hdr.GetInt32(0),
                        CardCode = hdr.GetString(1),
                        NumAtCard = hdr.GetString(2),
                        DocTotal = hdr.GetDecimal(3)
                    };
                    lista.Add(f);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
    }
}
