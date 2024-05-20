using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Capa_Entidad.Rutas_ENT.TablasSql
{
    public class RRU1_E
    {
        public int DocEntry { get; set; }
        public int Linea { get; set; }
        public string Guia { get; set; }
        public int NroSap { get; set; }
        public string OpEnvio { get; set; }
        public int Cajas { get; set; }
        public string OpRecepcion { get; set; }
        public string Verificado { get; set; }
        public string Estado { get; set; }
        public decimal TempI1 { get; set; }
        public decimal TempI2 { get; set; }
        public decimal HumedI1 { get; set; }
        public decimal HumedI2 { get; set; }
        public decimal TempF1 { get; set; }
        public decimal TempF2 { get; set; }
        public decimal HumedF1 { get; set; }
        public decimal HumedF2 { get; set; }
        public string OpEntrega { get; set; }
        public string FechaEntrega { get; set; }
        public string HoraEntrega { get; set; }
        //no de tabla
        public HttpPostedFileBase Archivo { get; set; }
        public List<RRU11_E> ListaRRU11 { get; set; }
        public static List<RRU1_E> listaFinalDetalles(List<RRU1_E> dt)
        {
            List<RRU1_E> lista = new List<RRU1_E>();
            int linea = 1;
            foreach (RRU1_E reg in dt)
            {
                if (reg.Guia != null && reg.Guia.Length > 0)
                {
                    reg.Linea = linea;
                    lista.Add(reg);
                    linea++;
                }
            }
            return lista;
        }
        public static DataTable tbDetalle(List<RRU1_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("Guia", typeof(string));
            tb.Columns.Add("NroSap", typeof(int));
            tb.Columns.Add("OpEnvio", typeof(string));
            tb.Columns.Add("Cajas", typeof(int));
            tb.Columns.Add("OpRecepcion", typeof(string));
            tb.Columns.Add("Verificado", typeof(string));
            tb.Columns.Add("Estado", typeof(string));
            tb.Columns.Add("TempI1", typeof(decimal));
            tb.Columns.Add("HumedI1", typeof(decimal));
            tb.Columns.Add("TempI2", typeof(decimal));
            tb.Columns.Add("HumedI2", typeof(decimal));
            tb.Columns.Add("TempF1", typeof(decimal));
            tb.Columns.Add("HumedF1", typeof(decimal));
            tb.Columns.Add("TempF2", typeof(decimal));
            tb.Columns.Add("HumedF2", typeof(decimal));
            tb.Columns.Add("OpEntrega", typeof(string));
            tb.Columns.Add("FechaEntrega", typeof(string));
            tb.Columns.Add("HoraEntrega", typeof(string));

            foreach (RRU1_E reg in listaFinalDetalles(dt))
            {
                tb.Rows.Add(reg.DocEntry, reg.Linea, reg.Guia, reg.NroSap, reg.OpEnvio, reg.Cajas, reg.OpRecepcion,
                    reg.Verificado, reg.Estado, reg.TempI1, reg.HumedI1, reg.TempI2, reg.HumedI2, reg.TempF1,
                    reg.HumedF1, reg.TempF2, reg.HumedF2, reg.OpEntrega, reg.FechaEntrega, reg.HoraEntrega);
            }
            return tb;
        }

    }
}