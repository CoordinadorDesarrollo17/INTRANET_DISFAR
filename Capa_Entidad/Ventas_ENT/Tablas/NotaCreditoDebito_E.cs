using Humanizer;
using System;
using System.Collections.Generic;

namespace Capa_Entidad.Ventas_ENT.Tablas
{
    public class NotaCreditoDebito_E
    {
        public int DocEntry { get; set; }
        public string ElaboradoPor { get; set; }
        public string NombreBD { get; set; }
        public string DireccionBD { get; set; }
        public string RucBD { get; set; }
        public string TipoDoc { get; set; }
        public string SerieDoc { get; set; }
        public string CorreDoc { get; set; }
        public string TipoDocRel { get; set; }
        public string SerieDocRel { get; set; }
        public string CorreDocRel { get; set; }
        public string DocDateRel { get; set; }
        public string Motivo { get; set; }
        public int DocNum { get; set; }
        public int SerieSap { get; set; }
        public string NombreSocio { get; set; }
        public string DirDestino { get; set; }
        public string DirPagar { get; set; }
        public string DistritoCli { get; set; }
        public string Ruc { get; set; }
        public string Email { get; set; }
        public string DocDate { get; set; }
        public string Moneda { get; set; }
        public string MonedaLetras { get; set; }
        public string Telefonos { get; set; }
        public string DescripcionLinea { get; set; }
        public string Um { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioLinea { get; set; }
        public string FechaEntrega { get; set; }
        public string LugarEntrega { get; set; }
        public string CondicionPago { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal DocTotal { get; set; }
        public decimal ImpuestoPorcentaje { get; set; }
        public string CodigoGasto { get; set; }
        public string TipoDocumentoSAP { get; set; }
        public string LoteNum { get; set; }
        public string Laboratorio { get; set; }
        public decimal CantidadL { get; set; }
        public decimal QUMVta { get; set; }
        public decimal Descuento { get; set; }
        public decimal PreVentaNeto { get; set; }
        public decimal PreUnitSinIgv { get; set; }
        public decimal PrecioLineaTotal { get; set; }
        public string VencLote { get; set; }
        public string CodImpuesto { get; set; }
        public string UmLinea { get; set; }
        public string TipoDescripcionC { get; set; }
        public string ItemCode { get; set; }
        public string FechaVencimiento { get; set; }
        public string MotivoNC { get; set; }
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

        // metodos del layout
        public decimal cantVta()
        {
            if (LoteNum == null || LoteNum == "") { return Cantidad; }
            else
            {
                if ((CantidadL / QUMVta) < 1) { return CantidadL; }
                else { return CantidadL / QUMVta; }
            }
        }
        public string UM()
        {
            if ((CantidadL / QUMVta) < 1) { return "PZA"; }
            else { return Um; }
        }
        public decimal F_ItemPrecio()
        {
            if (ImpuestoPorcentaje == 0.00M || CodImpuesto != "IGV")
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
            if (ImpuestoPorcentaje == 0.00M)
            {
                return cantVta() * PreVentaNeto;
            }
            else
            {
                return cantVta() * PreVentaNeto;
            }
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
        public decimal Sum_Tinafecto(List<NotaCreditoDebito_E> lista)
        {
            decimal suma = 0.00M;
            foreach (NotaCreditoDebito_E c in lista)
            {
                if (c.CodImpuesto != "IGV" && c.F_ItemPrecio() >= 0.02M)
                {
                    suma += c.PrecioLinea;
                }
            }
            return suma;
        }
        public decimal Sum_Topgratuita(List<NotaCreditoDebito_E> lista)
        {
            decimal suma = 0.00M;
            foreach (NotaCreditoDebito_E c in lista)
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