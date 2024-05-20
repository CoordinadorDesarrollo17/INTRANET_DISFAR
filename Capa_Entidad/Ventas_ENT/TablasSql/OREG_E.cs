using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class OREG_E
    {
        public int Id { get; set; }
        public string Categoria { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public decimal StockTotal { get; set; }
        public decimal StockDisp { get; set; }
        public decimal StockComp { get; set; }
        public string OpRegistro { get; set; }
        public string FechaRegistro { get; set; }
        public string HoraRegistro { get; set; }

    }
}