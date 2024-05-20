using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class USR1_D
    {
        //cuotas
        Utilitarios uti = new Utilitarios();
        public List<USR1_E> listarVenUltCuotas(USR1_E fil)
        {
            List<USR1_E> lista = new List<USR1_E>();
            string filtro = "";
            if (fil != null)
            {
                if (fil.Nombres != null) { filtro += $" AND CONCAT(T0.Nombres,' ', T0.Apellidos) LIKE '%{fil.Nombres}%'"; }
            }
            string query = "select top 50 CONCAT(T0.Prefijo,'',T0.Id) AS Usuario, T0.DocEntry, CONCAT(T0.Nombres,' ', T0.Apellidos),MAX(T1.YearU),MAX(T1.MonthU),Max(T1.Cuota),Max(T1.FechaRegistro),Max(T1.OpRegistro)  from ousr T0 left join usr1 T1 on T1.DocEntry = T0.DocEntry" +
                " where T0.DocEntry is not null and T0.idRol in(6, 7) " + filtro +
				" group by CONCAT(T0.Prefijo,'',T0.Id), T0.DocEntry, CONCAT(T0.Nombres,' ', T0.Apellidos) order by CONCAT(T0.Nombres,' ', T0.Apellidos)";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    USR1_E obj = new USR1_E();
                    if (!dr.IsDBNull(0)) { obj.Usuario = dr.GetString(0); }
                    if (!dr.IsDBNull(1)) { obj.DocEntry = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { obj.Nombres = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { obj.YearU = dr.GetInt32(3); }
                    if (!dr.IsDBNull(4)) { obj.MonthU = dr.GetInt32(4); }
                    if (!dr.IsDBNull(5)) { obj.Cuota = dr.GetDecimal(5); }
                    if (!dr.IsDBNull(6)) { obj.FechaRegistro = dr.GetDateTime(6).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(7)) { obj.OpRegistro = dr.GetString(7); }
                    lista.Add(obj);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public List<USR1_E> listarUsrCuotas(int DocEntry)
        {
            List<USR1_E> lista = new List<USR1_E>();
            string query = "select top 50 * from usr1 where DocEntry=@DocEntry order by FechaRegistro desc";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    USR1_E obj = new USR1_E();
                    if (!dr.IsDBNull(0)) { obj.DocEntry= dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { obj.YearU = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { obj.MonthU = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { obj.Cuota = dr.GetDecimal(3); }
                    if (!dr.IsDBNull(4)) { obj.FechaRegistro = dr.GetDateTime(4).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(5)) { obj.HoraRegistro = dr.GetTimeSpan(5).ToString(); }
                    if (!dr.IsDBNull(6)) { obj.OpRegistro = dr.GetString(6); }
                    lista.Add(obj);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public void registrarUsr1(USR1_E obj)
        {
            string query = "insert into dbo.usr1 values(@DocEntry,@YearU,@MonthU,@Cuota,@FechaRegistro,@HoraRegistro,@OpRegistro)";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn, tran);
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry);
                    cmd.Parameters.AddWithValue("@YearU", obj.YearU);
                    cmd.Parameters.AddWithValue("@MonthU", obj.MonthU);
                    cmd.Parameters.AddWithValue("@Cuota", obj.Cuota);
                    cmd.Parameters.AddWithValue("@FechaRegistro", DateTime.Now.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@HoraRegistro", DateTime.Now.ToString("HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@OpRegistro", obj.OpRegistro);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    cn.Close();
                }
                catch(Exception e1) { tran.Rollback(); throw new Exception(uti.msjError(e1.HResult)); }
            }
            catch(Exception e2) { cn.Close(); throw new Exception(e2.Message); }
        }
        public void borrarUsr1(USR1_E obj)
        {
            string query = "delete from usr1 where DocEntry=@DocEntry and YearU=@YearU and MonthU=@MonthU";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn, tran);
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry);
                    cmd.Parameters.AddWithValue("@YearU", obj.YearU);
                    cmd.Parameters.AddWithValue("@MonthU", obj.MonthU);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    cn.Close();
                }
                catch (Exception e1) { tran.Rollback(); throw new Exception(uti.msjError(e1.HResult)); }
            }
            catch (Exception e2) { cn.Close(); throw new Exception(e2.Message); }
        }
        public USR1_E buscarUsr1(int DocEntry, int YearU,int MonthU)
        {
            USR1_E obj = new USR1_E();
            string query = "select * from usr1 where DocEntry=@DocEntry and YearU=@YearU and MonthU=@MonthU";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@YearU",YearU);
                cmd.Parameters.AddWithValue("@MonthU",MonthU);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                if (!dr.IsDBNull(0)) { obj.DocEntry = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { obj.YearU = dr.GetInt32(1); }
                if (!dr.IsDBNull(2)) { obj.MonthU = dr.GetInt32(2); }
                if (!dr.IsDBNull(3)) { obj.Cuota = dr.GetDecimal(3); }
                if (!dr.IsDBNull(4)) { obj.FechaRegistro = dr.GetDateTime(4).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(5)) { obj.HoraRegistro = dr.GetTimeSpan(5).ToString(); }
                if (!dr.IsDBNull(6)) { obj.OpRegistro = dr.GetString(6); }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return obj;
        }
    }
}
