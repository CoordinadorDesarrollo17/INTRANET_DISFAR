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
    public class UbicacionesLotesMaster_N
    {
        readonly UbicacionesLotesMaster_D datosUbicacionesLM = new UbicacionesLotesMaster_D();

        public Helper_E Ingreso (TransferenciaReserva_E ingreso, SqlConnection cn)
        {
            return datosUbicacionesLM.Ingreso(ingreso, cn);
        }
        public Helper_E Egreso(TransferenciaReserva_E egreso, SqlConnection cn)
        {
            return datosUbicacionesLM.Egreso(egreso, cn);
        }
        
    }
}
