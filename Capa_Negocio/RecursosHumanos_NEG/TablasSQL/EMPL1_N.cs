using Capa_Datos.RecursosHumanos_DAO;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.RecursosHumanos_NEG.TablasSQL
{
    public class EMPL1_N
    {
        EMPL1_D empl1D = new EMPL1_D();
        private readonly Helpers helper = new Helpers();

        public List<EMPL1_E> ObtenerDatosLaborales(EMPL1_E filtros)
        {
            return empl1D.ListarDatosLaborales(filtros);
        }
    }
}