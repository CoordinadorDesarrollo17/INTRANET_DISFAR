using Capa_Datos.RecursosHumanos_DAO.TablasExternas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.RecursosHumanos_NEG.TablasExternas
{
    public class OAREA_N
    {
        OAREA_D _datos = new OAREA_D();
        public void MigrarAreasHANA() { _datos.MigrarAreasHANA(); }
    }
}
