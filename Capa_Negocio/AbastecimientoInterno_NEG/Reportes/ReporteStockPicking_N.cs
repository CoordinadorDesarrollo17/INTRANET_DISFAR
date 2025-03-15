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
                .Where(x => x.OnHand > 0)
                .ToList();

            if (articulos != null && articulos.Any())
            {
                // Iterar sobre una copia de la lista para evitar el error de modificación
                foreach (var item in articulos.ToList())
                {
                    List<DetalleRequerimientos_E> resultDetReq = _requerimientosN.ListarDetalles(item.ItemCode, "CantidadSolicitada");
                    int quantityReq = resultDetReq?.Sum(r => r.QuantityUnidadesCajas) ?? 0;

                    List<UbicacionesLotes_E> resultUbicacionesLotes = _ubicacionesLotesN.Obtener(item.ItemCode).Where(x => x.Almacen.Equals("RESERVA")).ToList(); 
                    int quantityUbicacionesLote = resultUbicacionesLotes?.Sum(r => r.QuantityUnidadesCajas) ?? 0;

                    int stockDeAlmReserva = quantityUbicacionesLote - quantityReq;
                    int stockEnPicking = Convert.ToInt32(item.StockLibre) - stockDeAlmReserva;

                    var controlPorItemCode = controlStockInternoPicking.FirstOrDefault(i => i.ItemCode == item.ItemCode);

                    item.StockLibre = stockEnPicking;
                    item.Clasificacion = (controlPorItemCode != null) ? controlPorItemCode.Clasificacion : string.Empty;
                    item.StockMinAbastecimiento = (controlPorItemCode != null) ? controlPorItemCode.StockMinAbastecimiento : 0;
                }
            }

            // Crear la nueva lista solo con los artículos restantes
            var nuevaLista = articulos.Select(a => new ReporteStockPicking_E
            {
                ItemCode = a.ItemCode,
                ItemName = a.ItemName,
                StockActual = Convert.ToInt32(a.StockLibre),
                Almacen = a.WhsCode,
                Clasificacion = a.Clasificacion,
                StockMinAbastecimiento = a.StockMinAbastecimiento
            }).ToList();

            return nuevaLista.OrderBy(x=> x.ItemCode).ToList();
        }

    }
}