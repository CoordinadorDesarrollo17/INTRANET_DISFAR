using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.TablasSql
{
    public class IAR1_E
    {
        public int DocEntry { get; set; }
        public int Fase { get; set; }
        public string NombreFase { get; set; }
        public string TipoOperario { get; set; }
        public string Operario { get; set; }
        public string FechaFase { get; set; }
        public string HoraFase { get; set; }
        public string Observacion { get; set; }
        //campos no tabla
        public List<IAR11_E> DetApoyos { get; set; }
        public List<IAR12_E> DetContab { get; set; }
        //constructor
        public IAR1_E()
        {
            DetApoyos = new List<IAR11_E>();
            DetContab = new List<IAR12_E>();
        }
        //metodos
        public static DataTable tbDetalle(List<IAR1_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Fase", typeof(int));
            tb.Columns.Add("NombreFase", typeof(string));
            tb.Columns.Add("TipoOperario", typeof(string));
            tb.Columns.Add("Operario", typeof(string));
            tb.Columns.Add("FechaFase", typeof(string));
            tb.Columns.Add("HoraFase", typeof(string));
            tb.Columns.Add("Observacion", typeof(string));
            foreach (IAR1_E reg in dt)
            {
                tb.Rows.Add(reg.DocEntry, reg.Fase, reg.NombreFase, reg.TipoOperario, reg.Operario,
                    reg.FechaFase, reg.HoraFase, reg.Observacion);
            }
            return tb;
        }
        public string enlistarLotes()
        {
            string lista = "";
            foreach (IAR12_E obj in DetContab)
            {
                lista += obj.BatchNum + ",\n";
            }
            return lista;
        }
    }
}