using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class RTV6_E
    {
        public string Verificar { get; set; }
        public int DocEntry { get; set; }
        public int Linea { get; set; }
        public decimal Peso { get; set; }
        public string UniMed { get; set; }
        //public string Observacion { get; set; }
        public decimal PrecioEnv { get; set; }
        // Campos que no son de tabla BD
        public decimal PesoTotal { get; set; }
        public static List<RTV6_E> registros(List<RTV6_E> dt2, int DocEntry)
        {
            List<RTV6_E> lista = new List<RTV6_E>();
            int ln = 1;
            foreach (RTV6_E d in dt2)
            {
                if (d.Verificar != null && d.Verificar != "null" && d.Verificar.Equals("on"))
                {
                    d.DocEntry = DocEntry;
                    d.Linea = ln;
                    lista.Add(d);
                    ln++;
                }
            }
            return lista;
        }
        public static DataTable tbDetalle(List<RTV6_E> dt, int DocEntry)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("Peso", typeof(decimal));
            tb.Columns.Add("UniMed", typeof(string));
            tb.Columns.Add("PrecioEnv", typeof(decimal));

            foreach (RTV6_E reg in registros(dt, DocEntry))
            {
                tb.Rows.Add(DocEntry, reg.Linea, reg.Peso, reg.UniMed, reg.PrecioEnv);
            }
            return tb;
        }
    }
}
