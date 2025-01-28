using Capa_Datos.General_DAO.TablasSql;
using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.TablasSql
{
    public class UBICACIONES_N
    {
        UBICACIONES_D ubicacionesD = new UBICACIONES_D();
        public string[] BuscarUbicaciones(string itemCode, string lote, string almProcedencia)
        {
            return ubicacionesD.BuscarUbicaciones(itemCode, lote, almProcedencia);
        }
    }
}
