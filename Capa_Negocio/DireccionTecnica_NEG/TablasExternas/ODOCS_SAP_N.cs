using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.TablasSql;
using Capa_Entidad;
using Capa_Datos.DireccionTecnica_DAO.TablasExternas;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System.Data.SqlClient;

namespace Capa_Negocio.DireccionTecnica_NEG.TablasExternas
{
    public class ODOCS_SAP_N
    {
        private readonly ODOCS_SAP_D _datos = new ODOCS_SAP_D();

        public (Helper_E, ODOCS_E) BuscarDocEntradaMercaderia(long docNum, string guia)
        {
            return _datos.BuscarDocEntradaMercaderia(Convert.ToDecimal(docNum), guia);
        }

        public (Helper_E, ODOCS_E) BuscarDocSolicitudTraslado(long docNum, string guia)
        {
            return _datos.BuscarDocSolicitudTraslado(Convert.ToDecimal(docNum), guia);
        }
    }
}
