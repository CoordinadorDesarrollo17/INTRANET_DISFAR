using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class OLDS_E
    {
        public string CardCode { get; set; }
        [DisplayName("Cliente")] public string CardName { get; set; }
        public int NroOpe { get; set; }
        public decimal SaldoActual { get; set; }
        public List<LDS1_E> Det { get; set; }
        //metodos
    }
}
