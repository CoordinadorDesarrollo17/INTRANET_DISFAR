using System.Collections.Generic;
using Capa_Datos.Caja_DAO;
using Capa_Entidad.Caja_ENT;

namespace Capa_Negocio.Caja_NEG
{
    public class OPP_N
    {
        OPP_D ppD = new OPP_D();

        public List<OPP_E> ObtenerDatosPagosParciales(int idOTC)
        {
            return ppD.ObtenerDatosPagosParciales(idOTC);
        }

        public int EliminarPagoParcial(OPP_E datos)
        {
            var result = ppD.EliminarPagoParcial(datos);

            return result;
        }

        public decimal ObtenerTotalPagos(int idOTC)
        {
            return ppD.ObtenerTotalPagos(idOTC);
        }
    }
}
