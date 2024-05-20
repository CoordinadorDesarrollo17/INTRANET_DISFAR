using Capa_Entidad.Ventas_ENT.TablasSql;
using System.Collections.Generic;
using System.Data;

namespace Capa_Entidad.Caja_ENT
{
    public class OPP_E
    {
        public int IdOPP { get; set; }
        public int IdOTC { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; }
        public string RegistradoPor { get; set; }
        public string FechaRegistro { get; set; }
        public string HoraRegistro { get; set; }
        public string EliminadoPor { get; set; }
        public string FechaEliminacion { get; set; }
        public string HoraEliminacion { get; set; }
        public string Comentario { get; set; }

        public static List<OPP_E> Registros(List<OPP_E> dt)
        {
            List<OPP_E> lista = new List<OPP_E>();
            foreach (OPP_E d in dt)
            {
                lista.Add(d);
            }
            return lista;
        }

        public static DataTable TbDetalle(List<OPP_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("IdOTC", typeof(int));
            tb.Columns.Add("Monto", typeof(decimal));
            tb.Columns.Add("Comentario", typeof(string));
            tb.Columns.Add("RegistradoPor", typeof(string));

            foreach (OPP_E reg in Registros(dt))
            {
                tb.Rows.Add(reg.IdOTC, reg.Monto, reg.Comentario, reg.RegistradoPor);
            }
            return tb;
        }
    }
}