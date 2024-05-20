using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.TablasSql
{
    public class IAR12_E
    {
        public int DocEntry { get; set; }
        public int Fase { get; set; }
        public int Linea { get; set; }
        public string BatchNum { get; set; }
        public string ExpDate { get; set; }
        public decimal QuantityCajas { get; set; }
        public decimal QuantityPiezas { get; set; }
        public string ObsLote { get; set; }
        //campos no de la tabla
        public static DataTable tbDetalle(List<IAR12_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Fase", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("BatchNum", typeof(string));
            tb.Columns.Add("ExpDate", typeof(DateTime));
            tb.Columns.Add("QuantityCajas", typeof(decimal));
            tb.Columns.Add("QuantityPiezas", typeof(decimal));
            tb.Columns.Add("ObsLote", typeof(string));
            foreach (IAR12_E reg in dt)
            {
                tb.Rows.Add(reg.DocEntry, reg.Fase, reg.Linea, reg.BatchNum, DateTime.Parse(reg.ExpDate), reg.QuantityCajas
                    ,reg.QuantityPiezas,reg.ObsLote);
            }
            return tb;
        }
        
    }
}
