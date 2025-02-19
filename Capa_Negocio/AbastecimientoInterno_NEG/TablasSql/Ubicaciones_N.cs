using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class Ubicaciones_N
    {
        Ubicaciones_D _datos = new Ubicaciones_D();

        public string[] BuscarUbicaciones(string almacen, string itemCode)
        {
            return _datos.BuscarUbicaciones(almacen, itemCode);
        }

        public Helper_E EliminarUbicacionGeneral(string codigoUbicacion)
        {
            codigoUbicacion = codigoUbicacion.Trim();
            codigoUbicacion = codigoUbicacion.Replace("'", "''");

            return _datos.EliminarUbicacionGeneral(codigoUbicacion);
        }
    }
}
