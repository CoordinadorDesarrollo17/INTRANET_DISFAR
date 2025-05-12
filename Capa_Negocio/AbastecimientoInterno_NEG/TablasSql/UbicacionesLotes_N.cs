using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class UbicacionesLotes_N
    {
        readonly UbicacionesLotes_D datosUbicacionesL = new UbicacionesLotes_D();
        private readonly Helpers _helper = new Helpers();
        private readonly Ubicaciones_N _ubicacionesN = new Ubicaciones_N();

        public List<UbicacionesLotes_E> ListarUbicaciones(UbicacionesLotes_E filtros, SqlConnection cn = null, Dictionary<string, object> parametros = null)
        {
            StringBuilder condicion = new StringBuilder();

            if (parametros == null)
                parametros = new Dictionary<string, object>();

            if (filtros != null)
            {
                if (filtros.Id > 0)
                {
                    condicion.AppendLine("AND ULM.Id = @Id");
                    parametros["@Id"] = filtros.Id;
                }

                if (!string.IsNullOrWhiteSpace(filtros.CodigoUbicacion))
                {
                    condicion.AppendLine("AND ULM.CodigoUbicacion = @CodigoUbicacion");
                    parametros["@CodigoUbicacion"] = filtros.CodigoUbicacion;
                }

                if (!string.IsNullOrWhiteSpace(filtros.ItemCode))
                {
                    condicion.AppendLine("AND ULM.ItemCode = @ItemCode");
                    parametros["@ItemCode"] = filtros.ItemCode;
                }

                if (!string.IsNullOrWhiteSpace(filtros.ItemName))
                {
                    condicion.AppendLine("AND ULM.ItemName = @ItemName");
                    parametros["@ItemName"] = filtros.ItemName;
                }

                if (!string.IsNullOrWhiteSpace(filtros.Almacen))
                {
                    condicion.AppendLine("AND ULM.Almacen = @Almacen");
                    parametros["@Almacen"] = filtros.Almacen;
                }
            }

            return datosUbicacionesL.ListarUbicaciones(condicion.ToString(), parametros, cn);
        }

        public List<UbicacionesLotes_E> Obtener(string itemCode, string batchNum = null)
        {
            return datosUbicacionesL.Obtener(itemCode, batchNum);
        }

        public UbicacionesLotes_E ObtenerPorId(int id)
        {
            return datosUbicacionesL.ObtenerPorId(id);
        }

        public Helper_E Ingreso(DetalleTransferenciaReserva_E ingreso, SqlConnection cn)
        {
            return datosUbicacionesL.Ingreso(ingreso, cn);
        }
        public Helper_E RevertirIngreso(TransferenciaReserva_E ingreso, SqlConnection cn)
        {
            return datosUbicacionesL.RevertirIngreso(ingreso, cn);
        }
        public Helper_E Salida(List<DetalleRequerimientos_E> salida, SqlConnection cn)
        {
            return datosUbicacionesL.Salida(salida, cn);
        }

        public Helper_E EliminarArticulo(string itemCode, string codigoUbicacion)
        {
            if (string.IsNullOrWhiteSpace(itemCode))
                return _helper.CrearRespuestaError("El código de artículo no es válido.");

            if (string.IsNullOrWhiteSpace(codigoUbicacion))
                return _helper.CrearRespuestaError("El código de ubicación no es válido.");

            return datosUbicacionesL.EliminarArticulo(itemCode, codigoUbicacion);
        }

        public Helper_E RegistrarCodigoUbicacionPicking(List<DetalleRequerimientos_E> detalle, SqlConnection cn)
        {
            Helper_E resultRegistro = new Helper_E();

            if (detalle.Where(d => d?.CodigoUbicacionDestino == null).Any())
            {
                return _helper.CrearRespuestaError("Las ubicaciones Picking deben estar definidas.");
            }

            foreach (var item in detalle)
            {
                // Separar por comas y limpiar espacios
                var ubicaciones = item.CodigoUbicacionDestino
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(u => u.Trim()) // Eliminar espacios en blanco alrededor
                    .ToList();

                foreach (var ubicacionCodigo in ubicaciones)
                {
                    var ubicacion = _ubicacionesN.ListarUbicaciones(
                        new Ubicaciones_E { Almacen = "PICKING", CodigoUbicacion = ubicacionCodigo }, cn);

                    var resultULotes = ListarUbicaciones(
                        new UbicacionesLotes_E
                        {
                            ItemCode = item.ItemCode,
                            BatchNum = item.BatchNum,
                            CodigoUbicacion = ubicacionCodigo,
                            Almacen = "PICKING"
                        });

                    if (!resultULotes.Any())
                    {
                        item.UbicacionId = ubicacion.First().Id;
                        item.CodigoUbicacionDestino = ubicacionCodigo;
                        item.QuantityUnidadesCajas = 0;        // Para PICKING no necesitamos guardar el QuantityUnidadesCajas
                        resultRegistro = datosUbicacionesL.RegistrarCodigoUbicacionPicking(new List<DetalleRequerimientos_E> { item }, cn);
                    }
                    else
                    {
                        resultRegistro = _helper.CrearAlertaUI(new List<string> { $"Código de ubicación PICKING '{ubicacionCodigo}' ya se encuentra registrado." }, "info");
                    }
                }
            }

            return resultRegistro;
        }

        public Helper_E CambiarUbicacionPicking(string nuevoCodigoUbicacion, int ubicacionLoteId)
        {
            var obj = ObtenerPorId(ubicacionLoteId);
            if (obj == null || obj.Id <= 0)
                return _helper.CrearAlertaUI(new List<string> { "El código de ubicación actual ya no es válido.", "Actualiza la página y vuelve a intentar." }, "error");

            return datosUbicacionesL.CambiarUbicacionPicking(nuevoCodigoUbicacion, obj);
        }
    }
}
