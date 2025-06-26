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
        private readonly Utilitarios uti = new Utilitarios();

        public OAREA_E BuscarArea(int id)
        {
            OAREA_E obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT AR.Id, AR.IdDepartamento, AR.Nombre, AR.Estado, CONVERT(varchar, AR.FechaRegistro, 103),");
                    sb.Append(" CASE WHEN AR.Estado = 'Y' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado, AR.Codigo");
                    sb.Append(" FROM dbo.OAREA AR");
                    sb.Append(" WHERE AR.ID=@Id ");
                    cmd.Parameters.AddWithValue("@Id", id);

                    cmd.CommandText = sb.ToString();
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            obj = new OAREA_E();
                            if (!dr.IsDBNull(0)) { obj.Id = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { obj.IdDepartamento = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { obj.Nombre = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { obj.Estado = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { obj.FechaRegistro = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { obj.DescripcionEstado = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { obj.Codigo = dr.GetInt32(6); }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(uti.directorioLogs + "OAREA_D - BuscarArea.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");
            }
            return obj;
        }

        public List<OAREA_E> ListarAreas(OAREA_E filtros)
        {
            List<OAREA_E> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT AR.Id, AR.IdDepartamento, AR.Nombre, AR.Estado, CONVERT(varchar, AR.FechaRegistro, 103),");
                    sb.Append(" CASE WHEN AR.Estado = 'Y' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado, AR.Codigo");
                    sb.Append(" FROM dbo.OAREA AR");
                    sb.Append(" WHERE 1 = 1");
                    if (filtros != null)
                    {
                        if (filtros.Id > 0)
                        {
                            sb.Append(" AND AR.Id = @Id");
                            cmd.Parameters.AddWithValue("@Id", filtros.Id);
                        }
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
                                if (!dr.IsDBNull(0)) { obj.Id = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.IdDepartamento = dr.GetInt32(1); }
                                if (!dr.IsDBNull(2)) { obj.Nombre = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.Estado = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.FechaRegistro = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.DescripcionEstado = dr.GetString(5); }
                                if (!dr.IsDBNull(6)) { obj.Codigo = dr.GetInt32(6); }
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

        public void InsertarAreas(List<OAREA_E> areas)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                cn.Open();
                foreach (var area in areas)
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.MANT_OAREA", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Accion", "INS");
                        cmd.Parameters.AddWithValue("@Codigo", area.Codigo);
                        cmd.Parameters.AddWithValue("@IdDepartamento", area.IdDepartamento);
                        cmd.Parameters.AddWithValue("@Nombre", area.Nombre);
                        cmd.Parameters.AddWithValue("@Estado", area.Estado);
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.RegistrarError(ex, "OAREA_D - InsertarAreas");
                        }
                    }
                }
                cn.Close();
            }
        }

        public void ActualizarAreas(List<OAREA_E> areas)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                cn.Open();
                foreach (var area in areas)
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.MANT_OAREA", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Accion", "UPD");
                        cmd.Parameters.AddWithValue("@Codigo", area.Codigo);
                        cmd.Parameters.AddWithValue("@IdDepartamento", area.IdDepartamento);
                        cmd.Parameters.AddWithValue("@Nombre", area.Nombre);
                        cmd.Parameters.AddWithValue("@Estado", area.Estado);
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            LogHelper.RegistrarError(ex, "OAREA_D - ActualizarAreas");
                        }
                    }
                }
                cn.Close();
            }
        }
    }
}