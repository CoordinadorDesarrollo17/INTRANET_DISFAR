using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class UbicacionesLotes_D
    {
        readonly Utilitarios uti = new Utilitarios();
        readonly DBHelper db = new DBHelper();
        //Operacion desde transaccion ingreso en Kardex que suma la cantidad disponible  inserta un nuevo registro de ItemCode, Almacen, CodigoUbicacion y Lote.
        public Helper_E Ingreso(TransferenciaReserva_E ingreso, SqlConnection cn)
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

                    int idGenerado = 0;
                    foreach (var detalle in ingreso.Detalle)
                    {
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
                            throw new Exception("Error en Ingreso.");

                        }
                    }

                    mensaje = "Operacion de ingreso en UbicacionesLotes registrado correctamente";
                    icono = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotes_D - Ingreso");
                mensaje = "Ocurrió un error al registrar un ingreso en UbicacionesLotes. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en Ingreso.", ex);
            }

            return new Helper_E { Mensaje = mensaje, IconoSweetAlert = icono };
        }
        public Helper_E Egreso(TransferenciaReserva_E egreso, SqlConnection cn)
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

                    foreach (var detalle in egreso.Detalle)
                    {
                        cmd.Parameters.Clear();

                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "EGRESO");
                        cmd.Parameters.AddWithValue("@Almacen", "RESERVA");
                        cmd.Parameters.AddWithValue("@ItemCode", detalle.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemName", detalle.ItemName);
                        cmd.Parameters.AddWithValue("@CodigoUbicacion", detalle.CodigoUbicacion);
                        cmd.Parameters.AddWithValue("@BatchNum", detalle.BatchNum);
                        cmd.Parameters.AddWithValue("@QuantityUnidadesCajas", detalle.QuantityUnidadesCajas);

                        cmd.ExecuteNonQuery();
                    }

                    mensaje = "Operacion de egreso en UbicacionesLotes registrado correctamente";
                    icono = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotes_D - Egreso");
                mensaje = "Ocurrió un error al registrar un egreso en UbicacionesLotes. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en Egreso.", ex);
            }

            return new Helper_E { Mensaje = mensaje, IconoSweetAlert = icono };
        }
    }
}
