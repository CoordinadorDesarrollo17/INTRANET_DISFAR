using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using DocumentFormat.OpenXml.Office.Word;
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
    public class HIST_OEMPL_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public void RegistrarHistoricoEmpleado(OEMPL_E datos)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("rrhh.MANT_HIST_OEMPL", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", "INS");
                    cmd.Parameters.AddWithValue("@IdOEMPL", datos.IdOEMPL);
                    cmd.Parameters.AddWithValue("@EstadoCivil", datos.EstadoCivil);
                    cmd.Parameters.AddWithValue("@Direccion", datos.Direccion);
                    cmd.Parameters.AddWithValue("@Ubigeo", datos.Ubigeo);
                    cmd.Parameters.AddWithValue("@ReferenciaDomicilio", datos.ReferenciaDomicilio);
                    cmd.Parameters.AddWithValue("@CorreoElectronico", datos.CorreoElectronico);
                    cmd.Parameters.AddWithValue("@Celular", datos.Celular);
                    cmd.Parameters.AddWithValue("@NombreContactoEmergencia", datos.NombreContactoEmergencia);
                    cmd.Parameters.AddWithValue("@CelularContactoEmergencia", datos.CelularContactoEmergencia);
                    cmd.Parameters.AddWithValue("@RegistradoPor", datos.RegistradoPor);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    RegistrarError(ex, "HIST_OEMPL_D - RegistrarHistoricoEmpleado");
                }
            }
        }

        public void RegistrarHistoricoDatosLaborales(EMPL1_E datos)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd2 = new SqlCommand("rrhh.MANT_HIST_OEMPL", cn);
                    cmd2.CommandType = CommandType.StoredProcedure;

                    cmd2.Parameters.AddWithValue("@Accion", "UPD");
                    cmd2.Parameters.AddWithValue("@IdOEMPL", datos.IdOEMPL);
                    cmd2.Parameters.AddWithValue("@TipoContrato", datos.TipoContrato ?? string.Empty);
                    cmd2.Parameters.AddWithValue("@FechaContratacion", datos.FechaContratacion);
                    cmd2.Parameters.AddWithValue("@Salario", datos.Salario);
                    cmd2.Parameters.AddWithValue("@FechaCese", datos.FechaCese);
                    cmd2.Parameters.AddWithValue("@IdSede", datos.IdSede);
                    cmd2.Parameters.AddWithValue("@IdDepartamento", datos.IdDepartamento);
                    cmd2.Parameters.AddWithValue("@IdArea", datos.IdArea);
                    cmd2.Parameters.AddWithValue("@IdCargo", datos.IdCargo);
                    cmd2.Parameters.AddWithValue("@IdNumeroCorporativo", datos.IdNumeroCorporativo);
                    cmd2.Parameters.AddWithValue("@AnexoCorporativo", datos.AnexoCorporativo ?? string.Empty);
                    cmd2.Parameters.AddWithValue("@CorreoCorporativo", datos.CorreoCorporativo ?? string.Empty);
                    cmd2.Parameters.AddWithValue("@TurnoTrabajo", datos.TurnoTrabajo ?? string.Empty);
                    cmd2.Parameters.AddWithValue("@Discapacidad",datos.Discapacidad ?? string.Empty);

                    cmd2.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    RegistrarError(ex, "HIST_OEMPL_D - RegistrarHistoricoDatosLaborales");
                }
            }
        }

        private void RegistrarError(Exception ex, string nombreArchivo)
        {
            File.AppendAllText(uti.directorioLogs + nombreArchivo + ".txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");
        }
    }
}
