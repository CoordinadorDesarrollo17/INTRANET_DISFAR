using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos.General_DAO.TablasSql
{
    public class COUR_D
    {
        Utilitarios uti = new Utilitarios();
        public COUR_E Obtener(string Nombre)
        {
            string query = "select Id, MinDomicilio, MinAgencia from al.COUR where Nombre=@Nombre";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            COUR_E bean = new COUR_E();
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
                cmd.Parameters.AddWithValue("@Nombre", Nombre);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    if (!dr.IsDBNull(0)) { bean.Id = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { bean.MinDomicilio = dr.GetDecimal(1); }
                    if (!dr.IsDBNull(2)) { bean.MinAgencia = dr.GetDecimal(2); }
                }
                dr.Close();
                cn.Close();
            }

            catch { cn.Close(); }
            return bean;
        }
        public List<COUR_E> Listar()
        {
            List<COUR_E> lista = new List<COUR_E>();
            string query = "select  Id, Ruc, Nombre,MinDomicilio,MinAgencia from al.COUR";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    COUR_E bean = new COUR_E();
                    if (!dr.IsDBNull(0)) { bean.Id = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { bean.Ruc = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { bean.Nombre = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { bean.MinDomicilio = dr.GetDecimal(3); }
                    if (!dr.IsDBNull(4)) { bean.MinAgencia = dr.GetDecimal(4); }
                    lista.Add(bean);
                }
                dr.Close();
                cn.Close();
            }

            catch { cn.Close(); }
            return lista;
        }
        public string Registrar(COUR_E bean)
        {
            string res = string.Empty;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            string query = "insert into al.COUR values(@Ruc,@Nombre,@DireccionFiscal,@MinAgencia,@MinDomicilio)";
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn) { Transaction = tran, CommandType = CommandType.Text };
                    cmd.Parameters.AddWithValue("@Ruc", bean.Ruc);
                    cmd.Parameters.AddWithValue("@Nombre", bean.Nombre);
                    cmd.Parameters.AddWithValue("@DireccionFiscal", bean.DireccionFiscal);
                    cmd.Parameters.AddWithValue("@MinAgencia", bean.MinAgencia);
                    cmd.Parameters.AddWithValue("@MinDomicilio", bean.MinDomicilio);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    res = "Se registro agencia courier";
                }
                catch (SqlException e) { tran.Rollback(); cn.Close(); res = e.Message; }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); res = e2.Message; }
            return res;
        }
    }
}