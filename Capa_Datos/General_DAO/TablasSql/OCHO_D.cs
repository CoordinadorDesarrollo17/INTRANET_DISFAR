using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos.General_DAO.TablasSql
{
    public class OCHO_D
    {
        Utilitarios uti = new Utilitarios();
        public OCHO_E buscaChofer(string Id)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            OCHO_E o = new OCHO_E();
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("Select * from al.OCHO where Code='" + Id + "'", cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();

                if (!dr.IsDBNull(0)) { o.Code = dr.GetString(0); }
                if (!dr.IsDBNull(1)) { o.Name = dr.GetString(1); }
                if (!dr.IsDBNull(2)) { o.U_SYP_CHLI = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { o.U_SYP_CHNO = dr.GetString(3); }
                dr.Close(); cn.Close();
            }
            catch { cn.Close(); }
            return o;
        }
        public List<OCHO_E> listaChoferes(int Top, OCHO_E filtro)
        {
            List<OCHO_E> lista = new List<OCHO_E>();
            string query = "";
            string fil = "";
            if (filtro != null)
            {
                if (filtro.Code != null) { fil += " and Code like '%" + filtro.Code + "%'"; }
                if (filtro.Name != null) { fil += " and Name like '%" + filtro.Name + "%'"; }
                if (filtro.U_SYP_CHLI != null) { fil += " and U_SYP_CHLI like '%" + filtro.U_SYP_CHLI + "%'"; }
                if (filtro.U_SYP_CHNO != null) { fil += " and U_SYP_CHNO like '%" + filtro.U_SYP_CHNO + "%'"; }
            }
            if (Top > 0)
            {
                query = "select top " + Top;
            }
            else
            {
                query = "select ";
            }
            query += $" * FROM al.ocho WHERE Code is not null {fil} ORDER BY U_SYP_CHNO";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    OCHO_E o = new OCHO_E();
                    if (!dr.IsDBNull(0)) { o.Code = dr.GetString(0); }
                    if (!dr.IsDBNull(1)) { o.Name = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.U_SYP_CHLI = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.U_SYP_CHNO = dr.GetString(3); }
                    lista.Add(o);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public void registrarChofer(OCHO_E o)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OCHO", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "A");
                    cmd.Parameters.AddWithValue("@Code", o.Code).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Name", o.Name);
                    cmd.Parameters.AddWithValue("@U_SYP_CHLI", o.U_SYP_CHLI);
                    cmd.Parameters.AddWithValue("@U_SYP_CHNO", o.U_SYP_CHNO);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (SqlException e) { tran.Rollback(); cn.Close(); throw new Exception(e.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error : " + e2.Message); }
        }
        public void eliminarChofer(string Code)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OCHO", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "D");
                    cmd.Parameters.AddWithValue("@Code", Code).Direction = ParameterDirection.InputOutput;
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (SqlException e) { tran.Rollback(); cn.Close(); throw new Exception(e.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error : " + e2.Message); }

        }
    }
}