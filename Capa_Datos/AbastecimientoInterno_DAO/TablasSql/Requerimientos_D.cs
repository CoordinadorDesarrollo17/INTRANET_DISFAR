using Capa_Entidad;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Capa_Datos.AbastecimientoInterno_DAO.TablasSql
{
    public class Requerimientos_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public Requerimientos_E ObtenerRequerimiento(int id)
        {
            Requerimientos_E requerimiento = null;

            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("sp_MantenimientoRequerimiento", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "GET");
                    cmd.Parameters.AddWithValue("@Id", id);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        requerimiento = new Requerimientos_E();

                        if (!dr.IsDBNull(0)) { requerimiento.Id = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { requerimiento.Origen = dr.GetString(1); }
                        if (!dr.IsDBNull(2)) { requerimiento.Destino = dr.GetString(2); }
                        if (!dr.IsDBNull(3)) { requerimiento.TipoAbastecimiento = dr.GetString(3); }
                        if (!dr.IsDBNull(4)) { requerimiento.TiempoRegistro = dr.GetDateTime(4); }
                        if (!dr.IsDBNull(5)) { requerimiento.OperarioRegistra = dr.GetString(5); }

                        requerimiento.Detalle = new List<DetalleRequerimientos_E>();
                    }

                    if (dr.NextResult() && requerimiento != null)
                    {
                        while (dr.Read())
                        {
                            var detalle = new DetalleRequerimientos_E();

                            if (!dr.IsDBNull(0)) { detalle.Id = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { detalle.RequerimientoId = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { detalle.ItemCode = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { detalle.ItemName = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { detalle.BatchNum = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { detalle.CodigoUbicacionOrigen = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { detalle.CodigoUbicacionDestino = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { detalle.UmAlm = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { detalle.ValorUmAlm = dr.GetInt32(8); }
                            if (!dr.IsDBNull(9)) { detalle.QuantityMaster = dr.GetInt32(9); }
                            if (!dr.IsDBNull(10)) { detalle.QuantitySaldo = dr.GetInt32(10); }
                            if (!dr.IsDBNull(11)) { detalle.QuantityUnidadesCajas = dr.GetInt32(11); }
                            if (!dr.IsDBNull(12)) { detalle.AtendidoReserva = dr.GetInt32(12); }
                            if (!dr.IsDBNull(13)) { detalle.AtendidoPicking = dr.GetInt32(13); }

                            requerimiento.Detalle.Add(detalle);
                        }
                    }

                    dr.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el requerimiento.", ex);
                }
            }

            return requerimiento;
        }
        public Helper_E AtenderReserva(int detalleId)
        {
            string mensaje, icono;

            try
            {
              using (SqlConnection  cn = new SqlConnection()) { 
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_MantenimientoRequerimiento", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "ATD_RESERVA");
                        cmd.Parameters.AddWithValue("@DetalleId", detalleId);

                        cmd.ExecuteNonQuery();

                        mensaje = "Detalle requerimiento AtendidoReserva actualizado";
                        icono = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Requerimientos_D - AtenderReserva");
                mensaje = "Ocurrió un error al actualizar AtendidoReserva. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en AtenderReserva.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }
        public Helper_E AtenderPicking(int detalleId)
        {
            string mensaje, icono;

            try
            {
                using (SqlConnection cn = new SqlConnection())
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_MantenimientoRequerimiento", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "ATD_PICKING");
                        cmd.Parameters.AddWithValue("@DetalleId", detalleId);

                        cmd.ExecuteNonQuery();

                        mensaje = "Detalle requerimiento AtendidoPicking actualizado";
                        icono = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Requerimientos_D - AtenderPicking");
                mensaje = "Ocurrió un error al actualizar AtenderPicking. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en AtenderPicking.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, IconoSweetAlert = icono };
        }
        public List<DetalleRequerimientos_E> ListarDetalles()
        {
            List<DetalleRequerimientos_E> lista = null;

            using (SqlConnection cn = new SqlConnection(uti.cadSql2))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("sp_MantenimientoRequerimiento", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "LIST");
                    SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idGeneradoParam);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        lista = new List<DetalleRequerimientos_E>();
                        while (dr.Read())
                        {
                            var detalle = new DetalleRequerimientos_E();
                            if (!dr.IsDBNull(0)) { detalle.Id = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { detalle.RequerimientoId = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { detalle.ItemCode = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { detalle.ItemName = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { detalle.BatchNum = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { detalle.CodigoUbicacionOrigen = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { detalle.CodigoUbicacionDestino = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { detalle.UmAlm = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { detalle.ValorUmAlm = dr.GetInt32(8); }
                            if (!dr.IsDBNull(9)) { detalle.QuantityMaster = dr.GetInt32(9); }
                            if (!dr.IsDBNull(10)) { detalle.QuantitySaldo = dr.GetInt32(10); }
                            if (!dr.IsDBNull(11)) { detalle.QuantityUnidadesCajas = dr.GetInt32(11); }
                            if (!dr.IsDBNull(12)) { detalle.AtendidoReserva = dr.GetInt32(12); }
                            if (!dr.IsDBNull(13)) { detalle.AtendidoPicking = dr.GetInt32(13); }
                            if (!dr.IsDBNull(14)) { detalle.Nivel = dr.GetString(14); }
                            if (!dr.IsDBNull(15)) { detalle.Posicion = dr.GetString(15); }
                            if (!dr.IsDBNull(16)) { detalle.RackBloque = dr.GetString(16); }
                            lista.Add(detalle);
                        }
                    }

                    dr.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al obtener el detalle de los requerimientos sin atendidoPicking.", ex);
                }
            }

            return lista;
        }
        public Requerimientos_E RegistrarRequerimiento(Requerimientos_E requerimiento, SqlConnection cn)
        {
            if (cn.State != ConnectionState.Open)
            {
                cn.Open();
            }
            using (var transaction = cn.BeginTransaction())
            {
                try
                {
                    using (SqlCommand cmd = new SqlCommand("sp_MantenimientoRequerimiento", cn, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "INSERT");
                        cmd.Parameters.AddWithValue("@Origen", requerimiento.Origen);
                        cmd.Parameters.AddWithValue("@Destino", requerimiento.Destino);
                        cmd.Parameters.AddWithValue("@TipoAbastecimiento", requerimiento.TipoAbastecimiento);
                        cmd.Parameters.AddWithValue("@OperarioRegistra", requerimiento.OperarioRegistra);

                        // Crear tabla de parámetros para el tipo DetalleRequerimientosType
                        DataTable detalleTable = new DataTable();
                        detalleTable.Columns.Add("ItemCode", typeof(string));
                        detalleTable.Columns.Add("ItemName", typeof(string));
                        detalleTable.Columns.Add("BatchNum", typeof(string));
                        detalleTable.Columns.Add("CodigoUbicacionOrigen", typeof(string));
                        detalleTable.Columns.Add("CodigoUbicacionDestino", typeof(string));
                        detalleTable.Columns.Add("UmAlm", typeof(string));
                        detalleTable.Columns.Add("ValorUmAlm", typeof(int));
                        detalleTable.Columns.Add("QuantityMaster", typeof(int));
                        detalleTable.Columns.Add("QuantitySaldo", typeof(int));
                        detalleTable.Columns.Add("QuantityUnidadesCajas", typeof(int));
                        detalleTable.Columns.Add("AtendidoReserva", typeof(int));
                        detalleTable.Columns.Add("AtendidoPicking", typeof(int));

                        foreach (var detalle in requerimiento.Detalle)
                        {
                            detalleTable.Rows.Add(
                                detalle.ItemCode,
                                detalle.ItemName,
                                detalle.BatchNum,
                                detalle.CodigoUbicacionOrigen,
                                detalle.CodigoUbicacionDestino,
                                detalle.UmAlm,
                                detalle.ValorUmAlm,
                                (object)detalle.QuantityMaster ?? DBNull.Value,
                                (object)detalle.QuantitySaldo ?? DBNull.Value,
                                (object)detalle.QuantityUnidadesCajas ?? DBNull.Value,
                                0,
                                0
                            );
                        }

                        SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(idGeneradoParam);

                        SqlParameter detalleParam = new SqlParameter("@Detalle", SqlDbType.Structured)
                        {
                            TypeName = "dbo.DetalleRequerimientosType",
                            Value = detalleTable
                        };
                        cmd.Parameters.Add(detalleParam);

                        cmd.ExecuteNonQuery();
                        int idGenerado = (int)idGeneradoParam.Value;

                        if (idGenerado > 0)
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                            throw new Exception("No se pudo registrar el requerimiento.");
                        }

                        requerimiento.Id = idGenerado;
                        return requerimiento;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error al registrar el requerimiento.", ex);
                }
            }
        }


    }
}
