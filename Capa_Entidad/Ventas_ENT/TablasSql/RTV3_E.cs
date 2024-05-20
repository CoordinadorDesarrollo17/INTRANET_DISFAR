
using System.Collections.Generic;
using System.Data;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class RTV3_E
    {
        public int? DocEntry { get; set; }
        public int? IdDireccion { get; set; }
        public int? Ubigeo { get; set; }
        public string Distrito { get; set; }
        public string Provincia { get; set; }
        public string Departamento { get; set; }
        public string Calle { get; set; }
        public string DirDestino { get; set; }
        public string Zona { get; set; }
        public static List<RTV3_E> listaDetalle(List<RTV3_E> dt, int DocEntry)
        {
            List<RTV3_E> lista = new List<RTV3_E>();
            int id = 1;
            foreach (RTV3_E reg in dt)
            {
                if (reg.Ubigeo > 0 && reg.Calle != null)
                {
                    reg.DocEntry = DocEntry;
                    reg.IdDireccion = id;
                    lista.Add(reg);
                    id++;
                }


            }
            return lista;
        }
        public static DataTable tbDetalle(List<RTV3_E> dt, int DocEntry)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("IdDireccion", typeof(int));
            tb.Columns.Add("Ubigeo", typeof(int));
            tb.Columns.Add("Distrito", typeof(string));
            tb.Columns.Add("Provincia", typeof(string));
            tb.Columns.Add("Departamento", typeof(string));
            tb.Columns.Add("Calle", typeof(string));

            foreach (RTV3_E reg in listaDetalle(dt, DocEntry))
            {
                tb.Rows.Add(reg.DocEntry, reg.IdDireccion, reg.Ubigeo, reg.Distrito, reg.Provincia, reg.Departamento, reg.Calle);
            }
            return tb;
        }
    }
}
