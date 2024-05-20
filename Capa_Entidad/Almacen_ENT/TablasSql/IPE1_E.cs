using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.TablasSql
{
    public class IPE1_E
    {
        public int DocEntry { get; set; }
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public static DataTable tbDetalle(List<IPE1_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("WhsCode", typeof(string));
            tb.Columns.Add("WhsName", typeof(string));
            foreach (IPE1_E reg in dt)
            {
                tb.Rows.Add(reg.DocEntry, reg.WhsCode, reg.WhsName);
            }
            return tb;
        }
    }
}
