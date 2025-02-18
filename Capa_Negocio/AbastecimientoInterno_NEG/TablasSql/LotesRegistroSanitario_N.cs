using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class LotesRegistroSanitario_N
    {
        LotesRegistroSanitario_D lotesDatos = new LotesRegistroSanitario_D();
        public void ValidarLotesRegistroSanitario(List<DetTransferenciaStock_E> detalleTransferencia)
        {
            lotesDatos.ValidarLotesRegistroSanitario(detalleTransferencia);
        }
    }
}
