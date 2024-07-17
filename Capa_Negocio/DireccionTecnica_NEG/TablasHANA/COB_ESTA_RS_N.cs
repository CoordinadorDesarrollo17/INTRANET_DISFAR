using Capa_Datos.DireccionTecnica_DAO.TablasHANA;
using Capa_Entidad.DireccionTecnica_ENT.TablasHANA;
using System.Collections.Generic;

namespace Capa_Negocio.DireccionTecnica_NEG.TablasHANA
{
    public class COB_ESTA_RS_N
    {
        COB_ESTA_RS_D estadoRS_D = new COB_ESTA_RS_D();

        public List<COB_ESTA_RS_E> ListarEstadoRegistrosSanitarios()
        {
            return estadoRS_D.ListarEstadoRegistrosSanitarios();
        }
    }
}