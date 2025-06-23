using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Negocio.Almacen_NEG.Tablas;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class UbicacionesLotesMaster_N
    {
        readonly UbicacionesLotesMaster_D datosUbicacionesLM = new UbicacionesLotesMaster_D();

        public List<UbicacionesLotesMaster_E> ListarUbicaciones(UbicacionesLotesMaster_E filtros, SqlConnection cn = null, Dictionary<string, object> parametros = null)
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

            return datosUbicacionesLM.ListarUbicaciones(condicion.ToString(), parametros, cn);
        }

        public Helper_E Ingreso(int ubicacionLoteId, DetalleTransferenciaReserva_E ingreso, SqlConnection cn)
        {
            return datosUbicacionesLM.Ingreso(ubicacionLoteId, ingreso, cn);
        }
        public Helper_E RevertirIngreso(TransferenciaReserva_E ingreso, SqlConnection cn)
        {
            return datosUbicacionesLM.RevertirIngreso(ingreso, cn);
        }
        public Helper_E Salida(List<DetalleRequerimientos_E> salida, SqlConnection cn)
        {
            var resultado = datosUbicacionesLM.Salida(salida, cn);
            var almacenes = new List<string> { "ALM07", "ALM08", "16", "03" };

            if (resultado.Icono == "success")
            {
                foreach (var item in salida)
                {
                    var stock = new OITW_N().ObtenerStockSKUPorAlmacen(item.ItemCode, almacenes);

                    if (stock == 0)
                        datosUbicacionesLM.LimpiarRegistros(item, cn);
                }
            }

            return resultado;
        }

        public List<UbicacionesLotesMaster_E> BuscarArticulos(UbicacionesLotesMaster_E filtros = null, StringBuilder condicion = null, Dictionary<string, object> parametros = null)
        {
            condicion = new StringBuilder();
            parametros = new Dictionary<string, object>();

            if (filtros != null)
            {
                if (!string.IsNullOrWhiteSpace(filtros.ItemCode))
                {
                    condicion.AppendLine("AND ULM.[ItemCode] = @ItemCode");
                    parametros["@ItemCode"] = filtros.ItemCode;
                }
            }

            return datosUbicacionesLM.BuscarArticulos(condicion.ToString(), parametros);
        }
        public UbicacionesLotesMaster_E Obtener(int id)
        {
            return datosUbicacionesLM.Obtener(id);
        }
    }
}
