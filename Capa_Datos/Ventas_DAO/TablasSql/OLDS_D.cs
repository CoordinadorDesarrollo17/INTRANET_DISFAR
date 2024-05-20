using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System.Data.SqlClient;
using System.Data;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class OLDS_D
    {
        DBHelper db = new DBHelper();
        public List<OLDS_E> listarLibrosSaldo(OLDS_E li)
        {
            List<OLDS_E> lista = new List<OLDS_E>();
            string query;
            if (li == null) { query = "select top 100 CardCode from vt.OLDS order by CardName"; }
            else
            {
                query = "select top 100 CardCode from vt.OLDS where CardCode is not null ";
                if (li.CardName != null) { query += " and CardName like '%" + li.CardName + "%'"; }
                if (li.NroOpe > 0) { query += " and NroOpe >=" + li.NroOpe; }
                if (li.SaldoActual > 0) { query += " and SaldoActual > 0.00"; }
                if (li.SaldoActual < 0) { query += " and SaldoActual < 0.00"; }
                query += " order by CardName";
            }
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    OLDS_E l = obtenerLibroSaldo(dr.GetString(0));
                    lista.Add(l);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
        public OLDS_E obtenerLibroSaldo(string CardCode)
        {
            OLDS_E l = new OLDS_E();
            string query = "select CardCode,CardName,NroOpe,SaldoActual from vt.OLDS where CardCode=@CardCode";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@CardCode" }, CardCode);
                dr.Read();
                l.CardCode = dr.GetString(0);
                l.CardName = dr.GetString(1);
                if (!dr.IsDBNull(2)) { l.NroOpe = dr.GetInt32(2); }
                if (!dr.IsDBNull(3)) { l.SaldoActual = dr.GetDecimal(3); }
                l.Det = obtenerDetLibroSaldo(l.CardCode);
                dr.Close();
            }
            catch { }
            return l;
        }
        public List<LDS1_E> obtenerDetLibroSaldo(string CardCode)
        {
            List<LDS1_E> lista = new List<LDS1_E>();
            string query = "select CardCode,Linea,FechaOpe,Operacion,DetOpe,Ingreso,Egreso,Saldo,FechaRegistro,HoraRegistro,OpRegistro from vt.LDS1 where CardCode=@CardCode order by Linea";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string> { "@CardCode" }, CardCode);
                while (dr.Read())
                {
                    LDS1_E d = new LDS1_E();
                    d.CardCode = CardCode;
                    d.Linea = dr.GetInt32(1);
                    if (!dr.IsDBNull(2)) { d.FechaOpe = dr.GetDateTime(2).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(3)) { d.Operacion = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { d.DetOpe = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { d.Ingreso = dr.GetDecimal(5); }
                    if (!dr.IsDBNull(6)) { d.Egreso = dr.GetDecimal(6); }
                    if (!dr.IsDBNull(7)) { d.Saldo = dr.GetDecimal(7); }
                    if (!dr.IsDBNull(8)) { d.FechaReg = dr.GetDateTime(8).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(9)) { d.HoraReg = dr.GetTimeSpan(9).ToString(); }
                    if (!dr.IsDBNull(10)) { d.OperarioReg = dr.GetString(10); }
                    lista.Add(d);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
        public int crearLibroSaldo(OLDS_E l)
        {
            int status = 0;
            try
            {
                db.ExecuteNonQuery("vt.MANT_OLDS", "AC", l.CardCode, l.CardName, 0, 0.00M);
                status = 1;
            }
            catch { status = -1; }
            return status;
        }
        // detalles
        public int agregarDetLibroSaldo(LDS1_E d)
        {
            int status = 0;
            try
            {
                db.ExecuteNonQuery("vt.MANT_OLDS", "AD", d.CardCode, null, null, null, d.Linea,
                    d.FechaOpe, d.Operacion, d.DetOpe, d.Ingreso, d.Egreso, null, null,null, d.OperarioReg);
                status = 1;
            }
            catch (Exception e) { status = -1; throw new Exception(e.Message); }
            return status;
        }
        public void agregarLDS1(LDS1_E o, SqlTransaction tran, SqlConnection cn)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("vt.MANT_OLDS", cn, tran);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "AD");
                cmd.Parameters.AddWithValue("@C_CardCode", o.CardCode);
                cmd.Parameters.AddWithValue("@FechaOpe", o.FechaOpe);
                cmd.Parameters.AddWithValue("@Operacion", o.Operacion);
                cmd.Parameters.AddWithValue("@DetOpe", o.DetOpe);
                cmd.Parameters.AddWithValue("@Ingreso", o.Ingreso);
                cmd.Parameters.AddWithValue("@Egreso", o.Egreso);
                cmd.Parameters.AddWithValue("@OperarioRegistro", o.OperarioReg);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception(e.Message); }
        }
    }
}