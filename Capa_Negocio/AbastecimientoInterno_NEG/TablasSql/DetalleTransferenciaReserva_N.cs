using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class DetalleTransferenciaReserva_N
    {
        DetalleTransferenciaReserva_D _datos = new DetalleTransferenciaReserva_D();

        public (Helper_E, List<DetalleTransferenciaReserva_E>) ObtenerDetalleTransferenciaReserva(DetalleTransferenciaReserva_E filtros = null, Dictionary<string, object> parametros = null)
        {
            StringBuilder condicion = new StringBuilder();
            Helper_E _helper = new Helper_E();

            if (parametros == null)
                parametros = new Dictionary<string, object>();

            // Obligatorio enviar al menos el RequerimientoId
            if (filtros == null)
            {
                // No se valida porque el valor por defecto es cero
                condicion.AppendLine("WHERE AtendidoReserva = @AtendidoReserva");
                parametros["@AtendidoReserva"] = filtros.AtendidoReserva;

                // No se valida porque el valor por defecto es cero
                condicion.AppendLine("WHERE Validado = @Validado");
                parametros["@Validado"] = filtros.Validado;
            }

            return _datos.ObtenerDetalleTransferenciaReserva(condicion.ToString(), parametros);
        }
    }
}
