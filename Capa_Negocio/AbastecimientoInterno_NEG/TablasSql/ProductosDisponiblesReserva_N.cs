using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class ProductosDisponiblesReserva_N
    {
        ProductosDisponiblesReserva_D _datos = new ProductosDisponiblesReserva_D();
        public List<ProductosDisponiblesReserva_E> ObtenerProductosDisponiblesReserva()
        {
            return _datos.ObtenerProductosDisponiblesReserva();
        }
        }
}
