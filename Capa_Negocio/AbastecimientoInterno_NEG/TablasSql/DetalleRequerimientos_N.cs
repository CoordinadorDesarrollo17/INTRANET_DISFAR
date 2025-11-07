using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Datos;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class DetalleRequerimientos_N
    {
        DetalleRequerimiento_D _datos = new DetalleRequerimiento_D();

        public (Helper_E, List<DetalleRequerimientos_E>) ObtenerDetalleRequerimiento(
    DetalleRequerimientos_E filtros = null,
    Dictionary<string, object> parametros = null,
    bool esParaListado = false)
        {
            StringBuilder condicion = new StringBuilder();
            Helper_E _helper = new Helper_E();

            if (parametros == null)
                parametros = new Dictionary<string, object>();

            if (filtros == null)
            {
                _helper.Titulo = "Error";
                _helper.Mensajes.Add("Ocurrió un error al obtener datos.");
                _helper.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                _helper.Icono = "error";
                return (_helper, null);
            }

            if (esParaListado == false)
            {
                condicion.AppendLine("WHERE DET.AtendidoReserva = @AtendidoReserva");
                parametros["@AtendidoReserva"] = filtros.AtendidoReserva;

                condicion.AppendLine("AND DET.AtendidoPicking = @AtendidoPicking");
                parametros["@AtendidoPicking"] = filtros.AtendidoPicking;
            }

            if (filtros.RequerimientoId > 0)
            {
                var prefijoCondicion = string.IsNullOrEmpty(condicion.ToString()) ? "WHERE" : "AND";
                condicion.AppendLine($"{prefijoCondicion} DET.RequerimientoId = @RequerimientoId");
                parametros["@RequerimientoId"] = filtros.RequerimientoId;
            }

            var (helper, lista) = _datos.ObtenerDetalleRequerimiento(condicion.ToString(), parametros);

            if (lista != null && lista.Any())
            {
                // Obtener todos los ItemCodes únicos
                var itemCodes = lista.Select(d => d.ItemCode).Distinct().ToList();

                // Stock libre en Picking (almacén 16)
                var articulosPicking = new Capa_Negocio.Almacen_NEG.Tablas.OITW_N()
                    .ListarDetArticulosInv(new Capa_Entidad.Almacen_ENT.Tablas.OITW_E { WhsCode = "16" })
                    .Where(x => itemCodes.Contains(x.ItemCode))
                    .ToDictionary(x => x.ItemCode, x => x);

                // Cantidad comprometida en requerimientos
                var detallesReq = new Capa_Negocio.AbastecimientoInterno_NEG.TablasSql.Requerimientos_N()
                    .ListarDetalles(itemCodes, "CantidadSolicitada")
                    .GroupBy(r => r.ItemCode)
                    .ToDictionary(g => g.Key, g => (int?)g.Sum(x => x.QuantityUnidadesCajas));

                // Cantidad disponible en Reserva
                var ubicacionesLotes = new Capa_Negocio.AbastecimientoInterno_NEG.TablasSql.UbicacionesLotes_N()
                    .ObtenerDatosPorItemCode(itemCodes)
                    .Where(x => x.Almacen == "RESERVA")
                    .GroupBy(x => x.ItemCode)
                    .ToDictionary(g => g.Key, g => (int?)g.Sum(x => x.QuantityUnidadesCajas));

                // Stock mínimo de abastecimiento
                var controlStockInternoPicking = new Capa_Datos.AbastecimientoInterno_DAO.Reportes.ReporteStockPicking_D().ControlHistoricoDeIngresosAPicking();
                var controlPorItemCode = controlStockInternoPicking
                    .GroupBy(x => x.ItemCode)
                    .ToDictionary(g => g.Key, g => g.First());

                foreach (var detalle in lista)
                {
                    // StockMinAbastecimiento
                    if (controlPorItemCode.TryGetValue(detalle.ItemCode, out var control))
                        detalle.StockMinAbastecimiento = control.StockMinAbastecimiento;
                    else
                        detalle.StockMinAbastecimiento = 0;

                    // StockActualUnidades y StockPicking
                    decimal stockLibreUnidades = 0;
                    if (articulosPicking.TryGetValue(detalle.ItemCode, out var articulo))
                    {
                        stockLibreUnidades = articulo.StockLibreUnidades;
                        detalle.StockActualUnidades = articulo.StockLibreUnidades;
                    }
                    else
                    {
                        detalle.StockActualUnidades = 0;
                    }

                    int? quantityReq = 0;
                    detallesReq.TryGetValue(detalle.ItemCode, out quantityReq);

                    int? quantityUbicacionesLote = 0;
                    ubicacionesLotes.TryGetValue(detalle.ItemCode, out quantityUbicacionesLote);

                    int stockDeAlmReserva = (quantityUbicacionesLote ?? 0) - (quantityReq ?? 0);
                    decimal stockEnPicking = stockLibreUnidades - stockDeAlmReserva;

                    detalle.StockPicking = stockEnPicking;
                }
            }

            return (helper, lista);
        }

        public Helper_E EliminarItem(int id, string operarioRegistra)
        {
            return _datos.EliminarItem(id, operarioRegistra);
        }
    }
}