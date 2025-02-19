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
    public class UbicacionesReserva_N
    {
        UbicacionesReserva_D _datos = new UbicacionesReserva_D();
        Helpers _helper = new Helpers();

        public List<UbicacionesReserva_E> ListarUbicacionesReserva(UbicacionesReserva_E filtros, StringBuilder condicion = null, Dictionary<string, object> parametros = null)
        {
            condicion = new StringBuilder();
            parametros = new Dictionary<string, object>();

            if (filtros != null)
            {
                if (filtros.Id > 0)
                {
                    condicion.AppendLine("AND UL.Id = @Id");
                    parametros["@Id"] = filtros.Id;
                }

                if (!string.IsNullOrWhiteSpace(filtros.ItemCode))
                {
                    condicion.AppendLine("AND UL.ItemCode = @ItemCode");
                    parametros["@ItemCode"] = filtros.ItemCode;
                }
            }

            return _datos.ListarUbicacionesReserva(condicion.ToString(), parametros);
        }

        public UbicacionesReserva_E ObtenerDatosUbicacionReserva(UbicacionesReserva_E filtros)
        {
            var result = ListarUbicacionesReserva(filtros)
                .DefaultIfEmpty(new UbicacionesReserva_E())
                .First();

            return result;
        }

        public Helper_E RegistrarUbicacionReserva(UbicacionesReserva_E datos)
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

            var ubicacionReserva = ObtenerDatosUbicacionReserva(new UbicacionesReserva_E { ItemCode = datos.ItemCode, ItemName = datos.ItemName, CodigoUbicacion = datos.CodigoUbicacion });
            if (ubicacionReserva != null && ubicacionReserva.Id > 0)
                errores.Add("El código de ubicación del producto ya se encuentra registrado.");

            if (errores.Any())
                return new Helper_E { Mensajes = errores, IconoSweetAlert = "error" };

            return _datos.RegistrarUbicacionReserva(datos);
        }

        public Helper_E EliminarUbicacionReserva(int ubicacionId)
        {
            var ubicacionReserva = ObtenerDatosUbicacionReserva(new UbicacionesReserva_E { UbicacionId = ubicacionId });

            // Verificar si existe la ubicación
            if (ubicacionId <= 0 || ubicacionReserva == null || ubicacionReserva.Id <= 0)
                return _helper.CrearRespuestaError("Ubicación no válida. Recargar página y volver a intentar.");

            return _datos.EliminarUbicacionReserva(ubicacionId);
        }        
    }
}
