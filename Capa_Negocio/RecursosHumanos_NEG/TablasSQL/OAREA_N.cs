using Capa_Datos.RecursosHumanos_DAO.TablasSQL;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Capa_Negocio.RecursosHumanos_NEG.TablasSQL
{
    public class OAREA_N
    {
        OAREA_D areaD = new OAREA_D();
        private readonly Helpers helper = new Helpers();
        public List<OAREA_E> ListarAreas(OAREA_E filtros)
        {
            return areaD.ListarAreas(filtros);
        }
    }
}
