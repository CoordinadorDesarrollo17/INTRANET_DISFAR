using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace Capa_Entidad.Rutas_ENT.TablasSql
{
    public class RRU01_E
    {
        public int Id { get; set; }
        public int DocEntryORRU { get; set; }
        public int DocEntryTicket { get; set; }
        public int Linea { get; set; }
        public string TablaSAP { get; set; }
        public string Identificador { get; set; }
        [DisplayName("Tipo")]
        public string U_SYP_MDTD { get; set; }
        [DisplayName("Serie")]
        public string U_SYP_MDSD { get; set; }
        [DisplayName("Correlativo")]
        public string U_SYP_MDCD { get; set; }
        [DisplayName("Fecha creación")]
        public string DocDate { get; set; }
        [DisplayName("Fecha Traslado")]
        public string U_BPP_FECINITRA { get; set; }
        public int Impreso { get; set; }
        public string Estado { get; set; }
        public string Operario { get; set; }
        public string FechaOperación { get; set; }
        public string HoraOperación { get; set; }
        public static List<RRU01_E> compararTemp(List<TEMP_RRU01_E> Listtemp)
        {
            List<RRU01_E> listaF = new List<RRU01_E>();
            foreach (var temp in Listtemp)
            {
                RRU01_E obj = new RRU01_E();
                obj.DocEntryTicket = temp.DocEntryTicket;
                obj.TablaSAP = temp.TablaSAP;
                obj.Identificador = temp.Identificador;
                obj.U_SYP_MDTD = temp.U_SYP_MDTD;
                obj.U_SYP_MDSD = temp.U_SYP_MDSD;
                obj.U_SYP_MDCD = temp.U_SYP_MDCD;
                obj.DocDate = temp.DocDate;
                obj.U_BPP_FECINITRA = temp.U_BPP_FECINITRA;
                obj.Impreso = temp.Impreso;
                listaF.Add(obj);
            }
            return listaF;
        }
        public static List<RRU01_E> agregarDetalles(List<RRU01_E> dt)
        {
            List<RRU01_E> lista = new List<RRU01_E>();
            int linea = 1;
            foreach (RRU01_E reg in dt)
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
        public static DataTable tbDetalle(List<RRU01_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("Id", typeof(int));
            tb.Columns.Add("DocEntryORRU", typeof(int));
            tb.Columns.Add("DocEntryTicket", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("TablaSAP", typeof(string));
            tb.Columns.Add("Identificador", typeof(string));
            tb.Columns.Add("U_SYP_MDTD", typeof(string));
            tb.Columns.Add("U_SYP_MDSD", typeof(string));
            tb.Columns.Add("U_SYP_MDCD", typeof(string));
            tb.Columns.Add("DocDate", typeof(string));
            tb.Columns.Add("U_BPP_FECINITRA", typeof(string));
            tb.Columns.Add("Impreso", typeof(string));
            tb.Columns.Add("Estado", typeof(string));
            tb.Columns.Add("Operario", typeof(string));
            tb.Columns.Add("FechaOperación", typeof(string));
            tb.Columns.Add("HoraOperación", typeof(string));

            foreach (RRU01_E reg in agregarDetalles(dt))
            {
                tb.Rows.Add(reg.Id, reg.DocEntryORRU, reg.DocEntryTicket, reg.Linea, reg.TablaSAP, reg.Identificador, reg.U_SYP_MDTD, reg.U_SYP_MDSD, reg.U_SYP_MDCD, reg.DocDate, reg.U_BPP_FECINITRA, reg.Impreso, reg.Estado, reg.Operario, reg.FechaOperación, reg.HoraOperación);

            }
            return tb;
        }

    }
}
