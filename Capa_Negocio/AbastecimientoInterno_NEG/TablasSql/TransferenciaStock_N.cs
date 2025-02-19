using Capa_Datos.AbastecimientoInterno_DAO.TablasExternas;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class TransferenciaStock_N
    {
        TransferenciaReserva_D datosTransferencia = new TransferenciaReserva_D();
        public Helper_E RegistrarTransferenciaDeStock(TransferenciaReserva_E transferencia)
        {
            return datosTransferencia.RegistrarTransferenciaDeStock(transferencia);
        }
    }
 }
