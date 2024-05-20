using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class CLR1_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public List<CLR1_E> listarDetallesCli(string CardCode)
        {
            List<CLR1_E> lista = new List<CLR1_E>();
            string query = "select  CardCode,IdReg,Categoria,Tipo,Cantidad  from vt.CLR1 where CardCode=@CardCode";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string>() { "@CardCode" }, CardCode);
                while (dr.Read())
                {
                    CLR1_E o = new CLR1_E();
                    if (!dr.IsDBNull(0)) { o.CardCode = dr.GetString(0); }
                    if (!dr.IsDBNull(1)) { o.IdReg = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { o.Categoria = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.Tipo = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.Cantidad = dr.GetDecimal(4); }
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
        public CLR1_E buscarDetCliReg(string CardCode, int IdReg)
        {
            CLR1_E o = new CLR1_E();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT CardCode,IdReg,Categoria,Tipo,Cantidad FROM vt.CLR1 WHERE CardCode=@CardCode and IdReg=@IdReg";
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string>() { "@CardCode", "@IdReg" }, CardCode, IdReg);
                cn.Open();

                try
                {
                    dr.Read();
                    if (!dr.IsDBNull(0)) { o.CardCode = dr.GetString(0); }
                    if (!dr.IsDBNull(1)) { o.IdReg = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { o.Categoria = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.Tipo = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.Cantidad = dr.GetDecimal(4); }

                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

                cn.Close();
            }
            
            return o;
        }
    }
}