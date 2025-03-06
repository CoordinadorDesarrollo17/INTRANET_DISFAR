using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
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

        public Helper_E Ingreso (int ubicacionLoteId, DetalleTransferenciaReserva_E ingreso, SqlConnection cn)
        {
            return datosUbicacionesLM.Ingreso(ubicacionLoteId,ingreso, cn);
        }
        public Helper_E RevertirIngreso(TransferenciaReserva_E ingreso, SqlConnection cn)
        {
            return datosUbicacionesLM.RevertirIngreso(ingreso, cn);
        }
        public Helper_E Salida(List<DetalleRequerimientos_E> salida, SqlConnection cn)
        {
            return datosUbicacionesLM.Salida(salida, cn);
        }
        public List<string> BuscarUnidadAlm(SqlConnection cn, UbicacionesLotesMaster_E filtros = null, StringBuilder condicion = null, Dictionary<string, object> parametros = null)
        {
            condicion = new StringBuilder();
            parametros = new Dictionary<string, object>();

            if (filtros != null)
            {
                if (!string.IsNullOrWhiteSpace(filtros.Almacen))
                {
                    condicion.AppendLine("AND Almacen = @Almacen");
                    parametros["@Almacen"] = filtros.Almacen;
                }

                if (!string.IsNullOrWhiteSpace(filtros.ItemCode))
                {
                    condicion.AppendLine("AND ItemCode = @ItemCode");
                    parametros["@ItemCode"] = filtros.ItemCode;
                }

                if (!string.IsNullOrWhiteSpace(filtros.BatchNum))
                {
                    condicion.AppendLine("AND BatchNum = @BatchNum");
                    parametros["@BatchNum"] = filtros.BatchNum;
                }
            }

            return datosUbicacionesLM.BuscarUnidadAlm(cn, condicion.ToString(), parametros);
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
    }
}
