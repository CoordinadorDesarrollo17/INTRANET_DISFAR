using Capa_Datos.Repartos_DAO.TablasHana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Repartos_NEG.TablasHana
{
    public class SYP_CONDUC_N
    {
        SYP_CONDUC_D conducD = new SYP_CONDUC_D();
        public List<string> listar()
        {
            return conducD.listar();
        }
    }
}
