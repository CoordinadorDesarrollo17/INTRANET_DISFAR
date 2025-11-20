using Capa_Datos.Ventas_DAO.Tablas;
using Capa_Entidad.ComprobantesContables_ENT;
using Capa_Entidad.ReportesDigemid_ENT;
using Capa_Entidad.Rutas_ENT.ReportesSql;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.Reportes;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.TablasSql;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Deployment.Internal;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;
namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class ORTV_D
    {
        readonly Utilitarios uti = new Utilitarios();
        readonly DBHelper db = new DBHelper();
        CC_ORTV_D _ccTicketD = new CC_ORTV_D();
        OLDS_D _oldsD = new OLDS_D();
        //Lista de tickets con autorizacion fuera de horario que requeirn regularizar
        public List<dynamic> ListarTicketsPorRegularizarContraEntrega()
        {
            List<dynamic> lista = new List<dynamic>();
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "EXEC [vt].[AutorizacionPorRegularizar]";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dynamic ticket = new ExpandoObject();
                            ticket.IdOTC = dr.IsDBNull(dr.GetOrdinal("IdOTC")) ? 0 : dr.GetInt32(dr.GetOrdinal("IdOTC"));
                            ticket.DocNumTicket = dr.IsDBNull(dr.GetOrdinal("DocNumTicket")) ? 0 : dr.GetInt32(dr.GetOrdinal("DocNumTicket"));
                            ticket.TipoPago = dr.IsDBNull(dr.GetOrdinal("TipoPago")) ? string.Empty : dr.GetString(dr.GetOrdinal("TipoPago"));
                            ticket.Estado = dr.IsDBNull(dr.GetOrdinal("Estado")) ? string.Empty : dr.GetString(dr.GetOrdinal("Estado"));
                            ticket.MontoRecibidoEfectivo = dr.IsDBNull(dr.GetOrdinal("MontoRecibidoEfectivo")) ? 0 : dr.GetDecimal(dr.GetOrdinal("MontoRecibidoEfectivo"));
                            ticket.MontoRecibidoDeposito = dr.IsDBNull(dr.GetOrdinal("MontoRecibidoDeposito")) ? 0 : dr.GetDecimal(dr.GetOrdinal("MontoRecibidoDeposito"));
                            ticket.Suma = dr.IsDBNull(dr.GetOrdinal("Suma")) ? 0 : dr.GetDecimal(dr.GetOrdinal("Suma"));
                            ticket.MontoFinal = dr.IsDBNull(dr.GetOrdinal("MontoFinal")) ? 0 : dr.GetDecimal(dr.GetOrdinal("MontoFinal"));
                            ticket.TienePagoParcial = dr.IsDBNull(dr.GetOrdinal("TienePagoParcial")) ? string.Empty : dr.GetString(dr.GetOrdinal("TienePagoParcial"));
                            ticket.Registro = dr.IsDBNull(dr.GetOrdinal("Registro")) ? string.Empty : dr.GetString(dr.GetOrdinal("Registro"));
                            ticket.Cuadre = dr.IsDBNull(dr.GetOrdinal("Cuadre")) ? string.Empty : dr.GetString(dr.GetOrdinal("Cuadre"));
                            ticket.CardName = dr.IsDBNull(dr.GetOrdinal("CardName")) ? string.Empty : dr.GetString(dr.GetOrdinal("CardName"));
                            ticket.Vendedor = dr.IsDBNull(dr.GetOrdinal("Vendedor")) ? string.Empty : dr.GetString(dr.GetOrdinal("Vendedor"));
                            lista.Add(ticket);
                        }
                    }
                }
            }
            return lista;
        }
        //Reporte para entrega de regalos en tickets
        public List<ReporteRegalos> ListarTicketsRegalo(string fechaTicketDesde, string fechaTicketHasta, string estadoTicket, string estadoRegalo)
        {
            List<ReporteRegalos> lista = new List<ReporteRegalos>();
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "EXEC vt.ReporteRegalosCliente @FechaTicketDesde, @FechaTicketHasta, @EstadoTicket, @EstadoRegalo";
                SqlCommand cmd = new SqlCommand(query, cn);
                // Parámetro @FechaTicketDesde
                if (fechaTicketDesde == null || fechaTicketDesde.Trim() == "")
                {
                    cmd.Parameters.AddWithValue("@FechaTicketDesde", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@FechaTicketDesde", fechaTicketDesde);
                }
                // Parámetro @FechaTicketHasta
                if (fechaTicketHasta == null || fechaTicketHasta.Trim() == "")
                {
                    cmd.Parameters.AddWithValue("@FechaTicketHasta", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@FechaTicketHasta", fechaTicketHasta);
                }
                // Parámetro @EstadoTicket
                if (estadoTicket == null || estadoTicket.Trim() == "")
                {
                    cmd.Parameters.AddWithValue("@EstadoTicket", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@EstadoTicket", estadoTicket);
                }
                // Parámetro @EstadoRegalo
                if (estadoRegalo == null || estadoRegalo.Trim() == "")
                {
                    cmd.Parameters.AddWithValue("@EstadoRegalo", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@EstadoRegalo", estadoRegalo);
                }
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ReporteRegalos ticket = new ReporteRegalos();
                            ticket.DocNum = dr.IsDBNull(dr.GetOrdinal("DocNum")) ? 0 : dr.GetInt32(dr.GetOrdinal("DocNum"));
                            ticket.FechaSapTicket = dr.IsDBNull(dr.GetOrdinal("FechaSapTicket")) ? null : dr.GetDateTime(dr.GetOrdinal("FechaSapTicket")).ToString("yyyy-MM-dd");
                            ticket.CardCode = dr.IsDBNull(dr.GetOrdinal("Ruc")) ? string.Empty : dr.GetString(dr.GetOrdinal("Ruc"));
                            ticket.CardName = dr.IsDBNull(dr.GetOrdinal("Razon Social")) ? string.Empty : dr.GetString(dr.GetOrdinal("Razon Social"));
                            ticket.Linea = dr.IsDBNull(dr.GetOrdinal("Linea")) ? 0 : dr.GetInt32(dr.GetOrdinal("Linea"));
                            ticket.NombreRegalo = dr.IsDBNull(dr.GetOrdinal("Nombre regalo")) ? string.Empty : dr.GetString(dr.GetOrdinal("Nombre regalo"));
                            ticket.Cantidad = dr.IsDBNull(dr.GetOrdinal("Cantidad")) ? 0 : dr.GetInt32(dr.GetOrdinal("Cantidad"));
                            ticket.EstadoRegalo = dr.IsDBNull(dr.GetOrdinal("Estado entrega de regalo")) ? string.Empty : dr.GetString(dr.GetOrdinal("Estado entrega de regalo"));
                            ticket.Estado = dr.IsDBNull(dr.GetOrdinal("Estado")) ? string.Empty : dr.GetString(dr.GetOrdinal("Estado"));
                            ticket.LugarDestino = dr.IsDBNull(dr.GetOrdinal("Lugar Destino")) ? string.Empty : dr.GetString(dr.GetOrdinal("Lugar Destino"));
                            ticket.Vendedor = dr.IsDBNull(dr.GetOrdinal("Vendedor")) ? string.Empty : dr.GetString(dr.GetOrdinal("Vendedor"));
                            lista.Add(ticket);
                        }
                    }
                }
            }
            return lista;
        }
        public List<ORTV_E> listarTicketsSeparados(int Id)
        {
            List<ORTV_E> lista = new List<ORTV_E>();
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT DocEntry, DocNum FROM vt.ORTV WHERE Estado = 'SEPARADO' AND CodSapVendedor = @Id";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@Id", Id);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ORTV_E t = new ORTV_E
                            {
                                DocEntry = dr.GetInt32(0),
                                DocNum = dr.GetInt32(1)
                            };
                            lista.Add(t);
                        }
                    }
                }
            }
            return lista;
        }
        private List<RTV1_E> obtenerDet1Ticket(int DocEntry)
        {
            List<RTV1_E> lista = new List<RTV1_E>();
            string query = "select DocEntry,Linea,NombrePer,TelfPer,TipoDocPer,DocPer from vt.RTV1 where DocEntry=" + DocEntry + " order by Linea";
            SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    RTV1_E d = new RTV1_E();
                    if (!dr.IsDBNull(0)) { d.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { d.Linea = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { d.NombrePer = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { d.TelfPer = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { d.TipoDocPer = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { d.DocPer = dr.GetString(5); }
                    lista.Add(d);
                }
            }
            dr.Close();
            return lista;
        }
        public List<RTV2_E> obtenerDet2Ticket(int DocEntry)
        {
            List<RTV2_E> lista = new List<RTV2_E>();
            string query = "select * from vt.RTV2 WHERE DocEntry=@DocEntry order by Linea";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RTV2_E o = new RTV2_E();
                        if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { o.Linea = dr.GetInt32(1); }
                        if (!dr.IsDBNull(2)) { o.Monto = dr.GetDecimal(2); }
                        if (!dr.IsDBNull(3)) { o.NroSap = dr.GetInt32(3); }
                        if (!dr.IsDBNull(4)) { o.TipoComprobante = dr.GetString(4); }
                        if (!dr.IsDBNull(5)) { o.Vendedor = dr.GetString(5); }
                        if (!dr.IsDBNull(6)) { o.LugarDeEntrega = dr.GetString(6); }
                        if (!dr.IsDBNull(7)) { o.Observaciones = dr.GetString(7); }
                        if (!dr.IsDBNull(8)) { o.AlmacenSalida = dr.GetString(8); }
                        lista.Add(o);
                    }
                }
                dr.Close();
            }
            catch (Exception e) { throw new Exception(e.Message); }
            return lista;
        }
        private List<RTV3_E> obtenerDet3Ticket(int DocEntry)
        {
            List<RTV3_E> lista = new List<RTV3_E>();
            string query = string.Empty;
            query = "select * from vt.RTV3 where DocEntry=@DocEntry order by IdDireccion";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
                while (dr.Read())
                {
                    RTV3_E d3 = new RTV3_E();
                    if (!dr.IsDBNull(0)) { d3.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { d3.IdDireccion = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { d3.Ubigeo = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { d3.Distrito = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { d3.Provincia = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { d3.Departamento = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { d3.Calle = dr.GetString(6); }
                    lista.Add(d3);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
        public List<RTV4_E> obtenerDet4Ticket(int DocEntry, int DocNum = 0)
        {
            List<RTV4_E> lista = new List<RTV4_E>();
            string query = string.Empty;
            if (DocNum == 0)
            {
                query = "select * from vt.RTV4 where DocEntry=@DocEntry order by Linea";
                try
                {
                    SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
                    while (dr.Read())
                    {
                        RTV4_E d4 = new RTV4_E();
                        ORIN_E Nc = new ORIN_E();
                        if (!dr.IsDBNull(0)) { d4.DocEntry = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { d4.Linea = dr.GetInt32(1); }
                        if (!dr.IsDBNull(2)) { Nc.CardCode = dr.GetString(2); }
                        if (!dr.IsDBNull(3)) { Nc.CardName = dr.GetString(3); }
                        if (!dr.IsDBNull(4)) { Nc.DocTotal = dr.GetDecimal(4); }
                        if (!dr.IsDBNull(5)) { Nc.DocDate = dr.GetDateTime(5).ToString("yyyy-MM-dd"); }
                        if (!dr.IsDBNull(6)) { Nc.DocNum = dr.GetInt32(6); }
                        if (Nc != null) { d4.Nc = Nc; }
                        lista.Add(d4);
                    }
                    dr.Close();
                }
                catch { }
            }
            else if (DocNum > 0)
            {
                //DocNum de la N.C
                query = "select t0.* FROM vt.RTV4 t0 INNER JOIN vt.ORTV t1 on t1.DocEntry = t0.DocEntry AND t1.Estado not in ('ANULADO','CANCELADO') WHERE t0.DocNum = @DocNum order by t0.Linea";
                try
                {
                    SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string>() { "@DocNum" }, DocNum);
                    while (dr.Read())
                    {
                        RTV4_E d4 = new RTV4_E();
                        ORIN_E Nc = new ORIN_E();
                        if (!dr.IsDBNull(0)) { d4.DocEntry = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { d4.Linea = dr.GetInt32(1); }
                        if (!dr.IsDBNull(2)) { Nc.CardCode = dr.GetString(2); }
                        if (!dr.IsDBNull(3)) { Nc.CardName = dr.GetString(3); }
                        if (!dr.IsDBNull(4)) { Nc.DocTotal = dr.GetDecimal(4); }
                        if (!dr.IsDBNull(5)) { Nc.DocDate = dr.GetDateTime(5).ToString("yyyy-MM-dd"); }
                        if (!dr.IsDBNull(6)) { Nc.DocNum = dr.GetInt32(6); }
                        if (Nc != null) { d4.Nc = Nc; }
                        lista.Add(d4);
                    }
                    dr.Close();
                }
                catch { }
            }
            return lista;
        }
        public List<RTV5_E> obtenerDet5Ticket(int DocEntry)
        {
            List<RTV5_E> lista = new List<RTV5_E>();
            string query = "select * from vt.RTV5 where DocEntry=@DocEntry order by Linea";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
                while (dr.Read())
                {
                    RTV5_E d2 = new RTV5_E();
                    if (!dr.IsDBNull(0)) { d2.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { d2.Linea = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { d2.IdReg = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { d2.RegTipo = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { d2.RegCate = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { d2.RegCant = dr.GetDecimal(5); }
                    if (!dr.IsDBNull(6)) { d2.RegEstado = dr.GetString(6); }
                    lista.Add(d2);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
        private List<RTV6_E> obtenerDet6Ticket(int DocEntry)
        {
            List<RTV6_E> lista = new List<RTV6_E>();
            string query = "select DocEntry,Linea,Peso,UniMed,PrecioEnv from vt.RTV6 where DocEntry=@DocEntry order by Linea";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
                while (dr.Read())
                {
                    RTV6_E bean = new RTV6_E();
                    if (!dr.IsDBNull(0)) { bean.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { bean.Linea = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { bean.Peso = dr.GetDecimal(2); }
                    if (!dr.IsDBNull(3)) { bean.UniMed = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { bean.PrecioEnv = dr.GetDecimal(4); }
                    lista.Add(bean);
                }
                dr.Close();
            }
            catch (Exception e) { throw new Exception("Error al obtener Det6 " + e.Message, e); }
            return lista;
        }
        private List<RTV7_E> obtenerDet7Ticket(int DocEntry)
        {
            List<RTV7_E> lista = new List<RTV7_E>();
            string query = "select DocEntry,Linea,DocNumVinc,CardCode,CardName,MontoFinal from vt.RTV7 where DocEntry=@DocEntry order by Linea";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@DocEntry" }, DocEntry);
                while (dr.Read())
                {
                    RTV7_E bean = new RTV7_E();
                    if (!dr.IsDBNull(0)) { bean.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { bean.Linea = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { bean.DocNumVinc = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { bean.CardCode = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { bean.CardName = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { bean.MontoFinal = dr.GetDecimal(5); }
                    lista.Add(bean);
                }
                dr.Close();
            }
            catch (Exception e) { throw new Exception("Error al obtener Det7 " + e.Message, e); }
            return lista;
        }
        /**********************************************************************/
        public ORTV_E Separar(Usuario_E u)
        {
            //tipo mantenimiento AS(add separo) transaccion tipo A(add)
            ORTV_E t = new ORTV_E();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("TR01");
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "AS");
                    cmd.Parameters.AddWithValue("@DocEntry", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@CodSapVendedor", u.CodigoSap);
                    cmd.Parameters.AddWithValue("@Vendedor ", u.Nombres + " " + u.Apellidos);
                    cmd.ExecuteNonQuery();
                    t.DocEntry = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    t.DocNum = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
                    t.Estado = "SEPARADO";
                    t.Vendedor = u.Nombres + " " + u.Apellidos;
                    t.CodSapVendedor = u.DocEntry;
                    SqlCommand cmd2 = new SqlCommand("dbo.POST_TRANSACCIONES", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "ORTV");
                    cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@DocNum"].Value);
                    cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@DocEntry"].Value);
                    cmd2.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception e1) { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: " + e1.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return t;
        }
        public int Registrar(ORTV_E ticket)
        {
            int status = -1;
            string zonaTk = ObtenerZonaDestino(ticket.LugarDestino, ticket.Zona);
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlTransaction tran = cn.BeginTransaction())
                    {
                        try
                        {
                            if (ticket.Det5 != null && ticket.Det5.Any() && ticket.Det5[0].RegCant > 0)
                            {
                                ProcesarCompromisosStock(ticket, tran); // en este proceso de creacion solo lo manda como positivo para la asignacion inicial del regalo si hubiera
                            }
                            SqlCommand cmd = CrearComandoRegistrarTicket(cn, ticket, zonaTk);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            // Si el ticket tiene deuda cliente, procesamos las operaciones relacionadas
                            if (ticket.CardCode != null && ticket.CardName != null)
                            {
                                EjecutarOperacionesDeDeuda(ticket, tran);
                            }
                            tran.Commit();
                            status = ticket.DocNum;
                        }
                        catch (Exception e1)
                        {
                            tran.Rollback();
                            throw new Exception("Error en registro: " + e1.Message);
                        }
                    }
                }
                catch (Exception e2)
                {
                    throw new Exception("Error en registro y conexión: " + e2.Message);
                }
            }
            return status;
        }
        private SqlCommand CrearComandoRegistrarTicket(SqlConnection cn, ORTV_E ticket, string zonaTk)
        {
            SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            // Agregar los parámetros del ticket
            cmd.Parameters.AddWithValue("@TipoMantenimiento", "UR");
            cmd.Parameters.AddWithValue("@DocEntry", ticket.DocEntry).Direction = ParameterDirection.InputOutput;
            cmd.Parameters.AddWithValue("@DocNum", ticket.DocNum).Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@FechaSapTicket", ticket.FechaSapTicket);
            cmd.Parameters.AddWithValue("@CardCode", ticket.CardCode);
            cmd.Parameters.AddWithValue("@CardName", ticket.CardName);
            cmd.Parameters.AddWithValue("@TipoVenta", ticket.TipoVenta);
            cmd.Parameters.AddWithValue("@LugarDestino", ticket.LugarDestino);
            cmd.Parameters.AddWithValue("@EnvioAgencia", ticket.EnvioAgencia);
            cmd.Parameters.AddWithValue("@Referencia", ticket.Referencia);
            cmd.Parameters.AddWithValue("@Agencia", ticket.Agencia);
            cmd.Parameters.AddWithValue("@DirDestino", ticket.DirDestino);
            cmd.Parameters.AddWithValue("@Operario", ticket.Vendedor);
            cmd.Parameters.AddWithValue("@Estado", ticket.Estado);
            cmd.Parameters.AddWithValue("@Observaciones", ticket.Observaciones);
            cmd.Parameters.AddWithValue("@Embalaje", ticket.Embalaje);
            cmd.Parameters.AddWithValue("@Comentario", ticket.Comentario);
            cmd.Parameters.AddWithValue("@MontoTotal", ticket.MontoTotal);
            cmd.Parameters.AddWithValue("@Flete", ticket.Flete);
            cmd.Parameters.AddWithValue("@GastoEnvio", ticket.GastoEnvio);
            cmd.Parameters.AddWithValue("@DescuentoNC", ticket.DescuentoNC);
            cmd.Parameters.AddWithValue("@DeudaCliente", ticket.DeudaCliente);
            cmd.Parameters.AddWithValue("@DeudaEmpresa", ticket.DeudaEmpresa);
            cmd.Parameters.AddWithValue("@MontoFinal", ticket.MontoFinal);
            cmd.Parameters.AddWithValue("@Observaciones2", ticket.Observaciones2);
            cmd.Parameters.AddWithValue("@Observaciones3", ticket.Observaciones3);
            cmd.Parameters.AddWithValue("@FormaPago", ticket.FormaPago);
            cmd.Parameters.AddWithValue("@AlmProcedencia", ticket.AlmProcedencia);
            cmd.Parameters.AddWithValue("@Zona", zonaTk);
            cmd.Parameters.AddWithValue("@FechaNC", ticket.FechaNC);
            cmd.Parameters.AddWithValue("@Presupuesto", ticket.Presupuesto);
            cmd.Parameters.AddWithValue("@Visible", "NO");
            if (ticket.TiempoEntrega != null)
            {
                cmd.Parameters.AddWithValue("@TiempoEntrega", ticket.TiempoEntrega);
            }
            // Si hay gasto de envío, se marca su estado como pendiente
            if (ticket.GastoEnvio > 0)
            {
                cmd.Parameters.AddWithValue("@EstadoGasto", "PENDIENTE");
            }
            // Agregar detalles de la venta
            AgregarDetallesComando(cmd, ticket, ticket.DocEntry);
            return cmd;
        }
        private void EjecutarOperacionesDeDeuda(ORTV_E ticket, SqlTransaction tran)
        {
            SqlCommand cmd = new SqlCommand("vt.MANT_OLDS", tran.Connection, tran)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@TipoMantenimiento", "AC");
            cmd.Parameters.AddWithValue("@C_CardCode", ticket.CardCode);
            cmd.Parameters.AddWithValue("@C_CardName", ticket.CardName);
            cmd.ExecuteNonQuery();
            if (ticket.DeudaCliente > 0)
            {
                cmd.Parameters.Clear(); // Limpiar parámetros previos
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                cmd.Parameters.AddWithValue("@C_CardCode", ticket.CardCode);
                cmd.Parameters.AddWithValue("@FechaOpe", ticket.FechaSapTicket);
                cmd.Parameters.AddWithValue("@Operacion", "VENTA");
                cmd.Parameters.AddWithValue("@DetOpe", "VENTA DeudaCliente, ticket:" + ticket.DocNum + " MR:" + ticket.MontoFinal);
                cmd.Parameters.AddWithValue("@Ingreso", ticket.DeudaCliente);
                cmd.Parameters.AddWithValue("@OperarioRegistro", ticket.OpRegistro);
                cmd.ExecuteNonQuery();
            }
            if (ticket.DeudaEmpresa > 0)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                cmd.Parameters.AddWithValue("@C_CardCode", ticket.CardCode);
                cmd.Parameters.AddWithValue("@FechaOpe", ticket.FechaSapTicket);
                cmd.Parameters.AddWithValue("@Operacion", "VENTA");
                cmd.Parameters.AddWithValue("@DetOpe", "SALIDASALDO DeudaEmpresa, ticket:" + ticket.DocNum + " MR:" + ticket.MontoFinal);
                cmd.Parameters.AddWithValue("@Egreso", ticket.DeudaEmpresa);
                cmd.Parameters.AddWithValue("@OperarioRegistro", ticket.OpRegistro);
                cmd.ExecuteNonQuery();
            }
        }
        public int Editar(int docEntry, ORTV_E ticket)
        {
            int status = -1;
            string zonaTk = ObtenerZonaDestino(ticket.LugarDestino, ticket.Zona);
            ORTV_E auxTK = ObtenerDatosCompletosTicket(docEntry);
            if (ticket.Det5[0].IdReg == 0) { ticket.Det5 = null; }
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlTransaction tran = cn.BeginTransaction())
                    {
                        try
                        {
                            if (ticket.Det5 != null && ticket.Det5.Any() && ticket.Det5[0].RegCant > 0)
                            {
                                ProcesarCompromisosStock(ticket, tran, auxTK);
                            }
                            // Crear el comando para actualizar el ticket y asociarlo con la transacción
                            SqlCommand cmd = CrearComandoActualizarTicket(cn, ticket, docEntry, zonaTk);
                            cmd.Transaction = tran;
                            cmd.ExecuteNonQuery();
                            tran.Commit();
                            status = ticket.DocNum;
                        }
                        catch (Exception e)
                        {
                            tran.Rollback();
                            throw new Exception("Error en edición: " + e.Message);
                        }
                    }
                }
                catch (Exception e2)
                {
                    throw new Exception("Error en conexión y edición: " + e2.Message);
                }
            }
            return status;
        }
        private string ObtenerZonaDestino(string lugarDestino, string zona)
        {
            var zonasDestino = new Dictionary<string, string>
            {
                { "Agencia", "AGENCIA" },
                { "Centro", "CONO CENTRO" },
                { "Arriola", "ARRIOLA" },
                { "Domicilio", zona }
            };
            // Si el lugarDestino está en el diccionario, devuelve su valor; de lo contrario, devuelve una cadena vacía
            return zonasDestino.TryGetValue(lugarDestino, out var resultado) ? resultado : string.Empty;
        }
        private SqlCommand CrearComandoActualizarTicket(SqlConnection cn, ORTV_E ticket, int docEntry, string zonaTk)
        {
            SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn)
            {
                CommandType = CommandType.StoredProcedure
            };
            // Agregar los parámetros del ticket
            cmd.Parameters.AddWithValue("@TipoMantenimiento", "UU");
            cmd.Parameters.AddWithValue("@Estado", ticket.Estado);
            cmd.Parameters.AddWithValue("@TipoVenta", ticket.TipoVenta);
            cmd.Parameters.AddWithValue("@LugarDestino", ticket.LugarDestino);
            cmd.Parameters.AddWithValue("@Agencia", ticket.Agencia);
            cmd.Parameters.AddWithValue("@DirDestino", ticket.DirDestino);
            cmd.Parameters.AddWithValue("@Embalaje", ticket.Embalaje);
            cmd.Parameters.AddWithValue("@Comentario", ticket.Comentario);
            cmd.Parameters.AddWithValue("@Flete", ticket.Flete);
            cmd.Parameters.AddWithValue("@GastoEnvio", ticket.GastoEnvio);
            cmd.Parameters.AddWithValue("@DescuentoNC", ticket.DescuentoNC);
            cmd.Parameters.AddWithValue("@DeudaCliente", ticket.DeudaCliente);
            cmd.Parameters.AddWithValue("@DeudaEmpresa", ticket.DeudaEmpresa);
            cmd.Parameters.AddWithValue("@MontoFinal", ticket.MontoFinal);
            cmd.Parameters.AddWithValue("@Observaciones", ticket.Observaciones);
            cmd.Parameters.AddWithValue("@Observaciones2", ticket.Observaciones2);
            cmd.Parameters.AddWithValue("@Observaciones3", ticket.Observaciones3);
            cmd.Parameters.AddWithValue("@FormaPago", ticket.FormaPago);
            cmd.Parameters.AddWithValue("@FechaNC", ticket.FechaNC);
            if (ticket.TiempoEntrega != null)
            {
                cmd.Parameters.AddWithValue("@TiempoEntrega", ticket.TiempoEntrega);
            }
            cmd.Parameters.AddWithValue("@EstadoGasto", ticket.EstadoGasto);
            cmd.Parameters.AddWithValue("@EnvioAgencia", ticket.EnvioAgencia);
            cmd.Parameters.AddWithValue("@Referencia", ticket.Referencia);
            cmd.Parameters.AddWithValue("@DocEntry", docEntry);
            cmd.Parameters.AddWithValue("@DocNum", ticket.DocNum);
            cmd.Parameters.AddWithValue("@Operario", ticket.Vendedor);
            cmd.Parameters.AddWithValue("@Zona", zonaTk);
            cmd.Parameters.AddWithValue("@Presupuesto", ticket.Presupuesto);
            cmd.Parameters.AddWithValue("@AlmProcedencia", ticket.AlmProcedencia);
            AgregarDetallesComando(cmd, ticket, docEntry);
            return cmd;
        }
        private void AgregarDetallesComando(SqlCommand cmd, ORTV_E ticket, int docEntry)
        {
            if (ticket.Det1?.Count > 0)
            {
                SqlParameter tbDet1 = new SqlParameter("@TPRTV1", SqlDbType.Structured)
                {
                    Value = RTV1_E.GenerarDataTable(ticket.Det1, docEntry),
                    TypeName = "vt.TPRTV1"
                };
                cmd.Parameters.Add(tbDet1);
            }
            if (ticket.Det2?.Count > 0)
            {
                SqlParameter tbDet2 = new SqlParameter("@TPRTV2", SqlDbType.Structured)
                {
                    Value = RTV2_E.GenerarDataTable(ticket.Det2, docEntry),
                    TypeName = "vt.TPRTV2"
                };
                cmd.Parameters.Add(tbDet2);
            }
            if (ticket.Det3?.Count > 0)
            {
                SqlParameter tbDet3 = new SqlParameter("@TPRTV3", SqlDbType.Structured)
                {
                    Value = RTV3_E.GenerarDataTable(ticket.Det3, docEntry),
                    TypeName = "vt.TPRTV3"
                };
                cmd.Parameters.Add(tbDet3);
            }
            if (ticket.Det4?.Count > 0)
            {
                SqlParameter tbDet4 = new SqlParameter("@TPRTV4", SqlDbType.Structured)
                {
                    Value = RTV4_E.GenerarDataTable(ticket.Det4, ticket),
                    TypeName = "vt.TPRTV4"
                };
                cmd.Parameters.Add(tbDet4);
            }
            if (ticket.Det5?.Count > 0)
            {
                ticket.Det5[0].DocEntry = docEntry;
                ticket.Det5[0].RegEstado = "Pendiente";
                SqlParameter tbDet5 = new SqlParameter("@TPRTV5", SqlDbType.Structured)
                {
                    Value = RTV5_E.GenerarDataTable(ticket.Det5, docEntry),
                    TypeName = "vt.TPRTV5"
                };
                cmd.Parameters.Add(tbDet5);
            }
            if (ticket.Observaciones2 == "SI" && ticket.Det7?.Count >= 1)
            {
                SqlParameter tbDet7 = new SqlParameter("@TPRTV7", SqlDbType.Structured)
                {
                    Value = RTV7_E.GenerarDataTable(ticket.Det7, docEntry),
                    TypeName = "vt.TPRTV7"
                };
                cmd.Parameters.Add(tbDet7);
            }
        }
        private List<ORTV_E> GenerarListaUnificadaStock(ORTV_E ticket, ORTV_E auxTK)
        {
            var ticketUnificado = new List<ORTV_E>();
            if (auxTK != null && auxTK.Det5?.Count >= 1 && auxTK.Det5[0].IdReg > 0)
            {
                auxTK.Det5[0].RegCant = -1 * auxTK.Det5[0].RegCant;
                ticketUnificado.Add(auxTK);
            }
            if (ticket.Det5?.Count >= 1 && ticket.Det5[0].IdReg > 0)
            {
                ticketUnificado.Add(ticket);
            }
            return ticketUnificado;
        }
        //Validado, se usa en crear y editar
        private void ProcesarCompromisosStock(ORTV_E ticket, SqlTransaction tran, ORTV_E auxTK = null)
        {
            OREG_D oregD = new OREG_D();
            if (ticket.Estado.Equals("SEPARADO") || ticket.Estado.Equals("ABIERTO"))
            {
                //unir en una sola lista
                var ticketUnificado = GenerarListaUnificadaStock(ticket, auxTK);
                oregD.CompromisosStock(ticketUnificado, tran);
            }
        }
        //Automatico desde click a Boton Ver Layout
        public int EditarVisibilidadTicket(int docEntry, string opImpresion, string proceso)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand();
                if (proceso.Equals("PI")) // EN PROCESO DE IMPRESION
                {
                    cmd = new SqlCommand($"UPDATE vt.ortv SET Visible='{proceso}' where DocEntry=@DocEntry; insert into vt.CC_ORTV values (@DocEntry,'{proceso}','{opImpresion}',(select convert(varchar,getdate(),23)),(select convert(char(5),getdate(),108)))", cn);
                }
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DocEntry", docEntry);
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            catch { }
            return docEntry;
        }
        //Hace que sea visible para recepcion junto con ticket
        public int EditarPresupuestoTicket(int DocEntry)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE vt.ortv SET Presupuesto='NO' where DocEntry=@DocEntry", cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            catch { }
            return DocEntry;
        }
        public int EditarProductosPendientesTicket(int DocEntry)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE VT.BUSQUEDAPRODUCTO SET Estado='CONCLUIDO' WHERE DocEntry=@DocEntry", cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            catch { }
            return DocEntry;
        }
        /**********************************************************************/
        /********************** METODOS AUXILIARES privados ********************/
        private List<OCRD_E> listarClientes(string Fecha)
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
        private List<RTV3_E> listarDirDestinos(string CardCode)
        {
            List<RTV3_E> lista = new List<RTV3_E>();
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand("select \"Address\"||': '||ifnull(\"Street\",'')||' '||ifnull(\"Block\",'') ||' '||ifnull(\"City\",'')||' '||ifnull(\"County\",'')as \"Dir\", \"Block\",\"City\",\"County\", \"ZipCode\", \"Street\" ,\"Address2\"  from " + uti.schemaHana + "crd1 where \"CardCode\"='" + CardCode + "' and \"Address\" like 'ENV%' ORDER BY \"LineNum\" ", hcn);
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    RTV3_E obj = new RTV3_E();
                    if (!hdr.IsDBNull(0)) { obj.DirDestino = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { obj.Distrito = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { obj.Provincia = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { obj.Departamento = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { obj.Ubigeo = hdr.GetInt32(4); }
                    if (!hdr.IsDBNull(5)) { obj.Calle = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { obj.Zona = hdr.GetString(6); }
                    lista.Add(obj);
                }
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        private List<OrdenDeVenta_E> ListarOrdenesdeVenta(string fecha, string cardCode, int docNum)
        {
            var lista = new List<OrdenDeVenta_E>();
            using (var hcn = new HanaConnection(uti.cadHana))
            {
                try
                {
                    hcn.Open();
                    var query = $@"
                SELECT T0.""DocNum"",
                       S.""SlpName"",
                       T0.""DocTotal"",
                       L.""Name"",
                       T1.""WhsCode"",
                       P.""PymntGroup""
                FROM {uti.schemaHana}ORDR T0
                INNER JOIN {uti.schemaHana}RDR1 T1 ON T1.""DocEntry"" = T0.""DocEntry""
                INNER JOIN {uti.schemaHana}OSLP S ON S.""SlpCode"" = T0.""SlpCode""
                INNER JOIN {uti.schemaHana}""@COB_LUG_ENTREGA"" L ON L.""Code"" = T0.""U_COB_LUGAREN""
                INNER JOIN {uti.schemaHana}OCTG P ON P.""GroupNum"" = T0.""GroupNum""
                WHERE T0.""DocDate"" = '{fecha}'
                  AND T0.""CardCode"" = '{cardCode}'
                  AND T0.""Comments"" ='{docNum}'
                  AND T0.""CANCELED"" = 'N'
                GROUP BY T0.""DocEntry"", T0.""DocNum"", T0.""SlpCode"", T0.""DocTotal"",
                         T0.""U_COB_LUGAREN"", T1.""WhsCode"", T0.""DocDate"", T0.""CardCode"", T0.""GroupNum"",
                         S.""SlpName"", L.""Name"", P.""PymntGroup""
                ORDER BY T0.""DocDate"", T0.""CardCode"", T0.""DocEntry""";
                    var hcmd = new HanaCommand(query, hcn);
                    using (var hdr = hcmd.ExecuteReader())
                    {
                        while (hdr.Read())
                        {
                            var o = new OrdenDeVenta_E
                            {
                                DocNum = hdr.GetInt32(0),
                                CardCode = cardCode,
                                SlpName = hdr.GetString(1),
                                DocTotal = hdr.GetDecimal(2),
                                LugarDeEntrega = hdr.GetString(3),
                                AlmacenSalida = hdr.GetString(4),
                                TipoVenta = hdr.GetString(5) // Crédito, contado.
                            };
                            lista.Add(o);
                        }
                    }
                }
                catch
                {
                }
            }
            return lista;
        }
        private List<OrdenDeVenta_E> ListarOrdenesdeVentaFinales(string fecha, string cardCode, int docNum)
        {
            List<OrdenDeVenta_E> lista = new List<OrdenDeVenta_E>();
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                //Primero se realiza un foreach de la primera lista que obtenemos de HANA para obtener la lista general con los filtros enviados
                var ordenes = ListarOrdenesdeVenta(fecha, cardCode, docNum);
                foreach (OrdenDeVenta_E o in ordenes)
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "select COUNT(1) from vt.ORTV T0 inner join vt.RTV2 T1 on T1.DocEntry = T0.DocEntry " +
                        " where T0.Estado not in ('CANCELADO','ANULADO') and NroSap=@DocNum", cn)) // Consulta en la intranet tabla RTV2 para saber si la orden ya se uso.
                    {
                        cmd.Parameters.AddWithValue("@DocNum", o.DocNum);
                        int count = (int)cmd.ExecuteScalar();
                        if (count == 0)
                        {
                            lista.Add(o);
                        }
                    }
                }
            }
            return lista;
        }
        /***********************************************************************/
        //Método usa un procedure para buscar tickets vinculados, solo se usa en el REGISTRO Y EDICION de la hoja de repartos
        public List<string> BuscarVinculados(int DocEntry, int DocNum)
        {
            List<string> listaVinculados = new List<string>();
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("vt.TicketsVinculados", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@DocNum", DocNum);
                SqlDataReader dr = cmd.ExecuteReader();
                string vinculadosCadena;
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                    {
                        vinculadosCadena = dr.GetString(0);
                        string[] elementos = vinculadosCadena.Split(',');
                        foreach (string elemento in elementos)
                        {
                            // Agregar el elemento a la lista si no está vacío y no existe en la lista, para evitar valores repetidos en caso lo hayan vinculado mas de una vez.
                            if (!string.IsNullOrWhiteSpace(elemento) && !listaVinculados.Contains(elemento.Trim()))
                            {
                                listaVinculados.Add(elemento.Trim());
                            }
                        }
                    }
                }
                dr.Close(); cn.Close();
            }
            return listaVinculados;
        }
        /***********************************************************************/
        /************** METODOS PRICIPALES QUE GENERAN TRANSACCION *************/
        public int EditarTicketDesdeSeguimiento(Dictionary<string, Object> datos)
        {
            int docNum = -1;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@TipoMantenimiento", datos["tipoMantenimiento"]);
                cmd.Parameters.AddWithValue("@DocEntry", datos["docEntryTicket"]);
                cmd.Parameters.AddWithValue("@DocNum", datos["docNumTicket"]);
                cmd.Parameters.AddWithValue("@Estado", datos["estadoTicket"]);
                if (datos["tipoMantenimiento"].ToString() != "UDEMP")
                {
                    cmd.Parameters.AddWithValue("@Operario", datos["opRegistro"]);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@NroMesa", datos["NroMesa"]);
                    cmd.Parameters.AddWithValue("@Cajas", datos["Cajas"]);
                }
                /*se añadio para anular entregado diferente a Agencia*/
                if (datos["tipoMantenimiento"].Equals("USAT"))
                {
                    cmd.Parameters.AddWithValue("@LugarDestino", datos["LugarDestino"]);
                }
                cn.Open();
                SqlTransaction tran = null;
                try
                {
                    tran = cn.BeginTransaction();
                    cmd.Transaction = tran;
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    docNum = Convert.ToInt32(datos["docNumTicket"]);
                }
                catch (Exception ex)
                {
                    tran?.Rollback();
                    throw new Exception("Error al editar en estado: " + datos["estadoTicket"] + " " + ex.Message);
                }
                cn.Close();
            }
            return docNum;
        }
        public int EditarSeguimientoTicket(string Estado, int DocEntry, ORTV_E ticket)
        {
            int status = -1;
            string TipoMantenimiento = string.Empty;
            ORTV_E auxTK = ObtenerDatosCompletosTicket(DocEntry);
            if (Estado.Equals("RECIBIDO"))
            { TipoMantenimiento = "USRE"; } // update seguimiento recibir
            else if (Estado.Equals("ANULARRECIBIDO"))
            { TipoMantenimiento = "USAR"; } // update seguimiento anular recibir
            else if (Estado.Equals("INICIO PICKING"))
            { TipoMantenimiento = "UISSA"; } // update seguimiento inicio picking
            else if (Estado.Equals("ANULAR INICIO PICKING"))
            { TipoMantenimiento = "UISAS"; } // update seguimiento anular inicio picking
            else if (Estado.Equals("FIN PICKING"))
            { TipoMantenimiento = "USSA"; } // update seguimiento fin picking
            else if (Estado.Equals("ANULAR FIN PICKING"))
            { TipoMantenimiento = "USAS"; } // update seguimiento anular fin picking
            else if (Estado.Equals("INICIO VERIFICAR"))
            { TipoMantenimiento = "UISVE"; } // update seguimiento iniciar verificado
            else if (Estado.Equals("ANULAR INICIO VERIFICAR"))
            { TipoMantenimiento = "UISAV"; } // update seguimiento anular iniciar verificado
            else if (Estado.Equals("FIN VERIFICAR"))
            { TipoMantenimiento = "USVE"; } // update seguimiento fin verificado
            else if (Estado.Equals("ANULAR FIN VERIFICAR"))
            { TipoMantenimiento = "USAV"; } // update seguimiento anular fin verificado
            else if (Estado.Equals("INICIO EMPACAR"))
            { TipoMantenimiento = "UISEM"; } // update seguimiento iniciar empacado
            else if (Estado.Equals("ANULAR INICIO EMPACAR"))
            { TipoMantenimiento = "UISAE"; } // update seguimiento anular iniciar empacado
            else if (Estado.Equals("FIN EMPACAR"))
            {
                TipoMantenimiento = "USEM"; // update seguimiento fin empacado
                if (ticket.Det5 != null && ticket.Det5.Count >= 1 && ticket.LugarDestino == "Agencia")
                {
                    ticket.Det5[0].RegEstado = "Entregado";
                }
            }
            else if (Estado.Equals("ANULAR FIN EMPACAR"))
            { TipoMantenimiento = "USAE"; } // update seguimiento anular fin empacado
            else if (Estado.Equals("PESADO"))
            { TipoMantenimiento = "USPE"; } // update seguimiento pesado
            else if (Estado.Equals("ANULARPESADO"))
            { TipoMantenimiento = "USAP"; } // update seguimiento anular pesado
            else if (Estado.Equals("ENVIADO"))
            { TipoMantenimiento = "USEN"; } // update seguimiento enviado
            else if (Estado.Equals("ANULARENVIADO"))
            { TipoMantenimiento = "USAN"; } // update seguimiento anular enviado
            else if (Estado.Equals("ENTREGADO"))
            { TipoMantenimiento = "USET"; } // update seguimiento entregado
            else if (Estado.Equals("ANULARENTREGADO"))
            { TipoMantenimiento = "USAT"; } // update seguimiento anular entregado
            SqlConnection cn = new SqlConnection(uti.cadSql);
            cn.Open();
            SqlTransaction tran = cn.BeginTransaction("ACTUALIZAR DESDE SEGUIMIENTO");
            try
            {
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn, tran) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@TipoMantenimiento", TipoMantenimiento);
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@DocNum", ticket.DocNum);
                cmd.Parameters.AddWithValue("@Estado", ticket.Estado);
                cmd.Parameters.AddWithValue("@Operario", ticket.OpRegistro);
                // Detalles de OpSacando cuando existe más de 1 sacador
                if (TipoMantenimiento == "USSA")
                {
                    if (ticket.Det11 != null && ticket.Det11.Count > 1 && !string.IsNullOrWhiteSpace(ticket.Det11[1].Operario))
                    {
                        ticket.Det11.RemoveAt(0);                           // Eliminamos el primer elemento porque este ya fue guardado en CC_ORTV
                        cmd.Parameters.AddWithValue("@MasOperariosSac", 1);
                        SqlParameter tbDet11 = new SqlParameter("@TPRTV11", SqlDbType.Structured);
                        tbDet11.Value = RTV11_E.tbDet11(ticket.Det11, ticket.DocEntry);
                        tbDet11.TypeName = "vt.TPRTV11";
                        cmd.Parameters.AddWithValue("@TPRTV11", tbDet11.Value);
                    }
                }
                else if (TipoMantenimiento == "UISVE")
                {
                    string EstadoNuevo = "PICKEANDO";
                    var UltimaOperacion = _ccTicketD.ListarCC_ORTV(ticket.DocEntry, null, true).FirstOrDefault().Operacion;
                    if (UltimaOperacion == "FIN PICKING") { EstadoNuevo = "VERIFICANDO"; }
                    cmd.Parameters.AddWithValue("@EstadoNuevo", EstadoNuevo);
                }
                else if (TipoMantenimiento.Equals("USVE"))
                {
                    //si tiene el dato de productos pendientes se manda como parametro para actualizar vt.BusquedaProducto
                    cmd.Parameters.AddWithValue("@ProductosPendientes", ticket.ProductoPendiente);
                    // Verificadores de apoyo
                    if (ticket.Det12 != null && ticket.Det12.Count > 1 && !String.IsNullOrWhiteSpace(ticket.Det12[1].Operario))
                    {
                        ticket.Det12.RemoveAt(0);
                        cmd.Parameters.AddWithValue("@MasOperariosChe", 1);
                        SqlParameter tbDet12 = new SqlParameter("@TPRTV12", SqlDbType.Structured);
                        tbDet12.Value = RTV12_E.tbDet12(ticket.Det12, ticket.DocEntry);
                        tbDet12.TypeName = "vt.TPRTV12";
                        cmd.Parameters.AddWithValue("@TPRTV12", tbDet12.Value);
                    }
                }
                else if (TipoMantenimiento == "UISEM")
                {
                    string EstadoNuevo = auxTK.Estado;
                    var UltimaOperacion = _ccTicketD.ListarCC_ORTV(ticket.DocEntry, null, true).FirstOrDefault().Operacion;
                    if (UltimaOperacion == "FIN VERIFICAR") { EstadoNuevo = "EMPACANDO"; }
                    if (UltimaOperacion == "FIN PICKING") { EstadoNuevo = "VERIFICANDO"; }
                    cmd.Parameters.AddWithValue("@EstadoNuevo", EstadoNuevo);
                }
                else if (TipoMantenimiento.Equals("USAE"))
                {
                    string EstadoNuevo = string.Empty;
                    if (!string.IsNullOrWhiteSpace(ticket.Operario) && ticket.Operario == "07")
                    {
                        EstadoNuevo = "PICKEANDO";
                    }
                    else { EstadoNuevo = "EMPACANDO"; }
                    cmd.Parameters.AddWithValue("@EstadoNuevo", EstadoNuevo);
                    if (!string.IsNullOrWhiteSpace(ticket.Operario) && ticket.Operario == "07")
                    { cmd.Parameters.AddWithValue("@Almacen", ticket.Operario); }
                }
                //  Párametros enviados solo cuando Empacamos ticket
                else if (TipoMantenimiento.Equals("USEM"))
                {
                    // update seguimiento empacado
                    cmd.Parameters.AddWithValue("@Cajas", ticket.Cajas);
                    cmd.Parameters.AddWithValue("@NroMesa", ticket.NroMesa);
                    cmd.Parameters.AddWithValue("@AlmProcedencia", ticket.AlmProcedencia);
                    if (!string.IsNullOrWhiteSpace(ticket.Operario) && ticket.Operario == "07")
                    { cmd.Parameters.AddWithValue("@Almacen", ticket.Operario); }
                    if (ticket.Det13 != null && ticket.Det13.Count > 1)
                    {
                        if (!string.IsNullOrWhiteSpace(ticket.Det13[1].Operario))
                        {
                            // Empacadores de apoyo
                            ticket.Det13.RemoveAt(0);                           // Eliminamos el primer elemento porque este ya fue guardado en CC_ORTV
                            cmd.Parameters.AddWithValue("@MasOperariosEmp", 1);
                            SqlParameter tbDet13 = new SqlParameter("@TPRTV13", SqlDbType.Structured);
                            tbDet13.Value = RTV13_E.tbDet13(ticket.Det13, ticket.DocEntry);
                            tbDet13.TypeName = "vt.TPRTV13";
                            cmd.Parameters.AddWithValue("@TPRTV13", tbDet13.Value);
                        }
                    }
                }
                // datos de pesos
                else if (TipoMantenimiento.Equals("USPE") && ticket.Det6 != null && ticket.Det6.Count > 0)
                {
                    SqlParameter tbDet6 = new SqlParameter("@TPRTV6", SqlDbType.Structured);
                    tbDet6.Value = RTV6_E.GenerarDataTable(ticket.Det6, ticket.DocEntry);
                    tbDet6.TypeName = "vt.TPRTV6";
                    cmd.Parameters.AddWithValue("@TPRTV6", tbDet6.Value);
                }
                /*se añadio para anular entregado diferente a Agencia*/
                else if (TipoMantenimiento.Equals("USAT"))
                {
                    cmd.Parameters.AddWithValue("@LugarDestino", ticket.LugarDestino);
                }
                // Ejecutar comando para modificar el estado del ticket
                cmd.ExecuteNonQuery();
                status = ticket.DocNum;
                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw new Exception("Error al editar en estado =>" + Estado + " " + e.Message);
            }
            finally
            {
                cn.Close();
            }
            return status;
        }
        public int RegistrarImpresionTicket(int docEntry, string operario, string area)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO vt.CC_ORTV_print VALUES (@DocEntry,getdate(),@Area,@Operario)", cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DocEntry", docEntry);
                cmd.Parameters.AddWithValue("@Operario", operario);
                cmd.Parameters.AddWithValue("@Area", area);
                cmd.ExecuteNonQuery();
                cn.Close();
            }
            catch { }
            return docEntry;
        }
        public void EditarTicketSup(int DocEntry, ORTV_E ticket)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                string ZonaTk = string.Empty;
                cn.Open();
                switch (ticket.LugarDestino)
                {
                    case "Agencia":
                    case "Agencia Courier":
                        ZonaTk = "AGENCIA";
                        break;
                    case "Centro":
                        ZonaTk = "CONO CENTRO";
                        break;
                    case "Arriola":
                        ZonaTk = "ARRIOLA";
                        break;
                    case "Domicilio":
                        ZonaTk = ticket.Zona;
                        break;
                }
                using (SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "USUPV");
                    cmd.Parameters.AddWithValue("@Estado", ticket.Estado);
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", ticket.DocNum);
                    cmd.Parameters.AddWithValue("@TipoVenta", ticket.TipoVenta);
                    cmd.Parameters.AddWithValue("@DirDestino", ticket.DirDestino);
                    cmd.Parameters.AddWithValue("@Zona", ZonaTk);
                    cmd.Parameters.AddWithValue("@FormaPago", ticket.FormaPago);
                    cmd.Parameters.AddWithValue("@Observaciones", ticket.Observaciones);
                    cmd.Parameters.AddWithValue("@Observaciones2", ticket.Observaciones2);
                    cmd.Parameters.AddWithValue("@Observaciones3", ticket.Observaciones3);
                    cmd.Parameters.AddWithValue("@Operario", ticket.OpRegistro);
                    cmd.Parameters.AddWithValue("@LugarDestino", ticket.LugarDestino);
                    cmd.Parameters.AddWithValue("@Referencia", ticket.Referencia);
                    cmd.Parameters.AddWithValue("@AlmProcedencia", ticket.AlmProcedencia);
                    if (ticket.LugarDestino.Equals("Agencia") || ticket.LugarDestino.Equals("Agencia Courier"))
                    {
                        cmd.Parameters.AddWithValue("@Agencia", ticket.Agencia);
                        cmd.Parameters.AddWithValue("@EnvioAgencia", ticket.EnvioAgencia);
                    }
                    if (ticket.Estado.Equals("ABIERTO"))
                    {
                        cmd.Parameters.AddWithValue("@TiempoEntrega", ticket.TiempoEntrega);
                    }
                    if (ticket.Det1 != null && ticket.Det1.Count >= 1)
                    {
                        SqlParameter tbDet1 = new SqlParameter("@TPRTV1", SqlDbType.Structured);
                        tbDet1.Value = RTV1_E.GenerarDataTable(ticket.Det1, ticket.DocEntry);
                        tbDet1.TypeName = "vt.TPRTV1";
                        cmd.Parameters.AddWithValue("@TPRTV1", tbDet1.Value);
                    }
                    if (ticket.Det2 != null && ticket.Det2.Count >= 1)
                    {
                        SqlParameter tbDet2 = new SqlParameter("@TPRTV2", SqlDbType.Structured);
                        tbDet2.Value = RTV2_E.GenerarDataTable(ticket.Det2, ticket.DocEntry);
                        tbDet2.TypeName = "vt.TPRTV2";
                        cmd.Parameters.AddWithValue("@TPRTV2", tbDet2.Value);
                    }
                    if (ticket.Det3 != null && ticket.Det3.Count >= 1)
                    {
                        SqlParameter tbDet3 = new SqlParameter("@TPRTV3", SqlDbType.Structured);
                        tbDet3.Value = RTV3_E.GenerarDataTable(ticket.Det3, ticket.DocEntry);
                        tbDet3.TypeName = "vt.TPRTV3";
                        cmd.Parameters.AddWithValue("@TPRTV3", tbDet3.Value);
                    }
                    // Si el ticket tiene vinculación "SI" en su campo Observaciones2 se mandan los datos de RTV7
                    if (ticket.Observaciones2 == "SI" && ticket.Det7 != null && ticket.Det7.Count >= 1)
                    {
                        SqlParameter tbDet7 = new SqlParameter("@TPRTV7", SqlDbType.Structured);
                        tbDet7.Value = RTV7_E.GenerarDataTable(ticket.Det7, ticket.DocEntry);
                        tbDet7.TypeName = "vt.TPRTV7";
                        cmd.Parameters.AddWithValue("@TPRTV7", tbDet7.Value);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error en edición: " + e.Message);
            }
        }
        /**********************************************************************/
        /******************** METODOS PRINCIPALES EN MODULOS ******************/
        public int PagarTicket(int DocEntry, ORTV_E ticket)
        {   // tipo UPG update pago
            int status = -1;
            ORTV_E auxTK = ObtenerDatosCompletosTicket(DocEntry);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UPG");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", ticket.DocNum);
                    cmd.Parameters.AddWithValue("@FormaPago", ticket.FormaPago);
                    cmd.Parameters.AddWithValue("@MontoRecibido", ticket.MontoRecibido);
                    cmd.Parameters.AddWithValue("@CodSapCajero", ticket.CodSapCajero);
                    cmd.Parameters.AddWithValue("@Cajero", ticket.Cajero);
                    cmd.Parameters.AddWithValue("@Estado", ticket.Estado);
                    cmd.ExecuteNonQuery();
                    status = ticket.DocNum;
                    if (auxTK.CardCode != null && auxTK.CardName != null)
                    {
                        SqlCommand cmd2 = new SqlCommand("vt.MANT_OLDS", cn);
                        cmd2.Transaction = tran;
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@TipoMantenimiento", "AC");           // AC: ADD CABECERA
                        cmd2.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                        cmd2.Parameters.AddWithValue("@C_CardName", auxTK.CardName);
                        cmd2.ExecuteNonQuery();
                        if (auxTK.GastoEnvio > 0)
                        {
                            SqlCommand cmd3 = new SqlCommand("vt.MANT_OLDS", cn);
                            cmd3.Transaction = tran;
                            cmd3.CommandType = CommandType.StoredProcedure;
                            cmd3.Parameters.AddWithValue("@TipoMantenimiento", "AD");           // AD: ADD DETALLES
                            cmd3.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd3.Parameters.AddWithValue("@FechaOpe", auxTK.FechaSapTicket);
                            cmd3.Parameters.AddWithValue("@Operacion", ticket.FormaPago);
                            cmd3.Parameters.AddWithValue("@DetOpe", ticket.FormaPago + " GastoEnvio, ticket:" + auxTK.DocNum + " MR:" + ticket.MontoRecibido);
                            cmd3.Parameters.AddWithValue("@Ingreso", auxTK.GastoEnvio);
                            cmd3.Parameters.AddWithValue("@OperarioRegistro", ticket.Cajero);
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
        public int AnularPagoTicket(int DocEntry)
        {   //TIPO MANT UAPG UPDATE ANULAR PAGO
            int status = -1;
            ORTV_E auxTK = ObtenerDatosCompletosTicket(DocEntry);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UAPG");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", auxTK.DocNum);
                    cmd.Parameters.AddWithValue("@Estado", auxTK.Estado);
                    cmd.ExecuteNonQuery();
                    //status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
                    status = auxTK.DocNum;
                    if (auxTK.CardCode != null && auxTK.CardName != null)
                    {
                        SqlCommand cmd2 = new SqlCommand("vt.MANT_OLDS", cn);
                        cmd2.Transaction = tran;
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@TipoMantenimiento", "AC");       // ADD CABECERA
                        cmd2.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                        cmd2.Parameters.AddWithValue("@C_CardName", auxTK.CardName);
                        cmd2.ExecuteNonQuery();
                        if (auxTK.GastoEnvio > 0)
                        {
                            SqlCommand cmd3 = new SqlCommand("vt.MANT_OLDS", cn);
                            cmd3.Transaction = tran;
                            cmd3.CommandType = CommandType.StoredProcedure;
                            cmd3.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd3.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd3.Parameters.AddWithValue("@FechaOpe", auxTK.FechaPago);
                            cmd3.Parameters.AddWithValue("@Operacion", "ANULACIONPAGO");
                            cmd3.Parameters.AddWithValue("@DetOpe", "ANULACIONPAGO GastoEnvio, ticket:" + auxTK.DocNum + " MR:" + auxTK.MontoRecibido);
                            cmd3.Parameters.AddWithValue("@Egreso", auxTK.GastoEnvio);
                            cmd3.Parameters.AddWithValue("@OperarioRegistro", auxTK.Vendedor);
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
        public List<Rpt_TicketVenta_E> ListarTicketsAgencia()
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
                    if (!dr.IsDBNull(0)) { t.Almacen = dr.GetString(0); }
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
            catch { }
            return lista;
        }
        public int Cancelar(int DocEntry, string Estado, string Operario)
        {   //tipo mant UAN
            int status = -1;
            ORTV_E auxTK = ObtenerDatosCompletosTicket(DocEntry);
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UCA");           // update cancelar ticket
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", auxTK.DocNum);
                    cmd.Parameters.AddWithValue("@Operario", Operario);
                    cmd.Parameters.AddWithValue("@Estado", Estado);
                    cmd.ExecuteNonQuery();
                    status = auxTK.DocNum;
                    if (auxTK.CardCode != null && auxTK.CardName != null)
                    {
                        SqlCommand cmd2 = new SqlCommand("vt.MANT_OLDS", cn);
                        cmd2.Transaction = tran;
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@TipoMantenimiento", "AC");
                        cmd2.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                        cmd2.Parameters.AddWithValue("@C_CardName", auxTK.CardName);
                        cmd2.ExecuteNonQuery();
                        if (auxTK.DeudaCliente > 0)
                        {
                            SqlCommand cmd4 = new SqlCommand("vt.MANT_OLDS", cn);
                            cmd4.Transaction = tran;
                            cmd4.CommandType = CommandType.StoredProcedure;
                            cmd4.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd4.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd4.Parameters.AddWithValue("@FechaOpe", auxTK.FechaSapTicket);
                            cmd4.Parameters.AddWithValue("@Operacion", "ANULACIONVENTA");
                            cmd4.Parameters.AddWithValue("@DetOpe", "ANULACIONVENTA DeudaCliente, ticket:" + auxTK.DocNum + " MR:" + auxTK.MontoFinal);
                            cmd4.Parameters.AddWithValue("@Egreso", auxTK.DeudaCliente);
                            cmd4.Parameters.AddWithValue("@OperarioRegistro", Operario);
                            cmd4.ExecuteNonQuery();
                        }
                        if (auxTK.DeudaEmpresa > 0)
                        {
                            SqlCommand cmd5 = new SqlCommand("vt.MANT_OLDS", cn);
                            cmd5.Transaction = tran;
                            cmd5.CommandType = CommandType.StoredProcedure;
                            cmd5.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                            cmd5.Parameters.AddWithValue("@C_CardCode", auxTK.CardCode);
                            cmd5.Parameters.AddWithValue("@FechaOpe", auxTK.FechaSapTicket);
                            cmd5.Parameters.AddWithValue("@Operacion", "ANULACIONVENTA");
                            cmd5.Parameters.AddWithValue("@DetOpe", "ANULACION-SALIDASALDO DeudaEmpresa, ticket:" + auxTK.DocNum + " MR:" + auxTK.MontoFinal);
                            cmd5.Parameters.AddWithValue("@Ingreso", auxTK.DeudaEmpresa);
                            cmd5.Parameters.AddWithValue("@OperarioRegistro", Operario);
                            cmd5.ExecuteNonQuery();
                        }
                    }
                    if (status >= 1)
                    {
                        //Operaciones para regalos
                        if (auxTK.Det5 != null && auxTK.Det5.Count >= 1)
                        {
                            if (auxTK.Det5[0].IdReg > 0 && auxTK.Det5[0].RegCant > 0)
                            {
                                OREG_D oregD = new OREG_D();
                                auxTK.Det5[0].RegCant = -1 * auxTK.Det5[0].RegCant;
                                oregD.CompromisosStock(new List<ORTV_E> { auxTK }, tran);
                            }
                        }
                    }
                    tran.Commit();
                    cn.Close();
                }
                catch (Exception e1) { tran.Rollback(); cn.Close(); throw new Exception("error y anulacion: " + e1.Message); }
            }
            catch (Exception e2) { status = 0; cn.Close(); throw new Exception("Error en anulacion: " + e2.Message); }
            return status;
        }
        /*
		 * Descripción: Método para editar el estado del ticket desde el view SeguimientoDeTicket
		 * Parámetros: (int) @docEntry, (string) @tipoMantenimiento
		 * Usos: SeguimientoDeTicket
		 */
        public int EmitirGuia(int DocEntry, Usuario_E u)
        {
            int status = -1;
            ORTV_E t;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                t = ObtenerDatosCompletosTicket(DocEntry);
                cn.Open();
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "USGE");
                cmd.Parameters.AddWithValue("@Estado", t.Estado);
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@OpFacturacion", $"{u.Nombres} {u.Apellidos}");
                cmd.ExecuteNonQuery();
                cn.Close();
                status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en emitir guia: " + e.Message); }
            return status;
        }
        public int RevertirGuiasTicket(int DocEntry, string operario)
        {
            int status = -1;
            ORTV_E t;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                t = ObtenerDatosCompletosTicket(DocEntry);
                cn.Open();
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "URVG");
                cmd.Parameters.AddWithValue("@Estado", t.Estado);
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@Operario", operario);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                cn.Close();
                status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en revertir guias: " + e.Message); }
            return status;
        }
        public int FacturarTicket(int DocEntry, Usuario_E u)
        {
            int status = -1;
            ORTV_E t;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                t = ObtenerDatosCompletosTicket(DocEntry);
                cn.Open();
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "USFC");
                cmd.Parameters.AddWithValue("@Estado", t.Estado);
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@OpFacturacion", $"{u.Nombres} {u.Apellidos}");
                cmd.ExecuteNonQuery();
                cn.Close();
                status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en facturacion: " + e.Message); }
            return status;
        }
        public int RevertirFacturarTicket(int DocEntry, string operario)
        {
            int status = -1;
            ORTV_E t;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                t = ObtenerDatosCompletosTicket(DocEntry);
                cn.Open();
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "UAFC");
                cmd.Parameters.AddWithValue("@Estado", t.Estado);
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@Operario", operario);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                cn.Close();
                status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
            }
            catch (Exception e) { status = 0; cn.Close(); throw new Exception("Error en AnFact: " + e.Message); }
            return status;
        }
        public List<ORIN_E> ListarNotasDeCreditoV(string CardCode)
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
                    if (obtenerDet4Ticket(0, n.DocNum).Count == 0)
                    {
                        lista.Add(n);
                    }
                }
                hdr.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        //metodos para reportes
        private List<ORTV_E> RptAnalisisVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnalisisVentas_E obj)
        {
            List<ORTV_E> lista = new List<ORTV_E>();
            string fil = string.Empty;
            // Operarios que sus acciones se encuentran en la tabla CC_ORTV
            string[] operarios = { "SEPARAR", "REGISTRAR", "RECIBIR", "INICIO PICKING", "FIN PICKING", "INICIO VERIFICAR", "FIN VERIFICAR", "INICIO EMPACAR", "FIN EMPACAR", "PESAR", "ENVIAR", "ENTREGAR", "ANULAR", "CANCELAR" };
            int indice = Array.IndexOf(operarios, obj.TipoOperario);
            if (!string.IsNullOrWhiteSpace(obj.AlmIni) && !string.IsNullOrWhiteSpace(obj.AlmFin)) { fil += " AND (select top 1 y.LugarDeEntrega from vt.RTV2 y where y.DocEntry=t0.DocEntry) between @AlmIni and @AlmFin"; }
            if (!string.IsNullOrWhiteSpace(obj.FecIni) && !string.IsNullOrWhiteSpace(obj.FecFin)) { fil += " AND t0.FechaSapTicket between @FecIni and @FecFin"; }
            if (!string.IsNullOrWhiteSpace(obj.CardCode)) { fil += " AND t0.CardCode=@CardCode"; }
            if (!string.IsNullOrWhiteSpace(obj.LugarDestino)) { fil += " AND t0.LugarDestino=@LugarDestino"; }
            if (!string.IsNullOrWhiteSpace(obj.FormaPago)) { fil += " AND t0.FormaPago=@FormaPago"; }
            if (obj.TipoOperario != null)
            {
                // Si el Tipooperario se encuentra dentro de los valors de @operarios
                if (indice != -1)
                {
                    fil += " AND (SELECT TOP 1 Operario from vt.CC_ORTV where Operacion ='" + obj.TipoOperario + "' and DocEntry=t0.DocEntry order by concat(FechaOperacion,HoraOperacion)  desc )  like concat('%',@Operario,'%')";
                }
                else
                {
                    fil += " AND t0." + obj.TipoOperario + " LIKE CONCAT('%',@Operario,'%')";
                }
            }
            string query = "SELECT t0.DocNum, t0.FechaSapTicket, t0.CardCode, t0.CardName, (SELECT TOP 1 HoraOperacion from vt.CC_ORTV where DocEntry=t0.DocEntry and Operacion='REGISTRAR' order by FechaOperacion,HoraOperacion DESC)" +
                                    ",t0.TipoVenta, t0.Embalaje, t0.Vendedor, t0.MontoTotal, t0.Flete, t0.GastoEnvio, t0.DescuentoNC, t0.DeudaCliente" +
                                    ",t0.DeudaEmpresa, t0.MontoFinal, (SELECT STUFF((SELECT ', ' + cast(t1.NroSap as varchar(max)) FROM vt.RTV2 t1 " +
                                    "INNER JOIN  vt.ORTV x ON x.DocEntry = t1.DocEntry WHERE t1.DocEntry = t0.DocEntry " +
                                    "FOR XML PATH('')), 1,2, '')) as 'NroVentas'  " +
                                    ",(select top 1 y.Vendedor from vt.RTV2 y where y.DocEntry=t0.DocEntry) as 'VendedorSap', t0.FormaPago, t0.EstadoPago" +
                                    ",(select top 1 y.LugarDeEntrega from vt.RTV2 y where y.DocEntry=t0.DocEntry) as 'LugEntregaSap'" +
                                " FROM vt.ORTV t0 where t0.DocEntry>0 " + fil;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                if (!string.IsNullOrWhiteSpace(obj.AlmIni)) { cmd.Parameters.AddWithValue("@AlmIni", obj.AlmIni); }
                if (!string.IsNullOrWhiteSpace(obj.AlmFin)) { cmd.Parameters.AddWithValue("@AlmFin", obj.AlmFin); }
                if (!string.IsNullOrWhiteSpace(obj.FecIni)) { cmd.Parameters.AddWithValue("@FecIni", obj.FecIni); }
                if (!string.IsNullOrWhiteSpace(obj.FecFin)) { cmd.Parameters.AddWithValue("@FecFin", obj.FecFin); }
                if (!string.IsNullOrWhiteSpace(obj.CardCode)) { cmd.Parameters.AddWithValue("@CardCode", obj.CardCode); }
                if (!string.IsNullOrWhiteSpace(obj.LugarDestino)) { cmd.Parameters.AddWithValue("@LugarDestino", obj.LugarDestino); }
                if (!string.IsNullOrWhiteSpace(obj.FormaPago)) { cmd.Parameters.AddWithValue("@FormaPago", obj.FormaPago); }
                if (!string.IsNullOrWhiteSpace(obj.Operario)) { cmd.Parameters.AddWithValue("@Operario", obj.Operario); }
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ORTV_E o = new ORTV_E();
                    if (!dr.IsDBNull(0)) { o.DocNum = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.FechaSapTicket = dr.GetDateTime(1).ToString("dd/MM/yyyy"); }
                    if (!dr.IsDBNull(2)) { o.CardCode = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.CardName = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.HoraRegistro = dr.GetTimeSpan(4).ToString(); }
                    if (!dr.IsDBNull(5)) { o.TipoVenta = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { o.Embalaje = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { o.Vendedor = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { o.MontoTotal = dr.GetDecimal(8); }
                    if (!dr.IsDBNull(9)) { o.Flete = dr.GetDecimal(9); }
                    if (!dr.IsDBNull(10)) { o.GastoEnvio = dr.GetDecimal(10); }
                    if (!dr.IsDBNull(11)) { o.DescuentoNC = dr.GetDecimal(11); }
                    if (!dr.IsDBNull(12)) { o.DeudaCliente = dr.GetDecimal(12); }
                    if (!dr.IsDBNull(13)) { o.DeudaEmpresa = dr.GetDecimal(13); }
                    if (!dr.IsDBNull(14)) { o.MontoFinal = dr.GetDecimal(14); }
                    if (!dr.IsDBNull(15)) { o.NroVentas = dr.GetString(15); }
                    if (!dr.IsDBNull(16)) { o.VendedorSap = dr.GetString(16); }
                    if (!dr.IsDBNull(17)) { o.FormaPago = dr.GetString(17); }
                    if (!dr.IsDBNull(18)) { o.EstadoPago = dr.GetString(18); }
                    if (!dr.IsDBNull(19)) { o.LugEntrega = dr.GetString(19); }
                    lista.Add(o);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public System.Data.DataTable TbRptAnalisisVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnalisisVentas_E obj)
        {
            List<string> campos = new List<string>();
            List<Type> tipos = new List<Type>();
            campos.Add("Orden"); tipos.Add(typeof(string));
            campos.Add("DocNum"); tipos.Add(typeof(string));
            campos.Add("FechaRegistro"); tipos.Add(typeof(string));
            campos.Add("CardCode"); tipos.Add(typeof(string));
            campos.Add("CardName"); tipos.Add(typeof(string));
            campos.Add("HoraRegistro"); tipos.Add(typeof(string));
            campos.Add("TipoVenta"); tipos.Add(typeof(string));
            campos.Add("Embalaje"); tipos.Add(typeof(string));
            campos.Add("Vendedor"); tipos.Add(typeof(string));
            campos.Add("MontoTotal"); tipos.Add(typeof(string));
            campos.Add("Flete"); tipos.Add(typeof(string));
            campos.Add("GastoEnvio"); tipos.Add(typeof(string));
            campos.Add("DescuentoNC"); tipos.Add(typeof(string));
            campos.Add("DeudaCliente"); tipos.Add(typeof(string));
            campos.Add("DeudaEmpresa"); tipos.Add(typeof(string));
            campos.Add("MontoFinal"); tipos.Add(typeof(string));
            campos.Add("NroVentas"); tipos.Add(typeof(string));
            campos.Add("VendedorSap"); tipos.Add(typeof(string));
            campos.Add("FormaPago"); tipos.Add(typeof(string));
            campos.Add("EstadoPago"); tipos.Add(typeof(string));
            campos.Add("LugEntregaSap"); tipos.Add(typeof(string));
            DataTable tb = DefinirTabla(campos, tipos, "DataTableReporteAnalisisVentas");
            int i = 0;
            foreach (ORTV_E p in RptAnalisisVentas(obj))
            {
                DataRow row = tb.NewRow();
                row["Orden"] = i++;
                row["DocNum"] = p.DocNum;
                row["FechaRegistro"] = p.FechaSapTicket;
                row["CardCode"] = p.CardCode;
                row["CardName"] = p.CardName;
                row["HoraRegistro"] = p.HoraRegistro;
                row["TipoVenta"] = p.TipoVenta;
                row["Embalaje"] = p.Embalaje;
                row["Vendedor"] = p.Vendedor;
                row["MontoTotal"] = p.MontoTotal;
                row["Flete"] = p.Flete;
                row["GastoEnvio"] = p.GastoEnvio;
                row["DescuentoNC"] = p.DescuentoNC;
                row["DeudaCliente"] = p.DeudaCliente;
                row["DeudaEmpresa"] = p.DeudaEmpresa;
                row["MontoFinal"] = p.MontoFinal;
                row["NroVentas"] = p.NroVentas;
                row["VendedorSap"] = p.VendedorSap;
                row["FormaPago"] = p.FormaPago;
                row["EstadoPago"] = p.EstadoPago;
                row["LugEntregaSap"] = p.LugEntrega;
                tb.Rows.Add(row);
            }
            return tb;
        }
        // Método para listar los números de tickets en el input "DocNumTicket" en el view "Registrar solicitud" en AtencionCliente
        public List<ORTV_E> ListarTicketsParaAtencion()
        {
            List<ORTV_E> lista = new List<ORTV_E>();
            String select = "DocEntry, DocNum, CardCode, CardName, LugarDestino, FechaFacturacion";
            String query = $"SELECT {select} from vt.ORTV WHERE Estado = 'ENTREGADO' and FechaSapTicket >=DATEADD(DAY,-25,getdate()) ORDER BY DocNum DESC";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
            cn.Open();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        ORTV_E ticket = new ORTV_E();
                        if (!dr.IsDBNull(0)) { ticket.DocEntry = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { ticket.DocNum = dr.GetInt32(1); }
                        if (!dr.IsDBNull(2)) { ticket.CardCode = dr.GetString(2); }
                        if (!dr.IsDBNull(3)) { ticket.CardName = dr.GetString(3); }
                        if (!dr.IsDBNull(4)) { ticket.LugarDestino = dr.GetString(4); }
                        if (!dr.IsDBNull(5)) { ticket.FechaFacturacion = dr.GetDateTime(5).ToString("dd/MM/yyyy"); }
                        lista.Add(ticket);
                    }
                }
                dr.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
            cn.Close();
            return lista;
        }
        public ORTV_E CalcularMontos(ORTV_E t)
        {
            t.MontoTotal = (decimal)(0.00);
            t.MontoFinal = (decimal)(0.00);
            if (t.Det2 != null)
            {
                foreach (RTV2_E d2 in t.Det2)
                {
                    if (d2.Verificar == "on")
                    {
                        t.MontoTotal += d2.Monto;
                    }
                }
            }
            if (t.Det4 != null)
            {
                t.DescuentoNC = 0.00M;
                foreach (RTV4_E d4 in t.Det4)
                {
                    if (d4.Verificar == "on")
                    {
                        t.DescuentoNC += d4.Nc.DocTotal;
                    }
                }
            }
            t.MontoFinal += t.MontoTotal + t.Flete + t.GastoEnvio + t.DeudaCliente - t.DescuentoNC - t.DeudaEmpresa;
            return t;
        }
        public ORTV_E CalcularPesoTotal(ORTV_E t)
        {
            t.PesoTotal = (decimal)(0.00);
            if (t.Det6 != null)
            {
                foreach (RTV6_E d in t.Det6)
                {
                    if (d.Verificar == "on")
                    {
                        t.PesoTotal += d.Peso;
                    }
                }
            }
            return t;
        }
        public void ConfGastEnvio(ORTV_E o)
        {
            string query = "update vt.ORTV set EstadoGasto='CONFIRMADO',PagoEnv=@PagoEnv where DocEntry=@DocEntry";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("TR02");
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn, tran);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry);
                    cmd.Parameters.AddWithValue("@PagoEnv", o.PagoEnv);
                    cmd.ExecuteNonQuery();
                    _oldsD.agregarLDS1(new LDS1_E
                    {
                        CardCode = o.CardCode,
                        FechaOpe = DateTime.Now.ToString("yyyy-MM-dd"),
                        Operacion = "ConfEnvio",
                        DetOpe = "Tk: " + o.DocNum + " " + o.DetOpe,
                        Egreso = o.PagoEnv,
                        OperarioReg = o.OpRegistro
                    }, tran, cn);
                    tran.Commit();
                    cn.Close();
                }
                catch { tran.Rollback(); cn.Close(); }
            }
            catch { cn.Close(); }
        }
        DataTable DefinirTabla(List<string> campos, List<Type> tipos, string nombre)
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
        public string GeneraInfoListaClientes(string Fecha)
        {
            string info = "<datalist id='ListaClientes'>";
            foreach (OCRD_E x in listarClientes(Fecha))
            {
                info += "<option CardCode='" + x.CardCode + "' value='" + x.CardName + "'></option>";
            }
            info += "</datalist>";
            return info;
        }
        public string GeneraInfoListaDirDestinos(string CardCode)
        {
            string info = "<option value=''>Seleccione</option>";
            foreach (RTV3_E x in listarDirDestinos(CardCode))
            {
                info += "<option  value ='" + x.DirDestino + "' Zona='" + x.Zona + "' Distrito='" + x.Distrito + "' Provincia='" + x.Provincia + " 'Departamento='" + x.Departamento + "' Ubigeo='" + x.Ubigeo + "' Calle='" + x.Calle + "'>" + x.DirDestino + "</option>";
            }
            return info;
        }
        public (string Persona, string Documento) ObtenerPersonaRecojoParaGuia(int docNum)
        {
            string Persona = ""; string Documento = "";
            int docEntry = DocEntryTicket(docNum);
            var tk = ObtenerTicketVenta(docEntry);
            if (tk.LugarDestino.Equals("Domicilio") || tk.LugarDestino.Equals("Agencia"))
            {
                List<RTV1_E> rtv1 = obtenerDet1Ticket(docEntry);
                Persona = rtv1[0].NombrePer;
                Documento = rtv1[0].DocPer;
            }
            return (Persona, Documento);
        }
        public (string HtmlContent, string TipoVenta) GeneraInfoListaOrdenesDeVenta(string fecha, string cardCode, int docNum)
        {
            string info = string.Empty;
            int linea = 1;
            List<OrdenDeVenta_E> lista = ListarOrdenesdeVentaFinales(fecha, cardCode, docNum);
            string tipoVenta = lista.Where(x => x.DocTotal != 0).Select(x => x.TipoVenta).Distinct().SingleOrDefault();
            //Verifica si existe un solo TipoVenta en todas las órdenes relacionadas
            if (tipoVenta != null && lista.Where(x => x.DocTotal != 0).Select(x => x.TipoVenta).Distinct().Count() == 1)
            {
                info += "<thead class='bg-cobefar text-white'><tr><th class='text-center'>#</th><th class='text-center'>VER</th><th class='text-center'>Monto</th>" +
                              "<th class='text-center'>Nro SAP</th><th class='text-center'>Tipo Comprobante</th><th class='text-center'>Vendedor</th>" +
                              "<th class='text-center'>Lugar de Entrega</th><th class='text-center'>ALM Salida</th><th class='text-center font-24'>Observación</th></tr></thead><tbody style='background: #D1D1D1'>";
                foreach (OrdenDeVenta_E o in lista)
                {
                    info += "<tr><td  class='text-center'><input id='Linea" + linea + "' name='Det2[" + (linea - 1) + "].Linea' type='hidden' value='" + linea + "' readonly />" + linea + "</td>" +
                        "<td class='text-center'><input id='Verificar" + linea + "' name='Det2[" + (linea - 1) + "].Verificar' type='checkbox' onclick=\"validacionVerificarMontos('')\" /></td>" +
                        "<td class='text-center'><input id='Monto" + linea + "' name='Det2[" + (linea - 1) + "].Monto' type='hidden' value='" + String.Format("{0:0.00}", o.DocTotal) + "' readonly />" + String.Format("{0:0.00}", o.DocTotal) + "</td>" +
                        "<td class='text-center'><input id='NroSap" + linea + "' name='Det2[" + (linea - 1) + "].NroSap' type='hidden' value='" + o.DocNum + "' readonly size=8 />" + o.DocNum + "</td>" +
                        "<td class='text-center'><select id='TipoComprobante" + linea + "' name='Det2[" + (linea - 1) + "].TipoComprobante' class='form-control'><option value=''>Seleccione</option><option value='Factura'>Factura</option><option value='Boleta'>Boleta</option><option value='F/B'>F/B</option></select></td>" +
                        "<td class='text-center'><input id='Vendedor" + linea + "' name='Det2[" + (linea - 1) + "].Vendedor' type='hidden' value='" + o.SlpName + "' readonly />" + o.SlpName + "</td>" +
                        "<td class='text-center'><input id='LugarDeEntrega" + linea + "' name='Det2[" + (linea - 1) + "].LugarDeEntrega' type='hidden' value='" + o.LugarDeEntrega + "' readonly />" + o.LugarDeEntrega + "</td>" +
                        "<td class='text-center'><input id='AlmacenSalida" + linea + "' name='Det2[" + (linea - 1) + "].AlmacenSalida' type='hidden' value='" + o.AlmacenSalida + "' readonly />" + o.AlmacenSalida + "</td>" +
                        "<td class='text-center' style=width:'500px'><input id='Observaciones" + linea + "' name='Det2[" + (linea - 1) + "].Observaciones' type='text' size='30' class='form-control' /></td></tr>";
                    linea++;
                }
                info += "</tbody>";
            }
            return (info, tipoVenta);
        }
        public string GeneraInfoListaNotasDeCreditoV(string CardCode)
        {
            string info = string.Empty;
            int linea = 1;
            List<ORIN_E> lista = ListarNotasDeCreditoV(CardCode);
            if (lista.Count == 0) { return string.Empty; }
            foreach (ORIN_E n in lista)
            {
                info += "<tr>" +
                            "<td><input type='checkbox' name='Det4[" + (linea - 1) + "].Verificar' class=\"form-control text-center\" onclick=\"validacionVerificarMontos('')\"></td>" +
                            "<td><input type='text' name='Det4[" + (linea - 1) + "].Linea' class=\"form-control text-center\" style=\"width:40px;\" value='" + linea + "' readonly></td>" +
                            "<td><input type='text' name='Det4[" + (linea - 1) + "].Nc.DocTotal' class=\"form-control text-center\" style=\"width:100px;\" value='" + String.Format("{0:0.00}", n.DocTotal) + "' readonly></td>" +
                            "<td><input type='text' name='Det4[" + (linea - 1) + "].Nc.DocDate' class=\"form-control text-center\" style=\"width:120px;\" value='" + n.DocDate + "' readonly></td>" +
                            "<td><input type='text' name='Det4[" + (linea - 1) + "].Nc.DocNum' class=\"form-control text-center\" style=\"width:120px;\" value='" + n.DocNum + "' readonly></td>" +
                        "</tr>";
                ++linea;
            }
            return info;
        }
        public string GuiasTicket(int DocEntry)
        {
            string Guias = string.Empty;
            Tablas.ORDR_D ordrD = new Tablas.ORDR_D();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select NroSap from vt.rtv2 where DocEntry=" + DocEntry, cn);
                SqlDataReader dr = cmd.ExecuteReader();
                string guiasTicket = string.Empty;
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                    {
                        var g = ordrD.guiasTraslado(dr.GetInt32(0));
                        if (g.Length > 6) { guiasTicket += g; }
                    }
                }
                Guias = guiasTicket;
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return Guias;
        }

        public string ConducyPlacaTicket(int DocEntry)
        {
            string ConducYPlaca = string.Empty;
            Tablas.ORDR_D ordrD = new Tablas.ORDR_D();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select NroSap from vt.rtv2 where DocEntry=" + DocEntry, cn);
                SqlDataReader dr = cmd.ExecuteReader();
                string conducyPlacaTicket = string.Empty;
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                    {
                        var cp = ordrD.conducyPlacaTraslado(dr.GetInt32(0));
                        if (!string.IsNullOrWhiteSpace(cp)) { conducyPlacaTicket += cp; }
                    }
                }
                ConducYPlaca = conducyPlacaTicket;
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return ConducYPlaca;
        }

        //Metodos desde Hojas de Reparto
        public void Preenviar(int DocEntry, string Operario, SqlTransaction tran, SqlConnection cn)
        {
            ORTV_E tk = ObtenerDatosCompletosTicket(DocEntry);
            try
            {
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn, tran)
                {
                    Transaction = tran,
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "UPT");
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@Estado", tk.Estado);
                cmd.Parameters.AddWithValue("@Operario", Operario);
                cmd.ExecuteNonQuery();
            }
            catch { tran.Rollback(); cn.Close(); }
        }
        public void Liberar(int DocEntry, string Operario, SqlTransaction tran, SqlConnection cn)
        {
            ORTV_E tk = ObtenerDatosCompletosTicket(DocEntry);
            if (tk.Estado != "PREENVIO" && tk.Estado != "ENVIADO")
            {
                return;
            }
            try
            {
                using (SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "ULT");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Estado", tk.Estado);
                    cmd.Parameters.AddWithValue("@Operario", Operario);
                    cmd.Parameters.AddWithValue("@LugarDestino", tk.LugarDestino);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw new Exception(e.Message);
            }
        }
        public void Enviar(ORTV_E o, SqlTransaction tran, SqlConnection cn)
        {
            ORTV_E ortvE = ObtenerDatosCompletosTicket(o.DocEntry);
            if (ortvE.Estado != "PREENVIO") { throw new Exception("Error envio: El ticket " + ortvE.DocNum + " no esta preenvio"); }
            try
            {
                SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn, tran)
                {
                    Transaction = tran,
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "UET");
                cmd.Parameters.AddWithValue("@Operario", o.Operario);
                cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry);
                cmd.Parameters.AddWithValue("@Estado", ortvE.Estado);   // Para la validación del Proc. Almacenado (que sea distinto ANULADO)
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception(e.Message); }
        }
        public void EntregarDesdeReparto(ORTV_E o, SqlTransaction tran)
        {
            bool gestionarStock = false;
            ORTV_E ortvE = ObtenerDatosCompletosTicket(o.DocEntry);
            if (ortvE.Estado != "ENVIADO")
            {
                throw new Exception("Error entrega: El ticket " + ortvE.DocNum + " no está enviado");
            }
            // Para las rutas hacia agencia con regalo siempre pasa a ENTREGADO internamente
            if (ortvE.LugarDestino == "Agencia" && ortvE.Det5 != null && ortvE.Det5.Count >= 1)
            {
                if (ortvE.Det5[0].IdReg > 0 && ortvE.Det5[0].RegCant > 0)
                {
                    o.Det5[0].RegEstado = "Entregado";
                }
            }
            try
            {
                using (SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", tran.Connection, tran))  // Usamos la transacción proporcionada
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UETR");
                    cmd.Parameters.AddWithValue("@Operario", o.Operario);
                    cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry);
                    cmd.Parameters.AddWithValue("@Estado", ortvE.Estado);
                    cmd.Parameters.AddWithValue("@PagoEnv", ((object)o.PagoEnv) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ClaveEnv", ((object)o.ClaveEnv) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    if (ortvE.Det5 != null && ortvE.Det5.Count >= 1)
                    {
                        if (ortvE.Det5[0].IdReg > 0 && ortvE.Det5[0].RegCant > 0)
                        {
                            if (o.Det5[0].RegEstado != "Entregado")
                            {
                                throw new Exception("Debe entregar regalo");
                            }
                            ortvE.Det5[0].RegEstado = o.Det5[0].RegEstado;
                            SqlParameter tbDet5 = new SqlParameter("@TPRTV5", SqlDbType.Structured);
                            tbDet5.Value = RTV5_E.GenerarDataTable(ortvE.Det5, ortvE.DocEntry);
                            tbDet5.TypeName = "vt.TPRTV5";
                            cmd.Parameters.AddWithValue("@TPRTV5", tbDet5.Value);
                            gestionarStock = true;
                        }
                    }
                    cmd.ExecuteNonQuery();
                    if (gestionarStock)
                    {
                        OREG_D oregD = new OREG_D();
                        if (ortvE.Det5 != null && ortvE.Det5.Count > 0)
                        {
                            ortvE.Det5[0].RegCant = -1 * ortvE.Det5[0].RegCant;
                            ortvE.OpRegistro = o.Operario;
                            oregD.CompromisosStock(new List<ORTV_E> { ortvE }, tran);
                            oregD.RegistrarGestionStock(
                                new OREG_E() { Id = ortvE.Det5[0].IdReg, StockDisp = ortvE.Det5[0].RegCant },
                                new OTRC_E()
                                {
                                    IdReg = ortvE.Det5[0].IdReg,
                                    RegName = ortvE.Det5[0].RegCate + " " + ortvE.Det5[0].RegTipo,
                                    CardCode = ortvE.CardCode,
                                    CardName = ortvE.CardName,
                                    Sentido = "Salida",
                                    Detalle = ortvE.DocNum.ToString(),
                                    Cantidad = ortvE.Det5[0].RegCant,
                                    Operario = ortvE.OpRegistro
                                },
                                tran);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error al entregar el ticket: " + e.Message, e);
            }
        }
        public int Entregar(ORTV_E t)//viene desde listado despacho cuando se entrega de centro y arriola
        {
            int status = 0, regalos = 0;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlTransaction tran = cn.BeginTransaction("ENTREGA DESDE DESPACHO"))
                    {
                        using (SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn, tran))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@TipoMantenimiento", "USET");
                            cmd.Parameters.AddWithValue("@DocEntry", t.DocEntry);
                            cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                            cmd.Parameters.AddWithValue("@Estado", t.Estado);
                            // Gestión de Regalos
                            if (t.Det5 != null && t.Det5.Count > 0)
                            {
                                cmd.Parameters.AddWithValue("@RegEstado", (t.Det5[0].IdReg > 0 && t.Det5[0].RegCant > 0) ? "Entregado" : t.Det5[0].RegEstado);
                                regalos = 1;  // 1: SI
                            }
                            cmd.Parameters.AddWithValue("@TieneRegalos", regalos);
                            cmd.Parameters.AddWithValue("@Operario", t.OpRegistro);
                            cmd.ExecuteNonQuery();
                            // Obtener el número de documento generado
                            status = Convert.ToInt32(cmd.Parameters["@DocNum"].Value);
                        }
                        // Gestionar el stock si hay regalos
                        if (status >= 1 && t.Det5 != null && t.Det5.Count > 0)
                        {
                            if (t.Det5[0].IdReg > 0 && t.Det5[0].RegCant > 0)
                            {
                                OREG_D oregD = new OREG_D();
                                t.Det5[0].RegCant = -1 * t.Det5[0].RegCant;  // Restar cantidad del regalo
                                // Registrar EN OTRC un descuento de imputado como negativo
                                oregD.CompromisosStock(new List<ORTV_E> { t }, tran);
                                //HACER UN ENVIO DE TRANSACCION DE ASIGNACION NEGATIVA 
                                // Registrar  en otrc la salida como negativo y el cambio en oreg
                                oregD.RegistrarGestionStock(
                                    new OREG_E() { Id = t.Det5[0].IdReg, StockDisp = t.Det5[0].RegCant },
                                    new OTRC_E()
                                    {
                                        IdReg = t.Det5[0].IdReg,
                                        RegName = t.Det5[0].RegCate + " " + t.Det5[0].RegTipo,
                                        CardCode = t.CardCode,
                                        CardName = t.CardName,
                                        Sentido = "Salida",
                                        Detalle = t.DocNum.ToString(),
                                        Cantidad = t.Det5[0].RegCant,
                                        Operario = t.OpRegistro
                                    },
                                    tran);
                            }
                        }
                        tran.Commit();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Error al entregar el ticket masivo: " + e.Message, e);
                }
            }
            return status;
        }
        //Entregado masivo desde despacho (centro y arriola)
        public int EntregarMasivoTicket(int DocEntry, Tickets t)
        {
            int status = 0, regalos = 0;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlTransaction tran = cn.BeginTransaction("ENTREGA MASIVA DE TICKETS"))
                    {
                        // Creamos el comando asociado a la transacción
                        using (SqlCommand cmd = new SqlCommand("vt.MANT_ORTV", cn, tran))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 120;
                            cmd.Parameters.AddWithValue("@TipoMantenimiento", "USET");
                            cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                            cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                            cmd.Parameters.AddWithValue("@Estado", t.Estado);
                            // Gestión de Regalos
                            if (t.Det5 != null && t.Det5.Count > 0)
                            {
                                cmd.Parameters.AddWithValue("@RegEstado", (t.Det5[0].IdReg > 0 && t.Det5[0].RegCant > 0) ? "Entregado" : t.Det5[0].RegEstado);
                                regalos = 1;  // 1: SI
                            }
                            cmd.Parameters.AddWithValue("@TieneRegalos", regalos);
                            cmd.Parameters.AddWithValue("@Operario", t.Operario);
                            cmd.ExecuteNonQuery();
                            // Obtener el número de documento generado
                            status = Convert.ToInt32(cmd.Parameters["@DocNum"].Value);
                        }
                        // Gestionar el stock si hay regalos
                        if (status >= 1 && t.Det5 != null && t.Det5.Count > 0)
                        {
                            if (t.Det5[0].IdReg > 0 && t.Det5[0].RegCant > 0)
                            {
                                OREG_D oregD = new OREG_D();
                                t.Det5[0].RegCant = -1 * t.Det5[0].RegCant;  // Restar cantidad del regalo
                                // Registrar el compromiso de stock
                                oregD.CompromisosStock(new List<ORTV_E> { Tickets.ORTV_EntregaMasiva(t) }, tran);
                                // Registrar la gestión de stock
                                oregD.RegistrarGestionStock(
                                    new OREG_E() { Id = t.Det5[0].IdReg, StockDisp = t.Det5[0].RegCant },
                                    new OTRC_E()
                                    {
                                        IdReg = t.Det5[0].IdReg,
                                        RegName = t.Det5[0].RegCate + " " + t.Det5[0].RegTipo,
                                        CardCode = t.CardCode,
                                        CardName = t.CardName,
                                        Sentido = "Salida",
                                        Detalle = t.DocNum.ToString(),
                                        Cantidad = t.Det5[0].RegCant,
                                        Operario = t.Operario
                                    },
                                    tran);
                            }
                        }
                        tran.Commit();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Error al entregar el ticket masivo: " + e.Message, e);
                }
            }
            return status;
        }
        public Tickets Buscar(int DocEntry)
        {
            Tickets result = new Tickets();
            string query = "select t0.DocEntry,t0.DocNum, t0.CardCode,t0.CardName, t0.Estado,t1.RegCant, t1.RegCate, t1.RegEstado, t1.IdReg  from vt.ORTV t0" +
                            "  LEFT JOIN  vt.RTV5 t1 on  t1.DocEntry= t0.DocEntry  where t0.DocEntry = @DocEntry";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr = cmd.ExecuteReader();             // ejecuta
                dr.Read();                                      // lectura 
                RTV5_E reg = new RTV5_E();
                List<RTV5_E> listaReg = new List<RTV5_E>();
                if (!dr.IsDBNull(0)) { result.DocEntry = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { result.DocNum = dr.GetInt32(1); }
                if (!dr.IsDBNull(2)) { result.CardCode = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { result.CardName = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { result.Estado = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { reg.RegCant = dr.GetDecimal(5); }
                if (!dr.IsDBNull(6)) { reg.RegCate = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { reg.RegEstado = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { reg.IdReg = dr.GetInt32(8); }
                if (reg.RegCant >= 1 && reg.RegCate != null && reg.RegEstado != null && reg.IdReg >= 1)
                {
                    listaReg.Add(reg);
                    result.Det5 = listaReg;
                }
                dr.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                cn.Close();
            }
            return result;
        }
        public List<Tickets> BuscarVariosTickets(int[] arrDocNum)
        {
            List<Tickets> lista = new List<Tickets>();
            int cantidadElementos = arrDocNum.Count();
            var parametros = arrDocNum.Select((x, i) => string.Concat("@arrDocNumConv2", i)).ToList();
            string query = $"select t0.DocEntry,t0.DocNum, t0.CardName, t0.Estado,t1.RegCant, t1.RegEstado,t1.RegTipo, t1.RegCate,(select top 1 Operario from vt.CC_ORTV" +
                $" where DocEntry=t0.DocEntry and Operacion='ENTREGAR' order by FechaOperacion,HoraOperacion desc), t1.IdReg,t0.CardCode  from vt.ORTV t0" +
                $"  inner join  vt.RTV5 t1 on  t1.DocEntry= t0.DocEntry where  t1.Linea=1 and t0.DocNum in ({string.Join(", ", parametros)})";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                for (var i = 0; i < cantidadElementos; i++)
                {
                    cmd.Parameters.AddWithValue(parametros[i], arrDocNum[i]);
                }
                SqlDataReader dr = cmd.ExecuteReader();             // ejecuta
                while (dr.Read())
                {
                    Tickets ticket = new Tickets();
                    RTV5_E reg = new RTV5_E();
                    List<RTV5_E> listaReg = new List<RTV5_E>();
                    if (!dr.IsDBNull(0)) { ticket.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { ticket.DocNum = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { ticket.CardName = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { ticket.Estado = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { reg.RegCant = dr.GetDecimal(4); }
                    if (!dr.IsDBNull(5)) { reg.RegEstado = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { reg.RegTipo = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { reg.RegCate = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { ticket.Operario = dr.GetString(8); }
                    if (!dr.IsDBNull(9)) { reg.IdReg = dr.GetInt32(9); }
                    if (!dr.IsDBNull(10)) { ticket.CardCode = dr.GetString(10); }
                    if (reg.RegCant >= 1 && reg.RegCate != null && reg.RegEstado != null && reg.IdReg >= 1)
                    {
                        listaReg.Add(reg);
                        ticket.Det5 = listaReg;
                    }
                    lista.Add(ticket);
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                cn.Close();
            }
            return lista;
        }
        public List<RptAnalisisTickets_E> ListarRptAnalisisTickets(RptFiltrosAnalisisTickets_E datosFiltro)
        {
            string condWhere = string.Empty;
            List<RptAnalisisTickets_E> lista = new List<RptAnalisisTickets_E>();
            Dictionary<string, string> operarios = new Dictionary<string, string>
                {
                    { "OpSeparo", "SEPARAR" },
                    { "OpAbierto", "REGISTRAR" },
                    { "OpRecibido", "RECIBIR" },
                    { "OpPicking", "FIN PICKING" },
                    { "OpVerificador", "FIN VERIFICAR" },
                    { "OpEmpacado", "FIN EMPACAR" },
                    { "OpEnvio", "ENVIAR" },
                    { "OpEntrega", "ENTREGAR" },
                    { "OpAnular", "ANULAR" },
                    { "OpCancelar", "CANCELAR" },
                };
            Dictionary<string, string> operariosApoyo = new Dictionary<string, string>
                {
                    { "OpPicking2", "vt.RTV11" },
                    { "OpVerificador2", "vt.RTV12" },
                    { "OpEmpacado2", "vt.RTV13" },
                };
            if (!string.IsNullOrWhiteSpace(datosFiltro.TipoOperario) && datosFiltro.Operario != null)
            {
                if (!datosFiltro.TipoOperario.Equals("OpPicking2") && !datosFiltro.TipoOperario.Equals("OpVerificador2") && !datosFiltro.TipoOperario.Equals("OpEmpacado2"))
                {
                    if (operarios[datosFiltro.TipoOperario] != null)
                    {
                        condWhere += $" AND (SELECT TOP 1 Operario FROM vt.CC_ORTV WHERE Operacion='{operarios[datosFiltro.TipoOperario]}' and DocEntry=t0.DocEntry order by FechaOperacion,HoraOperacion desc) LIKE '%{datosFiltro.Operario}%'";
                    }
                }
                if (operariosApoyo.ContainsKey(datosFiltro.TipoOperario))
                {
                    condWhere += $" AND (SELECT STUFF((SELECT cast(x.operario as varchar(max)) + ', ' FROM {operariosApoyo[datosFiltro.TipoOperario]} x where t0.DocEntry= x.DocEntry FOR XML PATH('')), 1,2, ''))  LIKE concat('%', '{datosFiltro.Operario}', '%')";
                }
                else if (datosFiltro.TipoOperario.Equals("OpFacturacion"))
                {
                    condWhere += $" AND T0.OpFacturacion = '{datosFiltro.Operario}'";
                }
            }
            if (datosFiltro.AlmacenSalida != null)
            {
                condWhere += $" AND (SELECT TOP 1 AlmacenSalida from vt.RTV2 where DocEntry=t0.DocEntry) = '{datosFiltro.AlmacenSalida}'";
            }
            if (datosFiltro.AlmIni != null && datosFiltro.AlmFin != null)
            {
                condWhere += $" AND (T1.LugarDeEntrega BETWEEN '{datosFiltro.AlmIni}' and '{datosFiltro.AlmFin}')";
            }
            if (datosFiltro.Estado != null)
            {
                condWhere += $" AND T0.Estado='{datosFiltro.Estado}'";
            }
            if (datosFiltro.FecIni != null && datosFiltro.FecFin != null)
            {
                condWhere += $" AND (T0.FechaSapTicket BETWEEN '{datosFiltro.FecIni}' and '{datosFiltro.FecFin}' )";
            }
            if (datosFiltro.CardCode != null)
            {
                condWhere += $" AND T0.CardCode= '{datosFiltro.CardCode}'";
            }
            if (datosFiltro.LugarDestino != null)
            {
                condWhere += $" AND T0.LugarDestino= '{datosFiltro.LugarDestino}'";
            }
            if (datosFiltro.MontoFinalIni > 0 && datosFiltro.MontoFinalFin > 0)
            {
                condWhere += $" AND T0.MontoFinal BETWEEN {datosFiltro.MontoFinalIni} and {datosFiltro.MontoFinalFin}";
            }
            if (!string.IsNullOrWhiteSpace(datosFiltro.FormaPagoFil))
            {
                condWhere += $" AND T0.FormaPago='{datosFiltro.FormaPagoFil}'";
            }
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT T0.DocNum AS  'NRO TICKET', CONVERT(varchar, T0.FechaSapTicket, 103) AS 'FECHA SAP TICKET', T0.CardName AS 'CLIENTE', T0.MontoTotal AS 'MONTO TOTAL', T0.Vendedor AS 'VENDEDOR',T0.Estado AS 'ESTADO PEDIDO', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='SEPARAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA ABIERTO', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='SEPARAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA ABIERTO', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='RECIBIR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA RECIBIDO', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='RECIBIR' order by FechaOperacion,HoraOperacion desc ) as 'HORA RECIBIDO', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='RECIBIR' order by FechaOperacion,HoraOperacion desc ) AS 'OP RECIBIDO', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO PICKING' order by FechaOperacion,HoraOperacion DESC ) AS 'FECHA INICIO PICKING', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO PICKING' order by FechaOperacion,HoraOperacion DESC ) AS 'HORA INICIO PICKING', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO PICKING' order by FechaOperacion,HoraOperacion DESC ) AS 'OP INICIO PICKING', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN PICKING' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA FIN PICKING', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN PICKING' order by FechaOperacion,HoraOperacion desc ) as 'HORA FIN PICKING', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN PICKING' order by FechaOperacion,HoraOperacion desc ) AS 'OP FIN PICKING', " +
                                        "(Select top 1 Operario from vt.RTV11 where DocEntry=T0.DocEntry AND Linea = 1) AS 'OP APOYO 1 FIN PICKING', " +
                                        "(Select top 1 Operario from vt.RTV11 where DocEntry=T0.DocEntry AND Linea = 2) AS 'OP APOYO 2 FIN PICKING', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO VERIFICAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA INICIO VERIFICAR', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO VERIFICAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA INICIO VERIFICAR', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO VERIFICAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP INICIO VERIFICAR', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN VERIFICAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA FIN VERIFICAR', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN VERIFICAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA FIN VERIFICAR', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN VERIFICAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP FIN VERIFICAR', " +
                                        "(Select top 1 Operario from vt.RTV12 where DocEntry=T0.DocEntry AND Linea = 1) AS 'OP APOYO 1 FIN VERIFICAR', " +
                                        "(Select top 1 Operario from vt.RTV12 where DocEntry=T0.DocEntry AND Linea = 2) AS 'OP APOYO 2 FIN VERIFICAR', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO EMPACAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA INICIO EMPACAR', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO EMPACAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA INICIO EMPACAR', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='INICIO EMPACAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP INICIO EMPACAR', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN EMPACAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA FIN EMPACAR', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN EMPACAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA FIN EMPACAR', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='FIN EMPACAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP FIN EMPACAR', " +
                                        "(Select top 1 Operario from vt.RTV13 where DocEntry=T0.DocEntry AND Linea = 1) AS 'OP APOYO 1 FIN EMPACAR', " +
                                        "(Select top 1 Operario from vt.RTV13 where DocEntry=T0.DocEntry AND Linea = 2) AS 'OP APOYO 2 FIN EMPACAR', " +
                                        "T0.LugarDestino AS 'LUGAR DESTINO',  (SELECT STUFF((SELECT ', ' + cast(t1.NroSap as varchar(max)) FROM vt.RTV2 t1 INNER JOIN  vt.ORTV x ON x.DocEntry = t1.DocEntry WHERE t1.DocEntry = t0.DocEntry FOR XML PATH('')), 1,2, '')) AS 'NRO DE VENTAS', " +
                                        "(select COUNT(m.NroSap) from vt.RTV2 m where m.DocEntry=T0.DocEntry)  AS 'TOTAL NRO VENTAS', T0.Cajas AS 'CAJAS', T0.EstadoPago AS 'ESTADO PAGO', T0.FormaPago , T0.EstadoFacturacion AS 'ESTADO FACTURACION', CONVERT(varchar, T0.FechaFacturacion, 103) AS 'FECHA FACTURACION', " +
                                        "convert(varchar(8),T0.HoraFacturacion) as 'HORA FACTURACION', T0.OpFacturacion AS 'OP FACTURACION'," +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='ENTREGAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA ENTREGA', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='ENTREGAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA ENTREGA', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='ENTREGAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP ENTREGA', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='ANULAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA ANULACION', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='ANULAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA ANULACION', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='ANULAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP ANULACION', " +
                                        "(Select top 1 CONVERT(varchar, FechaOperacion , 103) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='CANCELAR' order by FechaOperacion,HoraOperacion desc ) AS 'FECHA CANCELADO', " +
                                        "(Select top 1 CONVERT(varchar(8),HoraOperacion) from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='CANCELAR' order by FechaOperacion,HoraOperacion desc ) as 'HORA CANCELADO', " +
                                        "(Select top 1 Operario from vt.CC_ORTV where DocEntry=T0.DocEntry and Operacion='CANCELAR' order by FechaOperacion,HoraOperacion desc ) AS 'OP CANCELADO', " +
                                        "CONVERT(varchar, T0.TiempoEntrega , 103) AS 'FECHA ESTIMADA ENTREGA', CONVERT(varchar, T0.TiempoEntrega , 8) AS 'HORA ESTIMADA ENTREGA', T1.LugarDeEntrega AS 'LUGAR ENTREGA', T0.NroMesa AS 'NRO MESA', " +
                                        "(SELECT STUFF((SELECT ', ' + cast(t1.AlmacenSalida as varchar(max)) FROM vt.RTV2 t1 INNER JOIN  vt.ORTV x ON x.DocEntry = t1.DocEntry WHERE t1.DocEntry = t0.DocEntry FOR XML PATH('')), 1,2, '')) AS 'ALMACEN SALIDA',T0.Comentario," +
                                        "(SELECT TOP 1 C.Comentario FROM vt.ComentarioFac C WHERE C.DocEntry = T0.DocEntry ORDER BY C.DocEntry DESC) AS 'COMENTARIO FAC', "+
                                        "T0.Zona, T0.DirDestino,RTV3.Calle,RTV3.Distrito,RTV3.Provincia,RTV3.Departamento, PesadoPedido.PesoTotal, CONVERT (varchar, T0.FechaPago, 103) AS 'FechaPagoVenta', CONVERT (varchar, T0.HoraPago, 108) AS 'HoraPagoVenta' " +
                                        "FROM VT.ORTV T0 INNER JOIN VT.RTV2 T1 ON T0.DocEntry=T1.DocEntry  " +
                                        "OUTER APPLY ( " + "" +
                                            "SELECT TOP 1 T3.Calle, T3.Distrito, T3.Departamento,T3.Provincia FROM VT.RTV3 T3 "+  
                                            "WHERE T3.DocEntry = T0.DocEntry AND T3.IdDireccion = 2"+
                                            ") RTV3 " +
                                        "OUTER APPLY ( " +
                                            "SELECT TOP 1 SUM(peso) AS 'PesoTotal' FROM VT.RTV6  " +
                                            "WHERE DocEntry = T0.DocEntry " +
                                        ") PesadoPedido " +
                                        "WHERE T1.Linea=1";

                query += condWhere;
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cn.Open();
                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            RptAnalisisTickets_E rpt = new RptAnalisisTickets_E();
                            if (!dr.IsDBNull(0)) { rpt.DocNum = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { rpt.FechaSapTicket = dr.GetString(1); }
                            if (!dr.IsDBNull(2)) { rpt.CardName = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { rpt.MontoTotal = dr.GetDecimal(3); }
                            if (!dr.IsDBNull(4)) { rpt.Vendedor = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { rpt.Estado = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { rpt.FechaAbierto = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { rpt.HoraAbierto = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { rpt.FechaRecibido = dr.GetString(8); }
                            if (!dr.IsDBNull(9)) { rpt.HoraRecibido = dr.GetString(9); }
                            if (!dr.IsDBNull(10)) { rpt.OpRecibido = dr.GetString(10); }
                            if (!dr.IsDBNull(11)) { rpt.FechaInicioPicking = dr.GetString(11); }
                            if (!dr.IsDBNull(12)) { rpt.HoraInicioPicking = dr.GetString(12); }
                            if (!dr.IsDBNull(13)) { rpt.OpInicioPicking = dr.GetString(13); }
                            if (!dr.IsDBNull(14)) { rpt.FechaFinPicking = dr.GetString(14); }
                            if (!dr.IsDBNull(15)) { rpt.HoraFinPicking = dr.GetString(15); }
                            if (!dr.IsDBNull(16)) { rpt.OpFinPicking1 = dr.GetString(16); }
                            if (!dr.IsDBNull(17)) { rpt.OpFinPicking2 = dr.GetString(17); }
                            if (!dr.IsDBNull(18)) { rpt.OpFinPicking3 = dr.GetString(18); }
                            if (!dr.IsDBNull(19)) { rpt.FechaInicioVerificar = dr.GetString(19); }
                            if (!dr.IsDBNull(20)) { rpt.HoraInicioVerificar = dr.GetString(20); }
                            if (!dr.IsDBNull(21)) { rpt.OpInicioVerificar = dr.GetString(21); }
                            if (!dr.IsDBNull(22)) { rpt.FechaFinVerificar = dr.GetString(22); }
                            if (!dr.IsDBNull(23)) { rpt.HoraFinVerificar = dr.GetString(23); }
                            if (!dr.IsDBNull(24)) { rpt.OpFinVerificador1 = dr.GetString(24); }
                            if (!dr.IsDBNull(25)) { rpt.OpFinVerificador2 = dr.GetString(25); }
                            if (!dr.IsDBNull(26)) { rpt.OpFinVerificador3 = dr.GetString(26); }
                            if (!dr.IsDBNull(27)) { rpt.FechaInicioEmpacar = dr.GetString(27); }
                            if (!dr.IsDBNull(28)) { rpt.HoraInicioEmpacar = dr.GetString(28); }
                            if (!dr.IsDBNull(29)) { rpt.OpInicioEmpacar = dr.GetString(29); }
                            if (!dr.IsDBNull(30)) { rpt.FechaFinEmpacar = dr.GetString(30); }
                            if (!dr.IsDBNull(31)) { rpt.HoraFinEmpacar = dr.GetString(31); }
                            if (!dr.IsDBNull(32)) { rpt.OpFinEmpacar1 = dr.GetString(32); }
                            if (!dr.IsDBNull(33)) { rpt.OpFinEmpacar2 = dr.GetString(33); }
                            if (!dr.IsDBNull(34)) { rpt.OpFinEmpacar3 = dr.GetString(34); }
                            if (!dr.IsDBNull(35)) { rpt.LugarDestino = dr.GetString(35); }
                            if (!dr.IsDBNull(36)) { rpt.NroVentas = dr.GetString(36); }
                            if (!dr.IsDBNull(37)) { rpt.TotalNroVentas = dr.GetInt32(37); }
                            if (!dr.IsDBNull(38)) { rpt.Cajas = dr.GetInt32(38); }
                            if (!dr.IsDBNull(39)) { rpt.EstadoPago = dr.GetString(39); }
                            if (!dr.IsDBNull(40)) { rpt.FormaPago = dr.GetString(40); }
                            if (!dr.IsDBNull(41)) { rpt.EstadoFacturacion = dr.GetString(41); }
                            if (!dr.IsDBNull(42)) { rpt.FechaFacturacion = dr.GetString(42); }
                            if (!dr.IsDBNull(43)) { rpt.HoraFacturacion = dr.GetString(43); }
                            if (!dr.IsDBNull(44)) { rpt.OpFacturacion = dr.GetString(44); }
                            if (!dr.IsDBNull(45)) { rpt.FechaEntrega = dr.GetString(45); }
                            if (!dr.IsDBNull(46)) { rpt.HoraEntrega = dr.GetString(46); }
                            if (!dr.IsDBNull(47)) { rpt.OpEntrega = dr.GetString(47); }
                            if (!dr.IsDBNull(48)) { rpt.FechaCancelacion = dr.GetString(48); }
                            if (!dr.IsDBNull(49)) { rpt.HoraCancelacion = dr.GetString(49); }
                            if (!dr.IsDBNull(50)) { rpt.OpCancelacion = dr.GetString(50); }
                            if (!dr.IsDBNull(51)) { rpt.FechaAnulacion = dr.GetString(51); }
                            if (!dr.IsDBNull(52)) { rpt.HoraAnulacion = dr.GetString(52); }
                            if (!dr.IsDBNull(53)) { rpt.OpAnulacion = dr.GetString(53); }
                            if (!dr.IsDBNull(54)) { rpt.FechaTiempoEntrega = dr.GetString(54); }
                            if (!dr.IsDBNull(55)) { rpt.HoraTiempoEntrega = dr.GetString(55); }
                            if (!dr.IsDBNull(56)) { rpt.LugarDeEntrega = dr.GetString(56); }
                            if (!dr.IsDBNull(57)) { rpt.NroMesa = dr.GetInt32(57); }
                            if (!dr.IsDBNull(58)) { rpt.AlmacenSalida = dr.GetString(58); }
                            if (!dr.IsDBNull(59)) { rpt.Comentario = dr.GetString(59); }
                            if (!dr.IsDBNull(60)) { rpt.ComentarioFac = dr.GetString(60); }
                            if (!dr.IsDBNull(61)) { rpt.ZonaVenta = dr.GetString(61); }
                            if (!dr.IsDBNull(62)) { rpt.DirDestinoVenta = dr.GetString(62); }
                            if (!dr.IsDBNull(63)) { rpt.Calle2 = dr.GetString(63); }
                            if (!dr.IsDBNull(64)) { rpt.Distrito2 = dr.GetString(64); }
                            if (!dr.IsDBNull(65)) { rpt.Provincia2 = dr.GetString(65); }
                            if (!dr.IsDBNull(66)) { rpt.Departamento2 = dr.GetString(66); }
                            if (!dr.IsDBNull(67)) { rpt.PesoTotalPedido = dr.GetDecimal(67); }
                            if (!dr.IsDBNull(68)) { rpt.FechaPagoTicket = dr.GetString(68); }
                            if (!dr.IsDBNull(69)) { rpt.HoraPagoTicket = dr.GetString(69); }
                            lista.Add(rpt);
                        }
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                cn.Close();
            }
            return lista;
        }
        public List<ORTV_E> ListarTicketsParaRepartos(ORTV_E filtro, string[] estados, out int cantidadTicketsNoEnviados)
        {
            List<ORTV_E> lista = new List<ORTV_E>(); cantidadTicketsNoEnviados = 0;
            int cantidadElementos = estados.Count();
            var parametros = estados.Select((x, i) => string.Concat("@Estado", i)).ToList();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("al.ListarTicketsReparto_2", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CantidadTicketsNoEnviados", 0).Direction = ParameterDirection.Output;
                for (var i = 0; i < cantidadElementos; i++)
                {
                    cmd.Parameters.AddWithValue(parametros[i], estados[i]);
                }
                cmd.Parameters.AddWithValue("@LugarDestino", filtro.LugarDestino);
                cmd.Parameters.AddWithValue("@Fecha", filtro.FechaSapTicket);
                cmd.Parameters.AddWithValue("@LugEntrega", filtro.LugEntrega);
                cmd.Parameters.AddWithValue("@Zona", filtro.Zona);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ORTV_E o = new ORTV_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.CardCode = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.CardName = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.DocNum = dr.GetInt32(3); }
                    if (!dr.IsDBNull(4)) { o.Cajas = dr.GetInt32(4); }
                    if (!dr.IsDBNull(5)) { o.Observaciones = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { o.GastoEnvio = dr.GetDecimal(6); }
                    if (!dr.IsDBNull(7)) { o.MontoFinal = dr.GetDecimal(7); }
                    o.DirDestino = !dr.IsDBNull(8) ? dr.GetString(8) : null;
                    o.DirDestino = string.IsNullOrWhiteSpace(o.DirDestino) || o.DirDestino.Trim() == "," ? null : o.DirDestino;
                    if (!dr.IsDBNull(9)) { o.Agencia = dr.GetString(9); }
                    if (!dr.IsDBNull(10)) { o.LugarDestino = dr.GetString(10); }
                    if (!dr.IsDBNull(11)) { o.Zona = dr.GetString(11); }
                    if (!dr.IsDBNull(12)) { o.EstadoPago = dr.GetString(12); }
                    if (!dr.IsDBNull(13)) { o.EstadoFacturacion = dr.GetString(13); }
                    if (!dr.IsDBNull(14)) { o.TipoVenta = dr.GetString(14); }
                    if (!dr.IsDBNull(15)) o.FechaPago = dr.GetString(15);
                    if (!dr.IsDBNull(16)) o.HoraPago = dr.GetString(16);
                    if (!dr.IsDBNull(17)) o.TiempoEntrega = dr.GetDateTime(17);
                    if (!dr.IsDBNull(18)) { o.Vinculados = dr.GetString(18); }
                    if (o.LugarDestino == "Domicilio" || o.LugarDestino == "Agencia")
                    { 
                        o.Guias = GuiasTicket(o.DocEntry);
                        o.ConducYPlaca = ConducyPlacaTicket(o.DocEntry); // Conductor y placa
                    }
                    else
                    {
                        Almacen_DAO.Tablas.OWTR_D owtrD = new Almacen_DAO.Tablas.OWTR_D();
                        if (o.LugarDestino == "Arriola")
                        {
                            o.Guias = owtrD.GuiasTicketTransferencia(o.DocNum, "09", o.CardCode);
                            o.ConducYPlaca = owtrD.ConducyPlacaTicketTransferencia(o.DocNum, "09", o.CardCode);
                        }
                        else
                        {
                            o.Guias = owtrD.GuiasTicketTransferencia(o.DocNum, "01", o.CardCode);
                            o.ConducYPlaca = owtrD.ConducyPlacaTicketTransferencia(o.DocNum, "01", o.CardCode);
                        }
                    }
                    lista.Add(o);
                }
                dr.Close();
                if (cmd.Parameters.Contains("@CantidadTicketsNoEnviados") && cmd.Parameters["@CantidadTicketsNoEnviados"].Value != DBNull.Value)
                {
                    // Obtener el valor del parámetro de salida, colocar Despues de cerrar el DataReader 
                    cantidadTicketsNoEnviados = Convert.ToInt32(cmd.Parameters["@CantidadTicketsNoEnviados"].Value);
                }
                cn.Close();
            }
            catch (SqlException sqlEx)
            {
                LogHelper.RegistrarError(sqlEx, $"Error SQL en ORTV_D - ListarTicketsParaRepartos()");
                cn.Close();
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, $"Error inesperado en ORTV_D - ListarTicketsParaRepartos()");
                cn.Close();
            }

            return lista;
        }
        public List<ORTV_E> ListarTicketsRepartosNoEnviados(ORTV_E filtro, string[] estados)
        {
            List<ORTV_E> lista = new List<ORTV_E>();
            int cantidadElementos = estados.Count();
            var parametros = estados.Select((x, i) => string.Concat("@Estado", i)).ToList();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("al.ListarTicketsRepartoNE", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                for (var i = 0; i < cantidadElementos; i++)
                {
                    cmd.Parameters.AddWithValue(parametros[i], estados[i]);
                }
                cmd.Parameters.AddWithValue("@LugarDestino", filtro.LugarDestino);
                cmd.Parameters.AddWithValue("@Fecha", filtro.FechaSapTicket);
                cmd.Parameters.AddWithValue("@LugEntrega", filtro.LugEntrega);
                cmd.Parameters.AddWithValue("@Zona", filtro.Zona);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ORTV_E o = new ORTV_E();
                    if (!dr.IsDBNull(0)) { o.DocNum = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.FechaSapTicket = dr.GetDateTime(1).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(2)) { o.CardName = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.MontoFinal = dr.GetDecimal(3); }
                    lista.Add(o);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public string EstadoTicket(int docEntry)
        {
            string estado = "";
            string query = "SELECT Estado FROM vt.ortv WHERE DocEntry = @DocEntry";
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@DocEntry", docEntry);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();
                                estado = dr.GetString(0);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return estado;
        }
        public int DocNumTicket(int docEntry)
        {
            int docnum = 0;
            string query = "SELECT DocNum FROM vt.ortv WHERE DocEntry = @DocEntry";
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@DocEntry", docEntry);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();
                                docnum = dr.GetInt32(0);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return docnum;
        }
        public int DocNumTicketLike(int docNumLike)
        {
            int docnum = 0;
            string query = "SELECT top 1 DocNum FROM vt.ortv WHERE DocNum like '%' + @docNumLike + '%' order by DocNum desc ";
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@docNumLike", Convert.ToString(docNumLike));
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            dr.Read();
                            docnum = dr.GetInt32(0);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return docnum;
        }
        public int DocEntryTicket(int docNum)
        {
            int docentry = 0;
            string query = "SELECT DocEntry FROM vt.ortv WHERE DocNum = @DocNum";
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@DocNum", docNum);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();
                                docentry = dr.GetInt32(0);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return docentry;
        }
        public int CantidadTicketsProductosPendientes()
        {
            int cant = 0;
            string query = $"Select count(*) from vt.BusquedaProducto where Estado='PENDIENTE';";
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();
                                cant = dr.GetInt32(0);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return cant;
        }
        public int CantidadTicketsFacturacion(string estadoFacturacion)
        {
            int cant = 0;
            string query = $@"SELECT COUNT(*) FROM vt.ortv T0 WHERE T0.EstadoFacturacion = @estadoFacturacion and T0.Estado IN ('PICKEANDO', 'VERIFICANDO', 'EMPACANDO', 'EMPACADO', 'PESADO', 'PREENVIO', 'ENVIADO') 
                AND EXISTS (
                                        SELECT 1 
                                        FROM VT.CC_ORTV 
                                        WHERE DocEntry = T0.DocEntry AND Operacion = 'FIN VERIFICAR'
                                    ) 
                                    AND NOT EXISTS (
                                        SELECT 1 
                                        FROM VT.CC_ORTV 
                                        WHERE DocEntry = T0.DocEntry 
                                        AND Operacion = 'ANULAR FIN VERIFICAR' 
                                        AND (
                                            (SELECT TOP 1 Id FROM VT.CC_ORTV WHERE DocEntry = T0.DocEntry AND Operacion = 'FIN VERIFICAR' ORDER BY Id DESC) 
                                            < 
                                            (SELECT TOP 1 Id FROM VT.CC_ORTV WHERE DocEntry = T0.DocEntry AND Operacion = 'ANULAR FIN VERIFICAR' ORDER BY Id DESC)
                                        ) 
                                    )
AND YEAR(T0.FechaSapTicket) = 2025 AND ((SELECT  Estado FROM vt.BusquedaProducto WHERE DocEntry=t0.DocEntry )='CONCLUIDO' or not exists (SELECT  Estado FROM vt.BusquedaProducto WHERE DocEntry=t0.DocEntry ))";
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@estadoFacturacion", estadoFacturacion);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.HasRows)
                            {
                                dr.Read();
                                cant = dr.GetInt32(0);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return cant;
        }
        public ORTV_E ObtenerTicketFacturacion(int DocEntry)
        {
            ORTV_E t = new ORTV_E();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select DocEntry,DocNum,CardCode, CardName,Estado,TipoVenta,LugarDestino, DirDestino,Referencia,Agencia,EnvioAgencia,Embalaje,CodSapVendedor,Vendedor,MontoTotal,Flete,GastoEnvio,EstadoGasto,PagoEnv,ClaveEnv,TiempoEntrega,DescuentoNC,DeudaCliente,DeudaEmpresa,MontoFinal,FormaPago,MontoRecibido,EstadoPago,FechaPago,HoraPago,Cajero,Comentario,Cajas,NroMesa,FechaNC,EstadoFacturacion,FechaFacturacion,HoraFacturacion,OpFacturacion, Observaciones,Observaciones2,Observaciones3,FechaSapTicket, (Select top 1 FechaOperacion from vt.CC_ORTV where DocEntry=" + DocEntry + " and Operacion='REGISTRAR' order by FechaOperacion DESC,HoraOperacion DESC ) AS 'FECHA REGISTRO', (Select top 1 HoraOperacion from vt.CC_ORTV where DocEntry=" + DocEntry + " and Operacion='REGISTRAR' order by FechaOperacion,HoraOperacion desc ) AS 'HORA REGISTRO' ,Zona,Notificado,Visible,Presupuesto  from vt.ORTV where DocEntry=" + DocEntry, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                t.DocEntry = dr.GetInt32(0);
                t.DocNum = dr.GetInt32(1);
                if (!dr.IsDBNull(2)) { t.CardCode = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { t.CardName = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { t.Estado = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { t.TipoVenta = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { t.LugarDestino = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { t.DirDestino = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { t.Referencia = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { t.Agencia = dr.GetString(9); }
                if (!dr.IsDBNull(10)) { t.EnvioAgencia = dr.GetString(10); }
                if (!dr.IsDBNull(11)) { t.Embalaje = dr.GetString(11); }
                if (!dr.IsDBNull(12)) { t.CodSapVendedor = dr.GetInt32(12); }
                if (!dr.IsDBNull(13)) { t.Vendedor = dr.GetString(13); }
                if (!dr.IsDBNull(14)) { t.MontoTotal = dr.GetDecimal(14); }
                if (!dr.IsDBNull(15)) { t.Flete = dr.GetDecimal(15); }
                if (!dr.IsDBNull(16)) { t.GastoEnvio = dr.GetDecimal(16); }
                if (!dr.IsDBNull(17)) { t.EstadoGasto = dr.GetString(17); }
                if (!dr.IsDBNull(18)) { t.PagoEnv = dr.GetDecimal(18); }
                if (!dr.IsDBNull(19)) { t.ClaveEnv = dr.GetString(19); }
                if (!dr.IsDBNull(20)) { t.TiempoEntrega = dr.GetDateTime(20); }
                if (!dr.IsDBNull(21)) { t.DescuentoNC = dr.GetDecimal(21); }
                if (!dr.IsDBNull(22)) { t.DeudaCliente = dr.GetDecimal(22); }
                if (!dr.IsDBNull(23)) { t.DeudaEmpresa = dr.GetDecimal(23); }
                if (!dr.IsDBNull(24)) { t.MontoFinal = dr.GetDecimal(24); }
                if (!dr.IsDBNull(25)) { t.FormaPago = dr.GetString(25); }
                if (!dr.IsDBNull(26)) { t.MontoRecibido = dr.GetDecimal(26); }
                if (!dr.IsDBNull(27)) { t.EstadoPago = dr.GetString(27); }
                if (!dr.IsDBNull(28)) { t.FechaPago = dr.GetDateTime(28).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(29)) { t.HoraPago = dr.GetTimeSpan(29).ToString(); }
                if (!dr.IsDBNull(30)) { t.Cajero = dr.GetString(30); }
                if (!dr.IsDBNull(31)) { t.Comentario = dr.GetString(31); }
                if (!dr.IsDBNull(32)) { t.Cajas = dr.GetInt32(32); }
                if (!dr.IsDBNull(33)) { t.NroMesa = dr.GetInt32(33); }
                if (!dr.IsDBNull(34)) { t.FechaNC = dr.GetDateTime(34).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(35)) { t.EstadoFacturacion = dr.GetString(35); }
                if (!dr.IsDBNull(36)) { t.FechaFacturacion = dr.GetDateTime(36).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(37)) { t.HoraFacturacion = dr.GetTimeSpan(37).ToString(); }
                if (!dr.IsDBNull(38)) { t.OpFacturacion = dr.GetString(38); }
                if (!dr.IsDBNull(39)) { t.Observaciones = dr.GetString(39); }
                if (!dr.IsDBNull(40)) { t.Observaciones2 = dr.GetString(40); }
                if (!dr.IsDBNull(41)) { t.Observaciones3 = dr.GetString(41); }
                if (!dr.IsDBNull(42)) { t.FechaSapTicket = dr.GetDateTime(42).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(43)) { t.FechaRegistro = dr.GetDateTime(43).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(44)) { t.HoraRegistro = dr.GetTimeSpan(44).ToString(); }
                if (!dr.IsDBNull(45)) { t.Zona = dr.GetString(45); }
                if (!dr.IsDBNull(46)) { t.Notificado = dr.GetInt32(46); }
                if (!dr.IsDBNull(47)) { t.Visible = dr.GetString(47); }
                if (!dr.IsDBNull(48)) { t.Presupuesto = dr.GetString(48); }
                dr.Close();
                cn.Close();
                t.Det1 = obtenerDet1Ticket(DocEntry); if (t.Det1.Count == 0) { t.Det1 = null; }      //Datos de recojo
                t.Det2 = obtenerDet2Ticket(DocEntry); if (t.Det2.Count == 0) { t.Det2 = null; }     //Ordenes de venta
            }
            catch (Exception e) { cn.Close(); throw new Exception(e.Message); }
            return t;
        }
        public ORTV_E ObtenerTicketVenta(int DocEntry)
        {
            ORTV_E t = new ORTV_E();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select top 1 DocEntry,DocNum,Estado,Zona,Referencia,LugarDestino,AlmProcedencia,CardCode,CardName,MontoFinal from vt.ORTV where DocEntry=" + DocEntry + "" +
                    " or DocNum like '%" + DocEntry + "%' ", cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                t.DocEntry = dr.GetInt32(0);
                t.DocNum = dr.GetInt32(1);
                if (!dr.IsDBNull(2)) { t.Estado = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { t.Zona = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { t.Referencia = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { t.LugarDestino = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { t.AlmProcedencia = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { t.CardCode = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { t.CardName = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { t.MontoFinal = dr.GetDecimal(9); }
                dr.Close();
                cn.Close();
                t.Det2 = obtenerDet2Ticket(t.DocEntry); if (t.Det2.Count == 0) { t.Det2 = null; }     //Ordenes de venta
                t.Det3 = obtenerDet3Ticket(t.DocEntry); if (t.Det3.Count == 0) { t.Det3 = null; }     //Direcciones
            }
            catch (Exception e) { cn.Close(); throw new Exception(e.Message); }
            return t;
        }
        public ORTV_E ObtenerDatosCompletosTicket(int DocEntry)
        {
            ORTV_E t = new ORTV_E();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select DocEntry,DocNum,CardCode, CardName,Estado,TipoVenta,LugarDestino, " +
                    "DirDestino,Referencia,Agencia,EnvioAgencia,Embalaje,CodSapVendedor,Vendedor,MontoTotal,Flete,GastoEnvio," +
                    "EstadoGasto,PagoEnv,ClaveEnv,TiempoEntrega,DescuentoNC,DeudaCliente,DeudaEmpresa,MontoFinal,FormaPago,MontoRecibido," +
                    "EstadoPago,FechaPago,HoraPago,Cajero,Comentario,Cajas,NroMesa,FechaNC,EstadoFacturacion,FechaFacturacion,HoraFacturacion," +
                    "OpFacturacion, Observaciones,Observaciones2,Observaciones3,FechaSapTicket, (Select top 1 FechaOperacion " +
                    "from vt.CC_ORTV where DocEntry=" + DocEntry + " and Operacion='REGISTRAR' order by FechaOperacion DESC,HoraOperacion DESC ) AS 'FECHA REGISTRO'," +
                    " (Select top 1 HoraOperacion from vt.CC_ORTV where DocEntry=" + DocEntry + " and Operacion='REGISTRAR' order by FechaOperacion,HoraOperacion desc ) " +
                    "AS 'HORA REGISTRO' ,AlmProcedencia, Zona,Notificado,Visible,Presupuesto, (Select  Estado from vt.BusquedaProducto where DocEntry=vt.ORTV.DocEntry )   " +
                    "from vt.ORTV where DocEntry=" + DocEntry, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                t.DocEntry = dr.GetInt32(0);
                t.DocNum = dr.GetInt32(1);
                if (!dr.IsDBNull(2)) { t.CardCode = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { t.CardName = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { t.Estado = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { t.TipoVenta = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { t.LugarDestino = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { t.DirDestino = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { t.Referencia = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { t.Agencia = dr.GetString(9); }
                if (!dr.IsDBNull(10)) { t.EnvioAgencia = dr.GetString(10); }
                if (!dr.IsDBNull(11)) { t.Embalaje = dr.GetString(11); }
                if (!dr.IsDBNull(12)) { t.CodSapVendedor = dr.GetInt32(12); }
                if (!dr.IsDBNull(13)) { t.Vendedor = dr.GetString(13); }
                if (!dr.IsDBNull(14)) { t.MontoTotal = dr.GetDecimal(14); }
                if (!dr.IsDBNull(15)) { t.Flete = dr.GetDecimal(15); }
                if (!dr.IsDBNull(16)) { t.GastoEnvio = dr.GetDecimal(16); }
                if (!dr.IsDBNull(17)) { t.EstadoGasto = dr.GetString(17); }
                if (!dr.IsDBNull(18)) { t.PagoEnv = dr.GetDecimal(18); }
                if (!dr.IsDBNull(19)) { t.ClaveEnv = dr.GetString(19); }
                if (!dr.IsDBNull(20)) { t.TiempoEntrega = dr.GetDateTime(20); }
                if (!dr.IsDBNull(21)) { t.DescuentoNC = dr.GetDecimal(21); }
                if (!dr.IsDBNull(22)) { t.DeudaCliente = dr.GetDecimal(22); }
                if (!dr.IsDBNull(23)) { t.DeudaEmpresa = dr.GetDecimal(23); }
                if (!dr.IsDBNull(24)) { t.MontoFinal = dr.GetDecimal(24); }
                if (!dr.IsDBNull(25)) { t.FormaPago = dr.GetString(25); }
                if (!dr.IsDBNull(26)) { t.MontoRecibido = dr.GetDecimal(26); }
                if (!dr.IsDBNull(27)) { t.EstadoPago = dr.GetString(27); }
                if (!dr.IsDBNull(28)) { t.FechaPago = dr.GetDateTime(28).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(29)) { t.HoraPago = dr.GetTimeSpan(29).ToString(); }
                if (!dr.IsDBNull(30)) { t.Cajero = dr.GetString(30); }
                if (!dr.IsDBNull(31)) { t.Comentario = dr.GetString(31); }
                if (!dr.IsDBNull(32)) { t.Cajas = dr.GetInt32(32); }
                if (!dr.IsDBNull(33)) { t.NroMesa = dr.GetInt32(33); }
                if (!dr.IsDBNull(34)) { t.FechaNC = dr.GetDateTime(34).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(35)) { t.EstadoFacturacion = dr.GetString(35); }
                if (!dr.IsDBNull(36)) { t.FechaFacturacion = dr.GetDateTime(36).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(37)) { t.HoraFacturacion = dr.GetTimeSpan(37).ToString(); }
                if (!dr.IsDBNull(38)) { t.OpFacturacion = dr.GetString(38); }
                if (!dr.IsDBNull(39)) { t.Observaciones = dr.GetString(39); }
                if (!dr.IsDBNull(40)) { t.Observaciones2 = dr.GetString(40); }
                if (!dr.IsDBNull(41)) { t.Observaciones3 = dr.GetString(41); }
                if (!dr.IsDBNull(42)) { t.FechaSapTicket = dr.GetDateTime(42).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(43)) { t.FechaRegistro = dr.GetDateTime(43).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(44)) { t.HoraRegistro = dr.GetTimeSpan(44).ToString(); }
                if (!dr.IsDBNull(45)) { t.AlmProcedencia = dr.GetString(45); }
                if (!dr.IsDBNull(46)) { t.Zona = dr.GetString(46); }
                if (!dr.IsDBNull(47)) { t.Notificado = dr.GetInt32(47); }
                if (!dr.IsDBNull(48)) { t.Visible = dr.GetString(48); }
                if (!dr.IsDBNull(49)) { t.Presupuesto = dr.GetString(49); }
                if (!dr.IsDBNull(50))
                {
                    var ProductoPendiente = dr.GetString(50);
                    if (ProductoPendiente == "PENDIENTE")
                    {
                        t.ProductoPendiente = 1;
                    }
                    else { t.ProductoPendiente = 0; }
                }
                dr.Close();
                cn.Close();
                t.Det1 = obtenerDet1Ticket(DocEntry); if (t.Det1.Count == 0) { t.Det1 = null; }      //Datos de recojo
                t.Det2 = obtenerDet2Ticket(DocEntry); if (t.Det2.Count == 0) { t.Det2 = null; }     //Ordenes de venta
                t.Det3 = obtenerDet3Ticket(DocEntry); if (t.Det3.Count == 0) { t.Det3 = null; }       //Direcciones
                t.Det4 = obtenerDet4Ticket(DocEntry); if (t.Det4.Count == 0) { t.Det4 = null; }       //Notas de credito
                t.Det5 = obtenerDet5Ticket(DocEntry); if (t.Det5.Count == 0) { t.Det5 = null; }       // Regalos
                t.Det6 = obtenerDet6Ticket(DocEntry); if (t.Det6.Count == 0) { t.Det6 = null; }       // Pesos
                t.Det7 = obtenerDet7Ticket(DocEntry); if (t.Det7.Count == 0) { t.Det7 = null; }       // Tickets vinculados para reparto
            }
            catch (Exception e) { cn.Close(); throw new Exception(e.Message); }
            return t;
        }
        public ORTV_E ObtenerTicketRotulado(int DocEntry)
        {
            ORTV_E t = new ORTV_E();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select DocEntry,DocNum,DirDestino,Referencia,Cajas,EnvioAgencia,Estado from vt.ORTV where DocEntry=" + DocEntry, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                t.DocEntry = dr.GetInt32(0);
                t.DocNum = dr.GetInt32(1);
                if (!dr.IsDBNull(2)) { t.DirDestino = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { t.Referencia = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { t.Cajas = dr.GetInt32(4); }
                if (!dr.IsDBNull(5)) { t.EnvioAgencia = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { t.Estado = dr.GetString(6); }
                dr.Close();
                cn.Close();
                t.Det1 = obtenerDet1Ticket(DocEntry); if (t.Det1.Count == 0) { t.Det1 = null; }       //Persona de recojo
                t.Det3 = obtenerDet3Ticket(DocEntry); if (t.Det3.Count == 0) { t.Det3 = null; }       //Direcciones
            }
            catch (Exception e) { cn.Close(); throw new Exception(e.Message); }
            return t;
        }
        public ORTV_E ObtenerTicketTacoEmpaque(int DocEntry)
        {
            ORTV_E t = new ORTV_E();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select DocEntry,DocNum,Comentario,Cajas,EnvioAgencia,CardCode,CardName,LugarDestino,MontoTotal,Embalaje,TiempoEntrega,Vendedor,Estado from vt.ORTV where DocEntry=" + DocEntry, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                t.DocEntry = dr.GetInt32(0);
                t.DocNum = dr.GetInt32(1);
                if (!dr.IsDBNull(2)) { t.Comentario = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { t.Cajas = dr.GetInt32(3); }
                if (!dr.IsDBNull(4)) { t.EnvioAgencia = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { t.CardCode = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { t.CardName = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { t.LugarDestino = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { t.MontoTotal = dr.GetDecimal(8); }
                if (!dr.IsDBNull(9)) { t.Embalaje = dr.GetString(9); }
                if (!dr.IsDBNull(10)) { t.TiempoEntrega = dr.GetDateTime(10); }
                if (!dr.IsDBNull(11)) { t.Vendedor = dr.GetString(11); }
                if (!dr.IsDBNull(12)) { t.Estado = dr.GetString(12); }
                dr.Close();
                cn.Close();
            }
            catch (Exception e) { cn.Close(); throw new Exception(e.Message); }
            return t;
        }
        public ORTV_E ObtenerDatosTicketParaDocumentos(int DocEntry)
        {
            ORTV_E t = new ORTV_E();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select DocEntry,DocNum,LugarDestino,MontoTotal,EstadoFacturacion,Estado,Flete,GastoEnvio,DescuentoNC,CardCode,AlmProcedencia from vt.ORTV where DocEntry=" + DocEntry, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                t.DocEntry = dr.GetInt32(0);
                t.DocNum = dr.GetInt32(1);
                if (!dr.IsDBNull(2)) { t.LugarDestino = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { t.MontoTotal = dr.GetDecimal(3); }
                if (!dr.IsDBNull(4)) { t.EstadoFacturacion = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { t.Estado = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { t.Flete = dr.GetDecimal(6); }
                if (!dr.IsDBNull(7)) { t.GastoEnvio = dr.GetDecimal(7); }
                if (!dr.IsDBNull(8)) { t.DescuentoNC = dr.GetDecimal(8); }
                if (!dr.IsDBNull(9)) { t.CardCode = dr.GetString(9); }
                if (!dr.IsDBNull(10)) { t.AlmProcedencia = dr.GetString(10); }
                dr.Close();
                cn.Close();
                t.Det2 = obtenerDet2Ticket(DocEntry); t.Det2 = t.Det2.Count == 0 ? null : t.Det2;      //Ordenes de venta SAP
                t.Det4 = obtenerDet4Ticket(DocEntry); t.Det4 = t.Det4.Count == 0 ? null : t.Det4;      //Notas de crédito
            }
            catch (Exception e) { cn.Close(); throw new Exception(e.Message); }
            return t;
        }
        // Método auxiliar para determinar el estado de una operación
        private bool ObtenerEstadoOperacion(List<CC_ORTV_E> listaPrincipal, List<CC_ORTV_E> listaAnulada, string operacionPrincipal, string operacionAnulada)
        {
            var listaCombinada = new List<CC_ORTV_E>(listaPrincipal);
            listaCombinada.AddRange(listaAnulada);
            var listaOrdenada = listaCombinada.OrderByDescending(x => x.Id).ToList();
            // Verificar la operación principal y anulada
            if (listaOrdenada != null && listaOrdenada.Any() && listaOrdenada.FirstOrDefault().Operacion == operacionPrincipal)
            {
                return true;
            }
            else if (listaOrdenada != null && listaOrdenada.Any() && listaOrdenada.FirstOrDefault().Operacion == operacionAnulada)
            {
                return false;
            }
            return false;
        }
        public ORTV_E ObtenerReferenciaEstadosTicket(ORTV_E ticket)
        {
            CC_ORTV_D ccORTV = new CC_ORTV_D();
            // Revisamos si hay RECIBIR y ANULAR RECIBIR
            List<CC_ORTV_E> tkRecibido = ccORTV.ListarCC_ORTV(ticket.DocEntry, "RECIBIR");
            List<CC_ORTV_E> tkAnularRecibido = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR RECIBIR");
            ticket.hayRecibir = ObtenerEstadoOperacion(tkRecibido, tkAnularRecibido, "RECIBIR", "ANULAR RECIBIR");
            // Revisamos si hay INICIO PICKING y ANULAR INICIO PICKING
            List<CC_ORTV_E> ticketIniPicking = ccORTV.ListarCC_ORTV(ticket.DocEntry, "INICIO PICKING");
            List<CC_ORTV_E> ticketAnularIniPicking = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR INICIO PICKING");
            ticket.hayIniPicking = ObtenerEstadoOperacion(ticketIniPicking, ticketAnularIniPicking, "INICIO PICKING", "ANULAR INICIO PICKING");
            // Revisamos si hay FIN PICKING y ANULAR FIN PICKING
            List<CC_ORTV_E> ticketFinPicking = ccORTV.ListarCC_ORTV(ticket.DocEntry, "FIN PICKING");
            List<CC_ORTV_E> ticketAnularFinPicking = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR FIN PICKING");
            ticket.hayFinPicking = ObtenerEstadoOperacion(ticketFinPicking, ticketAnularFinPicking, "FIN PICKING", "ANULAR FIN PICKING");
            // Revisamos si hay INICIO VERIFICAR y ANULAR INICIO VERIFICAR
            List<CC_ORTV_E> ticketIniVerificar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "INICIO VERIFICAR");
            List<CC_ORTV_E> ticketAnularIniVerificar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR INICIO VERIFICAR");
            ticket.hayIniVerificar = ObtenerEstadoOperacion(ticketIniVerificar, ticketAnularIniVerificar, "INICIO VERIFICAR", "ANULAR INICIO VERIFICAR");
            // Revisamos si hay FIN VERIFICAR y ANULAR FIN VERIFICAR
            List<CC_ORTV_E> ticketFinVerificar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "FIN VERIFICAR");
            List<CC_ORTV_E> ticketAnularFinVerificar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR FIN VERIFICAR");
            ticket.hayFinVerificar = ObtenerEstadoOperacion(ticketFinVerificar, ticketAnularFinVerificar, "FIN VERIFICAR", "ANULAR FIN VERIFICAR");
            // Revisamos si hay INICIO EMPACAR y ANULAR INICIO EMPACAR
            List<CC_ORTV_E> ticketIniEmpacar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "INICIO EMPACAR");
            List<CC_ORTV_E> ticketAnularIniEmpacar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR INICIO EMPACAR");
            ticket.hayIniEmpacar = ObtenerEstadoOperacion(ticketIniEmpacar, ticketAnularIniEmpacar, "INICIO EMPACAR", "ANULAR INICIO EMPACAR");
            // Revisamos si hay FIN EMPACAR y ANULAR FIN EMPACAR
            List<CC_ORTV_E> ticketFinEmpacar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "FIN EMPACAR");
            List<CC_ORTV_E> ticketAnularFinEmpacar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR FIN EMPACAR");
            ticket.hayFinEmpacar = ObtenerEstadoOperacion(ticketFinEmpacar, ticketAnularFinEmpacar, "FIN EMPACAR", "ANULAR FIN EMPACAR");
            // Revisamos si hay ENVIAR o LIBERAR
            List<CC_ORTV_E> tkEnviado = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ENVIAR");
            List<CC_ORTV_E> tkLiberado = ccORTV.ListarCC_ORTV(ticket.DocEntry, "LIBERAR");
            ticket.hayEnviar = ObtenerEstadoOperacion(tkEnviado, tkLiberado, "ENVIAR", "LIBERAR");
            // Revisamos si hay ENTREGAR y ANULAR ENTREGAR
            List<CC_ORTV_E> tkEntregar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ENTREGAR");
            List<CC_ORTV_E> tkAnularEntregar = ccORTV.ListarCC_ORTV(ticket.DocEntry, "ANULAR ENTREGAR");
            ticket.hayEntregar = ObtenerEstadoOperacion(tkEntregar, tkAnularEntregar, "ENTREGAR", "ANULAR ENTREGAR");
            // Establecer aptoIniVerificar en true si no hay un previo inicio Verificar y si hay inicio de Picking
            ticket.aptoIniVerificar = !ticket.hayIniVerificar && ticket.hayIniPicking;
            // Establecer aptoFinVerificar en true si hay inicio verificar y que no haya previamente un fin verificar y tiene que haber fin Picking
            ticket.aptoFinVerificar = ticket.hayIniVerificar && !ticket.hayFinVerificar && ticket.hayFinPicking;
            return ticket;
        }
        public List<ORTV_E> ListarTicketsAreaVenta(Usuario_E user, ORTV_E t)
        {
            List<ORTV_E> lista = new List<ORTV_E>();
            string condWhere = string.Empty;
            if (user.IdRol == 7)
            {
                condWhere += $" AND t0.CodSapVendedor=@CodSapVendedor";
            }
            if (t != null)
            {
                condWhere += t.DocNum > 0 ? $" AND t0.DocNum like '%{t.DocNum}%'" : "";
                condWhere += t.FechaSapTicket != null ? $" AND t0.FechaSapTicket='{t.FechaSapTicket}'" : "";
                condWhere += t.CardName != null ? $" AND t0.CardName like '%{t.CardName}%'" : "";
                condWhere += t.Vendedor != null ? $" AND t0.Vendedor like '%{t.Vendedor}%'" : "";
                condWhere += t.MontoFinal > 0 ? $" AND t0.MontoFinal like '{t.MontoFinal}%'" : "";
                condWhere += t.LugarDestino != null ? $" AND t0.LugarDestino='{t.LugarDestino}'" : "";
                condWhere += t.Estado != null ? $" AND t0.Estado='{t.Estado}'" : "";
                condWhere += t.EstadoPago != null ? $" AND t0.EstadoPago='{t.EstadoPago}'" : "";
                condWhere += t.EstadoGasto != null ? $" AND t0.EstadoGasto='{t.EstadoGasto}'" : "";
                condWhere += t.PagoEnv == 0.01M ? " AND t0.PagoEnv>0" : "";
                condWhere += t.Visible != null ? $" AND t0.Visible='{t.Visible}' AND T0.Estado not in ('CANCELADO','ANULADO')" : "";
                condWhere += t.Presupuesto != null ? $" AND t0.Presupuesto='{t.Presupuesto}'" : "";
            }
            string query = $"SELECT TOP 200 t0.DocEntry, t0.DocNum, t0.CardCode, t0.CardName, t0.Estado,t0.FechaSapTicket, (Select top 1 HoraOperacion from vt.CC_ORTV where DocEntry=t0.DocEntry " +
                $" and Operacion='REGISTRAR' order by FechaOperacion,HoraOperacion desc ) as 'HoraAbierto',t0.LugarDestino,t0.CodSapVendedor,t0.Vendedor,t0.MontoFinal,t0.EstadoPago,t0.EstadoGasto," +
                $" t0.PagoEnv,t0.Visible,t0.FechaPago,t0.HoraPago,T0.Presupuesto FROM vt.ORTV t0  WHERE t0.DocEntry>0 AND (SELECT top 1 FechaOperacion from vt.CC_ORTV where Operacion= 'SEPARAR' and DocEntry =" +
                $" t0.DocEntry  order by FechaOperacion DESC, HoraOperacion desc) between dateadd(day,-1000, getdate()) and getdate() {condWhere} ORDER BY t0.DocNum DESC";
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@CodSapVendedor", user.CodigoSap);
                cn.Open();
                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ORTV_E ticket = new ORTV_E();
                            if (!dr.IsDBNull(0)) { ticket.DocEntry = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { ticket.DocNum = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { ticket.CardCode = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { ticket.CardName = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { ticket.Estado = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { ticket.FechaSapTicket = dr.GetDateTime(5).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(6)) { ticket.HoraAbierto = dr.GetTimeSpan(6).ToString(); }
                            if (!dr.IsDBNull(7)) { ticket.LugarDestino = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { ticket.CodSapVendedor = dr.GetInt32(8); }
                            if (!dr.IsDBNull(9)) { ticket.Vendedor = dr.GetString(9); }
                            if (!dr.IsDBNull(10)) { ticket.MontoFinal = dr.GetDecimal(10); }
                            if (!dr.IsDBNull(11)) { ticket.EstadoPago = dr.GetString(11); }
                            if (!dr.IsDBNull(12)) { ticket.EstadoGasto = dr.GetString(12); }
                            if (!dr.IsDBNull(13)) { ticket.PagoEnv = dr.GetDecimal(13); }
                            if (!dr.IsDBNull(14)) { ticket.Visible = dr.GetString(14); }
                            if (!dr.IsDBNull(15)) { ticket.FechaPago = dr.GetDateTime(15).ToString("yyyy-MM-dd"); }//Se usa en Metodo : ListarTicketsNoVisiblesPagados
                            if (!dr.IsDBNull(16)) { ticket.HoraPago = dr.GetTimeSpan(16).ToString(); }
                            if (!dr.IsDBNull(17)) { ticket.Presupuesto = dr.GetString(17); }
                            ticket.FechaSapTicket = (ticket.FechaSapTicket != null) ? Convert.ToDateTime(ticket.FechaSapTicket).ToString("dd/MM/yyyy") : null;
                            lista.Add(ticket);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
            return lista;
        }
        public List<ORTV_E> ListarTicketsAreaFacturacion(Usuario_E user, ORTV_E t, int SoloConObservacion = 0)
        {
            List<ORTV_E> lista = new List<ORTV_E>();
            string condWhere = string.Empty;
            string joinObservacion = string.Empty;
            string orderBy = string.Empty;
            if (SoloConObservacion == 1)
            {
                joinObservacion = " INNER JOIN vt.ComentarioFac cf ON cf.DocEntry = t0.DocEntry ";
                condWhere += " AND cf.Comentario IS NOT NULL AND LTRIM(RTRIM(cf.Comentario)) <> '' ";
            }
            if (t != null)
            {
                condWhere += t.DocNum > 0 ? $" AND t0.DocNum like '%{t.DocNum}%'" : "";
                condWhere += t.FechaSapTicket != null ? $" AND t0.FechaSapTicket='{t.FechaSapTicket}'" : "";
                condWhere += t.AlmProcedencia != null ? $" AND t0.AlmProcedencia='{t.AlmProcedencia}'" : "";
                condWhere += t.CardName != null ? $" AND t0.CardName like '%{t.CardName}%'" : "";
                condWhere += t.Vendedor != null ? $" AND t0.Vendedor like '%{t.Vendedor}%'" : "";
                condWhere += t.Zona != null ? $" AND t0.Zona ='{t.Zona}'" : "";
                condWhere += t.LugarDestino != null ? $" AND t0.LugarDestino='{t.LugarDestino}'" : "";
                condWhere += t.Estado != null ? $" AND t0.Estado='{t.Estado}'" : " AND T0.Estado != 'CANCELADO'";

                if (!string.IsNullOrWhiteSpace(t.EstadoFacturacion))
                {
                    condWhere += $" AND t0.EstadoFacturacion='{t.EstadoFacturacion}'";
                    orderBy = "CASE WHEN EXISTS (SELECT 1 FROM vt.CC_ORTV_print WHERE DocEntryTicket = t0.DocEntry AND Id_Usuario = 'Facturacion') THEN 1 ELSE 0 END,";
                }

                condWhere += t.EstadoPago != null ? $" AND t0.EstadoPago='{t.EstadoPago}'" : "";
                condWhere += t.TipoVenta != null ? $" AND t0.TipoVenta='{t.TipoVenta}'" : "";
                condWhere += t.EstadoGasto != null ? $" AND t0.EstadoGasto='{t.EstadoGasto}'" : "";
                condWhere += t.Flete == 0.01M ? " AND t0.Flete>0" : "";
                condWhere += t.DescuentoNC == 0.01M ? " AND t0.DescuentoNC>0" : "";
                condWhere += t.TiempoEntrega != null ? $" AND CONVERT(char(10), t0.TiempoEntrega,126) = '{Convert.ToDateTime(t.TiempoEntrega).ToString("yyyy-MM-dd")}'" : "";
            }

            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("SELECT TOP 200");
            queryBuilder.AppendLine("   t0.DocEntry, t0.DocNum, t0.CardCode, t0.CardName, t0.Estado, t0.FechaSapTicket,");
            queryBuilder.AppendLine("    (");
            queryBuilder.AppendLine("        SELECT TOP 1 HoraOperacion");
            queryBuilder.AppendLine("        FROM vt.CC_ORTV");
            queryBuilder.AppendLine("        WHERE DocEntry = t0.DocEntry AND Operacion = 'REGISTRAR'");
            queryBuilder.AppendLine("        ORDER BY FechaOperacion, HoraOperacion DESC");
            queryBuilder.AppendLine("    ) AS 'HoraAbierto',");
            queryBuilder.AppendLine("   t0.LugarDestino, t0.Flete, t0.Vendedor, t0.EstadoPago, t0.EstadoGasto,");
            queryBuilder.AppendLine("   t0.PagoEnv, t0.TipoVenta, t0.EstadoFacturacion, t0.DescuentoNC, t0.Zona, t0.TiempoEntrega, t0.AlmProcedencia,");
            queryBuilder.AppendLine("   subBusquedaProducto.Estado AS EstadoBusquedaProducto,");
            queryBuilder.AppendLine("   CASE");
            queryBuilder.AppendLine("       WHEN EXISTS (SELECT 1 FROM vt.CC_ORTV_print WHERE DocEntryTicket = t0.DocEntry AND Id_Usuario = 'Facturacion') THEN 1 ELSE 0 END AS ExisteEnCC_ORTV_print");
            queryBuilder.AppendLine("FROM vt.ORTV t0");
            if (!string.IsNullOrEmpty(joinObservacion))
                queryBuilder.AppendLine(joinObservacion);
            queryBuilder.AppendLine("OUTER APPLY (");
            queryBuilder.AppendLine("   SELECT TOP 1 Estado");
            queryBuilder.AppendLine("   FROM vt.BusquedaProducto bp");
            queryBuilder.AppendLine("   WHERE bp.DocEntry = t0.DocEntry");
            queryBuilder.AppendLine(") AS subBusquedaProducto");
            queryBuilder.AppendLine("WHERE");
            queryBuilder.AppendLine("    YEAR(T0.FechaSapTicket) = 2025");
            queryBuilder.AppendLine("    AND ((SELECT TOP 1 Estado FROM vt.BusquedaProducto WHERE DocEntry=T0.DocEntry) = 'CONCLUIDO'");
            queryBuilder.AppendLine("    OR NOT EXISTS (SELECT 1 FROM vt.BusquedaProducto WHERE DocEntry=T0.DocEntry))");
            queryBuilder.AppendLine($"    {condWhere}");
            queryBuilder.AppendLine("ORDER BY");
            queryBuilder.AppendLine($"   {orderBy} CASE WHEN t0.EstadoFacturacion = 'PENDIENTE'");
            queryBuilder.AppendLine("       THEN 0 WHEN t0.EstadoFacturacion = 'GRE EMITIDA'");
            queryBuilder.AppendLine("       THEN 1 WHEN t0.EstadoFacturacion = 'FACTURADO'");
            queryBuilder.AppendLine("       THEN 2 ELSE 3");
            queryBuilder.AppendLine("   END,");
            queryBuilder.AppendLine("   subBusquedaProducto.Estado DESC, t0.TiempoEntrega, t0.DocNum DESC");

            string query = queryBuilder.ToString();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ORTV_E ticket = new ORTV_E();
                            if (!dr.IsDBNull(0)) { ticket.DocEntry = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { ticket.DocNum = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { ticket.CardCode = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { ticket.CardName = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { ticket.Estado = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { ticket.FechaSapTicket = dr.GetDateTime(5).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(6)) { ticket.HoraAbierto = dr.GetTimeSpan(6).ToString(); }
                            if (!dr.IsDBNull(7)) { ticket.LugarDestino = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { ticket.Flete = dr.GetDecimal(8); }
                            if (!dr.IsDBNull(9)) { ticket.Vendedor = dr.GetString(9); }
                            if (!dr.IsDBNull(10)) { ticket.EstadoPago = dr.GetString(10); }
                            if (!dr.IsDBNull(11)) { ticket.EstadoGasto = dr.GetString(11); }
                            if (!dr.IsDBNull(12)) { ticket.PagoEnv = dr.GetDecimal(12); }
                            if (!dr.IsDBNull(13)) { ticket.TipoVenta = dr.GetString(13); }
                            if (!dr.IsDBNull(14)) { ticket.EstadoFacturacion = dr.GetString(14); }
                            if (!dr.IsDBNull(15)) { ticket.DescuentoNC = dr.GetDecimal(15); }
                            if (!dr.IsDBNull(16)) { ticket.Zona = dr.GetString(16); }
                            if (!dr.IsDBNull(17)) { ticket.TiempoEntrega = dr.GetDateTime(17); }
                            if (!dr.IsDBNull(18)) { ticket.AlmProcedencia = dr.GetString(18); }
                            if (!dr.IsDBNull(19))
                            {
                                var EstadoProductoPendiente = dr.GetString(19);
                                if (EstadoProductoPendiente == "PENDIENTE") { ticket.ProductoPendiente = 1; }
                                else { ticket.ProductoPendiente = 0; }
                            }
                            if (!dr.IsDBNull(20)) { ticket.Impreso = dr.GetInt32(20); }
                            ticket.FechaSapTicket = (ticket.FechaSapTicket != null) ? Convert.ToDateTime(ticket.FechaSapTicket).ToString("dd/MM/yyyy") : null;
                            ticket.Det1 = obtenerDet1Ticket(ticket.DocEntry); if (ticket.Det1.Count == 0) { ticket.Det1 = null; }      //Datos de recojo
                            ticket.Det2 = obtenerDet2Ticket(ticket.DocEntry);

                            var validLugarDestino = new List<string> { "Domicilio", "Agencia" };
                            if (validLugarDestino.Contains(ticket.LugarDestino))
                            {
                                //Verificar si la zona es diferente a la de orden de venta.
                                var ordrD = new Capa_Datos.Ventas_DAO.Tablas.ORDR_D();
                                var crd1D = new Capa_Datos.SocioNegocios_DAO.TablasExternas.CRD1_D();
                                var nroSap = ticket.Det2[0].NroSap;
                                var ordenDeVenta = ordrD.obtenerOrdenDeVenta(nroSap);
                                var zonaPedido = crd1D.BuscarZonaPedido(ordenDeVenta.ShipToCode, ticket.CardCode);
                                if (ticket.Zona != zonaPedido) { ticket.zonaDistinta = true; }
                            }

                            //Busco si tiene obligatoriamente Fin verificar si tiene Filtro de DOCNUM
                            CC_ORTV_D ccOrtv = new CC_ORTV_D();
                            ticket.hayFinVerificar = false;
                            var hayFinVerificar = ccOrtv.ListarCC_ORTV(ticket.DocEntry, "FIN VERIFICAR", false);
                            var hayAnularFinVerificar = ccOrtv.ListarCC_ORTV(ticket.DocEntry, "ANULAR FIN VERIFICAR", false);

                            List<CC_ORTV_E> listOperacionVerificar = new List<CC_ORTV_E>();
                            if (hayFinVerificar != null && hayFinVerificar.Count > 0)
                                listOperacionVerificar.Add(hayFinVerificar.FirstOrDefault());

                            if (hayAnularFinVerificar != null && hayAnularFinVerificar.Count > 0)
                                listOperacionVerificar.Add(hayAnularFinVerificar.FirstOrDefault());

                            var listaOrdenada = listOperacionVerificar.OrderByDescending(x => x.Id).ToList();
                            if (listaOrdenada.FirstOrDefault() != null && !string.IsNullOrWhiteSpace(listaOrdenada[0].Operacion) && listaOrdenada[0].Operacion.Equals("FIN VERIFICAR"))
                                ticket.hayFinVerificar = true;

                            //si solo se desea ver los tickets ya cargados que tengan una zona distinta
                            if (t.zonaDistinta)
                            {
                                if (ticket.zonaDistinta) { lista.Add(ticket); }
                            }
                            else
                            {
                                lista.Add(ticket);
                            }
                        }
                    }
                }
                catch (Exception e) { throw new Exception(e.Message); }
            }
            return lista;
        }
        public List<ORTV_E> ListarTicketsAreaRecepcion(Usuario_E user, ORTV_E t)
        {
            List<ORTV_E> lista = new List<ORTV_E>();
            string condWhere = string.Empty; string orderby = "t0.DocNum DESC";
            string whereSubConsulta = string.Empty;
            if (t != null)
            {
                condWhere += $" AND t0.Estado not in ('SEPARADO')";
                //Cuando viene sin parametros de filtro 
                if (t.DocNum == 0 && t.FechaSapTicket == null && t.CardName == null && t.Vendedor == null && t.MontoTotal == 0 && t.LugarDestino == null && t.Estado == null
                     && t.EstadoPago == null)
                {
                    //Y los roles conectados son:
                    //4 supervisor almacen
                    //5 operario almacen recepcion
                    //51 operario almacen verificador
                    if (user.IdRol == 5 || user.IdRol == 4 || user.IdRol == 51)
                    {
                        condWhere += $" AND t0.Visible in ('PI','SI') AND t0.Presupuesto in ('NO') AND t0.Estado in ('ABIERTO','RECIBIDO') ";
                        condWhere += t.AlmProcedencia != null ? $" AND ((t0.LugarDestino IN ('Centro', 'Arriola') AND t0.AlmProcedencia IN ('{t.AlmProcedencia}')) " +
                            $" OR (t0.LugarDestino IN ('Domicilio', 'Agencia') AND (SELECT TOP 1 T2.AlmacenSalida FROM vt.RTV2 T2 WHERE T2.AlmacenSalida NOT IN ('07') AND T2.DocEntry = t0.DocEntry) IN ('{t.AlmProcedencia}')))" : "";
                        orderby = "t0.Estado ,t0.FechaSapTicket desc";
                    }
                }
                else
                {
                    if (user.IdRol == 5 || user.IdRol == 4 || user.IdRol == 51)
                    {
                        condWhere += $" AND t0.Visible in ('PI','SI') AND t0.Presupuesto in ('NO') ";
                    }
                    condWhere += t.AlmProcedencia != null ? $" AND ((t0.LugarDestino IN ('Centro', 'Arriola') AND t0.AlmProcedencia IN ('{t.AlmProcedencia}')) " +
                            $" OR (t0.LugarDestino IN ('Domicilio', 'Agencia') AND (SELECT TOP 1 T2.AlmacenSalida FROM vt.RTV2 T2 WHERE T2.AlmacenSalida NOT IN ('07') AND T2.DocEntry = t0.DocEntry) IN ('{t.AlmProcedencia}')))" : "";
                    condWhere += t.DocNum > 0 ? $" AND t0.DocNum like '%{t.DocNum}%'" : "";
                    condWhere += t.FechaSapTicket != null ? $" AND t0.FechaSapTicket='{t.FechaSapTicket}'" : "";
                    condWhere += t.CardName != null ? $" AND t0.CardName like '%{t.CardName}%'" : "";
                    condWhere += t.Vendedor != null ? $" AND t0.Vendedor like '%{t.Vendedor}%'" : "";
                    condWhere += t.MontoTotal > 0 ? $" AND t0.MontoTotal like '{t.MontoTotal}%'" : "";
                    condWhere += t.LugarDestino != null ? $" AND t0.LugarDestino='{t.LugarDestino}'" : "";
                    condWhere += t.Estado != null ? $" AND t0.Estado='{t.Estado}'" : "";
                    condWhere += t.EstadoPago != null ? $" AND t0.EstadoPago='{t.EstadoPago}'" : "";
                }
            }
            string query = $"SELECT TOP 100 t0.DocEntry, t0.DocNum, t0.CardCode, t0.CardName, t0.Estado,t0.FechaSapTicket, (Select top 1 concat(FechaOperacion,' ',HoraOperacion) from vt.CC_ORTV where DocEntry=t0.DocEntry " +
                $" and Operacion='REGISTRAR' order by FechaOperacion,HoraOperacion desc ) as 'TiempoAbierto',t0.LugarDestino,t0.Vendedor,t0.EstadoPago, t0.TipoVenta,t0.MontoTotal FROM vt.ORTV t0  WHERE 1=1 {condWhere} ORDER BY {orderby}";
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ORTV_E ticket = new ORTV_E();
                            if (!dr.IsDBNull(0)) { ticket.DocEntry = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { ticket.DocNum = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { ticket.CardCode = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { ticket.CardName = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { ticket.Estado = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { ticket.FechaSapTicket = dr.GetDateTime(5).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(6)) { ticket.FechaAbierto = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { ticket.LugarDestino = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { ticket.Vendedor = dr.GetString(8); }
                            if (!dr.IsDBNull(9)) { ticket.EstadoPago = dr.GetString(9); }
                            if (!dr.IsDBNull(10)) { ticket.TipoVenta = dr.GetString(10); }
                            if (!dr.IsDBNull(11)) { ticket.MontoTotal = dr.GetDecimal(11); }
                            ticket.Vendedor = (ticket.Vendedor.Length > 15) ? ticket.Vendedor.Substring(0, 15) : ticket.Vendedor;
                            ticket.FechaSapTicket = (ticket.FechaSapTicket != null) ? Convert.ToDateTime(ticket.FechaSapTicket).ToString("dd/MM/yyyy") : null;
                            ticket.FechaAbierto = (ticket.FechaAbierto != null) ? Convert.ToDateTime(ticket.FechaAbierto).ToString("dd/MM/yyyy HH:mm:ss") : null;
                            ticket.Det2 = obtenerDet2Ticket(ticket.DocEntry); if (ticket.Det2.Count == 0) { ticket.Det2 = null; }     //Ordenes de venta
                            lista.Add(ticket);
                        }
                    }
                }
                catch (Exception e) { throw new Exception(e.Message); }
            }
            return lista;
        }
        public List<ORTV_E> ListarTicketsAreaAlmacén(Usuario_E user, ORTV_E t)
        {
            List<ORTV_E> lista = new List<ORTV_E>();
            CC_ORTV_D ccORTV = new CC_ORTV_D();
            string condWhere = string.Empty;
            if (t != null)
            {
                condWhere += $" AND t0.Estado not in ('SEPARADO')";
                condWhere += t.DocNum > 0 ? $" AND t0.DocNum like '%{t.DocNum}%'" : "";
                condWhere += t.FechaSapTicket != null ? $" AND t0.FechaSapTicket='{t.FechaSapTicket}'" : "";
                condWhere += t.LugarDestino != null ? $" AND t0.LugarDestino='{t.LugarDestino}'" : "";
                condWhere += t.CardName != null ? $" AND t0.CardName like '%{t.CardName}%'" : "";
                condWhere += t.Vendedor != null ? $" AND t0.Vendedor like '%{t.Vendedor}%'" : "";
                condWhere += t.Zona != null ? $" AND t0.Zona ='{t.Zona}'" : "";
                condWhere += t.MontoTotal > 0 ? $" AND t0.MontoTotal like '{t.MontoTotal}%'" : "";
                condWhere += t.MontoFinal > 0 ? $" AND t0.MontoFinal like '{t.MontoFinal}%'" : "";
                condWhere += t.LugarDestino != null ? $" AND t0.LugarDestino='{t.LugarDestino}'" : "";
                condWhere += t.Estado != null ? $" AND t0.Estado='{t.Estado}'" : "";
                condWhere += t.EstadoFacturacion != null ? $" AND t0.EstadoFacturacion='{t.EstadoFacturacion}'" : "";
                condWhere += t.EstadoPago != null ? $" AND t0.EstadoPago='{t.EstadoPago}'" : "";
                condWhere += t.TipoVenta != null ? $" AND t0.TipoVenta='{t.TipoVenta}'" : "";
                condWhere += t.EstadoGasto != null ? $" AND t0.EstadoGasto='{t.EstadoGasto}'" : "";
                condWhere += t.Flete == 0.01M ? " AND t0.Flete>0" : "";
                condWhere += t.DescuentoNC == 0.01M ? " AND t0.DescuentoNC>0" : "";
                condWhere += t.TiempoEntrega != null ? $" AND CONVERT(char(10), t0.TiempoEntrega,126) = '{Convert.ToDateTime(t.TiempoEntrega).ToString("yyyy-MM-dd")}'" : "";
                if (t.ProductoPendiente != null)
                {
                    int valorProductoPendiente = (int)t.ProductoPendiente;
                    //Si es 0 esta en CONCLUIDO y si es 1 es PENDIENTE
                    var ProductoPendienteTexto = (valorProductoPendiente == 0) ? "CONCLUIDO" : "PENDIENTE";
                    condWhere += ProductoPendienteTexto != null ? $" AND (Select Estado from vt.BusquedaProducto where DocEntry=t0.DocEntry) ='{ProductoPendienteTexto}'" : "";
                }
            }
            string query = $"SELECT TOP 100 t0.DocEntry, t0.DocNum, t0.CardCode, t0.CardName, t0.Estado,t0.FechaSapTicket, (Select top 1 HoraOperacion from vt.CC_ORTV where DocEntry=t0.DocEntry " +
                $" and Operacion='REGISTRAR' order by FechaOperacion,HoraOperacion desc ) as 'HoraAbierto',t0.LugarDestino,t0.Flete,t0.Vendedor,t0.EstadoPago,t0.EstadoGasto," +
                $" t0.PagoEnv,t0.TipoVenta ,t0.EstadoFacturacion,t0.DescuentoNC,t0.Zona,t0.TiempoEntrega ,t0.MontoTotal,t0.Cajas,t0.LugarDestino, (Select Estado from vt.BusquedaProducto where DocEntry=t0.DocEntry) FROM vt.ORTV t0  WHERE 1=1 {condWhere} ORDER BY t0.DocNum DESC";
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ORTV_E ticket = new ORTV_E();
                            RTV11_D datosRTV11 = new RTV11_D();
                            RTV12_D datosRTV12 = new RTV12_D();
                            RTV13_D datosRTV13 = new RTV13_D();
                            if (!dr.IsDBNull(0)) { ticket.DocEntry = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { ticket.DocNum = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { ticket.CardCode = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { ticket.CardName = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { ticket.Estado = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { ticket.FechaSapTicket = dr.GetDateTime(5).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(6)) { ticket.HoraAbierto = dr.GetTimeSpan(6).ToString(); }
                            if (!dr.IsDBNull(7)) { ticket.LugarDestino = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { ticket.Flete = dr.GetDecimal(8); }
                            if (!dr.IsDBNull(9)) { ticket.Vendedor = dr.GetString(9); }
                            if (!dr.IsDBNull(10)) { ticket.EstadoPago = dr.GetString(10); }
                            if (!dr.IsDBNull(11)) { ticket.EstadoGasto = dr.GetString(11); }
                            if (!dr.IsDBNull(12)) { ticket.PagoEnv = dr.GetDecimal(12); }
                            if (!dr.IsDBNull(13)) { ticket.TipoVenta = dr.GetString(13); }
                            if (!dr.IsDBNull(14)) { ticket.EstadoFacturacion = dr.GetString(14); }
                            if (!dr.IsDBNull(15)) { ticket.DescuentoNC = dr.GetDecimal(15); }
                            if (!dr.IsDBNull(16)) { ticket.Zona = dr.GetString(16); }
                            if (!dr.IsDBNull(17)) { ticket.TiempoEntrega = dr.GetDateTime(17); }
                            if (!dr.IsDBNull(18)) { ticket.MontoTotal = dr.GetDecimal(18); }
                            if (!dr.IsDBNull(19)) { ticket.Cajas = dr.GetInt32(19); }
                            if (!dr.IsDBNull(20)) { ticket.LugarDestino = dr.GetString(20); }
                            if (!dr.IsDBNull(21))
                            {
                                var ProductoPendiente = dr.GetString(21);
                                if (ProductoPendiente == "PENDIENTE")
                                {
                                    ticket.ProductoPendiente = 1;
                                }
                                else { ticket.ProductoPendiente = 0; }
                            }
                            ticket.FechaSapTicket = (ticket.FechaSapTicket != null) ? Convert.ToDateTime(ticket.FechaSapTicket).ToString("dd/MM/yyyy") : null;
                            //Buscamos el ultimo estado del ticket excluyendo a los estados que no trascienden en las operaciones del ticket.
                            ticket.ultimoCCEstado = _ccTicketD.ListarCC_ORTV(ticket.DocEntry, null, true).FirstOrDefault()?.Operacion;
                            //Consulta referencia para los estados, acopla los nuevos datos sin perder lo anterior consultado.
                            ObtenerReferenciaEstadosTicket(ticket);
                            /**************************************/
                            if (ticket.hayFinPicking)
                            {
                                // Trae el operario de picking principal y sus apoyos 
                                List<CC_ORTV_E> ticketSacando = ccORTV.ListarCC_ORTV(ticket.DocEntry, "FIN PICKING");
                                if (ticketSacando != null && ticketSacando.Count > 0)
                                {
                                    ticket.OpSacando = ticketSacando[0].Operario;
                                }
                                List<string> operariosSacando = datosRTV11.BuscarOperariosSacando(ticket.DocEntry);
                                if (operariosSacando != null && operariosSacando.Count > 0)
                                {
                                    ticket.OpSacandoApoyo = operariosSacando;
                                }
                            }
                            if (ticket.hayFinVerificar)
                            {
                                // Trae el operario de verificacion principal y sus apoyos 
                                List<CC_ORTV_E> ticketVerificando = ccORTV.ListarCC_ORTV(ticket.DocEntry, "FIN VERIFICAR");
                                if (ticketVerificando != null && ticketVerificando.Count > 0)
                                {
                                    ticket.OpVerificado = ticketVerificando[0].Operario;
                                    List<string> operariosChequeando = datosRTV12.BuscarOperariosChequeando(ticket.DocEntry);
                                    if (operariosChequeando != null && operariosChequeando.Count > 0)
                                    {
                                        ticket.OpVerificadoApoyo = operariosChequeando;
                                    }
                                }
                                if (ticket.hayFinEmpacar && ticket.Cajas >= 1)
                                {
                                    // Trae el operario de packing principal y sus apoyos
                                    List<CC_ORTV_E> ticketEmpacando = ccORTV.ListarCC_ORTV(ticket.DocEntry, "FIN EMPACAR");
                                    if (ticketEmpacando != null && ticketEmpacando.Count > 0) { ticket.OpEmpacado = ticketEmpacando[0].Operario; }
                                    List<string> operariosEmpacando = datosRTV13.BuscarOperariosEmpacando(ticket.DocEntry);
                                    if (operariosEmpacando != null && operariosEmpacando.Count > 0)
                                    {
                                        ticket.OpEmpacadoApoyo = operariosEmpacando;
                                    }
                                }
                            }
                            lista.Add(ticket);
                        }
                    }
                }
                catch (Exception e) { throw new Exception(e.Message); }
            }
            return lista;
        }
        public List<ORTV_E> ListarTicketsAreaDespacho(Usuario_E user, ORTV_E t)
        {
            List<ORTV_E> lista = new List<ORTV_E>();
            string condWhere = string.Empty;
            if (t != null)
            {
                condWhere += $" AND t0.Estado not in ('SEPARADO')";
                if (user.IdRol == 53)
                {
                    if (t.DocNum == 0 && t.FechaSapTicket == null && t.CardName == null && t.Vendedor == null && t.MontoFinal == 0 && t.Estado == null
                    && t.EstadoFacturacion == null && t.EstadoPago == null && t.TipoVenta == null && t.TiempoEntrega == null && t.Zona == null)
                    {
                        condWhere += $" AND t0.Estado in ( 'PICKEANDO','VERIFICANDO','EMPACANDO','EMPACADO','PESADO','ENVIADO','PREENVIO','ENTREGADO') " +
                            $"AND t0.EstadoFacturacion in ('GRE EMITIDA','FACTURADO') AND t0.LugarDestino='{t.LugarDestino}'";
                    }
                    else
                    {
                        condWhere += t.DocNum > 0 ? $" AND t0.DocNum like '%{t.DocNum}%'" : "";
                        condWhere += t.FechaSapTicket != null ? $" AND t0.FechaSapTicket='{t.FechaSapTicket}'" : "";
                        condWhere += t.CardName != null ? $" AND t0.CardName like '%{t.CardName}%'" : "";
                        condWhere += t.Vendedor != null ? $" AND t0.Vendedor like '%{t.Vendedor}%'" : "";
                        condWhere += t.MontoFinal > 0 ? $" AND t0.MontoFinal like '{t.MontoFinal}%'" : "";
                        condWhere += t.LugarDestino != null ? $" AND t0.LugarDestino='{t.LugarDestino}'" : "";
                        condWhere += t.Zona != null ? $" AND t0.Zona ='{t.Zona}'" : "";
                        condWhere += t.Estado != null ? $" AND t0.Estado='{t.Estado}'" : "";
                        condWhere += t.EstadoFacturacion != null ? $" AND t0.EstadoFacturacion='{t.EstadoFacturacion}'" : "";
                        condWhere += t.EstadoPago != null ? $" AND t0.EstadoPago='{t.EstadoPago}'" : "";
                        condWhere += t.TipoVenta != null ? $" AND t0.TipoVenta='{t.TipoVenta}'" : "";
                        condWhere += t.TiempoEntrega != null ? $" AND CONVERT(varchar, t0.TiempoEntrega,121) = '{Convert.ToDateTime(t.TiempoEntrega).ToString("yyyy-MM-dd HH:mm:ss.fff")}'" : "";
                    }
                }
                else
                {
                    condWhere += t.DocNum > 0 ? $" AND t0.DocNum like '%{t.DocNum}%'" : "";
                    condWhere += t.FechaSapTicket != null ? $" AND t0.FechaSapTicket='{t.FechaSapTicket}'" : "";
                    condWhere += t.CardName != null ? $" AND t0.CardName like '%{t.CardName}%'" : "";
                    condWhere += t.Vendedor != null ? $" AND t0.Vendedor like '%{t.Vendedor}%'" : "";
                    condWhere += t.MontoFinal > 0 ? $" AND t0.MontoFinal like '{t.MontoFinal}%'" : "";
                    condWhere += t.Zona != null ? $" AND t0.Zona ='{t.Zona}'" : "";
                    condWhere += t.LugarDestino != null ? $" AND t0.LugarDestino='{t.LugarDestino}'" : "";
                    condWhere += t.Estado != null ? $" AND t0.Estado='{t.Estado}'" : "";
                    condWhere += t.EstadoFacturacion != null ? $" AND t0.EstadoFacturacion='{t.EstadoFacturacion}'" : "";
                    condWhere += t.EstadoPago != null ? $" AND t0.EstadoPago='{t.EstadoPago}'" : "";
                    condWhere += t.TipoVenta != null ? $" AND t0.TipoVenta='{t.TipoVenta}'" : "";
                    condWhere += t.TiempoEntrega != null ? $" AND CONVERT(varchar, t0.TiempoEntrega,121) = '{Convert.ToDateTime(t.TiempoEntrega).ToString("yyyy-MM-dd HH:mm:ss.fff")}'" : "";
                }
            }
            string query = $"SELECT TOP 100 t0.DocEntry, t0.DocNum, t0.CardCode, t0.CardName, t0.Estado,t0.FechaSapTicket, (Select top 1 HoraOperacion from vt.CC_ORTV where DocEntry=t0.DocEntry " +
                $" and Operacion='REGISTRAR' order by FechaOperacion,HoraOperacion desc ) as 'HoraAbierto',t0.LugarDestino,t0.Flete,t0.Vendedor,t0.EstadoPago,t0.EstadoGasto," +
                $" T0.PagoEnv,t0.TipoVenta ,t0.EstadoFacturacion,t0.DescuentoNC,t0.TiempoEntrega ,CASE WHEN EXISTS (SELECT * FROM vt.CC_ORTV_print WHERE DocEntryTicket = t0.DocEntry and Id_Usuario = 'DespachoCentroArriola') THEN 1 ELSE 0 END,T0.MontoFinal, " +
                $" T0.Cajas , T0.Zona, T0.AlmProcedencia FROM vt.ORTV t0  WHERE 1=1 {condWhere} ORDER BY t0.DocNum desc";
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ORTV_E ticket = new ORTV_E();
                            if (!dr.IsDBNull(0)) { ticket.DocEntry = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { ticket.DocNum = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { ticket.CardCode = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { ticket.CardName = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { ticket.Estado = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { ticket.FechaSapTicket = dr.GetDateTime(5).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(6)) { ticket.HoraAbierto = dr.GetTimeSpan(6).ToString(); }
                            if (!dr.IsDBNull(7)) { ticket.LugarDestino = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { ticket.Flete = dr.GetDecimal(8); }
                            if (!dr.IsDBNull(9)) { ticket.Vendedor = dr.GetString(9); }
                            if (!dr.IsDBNull(10)) { ticket.EstadoPago = dr.GetString(10); }
                            if (!dr.IsDBNull(11)) { ticket.EstadoGasto = dr.GetString(11); }
                            if (!dr.IsDBNull(12)) { ticket.PagoEnv = dr.GetDecimal(12); }
                            if (!dr.IsDBNull(13)) { ticket.TipoVenta = dr.GetString(13); }
                            if (!dr.IsDBNull(14)) { ticket.EstadoFacturacion = dr.GetString(14); }
                            if (!dr.IsDBNull(15)) { ticket.DescuentoNC = dr.GetDecimal(15); }
                            if (!dr.IsDBNull(16)) { ticket.TiempoEntrega = dr.GetDateTime(16); }
                            if (!dr.IsDBNull(17)) { ticket.Impreso = dr.GetInt32(17); }
                            if (!dr.IsDBNull(18)) { ticket.MontoFinal = dr.GetDecimal(18); }
                            if (!dr.IsDBNull(19)) { ticket.Cajas = dr.GetInt32(19); }
                            if (!dr.IsDBNull(20)) { ticket.Zona = dr.GetString(20); }
                            if (!dr.IsDBNull(21)) ticket.AlmProcedencia = dr.GetString(21);

                            ticket.FechaSapTicket = (ticket.FechaSapTicket != null) ? Convert.ToDateTime(ticket.FechaSapTicket).ToString("dd/MM/yyyy") : null;
                            lista.Add(ticket);
                        }
                    }
                }
                catch (Exception e) { throw new Exception(e.Message); }
            }
            return lista;
        }

        public bool GuardarComentario(int docEntry, string comentario)
        {
            bool exito = false;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    string updateQuery = "UPDATE vt.ComentarioFac SET Comentario = @Comentario WHERE DocEntry = @DocEntry";
                    string insertQuery = "INSERT INTO vt.ComentarioFac (DocEntry, Comentario) VALUES (@DocEntry, @Comentario)";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, cn))
                    {
                        cmd.Parameters.AddWithValue("@Comentario", (object)comentario ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DocEntry", docEntry);

                        cn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            // No existe, así que lo insertamos
                            cmd.CommandText = insertQuery;
                            rowsAffected = cmd.ExecuteNonQuery();
                        }

                        exito = rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al guardar el comentario para el DocEntry {docEntry}.", ex);
            }

            return exito;
        }



        public string LeerComentario(int docEntry)
        {
            string comentarioFac = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT Comentario FROM vt.ComentarioFac WHERE DocEntry = @DocEntry", cn))
                    {
                        cmd.Parameters.AddWithValue("@DocEntry", docEntry);

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read() && !dr.IsDBNull(0))
                            {
                                comentarioFac = dr.GetString(0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al leer el comentario del DocEntry {docEntry}.", ex);
            }

            return comentarioFac;
        }

        public int AgenciaFill(string Agencia)
        {
            int resultado = 0;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    cn.Open();
                    string query = "SELECT fill FROM dbo.AgenciaFill WHERE UPPER(agencia) = TRIM(UPPER(@Agencia))";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@Agencia", Agencia);
                        object valor = cmd.ExecuteScalar();
                        if (valor != null && valor != DBNull.Value)
                        {
                            resultado = Convert.ToInt32(valor);
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                // Puedes registrar el error si tienes un logger
                resultado = -1; // -1 indica error
            }
            return resultado;
        }

        public int RevertirAsignacionRegalo(int docEntry, ORTV_E ticket)
        {
            if (ticket == null) throw new ArgumentNullException(nameof(ticket));
            int resultado = 0;
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                cn = new SqlConnection(new Utilitarios().cadSql);
                cmd = new SqlCommand("vt.MANT_OREG", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@TipoMantenimiento", "REVASIG");
                var pId = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(pId);

                cmd.Parameters.AddWithValue("@OpRegistro", ticket.Operario ?? string.Empty);
                cmd.Parameters.AddWithValue("@Detalle", ticket.DocNum.ToString());

                var tvp = new DataTable();
                tvp.Columns.Add("IdReg", typeof(int));
                tvp.Columns.Add("StockComp", typeof(decimal));
                cmd.Parameters.Add(new SqlParameter("@TablaDatos", SqlDbType.Structured)
                {
                    TypeName = "dbo.TbDatosReg",
                    Value = tvp
                });

                cn.Open();
                cmd.ExecuteNonQuery();
                resultado = pId.Value != DBNull.Value ? (int)pId.Value : 0;
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al revertir la asignación de regalo: " + ex.Message, ex);
            }
            return resultado;
        }

    }
}
