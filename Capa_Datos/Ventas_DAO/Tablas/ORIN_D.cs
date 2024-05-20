using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.Tablas;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;

namespace Capa_Datos.Ventas_DAO.Tablas
{
    public class ORIN_D
    {
        DBHelper db = new DBHelper(); Utilitarios uti = new Utilitarios();
        public ORIN_E buscarNotaDeCredito(int DocEntry)
        {
            ORIN_E o = null;
            string query = "select \"DocEntry\",\"DocNum\",\"DocDate\",\"CardName\",\"NumAtCard\",\"DocTotal\",\"U_SYP_STATUS\", IFNULL(\"U_SYP_MDTO\"||'-','')||IFNULL(\"U_SYP_MDSO\"||'-','')||IFNULL(\"U_SYP_MDCO\",''), \"DocType\" from " + uti.schemaHana + "ORIN where \"DocEntry\"=" + DocEntry;
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                hdr.Read();
                o = new ORIN_E();
                if (!hdr.IsDBNull(0)) { o.DocEntry = hdr.GetInt32(0); }
                if (!hdr.IsDBNull(1)) { o.DocNum = hdr.GetInt32(1); }
                if (!hdr.IsDBNull(2)) { o.DocDate = hdr.GetDateTime(2).ToString("dd/MM/yyyy"); }
                if (!hdr.IsDBNull(3)) { o.CardName = hdr.GetString(3); }
                if (!hdr.IsDBNull(4)) { o.NumAtCard = hdr.GetString(4); }
                if (!hdr.IsDBNull(5)) { o.DocTotal = Math.Round(hdr.GetDecimal(5), 2); }
                if (!hdr.IsDBNull(6)) { o.U_SYP_STATUS = hdr.GetString(6); }
                if (!hdr.IsDBNull(7)) { o.RefFactura = hdr.GetString(7); }
                if (!hdr.IsDBNull(8)) { o.DocType = hdr.GetString(8); }

                hdr.Close();
            }
            catch { }
            return o;
        }
        public List<ORIN_E> listarNotasDeCredito(ORIN_E fil)
        {
            List<ORIN_E> lista = new List<ORIN_E>();
            string filtros = string.Empty;
            if (filtros != null)
            {
                if (fil.DocNum > 0) { filtros += " and \"DocNum\" like '%" + fil.DocNum + "'"; }
                if (!string.IsNullOrEmpty(fil.DocDate)) { filtros += " and \"DocDate\"='" + fil.DocDate + "'"; }
                if (!string.IsNullOrEmpty(fil.CardName)) { filtros += " and UPPER(\"CardName\") like UPPER('%" + fil.CardName + "%')"; }
                if (!string.IsNullOrEmpty(fil.NumAtCard)) { filtros += " and UPPER(\"NumAtCard\") like UPPER('%" + fil.NumAtCard + "')"; }
                if (fil.DocTotal > 0.00M) { filtros += " and \"DocTotal\" like '%" + fil.DocTotal + "%'"; }
                if (!string.IsNullOrEmpty(fil.U_SYP_STATUS)) { filtros += " and UPPER(\"U_SYP_STATUS\")=UPPER('" + fil.U_SYP_STATUS + "')"; }
                if (!string.IsNullOrEmpty(fil.RefFactura)) { filtros += " and UPPER(IFNULL(\"U_SYP_MDTO\"||'-','')||IFNULL(\"U_SYP_MDSO\"||'-','')||IFNULL(\"U_SYP_MDCO\",'')) like UPPER('%" + fil.RefFactura + "%')"; }
            }
            string query = "select top 50 \"DocEntry\" from " + uti.schemaHana + "ORIN where \"DocEntry\">0 " + filtros + " order by 1 desc";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    lista.Add(buscarNotaDeCredito(hdr.GetInt32(0)));
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public TEMP_RRU01_E NotaCreditoSap(int DocNum)
        {
            TEMP_RRU01_E obj = new TEMP_RRU01_E();
            string query = "select 'ORIN',\"U_SYP_MDTD\",\"U_SYP_MDSD\",\"U_SYP_MDCD\", TO_CHAR(\"DocDate\",'YYYY-MM-DD'),'NC'  from " + uti.schemaHana + "ORIN where \"DocNum\"=" + DocNum;
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                hdr.Read();
                if (!hdr.IsDBNull(0)) { obj.TablaSAP = hdr.GetString(0); }
                if (!hdr.IsDBNull(1)) { obj.U_SYP_MDTD = hdr.GetString(1); }
                if (!hdr.IsDBNull(2)) { obj.U_SYP_MDSD = hdr.GetString(2); }
                if (!hdr.IsDBNull(3)) { obj.U_SYP_MDCD = hdr.GetString(3); }
                if (!hdr.IsDBNull(4)) { obj.DocDate = hdr.GetString(4); }
                if (!hdr.IsDBNull(5)) { obj.Identificador = hdr.GetString(5); }
                hdr.Close();
            }
            catch { }
            return obj;
        }
        public List<NotaCreditoDebito_E> buscarNotaCreditoSapServicio(string NumAtCard)
        {
            List<NotaCreditoDebito_E> lista = new List<NotaCreditoDebito_E>();
            int DocEntry = 0;
            string queryDE = $"SELECT  \"DocEntry\" FROM {uti.schemaHana}ORIN  WHERE \"U_SYP_MDTD\" || '-' ||\"U_SYP_MDSD\" || '-' || \"U_SYP_MDCD\" = '{NumAtCard}'";
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
                string query = $"CALL {uti.schemaHana}DIEGO_LYT_NCS_COBEFAR({DocEntry})";
                try
                {
                    HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                    if (hdr.HasRows)
                    {
                        while (hdr.Read())
                        {
                            NotaCreditoDebito_E c = new NotaCreditoDebito_E();
                            if (!hdr.IsDBNull(0)) { c.TipoDoc = hdr.GetString(0); }
                            if (!hdr.IsDBNull(1)) { c.SerieDoc = hdr.GetString(1); }
                            if (!hdr.IsDBNull(2)) { c.CorreDoc = hdr.GetString(2); }
                            if (!hdr.IsDBNull(3)) { c.NombreSocio = hdr.GetString(3); }
                            if (!hdr.IsDBNull(4)) { c.DirPagar = hdr.GetString(4); }
                            if (!hdr.IsDBNull(5)) { c.Ruc = hdr.GetString(5); }
                            if (!hdr.IsDBNull(6)) { c.TipoDocRel = hdr.GetString(6); }
                            if (!hdr.IsDBNull(7)) { c.SerieDocRel = hdr.GetString(7); }
                            if (!hdr.IsDBNull(8)) { c.CorreDocRel = hdr.GetString(8); }
                            if (!hdr.IsDBNull(9)) { c.DocDateRel = hdr.GetDateTime(9).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(10)) { c.DocDate = hdr.GetDateTime(10).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(11)) { c.ElaboradoPor = hdr.GetString(11); }
                            if (!hdr.IsDBNull(13)) { c.DescripcionLinea = hdr.GetString(13); }
                            if (!hdr.IsDBNull(15)) { c.PrecioLinea = Math.Round(hdr.GetDecimal(15), 2); }
                            if (!hdr.IsDBNull(16)) { c.PreUnitSinIgv = Math.Round(hdr.GetDecimal(16), 2); }
                            if (!hdr.IsDBNull(17)) { c.Moneda = hdr.GetString(17); }
                            if (!hdr.IsDBNull(18)) { c.MonedaLetras = hdr.GetString(18); }
                            if (!hdr.IsDBNull(23)) { c.Impuesto = Math.Round(hdr.GetDecimal(23), 2); }
                            if (!hdr.IsDBNull(24)) { c.DocTotal = Math.Round(hdr.GetDecimal(24), 2); }
                            if (!hdr.IsDBNull(28)) { c.Motivo = hdr.GetString(28); }

                            lista.Add(c);
                        }
                    }
                    hdr.Close();
                }
                catch (Exception e) { throw new Exception(e.Message); }
            }
            return lista;
        }
        public List<NotaCreditoDebito_E> buscarNotaCreditoSapArticulo(string NumAtCard)
        {
            List<NotaCreditoDebito_E> lista = new List<NotaCreditoDebito_E>();
            int DocEntry = 0;
            string queryDE = $"SELECT  \"DocEntry\" FROM {uti.schemaHana}ORIN where  \"U_SYP_MDTD\" || '-' ||\"U_SYP_MDSD\" || '-' || \"U_SYP_MDCD\" = '{NumAtCard}'";
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
                string query = $"CALL {uti.schemaHana}DIEGO_LYT_NC_ELECT ({DocEntry})";
                try
                {
                    HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                    if (hdr.HasRows)
                    {
                        while (hdr.Read())
                        {
                            NotaCreditoDebito_E c = new NotaCreditoDebito_E();
                            if (!hdr.IsDBNull(1)) { c.ElaboradoPor = hdr.GetString(1); }
                            if (!hdr.IsDBNull(2)) { c.SerieDoc = hdr.GetString(2); }
                            if (!hdr.IsDBNull(3)) { c.CorreDoc = hdr.GetString(3); }
                            if (!hdr.IsDBNull(4)) { c.TipoDocRel = hdr.GetString(4); }
                            if (!hdr.IsDBNull(5)) { c.SerieDocRel = hdr.GetString(5); }
                            if (!hdr.IsDBNull(6)) { c.CorreDocRel = hdr.GetString(6); }
                            if (!hdr.IsDBNull(7)) { c.DocDateRel = hdr.GetDateTime(7).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(8)) { c.Motivo = hdr.GetString(8); }
                            if (!hdr.IsDBNull(9)) { c.NombreSocio = hdr.GetString(9); }
                            if (!hdr.IsDBNull(10)) { c.DirPagar = hdr.GetString(10); }

                            if (!hdr.IsDBNull(11)) { c.Ruc = hdr.GetString(11); }
                            if (!hdr.IsDBNull(12)) { c.DocDate = hdr.GetDateTime(12).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(13)) { c.Moneda = hdr.GetString(13); }
                            if (!hdr.IsDBNull(14)) { c.MonedaLetras = hdr.GetString(14); }
                            if (!hdr.IsDBNull(15)) { c.DescripcionLinea = hdr.GetString(15); }
                            if (!hdr.IsDBNull(16)) { c.PrecioLinea = Math.Round(hdr.GetDecimal(16), 2); }
                            if (!hdr.IsDBNull(17)) { c.PrecioLineaTotal = Math.Round(hdr.GetDecimal(17), 2); }

                            if (!hdr.IsDBNull(18)) { c.Impuesto = Math.Round(hdr.GetDecimal(18), 2); }
                            if (!hdr.IsDBNull(19)) { c.DocTotal = Math.Round(hdr.GetDecimal(19), 2); }
                            if (!hdr.IsDBNull(20)) { c.ImpuestoPorcentaje = Math.Round(hdr.GetDecimal(20), 0); }
                            if (!hdr.IsDBNull(21)) { c.LoteNum = hdr.GetString(21); }
                            if (!hdr.IsDBNull(22)) { c.CantidadL = Math.Round(hdr.GetDecimal(22), 2); }
                            if (!hdr.IsDBNull(26)) { c.Laboratorio = hdr.GetString(26); }
                            if (!hdr.IsDBNull(27)) { c.QUMVta = Math.Round(hdr.GetDecimal(27), 2); }
                            if (!hdr.IsDBNull(28)) { c.CodImpuesto = hdr.GetString(28); }
                            if (!hdr.IsDBNull(29)) { c.UmLinea = hdr.GetString(29); }
                            if (!hdr.IsDBNull(30)) { c.Descuento = Math.Round(hdr.GetDecimal(30), 2); }
                            if (!hdr.IsDBNull(31)) { c.PreVentaNeto = Math.Round(hdr.GetDecimal(31), 2); }
                            if (!hdr.IsDBNull(32)) { c.PreUnitSinIgv = Math.Round(hdr.GetDecimal(32), 2); }
                            if (!hdr.IsDBNull(33)) { c.VencLote = hdr.GetDateTime(33).ToString("dd/MM/yyyy"); }

                            lista.Add(c);
                        }
                    }
                    hdr.Close();
                }
                catch (Exception e) { throw new Exception(e.Message); }
            }
            return lista;
        }
    }
}
