using Capa_Datos.AbastecimientoInterno_DAO.TablasExternas;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class SolicitudTraslado_N
    {
        SolicitudTraslado_D datosTraslado = new SolicitudTraslado_D();
        public SolicitudTraslado_E ObtenerSolicitudDeTraslado(int DocNum)
        {
            return datosTraslado.ObtenerSolicitudDeTraslado(DocNum);
        }
        public SolicitudTraslado_E ImportarSolicitudDeTraslado(SolicitudTraslado_E obj)
        {
            return datosTraslado.ImportarSolicitudDeTraslado(obj);
        }
    }
}
