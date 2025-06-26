using Capa_Datos.RecursosHumanos_DAO.TablasSQL;
using Capa_Entidad;
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
    public class EMPL1_D
    {
        private readonly Utilitarios uti = new Utilitarios();

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
                    sb.Append("SELECT DL.IdEMPL1, DL.IdOEMPL, DL.TipoContrato, ISNULL(CONVERT(varchar, DL.FechaContratacion, 103), ''), DL.Salario, ISNULL(CONVERT(varchar, DL.FechaCese, 103), ''), DL.IdSede, DL.IdDepartamento, DL.IdArea, DL.IdCargo, DL.TurnoTrabajo, DL.Discapacidad, DL.CondicionLaboral")
                    .Append(" FROM rrhh.EMPL1 DL")
                    .Append(" WHERE 1 = 1");
                    if (filtros != null)
                    {
                        if (filtros.IdOEMPL > 0)
                        {
                            sb.Append(" AND DL.IdOEMPL = @EmpleadoID");
                            cmd.Parameters.AddWithValue("@EmpleadoID", filtros.IdOEMPL);
                        }
                        if (filtros.IdSede > 0)
                        {
                            sb.Append(" AND DL.IdSede = @SedeID");
                            cmd.Parameters.AddWithValue("@SedeID", filtros.IdSede);
                        }
                        if (filtros.IdDepartamento > 0)
                        {
                            sb.Append(" AND DL.IdDepartamento = @DepartamentoID");
                            cmd.Parameters.AddWithValue("@DepartamentoID", filtros.IdDepartamento);
                        }
                        if (filtros.IdArea > 0)
                        {
                            sb.Append(" AND DL.IdArea = @AreaID");
                            cmd.Parameters.AddWithValue("@AreaID", filtros.IdArea);
                        }
                        if (filtros.IdCargo > 0)
                        {
                            sb.Append(" AND DL.IdCargo = @CargoID");
                            cmd.Parameters.AddWithValue("@CargoID", filtros.IdCargo);
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
                                    IdEMPL1 = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
                                    IdOEMPL = dr.IsDBNull(1) ? 0 : dr.GetInt32(1),
                                    TipoContrato = dr.IsDBNull(2) ? string.Empty : dr.GetString(2),
                                    FechaContratacion = dr.IsDBNull(3) ? string.Empty : dr.GetString(3),
                                    Salario = dr.IsDBNull(4) ? 0 : dr.GetDecimal(4),
                                    FechaCese = dr.IsDBNull(5) ? string.Empty : dr.GetString(5),
                                    IdSede = dr.IsDBNull(6) ? 0 : dr.GetInt32(6),
                                    IdDepartamento = dr.IsDBNull(7) ? 0 : dr.GetInt32(7),
                                    IdArea = dr.IsDBNull(8) ? 0 : dr.GetInt32(8),
                                    IdCargo = dr.IsDBNull(9) ? 0 : dr.GetInt32(9),
                                    TurnoTrabajo = dr.IsDBNull(10) ? string.Empty : dr.GetString(10),
                                    Discapacidad = dr.IsDBNull(11) ? string.Empty : dr.GetString(11),
                                    CondicionLaboral = dr.IsDBNull(12) ? string.Empty : dr.GetString(12)
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

        public Dictionary<string, string> BuscarAnexoCorreoDuplicado(string anexo, string correoCorporativo, int id)
        {
            var obj = new Dictionary<string, string>();
            try
            {
                var helper = new DBHelper();
                string queryAnexo = "SELECT COUNT(DL.IdEMPL1) FROM rrhh.EMPL1 DL WHERE IdOEMPL != @IdOEMPL AND DL.AnexoCorporativo = @AnexoCorporativo";
                SqlParameter[] parametrosAnexo = new SqlParameter[]
                {
                        new SqlParameter("@AnexoCorporativo", anexo),
                        new SqlParameter("@IdOEMPL", id)
                };
                int cantidadAnexo = helper.ContarRegistros(queryAnexo, parametrosAnexo);
                if (cantidadAnexo > 0) { obj.Add("AnexoCorporativo", anexo); }
                string queryCorreo = "SELECT COUNT(DL.IdEMPL1) FROM rrhh.EMPL1 DL WHERE IdOEMPL != @IdOEMPL AND DL.CorreoCorporativo = @CorreoCorporativo";
                SqlParameter[] parametrosCorreo = new SqlParameter[]
                {
                        new SqlParameter("@CorreoCorporativo", correoCorporativo),
                        new SqlParameter("@IdOEMPL", id)
                };
                int cantidadCorreo = helper.ContarRegistros(queryCorreo, parametrosCorreo);
                if (cantidadCorreo > 0) { obj.Add("CorreoCorporativo", correoCorporativo); }
            }
            catch (Exception ex)
            {
                RegistrarError(ex, "EMPL1_D - BuscarAnexoCorreoDuplicado");
            }
            return obj;
        }

        public EMPL1_E ObtenerDatosLaborales(int idOEMPL)
        {
            EMPL1_E obj = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT DL.IdEMPL1, DL.IdOEMPL, DL.TipoContrato, CONVERT(varchar, DL.FechaContratacion, 103), DL.Salario, CONVERT(varchar, DL.FechaCese, 103), DL.IdSede, DL.IdDepartamento,");
                    sb.Append(" DL.IdArea, DL.IdCargo, DL.IdNumeroCorporativo, DL.AnexoCorporativo, DL.CorreoCorporativo, DL.TurnoTrabajo, DL.Discapacidad, ISNULL(DL.CondicionLaboral, ''), DEP.Nombre, ISNULL(CAR.Nombre, ''), ISNULL(NU.NumeroCorporativo, '')");
                    sb.Append(" FROM rrhh.EMPL1 DL");
                    sb.Append(" INNER JOIN dbo.ODPTO DEP ON DEP.Id = DL.IdDepartamento");
                    sb.Append(" LEFT JOIN dbo.CARGO CAR ON CAR.Id= DL.IdCargo");
                    sb.Append(" LEFT JOIN dbo.ONUM NU ON NU.IdNumero = DL.IdNumeroCorporativo");
                    sb.Append(" WHERE DL.IdOEMPL = @IdOEMPL");
                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("@IdOEMPL", idOEMPL);
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            obj = new EMPL1_E();
                            while (dr.Read())
                            {
                                if (!dr.IsDBNull(0)) { obj.IdEMPL1 = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.IdOEMPL = dr.GetInt32(1); }
                                if (!dr.IsDBNull(2)) { obj.TipoContrato = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.FechaContratacion = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.Salario = dr.GetDecimal(4); }
                                if (!dr.IsDBNull(5)) { obj.FechaCese = dr.GetString(5); }
                                if (!dr.IsDBNull(6)) { obj.IdSede = dr.GetInt32(6); }
                                if (!dr.IsDBNull(7)) { obj.IdDepartamento = dr.GetInt32(7); }
                                if (!dr.IsDBNull(8)) { obj.IdArea = dr.GetInt32(8); }
                                if (!dr.IsDBNull(9)) { obj.IdCargo = dr.GetInt32(9); }
                                if (!dr.IsDBNull(10)) { obj.IdNumeroCorporativo = dr.GetInt32(10); }
                                if (!dr.IsDBNull(11)) { obj.AnexoCorporativo = dr.GetString(11); }
                                if (!dr.IsDBNull(12)) { obj.CorreoCorporativo = dr.GetString(12); }
                                if (!dr.IsDBNull(13)) { obj.TurnoTrabajo = dr.GetString(13); }
                                if (!dr.IsDBNull(14)) { obj.Discapacidad = dr.GetString(14); }
                                if (!dr.IsDBNull(15)) { obj.CondicionLaboral = dr.GetString(15); }
                                if (!dr.IsDBNull(16)) { obj.NombreDepartamento = dr.GetString(16); }
                                if (!dr.IsDBNull(17)) { obj.NombreCargo = dr.GetString(17); }
                                if (!dr.IsDBNull(18)) { obj.NumeroCorporativo = dr.GetString(18); }
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex, "EMPL1_D - ObtenerDatosLaborales");
            }
            return obj;
        }

        private void RegistrarError(Exception ex, string nombreArchivo)
        {
            File.AppendAllText(uti.directorioLogs + nombreArchivo + ".txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");
        }
    }
}