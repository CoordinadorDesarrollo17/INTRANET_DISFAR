using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using Capa_Entidad.Ventas_ENT.Reportes;
using Capa_Entidad.Ventas_ENT.TablasSql;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Capa_Datos.General_DAO
{
    public class SEDE_D
    {
        readonly Utilitarios uti = new Utilitarios();
        public string RegistrarSede(SEDE_E datos)
        {
            string mensajeError;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_SEDE", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "INS");
                    cmd.Parameters.AddWithValue("@Nombre", datos.Nombre);
                    cmd.Parameters.AddWithValue("@UbigeoID", datos.UbigeoID);
                    cmd.Parameters.AddWithValue("@UsuarioOperacion", datos.UsuarioOperacion);
                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(uti.directorioLogs + "SEDE_D - RegistrarSede.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    mensajeError = "Ocurrió un error al registrar la sede. Por favor, comuníquese con el área de Sistemas para más información.";
                }
            }
            return mensajeError;
        }
        public string EditarSede(SEDE_E datos)
        {
            string mensajeError;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_SEDE", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "UPD");
                    cmd.Parameters.AddWithValue("@Id", datos.Id);
                    cmd.Parameters.AddWithValue("@Estado", datos.Estado);
                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(uti.directorioLogs + "SEDE_D - EditarSede.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    mensajeError = "Ocurrió un error al editar la sede. Por favor, comuníquese con el área de Sistemas para más información.";
                }
            }
            return mensajeError;
        }
        public string EliminarSede(int Id)
        {
            string mensajeError;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_SEDE", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "DEL");
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(uti.directorioLogs + "SEDE_D - EliminarSede.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    mensajeError = "Ocurrió un error al eliminar la sede. Por favor, comuníquese con el área de Sistemas para más información.";
                }
            }
            return mensajeError;
        }
        public List<SEDE_E> ListarSedes(SEDE_E filtros)
        {
            List<SEDE_E> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    var sb = new StringBuilder();
                    sb.Append("SELECT SD.Id, SD.Nombre, SD.UbigeoID, SD.Estado, CASE WHEN SD.Estado = '1' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado")
                    .Append(" FROM dbo.SEDE SD")
                    .Append(" WHERE 1 = 1");
                    if (filtros != null)
                    {
                        if (filtros.Id > 0)
                        {
                            sb.Append(" AND SD.Id = @Id");
                            cmd.Parameters.AddWithValue("@Id", filtros.Id);
                        }
                        if (!string.IsNullOrWhiteSpace(filtros.Nombre))
                        {
                            sb.Append(" AND SD.Nombre LIKE @Nombre");
                            cmd.Parameters.AddWithValue("@Nombre", string.Format("%{0}%", filtros.Nombre));
                        }
                        if (filtros.UbigeoID > 0)
                        {
                            sb.Append(" AND SD.UbigeoID = @UbigeoID");
                            cmd.Parameters.AddWithValue("@UbigeoID", filtros.UbigeoID);
                        }
                        if (!string.IsNullOrWhiteSpace(filtros.Estado))
                        {
                            sb.Append(" AND SD.Estado = @Estado");
                            cmd.Parameters.AddWithValue("@Estado", filtros.Estado);
                        }
                    }
                    sb.Append($" ORDER BY SD.Nombre ASC");   
                    cmd.CommandText = sb.ToString();
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            lista = new List<SEDE_E>();
                            while (dr.Read())
                            {
                                SEDE_E obj = new SEDE_E();
                                if (!dr.IsDBNull(0)) { obj.Id = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.Nombre = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.UbigeoID = dr.GetInt32(2); }
                                if (!dr.IsDBNull(3)) { obj.Estado = dr.GetString(3); }
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
                File.AppendAllText(uti.directorioLogs + "SEDE_D - ListarSedes.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
            }
            return lista;
        }
        public SEDE_E ObtenerDatosSede(int Id)
        {
            SEDE_E obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    var sb = new StringBuilder();
                    sb.Append("SELECT SD.Id, SD.Nombre, SD.UbigeoID, SD.Estado, CASE WHEN SD.Estado = '1' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado")
                    .Append(" FROM dbo.SEDE SD")
                    .Append(" WHERE SD.Id = @Id");
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            obj = new SEDE_E();
                            while (dr.Read())
                            {
                                if (!dr.IsDBNull(0)) { obj.Id = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.Nombre = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.UbigeoID = dr.GetInt32(2); }
                                if (!dr.IsDBNull(3)) { obj.Estado = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.DescripcionEstado = dr.GetString(4); }
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(uti.directorioLogs + "SEDE_D - ObtenerDatosSede.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
            }
            return obj;
        }
    }
}