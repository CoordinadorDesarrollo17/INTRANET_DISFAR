using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.TablasSql;
using Capa_Entidad;
using Capa_Datos.DireccionTecnica_DAO.TablasSql;
using System.Reflection;
using System.Collections;
using Capa_Entidad.DireccionTecnica_ENT.TablasSql;

namespace Capa_Negocio.DireccionTecnica_NEG.TablasSql
{
    public class DOCS1_N
    {
        private readonly DOCS1_D _datos = new DOCS1_D();
        private readonly Helpers _helpers = new Helpers();

        public Helper_E TransferirArticulo(int cantidad, string area, int id, string usuarioRegistro)
        {
            var detalle = new DOCS1_E();
            var lista = ListarDetalleDocumento(new DOCS1_E { Id = id });

            if (lista != null && lista.Any())
                detalle = lista.First();

            if (detalle.Liberado == 0)
                return _helpers.CrearRespuestaError("Para transferir este artículo, primero debe estar en estado LIBERADO.");

            if (detalle.Liberado == 1 && detalle.Transferido == 1)
                return _helpers.CrearRespuestaError("El artículo ya se encuentra TRANSFERIDO.");

            // Esta validación nos ayuda para los casos que el Operario de Almacén ya está trabajando con el detalle cargado en la vista pero el área de DT revirtió un ítem y cambio las cantidades
            // entonces el Operario de Almacén al recargar la página, obtendrá las nuevas cantidades.
            bool esValido = false;
            switch (area.ToLower())
            {
                case "aprobados":
                    esValido = detalle.CantidadAprobados == cantidad;
                    break;

                case "baja":
                    esValido = detalle.CantidadBaja == cantidad;
                    break;

                case "devolucion":
                    esValido = detalle.CantidadDevolucion == cantidad;
                    break;
                case "faltante":
                    esValido = detalle.CantidadFaltante == cantidad;
                    break;

                default:
                    return _helpers.CrearRespuestaError("Verificar los datos enviados.");
            }

            if (!esValido)
                return _helpers.CrearRespuestaError("Las cantidades del detalle no coinciden, por favor recargar la página.");

            return _datos.TransferirArticulo(area, id, usuarioRegistro);
        }

        public Helper_E RevertirTransferenciaArticulo(int id, string area, string usuarioRegistro)
        {
            var lista = ListarDetalleDocumento(new DOCS1_E { Id = id });

            if (lista == null || lista.Count == 0)
                return _helpers.CrearRespuestaError("No se encontró información del artículo a revertir transferencia.");

            if (string.IsNullOrWhiteSpace(area))
                return _helpers.CrearRespuestaError("Error en el envío de área.");

            var detalle = lista.First();

            if (detalle.Liberado == 0)
                return _helpers.CrearRespuestaError("Para revertir la transferencia de este artículo, primero debe estar en estado LIBERADO.");

            if (detalle.Liberado == 1 && detalle.Transferido == 0)
                return _helpers.CrearRespuestaError("El artículo ya se encuentra LIBERADO y no ha sido transferido.");

            return _datos.RevertirTransferenciaArticulo(id, area, usuarioRegistro);
        }

        public Helper_E LiberarArticulos(List<int> grupoIds, string usuarioRegistro)
        {
            List<Helper_E> helper = new List<Helper_E>();
            Helper_E resultado = new Helper_E { Titulo = "Acción completada", Mensajes = new List<string> { "Artículo liberado correctamente." }, Icono = "success" };

            foreach (var id in grupoIds)
            {
                var lista = ListarDetalleDocumento(new DOCS1_E { Id = id });

                if (lista == null || !lista.Any())
                    continue;

                // Filtrar artículos pendientes por liberar
                var detallesPendientes = lista.Where(x => x.Liberado == 0 && x.Transferido == 0).ToList();

                if (!detallesPendientes.Any())
                    continue;

                var primerDetalle = detallesPendientes.First();

                var validacion = ValidarParaLiberacion(primerDetalle);
                if (validacion == null)
                {
                    helper.Add(_datos.LiberarArticulo(id, usuarioRegistro));
                }
                else
                {
                    helper.Add(validacion);
                }
            }

            if (!helper.Any())
            {
                resultado.Titulo = "Error";
                resultado.Mensajes.Clear();
                resultado.Mensajes.Add("Todos los artículos del detalle ya se encuentran liberados.");
                resultado.Icono = "error";
            }

            // Al menos un error en la lista
            if (helper.Any(l => l.Icono == "error"))
            {
                resultado.Titulo = "Error";
                resultado.Mensajes.Clear();
                resultado.Mensajes = helper.Where(l => l.Icono == "error" && l.Mensajes != null).SelectMany(l => l.Mensajes).Distinct().ToList();
                resultado.Icono = "error";
            }

            return resultado;
        }

