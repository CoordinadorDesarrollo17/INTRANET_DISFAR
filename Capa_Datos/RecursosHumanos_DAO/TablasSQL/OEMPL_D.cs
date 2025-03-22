using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.RecursosHumanos_DAO
{
    public class OEMPL_D
    {
        readonly Utilitarios uti = new Utilitarios();
        public string RegistrarEmpleado(OEMPL_E datosEmpleado, EMPL1_E datosLaborales)
        {
            string mensajeError = "Ocurrió un error al registrar al empleado. Por favor, comunicarse con SISTEMAS.";
            int idEmpleado = 0;

            // Primer bloque: ejecutar en la primera conexión y transacción
            using (SqlConnection cn1 = new SqlConnection(uti.cadSql))
            {
                cn1.Open();
                using (SqlTransaction transaction1 = cn1.BeginTransaction())
                {
                    try
                    {
                        // Ejecutar primer comando para rrhh.MANT_OEMPL
                        using (SqlCommand cmd = new SqlCommand("rrhh.MANT_OEMPL", cn1, transaction1))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Accion", "INS");
                            cmd.Parameters.AddWithValue("@Nombres", datosEmpleado.Nombres);
                            cmd.Parameters.AddWithValue("@Apellidos", datosEmpleado.Apellidos);
                            cmd.Parameters.AddWithValue("@TipoDocumento", "DNI");
                            cmd.Parameters.AddWithValue("@NroDocumento", datosEmpleado.NroDocumento);
                            cmd.Parameters.AddWithValue("@FechaNacimiento", datosEmpleado.FechaNacimiento);
                            cmd.Parameters.AddWithValue("@EstadoCivil", datosEmpleado.EstadoCivil);
                            cmd.Parameters.AddWithValue("@Genero", datosEmpleado.Genero);
                            cmd.Parameters.AddWithValue("@Direccion", datosEmpleado.Direccion);
                            cmd.Parameters.AddWithValue("@Ubigeo", datosEmpleado.Ubigeo);
                            cmd.Parameters.AddWithValue("@ReferenciaDomicilio", datosEmpleado.ReferenciaDomicilio);
                            cmd.Parameters.AddWithValue("@Nacionalidad", datosEmpleado.Nacionalidad);
                            cmd.Parameters.AddWithValue("@CorreoElectronico", datosEmpleado.CorreoElectronico);
                            cmd.Parameters.AddWithValue("@Celular", datosEmpleado.Celular);
                            cmd.Parameters.AddWithValue("@LicenciaConducir", datosEmpleado.LicenciaConducir);
                            cmd.Parameters.AddWithValue("@NombreContactoEmergencia", datosEmpleado.NombreContactoEmergencia);
                            cmd.Parameters.AddWithValue("@CelularContactoEmergencia", datosEmpleado.CelularContactoEmergencia);
                            cmd.Parameters.AddWithValue("@RegistradoPor", datosEmpleado.RegistradoPor);
                            cmd.Parameters.AddWithValue("@PrefijoId", datosEmpleado.PrefijoId);

                            SqlParameter paramIdEmpleado = cmd.Parameters.Add("@IdOEMPL", SqlDbType.Int);
                            paramIdEmpleado.Direction = ParameterDirection.Output;

                            cmd.ExecuteNonQuery();

                            idEmpleado = Convert.ToInt32(paramIdEmpleado.Value);
                        }

                        // Actualizar datosLaborales con el idEmpleado obtenido
                        datosLaborales.IdOEMPL = idEmpleado;

                        // Ejecutar segundo comando para rrhh.MANT_EMPL1
                        using (SqlCommand cmd = new SqlCommand("rrhh.MANT_EMPL1", cn1, transaction1))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Accion", "INS");
                            cmd.Parameters.AddWithValue("@IdOEMPL", datosLaborales.IdOEMPL);
                            cmd.Parameters.AddWithValue("@TipoContrato", datosLaborales.TipoContrato);
                            cmd.Parameters.AddWithValue("@FechaContratacion", datosLaborales.FechaContratacion);
                            cmd.Parameters.AddWithValue("@Salario", datosLaborales.Salario);
                            cmd.Parameters.AddWithValue("@FechaCese", datosLaborales.FechaCese);
                            cmd.Parameters.AddWithValue("@SedeID", datosLaborales.IdSede);
                            cmd.Parameters.AddWithValue("@DepartamentoID", datosLaborales.IdDepartamento);
                            cmd.Parameters.AddWithValue("@AreaID", datosLaborales.IdArea);
                            cmd.Parameters.AddWithValue("@CargoID", datosLaborales.IdCargo);
                            cmd.Parameters.AddWithValue("@IdNumeroCorporativo", datosLaborales.IdNumeroCorporativo);
                            cmd.Parameters.AddWithValue("@AnexoCorporativo", datosLaborales.AnexoCorporativo);
                            cmd.Parameters.AddWithValue("@CorreoCorporativo", datosLaborales.CorreoCorporativo);
                            cmd.Parameters.AddWithValue("@TurnoTrabajo", datosLaborales.TurnoTrabajo);
                            cmd.Parameters.AddWithValue("@Discapacidad", datosLaborales.Discapacidad);
                            cmd.Parameters.AddWithValue("@CondicionLaboral", datosLaborales.CondicionLaboral);

                            cmd.ExecuteNonQuery();
                        }

                        // Confirmar la transacción en la primera conexión
                        transaction1.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Si ocurre un error, revertir la transacción en la primera conexión
                        transaction1.Rollback();
                        RegistrarError(ex, "OEMPL_D - RegistrarEmpleado (Conexión 1)");
                        return mensajeError; // Salir después de manejar el error
                    }
                }
            }

            // Segundo bloque: ejecutar en la segunda conexión y transacción
            //using (SqlConnection cn2 = new SqlConnection(uti.cadSql2))
            //{
            //    cn2.Open();
            //    using (SqlTransaction transaction2 = cn2.BeginTransaction())
            //    {
            //        try
            //        {
            //            // Ejecutar tercer comando para rrhh.MANT_OEMPL
            //            using (SqlCommand cmd = new SqlCommand("rrhh.MANT_OEMPL", cn2, transaction2))
            //            {
            //                cmd.CommandType = CommandType.StoredProcedure;
            //                cmd.Parameters.AddWithValue("@Accion", "INS");
            //                cmd.Parameters.AddWithValue("@Nombres", datosEmpleado.Nombres);
            //                cmd.Parameters.AddWithValue("@Apellidos", datosEmpleado.Apellidos);
            //                cmd.Parameters.AddWithValue("@TipoDocumento", "DNI");
            //                cmd.Parameters.AddWithValue("@NroDocumento", datosEmpleado.NroDocumento);
            //                cmd.Parameters.AddWithValue("@FechaNacimiento", datosEmpleado.FechaNacimiento);
            //                cmd.Parameters.AddWithValue("@EstadoCivil", "");
            //                cmd.Parameters.AddWithValue("@Genero", "");
            //                cmd.Parameters.AddWithValue("@Direccion", "");
            //                cmd.Parameters.AddWithValue("@UbigeoID", datosEmpleado.UbigeoID);
            //                cmd.Parameters.AddWithValue("@ReferenciaDomicilio", "");
            //                cmd.Parameters.AddWithValue("@Nacionalidad", "");
            //                cmd.Parameters.AddWithValue("@CorreoElectronico", "");
            //                cmd.Parameters.AddWithValue("@Celular", "");
            //                cmd.Parameters.AddWithValue("@LicenciaConducir", "");
            //                cmd.Parameters.AddWithValue("@NombreContactoEmergencia", "");
            //                cmd.Parameters.AddWithValue("@CelularContactoEmergencia", "");
            //                cmd.Parameters.AddWithValue("@UsuarioOperacion", datosEmpleado.UsuarioOperacion);

            //                SqlParameter paramIdEmpleado = cmd.Parameters.Add("@Id", SqlDbType.Int);
            //                paramIdEmpleado.Direction = ParameterDirection.Output;

            //                cmd.ExecuteNonQuery();

            //                idEmpleado = Convert.ToInt32(paramIdEmpleado.Value);
            //            }

            //            // Actualizar datosLaborales con el idEmpleado obtenido
            //            datosLaborales.Id = idEmpleado;
            //            datosLaborales.UsuarioOperacion = datosEmpleado.UsuarioOperacion;

            //            // Ejecutar cuarto comando para rrhh.MANT_EMPL1
            //            using (SqlCommand cmd = new SqlCommand("rrhh.MANT_EMPL1", cn2, transaction2))
            //            {
            //                cmd.CommandType = CommandType.StoredProcedure;

            //                cmd.Parameters.AddWithValue("@Accion", "INS");
            //                cmd.Parameters.AddWithValue("@EmpleadoID", datosLaborales.Id);
            //                cmd.Parameters.AddWithValue("@TipoContrato", datosLaborales.TipoContrato);
            //                cmd.Parameters.AddWithValue("@FechaContratacion", datosLaborales.FechaContratacion);
            //                cmd.Parameters.AddWithValue("@Salario", datosLaborales.Salario);
            //                cmd.Parameters.AddWithValue("@FechaCese", datosLaborales.FechaCese);
            //                cmd.Parameters.AddWithValue("@SedeID", datosLaborales.SedeID);
            //                cmd.Parameters.AddWithValue("@DepartamentoID", datosLaborales.DepartamentoID);
            //                cmd.Parameters.AddWithValue("@AreaID", datosLaborales.AreaID);
            //                cmd.Parameters.AddWithValue("@CargoID", datosLaborales.CargoID);
            //                cmd.Parameters.AddWithValue("@TurnoTrabajo", datosLaborales.TurnoTrabajo);
            //                cmd.Parameters.AddWithValue("@Discapacidad", datosLaborales.Discapacidad);
            //                cmd.Parameters.AddWithValue("@CondicionLaboral", datosLaborales.CondicionLaboral);
            //                cmd.Parameters.AddWithValue("@UsuarioOperacion", datosLaborales.UsuarioOperacion);

            //                cmd.ExecuteNonQuery();
            //            }

            //            // Confirmar la transacción en la segunda conexión
            //            transaction2.Commit();
            //        }
            //        catch (Exception ex)
            //        {
            //            // Si ocurre un error, revertir la transacción en la segunda conexión
            //            transaction2.Rollback();
            //            RegistrarError(ex, "OEMPL_D - RegistrarEmpleado (Conexión 2)");
            //            return mensajeError; // Salir después de manejar el error
            //        }
            //    }
            //}

            return string.Empty;
        }
        public string EditarEmpleado(OEMPL_E empleado, EMPL1_E datosLaborales)
        {
            string mensajeError = "Ocurrió un error al editar al empleado. Por favor, comunicarse con SISTEMAS.";

            // Primer bloque: ejecutar en la primera conexión y transacción
            using (SqlConnection cn1 = new SqlConnection(uti.cadSql))
            {
                cn1.Open();
                using (SqlTransaction transaction1 = cn1.BeginTransaction())
                {
                    try
                    {
                        // Ejecutar primer comando para rrhh.MANT_OEMPL
                        using (SqlCommand cmd = new SqlCommand("rrhh.MANT_OEMPL", cn1, transaction1))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Accion", "UPD");
                            cmd.Parameters.AddWithValue("@IdOEMPL", empleado.IdOEMPL);
                            cmd.Parameters.AddWithValue("@Nombres", empleado.Nombres);
                            cmd.Parameters.AddWithValue("@Apellidos", empleado.Apellidos);
                            cmd.Parameters.AddWithValue("@TipoDocumento", "DNI");
                            cmd.Parameters.AddWithValue("@NroDocumento", empleado.NroDocumento);
                            cmd.Parameters.AddWithValue("@FechaNacimiento", empleado.FechaNacimiento);
                            cmd.Parameters.AddWithValue("@EstadoCivil", empleado.EstadoCivil);
                            cmd.Parameters.AddWithValue("@Genero", empleado.Genero);
                            cmd.Parameters.AddWithValue("@Direccion", empleado.Direccion);
                            cmd.Parameters.AddWithValue("@Ubigeo", empleado.Ubigeo);
                            cmd.Parameters.AddWithValue("@ReferenciaDomicilio", empleado.ReferenciaDomicilio);
                            cmd.Parameters.AddWithValue("@Nacionalidad", empleado.Nacionalidad);
                            cmd.Parameters.AddWithValue("@CorreoElectronico", empleado.CorreoElectronico);
                            cmd.Parameters.AddWithValue("@Celular", empleado.Celular);
                            cmd.Parameters.AddWithValue("@LicenciaConducir", empleado.LicenciaConducir);
                            cmd.Parameters.AddWithValue("@NombreContactoEmergencia", empleado.NombreContactoEmergencia);
                            cmd.Parameters.AddWithValue("@CelularContactoEmergencia", empleado.CelularContactoEmergencia);
                            cmd.Parameters.AddWithValue("@RegistradoPor", empleado.RegistradoPor);
                            cmd.Parameters.AddWithValue("@PrefijoId", empleado.PrefijoId);
                            cmd.Parameters.AddWithValue("@Estado", empleado.Estado);

                            cmd.ExecuteNonQuery();
                        }
                        // Ejecutar cuarto comando para rrhh.MANT_EMPL1
                        using (SqlCommand cmd = new SqlCommand("rrhh.MANT_EMPL1", cn1, transaction1))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Accion", "UPD");
                            cmd.Parameters.AddWithValue("@IdOEMPL", datosLaborales.IdOEMPL);
                            cmd.Parameters.AddWithValue("@TipoContrato", datosLaborales.TipoContrato);
                            cmd.Parameters.AddWithValue("@FechaContratacion", datosLaborales.FechaContratacion);
                            cmd.Parameters.AddWithValue("@Salario", datosLaborales.Salario);
                            cmd.Parameters.AddWithValue("@FechaCese", datosLaborales.FechaCese);
                            cmd.Parameters.AddWithValue("@SedeID", datosLaborales.IdSede);
                            cmd.Parameters.AddWithValue("@DepartamentoID", datosLaborales.IdDepartamento);
                            cmd.Parameters.AddWithValue("@AreaID", datosLaborales.IdArea);
                            cmd.Parameters.AddWithValue("@CargoID", datosLaborales.IdCargo);
                            cmd.Parameters.AddWithValue("@IdNumeroCorporativo", datosLaborales.IdNumeroCorporativo);
                            cmd.Parameters.AddWithValue("@AnexoCorporativo", datosLaborales.AnexoCorporativo);
                            cmd.Parameters.AddWithValue("@CorreoCorporativo", datosLaborales.CorreoCorporativo);
                            cmd.Parameters.AddWithValue("@TurnoTrabajo", datosLaborales.TurnoTrabajo);
                            cmd.Parameters.AddWithValue("@Discapacidad", datosLaborales.Discapacidad);
                            cmd.Parameters.AddWithValue("@CondicionLaboral", datosLaborales.CondicionLaboral);

                            cmd.ExecuteNonQuery();
                        }
                        // Confirmar la transacción en la primera conexión
                        transaction1.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Si ocurre un error, revertir la transacción en la primera conexión
                        transaction1.Rollback();
                        RegistrarError(ex, "OEMPL_D - EditarEmpleado (Conexión 1)");
                        return mensajeError; // Salir después de manejar el error
                    }
                }
            }

            ////// Segundo bloque: ejecutar en la segunda conexión y transacción
            //using (SqlConnection cn2 = new SqlConnection(uti.cadSql2))
            //{
            //    cn2.Open();
            //    using (SqlTransaction transaction2 = cn2.BeginTransaction())
            //    {
            //        try
            //        {
            //            // Ejecutar tercer comando para rrhh.MANT_OEMPL
            //            using (SqlCommand cmd = new SqlCommand("rrhh.MANT_OEMPL", cn2, transaction2))
            //            {
            //                cmd.CommandType = CommandType.StoredProcedure;
            //                cmd.Parameters.AddWithValue("@Accion", "UPD");
            //                cmd.Parameters.AddWithValue("@Id", empleado.IdOEMPL);
            //                cmd.Parameters.AddWithValue("@Nombres", empleado.Nombres);
            //                cmd.Parameters.AddWithValue("@Apellidos", empleado.Apellidos);
            //                cmd.Parameters.AddWithValue("@TipoDocumento", "DNI");
            //                cmd.Parameters.AddWithValue("@NroDocumento", empleado.NroDocumento);
            //                cmd.Parameters.AddWithValue("@FechaNacimiento", empleado.FechaNacimiento);
            //                cmd.Parameters.AddWithValue("@EstadoCivil", "");
            //                cmd.Parameters.AddWithValue("@Genero", "");
            //                cmd.Parameters.AddWithValue("@Direccion", "");
            //                cmd.Parameters.AddWithValue("@UbigeoID", empleado.Ubigeo);
            //                cmd.Parameters.AddWithValue("@ReferenciaDomicilio", "");
            //                cmd.Parameters.AddWithValue("@Nacionalidad", "");
            //                cmd.Parameters.AddWithValue("@CorreoElectronico", "");
            //                cmd.Parameters.AddWithValue("@Celular", "");
            //                cmd.Parameters.AddWithValue("@LicenciaConducir", "");
            //                cmd.Parameters.AddWithValue("@NombreContactoEmergencia", "");
            //                cmd.Parameters.AddWithValue("@CelularContactoEmergencia", "");
            //                cmd.Parameters.AddWithValue("@UsuarioOperacion", empleado.UsuarioOperacion);

            //                cmd.ExecuteNonQuery();
            //            }

            //            // Ejecutar cuarto comando para rrhh.MANT_EMPL1
            //            using (SqlCommand cmd = new SqlCommand("rrhh.MANT_EMPL1", cn2, transaction2))
            //            {
            //                cmd.CommandType = CommandType.StoredProcedure;
            //                cmd.Parameters.AddWithValue("@Accion", "UPD");
            //                cmd.Parameters.AddWithValue("@Id", datosLaborales.IdOEMPL);
            //                cmd.Parameters.AddWithValue("@EmpleadoID", datosLaborales.IdOEMPL);
            //                cmd.Parameters.AddWithValue("@TipoContrato", datosLaborales.TipoContrato);
            //                cmd.Parameters.AddWithValue("@FechaContratacion", datosLaborales.FechaContratacion);
            //                cmd.Parameters.AddWithValue("@Salario", datosLaborales.Salario);
            //                cmd.Parameters.AddWithValue("@FechaCese", datosLaborales.FechaCese);
            //                cmd.Parameters.AddWithValue("@SedeID", datosLaborales.SedeID);
            //                cmd.Parameters.AddWithValue("@DepartamentoID", datosLaborales.DepartamentoID);
            //                cmd.Parameters.AddWithValue("@AreaID", datosLaborales.AreaID);
            //                cmd.Parameters.AddWithValue("@CargoID", datosLaborales.CargoID);
            //                cmd.Parameters.AddWithValue("@TurnoTrabajo", datosLaborales.TurnoTrabajo);
            //                cmd.Parameters.AddWithValue("@Discapacidad", datosLaborales.Discapacidad);
            //                cmd.Parameters.AddWithValue("@CondicionLaboral", datosLaborales.CondicionLaboral);
            //                cmd.Parameters.AddWithValue("@UsuarioOperacion", datosLaborales.UsuarioOperacion);

            //                cmd.ExecuteNonQuery();
            //            }

            //            // Confirmar la transacción en la segunda conexión
            //            transaction2.Commit();
            //        }
            //        catch (Exception ex)
            //        {
            //            // Si ocurre un error, revertir la transacción en la segunda conexión
            //            transaction2.Rollback();
            //            RegistrarError(ex, "EMPL1_D - EditarEmpleado (Conexión 2)");
            //            return mensajeError; // Salir después de manejar el error
            //        }
            //    }
            //}

            // Registrar histórico del empleado
            try
            {
                new TablasSQL.HIST_OEMPL_D().RegistrarHistoricoEmpleado(empleado);
            }
            catch (Exception ex)
            {
                RegistrarError(ex, "HIST_OEMPL_D - RegistrarHistoricoEmpleado");
                mensajeError = "Ocurrió un error al registrar el histórico del empleado. Por favor, comunicarse con SISTEMAS.";
            }

            return string.Empty;
        }
        public string EliminarEmpleado(int id)
        {
            string mensajeError;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("rrhh.MANT_OEMPL", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "DEL");
                    cmd.Parameters.AddWithValue("@IdOEMPL", id);
                    cmd.ExecuteNonQuery();
                    mensajeError = string.Empty;
                }
                catch (Exception ex)
                {
                    RegistrarError(ex, "OEMPL_D - EliminarEmpleado");
                    mensajeError = "Ocurrió un error al eliminar al empleado. Por favor, comunicarse con SISTEMAS.";
                }
            }

            return mensajeError;
        }
        public List<OEMPL_E> ListarEmpleados(OEMPL_E filtros, int? IdRol)
        {
            DateTime fechaActual = DateTime.Now;
            List<OEMPL_E> lista = null;
            int razon = 12;

            Dictionary<string, int> valoresRango = new Dictionary<string, int>
            {
                { "inicio", 0 },
                { "rango", 12 },
            };

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT EMP.IdOEMPL, EMP.Nombres, EMP.Apellidos, EMP.TipoDocumento, EMP.NroDocumento, CONVERT(varchar, EMP.FechaNacimiento, 103), EMP.EstadoCivil, EMP.Genero,");
                    sb.Append(" EMP.Direccion, EMP.Ubigeo, EMP.ReferenciaDomicilio, EMP.Nacionalidad, EMP.CorreoElectronico, EMP.Celular, EMP.LicenciaConducir, EMP.NombreContactoEmergencia,");
                    sb.Append(" EMP.CelularContactoEmergencia, EMP.RegistradoPor, EMP.PrefijoId, EMP.Estado,");
                    sb.Append(" CASE WHEN EMP.Estado = '1' THEN 'ACTIVO' ELSE 'CESADO' END AS DescripcionEstado, CONCAT(EMP.Nombres, ' ' , EMP.Apellidos) AS NombresApellidos,");
                    sb.Append(" DL.IdDepartamento, DL.IdArea, DL.IdCargo, DL.IdSede, DEP.Nombre, ISNULL(CAR.Nombre, ''), DL.CorreoCorporativo, DL.IdNumeroCorporativo, ISNULL(NU.NumeroCorporativo, ''), ISNULL(AR.Nombre, ''),");
                    sb.Append(" ISNULL(DL.AnexoCorporativo, ''), ISNULL(DL.CondicionLaboral, ''), (SELECT TOP 1 H.FechaRegistro FROM rrhh.HIST_OEMPL H WHERE H.IdOEMPL = EMP.IdOEMPL ORDER BY H.IdHIST_OEMPL DESC )");
                    sb.Append(" ,CASE ");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Gerente General' THEN  1");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Gerente%' THEN  2");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Asistente de Gerencia%' THEN  3");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Coordinador de Sst%' THEN  4");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Auxiliar Administrativo%' THEN  5");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Subgerente%' THEN  6");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Analista%' THEN  7");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Contador%' THEN  8");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Directora Tecnica%' THEN  9");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Jefe de Aseguramiento%' THEN  10");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Jefe De Recursos%' THEN  11");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Jefe De Ventas Call%' THEN  12");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Jefe De Ventas Hori%' THEN  13");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Jefe De Ventas Estrat%' THEN  14");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Jefe De Marketing%' THEN  15");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Jefe De Recep%' THEN  16");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Jefe De Alm%' THEN  17");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Jefe De Transpo%' THEN  18");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Jefe De Despacho%' THEN  19");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Coordinador De Rece%' THEN  20");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Coordinador De Verifi%' THEN  21");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Coordinador De Pickin%' THEN  22");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Coordinador De Prepa%' THEN  23");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Coordinador De Despacho%' THEN  24");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Asistente de Operaciones' THEN  25");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Secretaria%' THEN  26");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Asistente De Verifi%' THEN  27");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Asistente De Recep%' THEN  28");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Asistente De Transfe%' THEN  29");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Asistente De Ingreso%' THEN  30");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Asistente De Picking%' THEN  31");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Auxiliar De Pedi%' THEN  32");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Auxiliar De Control%' THEN  33");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Diseñador%' THEN  34");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Asistente Comercial%' THEN  35");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Vendedor Call Center' THEN  36");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Vendedor Call Center Horizontal Lima%' THEN  37");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Vendedor Call Center Redes' THEN  38");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Vendedor Presencial Lima%' THEN  39");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Vendedor Presencial Provincia%' THEN  40");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Vendedor Ventas Estr%' THEN  41");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Promotor De Ventas Lima%' THEN  42");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Promotor De Ventas Provincia%' THEN  43");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Impulsadora' THEN  44");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Conductor Auxili%' THEN  45");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Conductor%' THEN  46");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Motorizado%' THEN  47");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Mecanico%' OR CAR.Nombre LIKE 'Mecánico%' THEN  48");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Verificador De Re%' THEN  49");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Auxiliar De Verificación' THEN  50");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Auxiliar De Verificacion Y%' THEN  51");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Operario De Alma%' THEN  52");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Auxiliar%' THEN  53");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Programador Web%' THEN  54");
                    sb.Append(" WHEN CAR.Nombre LIKE 'Asistente de Soporte%' THEN  55");
                    sb.Append(" ELSE 56");
                    sb.Append(" END AS Posicion ");
                    sb.Append(" FROM rrhh.OEMPL EMP ");
                    sb.Append(" INNER JOIN rrhh.EMPL1 DL ON DL.IdOEMPL = EMP.IdOEMPL");
                    sb.Append(" LEFT JOIN dbo.ODPTO DEP ON DEP.IdDepartamento = DL.IdDepartamento");
                    sb.Append(" LEFT JOIN dbo.OAREA AR ON AR.IdArea = DL.IdArea AND AR.IdDepartamento = DEP.IdDepartamento");
                    sb.Append(" LEFT JOIN dbo.CARGO CAR ON CAR.Id = DL.IdCargo");
                    sb.Append(" LEFT JOIN dbo.ONUM NU ON NU.IdNumero = DL.IdNumeroCorporativo");
                    sb.Append(" WHERE 1 = 1");

                    if (filtros != null)
                    {
                        if (!string.IsNullOrWhiteSpace(filtros.NombresApellidos))
                        {
                            sb.Append(" AND CONCAT(EMP.Nombres, ' ', EMP.Apellidos) LIKE @NombresApellidos");
                            cmd.Parameters.AddWithValue("@NombresApellidos", string.Format("%{0}%", filtros.NombresApellidos));
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.TipoDocumento))
                        {
                            sb.Append(" AND EMP.TipoDocumento = @TipoDocumento");
                            cmd.Parameters.AddWithValue("@TipoDocumento", filtros.TipoDocumento);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.NroDocumento))
                        {
                            sb.Append(" AND EMP.NroDocumento = @NroDocumento");
                            cmd.Parameters.AddWithValue("@NroDocumento", filtros.NroDocumento);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.FechaNacimiento))
                        {
                            sb.Append(" AND EMP.FechaNacimiento = @FechaNacimiento");
                            cmd.Parameters.AddWithValue("@FechaNacimiento", filtros.FechaNacimiento);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.EstadoCivil))
                        {
                            sb.Append(" AND EMP.EstadoCivil = @EstadoCivil");
                            cmd.Parameters.AddWithValue("@EstadoCivil", filtros.EstadoCivil);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.Genero))
                        {
                            sb.Append(" AND EMP.Genero = @Genero");
                            cmd.Parameters.AddWithValue("@Genero", filtros.Genero);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.Nacionalidad))
                        {
                            sb.Append(" AND EMP.Nacionalidad = @Nacionalidad");
                            cmd.Parameters.AddWithValue("@Nacionalidad", filtros.Nacionalidad);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.CorreoElectronico))
                        {
                            sb.Append(" AND EMP.CorreoElectronico = @CorreoElectronico");
                            cmd.Parameters.AddWithValue("@CorreoElectronico", filtros.CorreoElectronico);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.Estado))
                        {
                            sb.Append(" AND EMP.Estado = @Estado");
                            cmd.Parameters.AddWithValue("@Estado", filtros.Estado);
                        }

                        if (filtros.DatosLaborales != null)
                        {
                            if (filtros.DatosLaborales.IdDepartamento > 0)
                            {
                                sb.Append(" AND DL.IdDepartamento = @DepartamentoID");
                                cmd.Parameters.AddWithValue("@DepartamentoID", filtros.DatosLaborales.IdDepartamento);
                            }

                            if (filtros.DatosLaborales.IdArea > 0)
                            {
                                sb.Append(" AND DL.IdArea = @AreaID");
                                cmd.Parameters.AddWithValue("@AreaID", filtros.DatosLaborales.IdArea);
                            }

                            if (filtros.DatosLaborales.IdCargo > 0)
                            {
                                sb.Append(" AND DL.IdCargo = @CargoID");
                                cmd.Parameters.AddWithValue("@CargoID", filtros.DatosLaborales.IdCargo);
                            }

                            if (filtros.DatosLaborales.IdSede > 0)
                            {
                                sb.Append(" AND DL.IdSede = @SedeID");
                                cmd.Parameters.AddWithValue("@SedeID", filtros.DatosLaborales.IdSede);
                            }
                        }

                        if (IdRol == null)
                        {
                            sb.Append(" AND EMP.Estado = '1' AND (DL.CorreoCorporativo != '' OR DL.AnexoCorporativo != '' OR (DL.IdNumeroCorporativo > 0 OR EMP.Celular != ''))");
                        }

                        if (filtros.PaginacionResultados > 0)
                        {
                            for (int k = 1; k < filtros.PaginacionResultados; k++)
                            {
                                valoresRango["inicio"] += razon;
                            }
                            sb.Append($" ORDER BY Posicion OFFSET {valoresRango["inicio"]} ROWS FETCH NEXT {valoresRango["rango"]} ROWS ONLY");
                        }
                    }


                    cmd.CommandText = sb.ToString();
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            lista = new List<OEMPL_E>();

                            while (dr.Read())
                            {
                                OEMPL_E obj = new OEMPL_E();

                                if (!dr.IsDBNull(0)) { obj.IdOEMPL = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.Nombres = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.Apellidos = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.TipoDocumento = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.NroDocumento = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.FechaNacimiento = dr.GetString(5); }
                                if (!dr.IsDBNull(6)) { obj.EstadoCivil = dr.GetString(6); }
                                if (!dr.IsDBNull(7)) { obj.Genero = dr.GetString(7); }
                                if (!dr.IsDBNull(8)) { obj.Direccion = dr.GetString(8); }
                                if (!dr.IsDBNull(9)) { obj.Ubigeo = dr.GetInt32(9); }
                                if (!dr.IsDBNull(10)) { obj.ReferenciaDomicilio = dr.GetString(10); }
                                if (!dr.IsDBNull(11)) { obj.Nacionalidad = dr.GetString(11); }
                                if (!dr.IsDBNull(12)) { obj.CorreoElectronico = dr.GetString(12); }
                                if (!dr.IsDBNull(13)) { obj.Celular = dr.GetString(13); }
                                if (!dr.IsDBNull(14)) { obj.LicenciaConducir = dr.GetString(14); }
                                if (!dr.IsDBNull(15)) { obj.NombreContactoEmergencia = dr.GetString(15); }
                                if (!dr.IsDBNull(16)) { obj.CelularContactoEmergencia = dr.GetString(16); }
                                if (!dr.IsDBNull(17)) { obj.RegistradoPor = dr.GetInt32(17); }
                                if (!dr.IsDBNull(18)) { obj.PrefijoId = dr.GetString(18); }
                                if (!dr.IsDBNull(19)) { obj.Estado = dr.GetString(19); }
                                if (!dr.IsDBNull(20)) { obj.DescripcionEstado = dr.GetString(20); }
                                if (!dr.IsDBNull(21)) { obj.NombresApellidos = dr.GetString(21); }

                                obj.DatosLaborales = new EMPL1_E
                                {
                                    IdDepartamento = dr.GetInt32(22),
                                    IdArea = dr.GetInt32(23),
                                    IdCargo = dr.GetInt32(24),
                                    IdSede = dr.GetInt32(25),
                                    NombreDepartamento = dr.GetString(26),
                                    NombreCargo = dr.GetString(27),
                                    CorreoCorporativo = dr.GetString(28),
                                    IdNumeroCorporativo = dr.GetInt32(29),
                                    NumeroCorporativo = dr.GetString(30),
                                    NombreArea = dr.GetString(31),
                                    AnexoCorporativo = dr.GetString(32),
                                    CondicionLaboral = dr.GetString(33)
                                };

                                if (!dr.IsDBNull(34))
                                {
                                    DateTime fechaLimite = dr.GetDateTime(34).AddDays(7);       // Notificacion debe mostrarse durante 7 días
                                    obj.MostrarNotificacionCambio = fechaActual <= fechaLimite ? "SI" : "NO";
                                }
                                else
                                {
                                    obj.MostrarNotificacionCambio = "NO";
                                }

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex, "OEMPL_D - ListarEmpleados");
            }

            return lista;
        }
        //public List<OEMPL_E> ListarEmpleados(OEMPL_E filtros)
        //{
        //    List<OEMPL_E> lista = new List<OEMPL_E>();

        //    try
        //    {
        //        using (SqlConnection cn = new SqlConnection(uti.cadSql))
        //        {
        //            SqlCommand cmd = new SqlCommand();
        //            cmd.Connection = cn;

        //            var sb = new StringBuilder();

        //            sb.Append("SELECT EMP.Id, EMP.Nombres, EMP.Apellidos, EMP.TipoDocumento, EMP.NroDocumento, CONVERT(varchar, EMP.FechaNacimiento, 103), EMP.EstadoCivil, EMP.Genero,")
        //            .Append(" EMP.Direccion, EMP.UbigeoID, EMP.ReferenciaDomicilio, EMP.Nacionalidad, EMP.CorreoElectronico, EMP.Celular, EMP.LicenciaConducir, EMP.NombreContactoEmergencia,")
        //            .Append(" EMP.CelularContactoEmergencia, EMP.Estado")
        //            .Append(" FROM rrhh.OEMPL EMP")
        //            .Append(" WHERE 1 = 1");

        //            if (filtros != null)
        //            {
        //                if (filtros.Id > 0)
        //                {
        //                    sb.Append(" AND EMP.Id = @Id");
        //                    cmd.Parameters.AddWithValue("@Id", filtros.Id);
        //                }

        //                if (!string.IsNullOrWhiteSpace(filtros.NombresApellidos))
        //                {
        //                    sb.Append(" AND CONCAT(EMP.Nombres, ' ', EMP.Apellidos) LIKE @NombresApellidos");
        //                    cmd.Parameters.AddWithValue("@NombresApellidos", string.Format("%{0}%", filtros.NombresApellidos));
        //                }

        //                if (!string.IsNullOrWhiteSpace(filtros.TipoDocumento))
        //                {
        //                    sb.Append(" AND EMP.TipoDocumento = @TipoDocumento");
        //                    cmd.Parameters.AddWithValue("@TipoDocumento", filtros.TipoDocumento);
        //                }

        //                if (!string.IsNullOrWhiteSpace(filtros.NroDocumento))
        //                {
        //                    sb.Append(" AND EMP.NroDocumento = @NroDocumento");
        //                    cmd.Parameters.AddWithValue("@NroDocumento", filtros.NroDocumento);
        //                }

        //                if (!string.IsNullOrWhiteSpace(filtros.FechaNacimiento))
        //                {
        //                    sb.Append(" AND EMP.FechaNacimiento = @FechaNacimiento");
        //                    cmd.Parameters.AddWithValue("@FechaNacimiento", filtros.FechaNacimiento);
        //                }

        //                if (!string.IsNullOrWhiteSpace(filtros.EstadoCivil))
        //                {
        //                    sb.Append(" AND EMP.EstadoCivil = @EstadoCivil");
        //                    cmd.Parameters.AddWithValue("@EstadoCivil", filtros.EstadoCivil);
        //                }

        //                if (!string.IsNullOrWhiteSpace(filtros.Genero))
        //                {
        //                    sb.Append(" AND EMP.Genero = @Genero");
        //                    cmd.Parameters.AddWithValue("@Genero", filtros.Genero);
        //                }

        //                if (!string.IsNullOrWhiteSpace(filtros.Nacionalidad))
        //                {
        //                    sb.Append(" AND EMP.Nacionalidad = @Nacionalidad");
        //                    cmd.Parameters.AddWithValue("@Nacionalidad", filtros.Nacionalidad);
        //                }

        //                if (!string.IsNullOrWhiteSpace(filtros.CorreoElectronico))
        //                {
        //                    sb.Append(" AND EMP.CorreoElectronico = @CorreoElectronico");
        //                    cmd.Parameters.AddWithValue("@CorreoElectronico", filtros.CorreoElectronico);
        //                }

        //                if (!string.IsNullOrWhiteSpace(filtros.Celular))
        //                {
        //                    sb.Append(" AND EMP.Celular = @Celular");
        //                    cmd.Parameters.AddWithValue("@Celular", filtros.Celular);
        //                }

        //                if (!string.IsNullOrWhiteSpace(filtros.Estado))
        //                {
        //                    sb.Append(" AND EMP.Estado = @Estado");
        //                    cmd.Parameters.AddWithValue("@Estado", filtros.Estado);
        //                }
        //            }

        //            sb.Append(" ORDER BY EMP.Nombres ");
        //            cmd.CommandText = sb.ToString();

        //            cn.Open();
        //            using (SqlDataReader dr = cmd.ExecuteReader())
        //            {
        //                if (dr.HasRows)
        //                {
        //                    while (dr.Read())
        //                    {
        //                        var obj = new OEMPL_E
        //                        {
        //                            Id = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
        //                            Nombres = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
        //                            Apellidos = dr.IsDBNull(2) ? string.Empty : dr.GetString(2),
        //                            TipoDocumento = dr.IsDBNull(3) ? string.Empty : dr.GetString(3),
        //                            NroDocumento = dr.IsDBNull(4) ? string.Empty : dr.GetString(4),
        //                            FechaNacimiento = dr.IsDBNull(5) ? string.Empty : dr.GetString(5),
        //                            EstadoCivil = dr.IsDBNull(6) ? string.Empty : dr.GetString(6),
        //                            Genero = dr.IsDBNull(7) ? string.Empty : dr.GetString(7),
        //                            Direccion = dr.IsDBNull(8) ? string.Empty : dr.GetString(8),
        //                            UbigeoID = dr.IsDBNull(9) ? 0 : dr.GetInt32(9),
        //                            ReferenciaDomicilio = dr.IsDBNull(10) ? string.Empty : dr.GetString(10),
        //                            Nacionalidad = dr.IsDBNull(11) ? string.Empty : dr.GetString(11),
        //                            CorreoElectronico = dr.IsDBNull(12) ? string.Empty : dr.GetString(12),
        //                            Celular = dr.IsDBNull(13) ? string.Empty : dr.GetString(13),
        //                            LicenciaConducir = dr.IsDBNull(14) ? string.Empty : dr.GetString(14),
        //                            NombreContactoEmergencia = dr.IsDBNull(15) ? string.Empty : dr.GetString(15),
        //                            CelularContactoEmergencia = dr.IsDBNull(16) ? string.Empty : dr.GetString(16),
        //                            Estado = dr.IsDBNull(17) ? string.Empty : dr.GetString(17)
        //                        };

        //                        obj.NombresApellidos = $"{obj.Nombres} {obj.Apellidos}";
        //                        lista.Add(obj);
        //                    }
        //                }
        //            }
        //            cn.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RegistrarError(ex, "OEMPL_D - ListarEmpleados");
        //    }

        //    return lista;
        //}
        public OEMPL_E ObtenerDatosEmpleado(int id, string nroDocumento = "")
        {
            OEMPL_E obj = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT TOP 100 EMP.IdOEMPL, EMP.Nombres, EMP.Apellidos, EMP.TipoDocumento, EMP.NroDocumento, CONVERT(varchar, EMP.FechaNacimiento, 103), EMP.EstadoCivil, EMP.Genero,");
                    sb.Append(" EMP.Direccion, EMP.Ubigeo, EMP.ReferenciaDomicilio, EMP.Nacionalidad, EMP.CorreoElectronico, EMP.Celular, EMP.LicenciaConducir, EMP.NombreContactoEmergencia,");
                    sb.Append(" EMP.CelularContactoEmergencia, EMP.RegistradoPor, EMP.PrefijoId, EMP.Estado,");
                    sb.Append(" CASE WHEN EMP.Estado = '1' THEN 'ACTIVO' ELSE 'CESADO' END AS DescripcionEstado, CONCAT(EMP.Nombres, ' ' , EMP.Apellidos) AS NombresApellidos");
                    sb.Append(" FROM rrhh.OEMPL EMP");
                    sb.Append(" WHERE EMP.IdOEMPL = @IdOEMPL");

                    if (!string.IsNullOrWhiteSpace(nroDocumento))
                    {
                        sb.Append(" OR EMP.NroDocumento = @NroDocumento");
                        cmd.Parameters.AddWithValue("@NroDocumento", nroDocumento);
                    }

                    cmd.CommandText = sb.ToString();
                    cmd.Parameters.AddWithValue("@IdOEMPL", id);

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            obj = new OEMPL_E();

                            while (dr.Read())
                            {
                                if (!dr.IsDBNull(0))
                                {
                                    obj.IdOEMPL = dr.GetInt32(0);
                                    obj.DatosLaborales = new EMPL1_D().ObtenerDatosLaborales(dr.GetInt32(0));
                                }
                                if (!dr.IsDBNull(1)) { obj.Nombres = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.Apellidos = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.TipoDocumento = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.NroDocumento = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.FechaNacimiento = dr.GetString(5); }
                                if (!dr.IsDBNull(6)) { obj.EstadoCivil = dr.GetString(6); }
                                if (!dr.IsDBNull(7)) { obj.Genero = dr.GetString(7); }
                                if (!dr.IsDBNull(8)) { obj.Direccion = dr.GetString(8); }
                                if (!dr.IsDBNull(9)) { obj.Ubigeo = dr.GetInt32(9); }
                                if (!dr.IsDBNull(10)) { obj.ReferenciaDomicilio = dr.GetString(10); }
                                if (!dr.IsDBNull(11)) { obj.Nacionalidad = dr.GetString(11); }
                                if (!dr.IsDBNull(12)) { obj.CorreoElectronico = dr.GetString(12); }
                                if (!dr.IsDBNull(13)) { obj.Celular = dr.GetString(13); }
                                if (!dr.IsDBNull(14)) { obj.LicenciaConducir = dr.GetString(14); }
                                if (!dr.IsDBNull(15)) { obj.NombreContactoEmergencia = dr.GetString(15); }
                                if (!dr.IsDBNull(16)) { obj.CelularContactoEmergencia = dr.GetString(16); }
                                if (!dr.IsDBNull(17)) { obj.RegistradoPor = dr.GetInt32(17); }
                                if (!dr.IsDBNull(18)) { obj.PrefijoId = dr.GetString(18); }
                                if (!dr.IsDBNull(19)) { obj.Estado = dr.GetString(19); }
                                if (!dr.IsDBNull(20)) { obj.DescripcionEstado = dr.GetString(20); }
                                if (!dr.IsDBNull(21)) { obj.NombresApellidos = dr.GetString(21); }
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex, "OEMPL_D - ObtenerDatosEmpleado.txt");
            }

            return obj;
        }
        public List<Capa_Entidad.RecursosHumanos_ENT.Reportes.RptEmpleados_E> ExportarListaEmpleados(OEMPL_E filtros)
        {
            List<Capa_Entidad.RecursosHumanos_ENT.Reportes.RptEmpleados_E> lista = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT DISTINCT TOP 100  CONCAT(EMP.Apellidos, ' ', EMP.Nombres), SD.Nombre, DEP.Nombre, AR.Nombre, ISNULL(CAR.Nombre, ''), ISNULL(NU.NumeroCorporativo, ''), DL.AnexoCorporativo, DL.CorreoCorporativo");
                    sb.Append(" FROM rrhh.OEMPL EMP");
                    sb.Append(" INNER JOIN rrhh.EMPL1 DL ON DL.IdOEMPL = EMP.IdOEMPL");
                    sb.Append(" LEFT JOIN dbo.SEDE SD ON SD.Id = DL.IdSede");
                    sb.Append(" LEFT JOIN dbo.ODPTO DEP ON DEP.IdDepartamento = DL.IdDepartamento");
                    sb.Append(" LEFT JOIN dbo.OAREA AR ON AR.IdDepartamento = DL.IdDepartamento AND AR.IdArea = DL.IdArea");
                    sb.Append(" LEFT JOIN dbo.CARGO CAR ON CAR.Id = DL.IdCargo");
                    sb.Append(" LEFT JOIN dbo.ONUM NU ON NU.IdNumero = DL.IdNumeroCorporativo");
                    sb.Append(" WHERE 1 = 1");

                    if (filtros.DatosLaborales != null)
                    {
                        if (filtros.DatosLaborales.IdDepartamento > 0)
                        {
                            sb.Append(" AND DL.IdDepartamento = @DepartamentoID");
                            cmd.Parameters.AddWithValue("@DepartamentoID", filtros.DatosLaborales.IdDepartamento);
                        }

                        if (filtros.DatosLaborales.IdArea > 0)
                        {
                            sb.Append(" AND DL.IdArea = @AreaID");
                            cmd.Parameters.AddWithValue("@AreaID", filtros.DatosLaborales.IdArea);
                        }

                        if (filtros.DatosLaborales.IdCargo > 0)
                        {
                            sb.Append(" AND DL.IdCargo = @CargoID");
                            cmd.Parameters.AddWithValue("@CargoID", filtros.DatosLaborales.IdCargo);
                        }

                        if (filtros.DatosLaborales.IdSede > 0)
                        {
                            sb.Append(" AND DL.IdSede = @SedeID");
                            cmd.Parameters.AddWithValue("@SedeID", filtros.DatosLaborales.IdSede);
                        }
                    }

                    cmd.CommandText = sb.ToString();
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            lista = new List<Capa_Entidad.RecursosHumanos_ENT.Reportes.RptEmpleados_E>();

                            while (dr.Read())
                            {
                                Capa_Entidad.RecursosHumanos_ENT.Reportes.RptEmpleados_E obj = new Capa_Entidad.RecursosHumanos_ENT.Reportes.RptEmpleados_E();

                                if (!dr.IsDBNull(0)) { obj.ApellidosNombres = dr.GetString(0); }
                                if (!dr.IsDBNull(1)) { obj.Sede = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.Departamento = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.Area = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.Cargo = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.NumeroCorporativo = dr.GetString(5); }
                                if (!dr.IsDBNull(6)) { obj.AnexoCorporativo = dr.GetString(6); }
                                if (!dr.IsDBNull(7)) { obj.CorreoCorporativo = dr.GetString(7); }

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex, "OEMPL_D - ExportarListaEmpleados");
            }

            return lista;
        }
        private void RegistrarError(Exception ex, string nombreArchivo)
        {
            File.AppendAllText(uti.directorioLogs + nombreArchivo + ".txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");
        }
    }
}
