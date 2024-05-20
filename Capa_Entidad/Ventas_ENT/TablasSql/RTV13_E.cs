using System.Collections.Generic;
using System.Data;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class RTV13_E
    {
        public int DocEntry { get; set; }
        public int Linea { get; set; }
        public string Operario { get; set; }

        public static List<RTV13_E> registros(List<RTV13_E> dt, int DocEntryC)
        {
            List<RTV13_E> lista = new List<RTV13_E>();
            int ln = 1;
            foreach (RTV13_E d in dt)
            {
                if (!string.IsNullOrEmpty(d.Operario))
                {
                    d.DocEntry = DocEntryC;
                    d.Linea = ln;
                    lista.Add(d);
                    ln++;
                }
            }
            return lista;
        }
        public static DataTable tbDet13(List<RTV13_E> dt, int DocEntryC)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("Operario", typeof(string));

            foreach (RTV13_E reg in registros(dt, DocEntryC))
            {
                tb.Rows.Add(reg.DocEntry, reg.Linea, reg.Operario);
            }
            return tb;
        }
    }
}
