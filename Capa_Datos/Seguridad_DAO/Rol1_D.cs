using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Capa_Entidad.Seguridad_ENT;

namespace Capa_Datos.Seguridad_DAO
{
    public class Rol1_D
    {
        Utilitarios uti = new Utilitarios();
        DBHelper db = new DBHelper();
        public int verificarAccesoOperacion(int idRol, int idOperacion, string nombreOperacion, int modulo)
        {
            registrarOperacion(idOperacion, nombreOperacion, modulo);
            int result = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ROL1 WHERE idRol=" + idRol + " AND idOperacion=" + idOperacion, cn);
                result = (int)cmd.ExecuteScalar();
                cn.Close();
            }
            catch (Exception e) { cn.Close(); throw new Exception(e.Message); }
            return result;
        }
        private void registrarOperacion(int idOperation, string operacion, int modulo)
        {
            int existOpe = 1;
            string query = "";
            query = "select count(*) from oope where id=" + idOperation;
            try
            {
                existOpe = (int)db.ExecuteScalarNoSp(query);
            }
            catch { existOpe = -1; }
            if (existOpe == 0 && idOperation > 0)
            {
                query = "insert into oope values(@idOperation,@operacion,@modulo)";
                try
                {
                    db.ExecuteNonQueryTrxNoSp(query, new List<string> { "@idOperation", "@operacion", "@modulo" }, idOperation, operacion, modulo);
                }
                catch { }
            }
        }
        /*alisson*/
        public void crudOperacion(int idRol, int[] numeros)
        {

            string query0 = ""; string query1 = ""; string query2 = "";
            List<int> lista = new List<int>(); int cont = 0;
            query0 = "SELECT max(id) FROM ROL1 ";
            query1 = "SELECT idOperacion FROM ROL1 where idRol=" + idRol + " order by idOperacion";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            cn.Open();
            try
            {
                SqlCommand cmd = new SqlCommand(query0, cn);
                cmd.CommandType = CommandType.Text;
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                if (!dr.IsDBNull(0)) { cont = dr.GetInt32(0); }
                dr.Close();
            }
            catch { cn.Close(); }
            try
            {
                SqlDataReader dr1 = db.ExecuteReaderNoSp(query1);
                while (dr1.Read())
                {
                    lista.Add(dr1.GetInt32(0));
                }
                dr1.Close();
                int[] operaciones = lista.ToArray();
                foreach (int a in numeros)
                {

                    if (operaciones.Contains(a) == false)
                    {
                        cont = cont + 1;
                        query2 = "insert into rol1 values(" + cont + "," + idRol + "," + a + ")";
                        SqlCommand cmd = new SqlCommand(query2, cn);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        foreach (int b in operaciones)
                        {
                            if (numeros.Contains(b) == false)
                            {
                                query2 = "delete from rol1 where idOperacion=" + b + " and idRol=" + idRol;
                                SqlCommand cmd = new SqlCommand(query2, cn);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch { cn.Close(); }

            cn.Close();

        }
    }
}