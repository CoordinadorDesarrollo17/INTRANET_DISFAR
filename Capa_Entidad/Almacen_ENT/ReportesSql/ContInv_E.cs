using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.ReportesSql
{
    // entidad para el reporte de contabilizacion de invetario
    public class ContInv_E
    {
        public int Orden { get; set; }
        public int DocEntryPer { get; set; }
        public string DescripcionPer { get; set; }
        public string FecIniPer { get; set; }
        public string FecFinPer { get; set; }
        public int Fase { get; set; }
        public string NombreFase { get; set; }
        //detalles
        public string WhsCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        //analisis
        public string BatchNum { get; set; }
        public string ExpDate { get; set; }
        public string Pisos { get; set; }
        // fin analisis
        public decimal QuantityCajas { get; set; }
        public decimal QuantityPiezas { get; set; }
        public decimal QuantityTotalPzCont { get; set; }
        public decimal QuantityTotalPzSist { get; set; }
        public decimal DifConteo { get; set; }
        public decimal DifReConteo { get; set; }
        public decimal DifAnalisis { get; set; }
        public decimal DifDif { get; set; }
        public string Resultado { get; set; }
        public string Equipos { get; set; }
        public decimal NumInBuy { get; set; }
        public decimal AvgPrice { get; set; }
        //analisis
        public string ObsLoteC { get; set; }
        public string ObsLoteR { get; set; }
        public string ObsLoteA { get; set; }
        public string ParticipantesC { get; set; }
        public string ParticipantesR { get; set; }
        public string ParticipantesA { get; set; }
        //metodos
        public void inicializarConteo()
        {
            if (NumInBuy == 0) { NumInBuy = 1; }
            QuantityTotalPzCont = QuantityCajas * NumInBuy + QuantityPiezas;
            DifConteo = QuantityTotalPzCont - QuantityTotalPzSist;
            if (DifConteo > 0) { Resultado = "Sobrante"; }
            else if (DifConteo < 0) { Resultado = "Faltante"; }
            else { Resultado = "Ok"; }
        }
        public void inicializarReconteo()
        {
            if (NumInBuy == 0) { NumInBuy = 1; }
            QuantityTotalPzCont = QuantityCajas * NumInBuy + QuantityPiezas;
            DifReConteo = QuantityTotalPzCont - QuantityTotalPzSist;
            if (DifReConteo > 0) { Resultado = "Sobrante"; }
            else if (DifReConteo < 0) { Resultado = "Faltante"; }
            else { Resultado = "Ok"; }
            DifDif = DifConteo - DifReConteo;
        }
        public void inicializarAnalisis()
        {
            if (NumInBuy == 0) { NumInBuy = 1; }
            QuantityTotalPzCont = QuantityCajas * NumInBuy + QuantityPiezas;
            if ((QuantityTotalPzCont - QuantityTotalPzSist)>0){Resultado = "Sobrante";}
            else if((QuantityTotalPzCont - QuantityTotalPzSist) < 0) { Resultado = "Faltante"; }
            else { Resultado = "Ok"; }
        }
        
    }
}
