using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class MotivosDevoluciones_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<MotivosDevoluciones_E> ListarMotivosDevoluciones(MotivosDevoluciones_E filtro)
        {
            List<MotivosDevoluciones_E> lista = new List<MotivosDevoluciones_E>();
            string condWhere = string.Empty;

            if (filtro != null)
            {
                if (!string.IsNullOrWhiteSpace(filtro.Estado))
                {
                    condWhere = $" AND Estado = '{filtro.Estado}'";
                }
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = $"SELECT IdMotivo, Descripcion, Estado, CASE Estado WHEN '1' THEN 'ACTIVO' ELSE 'INACTIVO' END as 'DescEstado' FROM al.MotivosDevoluciones WHERE IdMotivo > 0 {condWhere} ORDER BY Descripcion ASC";
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            MotivosDevoluciones_E md = new MotivosDevoluciones_E();
                            if (!dr.IsDBNull(0)) { md.IdMotivo = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { md.Descripcion = dr.GetString(1); }
                            if (!dr.IsDBNull(2)) { md.Estado = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { md.DescEstado = dr.GetString(3); }
                            lista.Add(md);
                        }
                    }

                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cn.Close();
                }
            }

            return lista;
        }

        public string RegistrarMotivoDevolucion(MotivosDevoluciones_E motivo)
        {
            string result = string.Empty;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_MOTIVOS_DEVOLUCIONES", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", "INS");
                    cmd.Parameters.AddWithValue("@Descripcion", motivo.Descripcion);
                    cmd.Parameters.AddWithValue("@Estado", motivo.Estado);
                    cmd.Parameters.AddWithValue("@OpRegistro", motivo.Operario);
                    cmd.Parameters.AddWithValue("@IdMotivo", 0);

                    cmd.ExecuteNonQuery();
                    result = "Motivo agregado exitosamente";

                    tran.Commit();
                }
                catch (Exception ex2)
                {
                    result = "Error al registrar motivo"; tran.Rollback();
                    throw new Exception("Error en registro: " + ex2.Message);
                }
                finally
                {
                    cn.Close();
                }
            }

            return result;
        }

        public string EditarMotivoDevolucion(MotivosDevoluciones_E motivo)
        {
            string result = string.Empty;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_MOTIVOS_DEVOLUCIONES", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", "EDI");
                    cmd.Parameters.AddWithValue("@Descripcion", motivo.Descripcion);
                    cmd.Parameters.AddWithValue("@Estado", motivo.Estado);
                    cmd.Parameters.AddWithValue("@OpModificacion", motivo.Operario);
                    cmd.Parameters.AddWithValue("@IdMotivo", motivo.IdMotivo);

                    cmd.ExecuteNonQuery();
                    result = "Motivo editado exitosamente";

                    tran.Commit();
                }
                catch (Exception ex2)
                {
                    result = "Error al editar motivo"; tran.Rollback();
                    throw new Exception("Error en registro: " + ex2.Message);
                }
                finally
                {
                    cn.Close();
                }
            }

            return result;
        }
    }
}