        public Helper_E RevertirLiberacionArticulo(int id, string estado, string usuarioRegistro)
        {
            if (string.IsNullOrWhiteSpace(estado))
                return _helpers.CrearRespuestaError("Acción no identificada, recargar la página.");

            var lista = ListarDetalleDocumento(new DOCS1_E { Id = id });
            if (lista == null || lista.Count == 0)
                return _helpers.CrearRespuestaError("No se encontró información del artículo a revertir liberación.");

            var detalle = lista.First();

            if (detalle.Transferido == 1)
                return _helpers.CrearRespuestaError("Para revertir la liberación de este artículo, primero debe revertir la transferencia.");

            if (detalle.Liberado == 0)
                return _helpers.CrearRespuestaError("Para revertir la liberación de este artículo, primero debe estar en estado LIBERADO.");

            var solicitudes = new SolicitudesReversion_DOCS1_N().ListarSolicitudesReversion(new SolicitudesReversion_DOCS1_E { ODOCSId = id });
            if (solicitudes != null && solicitudes.Any() && solicitudes.First().Estado == "Aprobada")
                return _helpers.CrearRespuestaError("La solicitud ya fue APROBADA.");

            if (solicitudes != null && solicitudes.Any() && solicitudes.First().Estado == "Rechazada")
                return _helpers.CrearRespuestaError("La solicitud ya fue RECHAZADA.");

            // Diccionario que mapea los códigos de estado internos a sus descripciones legibles.
            // "A" representa una solicitud Aprobada, y "R" representa una solicitud Rechazada.
            Dictionary<string, string> estadosDisponibles = new Dictionary<string, string> { { "A", "Aprobada" }, { "R", "Rechazada" } };

            return _datos.RevertirLiberacionArticulo(id, estadosDisponibles[estado], usuarioRegistro);
        }

        public List<DOCS1_E> ListarDetalleDocumento(DOCS1_E filtros = null, Dictionary<string, object> parametros = null, bool traerTodos = false)
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
            }

