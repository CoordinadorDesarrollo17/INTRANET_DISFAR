using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.DireccionTecnica_DAO.TablasSql;
using Capa_Entidad;
using Capa_Entidad.DireccionTecnica_ENT.TablasSql;
using Capa_Entidad.TablasSql;

namespace Capa_Negocio.DireccionTecnica_NEG.TablasSql
{
    public class SolicitudesReversion_DOCS1_N
    {
        private readonly SolicitudesReversion_DOCS1_D _datos = new SolicitudesReversion_DOCS1_D();
        private readonly Helpers _helpers = new Helpers();

        public List<SolicitudesReversion_DOCS1_E> ListarSolicitudesReversion(SolicitudesReversion_DOCS1_E filtros = null, Dictionary<string, object> parametros = null)
        {
            StringBuilder condicion = new StringBuilder();

            if (parametros == null)
                parametros = new Dictionary<string, object>();

            if (filtros != null)
            {
                if (filtros.Id > 0)
                {
                    condicion.AppendLine("AND SOL.Id = @Id");
                    parametros["@Id"] = filtros.Id;
                }

                if (filtros.DOCS1Id > 0)
                {
                    condicion.AppendLine("AND SOL.DOCS1Id = @DOCS1Id");
                    parametros["@DOCS1Id"] = filtros.DOCS1Id;
                }

                if (!string.IsNullOrWhiteSpace(filtros.Estado))
                {
                    condicion.AppendLine("AND SOL.Estado = @Estado");
                    parametros["@Estado"] = filtros.Estado;
                }
            }

            return _datos.ListarSolicitudesReversion(condicion.ToString(), parametros);
        }

        public Helper_E SolicitarReversionLiberacionArticulo(int id, string usuarioRegistro)
        {
            var lista = ListarSolicitudesReversion(new SolicitudesReversion_DOCS1_E { DOCS1Id = id }).OrderByDescending(sr => sr.Id).ToList();

            if (lista != null && lista.Any())
            {
                if (lista.First().Estado == "Pendiente")
                    return _helpers.CrearRespuestaError("La solicitud de reversión ya se encuentra PENDIENTE. Por favor, esperar la confirmación del Director Técnico.");

                if (lista.First().Estado == "Aprobada")
                    return _helpers.CrearRespuestaError("Ya existe una solicitud de reversión APROBADA.");
            }

            return _datos.SolicitarReversionLiberacionArticulo(id, usuarioRegistro);
        }
    }
}
