using Capa_Datos.Rutas_DAO.TablasSql;
using Capa_Entidad.Rutas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Rutas_NEG.TablasSql
{
    public class RRU11_N
    {
        RRU11_D rru11D = new RRU11_D(); 

        public List<RRU11_E> BuscarRRU11(int docEntry, int linea)
        {
            return rru11D.ListarRRU11(docEntry, linea);
        }

        public void EditarDetalleOrdenRuta(int BaseEntry, int BaseLinea, int[] DetRRU11)
        {
            rru11D.EditarDetalleOrdenRuta(BaseEntry, BaseLinea, DetRRU11);
        }

    }
}
