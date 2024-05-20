using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos.General_DAO.TablasSql
{
    public class OUR1_D
    {
        Utilitarios uti = new Utilitarios();
        public List<OUR1_E> Listar()
        {
            List<OUR1_E> lista = new List<OUR1_E>();
            string query = "select  t0.Id, t0.Ubigeo, t2.Distrito,t2.Provincia,t2.Departamento,t0.Calle, ( select Nombre from al.COUR where Id=t0.IdCourier) AS 'NombreAgencia' from al.OUR1 t0 inner join UBIG t2 on t0.Ubigeo=t2.Ubigeo";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    OUR1_E bean = new OUR1_E();
                    if (!dr.IsDBNull(0)) { bean.Id = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { bean.Ubigeo = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { bean.Distrito = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { bean.Provincia = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { bean.Departamento = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { bean.Calle = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { bean.NombreAgencia = dr.GetString(6); }
                    lista.Add(bean);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public void Registrar(OUR1_E bean)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            string query = "insert into al.OUR1 values(@IdCourier,@Ubigeo,@Calle)";
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("");
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn) { Transaction = tran, CommandType = CommandType.Text };
                    cmd.Parameters.AddWithValue("@IdCourier", bean.IdCourier);
                    cmd.Parameters.AddWithValue("@Ubigeo", bean.Ubigeo);
                    cmd.Parameters.AddWithValue("@Calle", bean.Calle);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (SqlException e) { tran.Rollback(); cn.Close(); throw new Exception(e.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error : " + e2.Message); }
        }
        public void Eliminar(int Id)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            string query = "delete  from al.OUR1 where Id=@Id";
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", Id);
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