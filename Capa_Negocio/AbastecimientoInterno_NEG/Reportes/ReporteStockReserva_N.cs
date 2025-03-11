using Capa_Datos.AbastecimientoInterno_DAO.Reportes;
using Capa_Entidad.AbastecimientoInterno_ENT.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.AbastecimientoInterno_NEG.Reportes
{
    public class ReporteStockReserva_N
    {
        ReporteStockReserva_D _datos = new ReporteStockReserva_D();
        public List<ReporteStockReserva_E> Listar()
        {
            return _datos.Listar();
        }
    }
}
