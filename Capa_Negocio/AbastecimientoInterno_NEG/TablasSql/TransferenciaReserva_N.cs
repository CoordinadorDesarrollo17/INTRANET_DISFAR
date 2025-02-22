using Capa_Datos.AbastecimientoInterno_DAO.TablasExternas;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.Interfaces;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class TransferenciaReserva_N
    {
        TransferenciaReserva_D datosTransferencia = new TransferenciaReserva_D();
        public TransferenciaReserva_E RegistrarTransferenciaReserva(TransferenciaReserva_E transferencia,SqlConnection cn)
        {
                    transferencia.Detalle = transferencia.Detalle
               .Where(t => !string.IsNullOrWhiteSpace(t.UmAlm) &&
                           t.ValorUmAlm > 0 &&
                           t.QuantityUnidadesCajas > 0 &&
                           !string.IsNullOrWhiteSpace(t.CodigoUbicacion) &&
                           t.TransferenciaReservaId == 0 &&
                           t.Id == 0)
               .ToList();   

            return datosTransferencia.RegistrarTransferenciaReserva(transferencia, cn);
        }
        public TransferenciaReserva_E ObtenerTransferenciaReserva(int docNum)
        {
            return datosTransferencia.ObtenerTransferenciaReserva(docNum);
        }
        public Helper_E DeleteTransferenciaReserva(int docNum,SqlConnection cn)
        {
            return datosTransferencia.DeleteTransferenciaReserva(docNum, cn);
        }
        public Helper_E DeleteDetalleItemTransferenciaReserva(List<DetalleTransferenciaReserva_E> ids , SqlConnection cn)
        {
            return datosTransferencia.DeleteDetalleItemTransferenciaReserva(ids, cn);
        }
    }
 }
