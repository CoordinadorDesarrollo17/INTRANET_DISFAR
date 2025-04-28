using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.General_DAO.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class MenuSistema_N
    {
        private readonly MenuSistema_D _datos = new MenuSistema_D();
        public (Capa_Entidad.Helper_E, List<MenuSistema_E>) ListarMenuSistema(MenuSistema_E filtros = null, Dictionary<string, object> parametros = null)
        {
            StringBuilder condicion = new StringBuilder();

            if (parametros == null)
                parametros = new Dictionary<string, object>();

            if (filtros != null)
            {
                if (filtros.Id > 0)
                {
                    condicion.AppendLine("AND Id = @Id");
                    parametros["@Id"] = filtros.Id;
                }

                if (!string.IsNullOrWhiteSpace(filtros.Nombre))
                {
                    condicion.AppendLine("AND Nombre = @Nombre");
                    parametros["@Nombre"] = filtros.Nombre;
                }

                if (filtros.Nivel > 0)
                {
                    condicion.AppendLine("AND Nivel = @Nivel");
                    parametros["@Nivel"] = filtros.Nivel;
                }

                if (filtros.SuperiorId != null && filtros.SuperiorId > 0)
                {
                    condicion.AppendLine("AND SuperiorId = @SuperiorId");
                    parametros["@SuperiorId"] = filtros.SuperiorId;
                }

                if (filtros.SuperiorId > 0)
                {
                    condicion.AppendLine("AND Orden = @Orden");
                    parametros["@Orden"] = filtros.Orden;
                }
            }

            return _datos.ListarMenuSistema(condicion.ToString(), parametros);
        }
    }
}
