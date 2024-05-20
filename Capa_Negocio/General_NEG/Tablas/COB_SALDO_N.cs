using Capa_Datos.General_DAO.Tablas;
using Capa_Entidad.General_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.General_NEG.Tablas
{
    public class COB_SALDO_N
    {
        COB_SALDO_D cD = new COB_SALDO_D();
        public List<COB_SALDO_E> listarSaldosAnteriores(COB_SALDO_E fil)
        {
            return cD.listarSaldosAnteriores(fil);
        }
        public int agregarSaldoAnterior(COB_SALDO_E c)
        {
            if (c.U_COB_TRIMESTRE == 0) { throw new Exception("Debe elejir trimestre"); }
            if (c.U_COB_TIPOCONT == null || c.U_COB_TIPOCONT == "") { throw new Exception("Debe elejir tipo controlado"); }
            if (c.U_COB_DESCRIPCION == null || c.U_COB_DESCRIPCION == "") { throw new Exception("Debe haber una descripcion"); }
            return cD.agregarSaldoAnterior(c);
        }
        public int editarSaldoAnterior(COB_SALDO_E c)
        {
            if (c.U_COB_TRIMESTRE == 0) { throw new Exception("Debe elejir trimestre"); }
            if (c.U_COB_TIPOCONT == null || c.U_COB_TIPOCONT == "") { throw new Exception("Debe elejir tipo controlado"); }
            return cD.editarSaldoAnterior(c);
        }
        public COB_SALDO_E buscarSaldoAnterior(string Code)
        {
            return cD.buscarSaldoAnterior(Code);
        }
        public int eliminarSaldoAnterior(string Code)
        {
            return cD.eliminarSaldoAnterior(Code);
        }
    }
}
