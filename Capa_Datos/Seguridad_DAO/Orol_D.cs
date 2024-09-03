using Capa_Entidad.Seguridad_ENT;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Seguridad_DAO
{
    public class Orol_D
    {
        DBHelper db = new DBHelper();
        readonly Utilitarios uti = new Utilitarios();

        public List<Orol_E> listarRoles(int idRol)
        {
            List<Orol_E> lista = new List<Orol_E>();
            string query = "";
            if (idRol == 1) { query = "select * from dbo.OROL ORDER BY Nombre ASC"; }
            else if (idRol == 2) { query = "select * from dbo.OROL where id in (" + idRol + ",3) ORDER BY Nombre ASC "; }
            else if (idRol == 4) { query = "select * from dbo.OROL where id in (" + idRol + ",5,50,51,52,53,54,55) ORDER BY Nombre ASC "; }
            else if (idRol == 6) { query = "select * from dbo.OROL where id in (" + idRol + ",7) ORDER BY Nombre ASC "; }
            else if (idRol == 8) { query = "select * from dbo.OROL where id in (" + idRol + ",9) ORDER BY Nombre ASC "; }
            else if (idRol == 10) { query = "select * from dbo.OROL where id in (" + idRol + ") ORDER BY Nombre ASC "; }
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    Orol_E o = new Orol_E();
                    o.Id = dr.GetInt32(0);
                    if (!dr.IsDBNull(1)) { o.Nombre = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.PrefijoId = dr.GetString(2); }
                    lista.Add(o);
                }
                dr.Close();
            }
            catch (Exception e) { throw new Exception(e.Message); }
            return lista;
        }

        public string ObtenerRol(int idRol)
        {
            string result;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT TOP 1 Nombre FROM dbo.OROL WHERE Id = @id";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@id", idRol);

                cn.Open();
                object objResult = cmd.ExecuteScalar();
                result = objResult != null ? objResult.ToString() : "";
                cn.Close();
            }

            return result;
        }
    }
}
