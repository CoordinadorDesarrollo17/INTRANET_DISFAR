using Capa_Datos.Seguridad_DAO;
using Capa_Entidad.Seguridad_ENT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Seguridad_NEG
{
    public class OOPE_N
    {
        OOPE_D opD = new OOPE_D();

        public List<OOPE_E> listarOperacionesRolModulo(int idModulo, int idRol)
        {
            return opD.listarOperacionesRolModulo(idModulo, idRol);
        }
    }
}

