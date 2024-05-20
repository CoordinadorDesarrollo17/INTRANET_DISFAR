using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Capa_Entidad.Ventas_ENT
{
    public class LibroSaldo_E
    {
        public string CardCode { get; set; }
        [DisplayName("Cliente")]
        public string CardName { get; set; }
        public int NroOpe { get; set; }
        public decimal SaldoActual { get; set; }
        public List<DetLibroSaldo_E> Det { get; set; }
        //metodos
    }
}
