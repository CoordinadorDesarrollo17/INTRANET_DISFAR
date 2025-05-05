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

        public List<UbicacionesLotes_E> ListarUbicaciones(string condicion, Dictionary<string, object> parametros, SqlConnection cn)
        {
            List<UbicacionesLotes_E> lista = new List<UbicacionesLotes_E>();

            try
            {
                if (cn == null)
                {
                    cn = new SqlConnection(uti.cadSql2);
                }

                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }


                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;

                var sb = new StringBuilder();

                sb.AppendLine("SELECT ULM.[Id], ULM.[UbicacionId], ULM.[Almacen], ULM.[ItemCode], ULM.[ItemName], ULM.[CodigoUbicacion], ULM.[BatchNum], ULM.[QuantityUnidadesCajas]");
                sb.AppendLine("FROM UbicacionesLotes ULM");
                sb.AppendLine("WHERE 1=1");
                sb.AppendLine(condicion);

                // Agregamos los parámetros dinámicamente
                foreach (var param in parametros)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }

                cmd.CommandText = sb.ToString();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            var obj = new UbicacionesLotes_E();

                            if (!dr.IsDBNull(0)) obj.Id = dr.GetInt32(0);
                            if (!dr.IsDBNull(1)) obj.UbicacionId = dr.GetInt32(1);
                            if (!dr.IsDBNull(2)) obj.Almacen = dr.GetString(2);
                            if (!dr.IsDBNull(3)) obj.ItemCode = dr.GetString(3);
                            if (!dr.IsDBNull(4)) obj.ItemName = dr.GetString(4);
                            if (!dr.IsDBNull(5)) obj.CodigoUbicacion = dr.GetString(5);
                            if (!dr.IsDBNull(6)) obj.BatchNum = dr.GetString(6);
                            if (!dr.IsDBNull(7)) obj.QuantityUnidadesCajas = dr.GetInt32(7);

                            lista.Add(obj);
                        }
                    }
                }
                cn.Close();

            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotesMaster_D - ListarUbicaciones");
            }

            return lista;
        }

        //Operacion desde transaccion ingreso en Kardex que suma la cantidad disponible  inserta un nuevo registro de ItemCode, Almacen, CodigoUbicacion y Lote.
        public List<UbicacionesLotes_E> Obtener(string itemCode, string batchNum)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    string query = @"SELECT Id, UbicacionId, Almacen, ItemCode, ItemName, CodigoUbicacion, BatchNum, QuantityUnidadesCajas 
                         FROM UbicacionesLotes 
                         WHERE ItemCode = @ItemCode";

                    if (!string.IsNullOrEmpty(batchNum))
                    {
                        query += " AND BatchNum = @BatchNum";
                    }

                    List<UbicacionesLotes_E> resultado = null;
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@ItemCode", itemCode);

                        if (!string.IsNullOrEmpty(batchNum))
                        {
                            cmd.Parameters.AddWithValue("@BatchNum", batchNum);
                        }

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
                    return resultado ?? new List<UbicacionesLotes_E>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotes_D - Obtener");
                throw new Exception("Error al obtener la ubicación del lote.", ex);
            }
        }

        public UbicacionesLotes_E ObtenerPorId(int id)
        {
            UbicacionesLotes_E resultado = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    string query = @"SELECT Id, UbicacionId, Almacen, ItemCode, ItemName, CodigoUbicacion, BatchNum, QuantityUnidadesCajas FROM UbicacionesLotes WHERE Id = @Id";

                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            resultado = new UbicacionesLotes_E();

                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0)) resultado.Id = reader.GetInt32(0);
                                if (!reader.IsDBNull(1)) resultado.UbicacionId = reader.GetInt32(1);
                                if (!reader.IsDBNull(2)) resultado.Almacen = reader.GetString(2);
                                if (!reader.IsDBNull(3)) resultado.ItemCode = reader.GetString(3);
                                if (!reader.IsDBNull(4)) resultado.ItemName = reader.GetString(4);
                                if (!reader.IsDBNull(5)) resultado.CodigoUbicacion = reader.GetString(5);
                                if (!reader.IsDBNull(6)) resultado.BatchNum = reader.GetString(6);
                                if (!reader.IsDBNull(7)) resultado.QuantityUnidadesCajas = reader.GetInt32(7);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotes_D - Obtener");
                throw new Exception("Error al obtener la ubicación del lote.", ex);
            }

            return resultado ?? new UbicacionesLotes_E();
        }

        public Helper_E Ingreso(DetalleTransferenciaReserva_E detalle, SqlConnection cn)
        {
            string mensaje, icono;
            int id = 0;

            if (cn == null)
            {
                cn = new SqlConnection(uti.cadSql2);
            }

            if (cn.State != ConnectionState.Open)
            {
                cn.Open();
            }

            try
            {
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
                        throw new Exception("Error en Ingreso.");
                    }

                    id = idGenerado;
                    mensaje = "Operacion de ingreso en UbicacionesLotes registrado correctamente";
                    icono = "success";
                }
            }
            catch (SqlException sqlEx)
            {
                LogHelper.RegistrarError(sqlEx, "UbicacionesLotes_D - Ingreso");

                if (sqlEx.Message.Contains("no existe en almacén Reserva."))
                {
                    mensaje = $"{sqlEx.Message} Debe crear la ubicación antes de registrar el stock.";
                    icono = "error";
                }
                else
                {
                    mensaje = $"Error en base de datos: {sqlEx.Message}";
                    icono = "error";
                }
                return new Helper_E { Mensajes = new List<string> { mensaje }, Icono = icono, Id = id };
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotes_D - Ingreso");
                mensaje = "Ocurrió un error al registrar un ingreso en UbicacionesLotes. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                return new Helper_E { Mensajes = new List<string> { mensaje }, Icono = icono, Id = id };
            }
            return new Helper_E { Mensajes = new List<string> { mensaje }, Icono = icono, Id = id };
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

            return new Helper_E { Mensajes = new List<string> { mensaje }, Icono = icono };
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
                mensaje = ex.Message;
                icono = "error";
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, Icono = icono };
        }

        public Helper_E EliminarArticulo(string itemCode, string codigoUbicacion)
        {
            string mensajeUsuario, icono;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_MantenimientoUbicacionesLotes", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "DELETE_ARTICULO");
                        cmd.Parameters.AddWithValue("@ItemCode", itemCode);
                        cmd.Parameters.AddWithValue("@CodigoUbicacion", codigoUbicacion);
                        SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(idGeneradoParam);

                        cmd.ExecuteNonQuery();

                    }
                }
                mensajeUsuario = "Ubicación eliminada correctamente.";
                icono = "success";
            }
            catch (SqlException sqlEx)
            {
                mensajeUsuario = (sqlEx.Number == 50000) ? sqlEx.Message : "No se pudo eliminar el artículo. Intente nuevamente.";
                icono = "error";

                LogHelper.RegistrarError(sqlEx, $"Error SQL en UbicacionesLotes_D - EliminarArticulo.");
            }
            catch (Exception ex)
            {
                mensajeUsuario = "Ocurrió un problema inesperado. Por favor, comunicarse con SISTEMAS.";
                icono = "error";

                LogHelper.RegistrarError(ex, $"Error inesperado en UbicacionesLotes_D - EliminarArticulo.");
            }

            return new Helper_E { Mensajes = new List<string> { mensajeUsuario }, Icono = icono };
        }

        public Helper_E RegistrarCodigoUbicacionPicking(List<DetalleRequerimientos_E> detalle, SqlConnection cn)
        {
            string mensajeUsuario, icono;

            try
            {
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoUbicacionesLotes", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "INS_CODUBI_PICKING");

                    if (detalle.Any())
                    {
                        DataTable detalleTable = new DataTable();
                        detalleTable.Columns.Add("Id", typeof(int));
                        detalleTable.Columns.Add("UbicacionId", typeof(int));
                        detalleTable.Columns.Add("Almacen", typeof(string));
                        detalleTable.Columns.Add("ItemCode", typeof(string));
                        detalleTable.Columns.Add("ItemName", typeof(string));
                        detalleTable.Columns.Add("CodigoUbicacion", typeof(string));
                        detalleTable.Columns.Add("BatchNum", typeof(string));
                        detalleTable.Columns.Add("QuantityUnidadesCajas", typeof(int));

                        foreach (var item in detalle)
                        {
                            detalleTable.Rows.Add(
                                0,
                                item.UbicacionId,
                                "PICKING",
                                item.ItemCode,
                                item.ItemName,
                                item.CodigoUbicacionDestino,
                                item.BatchNum,
                                (object)item.QuantityUnidadesCajas ?? DBNull.Value
                            );
                        }

                        SqlParameter detalleParam = new SqlParameter("@ListaCUP", SqlDbType.Structured)
                        {
                            TypeName = "dbo.CodigoUbicacionPickingType",
                            Value = detalleTable
                        };
                        cmd.Parameters.Add(detalleParam);
                    }


                    SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idGeneradoParam);

                    cmd.ExecuteNonQuery();

                }


                mensajeUsuario = "Ubicación registrada correctamente.";
                icono = "success";
            }
            catch (SqlException sqlEx)
            {
                mensajeUsuario = (sqlEx.Number == 50000) ? sqlEx.Message : "No se pudo registrar el código de ubicación. Intente nuevamente.";
                icono = "error";
                throw new Exception(mensajeUsuario, sqlEx);
                LogHelper.RegistrarError(sqlEx, $"Error SQL en UbicacionesLotes_D - RegistrarCodigoUbicacionPicking.");
            }
            catch (Exception ex)
            {
                mensajeUsuario = "Ocurrió un problema inesperado. Por favor, comunicarse con SISTEMAS.";
                icono = "error";
                throw new Exception(mensajeUsuario, ex);
                LogHelper.RegistrarError(ex, $"Error inesperado en UbicacionesLotes_D - RegistrarCodigoUbicacionPicking.");
            }

            return new Helper_E { Mensajes = new List<string> { mensajeUsuario }, Icono = icono };
        }

        public Helper_E CambiarUbicacionPicking(string nuevoCodigoUbicacion, UbicacionesLotes_E obj)
        {
            var result = new Helper_E { Titulo = "", Mensajes = new List<string>(), Icono = "" };
            // 2. nuevoCodigoUbicacion: si en caso no exista, se inserta (itemcode, itemname, nuevocodigoubicacion, batchnum)
            // 3. ubicacionLoteId: eliminar ubicación old
            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                cn.Open();
                using (SqlTransaction transaction = cn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_MantenimientoUbicacionesLotes", cn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@TipoMantenimiento", "CAMBIAR_UBI_PICKING");
                            cmd.Parameters.AddWithValue("@Id", obj.Id);
                            cmd.Parameters.AddWithValue("@UbicacionId", obj.UbicacionId);
                            cmd.Parameters.AddWithValue("@Almacen", "PICKING");
                            cmd.Parameters.AddWithValue("@ItemCode", obj.ItemCode);
                            cmd.Parameters.AddWithValue("@ItemName", obj.ItemName);
                            cmd.Parameters.AddWithValue("@CodigoUbicacion", obj.CodigoUbicacion);
                            cmd.Parameters.AddWithValue("@NuevoCodigoUbicacion", nuevoCodigoUbicacion);
                            cmd.Parameters.AddWithValue("@BatchNum", obj.BatchNum);

                            // Agregar parámetro de salida para Id
                            SqlParameter IdGenerado = new SqlParameter("@IdGenerado", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(IdGenerado);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        result.Titulo = "Acción completada";
                        result.Mensajes.Add("Código de ubicación cambiado correctamente.");
                        result.Icono = "success";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        LogHelper.RegistrarError(ex, "UbicacionesLotes_D - CambiarUbicacionPicking");

                        result.Titulo = "Error";
                        result.Mensajes.Add("Ocurrió un error al cambiar código de ubicación picking.");
                        result.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                        result.Icono = "error";
                    }
                }
            }

            return result;
        }
    }
}
