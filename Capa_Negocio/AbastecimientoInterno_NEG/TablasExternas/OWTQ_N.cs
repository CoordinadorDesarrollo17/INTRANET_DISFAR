using Capa_Datos.AbastecimientoInterno_DAO.TablasExternas;
using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasExternas;
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

        public OWTQ_E BuscarSolicitudDeTraslado(int DocNum)
        {
            return datosTraslado.BuscarSolicitudDeTraslado(DocNum);
        }
    }
}
