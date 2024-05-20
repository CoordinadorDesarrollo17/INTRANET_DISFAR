using Capa_Datos.General_DAO.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class UBIG_N
    {
        UBIG_D ubigD = new UBIG_D();
        public List<UBIG_E> Listar()
        {
            return ubigD.Listar();
        }
        }
}
