using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.AbastecimientoInterno_DAO.Reportes;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.Reportes;

namespace Capa_Negocio.AbastecimientoInterno_NEG.Reportes
{
    public class ReporteKardex_N
    {
        ReporteKardex_D _datos = new ReporteKardex_D();

        public List<ReporteKardexIngreso_E> ListarKardexIngreso(string fechaInicio, string fechaFin)
        {
            return _datos.ListarKardexIngreso(fechaInicio, fechaFin);
        }

        public List<ReporteKardexSalida_E> ListarKardexSalida(string fechaInicio, string fechaFin)
        {
            return _datos.ListarKardexSalida(fechaInicio, fechaFin);
        }
    }
}