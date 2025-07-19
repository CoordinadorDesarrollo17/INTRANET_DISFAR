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
                .OrderBy(n => n.StockActualPiezas <= 0)                                                     // Sin stock SAP (NEGRO)
                .ThenBy(n => n.StockPicking < 0)                                                                   // Stock picking NEGATIVO (no debería pasar frecuentemente) 
                .ThenBy(n => n.StockActualUnidades == n.StockPicking || n.StockActualUnidades < n.StockMinAbastecimiento)     // Stock SAP al máximo y que no puede cubrir el stock mín. abast., se considera 100% de todo lo que existe en SAP (NEGRO) - NO NECESITA ABASTECIMIENTO
                .ThenBy(n => n.StockMinAbastecimiento <= 0)                                          // Sin parámetros de stock mín. abast. (GRIS)
                .ThenBy(n => n.StockPicking > 0 && n.StockPicking >= n.StockMinAbastecimiento)                  // Stock picking cumpliendo el 100% o más del stock mín. abast. (VERDE)
                .ThenBy(n => n.StockPicking > 0 && n.StockPicking >= n.StockMinAbastecimiento * 0.5M)    // Stock picking por abastecer (>=50% AMARILLO)
                .ThenBy(n => n.StockPicking > 0 && n.StockPicking < n.StockMinAbastecimiento * 0.5M)      // Stock picking crítico por abastecer (<50% ROJO)
                .ThenBy(n => n.StockMinAbastecimiento > 0 ? ((decimal)n.StockPicking * 100 / n.StockMinAbastecimiento) : decimal.MaxValue)       // Finalmente, ordena por % de abastecimiento ASC, pero evita división por cero
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
            List<OITW_E> articulos = new Capa_Negocio.Almacen_NEG.Tablas.OITW_N()
                .ListarDetArticulosInv(new OITW_E { WhsCode = "16" })
                .Where(x => x.OnHand > 0 && !articulosEnRequerimientos.Contains(x.ItemCode))
                .ToList();

            int contador = 1;
            if (articulos != null && articulos.Any())
            {
                // Iterar sobre una copia de la lista para evitar el error de modificación
                foreach (var item in articulos.ToList())
                {
                    // Tener en cuenta solo 20 SKUs
                    //if (contador == 20)
                    //    break;

                    // 1. Obtener la cantidad solicitada para este ItemCode
                    List<DetalleRequerimientos_E> resultDetReq = _requerimientosN.ListarDetalles_OLD(item.ItemCode, "CantidadSolicitada");
                    int quantityReq = resultDetReq?.Sum(r => r.QuantityUnidadesCajas) ?? 0;

                    // 2. Obtener los lotes disponibles para ese ItemCode
                    List<UbicacionesLotes_E> resultUbicacionesLotes = _ubicacionesLotesN.Obtener(item.ItemCode).Where(x => x.Almacen.Equals("RESERVA")).ToList();
                    int quantityUbicacionesLote = resultUbicacionesLotes?.Sum(r => r.QuantityUnidadesCajas) ?? 0;

                    int stockDeAlmReserva = quantityUbicacionesLote - quantityReq;
                    decimal stockEnPicking = item.StockLibreUnidades - stockDeAlmReserva;
                    decimal stockMinimoParaLaVenta = new StockMinProductos_N().Obtener(item.ItemCode).StockMinAbastecimiento;
                    item.CantidadSolicitada = Convert.ToInt32(stockMinimoParaLaVenta - stockEnPicking);

                    var controlPorItemCode = controlStockInternoPicking.FirstOrDefault(i => i.ItemCode == item.ItemCode);

                    item.StockPicking = stockEnPicking;
                    item.StockMinAbastecimiento = (controlPorItemCode != null && item.StockLibreUnidades > 0) ? controlPorItemCode.StockMinAbastecimiento : 0;        // Debe existir stock en RESERVA 

                    // Si está en condición crítica (<50%) y tiene stock en RESERVA, lo agregamos al resultado
                    if (stockDeAlmReserva > 0 && item.StockPicking > 0 && item.StockPicking < item.StockMinAbastecimiento * 0.5M)
                    {
                        resultado.Add(new OITW_E
                        {
                            ItemCode = item.ItemCode,
                            CantidadSolicitada = item.CantidadSolicitada
                        });
                        ++contador;
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