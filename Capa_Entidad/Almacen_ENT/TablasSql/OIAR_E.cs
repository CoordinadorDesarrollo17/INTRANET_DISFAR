using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.TablasSql
{
    public class OIAR_E
    {
        [DisplayName("Nro")]
        public int DocEntry { get; set; }
        public int DocEntryPer { get; set; }
        public string Propietario { get; set; }
        public int DocEntryEqu { get; set; }
        [DisplayName("Almacen")]
        public string WhsCode { get; set; }
        public int Piso { get; set; }
        [DisplayName("CodArticulo")]
        public string ItemCode { get; set; }
        public string Estado { get; set; }
        public int Fase { get; set; }
        [DisplayName("Articulo")]
        public string ItemName { get; set; }
        public int FirmCode { get; set; }
        public int FirmCode2 { get; set; }
        public decimal NumInBuy { get; set; }
        public string FechaRegistro { get; set; }
        public string HoraRegistro { get; set; }
        //campo no de la tabla
        public List<IAR1_E> DetFases { get; set; }
    }
}