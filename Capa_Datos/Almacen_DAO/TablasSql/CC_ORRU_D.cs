using System;
using System.Collections.Generic;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using Capa_Entidad.Seguridad_ENT;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class CC_ORRU_D
    {
        Utilitarios uti = new Utilitarios();
        /*
         * Método para listar todos los cambios de estado del ticket
         * Enviar que estado se desea buscar en el parametro @operacion, este solo mostrará el último en caso exista más de 1 (por ejm al Editar Ticket)
         * En caso este parametro se envíe vacío, listará todos los cambios que se han realizado
         */
        public List<CC_ORRU_E> ListarCC_ORRU(int DocEntry, string operacion)
        {
            List<CC_ORRU_E> lista = new List<CC_ORRU_E>();
            String condWhere = "", top = "";
            if (operacion != "")
            {
                top = "TOP 1";
                condWhere = $"AND Operacion = '{operacion}'";
            }

            string query = $"SELECT {top} Id, DocEntry, Operacion, Operario, CONVERT(varchar, FechaOperacion, 103) AS FechaOperacion, HoraOperacion FROM vt.CC_ORRU WHERE DocEntry = @DocEntry {condWhere} ORDER BY Id DESC";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        CC_ORRU_E cc = new CC_ORRU_E();
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
                    CC_ORRU_E cc = new CC_ORRU_E { Id = 0, DocEntry = 0, Operacion = operacion, Operario = "", FechaOperacion = "", HoraOperacion = "" };
                    lista.Add(cc);
                }

                dr.Close();
                cn.Close();
            }
            catch
            {
                cn.Close();
            }
            return lista;
        }

        /*
         * Descripción: Método para saber el último estado del ticket
         * Parámetros: @DocEntry (int)
         * Usos: Seguimiento de Ticket
         */
        public string UltimoEstadoCC_ORRU(int DocEntry)
        {
            String query = "", ultimoEstado = "";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                query = $"SELECT TOP 1 Operacion FROM vt.CC_ORRU WHERE DocEntry = @DocEntry ORDER BY Id DESC";

                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr = cmd.ExecuteReader();                     // ejecuta

                if (dr.HasRows)
                {
                    dr.Read();
                    if (!dr.IsDBNull(0)) { ultimoEstado = dr.GetString(0); }
                }
            }
            catch(Exception e)
            {
                cn.Close();
                throw new Exception(e.Message);
            }

            return ultimoEstado;
        }

    }
}
