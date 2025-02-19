using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class Masters_N
    {
        Masters_D _datos = new Masters_D();

        public List<Masters_E> ListarMasters(Masters_E filtros = null, StringBuilder condicion = null, Dictionary<string, object> parametros = null)
        {
            condicion = new StringBuilder();
            parametros = new Dictionary<string, object>();

            if (filtros != null)
            {
                if (!string.IsNullOrWhiteSpace(filtros.UmAlm))
                {
                    condicion.AppendLine("AND UmAlm = @UmAlm");
                    parametros["@UmAlm"] = filtros.UmAlm;
                }
            }

            return _datos.ListarMasters(condicion.ToString(), parametros);
        }
    }
}
