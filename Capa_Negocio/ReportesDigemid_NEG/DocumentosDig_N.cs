using Capa_Datos.ReportesDigemid_DAO;
using Capa_Entidad.ReportesDigemid_ENT;
using Capa_Entidad.ReportesDigemid_ENT.Formularios;
using Capa_Entidad.ReportesDigemid_ENT.Reportes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Capa_Negocio.ReportesDigemid_NEG
{
    public class DocumentosDig_N
    {
        DocumentosDig_D dD = new DocumentosDig_D();
        string PatternIny = @"[!@#$%^&*,()¿~'<>:;+.=?]";
        public List<ActaRecepcionVt_E> ConsultarActaRecepcionVt(int DocEntry)
        {
            return dD.ConsultarActaRecepcionVt(DocEntry);
        }
        public List<ActaDespachoVt_E> ConsultarActaDespachoVt(int DocEntry)
        {
            return dD.ConsultarActaDespachoVt(DocEntry);
        }
        public List<OrganolepticoVt_E> ConsultarOrganolepticoVt(int DocEntry)
        {
            return dD.ConsultarOrganolepticoVt(DocEntry);
        }
        public List<ActaRecepcionTs_E> ConsultarActaRecepcionTs(int DocEntry)
        {
            return dD.ConsultarActaRecepcionTs(DocEntry);
        }
        public List<ActaDespachoTs_E> ConsultarActaDespachoTs(int DocEntry)
        {
            return dD.ConsultarActaDespachoTs(DocEntry);
        }
        public List<OrganolepticoTs_E> ConsultarOrganolepticoTs(int DocEntry)
        {
            return dD.ConsultarOrganolepticoTs(DocEntry);
        }
        public List<ActaRecepcionEm_E> ConsultarActaRecepcionEm(int DocEntry)
        {
            return dD.ConsultarActaRecepcionEm(DocEntry);
        }
        public List<OrganolepticoEm_E> ConsultarOrganolepticoEm(int DocEntry)
        {
            return dD.ConsultarOrganolepticoEm(DocEntry);
        }
        public List<ComprobanteDePago_E> ConsultarComprobanteDePago(int DocEntry)
        {
            return dD.ConsultarComprobanteDePago(DocEntry);
        }
        public List<OrdenDeVenta_E> ConsultarOrdenDeVenta(int DocNum)
        {
            return dD.ConsultarOrdenDeVenta(DocNum);
        }
        public List<AuditoriaStocks_E> ReporteAuditoriaStocks(FrmAuditoriaStocks_E frm)
        {
            if (frm.ArtIni == null) { frm.ArtIni = ""; }
            if (frm.ArtFin == null) { frm.ArtFin = ""; }
            if (frm.FecIni == null) { throw new Exception("Debe ingresar fecha inicial"); }
            if (frm.FecFin == null) { throw new Exception("Debe ingresar fecha final"); }
            try
            {
                DateTime DtFecIni = DateTime.Parse(frm.FecIni);
                DateTime DtFecFin = DateTime.Parse(frm.FecFin);
                if ((DtFecFin.DayOfYear - DtFecIni.DayOfYear) > 31) { throw new Exception("El rango de fechas no debe ser superior a 31 dias"); }
            }
            catch { throw new Exception("Fechas incorrectas"); }
            return dD.ReporteAuditoriaStocks(frm);

        }
        public List<OperacionesLotes_E> ReporteOperacionesLotes(FrmOperacionesLotes_E frm, string cab)
        {
            return dD.ReporteOperacionesLotes(frm, cab);
        }
        public DataTable tbReportePreciosOpm(string FecIni, string FecFin)
        {
            return dD.tbReportePreciosOpm(FecIni, FecFin);
        }
        public List<NotaCreditoVentaArticulo_E> ConsultarNotaCreditoVentaArticulos(int DocEntry)
        {
            return dD.ConsultarNotaCreditoVentaArticulos(DocEntry);
        }
        public List<VentasArtLote_E> ListaVentasArtLote(FrmKardex_E f)
        {
            
            if(string.IsNullOrWhiteSpace(f.FecIni)) { throw new Exception("No ingreso fecha inicial"); }
            if (string.IsNullOrWhiteSpace(f.FecFin)) { throw new Exception("No ingreso fecha final"); }
            if (string.IsNullOrWhiteSpace(f.ItemCode)){ throw new Exception("No ingreso el codigo de articulo"); }

            if (string.IsNullOrWhiteSpace(f.Lote)) { throw new Exception("No ingreso el lote de articulo"); }
            MatchCollection matches = Regex.Matches(f.Lote, PatternIny, RegexOptions.IgnoreCase);
            if (matches.Count > 0) { throw new Exception("El lote tiene caracteres invalidos"); }

            return dD.ListaVentasArtLote(f);
        }
        
    }
}
