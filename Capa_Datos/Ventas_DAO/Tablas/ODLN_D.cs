using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.Tablas;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;

namespace Capa_Datos.Ventas_DAO.Tablas
{
    public class ODLN_D
    {
        DBHelper db = new DBHelper(); Utilitarios uti = new Utilitarios();

        public List<ODLN_E> listarEntregasVenta(ODLN_E fil)
        {
            List<ODLN_E> lista = new List<ODLN_E>();
            string condWhere = string.Empty;
            if (fil != null)
            {
                if (fil.DocNum > 0) { condWhere += " and \"DocNum\" like '%" + fil.DocNum + "'"; }
                if (fil.DocDate != null) { condWhere += " and \"DocDate\"='" + fil.DocDate + "'"; }
                if (fil.CardName != null) { condWhere += " and UPPER(\"CardName\") like UPPER('%" + fil.CardName + "%')"; }
                if (fil.NumAtCard != null) { condWhere += " and UPPER(\"NumAtCard\") like UPPER('%" + fil.NumAtCard + "')"; }
                if (fil.DocTotal > 0.00M) { condWhere += " and \"DocTotal\" like '%" + fil.DocTotal + "%'"; }
                if (fil.U_BPP_FECINITRA != null) { condWhere += " and \"U_BPP_FECINITRA\"='" + fil.U_BPP_FECINITRA + "'"; }
                if (fil.U_SYP_STATUS != null) { condWhere += " and UPPER(\"U_SYP_STATUS\")=UPPER('" + fil.U_SYP_STATUS + "')"; }
                if (fil.U_COB_LUGAREN != null) { condWhere += " and UPPER(\"U_COB_LUGAREN\") like UPPER('%" + fil.U_COB_LUGAREN + "')"; }
            }
            string query = "select top 50 \"DocEntry\",\"DocNum\",\"DocDate\",\"CardName\",\"NumAtCard\",\"DocTotal\",\"U_SYP_STATUS\",\"U_COB_LUGAREN\",\"U_BPP_FECINITRA\" " +
                $" from {uti.schemaHana}odln where \"DocEntry\">0 {condWhere} ORDER BY 1 DESC";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    ODLN_E o = new ODLN_E();
                    if (!hdr.IsDBNull(0)) { o.DocEntry = hdr.GetInt32(0); }
                    if (!hdr.IsDBNull(1)) { o.DocNum = hdr.GetInt32(1); }
                    if (!hdr.IsDBNull(2)) { o.DocDate = hdr.GetDateTime(2).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(3)) { o.CardName = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { o.NumAtCard = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { o.DocTotal = Math.Round(hdr.GetDecimal(5), 2); }
                    if (!hdr.IsDBNull(6)) { o.U_SYP_STATUS = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { o.U_COB_LUGAREN = hdr.GetString(7); }
                    if (!hdr.IsDBNull(8))
                    {
                        o.U_BPP_FECINITRA = hdr.GetDateTime(8).ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        o.U_BPP_FECINITRA = o.DocDate;
                    }

                    lista.Add(o);
                }
                hdr.Close();
            }
            catch (Exception e) { throw new Exception("Error: " + e.Message); }
            return lista;
        }
        public string buscarGuiasRemision(int DocEntryVenta)
        {
            string guias = "";
            string query = "select t0.\"NumAtCard\" from " + uti.schemaHana + "odln t0 " +
                            " inner join " + uti.schemaHana + "dln1 t1 on t1.\"DocEntry\"=t0.\"DocEntry\"" +
                            " inner join " + uti.schemaHana + "rdr1 t2 on t2.\"DocEntry\"=t1.\"BaseEntry\" and t2.\"ObjType\"=t1.\"BaseType\" " +
                                                    " and t2.\"ItemCode\"=t1.\"ItemCode\" where t0.\"CANCELED\"='N' and t2.\"DocEntry\"=" + DocEntryVenta
                                                    + " group by t0.\"NumAtCard\"";
            HanaConnection cn = new HanaConnection(uti.cadHana);
            try
            {
                cn.Open();
                HanaCommand cmd = new HanaCommand(query, cn);
                cmd.CommandType = System.Data.CommandType.Text;
                HanaDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0)) { guias += dr.GetString(0) + ","; }
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return guias;
        }
        public TEMP_RRU01_E obtenerGuiaRemision(string NumAtCard)
        {
            TEMP_RRU01_E obj = new TEMP_RRU01_E();
            string query = $"SELECT 'ODLN', \"U_SYP_MDTD\", \"U_SYP_MDSD\", \"U_SYP_MDCD\", TO_CHAR(\"DocDate\", 'YYYY-MM-DD'), TO_CHAR(\"U_BPP_FECINITRA\", 'YYYY-MM-DD'),'G' FROM {uti.schemaHana}ODLN  WHERE \"NumAtCard\" ='{NumAtCard}'";
            HanaConnection cn = new HanaConnection(uti.cadHana);
            try
            {
                cn.Open();
                HanaCommand cmd = new HanaCommand(query, cn);
                cmd.CommandType = System.Data.CommandType.Text;
                HanaDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0)) { obj.TablaSAP = dr.GetString(0); }
                    if (!dr.IsDBNull(1)) { obj.U_SYP_MDTD = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { obj.U_SYP_MDSD = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { obj.U_SYP_MDCD = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { obj.DocDate = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { obj.U_BPP_FECINITRA = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { obj.Identificador = dr.GetString(6); }
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return obj;
        }
        public List<Guia_Remision_E> buscarGuiaRemisionSap(string NumAtCard)
        {
            List<Guia_Remision_E> lista = new List<Guia_Remision_E>();
            int DocEntry = 0;
            //busca DocEntry de NumAtCard en ODLN
            string queryDE = $"SELECT  \"DocEntry\" FROM {uti.schemaHana}ODLN  WHERE \"NumAtCard\" = '{NumAtCard}'";
            HanaConnection cn = new HanaConnection(uti.cadHana);
            try
            {
                cn.Open();
                HanaCommand cmd = new HanaCommand(queryDE, cn);
                cmd.CommandType = System.Data.CommandType.Text;
                HanaDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0)) { DocEntry = dr.GetInt32(0); }
                }
                dr.Close(); cn.Close();
            }
            catch { return lista; }

            if (DocEntry > 0)
            {
                string query = $"CALL {uti.schemaHana} DIEGO_LYT_EV({DocEntry})";
                try
                {
                    HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                    if (hdr.HasRows)
                    {
                        while (hdr.Read())
                        {
                            Guia_Remision_E c = new Guia_Remision_E();
                            if (!hdr.IsDBNull(0)) { c.NumAtCard = hdr.GetString(0); }
                            if (!hdr.IsDBNull(1)) { c.DocDate = Convert.ToDateTime(hdr.GetString(1)).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(2)) { c.DirCliente = hdr.GetString(2); }
                            if (!hdr.IsDBNull(3)) { c.DirSalida = hdr.GetString(3); }
                            if (!hdr.IsDBNull(4)) { c.CardName = hdr.GetString(4); }
                            if (!hdr.IsDBNull(5)) { c.CardCode = hdr.GetString(5); }
                            if (!hdr.IsDBNull(6)) { c.Cantidad = Math.Round(hdr.GetDecimal(6), 0); }
                            if (!hdr.IsDBNull(7)) { c.CantidadL = Math.Round(hdr.GetDecimal(7), 0); }
                            if (!hdr.IsDBNull(8)) { c.QUMVta = Math.Round(hdr.GetDecimal(8), 0); }
                            if (!hdr.IsDBNull(9)) { c.UniMedida = hdr.GetString(9); }
                            if (!hdr.IsDBNull(10)) { c.DescripcionArticulo = hdr.GetString(10); }
                            if (!hdr.IsDBNull(11)) { c.Laboratorio = hdr.GetString(11); }
                            if (!hdr.IsDBNull(12)) { c.LoteNum = hdr.GetString(12); }
                            if (!hdr.IsDBNull(13)) { c.VctoLote = Convert.ToDateTime(hdr.GetString(13)).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(14)) { c.Motivo = hdr.GetString(14); }
                            if (!hdr.IsDBNull(15)) { c.LineaOrden = hdr.GetInt32(15); }
                            if (!hdr.IsDBNull(16)) { c.Texto = hdr.GetString(16); }
                            if (!hdr.IsDBNull(17)) { c.FechaTrasl = Convert.ToDateTime(hdr.GetString(17)).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(18)) { c.Motivo_Trasl = hdr.GetString(18); }
                            if (!hdr.IsDBNull(19)) { c.Modalidad_Trasl = hdr.GetString(19); }
                            if (!hdr.IsDBNull(20)) { c.PesoTotal = Math.Round(hdr.GetDecimal(20), 0); }
                            if (!hdr.IsDBNull(21)) { c.Conductor = hdr.GetString(21); }
                            if (!hdr.IsDBNull(22)) { c.DNI_Conduc = hdr.GetString(22); }
                            if (!hdr.IsDBNull(23)) { c.Licencia = hdr.GetString(23); }
                            if (!hdr.IsDBNull(24)) { c.Marca = hdr.GetString(24); }
                            if (!hdr.IsDBNull(25)) { c.Placa = hdr.GetString(25); }
                            if (!hdr.IsDBNull(26)) { c.TipoComprobantePago = hdr.GetString(26); }
                            if (!hdr.IsDBNull(27)) { c.NroComprobantePago = hdr.GetString(27); }

                            lista.Add(c);
                        }
                    }
                    hdr.Close();
                }
                catch (Exception e) { throw new Exception(e.Message); }
            }
            return lista;
        }
        public List<ODLN_E> listarEntregasPorNroVenta(int DocEntryVenta)
        {
            List<ODLN_E> lista = new List<ODLN_E>();
            string query = "select t0.\"DocEntry\" from " + uti.schemaHana + "odln t0 " +
                            " inner join " + uti.schemaHana + "dln1 t1 on t1.\"DocEntry\"=t0.\"DocEntry\"" +
                            " inner join " + uti.schemaHana + "rdr1 t2 on t2.\"DocEntry\"=t1.\"BaseEntry\" and t2.\"ObjType\"=t1.\"BaseType\" " +
                                                    " and t2.\"ItemCode\"=t1.\"ItemCode\" where t0.\"CANCELED\"='N' and t2.\"DocEntry\"=" + DocEntryVenta
                                                    + " group by t0.\"DocEntry\"";
            HanaConnection cn = new HanaConnection(uti.cadHana);
            try
            {
                cn.Open();
                HanaCommand cmd = new HanaCommand(query, cn);
                cmd.CommandType = System.Data.CommandType.Text;
                HanaDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ODLN_E o = new ODLN_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    lista.Add(o);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public string CalcularPdfsActaDespachoODLN(string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN, string TipoComprobante)
        {
            string doc = string.Empty;
            string filtros = string.Empty;

            if (!string.IsNullOrEmpty(Fecha))
            {
                filtros += " and \"U_BPP_FECINITRA\"='" + Fecha + "'";
            }
            if (!string.IsNullOrEmpty(U_SYP_STATUS))
            {
                if (U_SYP_STATUS == "V") { filtros += " and \"CANCELED\"='N'"; }
                filtros += " and \"U_SYP_STATUS\"='" + U_SYP_STATUS + "'";
            }
            if (!string.IsNullOrEmpty(U_COB_LUGAREN))
            {
                filtros += " and \"U_COB_LUGAREN\"='" + U_COB_LUGAREN + "'";
            }
            string query = "select count(*)  from " + uti.schemaHana + " ODLN where \"DocEntry\">0 " + filtros;
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                hdr.Read();
                doc = "Son " + hdr.GetInt32(0) + " Documentos";
                hdr.Close();
            }
            catch { }
            return doc;
        }
    }
}
