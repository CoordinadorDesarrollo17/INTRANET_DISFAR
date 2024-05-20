using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.TablasSql
{
    public class IEQ2_E
    {
        public int DocEntry { get; set; }
        public int Linea { get; set; }
        public int FirmCode { get; set; }
        public string U_SYP_DESC { get; set; }
        public static DataTable tbDetalle(List<IEQ2_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("FirmCode", typeof(string));
            tb.Columns.Add("U_SYP_DESC", typeof(string));
            foreach (IEQ2_E reg in dt)
            {
                tb.Rows.Add(reg.DocEntry, reg.Linea, reg.FirmCode, reg.U_SYP_DESC);
            }
            return tb;
        }
    }
}
