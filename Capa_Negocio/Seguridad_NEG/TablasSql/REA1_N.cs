using Capa_Datos;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Seguridad_NEG.TablasSql
{
    public class REA1_N
    {
        Area_D rea1D = new Area_D();
        public List<AreaFc_E> listarAreasFc()
        {
            return rea1D.listarAreasFc();
        }
    }
}
