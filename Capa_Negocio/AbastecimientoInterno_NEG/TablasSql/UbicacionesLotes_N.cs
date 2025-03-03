using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class UbicacionesLotes_N
    {
        readonly UbicacionesLotes_D datosUbicacionesL = new UbicacionesLotes_D();
        public List<UbicacionesLotes_E> Obtener(string itemCode, string batchNum = null)
        {
            return datosUbicacionesL.Obtener(itemCode, batchNum);
        }
        public Helper_E Ingreso(DetalleTransferenciaReserva_E ingreso, SqlConnection cn)
        {
            return datosUbicacionesL.Ingreso(ingreso, cn);
        }
        public Helper_E RevertirIngreso(TransferenciaReserva_E ingreso, SqlConnection cn)
        {
            return datosUbicacionesL.RevertirIngreso(ingreso, cn);
        }
        public Helper_E Salida(List<DetalleRequerimientos_E> salida, SqlConnection cn)
        {
            return datosUbicacionesL.Salida(salida, cn);
        }
    }
}
