using Capa_Datos.AbastecimientoInterno_DAO.TablasExternas;
using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Negocio.AbastecimientoInterno_NEG.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasExternas
{
    //Solicitudes de traslado
    public class OWTQ_N
    {
        OWTQ_D datosTraslado = new OWTQ_D();
        private readonly UbicacionesLotesMaster_N _ubicacionesLotesMasterN = new UbicacionesLotesMaster_N();

        public SolicitudesTraslado_E BuscarSolicitudDeTraslado(int DocNum)
        {
            var solicitudTraslado = datosTraslado.BuscarSolicitudDeTraslado(DocNum);

            return solicitudTraslado;
        }

    }
}
