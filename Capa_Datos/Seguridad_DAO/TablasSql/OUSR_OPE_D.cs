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
    public class OUSR_OPE_D
    {
        Utilitarios uti = new Utilitarios();

        public int VerificarAccesoOperacion(OUSR_OPE_E filtros)
        {
            int result = -1;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT COUNT(*) FROM OUSR_OPE WHERE UsrDocEntry = @UsrDocEntry AND OpeID = @OpeID";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@UsrDocEntry", filtros.UsrDocEntry);
                    cmd.Parameters.AddWithValue("@OpeID", filtros.OpeID);

                    cn.Open();
                    result = (int)cmd.ExecuteScalar();
                }
            }

            return result;
        }

        public List<OUSR_OPE_E> ListarOperacionesPorUsuario(int usrDocEntry)
        {
            List<OUSR_OPE_E> lista = new List<OUSR_OPE_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.Append("SELECT UO.Id, UO.UsrDocEntry, UO.OpeID, OP.Nombre, OP.IdModulo, MD.Nombre")
                    .Append(" FROM OUSR_OPE UO")
                    .Append(" LEFT JOIN OOPE OP ON UO.OpeID = OP.Id")
                    .Append(" LEFT JOIN OMOD MD ON OP.IdModulo = MD.Id")
                    .Append(" WHERE UO.UsrDocEntry = @UsrDocEntry");

                    cmd.Parameters.AddWithValue("@UsrDocEntry", usrDocEntry);
                    cmd.CommandText = sb.ToString();

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                OUSR_OPE_E obj = new OUSR_OPE_E();

                                if (!dr.IsDBNull(0)) { obj.Id = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.UsrDocEntry = dr.GetInt32(1); }
                                if (!dr.IsDBNull(2)) { obj.OpeID = dr.GetInt32(2); }
                                if (!dr.IsDBNull(3)) { obj.OpeNombre = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.OpeIdModulo = dr.GetInt32(4); }
                                if (!dr.IsDBNull(5)) { obj.ModuloNombre = dr.GetString(5); }

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex, "OUSR_OPE_D - ListarOperacionesPorUsuario");
            }

            return lista;
        }     

        public string AsignarPermisosPorUsuario(List<int> operaciones, int usrDocEntry)
        {
            string mensajeError = string.Empty;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_OUSR_OPE", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "INS");

                    var tablaUsuOpe = new DataTable();
                    tablaUsuOpe.Columns.Add("UsrDocEntry", typeof(int));
                    tablaUsuOpe.Columns.Add("OpeID", typeof(int));

                    foreach (var op in operaciones)
                    {
                        tablaUsuOpe.Rows.Add(usrDocEntry, op);
                    }

                    var parameter = cmd.Parameters.AddWithValue("@UsrOpeList", tablaUsuOpe);
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.TypeName = "dbo.TPUsrOpe";

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    RegistrarError(ex, "OUSR_OPE_D - AsignarPermisosPorUsuario");        // Registro de error
                    mensajeError = "Ocurrió un error al asignar permisos. Por favor, comunicarse con SISTEMAS.";
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