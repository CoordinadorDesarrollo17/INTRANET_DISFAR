using Capa_Datos.Ventas_DAO.Tablas;
using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Ventas_ENT.Reportes;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
	public class OEP_N
	{
		OEP_D oepD = new OEP_D();

		public string RegistrarErroresPicking(OEP_E cabecera, List<EP1_E> detalleErroresPicking)
		{
            // Verificar si ya existen errores de picking para este ticket
            if (oepD.ObtenerDatosErroresPicking(cabecera.DocEntryTicket) != null)
            {
                throw new Exception("El ticket ya cuenta con errores de picking registrados");
            }

            // Verificar la validez del ticket
            if (cabecera.DocEntryTicket <= 0 || cabecera.DocNumTicket <= 0)
			{
				throw new Exception("El detalle debe estar enlazado a un ticket válido");
			}

            // Verificar si hay detalles de errores de picking
            if (detalleErroresPicking == null || detalleErroresPicking.Count == 0)
            {
                throw new ArgumentException("Debe proporcionar al menos un artículo para registrar errores de picking");
            }

            // Verificar cada detalle de errores de picking
            foreach (var detalle in detalleErroresPicking)
            {
                if (detalle.IdTipoErrorPicking <= 0)
                {
                    throw new ArgumentException($"Debe especificar un tipo de error para el producto {detalle.CodigoProducto}");
                }
            }

            return oepD.RegistrarErroresPicking(cabecera, detalleErroresPicking);
		}

		public List<RptErroresPicking_E> ExportarReporteErroresPicking(RptFiltrosErroresPicking_E filtros)
		{
			return oepD.ExportarReporteErroresPicking(filtros);
		}
	}
}
