using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class CLR1_E
    {
        public string CardCode { get; set; }
        public int? IdReg { get; set; }
        public string Categoria { get; set; }
        public string Tipo { get; set; }
        public decimal? Cantidad { get; set; }
        //
        public static DataTable tbDetalle(List<CLR1_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("CardCode", typeof(string));
            tb.Columns.Add("IdReg", typeof(int));
            tb.Columns.Add("Categoria", typeof(string));
            tb.Columns.Add("Tipo", typeof(string));
            tb.Columns.Add("Cantidad", typeof(decimal));


            foreach (CLR1_E reg in dt)
            {
                tb.Rows.Add(reg.CardCode, reg.IdReg, reg.Categoria, reg.Tipo, reg.Cantidad);
            }
            return tb;
        }
    }
}
