using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class RTV1_E
    {
        public int? DocEntry { get; set; }
        public int? Linea { get; set; }
        public string NombrePer { get; set; }
        public string TelfPer { get; set; }
        public string TipoDocPer { get; set; }
        public string DocPer { get; set; }

        public static List<RTV1_E> registros(List<RTV1_E> dt, int DocEntryC)
        {
            List<RTV1_E> lista = new List<RTV1_E>();
            int ln = 1;
            foreach (RTV1_E d in dt)
            {
                if (d.DocEntry > 0)
                {
                    d.DocEntry = DocEntryC;
                    d.Linea = ln;
                    lista.Add(d);
                    ln++;
                }
            }
            return lista;
        }
        public static DataTable tbDetalle(List<RTV1_E> dt, int DocEntryC)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("NombrePer", typeof(string));
            tb.Columns.Add("TelfPer", typeof(string));
            tb.Columns.Add("TipoDocPer", typeof(string));
            tb.Columns.Add("DocPer", typeof(string));

            foreach (RTV1_E reg in registros(dt, DocEntryC))
            {
                tb.Rows.Add(reg.DocEntry, reg.Linea, reg.NombrePer, reg.TelfPer, reg.TipoDocPer, reg.DocPer);
            }
            return tb;
        }
    }
}
