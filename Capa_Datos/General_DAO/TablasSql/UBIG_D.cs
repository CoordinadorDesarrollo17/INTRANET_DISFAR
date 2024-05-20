using Capa_Entidad.General_ENT.TablasSql;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos.General_DAO.TablasSql
{
    public class UBIG_D
    {
        Utilitarios uti = new Utilitarios();
        public List<UBIG_E> Listar()
        {
            List<UBIG_E> lista = new List<UBIG_E>();
            string query = "select * from UBIG";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    UBIG_E o = new UBIG_E();
                    if (!dr.IsDBNull(0)) { o.Ubigeo = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.Distrito = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.Provincia = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.Departamento = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.Zona = dr.GetString(4); }
                    lista.Add(o);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return lista;
        }
    }
}
