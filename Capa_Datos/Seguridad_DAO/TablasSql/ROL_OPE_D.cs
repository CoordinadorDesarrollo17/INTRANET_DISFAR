using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Capa_Entidad.Seguridad_ENT;

namespace Capa_Datos.Seguridad_DAO.TablasSql
{
    public class ROL_OPE_D
    {
        Utilitarios uti = new Utilitarios();

        public int VerificarAccesoOperacion(int idRol, int idOperacion)
        {
            int result = -1;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT COUNT(*) FROM ROL_OPE WHERE RolID = @idRol AND OpeID = @idOperacion";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@idRol", idRol);
                    cmd.Parameters.AddWithValue("@idOperacion", idOperacion);

                    cn.Open();
                    result = (int)cmd.ExecuteScalar();
                }
            }

            return result;
        }

        public List<ROL_OPE_E> ListarGrupoOperacionesPorRol(int rolID)
        {
            List<ROL_OPE_E> lista = new List<ROL_OPE_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.Append("SELECT RO.Id, RO.OpeID, RO.RolID")
                        .Append(" FROM ROL_OPE RO")
                        .Append(" WHERE RO.RolID = @RolID");

                    cmd.Parameters.AddWithValue("@RolID", rolID);
                    cmd.CommandText = sb.ToString();

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var obj = new ROL_OPE_E
                                {
                                    Id = dr.IsDBNull(0) ? 0 : dr.GetInt32(0),
                                    OpeID = dr.IsDBNull(1) ? 0 : dr.GetInt32(1),
                                    RolID = dr.IsDBNull(2) ? 0 : dr.GetInt32(2)
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
                RegistrarError(ex, "ROL_OPE_D - ListarGrupoOperacionesPorRol");
            }

            return lista;
        }

        public string AsignarPermisosPorRol(List<int> operaciones, int rolID)
        {
            string mensajeError = string.Empty;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_ROL_OPE", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "INS");

                    var tablaRolOpe = new DataTable();
                    tablaRolOpe.Columns.Add("OpeID", typeof(int));
                    tablaRolOpe.Columns.Add("RolID", typeof(int));

                    foreach (var op in operaciones)
                    {
                        tablaRolOpe.Rows.Add(op, rolID);
                    }

                    var parameter = cmd.Parameters.AddWithValue("@RolOpeList", tablaRolOpe);
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.TypeName = "dbo.TPRolOpe";

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    RegistrarError(ex, "ROL_OPE_D - AsignarPermisosPorRol");        // Registro de error
                    mensajeError = "Ocurrió un error al asignar permisos. Por favor, comuníquese con el área de Sistemas para más información.";
                }
            }

            return mensajeError;
        }

        private void RegistrarError(Exception ex, string nombreArchivo)
        {
            File.AppendAllText(uti.directorioLogs + nombreArchivo + ".txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");
        }

    }
}