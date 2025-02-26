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

        public Requerimientos_E ObtenerRequerimiento(int id)
        {
            return _requerimientoD.ObtenerRequerimiento(id);
        }
        public Helper_E AtenderReserva(int detalleId)
        {
            return _requerimientoD.AtenderReserva(detalleId);
        }
        public Helper_E AtenderPicking(int detalleId)
        {
            return _requerimientoD.AtenderPicking(detalleId);
        }
        public List<DetalleRequerimientos_E> ListarDetalles(string itemCode="", string proceso = "")
        {
             List<DetalleRequerimientos_E> result = null;
            switch (proceso)
            {
                case "CantidadSolicitada" : 
                    result = _requerimientoD.ListarDetalles().Where(x => x.AtendidoPicking == 0 && x.ItemCode == itemCode).ToList();
                    break;
                case "ListarApiladores":
                    result = _requerimientoD.ListarDetalles().Where(x => x.AtendidoReserva == 0 && x.AtendidoPicking == 0 && x.QuantityMaster>0).ToList();
                    break;
                case "ListarPicking":
                    result = _requerimientoD.ListarDetalles().Where(x => x.AtendidoReserva == 1 && x.AtendidoPicking == 0 ).ToList();
                    break;
                default:
                    result = _requerimientoD.ListarDetalles();
                    break;

            }
           
            return result;
        }
        public Requerimientos_E RegistrarRequerimiento(Requerimientos_E requerimiento, SqlConnection cn)
        {
            //Validar que las Cantidades que se desean imputar se encuentren disponibles

            return _requerimientoD.RegistrarRequerimiento(requerimiento, cn);

        }
        public bool ValidarSkuParaKardexSalida(DetalleRequerimientos_E detalle)
        {
            Requerimientos_E requerimientoCompleto = ObtenerRequerimiento(detalle.RequerimientoId);

            // Validar si ya no existe algún elemento con el mismo ItemCode y AtendidoPicking en 0, por lo tanto todo esta Atendido y listo para sacarlo por Kardex
            bool valido = !requerimientoCompleto.Detalle.Any(d => d.ItemCode == detalle.ItemCode && d.AtendidoPicking == 0);

            return valido;
        }
    }
}
