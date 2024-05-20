using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.TablasSql;
using Sap.Data.Hana;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class IPE2_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        OITM_D oitmD = new OITM_D();
        //meotodos de hana
        public List<IPE2_E> listarArticulosLotes(OIPE_E filtroPeriodo, IPE2_E filtro = null)
        {
            List<IPE2_E> lista = new List<IPE2_E>();
            string filPer = "";
            string fil = "";
            string query = "";
            if (filtroPeriodo != null)
            {
                if (filtroPeriodo.enlistarDetAlmacenes() != null) { filPer += " and x.\"WhsCode\" in(" + filtroPeriodo.enlistarDetAlmacenes() + "'')"; }
            }
            if (filtro == null)
            {
                query = "select top 50 \"ItemCode\",\"BatchNum\",\"WhsCode\",\"ItemName\",\"ExpDate\",\"Quantity\" " +
                " from " + uti.schemaHana + "OIBT where \"ItemCode\" is not null order by \"ItemCode\"";
            }
            else
            {
                if (filtro.Quantity > 0) { fil += " and x.\"Quantity\">0"; }
                if (filtro.WhsCode != null) { fil += " and x.\"WhsCode\"='" + filtro.WhsCode + "'"; }
                if (filtro.ItemCode != null) { fil += " and x.\"ItemCode\"='" + filtro.ItemCode + "'"; }
                query = "select x.\"ItemCode\",x.\"BatchNum\",x.\"WhsCode\",x.\"ItemName\",x.\"ExpDate\",x.\"Quantity\" " +
                    ",(select \"AvgPrice\" from " + uti.schemaHana + "oitw where \"ItemCode\" = x.\"ItemCode\" and \"WhsCode\" = x.\"WhsCode\" ) as \"AvgPrice\" " +
                " from " + uti.schemaHana + "OIBT x where x.\"ItemCode\" is not null " + filPer + fil + " order by x.\"ItemCode\"";
            }
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    IPE2_E o = new IPE2_E();
                    if (!hdr.IsDBNull(0)) { o.ItemCode = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { o.BatchNum = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { o.WhsCode = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { o.ItemName = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { o.ExpDate = hdr.GetDateTime(4); }
                    if (!hdr.IsDBNull(5)) { o.Quantity = hdr.GetDecimal(5); }
                    if (!hdr.IsDBNull(6)) { o.AvgPrice = hdr.GetDecimal(6); }
                    var oitmE = oitmD.Obtener(o.ItemCode);
                    o.NumInBuy = oitmE.NumInBuy;
                    o.ItmsGrpCod = oitmE.ItmsGrpCod;
                    o.FirmCode = oitmE.FirmCode;
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public List<IPE2_E> listarArticulosUsrEquiSap(OIEQ_E eq)
        {
            //para stocks <=0
            List<IPE2_E> lista = new List<IPE2_E>();
            string fil = "";
            if (eq != null)
            {
                fil += " and t1.\"WhsCode\" ='" + eq.WhsCode + "'";
                fil += " and t0.\"FirmCode\" in(0";
                foreach (IEQ2_E fab in eq.DetFabricantes)
                {
                    fil += "," + fab.FirmCode;
                }
                fil += ")";
                if (eq.Controlados == "Si") { fil += " and t0.\"ItmsGrpCod\"=103"; }
                else { fil += " and t0.\"ItmsGrpCod\"<>103"; }
            }
            //else { return lista; }
            string query = "select t0.\"ItemCode\",t0.\"ItemName\",t0.\"FirmCode\",t0.\"NumInBuy\" from " + uti.schemaHana + "oitm t0" +
                " inner join " + uti.schemaHana + "oitw t1 on t1.\"ItemCode\"=t0.\"ItemCode\" " +
                " where t0.\"ItemCode\" is not null and t1.\"OnHand\"<=0 " + fil + " order by t0.\"ItemName\"";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    IPE2_E o = new IPE2_E();
                    if (!hdr.IsDBNull(0)) { o.ItemCode = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { o.ItemName = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { o.FirmCode = hdr.GetInt32(2); }
                    if (!hdr.IsDBNull(3)) { o.NumInBuy = hdr.GetDecimal(3); }
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public List<IPE2_E> listarLotesSinStock(IPE2_E obj)
        {
            List<IPE2_E> lista = new List<IPE2_E>();
            string query = "select \"ItemCode\",\"BatchNum\",\"WhsCode\",\"ItemName\",\"ExpDate\" from " + uti.schemaHana + "oibt " +
                            " where \"ItemCode\"='" + obj.ItemCode + "' and \"WhsCode\"='" + obj.WhsCode + "' and \"Quantity\"=0 and \"ExpDate\">'" + obj.ExpDate.ToString("yyyy-MM-dd") + "'";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    IPE2_E o = new IPE2_E();
                    if (!hdr.IsDBNull(0)) { o.ItemCode = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { o.BatchNum = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { o.WhsCode = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { o.ItemName = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { o.ExpDate = hdr.GetDateTime(4); o.AuxExpDate = o.ExpDate.ToString("yyyy-MM-dd"); }
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        //metodo sql
        public List<IPE2_E> buscarDetallesArticulos(int DocEntry)
        {
            List<IPE2_E> lista = new List<IPE2_E>();
            string query = "select top 50 * from al.ipe2 where DocEntry=" + DocEntry + " order by ItemCode";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    IPE2_E o = new IPE2_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.ItemCode = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.BatchNum = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.WhsCode = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.ItemName = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { o.ExpDate = dr.GetDateTime(5); }
                    if (!dr.IsDBNull(6)) { o.Quantity = dr.GetDecimal(6); }
                    if (!dr.IsDBNull(7)) { o.NumInBuy = dr.GetDecimal(7); }
                    if (!dr.IsDBNull(8)) { o.AvgPrice = dr.GetDecimal(8); }
                    if (!dr.IsDBNull(9)) { o.ItmsGrpCod = dr.GetInt32(9); }
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
        public List<IPE2_E> listarArticulosUsrEqui(OIEQ_E eq)
        {
            List<IPE2_E> lista = new List<IPE2_E>();
            string fil = "";
            if (eq != null)
            {
                fil += " and DocEntry=" + eq.DocEntryPer;
                fil += " and WhsCode ='" + eq.WhsCode + "'";
                fil += " and FirmCode in(0";
                foreach (IEQ2_E fab in eq.DetFabricantes)
                {
                    fil += "," + fab.FirmCode;
                }
                fil += ")";
                if (eq.Controlados == "Si") { fil += " and ItmsGrpCod=103"; }
                else { fil += " and ItmsGrpCod<>103"; }
            }
            string query = "select DocEntry,ItemCode,ItemName,FirmCode,NumInBuy from al.ipe2 where DocEntry>0 " + fil +
                " group by Itemcode,DocEntry,ItemName,FirmCode,NumInBuy order by ItemName";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    IPE2_E o = new IPE2_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.ItemCode = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.ItemName = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.FirmCode = dr.GetInt32(3); }
                    if (!dr.IsDBNull(4)) { o.NumInBuy = dr.GetDecimal(4); }
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
        //para contabilizacion sql
        public List<IPE2_E> listarArticulos(IPE2_E filtro)
        {
            List<IPE2_E> lista = new List<IPE2_E>();
            string fil = "";
            if (filtro == null) { return lista; }
            else
            {
                if (filtro.DocEntry > 0) { fil += " and DocEntry=" + filtro.DocEntry; }
                if (filtro.ItemCode != null) { fil += " and ItemCode='" + filtro.ItemCode + "'"; }
                if (filtro.WhsCode != null) { fil += " and WhsCode='" + filtro.WhsCode + "'"; }
            }
            string query = "select * from al.ipe2 where DocEntry>0 " + fil + " order by ItemCode,BatchNum";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    IPE2_E o = new IPE2_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.ItemCode = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.BatchNum = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.WhsCode = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.ItemName = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { o.ExpDate = dr.GetDateTime(5); }
                    if (!dr.IsDBNull(6)) { o.Quantity = dr.GetDecimal(6); }
                    if (!dr.IsDBNull(7)) { o.NumInBuy = dr.GetDecimal(7); }
                    if (!dr.IsDBNull(8)) { o.AvgPrice = dr.GetDecimal(8); }
                    if (!dr.IsDBNull(9)) { o.ItmsGrpCod = dr.GetInt32(9); }
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
    }
}