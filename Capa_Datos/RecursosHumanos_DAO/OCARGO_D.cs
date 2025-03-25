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
    public class OCARGO_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public string RegistrarCargo(OCARGO_E datos)
        {
            string mensajeError;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_OCARGO", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", "INS");
                    cmd.Parameters.AddWithValue("@Nombre", datos.Nombre);

                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(uti.directorioLogs + "OCARGO_D - RegistrarCargo.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    mensajeError = "Ocurrió un error al registrar el cargo. Por favor, comunicarse con SISTEMAS.";
                }
            }

            return mensajeError;
        }

        public string EditarCargo(OCARGO_E datos)
        {
            string mensajeError;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_OCARGO", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", "UPD");
                    cmd.Parameters.AddWithValue("@IdCargo", datos.IdCargo);
                    cmd.Parameters.AddWithValue("@Estado", datos.Estado);

                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(uti.directorioLogs + "OCARGO_D - EditarCargo.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    mensajeError = "Ocurrió un error al editar el cargo. Por favor, comunicarse con SISTEMAS.";
                }
            }

            return mensajeError;
        }

        public string EliminarCargo(int idCargo)
        {
            string mensajeError;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_OCARGO", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "DEL");
                    cmd.Parameters.AddWithValue("@IdCargo", idCargo);
                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    File.AppendAllText(uti.directorioLogs + "OCARGO_D - EliminarCargo.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
                    mensajeError = "Ocurrió un error al eliminar el cargo. Por favor, comunicarse con SISTEMAS.";
                }
            }

            return mensajeError;
        }

        public List<OCARGO_E> ListarCargos(OCARGO_E filtros)
        {
            List<OCARGO_E> lista = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT CAR.IdCargo, CAR.Nombre, CAR.Estado, CONVERT(varchar, CAR.FechaRegistro, 103), CONVERT(varchar, CAR.FechaModificacion, 103),");
                    sb.Append(" CASE WHEN CAR.Estado = '1' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado");
                    sb.Append(" FROM dbo.OCARGO CAR");
                    sb.Append(" WHERE 1 = 1");

                    if (filtros != null)
                    {
                        if (filtros.IdCargo > 0)
                        {
                            sb.Append(" AND CAR.IdCargo = @IdCargo");
                            cmd.Parameters.AddWithValue("@IdCargo", filtros.IdCargo);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.Nombre))
                        {
                            sb.Append(" AND CAR.Nombre LIKE @Nombre");
                            cmd.Parameters.AddWithValue("@Nombre", string.Format("%{0}%", filtros.Nombre));
                        }

                        if (!string.IsNullOrEmpty(filtros.Estado))
                        {
                            sb.Append(" AND CAR.Estado LIKE @Estado");
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
                            lista = new List<OCARGO_E>();

                            while (dr.Read())
                            {
                                OCARGO_E obj = new OCARGO_E();
                                if (!dr.IsDBNull(0)) { obj.IdCargo = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.Nombre = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.Estado = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.FechaRegistro = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.FechaModificacion = dr.GetString(4); }
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
                File.AppendAllText(uti.directorioLogs + "OCARGO_D - ListarCargos.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
            }

            return lista;
        }

        public OCARGO_E ObtenerDatosCargo(int idCargo)
        {
            OCARGO_E obj = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT CAR.IdCargo, CAR.Nombre, CAR.Estado, CONVERT(varchar, CAR.FechaRegistro, 103), CONVERT(varchar, CAR.FechaModificacion, 103),");
                    sb.Append(" CASE WHEN CAR.Estado = '1' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado");
                    sb.Append(" FROM dbo.OCARGO CAR");
                    sb.Append(" WHERE CAR.IdCargo = @IdCargo");

                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("@IdCargo", idCargo);

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            obj = new OCARGO_E();

                            while (dr.Read())
                            {
                                if (!dr.IsDBNull(0)) { obj.IdCargo = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.Nombre = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.Estado = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.FechaRegistro = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.FechaModificacion = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.DescripcionEstado = dr.GetString(5); }
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(uti.directorioLogs + "OCARGO_D - ObtenerDatosCargo.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
            }

            return obj;
        }
    }
}