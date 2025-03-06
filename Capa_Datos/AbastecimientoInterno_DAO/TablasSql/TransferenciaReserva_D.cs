using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing.Diagrams;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class TransferenciaReserva_D
    {
        readonly Utilitarios uti = new Utilitarios();
        readonly DBHelper db = new DBHelper();

        public Helper_E RegistrarTransferenciaReserva(TransferenciaReserva_E transferencia, SqlConnection cn)
        {
            string mensaje, icono;
            if (cn.State != ConnectionState.Open)
            {
                cn.Open();
            }
                try
                {
                    using (var cmd = new SqlCommand("sp_MantenimientoTransferenciaReserva", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "INSERT");
                        cmd.Parameters.AddWithValue("@SolicitudTrasladoId", transferencia.SolicitudTrasladoId);
                        cmd.Parameters.AddWithValue("@SolicitudTrasladoDocNum", transferencia.SolicitudTrasladoDocNum);
                        cmd.Parameters.AddWithValue("@CardCode", transferencia.CardCode);
                        cmd.Parameters.AddWithValue("@CardName", transferencia.CardName);
                        cmd.Parameters.AddWithValue("@NroGuia", transferencia.NroGuia);
                        cmd.Parameters.AddWithValue("@OperarioRegistra", transferencia.OperarioRegistra);

                        SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(idGeneradoParam);

                        // Tabla de detalles
                        DataTable dtDetalle = ConvertirADatatable(transferencia.Detalle);
                        SqlParameter detallesParam = cmd.Parameters.AddWithValue("@Detalle", dtDetalle);
                        detallesParam.SqlDbType = SqlDbType.Structured;
                        detallesParam.TypeName = "dbo.DetalleTransferenciaReservaType";

                        cmd.ExecuteNonQuery();

                        int idGenerado = (int)idGeneradoParam.Value;
                        if (idGenerado > 0)
                        {
                            return new Helper_E { Mensajes = new List<string> { "Transferencia de reserva generada correctamente" }, IconoSweetAlert = "success", Id = idGenerado };
                        }
                        else
                        {
                            return new Helper_E { Mensajes = new List<string> { "No se registro transaferencia en la base de datos correctamente" }, IconoSweetAlert = "error" };
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.RegistrarError(ex, "TransferenciaReserva_D - RegistrarTransferenciaReserva");
                    mensaje = "Ocurrió un error al registrar la transferencia reserva. Comuníquese con el área de Sistemas para más información.";
                    icono = "error";
                    throw new Exception("Error en RegistrarTransferenciaReserva.", ex);
                }
           
        }
        private DataTable ConvertirADatatable(List<DetalleTransferenciaReserva_E> detalles)
        {
            DataTable table = new DataTable();
            table.Columns.Add("TransferenciaReservaId", typeof(int));
            table.Columns.Add("ItemCode", typeof(string));
            table.Columns.Add("ItemName", typeof(string));
            table.Columns.Add("BatchNum", typeof(string));
            table.Columns.Add("ExpDate", typeof(DateTime));
            table.Columns.Add("InDate", typeof(DateTime));
            table.Columns.Add("CodigoUbicacion", typeof(string));
            table.Columns.Add("UmAlm", typeof(string));
            table.Columns.Add("ValorUmAlm", typeof(int));
            table.Columns.Add("QuantityMaster", typeof(int));
            table.Columns.Add("QuantitySaldo", typeof(int));
            table.Columns.Add("QuantityUnidadesCajas", typeof(int));

            foreach (var detalle in detalles)
            {
                table.Rows.Add(0, detalle.ItemCode, detalle.ItemName, detalle.BatchNum, detalle.ExpDate, detalle.InDate,
                               detalle.CodigoUbicacion, detalle.UmAlm, detalle.ValorUmAlm,
                               detalle.QuantityMaster, detalle.QuantitySaldo, detalle.QuantityUnidadesCajas);
            }
            return table;
        }
        public TransferenciaReserva_E ObtenerTransferenciaReserva(int docNum,SqlConnection cn)
        {
            TransferenciaReserva_E transferencia = null;
            try
            {
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoTransferenciaReserva", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "GET");
                    cmd.Parameters.AddWithValue("@SolicitudTrasladoDocNum", docNum);
                    var outputId = new SqlParameter("@IdGenerado", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputId);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            transferencia = new TransferenciaReserva_E();
                            if (!dr.IsDBNull(0)) transferencia.Id = dr.GetInt32(0);
                            if (!dr.IsDBNull(1)) transferencia.SolicitudTrasladoId = dr.GetInt32(1);
                            if (!dr.IsDBNull(2)) transferencia.SolicitudTrasladoDocNum = dr.GetInt32(2);
                            if (!dr.IsDBNull(3)) transferencia.DocDate = dr.GetDateTime(3).ToString("yyyy-MM-dd");
                            if (!dr.IsDBNull(4)) transferencia.CardCode = dr.GetString(4);
                            if (!dr.IsDBNull(5)) transferencia.CardName = dr.GetString(5);
                            if (!dr.IsDBNull(6)) transferencia.NroGuia = dr.GetString(6);
                            if (!dr.IsDBNull(7)) transferencia.OperarioRegistra = dr.GetString(7);
                            transferencia.Detalle = new List<DetalleTransferenciaReserva_E>();
                        }

                        // Leer DetalleTransferenciaReserva
                        if (dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                var detalle = new DetalleTransferenciaReserva_E();

                                if (!dr.IsDBNull(0)) detalle.Id = dr.GetInt32(0);
                                if (!dr.IsDBNull(1)) detalle.TransferenciaReservaId = dr.GetInt32(1);
                                if (!dr.IsDBNull(2)) detalle.ItemCode = dr.GetString(2);
                                if (!dr.IsDBNull(3)) detalle.ItemName = dr.GetString(3);
                                if (!dr.IsDBNull(4)) detalle.BatchNum = dr.GetString(4);
                                if (!dr.IsDBNull(5)) detalle.InDate = dr.GetDateTime(5).ToString("yyyy-MM-dd");
                                if (!dr.IsDBNull(6)) detalle.ExpDate = dr.GetDateTime(6).ToString("yyyy-MM-dd");
                                if (!dr.IsDBNull(7)) detalle.CodigoUbicacion = dr.GetString(7);
                                if (!dr.IsDBNull(8)) detalle.UmAlm = dr.GetString(8);
                                if (!dr.IsDBNull(9)) detalle.ValorUmAlm = dr.GetInt32(9);
                                if (!dr.IsDBNull(10)) detalle.QuantityMaster = dr.GetInt32(10);
                                if (!dr.IsDBNull(11)) detalle.QuantitySaldo = dr.GetInt32(11);
                                if (!dr.IsDBNull(12)) detalle.QuantityUnidadesCajas = dr.GetInt32(12);

                                transferencia.Detalle.Add(detalle);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "TransferenciaReserva_D - ObtenerTransferenciaReserva");
            }

            return transferencia;
        }
        public Helper_E DeleteTransferenciaReserva(int docNum, SqlConnection cn)
        {
            string mensaje, icono;

            try
            {
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoTransferenciaReserva", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros extraidos de la cabecera 
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "DELETE");
                    cmd.Parameters.AddWithValue("@SolicitudTrasladoDocNum", docNum);
                    SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idGeneradoParam);
                    cmd.ExecuteNonQuery();

                    mensaje = "Transferencia de reserva eliminada correctamente";
                    icono = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "TransferenciaReserva_D - DeleteTransferenciaReserva");
                mensaje = "Ocurrió un error al eliminar la transferencia reserva. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en DeleteTransferenciaReserva.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }
        public Helper_E DeleteDetalleItemTransferenciaReserva(List<DetalleTransferenciaReserva_E> detTransferenciaReserva, DetalleSolicitudesTraslado_E detSolicitudTraslado, SqlConnection cn)
        {
            string mensaje, icono;
            //generar una variable string grupoIds concatenando los valores de Id de ids separados por una coma
            try
            {
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }

                List<string> ids = new List<string>();

                foreach (var item in detTransferenciaReserva)
                {
                        ids.Add(item.Id.ToString());
                }

                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoTransferenciaReserva", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "REVERT");
                    cmd.Parameters.AddWithValue("@GrupoIds", string.Join(",", ids));
                    cmd.Parameters.AddWithValue("@DetalleSolicitudesTrasladoId", detSolicitudTraslado.Id);
                    SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idGeneradoParam);

                    cmd.ExecuteNonQuery();

                    mensaje = "Ids en la Transferencia de reserva eliminada correctamente";
                    icono = "success";
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "TransferenciaReserva_D - DeleteDetalleItemTransferenciaReserva");
                mensaje = "Ocurrió un error al eliminar ids en la transferencia reserva. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en DeleteDetalleItemTransferenciaReserva.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }   , IconoSweetAlert = icono };
        }

    }
}
