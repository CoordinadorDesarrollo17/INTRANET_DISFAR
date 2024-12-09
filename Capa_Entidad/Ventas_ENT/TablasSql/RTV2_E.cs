using Capa_Entidad.Ventas_ENT.Tablas;
using System.Collections.Generic;
using System.Data;


namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class RTV2_E
    {
        public int DocEntry { get; set; }
        public int Linea { get; set; }
        public decimal Monto { get; set; }
        public int NroSap { get; set; }
        public string Verificar { get; set; }
        public string TipoComprobante { get; set; }
        public string Vendedor { get; set; }
        public string LugarDeEntrega { get; set; }
        public string Observaciones { get; set; }



        //agregado
        public string AlmacenSalida { get; set; }
        public ORIN_E Nc { get; set; }  // Notas de crédito

        public static List<RTV2_E> registros(List<RTV2_E> dt, int DocEntryC)
        {
            List<RTV2_E> lista = new List<RTV2_E>();
            int ln = 1;
            foreach (RTV2_E d in dt)
            {
                if (d.Verificar != null && d.Verificar != "null" && d.Verificar.Equals("on"))
                {
                    d.DocEntry = DocEntryC;
                    d.Linea = ln;
                    lista.Add(d);
                    ln++;
                }
            }
            return lista;
        }
        public static DataTable GenerarDataTable(List<RTV2_E> dt, int DocEntryC)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("Monto", typeof(decimal));
            tb.Columns.Add("NroSap", typeof(int));
            tb.Columns.Add("TipoComprobante", typeof(string));
            tb.Columns.Add("Vendedor", typeof(string));
            tb.Columns.Add("LugarDeEntrega", typeof(string));
            tb.Columns.Add("Observaciones", typeof(string));
            tb.Columns.Add("AlmacenSalida", typeof(string));

            foreach (RTV2_E reg in registros(dt, DocEntryC))
            {
                tb.Rows.Add(reg.DocEntry, reg.Linea, reg.Monto, reg.NroSap, reg.TipoComprobante, reg.Vendedor, reg.LugarDeEntrega,
                         reg.Observaciones, reg.AlmacenSalida);
            }
            return tb;
        }

        /*public static string NroVentas(List<RTV2_E> dt, int DocEntryC)
        {
            string NroVentas = "";
            foreach (RTV2_E d in registros(dt, DocEntryC))
            {
                NroVentas += d.NroSap + ",";
            }
            return NroVentas;
        }
        
        public static string AlmacenSalidas(List<RTV2_E> dt, int DocEntryC)
        {
            string AlmacenSalidas = "";
            foreach (RTV2_E d in registros(dt, DocEntryC))
            {
                AlmacenSalidas += d.AlmacenSalida + ",";
            }
            return AlmacenSalidas;
        }*/

    }
}
