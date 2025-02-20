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

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class KardexAbastecimiento_D
    {
        Utilitarios uti = new Utilitarios();
        DBHelper dbHelper = new DBHelper();

        public Helper_E InsertarTransaccionIngresoKardex(TransferenciaReserva_E ingreso, SqlConnection cn)
        {
            string mensaje, icono;
            var cardCode = ingreso.CardCode;
            var cardName = ingreso.CardName;
            var operario = ingreso.OperarioRegistra;

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
                    foreach (var detalle in ingreso.Detalle)
                    {
                        cmd.Parameters.Clear(); 
                        
                        // Parámetros extraidos de la cabecera 
                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "INGRESO");
                        cmd.Parameters.AddWithValue("@RucProveedor", cardCode);
                        cmd.Parameters.AddWithValue("@NombreProveedor", cardName);
                        cmd.Parameters.AddWithValue("@Sentido", "Ingreso");
                        cmd.Parameters.AddWithValue("@Tabla", "TransferenciaReserva");
                        cmd.Parameters.AddWithValue("@Referencia", ingreso.SolicitudTrasladoDocNum);

                        // Parámetros extraidos del detalle
                        cmd.Parameters.AddWithValue("@ItemCode", detalle.ItemCode);
                        cmd.Parameters.AddWithValue("@ItemName", detalle.ItemName);
                        cmd.Parameters.AddWithValue("@Almacen", "RESERVA");
                        cmd.Parameters.AddWithValue("@Cantidad", detalle.QuantityUnidadesCajas);
                        cmd.Parameters.AddWithValue("@Imputado", 0);
                        cmd.Parameters.AddWithValue("@Operario", operario);
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

            return new Helper_E { Mensaje = mensaje, IconoSweetAlert = icono };
        }


    }
}
