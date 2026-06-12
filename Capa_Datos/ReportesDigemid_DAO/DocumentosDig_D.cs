using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Capa_Entidad.General_ENT.Tablas;
using Capa_Entidad.ReportesDigemid_ENT;
using Capa_Entidad.ReportesDigemid_ENT.Formularios;
using Capa_Entidad.ReportesDigemid_ENT.Reportes;
using Sap.Data.Hana;

namespace Capa_Datos.ReportesDigemid_DAO
{
    public class DocumentosDig_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public List<ActaRecepcionVt_E> ConsultarActaRecepcionVt(int DocEntry)
        {
            List<ActaRecepcionVt_E> lista = new List<ActaRecepcionVt_E>();
            string query = "call " + uti.schemaHana + "DIEGO_COBEFAR_ACTARECEPCION_VTAS(" + DocEntry + ")";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    ActaRecepcionVt_E a = new ActaRecepcionVt_E();
                    if (!hdr.IsDBNull(0)) { a.T0_ObjType = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { a.T0_DocEntry = hdr.GetInt32(1); }
                    if (!hdr.IsDBNull(2)) { a.T0_DocNum = hdr.GetInt32(2); }
                    if (!hdr.IsDBNull(3)) { a.T0_CardCode = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { a.T0_CardName = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { a.T0_DocDate = hdr.GetDateTime(5).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(6)) { a.T0_NumAtCard = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { a.T0_TaxDate = hdr.GetDateTime(7).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(8)) { a.T1_LineNum = hdr.GetInt32(8); }
                    if (!hdr.IsDBNull(9)) { a.T1_ItemCode = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { a.CodAlmacen = hdr.GetString(10); }
                    if (!hdr.IsDBNull(11)) { a.T8_ItemName = hdr.GetString(11); }
                    if (!hdr.IsDBNull(12)) { a.T8_FrgnName = hdr.GetString(12); }
                    if (!hdr.IsDBNull(13)) { a.Concentracion = hdr.GetString(13); }
                    if (!hdr.IsDBNull(14)) { a.FormaPresentacion = hdr.GetString(14); }
                    if (!hdr.IsDBNull(15)) { a.FormaFarmaceutica = hdr.GetString(15); }
                    if (!hdr.IsDBNull(16)) { a.Fabricante = hdr.GetString(16); }
                    if (!hdr.IsDBNull(17)) { a.T4_AbsEntry = hdr.GetInt32(17); }
                    if (!hdr.IsDBNull(18)) { a.T4_DistNumber = hdr.GetString(18); }
                    if (!hdr.IsDBNull(19)) { a.T4_MnfSerial = hdr.GetString(19); }
                    if (!hdr.IsDBNull(20)) { a.T4_ExpDate = hdr.GetDateTime(20).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(21)) { a.Quantity = hdr.GetDecimal(21); }
                    if (!hdr.IsDBNull(22)) { a.T7_Location = hdr.GetString(22); }
                    if (!hdr.IsDBNull(23)) { a.T0_U_COB_TIPODOC = hdr.GetString(23); }
                    if (!hdr.IsDBNull(24)) { a.T0_U_COB_SERIE = hdr.GetString(24); }
                    if (!hdr.IsDBNull(25)) { a.T0_U_COB_CORDOC = hdr.GetString(25); }
                    if (!hdr.IsDBNull(26)) { a.TaxOffice = hdr.GetString(26); }
                    if (!hdr.IsDBNull(27)) { a.Almacen = hdr.GetString(27); }
                    lista.Add(a);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public List<ActaDespachoVt_E> ConsultarActaDespachoVt(int DocEntry)
        {
            List<ActaDespachoVt_E> lista = new List<ActaDespachoVt_E>();
            string query = "call " + uti.schemaHana + "DIEGO_COBEFAR_ACTADESPACHO(" + DocEntry + ")";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    ActaDespachoVt_E a = new ActaDespachoVt_E();
                    if (!hdr.IsDBNull(0)) { a.T0_ObjType = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { a.T0_DocEntry = hdr.GetInt32(1); }
                    if (!hdr.IsDBNull(2)) { a.T0_DocNum = hdr.GetInt32(2); }
                    if (!hdr.IsDBNull(3)) { a.T0_CardCode = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { a.T0_CardName = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { a.T0_DocDate = hdr.GetDateTime(5).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(6)) { a.T0_NumAtCard = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { a.T0_TaxDate = hdr.GetDateTime(7).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(8)) { a.T1_LineNum = hdr.GetInt32(8); }
                    if (!hdr.IsDBNull(9)) { a.T1_ItemCode = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { a.T1_WhsCode = hdr.GetString(10); }
                    if (!hdr.IsDBNull(11)) { a.T8_ItemName = hdr.GetString(11); }
                    if (!hdr.IsDBNull(12)) { a.T8_FrgnName = hdr.GetString(12); }
                    if (!hdr.IsDBNull(13)) { a.Concentracion = hdr.GetString(13); }
                    if (!hdr.IsDBNull(14)) { a.FormaPresentacion = hdr.GetString(14); }
                    if (!hdr.IsDBNull(15)) { a.FormaFarmaceutica = hdr.GetString(15); }
                    if (!hdr.IsDBNull(16)) { a.Fabricante = hdr.GetString(16); }
                    if (!hdr.IsDBNull(17)) { a.T4_AbsEntry = hdr.GetInt32(17); }
                    if (!hdr.IsDBNull(18)) { a.T4_DistNumber = hdr.GetString(18); }
                    if (!hdr.IsDBNull(19)) { a.T4_MnfSerial = hdr.GetString(19); }
                    if (!hdr.IsDBNull(20)) { a.T4_ExpDate = hdr.GetDateTime(20).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(21)) { a.Quantity = hdr.GetDecimal(21); }
                    if (!hdr.IsDBNull(22)) { a.T7_Location = hdr.GetString(22); }
                    if (!hdr.IsDBNull(23)) { a.T0_U_COB_TIPODOC = hdr.GetString(23); }
                    if (!hdr.IsDBNull(24)) { a.T0_U_COB_SERIE = hdr.GetString(24); }
                    if (!hdr.IsDBNull(25)) { a.T0_U_COB_CORDOC = hdr.GetString(25); }

                    lista.Add(a);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public List<OrganolepticoVt_E> ConsultarOrganolepticoVt(int DocEntry)
        {
            List<OrganolepticoVt_E> lista = new List<OrganolepticoVt_E>();
            string query = "call " + uti.schemaHana + "DISFAR_ORGA_VTAS(" + DocEntry + ")";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    OrganolepticoVt_E a = new OrganolepticoVt_E();
                    if (!hdr.IsDBNull(0)) { a.NroFactura = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { a.NroGuia = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { a.FechaContab = hdr.GetDateTime(2).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(3)) { a.Cliente = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { a.CodAlmacen = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { a.CodUnidMed = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { a.CantidadL = hdr.GetDecimal(6); }
                    if (!hdr.IsDBNull(7)) { a.Lote = hdr.GetString(7); }
                    if (!hdr.IsDBNull(8)) { a.ResgistroS = hdr.GetString(8); }
                    if (!hdr.IsDBNull(9)) { a.FechaVenc = hdr.GetDateTime(9).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(10)) { a.Nombre = hdr.GetString(10); }
                    if (!hdr.IsDBNull(11)) { a.Concentracion = hdr.GetString(11); }
                    if (!hdr.IsDBNull(12)) { a.FormaPresentacion = hdr.GetString(12); }
                    if (!hdr.IsDBNull(13)) { a.FormaFarmaceutica = hdr.GetString(13); }
                    if (!hdr.IsDBNull(14)) { a.Fabricante = hdr.GetString(14); }
                    if (!hdr.IsDBNull(15)) { a.CondicionAl = hdr.GetString(15); }
                    if (!hdr.IsDBNull(16)) { a.TaxOfficeAlmacen = hdr.GetString(16); }
                    if (!hdr.IsDBNull(17)) { a.Almacen = hdr.GetString(17); }
                    if (!hdr.IsDBNull(18)) { a.ItemName = hdr.GetString(18); }
                    lista.Add(a);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public List<ActaRecepcionTs_E> ConsultarActaRecepcionTs(int DocEntry)
        {
            List<ActaRecepcionTs_E> lista = new List<ActaRecepcionTs_E>();
            string query = "call " + uti.schemaHana + "DIEGO_COBEFAR_ACTARECEPCION_TS(" + DocEntry + ")";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query); 
                while (hdr.Read())
                {
                    ActaRecepcionTs_E a = new ActaRecepcionTs_E();
                    if (!hdr.IsDBNull(0)) { a.T1_DocDate = hdr.GetDateTime(0).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(1)) { a.CodAlmacenEnvio = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { a.CodAlmacenDestino = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { a.NroDeGuia = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { a.T2_ItemCode = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { a.T2_Dscription = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { a.T3_Quantity = hdr.GetDecimal(6); }
                    if (!hdr.IsDBNull(7)) { a.T8_ItemName = hdr.GetString(7); }
                    if (!hdr.IsDBNull(8)) { a.T8_FrgnName = hdr.GetString(8); }
                    if (!hdr.IsDBNull(9)) { a.Concentracion = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { a.FormaPresentacion = hdr.GetString(10); }
                    if (!hdr.IsDBNull(11)) { a.FormaFarmaceutica = hdr.GetString(11); }
                    if (!hdr.IsDBNull(12)) { a.Fabricante = hdr.GetString(12); }
                    if (!hdr.IsDBNull(13)) { a.Lote = hdr.GetString(13); }
                    if (!hdr.IsDBNull(14)) { a.FechaVenc = hdr.GetDateTime(14).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(15)) { a.RegistroSan = hdr.GetString(15); }
                    if (!hdr.IsDBNull(16)) { a.CondAlmac = hdr.GetString(16); }
                    if (!hdr.IsDBNull(17)) { a.TaxOfficeAlmacenEnvio = hdr.GetString(17); }
                    if (!hdr.IsDBNull(18)) { a.AlmacenEnvio = hdr.GetString(18); }
                    if (!hdr.IsDBNull(19)) { a.TaxOfficeAlmacenDestino = hdr.GetString(19); }
                    if (!hdr.IsDBNull(20)) { a.AlmacenDestino = hdr.GetString(20); }

                    lista.Add(a);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public List<ActaDespachoTs_E> ConsultarActaDespachoTs(int DocEntry)
        {
            List<ActaDespachoTs_E> lista = new List<ActaDespachoTs_E>();
            string query = "call " + uti.schemaHana + "DIEGO_COBEFAR_ACTADESPACHO_TS(" + DocEntry + ")";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    var a = new ActaDespachoTs_E()
                    {
                        FechaActa = (hdr.IsDBNull(0)) ? string.Empty : hdr.GetDateTime(0).ToString("dd/MM/yyyy"),
                        NroGuia = (hdr.IsDBNull(1)) ? string.Empty : hdr.GetString(1),
                        Proveedor = (hdr.IsDBNull(2)) ? string.Empty : hdr.GetString(2),
                        CodAlmOrigen = (hdr.IsDBNull(3)) ? string.Empty : hdr.GetString(3),
                        CodAlmDestino = (hdr.IsDBNull(4)) ? string.Empty : hdr.GetString(4),
                        CantidadLote = (hdr.IsDBNull(5)) ? 0 : hdr.GetDecimal(5),
                        NombreComercial = (hdr.IsDBNull(6)) ? string.Empty : hdr.GetString(6),
                        Concentracion = (hdr.IsDBNull(7)) ? string.Empty : hdr.GetString(7),
                        FormaFarmaceutica = (hdr.IsDBNull(8)) ? string.Empty : hdr.GetString(8),
                        FormaPresentacion = (hdr.IsDBNull(9)) ? string.Empty : hdr.GetString(9),
                        Almacen = (hdr.IsDBNull(10)) ? string.Empty : hdr.GetString(10),
                        NroLote = (hdr.IsDBNull(11)) ? string.Empty : hdr.GetString(11),
                        FechaVcto = (hdr.IsDBNull(12)) ? string.Empty : hdr.GetDateTime(12).ToString("dd/MM/yyyy"),
                        RegistroSanit = (hdr.IsDBNull(13)) ? string.Empty : hdr.GetString(13),
                        CondicionAlm = (hdr.IsDBNull(14)) ? string.Empty : hdr.GetString(14),
                        AlmOrigen = (hdr.IsDBNull(15)) ? string.Empty : hdr.GetString(15),
                        TaxOfficeOrigen = (hdr.IsDBNull(16)) ? string.Empty : hdr.GetString(16),
                        AlmDestino = (hdr.IsDBNull(17)) ? string.Empty : hdr.GetString(17),
                        TaxOfficeDestino = (hdr.IsDBNull(18)) ? string.Empty : hdr.GetString(18),
                        ItemName = (hdr.IsDBNull(19)) ? string.Empty : hdr.GetString(19)
                    };

                    lista.Add(a);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }

        public List<OrganolepticoTs_E> ConsultarOrganolepticoTs(int DocEntry)
        {
            List<OrganolepticoTs_E> lista = new List<OrganolepticoTs_E>();
            string query = "call " + uti.schemaHana + "DIEGO_COBEFAR_ACTARECEPCION_TS(" + DocEntry + ")";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    var a = new OrganolepticoTs_E()
                    {
                        T1_DocDate = (hdr.IsDBNull(0)) ? string.Empty : hdr.GetDateTime(0).ToString("dd/MM/yyyy"),
                        CodAlmacenEnvio = (hdr.IsDBNull(1)) ? string.Empty : hdr.GetString(1),
                        CodAlmacenDestino = (hdr.IsDBNull(2)) ? string.Empty : hdr.GetString(2),
                        NroDeGuia = (hdr.IsDBNull(3)) ? string.Empty : hdr.GetString(3),
                        T2_ItemCode = (hdr.IsDBNull(4)) ? string.Empty : hdr.GetString(4),
                        T2_Dscription = (hdr.IsDBNull(5)) ? string.Empty : hdr.GetString(5),
                        T3_Quantity = (hdr.IsDBNull(6)) ? 0 : hdr.GetDecimal(6),
                        ItemName = (hdr.IsDBNull(7)) ? string.Empty : hdr.GetString(7),
                        T8_FrgnName = (hdr.IsDBNull(8)) ? string.Empty : hdr.GetString(8),
                        Concentracion = (hdr.IsDBNull(9)) ? string.Empty : hdr.GetString(9),
                        FormaPresentacion = (hdr.IsDBNull(10)) ? string.Empty : hdr.GetString(10),
                        FormaFarmaceutica = (hdr.IsDBNull(11)) ? string.Empty : hdr.GetString(11),
                        Fabricante = (hdr.IsDBNull(12)) ? string.Empty : hdr.GetString(12),
                        Lote = (hdr.IsDBNull(13)) ? string.Empty : hdr.GetString(13),
                        FechaVenc = (hdr.IsDBNull(14)) ? string.Empty : hdr.GetDateTime(14).ToString("dd/MM/yyyy"),
                        RegistroSan = (hdr.IsDBNull(15)) ? string.Empty : hdr.GetString(15),
                        CondAlmac = (hdr.IsDBNull(16)) ? string.Empty : hdr.GetString(16),
                        TaxOfficeAlmacenEnvio = (hdr.IsDBNull(17)) ? string.Empty : hdr.GetString(17),
                        AlmacenEnvio = (hdr.IsDBNull(18)) ? string.Empty : hdr.GetString(18),
                        TaxOfficeAlmacenDestino = (hdr.IsDBNull(19)) ? string.Empty : hdr.GetString(19),
                        AlmacenDestino = (hdr.IsDBNull(20)) ? string.Empty : hdr.GetString(20)
                    };

                    lista.Add(a);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public List<ActaRecepcionEm_E> ConsultarActaRecepcionEm(int DocEntry)
        {
            List<ActaRecepcionEm_E> lista = new List<ActaRecepcionEm_E>();
            string query = "call " + uti.schemaHana + "DIEGO_COBEFAR_ACTARECEPCION_EM(" + DocEntry + ")";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    ActaRecepcionEm_E a = new ActaRecepcionEm_E();
                    if (!hdr.IsDBNull(0)) { a.T0_CardName = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { a.T0_DocDate = hdr.GetDateTime(1).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(2)) { a.Canti_pza_lot = hdr.GetDecimal(2); }
                    if (!hdr.IsDBNull(3)) { a.Lote = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { a.T3_ExpDate = hdr.GetDateTime(4).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(5)) { a.Registro = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { a.T6_Location = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { a.ItemName = hdr.GetString(7); }
                    if (!hdr.IsDBNull(8)) { a.T7_FrgnName = hdr.GetString(8); }
                    if (!hdr.IsDBNull(9)) { a.Concentracion = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { a.FormaPresentacion = hdr.GetString(10); }
                    if (!hdr.IsDBNull(11)) { a.FormaFarmaceutica = hdr.GetString(11); }
                    if (!hdr.IsDBNull(12)) { a.Fabricante = hdr.GetString(12); }
                    if (!hdr.IsDBNull(13)) { a.NroFactura = hdr.GetString(13); }
                    if (!hdr.IsDBNull(14)) { a.Comentario = hdr.GetString(14); }
                    if (!hdr.IsDBNull(15)) { a.Comentario2 = hdr.GetString(15); }
                    lista.Add(a);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public List<OrganolepticoEm_E> ConsultarOrganolepticoEm(int DocEntry)
        {
            List<OrganolepticoEm_E> lista = new List<OrganolepticoEm_E>();
            string query = "call " + uti.schemaHana + "DIEGO_COBEFAR_ORGANOLEPTICO_EM(" + DocEntry + ")";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    OrganolepticoEm_E a = new OrganolepticoEm_E();
                    if (!hdr.IsDBNull(0)) { a.T0_CardName = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { a.T0_DocDate = hdr.GetDateTime(1).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(2)) { a.T1_NumPerMsr = hdr.GetDecimal(2); }
                    if (!hdr.IsDBNull(3)) { a.Canti_pza_lot = hdr.GetDecimal(3); }
                    if (!hdr.IsDBNull(4)) { a.Lote = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { a.T3_ExpDate = hdr.GetDateTime(5).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(6)) { a.Registro = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { a.T6_Location = hdr.GetString(7); }
                    if (!hdr.IsDBNull(8)) { a.ItemName = hdr.GetString(8); }
                    if (!hdr.IsDBNull(9)) { a.T7_FrgnName = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { a.Concentracion = hdr.GetString(10); }
                    if (!hdr.IsDBNull(11)) { a.FormaPresentacion = hdr.GetString(11); }
                    if (!hdr.IsDBNull(12)) { a.FormaFarmaceutica = hdr.GetString(12); }
                    if (!hdr.IsDBNull(13)) { a.Fabricante = hdr.GetString(13); }
                    if (!hdr.IsDBNull(14)) { a.NroFactura = hdr.GetString(14); }
                    if (!hdr.IsDBNull(15)) { a.XDevolucion = hdr.GetDecimal(15); }
                    lista.Add(a);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public List<ComprobanteDePago_E> ConsultarComprobanteDePago(int DocEntry)
        {
            List<ComprobanteDePago_E> lista = new List<ComprobanteDePago_E>();
            System.Globalization.CultureInfo culturaES = new System.Globalization.CultureInfo("es-PE");
            string query = "CALL " + uti.schemaHana + "DIEGO_LYT_FV(" + DocEntry + ")";
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
                        if (!hdr.IsDBNull(16)) { c.Fecha = hdr.GetDateTime(16).ToString("dd/MM/yyyy", culturaES); }
                        //if (!hdr.IsDBNull(16)) { c.Fecha = hdr.GetDateTime(16).ToString("dd/MM/yyyy"); }
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
                        if (!hdr.IsDBNull(30)) { c.FechaEntrega = hdr.GetDateTime(30).ToString("dd/MM/yyyy"); }
                        if (!hdr.IsDBNull(31)) { c.Impuesto = hdr.GetDecimal(31); }
                        if (!hdr.IsDBNull(32)) { c.DocTotal = hdr.GetDecimal(32); }
                        if (!hdr.IsDBNull(33)) { c.PorcenImpto = hdr.GetDecimal(33); }
                        if (!hdr.IsDBNull(34)) { c.LoteNum = hdr.GetString(34); }
                        if (!hdr.IsDBNull(35)) { c.CantidadL = hdr.GetDecimal(35); }
                        if (!hdr.IsDBNull(36)) { c.TieneAnticipo = hdr.GetInt32(36); }
                        if (!hdr.IsDBNull(37)) { c.Laboratorio = hdr.GetString(37); }
                        if (!hdr.IsDBNull(38)) { c.VctoLote = hdr.GetDateTime(38).ToString("dd/MM/yyyy"); }
                        if (!hdr.IsDBNull(39)) { c.QUMVta = Math.Round(hdr.GetDecimal(39), 0); }
                        if (!hdr.IsDBNull(40)) { c.CondPago = hdr.GetString(40); }
                        if (!hdr.IsDBNull(41)) { c.NroOrdVenta = hdr.GetString(41); }
                        if (!hdr.IsDBNull(42)) { c.CodImpuesto = hdr.GetString(42); }
                        if (!hdr.IsDBNull(43)) { c.Almacen = hdr.GetString(43); }
                        if (!hdr.IsDBNull(44)) { c.PtoPartida = hdr.GetString(44); }
                        if (!hdr.IsDBNull(45)) { c.DirEnvio = hdr.GetString(45); }

                        lista.Add(c);
                    }
                }
                hdr.Close();
            }
            catch (Exception e) { throw new Exception(e.Message); }
            return lista;
        }
        public List<OrdenDeVenta_E> ConsultarOrdenDeVenta(int docNum)
        {
            List<OrdenDeVenta_E> lista = new List<OrdenDeVenta_E>();
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand("select \"DocEntry\" from " + uti.schemaHana + "ordr where \"DocNum\"=" + docNum, hcn);
                HanaDataReader dr = hcmd.ExecuteReader();
                dr.Read();
                int DocEntry = dr.GetInt32(0);
                dr.Close();

                HanaCommand hcmd2 = new HanaCommand(uti.schemaHana + "DIEGO_LYT_OV_2", hcn);
                hcmd2.CommandType = CommandType.StoredProcedure;
                hcmd2.Parameters.AddWithValue("DocEntry", DocEntry);
                HanaDataReader hdr2 = hcmd2.ExecuteReader();
                while (hdr2.Read())
                {
                    OrdenDeVenta_E o = new OrdenDeVenta_E();
                    if (!hdr2.IsDBNull(0)) { o.NombreBd = hdr2.GetString(0); }
                    if (!hdr2.IsDBNull(1)) { o.DocNum = hdr2.GetInt32(1); }
                    if (!hdr2.IsDBNull(2)) { o.Fecha = hdr2.GetDateTime(2).ToString(); }
                    if (!hdr2.IsDBNull(3)) { o.CardName = hdr2.GetString(3); }
                    if (!hdr2.IsDBNull(4)) { o.RucCliente = hdr2.GetString(4); }
                    if (!hdr2.IsDBNull(7)) { o.UniMedidVend = hdr2.GetString(7); }
                    if (!hdr2.IsDBNull(8)) { o.NumUnidVend = hdr2.GetDecimal(8); }
                    if (!hdr2.IsDBNull(9)) o.CantidadSolicitadaVenta = hdr2.GetDecimal(9);
                    if (!hdr2.IsDBNull(11)) { o.Laboratorio = hdr2.GetString(11); }
                    if (!hdr2.IsDBNull(12)) { o.Producto = hdr2.GetString(12); }
                    if (!hdr2.IsDBNull(13)) { o.Lote = hdr2.GetString(13); }
                    if (!hdr2.IsDBNull(14)) { o.FechaVenc = hdr2.GetDateTime(14).ToString("dd/MM/yyyy"); }
                    if (!hdr2.IsDBNull(15)) { o.PrecioProdIgvVend = hdr2.GetDecimal(15); }
                    if (!hdr2.IsDBNull(17)) { o.TotalProdIgvVend = hdr2.GetDecimal(17); }
                    if (!hdr2.IsDBNull(18)) { o.SlpName = hdr2.GetString(18); }
                    if (!hdr2.IsDBNull(19)) { o.Comentarios = hdr2.GetString(19); }
                    if (!hdr2.IsDBNull(20)) { o.DocTotal = hdr2.GetDecimal(20); }
                    if (!hdr2.IsDBNull(21)) { o.Almacen = hdr2.GetString(21); }
                    if (!hdr2.IsDBNull(22)) { o.Ubicaciones = new string[] { hdr2.GetString(22) }; }
                    if (!hdr2.IsDBNull(23)) { o.RegSanit = hdr2.GetString(23); }
                    if (!hdr2.IsDBNull(24)) { o.ItemCode = hdr2.GetString(24); }

                    lista.Add(o);
                }
                hdr2.Close();
                hcn.Close();
            }
            catch { hcn.Close(); }
            return lista;
        }
        public List<AuditoriaStocks_E> ReporteAuditoriaStocks(FrmAuditoriaStocks_E frm)
        {
            List<AuditoriaStocks_E> lista = new List<AuditoriaStocks_E>();
            string filtro = string.Empty;

            if (frm.Almacenes != null)
            {
                string query2 = string.Empty;
                string almacenesIn = "''";
                foreach (OWHS_E alm in frm.Almacenes)
                {
                    if (alm.WhsCode != null && alm.WhsCode != "")
                    {
                        almacenesIn += ",'" + alm.WhsCode + "'";
                    }
                }
                filtro = " and (T0.\"CreateDate\" between '" + frm.FecIni + "' and '" + frm.FecFin + "') and (T0.\"Warehouse\" in(" + almacenesIn + "))";
                if (frm.ItmsGrpCod > 0)
                {
                    filtro += " and (T0.\"ItemCode\" in (select \"ItemCode\" from " + uti.schemaHana + "oitm where \"ItmsGrpCod\"=" + frm.ItmsGrpCod + "))";
                }
                if (frm.ArtIni != null && frm.ArtIni != "" && frm.ArtFin != null && frm.ArtFin != "")
                {
                    filtro += " and T0.\"ItemCode\" between '" + frm.ArtIni + "' and '" + frm.ArtFin + "'";
                }
                query2 = "SELECT T0.\"CreateDate\",T0.\"DocDate\",(CASE T0.\"TransType\" WHEN '13' THEN 'IN'	WHEN '14' THEN 'RC'	" +
                                                "WHEN '15' THEN 'NE' WHEN '18' THEN 'TT' WHEN '19' THEN 'AC' WHEN '20' THEN 'EP' WHEN '21' THEN 'DM' " +
                                                "WHEN '59' THEN 'EM' WHEN '60' THEN 'SM' WHEN '67' THEN 'TR' ELSE '' END) AS \"Abreviatura\"" +
                                                ",T0.\"BASE_REF\",T0.\"ItemCode\",T0.\"Dscription\",T0.\"Warehouse\",(T0.\"InQty\"-T0.\"OutQty\")" +
                                                ",T0.\"CalcPrice\",T0.\"TransValue\"" +
                                                ",(select (sum(Y.\"TransValue\")) from " + uti.schemaHana + "oinm Y " +
                                                                " where Y.\"CreateDate\" < T0.\"CreateDate\" and Y.\"Warehouse\" in(" + almacenesIn + ") " +
                                                                       " and Y.\"ItemCode\" = T0.\"ItemCode\" and Y.\"TransNum\"<T0.\"TransNum\")	AS \"ValorAcumulado\"" +
                                                ",T0.\"InvntAct\",T0.\"CardName\"" +
                                                ",(select (sum(Y.\"InQty\"-Y.\"OutQty\")) from " + uti.schemaHana + "oinm Y	" +
                                                                " where Y.\"CreateDate\" <= T0.\"CreateDate\" and Y.\"Warehouse\" in(" + almacenesIn + ") " +
                                                                        " and Y.\"ItemCode\" = T0.\"ItemCode\" and Y.\"TransNum\"<T0.\"TransNum\")+(T0.\"InQty\"-T0.\"OutQty\")	AS \"CantidadAcumulada\" " +
                                         " FROM " + uti.schemaHana + "oinm T0 WHERE T0.\"TransNum\">0 " + filtro +
                                         " order by T0.\"ItemCode\",T0.\"CreateDate\",T0.\"TransNum\"";
                try
                {
                    HanaDataReader hdr = db.HanaExecuteReaderNoSp(query2);
                    decimal acum = 0.00M;
                    while (hdr.Read())
                    {
                        AuditoriaStocks_E a = new AuditoriaStocks_E();
                        if (!hdr.IsDBNull(0)) { a.CreateDate = hdr.GetDateTime(0).ToString("dd/MM/yyyy"); }
                        if (!hdr.IsDBNull(1)) { a.DocDate = hdr.GetDateTime(1).ToString("dd/MM/yyyy"); }
                        if (!hdr.IsDBNull(2)) { a.Abreviatura = hdr.GetString(2); }
                        if (!hdr.IsDBNull(3)) { a.DocNum = hdr.GetInt32(3); }
                        if (!hdr.IsDBNull(4)) { a.ItemCode = hdr.GetString(4); }
                        if (!hdr.IsDBNull(5)) { a.ItemName = hdr.GetString(5); }
                        if (!hdr.IsDBNull(6)) { a.WhsCode = hdr.GetString(6); }
                        if (!hdr.IsDBNull(7)) { a.Quantity = Math.Round(hdr.GetDecimal(7), 2); }
                        if (!hdr.IsDBNull(8)) { a.Costos = Math.Round(hdr.GetDecimal(8), 2); }
                        if (!hdr.IsDBNull(10)) { a.ValorAcumuladoTotal = Math.Round(hdr.GetDecimal(10), 2); }
                        if (!hdr.IsDBNull(9))
                        {
                            a.ValorTrans = Math.Round(hdr.GetDecimal(9), 2); acum += a.ValorTrans; a.ValorAcumulado = acum;
                        }
                        if (!hdr.IsDBNull(11)) { a.CuentaMayor = hdr.GetString(11); }
                        if (!hdr.IsDBNull(12)) { a.CardName = hdr.GetString(12); }
                        if (!hdr.IsDBNull(13)) { a.CantidadAcumulada = Math.Round(hdr.GetDecimal(13), 2); }
                        a.DeFecha = frm.FecIni;
                        a.HastaFecha = frm.FecFin;
                        a.Moneda = "Soles";
                        a.Cuentas = "Cuentas";
                        lista.Add(a);
                    }
                    hdr.Close();
                }
                catch { }
            }
            return lista;
        }
        public List<OperacionesLotes_E> ReporteOperacionesLotes(FrmOperacionesLotes_E frm, string cab)
        {
            List<OperacionesLotes_E> lista = new List<OperacionesLotes_E>();
            string query = "";
            if (cab.Equals("Si"))
            {
                query = "call " + uti.schemaHana + "DIEGO_RPT_OPERACIONESLOTESCAB('" + frm.ArtIni + "','" + frm.ArtFin + "'," + frm.ItmsGrpCod + ",'" + frm.FecIni + "','" + frm.FecFin + "','" + frm.AlmIni + "','" + frm.AlmFin + "')";
                try
                {
                    HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                    while (hdr.Read())
                    {
                        OperacionesLotes_E o = new OperacionesLotes_E();
                        if (!hdr.IsDBNull(0)) { o.ItemCode = hdr.GetString(0); }
                        if (!hdr.IsDBNull(1)) { o.ItemName = hdr.GetString(1); }
                        if (!hdr.IsDBNull(2)) { o.DistNumber = hdr.GetString(2); }
                        if (!hdr.IsDBNull(3)) { o.WhsCode = hdr.GetString(3); }
                        if (!hdr.IsDBNull(4)) { o.WhsName = hdr.GetString(4); }
                        if (!hdr.IsDBNull(5)) { o.QuantityTotal = Math.Round(hdr.GetDecimal(5), 2); }
                        if (!hdr.IsDBNull(6)) { o.ImputadoTotal = Math.Round(hdr.GetDecimal(6), 2); }
                        if (!hdr.IsDBNull(7)) { o.MnfSerial = hdr.GetString(7); }
                        if (!hdr.IsDBNull(8)) { o.ExpDate = hdr.GetDateTime(8).ToString("dd/MM/yyyy"); }
                        if (!hdr.IsDBNull(9)) { o.FechaAdmision = hdr.GetDateTime(9).ToString("dd/MM/yyyy"); }
                        if (!hdr.IsDBNull(10)) { o.Temperatura = hdr.GetString(10); }
                        lista.Add(o);
                    }
                    hdr.Close();
                }
                catch { }
            }
            else
            {
                query = "call " + uti.schemaHana + "DIEGO_RPT_OPERACIONESLOTES('" + frm.ArtIni + "','" + frm.FecIni + "','" + frm.FecFin + "','" + frm.AlmIni + "','" + frm.Lote + "')";
                try
                {
                    HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                    while (hdr.Read())
                    {
                        OperacionesLotes_E o = new OperacionesLotes_E();
                        if (!hdr.IsDBNull(0)) { o.ItemCode = hdr.GetString(0); }
                        if (!hdr.IsDBNull(1)) { o.ItemName = hdr.GetString(1); }
                        if (!hdr.IsDBNull(2)) { o.DistNumber = hdr.GetString(2); }
                        if (!hdr.IsDBNull(3)) { o.WhsCode = hdr.GetString(3); }
                        if (!hdr.IsDBNull(4)) { o.WhsName = hdr.GetString(4); }
                        if (!hdr.IsDBNull(5)) { o.DocNum = hdr.GetInt32(5); }
                        if (!hdr.IsDBNull(6)) { o.DocDate = hdr.GetDateTime(6).ToString("dd/MM/yyyy"); }
                        if (!hdr.IsDBNull(7)) { o.CardName = hdr.GetString(7); }
                        if (!hdr.IsDBNull(8)) { o.Quantity = Math.Round(hdr.GetDecimal(8), 2); }
                        if (!hdr.IsDBNull(9)) { o.Imputado = Math.Round(hdr.GetDecimal(9), 2); }
                        if (!hdr.IsDBNull(10)) { o.Sentido = hdr.GetString(10); }
                        if (!hdr.IsDBNull(11)) { o.Abreviatura = hdr.GetString(11); }
                        lista.Add(o);
                    }
                    hdr.Close();
                }
                catch { }
            }
            return lista;
        }
        private List<PreciosOpm_E> ReportePreciosOpm(string FecIni, string FecFin)
        {
            List<PreciosOpm_E> lista = new List<PreciosOpm_E>();
            List<PreciosOpm_E> listaAux = new List<PreciosOpm_E>();
            string query = "CALL " + uti.schemaHana + "DIEGO_RPT_REGVENTAS('" + FecIni + "','" + FecFin + "')";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    PreciosOpm_E p = new PreciosOpm_E();
                    if (!hdr.IsDBNull(0)) { p.ItemCode = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { p.Dscription = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { p.PrecioCajas = hdr.GetDecimal(2); }
                    if (!hdr.IsDBNull(3)) { p.MnfSerial = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { p.DocEntry = hdr.GetInt32(4); }
                    if (!hdr.IsDBNull(5)) { p.ObjType = hdr.GetString(5); }
                    listaAux.Add(p);
                }
                hdr.Close();
                lista = PreciosOpm_E.ListaConPreciosFinales(listaAux);
            }
            catch { }
            return lista;
        }
        public List<NotaCreditoVentaArticulo_E> ConsultarNotaCreditoVentaArticulos(int DocEntry)
        {
            List<NotaCreditoVentaArticulo_E> lista = new List<NotaCreditoVentaArticulo_E>();
            string query = "call " + uti.schemaHana + "DIEGO_LYT_NC_ELECT(" + DocEntry + ")";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    NotaCreditoVentaArticulo_E n = new NotaCreditoVentaArticulo_E();
                    if (!hdr.IsDBNull(0)) { n.DocEntry = hdr.GetInt32(0); }
                    if (!hdr.IsDBNull(1)) { n.ElaboradoPor = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { n.SerieDoc = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { n.CorreDoc = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { n.TipoDocOrigen = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { n.SerieDocOrigen = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { n.CorreDocOrigen = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { n.FDocOrigen = hdr.GetDateTime(7).ToString("dd/MM/yyyy"); }
                    if (!hdr.IsDBNull(8)) { n.Motivo = hdr.GetString(8); }
                    if (!hdr.IsDBNull(9)) { n.NombreSocio = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { n.DirPagar = hdr.GetString(10); }
                    if (!hdr.IsDBNull(11)) { n.RUC = hdr.GetString(11); }
                    if (!hdr.IsDBNull(12)) { n.Fecha = hdr.GetDateTime(12); }
                    if (!hdr.IsDBNull(13)) { n.Moneda = hdr.GetString(13); }
                    if (!hdr.IsDBNull(14)) { n.MonedaLetras = hdr.GetString(14); }
                    if (!hdr.IsDBNull(15)) { n.Descripcion = hdr.GetString(15); }
                    if (!hdr.IsDBNull(16)) { n.ItemPrecio = hdr.GetDecimal(16); }
                    if (!hdr.IsDBNull(17)) { n.ItemTotal = hdr.GetDecimal(17); }
                    if (!hdr.IsDBNull(18)) { n.Impuesto = hdr.GetDecimal(18); }
                    if (!hdr.IsDBNull(19)) { n.DocTotal = hdr.GetDecimal(19); }
                    if (!hdr.IsDBNull(20)) { n.Impto = hdr.GetDecimal(20); }
                    if (!hdr.IsDBNull(21)) { n.LoteNum = hdr.GetString(21); }
                    if (!hdr.IsDBNull(22)) { n.CantidadL = hdr.GetDecimal(22); }
                    if (!hdr.IsDBNull(23)) { n.TieneAnticipo = hdr.GetInt32(23); }
                    if (!hdr.IsDBNull(24)) { n.MontoAnticipo = hdr.GetDecimal(24); }
                    if (!hdr.IsDBNull(25)) { n.NumAnticipo = hdr.GetString(25); }
                    if (!hdr.IsDBNull(26)) { n.Laboratorio = hdr.GetString(26); }
                    if (!hdr.IsDBNull(27)) { n.QUMVta = hdr.GetDecimal(27); }
                    if (!hdr.IsDBNull(28)) { n.CodImpuesto = hdr.GetString(28); }
                    if (!hdr.IsDBNull(29)) { n.UM = hdr.GetString(29); }
                    if (!hdr.IsDBNull(30)) { n.Descuento = hdr.GetDecimal(30); }
                    if (!hdr.IsDBNull(31)) { n.PreVentaNeto = hdr.GetDecimal(31); }
                    if (!hdr.IsDBNull(32)) { n.PreUnitSinIgv = hdr.GetDecimal(32); }
                    if (!hdr.IsDBNull(33)) { n.VctoLote = hdr.GetDateTime(33).ToString("dd/MM/yyyy"); }

                    if (!hdr.IsDBNull(34)) { n.TipoDescripcionC = hdr.GetString(34); }
                    if (!hdr.IsDBNull(35)) { n.ItemCode = hdr.GetString(35); }

                    lista.Add(n);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public List<VentasArtLote_E> ListaVentasArtLote(FrmKardex_E f)
        {
            List<VentasArtLote_E> lista = new List<VentasArtLote_E>();
            string query = "select t0.\"ItemName\",t0.\"ItemCode\",t0.\"BatchNum\",t0.\"DocDate\"" +
                            ",(case t0.\"BaseType\" " +
                                     "when 13 then (select \"NumAtCard\" from " + uti.schemaHana + "oinv where \"DocEntry\" = t0.\"BaseEntry\") " +
                                     "when 15 then (select max(x.\"NumAtCard\") from " + uti.schemaHana + "dln1 y " +
                                                     "inner join " + uti.schemaHana + "inv1 y1 on y1.\"BaseEntry\" = y.\"DocEntry\" and y1.\"BaseType\" = y.\"ObjType\" " +
                                                     "inner join " + uti.schemaHana + "oinv x on x.\"DocEntry\" = y1.\"DocEntry\" " +
                                                    "where y.\"DocEntry\" = t0.\"BaseEntry\" )" +
                                   "when 14 then (select \"NumAtCard\" from " + uti.schemaHana + "orin where \"DocEntry\" = t0.\"BaseEntry\")  " +
                                   " end ) as \"Comprobante\" " +
                            ",t0.\"CardCode\",t0.\"CardName\"" +
                            ",(case when t0.\"Direction\"=1 then -1*t0.\"Quantity\" else t0.\"Quantity\" end) as \"Cantidad Vendida pza\" " +
                            ",(case when t0.\"Direction\"=1 then -1*t0.\"Quantity\" else t0.\"Quantity\" end)/(select \"NumInBuy\" from " + uti.schemaHana + "oitm where \"ItemCode\" = t0.\"ItemCode\") as \"Cantidad Vendida caja\" " +
                            ",(case t0.\"BaseType\" " +
                                      "when 13 then (select (select \"SlpName\" from " + uti.schemaHana + "oslp where \"SlpCode\" = h.\"SlpCode\") " +
                                                     "from " + uti.schemaHana + "oinv h where h.\"DocEntry\" = t0.\"BaseEntry\") " +
                                      "when 15 then (select (select \"SlpName\" from " + uti.schemaHana + "oslp where \"SlpCode\" = x.\"SlpCode\") " +
                                                      "from " + uti.schemaHana + "dln1 y " +
                                                      "inner join " + uti.schemaHana + "inv1 y1 on y1.\"BaseEntry\" = y.\"DocEntry\" and y1.\"BaseType\" = y.\"ObjType\" " +
                                                                              "and y1.\"ItemCode\" = y.\"ItemCode\" " +
                                                      "inner join " + uti.schemaHana + "oinv x on x.\"DocEntry\" = y1.\"DocEntry\" " +
                                                      "where y.\"DocEntry\" = t0.\"BaseEntry\" " +
                                                      "group by  x.\"SlpCode\" ) end ) as \"Vendedor\" " +
                            ",(select \"E_Mail\" from " + uti.schemaHana + "ocrd where \"CardCode\"=t0.\"CardCode\" ) as \"Correo\" " +
                            "from " + uti.schemaHana + "ibt1 t0 " +
                            "where t0.\"ItemCode\" = '" + f.ItemCode + "' and t0.\"BatchNum\" = '" + f.Lote + "' " +
                                    "and t0.\"DocDate\" between '" + f.FecIni + "' and '" + f.FecFin + "' " +
                                    "and t0.\"BaseType\" in (13,14,15,16) and t0.\"Direction\"<>'2'" +
                            "order by t0.\"DocDate\",t0.\"CardCode\",t0.\"BaseEntry\" ";
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand(query, hcn);
                hcmd.CommandType = CommandType.Text;
                HanaDataReader hdr = hcmd.ExecuteReader();
                while (hdr.Read())
                {
                    VentasArtLote_E o = new VentasArtLote_E();
                    if (!hdr.IsDBNull(0)) { o.ItemName = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { o.ItemCode = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { o.BatchNum = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { o.DocDate = hdr.GetDateTime(3); }
                    if (!hdr.IsDBNull(4)) { o.Comprobante = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { o.CardCode = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { o.CardName = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { o.CantPza = hdr.GetDecimal(7); }
                    if (!hdr.IsDBNull(8)) { o.CantCja = hdr.GetDecimal(8); }
                    if (!hdr.IsDBNull(9)) { o.Vendedor = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { o.Correo = hdr.GetString(10); }
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            finally { hcn.Close(); }
            return lista;
        }
        // tablas de reportes
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
        public DataTable tbReportePreciosOpm(string FecIni, string FecFin)
        {
            List<string> campos = new List<string>();
            List<Type> tipos = new List<Type>();
            campos.Add("Orden"); tipos.Add(typeof(string));
            campos.Add("Codigo"); tipos.Add(typeof(string));
            campos.Add("Descripcion"); tipos.Add(typeof(string));
            campos.Add("PrecioMinimo"); tipos.Add(typeof(string));
            campos.Add("PrecioMaximo"); tipos.Add(typeof(string));
            campos.Add("PrecioMediano"); tipos.Add(typeof(string));
            campos.Add("PrecioPromedio"); tipos.Add(typeof(string));
            campos.Add("RegistroSanitario"); tipos.Add(typeof(string));
            DataTable tb = definirTabla(campos, tipos, "DataTableReportePrecios");
            int i = 0;
            foreach (PreciosOpm_E p in ReportePreciosOpm(FecIni, FecFin))
            {
                DataRow row = tb.NewRow();
                row["Orden"] = i++;
                row["Codigo"] = p.ItemCode;
                row["Descripcion"] = p.Dscription;
                row["PrecioMinimo"] = Math.Round(p.PreMinimo, 2);
                row["PrecioMaximo"] = Math.Round(p.PreMaximo, 2);
                row["PrecioMediano"] = Math.Round(p.PreMediano, 2);
                row["PrecioPromedio"] = Math.Round(p.PrePromedio, 2);
                row["RegistroSanitario"] = p.MnfSerial;
                tb.Rows.Add(row);
            }
            return tb;
        }
    }
}