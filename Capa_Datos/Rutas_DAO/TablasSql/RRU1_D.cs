using Capa_Entidad.Rutas_ENT.TablasSql;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Capa_Datos.Rutas_DAO.TablasSql
{
    public class RRU1_D
    {
        Utilitarios uti = new Utilitarios();

        public void ActualizarNroCajas(int DocEntry, int BaseLinea)
        {
            int cajas = 0;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                // Primero traemos el cálculo del total de las cajas de la guía seleccionada para actualizar el RRU1 luego de haberse calculado la cantidad
                // Y la cantidad debe ser mayor a cero
                string query = "SELECT SUM(cajas) FROM al.RRU11 WHERE BaseEntry = @BaseEntry AND BaseLinea = @BaseLinea; ";

                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@BaseEntry", DocEntry);
                cmd.Parameters.AddWithValue("@Baselinea", BaseLinea);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            if (!dr.IsDBNull(0)) { cajas = dr.GetInt32(0); }
                        }
                    }

                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }


                if (cajas > 0)
                {
                    string query2 = $"UPDATE al.RRU1 SET Cajas = @Cajas WHERE DocEntry = @DocEntry AND Linea = @Linea";

                    
                    SqlTransaction tran = cn.BeginTransaction("ACTUALIZAR CAJAS");
                    try
                    {
                        SqlCommand cmd2 = new SqlCommand(query2, cn, tran);         // prepara
                        cmd2.Parameters.AddWithValue("@Cajas", cajas);
                        cmd2.Parameters.AddWithValue("@DocEntry", DocEntry);
                        cmd2.Parameters.AddWithValue("@Linea", BaseLinea);

                        cmd2.ExecuteNonQuery();
                        tran.Commit();
                    }
                    catch (Exception e) { tran.Rollback(); throw new Exception("Error: " + e.Message); }
                }
                cn.Close();
            }
        }

        public RRU1_E buscarRRU1(int DocEntry, int Linea)
        {
            RRU1_E o = new RRU1_E();
            string query = "select * from al.rru1 where DocEntry=@DocEntry and Linea=@Linea";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@Linea", Linea);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { o.Linea = dr.GetInt32(1); }
                if (!dr.IsDBNull(2)) { o.Guia = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { o.NroSap = dr.GetInt32(3); }
                if (!dr.IsDBNull(4)) { o.OpEnvio = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { o.Cajas = dr.GetInt32(5); }
                if (!dr.IsDBNull(6)) { o.OpRecepcion = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { o.Verificado = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { o.Estado = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { o.TempI1 = dr.GetDecimal(9); }
                if (!dr.IsDBNull(10)) { o.HumedI1 = dr.GetDecimal(10); }
                if (!dr.IsDBNull(11)) { o.TempI2 = dr.GetDecimal(11); }
                if (!dr.IsDBNull(12)) { o.HumedI2 = dr.GetDecimal(12); }
                if (!dr.IsDBNull(13)) { o.TempF1 = dr.GetDecimal(13); }
                if (!dr.IsDBNull(14)) { o.HumedF1 = dr.GetDecimal(14); }
                if (!dr.IsDBNull(15)) { o.TempF2 = dr.GetDecimal(15); }
                if (!dr.IsDBNull(16)) { o.HumedF2 = dr.GetDecimal(16); }
                if (!dr.IsDBNull(17)) { o.OpEntrega = dr.GetString(17); }
                if (!dr.IsDBNull(18)) { o.FechaEntrega = dr.GetDateTime(18).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(19)) { o.HoraEntrega = dr.GetTimeSpan(19).ToString(); }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return o;
        }
        public void enviarRRU1(RRU1_E o, SqlTransaction tran, SqlConnection cn)
        {
            RRU1_E rru1E = buscarRRU1(o.DocEntry, o.Linea);
            if (rru1E.Estado != "PREENVIO") { throw new Exception("La linea " + o.Linea + " no esta en preenvio"); }
            string query = "update al.rru1 set Estado='ENVIADO', TempI1=@TempI1, TempI2=@TempI2 WHERE DocEntry=@DocEntry AND Linea=@Linea";
            try
            {
                SqlCommand cmd = new SqlCommand(query, cn, tran);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry);
                cmd.Parameters.AddWithValue("@Linea", o.Linea);
                cmd.Parameters.AddWithValue("@TempI1", o.TempI1);
                cmd.Parameters.AddWithValue("@TempI2", o.TempI2);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception(e.Message); }
        }
        public void entregarRRU1(RRU1_E o)
        {
            string query = "update al.rru1 set Estado='ENTREGADO',TempF1=@TempF1,TempF2=@TempF2" +
                ",OpEntrega=@OpEntrega,FechaEntrega=(select convert(char(10),getdate(),126))," +
                "HoraEntrega=(select convert(char(5),getdate(),108))" +
                " where DocEntry=@DocEntry and Linea=@Linea";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand(query, cn, tran);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry);
                    cmd.Parameters.AddWithValue("@Linea", o.Linea);
                    cmd.Parameters.AddWithValue("@TempF1", o.TempF1);
                    cmd.Parameters.AddWithValue("@TempF2", o.TempF2);
                    cmd.Parameters.AddWithValue("@OpEntrega", o.OpEntrega);
                    cmd.ExecuteNonQuery();
                    if (o.Archivo != null)
                    {
                        Directory.CreateDirectory(uti.directorioFileServer + @"\Repartos\Evidencias\Traslados");
                        string pat = uti.directorioFileServer + @"\Repartos\Evidencias\Traslados\" + o.NroSap + Path.GetExtension(o.Archivo.FileName);
                        o.Archivo.SaveAs(pat);
                    }
                    tran.Commit();
                    cn.Close();
                }
                catch { tran.Rollback(); cn.Close(); }
            }
            catch { cn.Close(); }
        }
    }
}