using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Compras_ENT
{
    public class DetLineaProduccion_E
    {
        public int id { get; set; }
        public int Linea { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Status { get; set; }
        public static List<DetLineaProduccion_E> registros(List<DetLineaProduccion_E> dt)
        {
            List<DetLineaProduccion_E> lista = new List<DetLineaProduccion_E>() {};
            int ln = 1;
            foreach (DetLineaProduccion_E d in dt)
            {
                //if (d.Status != null && d.Status != "null" && d.Status.Equals("on"))
                if (d.ItemCode != null && d.ItemCode != "null")
                {
                    d.id = 0;
                    d.Linea = ln;
                    lista.Add(d);
                    ln++;
                }
            }
            return lista;
        }
        public static DataTable tbDetalle(List<DetLineaProduccion_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("id",typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("ItemCode", typeof(string));
            tb.Columns.Add("ItemName", typeof(string));
            foreach(DetLineaProduccion_E reg in registros(dt))
            {
                tb.Rows.Add(reg.id, reg.Linea, reg.ItemCode, reg.ItemName);
            }
            return tb;
        }
    }
}
