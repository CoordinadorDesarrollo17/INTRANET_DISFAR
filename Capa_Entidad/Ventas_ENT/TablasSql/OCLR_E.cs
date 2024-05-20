using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class OCLR_E
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public int NroOpe { get; set; }
        public decimal Saldo { get; set; }
        //campos que no son de la tabla
        public List<CLR1_E> Det { get; set; }
    }
}
