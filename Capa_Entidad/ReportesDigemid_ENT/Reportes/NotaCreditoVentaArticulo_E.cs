using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.ReportesDigemid_ENT.Reportes
{
    public class NotaCreditoVentaArticulo_E
    {
        //DIEGO_LYT_NC_ELECT
        public int DocEntry { get; set; }
        public string ElaboradoPor { get; set; }
        public string SerieDoc { get; set; }
        public string CorreDoc { get; set; }
        public string TipoDocOrigen { get; set; }
        public string SerieDocOrigen { get; set; }
        public string CorreDocOrigen { get; set; }
        public string FDocOrigen { get; set; }
        public string Motivo { get; set; }
        public string NombreSocio { get; set; }
        public string DirPagar { get; set; }
        public string RUC { get; set; }
        public DateTime Fecha { get; set; }
        public string Moneda { get; set; }
        public string MonedaLetras { get; set; }
        public string Descripcion { get; set; }
        public decimal ItemPrecio { get; set; }
        public decimal ItemTotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal DocTotal { get; set; }
        public decimal Impto { get; set; }
        public string LoteNum { get; set; }
        public decimal CantidadL { get; set; }
        public int TieneAnticipo { get; set; }
        public decimal MontoAnticipo { get; set; }
        public string NumAnticipo { get; set; }
        public string Laboratorio { get; set; }
        public decimal QUMVta { get; set; }
        public string CodImpuesto { get; set; }
        public string UM { get; set; }
        public decimal Descuento { get; set; }
        public decimal PreVentaNeto { get; set; }
        public decimal PreUnitSinIgv { get; set; }
        public string VctoLote { get; set; }
        //metodos
        public decimal cantVta()
        {
            if (LoteNum == null || LoteNum == "") { return CantidadL; }
            else
            {
                if ((CantidadL / QUMVta) < 1) { return CantidadL; }
                else { return CantidadL / QUMVta; }
            }
        }
        public string F_UM()
        {
            if ((CantidadL / QUMVta) < 1) { return "PZA"; }
            else { return ""; }
        }
        public decimal F_ItemPrecio()
        {
            if (Impto == 0.00M || CodImpuesto != "IGV")
            {
                if ((CantidadL / QUMVta) < 1)
                {
                    return PreUnitSinIgv / QUMVta;
                }
                else
                {
                    return PreUnitSinIgv;
                }
            }
            else //(PorcenImpto==18)
            {
                if ((CantidadL / QUMVta) < 1)
                {
                    return (PreUnitSinIgv / QUMVta) * 1.18M;
                }
                else
                {
                    return PreUnitSinIgv * 1.18M;
                }
            }
        }
        public decimal ItemTotalxLot()
        {
            if (Impto == 0.00M)
            {
                return cantVta() * PreVentaNeto;
            }
            else
            {
                return cantVta() * PreVentaNeto;
            }
        }
        public string TotalL()
        {
            string toWords = "";
            int parteEntera = 0;
            int parteDecimal = 0;
            parteEntera = (int)Math.Truncate(DocTotal);
            parteDecimal = (int)((DocTotal - Math.Truncate(DocTotal)) * 100);
            toWords = " " + parteEntera.ToWords() + " CON " + parteDecimal + "/100 " + MonedaLetras;
            toWords = toWords.ToUpper();
            return toWords;
        }

        public decimal F_Tinafecto(decimal s_Tinafecto)
        {
            return s_Tinafecto;
        }
        public decimal F_Topgratuita(decimal s_Topgratuita)
        {
            return s_Topgratuita;
        }
        public decimal TotalFactura(decimal s_Topgratuita)
        {
            return DocTotal - s_Topgratuita;
        }
        public decimal Tgrabado(decimal s_Topgratuita, decimal s_Tinafecto)
        {
            return TotalFactura(s_Topgratuita) - F_Tinafecto(s_Tinafecto) - F_Topgratuita(s_Topgratuita) - Impuesto;
        }
        // metodos de sumatorias
        public decimal Sum_Tinafecto(List<NotaCreditoVentaArticulo_E> lista)
        {
            decimal suma = 0.00M;
            foreach (NotaCreditoVentaArticulo_E c in lista)
            {
                if (c.CodImpuesto != "IGV" && c.F_ItemPrecio() >= 0.02M)
                {
                    suma += c.ItemTotal;
                }
            }
            return suma;
        }
        public decimal Sum_Topgratuita(List<NotaCreditoVentaArticulo_E> lista)
        {
            decimal suma = 0.00M;
            foreach (NotaCreditoVentaArticulo_E c in lista)
            {
                if (c.F_ItemPrecio() < 0.02M)
                {
                    suma += c.ItemTotalxLot();
                }
            }
            return suma;
        }
    }
}
