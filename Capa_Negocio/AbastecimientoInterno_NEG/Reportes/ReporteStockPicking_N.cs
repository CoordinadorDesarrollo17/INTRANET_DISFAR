using Capa_Entidad.AbastecimientoInterno_ENT.Reportes;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.AbastecimientoInterno_DAO.Reportes;
using Capa_Negocio.Almacen_NEG.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasSql;

namespace Capa_Negocio.AbastecimientoInterno_NEG.Reportes
{
    public class ReporteStockPicking_N
    {
        ReporteStockPicking_D _datosReporte = new ReporteStockPicking_D();
        private readonly OITW_N _oitwN = new OITW_N();
        private readonly Requerimientos_N _requerimientosN = new Requerimientos_N();
        private readonly UbicacionesLotes_N _ubicacionesLotesN = new UbicacionesLotes_N();

        public List<ReporteStockPicking_E> ControlStockInternoPicking()
        {
            var controlStockInternoPicking = _datosReporte.ControlHistoricoDeIngresosAPicking();
            List<OITW_E> articulos = new Capa_Negocio.Almacen_NEG.Tablas.OITW_N()
                .ListarDetArticulosInv(new OITW_E { WhsCode = "16" })
                .Where(x => x.OnHand > 0/* && x.ItemCode == "RBHEA0007"*/)
                .ToList();

            if (articulos != null && articulos.Any())
            {
                var itemCodes = articulos.Select(a => a.ItemCode).Distinct().ToList();

                // Precarga de detalles de requerimiento para todos los ítems
                var detallesReq = _requerimientosN.ListarDetalles(itemCodes, "CantidadSolicitada");
                var detallesAgrupados = detallesReq
                    .GroupBy(r => r.ItemCode)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.QuantityUnidadesCajas));

