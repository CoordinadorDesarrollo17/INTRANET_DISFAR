using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.Seguridad_DAO;
using Capa_Entidad.Seguridad_ENT;

namespace Capa_Negocio.Seguridad_NEG
{
    public class Rol1_N
    {
        Rol1_D rol1 = new Rol1_D();
        public int verificarAccesoOperacion(int idRol, int idOperacion, string nombreOperacion, int modulo)
        {
            return rol1.verificarAccesoOperacion(idRol, idOperacion, nombreOperacion, modulo);
        }
        public void crudOperacion(int idRol, int[] numeros)
        {
            rol1.crudOperacion(idRol, numeros);
        }
    }
}
