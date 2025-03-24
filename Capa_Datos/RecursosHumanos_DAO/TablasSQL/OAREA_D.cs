using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
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
    public class OAREA_D
    {
        readonly Utilitarios uti = new Utilitarios();
        public void RegistrarAreas(List<OAREA_E> listaDatos)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.MANT_OAREA", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        cn.Open();
                        // Procesar cada elemento en la lista
                        foreach (var area in listaDatos)
                        {
                            // Limpiar los parámetros antes de agregar nuevos
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@Accion", "INS");
                            cmd.Parameters.AddWithValue("@IdArea", area.IdArea);
                            cmd.Parameters.AddWithValue("@IdDepartamento", area.IdDepartamento);
                            cmd.Parameters.AddWithValue("@Nombre", area.Nombre);
                            cmd.Parameters.AddWithValue("@Estado", area.Estado);
                            cmd.ExecuteNonQuery();
                        }
                        Console.WriteLine("Inserción completada con éxito.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error al ejecutar el procedimiento almacenado: " + ex.Message);
                        File.AppendAllText(uti.directorioLogs + "OAREA_D - RegistrarAreas.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    }
                }
            }
        }
        public List<OAREA_E> ListarAreas(OAREA_E filtros)
        {
            List<OAREA_E> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT AR.IdArea, AR.IdDepartamento, AR.Nombre, AR.Estado, CONVERT(varchar, AR.FechaRegistro, 103),");
                    sb.Append(" CASE WHEN AR.Estado = 'Y' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado");
                    sb.Append(" FROM dbo.OAREA AR");
                    sb.Append(" WHERE 1 = 1");
                    if (filtros != null)
                    {
                        if (filtros.IdDepartamento > 0)
                        {
                            sb.Append(" AND AR.IdDepartamento = @IdDepartamento");
                            cmd.Parameters.AddWithValue("@IdDepartamento", filtros.IdDepartamento);
                        }
                        if (!string.IsNullOrWhiteSpace(filtros.Nombre))
                        {
                            sb.Append(" AND AR.Nombre LIKE @Nombre");
                            cmd.Parameters.AddWithValue("@Nombre", string.Format("%{0}%", filtros.Nombre));
                        }
                        if (!string.IsNullOrWhiteSpace(filtros.Estado))
                        {
                            sb.Append(" AND AR.Estado = @Estado");
                            cmd.Parameters.AddWithValue("@Estado", filtros.Estado);
                        }
                    }
                    //sb.Append($" ORDER BY ---- DESC");    DESCOMENTAR LÍNEA SI SE DESEA ORDENAR POR ALGÚN CAMPO EN ESPECIAL
                    cmd.CommandText = sb.ToString();
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            lista = new List<OAREA_E>();
                            while (dr.Read())
                            {
                                OAREA_E obj = new OAREA_E();
                                if (!dr.IsDBNull(0)) { obj.IdArea = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.IdDepartamento = dr.GetInt32(1); }
                                if (!dr.IsDBNull(2)) { obj.Nombre = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.Estado = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.FechaRegistro = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.DescripcionEstado = dr.GetString(5); }
                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(uti.directorioLogs + "OAREA_D - ListarAreas.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
            }
            return lista;
        }
        public OAREA_E ObtenerDatosArea(int idArea)
        {
            OAREA_E obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT AR.IdArea, AR.IdDepartamento, AR.Nombre, AR.Estado, CONVERT(varchar, AR.FechaRegistro, 103),");
                    sb.Append(" CASE WHEN AR.Estado = 'Y' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado");
                    sb.Append(" FROM dbo.OAREA AR");
                    sb.Append(" WHERE IdArea = @IdArea");
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("@IdArea", idArea);
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            obj = new OAREA_E();
                            while (dr.Read())
                            {
                                if (!dr.IsDBNull(0)) { obj.IdArea = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.IdDepartamento = dr.GetInt32(1); }
                                if (!dr.IsDBNull(2)) { obj.Nombre = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.Estado = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.FechaRegistro = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.DescripcionEstado = dr.GetString(5); }
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(uti.directorioLogs + "OAREA_D - ObtenerDatosArea.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
            }
            return obj;
        }
    }
}
