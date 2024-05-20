using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Almacen_NEG.Tablas
{
    public class OITB_N
    {
        OITB_D oD = new OITB_D();
        public List<OITB_E> listarGrupoArticulos()
        {
            return oD.listarGrupoArticulos();
        }
    }
}
