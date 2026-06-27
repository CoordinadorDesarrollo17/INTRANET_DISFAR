using Capa_Datos.Ventas_DAO.Tablas;
using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Rutas_ENT.ReportesSql;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using static Capa_Entidad.Rutas_ENT.TablasSql.ORRU_E;

namespace Capa_Datos.Rutas_DAO.TablasSql
{
    public class ORRU_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        ORTV_D ticketD = new ORTV_D();
        public List<ORRU_E> Listar(ORRU_E o)
        {
            List<ORRU_E> lista = new List<ORRU_E>();
            string query = string.Empty;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                if (o != null)
                {
                    // ✅ USAR ISNULL para asegurar que siempre devuelva algo, incluso si Socio es NULL
                    query = @"SELECT DISTINCT TOP 100 
                        x.DocEntry,
                        x.DocNum,
                        x.TipoRuta,
                        x.TransDesc,
                        x.FechaDoc,
                        x.Estado,
                        (SELECT SUM(cajas) FROM al.rru0 WHERE DocEntry=x.DocEntry AND Estado<>'LIBERADO'),
                        (SELECT SUM(cajas) FROM al.rru1 WHERE DocEntry=x.DocEntry AND Estado<>'LIBERADO'),
                        x.TiempoPac,
                        ISNULL((SELECT TOP 1 Socio FROM al.RRU0 WHERE DocEntry=x.DocEntry AND Socio IS NOT NULL ORDER BY Linea), '') AS ClienteNombre
                      FROM al.orru x 
                      WHERE DocNum>0 ";

                    if (o.DocNum > 0) { query += " AND DocNum=" + o.DocNum; }
                    if (o.TipoRuta != null) { query += " AND TipoRuta='" + o.TipoRuta.Replace("'", "''") + "'"; }
                    if (o.TransDesc != null) { query += " AND TransDesc like '%" + o.TransDesc.Replace("'", "''") + "%'"; }
                    if (o.FechaDoc != null) { query += " AND FechaDoc='" + o.FechaDoc + "'"; }
                    if (o.Estado != null) { query += " AND Estado='" + o.Estado.Replace("'", "''") + "'"; }
                    if (o.TiempoPac != null) { query += " AND CONVERT(date,TiempoPac) = '" + Convert.ToDateTime(o.TiempoPac).ToString("yyyy-MM-dd") + "'"; }

                    // ✅ FILTRO POR CLIENTE - Busca en RRU0.Socio
                    if (!string.IsNullOrWhiteSpace(o.CardName))
                    {
                        query += " AND EXISTS (SELECT 1 FROM al.RRU0 r WHERE r.DocEntry=x.DocEntry AND r.Socio LIKE '%" + o.CardName.Replace("'", "''") + "%')";
                    }

                    query += " ORDER BY x.DocEntry DESC";
                }
                else
                {
                    query = @"SELECT TOP 100 
                        x.DocEntry,
                        x.DocNum,
                        x.TipoRuta,
                        x.TransDesc,
                        x.FechaDoc,
                        x.Estado,
                        (SELECT SUM(cajas) FROM al.rru0 WHERE DocEntry=x.DocEntry),
                        (SELECT SUM(cajas) FROM al.rru1 WHERE DocEntry=x.DocEntry),
                        x.TiempoPac,
                        ISNULL((SELECT TOP 1 Socio FROM al.RRU0 WHERE DocEntry=x.DocEntry AND Socio IS NOT NULL ORDER BY Linea), '') AS ClienteNombre
                      FROM al.orru x 
                      ORDER BY DocEntry DESC";
                }

                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ORRU_E or = new ORRU_E();
                    or.DocEntry = dr.GetInt32(0);
                    or.DocNum = dr.GetInt32(1);
                    if (!dr.IsDBNull(2)) { or.TipoRuta = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { or.TransDesc = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { or.FechaDoc = dr.GetDateTime(4); }
                    if (!dr.IsDBNull(5)) { or.Estado = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { or.TotalCajas = dr.GetInt32(6); }
                    if (!dr.IsDBNull(7)) { or.TotalCajas += dr.GetInt32(7); }
                    if (!dr.IsDBNull(8)) { or.TiempoPac = dr.GetDateTime(8); }

                    // ✅ Leer CardName (que viene del Socio de RRU0)
                    if (!dr.IsDBNull(9))
                    {
                        string valorCliente = dr.GetString(9);
                        // Solo asignar si no está vacío
                        or.CardName = string.IsNullOrWhiteSpace(valorCliente) ? "" : valorCliente;
                    }

                    lista.Add(or);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }

            return lista;
        }
        public List<RRU11_E> listaProductosOWTQ(string guia, string Origen)
        {
            List<RRU11_E> lista = new List<RRU11_E>();
            try
            {
                HanaDataReader hdr2 = db.HanaExecuteReaderNoSp("SELECT \"DocEntry\" FROM " + uti.schemaHana + "" + Origen + " WHERE \"U_SYP_MDTD\"||'-'||\"U_SYP_MDSD\"||'-'||\"U_SYP_MDCD\"='" + guia + "'");

                hdr2.Read();
                int DocEntry = hdr2.GetInt32(0);
                hdr2.Close();

                string a = string.Empty;
                if (Origen == "OWTR") { a = "_TR"; }

                HanaDataReader hdr3 = db.HanaExecuteReaderNoSp("CALL " + uti.schemaHana + "CBW_ARTICULOSRUTA" + a + "(" + DocEntry + ")");
                int linea = 0;
                while (hdr3.Read())
                {
                    linea++;
                    RRU11_E r = new RRU11_E();
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
            catch { }
            return lista;
        }
        public List<ORTV_E> listarTicketsVenta(string FechaCont, string CardCode)
        {
            List<ORTV_E> lista = new List<ORTV_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                string query = string.Empty;
                query = "select DocEntry,DocNum,FechaRegistro,CardCode,CardName,HoraRegistro,LugarDestino," +
                    "Vendedor ,MontoFinal,EstadoPago,Estado from vt.ortv where FechaRegistro between dateadd(day,-15,convert(datetime,'" + FechaCont + "',120)) and '" + FechaCont + "' " +
                    "and CardCode='" + CardCode + "' and Estado='EMPACADO' order by \"DocNum\" desc";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ORTV_E ticket = new ORTV_E();
                    ticket.DocEntry = dr.GetInt32(0);
                    ticket.DocNum = dr.GetInt32(1);
                    if (!dr.IsDBNull(2)) { ticket.FechaRegistro = dr.GetDateTime(2).ToString("dd-MM-yyyy"); }
                    if (!dr.IsDBNull(3)) { ticket.CardCode = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { ticket.CardName = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { ticket.HoraRegistro = dr.GetTimeSpan(5).ToString(@"hh\:mm"); }
                    if (!dr.IsDBNull(6)) { ticket.LugarDestino = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { ticket.Vendedor = dr.GetString(7).Substring(0, 12); }
                    if (!dr.IsDBNull(8)) { ticket.MontoFinal = dr.GetDecimal(8); }
                    if (!dr.IsDBNull(9)) { ticket.EstadoPago = dr.GetString(9); }
                    if (!dr.IsDBNull(10)) { ticket.Estado = dr.GetString(10); }
                    lista.Add(ticket);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public List<OWTQ_E> listarGuiasTraslado(string Origen)
        {
            List<OWTQ_E> lista = new List<OWTQ_E>();
            string query = "SELECT top 200 T0.\"DocNum\",IFNULL(T0.\"U_SYP_MDTD\"||'-'||T0.\"U_SYP_MDSD\"||'-'||T0.\"U_SYP_MDCD\",'') " +
                    " ,(select \"SlpName\" from " + uti.schemaHana + "oslp where \"SlpCode\" = T0.\"SlpCode\")" +
                    " FROM " + uti.schemaHana + "" + Origen +
                    " T0  WHERE T0.CANCELED = 'N' AND T0.\"U_SYP_MDTD\" IS NOT NULL AND T0.\"U_SYP_MDSD\" IS NOT NULL " +
                    " AND T0.\"U_SYP_MDCD\" IS NOT NULL " +
                    /*AGREGAR PARA SOLICITUDES DE GUIAS EXTRAORDINARIAS*/
                    " ORDER BY T0.\"DocEntry\" desc";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    OWTQ_E o = new OWTQ_E();
                    if (!hdr.IsDBNull(0)) { o.DocNum = hdr.GetInt32(0); }
                    if (!hdr.IsDBNull(1)) { o.NumAtCard = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { o.SlpName = hdr.GetString(2); }
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            ;
            return lista;
        }
        public ORRU_E obtenerOrdenDeRuta(int DocEntry)
        {
            ORRU_E o = new ORRU_E();
            o.DetRRU0 = new List<RRU0_E>();
            o.DetRRU01 = new List<RRU01_E>();
            o.DetRRU1 = new List<RRU1_E>();
            o.DetRRU11 = new List<RRU11_E>();

            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("select T1.DocEntry,T1.DocNum,T1.TipoRuta,T1.TransCod,T1.TransDesc,T1.VehiculoCod,T1.Placa," +
                    " T1.Marca,T1.Modelo,T1.CopilDesc,T1.Copil2Desc,T1.Copil3Desc,T1.Copil4Desc,T1.FechaCont,T1.FechaDoc," +
                    " T1.AlmOrigenCod,T1.AlmOrigenDesc,T1.AlmOrigenDesc2,T1.AlmDestinoCod,T1.AlmDestinoDesc,T1.AlmDestinoDesc2," +
                    " T1.Propietario,T1.TiempoPac,(select top 1 concat(FechaOperacion,' ',convert(varchar(8),HoraOperacion)) from al.CC_ORRU " +
                    " where Operacion='INICIAR' and DocEntry=t1.DocEntry order by FechaOperacion,HoraOperacion desc),(select top 1 concat(FechaOperacion,' ',convert(varchar(8),HoraOperacion)) from al.CC_ORRU " +
                    " where Operacion='TERMINAR' and DocEntry=t1.DocEntry order by FechaOperacion,HoraOperacion desc),T1.Estado,T1.Observaciones,(select top 1 Operario from al.CC_ORRU " +
                    " where Operacion='INICIAR' and DocEntry=t1.DocEntry order by FechaOperacion,HoraOperacion desc) ,(select top 1 Operario from " +
                    "al.CC_ORRU where Operacion='TERMINAR' and DocEntry=t1.DocEntry order by FechaOperacion,HoraOperacion desc),T1.Agencia,T1.RucAgencia," +
                    " (select SerieT1 from al.oveh where Code = T1.VehiculoCod), (select SerieT2 from al.oveh where Code = T1.VehiculoCod),T1.HoraRegistro " + 
                    "from al.ORRU T1 where T1.DocEntry=@DocEntry", cn);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { o.DocNum = dr.GetInt32(1); }
                if (!dr.IsDBNull(2)) { o.TipoRuta = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { o.TransCod = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { o.TransDesc = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { o.VehiculoCod = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { o.Placa = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { o.Marca = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { o.Modelo = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { o.CopilDesc = dr.GetString(9); }
                if (!dr.IsDBNull(10)) { o.Copil2Desc = dr.GetString(10); }
                if (!dr.IsDBNull(11)) { o.Copil3Desc = dr.GetString(11); }
                if (!dr.IsDBNull(12)) { o.Copil4Desc = dr.GetString(12); }
                if (!dr.IsDBNull(13)) { o.FechaCont = dr.GetDateTime(13); }
                if (!dr.IsDBNull(14)) { o.FechaDoc = dr.GetDateTime(14); }
                if (!dr.IsDBNull(15)) { o.AlmOrigenCod = dr.GetString(15); }
                if (!dr.IsDBNull(16)) { o.AlmOrigenDesc = dr.GetString(16); }
                if (!dr.IsDBNull(17)) { o.AlmOrigenDesc2 = dr.GetString(17); }
                if (!dr.IsDBNull(18)) { o.AlmDestinoCod = dr.GetString(18); }
                if (!dr.IsDBNull(19)) { o.AlmDestinoDesc = dr.GetString(19); }
                if (!dr.IsDBNull(20)) { o.AlmDestinoDesc2 = dr.GetString(20); }
                if (!dr.IsDBNull(21)) { o.Propietario = dr.GetString(21); }
                if (!dr.IsDBNull(22)) { o.TiempoPac = dr.GetDateTime(22); }
                if (!dr.IsDBNull(23)) { o.TiempoIniEn = dr.GetString(23); }
                if (!dr.IsDBNull(24)) { o.TiempoTerEn = dr.GetString(24); }
                if (!dr.IsDBNull(25)) { o.Estado = dr.GetString(25); }
                if (!dr.IsDBNull(26)) { o.Observaciones = dr.GetString(26); }
                if (!dr.IsDBNull(27)) { o.OpInicio = dr.GetString(27); }
                if (!dr.IsDBNull(28)) { o.OpTermino = dr.GetString(28); }
                if (!dr.IsDBNull(29)) { o.Agencia = dr.GetString(29); }
                if (!dr.IsDBNull(30)) { o.RucAgencia = dr.GetString(30); }
                if (!dr.IsDBNull(31)) { o.SerieT1 = dr.GetString(31); }
                if (!dr.IsDBNull(32)) { o.SerieT2 = dr.GetString(32); }
                if (!dr.IsDBNull(33)) { o.HoraRegistro = dr.GetTimeSpan(33).ToString(@"hh\:mm\:ss"); }  // ✅ Formato HH:mm:ss
                dr.Close();
                o.Operario = o.Propietario;

                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT R0.DocEntry, R0.Linea, R0.DocEntryTicket, R0.DocNumTicket, R0.Guias, R0.Verificado, R0.Cajas, R0.Observaciones, R0.MontoFinal, R0.Envio, R0.Direcciones,");
                sb.Append(" R0.Estado, R0.TempI1, R0.TempI2, R0.TempF1, R0.TempF2, R0.OpEntrega, R0.FechaEntrega, R0.HoraEntrega,");
                sb.Append(" R0.Socio, VT.TipoVenta, VT.EstadoPago, TC.TipoPago, TC.estado, TC.MontoRecibidoEfectivo, TC.Id, TC.MontoRecibidoDeposito, R0.ConducYPlaca, R0.EnvioAgencia");
                sb.Append(" FROM al.RRU0 R0");
                sb.Append(" LEFT JOIN vt.ORTV VT ON VT.DocEntry = R0.DocEntryTicket");
                sb.Append(" LEFT JOIN cj.OTC TC ON TC.DocEntryTicket = R0.DocEntryTicket AND TC.Estado NOT IN ('ANULADO', 'RECHAZADO')");
                sb.Append(" WHERE R0.DocEntry=@DocEntry");
                sb.Append(" ORDER BY CASE WHEN R0.FechaEntrega IS NOT NULL THEN 1 ELSE 0 END");         // Prioridad de orden: Entregados -> Enviado

                string query = sb.ToString();

                SqlCommand cmd2 = new SqlCommand(query, cn);
                cmd2.CommandType = CommandType.Text;
                cmd2.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr2 = cmd2.ExecuteReader();

                while (dr2.Read())
                {
                    RRU0_E d = new RRU0_E();
                    if (!dr2.IsDBNull(0)) d.DocEntry = dr2.GetInt32(0);
                    if (!dr2.IsDBNull(1)) d.Linea = dr2.GetInt32(1);
                    if (!dr2.IsDBNull(2)) d.DocEntryTicket = dr2.GetInt32(2);
                    if (!dr2.IsDBNull(3)) d.DocNumTicket = dr2.GetInt32(3);
                    if (!dr2.IsDBNull(4)) d.Guias = dr2.GetString(4);
                    if (!dr2.IsDBNull(5)) d.Verificado = dr2.GetString(5);
                    if (!dr2.IsDBNull(6)) d.Cajas = dr2.GetInt32(6);
                    if (!dr2.IsDBNull(7)) d.Observaciones = dr2.GetString(7);
                    if (!dr2.IsDBNull(8)) d.MontoFinal = dr2.GetDecimal(8);
                    if (!dr2.IsDBNull(9)) d.Envio = dr2.GetDecimal(9);
                    if (!dr2.IsDBNull(10)) d.Direcciones = dr2.GetString(10);
                    if (!dr2.IsDBNull(11)) d.Estado = dr2.GetString(11);
                    if (!dr2.IsDBNull(12)) d.TempI1 = dr2.GetDecimal(12);
                    if (!dr2.IsDBNull(13)) d.TempI2 = dr2.GetDecimal(13);
                    if (!dr2.IsDBNull(14)) d.TempF1 = dr2.GetDecimal(14);
                    if (!dr2.IsDBNull(15)) d.TempF2 = dr2.GetDecimal(15);
                    if (!dr2.IsDBNull(16)) d.OpEntrega = dr2.GetString(16);
                    if (!dr2.IsDBNull(17)) d.FechaEntrega = dr2.GetDateTime(17).ToString("yyyy-MM-dd");
                    if (!dr2.IsDBNull(18)) d.HoraEntrega = dr2.GetTimeSpan(18).ToString();
                    if (!dr2.IsDBNull(19)) d.Socio = dr2.GetString(19);
                    if (!dr2.IsDBNull(20)) d.TipoVenta = dr2.GetString(20);
                    if (!dr2.IsDBNull(21)) d.EstadoPago = dr2.GetString(21);
                    if (!dr2.IsDBNull(22)) d.TipoPagoTC = dr2.GetString(22);
                    if (!dr2.IsDBNull(23)) d.EstadoTC = dr2.GetString(23);
                    if (!dr2.IsDBNull(24)) d.MontoRecibidoEfectivo = dr2.GetDecimal(24);
                    if (!dr2.IsDBNull(25)) d.IdOTC = dr2.GetInt32(25);
                    if (!dr2.IsDBNull(26)) d.MontoRecibidoDeposito = dr2.GetDecimal(26);
                    if (!dr2.IsDBNull(27)) d.ConducYPlaca = dr2.GetString(27);
                    if (!dr2.IsDBNull(28)) d.EnvioAgencia = dr2.GetString(28);

                    ORTV_D ortv = new ORTV_D();
                    d.Ticket = ortv.ObtenerDatosCompletosTicket(d.DocEntryTicket);

                    // --- INICIO DEL CAMBIO ---
                    // Si hay un "Socio" نوشته شده manualmente en RRU0, buscamos su RUC y reemplazamos el del ticket
                    if (!string.IsNullOrWhiteSpace(d.Socio))
                    {
                        try
                        {
                            // NOTA: Asegúrate de usar una nueva conexión o verificar si 'cn' permite MultipleActiveResultSets. 
                            // Para mayor seguridad, aquí abro una conexión rápida auxiliar solo para este dato.
                            using (SqlConnection cnAux = new SqlConnection(uti.cadSql))
                            {
                                cnAux.Open();
                                // Buscamos el CardCode en ORTV usando el nombre del Socio
                                string sqlRuc = "SELECT TOP 1 CardCode FROM vt.ORTV WHERE CardName = @SocioName";
                                SqlCommand cmdRuc = new SqlCommand(sqlRuc, cnAux);
                                cmdRuc.Parameters.AddWithValue("@SocioName", d.Socio); // Importante: que sea idéntico

                                object resultado = cmdRuc.ExecuteScalar();

                                if (resultado != null)
                                {
                                    // ¡AQUÍ ESTÁ EL TRUCO! 
                                    // Engañamos a la vista poniendo el RUC del socio dentro del objeto Ticket
                                    d.Ticket.CardCode = resultado.ToString();
                                }
                                cnAux.Close();
                            }
                        }
                        catch
                        {
                            // Si falla la búsqueda, no hacemos nada y se queda el RUC original del ticket
                        }
                    }
                    // --- FIN DEL CAMBIO ---
                    o.DetRRU0.Add(d);
                }
                dr2.Close();
                o.TotalCajas = o.TotCajas();

                SqlCommand cmd20 = new SqlCommand("select DocEntryORRU,DocEntryTicket,Linea, TablaSAP,Identificador,U_SYP_MDTD,U_SYP_MDSD,U_SYP_MDCD,DocDate,U_BPP_FECINITRA,Impreso,Estado,Id " +
                   " from al.RRU01 where DocEntryORRU=@DocEntry", cn);
                cmd20.CommandType = CommandType.Text;
                cmd20.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr20 = cmd20.ExecuteReader();

                while (dr20.Read())
                {
                    RRU01_E d = new RRU01_E();
                    if (!dr20.IsDBNull(0)) { d.DocEntryORRU = dr20.GetInt32(0); }
                    if (!dr20.IsDBNull(1)) { d.DocEntryTicket = dr20.GetInt32(1); }
                    if (!dr20.IsDBNull(2)) { d.Linea = dr20.GetInt32(2); }
                    if (!dr20.IsDBNull(3)) { d.TablaSAP = dr20.GetString(3); }
                    if (!dr20.IsDBNull(4)) { d.Identificador = dr20.GetString(4); }
                    if (!dr20.IsDBNull(5)) { d.U_SYP_MDTD = dr20.GetString(5); }
                    if (!dr20.IsDBNull(6)) { d.U_SYP_MDSD = dr20.GetString(6); }
                    if (!dr20.IsDBNull(7)) { d.U_SYP_MDCD = dr20.GetString(7); }
                    if (!dr20.IsDBNull(8)) { d.DocDate = dr20.GetDateTime(8).ToString("yyyy-MM-dd"); }
                    if (!dr20.IsDBNull(9)) { d.U_BPP_FECINITRA = dr20.GetDateTime(9).ToString("yyyy-MM-dd"); }
                    if (!dr20.IsDBNull(10)) { d.Impreso = dr20.GetInt32(10); }
                    if (!dr20.IsDBNull(11)) { d.Estado = dr20.GetString(11); }
                    if (!dr20.IsDBNull(12)) { d.Id = dr20.GetInt32(12); }

                    o.DetRRU01.Add(d);
                }

                dr20.Close();

                SqlCommand cmd3 = new SqlCommand("select DocEntry,Linea,Guia,NroSap,OpEnvio,Cajas,OpRecepcion,Verificado,Estado,TempI1," +
                    "TempI2,TempF1,TempF2,OpEntrega,FechaEntrega,HoraEntrega from al.RRU1 where DocEntry=" + DocEntry, cn);
                SqlDataReader dr3 = cmd3.ExecuteReader();
                while (dr3.Read())
                {
                    RRU1_E r1 = new RRU1_E(); r1.ListaRRU11 = new List<RRU11_E>();
                    r1.DocEntry = DocEntry;
                    if (!dr3.IsDBNull(1)) r1.Linea = dr3.GetInt32(1);
                    if (!dr3.IsDBNull(2)) r1.Guia = dr3.GetString(2);
                    if (!dr3.IsDBNull(3)) r1.NroSap = dr3.GetInt32(3);
                    if (!dr3.IsDBNull(4)) r1.OpEnvio = dr3.GetString(4);
                    if (!dr3.IsDBNull(5)) r1.Cajas = dr3.GetInt32(5);
                    if (!dr3.IsDBNull(6)) r1.OpRecepcion = dr3.GetString(6);
                    if (!dr3.IsDBNull(7)) r1.Verificado = dr3.GetString(7);
                    if (!dr3.IsDBNull(8)) r1.Estado = dr3.GetString(8);
                    if (!dr3.IsDBNull(9)) r1.TempI1 = dr3.GetDecimal(9);
                    if (!dr3.IsDBNull(10)) r1.TempI2 = dr3.GetDecimal(10);
                    if (!dr3.IsDBNull(11)) r1.TempF1 = dr3.GetDecimal(11);
                    if (!dr3.IsDBNull(12)) r1.TempF2 = dr3.GetDecimal(12);
                    if (!dr3.IsDBNull(13)) r1.OpEntrega = dr3.GetString(13);
                    if (!dr3.IsDBNull(14)) r1.FechaEntrega = dr3.GetDateTime(14).ToString("yyyy-MM-dd");
                    if (!dr3.IsDBNull(15)) r1.HoraEntrega = dr3.GetTimeSpan(15).ToString();

                    r1.ListaRRU11 = obtProdRutaLinea(r1.DocEntry, r1.Linea);
                    o.DetRRU1.Add(r1);
                }
                dr3.Close();
                cn.Close();

                List<RRU11_E> obtProdRutaLinea(int fdocE, int fli)
                {
                    List<RRU11_E> flis = new List<RRU11_E>();
                    SqlConnection fcn = new SqlConnection(uti.cadSql);
                    try
                    {
                        fcn.Open();
                        SqlCommand fcmd = new SqlCommand("select BaseEntry,BaseLinea, Linea,ItemCode,ItemName,Lote,CantidadL,LaboCod,LaboDesc," +
                            "UnitMed,CantUnitMed,Cajas  from al.RRU11 where BaseEntry=" + fdocE + " AND Baselinea=" + fli, fcn);
                        SqlDataReader fdr = fcmd.ExecuteReader();
                        while (fdr.Read())
                        {
                            RRU11_E fr12 = new RRU11_E();
                            fr12.BaseEntry = fdocE;
                            fr12.BaseLinea = fli;
                            if (!fdr.IsDBNull(2)) { fr12.Linea = fdr.GetInt32(2); }
                            if (!fdr.IsDBNull(3)) { fr12.ItemCode = fdr.GetString(3); }
                            if (!fdr.IsDBNull(4)) { fr12.ItemName = fdr.GetString(4); }
                            if (!fdr.IsDBNull(5)) { fr12.Lote = fdr.GetString(5); }
                            if (!fdr.IsDBNull(6)) { fr12.CantidadL = fdr.GetDecimal(6); }
                            if (!fdr.IsDBNull(7)) { fr12.LaboCod = fdr.GetInt32(7); }
                            if (!fdr.IsDBNull(8)) { fr12.LaboDesc = fdr.GetString(8); }
                            if (!fdr.IsDBNull(9)) { fr12.UnitMed = fdr.GetString(9); }
                            if (!fdr.IsDBNull(10)) { fr12.CantUnitMed = fdr.GetInt32(10); }
                            if (!fdr.IsDBNull(11)) { fr12.Cajas = fdr.GetInt32(11); }
                            flis.Add(fr12);
                        }
                        fdr.Close();
                        fcn.Close();
                    }
                    catch { fcn.Close(); }
                    return flis;
                }
                RRU11_D rru11_D = new RRU11_D();
                o.DetRRU11 = rru11_D.ListarRRU11(DocEntry, 0);

            }
            catch (Exception e) { cn.Close(); throw new Exception(e.Message); }
            return o;
        }
        public ORRU_E obtenerOrdenDeRutaTicket(int DocEntryTicket)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            ORRU_E o = new ORRU_E();
            try
            {
                cn.Open();
                string query = "";
                query = "Select T1.DocEntry,T1.DocNum,T1.TipoRuta,T1.TransCod,T1.TransDesc,T1.VehiculoCod,T1.Placa," +
                    " T1.Marca,T1.Modelo,T1.CopilDesc,T1.Copil2Desc,T1.Copil3Desc,T1.Copil4Desc,T1.FechaCont,T1.FechaDoc," +
                    " T1.AlmOrigenCod,T1.AlmOrigenDesc,T1.AlmOrigenDesc2,T1.AlmDestinoCod,T1.AlmDestinoDesc,T1.AlmDestinoDesc2," +
                    " T1.Propietario,T1.TiempoPac,(select top 1 concat(FechaOperacion,' ',convert(varchar(8),HoraOperacion)) from al.CC_ORRU " +
                    " where Operacion='INICIAR' and DocEntry=T1.DocEntry order by FechaOperacion,HoraOperacion desc),(select top 1 concat(FechaOperacion,' ',convert(varchar(8),HoraOperacion)) from al.CC_ORRU " +
                    " where Operacion='TERMINAR' and DocEntry=T1.DocEntry order by FechaOperacion,HoraOperacion desc),T1.Estado,T1.HoraRegistro from al.ORRU T1 where T1.DocEntry = " +
                "(Select DocEntry from al.RRU0 where DocEntryTicket =" + DocEntryTicket + " AND Estado NOT IN ('LIBERADO', 'ANULADO') )";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { o.DocNum = dr.GetInt32(1); }
                if (!dr.IsDBNull(2)) { o.TipoRuta = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { o.TransCod = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { o.TransDesc = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { o.VehiculoCod = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { o.Placa = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { o.Marca = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { o.Modelo = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { o.CopilDesc = dr.GetString(9); }
                if (!dr.IsDBNull(10)) { o.Copil2Desc = dr.GetString(10); }
                if (!dr.IsDBNull(11)) { o.Copil3Desc = dr.GetString(11); }
                if (!dr.IsDBNull(12)) { o.Copil4Desc = dr.GetString(12); }
                if (!dr.IsDBNull(13)) { o.FechaCont = dr.GetDateTime(13); }
                if (!dr.IsDBNull(14)) { o.FechaDoc = dr.GetDateTime(14); }
                if (!dr.IsDBNull(15)) { o.AlmOrigenCod = dr.GetString(15); }
                if (!dr.IsDBNull(16)) { o.AlmOrigenDesc = dr.GetString(16); }
                if (!dr.IsDBNull(17)) { o.AlmOrigenDesc2 = dr.GetString(17); }
                if (!dr.IsDBNull(18)) { o.AlmDestinoCod = dr.GetString(18); }
                if (!dr.IsDBNull(19)) { o.AlmDestinoDesc = dr.GetString(19); }
                if (!dr.IsDBNull(20)) { o.AlmDestinoDesc2 = dr.GetString(20); }
                if (!dr.IsDBNull(21)) { o.Propietario = dr.GetString(21); }
                if (!dr.IsDBNull(22)) { o.TiempoPac = dr.GetDateTime(22); }
                if (!dr.IsDBNull(23)) { o.TiempoIniEn = dr.GetString(23); }
                if (!dr.IsDBNull(24)) { o.TiempoTerEn = dr.GetString(24); }
                if (!dr.IsDBNull(25)) { o.Estado = dr.GetString(25); }
                if (!dr.IsDBNull(26)) { o.HoraRegistro = dr.GetTimeSpan(26).ToString(); }

                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return o;
        }
        public int NuevaHojaDeReparto(ORRU_E o)
        {
            int status = -1;
            string TipoMant = "AR"; string TipoPost = "A";
            if (o.TipoRuta == "VG" || o.TipoRuta == "AC")
            {
                TipoMant = "ARG"; TipoPost = "B";
            }


            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    SqlTransaction tran = cn.BeginTransaction();
                    try
                    {
                        // Bloque de depuración temporal para registrar los valores de los parámetros
                        var debugInfo = new StringBuilder();
                        debugInfo.AppendLine("--- DEBUG: Parámetros para al.MANT_ORRU ---");
                        Action<string, object> addDebugInfo = (name, val) => {
                            string valStr = (val == null || val == DBNull.Value) ? "NULL" : val.ToString();
                            debugInfo.AppendLine($"{name} (len: {valStr.Length}): {valStr}");
                        };

                        addDebugInfo("@TipoRuta", o.TipoRuta);
                        addDebugInfo("@TransCod", o.TransCod);
                        addDebugInfo("@TransDesc", o.TransDesc);
                        addDebugInfo("@VehiculoCod", o.VehiculoCod);
                        addDebugInfo("@Placa", o.Placa);
                        addDebugInfo("@Marca", o.Marca);
                        addDebugInfo("@Modelo", o.Modelo);
                        addDebugInfo("@CopilDesc", o.CopilDesc);
                        addDebugInfo("@Copil2Desc", o.Copil2Desc);
                        addDebugInfo("@Copil3Desc", o.Copil3Desc);
                        addDebugInfo("@Copil4Desc", o.Copil4Desc);
                        addDebugInfo("@AlmOrigenCod", o.AlmOrigenCod);
                        addDebugInfo("@AlmOrigenDesc", o.AlmOrigenDesc);
                        addDebugInfo("@AlmOrigenDesc2", o.AlmOrigenDesc2);
                        addDebugInfo("@AlmDestinoCod", o.AlmDestinoCod);
                        addDebugInfo("@AlmDestinoDesc", o.AlmDestinoDesc);
                        addDebugInfo("@AlmDestinoDesc2", o.AlmDestinoDesc2);
                        addDebugInfo("@Propietario", o.Propietario);
                        addDebugInfo("@Observaciones", o.Observaciones);
                        if (o.TipoRuta == "AC")
                        {
                            addDebugInfo("@Agencia", o.Agencia);
                            addDebugInfo("@RucAgencia", o.RucAgencia);
                        }
                        addDebugInfo("@Operario", o.Propietario);
                        addDebugInfo("@Origen", o.Origen);
                        addDebugInfo("@ProvDesc", o.ProvDesc);

                        // Escribir en la ventana de salida de depuración
                        System.Diagnostics.Debug.WriteLine(debugInfo.ToString());


                        SqlCommand cmd = new SqlCommand("al.MANT_ORRU", cn, tran)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        cmd.Parameters.AddWithValue("@TipoMantenimiento", TipoMant);
                        cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@DocNum", o.DocNum).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@TipoRuta", o.TipoRuta);
                        cmd.Parameters.AddWithValue("@TransCod", o.TransCod);
                        cmd.Parameters.AddWithValue("@TransDesc", o.TransDesc);
                        cmd.Parameters.AddWithValue("@VehiculoCod", o.VehiculoCod);
                        cmd.Parameters.AddWithValue("@Placa", o.Placa);
                        cmd.Parameters.AddWithValue("@Marca", o.Marca);
                        cmd.Parameters.AddWithValue("@Modelo", o.Modelo);
                        cmd.Parameters.AddWithValue("@CopilDesc", o.CopilDesc);
                        cmd.Parameters.AddWithValue("@Copil2Desc", o.Copil2Desc);
                        cmd.Parameters.AddWithValue("@Copil3Desc", o.Copil3Desc);
                        cmd.Parameters.AddWithValue("@Copil4Desc", o.Copil4Desc);
                        cmd.Parameters.AddWithValue("@FechaCont", o.FechaCont);
                        cmd.Parameters.AddWithValue("@AlmOrigenCod", o.AlmOrigenCod);
                        cmd.Parameters.AddWithValue("@AlmOrigenDesc", o.AlmOrigenDesc);
                        cmd.Parameters.AddWithValue("@AlmOrigenDesc2", o.AlmOrigenDesc2);
                        cmd.Parameters.AddWithValue("@AlmDestinoCod", o.AlmDestinoCod);
                        cmd.Parameters.AddWithValue("@AlmDestinoDesc", o.AlmDestinoDesc);
                        cmd.Parameters.AddWithValue("@AlmDestinoDesc2", o.AlmDestinoDesc2);
                        cmd.Parameters.AddWithValue("@Propietario", o.Propietario);
                        cmd.Parameters.AddWithValue("@ProvDesc", o.ProvDesc);
                        DateTime newTiempoPac = Convert.ToDateTime(o.TiempoPac);
                        DateTime tiempoPacFormatted = new DateTime(newTiempoPac.Year, newTiempoPac.Month, newTiempoPac.Day, newTiempoPac.Hour, 0, 0);

                        // Agregar el parámetro con la fecha ajustada
                        cmd.Parameters.AddWithValue("@TiempoPac", tiempoPacFormatted);
                        cmd.Parameters.AddWithValue("@Observaciones", o.Observaciones);
                        if (o.TipoRuta == "AC")
                        {
                            cmd.Parameters.AddWithValue("@Agencia", o.Agencia);
                            cmd.Parameters.AddWithValue("@RucAgencia", o.RucAgencia);
                        }

                        cmd.Parameters.AddWithValue("@Operario", o.Propietario);
                        cmd.Parameters.AddWithValue("@Origen", ((object)o.Origen) ?? DBNull.Value);

                        if (o.DetRRU0 != null && o.DetRRU0.Count > 0)
                        {
                            SqlParameter tbDet = new SqlParameter("@Det", SqlDbType.Structured);
                            tbDet.Value = RRU0_E.tbDetalle(o.DetRRU0);
                            tbDet.TypeName = "al.TPRRU0";
                            cmd.Parameters.AddWithValue("@Det", tbDet.Value);
                        }
                        if (o.DetRRU01 != null && o.DetRRU01.Count > 0)
                        {
                            SqlParameter tbDet01 = new SqlParameter("@DetDoc", SqlDbType.Structured);
                            tbDet01.Value = RRU01_E.tbDetalle(o.DetRRU01);
                            tbDet01.TypeName = "al.TPRRU01";
                            cmd.Parameters.AddWithValue("@DetDoc", tbDet01.Value);
                        }
                        if (o.DetRRU1 != null && o.DetRRU1.Count > 0)
                        {
                            SqlParameter tbDet1 = new SqlParameter("@Det1", SqlDbType.Structured);
                            tbDet1.Value = RRU1_E.tbDetalle(o.DetRRU1);
                            tbDet1.TypeName = "al.TPRRU1";
                            cmd.Parameters.AddWithValue("@Det1", tbDet1.Value);
                            // detalles de productos
                            SqlParameter tbDetProd = new SqlParameter("@DetProd", SqlDbType.Structured);
                            List<RRU11_E> ListaRRU11 = new List<RRU11_E>();
                            foreach (RRU1_E rru1 in o.DetRRU1)
                            {
                                if (rru1.ListaRRU11 != null && rru1.ListaRRU11.Count > 0)
                                {
                                    foreach (RRU11_E rru11 in rru1.ListaRRU11)
                                    {
                                        ListaRRU11.Add(rru11);
                                    }
                                }
                            }
                            tbDetProd.Value = RRU11_E.tbRRU12(ListaRRU11);
                            tbDetProd.TypeName = "al.TPRRU11";
                            cmd.Parameters.AddWithValue("@DetProd", tbDetProd.Value);
                        }
                        cmd.ExecuteNonQuery();
                        status = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());

                        if (o.DetRRU0 != null)
                        {
                            // cambio de estado ticket
                            if (o.DetRRU0 != null)
                            {
                                // cambio de estado ticket
                                foreach (RRU0_E dr in RRU0_E.listaFinalDetalles(o.DetRRU0))
                                {
                                    ORTV_E tcv = ticketD.ObtenerDatosCompletosTicket(dr.DocEntryTicket,cn,tran);
                                    CC_ORTV_D ccORTV_D = new CC_ORTV_D();

                                    // Si es devolución (TipoRuta == "DE") saltamos las validaciones de PESADO / EMPACADO
                                    if (o.TipoRuta != "DE")
                                    {
                                        List<CC_ORTV_E> estadoPesado = ccORTV_D.ListarCC_ORTV(tcv.DocEntry, "PESAR",false, cn, tran);
                                        List<CC_ORTV_E> estadoEmpacado = ccORTV_D.ListarCC_ORTV(tcv.DocEntry, "FIN EMPACAR", false, cn, tran);

                                        if (o.TipoRuta == "AC")
                                        {
                                            // Validar que exista registro PESAR con FechaOperacion
                                            bool pesadoValido = estadoPesado != null && estadoPesado.Count > 0 && estadoPesado[0].FechaOperacion != null;
                                            if (tcv.Estado != "PESADO" && !pesadoValido)
                                                throw new Exception("El ticket no se encuentra pesado: " + tcv.DocNum);
                                        }
                                        else
                                        {
                                            // Para otros tipos (no AC y no DE) validar EMPACADO
                                            bool empacadoValido = estadoEmpacado != null && estadoEmpacado.Count > 0 && estadoEmpacado[0].FechaOperacion != null;
                                            if (tcv.Estado != "EMPACADO" && !empacadoValido)
                                                throw new Exception("El ticket no esta empacado " + tcv.DocNum);
                                        }
                                    }

                                    ticketD.Preenviar(tcv.DocEntry, o.Propietario, tran, cn);
                                }
                            }
                        }

                        SqlCommand cmd2 = new SqlCommand("dbo.POST_TRANSACCIONES", cn, tran);
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@Tipo", TipoPost);
                        cmd2.Parameters.AddWithValue("@Tabla", "ORRU");
                        cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@DocNum"].Value);
                        cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@DocEntry"].Value);
                        cmd2.ExecuteNonQuery();

                        tran.Commit();
                        cn.Close();
                    }
                    catch (Exception e1)
                    {
                        tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: " + e1.Message);
                    }


                }
                catch (Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }
            }

            return status;
        }
        public int EditarHojaDeReparto(ORRU_E o)
        {
            int status = -1;

            // ✅ VERIFICAR SI ES DEVOLUCIÓN ANTES DE EDITAR
            ORRU_E rutaActual = obtenerOrdenDeRuta(o.DocEntry);
            string tipoMant = (rutaActual.TipoRuta == "DE") ? "UPDE" : "UP";  

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_ORRU", cn, tran)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", tipoMant);//UPDATE
                    cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@DocNum", o.DocNum).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@TransCod", o.TransCod);
                    cmd.Parameters.AddWithValue("@TransDesc", o.TransDesc);
                    cmd.Parameters.AddWithValue("@ProvDesc", o.ProvDesc);
                    cmd.Parameters.AddWithValue("@VehiculoCod", o.VehiculoCod);
                    cmd.Parameters.AddWithValue("@Placa", o.Placa);
                    cmd.Parameters.AddWithValue("@Marca", o.Marca);
                    cmd.Parameters.AddWithValue("@Modelo", o.Modelo);
                    cmd.Parameters.AddWithValue("@CopilDesc", o.CopilDesc);
                    cmd.Parameters.AddWithValue("@Copil2Desc", o.Copil2Desc);
                    cmd.Parameters.AddWithValue("@Copil3Desc", o.Copil3Desc);
                    cmd.Parameters.AddWithValue("@Copil4Desc", o.Copil4Desc);
                        cmd.Parameters.AddWithValue("@FechaCont", o.FechaCont ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AlmOrigenCod", o.AlmOrigenCod);
                    cmd.Parameters.AddWithValue("@AlmOrigenDesc", o.AlmOrigenDesc);
                    cmd.Parameters.AddWithValue("@AlmOrigenDesc2", o.AlmOrigenDesc2);
                    cmd.Parameters.AddWithValue("@AlmDestinoCod", o.AlmDestinoCod);
                    cmd.Parameters.AddWithValue("@AlmDestinoDesc", o.AlmDestinoDesc);
                    cmd.Parameters.AddWithValue("@AlmDestinoDesc2", o.AlmDestinoDesc2);

                    DateTime newTiempoPac = Convert.ToDateTime(o.TiempoPac);
                    DateTime tiempoPacFormatted = new DateTime(newTiempoPac.Year, newTiempoPac.Month, newTiempoPac.Day, newTiempoPac.Hour, 0, 0);

                    // Agregar el parámetro con la fecha ajustada
                    cmd.Parameters.AddWithValue("@TiempoPac", tiempoPacFormatted);
                    cmd.Parameters.AddWithValue("@Observaciones", o.Observaciones);
                    cmd.Parameters.AddWithValue("@Agencia", o.Agencia);
                    cmd.Parameters.AddWithValue("@RucAgencia", o.RucAgencia);
                    cmd.Parameters.AddWithValue("@Operario", o.Propietario);

                    // ✅ AGREGADO: Procesar detalles DetRRU0 (órdenes de devolución)
                    if (o.DetRRU0 != null && o.DetRRU0.Count > 0)
                    {
                        SqlParameter tbDet = new SqlParameter("@Det", SqlDbType.Structured);
                        tbDet.Value = RRU0_E.tbDetalle(o.DetRRU0);
                        tbDet.TypeName = "al.TPRRU0";
                        cmd.Parameters.AddWithValue("@Det", tbDet.Value);
                    }

                    // ✅ AGREGADO: Procesar detalles DetRRU01 si existen
                    if (o.DetRRU01 != null && o.DetRRU01.Count > 0)
                    {
                        SqlParameter tbDet01 = new SqlParameter("@DetDoc", SqlDbType.Structured);
                        tbDet01.Value = RRU01_E.tbDetalle(o.DetRRU01);
                        tbDet01.TypeName = "al.TPRRU01";
                        cmd.Parameters.AddWithValue("@DetDoc", tbDet01.Value);
                    }

                    // ✅ AGREGADO: Procesar detalles DetRRU1 (transferencias entre almacenes)
                    if (o.DetRRU1 != null && o.DetRRU1.Count > 0)
                    {
                        SqlParameter tbDet1 = new SqlParameter("@Det1", SqlDbType.Structured);
                        tbDet1.Value = RRU1_E.tbDetalle(o.DetRRU1);
                        tbDet1.TypeName = "al.TPRRU1";
                        cmd.Parameters.AddWithValue("@Det1", tbDet1.Value);

                        // Detalles de productos (RRU11)
                        SqlParameter tbDetProd = new SqlParameter("@DetProd", SqlDbType.Structured);
                        List<RRU11_E> ListaRRU11 = new List<RRU11_E>();
                        foreach (RRU1_E rru1 in o.DetRRU1)
                        {
                            if (rru1.ListaRRU11 != null && rru1.ListaRRU11.Count > 0)
                            {
                                foreach (RRU11_E rru11 in rru1.ListaRRU11)
                                {
                                    ListaRRU11.Add(rru11);
                                }
                            }
                        }
                        tbDetProd.Value = RRU11_E.tbRRU12(ListaRRU11);
                        tbDetProd.TypeName = "al.TPRRU11";
                        cmd.Parameters.AddWithValue("@DetProd", tbDetProd.Value);
                    }
                    
                    cmd.ExecuteNonQuery();
                    status = o.DocNum;

                    tran.Commit();
                    cn.Close();
                }
                catch (Exception e1)
                {
                    tran.Rollback(); cn.Close(); throw new Exception("Error en edición: " + e1.Message);
                }
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en conexión: " + e2.Message); }
            return status;
        }
        public int AnularOrdenDeRuta(int DocEntry, string OpRegistro)
        {
            int DcNum = 0;
            ORRU_E orru = obtenerOrdenDeRuta(DocEntry);
  
            // ✅ PERMITIR ANULAR TANTO TRANSFERENCIAS (TA) COMO DEVOLUCIONES (DE)
            if (orru.TipoRuta == "TA" || orru.TipoRuta == "DE")
      {
              SqlConnection cn = new SqlConnection(uti.cadSql);
        try
       {
  cn.Open();
       SqlTransaction tran = cn.BeginTransaction();
       try
            {
   SqlCommand cmd = new SqlCommand("al.MANT_ORRU", cn);
                cmd.Transaction = tran;
         cmd.CommandType = CommandType.StoredProcedure;
   cmd.Parameters.AddWithValue("@TipoMantenimiento", "UA");
         cmd.Parameters.AddWithValue("@DocEntry", DocEntry).Direction = ParameterDirection.InputOutput;
        cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
              cmd.Parameters.AddWithValue("@Operario", OpRegistro);
             cmd.ExecuteNonQuery();
              DcNum = int.Parse(cmd.Parameters["@DocNum"].Value.ToString());
        tran.Commit();
           cn.Close();
   }
      catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error : " + e.Message); }
  }
                catch (Exception e2) { cn.Close(); throw new Exception("Error en conexion : " + e2.Message); }
            }
   return DcNum;
        }
        public void IniciarReparto(ORRU_E o)
        {

            RRU0_D rru0D = new RRU0_D(); ORTV_D ortvD = new ORTV_D(); RRU1_D rru1D = new RRU1_D();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_ORRU", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UIR");
                    cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Operario", o.Operario);
                    cmd.ExecuteNonQuery();
                    if (o.DetRRU0 != null && o.DetRRU0.Count > 0)
                    {
                        foreach (RRU0_E a in o.DetRRU0.Where(x => x.Estado != "LIBERADO"))
                        {
                            rru0D.enviarRRU0(a, tran, cn);
                            ortvD.Enviar(new ORTV_E { DocEntry = a.DocEntryTicket, Operario = o.Operario }, tran, cn);
                        }
                    }
                    if (o.DetRRU1 != null && o.DetRRU1.Count > 0)
                    {
                        foreach (RRU1_E a in o.DetRRU1.Where(x => x.Estado != "LIBERADO"))
                        {
                            rru1D.enviarRRU1(a, tran, cn);
                        }
                    }

                    tran.Commit();
                    cn.Close();
                }
                catch (Exception e1) { tran.Rollback(); cn.Close(); throw new Exception("Error en inicio rep: " + e1.Message); }
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en en inicio rep y conexion: " + e2.Message); }
        }
        public void TerminarReparto(ORRU_E o)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_ORRU", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UTR");
                    cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Operario", o.Operario);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    cn.Close();
                }
                catch (Exception e1) { tran.Rollback(); cn.Close(); throw new Exception("Error : " + e1.Message); }
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en conexion: " + e2.Message); }
        }

        public string infoListaProductosOWTQ(string guia, int linea, string Origen)
        {
            if (guia == null || guia == "") { return ""; }
            string info = "<table class='table table-bordered table-hover table-secondary'>";

            info += "<tr style='background-color:darkgreen'>" +
                        "<th></th><th>#</th><th>Producto</th><th>Lote</th><th>Cant.</th><th>Lab.</th><th>Unid.Med.</th><th>CajasMaster</th>" +
                    "</tr>";

            int i = 0;
            foreach (RRU11_E r in listaProductosOWTQ(guia, Origen))
            {
                info += "<tr>" +
                            "<td><div class='d-flex justify-content-center '><input name='DetRRU1[" + linea + "].ListaRRU11[" + i + "].BaseLinea' type='text' value='" + (linea + 1) + "' class='form-control font-12 text-center' style='width:50px;' readonly></div></td>" +
                            "<td><div class='d-flex justify-content-center '><input name='DetRRU1[" + linea + "].ListaRRU11[" + i + "].Linea' id='ldrr" + linea + "rru" + i + "' type='text' value='" + (i + 1) + "' class='form-control font-12 text-center' style='width:50px;' readonly></div></td>" +
                            "<td><input name='DetRRU1[" + linea + "].ListaRRU11[" + i + "].ItemName' type='text' value='" + r.ItemName + "' class='form-control font-12 text-center' readonly>" +
                                "<input name='DetRRU1[" + linea + "].ListaRRU11[" + i + "].ItemCode' type='hidden' value='" + r.ItemCode + "' class='form-control' style='width:200px;' readonly>" +
                            "</td>" +
                            "<td><div class='d-flex justify-content-center '><input name='DetRRU1[" + linea + "].ListaRRU11[" + i + "].Lote' type='text' value='" + r.Lote + "' class='form-control font-12 text-center' style='width:100px;' readonly></div></td>" +
                            "<td><div class='d-flex justify-content-center '><input name='DetRRU1[" + linea + "].ListaRRU11[" + i + "].CantidadL' type='text' value='" + Math.Round(r.CantidadL, 0) + "' class='form-control font-12 text-center' style='width:100px;' readonly></div></td>" +
                            "<td><div class='d-flex justify-content-center '><input name='DetRRU1[" + linea + "].ListaRRU11[" + i + "].LaboDesc' type='text' value='" + r.LaboDesc + "' class='form-control font-12 text-center' style='width:100px;' readonly></div>" +
                                    "<input name='DetRRU1[" + linea + "].ListaRRU11[" + i + "].LaboCod' type='hidden' value='" + r.LaboCod + "' class='form-control font-12' style='width:50px;' readonly>" +
                            "</td>" +
                            "<td><div class='d-flex justify-content-center '><input name='DetRRU1[" + linea + "].ListaRRU11[" + i + "].UnitMed' type='text' value='" + r.UnitMed + "' class='form-control font-12 text-center' style='width:100px;' readonly></div>" +
                                "<input name='DetRRU1[" + linea + "].ListaRRU11[" + i + "].CantUnitMed' type='hidden' value='" + r.CantUnitMed + "'  readonly>" +
                            "</td>" +
                            "<td><div class='d-flex justify-content-center '><input name='DetRRU1[" + linea + "].ListaRRU11[" + i + "].Cajas' id='ldrr" + linea + "rruC" + i + "' type='number' class='form-control font-12' style='width:80px;' onchange='actualizarCajas();'></div></td>" +
                        "</tr>";

                i++;
            }
            info += "</table>";
            return info;
        }

        /********Reportes************/
        public List<ORRU_E.RptRutas> ReporteHojasRuta(ORRU_E o)
        {
            var lista = new List<ORRU_E.RptRutas>();

            try
            {
                using (var cn = new SqlConnection(uti.cadSql))
                using (var cmd = new SqlCommand("al.RptHojasRuta_2", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;

                    cmd.Parameters.AddWithValue("@AlmIni", o.AlmIni);
                    cmd.Parameters.AddWithValue("@AlmFin", o.AlmFin);
                    cmd.Parameters.AddWithValue("@Estado", o.Estado);
                    cmd.Parameters.AddWithValue("@FechaRegistroDesde", o.FechaRegistroDesde);
                    cmd.Parameters.AddWithValue("@FechaRegistroHasta", o.FechaRegistroHasta);
                    cmd.Parameters.AddWithValue("@CardCode", o.CardCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TipoRuta", o.TipoRuta ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@TransDesc", o.TransDesc ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Placa", o.Placa ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@MontoTotalIni", o.MontoTotalIni);
                    cmd.Parameters.AddWithValue("@MontoTotalFin", o.MontoTotalFin);

                    cn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var or = new ORRU_E.RptRutas();
                            if (!dr.IsDBNull(0)) or.TransDesc = dr.GetString(0);
                            if (!dr.IsDBNull(1)) or.TipoRuta = dr.GetString(1);
                            if (!dr.IsDBNull(2)) or.CopilDesc = dr.GetString(2);
                            if (!dr.IsDBNull(3)) or.Copil2Desc = dr.GetString(3);
                            if (!dr.IsDBNull(4)) or.Copil3Desc = dr.GetString(4);
                            if (!dr.IsDBNull(5)) or.Placa = dr.GetString(5);
                            if (!dr.IsDBNull(6)) or.AlmOrigenDesc = dr.GetString(6);
                            if (!dr.IsDBNull(7)) or.AlmDestinoDesc = dr.GetString(7);
                            if (!dr.IsDBNull(8)) or.Propietario = dr.GetString(8);
                            if (!dr.IsDBNull(9)) or.DocNum = dr.GetInt32(9);
                            if (!dr.IsDBNull(10)) or.FechaDoc = dr.GetDateTime(10).ToString("yyyy-MM-dd");
                            if (!dr.IsDBNull(11)) or.Hora = dr.GetString(11);
                            if (!dr.IsDBNull(12)) or.Estado = dr.GetString(12);
                            if (!dr.IsDBNull(13)) or.Linea = dr.GetInt32(13);
                            if (!dr.IsDBNull(14)) or.CardCode = dr.GetString(14);
                            if (!dr.IsDBNull(15)) or.CardName = dr.GetString(15);
                            if (!dr.IsDBNull(16)) or.DocNumTicket = dr.GetInt32(16);
                            if (!dr.IsDBNull(17)) or.Guias = dr.GetString(17);
                            if (!dr.IsDBNull(18)) or.Cajas = dr.GetInt32(18);
                            if (!dr.IsDBNull(19)) or.DirDestino = dr.GetString(19);
                            if (!dr.IsDBNull(20)) or.Distrito1 = dr.GetString(20);
                            if (!dr.IsDBNull(21)) or.Provincia1 = dr.GetString(21);
                            if (!dr.IsDBNull(22)) or.Departamento1 = dr.GetString(22);
                            if (!dr.IsDBNull(23)) or.MontoTotal = dr.GetDecimal(23);
                            if (!dr.IsDBNull(24)) or.MontoFinal = dr.GetDecimal(24);
                            if (!dr.IsDBNull(25)) or.Agencia = dr.GetString(25);
                            if (!dr.IsDBNull(26)) or.Flete = dr.GetDecimal(26);
                            if (!dr.IsDBNull(27)) or.GastoEnvio = dr.GetDecimal(27);
                            if (!dr.IsDBNull(28)) or.TipoVenta = dr.GetString(28);
                            if (!dr.IsDBNull(29)) or.ZonaVenta = dr.GetString(29);
                            if (!dr.IsDBNull(30)) or.FechaEntregaVenta = dr.GetString(30);
                            if (!dr.IsDBNull(31)) or.HoraEntregaVenta = dr.GetString(31);
                            if (!dr.IsDBNull(32)) or.FechaInicioReparto = dr.GetString(32);
                            if (!dr.IsDBNull(33)) or.HoraInicioReparto = dr.GetString(33);
                            if (!dr.IsDBNull(34)) or.FechaFinReparto = dr.GetString(34);
                            if (!dr.IsDBNull(35)) or.HoraFinReparto = dr.GetString(35);
                            if (!dr.IsDBNull(36)) or.PesoTotalVenta = dr.GetDecimal(36);
                            if (!dr.IsDBNull(37)) or.FormaPagoVenta = dr.GetString(37);
                            if (!dr.IsDBNull(38)) or.TipoPagoRepartoContraEntrega = dr.GetString(38);
                            if (!dr.IsDBNull(39)) or.ComentarioLiberado = dr.GetString(39);
                            if (!dr.IsDBNull(40)) or.MontoRecibidoDepositoContraEntrega = dr.GetDecimal(40);
                            if (!dr.IsDBNull(41)) or.MontoRecibidoEfectivoContraEntrega = dr.GetDecimal(41);
                            if (!dr.IsDBNull(42)) or.FechaPagoVenta = dr.GetString(42);
                            if (!dr.IsDBNull(43)) or.HoraPagoVenta = dr.GetString(43);
                            if (!dr.IsDBNull(44)) or.FechaEntregaReparto = dr.GetString(44);
                            if (!dr.IsDBNull(45)) or.HoraEntregaReparto = dr.GetString(45);
                            if (!dr.IsDBNull(46)) or.FechaPreenvio = dr.GetString(46);
                            if (!dr.IsDBNull(47)) or.FechaEnviado = dr.GetString(47);
                            if (!dr.IsDBNull(48)) or.NombreRecoge = dr.GetString(48);

                            lista.Add(or);
                        }
                    }

                    cn.Close();
                }
            }
            catch (Exception e)
            {
                LogHelper.RegistrarError(e, "Error inesperado en ORRU_D - ReporteHojasRuta()");
            }

            return lista;
        }

        public List<ORRU_E.RptRutasT> ReporteTransferencias(ORRU_E o)
        {
            var lista = new List<ORRU_E.RptRutasT>();

            try
            {
                using (var cn = new SqlConnection(uti.cadSql))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 300;

                    cmd.CommandText = @"
                       SELECT 
                        T0.TipoRuta,
                        T0.DocNum,
                        T0.FechaRegistro,
                        T0.AlmOrigenDesc,
                        T0.AlmDestinoDesc,
                        T0.TransDesc,
                        T0.Placa,
                        T1.Guia,
                        T1.NroSap,
                        T0.Estado
                    FROM al.ORRU AS T0
                    LEFT JOIN al.RRU1 AS T1 
                        ON T1.DocEntry = T0.DocEntry
                    WHERE T0.TipoRuta = 'TA'
                        AND (@AlmIni IS NULL OR T0.AlmOrigenCod = @AlmIni)
                        AND (@AlmFin IS NULL OR T0.AlmDestinoCod = @AlmFin)
                        AND (@Placa IS NULL OR T0.Placa = @Placa)
                        AND (
                            @FechaRegistroDesde IS NULL 
                            OR (T0.FechaRegistro BETWEEN @FechaRegistroDesde AND @FechaRegistroHasta)
                            )
                    ";

                    cmd.Parameters.AddWithValue("@AlmIni", o.AlmIni ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@AlmFin", o.AlmFin ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Placa", o.Placa ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaRegistroDesde", o.FechaRegistroDesde ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@FechaRegistroHasta", o.FechaRegistroHasta ?? (object)DBNull.Value);

                    cn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var or = new ORRU_E.RptRutasT();
                            if (!dr.IsDBNull(0)) or.TipoRuta = dr.GetString(0);
                            if (!dr.IsDBNull(1)) or.DocNum = dr.GetInt32(1);
                            if (!dr.IsDBNull(2)) or.FechaDoc = dr.GetDateTime(2).ToString("yyyy-MM-dd");
                            if (!dr.IsDBNull(3)) or.AlmOrigenDesc = dr.GetString(3);
                            if (!dr.IsDBNull(4)) or.AlmDestinoDesc = dr.GetString(4);
                            if (!dr.IsDBNull(5)) or.TransDesc = dr.GetString(5);
                            if (!dr.IsDBNull(6)) or.Placa = dr.GetString(6);
                            if (!dr.IsDBNull(7)) or.Guia = dr.GetString(7);
                            if (!dr.IsDBNull(8)) or.NroSap = dr.GetInt32(8);
                            if (!dr.IsDBNull(9)) or.Estado = dr.GetString(9);

                            lista.Add(or);
                        }
                    }

                    cn.Close();
                }
            }
            catch (Exception e)
            {
                LogHelper.RegistrarError(e, "Error inesperado en ORRU_D - ReporteTransferenciasSimple()");
            }

            return lista;
        }

        public List<Rpt_TempHumed_E> RptTempHumed(string Placa, string FechaTerEn, string Serie)
        {
            List<Rpt_TempHumed_E> lista = new List<Rpt_TempHumed_E>();

            List<ORRU_E> orrul = new List<ORRU_E>();
            string query = string.Empty;
            string queryAux = "select T1.DocEntry, T1.TipoRuta from al.ORRU T1 where  (select top 1 FechaOperacion from al.CC_ORRU " +
                    " where Operacion='TERMINAR' and DocEntry=T1.DocEntry order by FechaOperacion,HoraOperacion desc)='" + FechaTerEn + "' and T1.Placa='" + Placa + "' and T1.TipoRuta not in ('VG')";

            SqlConnection cn = new SqlConnection(uti.cadSql);

            cn.Open();
            SqlCommand cmdAux = new SqlCommand(queryAux, cn);
            SqlDataReader drAux = cmdAux.ExecuteReader();
            while (drAux.Read())
            {
                ORRU_E o = new ORRU_E();
                if (!drAux.IsDBNull(0)) { o.DocEntry = drAux.GetInt32(0); }
                if (!drAux.IsDBNull(1)) { o.TipoRuta = drAux.GetString(1); }
                orrul.Add(o);
            }
            drAux.Close();

            foreach (ORRU_E obj in orrul)
            {
                if (obj.TipoRuta != "TA")
                {
                    if (Serie == "SerieT1")
                    {
                        query = $@"SELECT '{Placa}', 
                  (SELECT SerieT1 FROM al.OVEH WHERE U_SYP_VEPL = '{Placa}'), 
                  (SELECT TOP 1 FechaOperacion 
                   FROM al.CC_ORRU 
                   WHERE Operacion = 'TERMINAR' 
                         AND DocEntry = T1.DocEntry 
                   ORDER BY FechaOperacion DESC, HoraOperacion DESC), 
                  T0.DocNum, 
                  CAST(T1.DocNumTicket AS VARCHAR), 
                  (SELECT TOP 1 CONVERT(VARCHAR, HoraOperacion, 108) 
                   FROM al.CC_ORRU 
                   WHERE Operacion = 'INICIAR' 
                         AND DocEntry = T1.DocEntry 
                   ORDER BY FechaOperacion DESC, HoraOperacion DESC), 
                  T1.TempI1, 
                  T1.HumedI1, 
                  CONVERT(VARCHAR(8), T1.HoraEntrega), 
                  T1.TempF1, 
                  T1.HumedF1, 
                  T0.TransDesc, 
                  T0.DocEntry, 
                  T1.DocNumTicket 
           FROM al.ORRU T0 
           INNER JOIN al.RRU0 T1 ON T0.DocEntry = T1.DocEntry 
           WHERE (SELECT TOP 1 FechaOperacion 
                  FROM al.CC_ORRU 
                  WHERE Operacion = 'TERMINAR' 
                        AND DocEntry = T1.DocEntry 
                  ORDER BY FechaOperacion DESC, HoraOperacion DESC) IN ('{FechaTerEn}') 
                 AND T1.Estado = 'ENTREGADO' 
                 AND T0.DocEntry = {obj.DocEntry} ";


                    }

                    if (Serie == "SerieT2")
                    {
                        query = $@"SELECT '{Placa}', 
                  (SELECT SerieT2 FROM al.OVEH WHERE U_SYP_VEPL = '{Placa}'), 
                  (SELECT TOP 1 FechaOperacion 
                   FROM al.CC_ORRU 
                   WHERE Operacion = 'TERMINAR' 
                         AND DocEntry = T1.DocEntry 
                   ORDER BY FechaOperacion DESC, HoraOperacion DESC), 
                  T0.DocNum, 
                  CAST(T1.DocNumTicket AS VARCHAR), 
                  (SELECT TOP 1 CONVERT(VARCHAR, HoraOperacion, 108) 
                   FROM al.CC_ORRU 
                   WHERE Operacion = 'INICIAR' 
                         AND DocEntry = T1.DocEntry 
                   ORDER BY FechaOperacion DESC, HoraOperacion DESC), 
                  T1.TempI2, 
                  T1.HumedI2, 
                  CONVERT(VARCHAR(8), T1.HoraEntrega), 
                  T1.TempF2, 
                  T1.HumedF2, 
                  T0.TransDesc, 
                  T0.DocEntry, 
                  T1.DocNumTicket 
           FROM al.ORRU T0 
           INNER JOIN al.RRU0 T1 ON T0.DocEntry = T1.DocEntry 
           WHERE (SELECT TOP 1 FechaOperacion 
                  FROM al.CC_ORRU 
                   WHERE Operacion = 'TERMINAR' 
                        AND DocEntry = T1.DocEntry 
                  ORDER BY FechaOperacion DESC, HoraOperacion DESC) IN ('{FechaTerEn}') 
                 AND T1.Estado = 'ENTREGADO' 
                 AND T0.DocEntry = {obj.DocEntry}";


                    }
                }
                else
                {
                    if (Serie == "SerieT1")
                    {
                        query = $@"SELECT '{Placa}', 
                  (SELECT SerieT1 FROM al.OVEH WHERE U_SYP_VEPL = '{Placa}'), 
                  (SELECT TOP 1 FechaOperacion 
                   FROM al.CC_ORRU 
                   WHERE Operacion = 'TERMINAR' 
                         AND DocEntry = T1.DocEntry 
                   ORDER BY FechaOperacion, HoraOperacion DESC), 
                  T0.DocNum, 
                  T1.Guia, 
                  (SELECT TOP 1 CONVERT(VARCHAR, HoraOperacion, 108) 
                   FROM al.CC_ORRU 
                   WHERE Operacion = 'INICIAR' 
                         AND DocEntry = T1.DocEntry 
                   ORDER BY FechaOperacion, HoraOperacion DESC), 
                  T1.TempI1, 
                  T1.HumedI1, 
                  CONVERT(VARCHAR(8), T1.HoraEntrega), 
                  T1.TempF1, 
                  T1.HumedF1, 
                  T0.TransDesc, 
                  T0.DocEntry, 
                  0 
           FROM al.ORRU T0 
           INNER JOIN al.RRU1 T1 ON T0.DocEntry = T1.DocEntry 
           WHERE (SELECT TOP 1 FechaOperacion 
                  FROM al.CC_ORRU  
                  WHERE Operacion = 'TERMINAR' 
                        AND DocEntry = T1.DocEntry 
                  ORDER BY FechaOperacion, HoraOperacion DESC) IN ('{FechaTerEn}') 
                 AND T1.Estado = 'ENTREGADO' 
                 AND T0.DocEntry = {obj.DocEntry}";


                    }

                    if (Serie == "SerieT2")
                    {
                        query = $@"SELECT '{Placa}', 
                  (SELECT SerieT2 FROM al.OVEH WHERE U_SYP_VEPL = '{Placa}'), 
                  (SELECT TOP 1 FechaOperacion 
                   FROM al.CC_ORRU 
                   WHERE Operacion = 'TERMINAR' 
                         AND DocEntry = T1.DocEntry 
                   ORDER BY FechaOperacion, HoraOperacion DESC), 
                  T0.DocNum, 
                  T1.Guia, 
                  (SELECT TOP 1 CONVERT(VARCHAR, HoraOperacion, 108) 
                   FROM al.CC_ORRU 
                   WHERE Operacion = 'INICIAR' 
                         AND DocEntry = T1.DocEntry 
                   ORDER BY FechaOperacion, HoraOperacion DESC), 
                  T1.TempI2, 
                  T1.HumedI2, 
                  CONVERT(VARCHAR(8), T1.HoraEntrega), 
                  T1.TempF2, 
                  T1.HumedF2, 
                  T0.TransDesc, 
                  T0.DocEntry, 
                  0 
           FROM al.ORRU T0 
           INNER JOIN al.RRU1 T1 ON T0.DocEntry = T1.DocEntry 
           WHERE (SELECT TOP 1 FechaOperacion 
                  FROM al.CC_ORRU 
                  WHERE Operacion = 'TERMINAR' 
                        AND DocEntry = T1.DocEntry 
                  ORDER BY FechaOperacion, HoraOperacion DESC) IN ('{FechaTerEn}') 
                 AND T1.Estado = 'ENTREGADO' 
                 AND T0.DocEntry = {obj.DocEntry}";


                    }
                }
                SqlCommand cmd = new SqlCommand(query, cn);
                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Rpt_TempHumed_E p = new Rpt_TempHumed_E();
                        if (!dr.IsDBNull(0)) { p.Placa = dr.GetString(0); }
                        if (!dr.IsDBNull(1)) { p.Serie = dr.GetString(1); }
                        if (!dr.IsDBNull(2)) { p.Fecha = dr.GetDateTime(2).ToString("dd/MM/yyyy"); }
                        if (!dr.IsDBNull(3)) { p.Documento = dr.GetInt32(3); }
                        if (!dr.IsDBNull(4)) { p.Codigo = dr.GetString(4); }
                        if (!dr.IsDBNull(5)) { p.HoraSalida = dr.GetString(5); }
                        if (!dr.IsDBNull(6)) { p.TempI = dr.GetDecimal(6); }
                        if (!dr.IsDBNull(7)) { p.HumedI = Convert.ToInt32(dr.GetDecimal(7)); }
                        if (!dr.IsDBNull(8)) { p.HoraLlegada = dr.GetString(8); }
                        if (!dr.IsDBNull(9)) { p.TempF = dr.GetDecimal(9); }
                        if (!dr.IsDBNull(10)) { p.HumedF = Convert.ToInt32(dr.GetDecimal(10)); }
                        if (!dr.IsDBNull(11)) { p.Encargado = dr.GetString(11); }
                        if (!dr.IsDBNull(12)) { p.DocEntry = dr.GetInt32(12); }
                        if (!dr.IsDBNull(13)) { p.DocNum = dr.GetInt32(13); }
                        lista.Add(p);
                    }

                    dr.Close();

                }
                catch { }
            }

            return lista.OrderBy(x => x.HoraLlegada).ToList();
        }
        public List<RptPesaje_E> ListarRptPesaje(FiltroRptPesaje datosFiltro)
        {

            List<RptPesaje_E> lista = new List<RptPesaje_E>();
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                SqlCommand cmd = new SqlCommand("al.RptPesaje", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@FecConIni", datosFiltro.FecConIni);
                cmd.Parameters.AddWithValue("@FecConFin", datosFiltro.FecConFin);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            RptPesaje_E rpt = new RptPesaje_E();

                            if (!dr.IsDBNull(0)) { rpt.DocEntry = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { rpt.DocNum = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { rpt.CardCode = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { rpt.CardName = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { rpt.Estado = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { rpt.TipoVenta = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { rpt.LugarDestino = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { rpt.DirDestino = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { rpt.Referencia = dr.GetString(8); }
                            if (!dr.IsDBNull(9)) { rpt.Agencia = dr.GetString(9); }
                            if (!dr.IsDBNull(10)) { rpt.EnvioAgencia = dr.GetString(10); }
                            if (!dr.IsDBNull(11)) { rpt.Embalaje = dr.GetString(11); }
                            if (!dr.IsDBNull(12)) { rpt.CodSapVendedor = dr.GetInt32(12); }
                            if (!dr.IsDBNull(13)) { rpt.Vendedor = dr.GetString(13); }
                            if (!dr.IsDBNull(14)) { rpt.MontoTotal = dr.GetDecimal(14); }
                            if (!dr.IsDBNull(15)) { rpt.Flete = dr.GetDecimal(15); }
                            if (!dr.IsDBNull(16)) { rpt.GastoEnvio = dr.GetDecimal(16); }
                            if (!dr.IsDBNull(17)) { rpt.EstadoGasto = dr.GetString(17); }
                            if (!dr.IsDBNull(18)) { rpt.PagoEnv = dr.GetDecimal(18); }
                            if (!dr.IsDBNull(19)) { rpt.ClaveEnv = dr.GetString(19); }
                            if (!dr.IsDBNull(20)) { rpt.TiempoEntrega = dr.GetDateTime(20).ToString("dd/MM/yyyy hh:mm:ss"); }
                            if (!dr.IsDBNull(21)) { rpt.DescuentoNC = dr.GetDecimal(21); }
                            if (!dr.IsDBNull(22)) { rpt.DeudaCliente = dr.GetDecimal(22); }
                            if (!dr.IsDBNull(23)) { rpt.DeudaEmpresa = dr.GetDecimal(23); }
                            if (!dr.IsDBNull(24)) { rpt.MontoFinal = dr.GetDecimal(24); }
                            if (!dr.IsDBNull(25)) { rpt.FormaPago = dr.GetString(25); }
                            if (!dr.IsDBNull(26)) { rpt.MontoRecibido = dr.GetDecimal(26); }
                            if (!dr.IsDBNull(27)) { rpt.EstadoPago = dr.GetString(27); }
                            if (!dr.IsDBNull(28)) { rpt.FechaPago = dr.GetDateTime(28).ToString("dd/MM/yyyy"); }
                            if (!dr.IsDBNull(29)) { rpt.HoraPago = dr.GetTimeSpan(29).ToString(); }
                            if (!dr.IsDBNull(30)) { rpt.CodSapCajero = dr.GetInt32(30); }
                            if (!dr.IsDBNull(31)) { rpt.Cajero = dr.GetString(31); }
                            if (!dr.IsDBNull(32)) { rpt.Comentario = dr.GetString(32); }
                            if (!dr.IsDBNull(33)) { rpt.Cajas = dr.GetInt32(33); }
                            if (!dr.IsDBNull(34)) { rpt.NroMesa = dr.GetInt32(34); }
                            if (!dr.IsDBNull(35)) { rpt.FechaNC = dr.GetString(35); }
                            if (!dr.IsDBNull(36)) { rpt.EstadoFacturacion = dr.GetString(36); }
                            if (!dr.IsDBNull(37)) { rpt.FechaFacturacion = dr.GetDateTime(37).ToString("dd/MM/yyyy"); }
                            if (!dr.IsDBNull(38)) { rpt.HoraFacturacion = dr.GetTimeSpan(38).ToString(); }
                            if (!dr.IsDBNull(39)) { rpt.OpFacturacion = dr.GetString(39); }
                            if (!dr.IsDBNull(40)) { rpt.Observaciones = dr.GetString(40); }
                            if (!dr.IsDBNull(41)) { rpt.Observaciones2 = dr.GetString(41); }
                            if (!dr.IsDBNull(42)) { rpt.Observaciones3 = dr.GetString(42); }
                            if (!dr.IsDBNull(43)) { rpt.FechaSapTicket = dr.GetDateTime(43).ToString("dd/MM/yyyy"); }
                            if (!dr.IsDBNull(44)) { rpt.AlmProcedencia = dr.GetString(44); }
                            if (!dr.IsDBNull(45)) { rpt.LineaPeso = dr.GetInt32(45); }
                            if (!dr.IsDBNull(46)) { rpt.Peso = dr.GetDecimal(46); }

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

        // Nuevo método: actualiza HoraEntrega (hora de llegada) en RRU0 o RRU1
        public bool ActualizarHoraLlegada(int docEntry, int docNum, string nuevaHora)
        {
            int filas = 0;
            var ts = TimeSpan.Parse(nuevaHora); // admite HH:mm:ss
            using (var cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                using (var cmd = new SqlCommand("update al.RRU0 set HoraEntrega = @Hora where DocEntry=@DocEntry and DocNumTicket=@DocNum and Estado='ENTREGADO'", cn))
                {
                    cmd.Parameters.AddWithValue("@Hora", ts);
                    cmd.Parameters.AddWithValue("@DocEntry", docEntry);
                    cmd.Parameters.AddWithValue("@DocNum", docNum);
                    filas = cmd.ExecuteNonQuery();
                }
                if (filas == 0)
                {
                    using (var cmd2 = new SqlCommand("update al.RRU1 set HoraEntrega = @Hora where DocEntry=@DocEntry and Guia=@Guia and Estado='ENTREGADO'", cn))
                    {
                        cmd2.Parameters.AddWithValue("@Hora", ts);
                        cmd2.Parameters.AddWithValue("@DocEntry", docEntry);
                        cmd2.Parameters.AddWithValue("@Guia", docNum.ToString());
                        filas = cmd2.ExecuteNonQuery();
                    }
                }
                cn.Close();
            }
            return filas > 0;
        }

        public List<OrdenDevolucionHana> ListarOrdenesDevolucionDesdeHana(string cardCode, string fecha)
        {
            var lista = new List<OrdenDevolucionHana>();

            if (string.IsNullOrWhiteSpace(cardCode) || string.IsNullOrWhiteSpace(fecha))
                return lista;

            if (!DateTime.TryParse(fecha, out DateTime fechaParsed))
                throw new Exception("Fecha inválida: " + fecha);

            string fechaFormato = fechaParsed.ToString("yyyy-MM-dd");

            // Sanitizar cardCode
            cardCode = cardCode.Replace("'", "''").Trim();

            string query = @"
                        SELECT 
                            T0.""DocEntry"",
                            T0.""DocNum"",
                            T0.""CardCode"",
                            T0.""CardName"",
                            CASE
                                    WHEN IFNULL(T0.""ShipToCode"", '') <> ''
                                         THEN T0.""Address2""
                                    ELSE T0.""Address""
                                END AS ""DireccionEnvio"",
                            T0.""Comments"",
                            T0.""JrnlMemo"",
                            IFNULL(T0.""U_SYP_MDTD"", '') || '-' || 
                            IFNULL(T0.""U_SYP_MDSD"", '') || '-' || 
                            IFNULL(T0.""U_SYP_MDCD"", '') AS ""NumGuia"",
                            T0.""U_SYP_MDMT"",
                            T0.""U_COB_LUGAREN"",
                            T0.""U_BPP_NUDOCCOND"",
                            T0.""U_SYP_MDFN"",
                            T0.""U_SYP_MDVC"",
                            IFNULL(T0.""U_BPP_NUMBUL"", 0) AS ""Bultos"",
                            IFNULL(T0.""DocTotal"", 0) AS ""DocTotal"",
                            T0.""U_BPP_FECINITRA""
                        FROM " + uti.schemaHana + @"ORRR T0
                        WHERE T0.""CardCode"" = '" + cardCode + @"'
                          AND T0.""DocDate"" = '" + fechaFormato + @"'
                          AND T0.""CANCELED"" = 'N'
                        ORDER BY T0.""DocNum"" DESC";
            try
            {
                using (var hdr = db.HanaExecuteReaderNoSp(query))
                {
                    while (hdr.Read())
                    {
                        var o = new OrdenDevolucionHana
                        {
                            DocEntry = !hdr.IsDBNull(0) ? hdr.GetInt32(0) : 0,
                            DocNum = !hdr.IsDBNull(1) ? hdr.GetInt32(1) : 0,
                            CardCode = !hdr.IsDBNull(2) ? hdr.GetString(2) : "",
                            CardName = Trunc(!hdr.IsDBNull(3) ? hdr.GetString(3) : "", 200),
                            Address = Trunc(!hdr.IsDBNull(4) ? hdr.GetString(4) : "", 200),
                            Comments = Trunc(!hdr.IsDBNull(5) ? hdr.GetString(5) : "", 400),
                            JrnlMemo = Trunc(!hdr.IsDBNull(6) ? hdr.GetString(6) : "", 100),
                            NumAtCard = Trunc(!hdr.IsDBNull(7) ? hdr.GetString(7) : "", 50),
                            TipoMotivo = Trunc(!hdr.IsDBNull(8) ? hdr.GetString(8) : "", 50),
                            Agencia = Trunc(!hdr.IsDBNull(9) ? hdr.GetString(9) : "", 100),
                            NumDocConductor = Trunc(!hdr.IsDBNull(10) ? hdr.GetString(10) : "", 20),
                            Conductor = Trunc(!hdr.IsDBNull(11) ? hdr.GetString(11) : "", 100),
                            Placa = Trunc(!hdr.IsDBNull(12) ? hdr.GetString(12) : "", 30),
                            Bultos = !hdr.IsDBNull(13) ? hdr.GetInt32(13) : 0,
                            DocTotal = !hdr.IsDBNull(14) ? hdr.GetDecimal(14) : 0m,
                            // ✅ Convertir DateTime de HANA a formato ISO 8601 (YYYY-MM-DDTHH:MM:SS)
                            U_BPP_FECINITRA = !hdr.IsDBNull(15) 
                                      ? (hdr.GetValue(15) is DateTime dt 
                                      ? dt.ToString("yyyy-MM-ddTHH:mm:ss")  // Formato para datetime-local
                                      : hdr.GetString(15))  // Si ya viene como string
                                      : ""
                        };

                        lista.Add(o);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Error al listar devoluciones (HANA ORRR): " + ex.Message,
                    ex
                );
            }

            return lista;

            string Trunc(string val, int max)
            {
                if (string.IsNullOrEmpty(val))
                    return "";

                return val.Length <= max ? val : val.Substring(0, max);
            }
        }

        public List<dynamic> ListarClientesPorFechaDesdeHana(string fecha)
        {
            var lista = new List<dynamic>();

            try
            {
                // Convertir fecha a formato YYYY-MM-DD para comparar con DocDate
                string fechaFormato = DateTime.Parse(fecha).ToString("yyyy-MM-dd");

                // Consulta a ORRR usando DocDate (campo correcto para la fecha)
                string query = @"
                SELECT DISTINCT
                    T0.""CardCode"",
                    T0.""CardName""
                FROM " + uti.schemaHana + @"ORRR T0
                WHERE T0.""DocDate"" = '" + fechaFormato + @"'
                ORDER BY T0.""CardName"" ASC";

                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);

                while (hdr.Read())
                {
                    var cliente = new
                    {
                        CardCode = !hdr.IsDBNull(0) ? hdr.GetString(0) : "",
                        CardName = !hdr.IsDBNull(1) ? hdr.GetString(1) : ""
                    };

                    lista.Add(cliente);
                }

                hdr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar clientes desde HANA (tabla ORRR): " + ex.Message);
            }

            return lista;
        }

        public void RecibirDevolucion(int DocEntry, string opRegistro)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("al.MANT_ORRU", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "URD");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@Operario", opRegistro);

                    // Parámetros no utilizados pero requeridos por el SP, enviar NULL
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Det", null);
                    cmd.Parameters.AddWithValue("@Det1", null);
                    cmd.Parameters.AddWithValue("@DetDoc", null);
                    cmd.Parameters.AddWithValue("@DetProd", null);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error en RecibirDevolucion: " + ex.Message);
                }
            }
        }

        public List<ORRU_E.RptRutasExcel> ObtenerRptRutasExcel(int docEntry)
        {
            var lista = new List<ORRU_E.RptRutasExcel>();
            string query = @"
        SELECT 
            T0.Guias,
            T0.DocNumTicket,
            T5.CardCode,
            T4.Calle,
            CONCAT(T4.Departamento, ', ', T4.Provincia, ', ', T4.Distrito) AS Departamento,
            SUM(T3.Peso) AS Peso,
            T0.DocEntryTicket,
            T0.Cajas,
	        T1.NombrePer,
	        T1.DocPer,
	        T1.TelfPer,
            T5.CardName
        FROM al.RRU0 T0
        LEFT OUTER JOIN vt.RTV6 T3 ON T3.DocEntry = T0.DocEntryTicket
        LEFT OUTER JOIN vt.RTV3 T4 ON T4.DocEntry = T0.DocEntryTicket
        LEFT OUTER JOIN vt.ORTV T5 ON T5.DocEntry = T0.DocEntryTicket
        LEFT OUTER JOIN vt.RTV1 T1 ON T1.DocEntry = T0.DocEntryTicket
        WHERE T0.DocEntry = @DocEntry AND T0.Estado <> 'LIBERADO'
        GROUP BY 
            T0.Guias,
            T0.DocNumTicket,
            T5.CardCode,
T4.Calle,
            T4.Departamento,
            T4.Provincia,
            T4.Distrito,
            T0.DocEntryTicket,
            T0.Cajas,
	        T1.NombrePer,
	        T1.DocPer,
	        T1.TelfPer,
T5.CardName        

        ";

            try
            {
                using (var cn = new SqlConnection(uti.cadSql))
                using (var cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@DocEntry", docEntry);
                    cn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var item = new ORRU_E.RptRutasExcel();
                            if (!dr.IsDBNull(0)) item.Guias = dr.GetString(0);
                            if (!dr.IsDBNull(1)) item.OrdenCompra = dr.GetInt32(1);
                            if (!dr.IsDBNull(2)) item.Ruc = dr.GetString(2);
                            if (!dr.IsDBNull(3)) item.Direccion = dr.GetString(3);
                            if (!dr.IsDBNull(4)) item.Departamento = dr.GetString(4);
                            if (!dr.IsDBNull(5)) item.Peso = dr.GetDecimal(5);
                            if (!dr.IsDBNull(6)) item.DocEntry = dr.GetInt32(6);
                            if (!dr.IsDBNull(7)) item.Cajas = dr.GetInt32(7);
                            if (!dr.IsDBNull(8)) item.PersonaRecojo = dr.GetString(8);
                            if (!dr.IsDBNull(9)) item.Documento = dr.GetString(9);
                            if (!dr.IsDBNull(10)) item.Telefono = dr.GetString(10);
                            if (!dr.IsDBNull(11)) item.RazonSocial = dr.GetString(11);
                            lista.Add(item);
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                // Puedes registrar el error si tienes un logger, por ejemplo:
                // LogHelper.RegistrarError(ex, "Error en ObtenerRptRutasExcel");
                throw new Exception("Error en ObtenerRptRutasExcel: " + ex.Message, ex);
            }
            return lista;
        }

        public List<ProvedorTrans> listarProvedores()
        {
            var lista = new List<ProvedorTrans>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                string query = "SELECT IdProvedor, Nombre, RUC FROM al.ProvTrans ORDER BY Nombre";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var p = new ProvedorTrans();
                    if (!dr.IsDBNull(0)) p.IdProvedor = dr.GetInt32(0);
                    if (!dr.IsDBNull(1)) p.Nombre = dr.GetString(1);
                    if (!dr.IsDBNull(2)) p.RUC = dr.GetString(2);
                    lista.Add(p);
                }
                dr.Close();
                cn.Close();
            }
            catch
            {
                cn.Close();
            }
            return lista;
        }

        public List<dynamic> ObtenerRptRutasExcelGuiaProveedor(int docEntry)
        {
            var lista = new List<dynamic>(); string query = @" SELECT
            T0.DocNumTicket, sp.val AS Guia, T2.TransDesc, PO.RUC FROM al.RRU0 T0 CROSS APPLY dbo.Split(REPLACE(REPLACE(T0.Guias, CHAR(13)+CHAR(10), ','), CHAR(10), ','), ',') AS sp INNER JOIN al.ORRU T2 ON T0.DocEntry = T2.DocEntry LEFT JOIN al.ProvTrans PO ON T2.TransDesc = PO.Nombre WHERE T0.DocEntry = @DocEntry GROUP BY sp.val, T0.DocNumTicket, T2.TransDesc, PO.RUC";
            using (var cn = new SqlConnection(uti.cadSql))
            using (var cmd = new SqlCommand(query, cn))
            {
                cmd.Parameters.AddWithValue("@DocEntry", docEntry);
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var item = new
                        {
                            DocNumTicket = !dr.IsDBNull(0) ? dr.GetInt32(0) : 0,
                            Guia = !dr.IsDBNull(1) ? dr.GetString(1) : "",
                            TransDesc = !dr.IsDBNull(2) ? dr.GetString(2) : "",
                            RUC = !dr.IsDBNull(3) ? dr.GetString(3) : ""
                        };
                        lista.Add(item);
                    }
                }
            }
            return lista;
        }

        public int  NuevaHojaDeDevolucion(ORRU_E o)
        {
            int status = -1;
            string TipoMant = "ARD";
            string TipoPost = "C";

            // ✅ DEBUG: Verificar qué está llegando
            System.Diagnostics.Debug.WriteLine($"DetRRU0DE Count: {o.DetRRU0DE?.Count ?? 0}");
            System.Diagnostics.Debug.WriteLine($"DetRRU0 Count: {o.DetRRU0?.Count ?? 0}");

            // Si DetRRU0DE está vacío pero DetRRU0 tiene datos, copiar
            if ((o.DetRRU0DE == null || o.DetRRU0DE.Count == 0) && o.DetRRU0 != null && o.DetRRU0.Count > 0)
            {
                o.DetRRU0DE = o.DetRRU0.Select(rru0 => new RRU0_DE_E
                {
                    Linea = rru0.Linea,
                    DocEntryTicket = rru0.DocEntryTicket,
                    DocNumTicket = rru0.DocNumTicket,
                    Socio = rru0.Socio,
                    Guias = rru0.Guias,
                    Verificado = rru0.Verificado,
                    Cajas = rru0.Cajas,
                    Observaciones = rru0.Observaciones,
                    MontoFinal = rru0.MontoFinal,
                    Envio = rru0.Envio,
                    Direcciones = rru0.Direcciones,
                    Estado = "PREENVIO",
                    ConducYPlaca = rru0.ConducYPlaca,
                    EnvioAgencia = rru0.EnvioAgencia
                }).ToList();

                System.Diagnostics.Debug.WriteLine($"Copié DetRRU0 a DetRRU0DE. Nuevo count: {o.DetRRU0DE.Count}");
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    SqlTransaction tran = cn.BeginTransaction();
                    try
                    {
                        SqlCommand cmd = new SqlCommand("al.MANT_ORRU_DEVOLUCIONES", cn, tran)
                        {
                            CommandType = CommandType.StoredProcedure
                        };

                        cmd.Parameters.AddWithValue("@TipoMantenimiento", TipoMant);
                        cmd.Parameters.Add("@DocEntry", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@DocNum", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@TipoRuta", "DE");
                        cmd.Parameters.AddWithValue("@TransCod", o.TransCod ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@TransDesc", o.TransDesc ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@VehiculoCod", o.VehiculoCod ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Placa", o.Placa ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Marca", o.Marca ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Modelo", o.Modelo ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CopilDesc", o.CopilDesc ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Copil2Desc", o.Copil2Desc ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Copil3Desc", o.Copil3Desc ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Copil4Desc", o.Copil4Desc ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaCont", o.FechaCont);
                        cmd.Parameters.AddWithValue("@AlmOrigenCod", o.AlmOrigenCod ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AlmOrigenDesc", o.AlmOrigenDesc ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AlmOrigenDesc2", o.AlmOrigenDesc2 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AlmDestinoCod", o.AlmDestinoCod ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AlmDestinoDesc", o.AlmDestinoDesc ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@AlmDestinoDesc2", o.AlmDestinoDesc2 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Propietario", o.Propietario ?? (object)DBNull.Value);

                        DateTime newTiempoPac = Convert.ToDateTime(o.TiempoPac);
                        DateTime tiempoPacFormatted = new DateTime(newTiempoPac.Year, newTiempoPac.Month, newTiempoPac.Day, newTiempoPac.Hour, 0, 0);
                        cmd.Parameters.AddWithValue("@TiempoPac", tiempoPacFormatted);
                        cmd.Parameters.AddWithValue("@Observaciones", o.Observaciones ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Operario", o.Propietario ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Origen", o.Origen ?? (object)DBNull.Value);

                        // ✅ PARÁMETROS DE TABLA ESTRUCTURADA
                        SqlParameter tbDet = new SqlParameter("@Det", SqlDbType.Structured)
                        {
                            TypeName = "al.TPRRU0",
                            Value = (o.DetRRU0DE != null && o.DetRRU0DE.Count > 0)
                                ? RRU0_DE_E.tbDetalleDE(o.DetRRU0DE)
                                : RRU0_DE_E.tbDetalleDE(new List<RRU0_DE_E>())
                        };
                        cmd.Parameters.Add(tbDet);

                        // DEBUG: Verificar el DataTable
                        DataTable dt = (DataTable)tbDet.Value;
                        System.Diagnostics.Debug.WriteLine($"DataTable @Det tiene {dt?.Rows.Count ?? 0} filas");

                        SqlParameter tbDetDoc = new SqlParameter("@DetDoc", SqlDbType.Structured)
                        {
                            TypeName = "al.TPRRU01",
                            Value = (o.DetRRU01 != null && o.DetRRU01.Count > 0)
                                ? RRU01_E.tbDetalle(o.DetRRU01)
                                : RRU01_E.tbDetalle(new List<RRU01_E>())
                        };
                        cmd.Parameters.Add(tbDetDoc);

                        cmd.ExecuteNonQuery();

                        int docEntry = (int)cmd.Parameters["@DocEntry"].Value;
                        int docNum = (int)cmd.Parameters["@DocNum"].Value;
                        status = docNum;

                        SqlCommand cmd2 = new SqlCommand("dbo.POST_TRANSACCIONES", cn, tran);
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@Tipo", TipoPost);
                        cmd2.Parameters.AddWithValue("@Tabla", "ORRU");
                        cmd2.Parameters.AddWithValue("@DocNum", docNum);
                        cmd2.Parameters.AddWithValue("@DocEntry", docEntry);
                        cmd2.ExecuteNonQuery();

                        tran.Commit();
                    }
                    catch (Exception e1)
                    {
                        tran.Rollback();
                        throw new Exception("Error en creación de devolución: " + e1.Message);
                    }
                }
                catch (Exception e2)
                {
                    throw new Exception("Error en creación y conexión: " + e2.Message);
                }
            }

            return status;
        }

        /// ✅ NUEVO MÉTODO: Obtener orden de ruta ESPECÍFICAMENTE para DEVOLUCIONES
        /// ✅ NUEVO MÉTODO: Obtener orden de ruta ESPECÍFICAMENTE para DEVOLUCIONES
        public ORRU_E obtenerOrdenDeRutaDevolucion(int DocEntry, SqlConnection cn = null, SqlTransaction tran = null)
        {
            ORRU_E o = new ORRU_E();
            o.DetRRU0 = new List<RRU0_E>();
            o.DetRRU01 = new List<RRU01_E>();
            o.DetRRU1 = new List<RRU1_E>();
            o.DetRRU11 = new List<RRU11_E>();
            o.DetRRU0DE = new List<RRU0_DE_E>();

            bool cerrarConexion = false;

            if (cn == null)
            {
                cn = new SqlConnection(uti.cadSql);
                cn.Open();
                cerrarConexion = true;
            }

            try
            {
                // ✅ CARGAR ENCABEZADO DE LA RUTA
                SqlCommand cmd = new SqlCommand("select T1.DocEntry,T1.DocNum,T1.TipoRuta,T1.TransCod,T1.TransDesc,T1.VehiculoCod,T1.Placa," +
                    " T1.Marca,T1.Modelo,T1.CopilDesc,T1.Copil2Desc,T1.Copil3Desc,T1.Copil4Desc,T1.FechaCont,T1.FechaDoc," +
                    " T1.AlmOrigenCod,T1.AlmOrigenDesc,T1.AlmOrigenDesc2,T1.AlmDestinoCod,T1.AlmDestinoDesc,T1.AlmDestinoDesc2," +
                    " T1.Propietario,T1.TiempoPac,(select top 1 concat(FechaOperacion,' ',convert(varchar(8),HoraOperacion)) from al.CC_ORRU " +
                    " where Operacion='INICIAR' and DocEntry=t1.DocEntry order by FechaOperacion,HoraOperacion desc),(select top 1 concat(FechaOperacion,' ',convert(varchar(8),HoraOperacion)) from al.CC_ORRU " +
                    " where Operacion='TERMINAR' and DocEntry=t1.DocEntry order by FechaOperacion,HoraOperacion desc),T1.Estado,T1.Observaciones,(select top 1 Operario from al.CC_ORRU " +
                    " where Operacion='INICIAR' and DocEntry=t1.DocEntry order by FechaOperacion,HoraOperacion desc) ,(select top 1 Operario from " +
                    "al.CC_ORRU where Operacion='TERMINAR' and DocEntry=t1.DocEntry order by FechaOperacion,HoraOperacion desc),T1.Agencia,T1.RucAgencia," +
                    " (select SerieT1 from al.oveh where Code = T1.VehiculoCod), (select SerieT2 from al.oveh where Code = T1.VehiculoCod), " +
                    " T1.HoraRegistro " +  // ✅ AGREGAR ESTA LÍNEA
                    "from al.ORRU T1 where T1.DocEntry=@DocEntry", cn, tran);

                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { o.DocNum = dr.GetInt32(1); }
                if (!dr.IsDBNull(2)) { o.TipoRuta = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { o.TransCod = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { o.TransDesc = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { o.VehiculoCod = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { o.Placa = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { o.Marca = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { o.Modelo = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { o.CopilDesc = dr.GetString(9); }
                if (!dr.IsDBNull(10)) { o.Copil2Desc = dr.GetString(10); }
                if (!dr.IsDBNull(11)) { o.Copil3Desc = dr.GetString(11); }
                if (!dr.IsDBNull(12)) { o.Copil4Desc = dr.GetString(12); }
                if (!dr.IsDBNull(13)) { o.FechaCont = dr.GetDateTime(13); }
                if (!dr.IsDBNull(14)) { o.FechaDoc = dr.GetDateTime(14); }
                if (!dr.IsDBNull(15)) { o.AlmOrigenCod = dr.GetString(15); }
                if (!dr.IsDBNull(16)) { o.AlmOrigenDesc = dr.GetString(16); }
                if (!dr.IsDBNull(17)) { o.AlmOrigenDesc2 = dr.GetString(17); }
                if (!dr.IsDBNull(18)) { o.AlmDestinoCod = dr.GetString(18); }
                if (!dr.IsDBNull(19)) { o.AlmDestinoDesc = dr.GetString(19); }
                if (!dr.IsDBNull(20)) { o.AlmDestinoDesc2 = dr.GetString(20); }
                if (!dr.IsDBNull(21)) { o.Propietario = dr.GetString(21); }
                if (!dr.IsDBNull(22)) { o.TiempoPac = dr.GetDateTime(22); }
                if (!dr.IsDBNull(23)) { o.TiempoIniEn = dr.GetString(23); }
                if (!dr.IsDBNull(24)) { o.TiempoTerEn = dr.GetString(24); }
                if (!dr.IsDBNull(25)) { o.Estado = dr.GetString(25); }
                if (!dr.IsDBNull(26)) { o.Observaciones = dr.GetString(26); }
                if (!dr.IsDBNull(27)) { o.OpInicio = dr.GetString(27); }
                if (!dr.IsDBNull(28)) { o.OpTermino = dr.GetString(28); }
                if (!dr.IsDBNull(29)) { o.Agencia = dr.GetString(29); }
                if (!dr.IsDBNull(30)) { o.RucAgencia = dr.GetString(30); }
                if (!dr.IsDBNull(31)) { o.SerieT1 = dr.GetString(31); }
                if (!dr.IsDBNull(32)) { o.SerieT2 = dr.GetString(32); }
                if (!dr.IsDBNull(33)) { o.HoraRegistro = dr.GetTimeSpan(33).ToString(); } // ✅ AGREGAR ESTA LÍNEA

                dr.Close();

                // ✅ CARGAR DETALLES DESDE RRU0_DE (SIN CARGAR TICKET COMPLETO)
                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT R0.DocEntry, R0.Linea, R0.DocEntryTicket, R0.DocNumTicket, R0.Guias, R0.Verificado, R0.Cajas, R0.Observaciones, R0.MontoFinal, R0.Envio, R0.Direcciones,");
                sb.Append(" R0.Estado, R0.TempI1, R0.TempI2, R0.TempF1, R0.TempF2, R0.OpEntrega, R0.FechaEntrega, R0.HoraEntrega, R0.Socio,");
                sb.Append(" ISNULL((SELECT TOP 1 CardCode FROM vt.ORTV WHERE CardName = R0.Socio), '') as CardCode");
                sb.Append(" FROM al.RRU0_DE R0");
                sb.Append(" WHERE R0.DocEntry=@DocEntry");
                sb.Append(" ORDER BY R0.Linea ASC");

                string query = sb.ToString();

                SqlCommand cmd2 = new SqlCommand(query, cn, tran);
                cmd2.CommandType = CommandType.Text;
                cmd2.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr2 = cmd2.ExecuteReader();

                while (dr2.Read())
                {
                    RRU0_DE_E d = new RRU0_DE_E();
                    if (!dr2.IsDBNull(0)) d.DocEntry = dr2.GetInt32(0);
                    if (!dr2.IsDBNull(1)) d.Linea = dr2.GetInt32(1);
                    if (!dr2.IsDBNull(2)) d.DocEntryTicket = dr2.GetInt32(2);
                    if (!dr2.IsDBNull(3)) d.DocNumTicket = dr2.GetInt32(3);
                    if (!dr2.IsDBNull(4)) d.Guias = dr2.GetString(4);
                    if (!dr2.IsDBNull(5)) d.Verificado = dr2.GetString(5);
                    if (!dr2.IsDBNull(6)) d.Cajas = dr2.GetInt32(6);
                    if (!dr2.IsDBNull(7)) d.Observaciones = dr2.GetString(7);
                    if (!dr2.IsDBNull(8)) d.MontoFinal = dr2.GetDecimal(8);
                    if (!dr2.IsDBNull(9)) d.Envio = dr2.GetDecimal(9);
                    if (!dr2.IsDBNull(10)) d.Direcciones = dr2.GetString(10);
                    if (!dr2.IsDBNull(11)) d.Estado = dr2.GetString(11);
                    if (!dr2.IsDBNull(12)) d.TempI1 = dr2.GetDecimal(12);
                    if (!dr2.IsDBNull(13)) d.TempI2 = dr2.GetDecimal(13);
                    if (!dr2.IsDBNull(14)) d.TempF1 = dr2.GetDecimal(14);
                    if (!dr2.IsDBNull(15)) d.TempF2 = dr2.GetDecimal(15);
                    if (!dr2.IsDBNull(16)) d.OpEntrega = dr2.GetString(16);
                    if (!dr2.IsDBNull(17)) d.FechaEntrega = dr2.GetDateTime(17).ToString("yyyy-MM-dd");
                    if (!dr2.IsDBNull(18)) d.HoraEntrega = dr2.GetTimeSpan(18).ToString();
                    if (!dr2.IsDBNull(19)) d.Socio = dr2.GetString(19);
                    if (!dr2.IsDBNull(20)) d.CardCode = dr2.GetString(20);  // ✅ NUEVO


                    // ✅ CREAR TICKET BÁSICO - SOLO CON SOCIO Y CARDCODE
                    d.Ticket = new ORTV_E();

                    // Si existe socio, buscar CardCode en vt.ORTV
                    if (!string.IsNullOrWhiteSpace(d.Socio))
                    {
                        try
                        {
                            string sqlRuc = "SELECT TOP 1 CardCode FROM vt.ORTV WHERE CardName = @SocioName";
                            SqlCommand cmdRuc = new SqlCommand(sqlRuc, cn, tran);
                            cmdRuc.Parameters.AddWithValue("@SocioName", d.Socio);

                            object resultado = cmdRuc.ExecuteScalar();

                            // ✅ ASIGNAR CARDCODE SI EXISTE
                            if (resultado != null && resultado != DBNull.Value)
                            {
                                d.Ticket.CardCode = resultado.ToString();
                                d.Ticket.CardName = d.Socio;
                            }
                            else
                            {
                                // Si no encuentra en ORTV, usar el nombre del socio como es
                                d.Ticket.CardCode = "";
                                d.Ticket.CardName = d.Socio;
                            }
                        }
                        catch
                        {
                            // Si falla la búsqueda, solo asignar el nombre
                            d.Ticket.CardCode = "";
                            d.Ticket.CardName = d.Socio;
                        }
                    }
                    else
                    {
                        // Si no hay socio, inicializar vacío
                        d.Ticket.CardCode = "";
                        d.Ticket.CardName = "";
                    }

                    o.DetRRU0DE.Add(d);
                }
                dr2.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                if (cerrarConexion && cn != null)
                {
                    cn.Close();
                }
            }
            return o;
        }

        //public List<ODLN_E> ObtenerListaGuiasSap(string zona, DateTime? fechaSapTicket, string socio = null, string guias = null, string nroSap = null)
        //{
        //    List<ODLN_E> lista = new List<ODLN_E>();
        //    OINV_D oinvD = new OINV_D();
        //    ODLN_D odlnD = new ODLN_D();

        //    var query = new System.Text.StringBuilder();

        //    query.AppendLine($@"
        //        SELECT 
        //            ""DocNum"",
        //            ""DocDate"",
        //            ""CardCode"",
        //            ""CardName"",
        //            ""NumAtCard"",
        //            ""DocTotal"",
        //            IFNULL(""U_SYP_MDFN"", '') || ' - ' || IFNULL(""U_SYP_MDVC"", '') AS ""ConductorYPlaca"",
        //            ""U_COB_ZONA""
        //        FROM {uti.schemaHana}ODLN
        //        WHERE 1=1   
        //    ");
        //    //AND IFNULL(""U_SYP_MDFN"", '') <> ''
        //    //AND IFNULL(""U_SYP_MDVC"", '') <> ''

        //    // filtros dinámicos
        //    if (!string.IsNullOrEmpty(zona))
        //        query.AppendLine(@" AND ""U_COB_ZONA"" = ? ");
        //    if (fechaSapTicket.HasValue)
        //        query.AppendLine(@" AND TO_DATE(""DocDate"") = ? ");
        //    if (!string.IsNullOrEmpty(socio))
        //        query.AppendLine(@" AND UPPER(""CardName"") LIKE ? ");
        //    if (!string.IsNullOrEmpty(guias))
        //        query.AppendLine(@" AND UPPER(""NumAtCard"") LIKE ? ");
        //    if (!string.IsNullOrEmpty(nroSap))
        //        query.AppendLine(@" AND ""DocNum"" = ? ");

        //    query.AppendLine(@" ORDER BY ""DocEntry"" DESC ");
        //    query.AppendLine(@" LIMIT 50 ");

        //    using (HanaConnection conn = new HanaConnection(uti.cadHana))
        //    {
        //        conn.Open();

        //        using (HanaCommand cmd = new HanaCommand(query.ToString(), conn))
        //        {
        //            if (!string.IsNullOrEmpty(zona))
        //                cmd.Parameters.Add(new HanaParameter { Value = zona });
        //            if (fechaSapTicket.HasValue)
        //                cmd.Parameters.Add(new HanaParameter { Value = fechaSapTicket.Value.Date });
        //            if (!string.IsNullOrEmpty(socio))
        //                cmd.Parameters.Add(new HanaParameter { Value = "%" + socio.ToUpper() + "%" });
        //            if (!string.IsNullOrEmpty(guias))
        //                cmd.Parameters.Add(new HanaParameter { Value = "%" + guias.ToUpper() + "%" });
        //            if (!string.IsNullOrEmpty(nroSap))
        //                cmd.Parameters.Add(new HanaParameter { Value = nroSap });

        //            using (HanaDataReader dr = cmd.ExecuteReader())
        //            {
        //                while (dr.Read())
        //                {
        //                    ODLN_E obj = new ODLN_E();

        //                    obj.DocNum = Convert.ToInt32(dr["DocNum"]);
        //                    obj.CardCode = dr["CardCode"].ToString();
        //                    obj.CardName = dr["CardName"].ToString();
        //                    obj.NumAtCard = dr["NumAtCard"].ToString();
        //                    obj.DocTotal = Convert.ToDecimal(dr["DocTotal"]);
        //                    obj.ConductorYPlaca = dr["ConductorYPlaca"].ToString();
        //                    //if(string.IsNullOrEmpty(obj.ConductorYPlaca) )
        //                    //{
        //                    //    obj.ConductorYPlaca = odlnD.buscarConducyPlacaRemision(obj.DocNum);
        //                    //    if (string.IsNullOrEmpty(obj.ConductorYPlaca))
        //                    //    {
        //                    //        obj.ConductorYPlaca = oinvD.buscarConducyPlacaSinEnt(obj.DocNum);
        //                    //    }
        //                    //}

        //                    obj.Zona = dr["U_COB_ZONA"].ToString();

        //                    lista.Add(obj);
        //                }

        //            }
        //        }
        //    }

        //    return lista;
        //}


    }
}