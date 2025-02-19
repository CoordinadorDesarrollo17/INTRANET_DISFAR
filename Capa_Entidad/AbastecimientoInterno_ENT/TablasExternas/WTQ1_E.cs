
using System;
using System.Collections.Generic;

namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasExternas
{
    public class WTQ1_E
    {
        public string AlmacenOrigen { get; set; }
        public string AlmacenDestino { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal CantidadTotalPorSKU { get; set; }
        public List<WTQ1_Lotes_E> DetalleLotes { get; set; }

        public static implicit operator List<object>(WTQ1_E v)
        {
            throw new NotImplementedException();
        }
    }
}
