using System.Collections.Generic;
using System.Data;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class RTV7_E
    {
        public int? DocEntry { get; set; }
        public int? Linea { get; set; }
        public int? DocNumVinc { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public decimal MontoFinal { get; set; }
        //pasar datos a formato DataTable
        public static DataTable GenerarDataTable(List<RTV7_E> dt, int DocEntry)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("DocNumVinc", typeof(int));
            tb.Columns.Add("CardCode", typeof(string));
            tb.Columns.Add("CardName", typeof(string));
            tb.Columns.Add("MontoFinal", typeof(decimal));

            int ln = 1;
            foreach (RTV7_E reg in dt)
            {
                if (!string.IsNullOrEmpty(reg.CardCode) && reg.DocNumVinc > 0)
                {
                    DataRow fila = tb.NewRow();
                    fila["DocEntry"] = DocEntry;
                    fila["Linea"] = ln;
                    fila["DocNumVinc"] = reg.DocNumVinc;
                    fila["CardCode"] = reg.CardCode;
                    fila["CardName"] = reg.CardName;
                    fila["MontoFinal"] = reg.MontoFinal;
                    tb.Rows.Add(fila);
                    ln++;
                }
            }
            return tb;
        }

    }
}
