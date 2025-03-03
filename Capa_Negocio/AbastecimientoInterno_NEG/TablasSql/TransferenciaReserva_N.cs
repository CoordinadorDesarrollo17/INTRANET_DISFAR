using Capa_Datos.AbastecimientoInterno_DAO.TablasExternas;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad;
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
        TransferenciaReserva_D _datosTransferencia = new TransferenciaReserva_D();
        SolicitudesTraslado_D _datosTraslado = new SolicitudesTraslado_D();
        public Helper_E RegistrarTransferenciaReserva(TransferenciaReserva_E transferencia, SqlConnection cn)
        {
            //Limpiamos con los datos que unicamente vienen con intencion de trasladarse:
            transferencia.Detalle = transferencia.Detalle
                .Where(t => !string.IsNullOrWhiteSpace(t.UmAlm) &&
                            t.ValorUmAlm > 0 &&
                            t.QuantityUnidadesCajas > 0 &&
                            !string.IsNullOrWhiteSpace(t.CodigoUbicacion) &&
                            t.TransferenciaReservaId == 0 &&
                            t.Id == 0)
                .ToList();

            //Validar si el SKU ha sido transferido en su totalidad segun la cantidad que figure en la solicitud de traslado:

            //1. Obtiene la solicitud de traslado completa
            var trasladoObtenido=_datosTraslado.ObtenerSolicitudDeTraslado(transferencia.SolicitudTrasladoDocNum);

            //2. Compara las cantidades agrupadas por ItemCode
            var cantidadTransferencia = transferencia.Detalle
                .Where(item => !string.IsNullOrEmpty(item.ItemCode)) // Filtra nulos o vacíos
                .GroupBy(item => item.ItemCode)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.QuantityUnidadesCajas));

            var cantidadTraslado = trasladoObtenido.Detalle
                .Where(item => item.Value != null && !string.IsNullOrEmpty(item.Value.ItemCode))
                .GroupBy(item => item.Value.ItemCode)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.Value.QuantityCajas));


            foreach (var item in cantidadTransferencia)
            {
                // Si las cantidades no coinciden, retorna null
                if (!cantidadTraslado.ContainsKey(item.Key) || cantidadTraslado[item.Key] != item.Value)
                {
                    return new Helper_E
                    {
                        Mensajes= new List<string>{ "Cantidad a transferir no cubre total de Sku: "+item.Key},
                        IconoSweetAlert = "error"
                    };
                }
            }
            return _datosTransferencia.RegistrarTransferenciaReserva(transferencia, cn);
        }
        public TransferenciaReserva_E ObtenerTransferenciaReserva(int docNum)
        {
            return _datosTransferencia.ObtenerTransferenciaReserva(docNum);
        }
        public Helper_E DeleteTransferenciaReserva(int docNum,SqlConnection cn)
        {
            return _datosTransferencia.DeleteTransferenciaReserva(docNum, cn);
        }
        public Helper_E DeleteDetalleItemTransferenciaReserva(List<DetalleTransferenciaReserva_E> ids , SqlConnection cn)
        {
            return _datosTransferencia.DeleteDetalleItemTransferenciaReserva(ids, cn);
        }
    }
 }
