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
        
        public bool BuscarUbicacion(string almacen, string  ubicacion)
        {
            return _datos.BuscarUbicacion(almacen, ubicacion);
        }
        public string[] ListarTotalUbicacionesEnArray(string almacen, string itemCode)
        {
            return _datos.ListarTotalUbicacionesEnArray(almacen, itemCode);
        }
        public List<Ubicaciones_E> ListarUbicaciones(Ubicaciones_E filtros = null, SqlConnection cn = null, Dictionary<string, object> parametros = null )
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

                if (!string.IsNullOrWhiteSpace(filtros.CodigoUbicacion))
                {
                    condicion.AppendLine("AND CodigoUbicacion = @CodigoUbicacion");
                    parametros["@CodigoUbicacion"] = filtros.CodigoUbicacion;
                }

                if (!string.IsNullOrWhiteSpace(filtros.Almacen))
                {
                    condicion.AppendLine("AND Almacen = @Almacen");
                    parametros["@Almacen"] = filtros.Almacen;
                }
            }

            return _datos.ListarUbicaciones(condicion.ToString(), parametros, cn);
        }
        public Ubicaciones_E ObtenerDatosUbicacion(Ubicaciones_E filtros)
        {
            return ListarUbicaciones(filtros).DefaultIfEmpty(new Ubicaciones_E()).First();
        }
        public Helper_E RegistrarUbicacion(Ubicaciones_E datos)
        {
            var errores = new List<string>();

            if (datos == null)
                errores.Add("Verificar datos enviados.");

            if (string.IsNullOrWhiteSpace(datos.CodigoUbicacion))
                errores.Add("Código de ubicación no válida.");

            var ubicacionExistente = ObtenerDatosUbicacion(new Ubicaciones_E { Almacen = datos.Almacen, CodigoUbicacion = datos.CodigoUbicacion });

            if (ubicacionExistente != null && ubicacionExistente.Id > 0)
                errores.Add("La ubicación ya se encuentra registrada.");

            if (errores.Any())
                return new Helper_E { Mensajes = errores, IconoSweetAlert = "error" };

            return _datos.RegistrarUbicacion(datos);
        }

        public Helper_E EliminarUbicacion(string codigoUbicacion)
        {
            // En el procedure tiene una validación para saber si cuenta con stock asociado en UbicacionesLotes o UbicacionesLotesMaster
            var ubicacionExistente = ObtenerDatosUbicacion(new Ubicaciones_E { CodigoUbicacion = codigoUbicacion });

            // Verificar si existe la ubicación
            if (string.IsNullOrWhiteSpace(codigoUbicacion) || ubicacionExistente == null || ubicacionExistente.Id <= 0)
                return _helper.CrearRespuestaError("Ubicación no válida. Recargar página y volver a intentar.");

            codigoUbicacion = codigoUbicacion.Trim();
            codigoUbicacion = codigoUbicacion.Replace("'", "''");

            return _datos.EliminarUbicacion(codigoUbicacion);
        }
    }
}
