using Capa_Entidad.Ventas_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT
{
    public class DetTicketVenta2_E
    {
        public string Verificar { get; set; }
        public int DocEntry { get; set; }
        public int Linea { get; set; }
        public ORIN_E Nc { get; set; }
        // metodos
        public static List<DetTicketVenta2_E> registros(List<DetTicketVenta2_E> dt2,int DocEntry)
        {
            List<DetTicketVenta2_E> lista = new List<DetTicketVenta2_E>();
            int ln = 1;
            foreach(DetTicketVenta2_E d in  dt2)
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
        public static DataTable tbDetalle(List<DetTicketVenta2_E> dt, TicketVenta_E ticket)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("CardCode", typeof(string));
            tb.Columns.Add("CardName", typeof(string));
            tb.Columns.Add("DocTotal", typeof(decimal));
            tb.Columns.Add("DocDate", typeof(string));
            tb.Columns.Add("DocNum", typeof(string));

            foreach (DetTicketVenta2_E reg in registros(dt, ticket.DocEntry))
            {
                tb.Rows.Add(reg.DocEntry, reg.Linea, ticket.CardCode, ticket.CardName, reg.Nc.DocTotal, reg.Nc.DocDate, reg.Nc.DocNum);
            }
            return tb;
        }
    }
}
