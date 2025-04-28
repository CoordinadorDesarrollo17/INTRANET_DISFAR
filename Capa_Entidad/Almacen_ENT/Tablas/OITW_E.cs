using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.Tablas
{
    public class OITW_E
    {
        //detalles de maestro de articulos almacenes e inventarios
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        [DisplayName("Codigo de Almacén")]
        public string WhsCode { get; set; }
        [DisplayName("En Stock")]
        public decimal OnHand { get; set; }
        [DisplayName("Comprometido")]
        public decimal IsCommited { get; set; }
        [DisplayName("Pedido")]
        public decimal OnOrder { get; set; }
        public decimal StockLibre { get; set; }
        public decimal StockLibreUnidades { get; set; }

        public string Clasificacion { get; set; }
        public int StockMinAbastecimiento { get; set; }
        //metodos
        public decimal Disponible()
        {
            return OnHand - IsCommited + OnOrder;
        }

    }
}
