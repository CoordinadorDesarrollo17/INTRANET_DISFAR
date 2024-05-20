using Capa_Datos;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Seguridad_NEG.TablasSql
{
    public class AREA_N
    {
        Area_D areaD = new Area_D();
        public List<Area_E> listarAreas()
        {
            return areaD.listarAreas();
        }
    }
}
