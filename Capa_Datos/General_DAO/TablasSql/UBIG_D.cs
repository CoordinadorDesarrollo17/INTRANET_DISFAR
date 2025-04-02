using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Capa_Datos.General_DAO.TablasSql
{
    public class UBIG_D
    {
        Utilitarios uti = new Utilitarios();

        public List<UBIG_E> Listar(UBIG_E filtros)
        {
            List<UBIG_E> lista = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT Ubigeo, Distrito, Provincia, Departamento, Zona");
                    sb.Append(" FROM dbo.UBIG");
                    sb.Append(" WHERE 1 = 1");

                    if (filtros != null)
                    {
                        if (filtros.Ubigeo > 0)
                        {
                            sb.Append(" AND Ubigeo = @Ubigeo");
                            cmd.Parameters.AddWithValue("@Ubigeo", filtros.Ubigeo);
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.Distrito))
                        {
                            sb.Append(" AND Distrito LIKE @Distrito");
                            cmd.Parameters.AddWithValue("@Distrito", string.Format("%{0}%", filtros.Distrito));
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.Provincia))
                        {
                            sb.Append(" AND Provincia LIKE @Provincia");
                            cmd.Parameters.AddWithValue("@Provincia", string.Format("%{0}%", filtros.Provincia));
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.Departamento))
                        {
                            sb.Append(" AND Departamento LIKE @Departamento");
                            cmd.Parameters.AddWithValue("@Departamento", string.Format("%{0}%", filtros.Departamento));
                        }

                        if (!string.IsNullOrWhiteSpace(filtros.Zona))
                        {
                            sb.Append(" AND Zona LIKE @Zona");
                            cmd.Parameters.AddWithValue("@Zona", string.Format("%{0}%", filtros.Zona));
                        }
                    }

                    //sb.Append($" ORDER BY ---- DESC");    DESCOMENTAR LÍNEA SI SE DESEA ORDENAR POR ALGÚN CAMPO EN ESPECIAL

                    cmd.CommandText = sb.ToString();
                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            lista = new List<UBIG_E>();

                            while (dr.Read())
                            {
                                UBIG_E obj = new UBIG_E();
                                if (!dr.IsDBNull(0)) { obj.Ubigeo = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.Distrito = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.Provincia = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.Departamento = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.Zona = dr.GetString(4); }

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(uti.directorioLogs + "UBIG_D - Listar.txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");        // Registro de error
            }

            return lista;
        }
    }
}