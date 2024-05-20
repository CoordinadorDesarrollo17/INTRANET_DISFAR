using Capa_Datos.Almacen_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System.Collections.Generic;

namespace Capa_Negocio.Almacen_NEG.TablasSql
{
    public class CC_ORPD_N
    {
        readonly CC_ORPD_D datos = new CC_ORPD_D();

        public List<CC_ORPD_E> ListarCC_ORPD(CC_ORPD_E filtros)
        {
            return datos.ListarCC_ORPD(filtros);
        }

        public string UltimoEstadoCC_ORPD(int DocEntry)
        {
            return datos.UltimoEstadoCC_ORPD(DocEntry);
        }
    }
}