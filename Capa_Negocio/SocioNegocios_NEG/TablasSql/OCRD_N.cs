using Capa_Datos.SocioNegocios_DAO.TablasSql;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.SocioNegocios_NEG.TablasSql
{
    public class OCRD_N
    {
        readonly OCRD_D ocrD = new OCRD_D();
        public List<OCRD_E> BuscarCliente(OCRD_E cliente)
        {
            return ocrD.BuscarCliente(cliente);
        }
    }
}
