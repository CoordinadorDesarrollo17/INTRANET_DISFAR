using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.General_DAO.TablasSql
{
    public class OAGE_D
    {
        Utilitarios uti = new Utilitarios();
        public List<Capa_Entidad.General_ENT.TablasSql.OAGE_E> Listar()
        {
            List<Capa_Entidad.General_ENT.TablasSql.OAGE_E> lista = new List<Capa_Entidad.General_ENT.TablasSql.OAGE_E>();
            string query = "select  Id, Nombre from dbo.OAGE";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Capa_Entidad.General_ENT.TablasSql.OAGE_E bean = new Capa_Entidad.General_ENT.TablasSql.OAGE_E();
                    if (!dr.IsDBNull(0)) { bean.Id = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { bean.Nombre = dr.GetString(1); }
                    lista.Add(bean);
                }
                dr.Close();
                cn.Close();
            }

            catch { cn.Close(); }
            return lista;
        }
        public string Registrar(Capa_Entidad.General_ENT.TablasSql.OAGE_E bean)
        {
            string res = string.Empty;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            string query = "insert into dbo.OAGE values (@Nombre)";
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn) { Transaction = tran, CommandType = CommandType.Text };
                    cmd.Parameters.AddWithValue("@Nombre", bean.Nombre);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    res = "Se registró agencia";
                }
                catch (SqlException e) { tran.Rollback(); cn.Close(); res = e.Message; }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); res = e2.Message; }
            return res;
        }
        public void Eliminar(int Id)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            string query = "delete  from dbo.OAGE where Id=@Id";
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
