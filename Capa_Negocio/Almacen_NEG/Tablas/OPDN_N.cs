using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Almacen_NEG.Tablas
{
    public class OPDN_N
    {
        OPDN_D Datos = new OPDN_D();
        public List<OPDN_E> Listar(OPDN_E filtro)
        {
            return Datos.Listar(filtro);
        }
        }
}
