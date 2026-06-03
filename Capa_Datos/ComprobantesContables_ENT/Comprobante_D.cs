using Capa_Datos.Ventas_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.ComprobantesContables_ENT;
using Capa_Entidad.ReportesDigemid_ENT.Reportes;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.TablasSql;
using DocumentFormat.OpenXml.Math;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Resources.ResXFileRef;

namespace Capa_Datos.ComprobantesContables_ENT
{
    public class Comprobante_D
    {
        readonly Utilitarios uti = new Utilitarios();
        readonly DBHelper db = new DBHelper();

        private decimal ObtenerDocTotal(int DocEntryOrden)
        {
            decimal docTotal = 0;
            HanaConnection cn = new HanaConnection(uti.cadHana);

            try
            {
                string query = $@"SELECT T0.""DocTotal"" FROM {uti.schemaHana}ODLN T0 
                    INNER JOIN {uti.schemaHana}DLN1 T1 ON T1.""DocEntry"" = T0.""DocEntry""
                    INNER JOIN {uti.schemaHana}RDR1 T2 ON T2.""DocEntry"" = T1.""BaseEntry""                    
                    WHERE T0.""CANCELED"" = 'N' AND T2.""ObjType"" = T1.""BaseType"" AND T2.""LineNum"" = T1.""BaseLine"" AND T2.""DocEntry"" = {DocEntryOrden}
                    GROUP BY T0.""DocTotal""";

                cn.Open();
                HanaCommand cmd = new HanaCommand(query, cn)
                {
                    CommandType = System.Data.CommandType.Text
                };
                HanaDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (!dr.IsDBNull(0)) { docTotal = dr.GetDecimal(0); }
                }

                dr.Close();
            }
            catch { }
            finally
            {
                cn.Close();
            }
            return docTotal;
        }
        public List<int> ObtenerDocEntryOV(List<RTV2_E> det2List, bool excluirCero)
        {
            List<int> ordenes = new List<int>(); string filtro = string.Empty;
            if (excluirCero) { filtro = "AND \"DocTotal\" > 0 "; }
            foreach (var ordr in det2List)
            {
                string query = $"SELECT TOP 100 \"DocEntry\" FROM {uti.schemaHana}ORDR WHERE \"DocNum\" = '{ordr.NroSap}' AND \"CANCELED\" = 'N' {filtro}";
                using (HanaConnection cn = new HanaConnection(uti.cadHana))
                {
                    try
                    {
                        cn.Open();
                        using (HanaCommand cmd = new HanaCommand(query, cn))
                        {
                            using (HanaDataReader dr = cmd.ExecuteReader())
                            {
                                if (dr.Read() && !dr.IsDBNull(0))
                                {
                                    ordenes.Add(dr.GetInt32(0));
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
            return ordenes;
        }
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
                    if (!dr.IsDBNull(8)) { obj.AnticipoBruto = dr.GetDecimal(8); }
                    lista.Add(obj);
                }
                dr.Close();
            }
            catch { }
            finally
            {
                cn.Close();
            }
            return lista;
        }
        //SEGUNDA CONSULTA DETALLE DE CADA DOCUMENTO PARA LAYOUT
        public List<Guia_Remision_E> ObtenerCabeceraGuia(string NumAtCard, string Tabla)
        {
            List<Guia_Remision_E> lista = new List<Guia_Remision_E>();
            string query = string.Empty;
            switch (Tabla)
            {
                //TRANSFERENCIA
                case "OWTR":
                    query = $"SELECT (SELECT \"CompnyName\" FROM {uti.schemaHana}\"OADM\"),\"DocDate\",(SELECT \"TaxIdNum\" FROM {uti.schemaHana}\"OADM\"),\"U_BPP_FECINITRA\",\"U_SYP_MDTD\" || '-' ||\"U_SYP_MDSD\" || '-' || \"U_SYP_MDCD\" FROM {uti.schemaHana}OWTR WHERE \"U_SYP_MDTD\" || '-' ||\"U_SYP_MDSD\" || '-' || \"U_SYP_MDCD\"='{NumAtCard}'";
                    try
                    {
                        HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                        if (hdr.HasRows)
                        {
                            while (hdr.Read())
                            {
                                Guia_Remision_E c = new Guia_Remision_E();
                                c.NombreBD = "Cobefar S.A.C.";
                                if (!hdr.IsDBNull(1)) { c.DocDate = Convert.ToDateTime(hdr.GetString(1)).ToString("dd/MM/yyyy"); }
                                if (!hdr.IsDBNull(2)) { c.RucBD = hdr.GetString(2); }
                                if (!hdr.IsDBNull(3)) { c.FechaTrasl = Convert.ToDateTime(hdr.GetString(3)).ToString("dd/MM/yyyy"); }
                                if (!hdr.IsDBNull(4)) { c.NumAtCard = hdr.GetString(4); }

                                lista.Add(c);
                            }
                        }
                        hdr.Close();
                    }
                    catch (Exception e) { throw new Exception(e.Message); }
                    break;
                //ENTREGA
                case "ODLN":
                    query = $"SELECT \"CardName\",\"DocDate\",\"CardCode\",\"U_BPP_FECINITRA\",\"NumAtCard\" FROM {uti.schemaHana}ODLN WHERE \"NumAtCard\"='{NumAtCard}'";
                    try
                    {
                        HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                        if (hdr.HasRows)
                        {
                            while (hdr.Read())
                            {
                                Guia_Remision_E c = new Guia_Remision_E();
                                if (!hdr.IsDBNull(0)) { c.CardName = hdr.GetString(0); }
                                if (!hdr.IsDBNull(1)) { c.DocDate = Convert.ToDateTime(hdr.GetString(1)).ToString("dd/MM/yyyy"); }
                                if (!hdr.IsDBNull(2)) { c.CardCode = hdr.GetString(2); }
                                if (!hdr.IsDBNull(3)) { c.FechaTrasl = Convert.ToDateTime(hdr.GetString(3)).ToString("dd/MM/yyyy"); }
                                if (!hdr.IsDBNull(4)) { c.NumAtCard = hdr.GetString(4); }
                                lista.Add(c);
                            }
                        }
                        hdr.Close();
                    }
                    catch (Exception e) { throw new Exception(e.Message); }
                    break;
            }
            return lista;
        }
        public List<Guia_Remision_E> ObtenerDetalleGuia(string NumAtCard, string Tabla) // devuelve una guia con su detalle desnormalizado segun el tipo consulta a una tabla 
        {
            List<Guia_Remision_E> lista = new List<Guia_Remision_E>();
            string query = string.Empty;
            switch (Tabla)
            {
                case "OWTR":
                    query = $"CALL {uti.schemaHana} COBE_LYT_TS('{NumAtCard}')";
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
                                c.NombreBD = "Cobefar S.A.C.";
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
                    break;
                case "ODLN":
                    query = $"CALL {uti.schemaHana} COBE_LYT_EV('{NumAtCard}')";
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
                                if (!hdr.IsDBNull(27)) { c.TipoComprobantePago = hdr.GetString(27); }
                                if (!hdr.IsDBNull(28)) { c.NroComprobantePago = hdr.GetString(28); }
                                if (!hdr.IsDBNull(29)) { c.RucTransportista = hdr.GetString(29); }
                                if (!hdr.IsDBNull(30)) { c.NomTransportista = hdr.GetString(30); }
                                lista.Add(c);
                            }
                        }
                        hdr.Close();
                    }
                    catch (Exception e) { throw new Exception(e.Message); }
                    break;
            }

            return lista;
        }

        public List<ComprobanteDePago_E> ObtenerDetalleFactura(string NumAtCard) // devuelve una factura con su detalle desnormalizado segun el tipo consulta a una tabla 
        {
            List<ComprobanteDePago_E> lista = new List<ComprobanteDePago_E>();
            string query = $"CALL {uti.schemaHana} COBE_LYT_FB_2025('{NumAtCard}')";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                if (hdr.HasRows)
                {
                    while (hdr.Read())
                    {
                        ComprobanteDePago_E c = new ComprobanteDePago_E();

                        if (!hdr.IsDBNull(0)) { c.DocEntry = hdr.GetInt32(0); }
                        if (!hdr.IsDBNull(1)) { c.DocNum = hdr.GetInt32(1); }
                        if (!hdr.IsDBNull(2)) { c.ElaboradoPor = hdr.GetString(2); }
                        if (!hdr.IsDBNull(3)) { c.TipoDoc = hdr.GetString(3); }
                        if (!hdr.IsDBNull(4)) { c.SerieDoc = hdr.GetString(4); }
                        if (!hdr.IsDBNull(5)) { c.CorreDoc = hdr.GetString(5); }
                        if (!hdr.IsDBNull(6)) { c.NroOCCliente = hdr.GetString(6); }
                        if (!hdr.IsDBNull(7)) { c.TotalBase = hdr.GetString(7); }
                        if (!hdr.IsDBNull(8)) { c.NroAnticipo = hdr.GetString(8); }
                        if (!hdr.IsDBNull(9)) { c.Anticipo = Math.Round(hdr.GetDecimal(9), 2); }
                        if (!hdr.IsDBNull(10)) { c.FechaAnticipo = hdr.GetDateTime(10).ToString("dd/MM/yyyy"); }
                        if (!hdr.IsDBNull(11)) { c.AnticipoBruto = Math.Round(hdr.GetDecimal(11), 2); }
                        if (!hdr.IsDBNull(12)) { c.NumGuias = hdr.GetString(12); }
                        if (!hdr.IsDBNull(13)) { c.NombreSocio = hdr.GetString(13); }
                        if (!hdr.IsDBNull(14)) { c.DirPagar = hdr.GetString(14); }
                        if (!hdr.IsDBNull(15)) { c.Ruc = hdr.GetString(15); }
                        if (!hdr.IsDBNull(16)) { c.Fecha = hdr.GetDateTime(16).ToString("dd/MM/yyyy"); }
                        if (!hdr.IsDBNull(17)) { c.FechaVencimiento = hdr.GetDateTime(17).ToLongDateString(); }
                        if (!hdr.IsDBNull(18)) { c.MonedaLetras = hdr.GetString(18); }
                        if (!hdr.IsDBNull(19)) { c.ItemCode = hdr.GetString(19); }
                        if (!hdr.IsDBNull(20)) { c.Descripcion = hdr.GetString(20); }
                        if (!hdr.IsDBNull(21)) { c.DocNumTicket = hdr.GetString(21); }
                        if (!hdr.IsDBNull(22)) { c.Um = hdr.GetString(22); } // es fraccion o no 
                        if (!hdr.IsDBNull(23)) { c.Cantidad = Math.Round(hdr.GetDecimal(23), 0); } //cantidad total por sku
                        if (!hdr.IsDBNull(24)) { c.PreUnitSinIgv = hdr.GetDecimal(24); }
                        if (!hdr.IsDBNull(25)) { c.Descuento = hdr.GetDecimal(25); }
                        if (!hdr.IsDBNull(26)) { c.PreVentaNeto = hdr.GetDecimal(26); } // Precio unitario con IGV
                        if (!hdr.IsDBNull(27)) { c.PrecioVenta = hdr.GetDecimal(27); } // Precio de venta por SKU 
                        if (!hdr.IsDBNull(28)) { c.FechaEntrega = hdr.GetDateTime(28).ToString("dd/MM/yyyy"); }
                        if (!hdr.IsDBNull(29)) { c.Impuesto = hdr.GetDecimal(29); }
                        if (!hdr.IsDBNull(30)) { c.DocTotal = hdr.GetDecimal(30); }
                        if (!hdr.IsDBNull(31)) { c.PorcenImpto = hdr.GetDecimal(31); }
                        if (!hdr.IsDBNull(32)) { c.LoteNum = hdr.GetString(32); }
                        if (!hdr.IsDBNull(33)) { c.CantidadL = hdr.GetDecimal(33); }
                        if (!hdr.IsDBNull(34)) { c.TieneAnticipo = hdr.GetInt32(34); }
                        if (!hdr.IsDBNull(35)) { c.Laboratorio = hdr.GetString(35); }
                        if (!hdr.IsDBNull(36)) { c.VctoLote = hdr.GetDateTime(36).ToString("dd/MM/yyyy"); }
                        if (!hdr.IsDBNull(37)) { c.QUMVta = Math.Round(hdr.GetDecimal(37), 0); }
                        if (!hdr.IsDBNull(38)) { c.CondPago = hdr.GetString(38); }
                        if (!hdr.IsDBNull(39)) { c.NroOrdVenta = hdr.GetString(39); }
                        if (!hdr.IsDBNull(40)) { c.CodImpuesto = hdr.GetString(40); }
                        if (!hdr.IsDBNull(41)) { c.Almacen = hdr.GetString(41); }
                        if (!hdr.IsDBNull(42)) { c.PtoPartida = hdr.GetString(42); }
                        if (!hdr.IsDBNull(43)) { c.DirEnvio = hdr.GetString(43); }


                        lista.Add(c);
                    }

                }
                hdr.Close();
            }
            catch (Exception e) { throw new Exception(e.Message); }
            return lista;
        }
        public List<ComprobanteDePago_E> ObtenerCabeceraFactura(string NumAtCard) // devuelve la factura solo con los requeridos para la cabecera
        {
            List<ComprobanteDePago_E> lista = new List<ComprobanteDePago_E>();
            string query = $"SELECT \"U_SYP_MDSD\" , \"U_SYP_MDCD\",\"U_SYP_NOCCLIENTE\",\"CardName\",\"Address\", (select \"LicTradNum\" from {uti.schemaHana}OCRD where \"CardCode\"={uti.schemaHana}OINV.\"CardCode\"),\"DocDate\" ,(SELECT distinct \"WhsCode\" FROM {uti.schemaHana}INV1 WHERE \"DocEntry\"={uti.schemaHana}OINV.\"DocEntry\") FROM {uti.schemaHana}OINV WHERE  \"NumAtCard\" = '{NumAtCard}'";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                if (hdr.HasRows)
                {
                    while (hdr.Read())
                    {
                        ComprobanteDePago_E c = new ComprobanteDePago_E();

                        if (!hdr.IsDBNull(0)) { c.SerieDoc = hdr.GetString(0); }
                        if (!hdr.IsDBNull(1)) { c.CorreDoc = hdr.GetString(1); }
                        if (!hdr.IsDBNull(2)) { c.NroOCCliente = hdr.GetString(2); }
                        if (!hdr.IsDBNull(3)) { c.NombreSocio = hdr.GetString(3); }
                        if (!hdr.IsDBNull(4)) { c.DirPagar = hdr.GetString(4); }
                        if (!hdr.IsDBNull(5)) { c.Ruc = hdr.GetString(5); }
                        if (!hdr.IsDBNull(6)) { c.Fecha = hdr.GetDateTime(6).ToString("dd/MM/yyyy"); }
                        if (!hdr.IsDBNull(7)) { c.Almacen = hdr.GetString(7); }

                        lista.Add(c);
                    }

                }
                hdr.Close();
            }
            catch (Exception e) { throw new Exception(e.Message); }
            return lista;
        }
        //PRIMER CONSULTA SOLO CABECERAS 
        //Guias de Domicilio y Agencia
        public List<Comprobante_E> ObtenerEncabezadoGuiasPorEntrega(List<int> listDocEntrySap)
        {
            List<Comprobante_E> lista = new List<Comprobante_E>();
            Ventas_DAO.Tablas.ODLN_D odln = new Ventas_DAO.Tablas.ODLN_D();
            try
            {
                if (listDocEntrySap != null && listDocEntrySap.Count > 0)
                {
                    string guiasConcatenadas = string.Empty;
                    string docEntryList = string.Join(",", listDocEntrySap);
                    HanaConnection cn = new HanaConnection(uti.cadHana);

                    string query = $@"SELECT STRING_AGG(t.""FormatearNumAtCard"", ',') AS GuiasConcatenadas
                    FROM(
                        SELECT DISTINCT 
                            CASE 
                                WHEN t0.""NumAtCard"" IS NOT NULL AND t0.""NumAtCard"" <> ''
                                THEN '''' || t0.""NumAtCard"" || ''''
                                ELSE NULL
                            END AS ""FormatearNumAtCard""
                        FROM {uti.schemaHana}ODLN t0
                        INNER JOIN {uti.schemaHana}DLN1 t1
                            ON t1.""DocEntry"" = t0.""DocEntry""
                        INNER JOIN {uti.schemaHana}RDR1 t2
                            ON t2.""DocEntry"" = t1.""BaseEntry""
                            AND t2.""ObjType"" = t1.""BaseType""
                            AND t2.""ItemCode"" = t1.""ItemCode""
                        WHERE t0.""CANCELED"" = 'N'
                          AND t2.""DocEntry"" IN({docEntryList})
                    ) AS t WHERE t.""FormatearNumAtCard"" IS NOT NULL";

                    try
                    {
                        cn.Open();
                        HanaCommand cmd = new HanaCommand(query, cn);
                        cmd.CommandType = System.Data.CommandType.Text;
                        HanaDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            guiasConcatenadas = dr.GetString(0);
                        }
                        dr.Close();
                        cn.Close();
                    }
                    catch { cn.Close(); }

                    query = $"SELECT 'ODLN', \"U_SYP_MDTD\", \"U_SYP_MDSD\", \"U_SYP_MDCD\", TO_CHAR(\"DocDate\", 'YYYY-MM-DD'), TO_CHAR(\"U_BPP_FECINITRA\", 'YYYY-MM-DD'),'G',\"Max1099\",null FROM {uti.schemaHana}ODLN WHERE \"CANCELED\" = 'N' AND \"NumAtCard\" in({guiasConcatenadas})";

                    lista = EjecutarConsultaComprobante(query);

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }

            return lista;
        }
        //Guias de Centro y Arriola
        public List<Comprobante_E> ObtenerEncabezadoGuiasTransferencia(ORTV_E obj)
        {
            var lista = new List<Comprobante_E>();
            string guiasConcatenadas = string.Empty;
            if (obj != null)
            {
                HanaConnection cn = new HanaConnection(uti.cadHana);

                string query = $"SELECT STRING_AGG(t.\"FormatearGuia\",',') AS GuiasConcatenadas FROM (SELECT DISTINCT CASE WHEN T0.\"U_SYP_MDTD\" || '-' || T0.\"U_SYP_MDSD\" || '-' || T0.\"U_SYP_MDCD\" IS NOT NULL AND T0.\"U_SYP_MDTD\" || '-' || T0.\"U_SYP_MDSD\" || '-' || T0.\"U_SYP_MDCD\" <> '' THEN ''''||T0.\"U_SYP_MDTD\" || '-' || T0.\"U_SYP_MDSD\" || '-' || T0.\"U_SYP_MDCD\"||'''' ELSE NULL END AS \"FormatearGuia\" FROM {uti.schemaHana}OWTR T0 WHERE T0.\"CANCELED\" = 'N' AND T0.\"U_SYP_MDTD\" IS NOT NULL AND T0.\"U_SYP_MDSD\" IS NOT NULL AND T0.\"U_SYP_MDCD\" IS NOT NULL AND T0.\"ToWhsCode\" ='{(obj.LugarDestino.Equals("Arriola") ? "09" : "01")}' AND T0.\"U_COB_LUGAREN\" ='{(obj.LugarDestino.Equals("Arriola") ? "09" : "01")}' AND T0.\"CardCode\" ='{obj.CardCode}' AND T0.\"Comments\" like '%{obj.DocNum}%'  ORDER BY T0.\"DocEntry\" desc ) AS t WHERE t.\"FormatearGuia\" IS NOT NULL";

                try
                {
                    cn.Open();
                    HanaCommand cmd = new HanaCommand(query, cn);
                    cmd.CommandType = System.Data.CommandType.Text;
                    HanaDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        guiasConcatenadas = dr.GetString(0);
                    }
                    dr.Close();
                    cn.Close();
                }
                catch { cn.Close(); }

                query = $"SELECT 'OWTR', \"U_SYP_MDTD\", \"U_SYP_MDSD\", \"U_SYP_MDCD\", TO_CHAR(\"DocDate\", 'YYYY-MM-DD'), TO_CHAR(\"U_BPP_FECINITRA\", 'YYYY-MM-DD'),'G',null,null FROM {uti.schemaHana}OWTR WHERE \"U_SYP_MDTD\" || '-' ||\"U_SYP_MDSD\" || '-' || \"U_SYP_MDCD\" in ({guiasConcatenadas})";

                lista = EjecutarConsultaComprobante(query);
            }

            return lista;
        }
        public List<Comprobante_E> ObtenerEncabezadoFacturas(int DocEntryOrden, string LugarDestino)
        {
            string query = string.Empty;
            if ((LugarDestino.Equals("DOMICILIO") || LugarDestino.Equals("PROVINCIA")) && ObtenerDocTotal(DocEntryOrden) > 0)
            {
                //Consulta las entregas en tabla ODLN -- factura costo 0 no tienen entregas
                query = $" SELECT DISTINCT 'OINV',T4.\"U_SYP_MDTD\",T4.\"U_SYP_MDSD\",T4.\"U_SYP_MDCD\",to_char(T4.\"DocDate\",'YYYY-MM-DD'),to_char(T4.\"U_BPP_FECINITRA\",'YYYY-MM-DD'),'F',T4.\"DocTotal\",T5.\"Gross\" FROM {uti.schemaHana}ODLN T0 INNER JOIN {uti.schemaHana}DLN1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" INNER JOIN {uti.schemaHana}RDR1 T2 ON T2.\"DocEntry\" = T1.\"BaseEntry\" AND T2.\"ObjType\" = T1.\"BaseType\" AND T2.\"LineNum\" = T1.\"BaseLine\" AND T2.\"DocEntry\" = {DocEntryOrden} INNER JOIN {uti.schemaHana}INV1 T3 ON T3.\"BaseEntry\" = T1.\"DocEntry\" AND T3.\"BaseType\" = T1.\"ObjType\" AND T3.\"BaseLine\" = T1.\"LineNum\" INNER JOIN {uti.schemaHana}OINV T4 ON T4.\"DocEntry\" = T3.\"DocEntry\" AND T4.\"CANCELED\" = 'N' LEFT JOIN {uti.schemaHana}INV9 T5 ON  T4.\"DocEntry\" = T5.\"DocEntry\" WHERE T0.\"CANCELED\" = 'N'";
            }
            else
            {
                //Consulta las facturas en tabla OINV
                query = $" SELECT DISTINCT 'OINV',T0.\"U_SYP_MDTD\",T0.\"U_SYP_MDSD\",T0.\"U_SYP_MDCD\",to_char(T0.\"DocDate\",'YYYY-MM-DD'),to_char(T0.\"U_BPP_FECINITRA\",'YYYY-MM-DD'),'F',T0.\"DocTotal\",(select sum(\"Gross\") FROM {uti.schemaHana}INV9  WHERE \"DocEntry\"= T0.\"DocEntry\" ) FROM {uti.schemaHana}OINV T0  INNER JOIN {uti.schemaHana}INV1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" INNER JOIN {uti.schemaHana}RDR1 T2 ON T2.\"DocEntry\" = T1.\"BaseEntry\" AND T2.\"ObjType\" = T1.\"BaseType\" AND T2.\"LineNum\" = T1.\"BaseLine\" AND T2.\"DocEntry\" ={DocEntryOrden} WHERE T0.\"CANCELED\" = 'N'";

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
            { AndWhere = $" OR \"DocNum\" in ({DocNumNotasConcatenadas}) "; }

            string query = $"SELECT 'ORIN',\"U_SYP_MDTD\",\"U_SYP_MDSD\",\"U_SYP_MDCD\", TO_CHAR(\"DocDate\",'YYYY-MM-DD'),to_char(\"U_BPP_FECINITRA\", 'YYYY-MM-DD'),'NC',\"DocTotal\",null from {uti.schemaHana}ORIN where (\"U_SYP_MDTO\" || '-' || \"U_SYP_MDSO\" || '-' || \"U_SYP_MDCO\") IN ({FacturasConcatenadas}) {AndWhere}";
            List<Comprobante_E> lista = EjecutarConsultaComprobante(query);
            return lista;
        }
        public List<Comprobante_E> ObtenerEncabezadoNotaDebito(string FacturasConcatenadas)
        {
            string query = $"SELECT 'OINV', \"U_SYP_MDTD\", \"U_SYP_MDSD\", \"U_SYP_MDCD\", to_char(\"DocDate\", 'YYYY-MM-DD'), to_char(\"U_BPP_FECINITRA\", 'YYYY-MM-DD'), 'ND', \"DocTotal\",null FROM {uti.schemaHana}OINV WHERE \"U_SYP_MDTD\" = '08' AND (\"U_SYP_MDTO\" || '-' || \"U_SYP_MDSO\" || '-' || \"U_SYP_MDCO\") IN ({FacturasConcatenadas})";
            List<Comprobante_E> lista = EjecutarConsultaComprobante(query);
            return lista;
        }

    }
}
