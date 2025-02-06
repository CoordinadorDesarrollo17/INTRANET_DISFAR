using Capa_Entidad.AtencionCliente_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Sap.Data.Hana;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos.AtencionCliente_DAO.TablasSql
{
    public class SAT1_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        Ventas_DAO.TablasSql.RTV2_D rtv2D = new Ventas_DAO.TablasSql.RTV2_D();
        public List<SAT1_E> buscarDetallesSolicitudLista(int DocEntry)
        {
            List<SAT1_E> lista = new List<SAT1_E>();
            string query = "select DocEntry,ComprobanteFin,TareaFact,Comentario,TipoError,OpResponsable from ac.SAT1 where DocEntry=" + DocEntry;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    SAT1_E o = new SAT1_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.ComprobanteFin = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.TareaFact = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.Comentario = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.TipoError = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { o.OpResponsable = dr.GetString(5); }
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
        public List<SAT1_E> buscarDetallesSolicitud(int DocEntry)
        {
            List<SAT1_E> lista = new List<SAT1_E>();
            string query = "select DocEntry,Linea,NroSap,ItemCode,Dscription,UnitMsr,NumPerMsr,Quantity,BatchNum,ExpDate,unitMsrF,QuantityF,PriceAfVAT,LineTotalF,Problema,TipoError,OpResponsable,Comentario,Regalo,MotRegalo,TareaFact,ComprobanteVinc,AlmTransf,ComprobanteFin,AlmVenta, ErrorAlmacen, NCSAP from ac.SAT1 WHERE  DocEntry=@DocEntry";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string>() { "@DocEntry" }, DocEntry);
                while (dr.Read())
                {
                    SAT1_E o = new SAT1_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.Linea = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { o.NroSap = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { o.ItemCode = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.Dscription = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { o.UnitMsr = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { o.NumPerMsr = dr.GetDecimal(6); }
                    if (!dr.IsDBNull(7)) { o.Quantity = dr.GetDecimal(7); }
                    if (!dr.IsDBNull(8)) { o.BatchNum = dr.GetString(8); }
                    if (!dr.IsDBNull(9)) { o.ExpDate = dr.GetString(9); }
                    if (!dr.IsDBNull(10)) { o.unitMsrF = dr.GetString(10); }
                    if (!dr.IsDBNull(11)) { o.QuantityF = dr.GetDecimal(11); }
                    if (!dr.IsDBNull(12)) { o.PriceAfVAT = dr.GetDecimal(12); }
                    if (!dr.IsDBNull(13)) { o.LineTotalF = dr.GetDecimal(13); }
                    if (!dr.IsDBNull(14)) { o.Problema = dr.GetString(14); }
                    if (!dr.IsDBNull(15)) { o.TipoError = dr.GetString(15); }
                    if (!dr.IsDBNull(16)) { o.OpResponsable = dr.GetString(16); }
                    if (!dr.IsDBNull(17)) { o.Comentario = dr.GetString(17); }
                    if (!dr.IsDBNull(18)) { o.Regalo = dr.GetString(18); }
                    if (!dr.IsDBNull(19)) { o.MotRegalo = dr.GetString(19); }
                    if (!dr.IsDBNull(20)) { o.TareaFact = dr.GetString(20); }
                    if (!dr.IsDBNull(21)) { o.ComprobanteVinc = dr.GetString(21); }
                    if (!dr.IsDBNull(22)) { o.AlmTransf = dr.GetString(22); }
                    if (!dr.IsDBNull(23)) { o.ComprobanteFin = dr.GetString(23); }
                    if (!dr.IsDBNull(24)) { o.AlmVenta = dr.GetString(24); }
                    if (!dr.IsDBNull(25)) { o.ErrorAlmacen = dr.GetString(25); }
                    if (!dr.IsDBNull(26)) { o.NCSAP = dr.GetInt32(26); }
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }

        // Método de HANA - Mucho consumo de recursos cuando hay varios items
        public List<SAT1_E> ListarArticulosTicket(int DocNumTicket)
        {
            int i = 0;
            List<SAT1_E> lista = new List<SAT1_E>();

            // Obtenemos el NroSAP con el método "BuscarNroSAP", este método está preparado para recibir @DocNum o @DocEntry
            List<RTV2_E> listaRTV2 = rtv2D.BuscarNroSAPDeTicket("DocNum", DocNumTicket);

            //Para que la busqueda funcione de forma correcta se debe considerar siempre que las ordenes de venta en el detalle del ticket sean vigentes.
            foreach (RTV2_E d in listaRTV2)
            {
                string query = "SELECT " +
                        "T0.\"DocNum\",T1.\"ItemCode\",T1.\"Dscription\",T1.\"unitMsr\",T1.\"NumPerMsr\"" +
                        ",T2.\"Quantity\"/T1.\"NumPerMsr\",T2.\"BatchNum\",T3.\"ExpDate\",T1.\"PriceAfVAT\"" +
                        ",T1.\"WhsCode\"" +
                        " FROM " + uti.schemaHana + "ORDR T0" +
                        " INNER JOIN " + uti.schemaHana + "RDR1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\"" +
                        " LEFT OUTER JOIN " + uti.schemaHana + "IBT1 T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" AND T2.\"BaseType\" = T0.\"ObjType\"" +
                                                    " AND T2.\"BaseEntry\" = T0.\"DocEntry\" AND T2.\"Quantity\" > 0" +
                                                    " AND T2.\"BaseLinNum\" = T1.\"LineNum\" " +
                        " LEFT OUTER JOIN " + uti.schemaHana + "OBTN T3 ON T3.\"DistNumber\" = T2.\"BatchNum\" AND T3.\"ItemCode\" = T2.\"ItemCode\"" +
                        " WHERE T0.\"DocNum\" =" + d.NroSap +
                        " GROUP BY T0.\"DocNum\",T1.\"ItemCode\",T1.\"unitMsr\",T1.\"NumPerMsr\",T2.\"Quantity\",T1.\"Quantity\"" +
                                    ",T1.\"Dscription\",T2.\"BatchNum\",T1.\"PriceAfVAT\",T3.\"ExpDate\",T1.\"WhsCode\"" +
                                    " ORDER BY T1.\"Dscription\"";
                HanaConnection hcn = new HanaConnection(uti.cadHana);
                try
                {
                    hcn.Open();
                    HanaCommand hcmd = new HanaCommand(query, hcn);
                    hcmd.CommandType = CommandType.Text;
                    HanaDataReader hdr = hcmd.ExecuteReader();
                    while (hdr.Read())
                    {
                        SAT1_E o = new SAT1_E();
                        o.Linea = i++;
                        if (!hdr.IsDBNull(0)) { o.NroSap = hdr.GetInt32(0); }
                        if (!hdr.IsDBNull(1)) { o.ItemCode = hdr.GetString(1); }
                        if (!hdr.IsDBNull(2)) { o.Dscription = hdr.GetString(2); }
                        if (!hdr.IsDBNull(3)) { o.UnitMsr = hdr.GetString(3); }
                        if (!hdr.IsDBNull(4)) { o.NumPerMsr = hdr.GetDecimal(4); }
                        if (!hdr.IsDBNull(5)) { o.Quantity = hdr.GetDecimal(5); }
                        if (!hdr.IsDBNull(6)) { o.BatchNum = hdr.GetString(6); }
                        if (!hdr.IsDBNull(7)) { o.ExpDate = hdr.GetDateTime(7).ToString("yyyy-MM-dd"); }
                        if (!hdr.IsDBNull(8)) { o.PriceAfVAT = hdr.GetDecimal(8); }
                        if (!hdr.IsDBNull(9)) { o.AlmVenta = hdr.GetString(9); }
                        lista.Add(o);
                    }
                    hdr.Close();
                    hcn.Close();
                }
                catch { hcn.Close(); }
            }
            return lista;
        }
        public List<SAT1_E> BuscarCodProductosTicket(int DocNumTicket)
        {
            int i = 0;
            List<SAT1_E> lista = new List<SAT1_E>();

            // Obtenemos el NroSAP con el método "BuscarNroSAP", este método está preparado para recibir @DocNum o @DocEntry
            List<RTV2_E> listaRTV2 = rtv2D.BuscarNroSAPDeTicket("DocNum", DocNumTicket);

            if (listaRTV2 != null && listaRTV2.Count >= 1)
            {
                foreach (RTV2_E d in listaRTV2)
                {
                    string query = "SELECT T0.\"DocNum\", T1.\"ItemCode\", T1.\"Dscription\"" +
                                            $" FROM {uti.schemaHana}ORDR T0" +
                                            $" INNER JOIN {uti.schemaHana}RDR1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\"" +
                                            $" WHERE T0.\"DocNum\" ='{d.NroSap}'" +
                                            " GROUP BY T0.\"DocNum\",T1.\"ItemCode\",T1.\"Dscription\" ORDER BY T1.\"Dscription\"";

                    HanaConnection hcn = new HanaConnection(uti.cadHana);
                    try
                    {
                        hcn.Open();
                        HanaCommand hcmd = new HanaCommand(query, hcn);
                        hcmd.CommandType = CommandType.Text;
                        HanaDataReader hdr = hcmd.ExecuteReader();
                        while (hdr.Read())
                        {
                            SAT1_E o = new SAT1_E();
                            o.Linea = i++;
                            if (!hdr.IsDBNull(0)) { o.NroSap = hdr.GetInt32(0); }
                            if (!hdr.IsDBNull(1)) { o.ItemCode = hdr.GetString(1); }
                            if (!hdr.IsDBNull(2)) { o.Dscription = hdr.GetString(2); }

                            lista.Add(o);
                        }
                        hdr.Close();
                        hcn.Close();
                    }
                    catch { hcn.Close(); }
                }
            }

            return lista;
        }

        public List<SAT1_ComprobanteFin> buscarComprobanteFin(string DocFact = "")
        {
            List<SAT1_ComprobanteFin> result = new List<SAT1_ComprobanteFin>();
            string query = "SELECT DocEntry, ComprobanteFin FROM ac.SAT1 WHERE ComprobanteFin LIKE '%" + DocFact + "%'";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                SqlDataReader dr = cmd.ExecuteReader();             // ejecuta
                while (dr.Read())
                {
                    SAT1_ComprobanteFin obj = new SAT1_ComprobanteFin();
                    if (!dr.IsDBNull(0)) { obj.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { obj.ComprobanteFin = dr.GetString(1); }
                    result.Add(obj);
                }// lectura 

                dr.Close();
                cn.Close();
            }
            catch
            {
                cn.Close();
            }

            return result;
        }
    }
}