            return _datos.ListarDetalleDocumento(condicion?.ToString(), parametros, traerTodos);
        }

        public Helper_E EditarItemDetalleDoc(DOCS1_E detalle, string usuarioRegistro)
        {
            var lista = ListarDetalleDocumento(new DOCS1_E { Id = detalle.Id });

            if (lista != null && lista.Any())
            {
                detalle.DescargarArchivoProtocolo = lista.First().DescargarArchivoProtocolo;
                detalle.DescargarArchivoET = lista.First().DescargarArchivoET;
                detalle.DescargarArchivoRS = lista.First().DescargarArchivoRS;
            }

            return ValidarParaEdicion(detalle) ?? _datos.EditarItemDetalleDoc(detalle, usuarioRegistro);
        }

        public Helper_E ValidarParaRegistro(List<DOCS1_E> detalle)
        {
            if (detalle == null || !detalle.Any())
                return _helpers.CrearRespuestaError("El detalle del documento está vacío o no existe.");

            int index = 1;
            var indicadorFila = $" #{index}";

            foreach (var item in detalle)
            {
                if (string.IsNullOrWhiteSpace(item.ItemCode))
                    return _helpers.CrearRespuestaError($"Código de artículo no válido en ítem{indicadorFila}");

                if (string.IsNullOrWhiteSpace(item.ItemName))
                    return _helpers.CrearRespuestaError($"Descripción de artículo no válida en ítem{indicadorFila}");

                if (string.IsNullOrWhiteSpace(item.Lote))
                    return _helpers.CrearRespuestaError($"Lote no válido en ítem{indicadorFila}");

                if (string.IsNullOrWhiteSpace(item.FechaVencimiento))
                    return _helpers.CrearRespuestaError($"Fecha de vencimiento no válida en ítem{indicadorFila}");

                //if (string.IsNullOrWhiteSpace(item.RegistroSanitario))
                //    return _helpers.CrearRespuestaError($"Registro sanitario no válido en ítem{indicadorFila}");

                if (string.IsNullOrWhiteSpace(item.Fabricante))
                    return _helpers.CrearRespuestaError($"Fabricante no válido en ítem{indicadorFila}");

                if (string.IsNullOrWhiteSpace(item.CondicionAlmTrans))
                    return _helpers.CrearRespuestaError($"Condición de almacenamiento y transporte no válido en ítem{indicadorFila}");

                if (string.IsNullOrWhiteSpace(item.Almacen))
                    return _helpers.CrearRespuestaError($"Almacen no válido en ítem{indicadorFila}");

                if (item.CantidadAprobados <= 0 &&
                item.CantidadBaja <= 0 &&
                item.CantidadDevolucion <= 0 &&
                item.CantidadFaltante <= 0)
                {
                    return _helpers.CrearRespuestaError($"Debe ingresar al menos una cantidad mayor a 0 en ítem{indicadorFila}");
                }
                if (item.CantidadAprobados > 0 || item.CantidadBaja > 0 || item.CantidadDevolucion > 0 || item.CantidadFaltante > 0)
                    if (!EsCantidadValida(item.CantidadAprobados, item.CantidadBaja, item.CantidadDevolucion, item.CantidadFaltante, item.CantidadTotal))
                        return _helpers.CrearRespuestaError($"Las cantidades ingresadas no suman la cantidad total en ítem{indicadorFila}");
                
                index++;
            }

            return null;
        }

        public Helper_E ValidarParaEdicion(DOCS1_E detalle)
        {
            if (string.IsNullOrWhiteSpace(detalle.CertificadoAnalisis))
                return _helpers.CrearRespuestaError($"El certificado de análisis es obligatorio");

            if (detalle.ArchivoET == null && detalle.ArchivoProtocolo == null && detalle.ArchivoRS == null
                && detalle.DescargarArchivoET == null && detalle.DescargarArchivoProtocolo == null && detalle.DescargarArchivoRS == null)
                return _helpers.CrearRespuestaError($"Debe cargar un Protocolo, E.T. y/o R.S.");

            if (detalle.CantidadAprobados <= 0 && detalle.CantidadBaja <= 0 && detalle.CantidadDevolucion <= 0 && detalle.CantidadFaltante <= 0  )
                return _helpers.CrearRespuestaError("Debe ingresar las cantidades para Aprobados, Bajas y/o Devolución.");

            if (detalle.CantidadAprobados > 0 || detalle.CantidadBaja > 0 || detalle.CantidadDevolucion > 0 || detalle.CantidadFaltante > 0)
                if (!EsCantidadValida(detalle.CantidadAprobados, detalle.CantidadBaja, detalle.CantidadDevolucion, detalle.CantidadFaltante, detalle.CantidadTotal))
                    return _helpers.CrearRespuestaError($"Las cantidades ingresadas no suman la cantidad total.");

            return null;
        }

        public Helper_E ValidarParaLiberacion(DOCS1_E detalle)
        {
            bool certificadoInvalido = string.IsNullOrWhiteSpace(detalle.CertificadoAnalisis);
            bool archivosFaltantes = detalle.DescargarArchivoET == null && detalle.DescargarArchivoProtocolo == null && detalle.DescargarArchivoRS == null;
            bool cantidadesCero = detalle.CantidadAprobados <= 0 && detalle.CantidadBaja <= 0 && detalle.CantidadDevolucion <= 0 && detalle.CantidadFaltante <= 0;
            bool cantidadesInvalidas =
                (detalle.CantidadAprobados > 0 || detalle.CantidadBaja > 0 || detalle.CantidadDevolucion > 0 || detalle.CantidadFaltante > 0) &&
                !EsCantidadValida(detalle.CantidadAprobados, detalle.CantidadBaja, detalle.CantidadDevolucion, detalle.CantidadFaltante, detalle.CantidadTotal);

            if (certificadoInvalido || archivosFaltantes || cantidadesCero || cantidadesInvalidas)
                return _helpers.CrearRespuestaError("Para liberar el artículo, es necesario completar todos los campos requeridos.");

            return null;
        }

        private bool EsCantidadValida(int cantidadAprobados, int cantidadBaja, int cantidadDevolucion, int cantidadFaltante, int cantidadTotal)
        {
            var bandera = false;
            var calculo = cantidadAprobados + cantidadBaja + cantidadDevolucion + cantidadFaltante;

            if (calculo == cantidadTotal)
                bandera = true;

            return bandera;
        }
    }
}