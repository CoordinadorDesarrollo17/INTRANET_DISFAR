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

        public StockMinProductos_E Obtener(string itemCode)
        {
            return _datos.Obtener(itemCode);
         }
        public Helper_E ActualizarStocksMinimos(StockMinProductos_E form)
        {
            var errores = new List<string>();

            if (form == null)
                errores.Add("Verificar datos enviados.");

            if (string.IsNullOrWhiteSpace(form.ItemCode))
                errores.Add("Código de artículo no válido.");

            if (string.IsNullOrWhiteSpace(form.ItemName))
                errores.Add("Descripción no válida.");

            if (form.StockMinAbastecimiento <= 0)
                errores.Add("El Stock Mínimo de Abastecimiento debe ser mayor a 0.");
            
            if (form.StockMinVenta <= 0)
                errores.Add("El Stock Mínimo de Venta debe ser mayor a 0.");

            if (errores.Any())
                return new Helper_E { Mensajes = errores, IconoSweetAlert = "error" };

            return _datos.ActualizarStocksMinimos(form);
        }
    }
}
