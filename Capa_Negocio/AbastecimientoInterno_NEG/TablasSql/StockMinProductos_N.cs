using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class StockMinProductos_N
    {
        StockMinProductos_D _datos = new StockMinProductos_D();

        public List<StockMinProductos_E> ListarStockMinProductos(StockMinProductos_E filtros = null, Dictionary<string, object> parametros = null)
        {
            var condicion = new StringBuilder();

            if (parametros == null)
                parametros = new Dictionary<string, object>();

            if (filtros != null)
            {
                if (filtros.Id > 0)
                {
                    condicion.AppendLine("AND SM.Id = @Id");
                    parametros["@Id"] = filtros.Id;
                }

                if (!string.IsNullOrWhiteSpace(filtros.ItemCode))
                {
                    condicion.AppendLine("AND SM.ItemCode = @ItemCode");
                    parametros["@ItemCode"] = filtros.ItemCode;
                }

                if (!string.IsNullOrWhiteSpace(filtros.ItemName))
                {
                    condicion.AppendLine("AND SM.ItemName = @ItemName");
                    parametros["@ItemName"] = filtros.ItemName;
                }
            }

            return _datos.ListarStockMinProductos(condicion.ToString(), parametros);
        }

        public StockMinProductos_E Obtener(string itemCode)
        {
            return _datos.Obtener(itemCode);
        }
        public (List<string>, StockMinProductos_E) ValidarDatos(StockMinProductos_E datos)
        {
            var errores = new List<string>();

            if (datos == null)
                errores.Add("Verificar datos enviados.");

            if (string.IsNullOrWhiteSpace(datos.ItemCode))
                errores.Add("Código de artículo no válido.");

            if (string.IsNullOrWhiteSpace(datos.ItemName))
                errores.Add("Descripción no válida.");

            if (datos.StockMinAbastecimiento <= 0)
                errores.Add("El Stock Mínimo de Abastecimiento debe ser mayor a 0.");

            if (!string.IsNullOrWhiteSpace(datos.Clasificacion))
                datos.Clasificacion = datos.Clasificacion.ToUpper();

            return (errores, datos);
        }

        public Helper_E RegistrarStockMinimo(StockMinProductos_E form)
        {
            var (mensajes, datos) = ValidarDatos(form);

            if (mensajes != null && mensajes.Any())
                return new Helper_E { Mensajes = mensajes, IconoSweetAlert = "error" };

            var datoExistente = ListarStockMinProductos(form);
            if (datoExistente != null && datoExistente.Any()  && datoExistente.First().Id > 0)
            {
                return new Helper_E { Mensajes = new List<string> { "El artículo ya se encuentra registrado." }, IconoSweetAlert = "error" };
            }

            return _datos.RegistrarStockMinimo(datos);
        }

        public Helper_E ActualizarStockMinimo(StockMinProductos_E form)
        {
            var (mensajes, datos) = ValidarDatos(form);

            if (mensajes != null && mensajes.Any())
                return new Helper_E { Mensajes = mensajes, IconoSweetAlert = "error" };

            var datoExistente = ListarStockMinProductos(form);
            if (datoExistente != null && datoExistente.Any() && datoExistente.First().Id <= 0)
            {
                return new Helper_E { Mensajes = new List<string> { "Id no válido. Verificar los datos enviados." }, IconoSweetAlert = "error" };
            }

            return _datos.ActualizarStockMinimo(datos);
        }
    }
}
