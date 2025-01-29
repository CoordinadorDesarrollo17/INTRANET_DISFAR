using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Capa_Entidad.Ventas_ENT.Reportes;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.IO;
using Capa_Entidad.Caja_ENT;
using System.Security.Cryptography;
using Capa_Datos.Rutas_DAO.TablasSql;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Windows.Forms;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class OTC_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public string RegistrarTicketACuadrar(OTC_E tc)
        {
            string result;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd2 = new SqlCommand("cj.MANT_OTC", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;

                    cmd2.Parameters.AddWithValue("@Accion", "INS");
                    cmd2.Parameters.AddWithValue("@DocEntryTicket", tc.DocEntryTicket);
                    cmd2.Parameters.AddWithValue("@DocNumTicket", tc.DocNumTicket);
                    cmd2.Parameters.AddWithValue("@DocEntryORRU", tc.DocEntryORRU);
                    cmd2.Parameters.AddWithValue("@MontoRecibidoEfectivo", tc.MontoRecibidoEfectivo);
                    cmd2.Parameters.AddWithValue("@MontoRecibidoDeposito", tc.MontoRecibidoDeposito);
                    cmd2.Parameters.AddWithValue("@TipoPago", tc.TipoPago);
                    cmd2.Parameters.AddWithValue("@PersonaEntrega", tc.PersonaEntrega);
                    cmd2.Parameters.AddWithValue("@AutorizacionExcepcional", tc.AutorizacionExcepcional);
                    cmd2.Parameters.AddWithValue("@ComentarioCaja", (!string.IsNullOrWhiteSpace(tc.ComentarioCaja)) ? tc.ComentarioCaja.ToUpper() : "");
                    cmd2.Parameters.AddWithValue("@ComentarioVentas", (!string.IsNullOrWhiteSpace(tc.ComentarioVentas)) ? tc.ComentarioVentas.ToUpper() : "");
                    cmd2.Parameters.AddWithValue("@SaldoAFavor", tc.SaldoAFavor);
                    cmd2.Parameters.AddWithValue("@FechaCompromisoPago", tc.FechaCompromisoPago);
                    cmd2.Parameters.AddWithValue("@ComportamientoPago", tc.ComportamientoPago);
                    cmd2.Parameters.AddWithValue("@ComentarioAdjunto", (!string.IsNullOrWhiteSpace(tc.ComentarioAdjunto)) ? tc.ComentarioAdjunto.ToUpper() : "");

                    if (tc.ImgAdjunta != null && tc.ImgAdjunta.Count >= 1)
                    {
                        string path = string.Empty;
                        string rutaDirectorio = uti.directorioFileServer + @"\Repartos\ComprobantesPagoEfectivo\" + DateTime.Now.ToString("yyyy-MM-dd") + @"\";


                        if (!Directory.Exists(rutaDirectorio)) { Directory.CreateDirectory(rutaDirectorio); }

                        int num = 1;
                        foreach (var img in tc.ImgAdjunta)
                        {
                            path = rutaDirectorio + $"{tc.DocNumTicket}_{num}" + Path.GetExtension(img.FileName);
                            img.SaveAs(path);
                            ++num;
                        }
                    }

                    cmd2.ExecuteNonQuery();
                    result = "Solicitud enviada correctamente";
                    tran.Commit();
                }
                catch (Exception ex2)
                {
                    result = "Error al registrar solicitud"; tran.Rollback();
                    throw new Exception("Error en registro: " + ex2.Message);
                }
            }

            return result;
        }

        public OTC_E ObtenerDatosTicketACuadrar(int docEntryTicket, int idOTC)
        {
            OTC_E result = null;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                //ORRU_D rruD = new ORRU_D();
                //var rutaEncontrada = rruD.obtenerOrdenDeRutaTicket(docEntryTicket);

                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT TC.Id, TC.DocEntryTicket, TC.DocNumTicket, TC.DocEntryORRU, TC.MontoRecibidoEfectivo, TC.MontoRecibidoDeposito, TC.TipoPago, TC.PersonaEntrega, TC.Estado, TC.AutorizacionExcepcional,");
                sb.Append(" CASE TC.TipoPago WHEN 'PCE' THEN 'PAGO COMPLETO (EFECTIVO)' WHEN 'PMI' THEN 'PAGO MIXTO O INCOMPLETO' WHEN 'PCD' THEN 'PAGO COMPLETO (DEPÓSITO)' WHEN 'SP' THEN 'SIN PAGO' ELSE '' END AS DescTipoPago,");
                sb.Append(" TC.ComentarioCaja, TC.ComentarioVentas, TC.SaldoAFavor, TC.FechaCompromisoPago, TC.ComportamientoPago, TC.ComentarioAdjunto");
                sb.Append(" FROM cj.OTC TC");
                sb.Append($" WHERE TC.DocEntryTicket = @DocEntryTicket AND TC.Id = @IdOTC");
                //sb.Append($" WHERE TC.Estado NOT IN ('RECHAZADO', 'ANULADO') AND TC.DocEntryTicket = @DocEntryTicket AND TC.DocEntryORRU = @DocEntryORRU");

                string query = sb.ToString();

                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocEntryTicket", docEntryTicket);
                //cmd.Parameters.AddWithValue("@DocEntryORRU", rutaEncontrada.DocEntry);
                cmd.Parameters.AddWithValue("@IdOTC", idOTC);
            
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        result = new OTC_E();
                        while (dr.Read())
                        {
                            if (!dr.IsDBNull(0)) { result.IdOTC = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { result.DocEntryTicket = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { result.DocNumTicket = dr.GetInt32(2); }
                            if (!dr.IsDBNull(3)) { result.DocEntryORRU = dr.GetInt32(3); }
                            if (!dr.IsDBNull(4)) { result.MontoRecibidoEfectivo = dr.GetDecimal(4); }
                            if (!dr.IsDBNull(5)) { result.MontoRecibidoDeposito = dr.GetDecimal(5); }
                            if (!dr.IsDBNull(6)) { result.TipoPago = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { result.PersonaEntrega = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { result.Estado = dr.GetString(8); }
                            if (!dr.IsDBNull(9)) { result.AutorizacionExcepcional = dr.GetString(9); }
                            if (!dr.IsDBNull(10)) { result.DescTipoPago = dr.GetString(10); }
                            if (!dr.IsDBNull(11)) { result.ComentarioCaja = dr.GetString(11); }
                            if (!dr.IsDBNull(12)) { result.ComentarioVentas = dr.GetString(12); }
                            if (!dr.IsDBNull(13)) { result.SaldoAFavor = dr.GetString(13); }
                            if (!dr.IsDBNull(14)) { result.FechaCompromisoPago = dr.GetDateTime(14).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(15)) { result.ComportamientoPago = dr.GetString(15); }
                            if (!dr.IsDBNull(16)) { result.ComentarioAdjunto = dr.GetString(16); }
                        }
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
            }

            return result;
        }

        //metodo principal que alimenta todos los listados de ticket
        public List<ORTV_E> ListarTicketsVenta(ORTV_E filtro1)
        {
            bool param1 = false, param2 = false, param3 = false, param4 = false, param5 = false, param6 = false, param7 = false, param8 = false, param9 = false,
                    param10 = false, param11 = false, param12 = false, param13 = false;
            string condWhere = string.Empty;
            List<ORTV_E> lista = new List<ORTV_E>();

            if (filtro1 != null)
            {
                if (filtro1.DocNum > 0) { condWhere += $" AND VT.DocNum = @DocNum"; param1 = true; }
                if (filtro1.FechaSapTicket != null) { condWhere += $" AND VT.FechaSapTicket = @FechaSapTicket"; param2 = true; }
                if (filtro1.TipoVenta != null) { condWhere += $" AND VT.TipoVenta = @TipoVenta"; param3 = true; }
                if (filtro1.CardName != null) { condWhere += $" AND VT.CardName LIKE @CardName"; param4 = true; }
                if (filtro1.Estado != null) { condWhere += $" AND VT.Estado = @EstadoTicket"; param5 = true; }
                if (filtro1.Vendedor != null) { condWhere += $" AND VT.Vendedor LIKE @Vendedor"; param6 = true; }
                if (filtro1.MontoFinal > 0) { condWhere += $" AND VT.MontoFinal LIKE @MontoFinal"; param7 = true; }
                if (filtro1.LugarDestino != null) { condWhere += $" AND VT.LugarDestino = @LugarDestino"; param8 = true; }
                if (filtro1.EstadoPago != null) { condWhere += $" AND VT.EstadoPago = @EstadoPago"; param9 = true; }
                if (filtro1.EstadoGasto != null) { condWhere += $" AND VT.EstadoGasto = @EstadoGasto"; param10 = true; }
                if (filtro1.PagoEnv == 0.01M) { condWhere += $" AND VT.PagoEnv > @PagoEnv"; param11 = true; }
                if (!string.IsNullOrWhiteSpace(filtro1.TipoPago)) { condWhere += $" AND TC.TipoPago = @TipoPago"; param12 = true; }
                if (!string.IsNullOrWhiteSpace(filtro1.EstadoContraEntrega)) { condWhere += $" AND TC.Estado = @EstadoContraEntrega"; param13 = true; }
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT TOP 100 TC.Id, VT.DocEntry, VT.DocNum, CONVERT(varchar, VT.FechaSapTicket, 103), VT.TipoVenta, VT.CardName, VT.Estado, VT.Vendedor, VT.MontoFinal, VT.LugarDestino, VT.EstadoPago, VT.EstadoGasto, VT.PagoEnv,");
                sb.Append(" CASE TC.TipoPago WHEN 'PCE' THEN 'PAGO COMPLETO (EFECTIVO)' WHEN 'PMI' THEN 'PAGO MIXTO O INCOMPLETO' WHEN 'PCD' THEN 'PAGO COMPLETO (DEPÓSITO)' WHEN 'SP' THEN 'SIN PAGO' ELSE '' END AS DescTipoPago, TC.Estado");
                sb.Append(" FROM vt.ORTV VT");
                sb.Append(" LEFT JOIN cj.OTC TC ON TC.DocEntryTicket = VT.DocEntry");
                sb.Append($" WHERE VT.Estado != 'SEPARADO' AND (TC.Estado IS NULL OR TC.Estado != 'RECHAZADO')  {condWhere} ORDER BY VT.DocNum DESC");

                string query = sb.ToString();

                SqlCommand cmd = new SqlCommand(query, cn);         // prepara

                if (filtro1 != null && !string.IsNullOrWhiteSpace(condWhere))
                {
                    if (param1) { cmd.Parameters.AddWithValue("@DocNum", filtro1.DocNum); }
                    if (param2) { cmd.Parameters.AddWithValue("@FechaSapTicket", filtro1.FechaSapTicket); }
                    if (param3) { cmd.Parameters.AddWithValue("@TipoVenta", filtro1.TipoVenta); }
                    if (param4) { cmd.Parameters.AddWithValue("@CardName", string.Format("%{0}%", filtro1.CardName)); }
                    if (param5) { cmd.Parameters.AddWithValue("@EstadoTicket", filtro1.Estado); }
                    if (param6) { cmd.Parameters.AddWithValue("@Vendedor", string.Format("%{0}%", filtro1.Vendedor)); }
                    if (param7) { cmd.Parameters.AddWithValue("@MontoFinal", string.Format("{0}%", filtro1.MontoFinal)); }
                    if (param8) { cmd.Parameters.AddWithValue("@LugarDestino", filtro1.LugarDestino); }
                    if (param9) { cmd.Parameters.AddWithValue("@EstadoPago", filtro1.EstadoPago); }
                    if (param10) { cmd.Parameters.AddWithValue("@EstadoGasto", filtro1.EstadoGasto); }
                    if (param11) { cmd.Parameters.AddWithValue("@PagoEnv", 0); }
                    if (param12) { cmd.Parameters.AddWithValue("@TipoPago", filtro1.TipoPago); }
                    if (param13) { cmd.Parameters.AddWithValue("@EstadoContraEntrega", filtro1.EstadoContraEntrega); }
                }

                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ORTV_E ticket = new ORTV_E();
                            if (!dr.IsDBNull(0)) { ticket.IdOTC = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { ticket.DocEntry = dr.GetInt32(1); } /*ticket.DocEntryORRU = new ORRU_D().obtenerOrdenDeRutaTicket(dr.GetInt32(0))?.DocEntry ?? 0;*/
                            if (!dr.IsDBNull(2)) { ticket.DocNum = dr.GetInt32(2); }
                            if (!dr.IsDBNull(3)) { ticket.FechaSapTicket = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { ticket.TipoVenta = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { ticket.CardName = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { ticket.Estado = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { ticket.Vendedor = (dr.GetString(7).Length > 15) ? dr.GetString(7).Substring(0, 15) : dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { ticket.MontoFinal = dr.GetDecimal(8); }
                            if (!dr.IsDBNull(9)) { ticket.LugarDestino = dr.GetString(9); }
                            if (!dr.IsDBNull(10)) { ticket.EstadoPago = dr.GetString(10); }
                            if (!dr.IsDBNull(11)) { ticket.EstadoGasto = dr.GetString(11); }
                            if (!dr.IsDBNull(12)) { ticket.PagoEnv = dr.GetDecimal(12); }
                            if (!dr.IsDBNull(13)) { ticket.TipoPago = dr.GetString(13); }
                            if (!dr.IsDBNull(14)) { ticket.EstadoContraEntrega = dr.GetString(14); }

                            lista.Add(ticket);
                        }
                    }

                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            return lista;
        }
        public List<OTC_E> ListarTicketsACuadrar(OTC_E tc = null)
        {
            List<OTC_E> lista = null;
            string condWhere = string.Empty;
            bool param1 = false, param2 = false, param3 = false, param4 = false, param5 = false, param6 = false, param7 = false;

            if (tc != null)
            {
                if (tc.DocNumTicket > 0) { condWhere += " AND TC.DocNumTicket = @DocNumTicket"; param1 = true; }
                if (!string.IsNullOrWhiteSpace(tc.FechaSapTicket)) { condWhere += " AND VT.FechaSapTicket = @FechaSapTicket"; param2 = true; }
                if (!string.IsNullOrWhiteSpace(tc.CardCode)) { condWhere += " AND VT.CardCode = @CardCode"; param3 = true; }
                if (!string.IsNullOrWhiteSpace(tc.CardName)) { condWhere += " AND VT.CardName LIKE @CardName"; param4 = true; }
                if (!string.IsNullOrWhiteSpace(tc.TipoPago)) { condWhere += " AND TC.TipoPago = @TipoPago"; param5 = true; }
                if (!string.IsNullOrWhiteSpace(tc.PersonaEntrega)) { condWhere += " AND TC.PersonaEntrega LIKE @PersonaEntrega"; param6 = true; }
                if (!string.IsNullOrWhiteSpace(tc.Estado)) { condWhere += " AND TC.Estado = @Estado"; param7 = true; }
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT TOP 100 TC.DocEntryTicket, TC.DocNumTicket, CONVERT(varchar, VT.FechaSapTicket, 103), REPLACE(VT.CardCode, 'C', ''), VT.CardName, TC.TipoPago, TC.PersonaEntrega, TC.Estado");
                sb.Append(" FROM cj.OTC TC");
                sb.Append(" LEFT JOIN vt.ORTV VT ON VT.DocEntry = TC.DocEntryTicket");
                if (tc != null && !string.IsNullOrWhiteSpace(condWhere))
                {
                    sb.Append($" WHERE Id > 0 {condWhere}");
                }

                sb.Append($" ORDER BY TC.Id DESC");
                string query = sb.ToString();

                SqlCommand cmd = new SqlCommand(query, cn);         // prepara

                if (tc != null && !string.IsNullOrWhiteSpace(condWhere))
                {
                    if (param1) { cmd.Parameters.AddWithValue("@DocNumTicket", tc.DocNumTicket); }
                    if (param2) { cmd.Parameters.AddWithValue("@FechaSapTicket", tc.FechaSapTicket); }
                    if (param3) { cmd.Parameters.AddWithValue("@CardCode", "C" + tc.CardCode); }
                    if (param4) { cmd.Parameters.AddWithValue("@CardName", string.Format("%{0}%", tc.CardName)); }
                    if (param5) { cmd.Parameters.AddWithValue("@TipoPago", tc.TipoPago); }
                    if (param6) { cmd.Parameters.AddWithValue("@PersonaEntrega", string.Format("%{0}%", tc.PersonaEntrega)); }
                    if (param7) { cmd.Parameters.AddWithValue("@Estado", tc.Estado); }
                }

                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        lista = new List<OTC_E>();
                        while (dr.Read())
                        {
                            OTC_E obj = new OTC_E();
                            if (!dr.IsDBNull(0)) { obj.DocEntryTicket = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { obj.DocNumTicket = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { obj.FechaSapTicket = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { obj.CardCode = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { obj.CardName = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { obj.TipoPago = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { obj.PersonaEntrega = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { obj.Estado = dr.GetString(7); }

                            lista.Add(obj);
                        }
                    }

                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            return lista;
        }

        public string CambiarEstadoTicketACuadrar(OTC_E tc = null, string operacion = "", string area = "")
        {
            string result;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    cn.Open();
                    using (SqlTransaction tran = cn.BeginTransaction())
                    {
                        using (SqlCommand cmd = new SqlCommand("cj.MANT_OTC", cn, tran))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Accion", "UETC");
                            cmd.Parameters.AddWithValue("@Area", area);
                            cmd.Parameters.AddWithValue("@IdOTC", tc.IdOTC);
                            cmd.Parameters.AddWithValue("@Estado", tc.Estado);
                            cmd.Parameters.AddWithValue("@PersonaEntrega", tc.PersonaEntrega);
                            cmd.Parameters.AddWithValue("@Operacion", operacion);
                            cmd.Parameters.AddWithValue("@ComentarioCaja", string.IsNullOrWhiteSpace(tc.ComentarioCaja) ? string.Empty : tc.ComentarioCaja.ToUpper());
                            cmd.Parameters.AddWithValue("@ComentarioVentas", string.IsNullOrWhiteSpace(tc.ComentarioVentas) ? string.Empty : tc.ComentarioVentas.ToUpper());
                            cmd.Parameters.AddWithValue("@FechaCompromisoPago", tc.FechaCompromisoPago);
                            cmd.Parameters.AddWithValue("@SaldoAFavor", tc.SaldoAFavor);

                            cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                }

                result = "OK";
            }
            catch (Exception ex)
            {
                result = "ERROR";
                throw new Exception("Error al cambiar el estado: " + ex.Message);
            }

            return result;
        }

        public void ActualizarComportamientoPago(int idOTC, string comportamiento)
        {
            string query = "UPDATE cj.OTC SET ComportamientoPago=@ComportamientoPago WHERE Id=@IdOTC";

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ComportamientoPago", comportamiento);
                cmd.Parameters.AddWithValue("@IdOTC", idOTC);

                try
                {
                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex2)
                {
                    throw new Exception("Error en registro: " + ex2.Message);
                }
            }
        }

        public string AgregarPagosParciales(List<OPP_E> pp)
        {
            string result;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            using (SqlCommand cmd = new SqlCommand("cj.MANT_OPP", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    cn.Open();
                    cmd.Parameters.AddWithValue("@Accion", "INS");

                    if (pp != null && pp.Count >= 1)
                    {
                        SqlParameter tbDetalle = new SqlParameter("@TPOPP", SqlDbType.Structured);
                        tbDetalle.Value = OPP_E.TbDetalle(pp);
                        tbDetalle.TypeName = "cj.TPOPP";
                        cmd.Parameters.AddWithValue("@TPOPP", tbDetalle.Value);
                    }

                    cmd.ExecuteNonQuery();
                    result = "Pagos registrados correctamente";
                }
                catch (Exception ex)
                {
                    result = "Error al registrar los pagos parciales";
                    throw new Exception("Error en registro: " + ex.Message);
                }
            }

            return result;
        }

        // Solicitudes que autorizará el área de ventas
        public List<OTC_E> ObtenerSolicitudesAutorizar()
        {
            List<OTC_E> result = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    string query = @"SELECT
                                        TC.Id, TC.DocEntryTicket,TC.DocNumTicket,VT.Vendedor
                                    FROM
                                        cj.OTC TC
                                    INNER JOIN
                                        vt.ORTV VT ON VT.DocEntry = TC.DocEntryTicket
                                    WHERE 
                                        IIF(TC.TipoPago = 'SP'
	                                        OR (TC.MontoRecibidoEfectivo > 2000 AND TC.TipoPago = 'PCE' AND TC.Estado = 'PENDIENTE')
                                            OR (TC.MontoRecibidoEfectivo + TC.MontoRecibidoDeposito < VT.MontoFinal AND TC.MontoRecibidoDeposito > 0 AND TC.TipoPago = 'PMI' AND TC.Estado = 'VALIDADO') 
                                            OR (TC.MontoRecibidoEfectivo + TC.MontoRecibidoDeposito < VT.MontoFinal AND TC.MontoRecibidoEfectivo > 0 AND TC.TipoPago = 'PMI' AND TC.Estado = 'PENDIENTE')
	                                        , 'PENDIENTE', '') = 'PENDIENTE'
	                                    AND TC.Estado NOT IN ('AUTORIZADO', 'ANULADO', 'CUADRADO','RECHAZADO')";

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result = new List<OTC_E>();

                            while (dr.Read())
                            {
                                OTC_E obj = new OTC_E();

                                if (!dr.IsDBNull(0)) { obj.IdOTC = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.DocEntryTicket = dr.GetInt32(1); }
                                if (!dr.IsDBNull(2)) { obj.DocNumTicket = dr.GetInt32(2); }
                                if (!dr.IsDBNull(3)) { obj.Vendedor = dr.GetString(3); }

                                result.Add(obj);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error al obtener las solicitudes para autorizar: " + e.Message);
            }

            return result;
        }

        public int ConsultarNuevasSolicitudesTC()
        {
            int cantidadPendientes = 0;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    string query = @"SELECT COUNT(TC.Id) 
                                    FROM cj.OTC TC 
                                    INNER JOIN vt.ORTV VT ON VT.DocEntry = TC.DocEntryTicket
                                    WHERE (TC.MontoRecibidoDeposito > 0 AND (TC.TipoPago = 'PMI' OR TC.TipoPago = 'PCD') AND TC.Estado = 'PENDIENTE')
                                   AND (TC.MontoRecibidoEfectivo + TC.MontoRecibidoDeposito = VT.MontoFinal)
                                    ";
                    //Se agrego la ultima condicional solo caja visualiza  y atiende rapido si el pago es completo y por el contrario si es incompleto con deposito primero ventas coordina con el cliente para saber si pagara o no, Caja Valida buscando docnum y luego ventas Autoriza.

                    SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                    cn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        cantidadPendientes = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error al consultar nuevas solicitudes de TC: " + e.Message);
            }

            return cantidadPendientes;
        }

        public List<RptTicketACuadrar_E> ExportarExcelTicketsACuadrar(ORTV_E filtros = null)
        {
            List<RptTicketACuadrar_E> lista = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT TOP 100 TC.Id, TC.DocNumTicket, CONVERT(varchar, VT.FechaSapTicket, 103), REPLACE(VT.CardCode, 'C', ''), VT.CardName, VT.TipoVenta, VT.Vendedor, VT.FormaPago, VT.EstadoPago,");
                    sb.Append(" CASE TC.TipoPago WHEN 'PCE' THEN 'PAGO COMPLETO (EFECTIVO)' WHEN 'PMI' THEN 'PAGO MIXTO O INCOMPLETO' WHEN 'PCD' THEN 'PAGO COMPLETO (DEPÓSITO)' WHEN 'SP' THEN 'SIN PAGO' ELSE '' END AS DescTipoPago,");
                    sb.Append(" TC.Estado, TC.PersonaEntrega, TC.MontoRecibidoEfectivo, TC.MontoRecibidoDeposito, VT.MontoFinal, TC.ComportamientoPago");
                    sb.Append(" FROM cj.OTC TC");
                    sb.Append(" LEFT JOIN vt.ORTV VT ON VT.DocEntry = TC.DocEntryTicket");
                    sb.Append(" WHERE 1 = 1");

                    if (filtros != null)
                    {
                        if (filtros.DocNum > 0)
                        {
                            sb.Append(" AND VT.DocNum = @DocNum");
                            cmd.Parameters.AddWithValue("@DocNum", filtros.DocNum);
                        }

                        if (filtros.FechaSapTicket != null)
                        {
                            sb.Append(" AND VT.FechaSapTicket = @FechaSapTicket");
                            cmd.Parameters.AddWithValue("@FechaSapTicket", filtros.FechaSapTicket);
                        }

                        if (filtros.TipoVenta != null)
                        {
                            sb.Append(" AND VT.TipoVenta = @TipoVenta");
                            cmd.Parameters.AddWithValue("@TipoVenta", filtros.TipoVenta);
                        }

                        if (filtros.CardName != null)
                        {
                            sb.Append(" AND VT.CardName LIKE @CardName");
                            cmd.Parameters.AddWithValue("@CardName", string.Format("%{0}%", filtros.CardName));
                        }

                        if (filtros.Estado != null)
                        {
                            sb.Append(" AND VT.Estado = @EstadoTicket");
                            cmd.Parameters.AddWithValue("@EstadoTicket", filtros.Estado);
                        }

                        if (filtros.Vendedor != null)
                        {
                            sb.Append(" AND VT.Vendedor LIKE @Vendedor");
                            cmd.Parameters.AddWithValue("@Vendedor", string.Format("%{0}%", filtros.Vendedor));
                        }

                        if (filtros.MontoFinal > 0)
                        {
                            sb.Append(" AND VT.MontoFinal LIKE @MontoFinal");
                            cmd.Parameters.AddWithValue("@MontoFinal", string.Format("{0}%", filtros.MontoFinal));
                        }

                        if (filtros.LugarDestino != null)
                        {
                            sb.Append(" AND VT.LugarDestino = @LugarDestino");
                            cmd.Parameters.AddWithValue("@LugarDestino", filtros.LugarDestino);
                        }

                        if (filtros.EstadoPago != null)
                        {
                            sb.Append(" AND VT.EstadoPago = @EstadoPago");
                            cmd.Parameters.AddWithValue("@EstadoPago", filtros.EstadoPago);
                        }

                        if (filtros.EstadoGasto != null)
                        {
                            sb.Append(" AND VT.EstadoGasto = @EstadoGasto");
                            cmd.Parameters.AddWithValue("@EstadoGasto", filtros.EstadoGasto);
                        }

                        if (filtros.PagoEnv == 0.01M)
                        {
                            sb.Append(" AND VT.PagoEnv > @PagoEnv");
                            cmd.Parameters.AddWithValue("@PagoEnv", 0);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.TipoPago))
                        {
                            sb.Append(" AND TC.TipoPago = @TipoPago");
                            cmd.Parameters.AddWithValue("@TipoPago", filtros.TipoPago);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.EstadoContraEntrega))
                        {
                            sb.Append(" AND TC.Estado = @EstadoContraEntrega");
                            cmd.Parameters.AddWithValue("@EstadoContraEntrega", filtros.EstadoContraEntrega);
                        }
                    }

                    sb.Append($" ORDER BY TC.Id DESC");

                    cmd.CommandText = sb.ToString();
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            lista = new List<RptTicketACuadrar_E>();
                            while (dr.Read())
                            {
                                RptTicketACuadrar_E obj = new RptTicketACuadrar_E();
                                if (!dr.IsDBNull(1)) { obj.DocNumTicket = dr.GetInt32(1); }
                                if (!dr.IsDBNull(2)) { obj.FechaSapTicket = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.CardCode = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.CardName = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.TipoVenta = dr.GetString(5); }
                                if (!dr.IsDBNull(6)) { obj.Vendedor = dr.GetString(6); }
                                if (!dr.IsDBNull(7)) { obj.FormaPago = dr.GetString(7); }
                                if (!dr.IsDBNull(8)) { obj.EstadoPago = dr.GetString(8); }
                                if (!dr.IsDBNull(9)) { obj.TipoPago = dr.GetString(9); }
                                if (!dr.IsDBNull(10)) { obj.EstadoCuadre = dr.GetString(10); }
                                if (!dr.IsDBNull(11)) { obj.PersonaEntrega = dr.GetString(11); }
                                if (!dr.IsDBNull(12)) { obj.MontoRecibidoEfectivo = dr.GetDecimal(12); }
                                if (!dr.IsDBNull(13)) { obj.MontoRecibidoDeposito = dr.GetDecimal(13); }
                                if (!dr.IsDBNull(14)) { obj.MontoFinal = dr.GetDecimal(14); }
                                if (!dr.IsDBNull(15)) { obj.ComportamientoPago = dr.GetString(15); }

                                var datosCC = new CC_OTC_D().ObtenerFechaCobroFechaCuadre(dr.GetInt32(0));

                                obj.FechaCobro = datosCC != null && datosCC.ContainsKey("REGISTRAR") ? datosCC["REGISTRAR"].FechaOperacion : "";
                                obj.FechaCuadre = datosCC != null && datosCC.ContainsKey("CUADRAR") ? datosCC["CUADRAR"].FechaOperacion : "";
                                obj.PersonaRecepciona = datosCC != null && datosCC.ContainsKey("CUADRAR") ? datosCC["CUADRAR"].Operario : "";

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error al exportar los tickets a cuadrar: " + e.Message);
            }

            return lista;
        }
    }
}