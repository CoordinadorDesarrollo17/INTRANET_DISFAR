using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.General_DAO.TablasSql
{
    public class UBICACIONES_D
    {
        Utilitarios uti = new Utilitarios();
        public string[] BuscarUbicaciones(string ItemCode)
        {
            // Lista para almacenar los resultados de las ubicaciones
            List<string> ubicaciones = new List<string>();

            try
            {
                string query = "EXEC dbo.UbicacionesPorArticulo @ItemCode";

                using (SqlConnection connection = new SqlConnection(uti.cadSql))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemCode", ItemCode);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ubicaciones.Add(reader["CodigoUbicacion"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar ubicaciones: {ex.Message}");
            }

            return ubicaciones.ToArray();
        }
    }
}
