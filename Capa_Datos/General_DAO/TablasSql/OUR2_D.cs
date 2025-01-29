using Capa_Entidad.General_ENT.TablasSql;
using SpreadsheetLight;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.General_DAO.TablasSql
{
    public class OUR2_D
    {
        Utilitarios uti = new Utilitarios();
        public List<OUR2_E> Visualizar(string filename)
        {
            string filepath = string.Empty;
            filepath = uti.directorioGeneral + filename;
            SLDocument sld = new SLDocument(filepath);
            int iRow = 2;
            List<OUR2_E> list = new List<OUR2_E>();
            while (!string.IsNullOrWhiteSpace(sld.GetCellValueAsString(iRow, 1)))
            {
                OUR2_E bean = new OUR2_E();
                bean.Destino = sld.GetCellValueAsString(iRow, 1);
                bean.PrecioBase = sld.GetCellValueAsDecimal(iRow, 2);
                bean.TarifaKg = sld.GetCellValueAsDecimal(iRow, 3);
                list.Add(bean);
                iRow++;
            }
            return list;
        }
        public OUR2_E Obtener(int IdCourier, string Destino)
        {
            string query = "select PrecioBase,TarifaKg from al.OUR2 where IdCourier=@IdCourier AND Destino=@Destino";
            OUR2_E bean = new OUR2_E();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
                cmd.Parameters.AddWithValue("@IdCourier", IdCourier);
                cmd.Parameters.AddWithValue("@Destino", Destino);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0)) { bean.PrecioBase = dr.GetDecimal(0); }
                    if (!dr.IsDBNull(1)) { bean.TarifaKg = dr.GetDecimal(1); }
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return bean;
        }
        public List<OUR2_E> Listar(string orderBy)
        {
            List<OUR2_E> lista = new List<OUR2_E>();
            string query = "select  t0.Id,( select Nombre from al.COUR where Id=t0.IdCourier) AS Agencia , t0.Destino,t0.PrecioBase,t0.TarifaKg from al.OUR2 t0 " + orderBy;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    OUR2_E bean = new OUR2_E();
                    if (!dr.IsDBNull(0)) { bean.Id = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { bean.NombreAgencia = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { bean.Destino = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { bean.PrecioBase = dr.GetDecimal(3); }
                    if (!dr.IsDBNull(4)) { bean.TarifaKg = dr.GetDecimal(4); }
                    lista.Add(bean);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public string Registrar(OUR2_E bean)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            string res = string.Empty;
            string query = "insert into al.OUR2 values(@IdCourier,@Destino,@PrecioBase,@TarifaKg)";
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn) { Transaction = tran, CommandType = CommandType.Text };
                    cmd.Parameters.AddWithValue("@IdCourier", bean.IdCourier);
                    cmd.Parameters.AddWithValue("@Destino", bean.Destino.ToUpper());
                    cmd.Parameters.AddWithValue("@PrecioBase", bean.PrecioBase);
                    cmd.Parameters.AddWithValue("@TarifaKg", bean.TarifaKg);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    res = "Tarifario registrado satisfactoriamente";
                }
                catch (Exception e)
                {
                    tran.Rollback(); cn.Close(); if (e.Message.Contains("UNIQUE"))
                    {
                        res = "Tarifario con Destino y Agencia ingresada ya existe.";
                    }
                    else { res = e.Message; }
                }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); res = e2.Message; }
            return res;
        }
        public List<OUR2_E> Buscar(OUR2_E datos)
        {
            List<OUR2_E> lista = new List<OUR2_E>();
            string condWhere = "";

            if (datos.Id > 0)
            {
                condWhere += $" t1.Id = {datos.Id}";
            }
            else if (datos.Destino != "" || datos.NombreAgencia != "")
            {
                condWhere += $" t1.Destino LIKE '%{datos.Destino}%' OR t0.Nombre LIKE '%{datos.NombreAgencia}%'";
            }

            string query = $"SELECT t1.Id, t1.IdCourier, t1.Destino, t1.PrecioBase, t1.TarifaKg, t0.Nombre FROM al.OUR2 t1 LEFT JOIN al.COUR t0 ON t0.Id = t1.IdCourier WHERE {condWhere} ";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    OUR2_E our2 = new OUR2_E();
                    if (!dr.IsDBNull(0)) { our2.Id = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { our2.IdCourier = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { our2.Destino = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { our2.PrecioBase = dr.GetDecimal(3); }
                    if (!dr.IsDBNull(4)) { our2.TarifaKg = dr.GetDecimal(4); }
                    if (!dr.IsDBNull(5)) { our2.NombreAgencia = dr.GetString(5); }
                    lista.Add(our2);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
        public string Editar(OUR2_E bean)
        {
            string res = string.Empty;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            string query = "UPDATE al.OUR2 SET IdCourier=@IdCourier, Destino=@Destino, PrecioBase=@PrecioBase, TarifaKg=@TarifaKg WHERE Id=@Id";
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@IdCourier", bean.IdCourier);
                    cmd.Parameters.AddWithValue("@Destino", bean.Destino);
                    cmd.Parameters.AddWithValue("@PrecioBase", bean.PrecioBase);
                    cmd.Parameters.AddWithValue("@TarifaKg", bean.TarifaKg);
                    cmd.Parameters.AddWithValue("@Id", bean.Id);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    res = "Tarifario editado satisfactoriamente";
                }
                catch (Exception e) { tran.Rollback(); cn.Close(); res = e.Message; }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); res = e2.Message; }
            return res;
        }
        public void Eliminar(int Id)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            string query = "DELETE FROM al.OUR2 WHERE Id=@Id";
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