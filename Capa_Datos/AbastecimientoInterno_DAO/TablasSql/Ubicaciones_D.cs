using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class Ubicaciones_D
    {
        readonly Utilitarios uti = new Utilitarios();
        public string[] BuscarUbicaciones(string almacen, string itemCode)
        {
            List<string> ubicaciones = new List<string>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_AdministrarUbicaciones", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Operacion", "BUSCAR");
                        cmd.Parameters.AddWithValue("@Almacen", almacen);
                        cmd.Parameters.AddWithValue("@ItemCode", itemCode);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ubicaciones.Add(reader.GetString(0)); // Leer cada ubicación en la primera columna
                            }
                        }
                    }
                }

                if (ubicaciones.Count == 0)
                {
                    return new string[] { "No se encontraron ubicaciones para el producto." };
                }

                return ubicaciones.ToArray(); // Retornar el array de ubicaciones
            }
            catch (SqlException sqlEx)
            {
                LogHelper.RegistrarError(sqlEx, "Error SQL en UbicacionesDAL - BuscarUbicaciones.");
                return new string[] { "Error en la base de datos. Intente nuevamente." };
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error inesperado en UbicacionesDAL - BuscarUbicaciones.");
                return new string[] { "Ocurrió un problema inesperado. Contacte a soporte técnico." };
            }
        }

        public Helper_E EliminarUbicacionGeneral(string codigoUbicacion)
        {
            string mensajeUsuario, icono;
            int filasAfectadas = 0;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_AdministrarUbicaciones", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Operacion", "DELETE_GENERAL");
                        cmd.Parameters.AddWithValue("@CodigoUbicacion", codigoUbicacion);

                        filasAfectadas = cmd.ExecuteNonQuery();
                    }
                }

                if (filasAfectadas > 0)
                {
                    mensajeUsuario = "Ubicación eliminada correctamente.";
                    icono = "success";
                }
                else
                {
                    mensajeUsuario = "No se encontró ubicación a eliminar.";
                    icono = "warning";
                }
            }
            catch (SqlException sqlEx)
            {
                mensajeUsuario = (sqlEx.Number == 50000) ? sqlEx.Message : "No se pudo eliminar la ubicación. Intente nuevamente.";
                icono = "error";

                LogHelper.RegistrarError(sqlEx, $"Error SQL en UbicacionesReserva_D - EliminarUbicacionGeneral.");
            }
            catch (Exception ex)
            {
                mensajeUsuario = "Ocurrió un problema inesperado. Por favor, comunicarse con SISTEMAS.";
                icono = "error";

                LogHelper.RegistrarError(ex, $"Error inesperado en UbicacionesReserva_D - EliminarUbicacionGeneral.");
            }

            return new Helper_E { Mensaje = mensajeUsuario, IconoSweetAlert = icono };
        }
    }
}
