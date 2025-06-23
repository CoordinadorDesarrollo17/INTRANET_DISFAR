using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Almacen_NEG.Tablas
{
    public class OITW_N
    {
        OITW_D oD = new OITW_D();

        public List<OITW_E> ListarDetArticulosInv(OITW_E obj)
        {
            return oD.ListarDetArticulosInv(obj);
        }

        public int ObtenerStockSKUPorAlmacen(string itemCode, List<string> almacenes, Dictionary<string, object> parametros = null)
        {
            StringBuilder condicion = new StringBuilder();

            if (parametros == null)
                parametros = new Dictionary<string, object>();

            // Es obligatorio enviar el ItemCode
            if (string.IsNullOrEmpty(itemCode))
                return 0;

            condicion.AppendLine($@"WHERE ""ItemCode"" = '{itemCode}'");

            // Condición opcional, si no envía ningún almacén se mostrará el stock de todos los almacenes
            if (almacenes != null && almacenes.Any())
            {
                var almacenesFormateados = string.Join(", ", almacenes.Select(a => $"'{a}'"));
                condicion.AppendLine($@"AND ""WhsCode"" IN ({almacenesFormateados})");
            }

            return oD.ObtenerStockSKUPorAlmacen(condicion.ToString(), parametros);
        }
    }
}
