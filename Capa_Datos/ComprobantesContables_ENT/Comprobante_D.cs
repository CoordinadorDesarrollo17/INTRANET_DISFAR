using Capa_Datos.Ventas_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.ComprobantesContables_ENT;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.ComprobantesContables_ENT
{
    public class Comprobante_D
    {
        readonly Utilitarios uti = new Utilitarios();
        readonly DBHelper db = new DBHelper();
        private List<Comprobante_E> EjecutarConsultaComprobante(string query)
        {
            List<Comprobante_E> lista = new List<Comprobante_E>();
            HanaConnection cn = new HanaConnection(uti.cadHana);
            try
            {
                cn.Open();
                HanaCommand cmd = new HanaCommand(query, cn)
                {
                    CommandType = System.Data.CommandType.Text
                };
                HanaDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Comprobante_E obj = new Comprobante_E();
                    if (!dr.IsDBNull(0)) { obj.TablaSAP = dr.GetString(0); }
                    if (!dr.IsDBNull(1)) { obj.U_SYP_MDTD = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { obj.U_SYP_MDSD = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { obj.U_SYP_MDCD = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { obj.DocDate = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { obj.U_BPP_FECINITRA = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { obj.Identificador = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { obj.DocTotal = dr.GetDecimal(7); }
                    lista.Add(obj);
                }
                dr.Close();
            }
            catch {}
            finally
            {
                cn.Close();
            }
            return lista;
        }
        private Comprobante_E obtenerGuiaRemisionODLN(string NumAtCard)
        {
            string query = $"SELECT 'ODLN', \"U_SYP_MDTD\", \"U_SYP_MDSD\", \"U_SYP_MDCD\", TO_CHAR(\"DocDate\", 'YYYY-MM-DD'), TO_CHAR(\"U_BPP_FECINITRA\", 'YYYY-MM-DD'),'G',null FROM {uti.schemaHana}ODLN WHERE \"NumAtCard\" ='{NumAtCard}'";
            return EjecutarConsultaComprobante(query).FirstOrDefault();
        }
        private Comprobante_E obtenerGuiaRemisionOINV(string NumAtCard)
        {
            string query = $"SELECT 'OINV', \"U_COB_TIPODOC\", \"U_COB_SERIE\", \"U_COB_CORDOC\", TO_CHAR(\"DocDate\", 'YYYY-MM-DD'), TO_CHAR(\"U_BPP_FECINITRA\", 'YYYY-MM-DD'), 'G',null FROM {uti.schemaHana}OINV WHERE \"U_COB_TIPODOC\"||'-'||\"U_COB_SERIE\" ||'-'||\"U_COB_CORDOC\" = '{NumAtCard}'";
            return EjecutarConsultaComprobante(query).FirstOrDefault();
        }
        private Comprobante_E obtenerGuiaRemisionOWTR(string NumAtCard)
        {
            string query = $"SELECT 'OWTR', \"U_SYP_MDTD\", \"U_SYP_MDSD\", \"U_SYP_MDCD\", TO_CHAR(\"DocDate\", 'YYYY-MM-DD'), TO_CHAR(\"U_BPP_FECINITRA\", 'YYYY-MM-DD'),'G',null FROM {uti.schemaHana}OWTR WHERE \"U_SYP_MDTD\" || '-' ||\"U_SYP_MDSD\" || '-' || \"U_SYP_MDCD\" = '{NumAtCard}'";
            return EjecutarConsultaComprobante(query).FirstOrDefault();
        }
        //
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
        public List<Comprobante_E> ObtenerEncabezadoGuias(int DocEntry)
        {
            string guiasTicket = string.Empty;
            List<Comprobante_E> lista = new List<Comprobante_E>();
            Ventas_DAO.Tablas.ORDR_D ordrD = new Ventas_DAO.Tablas.ORDR_D();
            Ventas_DAO.Tablas.ODLN_D odln = new Ventas_DAO.Tablas.ODLN_D();
            OINV_D oinv = new OINV_D();
            SqlConnection cn = new SqlConnection(uti.cadSql);

            try
            {
                cn.Open();
                string query = "SELECT NroSap FROM vt.rtv2 WHERE DocEntry = @DocEntry";
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                    {
                        var guiaEncontrada= ordrD.guiasTraslado(dr.GetInt32(0));
                        if (!string.IsNullOrEmpty(guiaEncontrada) && guiaEncontrada.Trim()!=",") { guiasTicket += guiaEncontrada; }
                    }
                }

                // Separamos las guías del concatenado y buscamos su detalle
                List<string> itemGuias = guiasTicket.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (var numAtCard in itemGuias)
                {
                    Comprobante_E obj = obtenerGuiaRemisionODLN(numAtCard);

                    if (obj != null && string.IsNullOrEmpty(obj.U_SYP_MDCD))
                    {
                        obj = obtenerGuiaRemisionOINV(numAtCard);
                    }

                    else if (obj != null)
                    {
                        lista.Add(obj);
                    }
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                if (cn.State == System.Data.ConnectionState.Open)
                {
                    cn.Close();
                }
            }

            return lista;
        }
        public List<Comprobante_E> ObtenerEncabezadoGuiasTransferencia(int DocNum, string WhsCode)
        {
            var lista = new List<Comprobante_E>();
            var itemGuias = new List<string>();

            string query = "SELECT top 100 IFNULL(T0.\"U_SYP_MDTD\" || '-' || T0.\"U_SYP_MDSD\" || '-' || T0.\"U_SYP_MDCD\", '') as \"GUIAS\" " +
                           $"FROM {uti.schemaHana}OWTR T0 WHERE T0.\"CANCELED\" = 'N' AND T0.\"U_SYP_MDTD\" IS NOT NULL AND T0.\"U_SYP_MDSD\" IS NOT NULL " +
                           $"AND T0.\"U_SYP_MDCD\" IS NOT NULL AND T0.\"ToWhsCode\" ='{WhsCode}' AND T0.\"U_COB_LUGAREN\" ='{WhsCode}' " +
                           $"AND T0.\"Comments\" like '%{DocNum}%' ORDER BY T0.\"DocEntry\" desc";

            try
            {
                using (var cn = new HanaConnection(uti.cadHana))
                {
                    cn.Open();
                    using (var cmd = new HanaCommand(query, cn))
                    {
                        cmd.CommandType = System.Data.CommandType.Text;

                        using (var dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                if (!dr.IsDBNull(0))
                                {
                                    var guia = dr.GetString(0);
                                    if (!string.IsNullOrEmpty(guia))
                                    {
                                        itemGuias.Add(guia);
                                    }
                                }
                            }
                        }
                    }
                }

               
                foreach (var numAtCard in itemGuias)
                {
                    var obj = obtenerGuiaRemisionOWTR(numAtCard);
                    if (!string.IsNullOrEmpty(obj.U_SYP_MDCD))
                    {
                        lista.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return lista;
        }
        public List<Comprobante_E> ObtenerEncabezadoFacturas(int DocEntryOrden,string LugarDestino)
        {
            string query = string.Empty;
            switch (LugarDestino)
            {
                case "Domicilio":
                case "Agencia":
                    //Consulta las entregas en tabla ODLN
                    query = " SELECT 'OINV',T4.\"U_SYP_MDTD\",T4.\"U_SYP_MDSD\",T4.\"U_SYP_MDCD\",to_char(T4.\"DocDate\",'YYYY-MM-DD'),to_char(T4.\"U_BPP_FECINITRA\",'YYYY-MM-DD'),'F',T4.\"DocTotal\"  FROM " + uti.schemaHana + "ODLN T0 " +
                            " INNER JOIN " + uti.schemaHana + "DLN1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\"" +
                            " INNER JOIN " + uti.schemaHana + "RDR1 T2 ON T2.\"DocEntry\" = T1.\"BaseEntry\" AND T2.\"ObjType\" = T1.\"BaseType\"" +
                            " AND T2.\"LineNum\" = T1.\"BaseLine\" AND T2.\"DocEntry\" = " + DocEntryOrden +
                            " INNER JOIN " + uti.schemaHana + "INV1 T3 ON T3.\"BaseEntry\" = T1.\"DocEntry\" AND T3.\"BaseType\" = T1.\"ObjType\"" +
                            " AND T3.\"BaseLine\" = T1.\"LineNum\" " +
                            " INNER JOIN " + uti.schemaHana + "OINV T4 ON T4.\"DocEntry\" = T3.\"DocEntry\" AND T4.\"CANCELED\" = 'N' " +
                            " WHERE T0.\"CANCELED\" = 'N' GROUP BY  T4.\"U_SYP_MDTD\",T4.\"U_SYP_MDSD\",T4.\"U_SYP_MDCD\",to_char(T4.\"DocDate\",'YYYY-MM-DD'),to_char(T4.\"U_BPP_FECINITRA\",'YYYY-MM-DD'),T4.\"DocTotal\"";
                    break;
                case "Arriola":
                case "Centro":
                    //Consulta las facturas en tabla OINV
                    query = "SELECT 'OINV',T0.\"U_SYP_MDTD\",T0.\"U_SYP_MDSD\",T0.\"U_SYP_MDCD\",to_char(T0.\"DocDate\",'YYYY-MM-DD'),to_char(T0.\"U_BPP_FECINITRA\",'YYYY-MM-DD'),'F',T0.\"DocTotal\"  FROM " + uti.schemaHana + "OINV T0 " +
                            " INNER JOIN " + uti.schemaHana + "INV1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\"" +
                            " INNER JOIN " + uti.schemaHana + "RDR1 T2 ON T2.\"DocEntry\" = T1.\"BaseEntry\" AND T2.\"ObjType\" = T1.\"BaseType\"" +
                            " AND T2.\"LineNum\" = T1.\"BaseLine\" AND T2.\"DocEntry\" =" + DocEntryOrden +
                            " WHERE T0.\"CANCELED\" = 'N' GROUP BY T0.\"U_SYP_MDTD\",T0.\"U_SYP_MDSD\",T0.\"U_SYP_MDCD\",to_char(T0.\"DocDate\",'YYYY-MM-DD'),to_char(T0.\"U_BPP_FECINITRA\",'YYYY-MM-DD'),T0.\"DocTotal\" ";
                    break;
            }

            List<Comprobante_E> lista = EjecutarConsultaComprobante(query);
            return lista;
        }
        public List<Comprobante_E> ObtenerEncabezadoNotaCredito(List<RTV4_E> NotasCredito, string FacturasConcatenadas)
        {
            String AndWhere = string.Empty;
            List<int> listDocNumNotas = new List<int>();
            if (NotasCredito != null && NotasCredito.Count() > 0)
            {
                foreach (var NotaCredito in NotasCredito)
                {
                    listDocNumNotas.Add(NotaCredito.Nc.DocNum);
                }
            }
            string DocNumNotasConcatenadas = string.Join(", ", listDocNumNotas);
            if (DocNumNotasConcatenadas.Trim().Length > 0)
            {AndWhere = $" OR \"DocNum\" in ({DocNumNotasConcatenadas}) "; }

            string query = $"SELECT 'ORIN',\"U_SYP_MDTD\",\"U_SYP_MDSD\",\"U_SYP_MDCD\", TO_CHAR(\"DocDate\",'YYYY-MM-DD'),to_char(\"U_BPP_FECINITRA\", 'YYYY-MM-DD'),'NC',\"DocTotal\"  from {uti.schemaHana}ORIN where (\"U_SYP_MDTO\" || '-' || \"U_SYP_MDSO\" || '-' || \"U_SYP_MDCO\") IN ('{FacturasConcatenadas}') {AndWhere}";
            List<Comprobante_E> lista = EjecutarConsultaComprobante(query);
            return lista;
        }
        public List<Comprobante_E> ObtenerEncabezadoNotaDebito(int DocNum, string FacturasConcatenadas)
        {
            string query = $"SELECT 'OINV', \"U_SYP_MDTD\", \"U_SYP_MDSD\", \"U_SYP_MDCD\", to_char(\"DocDate\", 'YYYY-MM-DD'), to_char(\"U_BPP_FECINITRA\", 'YYYY-MM-DD'), 'ND', \"DocTotal\" FROM {uti.schemaHana}OINV WHERE \"U_SYP_MDTD\" = '08' AND (\"U_SYP_MDTO\" || '-' || \"U_SYP_MDSO\" || '-' || \"U_SYP_MDCO\") IN ('{FacturasConcatenadas}')";
            List<Comprobante_E> lista = EjecutarConsultaComprobante(query);
            return lista;
        }

    }
}
