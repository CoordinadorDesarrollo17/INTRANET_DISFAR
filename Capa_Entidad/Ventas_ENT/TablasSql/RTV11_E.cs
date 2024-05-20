using System.Collections.Generic;
using System.Data;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class RTV11_E
    {
        public int DocEntry { get; set; }
        public int Linea { get; set; }
        public string Operario { get; set; }

        public static List<RTV11_E> registros(List<RTV11_E> dt, int DocEntryC)
        {
            List<RTV11_E> lista = new List<RTV11_E>();
            int ln = 1;
            foreach (RTV11_E d in dt)
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
        public static DataTable tbDet11(List<RTV11_E> dt, int DocEntryC)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("Operario", typeof(string));

            foreach (RTV11_E reg in registros(dt, DocEntryC))
            {
                tb.Rows.Add(reg.DocEntry, reg.Linea, reg.Operario);
            }
            return tb;
        }
    }
}
