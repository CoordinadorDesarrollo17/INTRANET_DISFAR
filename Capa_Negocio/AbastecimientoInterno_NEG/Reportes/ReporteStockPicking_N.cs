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
                // Iterar sobre una copia de la lista para evitar el error de modificación
                foreach (var item in articulos.ToList())
                {
                    // CAN
                    List<DetalleRequerimientos_E> resultDetReq = _requerimientosN.ListarDetalles(item.ItemCode, "CantidadSolicitada");
                    int quantityReq = resultDetReq?.Sum(r => r.QuantityUnidadesCajas) ?? 0;

                    List<UbicacionesLotes_E> resultUbicacionesLotes = _ubicacionesLotesN.Obtener(item.ItemCode).Where(x => x.Almacen.Equals("RESERVA")).ToList();
                    int quantityUbicacionesLote = resultUbicacionesLotes?.Sum(r => r.QuantityUnidadesCajas) ?? 0;

                    int stockDeAlmReserva = quantityUbicacionesLote - quantityReq;
                    decimal stockEnPicking = item.StockLibreUnidades - stockDeAlmReserva;

                    var controlPorItemCode = controlStockInternoPicking.FirstOrDefault(i => i.ItemCode == item.ItemCode);

                    item.StockPicking = stockEnPicking;
                    item.Clasificacion = (controlPorItemCode != null) ? controlPorItemCode.Clasificacion : string.Empty;
                    item.StockMinAbastecimiento = (controlPorItemCode != null && item.StockLibreUnidades > 0) ? controlPorItemCode.StockMinAbastecimiento : 0;        // Debe existir stock en RESERVA 
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

        public (Helper_E, List<ReporteStockPicking_E>) ListarStockPicking()
        {
            return _datosReporte.ListarStockPicking();
        }

    }
}