using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Compras_ENT
{
    public class EspDetContratoRebate2_E
    {
        public int idOCRT { get; set; }
        public int BaseLinea { get; set; }
        public int Linea { get; set; }
        public string Rango { get; set; }
        public decimal CuotaMin { get; set; }
        public decimal Rebate { get; set; }
        public int MaxDia { get; set; }// primeros dias
        public int InfoDia { get; set; } // por informacion
        public int UltimasFacturas { get; set; } //
        public int MinDia { get; set; }
        public string NroFactura { get; set; }
        public string RangoF { get; set; }
        public decimal Displays { get; set; }
        //metodos
        public static List<EspDetContratoRebate2_E> registros(List<EspDetContratoRebate2_E> dt,int BaseLinea)
        {
            List<EspDetContratoRebate2_E> lista = new List<EspDetContratoRebate2_E>();
            int ln = 1;
            foreach(EspDetContratoRebate2_E esp in dt)
            {
                esp.BaseLinea = BaseLinea;
                esp.Linea = ln;
                lista.Add(esp);
                ln++;
            }
            return lista;
        }
    }
}
