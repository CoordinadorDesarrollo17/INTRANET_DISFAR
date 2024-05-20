using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Capa_Entidad.Seguridad_ENT;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class OIAR_D
    {
        DBHelper db = new DBHelper(); Utilitarios uti = new Utilitarios(); IAR1_D iar1D = new IAR1_D();
        public List<OIAR_E> Listar(OIAR_E filtro, Usuario_E user, string tipo)
        {
            List<OIAR_E> lista = new List<OIAR_E>();
            OIEQ_D oieqD = new OIEQ_D();
            string fil = string.Empty;
            string filjoin = string.Empty;
            if (filtro != null)
            {
                if (filtro.DocEntry > 0) { fil += " and t0.DocEntry=" + filtro.DocEntry; }
                if (filtro.DocEntryEqu > 0) { fil += " and t0.DocEntryEqu=" + filtro.DocEntryEqu; }
                if (filtro.DocEntryPer > 0) { fil += " and t0.DocEntryPer=" + filtro.DocEntryPer; }
                if (filtro.WhsCode != null) { fil += " and t0.WhsCode='" + filtro.WhsCode + "'"; }
                if (filtro.Piso > 0) { fil += " and t0.Piso=" + filtro.Piso; }
                if (filtro.ItemCode != null) { fil += " and t0.ItemCode like '%" + filtro.ItemCode + "%'"; }
                if (filtro.ItemName != null) { fil += " and t0.ItemName like '%" + filtro.ItemName + "%'"; }
                if (filtro.Estado != null) { fil += " and t0.Estado='" + filtro.Estado + "'"; }
                if (filtro.Fase > 0) { fil += " and t0.Fase between " + filtro.Fase + " and " + (filtro.Fase + 2); filjoin += " and t1.Fase between " + (filtro.Fase + 1) + " and " + (filtro.Fase + 2); }
                if (filtro.DetFases != null && filtro.DetFases.Count > 0 && filtro.DetFases[0].Operario != null) { fil += " and t1.Operario like '%" + filtro.DetFases[0].Operario + "%'"; }
            }
            if (user != null)
            {
                if (user.IdRol != 1 && user.IdRol != 2 && user.IdRol != 4)
                {
                    fil += " and (t1.Operario='" + $"{user.Nombres} {user.Apellidos}" + "' or t1.Operario is null)";

                    List<OIEQ_E> equiposDeUsuario = oieqD.buscarEquipoUsrPer(user, OIPE_E.PeriodoSeleccionado, new OIEQ_E { Tipo = tipo });
                    string Alms = "";
                    foreach (OIEQ_E obj in equiposDeUsuario)
                    {
                        foreach (IEQ2_E obj2 in obj.DetFabricantes)
                        {
                            Alms += "," + obj2.FirmCode;
                        }
                    }
                    fil += " and t0.FirmCode in(0" + Alms + ")";
                }

            }

            string query = "select top 50 t0.DocEntry,t0.DocEntryEqu,t0.WhsCode,t0.Piso,t0.ItemCode,t0.ItemName,t0.Estado,t0.Fase from al.oiar t0" +
                " left join al.iar1 t1 on t1.DocEntry=t0.DocEntry " + filjoin +
                " where t0.DocEntry>0 " + fil + " group by t0.DocEntry,t0.DocEntryEqu,t0.WhsCode,t0.Piso,t0.ItemCode,t0.ItemName,t0.Estado, t0.Fase " +
                    "order by t0.DocEntry desc";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    OIAR_E o = new OIAR_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.DocEntryEqu = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { o.WhsCode = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.Piso = dr.GetInt32(3); }
                    if (!dr.IsDBNull(4)) { o.ItemCode = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { o.ItemName = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { o.Estado = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { o.Fase = dr.GetInt32(7); }
                    o.DetFases = iar1D.buscarDetFases(o.DocEntry);
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
        public int Registrar(OIAR_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIAR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "A");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@DocEntryPer", obj.DocEntryPer);
                    cmd.Parameters.AddWithValue("@Propietario", obj.Propietario);
                    cmd.Parameters.AddWithValue("@DocEntryEqu", obj.DocEntryEqu);
                    cmd.Parameters.AddWithValue("@WhsCode", obj.WhsCode);
                    cmd.Parameters.AddWithValue("@Piso", obj.Piso);
                    cmd.Parameters.AddWithValue("@ItemCode", obj.ItemCode);
                    cmd.Parameters.AddWithValue("@ItemName", obj.ItemName);
                    cmd.Parameters.AddWithValue("@FirmCode", obj.FirmCode);
                    cmd.Parameters.AddWithValue("@NumInBuy", obj.NumInBuy);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    //post transacciones
                    SqlCommand cmd2 = new SqlCommand("POST_TRANSACCIONES", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "OIAR");
                    cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@DocEntry"].Value);
                    cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@DocEntry"].Value);
                    cmd2.ExecuteNonQuery();
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("El articulo ya fue registrado: " + e2.Message); }
            return status;
        }
        public OIAR_E Buscar(int DocEntry)
        {
            OIAR_E o = new OIAR_E();
            string query = "select * from al.oiar where DocEntry=" + DocEntry;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                dr.Read();
                if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { o.DocEntryPer = dr.GetInt32(1); }
                if (!dr.IsDBNull(2)) { o.Propietario = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { o.DocEntryEqu = dr.GetInt32(3); }
                if (!dr.IsDBNull(4)) { o.WhsCode = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { o.Piso = dr.GetInt32(5); }
                if (!dr.IsDBNull(6)) { o.ItemCode = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { o.Estado = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { o.Fase = dr.GetInt32(8); }
                if (!dr.IsDBNull(9)) { o.ItemName = dr.GetString(9); }
                if (!dr.IsDBNull(12)) { o.FechaRegistro = dr.GetDateTime(12).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(13)) { o.HoraRegistro = dr.GetTimeSpan(13).ToString(); }
                o.DetFases = iar1D.buscarDetFases(o.DocEntry);
                dr.Close();
            }
            catch { }
            return o;
        }
        public int IniciarConteo(OIAR_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIAR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UIC");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;

                    SqlParameter tbDet = new SqlParameter("@TPIAR1", SqlDbType.Structured);
                    tbDet.Value = IAR1_E.tbDetalle(obj.DetFases);
                    tbDet.TypeName = "al.TPIAR1";
                    cmd.Parameters.AddWithValue("@TPIAR1", tbDet.Value);

                    SqlParameter tbDet11 = new SqlParameter("@TPIAR11", SqlDbType.Structured);
                    tbDet11.Value = IAR11_E.tbDetalle(obj.DetFases[0].DetApoyos);
                    tbDet11.TypeName = "al.TPIAR11";
                    cmd.Parameters.AddWithValue("@TPIAR11", tbDet11.Value);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en proceso: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en proceso y conexion: " + e2.Message); }
            return status;
        }
        public int RevertirIniciarConteo(int DocEntry)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIAR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "URI");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry).Direction = ParameterDirection.InputOutput;

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en proceso: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en proceso y conexion: " + e2.Message); }
            return status;
        }
        public int TerminarConteo(OIAR_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIAR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UTC");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;

                    SqlParameter tbDet = new SqlParameter("@TPIAR1", SqlDbType.Structured);
                    tbDet.Value = IAR1_E.tbDetalle(obj.DetFases);
                    tbDet.TypeName = "al.TPIAR1";
                    cmd.Parameters.AddWithValue("@TPIAR1", tbDet.Value);

                    SqlParameter tbDet12 = new SqlParameter("@TPIAR12", SqlDbType.Structured);
                    tbDet12.Value = IAR12_E.tbDetalle(obj.DetFases[0].DetContab);
                    tbDet12.TypeName = "al.TPIAR12";
                    cmd.Parameters.AddWithValue("@TPIAR12", tbDet12.Value);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en proceso: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en proceso y conexion: " + e2.Message); }
            return status;
        }
        public int RevertirTerminoConteo(int DocEntry)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIAR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "URT");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry).Direction = ParameterDirection.InputOutput;

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en proceso: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en proceso y conexion: " + e2.Message); }
            return status;
        }
        public int IniciarReconteo(OIAR_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIAR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UIR");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;

                    SqlParameter tbDet = new SqlParameter("@TPIAR1", SqlDbType.Structured);
                    tbDet.Value = IAR1_E.tbDetalle(obj.DetFases);
                    tbDet.TypeName = "al.TPIAR1";
                    cmd.Parameters.AddWithValue("@TPIAR1", tbDet.Value);

                    SqlParameter tbDet11 = new SqlParameter("@TPIAR11", SqlDbType.Structured);
                    tbDet11.Value = IAR11_E.tbDetalle(obj.DetFases[0].DetApoyos);
                    tbDet11.TypeName = "al.TPIAR11";
                    cmd.Parameters.AddWithValue("@TPIAR11", tbDet11.Value);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en proceso: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en proceso y conexion: " + e2.Message); }
            return status;
        }
        public int RevertirIniciarReconteo(int DocEntry)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIAR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "RIR");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry).Direction = ParameterDirection.InputOutput;

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en proceso: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en proceso y conexion: " + e2.Message); }
            return status;
        }
        public int TerminarReconteo(OIAR_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIAR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UTR");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;

                    SqlParameter tbDet = new SqlParameter("@TPIAR1", SqlDbType.Structured);
                    tbDet.Value = IAR1_E.tbDetalle(obj.DetFases);
                    tbDet.TypeName = "al.TPIAR1";
                    cmd.Parameters.AddWithValue("@TPIAR1", tbDet.Value);

                    SqlParameter tbDet12 = new SqlParameter("@TPIAR12", SqlDbType.Structured);
                    tbDet12.Value = IAR12_E.tbDetalle(obj.DetFases[0].DetContab);
                    tbDet12.TypeName = "al.TPIAR12";
                    cmd.Parameters.AddWithValue("@TPIAR12", tbDet12.Value);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en proceso: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en proceso y conexion: " + e2.Message); }
            return status;
        }
        public int RevertirTerminoReconteo(int DocEntry)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIAR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "RTR");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry).Direction = ParameterDirection.InputOutput;

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en proceso: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en proceso y conexion: " + e2.Message); }
            return status;
        }
        public int IniciarAnalisisConteo(OIAR_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIAR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UIA");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;

                    SqlParameter tbDet = new SqlParameter("@TPIAR1", SqlDbType.Structured);
                    tbDet.Value = IAR1_E.tbDetalle(obj.DetFases);
                    tbDet.TypeName = "al.TPIAR1";
                    cmd.Parameters.AddWithValue("@TPIAR1", tbDet.Value);

                    SqlParameter tbDet11 = new SqlParameter("@TPIAR11", SqlDbType.Structured);
                    tbDet11.Value = IAR11_E.tbDetalle(obj.DetFases[0].DetApoyos);
                    tbDet11.TypeName = "al.TPIAR11";
                    cmd.Parameters.AddWithValue("@TPIAR11", tbDet11.Value);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en proceso: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en proceso y conexion: " + e2.Message); }
            return status;
        }
        public int RevertirIniciarAnalisisConteo(int DocEntry)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIAR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "RIA");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry).Direction = ParameterDirection.InputOutput;

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en proceso: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en proceso y conexion: " + e2.Message); }
            return status;
        }
        public int TerminarAnalisisConteo(OIAR_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIAR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UTA");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;

                    SqlParameter tbDet = new SqlParameter("@TPIAR1", SqlDbType.Structured);
                    tbDet.Value = IAR1_E.tbDetalle(obj.DetFases);
                    tbDet.TypeName = "al.TPIAR1";
                    cmd.Parameters.AddWithValue("@TPIAR1", tbDet.Value);

                    SqlParameter tbDet12 = new SqlParameter("@TPIAR12", SqlDbType.Structured);
                    tbDet12.Value = IAR12_E.tbDetalle(obj.DetFases[0].DetContab);
                    tbDet12.TypeName = "al.TPIAR12";
                    cmd.Parameters.AddWithValue("@TPIAR12", tbDet12.Value);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en proceso: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en proceso y conexion: " + e2.Message); }
            return status;
        }
        public int RevertirTerminoAnalisisConteo(int DocEntry)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIAR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "RTA");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry).Direction = ParameterDirection.InputOutput;

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en proceso: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en proceso y conexion: " + e2.Message); }
            return status;
        }

    }

}