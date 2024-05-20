using Capa_Datos.AtencionCliente_DAO.TablasSql;
using Capa_Entidad.AtencionCliente_ENT.TablasSql;
using System.Collections.Generic;

namespace Capa_Negocio.AtencionCliente_NEG.TablasSql
{
    public class SAT1_N
    {
        SAT1_D sat1D = new SAT1_D();
        public List<SAT1_E> BuscarDetallesSolicitud(int DocEntry)
        {
            return sat1D.buscarDetallesSolicitud(DocEntry);
        }

        public List<SAT1_E> ListarArticulosTicket(int docNumTicket)
        {
            return sat1D.listarArticulosTicket(docNumTicket);
        }

        public List<SAT1_E> BuscarCodProductosTicket(int docNumTicket)
        {
            return sat1D.BuscarCodProductosTicket(docNumTicket);
        }

    }
}
