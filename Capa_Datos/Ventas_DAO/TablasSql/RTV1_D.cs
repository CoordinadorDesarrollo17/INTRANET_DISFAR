using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Capa_Entidad.Ventas_ENT.TablasSql;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class RTV1_D
    {
        Utilitarios uti = new Utilitarios();
        public List<RTV1_E> BuscarRTV1(int DocEntry)
        {
            List<RTV1_E> lista = new List<RTV1_E>();

            string query = "SELECT * FROM vt.RTV1 where DocEntry=@DocEntry";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
            cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
            cn.Open();

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RTV1_E o = new RTV1_E();

                        if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { o.Linea = dr.GetInt32(1); }
                        if (!dr.IsDBNull(2)) { o.NombrePer = dr.GetString(2); }
                        if (!dr.IsDBNull(3)) { o.TelfPer = dr.GetString(3); }
                        if (!dr.IsDBNull(4)) { o.TipoDocPer = dr.GetString(4); }
                        if (!dr.IsDBNull(5)) { o.DocPer = dr.GetString(5); }

                        lista.Add(o);
                    }
                }
                
                dr.Close();
            } catch (Exception e)  {
                throw new Exception("Error: " + e.Message);
            }
            cn.Close();

            return lista;
        }
    }
}
