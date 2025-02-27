using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class Ubicaciones_N
    {
        Ubicaciones_D _datos = new Ubicaciones_D();
        Helpers _helper = new Helpers();

        public string[] BuscarUbicaciones(string almacen, string itemCode)
        {
            return _datos.BuscarUbicaciones(almacen, itemCode);
        }
        public List<Ubicaciones_E> ListarUbicaciones(Ubicaciones_E filtros)
        {
            return _datos.ListarUbicaciones(filtros);
        }
        public Ubicaciones_E ObtenerDatosUbicacion(Ubicaciones_E filtros)
        {
            var result = _datos.ListarUbicaciones(filtros)
                .DefaultIfEmpty(new Ubicaciones_E())
                .First();

            return result;
        }
        public Helper_E RegistrarUbicacion(Ubicaciones_E datos)
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

            var ubicacionExistente = ObtenerDatosUbicacion(new Ubicaciones_E { Almacen = datos.Almacen, ItemCode = datos.ItemCode, ItemName = datos.ItemName, CodigoUbicacion = datos.CodigoUbicacion });

            if (ubicacionExistente != null && ubicacionExistente.Id > 0)
                errores.Add("El código de ubicación del producto ya se encuentra registrado.");

            if (errores.Any())
                return new Helper_E { Mensajes = errores, IconoSweetAlert = "error" };

            return _datos.RegistrarUbicacion(datos);
        }
        public Helper_E EliminarUbicacion(int ubicacionId)
        {
            var ubicacion = ObtenerDatosUbicacion(new Ubicaciones_E { Id = ubicacionId });

            // Verificar si existe la ubicación
            if (ubicacionId <= 0 || ubicacion == null || ubicacion.Id <= 0)
                return _helper.CrearRespuestaError("Ubicación no válida. Recargar página y volver a intentar.");

            return _datos.EliminarUbicacion(ubicacionId);
        }
        public Helper_E EliminarUbicacionGeneral(string codigoUbicacion)
        {
            codigoUbicacion = codigoUbicacion.Trim();
            codigoUbicacion = codigoUbicacion.Replace("'", "''");

            return _datos.EliminarUbicacionGeneral(codigoUbicacion);
        }
    }
}
