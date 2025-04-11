using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.TablasSql;
using Capa_Entidad;
using System.Data.SqlClient;
using Capa_Datos.DireccionTecnica_DAO.TablasSql;

namespace Capa_Negocio.DireccionTecnica_NEG.TablasSql
{
    public class ODOCS_N
    {
        private readonly ODOCS_D _datos = new ODOCS_D();
        private readonly Helpers _helpers = new Helpers();

        public List<ODOCS_E> ListarInternamientos(ODOCS_E filtros = null, Dictionary<string, object> parametros = null)
        {
            StringBuilder condicion = new StringBuilder();

            if (parametros == null)
                parametros = new Dictionary<string, object>();

            if (filtros != null)
            {
                if (filtros.Id > 0)
                {
                    condicion.AppendLine("AND DOC.Id = @Id");
                    parametros["@Id"] = filtros.Id;
                }

                if (filtros.DocNum > 0)
                {
                    condicion.AppendLine("AND DOC.DocNum = @DocNum");
                    parametros["@DocNum"] = filtros.DocNum;
                }

                if (!string.IsNullOrWhiteSpace(filtros.TipoDocumento))
                {
                    condicion.AppendLine("AND DOC.TipoDocumento = @TipoDocumento");
                    parametros["@TipoDocumento"] = filtros.TipoDocumento;
                }

                if (!string.IsNullOrWhiteSpace(filtros.CardCode))
                {
                    condicion.AppendLine("AND DOC.CardCode = @CardCode");
                    parametros["@CardCode"] = filtros.CardCode;
                }

                if (!string.IsNullOrWhiteSpace(filtros.CardName))
                {
                    condicion.AppendLine("AND DOC.CardName = @CardName");
                    parametros["@CardName"] = filtros.CardName;
                }

                if (!string.IsNullOrWhiteSpace(filtros.Guia))
                {
                    condicion.AppendLine("AND DOC.Guia = @Guia");
                    parametros["@Guia"] = filtros.Guia;
                }

                if (!string.IsNullOrWhiteSpace(filtros.ComprobanteVinculado))
                {
                    condicion.AppendLine("AND DOC.ComprobanteVinculado = @ComprobanteVinculado");
                    parametros["@ComprobanteVinculado"] = filtros.ComprobanteVinculado;
                }

                if (!string.IsNullOrWhiteSpace(filtros.Estado))
                {
                    condicion.AppendLine("AND DOC.Estado = @Estado");
                    parametros["@Estado"] = filtros.Estado;
                }
            }

            return _datos.ListarInternamientos(condicion.ToString(), parametros);
        }

        public Helper_E RegistrarDocumento(ODOCS_E datos)
        {
            if (datos == null)
                return _helpers.CrearRespuestaError("Verificar los datos enviados.");

            if(ListarInternamientos(datos).Any())
                return _helpers.CrearRespuestaError("El documento ingresado ya se encuentra registrado.");

            if (string.IsNullOrWhiteSpace(datos.TipoDocumento))
                return _helpers.CrearRespuestaError("Debe seleccionar un tipo de documento");

            if (datos.DocNum <= 0)
                return _helpers.CrearRespuestaError("DocNum no válido.");

            if (string.IsNullOrWhiteSpace(datos.CardCode))
                return _helpers.CrearRespuestaError("Verificar el código de cliente.");

            if (string.IsNullOrWhiteSpace(datos.CardName))
                return _helpers.CrearRespuestaError("Verificar el nombre de cliente.");

            if (string.IsNullOrWhiteSpace(datos.Guia))
                return _helpers.CrearRespuestaError("Guía no válida.");

            if (string.IsNullOrWhiteSpace(datos.ComprobanteVinculado))
                return _helpers.CrearRespuestaError("Comprobante vinculado no válido.");

            if (string.IsNullOrWhiteSpace(datos.FechaContabilizacion))
                return _helpers.CrearRespuestaError("Fecha de contabilizacion no válida.");

            if (string.IsNullOrWhiteSpace(datos.FechaInicioTraslado))
                return _helpers.CrearRespuestaError("Fecha de inicio de traslado no válido.");

            return ValidarDetalleDocumento(datos.Detalle) ??_datos.RegistrarDocumento(datos);
        }

        private Helper_E ValidarDetalleDocumento(List<DOCS1_E> detalle)
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

                if (string.IsNullOrWhiteSpace(item.CertificadoAnalisis))
                    return _helpers.CrearRespuestaError($"Certificado de ánalisis no válido en ítem #{index}");

                if(!EsCantidadValida(item.CantidadAprobados, item.CantidadBaja, item.CantidadDevolucion, item.CantidadTotal))
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