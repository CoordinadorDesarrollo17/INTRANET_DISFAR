using Capa_Datos.Repartos_DAO.TablasHana;
using Capa_Entidad.Repartos_ENT.TablasHana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Repartos_NEG.TablasHana
{
    public class SYP_VEHICU_N
    {
        SYP_VEHICU_D vehD = new SYP_VEHICU_D();
        public List<SYP_VEHICU_E> listar()
        {
            return vehD.listar();
        }
        public (string placa, string conductor) buscarConductorYPlaca(string zona)
        {
            return vehD.buscarConductorYPlaca(zona);
        }
        
    }
}
