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

        public List<OEMPL_E> ListarEmpleados(OEMPL_E filtros)
        {
            List<OEMPL_E> lista = new List<OEMPL_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.Append("SELECT EMP.Id, EMP.Nombres, EMP.Apellidos, EMP.TipoDocumento, EMP.NroDocumento, CONVERT(varchar, EMP.FechaNacimiento, 103), EMP.EstadoCivil, EMP.Genero,")
                    .Append(" EMP.Direccion, EMP.UbigeoID, EMP.ReferenciaDomicilio, EMP.Nacionalidad, EMP.CorreoElectronico, EMP.Celular, EMP.LicenciaConducir, EMP.NombreContactoEmergencia,")
                    .Append(" EMP.CelularContactoEmergencia, EMP.Estado")
                    .Append(" FROM rrhh.OEMPL EMP")
                    .Append(" WHERE 1 = 1");

                    if (filtros != null)
                    {
                        if (filtros.Id > 0)
                        {
                            sb.Append(" AND EMP.Id = @Id");
                            cmd.Parameters.AddWithValue("@Id", filtros.Id);
                        }

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

                        if (!string.IsNullOrWhiteSpace(filtros.Celular))
                        {
                            sb.Append(" AND EMP.Celular = @Celular");
                            cmd.Parameters.AddWithValue("@Celular", filtros.Celular);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.Estado))
                        {
                            sb.Append(" AND EMP.Estado = @Estado");
                            cmd.Parameters.AddWithValue("@Estado", filtros.Estado);
                        }
                    }

                    sb.Append(" ORDER BY EMP.Nombres ");
                    cmd.CommandText = sb.ToString();

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var obj = new OEMPL_E
                                {
                                    Id = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
                                    Nombres = dr.IsDBNull(1) ? string.Empty : dr.GetString(1),
                                    Apellidos = dr.IsDBNull(2) ? string.Empty : dr.GetString(2),
                                    TipoDocumento = dr.IsDBNull(3) ? string.Empty : dr.GetString(3),
                                    NroDocumento = dr.IsDBNull(4) ? string.Empty : dr.GetString(4),
                                    FechaNacimiento = dr.IsDBNull(5) ? string.Empty : dr.GetString(5),
                                    EstadoCivil = dr.IsDBNull(6) ? string.Empty : dr.GetString(6),
                                    Genero = dr.IsDBNull(7) ? string.Empty : dr.GetString(7),
                                    Direccion = dr.IsDBNull(8) ? string.Empty : dr.GetString(8),
                                    UbigeoID = dr.IsDBNull(9) ? 0 : dr.GetInt32(9),
                                    ReferenciaDomicilio = dr.IsDBNull(10) ? string.Empty : dr.GetString(10),
                                    Nacionalidad = dr.IsDBNull(11) ? string.Empty : dr.GetString(11),
                                    CorreoElectronico = dr.IsDBNull(12) ? string.Empty : dr.GetString(12),
                                    Celular = dr.IsDBNull(13) ? string.Empty : dr.GetString(13),
                                    LicenciaConducir = dr.IsDBNull(14) ? string.Empty : dr.GetString(14),
                                    NombreContactoEmergencia = dr.IsDBNull(15) ? string.Empty : dr.GetString(15),
                                    CelularContactoEmergencia = dr.IsDBNull(16) ? string.Empty : dr.GetString(16),
                                    Estado = dr.IsDBNull(17) ? string.Empty : dr.GetString(17)
                                };

                                obj.NombresApellidos = $"{obj.Nombres} {obj.Apellidos}";
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

        private void RegistrarError(Exception ex, string nombreArchivo)
        {
            File.AppendAllText(uti.directorioLogs + nombreArchivo + ".txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");
        }
    }
}
