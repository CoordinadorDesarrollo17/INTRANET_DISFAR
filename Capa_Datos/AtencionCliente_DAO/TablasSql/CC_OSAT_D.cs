using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Capa_Entidad.AtencionCliente_ENT.TablasSql;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.AtencionCliente_DAO.TablasSql
{
	public class CC_OSAT_D
	{
		Utilitarios uti = new Utilitarios();

        /*
         * Método para listar todos los cambios de estado de la solicitud atención al cliente
         * Enviar que estado se desea buscar en el parametro @operacion, este solo mostrará el último en caso exista más de 1 (por ejm al SELECCIONAR)
         * En caso este parametro se envíe vacío, listará todos los cambios que se han realizado
         */
        public List<CC_OSAT_E> ListarCC_OSAT(int DocEntry, string operacion)
        {
            List<CC_OSAT_E> lista = new List<CC_OSAT_E>();
            String condWhere = "", top = "";
            if (operacion != "")
            {
                top = "TOP 1";
                condWhere = $"AND Operacion = '{operacion}'";
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = $"SELECT {top} Id, DocEntry, Operacion, Operario, CONVERT(varchar, FechaOperacion, 103) AS FechaOperacion, HoraOperacion FROM ac.CC_OSAT WHERE DocEntry = @DocEntry {condWhere} ORDER BY Id DESC";
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
                            CC_OSAT_E cc = new CC_OSAT_E();
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
                        CC_OSAT_E cc = new CC_OSAT_E { Id = 0, DocEntry = 0, Operacion = operacion, Operario = "", FechaOperacion = "", HoraOperacion = "" };
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
        * Descripción: Método para saber el último estado de la solicitud atención al cliente
        * Parámetros: @DocEntry (int)
        * Usos: AtenderSolicitud
        */
        public string UltimoEstadoCC_OSAT(int DocEntry)
        {
            String query = "", ultimoEstado = "";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                query = $"SELECT TOP 1 Operacion FROM ac.CC_OSAT WHERE DocEntry = @DocEntry ORDER BY Id DESC";

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
