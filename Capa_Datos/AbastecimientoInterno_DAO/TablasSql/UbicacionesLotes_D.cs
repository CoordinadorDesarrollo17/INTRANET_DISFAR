using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class UbicacionesLotes_D
    {
        readonly Utilitarios uti = new Utilitarios();
        readonly DBHelper db = new DBHelper();
        //Operacion desde transaccion ingreso en Kardex que suma la cantidad disponible  inserta un nuevo registro de ItemCode, Almacen, CodigoUbicacion y Lote.
        public List<UbicacionesLotes_E> Obtener(string itemCode, string batchNum)
        {
            try
            {
                string query = @"SELECT Id, UbicacionId, Almacen, ItemCode, ItemName, CodigoUbicacion, BatchNum, QuantityUnidadesCajas 
                         FROM UbicacionesLotes 
                         WHERE ItemCode = @ItemCode";

                if (!string.IsNullOrEmpty(batchNum))
                {
                    query += " AND BatchNum = @BatchNum";
                }

                List<UbicacionesLotes_E> resultado = null;

                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@ItemCode", itemCode);

                        if (!string.IsNullOrEmpty(batchNum))
                        {
                            cmd.Parameters.AddWithValue("@BatchNum", batchNum);
                        }
                        
                        cn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            resultado = new List<UbicacionesLotes_E>();

                            while (reader.Read())
                            {
                                var ubicacionLote = new UbicacionesLotes_E();

                                if (!reader.IsDBNull(0)) ubicacionLote.Id = reader.GetInt32(0);
                                if (!reader.IsDBNull(1)) ubicacionLote.UbicacionId = reader.GetInt32(1);
                                if (!reader.IsDBNull(2)) ubicacionLote.Almacen = reader.GetString(2);
                                if (!reader.IsDBNull(3)) ubicacionLote.ItemCode = reader.GetString(3);
                                if (!reader.IsDBNull(4)) ubicacionLote.ItemName = reader.GetString(4);
                                if (!reader.IsDBNull(5)) ubicacionLote.CodigoUbicacion = reader.GetString(5);
                                if (!reader.IsDBNull(6)) ubicacionLote.BatchNum = reader.GetString(6);
                                if (!reader.IsDBNull(7)) ubicacionLote.QuantityUnidadesCajas = reader.GetInt32(7);

                                resultado.Add(ubicacionLote);
                            }
                        }
                    }
                }

                return resultado ?? new List<UbicacionesLotes_E>();
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotes_D - Obtener");
                throw new Exception("Error al obtener la ubicación del lote.", ex);
            }
        }
        public Helper_E Ingreso(DetalleTransferenciaReserva_E detalle, SqlConnection cn)
        {
            string mensaje = "", icono;
            int id = 0;

            try
            {
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoUbicacionesLotes", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    int idGenerado = 0;

                    cmd.Parameters.Clear();

                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "INGRESO");
                    cmd.Parameters.AddWithValue("@Almacen", "RESERVA");
                    cmd.Parameters.AddWithValue("@ItemCode", detalle.ItemCode);
                    cmd.Parameters.AddWithValue("@ItemName", detalle.ItemName);
                    cmd.Parameters.AddWithValue("@CodigoUbicacion", detalle.CodigoUbicacion);
                    cmd.Parameters.AddWithValue("@BatchNum", detalle.BatchNum);
                    cmd.Parameters.AddWithValue("@QuantityUnidadesCajas", detalle.QuantityUnidadesCajas);

                    // Parámetro de salida
                    SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idGeneradoParam);

                    cmd.ExecuteNonQuery();

                    // Obtener el ID generado y verificar si es válido.
                    idGenerado = idGeneradoParam.Value != DBNull.Value ? (int)idGeneradoParam.Value : 0;

                    if (idGenerado <= 0)
                    {
                        mensaje = "Ocurrió un error al registrar un ingreso en UbicacionesLotes. Comuníquese con el área de Sistemas para más información.";
                        icono = "error";
                        return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono, Id = id };
                    }

                    id = idGenerado;
                    icono = "success";
                }
            }
            catch (SqlException sqlEx)
            {
                LogHelper.RegistrarError(sqlEx, "UbicacionesLotes_D - Ingreso");

                if (sqlEx.Message.Contains("Ubicación para el producto"))
                {
                    mensaje = $"{sqlEx.Message} Debe crear la ubicación antes de registrar el stock.";
                    icono = "error";
                }
                else
                {
                    mensaje = $"Error en base de datos: {sqlEx.Message}";
                    icono = "error";
                }
                return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono, Id = id };
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotes_D - Ingreso");
                mensaje = "Ocurrió un error al registrar un ingreso en UbicacionesLotes. Comuníquese con el área de Sistemas para más información.";
                icono = "error";

                return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono, Id = id };
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono, Id = id };
        }

        public Helper_E RevertirIngreso(TransferenciaReserva_E ingreso, SqlConnection cn)
        {
            string mensaje, icono;

            try
            {
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoUbicacionesLotes", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (var detalle in ingreso.Detalle)
                    {
                        cmd.Parameters.Clear();

                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "REVERTIR_INGRESO");
                        cmd.Parameters.AddWithValue("@Almacen", "RESERVA");
                        cmd.Parameters.AddWithValue("@ItemCode", detalle.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemName", detalle.ItemName);
                        cmd.Parameters.AddWithValue("@CodigoUbicacion", detalle.CodigoUbicacion);
                        cmd.Parameters.AddWithValue("@BatchNum", detalle.BatchNum);
                        cmd.Parameters.AddWithValue("@QuantityUnidadesCajas", detalle.QuantityUnidadesCajas);
                        SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(idGeneradoParam);
                        cmd.ExecuteNonQuery();
                    }

                    mensaje = "Operacion de egreso en UbicacionesLotes registrado correctamente";
                    icono = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotes_D - RevertirIngreso");
                mensaje = "Ocurrió un error al revertir ingreso en UbicacionesLotes. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en RevertirIngreso.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }
        public Helper_E Salida(List<DetalleRequerimientos_E> salida, SqlConnection cn)
        {
            string mensaje, icono;

            try
            {
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoUbicacionesLotes", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (var detalle in salida)
                    {
                        cmd.Parameters.Clear();

                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "SALIDA");
                        cmd.Parameters.AddWithValue("@Almacen", "RESERVA");
                        cmd.Parameters.AddWithValue("@ItemCode", detalle.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemName", detalle.ItemName);
                        cmd.Parameters.AddWithValue("@CodigoUbicacion", detalle.CodigoUbicacionOrigen);
                        cmd.Parameters.AddWithValue("@BatchNum", detalle.BatchNum);
                        cmd.Parameters.AddWithValue("@QuantityUnidadesCajas", detalle.QuantityUnidadesCajas);
                        SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(idGeneradoParam);
                        cmd.ExecuteNonQuery();
                    }

                    mensaje = "Operacion de salida en UbicacionesLotes registrado correctamente";
                    icono = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotes_D - Salida");
                mensaje = "Ocurrió un error al registrar un salida en UbicacionesLotes. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en Salida.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }
    }
}
