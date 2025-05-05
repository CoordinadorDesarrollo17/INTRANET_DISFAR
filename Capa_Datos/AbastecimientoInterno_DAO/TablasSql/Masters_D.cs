using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class Masters_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<Masters_E> ListarMasters(string condicion, Dictionary<string, object> parametrosCondicion)
        {
            List<Masters_E> lista = new List<Masters_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT [Id], [UmAlm], [ValorUmAlm]");
                    sb.AppendLine("FROM Masters");
                    sb.AppendLine("WHERE 1= 1");
                    sb.AppendLine(condicion);

                    // Agregamos los parámetros dinámicamente
                    foreach (var param in parametrosCondicion)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    cmd.CommandText = sb.ToString();

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var obj = new Masters_E();

                                if (!dr.IsDBNull(0)) obj.Id = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) obj.UmAlm = dr.GetString(1);
                                if (!dr.IsDBNull(2)) obj.ValorUmAlm= dr.GetInt32(2);

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Masters_D - ListarMasters");
            }

            return lista;
        }
    }
}
