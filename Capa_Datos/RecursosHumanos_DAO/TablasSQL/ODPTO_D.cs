using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.RecursosHumanos_DAO.TablasSQL
{
    public class ODPTO_D
    {
        private readonly Utilitarios uti = new Utilitarios();

        public void InsertarDepartamentos(List<ODPTO_E> departamentos)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                cn.Open();
                try
                {
                    foreach (var dpto in departamentos)
                    {
                        using (SqlCommand cmd = new SqlCommand("dbo.MANT_ODPTO", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Accion", "INS");
                            cmd.Parameters.AddWithValue("@Id", dpto.Id);
                            cmd.Parameters.AddWithValue("@Nombre", dpto.Nombre);
                            cmd.Parameters.AddWithValue("@Estado", dpto.Estado);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.RegistrarError(ex, "ODPTO_D - InsertarDepartamentos");
                }
                cn.Close();
            }
        }

        public void ActualizarDepartamentos(List<ODPTO_E> departamentos)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                cn.Open();
                try
                {
                    foreach (var dpto in departamentos)
                    {
                        using (SqlCommand cmd = new SqlCommand("dbo.MANT_ODPTO", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Accion", "UPD");
                            cmd.Parameters.AddWithValue("@Id", dpto.Id);
                            cmd.Parameters.AddWithValue("@Nombre", dpto.Nombre);
                            cmd.Parameters.AddWithValue("@Estado", dpto.Estado);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.RegistrarError(ex, "ODPTO_D - ActualizarDepartamentos");
                }
                cn.Close();
            }
        }

        public List<ODPTO_E> ListarDepartamentos(ODPTO_E filtros)
        {
            List<ODPTO_E> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT DEP.Id, DEP.Nombre, DEP.Estado, CONVERT(varchar, DEP.FechaRegistro, 103),");
                    sb.Append(" CASE WHEN DEP.Estado = 'Y' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado");
                    sb.Append(" FROM dbo.ODPTO DEP");
                    sb.Append(" WHERE 1 = 1");
                    if (filtros != null)
                    {
                        if (filtros.Id > 0)
                        {
                            sb.Append(" AND DEP.Id = @Id");
                            cmd.Parameters.AddWithValue("@Id", filtros.Id);
                        }
                        if (!string.IsNullOrWhiteSpace(filtros.Nombre))
                        {
                            sb.Append(" AND DEP.Nombre LIKE @Nombre");
                            cmd.Parameters.AddWithValue("@Nombre", string.Format("%{0}%", filtros.Nombre));
                        }
                        if (!string.IsNullOrEmpty(filtros.Estado))
                        {
                            sb.Append(" AND DEP.Estado = @Estado");
                            cmd.Parameters.AddWithValue("@Estado", filtros.Estado);
                        }
                    }
                    cmd.CommandText = sb.ToString();
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            lista = new List<ODPTO_E>();
                            while (dr.Read())
                            {
                                ODPTO_E obj = new ODPTO_E();
                                if (!dr.IsDBNull(0)) { obj.Id = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.Nombre = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.Estado = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.FechaRegistro = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.DescripcionEstado = dr.GetString(4); }
                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(uti.directorioLogs + "ODPTO_D - ListarDepartamentos.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
            }
            return lista;
        }
    }
}