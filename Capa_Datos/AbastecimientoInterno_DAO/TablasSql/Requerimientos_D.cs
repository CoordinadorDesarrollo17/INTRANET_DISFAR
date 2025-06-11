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

        public Requerimientos_E ObtenerRequerimiento(int id, SqlConnection cn)
        {
            Requerimientos_E requerimiento = null;

            string mensaje, icono;
            if (cn.State != ConnectionState.Open)
            {
                cn.Open();
            }

            try
            {
                SqlCommand cmd = new SqlCommand("sp_MantenimientoRequerimiento", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "GET");
                cmd.Parameters.AddWithValue("@Id", id);
                var outputId = new SqlParameter("@IdGenerado", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputId);

                SqlParameter param2 = new SqlParameter("@Aprobado", SqlDbType.Int)
                {
                    Value = 0
                };
                cmd.Parameters.Add(param2);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    requerimiento = new Requerimientos_E();

                    if (!dr.IsDBNull(0)) { requerimiento.Id = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { requerimiento.Origen = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { requerimiento.Destino = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { requerimiento.TipoAbastecimiento = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { requerimiento.Comentario = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { requerimiento.TiempoRegistro = dr.GetDateTime(5); }
                    if (!dr.IsDBNull(6)) { requerimiento.OperarioRegistra = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { requerimiento.Zona = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { requerimiento.Aprobado = dr.GetInt32(8); }

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

            return requerimiento;
        }

        public Helper_E AtenderReserva(int detalleId)
        {
            string mensaje, icono;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_MantenimientoRequerimiento", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "ATD_RESERVA");
                        cmd.Parameters.AddWithValue("@DetalleId", detalleId);
                        SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(idGeneradoParam);

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

            return new Helper_E { Mensajes = new List<string> { mensaje }, Icono = icono };
        }
        public Helper_E AtenderPicking(int detalleId, SqlConnection cn)
        {
            string mensaje, icono;
            if (cn.State != ConnectionState.Open)
            {
                cn.Open();
            }

            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoRequerimiento", cn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "ATD_PICKING");
                    cmd.Parameters.AddWithValue("@DetalleId", detalleId);
                    SqlParameter idGeneradoParam = new SqlParameter("@IdGenerado", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(idGeneradoParam);
                    cmd.ExecuteNonQuery();

                    mensaje = "Detalle requerimiento AtendidoPicking actualizado";
                    icono = "success";
                }

            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Requerimientos_D - AtenderPicking");
                mensaje = "Ocurrió un error al actualizar AtenderPicking. Comuníquese con el área de Sistemas para más información.";
                icono = "error";
                throw new Exception("Error en AtenderPicking.", ex);
            }

            return new Helper_E { Mensajes = new List<string> { mensaje }, Icono = icono };
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

                    SqlParameter param2 = new SqlParameter("@Aprobado", SqlDbType.Int)
                    {
                        Value = 0
                    };
                    cmd.Parameters.Add(param2);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
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
                            if (!dr.IsDBNull(17)) { detalle.Zona = dr.GetString(17); }
                            if (!dr.IsDBNull(18)) { detalle.Aprobado = dr.GetInt32(18); }
                            lista.Add(detalle);
                        }
                    }
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
            try
            {
                using (SqlCommand cmd = new SqlCommand("sp_MantenimientoRequerimiento", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "INSERT");
                    cmd.Parameters.AddWithValue("@Origen", requerimiento.Origen);
                    cmd.Parameters.AddWithValue("@Destino", requerimiento.Destino);
                    cmd.Parameters.AddWithValue("@TipoAbastecimiento", requerimiento.TipoAbastecimiento);
                    cmd.Parameters.AddWithValue("@Comentario", requerimiento.Comentario);
                    cmd.Parameters.AddWithValue("@OperarioRegistra", requerimiento.OperarioRegistra);
                    cmd.Parameters.AddWithValue("@Zona", requerimiento.Zona);

                    // Crear tabla de parámetros para el tipo DetalleRequerimientosType
                    DataTable detalleTable = new DataTable();
                    detalleTable.Columns.Add("Id", typeof(int));
                    detalleTable.Columns.Add("RequerimientoId", typeof(int));
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
                            0,
                            0,
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

                    SqlParameter param2 = new SqlParameter("@Aprobado", SqlDbType.Int)
                    {
                        Value = 0
                    };
                    cmd.Parameters.Add(param2);

                    SqlParameter detalleParam = new SqlParameter("@Detalle", SqlDbType.Structured)
                    {
                        TypeName = "dbo.DetalleRequerimientosType",
                        Value = detalleTable
                    };
                    cmd.Parameters.Add(detalleParam);

                    // Retorno de ids generados para cabecera y detalle
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var detalles = new List<DetalleRequerimientos_E>();
                        while (reader.Read())
                        {
                            var detalle = new DetalleRequerimientos_E
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                RequerimientoId = reader.GetInt32(reader.GetOrdinal("RequerimientoId")),
                                ItemCode = reader.GetString(reader.GetOrdinal("ItemCode")),
                                ItemName = reader.GetString(reader.GetOrdinal("ItemName")),
                                BatchNum = reader.GetString(reader.GetOrdinal("BatchNum")),
                                CodigoUbicacionOrigen = reader.GetString(reader.GetOrdinal("CodigoUbicacionOrigen")),
                                CodigoUbicacionDestino = reader.IsDBNull(reader.GetOrdinal("CodigoUbicacionDestino")) ? null : reader.GetString(reader.GetOrdinal("CodigoUbicacionDestino")),
                                UmAlm = reader.GetString(reader.GetOrdinal("UmAlm")),
                                ValorUmAlm = reader.GetInt32(reader.GetOrdinal("ValorUmAlm")),
                                QuantityMaster = reader.IsDBNull(reader.GetOrdinal("QuantityMaster")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("QuantityMaster")),
                                QuantitySaldo = reader.IsDBNull(reader.GetOrdinal("QuantitySaldo")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("QuantitySaldo")),
                                QuantityUnidadesCajas = reader.IsDBNull(reader.GetOrdinal("QuantityUnidadesCajas")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("QuantityUnidadesCajas")),
                                AtendidoReserva = reader.GetInt32(reader.GetOrdinal("AtendidoReserva")),
                                AtendidoPicking = reader.GetInt32(reader.GetOrdinal("AtendidoPicking"))
                            };
                            detalles.Add(detalle);
                        }
                        requerimiento.Detalle = detalles;
                    }

                    // Ahora es seguro acceder al valor del parámetro de salida
                    if (idGeneradoParam.Value == DBNull.Value)
                        throw new Exception("El procedimiento no devolvió un Id generado.");

                    int idGenerado = (int)idGeneradoParam.Value;
                    requerimiento.Id = idGenerado;

                    return requerimiento;
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Requerimientos_D - RegistrarRequerimiento");
                throw new Exception("Error al registrar el requerimiento.", ex);
            }
        }

        public (Helper_E, List<Requerimientos_E>) ListarRequerimientos(string condicion, Dictionary<string, object> parametros)
        {
            var lista = new List<Requerimientos_E>();
            Helper_E _helper = new Helper_E();
            var top = string.IsNullOrEmpty(condicion) ? "TOP 100" : string.Empty;

            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql2))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;

                    var sb = new StringBuilder();

                    sb.AppendLine($"SELECT {top} RQ.Id, RQ.Origen, RQ.Destino, RQ.TipoAbastecimiento, RQ.Comentario, RQ.TiempoRegistro, RQ.OperarioRegistra, RQ.Zona, RQ.Aprobado");
                    sb.AppendLine("FROM Requerimientos RQ");
                    sb.AppendLine(condicion?.ToString().Trim());

                    // Agregamos los parámetros dinámicamente
                    foreach (var prm in parametros)
                    {
                        cmd.Parameters.AddWithValue(prm.Key, prm.Value);
                    }

                    sb.AppendLine("ORDER BY 1 DESC");
                    cmd.CommandText = sb.ToString();

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var obj = new Requerimientos_E();
                                if (!dr.IsDBNull(0))  obj.Id = dr.GetInt32(0); 
                            }
                        }

                        _helper.Titulo = "Acción completada";
                        _helper.Mensajes.Add("Se envió la solicitud para revertir la liberación de este artículo correctamente.");
                        _helper.Icono = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error inesperado en Requerimientos_D - ListarRequerimientos()");

                _helper.Titulo = "Error";
                _helper.Mensajes.Add("Ocurrió un error al enviar la solicitud para revertir la liberación del artículo.");
                _helper.Mensajes.Add("Por favor, comuníquese con el área de Sistemas para más información.");
                _helper.Icono = "error";
            }

            return (_helper, lista);
        }
    }
}