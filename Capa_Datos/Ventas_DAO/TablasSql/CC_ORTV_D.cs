using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class CC_ORTV_D
    {
        Utilitarios uti = new Utilitarios();
        /*
         * Método para listar todos los cambios de estado del ticket
         * Enviar que estado se desea buscar en el parametro @operacion, este solo mostrará el último en caso exista más de 1 (por ejm al Editar Ticket)
         * En caso este parametro se envíe vacío, listará todos los cambios que se han realizado
         */

        public List<CC_ORTV_E> ObtenerOperacionesTicket(int DocEntry, List<string> tiposOperacion, bool excluir = false)
        {
            List<CC_ORTV_E> lista = new List<CC_ORTV_E>();
            string condWhere = string.Empty, top = string.Empty;

            if (excluir) { condWhere = $"AND Operacion not in ('IMPRIMIR','EDITAR','EDITAR SUPERVISOR','FACTURAR','ANULAR FACTURAR','GUIA EMITIDA','REVERTIR GUIA')"; }
            // Si se buscan tipos de operación específicos
            if (tiposOperacion != null && tiposOperacion.Count > 0)
            {
                string tiposOperacionString = string.Join("','", tiposOperacion);
                condWhere += $"AND Operacion IN ('{tiposOperacionString}')";
            }

            // Query para obtener la última operación por tipo de operación y DocEntry
            string query = $@"
            SELECT Id, DocEntry, Operacion, Operario, CONVERT(varchar, FechaOperacion, 103) AS FechaOperacion, HoraOperacion
            FROM vt.CC_ORTV
            WHERE DocEntry = @DocEntry {condWhere}
            AND Id IN (SELECT MAX(Id) FROM vt.CC_ORTV WHERE DocEntry = @DocEntry {condWhere} GROUP BY Operacion)
            ORDER BY Operacion, Id DESC";

            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    CC_ORTV_E cc = new CC_ORTV_E();
                    cc.Id = dr.GetInt32(0);
                    cc.DocEntry = dr.GetInt32(1);
                    cc.Operacion = dr.GetString(2);
                    cc.Operario = dr.GetString(3);
                    cc.FechaOperacion = dr.GetString(4);
                    cc.HoraOperacion = dr.GetTimeSpan(5).ToString();
                    lista.Add(cc);
                }

                dr.Close();
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener operaciones: {ex.Message}");
                cn.Close();
            }

            return lista;
        }

        public List<CC_ORTV_E> ListarCC_ORTV(int DocEntry, string operacion, bool excluir = false)
        {
            List<CC_ORTV_E> lista = new List<CC_ORTV_E>();
            string condWhere = string.Empty, top = string.Empty;
            if (!string.IsNullOrWhiteSpace(operacion))
            {
                top = "TOP 1";
                condWhere = $"AND Operacion = '{operacion}'";
            }
            if (excluir) { condWhere = $"AND Operacion not in ('IMPRIMIR','EDITAR','EDITAR SUPERVISOR','FACTURAR','ANULAR FACTURAR','GUIA EMITIDA','REVERTIR GUIA')"; }
            string query = $"SELECT {top} Id, DocEntry, Operacion, Operario, CONVERT(varchar, FechaOperacion, 103) AS FechaOperacion, HoraOperacion FROM vt.CC_ORTV WHERE DocEntry = @DocEntry {condWhere} ORDER BY Id DESC";
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
                        CC_ORTV_E cc = new CC_ORTV_E();
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
                    lista.Add(new CC_ORTV_E());
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
        public Dictionary<string, CC_ORTV_E> ListarCC_FlujoEstados(int DocEntry)
        {
            Dictionary<string, CC_ORTV_E> lista = new Dictionary<string, CC_ORTV_E>();
            string[] operaciones = {"IMPRIMIR","SEPARAR","REGISTRAR", "EDITAR", "CANCELAR", "ANULAR","RECIBIR", "ANULAR RECIBIR", "FACTURAR", "ANULAR FACTURAR",
                                                    "INICIO PICKING","ANULAR INICIO PICKING","FIN PICKING", "ANULAR FIN PICKING","INICIO VERIFICAR","ANULAR INICIO VERIFICAR",
                                                    "FIN VERIFICAR","ANULAR FIN VERIFICAR", "INICIO EMPACAR","ANULAR INICIO EMPACAR","FIN EMPACAR", "ANULAR FIN EMPACAR",
                                                    "PESAR", "ANULAR PESAR","LIBERAR", "PREENVIAR", "ENVIAR", "ANULAR ENVIAR", "ENTREGAR", "ANULAR ENTREGAR", "EDITAR SUPERVISOR" };

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    string query = string.Empty;
                    cn.Open();

                    foreach (string op in operaciones)
                    {
                        query = $"SELECT TOP 1 Id, DocEntry, Operacion, Operario, CONVERT(varchar, FechaOperacion, 103) AS FechaOperacion, HoraOperacion FROM vt.CC_ORTV WHERE DocEntry = @DocEntry AND Operacion = '{op}' ORDER BY Id DESC";

                        SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                        cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                        SqlDataReader dr = cmd.ExecuteReader();                     // ejecuta

                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                CC_ORTV_E cc = new CC_ORTV_E();
                                if (!dr.IsDBNull(0)) { cc.Id = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { cc.DocEntry = dr.GetInt32(1); }
                                if (!dr.IsDBNull(2)) { cc.Operacion = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { cc.Operario = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { cc.FechaOperacion = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { cc.HoraOperacion = dr.GetTimeSpan(5).ToString(); }
                                lista.Add($"{op}", cc);
                            }

                        }
                        else
                        {
                            //CC_ORTV_E cc = new CC_ORTV_E { Id = 0, DocEntry = 0, Operacion = op, Operario = "", FechaOperacion = "", HoraOperacion = "" };
                            CC_ORTV_E cc = new CC_ORTV_E { };
                            lista.Add($"{op}", cc);
                        }
                        dr.Close();
                    }

                    cn.Close();
                }
                catch (Exception e)
                {
                    cn.Close();
                    throw new Exception(e.Message);
                }
            }

            return lista;
        }
        /*
         * Descripción: Método para saber el último estado del ticket
         * Parámetros: @DocEntry (int)
         * Usos: Seguimiento de Ticket
         */
        public string UltimoEstadoCC_ORTV(int DocEntry)
        {
            String query = "", ultimoEstado = "";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                query = $"SELECT TOP 1 Operacion FROM vt.CC_ORTV WHERE DocEntry = @DocEntry AND FechaOperacion IS NOT NULL AND HoraOperacion IS NOT NULL ORDER BY Id DESC";

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
