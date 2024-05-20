using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.General_ENT.Tablas
{
    public class COB_SALDO_E
    {
        [DisplayName("Orden")]
        public string Code { get; set; }
        [DisplayName("Codigo")]
        public string Name { get; set; }
        [DisplayName("Descripcion")]
        public string U_COB_DESCRIPCION { get; set; }
        [DisplayName("TipoControlado")]
        public string U_COB_TIPOCONT { get; set; }
        [DisplayName("Trimestre")]
        public int U_COB_TRIMESTRE { get; set; }
        [DisplayName("SaldoActual")]
        public decimal U_COB_SALDOANTE { get; set; }
    }
}
