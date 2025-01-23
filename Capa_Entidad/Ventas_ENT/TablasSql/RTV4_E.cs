using Capa_Entidad.Ventas_ENT.Tablas;
using System.Collections.Generic;
using System.Data;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class RTV4_E
    {
        public int DocEntry { get; set; }
        public int Linea { get; set; }
        public string Verificar { get; set; }
        public ORIN_E Nc { get; set; }
        // metodos
        public static List<RTV4_E> registros(List<RTV4_E> dt4, int DocEntry)
        {
            List<RTV4_E> lista = new List<RTV4_E>();
            int ln = 1;
            foreach (RTV4_E d in dt4)
            {
                if (d.Verificar != null && d.Verificar != "null" && d.Verificar.Equals("on"))
                {
                    d.DocEntry = DocEntry;
                    d.Linea = ln;
                    lista.Add(d);
                    ln++;
                }
            }
            return lista;
        }
        public static DataTable GenerarDataTable(List<RTV4_E> dt, ORTV_E ticket)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("CardCode", typeof(string));
            tb.Columns.Add("CardName", typeof(string));
            tb.Columns.Add("DocTotal", typeof(decimal));
            tb.Columns.Add("DocDate", typeof(string));
            tb.Columns.Add("DocNum", typeof(string));

            foreach (RTV4_E reg in registros(dt, ticket.DocEntry))
            {
                tb.Rows.Add(reg.DocEntry, reg.Linea, ticket.CardCode, ticket.CardName, reg.Nc.DocTotal, reg.Nc.DocDate, reg.Nc.DocNum);
            }
            return tb;
        }


    }
}
