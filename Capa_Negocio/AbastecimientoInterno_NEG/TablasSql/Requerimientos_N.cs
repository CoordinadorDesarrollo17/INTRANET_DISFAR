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
using Capa_Entidad.TablasSql;

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
            return _requerimientoD.AtenderPicking(detalleId, cn);
        }
        public List<DetalleRequerimientos_E> ListarDetalles(string itemCode = "", string proceso = "")
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
                        .Where(x => x.AtendidoReserva == 0 && x.AtendidoPicking == 0 && x.Aprobado == 1)
                        .OrderByDescending(x => x.Zona)
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
            return _requerimientoD.RegistrarRequerimiento(requerimiento, cn);
        }
        public bool ValidarSkuParaKardexSalida(int requerimientoId, string itemCode, Requerimientos_E requerimiento)
        {
            // Validar si ya no existe algún elemento con el mismo ItemCode y AtendidoPicking en 0, por lo tanto todo esta Atendido y listo para sacarlo por Kardex
            bool valido = !requerimiento.Detalle.Any(d => d.ItemCode == itemCode && d.AtendidoPicking == 0);
            return valido;
        }
        public bool ValidarSkuParaCambioUbicacion(string itemCode, string batchNum, string codigoUbicacionReserva)
        {
            var lista = ListarDetalles(itemCode);
            bool valido = !lista.Any(d =>
            d.CodigoUbicacionOrigen == codigoUbicacionReserva &&
            d.BatchNum == batchNum &&
            d.AtendidoPicking == 0);
            return valido;
        }

        public (Helper_E, List<Requerimientos_E>) ListarRequerimientos(Requerimientos_E filtros = null, Dictionary<string, object> parametros = null)
        {
            StringBuilder condicion = new StringBuilder();

            if (parametros == null)
                parametros = new Dictionary<string, object>();

            if (filtros != null)
            {
                if (filtros.Id > 0)
                {
                    condicion.AppendLine("AND RQ.Id = @Id");
                    parametros["@Id"] = filtros.Id;
                }

                if (!string.IsNullOrWhiteSpace(filtros.TipoAbastecimiento))
                {
                    condicion.AppendLine("AND RQ.TipoAbastecimiento = @TipoAbastecimiento");
                    parametros["@TipoAbastecimiento"] = filtros.TipoAbastecimiento;
                }

                if (!string.IsNullOrWhiteSpace(filtros.Zona))
                {
                    condicion.AppendLine("AND RQ.Zona = @Zona");
                    parametros["@Zona"] = filtros.Zona;
                }

                if (!string.IsNullOrWhiteSpace(filtros.FechaRegistro))
                {
                    condicion.AppendLine("AND RQ.TiempoRegistro >= @FechaRegistro AND RQ.TiempoRegistro < DATEADD(DAY, 1, @FechaRegistro)");
                    parametros["@FechaRegistro"] = filtros.FechaRegistro;
                }

                if (filtros.Aprobado != null)
                {
                    condicion.AppendLine("AND RQ.Aprobado = @Aprobado");
                    parametros["@Aprobado"] = filtros.Aprobado;
                }

                if (!string.IsNullOrWhiteSpace(filtros.OperarioRegistra))
                {
                    condicion.AppendLine("AND RQ.OperarioRegistra = @OperarioRegistra");
                    parametros["@OperarioRegistra"] = filtros.OperarioRegistra;
                }
            }

            return _requerimientoD.ListarRequerimientos(condicion.ToString(), parametros);
        }

        public Helper_E AprobarRequerimiento(int id, string operarioRegistra)
        {
            return _requerimientoD.AprobarRequerimiento(id, operarioRegistra);
        }

        public Helper_E RechazarRequerimiento(int id, string operarioRegistra)
        {
            return _requerimientoD.RechazarRequerimiento(id, operarioRegistra);
        }
    }
}