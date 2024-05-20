using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Compras_ENT
{
    public class ProveedorListaNegra_E
    {
        public string CardCode { get; set; }
        [DisplayName("Proveedor")]
        public string CardName { get; set; }
    }
}
