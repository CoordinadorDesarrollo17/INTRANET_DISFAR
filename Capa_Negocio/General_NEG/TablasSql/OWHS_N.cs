using Capa_Datos.General_DAO.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class OWHS_N
    {
        OWHS_D owhsD = new OWHS_D();
        public List<OWHS_E> listarAlmacenes(string[] arrWhsCode = null)
        {
            return owhsD.listarAlmacenes(arrWhsCode);
        }
    }
}
