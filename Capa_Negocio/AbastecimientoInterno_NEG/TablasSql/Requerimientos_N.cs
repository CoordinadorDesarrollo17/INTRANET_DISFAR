using Capa_Datos.AbastecimientoInterno_DAO.TablasSql;
using Capa_Entidad.AbastecimientoInterno_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.AbastecimientoInterno_NEG.TablasSql
{
    public class Requerimientos_N
    {
        Requerimientos_D requerimientoD = new Requerimientos_D();

        public Requerimientos_E ObtenerRequerimiento(int id)
        {
            return requerimientoD.ObtenerRequerimiento(id);
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
