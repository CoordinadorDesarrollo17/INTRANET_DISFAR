using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Capa_Entidad.Almacen_ENT.TablasSql;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class CC_ORPD_D
    {
        readonly Utilitarios uti = new Utilitarios();

        /*
         * Método para listar todos los cambios de estado de Devolución de mercancías
         * Enviar que estado se desea buscar en el parametro @operacion, este solo mostrará el último en caso exista más de 1 (por ejm al SELECCIONAR)
         * En caso este parametro se envíe vacío, listará todos los cambios que se han realizado
         */
        public List<CC_ORPD_E> ListarCC_ORPD(CC_ORPD_E filtros)
        {
            List<CC_ORPD_E> lista = new List<CC_ORPD_E>();
            String condWhere = "", top = "";

            if (!string.IsNullOrEmpty(filtros.Operacion))
            {
                top = "TOP 1";
                condWhere += $"AND Operacion = '{filtros.Operacion}'";
            }

            if (!string.IsNullOrEmpty(filtros.Operario))
            {
                top = "TOP 1";
                condWhere += $"AND Operario LIKE '%{filtros.Operario}%'";
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = $"SELECT {top} Id, DocEntry, Operacion, Operario, CONVERT(varchar, FechaOperacion, 103) AS FechaOperacion, HoraOperacion FROM al.CC_ORPD WHERE DocEntry = @DocEntry {condWhere} ORDER BY Id DESC";
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocEntry", filtros.DocEntry);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            CC_ORPD_E cc = new CC_ORPD_E();
                            if (!dr.IsDBNull(0)) { cc.Id = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { cc.DocEntry = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { cc.Operacion = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { cc.Operario = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { cc.FechaOperacion = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { cc.HoraOperacion = dr.GetTimeSpan(5).ToString(); }
                            lista.Add(cc);
                        }
                    }

                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cn.Close();
                }

            }

            return lista;
        }

        /*
        * Descripción: Método para saber el último estado del periodo
        * Parámetros: @DocEntry (int)
        * Usos: Devolución de mercancías
        */
        public string UltimoEstadoCC_ORPD(int DocEntry)
        {
            string query = string.Empty, ultimoEstado = string.Empty;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                query = $"SELECT TOP 1 Operacion FROM al.CC_ORPD WHERE DocEntry = @DocEntry ORDER BY Id DESC";

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
                throw new Exception(e.Message);
            }
            finally
            {
                cn.Close();
            }

            return ultimoEstado;
        }
    }
}
