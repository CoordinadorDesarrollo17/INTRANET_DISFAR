using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.TablasSql;
using Capa_Entidad;
using Capa_Datos.DireccionTecnica_DAO.TablasSql;

namespace Capa_Negocio.DireccionTecnica_NEG.TablasSql
{
    public class DOCS1_N
    {
        private readonly DOCS1_D _datos = new DOCS1_D();
        private readonly Helpers _helpers = new Helpers();

        public Helper_E EditarItemDetalleDoc(DOCS1_E detalle, string usuarioRegistro)
        {
            // Agregar validación obligatoria por lo campos que vienen de la vista
            // porque al editar item, debe enviar todos los campos de la fila que llena el usuario
            //if (item.CantidadAprobados > 0 || item.CantidadBaja > 0 || item.CantidadDevolucion > 0)

                var detalleConErrores = ValidarDetalleDocumento(new List<DOCS1_E> { detalle });

            if (detalleConErrores == null)
                return detalleConErrores;

            return _datos.EditarItemDetalleDoc(detalle, usuarioRegistro);
        }

        public Helper_E ValidarDetalleDocumento(List<DOCS1_E> detalle)
        {
            if (detalle == null || !detalle.Any())
                return _helpers.CrearRespuestaError("El detalle del documento está vacío o no existe.");

            int index = 1;

            foreach (var item in detalle)
            {
                if (string.IsNullOrWhiteSpace(item.ItemCode))
                    return _helpers.CrearRespuestaError($"Código de artículo no válido en ítem #{index}");

                if (string.IsNullOrWhiteSpace(item.ItemName))
                    return _helpers.CrearRespuestaError($"Descripción de artículo no válida en ítem #{index}");

                if (string.IsNullOrWhiteSpace(item.Lote))
                    return _helpers.CrearRespuestaError($"Lote no válido en ítem #{index}");

                if (string.IsNullOrWhiteSpace(item.FechaVencimiento))
                    return _helpers.CrearRespuestaError($"Fecha de vencimiento no válida en ítem #{index}");

                if (string.IsNullOrWhiteSpace(item.RegistroSanitario))
                    return _helpers.CrearRespuestaError($"Registro sanitario no válido en ítem #{index}");

                if (string.IsNullOrWhiteSpace(item.Fabricante))
                    return _helpers.CrearRespuestaError($"Fabricante no válido en ítem #{index}");

                if (string.IsNullOrWhiteSpace(item.CondicionAlmTrans))
                    return _helpers.CrearRespuestaError($"Condición de almacenamiento y transporte no válido en ítem #{index}");

                if (string.IsNullOrWhiteSpace(item.Almacen))
                    return _helpers.CrearRespuestaError($"Almacen no válido en ítem #{index}");

                if (item.ArchivoET == null && item.ArchivoProtocolo == null)
                    return _helpers.CrearRespuestaError($"Debe cargar un Protocolo y/o ET en el ítem #{index}");

                if (item.CantidadAprobados > 0 || item.CantidadBaja > 0 || item.CantidadDevolucion > 0)
                    if (!EsCantidadValida(item.CantidadAprobados, item.CantidadBaja, item.CantidadDevolucion, item.CantidadTotal))
                        return _helpers.CrearRespuestaError($"Las cantidades ingresadas no suman la cantidad total en ítem #{index}");

                index++;
            }

            return null;
        }

        private bool EsCantidadValida(int cantidadAprobados, int cantidadBaja, int cantidadDevolucion, int cantidadTotal)
        {
            var bandera = false;
            var calculo = cantidadAprobados + cantidadBaja + cantidadDevolucion;

            if (calculo == cantidadTotal)
                bandera = true;

            return bandera;
        }
    }
}
