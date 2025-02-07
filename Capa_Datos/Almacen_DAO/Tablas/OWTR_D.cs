using Capa_Datos.DireccionTecnica_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.ComprobantesContables_ENT;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Capa_Datos.Almacen_DAO.Tablas
{
    public class OWTR_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();

        public List<OWTR_E> listadoTransferenciasStock(OWTR_E fil)
        {
            List<OWTR_E> lista = new List<OWTR_E>();
            string filtro = string.Empty;
            if (fil != null)
            {
                if (fil.DocNum > 0) { filtro += " and \"DocNum\" like '%" + fil.DocNum + "'"; }
                if (!string.IsNullOrWhiteSpace(fil.DocDate)) { filtro += " and \"DocDate\"='" + fil.DocDate + "'"; }
                if (!string.IsNullOrWhiteSpace(fil.U_BPP_FECINITRA)) { filtro += " and \"U_BPP_FECINITRA\"='" + fil.U_BPP_FECINITRA + "'"; }
                if (!string.IsNullOrWhiteSpace(fil.Filler)) { filtro += " and \"Filler\"='" + fil.Filler + "'"; }
                if (!string.IsNullOrWhiteSpace(fil.ToWhsCode)) { if (fil.ToWhsCode == "ALM07") { filtro += " and \"ToWhsCode\" in ('" + fil.ToWhsCode + "','CUAR07')"; } else { filtro += " and \"ToWhsCode\"='" + fil.ToWhsCode + "'"; } }
                if (fil.SlpCode > 0) { filtro += " and \"SlpCode\"='" + fil.SlpCode + "'"; }
                if (!string.IsNullOrWhiteSpace(fil.U_SYP_MDCD)) { filtro += " and \"U_SYP_MDCD\" like '%" + fil.U_SYP_MDCD + "'"; }
                if (!string.IsNullOrWhiteSpace(fil.U_SYP_STATUS)) { filtro += " and UPPER(\"U_SYP_STATUS\")=UPPER('" + fil.U_SYP_STATUS + "')"; }
            }
            string query = "select top 500 \"DocEntry\",\"DocNum\",\"DocDate\",\"Filler\",\"ToWhsCode\",\"SlpCode\",\"U_SYP_MDTD\",\"U_SYP_MDSD\",\"U_SYP_MDCD\",\"U_SYP_STATUS\", \"U_BPP_FECINITRA\"  from " + uti.schemaHana + "OWTR where \"DocEntry\">0 " + filtro + " order by 1 desc";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    OWTR_E o = new OWTR_E();
                    o.DocEntry = hdr.GetInt32(0);
                    if (!hdr.IsDBNull(1)) { o.DocNum = hdr.GetInt32(1); }
                    if (!hdr.IsDBNull(2)) { o.DocDate = hdr.GetDateTime(2).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(3)) { o.Filler = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { o.ToWhsCode = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { o.SlpCode = hdr.GetInt32(5); }
                    if (!hdr.IsDBNull(6)) { o.U_SYP_MDTD = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { o.U_SYP_MDSD = hdr.GetString(7); }
                    if (!hdr.IsDBNull(8)) { o.U_SYP_MDCD = hdr.GetString(8); }
                    if (!hdr.IsDBNull(9)) { o.U_SYP_STATUS = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { o.U_BPP_FECINITRA = hdr.GetDateTime(10).ToString("dd/MM/yyyy"); }

                    SQL_OWTR_D owtrD = new SQL_OWTR_D();
                    var resultOWTR = owtrD.ObtenerOWTR(o.DocNum);
                    o.Estado = (resultOWTR != null && !string.IsNullOrWhiteSpace(resultOWTR.Estado)) ? resultOWTR.Estado : "Pendiente";

                    // Si se filtrar por Estado, se considera de prioridad que solo se agreguen a la lista los que tienen el mismo Estado filtrado
                    if (fil != null && !string.IsNullOrWhiteSpace(fil.Estado) && fil.Estado.Equals(o.Estado))
                    {
                        lista.Add(o);
                    }
                    else if (string.IsNullOrWhiteSpace(fil.Estado))
                    {
                        // En caso el usuario no filtre por Estado, entonces se agregarán todos a la lista sin excepción
                        lista.Add(o);
                    }

                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public string GuiasTicketTransferencia(int DocNum, string WhsCode ,string CardCode)
        {
            string guias = string.Empty;
            string query = "SELECT top 10 IFNULL(T0.\"U_SYP_MDTD\" || '-' || T0.\"U_SYP_MDSD\" || '-' || T0.\"U_SYP_MDCD\", '') as \"GUIAS\" " +
                    "FROM " + uti.schemaHana + "OWTR T0 WHERE T0.\"CANCELED\" = 'N' AND T0.\"U_SYP_MDTD\" IS NOT NULL AND T0.\"U_SYP_MDSD\" IS NOT NULL " +
                    "AND T0.\"U_SYP_MDCD\" IS NOT NULL AND T0.\"ToWhsCode\" ='" + WhsCode + "' AND T0.\"U_COB_LUGAREN\" ='" + WhsCode + "' " +
                    "AND T0.\"Comments\" like '" + "%" + DocNum + "%" + "' AND T0.\"CardCode\" ='" + CardCode + "' ORDER BY T0.\"DocEntry\" desc";

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
            catch { cn.Close(); return guias; }
            return guias;
        }
        public List<Guia_Remision_E> buscarGuiaRemisionSap(string NumAtCard)
        {
            List<Guia_Remision_E> lista = new List<Guia_Remision_E>();
            int DocEntry = 0;
            string queryDE = $"SELECT  \"DocEntry\" FROM {uti.schemaHana}OWTR  WHERE \"U_SYP_MDTD\" || '-' ||\"U_SYP_MDSD\" || '-' || \"U_SYP_MDCD\" = '{NumAtCard}'";
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
                string query = $"CALL {uti.schemaHana} DIEGO_LYT_TS({DocEntry})";
                try
                {
                    HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                    if (hdr.HasRows)
                    {
                        while (hdr.Read())
                        {
                            Guia_Remision_E c = new Guia_Remision_E();
                            if (!hdr.IsDBNull(0)) { c.DocEntry = hdr.GetInt32(0); }
                            if (!hdr.IsDBNull(2)) { c.ElaboradoPor = hdr.GetString(2); }
                            if (!hdr.IsDBNull(3)) { c.NombreBD = hdr.GetString(3); }
                            if (!hdr.IsDBNull(4)) { c.DireccionBD = hdr.GetString(4); }
                            if (!hdr.IsDBNull(5)) { c.RucBD = hdr.GetString(5); }
                            if (!hdr.IsDBNull(6)) { c.TelBD = hdr.GetString(6); }
                            if (!hdr.IsDBNull(7)) { c.DocNum = hdr.GetInt32(7); }
                            if (!hdr.IsDBNull(8)) { c.DocDate = Convert.ToDateTime(hdr.GetString(8)).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(9)) { c.CardCode = hdr.GetString(9); }
                            if (!hdr.IsDBNull(10)) { c.CardName = hdr.GetString(10); }
                            if (!hdr.IsDBNull(11)) { c.NumAtCard = hdr.GetString(11); }
                            if (!hdr.IsDBNull(12)) { c.DirAlmacen = hdr.GetString(12); }
                            if (!hdr.IsDBNull(13)) { c.NumAlmacen = hdr.GetString(13); }
                            if (!hdr.IsDBNull(14)) { c.DistritoAlmacen = hdr.GetString(14); }
                            if (!hdr.IsDBNull(15)) { c.ProvinciaAlmacen = hdr.GetString(15); }
                            if (!hdr.IsDBNull(16)) { c.DepartamentoAlmacen = hdr.GetString(16); }
                            if (!hdr.IsDBNull(17)) { c.DirLlegada = hdr.GetString(17); }
                            if (!hdr.IsDBNull(18)) { c.DirProveedor = hdr.GetString(18); }
                            if (!hdr.IsDBNull(19)) { c.Placa = hdr.GetString(19); }
                            if (!hdr.IsDBNull(20)) { c.Marca = hdr.GetString(20); }
                            if (!hdr.IsDBNull(21)) { c.CertiInscrip = hdr.GetString(21); }
                            if (!hdr.IsDBNull(22)) { c.Conductor = hdr.GetString(22); }
                            if (!hdr.IsDBNull(23)) { c.Licencia = hdr.GetString(23); }
                            if (!hdr.IsDBNull(24)) { c.ItemCode = hdr.GetString(24); }
                            if (!hdr.IsDBNull(25)) { c.DescripcionArticulo = hdr.GetString(25); }
                            if (!hdr.IsDBNull(26)) { c.UniMedida = hdr.GetString(26); }
                            if (!hdr.IsDBNull(27)) { c.Cantidad = Math.Round(hdr.GetDecimal(27), 0); }
                            if (!hdr.IsDBNull(28)) { c.DocOrigen = hdr.GetString(28); }
                            if (!hdr.IsDBNull(29)) { c.NomTransportista = hdr.GetString(29); }
                            if (!hdr.IsDBNull(30)) { c.RucTransportista = hdr.GetString(30); }
                            if (!hdr.IsDBNull(31)) { c.UndPesoLinea = Math.Round(hdr.GetDecimal(31), 0); }
                            if (!hdr.IsDBNull(32)) { c.LoteNum = hdr.GetString(32); }
                            if (!hdr.IsDBNull(33)) { c.CantidadL = Math.Round(hdr.GetDecimal(33), 0); }
                            if (!hdr.IsDBNull(34)) { c.UnidadMedidaLote = hdr.GetString(34); }
                            if (!hdr.IsDBNull(35)) { c.UnidadMedidaLote2 = hdr.GetString(35); }
                            if (!hdr.IsDBNull(36)) { c.TextoPermanente = hdr.GetString(36); }
                            if (!hdr.IsDBNull(37)) { c.Motivo = hdr.GetString(37); }
                            if (!hdr.IsDBNull(38)) { c.RegSanit = hdr.GetString(38); }
                            if (!hdr.IsDBNull(39)) { c.VctoLote = Convert.ToDateTime(hdr.GetString(39)).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(40)) { c.Texto = hdr.GetString(40); }
                            if (!hdr.IsDBNull(41)) { c.FechaTrasl = Convert.ToDateTime(hdr.GetString(41)).ToString("dd/MM/yyyy"); }
                            if (!hdr.IsDBNull(42)) { c.Motivo_Trasl = hdr.GetString(42); }
                            if (!hdr.IsDBNull(43)) { c.Modalidad_Trasl = hdr.GetString(43); }
                            if (!hdr.IsDBNull(44)) { c.PesoTotal = Math.Round(hdr.GetDecimal(44), 0); }
                            if (!hdr.IsDBNull(45)) { c.Conductor = hdr.GetString(45); }
                            if (!hdr.IsDBNull(46)) { c.DNI_Conduc = hdr.GetString(46); }
                            if (!hdr.IsDBNull(47)) { c.Licencia = hdr.GetString(47); }
                            if (!hdr.IsDBNull(48)) { c.Marca = hdr.GetString(48); }
                            if (!hdr.IsDBNull(49)) { c.Placa = hdr.GetString(49); }
                            if (!hdr.IsDBNull(50)) { c.Bulto = hdr.GetInt32(50); }
                            if (!hdr.IsDBNull(51)) { c.Laboratorio = hdr.GetString(51); }
                            if (!hdr.IsDBNull(52)) { c.TipoComprobantePago = hdr.GetString(52); }
                            if (!hdr.IsDBNull(53)) { c.NroComprobantePago = hdr.GetString(53); }
                            if (!hdr.IsDBNull(54)) { c.QUMVta = Math.Round(hdr.GetDecimal(54), 0); }
                            if (!hdr.IsDBNull(55)) { c.DirSalida = hdr.GetString(55); }
                            if (!hdr.IsDBNull(56)) { c.DirLlegada = hdr.GetString(56); }
                            lista.Add(c);
                        }
                    }
                    hdr.Close();
                }
                catch (Exception e) { throw new Exception(e.Message); }
            }
            return lista;
        }
        public string CalcularPdfsActaRecepcion(string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN)
        {
            string doc = string.Empty;
            string filtros = string.Empty;

            if (!string.IsNullOrWhiteSpace(Fecha))
            {
                filtros += " and \"U_BPP_FECINITRA\"='" + Fecha + "'";
            }
            if (!string.IsNullOrWhiteSpace(U_SYP_STATUS))
            {
                if (U_SYP_STATUS == "V") { filtros += " and \"CANCELED\"='N'"; }
                filtros += " and \"U_SYP_STATUS\"='" + U_SYP_STATUS + "'";
            }
            if (!string.IsNullOrWhiteSpace(U_COB_LUGAREN))
            {
                filtros += " and \"U_COB_LUGAREN\"='" + U_COB_LUGAREN + "'";
            }
            string query = "select count(*)  from " + uti.schemaHana + " OWTR where \"U_SYP_MDCD\" is not null and  \"U_SYP_MDSD\" is not null " + filtros;
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
        public List<(string, int)> DetalleCalculadoraPdf(string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN)
        {
            List<(string, int)> detalles = new List<(string, int)>();
            string filtros = string.Empty;

            if (!string.IsNullOrWhiteSpace(Fecha))
            {
                filtros += " and \"U_BPP_FECINITRA\"='" + Fecha + "'";
            }
            if (!string.IsNullOrWhiteSpace(U_SYP_STATUS))
            {
                if (U_SYP_STATUS == "V")
                {
                    filtros += " and \"CANCELED\"='N'";
                }
                filtros += " and \"U_SYP_STATUS\"='" + U_SYP_STATUS + "'";
            }
            if (!string.IsNullOrWhiteSpace(U_COB_LUGAREN))
            {
                filtros += " and \"U_COB_LUGAREN\"='" + U_COB_LUGAREN + "'";
            }

            string query = $"SELECT TO_CHAR(\"DocDate\", 'YYYY-MM-DD') AS \"FECHADOC\", COUNT(*) AS \"CANTIDAD\" FROM {uti.schemaHana}OWTR WHERE \"DocEntry\" > 0 AND \"U_SYP_MDCD\" is not null AND  \"U_SYP_MDSD\" is not null  {filtros} AND \"DocDate\" in (SELECT distinct \"DocDate\" FROM {uti.schemaHana}OWTR WHERE \"DocEntry\" > 0 AND \"U_SYP_MDCD\" is not null AND  \"U_SYP_MDSD\" is not null {filtros} ) GROUP BY \"DocDate\" ORDER BY \"DocDate\" ASC";

            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);

                while (hdr.Read())
                {
                    if (!hdr.IsDBNull(0) && !hdr.IsDBNull(1))
                    {
                        detalles.Add((hdr.GetString(0), hdr.GetInt32(1)));
                    }
                }

                hdr.Close();
            }
            catch (Exception ex)
            {

            }

            return detalles;
        }
        public string CalcularPdfsActaDespachoOWTR(string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN)
        {
            string doc = string.Empty;
            string filtros = string.Empty;

            if (!string.IsNullOrWhiteSpace(Fecha))
            {
                filtros += " and \"U_BPP_FECINITRA\"='" + Fecha + "'";
            }
            if (!string.IsNullOrWhiteSpace(U_SYP_STATUS))
            {
                if (U_SYP_STATUS == "V") { filtros += " and \"CANCELED\"='N'"; }
                filtros += " and \"U_SYP_STATUS\"='" + U_SYP_STATUS + "'";
            }
            if (!string.IsNullOrWhiteSpace(U_COB_LUGAREN))
            {
                if (U_COB_LUGAREN == "15") { filtros += " and \"Filler\" in ('15','16')"; }
                else
                {
                    filtros += " and \"Filler\"='" + U_COB_LUGAREN + "'";
                }
            }
            string query = "select count(*)  from " + uti.schemaHana + " OWTR  WHERE \"ToWhsCode\" IN ('09','01','ALM07')  " + filtros;
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
        public List<(string, int)> DetalleCalculadoraPdfOWTR(string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN)
        {
            List<(string, int)> detalles = new List<(string, int)>();
            string filtros = string.Empty;

            if (!string.IsNullOrWhiteSpace(Fecha))
            {
                filtros += " and \"U_BPP_FECINITRA\"='" + Fecha + "'";
            }
            if (!string.IsNullOrWhiteSpace(U_SYP_STATUS))
            {
                if (U_SYP_STATUS == "V")
                {
                    filtros += " and \"CANCELED\"='N'";
                }
                filtros += " and \"U_SYP_STATUS\"='" + U_SYP_STATUS + "'";
            }
            if (!string.IsNullOrWhiteSpace(U_COB_LUGAREN))
            {
                if (U_COB_LUGAREN == "15") { filtros += " and \"Filler\" in ('15','16')"; }
                else
                {
                    filtros += " and \"Filler\"='" + U_COB_LUGAREN + "'";
                }
            }

            string query = $"SELECT TO_CHAR(\"DocDate\", 'YYYY-MM-DD') AS \"FECHADOC\", COUNT(*) AS \"CANTIDAD\" FROM {uti.schemaHana}OWTR WHERE \"DocEntry\" > 0 AND \"ToWhsCode\" IN ('09','01') {filtros} AND \"DocDate\" in (SELECT distinct \"DocDate\" FROM {uti.schemaHana}OWTR WHERE \"DocEntry\" > 0 AND  \"ToWhsCode\" IN ('09','01','ALM07') {filtros} ) GROUP BY \"DocDate\" ORDER BY \"DocDate\" ASC";

            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);

                while (hdr.Read())
                {
                    if (!hdr.IsDBNull(0) && !hdr.IsDBNull(1))
                    {
                        detalles.Add((hdr.GetString(0), hdr.GetInt32(1)));
                    }
                }

                hdr.Close();
            }
            catch (Exception ex)
            {
                
            }

            return detalles;
        }
    }
}
