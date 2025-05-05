using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class Productos_N
    {
        Productos_D _datos = new Productos_D();

        public List<Productos_E> ListarProductos(Productos_E filtros = null, StringBuilder condicion = null, Dictionary<string, object> parametros = null)
        {
            condicion = new StringBuilder();
            parametros = new Dictionary<string, object>();

            if (filtros != null)
            {
                if (!string.IsNullOrWhiteSpace(filtros.ItemCode))
                {
                    condicion.AppendLine("AND P.ItemCode = @ItemCode");
                    parametros["@ItemCode"] = filtros.ItemCode;
                }

                if (!string.IsNullOrWhiteSpace(filtros.ItemName))
                {
                    condicion.AppendLine("AND P.ItemName = @ItemName");
                    parametros["@ItemName"] = filtros.ItemName;
                }

                if (filtros.FirmCode > 0)
                {
                    condicion.AppendLine("AND P.FirmCode = @FirmCode");
                    parametros["@FirmCode"] = filtros.FirmCode;
                }
            }

            return _datos.ListarProductos(condicion.ToString(), parametros);
        }


    }
}
