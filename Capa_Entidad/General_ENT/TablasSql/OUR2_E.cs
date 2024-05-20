using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.General_ENT.TablasSql
{
    public class OUR2_E
    {
        public int Id { get; set; }
        public int IdCourier { get; set; }
        public string Destino { get; set; }
        public decimal PrecioBase { get; set; }
        public decimal TarifaKg { get; set; }
        /************************/
        public string NombreAgencia { get; set; }
    }
}
