using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Capa_Entidad.Almacen_ENT.TablasSql;
namespace Capa_Datos.Almacen_DAO.TablasSql
{
	public class CC_OIPE_D
	{
		Utilitarios uti = new Utilitarios();

        /*
         * Método para listar todos los cambios de estado del periodo
         * Enviar que estado se desea buscar en el parametro @operacion, este solo mostrará el último en caso exista más de 1 (por ejm al SELECCIONAR)
         * En caso este parametro se envíe vacío, listará todos los cambios que se han realizado
         */
        public List<CC_OIPE_E> ListarCC_OIPE(int DocEntry, string operacion)
        {
            List<CC_OIPE_E> lista = new List<CC_OIPE_E>();
            String condWhere = "", top = "";
            if (operacion != "")
            {
                top = "TOP 1";
                condWhere = $"AND Operacion = '{operacion}'";
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = $"SELECT {top} Id, DocEntry, Operacion, Operario, CONVERT(varchar, FechaOperacion, 103) AS FechaOperacion, HoraOperacion FROM al.CC_OIPE WHERE DocEntry = @DocEntry {condWhere} ORDER BY Id DESC";
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            CC_OIPE_E cc = new CC_OIPE_E();
                            if (!dr.IsDBNull(0)) { cc.Id = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { cc.DocEntry = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { cc.Operacion = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { cc.Operario = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { cc.FechaOperacion = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { cc.HoraOperacion = dr.GetTimeSpan(5).ToString(); }
                            lista.Add(cc);
                        }
                    }
                    else
                    {
                        CC_OIPE_E cc = new CC_OIPE_E { Id = 0, DocEntry = 0, Operacion = operacion, Operario = "", FechaOperacion = "", HoraOperacion = "" };
                        lista.Add(cc);
                    }

                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }

                cn.Close();
            }

            return lista;
        }

        /*
        * Descripción: Método para saber el último estado del periodo
        * Parámetros: @DocEntry (int)
        * Usos: Gestión de periodos
        */
        public string UltimoEstadoCC_OIPE(int DocEntry)
        {
            String query = "", ultimoEstado = "";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                query = $"SELECT TOP 1 Operacion FROM al.CC_OIPE WHERE DocEntry = @DocEntry ORDER BY Id DESC";

                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr = cmd.ExecuteReader();                     // ejecuta

                if (dr.HasRows)
                {
                    dr.Read();
                    if (!dr.IsDBNull(0)) { ultimoEstado = dr.GetString(0); }
                }
            }
            catch (Exception e)
            {
                cn.Close();
                throw new Exception(e.Message);
            }

            return ultimoEstado;
        }
    }
}
