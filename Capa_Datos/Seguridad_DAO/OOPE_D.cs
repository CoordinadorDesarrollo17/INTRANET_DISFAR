using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Capa_Entidad.Seguridad_ENT;
using System.Data.SqlClient;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System.IO;

namespace Capa_Datos.Seguridad_DAO
{
    public class OOPE_D
    {
        Utilitarios uti = new Utilitarios();

        public List<OOPE_E> ListarOperaciones(OOPE_E filtros)
        {
            List<OOPE_E> lista = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT OP.Id, OP.Nombre, OP.IdModulo, MD.Nombre");
                    sb.Append(" FROM dbo.OOPE OP");
                    sb.Append(" INNER JOIN dbo.OMOD MD ON OP.IdModulo = MD.Id");
                    sb.Append(" WHERE 1 = 1");

                    if (filtros != null)
                    {
                        if (filtros.IdModulo > 0)
                        {
                            sb.Append(" AND OP.IdModulo = @IdModulo");
                            cmd.Parameters.AddWithValue("@IdModulo", filtros.IdModulo);
                        }
                    }

                    cmd.CommandText = sb.ToString();
                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            lista = new List<OOPE_E>();

                            while (dr.Read())
                            {
                                OOPE_E obj = new OOPE_E();

                                if (!dr.IsDBNull(0)) { obj.Id = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.Nombre = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.IdModulo = dr.GetInt32(2); }
                                if (!dr.IsDBNull(3)) { obj.ModuloNombre = dr.GetString(3); }

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex, "OOPE_D - ListarOperaciones");
            }

            return lista;
        }

        private void RegistrarError(Exception ex, string nombreArchivo)
        {
            File.AppendAllText(uti.directorioLogs + nombreArchivo + ".txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");
        }
    }
}