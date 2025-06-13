using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Capa_Entidad.AtencionCliente_ENT.TablasExternas;

namespace Capa_Entidad.AtencionCliente_ENT.TablasSql
{
    public class SAT1_E
    {
        public int DocEntry { get; set; }
        public int Linea { get; set; }
        public int NroSap { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string UnitMsr { get; set; }
        public decimal NumPerMsr { get; set; }
        public decimal Quantity { get; set; }
        public string BatchNum { get; set; }
        public decimal NumPerMsrF { get; set; }
        public string ExpDate { get; set; }
        public string unitMsrF { get; set; }
        public decimal QuantityF { get; set; }
        public decimal PriceAfVAT { get; set; }
        public decimal LineTotalF { get; set; }
        public string Problema { get; set; }
        public string TipoError { get; set; }
        public string OpResponsable { get; set; }
        public string Comentario { get; set; }
        public string Regalo { get; set; }
        public string MotRegalo { get; set; }
        public string TareaFact { get; set; }
        public string ComprobanteVinc { get; set; }
        public string AlmTransf { get; set; }
        [DisplayName("Doc. Fact")]
        public string ComprobanteFin { get; set; }
        public string AlmVenta { get; set; }
        public string ErrorAlmacen { get; set; }
        public int? NCSAP { get; set; }
<<<<<<< HEAD
        public string ErrAlmOtrCom { get; set; }
=======
        public decimal? NuevoPrecioArticulo { get; set; }
        public string ReferenciaNC_ND { get; set; }
>>>>>>> hotfix/NotaCreditoSolicitudes

        // CAMPOS QUE NO SON DE LA TABLA
        public Dictionary<string, NotaFinanciera_E> ComprobantesVinculados { get; set; }
        public static DataTable tbDetalle(List<SAT1_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("NroSap", typeof(int));
            tb.Columns.Add("ItemCode", typeof(string));
            tb.Columns.Add("Dscription", typeof(string));
            tb.Columns.Add("unitMsr", typeof(string));
            tb.Columns.Add("NumPerMsr", typeof(decimal));
            tb.Columns.Add("Quantity", typeof(decimal));
            tb.Columns.Add("BatchNum", typeof(string));
            tb.Columns.Add("ExpDate", typeof(string));
            tb.Columns.Add("unitMsrF", typeof(string));
            tb.Columns.Add("NumPerMsrF", typeof(decimal));
            tb.Columns.Add("QuantityF", typeof(decimal));
            tb.Columns.Add("PriceAfVAT", typeof(decimal));
            tb.Columns.Add("LineTotalF", typeof(decimal));
            tb.Columns.Add("Problema", typeof(string));
            tb.Columns.Add("TipoError", typeof(string));
            tb.Columns.Add("OpResponsable", typeof(string));
            tb.Columns.Add("Comentario", typeof(string));
            tb.Columns.Add("Regalo", typeof(string));
            tb.Columns.Add("MotRegalo", typeof(string));
            tb.Columns.Add("TareaFact", typeof(string));
            tb.Columns.Add("ComprobanteVinc", typeof(string));
            tb.Columns.Add("AlmTransf", typeof(string));
            tb.Columns.Add("ComprobanteFin", typeof(string));
            tb.Columns.Add("AlmVenta", typeof(string));
            tb.Columns.Add("ErrorAlmacen", typeof(string));
            tb.Columns.Add("NCSAP", typeof(int));
<<<<<<< HEAD
            tb.Columns.Add("ErrAlmOtrCom", typeof(string));
=======
            tb.Columns.Add("NuevoPrecioArticulo", typeof(decimal));
            tb.Columns.Add("ReferenciaNC_ND", typeof(string));
>>>>>>> hotfix/NotaCreditoSolicitudes

            foreach (SAT1_E reg in dt)
            {
                tb.Rows.Add(reg.DocEntry, reg.Linea, reg.NroSap, reg.ItemCode, reg.Dscription, reg.UnitMsr
                    , reg.NumPerMsr, reg.Quantity, reg.BatchNum, reg.ExpDate, reg.unitMsrF, reg.NumPerMsrF
                    , reg.QuantityF, reg.PriceAfVAT, reg.LineTotalF, reg.Problema, reg.TipoError, reg.OpResponsable
                    , reg.Comentario, reg.Regalo, reg.MotRegalo, reg.TareaFact, reg.ComprobanteVinc, reg.AlmTransf
<<<<<<< HEAD
                    , reg.ComprobanteFin, reg.AlmVenta, reg.ErrorAlmacen, reg.NCSAP, reg.ErrAlmOtrCom);
=======
                    , reg.ComprobanteFin, reg.AlmVenta, reg.ErrorAlmacen, reg.NCSAP, reg.NuevoPrecioArticulo, reg.ReferenciaNC_ND);
>>>>>>> hotfix/NotaCreditoSolicitudes
            }
            return tb;
        }
        public string selectedTipoError(string valor)
        {
            if (TipoError == valor)
            { return "selected"; }
            else return "";
        }

    }
    public class SAT1_ComprobanteFin
    {
        public int DocEntry { get; set; }
        public string ComprobanteFin { get; set; }
    }
}
