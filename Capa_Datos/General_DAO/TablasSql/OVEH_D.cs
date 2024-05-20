using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos.General_DAO.TablasSql
{
    public class OVEH_D
    {
        Utilitarios uti = new Utilitarios();
        public OVEH_E buscaVehiculo(string Id)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            OVEH_E o = new OVEH_E();
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("Select * from al.OVEH where Code='" + Id + "'", cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                if (!dr.IsDBNull(0)) { o.Code = dr.GetString(0); }
                if (!dr.IsDBNull(1)) { o.Name = dr.GetString(1); }
                if (!dr.IsDBNull(2)) { o.U_SYP_VEMA = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { o.U_SYP_VEMO = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { o.U_SYP_VEPL = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { o.SerieT1 = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { o.SerieT2 = dr.GetString(6); }
                dr.Close(); cn.Close();
            }
            catch { cn.Close(); }
            return o;
        }
        public List<OVEH_E> listaVeh(int Top, OVEH_E filtro)
        {
            List<OVEH_E> lista = new List<OVEH_E>();
            string CadTop = "";
            string fil = "";
            if (Top > 0) { CadTop = "top " + Top; }
            if (filtro != null)
            {
                if (filtro.Code != null) { fil += " and Code like '%" + filtro.Code + "%'"; }
                if (filtro.Name != null) { fil += " and Name like '%" + filtro.Name + "%'"; }
                if (filtro.U_SYP_VEMA != null) { fil += " and U_SYP_VEMA like '%" + filtro.U_SYP_VEMA + "%'"; }
                if (filtro.U_SYP_VEMO != null) { fil += " and U_SYP_VEMO like '%" + filtro.U_SYP_VEMO + "%'"; }
                if (filtro.U_SYP_VEPL != null) { fil += " and U_SYP_VEPL like '%" + filtro.U_SYP_VEPL + "%'"; }
                if (filtro.SerieT1 != null) { fil += " and SerieT1 like '%" + filtro.SerieT1 + "%'"; }
                if (filtro.SerieT2 != null) { fil += " and SerieT2 like '%" + filtro.SerieT2 + "%'"; }
            }
            string query = "select " + CadTop + " * from al.OVEH where Code is not null " + fil + " ORDER BY U_SYP_VEPL";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    OVEH_E o = new OVEH_E();
                    if (!dr.IsDBNull(0)) { o.Code = dr.GetString(0); }
                    if (!dr.IsDBNull(1)) { o.Name = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.U_SYP_VEMA = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.U_SYP_VEMO = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.U_SYP_VEPL = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { o.SerieT1 = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { o.SerieT2 = dr.GetString(6); }
                    lista.Add(o);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public void registrarVeh(OVEH_E o)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OVEH", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "A");
                    cmd.Parameters.AddWithValue("@Code", o.Code).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Name", o.Name);
                    cmd.Parameters.AddWithValue("@U_SYP_VEMA", o.U_SYP_VEMA);
                    cmd.Parameters.AddWithValue("@U_SYP_VEMO", o.U_SYP_VEMO);
                    cmd.Parameters.AddWithValue("@U_SYP_VEPL", o.U_SYP_VEPL);
                    cmd.Parameters.AddWithValue("@SerieT1", o.SerieT1);
                    cmd.Parameters.AddWithValue("@SerieT2", o.SerieT2);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (SqlException e) { tran.Rollback(); cn.Close(); throw new Exception(e.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error : " + e2.Message); }
        }
        public void eliminarVeh(string Code)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OVEH", cn);
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