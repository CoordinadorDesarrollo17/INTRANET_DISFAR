using Capa_Datos.General_DAO.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class DNLAB_N
    {
        DNLAB_D dnlabD = new DNLAB_D();
        public int registrarDiaNoLaborable(DNLAB_E obj)
        {
            return dnlabD.registrarDiaNoLaborable(obj);
        }
        public List<DNLAB_E> listadoDeDiasNLAB()
        {
            return dnlabD.listadoDeDiasNLAB();
        }
    }
}
