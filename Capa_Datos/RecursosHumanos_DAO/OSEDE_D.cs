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
    public class OSEDE_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public string RegistrarSede(OSEDE_E datos)
        {
            string mensajeError;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_OSEDE", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", "INS");
                    cmd.Parameters.AddWithValue("@Nombre", datos.Nombre);
                    cmd.Parameters.AddWithValue("@IdUbig", datos.IdUbig);

                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(uti.directorioLogs + "OSEDE_D - RegistrarSede.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    mensajeError = "Ocurrió un error al registrar la sede. Por favor, comunicarse con SISTEMAS.";
                }
            }

            return mensajeError;
        }

        public string EditarSede(OSEDE_E datos)
        {
            string mensajeError;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_OSEDE", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", "UPD");
                    cmd.Parameters.AddWithValue("@IdSede", datos.IdSede);
                    cmd.Parameters.AddWithValue("@Estado", datos.Estado);

                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(uti.directorioLogs + "OSEDE_D - EditarSede.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    mensajeError = "Ocurrió un error al editar la sede. Por favor, comunicarse con SISTEMAS.";
                }
            }

            return mensajeError;
        }

        public string EliminarSede(int idSede)
        {
            string mensajeError;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_OSEDE", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "DEL");
                    cmd.Parameters.AddWithValue("@IdSede", idSede);
                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(uti.directorioLogs + "OSEDE_D - EliminarSede.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    mensajeError = "Ocurrió un error al eliminar la sede. Por favor, comunicarse con SISTEMAS.";
                }
            }

            return mensajeError;
        }

        public List<OSEDE_E> ListarSedes(OSEDE_E filtros)
        {
            List<OSEDE_E> lista = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT SD.IdSede, SD.Nombre, SD.IdUbig, SD.Estado, CONVERT(varchar, SD.FechaRegistro, 103), CONVERT(varchar, SD.FechaModificacion, 103),");
                    sb.Append(" CASE WHEN SD.Estado = '1' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado");
                    sb.Append(" FROM dbo.OSEDE SD");
                    sb.Append(" WHERE 1 = 1");

                    if (filtros != null)
                    {
                        if (filtros.IdSede > 0)
                        {
                            sb.Append(" AND SD.IdSede = @IdSede");
                            cmd.Parameters.AddWithValue("@IdSede", filtros.IdSede);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.Nombre))
                        {
                            sb.Append(" AND SD.Nombre LIKE @Nombre");
                            cmd.Parameters.AddWithValue("@Nombre", string.Format("%{0}%", filtros.Nombre));
                        }

                        if (filtros.IdUbig > 0)
                        {
                            sb.Append(" AND SD.IdUbig = @IdUbig");
                            cmd.Parameters.AddWithValue("@IdUbig", filtros.IdUbig);
                        }

                        if (!string.IsNullOrEmpty(filtros.Estado))
                        {
                            sb.Append(" AND SD.Estado LIKE @Estado");
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
                            lista = new List<OSEDE_E>();

                            while (dr.Read())
                            {
                                OSEDE_E obj = new OSEDE_E();
                                if (!dr.IsDBNull(0)) { obj.IdSede = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.Nombre = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.IdUbig = dr.GetInt32(2); }
                                if (!dr.IsDBNull(3)) { obj.Estado = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.FechaRegistro = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.FechaModificacion = dr.GetString(5); }
                                if (!dr.IsDBNull(6)) { obj.DescripcionEstado = dr.GetString(6); }

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(uti.directorioLogs + "OSEDE_D - ListarSedes.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
            }

            return lista;
        }

        public OSEDE_E ObtenerDatosSede(int idSede)
        {
            OSEDE_E obj = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT SD.IdSede, SD.Nombre, SD.IdUbig, SD.Estado, CONVERT(varchar, SD.FechaRegistro, 103), CONVERT(varchar, SD.FechaModificacion, 103),");
                    sb.Append(" CASE WHEN SD.Estado = '1' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado");
                    sb.Append(" FROM dbo.OSEDE SD");
                    sb.Append(" WHERE SD.IdSede = @IdSede");

                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("@IdSede", idSede);

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            obj = new OSEDE_E();

                            while (dr.Read())
                            {
                                if (!dr.IsDBNull(0)) { obj.IdSede = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.Nombre = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.IdUbig = dr.GetInt32(2); }
                                if (!dr.IsDBNull(3)) { obj.Estado = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.FechaRegistro = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.FechaModificacion = dr.GetString(5); }
                                if (!dr.IsDBNull(6)) { obj.DescripcionEstado = dr.GetString(6); }
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(uti.directorioLogs + "OSEDE_D - ObtenerDatosSede.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
            }

            return obj;
        }
    }
}