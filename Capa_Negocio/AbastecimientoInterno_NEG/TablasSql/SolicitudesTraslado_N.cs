using Capa_Datos.AbastecimientoInterno_DAO.TablasExternas;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class SolicitudesTraslado_N
    {
        SolicitudesTraslado_D datosTraslado = new SolicitudesTraslado_D();
        private readonly UbicacionesLotesMaster_N _ubicacionesLotesMasterN = new UbicacionesLotesMaster_N();

        public SolicitudesTraslado_E ObtenerSolicitudDeTraslado(int DocNum)
        {
            var solicitudTraslado = datosTraslado.ObtenerSolicitudDeTraslado(DocNum);

            if (solicitudTraslado != null)
            {
                foreach (var item in solicitudTraslado.Detalle)
                {
                    item.UnidadAlmSugerido = _ubicacionesLotesMasterN.BuscarUnidadAlm(new UbicacionesLotesMaster_E { ItemCode = item.ItemCode, ItemName = item.ItemName });
                }
            }
            
            return solicitudTraslado;
        }
        public SolicitudesTraslado_E ImportarSolicitudDeTraslado(SolicitudesTraslado_E obj)
        {
            var solicitud = datosTraslado.ImportarSolicitudDeTraslado(obj);

            return solicitud;
        }
        public Helper_E DeleteSolicitudDeTraslado (int docNum, SqlConnection cn) {
            return datosTraslado.DeleteSolicitudDeTraslado(docNum,  cn);
        }
    }
}
