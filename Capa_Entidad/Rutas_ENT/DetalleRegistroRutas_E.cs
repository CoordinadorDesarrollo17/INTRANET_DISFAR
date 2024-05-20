using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Capa_Entidad.Ventas_ENT;

namespace Capa_Entidad.Rutas_ENT
{
    public class DetalleRegistroRutas_E
    {
        public int DocEntry { get; set; }
        public int Linea { get; set; }
        public int DocEntryTicket { get; set; }
        public int DocNumTicket { get; set; }
        public string Guias { get; set; }
        public string Verificado { get; set; }
        public TicketVenta_E Ticket { get; set; }
        public static List<DetalleRegistroRutas_E>  listaFinalDetalles(List<DetalleRegistroRutas_E> dt)
        {
            List<DetalleRegistroRutas_E> lista = new List<DetalleRegistroRutas_E>();
            int linea = 1;
            foreach (DetalleRegistroRutas_E reg in dt)
            {
                if (reg.DocEntryTicket > 0)
                {
                    reg.Linea = linea;
                    lista.Add(reg);
                    linea++;
                }
            }
            return lista;
        }
        public static DataTable tbDetalle(List<DetalleRegistroRutas_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("DocEntryTicket", typeof(int));
            tb.Columns.Add("DocNumTicket", typeof(int));
            tb.Columns.Add("Guias", typeof(string));
            tb.Columns.Add("Verificado", typeof(string));

            foreach (DetalleRegistroRutas_E reg in listaFinalDetalles(dt))
            {
                tb.Rows.Add(reg.DocEntry,reg.Linea,reg.DocEntryTicket,reg.DocNumTicket, reg.Guias, reg.Verificado);
            }
            return tb;
        }
    }
}
