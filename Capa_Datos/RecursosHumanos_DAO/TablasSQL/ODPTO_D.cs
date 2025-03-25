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
        readonly Utilitarios uti = new Utilitarios();
        public void RegistrarDepartamentos(List<ODPTO_E> listaDatos)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.MANT_ODPTO", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        cn.Open();
                        // Procesar cada elemento en la lista
                        foreach (var dpto in listaDatos)
                        {
                            // Limpiar los parámetros antes de agregar nuevos
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@Accion", "INS");
                            cmd.Parameters.AddWithValue("@IdDepartamento", dpto.IdDepartamento);
                            cmd.Parameters.AddWithValue("@Nombre", dpto.Nombre);
                            cmd.Parameters.AddWithValue("@Estado", dpto.Estado);
                            cmd.ExecuteNonQuery();
                        }
                        Console.WriteLine("Inserción completada con éxito.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al ejecutar el procedimiento almacenado: " + ex.Message);
                        File.AppendAllText(uti.directorioLogs + "ODPTO_D - RegistrarDepartamento.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    }
                }
            }
        }
        public List<ODPTO_E> ListarDepartamentos(ODPTO_E filtros)
        {
            List<ODPTO_E> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT DEP.IdDepartamento, DEP.Nombre, DEP.Estado, CONVERT(varchar, DEP.FechaRegistro, 103),");
                    sb.Append(" CASE WHEN DEP.Estado = 'Y' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado");
                    sb.Append(" FROM dbo.ODPTO DEP");
                    sb.Append(" WHERE 1 = 1");
                    if (filtros != null)
                    {
                        if (filtros.IdDepartamento > 0)
                        {
                            sb.Append(" AND DEP.IdDepartamento = @IdDepartamento");
                            cmd.Parameters.AddWithValue("@IdDepartamento", filtros.IdDepartamento);
                        }
                        if (!string.IsNullOrWhiteSpace(filtros.Nombre))
                        {
                            sb.Append(" AND DEP.Nombre LIKE @Nombre");
                            cmd.Parameters.AddWithValue("@Nombre", string.Format("%{0}%", filtros.Nombre));
                        }
                        if (!string.IsNullOrWhiteSpace(filtros.Estado))
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
                                if (!dr.IsDBNull(0)) { obj.IdDepartamento = dr.GetInt32(0); }
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
        public ODPTO_E ObtenerDatosDepartamento(int idDepartamento)
        {
            ODPTO_E obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT DEP.IdDepartamento, DEP.Nombre, DEP.Estado, CONVERT(varchar, DEP.FechaRegistro, 103),");
                    sb.Append(" CASE WHEN DEP.Estado = '1' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado");
                    sb.Append(" FROM dbo.ODPTO DEP");
                    sb.Append(" WHERE DEP.IdDepartamento = @IdDepartamento");
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("@IdDepartamento", idDepartamento);
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            obj = new ODPTO_E();
                            while (dr.Read())
                            {
                                if (!dr.IsDBNull(0)) { obj.IdDepartamento = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.Nombre = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.Estado = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.FechaRegistro = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.DescripcionEstado = dr.GetString(4); }
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(uti.directorioLogs + "ODPTO_D - ObtenerDatosDepartamento.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
            }
            return obj;
        }
    }
}
