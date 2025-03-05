using Capa_Entidad.AbastecimientoInterno_ENT.Reportes;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.AbastecimientoInterno_DAO.Reportes;

namespace Capa_Negocio.AbastecimientoInterno_NEG.Reportes
{
    public class ReporteStockPicking_N
    {
         ReporteStockPicking_D _datosReporte = new ReporteStockPicking_D();
        public List<ReporteStockPicking_E> ControlStockInternoPicking(/*ReporteStockPicking_E filtro = null*/)
        {
            //if (filtro != null)
            //{
            //    lista = lista.Where(x =>
            //        (string.IsNullOrEmpty(filtro.ItemCode) || x.ItemCode.Contains(filtro.ItemCode)) &&
            //        (string.IsNullOrEmpty(filtro.ItemName) || x.ItemName.Contains(filtro.ItemName)) &&
            //        (string.IsNullOrEmpty(filtro.Clasificacion) || x.Clasificacion.Contains(filtro.Clasificacion))
            //    ).ToList();
            //}
            return _datosReporte.ControlStockInternoPicking();
        }

    }
}
