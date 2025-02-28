using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class UbicacionesLotesMaster_D
    {
        readonly Utilitarios uti = new Utilitarios();
        readonly DBHelper db = new DBHelper();
        //Operacion desde transaccion ingreso en Kardex que suma la cantidad disponible  inserta un nuevo registro de ItemCode, Almacen, CodigoUbicacion, Lote y UmAlm.

        public Helper_E Ingreso(int ubicacionLoteId, DetalleTransferenciaReserva_E detalle, SqlConnection cn)
        {
            string mensaje, icono;

            try
            {
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoUbicacionesLotesMaster", cn))
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
                        cmd.Parameters.AddWithValue("@UmAlm", detalle.UmAlm);
                        cmd.Parameters.AddWithValue("@ValorUmAlm", detalle.ValorUmAlm);
                        cmd.Parameters.AddWithValue("@QuantityMaster", detalle.QuantityMaster);
                        cmd.Parameters.AddWithValue("@QuantitySaldo", detalle.QuantitySaldo);
                        cmd.Parameters.AddWithValue("@UbicacionLoteId", ubicacionLoteId);
                        //cmd.Parameters.AddWithValue("@QuantityUnidadesCajas", detalle.QuantityUnidadesCajas); // No es necesario enviarlas ya que se hace calculo en el procedure.

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
                            mensaje = "Ocurrió un error al registrar un ingreso en UbicacionesLotesMaster. Comuníquese con el área de Sistemas para más información.";
                            icono = "error";
                            throw new Exception("Error en Ingreso.");

                        }
                    

                    mensaje = "Operacion de ingreso en UbicacionesLotesMaster registrado correctamente";
                    icono = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotesMaster_D - Ingreso");
                mensaje = "Ocurrió un error al registrar un ingreso en UbicacionesLotesMaster. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en Ingreso.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
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

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoUbicacionesLotesMaster", cn))
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
                        cmd.Parameters.AddWithValue("@UmAlm", detalle.UmAlm);
                        cmd.Parameters.AddWithValue("@ValorUmAlm", detalle.ValorUmAlm);
                        cmd.Parameters.AddWithValue("@QuantityMaster", detalle.QuantityMaster);
                        cmd.Parameters.AddWithValue("@QuantitySaldo", detalle.QuantitySaldo);
                        //cmd.Parameters.AddWithValue("@QuantityUnidadesCajas", detalle.QuantityUnidadesCajas); // No es necesario enviarlas ya que se hace calculo en el procedure.
                        cmd.ExecuteNonQuery();
                    }
                    mensaje = "Operacion de revertir ingreso en UbicacionesLotesMaster realizado correctamente";
                    icono = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotesMaster_D - RevertirIngreso");
                mensaje = "Ocurrió un error al registrar un revertir ingreso en UbicacionesLotesMaster. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en Revertir ingreso.", ex);
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

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoUbicacionesLotesMaster", cn))
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
                        cmd.Parameters.AddWithValue("@UmAlm", detalle.UmAlm);
                        cmd.Parameters.AddWithValue("@ValorUmAlm", detalle.ValorUmAlm);
                        cmd.Parameters.AddWithValue("@QuantityMaster", detalle.QuantityMaster);
                        cmd.Parameters.AddWithValue("@QuantitySaldo", detalle.QuantitySaldo);
                        //cmd.Parameters.AddWithValue("@QuantityUnidadesCajas", detalle.QuantityUnidadesCajas); // No es necesario enviarlas ya que se hace calculo en el procedure.
                        cmd.ExecuteNonQuery();
                    }
                    mensaje = "Operacion de salida en UbicacionesLotesMaster realizado correctamente";
                    icono = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotesMaster_D - Salida");
                mensaje = "Ocurrió un error al registrar una salida en UbicacionesLotesMaster. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en Salida.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }
        public List<string> BuscarUnidadAlm(string condicion, Dictionary<string, object> parametrosCondicion)
        {
            
            List<string> lista = new List<string>();
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT [UmAlm]");
                    sb.AppendLine("FROM [dbo].[UbicacionesLotesMaster] WHERE 1=1");
                    sb.AppendLine(condicion);

                    // Agregamos los parámetros dinámicamente
                    foreach (var param in parametrosCondicion)
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
                                var unidadAlm = string.Empty;
                                if (!dr.IsDBNull(0)) unidadAlm = dr.GetString(0);
                                if (!string.IsNullOrEmpty(unidadAlm)) { lista.Add(unidadAlm); }
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotesMaster_D - BuscarUnidadAlm");
            }

            return lista;
        }

        public List<UbicacionesLotesMaster_E> BuscarArticulos(string condicion, Dictionary<string, object> parametrosCondicion)
        {
            List<UbicacionesLotesMaster_E> lista = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine("SELECT ULM.[Id], ULM.[ItemCode], ULM.[ItemName], ULM.[CodigoUbicacion], ULM.[BatchNum], ULM.[UmAlm], ULM.[ValorUmAlm], ULM.[QuantityMaster], ULM.[QuantitySaldo], ULM.[QuantityUnidadesCajas],");
                    sb.AppendLine("CONVERT(varchar, LRS.[InDate], 103), CONVERT(varchar, LRS.[ExpDate], 103)");
                    sb.AppendLine("FROM [dbo].[UbicacionesLotesMaster] ULM");
                    sb.AppendLine("INNER JOIN [dbo].[LotesRegistroSanitario] LRS ON ULM.ItemCode = LRS.ItemCode AND ULM.BatchNum = LRS.DistNumber");
                    sb.AppendLine("WHERE ULM.[Almacen] = @Almacen");
                    sb.AppendLine(condicion);

                    cmd.Parameters.AddWithValue("@Almacen", "RESERVA");
                    // Agregamos los parámetros dinámicamente
                    foreach (var param in parametrosCondicion)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    cmd.CommandText = sb.ToString();

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            lista = new List<UbicacionesLotesMaster_E>();

                            while (dr.Read())
                            {
                                var obj = new UbicacionesLotesMaster_E();

                                if (!dr.IsDBNull(0)) obj.Id = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) obj.ItemCode = dr.GetString(1);
                                if (!dr.IsDBNull(2)) obj.ItemName = dr.GetString(2);
                                if (!dr.IsDBNull(3)) obj.CodigoUbicacion = dr.GetString(3);
                                if (!dr.IsDBNull(4)) obj.BatchNum = dr.GetString(4);
                                if (!dr.IsDBNull(5)) obj.UmAlm = dr.GetString(5);
                                if (!dr.IsDBNull(6)) obj.ValorUmAlm = dr.GetInt32(6);
                                if (!dr.IsDBNull(7)) obj.QuantityMaster = dr.GetInt32(7);
                                if (!dr.IsDBNull(8)) obj.QuantitySaldo = dr.GetInt32(8);
                                if (!dr.IsDBNull(9)) obj.QuantityUnidadesCajas = dr.GetInt32(9);
                                if (!dr.IsDBNull(10)) obj.InDate = dr.GetString(10);
                                if (!dr.IsDBNull(11)) obj.ExpDate = dr.GetString(11);

                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "UbicacionesLotesMaster_D - BuscarArticulos");
            }

            return lista;
        }
    }
}
