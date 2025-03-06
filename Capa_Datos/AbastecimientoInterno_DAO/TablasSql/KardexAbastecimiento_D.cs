using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Data.SqlClient;
using System.Data;
using Capa_Datos;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class KardexAbastecimiento_D
    {
        Utilitarios uti = new Utilitarios();
        DBHelper dbHelper = new DBHelper();

        public Helper_E InsertarTransaccionIngresoKardex(TransferenciaReserva_E ingreso, SqlConnection cn)
        {
            string mensaje, icono;

            try
            {
                // Verificar si la conexión está abierta
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoKardexAbastecimiento ", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    // Agrupar los detalles por ItemCode sumando QuantityUnidadesCajas
                    var detallesAgrupados = ingreso.Detalle
                        .GroupBy(d => new { d.ItemCode, d.ItemName })
                        .Select(g => new
                        {
                            ItemCode = g.Key.ItemCode,
                            ItemName = g.Key.ItemName,
                            TotalQuantityUnidadesCajas = g.Sum(d => d.QuantityUnidadesCajas) // Sumar valores nulos como 0
                        })
                        .ToList();

                    int idGenerado = 0;
                    foreach (var detalle in detallesAgrupados)
                    {
                        cmd.Parameters.Clear();

                        // Parámetros extraidos de la cabecera 
                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "INSERT");
                        cmd.Parameters.AddWithValue("@RucProveedor", ingreso.CardCode);
                        cmd.Parameters.AddWithValue("@NombreProveedor", ingreso.CardName);
                        cmd.Parameters.AddWithValue("@Sentido", "Ingreso");
                        cmd.Parameters.AddWithValue("@Tabla", "TransferenciaReserva");
                        cmd.Parameters.AddWithValue("@Referencia", ingreso.SolicitudTrasladoDocNum);

                        // Parámetros extraidos del detalle
                        cmd.Parameters.AddWithValue("@ItemCode", detalle.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemName", detalle.ItemName);
                        cmd.Parameters.AddWithValue("@Almacen", "RESERVA");
                        cmd.Parameters.AddWithValue("@Cantidad", detalle.TotalQuantityUnidadesCajas);
                        cmd.Parameters.AddWithValue("@Imputado", 0);
                        cmd.Parameters.AddWithValue("@Operario", ingreso.OperarioRegistra);
                        cmd.Parameters.AddWithValue("@TiempoRegistro", DateTime.Now);

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
                            mensaje = "Ocurrió un error al registrar el kardex de ingreso. Comuníquese con el área de Sistemas para más información.";
                            icono = "error";
                            throw new Exception("Error en InsertarTransaccionIngresoKardex dentro del recorrido");

                        }
                    }

                    mensaje = "Kardex de ingreso registrado correctamente";
                    icono = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "KardexAbastecimiento_D - InsertarTransaccionIngresoKardex");
                mensaje = "Ocurrió un error al registrar el kardex de ingreso. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en InsertarTransaccionIngresoKardex.", ex);
            }

            return new Helper_E
            {
                Mensajes = new List<string> { mensaje },
                IconoSweetAlert = icono
            };
        }
        //public Helper_E ValidarTransaccionImputadoKardex(Requerimientos_E imputado, SqlConnection cn)
        //{
        //    string mensaje, icono;

        //    try
        //    {
        //        // Verificar si la conexión está abierta
        //        if (cn.State != ConnectionState.Open)
        //        {
        //            cn.Open();
        //        }

        //        using (SqlCommand cmd = new SqlCommand("sp_MantenimientoUbicacionesLotesMaster", cn))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            foreach (var detalle in imputado.Detalle)
        //            {
        //                cmd.Parameters.Clear();

        //                cmd.Parameters.AddWithValue("@TipoMantenimiento", "VALIDAR");
        //                cmd.Parameters.AddWithValue("@ItemCode", detalle.ItemCode);
        //                cmd.Parameters.AddWithValue("@ItemName", detalle.ItemName);
        //                cmd.Parameters.AddWithValue("@Almacen", "RESERVA");
        //                cmd.Parameters.AddWithValue("@CodigoUbicacion", detalle.CodigoUbicacionOrigen);
        //                cmd.Parameters.AddWithValue("@QuantityMaster", detalle.QuantityMaster);
        //                cmd.Parameters.AddWithValue("@QuantitySaldo", detalle.QuantitySaldo);
        //                cmd.Parameters.AddWithValue("@UmAlm", detalle.UmAlm);
        //                cmd.Parameters.AddWithValue("@ValorUmAlm", detalle.ValorUmAlm);

        //                cmd.ExecuteNonQuery();
        //            }

        //            mensaje = "Kardex de imputado validado correctamente";
        //            icono = "success";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.RegistrarError(ex, "KardexAbastecimiento_D - ValidarTransaccionImputadoKardex");
        //        mensaje = "Ocurrió un error al validar cantidades de imputado para el kardex. Comuníquese con el área de Sistemas para más información.";
        //        icono = "error";
        //        throw new Exception("Error en ValidarTransaccionImputadoKardex.", ex);
        //    }

        //    return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        //}
        public Helper_E InsertarTransaccionImputadoKardex(Requerimientos_E imputado, SqlConnection cn)
        {
            string mensaje, icono;

            try
            {
                // Verificar si la conexión está abierta
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoKardexAbastecimiento", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Agrupar los detalles por ItemCode sumando QuantityUnidadesCajas
                    var detallesAgrupados = imputado.Detalle
                        .GroupBy(d => new { d.ItemCode, d.ItemName })
                        .Select(g => new
                        {
                            ItemCode = g.Key.ItemCode,
                            ItemName = g.Key.ItemName,
                            TotalQuantityUnidadesCajas = g.Sum(d => d.QuantityUnidadesCajas ?? 0) // Sumar valores nulos como 0
                        })
                        .ToList();

                    int idGenerado = 0;
                    foreach (var detalle in detallesAgrupados)
                    {
                        cmd.Parameters.Clear();

                        // Parámetros de la cabecera
                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "INSERT");
                        cmd.Parameters.AddWithValue("@RucProveedor", "20600546041");
                        cmd.Parameters.AddWithValue("@NombreProveedor", "COBEFAR S.A.C");
                        cmd.Parameters.AddWithValue("@Sentido", "Asignación");
                        cmd.Parameters.AddWithValue("@Tabla", "Requerimientos");
                        cmd.Parameters.AddWithValue("@Referencia", imputado.Id);

                        // Parámetros del detalle agrupado
                        cmd.Parameters.AddWithValue("@ItemCode", detalle.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemName", detalle.ItemName);
                        cmd.Parameters.AddWithValue("@Almacen", "RESERVA");
                        cmd.Parameters.AddWithValue("@Cantidad", 0);
                        cmd.Parameters.AddWithValue("@Imputado", detalle.TotalQuantityUnidadesCajas * -1); // Aplicar negativo
                        cmd.Parameters.AddWithValue("@Operario", imputado.OperarioRegistra);
                        cmd.Parameters.AddWithValue("@TiempoRegistro", DateTime.Now);

                        SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(idGeneradoParam);

                        cmd.ExecuteNonQuery();

                        // Obtener el ID generado y verificar si es válido
                        idGenerado = idGeneradoParam.Value != DBNull.Value ? (int)idGeneradoParam.Value : 0;

                        if (idGenerado <= 0)
                        {
                            mensaje = "Ocurrió un error al registrar el kardex de imputado. Comuníquese con el área de Sistemas para más información.";
                            icono = "error";
                            throw new Exception("Error en InsertarTransaccionImputadoKardex dentro del recorrido");
                        }
                    }

                    mensaje = "Kardex de imputado registrado correctamente";
                    icono = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "KardexAbastecimiento_D - InsertarTransaccionImputadoKardex");
                mensaje = "Ocurrió un error al registrar el kardex de imputado. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en InsertarTransaccionImputadoKardex.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }
        public Helper_E InsertarTransaccionSalidaKardex(string itemCode, string itemName, int cantidad, string operarioRegistra, int requerimientoId, SqlConnection cn)
        {
            string mensaje, icono;

            try
            {
                // Verificar si la conexión está abierta
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoKardexAbastecimiento ", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    int idGenerado = 0;

                    // Parámetros extraidos de la cabecera 
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "INSERT");
                    cmd.Parameters.AddWithValue("@RucProveedor", "20600546041");
                    cmd.Parameters.AddWithValue("@NombreProveedor", "COBEFAR S.A.C");
                    cmd.Parameters.AddWithValue("@Sentido", "Salida");
                    cmd.Parameters.AddWithValue("@Tabla", "Requerimientos");
                    cmd.Parameters.AddWithValue("@Referencia", requerimientoId);

                    // Parámetros extraidos del detalle
                    cmd.Parameters.AddWithValue("@ItemCode", itemCode);
                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                    cmd.Parameters.AddWithValue("@Almacen", "RESERVA");
                    cmd.Parameters.AddWithValue("@Cantidad",0 );
                    cmd.Parameters.AddWithValue("@Imputado", cantidad);
                    cmd.Parameters.AddWithValue("@Operario", operarioRegistra);
                    cmd.Parameters.AddWithValue("@TiempoRegistro", DateTime.Now);

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
                        mensaje = "Ocurrió un error al registrar el kardex de salida. Comuníquese con el área de Sistemas para más información.";
                        icono = "error";
                        throw new Exception("Error en InsertarTransaccionSalidaKardex dentro del recorrido");

                    }
                }

                mensaje = "Kardex de salida registrado correctamente";
                icono = "success";
            }

            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "KardexAbastecimiento_D - InsertarTransaccionSalidaKardex");
                mensaje = "Ocurrió un error al registrar el kardex de salida. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en InsertarTransaccionSalidaKardex.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }
        public Helper_E EliminarTotalTransaccionesIngresoKardex(int docNum, SqlConnection cn)
        {
            string mensaje, icono;

            try
            {
                // Verificar si la conexión está abierta
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoKardexAbastecimiento ", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros extraidos de la cabecera 
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "DELETE");
                    cmd.Parameters.AddWithValue("@Tabla", "TransferenciaReserva");
                    cmd.Parameters.AddWithValue("@Referencia", docNum);
                    SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idGeneradoParam);
                    cmd.ExecuteNonQuery();

                    mensaje = "Kardex de ingreso eliminado correctamente";
                    icono = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "KardexAbastecimiento_D - EliminarTransaccionIngresoKardex");
                mensaje = "Ocurrió un error al eliminar el kardex de ingreso. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en EliminarTransaccionIngresoKardex.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }
        public Helper_E EliminarPorItemCodeTransaccionIngresoKardex(int docNum, string itemCode, SqlConnection cn)
        {
            string mensaje, icono;

            try
            {
                // Verificar si la conexión está abierta
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoKardexAbastecimiento ", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros extraidos de la cabecera 
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "REVERT");
                    cmd.Parameters.AddWithValue("@Tabla", "TransferenciaReserva");
                    cmd.Parameters.AddWithValue("@Referencia", docNum);
                    cmd.Parameters.AddWithValue("@ItemCode", itemCode);
                    SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idGeneradoParam);

                    cmd.ExecuteNonQuery();

                    mensaje = "Kardex de ingreso por sku eliminado correctamente";
                    icono = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "KardexAbastecimiento_D - EliminarPorItemCodeTransaccionIngresoKardex");
                mensaje = "Ocurrió un error al eliminar el kardex de ingreso por sku. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en EliminarPorItemCodeTransaccionIngresoKardex.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }
    }
}
