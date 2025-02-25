using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Almacen_NEG.Tablas
{
    public class OITW_N
    {
        OITW_D oD = new OITW_D();
<<<<<<< HEAD
        public List<OITW_E> ListarDetArticulosInv(OITW_E obj)
        {
            return oD.ListarDetArticulosInv(obj);
=======
<<<<<<< HEAD
        public List<OITW_E> listarDetArticulosInv(OITW_E obj)
        {
            return oD.listarDetArticulosInv(obj);
=======
        public List<OITW_E> ListarDetArticulosInv(OITW_E obj)
        {
            return oD.ListarDetArticulosInv(obj);
>>>>>>> 3076d5e (Cambios en OITW)
>>>>>>> e222ee29b1e5646d6a76aff220160fb8419df91d
        }
    }
}
