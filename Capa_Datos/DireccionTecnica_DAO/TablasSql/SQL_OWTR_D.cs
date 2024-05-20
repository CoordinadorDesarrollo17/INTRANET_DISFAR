using Capa_Datos.Almacen_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.TablasSql;
using Capa_Entidad.DireccionTecnica_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.DireccionTecnica_DAO.TablasSql
{
    public class SQL_OWTR_D
    {
        readonly Utilitarios uti = new Utilitarios();
        public SQL_OWTR_E ObtenerOWTR(int DocNumSAP)
        {
            SQL_OWTR_E owtr = new SQL_OWTR_E();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT DocNumSAP, Estado, OpRegistro, CONVERT(varchar, FechaRegistro,103), CONVERT(varchar, HoraRegistro, 8) FROM dt.SQL_OWTR WHERE DocNumSAP = @DocNumSAP";
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocNumSAP", DocNumSAP);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta
                    if (dr.HasRows)
                    {
                        dr.Read();

                        if (!dr.IsDBNull(0)) { owtr.DocNumSAP = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { owtr.Estado = dr.GetString(1); }
                        if (!dr.IsDBNull(2)) { owtr.OpRegistro = dr.GetString(2); }
                        if (!dr.IsDBNull(3)) { owtr.FechaRegistro = dr.GetString(3); }
                        if (!dr.IsDBNull(4)) { owtr.HoraRegistro = dr.GetString(4); }
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

            return owtr;
        }

        public int CambiarEstadoOWTR(SQL_OWTR_E owtr, string accion)
        {
            int status = -1;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("dt.MANT_SQL_OWTR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", accion);
                    cmd.Parameters.AddWithValue("@DocNumSAP", owtr.DocNumSAP);
                    cmd.Parameters.AddWithValue("@Estado", owtr.Estado);
                    cmd.Parameters.AddWithValue("@OpRegistro", owtr.OpRegistro);

                    cmd.ExecuteNonQuery();
                    status = 1;

                    tran.Commit();
                }
                catch (Exception ex2)
                {
                    status = 0; tran.Rollback();
                    throw new Exception("Error en registro: " + ex2.Message);
                }
                finally
                {
                    cn.Close();
                }
            }

            return status;
        }

    }
}
