using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
    public class USR1_N
    {
        USR1_D usr1D = new USR1_D();
        public List<USR1_E> listarVenUltCuotas(USR1_E fil)
        {
            return usr1D.listarVenUltCuotas(fil);
        }
        public List<USR1_E> listarUsrCuotas(int DocEntry)
        {
            return usr1D.listarUsrCuotas(DocEntry);
        }
        public void registrarUsr1(USR1_E obj)
        {
            DateTime ahora = DateTime.Now;
            if (obj.YearU < ahora.Year) { throw new Exception("El año no puede ser anterior actual"); }
            //if (obj.MonthU < ahora.Month && obj.MonthU > 12) { throw new Exception("El mes no puede ser anterior actual o ingreso un valor invalido"); }
            if (obj.MonthU <0 && obj.MonthU > 12) { throw new Exception("El mes no puede ser anterior actual o ingreso un valor invalido"); }
            if (obj.Cuota <= 0) { throw new Exception("La cuota no puede ser menor o igual a 0"); }
            usr1D.registrarUsr1(obj);
        }
        public void borrarUsr1(USR1_E obj)
        {
            usr1D.borrarUsr1(obj);
        }
    }
}
