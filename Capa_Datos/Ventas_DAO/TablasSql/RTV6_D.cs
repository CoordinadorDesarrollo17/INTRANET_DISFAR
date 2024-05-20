using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class RTV6_D
    {
        Utilitarios uti = new Utilitarios();
        public void AsignarPrecio(int DocEntry,int Linea,decimal Precio)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            string query = "update vt.RTV6 set PrecioEnv=@PrecioEnv where DocEntry=@DocEntry AND Linea=@Linea";
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn) { Transaction = tran, CommandType = CommandType.Text };
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@Linea", Linea);
                    cmd.Parameters.AddWithValue("@PrecioEnv",Precio);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (SqlException e) { tran.Rollback(); cn.Close(); throw new Exception(e.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception(e2.Message); }
        }

        /*
        * Descripción: Método para calcular el peso total del pedido
        * Parámetros: @DocEntry (int)
        * Usos: SeguimientoDeTicket
        */
        public decimal ObtenerPesoTotal(int DocEntry)
        {
            decimal pesoTotal = 0;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                    string query = "SELECT SUM(Peso) FROM vt.RTV6 WHERE DocEntry = @DocEntry";
                    SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cn.Open();
                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();                     // ejecuta

                    if (dr.HasRows)
                    {
                        dr.Read();
                        if (!dr.IsDBNull(0)) { pesoTotal = dr.GetDecimal(0); }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                    cn.Close();
            }

            return pesoTotal;
        }


    }
    
}
