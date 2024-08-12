using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.RecursosHumanos_DAO
{
    public class CARGO_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public string RegistrarCargo(CARGO_E datos)
        {
            string mensajeError;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_CARGO", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", "INS");
                    cmd.Parameters.AddWithValue("@RolID", datos.RolID);
                    cmd.Parameters.AddWithValue("@Nombre", datos.Nombre);
                    cmd.Parameters.AddWithValue("@Estado", 1);

                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(uti.directorioLogs + "CARGO_D - RegistrarCargo.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    mensajeError = "Ocurrió un error al registrar el cargo. Por favor, comunicarse con SISTEMAS.";
                }
            }

            return mensajeError;
        }

        public string EditarCargo(CARGO_E datos)
        {
            string mensajeError;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_CARGO", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", "UPD");
                    cmd.Parameters.AddWithValue("@Id", datos.Id);
                    cmd.Parameters.AddWithValue("@Estado", datos.Estado);

                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(uti.directorioLogs + "CARGO_D - EditarCargo.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    mensajeError = "Ocurrió un error al editar el cargo. Por favor, comunicarse con SISTEMAS.";
                }
            }

            return mensajeError;
        }

        public string EliminarCargo(int id)
        {
            string mensajeError;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_CARGO", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "DEL");
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(uti.directorioLogs + "CARGO_D - EliminarCargo.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    mensajeError = "Ocurrió un error al eliminar el cargo. Por favor, comunicarse con SISTEMAS.";
                }
            }

            return mensajeError;
        }

        public List<CARGO_E> ListarCargos(CARGO_E filtros)
        {
            List<CARGO_E> lista = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT CAR.Id, CAR.RolID, CAR.Nombre, CAR.Estado, CASE WHEN CAR.Estado = '1' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado");
                    sb.Append(" FROM dbo.CARGO CAR");
                    sb.Append(" WHERE 1 = 1");

                    if (filtros != null)
                    {
                        if (filtros.Id > 0)
                        {
                            sb.Append(" AND CAR.Id = @Id");
                            cmd.Parameters.AddWithValue("@Id", filtros.Id);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.Nombre))
                        {
                            sb.Append(" AND CAR.Nombre LIKE @Nombre");
                            cmd.Parameters.AddWithValue("@Nombre", string.Format("%{0}%", filtros.Nombre));
                        }

                        if (!string.IsNullOrEmpty(filtros.Estado))
                        {
                            sb.Append(" AND CAR.Estado = @Estado");
                            cmd.Parameters.AddWithValue("@Estado", filtros.Estado);
                        }
                    }

                    sb.Append($" ORDER BY CAR.Nombre ASC");

                    cmd.CommandText = sb.ToString();
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            lista = new List<CARGO_E>();

                            while (dr.Read())
                            {
                                CARGO_E obj = new CARGO_E();
                                if (!dr.IsDBNull(0)) { obj.Id = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.RolID = dr.GetInt32(1); }
                                if (!dr.IsDBNull(2)) { obj.Nombre = dr.GetString(2); }
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
                File.AppendAllText(uti.directorioLogs + "CARGO_D - ListarCargos.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
            }

            return lista;
        }

        public CARGO_E ObtenerDatosCargo(int id)
        {
            CARGO_E obj = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    string query = "SELECT Id, RolID, Nombre, Estado, CASE WHEN Estado = '1' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado FROM dbo.CARGO WHERE Id = @Id";

                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@Id", id);

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            obj = new CARGO_E();

                            while (dr.Read())
                            {
                                if (!dr.IsDBNull(0)) { obj.Id = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.RolID = dr.GetInt32(1); }
                                if (!dr.IsDBNull(2)) { obj.Nombre = dr.GetString(2); }
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
                File.AppendAllText(uti.directorioLogs + "CARGO_D - ObtenerDatosCargo.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
            }

            return obj;
        }
    }
}