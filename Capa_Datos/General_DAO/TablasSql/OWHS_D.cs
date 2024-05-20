using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos.General_DAO.TablasSql
{
    public class OWHS_D
    {
        Utilitarios uti = new Utilitarios();
        public List<OWHS_E> listarAlmacenes(string[] arrWhsCode = null)
        {
            List<OWHS_E> lista = new List<OWHS_E>();
            string condWhere = string.Empty;

            if (arrWhsCode != null && arrWhsCode.Count() >= 1)
            {
                condWhere = $"WHERE WhsCode IN ('{string.Join("', '", arrWhsCode)}')";
            }
            string query = "select * from al.OWHS "+condWhere;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    OWHS_E o = new OWHS_E();
                    if (!dr.IsDBNull(0)) { o.WhsCode = dr.GetString(0); }
                    if (!dr.IsDBNull(1)) { o.WhsName = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.Street = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.Block = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.ZipCode = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { o.City = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { o.County = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { o.Country = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { o.Building = dr.GetString(8); }
                    if (!dr.IsDBNull(9)) { o.StreetNo = dr.GetString(9); }
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