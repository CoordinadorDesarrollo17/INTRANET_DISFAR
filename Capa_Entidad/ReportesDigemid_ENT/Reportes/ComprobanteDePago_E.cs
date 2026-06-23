using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Capa_Entidad.ReportesDigemid_ENT.Reportes
{
    public class ComprobanteDePago_E
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string ElaboradoPor { get; set; }
        public string TipoDoc { get; set; }
        public string SerieDoc { get; set; }
        public string CorreDoc { get; set; }
        public string NroOCCliente { get; set; }
        // Propiedades de Anticipo
        public string TotalBase { get; set; }
        public string NroAnticipo { get; set; }
        public decimal Anticipo { get; set; }
        public string FechaAnticipo { get; set; }
        public decimal AnticipoBruto { get; set; }
        public string NumGuias { get; set; }
        public string NombreSocio { get; set; }
        public string DirPagar { get; set; }
        public string Ruc { get; set; }
        public string Fecha { get; set; }
        public string FechaVencimiento { get; set; }
        public string MonedaLetras { get; set; }
        public string ItemCode { get; set; }
        public string Descripcion { get; set; }
        public string DocNumTicket { get; set; }
        public string Um { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PreUnitSinIgv { get; set; }
        public decimal Descuento { get; set; }
        public decimal PreVentaNeto { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal ItemPrecio { get; set; }
        public decimal ItemTotal { get; set; }
        public string FechaEntrega { get; set; }
        public decimal Impuesto { get; set; }
        public decimal DocTotal { get; set; }
        public decimal PorcenImpto { get; set; }
        public string LoteNum { get; set; }
        public decimal CantidadL { get; set; }
        public int TieneAnticipo { get; set; }
        public string Laboratorio { get; set; }
        public string VctoLote { get; set; }
        public decimal QUMVta { get; set; }
        public string CondPago { get; set; }
        public string NroOrdVenta { get; set; }
        public string CodImpuesto { get; set; }
        public string Almacen { get; set; }
        public string PtoPartida { get; set; }
        public string DirEnvio { get; set; }
        public string Observacion { get; set; }
        public int TipoAfectacion { get; set; }
        // metodos del layout
        //public decimal cantVta()
        //{
        //    //define si es fraccion o lote
        //    if (LoteNum == null || LoteNum == "") { return Cantidad; }
        //    else
        //    {
        //        if ((CantidadL / QUMVta) < 1) { return CantidadL; }
        //        else { return CantidadL / QUMVta; }
        //    }
        //}

        public decimal cantVta()
        {
            // Si es PZA, retornar cantidad directamente
            if (Um != null && Um.ToUpper().Contains("PZA"))
            {
                return CantidadL;
            }

            // Si es CAJA u otra unidad, hacer el cálculo
            if (LoteNum == null || LoteNum == "")
            {
                return CantidadL / QUMVta;
            }
            else
            {
                if ((CantidadL / QUMVta) < 1)
                {
                    return CantidadL;
                }
                else
                {
                    return CantidadL / QUMVta;
                }
            }
        }
        //public string UM()
        //{
        //    if ((CantidadL / QUMVta) < 1) { return "PZA"; }
        //    else { return Um; }
        //}
        public string UM()
        {
            if (!string.IsNullOrWhiteSpace(Um))
            {
                if (Um.ToUpper().Contains("PZA"))
                    return "PZA";
            }

            // Si viene vacío = es CAJA
            return "CAJA";
        }

        public decimal F_ItemPrecio()
        {
            if (PorcenImpto == 0.00M || CodImpuesto != "IGV")
            {
                if ((CantidadL / QUMVta) < 1)
                {
                    return Math.Round(PreUnitSinIgv / QUMVta, 2);
                }
                else
                {
                    return Math.Round(PreUnitSinIgv, 2);
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
            if (PorcenImpto == 0.00M)
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
            return Math.Round(s_Tinafecto, 2);
        }
        public decimal F_Topgratuita(decimal s_Topgratuita)
        {
            return Math.Round(s_Topgratuita, 2);
        }
        public decimal TotalFactura(decimal s_Topgratuita)
        {
            return Math.Round(DocTotal - s_Topgratuita, 2);
        }
        public decimal Tgrabado(decimal s_Topgratuita, decimal s_Tinafecto)
        {
            return Math.Round(TotalFactura(s_Topgratuita) - F_Tinafecto(s_Tinafecto) - Impuesto, 2);
        }

        // metodos de sumatorias
        public decimal Sum_Tinafecto(List<ComprobanteDePago_E> lista)
        {
            decimal suma = 0.00M;
            foreach (ComprobanteDePago_E c in lista)
            {
                if ( (c.CodImpuesto != "IGV" && c.F_ItemPrecio() >= 0.02M) || c.TipoAfectacion == 31)
                {
                    suma += c.PrecioVenta;
                }
            }
            return suma;
        }
        public decimal CalcularOpExoneradas(List<ComprobanteDePago_E> lista)
        {
            decimal suma = 0.00M;

            foreach (ComprobanteDePago_E c in lista.Where(x => x.CodImpuesto == "EXE_IGV" || x.TipoAfectacion == 20 ))
            {
                suma += c.PrecioVenta;
            }
            return Math.Round(suma, 2);
        }

        public decimal OpGravadas(List<ComprobanteDePago_E> lista)
        {
            decimal opGravadas = 0.00M;

            foreach (ComprobanteDePago_E c in lista.Where(x => x.CodImpuesto == "IGV"))
            {
                opGravadas += c.ItemTotal;
            }
            return Math.Round(opGravadas, 2);
        }
        public decimal Sum_Topgratuita(List<ComprobanteDePago_E> lista)
        {
            decimal suma = 0.00M;
            foreach (ComprobanteDePago_E c in lista)
            {
                if (c.F_ItemPrecio() < 0.02M || c.TipoAfectacion == 21)               
                {
                    suma += c.ItemTotalxLot();
                }
            }
            return Math.Round(suma, 2);
        }
    }
}