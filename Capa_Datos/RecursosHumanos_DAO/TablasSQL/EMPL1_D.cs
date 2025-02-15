using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Capa_Entidad;

namespace Capa_Datos.RecursosHumanos_DAO
{
    public class EMPL1_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<EMPL1_E> ListarDatosLaborales(EMPL1_E filtros)
        {
            List<EMPL1_E> lista = new List<EMPL1_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.Append("SELECT DL.Id, DL.EmpleadoID, DL.TipoContrato, ISNULL(CONVERT(varchar, DL.FechaContratacion, 103), ''), DL.Salario, ISNULL(CONVERT(varchar, DL.FechaCese, 103), ''), DL.SedeID, DL.DepartamentoID, DL.AreaID, DL.CargoID, DL.TurnoTrabajo, DL.Discapacidad, DL.CondicionLaboral")
                    .Append(" FROM rrhh.EMPL1 DL")
                    .Append(" WHERE 1 = 1");

                    if (filtros != null)
                    {
                        if (filtros.EmpleadoID > 0)
                        {
                            sb.Append(" AND DL.EmpleadoID = @EmpleadoID");
                            cmd.Parameters.AddWithValue("@EmpleadoID", filtros.EmpleadoID);
                        }

                        if (filtros.SedeID > 0)
                        {
                            sb.Append(" AND DL.SedeID = @SedeID");
                            cmd.Parameters.AddWithValue("@SedeID", filtros.SedeID);
                        }

                        if (filtros.DepartamentoID > 0)
                        {
                            sb.Append(" AND DL.DepartamentoID = @DepartamentoID");
                            cmd.Parameters.AddWithValue("@DepartamentoID", filtros.DepartamentoID);
                        }

                        if (filtros.AreaID > 0)
                        {
                            sb.Append(" AND DL.AreaID = @AreaID");
                            cmd.Parameters.AddWithValue("@AreaID", filtros.AreaID);
                        }

                        if (filtros.CargoID > 0)
                        {
                            sb.Append(" AND DL.CargoID = @CargoID");
                            cmd.Parameters.AddWithValue("@CargoID", filtros.CargoID);
                        }
                    }

