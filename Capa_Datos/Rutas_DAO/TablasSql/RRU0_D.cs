using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Capa_Datos.Rutas_DAO.TablasSql
{
    public class RRU0_D
    {
        readonly Utilitarios uti = new Utilitarios(); readonly ORRU_D orruD = new ORRU_D();
        public void borrarRRU0(int DocEntry, int Linea)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_ORRU", cn, tran)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "DDR");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Linea", Linea);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("error al borrar detalle D:" + DocEntry + "-L:" + Linea); }
                cn.Close();
            }
            catch (Exception e) { cn.Close(); throw new Exception(e.Message); }
        }
        public void agregarRRU0(RRU0_E o)
        {
            ORTV_D ortvD = new ORTV_D(); ORRU_D orruD = new ORRU_D(); List<RRU0_E> lis = new List<RRU0_E>();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    ortvD.Preenviar(o.DocEntryTicket, o.Operario, tran, cn);
                    SqlCommand cmd = new SqlCommand("al.MANT_ORRU", cn, tran)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "IDR");
                    cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    var x = orruD.obtenerOrdenDeRuta(o.DocEntry);
                    cmd.Parameters.AddWithValue("@Estado", x.Estado);
                    cmd.Parameters.AddWithValue("@Linea", x.DetRRU0.Count() + 1);
                    lis.Add(o);
                    if (lis != null)
                    {
                        SqlParameter tbDet = new SqlParameter("@Det", SqlDbType.Structured);
                        tbDet.Value = RRU0_E.tbDetalle(lis);
                        tbDet.TypeName = "al.TPRRU0";
                        cmd.Parameters.AddWithValue("@Det", tbDet.Value);
                    }
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error al grabar: " + e.Message); }
                cn.Close();
            }
            catch (Exception e1) { cn.Close(); throw new Exception(e1.Message); }
        }
        public void preenviarRRU0(RRU0_E o, SqlTransaction tran, SqlConnection cn)
        {

            try
            {
                SqlCommand cmd = new SqlCommand("al.MANT_ORRU", cn, tran)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "UPDR");
                cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry).Direction = ParameterDirection.InputOutput;
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@Linea", o.Linea);
                cmd.ExecuteNonQuery();
            }
            catch { tran.Rollback(); cn.Close(); }
        }
        public void liberarRRU0(RRU0_E o)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlTransaction tran = cn.BeginTransaction())
                    {
                        try
                        {
                            SqlCommand cmd = new SqlCommand("al.MANT_ORRU", cn, tran) { CommandType = CommandType.StoredProcedure };
                            cmd.Parameters.AddWithValue("@TipoMantenimiento", "ULDR");
                            cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry).Direction = ParameterDirection.InputOutput;
                            cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                            cmd.Parameters.AddWithValue("@Linea", o.Linea);
                            cmd.Parameters.AddWithValue("@ComentarioLiberado", o.ComentarioLiberado);

                            cmd.ExecuteNonQuery();
                            new ORTV_D().Liberar(o.DocEntryTicket, o.Operario, tran, cn);
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw new Exception($"Error al liberar detalle D:{o.DocEntry}-L:{o.Linea}", ex);
                        }
                    }
                }
                catch (Exception e) { throw new Exception(e.Message); }
            }
        }

        public void anularRRU0(RRU0_E o)
        {

            ORTV_D ortvD = new ORTV_D();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_ORRU", cn, tran) { CommandType = CommandType.Text };
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UADR");
                    cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Linea", o.Linea);
                    cmd.ExecuteNonQuery();
                    ortvD.Liberar(o.DocEntryTicket, o.Operario, tran, cn);
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("error al anular detalle D:" + o.DocEntry + "-L:" + o.Linea); }
                cn.Close();
            }
            catch (Exception e) { cn.Close(); throw new Exception(e.Message); }
        }
        public RRU0_E buscarRRU0(int DocEntry, int Linea)
        {
            RRU0_E o = new RRU0_E() { Ticket = new ORTV_E() };
            string query = "select t0.*" +
                ",(select PagoEnv from vt.ortv where DocEntry = t0.DocEntryTicket)" +
                ",(select ClaveEnv from vt.ortv where DocEntry = t0.DocEntryTicket)" +
                ",(select RegCant from vt.rtv5 where DocEntry = t0.DocEntryTicket and Linea=1)" +
                ",(select RegCate from vt.rtv5 where DocEntry = t0.DocEntryTicket and Linea=1)" +
                ",(select RegEstado from vt.rtv5 where DocEntry = t0.DocEntryTicket and Linea=1)" +
                ",(select LugarDestino from vt.ortv where DocEntry = t0.DocEntryTicket)" +
                " from al.rru0 t0 where t0.DocEntry=@DocEntry and t0.Linea=@Linea";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cmd.Parameters.AddWithValue("@Linea", Linea);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { o.Linea = dr.GetInt32(1); }
                if (!dr.IsDBNull(2)) { o.DocEntryTicket = dr.GetInt32(2); }
                if (!dr.IsDBNull(3)) { o.DocNumTicket = dr.GetInt32(3); }
                if (!dr.IsDBNull(4)) { o.Socio = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { o.Guias = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { o.Verificado = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { o.Cajas = dr.GetInt32(7); }
                if (!dr.IsDBNull(8)) { o.Observaciones = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { o.MontoFinal = dr.GetDecimal(9); }
                if (!dr.IsDBNull(10)) { o.Envio = dr.GetDecimal(10); }
                if (!dr.IsDBNull(11)) { o.Direcciones = dr.GetString(11); }
                if (!dr.IsDBNull(12)) { o.Estado = dr.GetString(12); }
                if (!dr.IsDBNull(13)) { o.TempI1 = dr.GetDecimal(13); }
                if (!dr.IsDBNull(14)) { o.HumedI1 = dr.GetDecimal(14); }
                if (!dr.IsDBNull(15)) { o.TempI2 = dr.GetDecimal(15); }
                if (!dr.IsDBNull(16)) { o.HumedI2 = dr.GetDecimal(16); }
                if (!dr.IsDBNull(17)) { o.TempF1 = dr.GetDecimal(17); }
                if (!dr.IsDBNull(18)) { o.HumedF1 = dr.GetDecimal(18); }
                if (!dr.IsDBNull(19)) { o.TempF2 = dr.GetDecimal(19); }
                if (!dr.IsDBNull(20)) { o.HumedF2 = dr.GetDecimal(20); }
                if (!dr.IsDBNull(21)) { o.OpEntrega = dr.GetString(21); }
                if (!dr.IsDBNull(22)) { o.FechaEntrega = dr.GetDateTime(22).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(23)) { o.HoraEntrega = dr.GetTimeSpan(23).ToString(); }
                if (!dr.IsDBNull(24)) { o.Ticket.PagoEnv = dr.GetDecimal(24); }
                if (!dr.IsDBNull(25)) { o.Ticket.ClaveEnv = dr.GetString(25); }
                //regalo
                if (!dr.IsDBNull(26))
                {
                    RTV5_E Regalo = new RTV5_E(); Regalo.RegCant = dr.GetDecimal(26);
                    if (!dr.IsDBNull(27)) { Regalo.RegCate = dr.GetString(27); }
                    if (!dr.IsDBNull(28)) { Regalo.RegEstado = dr.GetString(28); }
                    o.Ticket.Det5 = new List<RTV5_E> { Regalo };
                }
                if (!dr.IsDBNull(29)) { o.Ticket.LugarDestino = dr.GetString(29); }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }
            return o;
        }
        public void enviarRRU0(RRU0_E o, SqlTransaction tran, SqlConnection cn)
        {
            RRU0_E rru0E = buscarRRU0(o.DocEntry, o.Linea); List<RRU0_E> lis = new List<RRU0_E>();
            if (rru0E.Estado != "PREENVIO") { throw new Exception("La linea " + o.Linea + " no esta en preenvio"); }
            try
            {
                SqlCommand cmd = new SqlCommand("al.MANT_ORRU", cn, tran) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@TipoMantenimiento", "UVDR");
                cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry).Direction = ParameterDirection.InputOutput;
                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@Linea", o.Linea);
                lis.Add(o);
                if (lis != null)
                {
                    SqlParameter tbDet = new SqlParameter("@Det", SqlDbType.Structured);
                    tbDet.Value = RRU0_E.tbDetalle(lis);
                    tbDet.TypeName = "al.TPRRU0";
                    cmd.Parameters.AddWithValue("@Det", tbDet.Value);
                }

                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception(e.Message); }
        }

        public void entregarRRU0(RRU0_E o)
        {
            ORTV_D ortvD = new ORTV_D();
            List<RRU0_E> lis = new List<RRU0_E>();
            lis.Add(o);

            ORRU_E orruE = orruD.obtenerOrdenDeRuta(o.DocEntry);
            var tk = ortvD.ObtenerDatosCompletosTicket(o.DocEntryTicket);

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlTransaction tran = cn.BeginTransaction("REPARTO DE PEDIDOS"))
                    {
                        try
                        {
                            if (o != null)
                            {
                                SqlCommand cmd = new SqlCommand("al.MANT_ORRU", cn, tran)
                                {
                                    CommandType = CommandType.StoredProcedure
                                };

                                cmd.Parameters.AddWithValue("@TipoMantenimiento", "UEDR");
                                cmd.Parameters.AddWithValue("@DocEntry", o.DocEntry).Direction = ParameterDirection.InputOutput;
                                cmd.Parameters.AddWithValue("@DocNum", 0).Direction = ParameterDirection.Output;
                                cmd.Parameters.AddWithValue("@Linea", o.Linea);
                                cmd.Parameters.AddWithValue("@OpEntrega", o.OpEntrega);

                                SqlParameter tbDet = new SqlParameter("@Det", SqlDbType.Structured);
                                tbDet.Value = RRU0_E.tbDetalle(lis);
                                tbDet.TypeName = "al.TPRRU0";
                                cmd.Parameters.AddWithValue("@Det", tbDet.Value);

                                cmd.ExecuteNonQuery();

                                // Condición para manejar tipos de rutas
                                if (orruE.TipoRuta == "VD" || orruE.TipoRuta == "VG") // no puede ser VC ni VA
                                {
                                    ORTV_E tk2 = new ORTV_E { DocEntry = o.DocEntryTicket, Operario = o.OpEntrega };

                                    if (o.Ticket != null)
                                    {
                                        tk2.PagoEnv = o.Ticket.PagoEnv;
                                        tk2.ClaveEnv = o.Ticket.ClaveEnv;
                                        if (tk.Det5 != null && tk.Det5.Count > 0)
                                        {
                                            tk2.Det5 = new List<RTV5_E> { tk.Det5[0] };
                                            // Para domicilio pasa el reg estado ENTREGADO
                                            if (orruE.TipoRuta == "VD") { tk2.Det5[0].RegEstado = o.Ticket.Det5[0].RegEstado; }
                                        }
                                    }

                                    ortvD.Entregar(tk2, tran);
                                }
                                else if (orruE.TipoRuta == "AC")
                                {
                                    string RegEstado = string.Empty;
                                    if (tk.Det5 != null && tk.Det5.Count >= 1)
                                    {
                                        if (tk.Det5[0].IdReg > 0) { RegEstado = "Entregado"; }
                                    }
                                    ORTV_E tk2 = new ORTV_E { DocEntry = o.DocEntryTicket, OpRegistro = o.OpEntrega };
                                    tk2.Det5 = new List<RTV5_E>();
                                    tk2.Det5[0].RegEstado = RegEstado;
                                    ortvD.Entregar(tk2, tran);
                                }

                                // Guardar archivo si existe
                                if (o.Archivo != null)
                                {
                                    Directory.CreateDirectory(uti.directorioFileServer + @"\Repartos\Evidencias\Ventas");
                                    string pat = uti.directorioFileServer + @"\Repartos\Evidencias\Ventas\" + o.DocEntryTicket + Path.GetExtension(o.Archivo.FileName);
                                    o.Archivo.SaveAs(pat);
                                }
                            }

                            tran.Commit();
                        }
                        catch (Exception e)
                        {
                            
                            tran.Rollback();
                            throw new Exception(e.Message);
                        }
                    }
                }
                catch
                {
                    cn.Close();
                }
            }
        }

    }
}