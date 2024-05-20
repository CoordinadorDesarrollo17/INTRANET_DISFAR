using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Compras_ENT
{
    public class DetContratoRebate_E
    {
        public int idOCRT { get; set; }
        public int Linea { get; set; }
        public int idORLP { get; set; }
        public string Descripcion { get; set; }
        public string U_SYP_DESC { get; set; }
        public string CardName { get; set; }
        public string PeriodoRebate { get; set; }
        public string SubTipo { get; set; } //
        public string ConDev { get; set; }
        // no en bd
        public string Status { get; set; }
        public List<EspDetContratoRebate2_E> EspDet2 { get; set; }
        public List<EspDetContratoRebate3_E> EspDet3 { get; set; }
        // metodos
        public static List<DetContratoRebate_E> registros(List<DetContratoRebate_E> dt)
        {
            List<DetContratoRebate_E> lista = new List<DetContratoRebate_E>();
            int ln = 1;
            foreach(DetContratoRebate_E d in dt)
            {
                if(d.Status != null && d.Status != "null" && d.Status.Equals("on") && d.EspDet2!=null)
                {
                    d.idOCRT = 0;
                    d.Linea = ln;
                    d.EspDet2 = EspDetContratoRebate2_E.registros(d.EspDet2, ln); 
                    lista.Add(d);
                    ln++;
                }
            }
            return lista;
        }
        public static DataTable tbDetalle(List<DetContratoRebate_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("idOCRT", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("idORLP", typeof(int));
            tb.Columns.Add("Descripcion", typeof(string));
            tb.Columns.Add("U_SYP_DESC", typeof(string));
            tb.Columns.Add("CardName", typeof(string));
            tb.Columns.Add("PeriodoRebate", typeof(string));
            tb.Columns.Add("SubTipo", typeof(string));
            tb.Columns.Add("ConDev", typeof(string));
            foreach (DetContratoRebate_E reg in registros(dt))
            {
                tb.Rows.Add(reg.idOCRT, reg.Linea, reg.idORLP, reg.Descripcion,reg.U_SYP_DESC
                    ,reg.CardName,reg.PeriodoRebate,reg.SubTipo,reg.ConDev);
            }
            return tb;
        }
        public static DataTable tbEspDet2(List<DetContratoRebate_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("idOCRT", typeof(int));
            tb.Columns.Add("BaseLinea", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("Rango", typeof(string));
            tb.Columns.Add("CuotaMin", typeof(decimal));
            tb.Columns.Add("Rebate", typeof(decimal));
            tb.Columns.Add("MaxDia", typeof(int));
            tb.Columns.Add("InfoDia", typeof(int));
            tb.Columns.Add("UltimasFacturas", typeof(int));
            tb.Columns.Add("MinDia", typeof(int));
            tb.Columns.Add("NroFactura", typeof(string));
            tb.Columns.Add("RangoF", typeof(string));
            tb.Columns.Add("Displays", typeof(decimal));
            foreach (DetContratoRebate_E reg in registros(dt))
            {
                foreach(EspDetContratoRebate2_E regEsp in reg.EspDet2)
                {
                    tb.Rows.Add(regEsp.idOCRT, regEsp.BaseLinea, regEsp.Linea, regEsp.Rango
                        , regEsp.CuotaMin, regEsp.Rebate,regEsp.MaxDia,regEsp.InfoDia
                        ,regEsp.UltimasFacturas,regEsp.MinDia,regEsp.NroFactura,regEsp.RangoF,regEsp.Displays);
                }
            }
            return tb;
        }
    }
}
