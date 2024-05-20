using Capa_Datos.General_DAO.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;
using System.Collections.Generic;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class Firmas_N
    {
        Firmas_D fir = new Firmas_D();

        public List<Firmas_E> ListarFirmas(Firmas_E filtros)
        {
            return fir.ListarFirmas(filtros);
        }
    }
}