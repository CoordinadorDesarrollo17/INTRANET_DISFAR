using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.Ventas_ENT;
using System.Data;
using System.Data.SqlClient;
using Sap.Data.Hana;
using Capa_Entidad;
using Capa_Datos.Rutas_DAO;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Entidad.Rutas_ENT.ReportesSql;

namespace Capa_Datos.Ventas_DAO
{
    public class TicketVenta_D
    {
        Utilitarios uti = new Utilitarios();
        DBHelper db = new DBHelper();
        LibroSaldo_D lD = new LibroSaldo_D();
        //conexion  a hana
        public List<OCRD_E> listarClientes(string Fecha)
        {
            List<OCRD_E> lista = new List<OCRD_E>();
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand("SELECT DISTINCT \"CardCode\",\"CardName\" FROM " + uti.schemaHana + "ORDR WHERE \"DocDate\" " +
                    "BETWEEN ADD_DAYS('" + Fecha + "',0) AND '" + Fecha + "' ORDER BY \"CardName\"", hcn);
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    OCRD_E c = new OCRD_E();
                    c.CardCode = hdr.GetString(0);
                    c.CardName = hdr.GetString(1);
                    lista.Add(c);
                }
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        public List<string> listarDirDestinos(string CardCode)
        {
            List<string> lista = new List<string>();
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand("select \"Address\"||': '||ifnull(\"Street\",'')||' '||ifnull(\"Block\",'')" +
                    "||' '||ifnull(\"City\",'')||' '||ifnull(\"County\",'')as \"Dir\" from " + uti.schemaHana + "crd1 where \"CardCode\"='" + CardCode + "'" +
                    " and \"Address\" like 'ENV%'", hcn);
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    string c = "";
                    c = hdr.GetString(0);
                    lista.Add(c);
                }
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        public List<string> listarCorreosCliente(string CardCode)
        {
            List<string> lista = new List<string>();
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand("select ifnull(\"E_Mail\",'') from " + uti.schemaHana + "ocrd where \"CardCode\"='" + CardCode + "'", hcn);
                HanaDataReader hdr = hcmd.ExecuteReader();
                hdr.Read();
                string c = "";
                c = hdr.GetString(0);
                lista.Add(c);
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        public List<Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E> listarOrdenesdeVenta(string Fecha, string CardCode) //HANA
        {
            List<Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E> lista = new List<Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E>();
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                /*
                HanaCommand hcmd = new HanaCommand("SELECT T0.\"DocNum\",(select \"SlpName\" from " + uti.schemaHana + "oslp where \"SlpCode\" = T0.\"SlpCode\") " +
                        ",\"DocTotal\",(select \"Name\" from " + uti.schemaHana + "\"@COB_LUG_ENTREGA\" where \"Code\"=T0.\"U_COB_LUGAREN\") FROM " + uti.schemaHana + "ORDR T0 WHERE T0.\"DocDate\" = '" + Fecha + "' " +
                        "AND T0.\"CardCode\" = '" + CardCode + "' AND T0.\"CANCELED\"= 'N' ORDER BY T0.\"DocDate\",T0.\"CardCode\",T0.\"DocEntry\"", hcn);
                */
                HanaCommand hcmd = new HanaCommand("SELECT T0.\"DocNum\",(select \"SlpName\" from " + uti.schemaHana + "oslp where \"SlpCode\" = T0.\"SlpCode\") " +
                       " ,\"DocTotal\",(select \"Name\" from " + uti.schemaHana + "\"@COB_LUG_ENTREGA\" where \"Code\"=T0.\"U_COB_LUGAREN\") , T1.\"WhsCode\" " +
                       "FROM " + uti.schemaHana + "ORDR T0 inner join " + uti.schemaHana + 
                       "RDR1 T1 on T1.\"DocEntry\"= T0.\"DocEntry\" WHERE T0.\"DocDate\" = '" + Fecha + "' " +
                       "AND T0.\"CardCode\" = '" + CardCode + "' AND T0.\"CANCELED\"= 'N' GROUP BY T0.\"DocEntry\", " +
                       "T0.\"DocNum\", T0.\"SlpCode\", \"DocTotal\", T0.\"U_COB_LUGAREN\", T1.\"WhsCode\", T0.\"DocDate\", T0.\"CardCode\"" +
                       "ORDER BY T0.\"DocDate\",T0.\"CardCode\",T0.\"DocEntry\"", hcn);
                hcmd.CommandType = CommandType.Text;
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E o = new Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E();
                    o.DocNum = hdr.GetInt32(0);
                    o.CardCode = CardCode;
                    o.SlpName = hdr.GetString(1);
                    o.DocTotal = hdr.GetDecimal(2);
                    o.LugarDeEntrega = hdr.GetString(3);
                    o.AlmacenSalida = hdr.GetString(4);
                    lista.Add(o);
                }
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        public List<Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E> listarOrdenesdeVentaFinales(string Fecha, string CardCode)
        {
            List<Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E> lista = new List<Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                foreach (Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E o in listarOrdenesdeVenta(Fecha, CardCode))
                {
                    SqlCommand cmd = new SqlCommand("select T1.NroSap from " +
                        " ortv T0 inner join rtv1 T1 on T1.DocEntry = T0.DocEntry " +
                        " where T0.EstadoPedido<>'ANULADO' and NroSap=" + o.DocNum, cn);
                    int DocNum = Convert.ToInt32(cmd.ExecuteScalar());
                    if (DocNum != o.DocNum)
                    {
                        lista.Add(o);
                    }
                }
                cn.Close();
            } catch { cn.Close(); }
            return lista;
        }
        public string GuiasTicket(int DocEntry)
        {
            string Guias = "";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select NroSap from rtv1 where DocEntry=" + DocEntry, cn);
                SqlDataReader dr = cmd.ExecuteReader();
                string guiasTicket = "";
                while (dr.Read())
                {
                    int DocNumTicket = dr.GetInt32(0);
                    HanaConnection hcn = new HanaConnection(uti.cadHana);
                    try
                    {
                        hcn.Open();
                        HanaCommand hcmd = new HanaCommand(uti.schemaHana + "CBW_GUIASTICKET", hcn);
                        hcmd.CommandType = CommandType.StoredProcedure;
                        hcmd.Parameters.AddWithValue("DocNum", DocNumTicket);
                        HanaDataReader hdr = hcmd.ExecuteReader();
                        string guiasVenta = "";
                        while (hdr.Read())
                        {
                            guiasVenta += hdr.GetString(1) + "," + hdr.GetString(3);
                        }
                        guiasTicket += guiasVenta;
                        hdr.Close();
                        hcn.Close();
                    }
                    catch { hcn.Close(); }
                }
                Guias = guiasTicket;
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return Guias;
        }
        //conexion a sql
        public List<TicketVenta_E> listarTicketsVenta(Usuario_E user, TicketVenta_E t)
        {
            string query = "";
            string formatoFecha(string fecha)
            {
                string f = null;
                if (fecha != null)
                {
                    DateTime dt = Convert.ToDateTime(fecha);
                    f = dt.ToString("dd/MM/yyyy");
                }
                return f;
            }
            List<TicketVenta_E> lista = new List<TicketVenta_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                query = "select top 25 DocEntry,DocNum from ortv" +
                    " where FechaSeparo between dateadd(day,-1000,getdate()) and getdate() ";
                if (user.idRol == 7)
                {
                    query += " and UserId='" + user.id + "'";
                }
                if (t != null)
                {
                    if (t.DocNum > 0) { query += " and DocNum=" + t.DocNum; }
                    if (t.FechaTicket != null) { query += " and FechaTicket='" + t.FechaTicket + "'"; }
                    if (t.CardName != null) { query += " and CardName like '%" + t.CardName + "%'"; }
                    if (t.LugarDestino != null) { query += " and LugarDestino='" + t.LugarDestino + "'"; }
                    if (t.PropietarioDesc != null) { query += " and PropietarioDesc like '%" + t.PropietarioDesc + "%'"; }
                    if (t.MontoTotal > 0) { query += " and MontoTotal like '" + t.MontoTotal + "%'"; }
                    if (t.MontoFinal > 0) { query += " and MontoFinal like '" + t.MontoFinal + "%'"; }
                    if (t.EstadoPago != null) { query += " and EstadoPago='" + t.EstadoPago + "'"; }
                    if (t.EstadoPedido != null) { query += " and EstadoPedido='" + t.EstadoPedido + "'"; }
                    if (t.Flete == 0.01M) { query += " and Flete>0"; }
                    if (t.DescuentoNC == 0.01M) { query += " and DescuentoNC>0"; }
                    if (t.EstadoFacturacion != null) { query += " and EstadoFacturacion='" + t.EstadoFacturacion + "'"; }
                    if (t.Protocolo != null) {
                        if (t.Protocolo.Equals("TODOS")) { query += " and Protocolo is not null"; }
                        else { query += " and Protocolo='" + t.Protocolo + "'"; }
                    }
                    if (t.EstadoProtocolo != null) { query += " and EstadoProtocolo='" + t.EstadoProtocolo + "'"; }
                    if (t.TipoVenta != null) { query += " and TipoVenta ='" + t.TipoVenta + "'"; }
                    if (t.FirstLastNameScVt != null) { query += " and FirstLastNameScVt like '%" + t.FirstLastNameScVt + "%'"; }
                }
                query += " order by \"DocNum\" desc";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TicketVenta_E ticket = new TicketVenta_E();
                    ticket = obtenerTicket(dr.GetInt32(0));
                    ticket.PropietarioDesc = ticket.PropietarioDesc.Substring(0, 12);
                    ticket.FechaTicket = formatoFecha(ticket.FechaTicket);  //usando funcion local
                    ticket.FechaSacar = formatoFecha(ticket.FechaSacar);
                    ticket.FechaRecibir = formatoFecha(ticket.FechaRecibir);
                    lista.Add(ticket);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public List<TicketVenta_E> listarTicketsSeparados(string Id)
        {
            List<TicketVenta_E> lista = new List<TicketVenta_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select \"DocEntry\",\"DocNum\" from ortv where EstadoPedido='SEPARADO' and UserId='" + Id + "'", cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TicketVenta_E t = new TicketVenta_E();
                    t.DocEntry = dr.GetInt32(0);
                    t.DocNum = dr.GetInt32(1);
                    lista.Add(t);
                }
                dr.Close();
                cn.Close();
            } catch { cn.Close(); }
            return lista;
        }
        public TicketVenta_E separarTicket(Usuario_E u)
        {
            //tipo mantenimiento AS(add separo) transaccion tipo A(add)
            TicketVenta_E t = new TicketVenta_E();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "AS");
                    cmd.Parameters.AddWithValue("@DocEntry", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@PropietarioCod", u.CodigoSap);
                    cmd.Parameters.AddWithValue("@PropietarioDesc", u.nombre);
                    cmd.Parameters.AddWithValue("@UserId", u.id);
                    cmd.ExecuteNonQuery();
                    t.DocEntry = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    t.DocNum = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
                    t.EstadoPedido = "SEPARADO";
                    t.PropietarioDesc = u.nombre;
                    t.PropietarioCod = u.CodigoSap;

                    SqlCommand cmd2 = new SqlCommand("POST_TRANSACCIONES", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "ORTV");
                    cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@DocNum"].Value);
                    cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@DocEntry"].Value);
                    cmd2.ExecuteNonQuery();
                    tran.Commit();
                } catch (Exception e1) { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: " + e1.Message); }
                cn.Close();
            } catch (Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return t;
        }
        public int registrarTicket(TicketVenta_E ticket)
        {
            //tipo mantenimiento UR(update registro) transaccion tipo U(update)
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UR");
                    cmd.Parameters.AddWithValue("@DocEntry", ticket.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@DocNum", ticket.DocNum).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@FechaTicket", ticket.FechaTicket);
                    cmd.Parameters.AddWithValue("@CardCode", ticket.CardCode);
                    cmd.Parameters.AddWithValue("@CardName", ticket.CardName);
                    cmd.Parameters.AddWithValue("@TipoVenta", ticket.TipoVenta);
                    cmd.Parameters.AddWithValue("@LugarDestino", ticket.LugarDestino);
                    cmd.Parameters.AddWithValue("@Agencia", ticket.Agencia);
                    cmd.Parameters.AddWithValue("@DirDestino1", ticket.DirDestino1);
                    cmd.Parameters.AddWithValue("@DirDestino2", ticket.DirDestino2);
                    cmd.Parameters.AddWithValue("@NombrePer1", ticket.NombrePer1);
                    cmd.Parameters.AddWithValue("@TipoDocPer1", ticket.TipoDocPer1);
                    cmd.Parameters.AddWithValue("@DocPer1", ticket.DocPer1);
                    cmd.Parameters.AddWithValue("@TelfPer1", ticket.TelfPer1);
                    cmd.Parameters.AddWithValue("@NombrePer2", ticket.NombrePer2);
                    cmd.Parameters.AddWithValue("@TipoDocPer2", ticket.TipoDocPer2);
                    cmd.Parameters.AddWithValue("@DocPer2", ticket.DocPer2);
                    cmd.Parameters.AddWithValue("@TelfPer2", ticket.TelfPer2);
                    cmd.Parameters.AddWithValue("@Correo", ticket.Correo);
                    cmd.Parameters.AddWithValue("@Observaciones", ticket.Observaciones);
                    cmd.Parameters.AddWithValue("@Protocolo", ticket.Protocolo);
                    cmd.Parameters.AddWithValue("@Embalaje", ticket.Embalaje);
                    cmd.Parameters.AddWithValue("@Comentario", ticket.Comentario);
                    cmd.Parameters.AddWithValue("@MontoTotal", ticket.MontoTotal);
                    cmd.Parameters.AddWithValue("@Flete", ticket.Flete);
                    cmd.Parameters.AddWithValue("@GastoEnvio", ticket.GastoEnvio);
                    cmd.Parameters.AddWithValue("@DescuentoNC", ticket.DescuentoNC);
                    cmd.Parameters.AddWithValue("@DeudaCliente", ticket.DeudaCliente);
                    cmd.Parameters.AddWithValue("@DeudaEmpresa", ticket.DeudaEmpresa);
                    cmd.Parameters.AddWithValue("@MontoFinal", ticket.MontoFinal);
                    cmd.Parameters.AddWithValue("@NroVentas", DetTicketVenta_E.NroVentas(ticket.Det, ticket.DocEntry));
                    //agregado
                    cmd.Parameters.AddWithValue("@AlmacenSalida", DetTicketVenta_E.AlmacenSalidas(ticket.Det, ticket.DocEntry));
                    cmd.Parameters.AddWithValue("@OpSacar", ticket.OpSacar);
                    cmd.Parameters.AddWithValue("@Observaciones2", ticket.Observaciones2);
                    cmd.Parameters.AddWithValue("@FechaNC", ticket.FechaNC);
                    if (ticket.TiempoEntrega != null)
                    {
                        cmd.Parameters.AddWithValue("@TiempoEntrega", DateTime.Parse(ticket.TiempoEntrega));
                    }
                    cmd.Parameters.AddWithValue("@CardCodeScVt", ticket.CardCodeScVt);
                    cmd.Parameters.AddWithValue("@CntctCode", ticket.CntctCode);
                    cmd.Parameters.AddWithValue("@NameScVt", ticket.NameScVt);
                    cmd.Parameters.AddWithValue("@FirstLastNameScVt", ticket.FirstLastNameScVt);
                    cmd.Parameters.AddWithValue("@AddressScVt", ticket.AddressScVt);

                    SqlParameter tbDet = new SqlParameter("@TPRTV1", SqlDbType.Structured);
                    tbDet.Value = DetTicketVenta_E.tbDetalle(ticket.Det, ticket.DocEntry);
                    tbDet.TypeName = "dbo.TPRTV1";
                    cmd.Parameters.AddWithValue("@TPRTV1", tbDet.Value);
                    // datos de notas de credito
                    if (ticket.Det2 != null && ticket.Det2.Count > 0)
                    {
                        SqlParameter tbDet2 = new SqlParameter("@TPRTV2", SqlDbType.Structured);
                        tbDet2.Value = DetTicketVenta2_E.tbDetalle(ticket.Det2, ticket);
                        tbDet2.TypeName = "dbo.TPRTV2";
                        cmd.Parameters.AddWithValue("@TPRTV2", tbDet2.Value);
                    }
                    //regalos
                    cmd.Parameters.AddWithValue("@IdReg", ticket.IdReg);
                    cmd.Parameters.AddWithValue("@RegCate", ticket.RegCate);
                    cmd.Parameters.AddWithValue("@RegTipo", ticket.RegTipo);
                    cmd.Parameters.AddWithValue("@RegCant", ticket.RegCant);
                    cmd.ExecuteNonQuery();
                    status = ticket.DocNum;
                    if (ticket.CardCode != null && ticket.CardName != null)
                    {
                        SqlCommand cmd2 = new SqlCommand("MANT_OLDS", cn);
                        cmd2.Transaction = tran;
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@TipoMantenimiento", "AC");
                        cmd2.Parameters.AddWithValue("@C_CardCode", ticket.CardCode);
                        cmd2.Parameters.AddWithValue("@C_CardName", ticket.CardName);
                        cmd2.ExecuteNonQuery();
                        if (ticket.DeudaCliente > 0)
                        {
                            SqlCommand cmd4 = new SqlCommand("MANT_OLDS", cn);
                            cmd4.Transaction = tran;
                            cmd4.CommandType = CommandType.StoredProcedure;
                            cmd4.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd4.Parameters.AddWithValue("@C_CardCode", ticket.CardCode);
                            cmd4.Parameters.AddWithValue("@FechaOpe", ticket.FechaTicket);
                            cmd4.Parameters.AddWithValue("@Operacion", "VENTA");
                            cmd4.Parameters.AddWithValue("@DetOpe", "VENTA DeudaCliente, ticket:" + ticket.DocNum + " MR:" + ticket.MontoFinal);
                            cmd4.Parameters.AddWithValue("@Ingreso", ticket.DeudaCliente);
                            cmd4.Parameters.AddWithValue("@OperarioReg", ticket.OpSacar);
                            cmd4.ExecuteNonQuery();
                        }
                        if (ticket.DeudaEmpresa > 0)
                        {
                            SqlCommand cmd5 = new SqlCommand("MANT_OLDS", cn);
                            cmd5.Transaction = tran;
                            cmd5.CommandType = CommandType.StoredProcedure;
                            cmd5.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd5.Parameters.AddWithValue("@C_CardCode", ticket.CardCode);
                            cmd5.Parameters.AddWithValue("@FechaOpe", ticket.FechaTicket);
                            cmd5.Parameters.AddWithValue("@Operacion", "VENTA");
                            cmd5.Parameters.AddWithValue("@DetOpe", "SALIDASALDO DeudaEmpresa, ticket:" + ticket.DocNum + " MR:" + ticket.MontoFinal);
                            cmd5.Parameters.AddWithValue("@Egreso", ticket.DeudaEmpresa);
                            cmd5.Parameters.AddWithValue("@OperarioReg", ticket.OpSacar);
                            cmd5.ExecuteNonQuery();
                        }
                    }
                    //regalos
                    if(ticket.IdReg>0)
                    {
                        TablasSql.OREG_D oregD = new TablasSql.OREG_D();
                        oregD.compromisosStock(ticket, cn, tran);
                    }                
                    tran.Commit();
                }
                catch (Exception e1) { status = 0; tran.Rollback(); cn.Close(); throw new Exception("Error en registro: " + e1.Message); }
                cn.Close();
            }
            catch (Exception e2) { status = 0; cn.Close(); throw new Exception("Error en registro y conexion: " + e2.Message); }
            return status;
        }
        public TicketVenta_E obtenerTicket(int DocEntry)
        {
            TicketVenta_E t = new TicketVenta_E() { Det = new List<DetTicketVenta_E>(), Det2 = new List<DetTicketVenta2_E>() };
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select * from ortv where DocEntry=" + DocEntry, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                t.DocEntry = dr.GetInt32(0);
                t.DocNum = dr.GetInt32(1);
                if (!dr.IsDBNull(2)) { t.FechaTicket = dr.GetDateTime(2).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(3)) { t.CardCode = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { t.CardName = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { t.HoraTicket = dr.GetTimeSpan(5).ToString(); }
                if (!dr.IsDBNull(6)) { t.TipoVenta = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { t.LugarDestino = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { t.Agencia = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { t.DirDestino1 = dr.GetString(9); }
                if (!dr.IsDBNull(10)) { t.DirDestino2 = dr.GetString(10); }
                if (!dr.IsDBNull(11)) { t.NombrePer1 = dr.GetString(11); }
                if (!dr.IsDBNull(12)) { t.TelfPer1 = dr.GetString(12); }
                if (!dr.IsDBNull(13)) { t.NombrePer2 = dr.GetString(13); }
                if (!dr.IsDBNull(14)) { t.TelfPer2 = dr.GetString(14); }
                if (!dr.IsDBNull(15)) { t.Correo = dr.GetString(15); }
                if (!dr.IsDBNull(16)) { t.Observaciones = dr.GetString(16); }
                if (!dr.IsDBNull(17)) { t.Protocolo = dr.GetString(17); }
                if (!dr.IsDBNull(18)) { t.Embalaje = dr.GetString(18); }
                if (!dr.IsDBNull(19)) { t.PropietarioCod = dr.GetInt32(19); }
                if (!dr.IsDBNull(20)) { t.PropietarioDesc = dr.GetString(20); }
                if (!dr.IsDBNull(21)) { t.Comentario = dr.GetString(21); }
                if (!dr.IsDBNull(22)) { t.MontoTotal = dr.GetDecimal(22); }
                if (!dr.IsDBNull(23)) { t.Flete = dr.GetDecimal(23); }
                if (!dr.IsDBNull(24)) { t.GastoEnvio = dr.GetDecimal(24); }
                if (!dr.IsDBNull(25)) { t.DescuentoNC = dr.GetDecimal(25); }
                if (!dr.IsDBNull(26)) { t.DeudaCliente = dr.GetDecimal(26); }
                if (!dr.IsDBNull(27)) { t.DeudaEmpresa = dr.GetDecimal(27); }
                if (!dr.IsDBNull(28)) { t.MontoFinal = dr.GetDecimal(28); }
                if (!dr.IsDBNull(29)) { t.NroVentas = dr.GetString(29); }
                if (!dr.IsDBNull(30)) { t.FormaPago = dr.GetString(30); }
                if (!dr.IsDBNull(31)) { t.MontoRecibido = dr.GetDecimal(31); }
                if (!dr.IsDBNull(32)) { t.EstadoPago = dr.GetString(32); }
                if (!dr.IsDBNull(33)) { t.FechaPago = dr.GetDateTime(33).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(34)) { t.HoraPago = dr.GetTimeSpan(34).ToString(); }
                if (!dr.IsDBNull(35)) { t.CajeroCod = dr.GetInt32(35); }
                if (!dr.IsDBNull(36)) { t.CajeroDesc = dr.GetString(36); }
                t.EstadoPedido = dr.GetString(37);
                if (!dr.IsDBNull(38)) { t.FechaSeparo = dr.GetDateTime(38).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(39)) { t.HoraSeparo = dr.GetTimeSpan(39).ToString(); }
                if (!dr.IsDBNull(40)) { t.OpSeparo = dr.GetString(40); }
                if (!dr.IsDBNull(41)) { t.FechaSacar = dr.GetDateTime(41).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(42)) { t.HoraSacar = dr.GetTimeSpan(42).ToString(); }
                if (!dr.IsDBNull(43)) { t.OpSacar = dr.GetString(43); }
                if (!dr.IsDBNull(44)) { t.FechaRecibir = dr.GetDateTime(44).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(45)) { t.HoraRecibir = dr.GetTimeSpan(45).ToString(); }
                if (!dr.IsDBNull(46)) { t.OpRecibir = dr.GetString(46); }
                if (!dr.IsDBNull(47)) { t.FechaSacando = dr.GetDateTime(47).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(48)) { t.HoraSacando = dr.GetTimeSpan(48).ToString(); }
                if (!dr.IsDBNull(49)) { t.OpSacando = dr.GetString(49); }
                if (!dr.IsDBNull(50)) { t.FechaEmpacado = dr.GetDateTime(50).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(51)) { t.HoraEmpacado = dr.GetTimeSpan(51).ToString(); }
                if (!dr.IsDBNull(52)) { t.OpEmpacado = dr.GetString(52); }
                if (!dr.IsDBNull(53)) { t.FechaEnvio = dr.GetDateTime(53).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(54)) { t.HoraEnvio = dr.GetTimeSpan(54).ToString(); }
                if (!dr.IsDBNull(55)) { t.OpEnvio = dr.GetString(55); }
                if (!dr.IsDBNull(56)) { t.FechaEntrega = dr.GetDateTime(56).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(57)) { t.HoraEntrega = dr.GetTimeSpan(57).ToString(); }
                if (!dr.IsDBNull(58)) { t.OpEntrega = dr.GetString(58); }
                if (!dr.IsDBNull(59)) { t.FechaAnulacion = dr.GetDateTime(59).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(60)) { t.HoraAnulacion = dr.GetTimeSpan(60).ToString(); }
                if (!dr.IsDBNull(61)) { t.OpAnulacion = dr.GetString(61); }
                if (!dr.IsDBNull(62)) { t.UserId = dr.GetString(62); }
                if (!dr.IsDBNull(63)) { t.Cajas = dr.GetInt32(63); }
                if (!dr.IsDBNull(64)) { t.Observaciones2 = dr.GetString(64); }
                if (!dr.IsDBNull(65)) { t.FechaNC = dr.GetDateTime(65).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(66)) { t.EstadoFacturacion = dr.GetString(66); }
                if (!dr.IsDBNull(67)) { t.FechaFacturacion = dr.GetDateTime(67).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(68)) { t.HoraFacturacion = dr.GetTimeSpan(68).ToString(); }
                if (!dr.IsDBNull(69)) { t.OpFacturacion = dr.GetString(69); }
                if (!dr.IsDBNull(70)) { t.EstadoProtocolo = dr.GetString(70); }
                if (!dr.IsDBNull(71)) { t.FechaProtocolo = dr.GetDateTime(71).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(72)) { t.HoraProtocolo = dr.GetTimeSpan(72).ToString(); }
                if (!dr.IsDBNull(73)) { t.OpProtocolo = dr.GetString(73); }
                if (!dr.IsDBNull(74)) { t.TiempoEntrega = dr.GetDateTime(74).ToString("yyyy-MM-ddTHH:mm:ss"); }
                if (!dr.IsDBNull(75)) { t.NroMesa = dr.GetInt32(75); }
                if (!dr.IsDBNull(76)) { t.TipoDocPer1 = dr.GetString(76); }
                if (!dr.IsDBNull(77)) { t.DocPer1 = dr.GetString(77); }
                if (!dr.IsDBNull(78)) { t.TipoDocPer2 = dr.GetString(78); }
                if (!dr.IsDBNull(79)) { t.DocPer2 = dr.GetString(79); }
                if (!dr.IsDBNull(80)) { t.OpSacando2 = dr.GetString(80); }
                if (!dr.IsDBNull(81)) { t.OpChequeador = dr.GetString(81); }
                if (!dr.IsDBNull(82)) { t.OpEmpacado2 = dr.GetString(82); }
                if (!dr.IsDBNull(83)) { t.CardCodeScVt = dr.GetString(83); }
                if (!dr.IsDBNull(84)) { t.CntctCode = dr.GetInt32(84); }
                if (!dr.IsDBNull(85)) { t.NameScVt = dr.GetString(85); }
                if (!dr.IsDBNull(86)) { t.FirstLastNameScVt = dr.GetString(86); }
                if (!dr.IsDBNull(87)) { t.AddressScVt = dr.GetString(87); }
                if (!dr.IsDBNull(88)) { t.IdReg = dr.GetInt32(88); }
                if (!dr.IsDBNull(89)) { t.RegTipo = dr.GetString(89); }
                if (!dr.IsDBNull(90)) { t.RegCate = dr.GetString(90); }
                if (!dr.IsDBNull(91)) { t.RegCant = dr.GetDecimal(91); }
                if (!dr.IsDBNull(92)) { t.RegEstado = dr.GetString(92); }
                try
                {
                    t.Guias = GuiasTicket(DocEntry);
                } catch { }
                dr.Close();

                SqlCommand cmd2 = new SqlCommand("select * from rtv1 where DocEntry=" + DocEntry + " order by Linea", cn);
                cmd2.CommandType = CommandType.Text;
                SqlDataReader dr2 = cmd2.ExecuteReader();
                if (dr2.HasRows)
                {
                    t.Det = new List<DetTicketVenta_E>();
                    while (dr2.Read())
                    {
                        DetTicketVenta_E d = new DetTicketVenta_E();
                        d.DocEntry = dr2.GetInt32(0);
                        d.Linea = dr2.GetInt32(1);
                        if (!dr2.IsDBNull(2)) { d.Monto = dr2.GetDecimal(2); }
                        if (!dr2.IsDBNull(3)) { d.NroSap = dr2.GetInt32(3); }
                        if (!dr2.IsDBNull(4)) { d.TipoComprobante = dr2.GetString(4); }
                        if (!dr2.IsDBNull(5)) { d.Vendedor = dr2.GetString(5); }
                        if (!dr2.IsDBNull(6)) { d.LugarDeEntrega = dr2.GetString(6); }
                        if (!dr2.IsDBNull(7)) { d.Observaciones = dr2.GetString(7); }
                        if (!dr2.IsDBNull(8)) { d.AlmacenSalida = dr2.GetString(8); }
                        t.Det.Add(d);
                    }
                }
                dr2.Close();
                cn.Close();

                t.Det2 = obtenerDet2Ticket(DocEntry);
            }
            catch { cn.Close(); }
            return t;
        }
        public int editarTicket(int DocEntry, TicketVenta_E ticket)
        {
            // tipo UU updateupdate actuliazacion
            int status = -1;
            TicketVenta_E auxTK = obtenerTicket(DocEntry);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("MANT_ORTV", cn,tran);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UU");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@TipoVenta", ticket.TipoVenta);
                    cmd.Parameters.AddWithValue("@LugarDestino", ticket.LugarDestino);
                    cmd.Parameters.AddWithValue("@Agencia", ticket.Agencia);
                    cmd.Parameters.AddWithValue("@DirDestino1", ticket.DirDestino1);
                    cmd.Parameters.AddWithValue("@DirDestino2", ticket.DirDestino2);
                    cmd.Parameters.AddWithValue("@NombrePer1", ticket.NombrePer1);
                    cmd.Parameters.AddWithValue("@TipoDocPer1", ticket.TipoDocPer1);
                    cmd.Parameters.AddWithValue("@DocPer1", ticket.DocPer1);
                    cmd.Parameters.AddWithValue("@TelfPer1", ticket.TelfPer1);
                    cmd.Parameters.AddWithValue("@NombrePer2", ticket.NombrePer2);
                    cmd.Parameters.AddWithValue("@TipoDocPer2", ticket.TipoDocPer2);
                    cmd.Parameters.AddWithValue("@DocPer2", ticket.DocPer2);
                    cmd.Parameters.AddWithValue("@TelfPer2", ticket.TelfPer2);
                    cmd.Parameters.AddWithValue("@Correo", ticket.Correo);
                    cmd.Parameters.AddWithValue("@Observaciones", ticket.Observaciones);
                    cmd.Parameters.AddWithValue("@Protocolo", ticket.Protocolo);
                    cmd.Parameters.AddWithValue("@Embalaje", ticket.Embalaje);
                    cmd.Parameters.AddWithValue("@Comentario", ticket.Comentario);
                    cmd.Parameters.AddWithValue("@Flete", ticket.Flete);
                    cmd.Parameters.AddWithValue("@GastoEnvio", ticket.GastoEnvio);
                    cmd.Parameters.AddWithValue("@DescuentoNC", ticket.DescuentoNC);
                    cmd.Parameters.AddWithValue("@DeudaCliente", ticket.DeudaCliente);
                    cmd.Parameters.AddWithValue("@DeudaEmpresa", ticket.DeudaEmpresa);
                    cmd.Parameters.AddWithValue("@MontoFinal", ticket.MontoFinal);
                    cmd.Parameters.AddWithValue("@Observaciones2", ticket.Observaciones2);
                    cmd.Parameters.AddWithValue("@FechaNC", ticket.FechaNC);
                    cmd.Parameters.AddWithValue("@TiempoEntrega", DateTime.Parse(ticket.TiempoEntrega));
                    cmd.Parameters.AddWithValue("@CardCodeScVt", ticket.CardCodeScVt);
                    cmd.Parameters.AddWithValue("@CntctCode", ticket.CntctCode);
                    cmd.Parameters.AddWithValue("@NameScVt", ticket.NameScVt);
                    cmd.Parameters.AddWithValue("@FirstLastNameScVt", ticket.FirstLastNameScVt);
                    cmd.Parameters.AddWithValue("@AddressScVt", ticket.AddressScVt);
                    cmd.Parameters.AddWithValue("@IdReg", ticket.IdReg);
                    cmd.Parameters.AddWithValue("@RegCate", ticket.RegCate);
                    cmd.Parameters.AddWithValue("@RegTipo", ticket.RegTipo);
                    cmd.Parameters.AddWithValue("@RegCant", ticket.RegCant);

                    SqlParameter tbDet = new SqlParameter("@TPRTV1", SqlDbType.Structured);
                    tbDet.Value = DetTicketVenta_E.tbDetalle(ticket.Det, ticket.DocEntry);
                    tbDet.TypeName = "dbo.TPRTV1";
                    cmd.Parameters.AddWithValue("@TPRTV1", tbDet.Value);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
                    //regalos
                    if (auxTK.IdReg > 0)
                    {
                        TablasSql.OREG_D oregD = new TablasSql.OREG_D();
                        auxTK.RegCant = -1 * auxTK.RegCant;
                        oregD.compromisosStock(auxTK, cn, tran);
                    }
                    if (ticket.IdReg > 0)
                    {
                        TablasSql.OREG_D oregD = new TablasSql.OREG_D();
                        oregD.compromisosStock(ticket, cn, tran);
                    }
                    tran.Commit();
                    cn.Close();
                } catch (Exception e) { status = 0; tran.Rollback(); cn.Close(); throw new Exception("Error en edicion: " + e.Message); }
            }
            catch (Exception e2) { status = 0; cn.Close(); throw new Exception("Error en edicion y conexion: " + e2.Message); }
            return status;
        }
        public int pagarTicket(int DocEntry, TicketVenta_E ticket)
        {   // tipo UPG update pago
            int status = -1;
            TicketVenta_E auxTK = obtenerTicket(DocEntry);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UPG");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@FormaPago", ticket.FormaPago);
                    cmd.Parameters.AddWithValue("@MontoRecibido", ticket.MontoRecibido);
                    cmd.Parameters.AddWithValue("@CajeroCod", ticket.CajeroCod);
                    cmd.Parameters.AddWithValue("@CajeroDesc", ticket.CajeroDesc);
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());

                    if (auxTK.CardCode != null && auxTK.CardName != null)
                    {
                        SqlCommand cmd2 = new SqlCommand("MANT_OLDS", cn);
                        cmd2.Transaction = tran;
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@TipoMantenimiento", "AC");
                        cmd2.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                        cmd2.Parameters.AddWithValue("@C_CardName", auxTK.CardName);
                        cmd2.ExecuteNonQuery();

                        if (auxTK.GastoEnvio > 0)
                        {
                            SqlCommand cmd3 = new SqlCommand("MANT_OLDS", cn);
                            cmd3.Transaction = tran;
                            cmd3.CommandType = CommandType.StoredProcedure;
                            cmd3.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd3.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd3.Parameters.AddWithValue("@FechaOpe", auxTK.FechaTicket);
                            cmd3.Parameters.AddWithValue("@Operacion", ticket.FormaPago);
                            cmd3.Parameters.AddWithValue("@DetOpe", ticket.FormaPago + " GastoEnvio, ticket:" + auxTK.DocNum + " MR:" + ticket.MontoRecibido);
                            cmd3.Parameters.AddWithValue("@Ingreso", auxTK.GastoEnvio);
                            cmd3.Parameters.AddWithValue("@OperarioReg", ticket.CajeroDesc);
                            cmd3.ExecuteNonQuery();
                        }
                    }
                    tran.Commit();
                    cn.Close();
                }
                catch { status = 0; tran.Rollback(); cn.Close(); }
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en pago: " + e.Message); }
            return status;
        }
        public int anularPagoTicket(int DocEntry)
        {   //TIPO MANT UAPG UPDATE ANULAR PAGO
            int status = -1;
            TicketVenta_E auxTK = obtenerTicket(DocEntry);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UAPG");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());

                    if (auxTK.CardCode != null && auxTK.CardName != null)
                    {
                        SqlCommand cmd2 = new SqlCommand("MANT_OLDS", cn);
                        cmd2.Transaction = tran;
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@TipoMantenimiento", "AC");
                        cmd2.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                        cmd2.Parameters.AddWithValue("@C_CardName", auxTK.CardName);
                        cmd2.ExecuteNonQuery();

                        if (auxTK.GastoEnvio > 0)
                        {
                            SqlCommand cmd3 = new SqlCommand("MANT_OLDS", cn);
                            cmd3.Transaction = tran;
                            cmd3.CommandType = CommandType.StoredProcedure;
                            cmd3.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd3.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd3.Parameters.AddWithValue("@FechaOpe", auxTK.FechaPago);
                            cmd3.Parameters.AddWithValue("@Operacion", "ANULACIONPAGO");
                            cmd3.Parameters.AddWithValue("@DetOpe", "ANULACIONPAGO GastoEnvio, ticket:" + auxTK.DocNum + " MR:" + auxTK.MontoRecibido);
                            cmd3.Parameters.AddWithValue("@Egreso", auxTK.GastoEnvio);
                            cmd3.Parameters.AddWithValue("@OperarioReg", auxTK.CajeroDesc);
                            cmd3.ExecuteNonQuery();
                        }
                    }
                    tran.Commit();
                    cn.Close();
                }
                catch { status = 0; tran.Rollback(); cn.Close(); }
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error de anularpago: " + e.Message); }
            return status;
        }
        public List<Rpt_TicketVenta_E> listarTicketsAgencia()
        {
            List<Rpt_TicketVenta_E> lista = new List<Rpt_TicketVenta_E>();
            //SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                //cn.Open();// SqlTransaction tran = cn.BeginTransaction("transaccion1");
                SqlDataReader dr = db.ExecuteReader("RptFormatoAgencia");
                while (dr.Read())
                    {
                        Rpt_TicketVenta_E t = new Rpt_TicketVenta_E();
                        if (!dr.IsDBNull(0)) {t.Almacen = dr.GetString(0); }
                        if (!dr.IsDBNull(1)) { t.GastoEnvio = dr.GetDecimal(1); }
                        if (!dr.IsDBNull(2)) { t.DocEntry = dr.GetInt32(2); }
                        if (!dr.IsDBNull(3)) { t.Observaciones2 = dr.GetString(3); }
                        if (!dr.IsDBNull(4)) { t.CardName = dr.GetString(4); }
                        if (!dr.IsDBNull(5)) { t.NombrePer1 = dr.GetString(5); }
                        if (!dr.IsDBNull(6)) { t.DocPer1 = dr.GetString(6); }
                        if (!dr.IsDBNull(7)) { t.TelfPer1 = dr.GetString(7); }
                        if (!dr.IsDBNull(8)) { t.PropietarioDesc = dr.GetString(8); }
                        if (!dr.IsDBNull(9)) { t.Agencia = dr.GetString(9); }
                        if (!dr.IsDBNull(10)) { t.DirDestino1 = dr.GetString(10); }
                        if (!dr.IsDBNull(11)) { t.DirDestino2 = dr.GetString(11); }
                        if (!dr.IsDBNull(12)) { t.Cajas = dr.GetInt32(12); }
                        
                    lista.Add(t);
                    }
                    dr.Close();
                    
                }
                catch (Exception e){ }

            return lista;
            }
        public int enviarTicketAgencia(int DocEntry, TicketVenta_E ticket)
        {

            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("MANT_ORTV", cn, tran);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "ETA");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", ticket.DocNum).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@OpEnvio", ticket.OpEnvio);
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());

                    tran.Commit();
                    cn.Close();
                }
                catch (Exception e) { status = 0; tran.Rollback(); cn.Close(); throw new Exception("Error en edicion: " + e.Message); }
            }
            catch (Exception e2) { status = 0; cn.Close(); throw new Exception("Error en edicion y conexion: " + e2.Message); }
            return status;
        }
        public int anularTicket(int DocEntry, TicketVenta_E ticket){   //tipo mant UAN
            int status = -1;
            TicketVenta_E auxTK = obtenerTicket(DocEntry);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UAN");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@OpAnulacion", ticket.OpAnulacion);
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());

                    if (auxTK.CardCode != null && auxTK.CardName != null)
                    {
                        SqlCommand cmd2 = new SqlCommand("MANT_OLDS", cn);
                        cmd2.Transaction = tran;
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@TipoMantenimiento", "AC");
                        cmd2.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                        cmd2.Parameters.AddWithValue("@C_CardName", auxTK.CardName);
                        cmd2.ExecuteNonQuery();
                        if (auxTK.DeudaCliente > 0)
                        {
                            SqlCommand cmd4 = new SqlCommand("MANT_OLDS", cn);
                            cmd4.Transaction = tran;
                            cmd4.CommandType = CommandType.StoredProcedure;
                            cmd4.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd4.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd4.Parameters.AddWithValue("@FechaOpe", auxTK.FechaTicket);
                            cmd4.Parameters.AddWithValue("@Operacion", "ANULACIONVENTA");
                            cmd4.Parameters.AddWithValue("@DetOpe", "ANULACIONVENTA DeudaCliente, ticket:" + auxTK.DocNum + " MR:" + auxTK.MontoFinal);
                            cmd4.Parameters.AddWithValue("@Egreso", auxTK.DeudaCliente);
                            cmd4.Parameters.AddWithValue("@OperarioReg", ticket.OpAnulacion);
                            cmd4.ExecuteNonQuery();
                        }
                        if (auxTK.DeudaEmpresa > 0)
                        {
                            SqlCommand cmd5 = new SqlCommand("MANT_OLDS", cn);
                            cmd5.Transaction = tran;
                            cmd5.CommandType = CommandType.StoredProcedure;
                            cmd5.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd5.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd5.Parameters.AddWithValue("@FechaOpe", auxTK.FechaTicket);
                            cmd5.Parameters.AddWithValue("@Operacion", "ANULACIONVENTA");
                            cmd5.Parameters.AddWithValue("@DetOpe", "ANULACION-SALIDASALDO DeudaEmpresa, ticket:" + auxTK.DocNum + " MR:" + auxTK.MontoFinal);
                            cmd5.Parameters.AddWithValue("@Ingreso", auxTK.DeudaEmpresa);
                            cmd5.Parameters.AddWithValue("@OperarioReg", ticket.OpAnulacion);
                            cmd5.ExecuteNonQuery();
                        }
                    }
                    //regalos
                    if (auxTK.IdReg > 0 && auxTK.RegCant>0)
                    {
                        TablasSql.OREG_D oregD = new TablasSql.OREG_D();
                        auxTK.RegCant = -1 * auxTK.RegCant;
                        oregD.compromisosStock(auxTK, cn, tran);
                    }
                    tran.Commit();
                    cn.Close();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("error y anulacion"); }
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en anulacion: " + e.Message); }
            return status;
        }
        public int cancelarTicket(int DocEntry, TicketVenta_E ticket)
        {   //tipo mant UAN
            int status = -1;
            TicketVenta_E auxTK = obtenerTicket(DocEntry);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UCA");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@OpAnulacion", ticket.OpAnulacion);
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());

                    if (auxTK.CardCode != null && auxTK.CardName != null)
                    {
                        SqlCommand cmd2 = new SqlCommand("MANT_OLDS", cn);
                        cmd2.Transaction = tran;
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@TipoMantenimiento", "AC");
                        cmd2.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                        cmd2.Parameters.AddWithValue("@C_CardName", auxTK.CardName);
                        cmd2.ExecuteNonQuery();
                        if (auxTK.DeudaCliente > 0)
                        {
                            SqlCommand cmd4 = new SqlCommand("MANT_OLDS", cn);
                            cmd4.Transaction = tran;
                            cmd4.CommandType = CommandType.StoredProcedure;
                            cmd4.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd4.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd4.Parameters.AddWithValue("@FechaOpe", auxTK.FechaTicket);
                            cmd4.Parameters.AddWithValue("@Operacion", "ANULACIONVENTA");
                            cmd4.Parameters.AddWithValue("@DetOpe", "ANULACIONVENTA DeudaCliente, ticket:" + auxTK.DocNum + " MR:" + auxTK.MontoFinal);
                            cmd4.Parameters.AddWithValue("@Egreso", auxTK.DeudaCliente);
                            cmd4.Parameters.AddWithValue("@OperarioReg", ticket.OpAnulacion);
                            cmd4.ExecuteNonQuery();
                        }
                        if (auxTK.DeudaEmpresa > 0)
                        {
                            SqlCommand cmd5 = new SqlCommand("MANT_OLDS", cn);
                            cmd5.Transaction = tran;
                            cmd5.CommandType = CommandType.StoredProcedure;
                            cmd5.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd5.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd5.Parameters.AddWithValue("@FechaOpe", auxTK.FechaTicket);
                            cmd5.Parameters.AddWithValue("@Operacion", "ANULACIONVENTA");
                            cmd5.Parameters.AddWithValue("@DetOpe", "ANULACION-SALIDASALDO DeudaEmpresa, ticket:" + auxTK.DocNum + " MR:" + auxTK.MontoFinal);
                            cmd5.Parameters.AddWithValue("@Ingreso", auxTK.DeudaEmpresa);
                            cmd5.Parameters.AddWithValue("@OperarioReg", ticket.OpAnulacion);
                            cmd5.ExecuteNonQuery();
                        }
                    }
                    //regalos
                    if (auxTK.IdReg > 0 && auxTK.RegCant > 0)
                    {
                        TablasSql.OREG_D oregD = new TablasSql.OREG_D();
                        auxTK.RegCant = -1 * auxTK.RegCant;
                        oregD.compromisosStock(auxTK, cn, tran);
                    }
                    tran.Commit();
                    cn.Close();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("error y anulacion"); }
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en anulacion: " + e.Message); }
            return status;
        }
        public int editarSeguimientoTicket(string Estado, int DocEntry, TicketVenta_E t)
        {
            int status = -1;
            string paramOp = "", op = "", TipoMantenimiento = "";
            TicketVenta_E auxTK = obtenerTicket(DocEntry);
            if (Estado.Equals("RECIBIDO"))
            { paramOp = "@OpRecibir"; op = t.OpRecibir; TipoMantenimiento = "USRE"; }
            else if (Estado.Equals("ANULARRECIBIDO"))
            { paramOp = "@OpRecibir"; op = ""; TipoMantenimiento = "USAR"; }
            else if (Estado.Equals("SACANDO"))
            { paramOp = "@OpSacando"; op = t.OpSacando; TipoMantenimiento = "USSA";
                t.OpSacando2 = t.sacador2 + ", " + t.sacador3 + ", " + t.sacador4;
            }
            else if (Estado.Equals("ANULARSACANDO"))
            { paramOp = "@OpSacando"; op = ""; TipoMantenimiento = "USAS"; }
            else if (Estado.Equals("EMPACADO"))
            { paramOp = "@OpEmpacado"; op = t.OpEmpacado; TipoMantenimiento = "USEM"; 
                t.OpChequeador = t.chequeador1+ ", " + t.chequeador2 + ", " + t.chequeador3;
                t.OpEmpacado2 = t.encajador2 + ", " + t.encajador3 ;
            }
            else if (Estado.Equals("ANULAREMPACADO"))
            { paramOp = "@OpEmpacado"; op = ""; TipoMantenimiento = "USAE"; }
            else if (Estado.Equals("ENVIADO"))
            { paramOp = "@OpEnvio"; op = t.OpEnvio; TipoMantenimiento = "USEN"; }
            else if (Estado.Equals("ANULARENVIADO"))
            { paramOp = "@OpEnvio"; op = ""; TipoMantenimiento = "USAN"; }
            else if (Estado.Equals("ENTREGADO"))
            { paramOp = "@OpEntrega"; op = t.OpEntrega; TipoMantenimiento = "USET"; }
            else if (Estado.Equals("ANULARENTREGADO"))
            { paramOp = "@OpEntrega"; op = ""; TipoMantenimiento = "USAT"; }
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1"); ;
                try
                {
                    SqlCommand cmd = new SqlCommand("MANT_ORTV", cn, tran);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", TipoMantenimiento);
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@EstadoPedido", t.EstadoPedido);
                    cmd.Parameters.AddWithValue(paramOp, op);
                    if (TipoMantenimiento.Equals("USEM"))
                    {
                        cmd.Parameters.AddWithValue("@Cajas", t.Cajas);
                        cmd.Parameters.AddWithValue("@NroMesa", t.NroMesa);
                    }
                    if (t.OpSacando2 != null) { cmd.Parameters.AddWithValue("@OpSacando2", t.OpSacando2); }
                    if (t.OpChequeador != null) { cmd.Parameters.AddWithValue("@OpChequeador", t.OpChequeador); }
                    if (t.OpEmpacado2 != null) { cmd.Parameters.AddWithValue("@OpEmpacado2", t.OpEmpacado2); }
                    cmd.ExecuteNonQuery();
                    // aqui va lo de regalos
                    if (TipoMantenimiento.Equals("USET") && auxTK.IdReg>0 && auxTK.RegCant>0)
                    {
                        
                        TablasSql.OREG_D oregD = new TablasSql.OREG_D();
                        auxTK.RegCant = -1 * auxTK.RegCant;
                        oregD.compromisosStock(auxTK, cn, tran);
                        oregD.registrarGestionStock(new Capa_Entidad.Ventas_ENT.TablasSql.OREG_E() { Id=auxTK.IdReg,StockDisp=auxTK.RegCant}
                        , new Capa_Entidad.Ventas_ENT.TablasSql.OTRC_E() {IdReg=auxTK.IdReg,RegName=auxTK.RegCate+" "+auxTK.RegTipo
                                                                     ,CardCode=auxTK.CardCode,CardName=auxTK.CardName,Sentido="Salida"
                                                                     ,Detalle=auxTK.DocNum.ToString(),Cantidad=auxTK.RegCant,Operario=t.OpEntrega},cn,tran);
                    }
                    status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
                    
                    tran.Commit();
                    cn.Close();
                }
                catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error al editarSegumimiento : " + Estado + ".." + e.Message); }
            }
            catch(Exception e2) { status = 0; cn.Close(); throw new Exception(e2.Message); }
            
            return status;
        }
        public int facturarTicket(int DocEntry, Usuario_E u)
        {
            int status = -1;
            TicketVenta_E t;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                t = obtenerTicket(DocEntry);
                cn.Open();
                SqlCommand cmd = new SqlCommand("MANT_ORTV", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "USFC");
                cmd.Parameters.AddWithValue("@EstadoPedido", t.EstadoPedido);
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@OpFacturacion", u.nombre);
                cmd.ExecuteNonQuery();
                cn.Close();
                status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en fact: " + e.Message); }
            return status;
        }
        public int anularFacturarTicket(int DocEntry)
        {
            int status = -1;
            TicketVenta_E t;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                t = obtenerTicket(DocEntry);
                cn.Open();
                SqlCommand cmd = new SqlCommand("MANT_ORTV", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "UAFC");
                cmd.Parameters.AddWithValue("@EstadoPedido", t.EstadoPedido);
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                cn.Close();
                status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en AnFact: " + e.Message); }
            return status;
        }
        public int enviarProtocoloTicket(int DocEntry, Usuario_E u)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("MANT_ORTV", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "UEPT");
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@OpProtocolo", u.nombre);
                cmd.ExecuteNonQuery();
                cn.Close();
                status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en realizacion de protocolo : " + e.Message); }
            return status;
        }
        public int anularEnviarProtocoloTicket(int DocEntry)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("MANT_ORTV", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "UAEP");
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                cn.Close();
                status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en anularEnviarProtocolo: " + e.Message); }
            return status;
        }
        public List<ORIN_E> listarNotasDeCreditoV(string CardCode)
        {
            List<ORIN_E> lista = new List<ORIN_E>();
            string query = "SELECT \"DocEntry\",\"CardCode\",\"CardName\",\"DocTotal\",\"DocDate\",\"DocNum\" " +
                "FROM " + uti.schemaHana + "ORIN WHERE \"DocStatus\"='O' AND \"CANCELED\"='N' AND \"CardCode\"='" + CardCode + "'";
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand(query, hcn);
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    ORIN_E n = new ORIN_E()
                    {
                        DocEntry = hdr.GetInt32(0),
                        CardCode = hdr.GetString(1),
                        CardName = hdr.GetString(2),
                        DocTotal = hdr.GetDecimal(3),
                        DocDate = hdr.GetDateTime(4).ToString("yyyy-MM-dd"),
                        DocNum = hdr.GetInt32(5)
                    };
                    if (obtenerDet2Ticket(0, n.DocNum).Count == 0)
                    {
                        lista.Add(n);
                    }
                }
                hdr.Close();
                hcn.Close();
            } catch { hcn.Close(); }
            return lista;
        }
        private List<DetTicketVenta2_E> obtenerDet2Ticket(int DocEntry, int DocNum = 0)
        {
            List<DetTicketVenta2_E> lista = new List<DetTicketVenta2_E>();
            string query = "";
            if (DocNum == 0)
            {
                query = "select * from rtv2 where DocEntry=@DocEntry order by Linea";
                try
                {
                    SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
                    while (dr.Read())
                    {
                        DetTicketVenta2_E d2 = new DetTicketVenta2_E
                        {
                            DocEntry = dr.GetInt32(0),
                            Linea = dr.GetInt32(1),
                            Nc = new ORIN_E()
                            {
                                CardCode = dr.GetString(2),
                                CardName = dr.GetString(3),
                                DocTotal = dr.GetDecimal(4),
                                DocDate = dr.GetDateTime(5).ToString("yyyy-MM-dd"),
                                DocNum = dr.GetInt32(6)
                            }
                        };
                        lista.Add(d2);
                    }
                    dr.Close();
                }
                catch { }
            }
            else if (DocNum > 0)
            { //DocNum de la N.C
                query = "select t0.* from rtv2 t0" +
                            " inner join ortv t1 on t1.DocEntry = t0.DocEntry and t1.EstadoPedido <> 'ANULADO'" +
                            " where t0.DocNum = @DocNum order by t0.Linea";
                try
                {
                    SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string>() { "@DocNum" }, DocNum);
                    while (dr.Read())
                    {
                        DetTicketVenta2_E d2 = new DetTicketVenta2_E
                        {
                            DocEntry = dr.GetInt32(0),
                            Linea = dr.GetInt32(1),
                            Nc = new ORIN_E()
                            {
                                CardCode = dr.GetString(2),
                                CardName = dr.GetString(3),
                                DocTotal = dr.GetDecimal(4),
                                DocDate = dr.GetDateTime(5).ToString("yyyy-MM-dd"),
                                DocNum = dr.GetInt32(6)
                            }
                        };
                        lista.Add(d2);
                    }
                    dr.Close();
                }
                catch { }
            }
            return lista;
        }
        //metodos para reportes
        private List<TicketVenta_E> RptAnalisisVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnalisisVentas_E obj)
        {
            List<TicketVenta_E> lista = new List<TicketVenta_E>();
            string fil = "";
            if (obj.AlmIni != null && obj.AlmFin != null) { fil += " and (select top 1 y.LugarDeEntrega from RTV1 y where y.DocEntry=t0.DocEntry) between @AlmIni and @AlmFin"; }
            if (obj.FecIni != null && obj.FecFin != null) { fil += " and t0.FechaTicket between @FecIni and @FecFin"; }
            if (obj.CardCode != null) { fil += " and t0.CardCode=@CardCode"; }
            if (obj.LugarDestino != null) { fil += " and t0.LugarDestino=@LugarDestino"; }
            if (obj.TipoOperario != null) { fil += " and t0." + obj.TipoOperario + " like concat('%',@Operario,'%')"; }

            string query = "select t0.DocNum,t0.FechaTicket,t0.CardCode,t0.CardName" +
                                    ",t0.HoraTicket,t0.TipoVenta,t0.Embalaje,t0.PropietarioDesc" +
                                    ",t0.MontoTotal,t0.Flete,t0.GastoEnvio,t0.DescuentoNC,t0.DeudaCliente" +
                                    ",t0.DeudaEmpresa,t0.MontoFinal,t0.NroVentas,t0.CardCodeScVt,t0.CntctCode" +
                                    ",t0.NameScVt,t0.FirstLastNameScVt,t0.AddressScVt" +
                                    ",(select top 1 y.Vendedor from RTV1 y where y.DocEntry=t0.DocEntry) as 'VendedorSap'" +
                                    ",t0.FormaPago,t0.EstadoPago" +
                                    ",(select top 1 y.LugarDeEntrega from RTV1 y where y.DocEntry=t0.DocEntry) as 'LugEntregaSap'" +
                                " from ortv t0 where t0.DocEntry>0 " + fil;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                if (obj.AlmIni != null) { cmd.Parameters.AddWithValue("@AlmIni", obj.AlmIni); }
                if (obj.AlmFin != null) { cmd.Parameters.AddWithValue("@AlmFin", obj.AlmFin); }
                if (obj.FecIni != null) { cmd.Parameters.AddWithValue("@FecIni", obj.FecIni); }
                if (obj.FecFin != null) { cmd.Parameters.AddWithValue("@FecFin", obj.FecFin); }
                if (obj.CardCode != null) { cmd.Parameters.AddWithValue("@CardCode", obj.CardCode); }
                if (obj.LugarDestino != null) { cmd.Parameters.AddWithValue("@LugarDestino", obj.LugarDestino); }
                if (obj.Operario != null) { cmd.Parameters.AddWithValue("@Operario", obj.Operario); }

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TicketVenta_E o = new TicketVenta_E();
                    if (!dr.IsDBNull(0)) { o.DocNum = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.FechaTicket = dr.GetDateTime(1).ToString("dd/MM/yyyy"); }
                    if (!dr.IsDBNull(2)) { o.CardCode = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.CardName = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.HoraTicket = dr.GetTimeSpan(4).ToString(); }
                    if (!dr.IsDBNull(5)) { o.TipoVenta = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { o.Embalaje = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { o.PropietarioDesc = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { o.MontoTotal = dr.GetDecimal(8); }
                    if (!dr.IsDBNull(9)) { o.Flete = dr.GetDecimal(9); }
                    if (!dr.IsDBNull(10)) { o.GastoEnvio = dr.GetDecimal(10); }
                    if (!dr.IsDBNull(11)) { o.DescuentoNC = dr.GetDecimal(11); }
                    if (!dr.IsDBNull(12)) { o.DeudaCliente = dr.GetDecimal(12); }
                    if (!dr.IsDBNull(13)) { o.DeudaEmpresa = dr.GetDecimal(13); }
                    if (!dr.IsDBNull(14)) { o.MontoFinal = dr.GetDecimal(14); }
                    if (!dr.IsDBNull(15)) { o.NroVentas = dr.GetString(15); }
                    if (!dr.IsDBNull(16)) { o.CardCodeScVt = dr.GetString(16); }
                    if (!dr.IsDBNull(17)) { o.CntctCode = dr.GetInt32(17); }
                    if (!dr.IsDBNull(18)) { o.NameScVt = dr.GetString(18); }
                    if (!dr.IsDBNull(19)) { o.FirstLastNameScVt = dr.GetString(19); }
                    if (!dr.IsDBNull(20)) { o.AddressScVt = dr.GetString(20); }
                    if (!dr.IsDBNull(21)) { o.VendedorSap = dr.GetString(21); }
                    if (!dr.IsDBNull(22)) { o.FormaPago = dr.GetString(22); }
                    if (!dr.IsDBNull(23)) { o.EstadoPago = dr.GetString(23); }
                    if (!dr.IsDBNull(24)) { o.LugEntrega = dr.GetString(24); }
                    lista.Add(o);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public DataTable tbRptAnalisisVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnalisisVentas_E obj)
        {
            List<string> campos = new List<string>();
            List<Type> tipos = new List<Type>();
            campos.Add("Orden"); tipos.Add(typeof(string));
            campos.Add("DocNum"); tipos.Add(typeof(string));
            campos.Add("FechaTicket"); tipos.Add(typeof(string));
            campos.Add("CardCode"); tipos.Add(typeof(string));
            campos.Add("CardName"); tipos.Add(typeof(string));
            campos.Add("HoraTicket"); tipos.Add(typeof(string));
            campos.Add("TipoVenta"); tipos.Add(typeof(string));
            campos.Add("Embalaje"); tipos.Add(typeof(string));
            campos.Add("PropietarioDesc"); tipos.Add(typeof(string));
            campos.Add("MontoTotal"); tipos.Add(typeof(string));
            campos.Add("Flete"); tipos.Add(typeof(string));
            campos.Add("GastoEnvio"); tipos.Add(typeof(string));
            campos.Add("DescuentoNC"); tipos.Add(typeof(string));
            campos.Add("DeudaCliente"); tipos.Add(typeof(string));
            campos.Add("DeudaEmpresa"); tipos.Add(typeof(string));
            campos.Add("MontoFinal"); tipos.Add(typeof(string));
            campos.Add("NroVentas"); tipos.Add(typeof(string));
            campos.Add("CardCodeScVt"); tipos.Add(typeof(string));
            campos.Add("CntctCode"); tipos.Add(typeof(string));
            campos.Add("NameScVt"); tipos.Add(typeof(string));
            campos.Add("FirstLastNameScVt"); tipos.Add(typeof(string));
            campos.Add("AddressScVt"); tipos.Add(typeof(string));
            campos.Add("VendedorSap"); tipos.Add(typeof(string));
            campos.Add("FormaPago"); tipos.Add(typeof(string));
            campos.Add("EstadoPago"); tipos.Add(typeof(string));
            campos.Add("LugEntrega"); tipos.Add(typeof(string));

            DataTable tb = definirTabla(campos, tipos, "DataTableReporteAnalisisVentas");
            int i = 0;
            foreach (TicketVenta_E p in RptAnalisisVentas(obj))
            {
                DataRow row = tb.NewRow();
                row["Orden"] = i++;
                row["DocNum"] = p.DocNum;
                row["FechaTicket"] = p.FechaTicket;
                row["CardCode"] = p.CardCode;
                row["CardName"] = p.CardName;
                row["HoraTicket"] = p.HoraTicket;
                row["TipoVenta"] = p.TipoVenta;
                row["Embalaje"] = p.Embalaje;
                row["PropietarioDesc"] = p.PropietarioDesc;
                row["MontoTotal"] = p.MontoTotal;
                row["Flete"] = p.Flete;
                row["GastoEnvio"] = p.GastoEnvio;
                row["DescuentoNC"] = p.DescuentoNC;
                row["DeudaCliente"] = p.DeudaCliente;
                row["DeudaEmpresa"] = p.DeudaEmpresa;
                row["MontoFinal"] = p.MontoFinal;
                row["NroVentas"] = p.NroVentas;
                row["CardCodeScVt"] = p.CardCodeScVt;
                row["CntctCode"] = p.CntctCode;
                row["NameScVt"] = p.NameScVt;
                row["FirstLastNameScVt"] = p.FirstLastNameScVt;
                row["AddressScVt"] = p.AddressScVt;
                row["VendedorSap"] = p.VendedorSap;
                row["FormaPago"] = p.FormaPago;
                row["EstadoPago"] = p.EstadoPago;
                row["LugEntrega"] = p.LugEntrega;
                tb.Rows.Add(row);
            }
            return tb;
        }
        // Para atencion al cliente
        public List<TicketVenta_E> listarTicketsParaAtencion()
        {
            string query = "";
            List<TicketVenta_E> lista = new List<TicketVenta_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                query = "select DocEntry,DocNum,CardCode,CardName,LugarDestino,FechaFacturacion from ortv" +
                    " where EstadoPedido ='Entregado' and FechaTicket>=DATEADD(DAY,-25,getdate()) order by DocNum desc";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TicketVenta_E ticket = new TicketVenta_E();
                    if (!dr.IsDBNull(0)) { ticket.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { ticket.DocNum = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { ticket.CardCode = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { ticket.CardName = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { ticket.LugarDestino = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { ticket.FechaFacturacion = dr.GetDateTime(5).ToString("dd/MM/yyyy"); }
                    lista.Add(ticket);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        //****************CALCULOS***********
        public TicketVenta_E CalcularMontos(TicketVenta_E t)
        {
            t.MontoTotal = (decimal)(0.00);
            t.MontoFinal = (decimal)(0.00);
            
            foreach (DetTicketVenta_E d in t.Det)
            {
                if(d.Verificar=="on")
                {
                    t.MontoTotal += d.Monto;
                }
            }
            if(t.Det2!=null)
            {
                t.DescuentoNC = 0.00M;
                foreach (DetTicketVenta2_E d2 in t.Det2)
                {
                    if(d2.Verificar=="on")
                    {
                        t.DescuentoNC += d2.Nc.DocTotal;
                    }
                }
            }
            t.MontoFinal += t.MontoTotal + t.Flete + t.GastoEnvio + t.DeudaCliente - t.DescuentoNC - t.DeudaEmpresa;
            return t;
        }
        //
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
    }
}
