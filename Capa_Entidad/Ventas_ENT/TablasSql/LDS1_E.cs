using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class LDS1_E
    {
        [Required]
        public string CardCode { get; set; }
        public int Linea { get; set; }
        [Required]
        public string FechaOpe { get; set; }
        [Required]
        public string Operacion { get; set; }
        public string DetOpe { get; set; }
        public decimal Ingreso { get; set; }
        public decimal Egreso { get; set; }
        public decimal Saldo { get; set; }
        public string FechaReg { get; set; }
        public string HoraReg { get; set; }
        public string OperarioReg { get; set; }
    }
}