using Capa_Entidad.Rutas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos.Rutas_DAO.TablasSql
{
    public class RRU01_D
    {
        Utilitarios uti = new Utilitarios();

        public RRU01_E BuscarCorrelativo(string Correlativo)
        {
            RRU01_E o = new RRU01_E();
            using (SqlConnection connection = new SqlConnection(uti.cadSql))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT top 1 Id,TablaSAP, Identificador, DocEntryTicket,U_SYP_MDTD, U_SYP_MDSD,U_SYP_MDCD,DocDate,U_BPP_FECINITRA,Impreso FROM al.RRU01 where concat(U_SYP_MDTD,'-',U_SYP_MDSD,'-',U_SYP_MDCD)=@Correlativo AND Impreso=1 AND Estado in ('LIBERADO','ANULADO') ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Correlativo", Correlativo);
                        command.CommandType = CommandType.Text;
                        SqlDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            if (!dr.IsDBNull(0)) { o.Id = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { o.TablaSAP = dr.GetString(1); }
                            if (!dr.IsDBNull(2)) { o.Identificador = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { o.DocEntryTicket = dr.GetInt32(3); }
                            if (!dr.IsDBNull(4)) { o.U_SYP_MDTD = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { o.U_SYP_MDSD = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { o.U_SYP_MDCD = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { o.DocDate = dr.GetDateTime(7).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(8)) { o.U_BPP_FECINITRA = dr.GetDateTime(8).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(9)) { o.Impreso = dr.GetInt32(9); }

                        }
                    }
                    return o;
                }
                catch
                {
                    return o;
                }
            }
        }
        public void Liberar(int id)
        {
            string query = "UPDATE al.RRU01 SET Estado='LIBERADO' WHERE Id=@Id";

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn, tran);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception ex2)
                {
                    tran.Rollback();
                    throw new Exception("Error al actualizar: " + ex2.Message);
                }
                finally
                {
                    cn.Close();
                }
            }
        }

        public void Agregar(RRU01_E obj)
        {
            int Linea = 0;
            using (SqlConnection connection = new SqlConnection(uti.cadSql))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT MAX(Linea) FROM AL.RRU01 WHERE DocEntryORRU=@DocEntryORRU";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DocEntryORRU", obj.DocEntryORRU);
                        command.CommandType = CommandType.Text;
                        SqlDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            Linea = dr.GetInt32(0);
                        }
                    }
                }
                catch
                {
                }
            }
            using (SqlConnection connection = new SqlConnection(uti.cadSql))
            {
                try
                {
                    connection.Open();
                    string query = $"INSERT INTO al.RRU01 values(@DocEntryORRU,@DocEntryTicket,@Linea,@TablaSAP,@Identificador,@U_SYP_MDTD,@U_SYP_MDSD,@U_SYP_MDCD,@DocDate,@U_BPP_FECINITRA,@Impreso,@Estado,@Operario,@FechaOperación,@HoraOperación)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        Linea = Linea + 1;
                        command.Parameters.AddWithValue("@DocEntryORRU", obj.DocEntryORRU);
                        command.Parameters.AddWithValue("@DocEntryTicket", obj.DocEntryTicket);
                        command.Parameters.AddWithValue("@Linea", Linea);
                        command.Parameters.AddWithValue("@TablaSAP", obj.TablaSAP);
                        command.Parameters.AddWithValue("@Identificador", obj.Identificador);
                        command.Parameters.AddWithValue("@U_SYP_MDTD", obj.U_SYP_MDTD);
                        command.Parameters.AddWithValue("@U_SYP_MDSD", obj.U_SYP_MDSD);
                        command.Parameters.AddWithValue("@U_SYP_MDCD", obj.U_SYP_MDCD);
                        command.Parameters.AddWithValue("@DocDate", obj.DocDate);
                        command.Parameters.AddWithValue("@U_BPP_FECINITRA", (object)obj.U_BPP_FECINITRA ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Impreso", obj.Impreso);
                        command.Parameters.AddWithValue("@Estado", obj.Estado);
                        command.Parameters.AddWithValue("@Operario", obj.Operario);
                        command.Parameters.AddWithValue("@FechaOperación", obj.FechaOperación);
                        command.Parameters.AddWithValue("@HoraOperación", obj.HoraOperación);
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                    }
                }
                catch
                {
                }
            }
        }
        public List<RRU01_E> BuscarComprobantes(int DocEntryTicket)
        {
            List<RRU01_E> lista = new List<RRU01_E>();
            using (SqlConnection connection = new SqlConnection(uti.cadSql))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT Id,TablaSAP, Identificador, DocEntryTicket,U_SYP_MDTD, U_SYP_MDSD,U_SYP_MDCD,DocDate,U_BPP_FECINITRA,Impreso FROM al.RRU01 where DocEntryTicket=@DocEntryTicket AND  Estado NOT IN ('LIBERADO', 'ANULADO') ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DocEntryTicket", DocEntryTicket);
                        command.CommandType = CommandType.Text;
                        SqlDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            RRU01_E o = new RRU01_E();
                            if (!dr.IsDBNull(0)) { o.Id = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { o.TablaSAP = dr.GetString(1); }
                            if (!dr.IsDBNull(2)) { o.Identificador = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { o.DocEntryTicket = dr.GetInt32(3); }
                            if (!dr.IsDBNull(4)) { o.U_SYP_MDTD = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { o.U_SYP_MDSD = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { o.U_SYP_MDCD = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { o.DocDate = dr.GetDateTime(7).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(8)) { o.U_BPP_FECINITRA = dr.GetDateTime(8).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(9)) { o.Impreso = dr.GetInt32(9); }
                            lista.Add(o);
                        }
                    }
                    return lista;
                }
                catch
                {
                    return lista;
                }
            }
        }
    }
}
