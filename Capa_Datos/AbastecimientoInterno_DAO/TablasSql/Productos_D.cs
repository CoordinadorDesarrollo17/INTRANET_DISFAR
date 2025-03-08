using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class Productos_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<Productos_E> ListarProductos(string condicion, Dictionary<string, object> parametrosCondicion)
        {
            List<Productos_E> lista = new List<Productos_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT P.[ItemCode], P.[ItemName], P.[FirmCode], P.[Estado_SKU]");
                    sb.AppendLine("FROM BD_39.DOCUMENTOS_REGULATORIOS.dbo.Productos P");
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
                        //REVISION
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var obj = new Productos_E();

                                if (!dr.IsDBNull(0)) obj.ItemCode = dr.GetString(0);
                                if (!dr.IsDBNull(1)) obj.ItemName = dr.GetString(1);
                                if (!dr.IsDBNull(2)) obj.FirmCode = dr.GetInt32(2);
                                if (!dr.IsDBNull(3)) obj.Estado_SKU = dr.GetString(3);

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Productos_D - ListarProductos");
            }

            return lista;
        }
    }
}