                // Precarga de ubicaciones de lotes para todos los ítems
                var ubicacionesLotes = _ubicacionesLotesN.ObtenerDatosPorItemCode(itemCodes);
                var ubicacionesAgrupadas = ubicacionesLotes
                    .Where(x => x.Almacen == "RESERVA")
                    .GroupBy(x => x.ItemCode)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.QuantityUnidadesCajas));

                // Indexación del control interno por ItemCode
                var controlPorItemCode = controlStockInternoPicking
                    .GroupBy(x => x.ItemCode)
                    .ToDictionary(g => g.Key, g => g.First());

                foreach (var item in articulos)
                {
                    detallesAgrupados.TryGetValue(item.ItemCode, out var quantityReq);
                    ubicacionesAgrupadas.TryGetValue(item.ItemCode, out var quantityUbicacionesLote);

                    int stockDeAlmReserva = quantityUbicacionesLote - (quantityReq ?? 0);
                    decimal stockEnPicking = item.StockLibreUnidades - stockDeAlmReserva;

                    item.StockPicking = stockEnPicking;

                    if (controlPorItemCode.TryGetValue(item.ItemCode, out var control))
                    {
                        item.Clasificacion = control.Clasificacion;
                        item.StockMinAbastecimiento = (item.StockLibreUnidades > 0) ? control.StockMinAbastecimiento : 0;
                    }
                    else
                    {
                        item.Clasificacion = string.Empty;
                        item.StockMinAbastecimiento = 0;
                    }
                }
            }

            // Crear la nueva lista solo con los artículos restantes
            var nuevaLista = articulos.Select(a => new ReporteStockPicking_E
            {
                ItemCode = a.ItemCode,
                ItemName = a.ItemName,
                StockActualPiezas = Convert.ToInt32(a.StockLibrePiezas),
                StockActualUnidades = a.StockLibreUnidades,
                StockPicking = a.StockPicking,
                Almacen = a.WhsCode,
                Clasificacion = a.Clasificacion,
                StockMinAbastecimiento = a.StockMinAbastecimiento
            }).ToList();

            return nuevaLista
                .OrderBy(n => n.StockActualPiezas <= 0)                          // Sin stock SAP (GRIS)
                .ThenBy(n => n.StockPicking < 0)                                 // Stock picking NEGATIVO (GRIS - no debería pasar frecuentemente) 
                .ThenBy(n => n.StockMinAbastecimiento <= 0)                      // Sin parámetros de stock mín. abastecimiento (GRIS)
                .ThenBy(n => n.StockActualUnidades == n.StockPicking)            // No necesita abastecimiento (NEGRO)
                .ThenBy(n => n.StockActualUnidades < n.StockMinAbastecimiento && n.StockPicking > n.StockMinAbastecimiento * 0.5m)   // Cuando el stock SAP no cubre el stock mín. (NEGRO)
                .ThenBy(n => n.StockPicking > 0 && n.StockPicking >= n.StockMinAbastecimiento)          // Stock picking cumpliendo el 100% o más del stock mín. abast. (VERDE)
                .ThenBy(n => n.StockPicking > 0 && n.StockPicking >= n.StockMinAbastecimiento * 0.5M)   // Stock picking por abastecer (>=50% AMARILLO)
                .ThenBy(n => n.StockPicking > 0 && n.StockPicking < n.StockMinAbastecimiento * 0.5M)    // Stock picking crítico por abastecer (<50% ROJO)
                .ThenBy(n => n.StockMinAbastecimiento > 0
                    ? (decimal)n.StockPicking * 100 / n.StockMinAbastecimiento
                    : decimal.MaxValue)
                .ToList();
        }

        public List<OITW_E> ListarArticulosConStockPickingInsuficiente()
        {
            var resultado = new List<OITW_E>();
            var controlStockInternoPicking = _datosReporte.ControlHistoricoDeIngresosAPicking();

            // Obtenemos todos los SKUs que ya se encuentran registrado en un requerimiento
            var filtrosDetalleReq = new DetalleRequerimientos_E { AtendidoReserva = 0, AtendidoPicking = 0 };
            var articulosEnRequerimientos = new DetalleRequerimientos_N().ObtenerDetalleRequerimiento(filtrosDetalleReq)
                .Item2.Select(r => r.ItemCode)
                .Distinct()
                .ToHashSet();

            // Obtener los artículos disponibles que no están en requerimientos
            List<OITW_E> articulos = _oitwN
                .ListarDetArticulosInv(new OITW_E { WhsCode = "16" })
                .Where(x => x.OnHand > 0 && !articulosEnRequerimientos.Contains(x.ItemCode))
                .ToList();

            if (articulos != null && articulos.Any())
            {
                // 1) Preparar índices por ItemCode para evitar N+1
                var itemCodes = articulos.Select(a => a.ItemCode).Distinct().ToList();

                // 1.a) Requerimientos por ItemCode
                var detallesReq = _requerimientosN.ListarDetalles(itemCodes, "CantidadSolicitada");
                var reqPorItem = detallesReq
                    .GroupBy(r => r.ItemCode)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.QuantityUnidadesCajas));

                // 1.b) Ubicaciones de lotes por ItemCode (solo RESERVA)
                var ubicaciones = _ubicacionesLotesN.ObtenerDatosPorItemCode(itemCodes);
                var ubiPorItem = ubicaciones
                    .Where(x => x.Almacen == "RESERVA")
                    .GroupBy(x => x.ItemCode)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.QuantityUnidadesCajas));

                // 1.c) Parámetros de control (StockMinAbastecimiento) por ItemCode
                var controlPorItem = controlStockInternoPicking
                    .GroupBy(x => x.ItemCode)
                    .ToDictionary(g => g.Key, g => g.First());

                foreach (var item in articulos)
                {
                    // Cantidad solicitada acumulada
                    int quantityReq = reqPorItem.TryGetValue(item.ItemCode, out var qReq) ? (qReq ?? 0) : 0;

                    // Cantidad en ubicaciones de RESERVA
                    int quantityUbi;
                    ubiPorItem.TryGetValue(item.ItemCode, out quantityUbi);

                    // Cálculos de stock
                    int stockDeAlmReserva = quantityUbi - quantityReq; // RESERVA neto
                    decimal stockEnPicking = item.StockLibreUnidades - stockDeAlmReserva;

                    // Stock mínimo desde control interno (evita consulta por ítem)
                    int stockMin = 0;
                    if (controlPorItem.TryGetValue(item.ItemCode, out var ctrl) && item.StockLibreUnidades > 0)
                        stockMin = ctrl.StockMinAbastecimiento;

                    // Asignaciones usadas por la condición de salida
                    item.StockPicking = stockEnPicking;
                    item.StockMinAbastecimiento = stockMin;

                    // Cantidad a solicitar respecto al mínimo (si es negativa, no se usa para filtrar)
                    item.CantidadSolicitada = Convert.ToInt32(stockMin - stockEnPicking);

                    // Condición: crítico (<50%) y con stock en RESERVA
                    if (stockDeAlmReserva > 0 && item.StockPicking > 0 && item.StockPicking < item.StockMinAbastecimiento * 0.5M)
                    {
                        resultado.Add(new OITW_E
                        {
                            ItemCode = item.ItemCode,
                            CantidadSolicitada = item.CantidadSolicitada
                        });
                    }
                }
            }

            return resultado;
        }

        public (Helper_E, List<ReporteStockPicking_E>) ListarStockPicking()
        {
            return _datosReporte.ListarStockPicking();
        }

    }
}