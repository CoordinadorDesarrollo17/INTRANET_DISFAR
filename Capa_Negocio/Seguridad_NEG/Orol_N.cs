using Capa_Datos.Seguridad_DAO;
using Capa_Entidad.Seguridad_ENT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Seguridad_NEG
{
    public class Orol_N
    {
        Orol_D orolD = new Orol_D();

        public List<Orol_E> listarRoles(int idRol)
        {
            return orolD.listarRoles(idRol);
        }

        public string ObtenerRol(int idRol)
        {
            return orolD.ObtenerRol(idRol);
        }
    }
}