using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.Rutas_ENT;
using Capa_Entidad.Almacen_ENT;
using Capa_Entidad.Ventas_ENT;
using System.Data;
using System.Data.SqlClient;
using Sap.Data.Hana;
using Capa_Datos.Ventas_DAO;
using Capa_Entidad.Almacen_ENT.Tablas;

namespace Capa_Datos.Rutas_DAO
{
    public class OrdenRegistroRutas_D
    {
        //utililarios_pruebas uti = new utililarios_pruebas();
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        TicketVenta_D ticketD = new TicketVenta_D();
        public List<OrdenRegistroRutas_E> listarOrru(OrdenRegistroRutas_E o)
        {
            List<OrdenRegistroRutas_E> lista = new List<OrdenRegistroRutas_E>();
            string query = "";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try 
            {
                if (o != null) 
                {
                    query = "select top 100 DocEntry,DocNum,TipoRuta,TransDesc,FechaDoc,Estado,TotalCajas" +
                            " from orru where DocNum>0 ";
                    if (o.DocNum> 0){query += " and DocNum="+o.DocNum;}
                    if(o.TipoRuta!=null){query +=" and TipoRuta='"+o.TipoRuta+"'";}
                    if(o.TransDesc!=null){query += " and TransDesc like '%" + o.TransDesc + "%'";}
                    if(o.FechaDoc!=null){query+=" and FechaDoc='"+o.FechaDoc+"'";}
                    if(o.Estado!=null){query += " and Estado='"+o.Estado+"'";}
                    if(o.TotalCajas>0){query += " and TotalCajas=" + o.TotalCajas;}
                    query += " order by 2 desc";
                }
                else {
                    query = "select top 100 DocEntry,DocNum,TipoRuta,TransDesc,FechaDoc,Estado,TotalCajas" +
                 " from orru order by 2 desc";
                }
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    OrdenRegistroRutas_E or = new OrdenRegistroRutas_E();
                    or.DocEntry = dr.GetInt32(0);
                    or.DocNum = dr.GetInt32(1);
                    if (!dr.IsDBNull(2)) { or.TipoRuta = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { or.TransDesc = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { or.FechaDoc = dr.GetDateTime(4).ToString("dd-MM-yyyy"); }
                    if (!dr.IsDBNull(5)) { or.Estado = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { or.TotalCajas = dr.GetInt16(6); }
                    lista.Add(or);
                }
                dr.Close();
                cn.Close();
            }catch { cn.Close(); }

            return lista;
        }
        public OrdenRegistroRutas_E inicializarOrdenDeRuta()
        {
            OrdenRegistroRutas_E o = new OrdenRegistroRutas_E();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select DocNum from gene where Tabla='ORRU'", cn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                o.DocNum = dr.GetInt32(0);
                dr.Close();
                cn.Close();
            }catch { cn.Close(); }
            return o;
        }
        public List<OrdenRegistroRutas_E> listaTranportistas()
        {
            List<OrdenRegistroRutas_E> lista = new List<OrdenRegistroRutas_E>();
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand("SELECT T0.\"DocEntry\",T0.\"U_SYP_CHNO\" FROM " + uti.schemaHana + "\"@SYP_CONDUC\" T0", hcn);
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    OrdenRegistroRutas_E o = new OrdenRegistroRutas_E();
                    o.TransCod= hdr.GetInt32(0);
                    o.TransDesc = hdr.GetString(1);
                    lista.Add(o);
                }
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        public List<OrdenRegistroRutas_E> listaPlacas()
        {
            List<OrdenRegistroRutas_E> lista = new List<OrdenRegistroRutas_E>();
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand("SELECT T0.\"DocEntry\",T0.\"U_SYP_VEPL\" FROM " + uti.schemaHana + "\"@SYP_VEHICU\" T0 ORDER BY T0.\"U_SYP_VEPL\" DESC", hcn);
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    OrdenRegistroRutas_E o = new OrdenRegistroRutas_E();
                    o.PlacaCod = hdr.GetInt32(0);
                    o.PlacaDesc = hdr.GetString(1);
                    lista.Add(o);
                }
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        public List<OrdenRegistroRutas_E> listaCopilotos()
        {
            List<OrdenRegistroRutas_E> lista = new List<OrdenRegistroRutas_E>();
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand("SELECT T0.\"empID\",(T0.\"firstName\"||' '||T0.\"lastName\") FROM " + uti.schemaHana + "OHEM T0 ORDER BY T0.\"firstName\"", hcn);
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    OrdenRegistroRutas_E o = new OrdenRegistroRutas_E();
                    o.CopilCod = hdr.GetInt32(0);
                    o.CopilDesc = hdr.GetString(1);
                    lista.Add(o);
                }
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        public List<OrdenRegistroRutas_E> listaOrigenesDestinos()
        {
            List<OrdenRegistroRutas_E> lista = new List<OrdenRegistroRutas_E>();
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand("SELECT T0.\"WhsCode\",T0.\"WhsName\",T0.\"Street\",T0.\"StreetNo\",T0.\"Building\",T0.\"Block\",T0.\"City\",T0.\"County\" FROM " + uti.schemaHana + "OWHS T0 ORDER BY T0.\"WhsCode\"", hcn);
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    OrdenRegistroRutas_E o = new OrdenRegistroRutas_E();
                    string direccionAlm = "";
                    o.AlmOrigenCod = hdr.GetString(0);
                    o.AlmOrigenDesc = hdr.GetString(1);
                    if (!hdr.IsDBNull(2)) { direccionAlm+= hdr.GetString(2)+" "; }
                    if (!hdr.IsDBNull(3)) { direccionAlm += hdr.GetString(3)+" "; }
                    if (!hdr.IsDBNull(4)) { direccionAlm += hdr.GetString(4) + " "; }
                    if (!hdr.IsDBNull(5)) { direccionAlm += hdr.GetString(5) + " "; }
                    if (!hdr.IsDBNull(6)) { direccionAlm += hdr.GetString(6) + " "; }
                    if (!hdr.IsDBNull(7)) { direccionAlm += hdr.GetString(7) + " "; }
                    o.AlmOrigenDesc2 = direccionAlm;
                    lista.Add(o);
                }
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        public List<RRU12_E> listaProductosOWTQ(string guia)
        {
            List<RRU12_E> lista = new List<RRU12_E>();
            try
            {
                HanaDataReader hdr2 = db.HanaExecuteReaderNoSp("SELECT \"DocEntry\" FROM " + uti.schemaHana + "OWTQ WHERE \"U_SYP_MDTD\"||'-'||\"U_SYP_MDSD\"||'-'||\"U_SYP_MDCD\"='" + guia + "'");
                hdr2.Read();
                int DocEntry = hdr2.GetInt32(0);
                hdr2.Close();

                HanaDataReader hdr3 = db.HanaExecuteReaderNoSp("CALL " + uti.schemaHana + "CBW_ARTICULOSRUTA(" + DocEntry + ")");
                int linea = 0;
                while (hdr3.Read())
                {
                    linea++;
                    RRU12_E r = new RRU12_E();
                    r.Linea = linea;
                    if (!hdr3.IsDBNull(0)) { r.ItemCode = hdr3.GetString(0); }
                    if (!hdr3.IsDBNull(1)) { r.ItemName = hdr3.GetString(1); }
                    if (!hdr3.IsDBNull(2)) { r.UnitMed = hdr3.GetString(2); }
                    if (!hdr3.IsDBNull(3)) { r.Lote = hdr3.GetString(3); }
                    if (!hdr3.IsDBNull(4)) { r.CantidadL = hdr3.GetDecimal(4); }
                    if (!hdr3.IsDBNull(5)) { r.LaboDesc = hdr3.GetString(5); }
                    if (!hdr3.IsDBNull(6)) { r.CantUnitMed = hdr3.GetDecimal(6); }
                    if (!hdr3.IsDBNull(7)) { r.LaboCod = hdr3.GetInt32(7); }
                    lista.Add(r);
                }
                hdr3.Close();
            }
            catch(Exception e) { }
            return lista;
        }
        public List<TicketVenta_E> listaSocios(string FechaCont)
        {
            List<TicketVenta_E> lista = new List<TicketVenta_E>();
            string query = "";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();                
                query = "SELECT DISTINCT CardCode,CardName from ortv where FechaTicket between dateadd(day,-7,convert(datetime,'"+FechaCont+"',120)) AND '" + FechaCont+"' AND EstadoPedido='EMPACADO' ORDER BY CardName"; 
                SqlCommand cmd = new SqlCommand(query,cn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TicketVenta_E t = new TicketVenta_E();
                    t.CardCode = dr.GetString(0);
                    t.CardName = dr.GetString(1);
                    lista.Add(t);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public List<TicketVenta_E> listarTicketsVenta(string FechaCont,string CardCode)
        {
            List<TicketVenta_E> lista = new List<TicketVenta_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                string query = "";
                query = "select DocEntry,DocNum,FechaTicket,CardCode,CardName,HoraTicket,LugarDestino," +
                    "PropietarioDesc,MontoFinal,EstadoPago,EstadoPedido from ortv where FechaTicket between dateadd(day,-7,convert(datetime,'" + FechaCont + "',120)) and '" + FechaCont + "' " +
                    "and CardCode='" + CardCode+"' and EstadoPedido='EMPACADO' order by \"DocNum\" desc";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TicketVenta_E ticket = new TicketVenta_E();
                    ticket.DocEntry = dr.GetInt32(0);
                    ticket.DocNum = dr.GetInt32(1);
                    if (!dr.IsDBNull(2)) { ticket.FechaTicket = dr.GetDateTime(2).ToString("dd-MM-yyyy"); }
                    if (!dr.IsDBNull(3)) { ticket.CardCode = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { ticket.CardName = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { ticket.HoraTicket = dr.GetTimeSpan(5).ToString(@"hh\:mm"); }
                    if (!dr.IsDBNull(6)) { ticket.LugarDestino = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { ticket.PropietarioDesc = dr.GetString(7).Substring(0, 12); }
                    if (!dr.IsDBNull(8)) { ticket.MontoFinal = dr.GetDecimal(8); }
                    if (!dr.IsDBNull(9)) { ticket.EstadoPago = dr.GetString(9); }
                    if (!dr.IsDBNull(10)) { ticket.EstadoPedido = dr.GetString(10); }
                    lista.Add(ticket);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public string GuiasTicket(int DocEntry)
        {
            DetalleRegistroRutas_E d = new DetalleRegistroRutas_E();
            d.Guias = "";
            Ventas_DAO.Tablas.ORDR_D ordrD = new Ventas_DAO.Tablas.ORDR_D();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select NroSap from rtv1 where DocEntry="+DocEntry, cn);
                SqlDataReader dr = cmd.ExecuteReader();
                string guiasTicket = "";
                while (dr.Read())
                {
                    d.DocNumTicket = dr.GetInt32(0);
                    
                    guiasTicket += ordrD.guiasTraslado(d.DocNumTicket);
                }
                d.Guias = guiasTicket;
                dr.Close();
                cn.Close();
            }catch { cn.Close(); }
            return d.Guias;
        }
        public List<OWTQ_E> listarGuiasTraslado()
        {
            List<OWTQ_E> lista = new List<OWTQ_E>();
            string query = "SELECT top 40 T0.\"DocNum\",IFNULL(T0.\"U_SYP_MDTD\"||'-'||T0.\"U_SYP_MDSD\"||'-'||T0.\"U_SYP_MDCD\",'') " +
                    ",(select \"SlpName\" from " + uti.schemaHana + "oslp where \"SlpCode\" = T0.\"SlpCode\")" +
                    "FROM " + uti.schemaHana + "OWTQ T0 " +
                    "WHERE T0.CANCELED = 'N' ORDER BY T0.\"DocEntry\" desc";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while(hdr.Read())
                {
                    OWTQ_E o = new OWTQ_E();
                    if (!hdr.IsDBNull(0)) { o.DocNum=hdr.GetInt32(0); }
                    if (!hdr.IsDBNull(1)) { o.NumAtCard = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { o.SlpName = hdr.GetString(2); }
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { };
            return lista;
        }
        public OrdenRegistroRutas_E obtenerOrdenDeRuta(int DocEntry)
        {
            OrdenRegistroRutas_E o = new OrdenRegistroRutas_E();
            o.ListaDetalleRegistroRutas = new List<DetalleRegistroRutas_E>();
            o.RRU1 = new List<RRU1_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select * from orru where DocEntry="+DocEntry, cn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                o.DocEntry = dr.GetInt32(0);
                o.DocSerie = dr.GetString(1);
                o.DocNum = dr.GetInt32(2);
                o.TipoRuta = dr.GetString(3);
                o.TransCod = dr.GetInt32(4);
                o.TransDesc = dr.GetString(5);
                o.PlacaCod = dr.GetInt32(6);
                o.PlacaDesc = dr.GetString(7);
                o.CopilCod = dr.GetInt32(8);
                o.CopilDesc = dr.GetString(9);
                if (!dr.IsDBNull(10)) { o.Copil2Cod = dr.GetInt32(10); }
                if (!dr.IsDBNull(11)) { o.Copil2Desc = dr.GetString(11); }
                if (!dr.IsDBNull(12)) { o.Copil3Cod = dr.GetInt32(12); }
                if (!dr.IsDBNull(13)) { o.Copil3Desc = dr.GetString(13); }
                if (!dr.IsDBNull(14)) { o.Copil4Cod = dr.GetInt32(14); }
                if (!dr.IsDBNull(15)) { o.Copil4Desc = dr.GetString(15); }
                o.FechaCont = dr.GetDateTime(16).ToString("dd-MM-yyyy");
                o.FechaDoc = dr.GetDateTime(17).ToString("dd-MM-yyyy");
                o.AlmOrigenCod = dr.GetString(18);
                o.AlmOrigenDesc = dr.GetString(19);
                o.AlmOrigenDesc2 = dr.GetString(20);
                o.AlmDestinoCod = dr.GetString(21);
                o.AlmDestinoDesc = dr.GetString(22);
                o.AlmDestinoDesc2 = dr.GetString(23);
                o.PropietarioCod = dr.GetInt32(24);
                o.PropietarioDesc = dr.GetString(25);
                o.HoraI = dr.GetTimeSpan(26).ToString();
                if (!dr.IsDBNull(27)) { o.HoraT = dr.GetTimeSpan(27).ToString(); }
                o.Estado = dr.GetString(28);
                o.TotalCajas = dr.GetInt16(29);
                if (!dr.IsDBNull(30)) { o.Observaciones = dr.GetString(30); }
                dr.Close();

                SqlCommand cmd2 = new SqlCommand("select * from rru0 where DocEntry="+DocEntry, cn);
                SqlDataReader dr2 = cmd2.ExecuteReader();
                while(dr2.Read())
                {
                    DetalleRegistroRutas_E d = new DetalleRegistroRutas_E();
                    d.Linea = dr2.GetInt32(1);
                    d.DocEntryTicket = dr2.GetInt32(2);
                    d.DocNumTicket = dr2.GetInt32(3);
                    d.Guias = dr2.GetString(4);
                    d.Verificado = dr2.GetString(5);
                    d.Ticket = ticketD.obtenerTicket(d.DocEntryTicket);
                    o.ListaDetalleRegistroRutas.Add(d);
                }
                dr2.Close();

                SqlCommand cmd3 = new SqlCommand("select * from rru1 where DocEntry=" + DocEntry, cn);
                SqlDataReader dr3 = cmd3.ExecuteReader();
                while(dr3.Read())
                {
                    RRU1_E r1 = new RRU1_E();r1.ListaRRU12 = new List<RRU12_E>();
                    r1.DocEntry = DocEntry;
                    r1.Linea = dr3.GetInt32(1);
                    if (!dr3.IsDBNull(2)) { r1.Otros = dr3.GetString(2); }
                    if (!dr3.IsDBNull(3)) { r1.Guia = dr3.GetString(3); }
                    if (!dr3.IsDBNull(4)) { r1.NroSap = int.Parse(dr3.GetString(4)); }
                    if (!dr3.IsDBNull(5)) { r1.OpEnvio = dr3.GetString(5); }
                    if (!dr3.IsDBNull(6)) { r1.Cajas = dr3.GetInt32(6); }
                    if (!dr3.IsDBNull(7)) { r1.OpRecepcion = dr3.GetString(7); }
                    if (!dr3.IsDBNull(8)) { r1.Verificado = dr3.GetString(8); }
                    r1.ListaRRU12 = obtProdRutaLinea(r1.DocEntry, r1.Linea);
                    o.RRU1.Add(r1);
                }
                dr3.Close();
                cn.Close();
                List<RRU12_E> obtProdRutaLinea(int fdocE,int fli)
                {
                    List<RRU12_E> flis= new List<RRU12_E>();
                    SqlConnection fcn = new SqlConnection(uti.cadSql);
                    try
                    {
                        fcn.Open();
                        SqlCommand fcmd = new SqlCommand("select * from rru12 where BaseEntry="+fdocE+" and Baselinea="+fli, fcn);
                        SqlDataReader fdr = fcmd.ExecuteReader();
                        while(fdr.Read())
                        {
                            RRU12_E fr12 = new RRU12_E();
                            fr12.BaseEntry = fdocE;
                            fr12.BaseLinea = fli;
                            fr12.Linea = fdr.GetInt32(2);
                            fr12.ItemCode = fdr.GetString(3);
                            fr12.ItemName = fdr.GetString(4);
                            fr12.Lote = fdr.GetString(5);
                            fr12.CantidadL = fdr.GetDecimal(6);
                            fr12.LaboCod = fdr.GetInt32(7);
                            fr12.LaboDesc = fdr.GetString(8);
                            fr12.UnitMed = fdr.GetString(9);
                            fr12.CantUnitMed = fdr.GetInt32(10);
                            fr12.Cajas = fdr.GetInt32(11);
                            flis.Add(fr12);
                        }
                        fdr.Close();
                        fcn.Close();
                    }
                    catch { fcn.Close(); }
                    return flis;
                }
            }catch { cn.Close(); }
            return o;
        }
        public OrdenRegistroRutas_E obtenerOrdenDeRutaTicket(int DocEntryTicket)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            OrdenRegistroRutas_E o = new OrdenRegistroRutas_E();
            try
            {
                cn.Open();
                string query = "";
                query = "Select * from ORRU where DocEntry = " +
                "(Select DocEntry from RRU0 where DocEntryTicket =" + DocEntryTicket + ")";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                o.DocEntry = dr.GetInt32(0);
                o.DocSerie = dr.GetString(1);
                o.DocNum = dr.GetInt32(2);
                o.TipoRuta = dr.GetString(3);
                o.TransCod = dr.GetInt32(4);
                o.TransDesc = dr.GetString(5);
                o.PlacaCod = dr.GetInt32(6);
                o.PlacaDesc = dr.GetString(7);
                o.CopilCod = dr.GetInt32(8);
                o.CopilDesc = dr.GetString(9);
                if (!dr.IsDBNull(10)) { o.Copil2Cod = dr.GetInt32(10); }
                if (!dr.IsDBNull(11)) { o.Copil2Desc = dr.GetString(11); }
                if (!dr.IsDBNull(12)) { o.Copil3Cod = dr.GetInt32(12); }
                if (!dr.IsDBNull(13)) { o.Copil3Desc = dr.GetString(13); }
                if (!dr.IsDBNull(14)) { o.Copil4Cod = dr.GetInt32(14); }
                if (!dr.IsDBNull(15)) { o.Copil4Desc = dr.GetString(15); }
                o.FechaCont = dr.GetDateTime(16).ToString("dd-MM-yyyy");
                o.FechaDoc = dr.GetDateTime(17).ToString("dd-MM-yyyy");
                o.AlmOrigenCod = dr.GetString(18);
                o.AlmOrigenDesc = dr.GetString(19);
                o.AlmOrigenDesc2 = dr.GetString(20);
                o.AlmDestinoCod = dr.GetString(21);
                o.AlmDestinoDesc = dr.GetString(22);
                o.AlmDestinoDesc2 = dr.GetString(23);
                o.PropietarioCod = dr.GetInt32(24);
                o.PropietarioDesc = dr.GetString(25);
                o.HoraI = dr.GetTimeSpan(26).ToString();
                if (!dr.IsDBNull(27)) { o.HoraT = dr.GetTimeSpan(27).ToString(); }
                o.Estado = dr.GetString(28);
                o.TotalCajas = dr.GetInt16(29);
                if (!dr.IsDBNull(30)) { o.Observaciones = dr.GetString(30); }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return o;
        }
        public int CrearOrdenDeRuta(OrdenRegistroRutas_E o)
        {
            int status = -1;
            foreach (OrdenRegistroRutas_E or in listaOrigenesDestinos())
            {
                if(o.AlmOrigenCod==or.AlmOrigenCod)
                {
                    o.AlmOrigenDesc2 = or.AlmOrigenDesc2;
                }
                if(o.AlmDestinoCod==or.AlmOrigenCod)
                {
                    o.AlmDestinoDesc2 = or.AlmOrigenDesc2;
                }
            }
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try { 
                    SqlCommand cmd = new SqlCommand("MANT_ORRU", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento","AR"); //add registrar
                    cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry).Direction = ParameterDirection.Output; ;
                    cmd.Parameters.AddWithValue("@DocSerie", 20);
                    cmd.Parameters.AddWithValue("@DocNum", o.DocNum).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@TipoRuta", o.TipoRuta);
                    cmd.Parameters.AddWithValue("@TransCod", o.TransCod);
                    cmd.Parameters.AddWithValue("@TransDesc", o.TransDesc);
                    cmd.Parameters.AddWithValue("@PlacaCod", o.PlacaCod);
                    cmd.Parameters.AddWithValue("@PlacaDesc", o.PlacaDesc);
                    cmd.Parameters.AddWithValue("@CopilCod", o.CopilCod);
                    cmd.Parameters.AddWithValue("@CopilDesc", o.CopilDesc);
                    cmd.Parameters.AddWithValue("@Copil2Cod", o.Copil2Cod);
                    cmd.Parameters.AddWithValue("@Copil2Desc", o.Copil2Desc);
                    cmd.Parameters.AddWithValue("@Copil3Cod", o.Copil3Cod);
                    cmd.Parameters.AddWithValue("@Copil3Desc", o.Copil3Desc);
                    cmd.Parameters.AddWithValue("@Copil4Cod", o.Copil4Cod);
                    cmd.Parameters.AddWithValue("@Copil4Desc", o.Copil4Desc);
                    cmd.Parameters.AddWithValue("@FechaCont", o.FechaCont);                
                    cmd.Parameters.AddWithValue("@AlmOrigenCod", o.AlmOrigenCod);
                    cmd.Parameters.AddWithValue("@AlmOrigenDesc", o.AlmOrigenDesc);
                    cmd.Parameters.AddWithValue("@AlmOrigenDesc2", o.AlmOrigenDesc2);
                    cmd.Parameters.AddWithValue("@AlmDestinoCod", o.AlmDestinoCod);
                    cmd.Parameters.AddWithValue("@AlmDestinoDesc", o.AlmDestinoDesc);
                    cmd.Parameters.AddWithValue("@AlmDestinoDesc2", o.AlmDestinoDesc2);
                    cmd.Parameters.AddWithValue("@PropietarioCod", o.PropietarioCod);
                    cmd.Parameters.AddWithValue("@PropietarioDesc", o.PropietarioDesc);
                    cmd.Parameters.AddWithValue("@HoraI", o.HoraI);
                    cmd.Parameters.AddWithValue("@HoraT", o.HoraT);
                    cmd.Parameters.AddWithValue("@TotalCajas", o.TotalCajas);
                    cmd.Parameters.AddWithValue("@Observaciones", o.Observaciones);

                    if (o.ListaDetalleRegistroRutas != null)
                    {
                        SqlParameter tbDet = new SqlParameter("@Det", SqlDbType.Structured);
                        tbDet.Value = DetalleRegistroRutas_E.tbDetalle(o.ListaDetalleRegistroRutas);
                        tbDet.TypeName = "dbo.TPRRU0";
                        cmd.Parameters.AddWithValue("@Det", tbDet.Value);
                    }
                    if(o.RRU1!=null)
                    {
                        SqlParameter tbDet1 = new SqlParameter("@Det1", SqlDbType.Structured);
                        tbDet1.Value = RRU1_E.tbDetalle(o.RRU1);
                        tbDet1.TypeName = "dbo.TPRRU1";
                        cmd.Parameters.AddWithValue("@Det1", tbDet1.Value);
                        // detalles de productos
                        SqlParameter tbDetProd = new SqlParameter("@DetProd", SqlDbType.Structured);
                        List<RRU12_E> listaRRU12 = new List<RRU12_E>();
                        foreach (RRU1_E rru1 in o.RRU1)
                        {
                            if (rru1.ListaRRU12 != null)
                            {
                                foreach (RRU12_E rru12 in rru1.ListaRRU12)
                                {
                                    listaRRU12.Add(rru12);
                                }
                            }
                        }
                        tbDetProd.Value = RRU12_E.tbRRU12(listaRRU12);
                        tbDetProd.TypeName = "dbo.TPRRU12";
                        cmd.Parameters.AddWithValue("@DetProd", tbDetProd.Value);
                    }
                    
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
                    
                    if(o.ListaDetalleRegistroRutas!=null)
                    {
                        // cambio de estado ticket
                        foreach (DetalleRegistroRutas_E dr in DetalleRegistroRutas_E.listaFinalDetalles(o.ListaDetalleRegistroRutas))
                        {
                            TicketVenta_E tcv = ticketD.obtenerTicket(dr.DocEntryTicket);
                            tcv.OpEnvio = o.PropietarioDesc;
                            ticketD.editarSeguimientoTicket("ENVIADO", dr.DocEntryTicket, tcv);
                        }
                    }

                    SqlCommand cmd2 = new SqlCommand("POST_TRANSACCIONES", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "ORRU");
                    cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@DocNum"].Value);
                    cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@DocEntry"].Value);
                    cmd2.ExecuteNonQuery();
                    tran.Commit();
                    cn.Close();
                }
                catch(Exception e1) { tran.Rollback();cn.Close(); throw new Exception("Error en creacion: " + e1.Message); }
            }catch(Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return status;
        }
        public int EditarOrdenDeRuta(OrdenRegistroRutas_E o)
        {
            int status = -1;
            foreach (OrdenRegistroRutas_E or in listaOrigenesDestinos())
            {
                if (o.AlmOrigenCod == or.AlmOrigenCod)
                {
                    o.AlmOrigenDesc2 = or.AlmOrigenDesc2;
                }
                if (o.AlmDestinoCod == or.AlmOrigenCod)
                {
                    o.AlmDestinoDesc2 = or.AlmOrigenDesc2;
                }
            }
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("MANT_ORRU", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento","UP");//UPDATE
                cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry).Direction = ParameterDirection.InputOutput;
                cmd.Parameters.AddWithValue("@DocNum", o.DocNum).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@TransCod",o.TransCod);
                cmd.Parameters.AddWithValue("@TransDesc",o.TransDesc);
                cmd.Parameters.AddWithValue("@PlacaCod",o.PlacaCod);
                cmd.Parameters.AddWithValue("@PlacaDesc",o.PlacaDesc);
                cmd.Parameters.AddWithValue("@CopilCod",o.CopilCod);
                cmd.Parameters.AddWithValue("@CopilDesc",o.CopilDesc);
                cmd.Parameters.AddWithValue("@Copil2Cod",o.Copil2Cod);
                cmd.Parameters.AddWithValue("@Copil2Desc",o.Copil2Desc);
                cmd.Parameters.AddWithValue("@Copil3Cod",o.Copil3Cod);
                cmd.Parameters.AddWithValue("@Copil3Desc",o.Copil3Desc);
                cmd.Parameters.AddWithValue("@Copil4Cod",o.Copil4Cod);
                cmd.Parameters.AddWithValue("@Copil4Desc",o.Copil4Desc);
                cmd.Parameters.AddWithValue("@AlmOrigenCod",o.AlmOrigenCod);
                cmd.Parameters.AddWithValue("@AlmOrigenDesc",o.AlmOrigenDesc);
                cmd.Parameters.AddWithValue("@AlmOrigenDesc2", o.AlmOrigenDesc2);
                cmd.Parameters.AddWithValue("@AlmDestinoCod",o.AlmDestinoCod);
                cmd.Parameters.AddWithValue("@AlmDestinoDesc",o.AlmDestinoDesc);
                cmd.Parameters.AddWithValue("@AlmDestinoDesc2", o.AlmDestinoDesc2);
                cmd.Parameters.AddWithValue("@HoraI",o.HoraI);
                cmd.Parameters.AddWithValue("@HoraT",o.HoraT);
                cmd.Parameters.AddWithValue("@Observaciones",o.Observaciones);
                cmd.ExecuteNonQuery();
                status = o.DocNum;
                cn.Close();
            }
            catch { cn.Close(); status = 0; }
            return status;
        }
        public int AnularOrdenDeRuta(int DocEntry)
        {
            int status = -1;
            OrdenRegistroRutas_E orru = obtenerOrdenDeRuta(DocEntry);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("trans1");
                try
                {
                    SqlCommand cmd = new SqlCommand("MANT_ORRU",cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UA");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
                    if(status>0)
                    {
                        foreach (DetalleRegistroRutas_E d in orru.ListaDetalleRegistroRutas)
                        {
                            ticketD.editarSeguimientoTicket("ANULARENVIADO", d.Ticket.DocEntry, d.Ticket);
                        }
                    }
                    tran.Commit();
                    cn.Close();
                }
                catch(Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error en ejecucion trans1: " + e.Message); }
                
            }catch(Exception e2) { cn.Close(); throw new Exception("Error en conexion de trans1: " + e2.Message); }
            return status;
        }
        //calculos 
        public int calcularTotalCajas(List<TicketVenta_E> l)
        {
            int TotalCajas = 0;
            foreach(TicketVenta_E t in l)
            {
                if (t.Cajas>0) { TotalCajas += t.Cajas; }
            }
            return TotalCajas;
        }
        
    }
}

