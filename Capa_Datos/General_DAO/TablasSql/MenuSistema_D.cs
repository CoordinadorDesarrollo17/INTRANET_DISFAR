using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capa_Entidad;
using Capa_Entidad.General_ENT.TablasSql;

namespace Capa_Datos.General_DAO.TablasSql
{
    public class MenuSistema_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public (Capa_Entidad.Helper_E, List<MenuSistema_E>) ListarMenuSistema(string condicion, Dictionary<string, object> parametros)
        {
            List<MenuSistema_E> lista = new List<MenuSistema_E>();
            var helper = new Helper_E();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            using (var cmd = new SqlCommand())
            {
                try
                {
                    cmd.Connection = cn;

                    var sb = new StringBuilder();
                    sb.AppendLine("SELECT Id, Nombre, Nivel, SuperiorId, Ruta, Icono, Orden");
                    sb.AppendLine("FROM MenuSistema");
                    sb.AppendLine("WHERE 1=1");
                    sb.AppendLine(condicion);

                    // Agregamos los parámetros dinámicamente
                    foreach (var param in parametros)
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
                                var obj = new MenuSistema_E();

                                if (!dr.IsDBNull(0)) obj.Id = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) obj.Nombre = dr.GetString(1);
                                if (!dr.IsDBNull(2)) obj.Nivel = dr.GetInt32(2);
                                if (!dr.IsDBNull(3)) obj.SuperiorId = dr.GetInt32(3);
                                if (!dr.IsDBNull(4)) obj.Ruta = dr.GetString(4);
                                if (!dr.IsDBNull(5)) obj.Icono = dr.GetString(5);
                                if (!dr.IsDBNull(6)) obj.Orden = dr.GetInt32(6);

                                lista.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.RegistrarError(ex, "MenuSistema_D - ListarMenuSistema");
                    helper.Titulo = "Error";
                    helper.Mensajes.Add("Ocurrió un error al listar");
                    helper.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                    helper.Icono = "error";
                }
            }

            return (helper, lista);
        }
    }
}
