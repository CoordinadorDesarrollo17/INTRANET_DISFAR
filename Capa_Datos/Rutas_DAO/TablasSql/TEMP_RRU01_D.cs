using Capa_Entidad.Rutas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Capa_Datos.Rutas_DAO.TablasSql
{
    public class TEMP_RRU01_D
    {
        Utilitarios uti = new Utilitarios();
        public List<TEMP_RRU01_E> Obtener(int DocEntryTicket)
        {
            List<TEMP_RRU01_E> lista = new List<TEMP_RRU01_E>();
            using (SqlConnection connection = new SqlConnection(uti.cadSql))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT * FROM al.TEMP_RRU01 where DocEntryTicket=@DocEntryTicket";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DocEntryTicket", DocEntryTicket);
                        command.CommandType = CommandType.Text;
                        SqlDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            TEMP_RRU01_E o = new TEMP_RRU01_E();
                            if (!dr.IsDBNull(1)) { o.TablaSAP = dr.GetString(1); }
                            if (!dr.IsDBNull(2)) { o.Identificador = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { o.DocEntryTicket = dr.GetInt32(3); }
                            if (!dr.IsDBNull(4)) { o.U_SYP_MDTD = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { o.U_SYP_MDSD = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { o.U_SYP_MDCD = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { o.DocDate = dr.GetDateTime(7).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(8)) { o.U_BPP_FECINITRA = dr.GetDateTime(8).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(9)) { o.Impreso = dr.GetInt32(9); }
                            lista.Add(o);
                        }
                    }
                    return lista;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en la función listar: {ex.Message}");
                    return lista;
                }
            }
        }
        public string Registrar(List<TEMP_RRU01_E> dataList)
        {
            using (SqlConnection connection = new SqlConnection(uti.cadSql))
            {
                try
                {
                    connection.Open();
                    int DocNum = dataList[0].DocEntryTicket + 2000000000;
                    // Eliminar todos los registros que tengan el DocEntryTicket en al.TEMP RRU01
                    string deleteQuery = "DELETE FROM al.TEMP_RRU01 WHERE DocEntryTicket = @DocEntryTicket";
                    using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@DocEntryTicket", dataList.First().DocEntryTicket);
                        deleteCommand.ExecuteNonQuery();
                    }

                    foreach (var data in dataList)
                    {
                        string query = $"INSERT INTO al.TEMP_RRU01 VALUES (@TablaSAP, @Identificador, @DocEntryTicket, @U_SYP_MDTD,@U_SYP_MDSD,@U_SYP_MDCD,@DocDate,@U_BPP_FECINITRA,0,@Operario,(select convert(varchar,getdate(),23)),(select convert(char(5),getdate(),108)) )";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@TablaSAP", data.TablaSAP);
                            command.Parameters.AddWithValue("@Identificador", data.Identificador);
                            command.Parameters.AddWithValue("@DocEntryTicket", data.DocEntryTicket);
                            command.Parameters.AddWithValue("@U_SYP_MDTD", data.U_SYP_MDTD);
                            command.Parameters.AddWithValue("@U_SYP_MDSD", data.U_SYP_MDSD);
                            command.Parameters.AddWithValue("@U_SYP_MDCD", data.U_SYP_MDCD);
                            command.Parameters.AddWithValue("@DocDate", data.DocDate);
                            command.Parameters.AddWithValue("@U_BPP_FECINITRA", (object)data.U_BPP_FECINITRA ?? DBNull.Value);
                            command.Parameters.AddWithValue("@Operario", data.Operario);
                            command.ExecuteNonQuery();
                        }
                    }

                    var gruposPorIdentificador = dataList.GroupBy(data => data.Identificador).OrderBy(grupo => grupo.Key);

                    // Formar una tabla completa HTML con las columnas específicas para cada identificador
                    string tabla = $"<div class='table-responsive'><table border='2' style='width: 100%;border-color:green'>";

                    var correlativosPorIdentificador = new Dictionary<string, List<string>>();

                    foreach (var grupo in gruposPorIdentificador)
                    {
                        string identificador = grupo.Key;

                        correlativosPorIdentificador[identificador] = grupo
                            .OrderBy(data => $"{data.U_SYP_MDTD}-{data.U_SYP_MDSD}-{data.U_SYP_MDCD}")
                            .Select(data => $"{data.U_SYP_MDTD}-{data.U_SYP_MDSD}-{data.U_SYP_MDCD}")
                            .ToList();
                    }

                    // Obtener la cantidad máxima de correlativos en un solo identificador
                    int maxCorrelativos = correlativosPorIdentificador.Values.Max(correlativos => correlativos.Count);

                    for (int i = 0; i < maxCorrelativos; i++)
                    {
                        tabla += "<tr>";

                        foreach (var correlativosList in correlativosPorIdentificador.Values)
                        {
                            string correlativo = i < correlativosList.Count ? correlativosList[i] : "";
                            string linkcorrelativo = $"<a onclick=\"ImprimirUnitarioDocumento(event,'{correlativo}',{DocNum},false)\" href=\"#\">{correlativo}</a>";

                            //string checkboxId = correlativo.Replace("-", "_"); // Reemplaza los guiones con guiones bajos para evitar problemas con el ID
                            //string checkboxHtml = string.IsNullOrEmpty(correlativo) ? "" : $"<input type='checkbox' id='{checkboxId}'>";
                            tabla += $"<td class='text-dark'>{linkcorrelativo}</td>";
                        }

                        tabla += "</tr>";
                    }

                    // Añadir las columnas faltantes en caso de que no haya correlativos para ellas
                    /*foreach (var grupo in gruposPorIdentificador)
                    {
                        string identificador = grupo.Key;
                        if (!correlativosPorIdentificador.ContainsKey(identificador))
                        {
                            int totalColumnas = correlativosPorIdentificador.Count;
                            for (int j = 0; j < totalColumnas; j++)
                            {
                                tabla += "<td></td>";
                            }
                        }
                    }*/
                    //Luego de armar toda la tabla con sus correlativos identificar 
                    /*int totalColumnasGene = gruposPorIdentificador.Count();
                    for (int i = 0; i < totalColumnasGene; i++)
                    {
                        foreach ( var o in gruposPorIdentificador)
                        {
                            string identificador = o.Key;
                            switch ( identificador)
                            {
                                case "F":
                                    cabecera += "<th class='text-center text-dark'>Facturas</th>";
                                    break;
                                case "B":
                                    cabecera += "<th class='text-center text-dark'>Boletas</th>";
                                    break;
                                case "G":
                                    cabecera += "<th class='text-center text-dark'>Guias</th>";
                                    break;
                                case "NC":
                                    cabecera += "<th class='text-center text-dark'>Notas Credito</th>";
                                    break;
                                case "ND":
                                    cabecera += "<th class='text-center text-dark'>Notas Debito</th>";
                                    break;
                            }
                            break;
                        }
                    }*/
                    tabla += "</table></div>";
                    return tabla;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en la función Registrar: {ex.Message}");
                    return null;
                }
            }
        }
        public List<string> Listar(int DocEntryTicket)
        {
            List<string> lista = new List<string>();
            using (SqlConnection connection = new SqlConnection(uti.cadSql))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT concat(U_SYP_MDTD,'-',U_SYP_MDSD,'-',U_SYP_MDCD) FROM al.TEMP_RRU01 where DocEntryTicket=@DocEntryTicket";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DocEntryTicket", DocEntryTicket);
                        command.CommandType = CommandType.Text;
                        SqlDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            string correlativo = "";
                            if (!dr.IsDBNull(0)) { correlativo = dr.GetString(0); }
                            lista.Add(correlativo);
                        }
                    }
                    return lista;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en la función Registrar: {ex.Message}");
                    return lista;
                }
            }
        }
        public void EditarImpreso(string NumAtCard, int DocEntryTicket)
        {
            using (SqlConnection connection = new SqlConnection(uti.cadSql))
            {
                try
                {
                    connection.Open();
                    string query = $"UPDATE al.TEMP_RRU01 SET Impreso=@Impreso WHERE DocEntryTicket=@DocEntryTicket AND CONCAT(U_SYP_MDTD,'-',U_SYP_MDSD,'-',U_SYP_MDCD)=@NumAtCard";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NumAtCard", NumAtCard);
                        command.Parameters.AddWithValue("@DocEntryTicket", DocEntryTicket);
                        command.Parameters.AddWithValue("@Impreso", 1);
                        command.ExecuteNonQuery();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en la función EditarImpreso: {ex.Message}");
                }
            }
        }
        public void Eliminar(int DocEntryTicket)
        {
            using (SqlConnection connection = new SqlConnection(uti.cadSql))
            {
                try
                {
                    connection.Open();
                    string query = $"DELETE  FROM al.TEMP_RRU01 WHERE Impreso=@Impreso AND DocEntryTicket=@DocEntryTicket ";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DocEntryTicket", DocEntryTicket);
                        command.Parameters.AddWithValue("@Impreso", 1);
                        command.ExecuteNonQuery();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en la función Eliminar: {ex.Message}");
                }
            }
        }
        public int ConsultarImpreso(string NumAtCard)
        {
            int Impreso = 0;
            using (SqlConnection connection = new SqlConnection(uti.cadSql))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT Impreso FROM al.TEMP_RRU01  WHERE  CONCAT(U_SYP_MDTD,'-',U_SYP_MDSD,'-',U_SYP_MDCD)=@NumAtCard";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NumAtCard", NumAtCard);
                        SqlDataReader dr = command.ExecuteReader();
                        while (dr.Read())
                        {
                            if (!dr.IsDBNull(0)) { Impreso = dr.GetInt32(0); }
                        }
                    }

                }
                catch { }
                return Impreso;
            }
        }
    }
}
