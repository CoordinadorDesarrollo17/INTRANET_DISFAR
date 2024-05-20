using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.TablasSql
{
    public class IAR11_E
    {
        public int DocEntry { get; set; }
        public int Fase { get; set; }
        public int Linea { get; set; }
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string ObsApoyo { get; set; }

        //campos no de la tabla
        public static DataTable tbDetalle(List<IAR11_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Fase", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("Id", typeof(string));
            tb.Columns.Add("Nombre", typeof(string));
            tb.Columns.Add("ObsApoyo", typeof(string));
            foreach (IAR11_E reg in dt)
            {
                tb.Rows.Add(reg.DocEntry, reg.Fase, reg.Linea, reg.Id, reg.Nombre, reg.ObsApoyo);
            }
            return tb;
        }
    }
}
