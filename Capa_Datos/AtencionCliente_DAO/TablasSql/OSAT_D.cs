using Capa_Datos.Ventas_DAO.Tablas;
using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.AtencionCliente_ENT.ReportesSql;
using Capa_Entidad.AtencionCliente_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
namespace Capa_Datos.AtencionCliente_DAO.TablasSql
{
    public class OSAT_D // atenciones al cliente
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper(); SAT1_D sat1D = new SAT1_D();
        RTV1_D rtv1D = new RTV1_D();
        RTV2_D rtv2D = new RTV2_D();
        OINV_D oinvD = new OINV_D();
        Ventas_DAO.Tablas.ORDR_D ordrD = new Ventas_DAO.Tablas.ORDR_D();
        public List<OSAT_E> ListarSolicitudes(OSAT_E filtro, bool todos = false, bool fact = false)
        {
            List<OSAT_E> lista = new List<OSAT_E>();
            string fil = string.Empty, concatDocEntry = string.Empty, topSelect = string.Empty;
            if (filtro != null)
            {
                if (filtro.DocNum != null) { fil += " and AC.DocNum like '%" + filtro.DocNum + "%'"; }
                if (filtro.FechaRegistro != null)
                {
                    if (todos) { fil += $" and AC.FechaRegistro >= '{filtro.FechaRegistro}' "; }
                    else
                    {
                        fil += $" and AC.FechaRegistro = '{filtro.FechaRegistro}' ";
                    }
                }
                if (filtro.FechaAtencion != null)
                {
                    if (todos) { fil += $" and (SELECT TOP 1 FechaOperacion FROM ac.CC_OSAT where Operacion='ATENDER' and DocEntry=AC.DocEntry\r\n order by FechaOperacion,HoraOperacion desc) >= '{filtro.FechaAtencion}' "; }
                    else
                    {
                        fil += $" and (SELECT TOP 1 FechaOperacion FROM ac.CC_OSAT where Operacion='ATENDER' and DocEntry=AC.DocEntry\r\n order by FechaOperacion,HoraOperacion desc) = '{filtro.FechaAtencion}' ";
                    }
                }
                if (filtro.DocNumTicket > 0) { fil += " and AC.DocNumTicket=" + filtro.DocNumTicket; }
                if (filtro.DetORTV != null)
                {
                    if (filtro.DetORTV.ContainsKey("CardCode") && filtro.DetORTV["CardCode"] != "")
                    {
                        fil += $" AND VT.CardCode = '{filtro.DetORTV["CardCode"]}'";
                    }
                    if (filtro.DetORTV.ContainsKey("CardName") && filtro.DetORTV["CardName"] != "")
                    {
                        fil += " and VT.CardName like '%" + filtro.DetORTV["CardName"] + "%'";
                    }
                    if (filtro.DetORTV.ContainsKey("LugarDestino") && filtro.DetORTV["LugarDestino"] != "")
                    {
                        fil += " and VT.LugarDestino like '%" + filtro.DetORTV["LugarDestino"] + "%'";
                    }
                }
                if (filtro.Estado != null) { fil += $" and AC.Estado LIKE '%{filtro.Estado}%'"; }
                if (filtro.Resultado != null) { fil += " and AC.Resultado like '%" + filtro.Resultado + "%'"; }
                if (filtro.Tipo != null) { fil += $" AND AC.Tipo LIKE '%{filtro.Tipo}%'"; }
                // Filtros exclusivos para botón Reclamos de CreaTicketVenta
                if (filtro.TipoSolicitudCreaTicketVenta != null)
                {
                    fil += $" AND AC.Tipo IN {filtro.TipoSolicitudCreaTicketVenta}";
                }
                //
                if (filtro.Factor != null) { fil += " and AC.Factor  ='" + filtro.Factor + "'"; }
                if (filtro.TipoSolucion != null) { fil += " and AC.TipoSolucion in" + filtro.TipoSolucion; }
                if (filtro.DocFact != null && filtro.DocFact != "")
                {
                    List<SAT1_ComprobanteFin> arrDocEntry = sat1D.buscarComprobanteFin(filtro.DocFact);
                    foreach (var valor in arrDocEntry)
                    {
                        concatDocEntry += $"{valor.DocEntry},";
                    }
                    fil += $" AND DocEntry IN ({concatDocEntry.Remove(concatDocEntry.Length - 1)})";
                }
            }
            if (todos == false)
            {
                topSelect = "TOP 50";
            }

            string estadoFiltro = filtro?.Estado != null ? filtro.Estado.Replace("'", "''") : "";

            string select = $@"
            SELECT {topSelect}
                AC.DocEntry,
                AC.DocNum,
                CONVERT(varchar, AC.FechaRegistro, 23) AS FechaRegistro,
                AC.DocNumTicket,
                AC.Estado,
                AC.Resultado,
                AC.Tipo,
                AC.Factor,
                CASE
            {(string.IsNullOrEmpty(estadoFiltro) ? "" : $@"
            WHEN AC.Estado = '{estadoFiltro}' AND '{estadoFiltro}' IN('Registrado','Proceso','Atendido')
                THEN DATEDIFF(
                    DAY,
                    (SELECT TOP 1 FechaOperacion
                     FROM ac.CC_OSAT
                     WHERE DocEntry = AC.DocEntry
                       AND Operacion = 
                            CASE 
                                WHEN '{estadoFiltro}' = 'Registrado' THEN 'REGISTRAR'
                                WHEN '{estadoFiltro}' = 'Proceso' THEN 'PROCESAR'
                                WHEN '{estadoFiltro}' = 'Atendido' THEN 'ATENDER'
                                WHEN '{estadoFiltro}' = 'Culminado' THEN 'CULMINAR'
                                WHEN '{estadoFiltro}' = 'Anulado' THEN 'ANULAR'
                                ELSE '{estadoFiltro}'
                            END
                     ORDER BY FechaOperacion ASC),
                    GETDATE()
                )
            WHEN AC.Estado = '{estadoFiltro}' AND '{estadoFiltro}' ='Culminado'
            THEN DATEDIFF(
                    DAY,
                    (SELECT TOP 1 FechaOperacion
                     FROM ac.CC_OSAT
                     WHERE DocEntry = AC.DocEntry
                     AND Operacion = 'REGISTRAR'
                     ORDER BY FechaOperacion ASC),
                    (SELECT TOP 1 FechaOperacion
                     FROM ac.CC_OSAT
                     WHERE DocEntry = AC.DocEntry
                     AND Operacion = 'CULMINAR'
                     ORDER BY FechaOperacion ASC)
                )
            ")}
            WHEN AC.Tipo IN ('Reclamo', 'Devolucion') AND AC.Estado IN ('Registrado', 'Proceso', 'Atendido')
                THEN DATEDIFF(
                    DAY,
                    (SELECT TOP 1 FechaOperacion
                     FROM ac.CC_OSAT
                     WHERE DocEntry = AC.DocEntry AND Operacion = 'REGISTRAR'
                     ORDER BY FechaOperacion ASC),
                    (
                        SELECT CASE
                            WHEN EXISTS (
                                SELECT 1 FROM ac.CC_OSAT
                                WHERE DocEntry = AC.DocEntry AND Operacion = 'ANULAR'
                            )
                                THEN NULL
                            ELSE ISNULL(
                                (SELECT TOP 1 FechaOperacion
                                 FROM ac.CC_OSAT
                                 WHERE DocEntry = AC.DocEntry AND Operacion = 'CULMINAR'
                                 ORDER BY FechaOperacion DESC),
                                CONVERT(char(10), GETDATE(), 126)
                            )
                        END
                    )
                )
            ELSE NULL
        END AS DiasRetraso,
        AC.Solucion,
        AC.TipoSolucion,
        VT.LugarDestino,
        VT.CardName,
        VT.CardCode,
        (SELECT TOP 1 FechaOperacion
         FROM ac.CC_OSAT
         WHERE Operacion = 'ATENDER' AND DocEntry = AC.DocEntry
         ORDER BY FechaOperacion, HoraOperacion DESC) AS FechaAtencion,
        AC.TipoVenta,
        AC.CanalVenta
";

            string query = $"{select} FROM ac.OSAT AS AC LEFT JOIN vt.ORTV AS VT ON VT.DocNum = AC.DocNumTicket WHERE AC.DocEntry>0 {fil} ORDER BY AC.DocEntry DESC";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    OSAT_E objOSAT = new OSAT_E();
                    Dictionary<string, string> detORTV = new Dictionary<string, string>();
                    CC_OSAT_D ccOSAT_D = new CC_OSAT_D();
                    if (!dr.IsDBNull(0)) { objOSAT.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { objOSAT.DocNum = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { objOSAT.FechaRegistro = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { objOSAT.DocNumTicket = dr.GetInt32(3); }
                    if (!dr.IsDBNull(4)) { objOSAT.Estado = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { objOSAT.Resultado = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { objOSAT.Tipo = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { objOSAT.Factor = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { objOSAT.DiasRetraso = dr.GetInt32(8); }
                    if (!dr.IsDBNull(9)) { objOSAT.Solucion = dr.GetString(9); }
                    if (!dr.IsDBNull(10)) { objOSAT.TipoSolucion = dr.GetString(10); }
                    if (!dr.IsDBNull(11)) { detORTV.Add("LugarDestino", dr.GetString(11)); } else { detORTV.Add("LugarDestino", ""); }
                    if (!dr.IsDBNull(12)) { detORTV.Add("CardName", dr.GetString(12)); } else { detORTV.Add("CardName", ""); }
                    if (!dr.IsDBNull(13)) { detORTV.Add("CardCode", dr.GetString(13)); } else { detORTV.Add("CardCode", ""); }
                    if (!dr.IsDBNull(14)) { objOSAT.FechaAtencion = dr.GetDateTime(14).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(15)) { objOSAT.TipoVenta = dr.GetString(15); }
                    if (!dr.IsDBNull(16)) { objOSAT.CanalVenta = dr.GetString(16); }
                    objOSAT.DetORTV = detORTV;
                    List<CC_OSAT_E> DatosAtencion = ccOSAT_D.ListarCC_OSAT(dr.GetInt32(0), "ATENDER");
                    if (DatosAtencion[0].Operacion == "ATENDER" && !String.IsNullOrEmpty(objOSAT.Resultado))
                    {
                        objOSAT.FechaAtencion = DatosAtencion[0].FechaOperacion;
                    }
                    objOSAT.Det = sat1D.buscarDetallesSolicitud(objOSAT.DocEntry);
                    if (fact)
                    {
                        if (objOSAT.alMenos1Nc())
                        {
                            lista.Add(objOSAT);
                        }
                    }
                    else
                    {
                        lista.Add(objOSAT);
                    }
                }
                dr.Close();
            }
            catch (Exception e) { throw new Exception(e.Message); }
            return lista;
        }
        protected Dictionary<string, string> DatosSolicitud(string tipoVenta, string canalVenta, string errorAlm)
        {
            if (string.IsNullOrWhiteSpace(tipoVenta)) { tipoVenta = ""; }
            if (string.IsNullOrWhiteSpace(canalVenta)) { canalVenta = ""; }
            if (string.IsNullOrWhiteSpace(errorAlm)) { errorAlm = ""; }
            Dictionary<string, string> opcionesTipoVenta = new Dictionary<string, string>
                {
                    { "", ""},
                    { "VCALLC", "Ventas Call Center"},
                    { "VHORIZ", "Ventas Horizontal"},
                    { "VESTRAT", "Ventas Estratégicas"}
                };
            Dictionary<string, string> opcionesCanalVenta = new Dictionary<string, string>
                {
                    { "", ""},
                    { "LIMA", "Lima"},
                    { "PROV", "Provincia"},
                    { "LIC", "Licitación"},
                    { "CAD", "Cadena"},
                    { "PROMLIMA", "Promotoría Lima"},
                    { "PROMPROV", "Promotoría Provincia"},
                    { "TELEV", "Televentas"},
                    { "CENTRO", "Centro"}
                };
            Dictionary<string, string> opcionesErrorAlmacen = new Dictionary<string, string>
                {
                    { "", ""},
                    { "ARECEP3", "Área de Recepción 3"},
                    { "ARECEP6", "Área de Recepción 6"},
                    { "ARECEP8", "Área de Recepción 8"},
                    { "ADESP", "Área de Despacho"},
                    { "AVERIF", "Área de Verificación"},
                    { "AEMB", "Área de Embalaje"},
                    { "APICK", "Área de Picking"},
                    { "AFACT", "Área de Facturación"},
                    { "AING", "Área de Ingreso"},
                    { "OTROS", "OTROS"}
                };
            Dictionary<string, string> result = new Dictionary<string, string>
                {
                    {"TipoVenta", opcionesTipoVenta[tipoVenta]},
                    {"CanalVenta", opcionesCanalVenta[canalVenta]},
                    {"ErrorAlmacen", opcionesErrorAlmacen[errorAlm]}
                };
            return result;
        }
        public List<Rpt_OSAT_E> ListarSolicitudesExcel(OSAT_E filtro)
        {
            List<Rpt_OSAT_E> lista = new List<Rpt_OSAT_E>();
            string condWhere = string.Empty, concatDocEntry = string.Empty;
            if (filtro != null)
            {
                if (filtro.DocNum != null) { condWhere += " and AC.DocNum like '%" + filtro.DocNum + "%'"; }
                if (filtro.FechaRegistro != null)
                {
                    condWhere += $" and AC.FechaRegistro >= '{filtro.FechaRegistro}' ";
                }
                if (filtro.FechaAtencion != null)
                {
                    condWhere += $" and (SELECT TOP 1 FechaOperacion FROM ac.CC_OSAT where Operacion='ATENDER' and DocEntry=AC.DocEntry\r\n order by FechaOperacion,HoraOperacion desc) >= '{filtro.FechaAtencion}' ";
                }
                if (filtro.DocNumTicket > 0) { condWhere += " and AC.DocNumTicket=" + filtro.DocNumTicket; }
                if (filtro.DetORTV != null)
                {
                    if (filtro.DetORTV.ContainsKey("CardCode") && filtro.DetORTV["CardCode"] != "")
                    {
                        condWhere += $" AND VT.CardCode = '{filtro.DetORTV["CardCode"]}'";
                    }
                    if (filtro.DetORTV.ContainsKey("CardName") && filtro.DetORTV["CardName"] != "")
                    {
                        condWhere += " and VT.CardName like '%" + filtro.DetORTV["CardName"] + "%'";
                    }
                    if (filtro.DetORTV.ContainsKey("LugarDestino") && filtro.DetORTV["LugarDestino"] != "")
                    {
                        condWhere += " and VT.LugarDestino like '%" + filtro.DetORTV["LugarDestino"] + "%'";
                    }
                }
                if (filtro.Estado != null) { condWhere += $" and AC.Estado LIKE '%{filtro.Estado}%'"; }
                if (filtro.Resultado != null) { condWhere += " and AC.Resultado like '%" + filtro.Resultado + "%'"; }
                if (filtro.Tipo != null) { condWhere += $" AND AC.Tipo LIKE '%{filtro.Tipo}%'"; }
                // Filtros exclusivos para botón Reclamos de CreaTicketVenta
                if (filtro.TipoSolicitudCreaTicketVenta != null)
                {
                    condWhere += $" AND AC.Tipo IN {filtro.TipoSolicitudCreaTicketVenta}";
                }
                //
                if (filtro.Factor != null) { condWhere += " and AC.Factor  ='" + filtro.Factor + "'"; }
                if (filtro.TipoSolucion != null) { condWhere += " and AC.TipoSolucion in" + filtro.TipoSolucion; }
                if (filtro.DocFact != null && filtro.DocFact != "")
                {
                    List<SAT1_ComprobanteFin> arrDocEntry = sat1D.buscarComprobanteFin(filtro.DocFact);
                    foreach (var valor in arrDocEntry)
                    {
                        concatDocEntry += $"{valor.DocEntry},";
                    }
                    condWhere += $" AND DocEntry IN ({concatDocEntry.Remove(concatDocEntry.Length - 1)})";
                }
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT AC.DocEntry, AC.DocNum, AC.DocNumTicket, AC.Estado, AC.Factor, CONVERT(varchar,AC.FechaRegistro,23) AS FechaRegistro, AC.Resultado, AC.Solucion, AC.FechaFacturacion, AC.TipoVenta, AC.CanalVenta");
            sb.Append(" ,VT.LugarDestino, VT.CardName, (SELECT TOP 1 FechaOperacion FROM ac.CC_OSAT WHERE Operacion='ATENDER' AND DocEntry=AC.DocEntry ORDER BY FechaOperacion DESC, HoraOperacion DESC) AS FechaAtencion");
            sb.Append(" ,DET.Problema, DET.TipoError, DET.OpResponsable, DET.Comentario, DET.ErrorAlmacen, DET.NCSAP, DET.ItemCode, DET.Dscription, DET.BatchNum, DET.ExpDate, DET.UnitMsrF, DET.QuantityF, DET.LineTotalF");
            sb.Append(" ,(SELECT TOP 1 FechaOperacion FROM ac.CC_OSAT WHERE Operacion='PROCESAR' AND DocEntry=AC.DocEntry ORDER BY FechaOperacion DESC, HoraOperacion DESC) AS FecProceso");
            sb.Append(" ,(SELECT TOP 1 FechaOperacion FROM ac.CC_OSAT WHERE Operacion='CULMINAR' AND DocEntry=AC.DocEntry ORDER BY FechaOperacion DESC, HoraOperacion DESC) AS FecCulminado");
            sb.Append(",AC.TipoSolucion");
            sb.Append(" FROM ac.OSAT AC");
            sb.Append(" LEFT JOIN vt.ORTV VT ON VT.DocNum = AC.DocNumTicket");
            sb.Append(" INNER JOIN ac.SAT1 DET ON DET.DocEntry = AC.DocEntry");
            sb.Append($" WHERE AC.DocEntry > 0 {condWhere} ORDER BY AC.DocEntry DESC");
            string query = sb.ToString();
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    Rpt_OSAT_E rpt = new Rpt_OSAT_E();
                    Dictionary<string, string> detORTV = new Dictionary<string, string>();
                    CC_OSAT_D ccOSAT_D = new CC_OSAT_D();
                    var datos = DatosSolicitud((!dr.IsDBNull(9)) ? dr.GetString(9) : "", (!dr.IsDBNull(10)) ? dr.GetString(10) : "", (!dr.IsDBNull(18)) ? dr.GetString(18) : "");
                    if (!dr.IsDBNull(1)) { rpt.DocNum = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { rpt.DocNumTicket = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { rpt.Estado = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { rpt.Factor = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { rpt.FechaRegistro = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { rpt.Resultado = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { rpt.Solucion = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { rpt.FechaFacturacion = dr.GetDateTime(8).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(9)) { rpt.TipoVenta = datos["TipoVenta"]; }
                    if (!dr.IsDBNull(10)) { rpt.CanalVenta = datos["CanalVenta"]; }
                    if (!dr.IsDBNull(11)) { rpt.LugarDestino = dr.GetString(11); }
                    if (!dr.IsDBNull(12)) { rpt.CardName = dr.GetString(12); }
                    if (!dr.IsDBNull(13)) { rpt.FechaAtencion = dr.GetDateTime(13).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(14)) { rpt.Problema = dr.GetString(14); }
                    if (!dr.IsDBNull(15)) { rpt.TipoError = dr.GetString(15); }
                    if (!dr.IsDBNull(16)) { rpt.OpResponsable = dr.GetString(16); }
                    if (!dr.IsDBNull(17)) { rpt.Comentario = dr.GetString(17); }
                    rpt.ErrorAlmacen = datos["ErrorAlmacen"];
                    if (!dr.IsDBNull(19)) { rpt.NCSAP = dr.GetInt32(19); }
                    if (!dr.IsDBNull(20)) { rpt.ItemCode = dr.GetString(20); }
                    if (!dr.IsDBNull(21)) { rpt.Dscription = dr.GetString(21); }
                    if (!dr.IsDBNull(22)) { rpt.BatchNum = dr.GetString(22); }
                    if (!dr.IsDBNull(23)) { rpt.ExpDate = dr.GetString(23); }
                    if (!dr.IsDBNull(24)) { rpt.UnitMsrF = dr.GetString(24); }
                    if (!dr.IsDBNull(25)) { rpt.QuantityF = dr.GetDecimal(25); }
                    if (!dr.IsDBNull(26)) { rpt.Total = dr.GetDecimal(26); }
                    if (!dr.IsDBNull(27)) { rpt.FechaProceso = dr.GetDateTime(27).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(28)) { rpt.FechaCulminado = dr.GetDateTime(28).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(29)) { rpt.TipoSolucion = dr.GetString(29); }
                    lista.Add(rpt);
                }
                dr.Close();
            }
            catch (Exception e) { throw new Exception(e.Message); }
            return lista;
        }
        public string registrarNuevaSolicitud(OSAT_E obj)
        {
            int status = -1;
            string rutaDirectorio = uti.directorioFileServer + "AtencionAlCliente_2023";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("ac.MANT_OSAT", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "A");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@DocNum", obj.DocNum);
                    cmd.Parameters.AddWithValue("@Estado", obj.Estado);
                    cmd.Parameters.AddWithValue("@Tipo", obj.Tipo);
                    cmd.Parameters.AddWithValue("@DocNumTicket", obj.DocNumTicket);
                    cmd.Parameters.AddWithValue("@DocEntryTicket", obj.DocEntryTicket);
                    cmd.Parameters.AddWithValue("@Factor", obj.Factor);
                    cmd.Parameters.AddWithValue("@Contacto", obj.Contacto);
                    cmd.Parameters.AddWithValue("@Telefono", obj.Telefono);
                    cmd.Parameters.AddWithValue("@Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("@DireccionRecojo", obj.DireccionRecojo);
                    cmd.Parameters.AddWithValue("@OpRegistro", obj.OpRegistro);     // Para la tabla OSAT
                    cmd.Parameters.AddWithValue("@Operario", obj.OpRegistro);       // Para el control de cambios
                    cmd.Parameters.AddWithValue("@UrlArchivo", obj.UrlArchivo);
                    cmd.Parameters.AddWithValue("@FechaFacturacion", obj.FechaFacturacion);
                    cmd.Parameters.AddWithValue("@TipoVenta", obj.TipoVenta);
                    cmd.Parameters.AddWithValue("@CanalVenta", obj.CanalVenta);
                    SqlParameter tbDet = new SqlParameter("@TPSAT1", SqlDbType.Structured);
                    tbDet.Value = SAT1_E.tbDetalle(obj.Det);
                    tbDet.TypeName = "ac.TPSAT1";
                    cmd.Parameters.AddWithValue("@TPSAT1", tbDet.Value);
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    //post transacciones
                    SqlCommand cmd2 = new SqlCommand("POST_TRANSACCIONES", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "OSAT");
                    cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@DocEntry"].Value);
                    cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@DocEntry"].Value);
                    cmd2.ExecuteNonQuery();
                    if (obj.Archivo != null)
                    {
                        if (obj.Archivo.Count >= 1 && obj.Archivo[0] != null)
                        {
                            SubirAdjuntos(rutaDirectorio, status, obj.Archivo, obj.OpRegistro);
                        }
                    }
                    tran.Commit();
                }
                catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: " + e.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return obj.DocNum;
        }
        public string anularSolicitud(OSAT_E obj)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("ac.MANT_OSAT", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "AS");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Operario", obj.OpRegistro);   // Para el control de cambios - Usuario en sesiön
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en anulacion: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en anulacion y conexion: " + e2.Message); }
            return obj.DocNum;
        }
        public OSAT_E buscarSolicitud(int DocEntry)
        {
            OSAT_E objOSAT = null;
            string select = "SELECT AC.DocEntry, AC.DocNum, AC.Tipo, AC.DocNumTicket, AC.DocEntryTicket, AC.Estado, AC.Factor, AC.Contacto, AC.Telefono, AC.Correo, AC.DireccionRecojo, CONVERT(varchar,AC.FechaRegistro,23) AS FechaRegistro, " +
                                    "AC.HoraRegistro, AC.OpRegistro, AC.UrlArchivo, AC.Resultado, AC.Solucion, AC.TipoSolucion, AC.FechaFacturacion, VT.LugarDestino, VT.CardName, VT.CardCode, AC.TipoVenta, AC.CanalVenta, AC.NotiCliente";
            string query = $"{select} FROM ac.OSAT AS AC LEFT JOIN vt.ORTV AS VT ON VT.DocNum = AC.DocNumTicket WHERE AC.DocEntry=@DocEntry ORDER BY AC.DocEntry DESC";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string>() { "@DocEntry" }, DocEntry);
                dr.Read();
                objOSAT = new OSAT_E();
                Dictionary<string, string> detORTV = new Dictionary<string, string>();
                if (!dr.IsDBNull(0)) { objOSAT.DocEntry = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { objOSAT.DocNum = dr.GetString(1); }
                if (!dr.IsDBNull(2)) { objOSAT.Tipo = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { objOSAT.DocNumTicket = dr.GetInt32(3); }
                if (!dr.IsDBNull(4)) { objOSAT.DocEntryTicket = dr.GetInt32(4); }
                if (!dr.IsDBNull(5)) { objOSAT.Estado = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { objOSAT.Factor = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { objOSAT.Contacto = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { objOSAT.Telefono = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { objOSAT.Correo = dr.GetString(9); }
                if (!dr.IsDBNull(10)) { objOSAT.DireccionRecojo = dr.GetString(10); }
                if (!dr.IsDBNull(11)) { objOSAT.FechaRegistro = dr.GetString(11); }
                if (!dr.IsDBNull(12)) { objOSAT.HoraRegistro = dr.GetTimeSpan(12).ToString(); }
                if (!dr.IsDBNull(13)) { objOSAT.OpRegistro = dr.GetString(13); }
                if (!dr.IsDBNull(14)) { objOSAT.UrlArchivo = dr.GetString(14); }
                if (!dr.IsDBNull(15)) { objOSAT.Resultado = dr.GetString(15); }
                if (!dr.IsDBNull(16)) { objOSAT.Solucion = dr.GetString(16); }
                if (!dr.IsDBNull(17)) { objOSAT.TipoSolucion = dr.GetString(17); }
                if (!dr.IsDBNull(18)) { objOSAT.FechaFacturacion = dr.GetDateTime(18).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(19)) { detORTV.Add("LugarDestino", dr.GetString(19)); } else { detORTV.Add("LugarDestino", ""); }
                if (!dr.IsDBNull(20)) { detORTV.Add("CardName", dr.GetString(20)); } else { detORTV.Add("CardName", ""); }
                if (!dr.IsDBNull(21)) { detORTV.Add("CardCode", dr.GetString(21)); } else { detORTV.Add("CardCode", ""); }
                if (!dr.IsDBNull(22)) { objOSAT.TipoVenta = dr.GetString(22); }
                if (!dr.IsDBNull(23)) { objOSAT.CanalVenta = dr.GetString(23); }
                if (!dr.IsDBNull(24)) { objOSAT.NotiCliente = dr.GetBoolean(24) ? 1 : 0; }
                objOSAT.DetORTV = detORTV;
                objOSAT.Det = sat1D.buscarDetallesSolicitud(DocEntry);
                foreach (SAT1_E sat1 in objOSAT.Det)
                {
                    sat1.ComprobantesVinculados = new List<string>();
                    foreach (RTV2_E bean in rtv2D.BuscarRTV2(objOSAT.DocEntryTicket))
                    {
                        foreach (OINV_E bean2 in oinvD.listadoComprobantesPorOrdrArticulo(ordrD.buscarDocEntry(bean.NroSap), sat1.ItemCode))
                        {
                            sat1.ComprobantesVinculados.Add(bean2.NumAtCard);
                        }
                    }
                }
                dr.Close();
            }
            catch (Exception e) { throw new Exception(e.Message); }
            return objOSAT;
        }
        public string editarSolicitud(OSAT_E obj)
        {
            int status = -1;
            string rutaDirectorio = uti.directorioFileServer + "AtencionAlCliente_2023";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("ac.MANT_OSAT", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "U");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Tipo", obj.Tipo);
                    cmd.Parameters.AddWithValue("@Factor", obj.Factor);
                    cmd.Parameters.AddWithValue("@Contacto", obj.Contacto);
                    cmd.Parameters.AddWithValue("@Telefono", obj.Telefono);
                    cmd.Parameters.AddWithValue("@Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("@DireccionRecojo", obj.DireccionRecojo);
                    cmd.Parameters.AddWithValue("@UrlArchivo", obj.UrlArchivo);
                    cmd.Parameters.AddWithValue("@Operario", obj.OpRegistro);       // Para el control de cambios
                    cmd.Parameters.AddWithValue("@TipoVenta", obj.TipoVenta);
                    cmd.Parameters.AddWithValue("@CanalVenta", obj.CanalVenta);
                    SqlParameter tbDet = new SqlParameter("@TPSAT1", SqlDbType.Structured);
                    tbDet.Value = SAT1_E.tbDetalle(obj.Det);
                    tbDet.TypeName = "ac.TPSAT1";
                    cmd.Parameters.AddWithValue("@TPSAT1", tbDet.Value);
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    if (obj.Archivo != null)
                    {
                        if (obj.Archivo.Count >= 1 && obj.Archivo[0] != null)
                        {
                            SubirAdjuntos(rutaDirectorio, status, obj.Archivo, obj.OpRegistro);
                        }
                    }
                    tran.Commit();
                }
                catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: " + e.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return obj.DocNum;
        }
        public string procesarSolicitud(OSAT_E obj)
        {
            int status = -1;
            string rutaDirectorio = uti.directorioFileServer + "AtencionAlCliente_2023";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("ac.MANT_OSAT", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UP");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Tipo", obj.Tipo);
                    cmd.Parameters.AddWithValue("@Factor", obj.Factor);
                    cmd.Parameters.AddWithValue("@Contacto", obj.Contacto);
                    cmd.Parameters.AddWithValue("@Telefono", obj.Telefono);
                    cmd.Parameters.AddWithValue("@Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("@NotiCliente", obj.NotiCliente);
                    cmd.Parameters.AddWithValue("@DireccionRecojo", obj.DireccionRecojo);
                    cmd.Parameters.AddWithValue("@UrlArchivo", obj.UrlArchivo);
                    cmd.Parameters.AddWithValue("@Operario", obj.OpRegistro);   // Para el control de cambios - Usuario en sesiön
                    SqlParameter tbDet = new SqlParameter("@TPSAT1", SqlDbType.Structured);
                    tbDet.Value = SAT1_E.tbDetalle(obj.Det);
                    tbDet.TypeName = "ac.TPSAT1";
                    cmd.Parameters.AddWithValue("@TPSAT1", tbDet.Value);
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    if (obj.Archivo != null)
                    {
                        if (obj.Archivo.Count >= 1)
                        {
                            SubirAdjuntos(rutaDirectorio, status, obj.Archivo, obj.OpRegistro);
                        }
                    }
                    tran.Commit();
                }
                catch (Exception e1) { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: " + e1.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return obj.DocNum;
        }
        public string revertirProcesarSolicitud(OSAT_E obj)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("ac.MANT_OSAT", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "RUP");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Operario", obj.OpRegistro);   // Para el control de cambios - Usuario en sesiön
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return obj.DocNum;
        }
        public string atenderSolicitud(OSAT_E obj)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("ac.MANT_OSAT", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UA");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Resultado", obj.Resultado);
                    cmd.Parameters.AddWithValue("@Solucion", obj.Solucion);
                    cmd.Parameters.AddWithValue("@Factor", obj.Factor);
                    cmd.Parameters.AddWithValue("@TipoSolucion", obj.TipoSolucion);
                    cmd.Parameters.AddWithValue("@Operario", obj.OpRegistro);   // Para el control de cambios - Usuario en sesiön
                    SqlParameter tbDet = new SqlParameter("@TPSAT1", SqlDbType.Structured);
                    tbDet.Value = SAT1_E.tbDetalle(obj.Det);
                    tbDet.TypeName = "ac.TPSAT1";
                    cmd.Parameters.AddWithValue("@TPSAT1", tbDet.Value);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return obj.DocNum;
        }
        public string revertirAtenderSolicitud(OSAT_E obj)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("ac.MANT_OSAT", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "RUA");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Operario", obj.OpRegistro);   // Para el control de cambios - Usuario en sesiön
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en edicion: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en edicion y conexion: " + e2.Message); }
            return obj.DocNum;
        }
        public string culminarSolicitud(OSAT_E obj)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("ac.MANT_OSAT", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UC");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Operario", obj.OpRegistro);   // Para el control de cambios - Usuario en sesiön
                    SqlParameter tbDet = new SqlParameter("@TPSAT1", SqlDbType.Structured);
                    tbDet.Value = SAT1_E.tbDetalle(obj.Det);
                    tbDet.TypeName = "ac.TPSAT1";
                    cmd.Parameters.AddWithValue("@TPSAT1", tbDet.Value);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en culminacion: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en culminacion y conexion: " + e2.Message); }
            return obj.DocNum;
        }
        public string revertirCulminarSolicitud(OSAT_E obj)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("ac.MANT_OSAT", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "RUC");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Operario", obj.OpRegistro);   // Para el control de cambios - Usuario en sesiön
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en revertir: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en revertir y conexion: " + e2.Message); }
            return obj.DocNum;
        }
        public string ObtenerNroSolicitud(string Tipo)
        {
            if (Tipo == null || Tipo == "") { return ""; }
            string nro = "";
            int DocEntry = 0;
            string query = "select DocEntry from gene where Tabla='OSAT'";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                dr.Read();
                if (!dr.IsDBNull(0)) { DocEntry = dr.GetInt32(0); nro = DocEntry.ToString(); }
                dr.Close();
                while (nro.Length < 4)
                {
                    nro = "0" + nro;
                }
                if (Tipo == "Reclamo") { nro = "REC" + nro + "-" + DateTime.Now.Year.ToString().Substring(2, 2); }
                else if (Tipo == "Devolucion") { nro = "DEV" + nro + "-" + DateTime.Now.Year.ToString().Substring(2, 2); }
                else if (Tipo == "Solicitud interna") { nro = "SIN" + nro + "-" + DateTime.Now.Year.ToString().Substring(2, 2); }
            }
            catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
            return nro;
        }
        public OSAT_E BuscarDatosTicket(int DocNumTicket)
        {
            OSAT_E objOSAT = new OSAT_E();
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                String query = $"SELECT DocEntry, CardCode, CardName, LugarDestino, Vendedor, FechaFacturacion, DirDestino FROM vt.ORTV WHERE DocNum = @DocNumTicket";
                SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
                cmd.Parameters.AddWithValue("@DocNumTicket", DocNumTicket);
                cn.Open();
                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        // Validamos en si el DocEntry no sea "null" ya que es prioridad para obtener datos de RTV1
                        if (!dr.IsDBNull(0))
                        {
                            List<RTV1_E> datosRTV1 = new List<RTV1_E>();
                            ORTV_E datosORTV = new ORTV_E();
                            Dictionary<string, string> DetORTV = new Dictionary<string, string>();
                            objOSAT.DocEntryTicket = dr.GetInt32(0);
                            objOSAT.DocNumTicket = DocNumTicket;
                            datosRTV1 = rtv1D.BuscarRTV1(dr.GetInt32(0));
                            if (datosRTV1.Count >= 1)
                            {
                                objOSAT.Contacto = (!string.IsNullOrWhiteSpace(datosRTV1[0].NombrePer)) ? datosRTV1[0].NombrePer : "";
                                objOSAT.Telefono = (!string.IsNullOrWhiteSpace(datosRTV1[0].TelfPer)) ? datosRTV1[0].TelfPer : "";
                            }
                            if (!dr.IsDBNull(1)) { DetORTV.Add("CardCode", dr.GetString(1)); }
                            if (!dr.IsDBNull(2)) { DetORTV.Add("CardName", dr.GetString(2)); }
                            if (!dr.IsDBNull(3)) { DetORTV.Add("LugarDestino", dr.GetString(3)); }
                            if (!dr.IsDBNull(4)) { DetORTV.Add("Vendedor", dr.GetString(4)); }
                            if (!dr.IsDBNull(5)) { objOSAT.FechaFacturacion = dr.GetDateTime(5).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(6)) { objOSAT.DireccionRecojo = dr.GetString(6); }
                            objOSAT.Det = sat1D.ListarArticulosTicket(DocNumTicket);
                            objOSAT.DetORTV = DetORTV;
                        }
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception("Error: " + e.Message);
                }
                cn.Close();
            }
            return objOSAT;
        }
        /*
         * Método para subir adjuntos al gestionar la solicitud AC
         */
        void SubirAdjuntos(string rutaDirectorio, int DocEntry, List<HttpPostedFileBase> archivos, string OpRegistro)
        {
            int linea = UltimaLineaAdjuntosOSAT(DocEntry);
            if (!Directory.Exists(rutaDirectorio + @"\" + DocEntry))                            // Revisar si existe el directorio
            {
                Directory.CreateDirectory(rutaDirectorio + @"\" + DocEntry);            // creamos
            }
            if (archivos != null && archivos.Count >= 1)
            {
                foreach (var arch in archivos)
                {
                    arch.SaveAs(rutaDirectorio + @"\" + DocEntry + @"\" + arch.FileName);
                    using (SqlConnection cn2 = new SqlConnection(uti.cadSql))
                    {
                        string query = "INSERT INTO ac.SAT11 VALUES (@DocEntry, @Linea, @NombreArchivo, @OpRegistro, @FechaCreacion,@HoraCreacion)";
                        SqlCommand cmd2 = new SqlCommand(query, cn2);
                        cmd2.Parameters.AddWithValue("@DocEntry", DocEntry);
                        cmd2.Parameters.AddWithValue("@Linea", linea);
                        cmd2.Parameters.AddWithValue("@NombreArchivo", arch.FileName);
                        cmd2.Parameters.AddWithValue("@OpRegistro", OpRegistro);
                        cmd2.Parameters.AddWithValue("@FechaCreacion", DateTime.Now.ToShortDateString());
                        cmd2.Parameters.AddWithValue("@HoraCreacion", DateTime.Now.TimeOfDay);
                        cn2.Open();
                        try
                        {
                            cmd2.ExecuteNonQuery();
                        }
                        catch (Exception e1)
                        {
                            throw new Exception("Error: " + e1.Message);
                        }
                        cn2.Close();
                    }
                    ++linea;
                }
            }
        }
        public Dictionary<int, string> BuscarAdjuntosOSAT(int docEntry, int linea)
        {
            Dictionary<int, string> lista = new Dictionary<int, string>();
            string condWhere = "";
            if (linea > 0)
            {
                condWhere = " AND Linea = @Linea";
            }
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = $"SELECT NombreArchivo, Linea FROM ac.SAT11 WHERE DocEntry = @DocEntry {condWhere}";
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocEntry", docEntry);
                if (linea > 0)
                {
                    cmd.Parameters.AddWithValue("@Linea", linea);
                }
                cn.Open();
                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            lista.Add(dr.GetInt32(1), $"{dr.GetString(0)}");
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
        public int UltimaLineaAdjuntosOSAT(int docEntry)
        {
            int ultimaLinea = 1;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT MAX(Linea)+1 FROM ac.SAT11 WHERE DocEntry = @DocEntry";
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocEntry", docEntry);
                cn.Open();
                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta
                    dr.Read();
                    if (!dr.IsDBNull(0))
                    {
                        ultimaLinea = dr.GetInt32(0);
                    }
                    else
                    {
                        ultimaLinea = 1;
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                cn.Close();
            }
            return ultimaLinea;
        }
        public List<OSAT_E> obtenerNotificadoCliente()
        {
            var resultado = new List<OSAT_E>();
            string query = @"SELECT DISTINCT CardName, COUNT(*) AS TicketsAbiertos
                     FROM vt.ORTV 
                     WHERE LTRIM(RTRIM(UPPER(CardName))) IN (
                         SELECT LTRIM(RTRIM(UPPER(Contacto)))
                         FROM ac.OSAT
                         WHERE Estado IN ('Registrado','Proceso','Atendido')
                         AND Tipo IN ('Reclamo','Devolucion') AND NotiCliente = 1
                     )
                     AND Estado IN ('RECIBIDO','PICKEANDO','VERIFICANDO','EMPACANDO','EMPACADO') 
                     GROUP BY CardName";
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        resultado.Add(new OSAT_E
                        {
                            CardName = dr.IsDBNull(0) ? "" : dr.GetString(0),
                            TicketsAbiertos = dr.IsDBNull(1) ? 0 : dr.GetInt32(1)
                        });
                    }
                }
                cn.Close();
            }
            return resultado;
        }


        public List<OSAT_E> obtenerNotificadoClienteDetalle(string cardName)
        {
            var resultado = new List<OSAT_E>();
            string query = @"
        SELECT 
            FechaSapTicket,
            DocNum,
            Estado,
            Vendedor,
            CardName
            FROM vt.ORTV
        WHERE LTRIM(RTRIM(UPPER(CardName))) IN (
            SELECT LTRIM(RTRIM(UPPER(Contacto)))
            FROM ac.OSAT
            WHERE Estado IN ('Registrado','Proceso','Atendido')
            AND Tipo IN ('Reclamo','Devolucion') AND NotiCliente = 1
        )
        AND Estado IN ('RECIBIDO','PICKEANDO','VERIFICANDO','EMPACANDO','EMPACADO')
        AND CardName = @CardName";
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@CardName", cardName);
                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var dto = new OSAT_E
                        {
                            FechaSapTicket = dr.IsDBNull(0) ? "" : dr.GetDateTime(0).ToString("yyyy-MM-dd"),
                            DocNumVt = dr.IsDBNull(1) ? 0 : dr.GetInt32(1),
                            EstadoVt = dr.IsDBNull(2) ? "" : dr.GetString(2),
                            Vendedor = dr.IsDBNull(3) ? "" : dr.GetString(3),
                            CardName = dr.IsDBNull(4) ? "" : dr.GetString(4)
                        };
                        resultado.Add(dto);
                    }
                }
                cn.Close();
            }
            return resultado;
        }
    }
}