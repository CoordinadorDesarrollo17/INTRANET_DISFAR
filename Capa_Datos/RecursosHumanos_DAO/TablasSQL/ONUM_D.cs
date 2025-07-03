using Capa_Datos.RecursosHumanos_DAO.Auditorias;
using Capa_Entidad.RecursosHumanos_ENT.Auditorias;
using Capa_Entidad.RecursosHumanos_ENT.Reportes;
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
    public class ONUM_D
    {
        private readonly Utilitarios uti = new Utilitarios();

        public string RegistrarNumero(ONUM_E datos)
        {
            string mensajeError;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_ONUM", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "INS");
                    cmd.Parameters.AddWithValue("@NumeroCorporativo", datos.NumeroCorporativo);
                    cmd.Parameters.AddWithValue("@Operador", datos.Operador);
                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    RegistrarError(ex, "ONUM_D - RegistrarNumero");
                    mensajeError = "Ocurrió un error al registrar el número. Por favor, comunicarse con SISTEMAS.";
                }
            }
            return mensajeError;
        }

        public string EditarNumero(ONUM_E num)
        {
            string mensajeError;
            var registroBD = ObtenerDatosNumero(num.IdNumero);
            var estados = new Dictionary<string, string>() { { "1", "ACTIVO" }, { "0", "INACTIVO" } };
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction transaction = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_ONUM", cn, transaction);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "UPD");
                    cmd.Parameters.AddWithValue("@IdNumero", num.IdNumero);
                    cmd.Parameters.AddWithValue("@Operador", num.Operador);
                    cmd.Parameters.AddWithValue("@NroDocumento", num.NroDocumento);
                    cmd.Parameters.AddWithValue("@Estado", num.Estado);
                    cmd.ExecuteNonQuery();
                    RegistrarAuditoriaCambios("Operador", registroBD.Operador, num.Operador, num.IdNumero, num.RegistradoPor);
                    RegistrarAuditoriaCambios("Estado", estados[registroBD.Estado], estados[num.Estado], num.IdNumero, num.RegistradoPor);
                    RegistrarAuditoriaCambios("NroDocumento", registroBD.NroDocumento, num.NroDocumento, num.IdNumero, num.RegistradoPor);
                    transaction.Commit();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    RegistrarError(ex, "ONUM_D - EditarNumero");
                    mensajeError = "Ocurrió un error al editar el número. Por favor, comunicarse con SISTEMAS.";
                }
            }
            return mensajeError;
        }

        public string EliminarNumero(int idNumero)
        {
            string mensajeError;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_ONUM", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "DEL");
                    cmd.Parameters.AddWithValue("@IdNumero", idNumero);
                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    RegistrarError(ex, "ONUM_D - EliminarNumero");
                    mensajeError = "Ocurrió un error al eliminar el número. Por favor, comunicarse con SISTEMAS.";
                }
            }
            return mensajeError;
        }

        public string AsignarNumero(int idNumero, string nroDocumento)
        {
            string mensajeError;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_ONUM", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "ASIG");
                    cmd.Parameters.AddWithValue("@IdNumero", idNumero);
                    cmd.Parameters.AddWithValue("@NroDocumento", nroDocumento);
                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    RegistrarError(ex, "ONUM_D - AsignarNumero");
                    mensajeError = "Ocurrió un error al asignar el número. Por favor, comunicarse con SISTEMAS.";
                }
            }
            return mensajeError;
        }

        public string LiberarNumero(int idNumero, int idOEMPL)
        {
            string mensajeError;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_ONUM", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "LIB");
                    cmd.Parameters.AddWithValue("@IdNumero", idNumero);
                    cmd.Parameters.AddWithValue("@IdOEMPL", idOEMPL);
                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    RegistrarError(ex, "ONUM_D - LiberarNumero");
                    mensajeError = "Ocurrió un error al liberar el número. Por favor, comunicarse con SISTEMAS.";
                }
            }
            return mensajeError;
        }

        public string LiberarNumerosEmpleado(string nroDocumento)
        {
            string mensajeError;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_ONUM", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "LIBNE");
                    cmd.Parameters.AddWithValue("@NroDocumento", nroDocumento);
                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    RegistrarError(ex, "ONUM_D - LiberarNumerosEmpleado");
                    mensajeError = "Ocurrió un error al liberar número del empleado. Por favor, comunicarse con SISTEMAS.";
                }
            }
            return mensajeError;
        }

        public List<ONUM_E> ListarNumeros(ONUM_E filtros)
        {
            List<ONUM_E> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT NU.IdNumero, NU.NumeroCorporativo, NU.Operador, NU.Asignado, NU.NroDocumento, NU.Estado, CONVERT(varchar, NU.FechaRegistro, 103), CONVERT(varchar, NU.FechaModificacion, 103),");
                    sb.Append(" CASE WHEN NU.Estado = '1' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado");
                    sb.Append(" FROM dbo.ONUM NU");
                    sb.Append(" WHERE 1 = 1");
                    if (filtros != null)
                    {
                        if (filtros.IdNumero > 0)
                        {
                            sb.Append(" AND NU.IdNumero = @IdNumero");
                            cmd.Parameters.AddWithValue("@IdNumero", filtros.IdNumero);
                        }
                        if (!string.IsNullOrWhiteSpace(filtros.NumeroCorporativo))
                        {
                            sb.Append(" AND NU.NumeroCorporativo LIKE @NumeroCorporativo");
                            cmd.Parameters.AddWithValue("@NumeroCorporativo", string.Format("%{0}%", filtros.NumeroCorporativo));
                        }
                        if (!string.IsNullOrWhiteSpace(filtros.Estado))
                        {
                            sb.Append(" AND NU.Estado = @Estado");
                            cmd.Parameters.AddWithValue("@Estado", filtros.Estado);
                        }
                        if (!string.IsNullOrWhiteSpace(filtros.Asignado))
                        {
                            sb.Append(" AND NU.Asignado = @Asignado");
                            cmd.Parameters.AddWithValue("@Asignado", filtros.Asignado);
                        }
                        if (!string.IsNullOrWhiteSpace(filtros.NroDocumento))
                        {
                            sb.Append(" AND NU.NroDocumento = @NroDocumento");
                            cmd.Parameters.AddWithValue("@NroDocumento", filtros.NroDocumento);
                        }
                    }
                    //sb.Append($" ORDER BY ---- DESC");    DESCOMENTAR LÍNEA SI SE DESEA ORDENAR POR ALGÚN CAMPO EN ESPECIAL
                    cmd.CommandText = sb.ToString();
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            lista = new List<ONUM_E>();
                            while (dr.Read())
                            {
                                ONUM_E obj = new ONUM_E();
                                if (!dr.IsDBNull(0)) { obj.IdNumero = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.NumeroCorporativo = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.Operador = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.Asignado = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.NroDocumento = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.Estado = dr.GetString(5); }
                                if (!dr.IsDBNull(6)) { obj.FechaRegistro = dr.GetString(6); }
                                if (!dr.IsDBNull(7)) { obj.FechaModificacion = dr.GetString(7); }
                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex, "ONUM_D - ListarNumeros");
            }
            return lista;
        }

        public ONUM_E ObtenerDatosNumero(int idNumero)
        {
            ONUM_E obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT NU.IdNumero, NU.NumeroCorporativo, NU.Operador, NU.Asignado, NU.NroDocumento, NU.Estado, CONVERT(varchar, NU.FechaRegistro, 103), CONVERT(varchar, NU.FechaModificacion, 103)");
                    sb.Append(" FROM dbo.ONUM NU");
                    sb.Append(" WHERE NU.IdNumero = @IdNumero");
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("@IdNumero", idNumero);
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            obj = new ONUM_E();
                            while (dr.Read())
                            {
                                if (!dr.IsDBNull(0)) { obj.IdNumero = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.NumeroCorporativo = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.Operador = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.Asignado = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.NroDocumento = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.Estado = dr.GetString(5); }
                                if (!dr.IsDBNull(6)) { obj.FechaRegistro = dr.GetString(6); }
                                if (!dr.IsDBNull(7)) { obj.FechaModificacion = dr.GetString(7); }
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex, "ONUM_D - ObtenerDatosNumero");
            }
            return obj;
        }

        public List<RptNumerosCorporativos_E> ExportarListaNumeros(RptNumerosCorporativos_E filtros)
        {
            List<RptNumerosCorporativos_E> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT NU.NumeroCorporativo, NU.Operador, CASE WHEN NU.Asignado = '1' THEN 'SI' ELSE 'NO' END AS DescripcionAsignado, NU.NroDocumento, CONCAT(EMP.Nombres, ' ', EMP.Apellidos),");
                    sb.Append(" ISNULL(EMP.Celular, ''), ISNULL(CAR.Nombre, ''), ISNULL(SD.Nombre, ''),");
                    sb.Append(" CASE WHEN NU.Estado = '1' THEN 'ACTIVO' ELSE 'INACTIVO' END AS DescripcionEstado, CONVERT(varchar, NU.FechaRegistro, 103), CONVERT(varchar, NU.FechaModificacion, 103)");
                    sb.Append(" FROM dbo.ONUM NU");
                    sb.Append(" LEFT JOIN rrhh.OEMPL EMP ON EMP.NroDocumento = NU.NroDocumento");
                    sb.Append(" LEFT JOIN rrhh.EMPL1 DL ON DL.IdOEMPL = EMP.IdOEMPL");
                    sb.Append(" LEFT JOIN dbo.OCARGO CAR ON CAR.IdCargo = DL.IdCargo");
                    sb.Append(" LEFT JOIN dbo.OSEDE SD ON SD.IdSede = DL.IdSede");
                    sb.Append(" WHERE 1 = 1");
                    //if (filtros != null)
                    //{
                    //    if (filtros.IdNumero > 0)
                    //    {
                    //        sb.Append(" AND NU.IdNumero = @IdNumero");
                    //        cmd.Parameters.AddWithValue("@IdNumero", filtros.IdNumero);
                    //    }
                    //    if (!string.IsNullOrWhiteSpace(filtros.NumeroCorporativo))
                    //    {
                    //        sb.Append(" AND NU.NumeroCorporativo LIKE @NumeroCorporativo");
                    //        cmd.Parameters.AddWithValue("@NumeroCorporativo", string.Format("%{0}%", filtros.NumeroCorporativo));
                    //    }
                    //    if (!string.IsNullOrWhiteSpace(filtros.Estado))
                    //    {
                    //        sb.Append(" AND NU.Estado = @Estado");
                    //        cmd.Parameters.AddWithValue("@Estado", filtros.Estado);
                    //    }
                    //}
                    //sb.Append($" ORDER BY ---- DESC");    DESCOMENTAR LÍNEA SI SE DESEA ORDENAR POR ALGÚN CAMPO EN ESPECIAL
                    cmd.CommandText = sb.ToString();
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            lista = new List<RptNumerosCorporativos_E>();
                            while (dr.Read())
                            {
                                RptNumerosCorporativos_E obj = new RptNumerosCorporativos_E();
                                if (!dr.IsDBNull(0)) { obj.NumeroCorporativo = dr.GetString(0); }
                                if (!dr.IsDBNull(1)) { obj.Operador = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.Asignado = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.NroDocumento = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.Empleado = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.Celular = dr.GetString(5); }
                                if (!dr.IsDBNull(6)) { obj.Cargo = dr.GetString(6); }
                                if (!dr.IsDBNull(7)) { obj.Sede = dr.GetString(7); }
                                if (!dr.IsDBNull(8)) { obj.Estado = dr.GetString(8); }
                                if (!dr.IsDBNull(9)) { obj.FechaRegistro = dr.GetString(9); }
                                if (!dr.IsDBNull(10)) { obj.FechaModificacion = dr.GetString(10); }
                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex, "ONUM_D - ExportarListaNumeros");
            }
            return lista;
        }

        private void RegistrarAuditoriaCambios(string campo, string valorAnterior, string valorActual, int idNumero, int registradoPor)
        {
            if (!string.IsNullOrWhiteSpace(valorActual) && !valorAnterior.Equals(valorActual))
            {
                new AUD_ONUM_D().RegistrarAuditoria(new AUD_ONUM_E
                {
                    IdNumero = idNumero,
                    Campo = campo,
                    ValorAnterior = valorAnterior.Trim(),
                    ValorActual = valorActual.Trim(),
                    RegistradoPor = registradoPor
                });
            }
        }

        private void RegistrarError(Exception ex, string nombreArchivo)
        {
            File.AppendAllText(uti.directorioLogs + nombreArchivo + ".txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");
        }
    }
}