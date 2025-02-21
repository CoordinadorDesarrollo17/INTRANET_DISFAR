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
        public SolicitudesTraslado_E ObtenerSolicitudDeTraslado(int docNum)
        {
            return datosTraslado.ObtenerSolicitudDeTraslado(docNum);
        }
        public SolicitudesTraslado_E ImportarSolicitudDeTraslado(SolicitudesTraslado_E obj)
        {
            return datosTraslado.ImportarSolicitudDeTraslado(obj);
        }
        public Helper_E DeleteSolicitudDeTraslado (int docNum, SqlConnection cn) {
            return datosTraslado.DeleteSolicitudDeTraslado(docNum,  cn);
        }
    }
}
