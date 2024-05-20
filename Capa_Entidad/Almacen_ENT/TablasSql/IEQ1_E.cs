using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.TablasSql
{
    public class IEQ1_E
    {
        public int DocEntry { get; set; }
        public int Linea { get; set; }
        public string Id { get; set; }
        public string Nombre { get; set; }
        //metodos
        public static DataTable tbDetalle(List<IEQ1_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("Id", typeof(string));
            tb.Columns.Add("Nombre", typeof(string));
            foreach (IEQ1_E reg in dt)
            {
                tb.Rows.Add(reg.DocEntry,reg.Linea, reg.Id, reg.Nombre);
            }
            return tb;
        }
    }
}
