using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using Capa_Entidad.Ventas_ENT.TablasSql;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class RTV2_D
    {
        Utilitarios uti = new Utilitarios();
        public List<RTV2_E> BuscarRTV2(int DocEntry)
        {
            List<RTV2_E> lista = new List<RTV2_E>();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                String query = "SELECT * FROM vt.RTV2 where DocEntry=@DocEntry";
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
                            RTV2_E objRTV2 = new RTV2_E();

                            if (!dr.IsDBNull(0)) { objRTV2.DocEntry = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { objRTV2.Linea = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { objRTV2.Monto = dr.GetDecimal(2); }
                            if (!dr.IsDBNull(3)) { objRTV2.NroSap = dr.GetInt32(3); }
                            if (!dr.IsDBNull(4)) { objRTV2.TipoComprobante = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { objRTV2.Vendedor = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { objRTV2.LugarDeEntrega = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { objRTV2.Observaciones = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { objRTV2.AlmacenSalida = dr.GetString(8); }

                            lista.Add(objRTV2);
                        }
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception("Error: " + e.Message);
                }
                cn.Close();
            }

            return lista;
        }


        /*
         *  Descripción: Método para obtener el NroSap ya sea por DocNum o DocEntry indicando explícitamente el tipo de campo con el que vamos a filtrar 
         *  Parámetros: @campo (string), @num (int)
         */
        public List<RTV2_E> BuscarNroSAP(string campo, int num)
        {
            string condWhere = "";
            List<RTV2_E> lista = new List<RTV2_E>();

            if (campo.Equals("DocNum"))
            {
                condWhere = "(SELECT DocEntry FROM vt.ORTV WHERE DocNum = @DocNum)";
            }
            else if (campo.Equals("DocEntry"))
            {
                condWhere = "@DocEntry";
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = $"SELECT x.NroSap from vt.RTV2 x WHERE x.DocEntry = {condWhere} ORDER BY x.Linea";
                SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
                cmd.Parameters.AddWithValue($"@{campo}", num);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            RTV2_E objRTV2 = new RTV2_E();
                            if (!dr.IsDBNull(0)) { objRTV2.NroSap = dr.GetInt32(0); }
                            lista.Add(objRTV2);
                        }
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception("Error: " + e.Message);
                }
                cn.Close();
            }

            return lista;
        }

    }
}
