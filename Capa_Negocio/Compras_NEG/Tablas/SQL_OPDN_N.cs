using Capa_Datos.Compras_DAO.Tablas;
using Capa_Entidad.Compras_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Compras_NEG.Tablas
{
    public class SQL_OPDN_N
    {
        SQL_OPDN_D sD = new SQL_OPDN_D();
        public int realizarSqlEntradaDeMercancias(SQL_OPDN_E s)
        {
            return sD.realizarSqlEntradaDeMercancias(s);
        }
        public int eliminarSqlEntradaDeMercancias(int DocEntry)
        {
            return sD.eliminarSqlEntradaDeMercancias(DocEntry);
        }
    }
}
