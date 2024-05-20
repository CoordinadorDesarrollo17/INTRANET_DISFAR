using Capa_Entidad.General_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sap.Data.Hana;

namespace Capa_Datos.General_DAO.Tablas
{
    public class COB_SALDO_D
    {
        Utilitarios uti = new Utilitarios();DBHelper db = new DBHelper();
        public List<COB_SALDO_E> listarSaldosAnteriores(COB_SALDO_E fil)
        {
            List<COB_SALDO_E> lista = new List<COB_SALDO_E>();
            string filtros = "";
            if(fil!=null)
            {
                if (fil.Code != null && fil.Code!="") { filtros += " and \"Code\"='" + fil.Code + "'"; }
                if (fil.Name != null && fil.Name!="") { filtros += " and UPPER(\"Name\") like UPPER('%" + fil.Name + "%')"; }
                if(fil.U_COB_DESCRIPCION!=null && fil.U_COB_DESCRIPCION != "") { filtros += " and UPPER(\"U_COB_DESCRIPCION\") like UPPER('%" + fil.U_COB_DESCRIPCION+"%')"; }
                if(fil.U_COB_TIPOCONT!=null && fil.U_COB_TIPOCONT != "") { filtros += " and UPPER(\"U_COB_TIPOCONT\") = UPPER('"+fil.U_COB_TIPOCONT+"')"; }
                if (fil.U_COB_TRIMESTRE > 0) { filtros += " and \"U_COB_TRIMESTRE\"=" + fil.U_COB_TRIMESTRE; }
                if (fil.U_COB_SALDOANTE > 0.00M) { filtros += " and \"U_COB_SALDOANTE\" like '%"+fil.U_COB_SALDOANTE+"%'"; }
            }
            string query = "select top 500 * from " + uti.schemaHana + "\"@COB_SALDO\" where \"Code\" is not null " + filtros + " order by \"U_COB_DESCRIPCION\"";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while(hdr.Read())
                {
                    COB_SALDO_E c = new COB_SALDO_E();
                    if (!hdr.IsDBNull(0)) { c.Code = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { c.Name = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { c.U_COB_DESCRIPCION = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { c.U_COB_TIPOCONT = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { c.U_COB_TRIMESTRE = hdr.GetInt32(4); }
                    if (!hdr.IsDBNull(5)) { c.U_COB_SALDOANTE = Math.Round(hdr.GetDecimal(5),0); }
                    lista.Add(c);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public int agregarSaldoAnterior(COB_SALDO_E c)
        {
            int status = 0;
            string query = "insert into "+uti.schemaHana+"\"@COB_SALDO\" " +
                "values('"+generarSigCodeSaldoAnt()+"','"+c.Name+"','"+c.U_COB_DESCRIPCION+"','"+c.U_COB_TIPOCONT+"',"+c.U_COB_TRIMESTRE+","+c.U_COB_SALDOANTE+")";
            try
            {
                db.HanaExecuteNonQueryTrx(query);
                status = 1;
            }catch { status = -1; throw new Exception("El codigo producto ya existe"); }
            return status;
        }
        public int editarSaldoAnterior(COB_SALDO_E c)
        {
            int status = 0;
            string query = "update "+uti.schemaHana+"\"@COB_SALDO\" " +
                " set \"U_COB_TIPOCONT\"='"+c.U_COB_TIPOCONT+"',\"U_COB_TRIMESTRE\"="+c.U_COB_TRIMESTRE+
                " ,\"U_COB_SALDOANTE\"="+c.U_COB_SALDOANTE +
                " where \"Code\"="+c.Code;
            try
            {
                db.HanaExecuteNonQueryTrx(query) ;
            }
            catch (Exception e){ status = -1; throw new Exception(e.Message); }
            return status;
        }
        public COB_SALDO_E buscarSaldoAnterior(string Code)
        {
            COB_SALDO_E c = null;
            string query = "select * from "+uti.schemaHana+"\"@COB_SALDO\" where \"Code\"='" + Code + "'";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                hdr.Read();
                    c = new COB_SALDO_E();
                    c.Code = hdr.GetString(0);
                    if (!hdr.IsDBNull(1)) { c.Name = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { c.U_COB_DESCRIPCION = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { c.U_COB_TIPOCONT = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { c.U_COB_TRIMESTRE = hdr.GetInt32(4); }
                    if (!hdr.IsDBNull(5)) { c.U_COB_SALDOANTE = hdr.GetDecimal(5); }
                hdr.Close();
            }
            catch { }
            return c;
        }
        private string generarSigCodeSaldoAnt()
        {
            string code = null;
            string query = "select ifnull(max(to_integer(\"Code\"))+1,1) from "+uti.schemaHana+"\"@COB_SALDO\"";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                hdr.Read();
                if (!hdr.IsDBNull(0)) { code = hdr.GetString(0); }
                hdr.Close();
            }
            catch { }
            return code;
        }
        public int eliminarSaldoAnterior(string Code)
        {
            int status = 0;
            string query = "delete from "+uti.schemaHana+"\"@COB_SALDO\" where \"Code\"='"+Code+"'";
            try
            {
                db.HanaExecuteNonQueryTrx(query);
            }catch(Exception e) { status = -1; throw new Exception(e.Message); }
            return status;
        }
    }
}
