using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.TablasSql;
using Capa_Entidad;
using Capa_Datos.DireccionTecnica_DAO.TablasSql;
using System.Reflection;

namespace Capa_Negocio.DireccionTecnica_NEG.TablasSql
{
    public class DOCS1_N
    {
        private readonly DOCS1_D _datos = new DOCS1_D();
        private readonly Helpers _helpers = new Helpers();

        public Helper_E LiberarArticulos(List<int> grupoIds, string usuarioRegistro)
        {
            List<Helper_E> lista = new List<Helper_E>();
            Helper_E resultado = new Helper_E { Titulo = "Acción completada", Mensajes = new List<string> { "Artículo liberado correctamente." }, Icono = "success" };

            foreach (var id in grupoIds)
            {
                var detalle = ListarDetalleDocumento(new DOCS1_E { Id = id });

                // Solo vamos a liberar a los artículos que se encuentran pendientes a liberar
                if (detalle != null && detalle.Any() && detalle.First().Liberado == 0 && detalle.First().Transferido == 0)
                    lista.Add(_datos.LiberarArticulo(id, usuarioRegistro));
            }

            if (!lista.Any())
            {
                resultado.Titulo = "Error";
                resultado.Mensajes.Clear();
                resultado.Mensajes.Add("Todos los artículos del detalle ya se encuentran liberados.");
                resultado.Icono = "error";
            }

            // Al menos un error en la lista
            if (lista.Any(l => l.Icono == "error"))
            {
                resultado.Titulo = "Error";
                resultado.Mensajes.Clear();
                resultado.Mensajes.Add("Ocurrió un error al liberar artículo.");
                resultado.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                resultado.Icono = "error";
            }

            return resultado;
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
            if (string.IsNullOrWhiteSpace(detalle.CertificadoAnalisis))
                return _helpers.CrearRespuestaError($"El certificado de análisis es obligatorio");

            if (detalle.CantidadAprobados <= 0 && detalle.CantidadBaja <= 0 && detalle.CantidadDevolucion <= 0)
                return _helpers.CrearRespuestaError("Debe ingresar las cantidades para Aprobados, Bajas y/o Devolución.");

            if (detalle.CantidadAprobados > 0 || detalle.CantidadBaja > 0 || detalle.CantidadDevolucion > 0)
                if (!EsCantidadValida(detalle.CantidadAprobados, detalle.CantidadBaja, detalle.CantidadDevolucion, detalle.CantidadTotal))
                    return _helpers.CrearRespuestaError($"Las cantidades ingresadas no suman la cantidad total.");

            return _datos.EditarItemDetalleDoc(detalle, usuarioRegistro);
        }

        public Helper_E ValidarDetalleDocumento(List<DOCS1_E> detalle)
        {
            if (detalle == null || !detalle.Any())
                return _helpers.CrearRespuestaError("El detalle del documento está vacío o no existe.");

            int index = 1;
            var indicadorFila = string.Empty;

            if (detalle.Count == 1)
                indicadorFila = $" #{index}";

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

                if (string.IsNullOrWhiteSpace(item.RegistroSanitario))
                    return _helpers.CrearRespuestaError($"Registro sanitario no válido en ítem{indicadorFila}");

                if (string.IsNullOrWhiteSpace(item.Fabricante))
                    return _helpers.CrearRespuestaError($"Fabricante no válido en ítem{indicadorFila}");

                if (string.IsNullOrWhiteSpace(item.CondicionAlmTrans))
                    return _helpers.CrearRespuestaError($"Condición de almacenamiento y transporte no válido en ítem{indicadorFila}");

                if (string.IsNullOrWhiteSpace(item.Almacen))
                    return _helpers.CrearRespuestaError($"Almacen no válido en ítem{indicadorFila}");

                if (item.ArchivoET == null && item.ArchivoProtocolo == null)
                    return _helpers.CrearRespuestaError($"Debe cargar un Protocolo y/o ET en el ítem{indicadorFila}");

                if (item.CantidadAprobados > 0 || item.CantidadBaja > 0 || item.CantidadDevolucion > 0)
                    if (!EsCantidadValida(item.CantidadAprobados, item.CantidadBaja, item.CantidadDevolucion, item.CantidadTotal))
                        return _helpers.CrearRespuestaError($"Las cantidades ingresadas no suman la cantidad total en ítem{indicadorFila}");

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
