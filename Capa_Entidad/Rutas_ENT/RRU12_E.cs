using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Rutas_ENT
{
    public class RRU12_E
    {
        public int BaseEntry { get; set; }
        public int BaseLinea { get; set; }
        public int Linea { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Lote { get; set; }
        public decimal CantidadL { get; set; }
        public int LaboCod { get; set; }
        public string LaboDesc { get; set; }
        public string UnitMed { get; set; }
        public decimal CantUnitMed { get; set; }
        public int Cajas { get; set; }
        public static DataTable tbRRU12(List<RRU12_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("BaseEntry", typeof(int));
            tb.Columns.Add("BaseLinea", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("ItemCode", typeof(string));
            tb.Columns.Add("ItemName", typeof(string));
            tb.Columns.Add("Lote", typeof(string));
            tb.Columns.Add("CantidadL", typeof(decimal));
            tb.Columns.Add("LaboCod", typeof(int));
            tb.Columns.Add("LaboDesc", typeof(string));
            tb.Columns.Add("UnitMed", typeof(string));
            tb.Columns.Add("CantUnitMed", typeof(int));
            tb.Columns.Add("Cajas", typeof(int));
            foreach (RRU12_E reg in dt)
            {
                tb.Rows.Add(reg.BaseEntry, reg.BaseLinea, reg.Linea, reg.ItemCode, reg.ItemName, reg.Lote, reg.CantidadL,
                             reg.LaboCod, reg.LaboDesc, reg.UnitMed, reg.CantUnitMed, reg.Cajas);
            }
            return tb;
        }
    }
}
