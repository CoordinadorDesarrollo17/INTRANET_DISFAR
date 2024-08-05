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

                    sb.Append("SELECT OP.Id, OP.Nombre, OP.IdModulo, OP.Grup_OpeId, GOPE.Controlador");
                    sb.Append(" FROM dbo.OOPE OP");
                    sb.Append(" INNER JOIN dbo.GRUP_OPE GOPE ON OP.Grup_OpeID = GOPE.Id");
                    sb.Append(" WHERE 1 = 1");

                    if (filtros != null)
                    {
                        if (filtros.idModulo > 0)
                        {
                            sb.Append(" AND OP.IdModulo = @IdModulo");
                            cmd.Parameters.AddWithValue("@IdModulo", filtros.idModulo);
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

                                if (!dr.IsDBNull(0)) { obj.id = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.nombre = dr.GetString(1); }
                                if (!dr.IsDBNull(2)) { obj.idModulo = dr.GetInt32(2); }
                                if (!dr.IsDBNull(3)) { obj.Grup_OpeID = dr.GetInt32(3); }
                                if (!dr.IsDBNull(4)) { obj.Controlador = dr.GetString(4); }

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