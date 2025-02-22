using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class UbicacionesLotes_N
    {
        readonly UbicacionesLotes_D datosUbicacionesL = new UbicacionesLotes_D();
        public Helper_E Ingreso(TransferenciaReserva_E ingreso, SqlConnection cn)
        {
            return datosUbicacionesL.Ingreso(ingreso, cn);
        }
        public Helper_E Egreso(TransferenciaReserva_E egreso, SqlConnection cn)
        {
            return datosUbicacionesL.Egreso(egreso, cn);
        }
    }
}
