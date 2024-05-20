using Capa_Entidad.Almacen_ENT.TablasSql;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Math;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class SubmotivosDevoluciones_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<SubmotivosDevoluciones_E> ListarSubmotivosDevoluciones(SubmotivosDevoluciones_E filtro)
        {
            List<SubmotivosDevoluciones_E> lista = new List<SubmotivosDevoluciones_E>();
            string condWhere = string.Empty;

            if (filtro != null)
            {
                if (!string.IsNullOrEmpty(filtro.Estado))
                {
                    condWhere = $" AND SUB.Estado = '{filtro.Estado}'";
                }
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT SUB.IdSubmotivo, SUB.IdMotivo, SUB.Descripcion, SUB.Estado, CASE SUB.Estado WHEN '1' THEN 'ACTIVO' ELSE 'INACTIVO' END as 'DescEstado', MD.Descripcion");
                sb.Append(" FROM al.SubmotivosDevoluciones SUB");
                sb.Append(" INNER JOIN al.MotivosDevoluciones MD ON MD.IdMotivo= SUB.IdMotivo");
                sb.Append($" WHERE SUB.IdSubmotivo > 0 {condWhere}");
                sb.Append(" ORDER BY SUB.Descripcion ASC");

                string query = sb.ToString();
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cn.Open();
                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            SubmotivosDevoluciones_E sub = new SubmotivosDevoluciones_E();
                            if (!dr.IsDBNull(0)) { sub.IdSubmotivo = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { sub.IdMotivo = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { sub.Descripcion = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { sub.Estado = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { sub.DescEstado = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { sub.DescMotivo = dr.GetString(5); }
                            lista.Add(sub);
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

        public string RegistrarSubmotivoDevolucion(SubmotivosDevoluciones_E submotivo)
        {
            string result = string.Empty;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_SUBMOTIVOS_DEVOLUCIONES", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", "INS");
                    cmd.Parameters.AddWithValue("@Descripcion", submotivo.Descripcion);
                    cmd.Parameters.AddWithValue("@Estado", submotivo.Estado);
                    cmd.Parameters.AddWithValue("@OpRegistro", submotivo.Operario);
                    cmd.Parameters.AddWithValue("@IdMotivo", submotivo.IdMotivo);
                    cmd.Parameters.AddWithValue("@IdSubmotivo", 0);

                    cmd.ExecuteNonQuery();
                    result = "Submotivo agregado exitosamente";

                    tran.Commit();
                }
                catch (Exception ex2)
                {
                    result = "Error al registrar submotivo"; tran.Rollback();
                    throw new Exception("Error en registro: " + ex2.Message);
                }
                finally
                {
                    cn.Close();
                }
            }

            return result;
        }

        public string EditarSubmotivoDevolucion(SubmotivosDevoluciones_E submotivo)
        {
            string result = string.Empty;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_SUBMOTIVOS_DEVOLUCIONES", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", "EDI");
                    cmd.Parameters.AddWithValue("@Descripcion", submotivo.Descripcion);
                    cmd.Parameters.AddWithValue("@Estado", submotivo.Estado);
                    cmd.Parameters.AddWithValue("@OpModificacion", submotivo.Operario);
                    cmd.Parameters.AddWithValue("@IdMotivo", submotivo.IdMotivo);
                    cmd.Parameters.AddWithValue("@IdSubmotivo", submotivo.IdSubmotivo);

                    cmd.ExecuteNonQuery();
                    result = "Submotivo editado exitosamente";

                    tran.Commit();
                }
                catch (Exception ex2)
                {
                    result = "Error al editar submotivo"; tran.Rollback();
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