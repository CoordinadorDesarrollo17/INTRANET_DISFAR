using Capa_Datos.AbastecimientoInterno_DAO.TablasExternas;
using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.Interfaces;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasExternas;
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

        public OWTQ_E BuscarSolicitudDeTraslado(int DocNum)
        {
            var solicitudTraslado = datosTraslado.BuscarSolicitudDeTraslado(DocNum);

            if (solicitudTraslado != null && solicitudTraslado.Detalle != null)
            {
                foreach (var item in solicitudTraslado.Detalle)
                {
                    item.Value.UnidadAlmSugerido = _ubicacionesLotesMasterN.BuscarUnidadAlm(new UbicacionesLotesMaster_E { ItemCode = item.Value.ItemCode, ItemName = item.Value.ItemName });
                }
            }
            
            return solicitudTraslado;
        }

    }
}
