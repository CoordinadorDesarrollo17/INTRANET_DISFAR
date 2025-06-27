using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Datos;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class DetalleRequerimientos_N
    {
        DetalleRequerimiento_D _datos = new DetalleRequerimiento_D();

        public (Helper_E, List<DetalleRequerimientos_E>) ObtenerDetalleRequerimiento(DetalleRequerimientos_E filtros = null, Dictionary<string, object> parametros = null)
        {
            StringBuilder condicion = new StringBuilder();
            Helper_E _helper = new Helper_E();

            if (parametros == null)
                parametros = new Dictionary<string, object>();

            // Obligatorio enviar al menos el RequerimientoId
            if (filtros == null/* || filtros.RequerimientoId <= 0*/)
            {
                _helper.Titulo = "Error";
                _helper.Mensajes.Add("Ocurrió un error al obtener datos.");
                _helper.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                _helper.Icono = "error";

                return (_helper, null);
            }
           
            condicion.AppendLine("WHERE DET.AtendidoReserva = @AtendidoReserva");
            parametros["@AtendidoReserva"] = filtros.AtendidoReserva;

            condicion.AppendLine("AND DET.AtendidoPicking = @AtendidoPicking");
            parametros["@AtendidoPicking"] = filtros.AtendidoPicking;

            if (filtros.RequerimientoId > 0)
            {
                condicion.AppendLine("AND DET.RequerimientoId = @RequerimientoId");
                parametros["@RequerimientoId"] = filtros.RequerimientoId;
            }

            return _datos.ObtenerDetalleRequerimiento(condicion.ToString(), parametros);
        }

        public Helper_E EliminarItem(int id, string operarioRegistra)
        {
            return _datos.EliminarItem(id, operarioRegistra);
        }
    }
}