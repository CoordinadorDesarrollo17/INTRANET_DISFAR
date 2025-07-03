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

            if (transferencia.Detalle == null || transferencia.Detalle.Count == 0)
            {
                return new Helper_E
                {
                    Mensajes = new List<string> { "No puede transferir con detalle de transferencia vacio" },
                    Icono = "error"
                };
            }
            //1. Obtiene la solicitud de traslado completa
            var trasladoObtenido = _datosTraslado.ObtenerSolicitudDeTraslado(transferencia.SolicitudTrasladoDocNum, cn);

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
                // 3. Si las cantidades no coinciden, retorna null
                if (!cantidadTraslado.ContainsKey(item.Key) || cantidadTraslado[item.Key] != item.Value)
                {
                    return new Helper_E
                    {
                        Mensajes = new List<string> { "Cantidad que se transfiere NO coincide con el total de Sku: " + item.Key + " según documento. Valide cantidades." },
                        Icono = "error"
                    };
                }
            }
            return _datosTransferencia.RegistrarTransferenciaReserva(transferencia, cn);
        }
        public TransferenciaReserva_E ObtenerTransferenciaReserva(int docNum, SqlConnection cn)
        {
            return _datosTransferencia.ObtenerTransferenciaReserva(docNum, cn);
        }
        public Helper_E DeleteTransferenciaReserva(int docNum, SqlConnection cn)
        {
            return _datosTransferencia.DeleteTransferenciaReserva(docNum, cn);
        }
        public Helper_E DeleteDetalleItemTransferenciaReserva(List<DetalleTransferenciaReserva_E> ids, SqlConnection cn)
        {
            return _datosTransferencia.DeleteDetalleItemTransferenciaReserva(ids, cn);
        }
        public Helper_E AtenderReserva(int detalleId, SqlConnection cn)
        {
            return _datosTransferencia.AtenderReserva(detalleId, cn);
        }
        public List<DetalleTransferenciaReserva_E> ListarDetalles()
        {
            var detalles = _datosTransferencia.ListarDetalles() ?? new List<DetalleTransferenciaReserva_E>();

            return detalles
                .Where(x => x.AtendidoReserva == 0 && x.Validado == 1)
                .ToList();
        }
        public bool ValidarSkuParaKardexIngreso(int transferenciaId, string itemCode, TransferenciaReserva_E transferencia)
        {
            bool valido = !transferencia.Detalle.Any(d => d.ItemCode == itemCode && d.AtendidoReserva == 0);
            return valido;
        }
        public Helper_E ValidarSkuParaApilar(int transferenciaId, string itemCode, SqlConnection cn)
        {
            return _datosTransferencia.ValidarSkuParaApilar(transferenciaId, itemCode, cn);
        }

        public Helper_E RevertirValidarSkuParaApilar(int transferenciaId, string itemCode)
        {
            return _datosTransferencia.RevertirValidarSkuParaApilar(transferenciaId, itemCode);
        }
    }
}
