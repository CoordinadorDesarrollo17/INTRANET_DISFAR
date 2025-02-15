using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad.AtencionCliente_ENT.TablasSql;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class UbicacionesPicking_N
    {
        UbicacionesPicking_D _datos = new UbicacionesPicking_D();
        Helpers _helper = new Helpers();

        public List<Ubicaciones_E> ListarUbicacionesPicking(Ubicaciones_E filtros, StringBuilder condicion = null, Dictionary<string, object> parametros = null)
        {
            condicion = new StringBuilder();
            parametros = new Dictionary<string, object>();

            if (filtros != null)
            {
                if (filtros.Id > 0)
                {
                    condicion.AppendLine("AND UP.Id = @Id");
                    parametros["@Id"] = filtros.Id;
                }

                if (!string.IsNullOrWhiteSpace(filtros.ItemCode))
                {
                    condicion.AppendLine("AND UP.ItemCode = @ItemCode");
                    parametros["@ItemCode"] = filtros.ItemCode;
                }
            }

            return _datos.ListarUbicacionesPicking(condicion.ToString(), parametros);
        }

        public Ubicaciones_E ObtenerDatosUbicacionPicking(Ubicaciones_E filtros)
        {
            var result = ListarUbicacionesPicking(filtros)
                .DefaultIfEmpty(new Ubicaciones_E())
                .First();

            return result;
        }

        public Helper_E RegistrarUbicacionPicking(Ubicaciones_E datos)
        {
            var errores = new List<string>();

            if (datos == null)
                errores.Add("Verificar datos enviados.");

            if (string.IsNullOrWhiteSpace(datos.ItemCode))
                errores.Add("Código de artículo no válido.");

            if (string.IsNullOrWhiteSpace(datos.ItemName))
                errores.Add("Descripción no válida.");

            if (string.IsNullOrWhiteSpace(datos.CodigoUbicacion))
                errores.Add("Código de ubicación no válida.");

            var ubicacionPicking = ObtenerDatosUbicacionPicking(new Ubicaciones_E { ItemCode = datos.ItemCode, ItemName = datos.ItemName, CodigoUbicacion = datos.CodigoUbicacion });
            if (ubicacionPicking != null && ubicacionPicking.Id > 0)
                errores.Add("El código de ubicación del producto ya se encuentra registrado.");

            if (errores.Any())
                return new Helper_E { Mensajes = errores, IconoSweetAlert = "error" };

            return _datos.RegistrarUbicacionPicking(datos);
        }

        public Helper_E EliminarUbicacionPicking(int id)
        {
            var ubicacionPicking = ObtenerDatosUbicacionPicking(new Ubicaciones_E { Id = id });

            if (id <= 0 || ubicacionPicking == null || ubicacionPicking.Id <= 0)
                return _helper.CrearRespuestaError("Ubicación no válida. Recargar página y volver a intentar.");

            return _datos.EliminarUbicacionPicking(id);
        }
    }
}
