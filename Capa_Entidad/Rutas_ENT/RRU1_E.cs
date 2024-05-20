using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Rutas_ENT
{
    public class RRU1_E
    {
        public int DocEntry { get; set; }
        public int Linea { get; set; }
        public string Otros { get; set; }
        public string Guia { get; set; }
        public int NroSap { get; set; }
        public string OpEnvio { get; set; }
        public int Cajas { get; set; }
        public string OpRecepcion { get; set; }
        public string Verificado { get; set; }
        public List<RRU12_E> ListaRRU12 { get; set; }
        public static List<RRU1_E> listaFinalDetalles(List<RRU1_E> dt)
        {
            List<RRU1_E> lista = new List<RRU1_E>();
            int linea = 1;
            foreach (RRU1_E reg in dt)
            {
                if (reg.Guia !=null && reg.Guia.Length>0)
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
            tb.Columns.Add("Otros", typeof(string));
            tb.Columns.Add("Guia", typeof(string));
            tb.Columns.Add("NroSap", typeof(int));
            tb.Columns.Add("OpEnvio", typeof(string));
            tb.Columns.Add("Cajas", typeof(int));
            tb.Columns.Add("OpRecepcion", typeof(string));
            tb.Columns.Add("Verificado", typeof(string));

            foreach (RRU1_E reg in listaFinalDetalles(dt))
            {
                tb.Rows.Add(reg.DocEntry, reg.Linea,reg.Otros, reg.Guia, reg.NroSap, reg.OpEnvio, reg.Cajas,reg.OpRecepcion,reg.Verificado);
            }
            return tb;
        }
    }
}
