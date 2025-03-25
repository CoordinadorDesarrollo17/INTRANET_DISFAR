using Capa_Datos.RecursosHumanos_DAO.TablasExternas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Capa_Negocio.RecursosHumanos_NEG.TablasExternas
{
    public class ODPTO_N
    {
        ODPTO_D _datos = new ODPTO_D();
        public void MigrarDepartamentosHANA()
        {
            _datos.MigrarDepartamentosHANA();
        }
    }
}