                    cmd.CommandText = sb.ToString();

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var obj = new EMPL1_E
                                {
                                    Id = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
                                    EmpleadoID = dr.IsDBNull(1) ? 0 : dr.GetInt32(1),
                                    TipoContrato = dr.IsDBNull(2) ? string.Empty : dr.GetString(2),
                                    FechaContratacion = dr.IsDBNull(3) ? string.Empty : dr.GetString(3),
                                    Salario = dr.IsDBNull(4) ? 0 : dr.GetDecimal(4),
                                    FechaCese = dr.IsDBNull(5) ? string.Empty : dr.GetString(5),
                                    SedeID = dr.IsDBNull(6) ? 0 : dr.GetInt32(6),
                                    DepartamentoID = dr.IsDBNull(7) ? 0 : dr.GetInt32(7),
                                    AreaID = dr.IsDBNull(8) ? 0 : dr.GetInt32(8),
                                    CargoID = dr.IsDBNull(9) ? 0 : dr.GetInt32(9),
                                    TurnoTrabajo = dr.IsDBNull(10) ? string.Empty : dr.GetString(10),
                                    Discapacidad = dr.IsDBNull(11) ? string.Empty : dr.GetString(11),
                                    CondicionLaboral = dr.IsDBNull(12) ? string.Empty : dr.GetString(12),
                                };

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex, "EMPL1_D - ListarEmpleados");
            }

            return lista;
        }

        //public string RegistrarDatosLaborales(EMPL1_E datos)
        //{
        //    string mensajeError;

        //    using (SqlConnection cn = new SqlConnection(uti.cadSql))
        //    {
        //        cn.Open();

        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand("rrhh.MANT_EMPL1", cn);
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.AddWithValue("@Accion", "INS");
        //            cmd.Parameters.AddWithValue("@IdOEMPL", datos.IdOEMPL);
        //            cmd.Parameters.AddWithValue("@TipoContrato", datos.TipoContrato);
        //            cmd.Parameters.AddWithValue("@FechaContratacion", datos.FechaContratacion);
        //            cmd.Parameters.AddWithValue("@Salario", datos.Salario);
        //            cmd.Parameters.AddWithValue("@FechaCese", datos.FechaCese);
        //            cmd.Parameters.AddWithValue("@IdSede", datos.IdSede);
        //            cmd.Parameters.AddWithValue("@IdDepartamento", datos.IdDepartamento);
        //            cmd.Parameters.AddWithValue("@IdArea", datos.IdArea);
        //            cmd.Parameters.AddWithValue("@IdCargo", datos.IdCargo);
        //            cmd.Parameters.AddWithValue("@IdNumeroCorporativo", datos.IdNumeroCorporativo);
        //            cmd.Parameters.AddWithValue("@AnexoCorporativo", datos.AnexoCorporativo);
        //            cmd.Parameters.AddWithValue("@CorreoCorporativo", datos.CorreoCorporativo);
        //            cmd.Parameters.AddWithValue("@TurnoTrabajo", datos.TurnoTrabajo);
        //            cmd.Parameters.AddWithValue("@Discapacidad", datos.Discapacidad);

        //            cmd.ExecuteNonQuery();

        //            mensajeError = string.Empty;
        //        }
        //        catch (Exception ex)
        //        {
        //            RegistrarError(ex, "EMPL1_D - RegistrarDatosLaborales");        // Registro de error
        //            mensajeError = "Ocurrió un error al registrar los datos laborales. Por favor, comuníquese con el área de Sistemas para más información.";
        //        }
        //    }

        //    return mensajeError;
        //}

        //public string EditarDatosLaborales(EMPL1_E datos, EMPL1_E empleado)
        //{
        //    string mensajeError;

        //    using (SqlConnection cn = new SqlConnection(uti.cadSql))
        //    {
        //        cn.Open();

        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand("rrhh.MANT_EMPL1", cn);
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.AddWithValue("@Accion", "UPD");
        //            cmd.Parameters.AddWithValue("@IdOEMPL", datos.IdOEMPL);
        //            cmd.Parameters.AddWithValue("@TipoContrato", datos.TipoContrato);
        //            cmd.Parameters.AddWithValue("@FechaContratacion", datos.FechaContratacion);
        //            cmd.Parameters.AddWithValue("@Salario", datos.Salario);
        //            cmd.Parameters.AddWithValue("@FechaCese", datos.FechaCese);
        //            cmd.Parameters.AddWithValue("@IdSede", datos.IdSede);
        //            cmd.Parameters.AddWithValue("@IdDepartamento", datos.IdDepartamento);
        //            cmd.Parameters.AddWithValue("@IdArea", datos.IdArea);
        //            cmd.Parameters.AddWithValue("@IdCargo", datos.IdCargo);
        //            cmd.Parameters.AddWithValue("@IdNumeroCorporativo", datos.IdNumeroCorporativo);
        //            cmd.Parameters.AddWithValue("@AnexoCorporativo", datos.AnexoCorporativo);
        //            cmd.Parameters.AddWithValue("@CorreoCorporativo", datos.CorreoCorporativo);
        //            cmd.Parameters.AddWithValue("@TurnoTrabajo", datos.TurnoTrabajo);
        //            cmd.Parameters.AddWithValue("@Discapacidad", datos.Discapacidad);
        //            cmd.Parameters.AddWithValue("@EstadoOEMPL", empleado.Estado);
        //            cmd.Parameters.AddWithValue("@NroDocumento", empleado.NroDocumento);

        //            cmd.ExecuteNonQuery();

        //            if (datos.IdOEMPL > 0)
        //            {
        //                new HIST_OEMPL_D().RegistrarHistoricoDatosLaborales(datos);
        //            }

        //            mensajeError = string.Empty;
        //        }
        //        catch (Exception ex)
        //        {
        //            RegistrarError(ex, "EMPL1_D - EditarDatosLaborales");        // Registro de error
        //            mensajeError = "Ocurrió un error al editar datos laborales. Por favor, comuníquese con el área de Sistemas para más información.";
        //        }
        //    }

        //    return mensajeError;
        //}

        //public string EliminarDatosLaborales(int idOEMPL)
        //{
        //    string mensajeError;

        //    using (SqlConnection cn = new SqlConnection(uti.cadSql))
        //    {
        //        cn.Open();

        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand("rrhh.MANT_EMPL1", cn);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@Accion", "DEL");
        //            cmd.Parameters.AddWithValue("@IdOEMPL", idOEMPL);
        //            cmd.ExecuteNonQuery();
        //            mensajeError = string.Empty;
        //        }
        //        catch (Exception ex)
        //        {
        //            RegistrarError(ex, "EMPL1_D - EliminarDatosLaborales");        // Registro de error
        //            mensajeError = "Ocurrió un error al eliminar datos laborales. Por favor, comuníquese con el área de Sistemas para más información.";
        //        }
        //    }

        //    return mensajeError;
        //}

        //public Dictionary<string, string> BuscarAnexoCorreoDuplicado(string anexo, string correoCorporativo, int id)
        //{
        //    Dictionary<string, string> obj = null;

        //    try
        //    {
        //        using (SqlConnection cn = new SqlConnection(uti.cadSql))
        //        {
        //            SqlCommand cmd = new SqlCommand();
        //            cmd.Connection = cn;

        //            StringBuilder sb = new StringBuilder();

        //            sb.Append("SELECT COUNT(DL.IdEMPL1)");
        //            sb.Append(" FROM rrhh.EMPL1 DL");
        //            sb.Append(" WHERE IdOEMPL != @IdOEMPL AND ((DL.AnexoCorporativo != '' AND DL.AnexoCorporativo = @AnexoCorporativo) OR (DL.CorreoCorporativo != '' AND DL.CorreoCorporativo = @CorreoCorporativo))");

        //            cmd.CommandText = sb.ToString();
        //            cmd.Parameters.AddWithValue("@AnexoCorporativo", anexo);
        //            cmd.Parameters.AddWithValue("@CorreoCorporativo", correoCorporativo);
        //            cmd.Parameters.AddWithValue("@IdOEMPL", id);

        //            cn.Open();
        //            int cantidad = (int)cmd.ExecuteScalar(); // Ejecutar la consulta y obtener el valor escalar                    
        //            cn.Close();

        //            if (cantidad > 0)
        //            {
        //                Crear y llenar el diccionario solo si se encuentran registros
        //                obj = new Dictionary<string, string>
        //                            {
        //                                { "AnexoCorporativo", anexo },
        //                                { "CorreoCorporativo", correoCorporativo }
        //                            };
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RegistrarError(ex, "EMPL1_D - BuscarAnexoCorreoDuplicado");
        //    }

        //    return obj;
        //}

        //public EMPL1_E ObtenerDatosLaborales(int idOEMPL)
        //{
        //    EMPL1_E obj = null;

        //    try
        //    {
        //        using (SqlConnection cn = new SqlConnection(uti.cadSql))
        //        {
        //            SqlCommand cmd = new SqlCommand();
        //            cmd.Connection = cn;

        //            StringBuilder sb = new StringBuilder();

        //            sb.Append("SELECT DL.IdEMPL1, DL.IdOEMPL, DL.TipoContrato, CONVERT(varchar, DL.FechaContratacion, 103), DL.Salario, CONVERT(varchar, DL.FechaCese, 103), DL.IdSede, DL.IdDepartamento,");
        //            sb.Append(" DL.IdArea, DL.IdCargo, DL.IdNumeroCorporativo, DL.AnexoCorporativo, DL.CorreoCorporativo, DL.TurnoTrabajo, DL.Discapacidad, DEP.Nombre, ISNULL(CAR.Nombre, ''), ISNULL(NU.NumeroCorporativo, '')");
        //            sb.Append(" FROM rrhh.EMPL1 DL");
        //            sb.Append(" INNER JOIN dbo.ODPTO DEP ON DEP.IdDepartamento = DL.IdDepartamento");
        //            sb.Append(" LEFT JOIN dbo.OCARGO CAR ON CAR.IdCargo = DL.IdCargo");
        //            sb.Append(" LEFT JOIN dbo.ONUM NU ON NU.IdNumero = DL.IdNumeroCorporativo");
        //            sb.Append(" WHERE DL.IdOEMPL = @IdOEMPL");

        //            cmd.CommandText = sb.ToString();
        //            cmd.Parameters.AddWithValue("@IdOEMPL", idOEMPL);

        //            cn.Open();
        //            using (SqlDataReader dr = cmd.ExecuteReader())
        //            {
        //                if (dr.HasRows)
        //                {
        //                    obj = new EMPL1_E();

        //                    while (dr.Read())
        //                    {
        //                        if (!dr.IsDBNull(0)) { obj.IdEMPL1 = dr.GetInt32(0); }
        //                        if (!dr.IsDBNull(1)) { obj.IdOEMPL = dr.GetInt32(1); }
        //                        if (!dr.IsDBNull(2)) { obj.TipoContrato = dr.GetString(2); }
        //                        if (!dr.IsDBNull(3)) { obj.FechaContratacion = dr.GetString(3); }
        //                        if (!dr.IsDBNull(4)) { obj.Salario = dr.GetDecimal(4); }
        //                        if (!dr.IsDBNull(5)) { obj.FechaCese = dr.GetString(5); }
        //                        if (!dr.IsDBNull(6)) { obj.IdSede = dr.GetInt32(6); }
        //                        if (!dr.IsDBNull(7)) { obj.IdDepartamento = dr.GetInt32(7); }
        //                        if (!dr.IsDBNull(8)) { obj.IdArea = dr.GetInt32(8); }
        //                        if (!dr.IsDBNull(9)) { obj.IdCargo = dr.GetInt32(9); }
        //                        if (!dr.IsDBNull(10)) { obj.IdNumeroCorporativo = dr.GetInt32(10); }
        //                        if (!dr.IsDBNull(11)) { obj.AnexoCorporativo = dr.GetString(11); }
        //                        if (!dr.IsDBNull(12)) { obj.CorreoCorporativo = dr.GetString(12); }
        //                        if (!dr.IsDBNull(13)) { obj.TurnoTrabajo = dr.GetString(13); }
        //                        if (!dr.IsDBNull(14)) { obj.Discapacidad = dr.GetString(14); }
        //                        if (!dr.IsDBNull(15)) { obj.NombreDepartamento = dr.GetString(15); }
        //                        if (!dr.IsDBNull(16)) { obj.NombreCargo = dr.GetString(16); }
        //                        if (!dr.IsDBNull(17)) { obj.NumeroCorporativo = dr.GetString(17); }
        //                    }
        //                }
        //            }
        //            cn.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RegistrarError(ex, "EMPL1_D - ObtenerDatosLaborales");
        //    }

        //    return obj;
        //}

        private void RegistrarError(Exception ex, string nombreArchivo)
        {
            File.AppendAllText(uti.directorioLogs + nombreArchivo + ".txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");
        }
    }
}