using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class Requerimientos_N
    {
        Requerimientos_D _requerimientoD = new Requerimientos_D();

        public Requerimientos_E ObtenerRequerimiento(int id, SqlConnection cn)
        {
            return _requerimientoD.ObtenerRequerimiento(id, cn);
        }
        public Helper_E AtenderReserva(int detalleId)
        {
            return _requerimientoD.AtenderReserva(detalleId);
        }
        public Helper_E AtenderPicking(int detalleId, SqlConnection cn)
        {
            return _requerimientoD.AtenderPicking(detalleId,cn);
        }
        public List<DetalleRequerimientos_E> ListarDetalles(string itemCode="", string proceso = "")
        {
            var detalles = _requerimientoD.ListarDetalles() ?? new List<DetalleRequerimientos_E>();
            var result = new List<DetalleRequerimientos_E>();

            switch (proceso)
            {
                case "CantidadSolicitada":
                    result = detalles
                        .Where(x => x.AtendidoPicking == 0 && x.ItemCode == itemCode)
                        .ToList();
                    break;

                case "ListarApiladores":
                    result = detalles
                        .Where(x => x.AtendidoReserva == 0 && x.AtendidoPicking == 0)
                        .ToList();
                    break;

                case "ListarPicking":
                    result = detalles
                        .Where(x => x.AtendidoReserva == 1 && x.AtendidoPicking == 0)
                        .ToList();
                    break;

                default:
                    result = detalles;
                    break;
            }
            return result;
        }
        public Requerimientos_E RegistrarRequerimiento(Requerimientos_E requerimiento, SqlConnection cn)
        {
            //Validar que las Cantidades que se desean imputar se encuentren disponibles
            return _requerimientoD.RegistrarRequerimiento(requerimiento, cn);
        }
        public bool ValidarSkuParaKardexSalida(int requerimientoId,string itemCode, Requerimientos_E requerimiento)
        {
            // Validar si ya no existe algún elemento con el mismo ItemCode y AtendidoPicking en 0, por lo tanto todo esta Atendido y listo para sacarlo por Kardex
            bool valido = !requerimiento.Detalle.Any(d => d.ItemCode == itemCode && d.AtendidoPicking == 0);
            return valido;
        }
       public bool ValidarSkuParaCambioUbicacion(string itemCode, string batchNum,string codigoUbicacionReserva)
        {
            var lista = ListarDetalles(itemCode);
            bool valido = !lista.Any(d => 
            d.CodigoUbicacionOrigen==codigoUbicacionReserva && 
            d.BatchNum==batchNum &&
            d.AtendidoPicking == 0);
            return valido;
        }
    }
}
