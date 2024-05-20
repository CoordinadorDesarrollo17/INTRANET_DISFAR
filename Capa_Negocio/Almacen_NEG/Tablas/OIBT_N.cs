using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Almacen_NEG.Tablas
{
    public class OIBT_N
    {
        OIBT_D oibtD = new OIBT_D();
        public List<OIBT_E> listarArticulosLotes(OIBT_E filtro = null, bool joinOITM = false, string limite = "500")
        { return oibtD.ListarArticulosLotes(filtro, joinOITM, limite); }
    }
}
