using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Almacen_NEG.Tablas
{
    public class OMRC_N
    {
        OMRC_D oD = new OMRC_D();
        public List<OMRC_E> listarFabricantes()
        {
            return oD.listarFabricantes();
        }
    }
}
