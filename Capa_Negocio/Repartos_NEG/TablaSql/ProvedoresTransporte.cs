using Capa_Datos.Repartos_DAO.TablasHana;
using Capa_Datos.Rutas_DAO.TablasSql;
using Capa_Entidad.Repartos_ENT.TablasHana;
using Capa_Entidad.Rutas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Repartos_NEG.TablaSql
{
    public class ProvedoresTransporte
    {
        ORRU_D orru = new ORRU_D();
        public List<ProvedorTrans> listarProvedores()
        {
            return orru.listarProvedores();
        }
    }
}
