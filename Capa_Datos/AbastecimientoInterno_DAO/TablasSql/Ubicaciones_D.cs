using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using DocumentFormat.OpenXml.Presentation;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class Ubicaciones_D
    {
        readonly Utilitarios uti = new Utilitarios();
        public Helper_E RegistrarUbicacion(Ubicaciones_E datos)
        {
            string mensaje, icono;
            int id = 0;

            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                cn.Open();
                SqlTransaction transaction = cn.BeginTransaction();

                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_AdministrarUbicaciones", cn, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Operacion", "INSERT");
                        cmd.Parameters.AddWithValue("@Almacen", datos.Almacen);
                        cmd.Parameters.AddWithValue("@CodigoUbicacion", datos.CodigoUbicacion);

                        // Agregar parámetro de salida para Id
                        SqlParameter outputId = new SqlParameter("@Id", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputId);

                        cmd.ExecuteNonQuery();
                        id = (int)outputId.Value;
                    }

                    transaction.Commit();
                    mensaje = "Ubicación registrada correctamente.";
                    icono = "success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LogHelper.RegistrarError(ex, "Ubicaciones_D - RegistrarUbicacion");
                    mensaje = "Ocurrió un error al registrar la ubicación. Por favor, comuníquese con el área de Sistemas para más información.";
                    icono = "error";
                }
            }

            return new Helper_E { Id = id, Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }
        public bool BuscarUbicacion(string almacen, (string elemento1, string elemento2) ubicacion)
        {
            string[] ubicaciones = ListarTotalUbicacionesEnArray(almacen, ubicacion.elemento2);
            return ubicaciones.Contains(ubicacion.elemento1);
        }
        public string[] ListarTotalUbicacionesEnArray(string almacen, string itemCode)
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

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ubicaciones.Add(reader.GetString(0)); // Leer cada ubicación en la primera columna
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                LogHelper.RegistrarError(sqlEx, "Error SQL en Ubicaciones_D - BuscarUbicaciones.");
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error inesperado en Ubicaciones_D - BuscarUbicaciones.");
            }

            return ubicaciones.ToArray(); // Retornar el array de ubicaciones
        }
        public List<Ubicaciones_E> ListarUbicaciones(string condicion, Dictionary<string, object> parametros)
        {
            List<Ubicaciones_E> lista = new List<Ubicaciones_E>();

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT UP.Id, UP.Almacen, UP.ItemCode, UP.ItemName, UP.CodigoUbicacion, SM.StockMinAbastecimiento, SM.Clasificacion");
                    sb.AppendLine("FROM Ubicaciones UP");
                    sb.AppendLine("WHERE 1=1");
                    sb.AppendLine(condicion);

                    // Agregamos los parámetros dinámicamente
                    foreach (var param in parametros)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    cmd.CommandText = sb.ToString();

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var obj = new Ubicaciones_E();

                                if (!dr.IsDBNull(0)) obj.Id = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) obj.Almacen = dr.GetString(1);
                                if (!dr.IsDBNull(2)) obj.ItemCode = dr.GetString(2);
                                if (!dr.IsDBNull(3)) obj.ItemName = dr.GetString(3);
                                if (!dr.IsDBNull(4)) obj.CodigoUbicacion = dr.GetString(4);
                                if (!dr.IsDBNull(5)) obj.StockMinAbastecimiento = dr.GetInt32(5);
                                if (!dr.IsDBNull(6)) obj.StockMinVenta = dr.GetInt32(6);
                                if (!dr.IsDBNull(7)) obj.Clasificacion = dr.GetString(7);

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesPicking_D - ListarUbicacionesPicking");
            }

            return lista;
        }
        public Helper_E EliminarUbicacion(int ubicacionId)
        {
            string mensajeUsuario, icono;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_AdministrarUbicaciones", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Operacion", "DELETE");
                        cmd.Parameters.AddWithValue("@Id", ubicacionId);
                        cmd.ExecuteNonQuery();
                    }
                }
                    mensajeUsuario = "Ubicación eliminada correctamente.";
                    icono = "success";
                
            }
            catch (SqlException sqlEx)
            {
                mensajeUsuario = (sqlEx.Number == 50000) ? sqlEx.Message : "No se pudo eliminar la ubicación. Intente nuevamente.";
                icono = "error";

                LogHelper.RegistrarError(sqlEx, $"Error SQL en UbicacionesReserva_D - EliminarUbicacionReserva.");
            }
            catch (Exception ex)
            {
                mensajeUsuario = "Ocurrió un problema inesperado. Por favor, comunicarse con SISTEMAS.";
                icono = "error";

                LogHelper.RegistrarError(ex, $"Error inesperado en UbicacionesReserva_D - EliminarUbicacionReserva.");
            }

            return new Helper_E { Mensajes = new List<string> { mensajeUsuario }, IconoSweetAlert = icono };
        }
        public Helper_E EliminarUbicacionGeneral(string codigoUbicacion)
        {
            string mensajeUsuario, icono;

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
                        cmd.ExecuteNonQuery();

                    }
                }
                mensajeUsuario = "Ubicación eliminada correctamente.";
                icono = "success";
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

            return new Helper_E { Mensajes = new List<string> { mensajeUsuario }, IconoSweetAlert = icono };
        }
    }
}
