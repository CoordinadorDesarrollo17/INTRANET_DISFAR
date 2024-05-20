using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.Caja_ENT;
using Capa_Entidad.Ventas_ENT.TablasSql;

namespace Capa_Datos.Caja_DAO
{
    public class OPP_D
    {
        readonly Utilitarios uti = new Utilitarios();
        DBHelper db = new DBHelper();

        public List<OPP_E> ObtenerDatosPagosParciales(int idOTC)
        {
            List<OPP_E> result = null;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT PP.IdOPP, PP.IdOTC, PP.Monto, PP.RegistradoPor, PP.Estado, CONVERT(varchar, PP.FechaRegistro, 103), CONVERT(varchar, PP.HoraRegistro, 8), PP.Comentario");
                sb.Append(" FROM cj.OPP PP");
                sb.Append($" WHERE PP.IdOTC = @IdOTC ORDER BY PP.IdOPP DESC");

                string query = sb.ToString();

                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@IdOTC", idOTC);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        result = new List<OPP_E>();
                        while (dr.Read())
                        {
                            OPP_E obj = new OPP_E();
                            if (!dr.IsDBNull(0)) { obj.IdOPP = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { obj.IdOTC = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { obj.Monto = dr.GetDecimal(2); }
                            if (!dr.IsDBNull(3)) { obj.RegistradoPor = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { obj.Estado = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { obj.FechaRegistro = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { obj.HoraRegistro = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { obj.Comentario = dr.GetString(7); }

                            result.Add(obj);
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

            return result;
        }

        public int EliminarPagoParcial(OPP_E datos)
        {
            int result = -1;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("cj.MANT_OPP", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", "ELI");
                    cmd.Parameters.AddWithValue("@IdOPP", datos.IdOPP);
                    cmd.Parameters.AddWithValue("@EliminadoPor", datos.RegistradoPor);

                    cmd.ExecuteNonQuery();
                    result = 1;
                    tran.Commit();
                }
                catch (Exception ex2)
                {
                    result = 0; tran.Rollback();
                    throw new Exception(ex2.Message);
                }
                finally
                {
                    cn.Close();
                }
            }

            return result;
        }

        public decimal ObtenerTotalPagos(int idOTC)
        {
            decimal result = 0;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT SUM(Monto)");
                sb.Append(" FROM cj.OPP WHERE IdOTC = @IdOTC AND Estado = '1'");

                string query = sb.ToString();

                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@IdOTC", idOTC);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        dr.Read();
                        if (!dr.IsDBNull(0)) { result = dr.GetDecimal(0); }
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

            return result;
        }
    }
}