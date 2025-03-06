using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class KardexAbastecimiento_N
    {
        KardexAbastecimiento_D kardexD = new KardexAbastecimiento_D();
        public Helper_E InsertarTransaccionIngresoKardex(TransferenciaReserva_E ingreso,SqlConnection cn)
        {
            return kardexD.InsertarTransaccionIngresoKardex(ingreso,cn);
        }
        //public Helper_E ValidarTransaccionImputadoKardex(Requerimientos_E imputado, SqlConnection cn)
        //{
        //    return kardexD.ValidarTransaccionImputadoKardex(imputado, cn);
        //}
        public Helper_E InsertarTransaccionImputadoKardex(Requerimientos_E imputado, SqlConnection cn)
        {
            return kardexD.InsertarTransaccionImputadoKardex(imputado, cn);
        }
        public Helper_E InsertarTransaccionSalidaKardex(string itemCode, string itemName, int cantidad, string operarioRegistra, int requerimientoId, SqlConnection cn)
        {
            return kardexD.InsertarTransaccionSalidaKardex(itemCode, itemName, cantidad, operarioRegistra, requerimientoId, cn);
        }
        public Helper_E EliminarTotalTransaccionesIngresoKardex(int docNum, SqlConnection cn)
        {
            return kardexD.EliminarTotalTransaccionesIngresoKardex(docNum, cn);
        }
        public Helper_E EliminarPorItemCodeTransaccionIngresoKardex(int docNum, string itemCode, SqlConnection cn)
        {
            return kardexD.EliminarPorItemCodeTransaccionIngresoKardex(docNum, itemCode,cn);
        }
        

    }
}
