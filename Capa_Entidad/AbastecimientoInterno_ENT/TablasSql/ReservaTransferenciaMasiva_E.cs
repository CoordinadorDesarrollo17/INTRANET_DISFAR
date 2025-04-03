using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class ReservaTransferenciaMasiva_E
    {
        public int DocNumSolicitudTraslado { get; set; }
        public List<DetalleReservaTransferenciaMasiva_E> Detalles { get; set; }
    }
}
