using System.Collections.Generic;
using System.Data;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class RTV5_E
    {
        public int? DocEntry { get; set; }
        public int? Linea { get; set; }
        public int IdReg { get; set; }
        public string RegTipo { get; set; }
        public string RegCate { get; set; }
        public decimal RegCant { get; set; }
        public string RegEstado { get; set; }

        public static List<RTV5_E> registros(List<RTV5_E> dt, int DocEntry)
        {
            List<RTV5_E> lista = new List<RTV5_E>();
            int ln = 1;
            foreach (RTV5_E reg in dt)
            {
                if (reg.IdReg > 0 && reg.RegTipo != null)
                {
                    reg.DocEntry = DocEntry;
                    reg.Linea = ln;
                    lista.Add(reg);
                    ln++;
                }


            }
            return lista;
        }

        public static DataTable GenerarDataTable(List<RTV5_E> dt, int DocEntry)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("IdReg", typeof(int));
            tb.Columns.Add("RegTipo", typeof(string));
            tb.Columns.Add("RegCate", typeof(string));
            tb.Columns.Add("RegCant", typeof(decimal));
            tb.Columns.Add("RegEstado", typeof(string));

            foreach (RTV5_E reg in registros(dt, DocEntry))
            {
                tb.Rows.Add(reg.DocEntry, reg.Linea, reg.IdReg, reg.RegTipo, reg.RegCate, reg.RegCant, reg.RegEstado);
            }
            return tb;
        }
    }
}
