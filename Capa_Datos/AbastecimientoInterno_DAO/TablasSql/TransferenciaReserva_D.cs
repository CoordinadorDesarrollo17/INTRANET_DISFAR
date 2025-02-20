using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using Capa_Entidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class TransferenciaReserva_D
    {
        readonly Utilitarios uti = new Utilitarios();
        readonly DBHelper db = new DBHelper();

        public TransferenciaReserva_E RegistrarTransferenciaReserva(TransferenciaReserva_E transferencia)
        {
            using (var connection = new SqlConnection(uti.cadSql2))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var command = new SqlCommand("[dbo].[sp_MantenimientoTransferenciaReserva]", connection, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Parámetros de entrada
                            command.Parameters.AddWithValue("@TipoMantenimiento", "INSERT");
                            command.Parameters.AddWithValue("@SolicitudTrasladoId", transferencia.SolicitudTrasladoId);
                            command.Parameters.AddWithValue("@SolicitudTrasladoDocNum", transferencia.SolicitudTrasladoDocNum);
                            command.Parameters.AddWithValue("@CardCode", transferencia.CardCode);
                            command.Parameters.AddWithValue("@CardName", transferencia.CardName);
                            command.Parameters.AddWithValue("@NroGuia", transferencia.NroGuia);
                            command.Parameters.AddWithValue("@OperarioRegistra", transferencia.OperarioRegistra);

                            // Parámetro de salida
                            SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            command.Parameters.Add(idGeneradoParam);

                            // Tabla de detalles
                            DataTable dtDetalle = ConvertirADatatable(transferencia.Detalle);
                            SqlParameter detallesParam = command.Parameters.AddWithValue("@Detalle", dtDetalle);
                            detallesParam.SqlDbType = SqlDbType.Structured;
                            detallesParam.TypeName = "dbo.DetalleTransferenciaReservaType";

                            // Ejecutar el procedimiento almacenado
                            command.ExecuteNonQuery();

                            // Obtener el ID generado
                            int idGenerado = (int)idGeneradoParam.Value;
                            transaction.Commit();

                            // Retornar objeto con el ID generado
                            transferencia.Id = idGenerado;
                            return transferencia;
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error al registrar la transferencia de reserva.", ex);
                    }
                }
            }
        }
        private DataTable ConvertirADatatable(List<DetalleTransferenciaReserva_E> detalles)
        {
            DataTable table = new DataTable();
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
                table.Rows.Add(detalle.ItemCode, detalle.ItemName, detalle.BatchNum, detalle.FechaVencimiento, detalle.FechaAdmision,
                               detalle.CodigoUbicacion, detalle.UmAlm, detalle.ValorUmAlm,
                               detalle.QuantityMaster, detalle.QuantitySaldo, detalle.QuantityUnidadesCajas);
            }
            return table;
        }


        public TransferenciaReserva_E ObtenerTransferenciaReserva(int docNum)
        {
            TransferenciaReserva_E transferencia = null;
            SqlConnection cn = new SqlConnection(uti.cadSql2);
            try
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand("sp.MantenimientoTransferenciaReserva", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "GET");
                    cmd.Parameters.AddWithValue("@DocNum", docNum);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            transferencia = new TransferenciaReserva_E();
                            if (!dr.IsDBNull(0)) { transferencia.Id = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { transferencia.SolicitudTrasladoId = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { transferencia.SolicitudTrasladoId = dr.GetInt32(2); }
                            if (!dr.IsDBNull(3)) { transferencia.DocDate = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { transferencia.CardCode = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { transferencia.CardName = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { transferencia.NroGuia = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { transferencia.OperarioRegistra = dr.GetString(7); }
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
                                if (!dr.IsDBNull(5)) detalle.FechaAdmision = dr.GetString(5);
                                if (!dr.IsDBNull(6)) detalle.FechaVencimiento = dr.GetString(6);
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
            catch { cn.Close(); }
            return transferencia;
        }

    }
}
