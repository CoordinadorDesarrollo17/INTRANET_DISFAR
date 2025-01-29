using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class OIEQ_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper(); OIPE_D oipeD = new OIPE_D();
        public List<OIEQ_E> listarEquipos(OIEQ_E filtro)
        {
            List<OIEQ_E> lista = new List<OIEQ_E>();
            string fil = "";
            if (filtro != null)
            {
                if (filtro.DocEntry > 0) { fil += " and DocEntry=" + filtro.DocEntry; }
                if (filtro.Nombre != null) { fil += " and Nombre like '%" + filtro.Nombre + "%'"; }
                if (filtro.DescripcionPeriodo != null)
                {
                    List<OIPE_E> l = oipeD.ListarPeriodosInventario(new OIPE_E() { Descripcion = filtro.DescripcionPeriodo });
                    string auxLista = "0";
                    foreach (OIPE_E auxoipe in l)
                    {
                        auxLista += "," + auxoipe.DocEntry;
                    }
                    fil += " and DocEntryPer in(" + auxLista + ")";
                }
                if (filtro.WhsCode != null) { fil += " and WhsCode = '" + filtro.WhsCode + "'"; }
                if (filtro.Piso > 0) { fil += " and Piso=" + filtro.Piso; }
                if (filtro.Estado != null) { fil += " and Estado like '%" + filtro.Estado + "%'"; }
                if (!string.IsNullOrWhiteSpace(filtro.FechaRegistro)) { fil += " and FechaRegistro= '" + filtro.FechaRegistro + "'"; }
                if (filtro.DocEntryPer > 0) { fil += " and DocEntryPer=" + filtro.DocEntryPer; }
            }
            string query = "select top 50 DocEntry,Nombre,DocEntryPer,WhsCode,Piso,Estado,FechaRegistro, HoraRegistro from al.oieq " +
                "where DocEntry>0 " + fil + " order by DocEntry desc";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    OIEQ_E o = new OIEQ_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.Nombre = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.DocEntryPer = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { o.WhsCode = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.Piso = dr.GetInt32(4); }
                    if (!dr.IsDBNull(5)) { o.Estado = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { o.FechaRegistro = dr.GetDateTime(6).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(7)) { o.HoraRegistro = dr.GetTimeSpan(7).ToString(); }
                    o.DescripcionPeriodo = oipeD.Buscar(o.DocEntryPer, false).Descripcion;
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
        public OIEQ_E buscarEquipos(int DocEntry)
        {
            OIEQ_E o = new OIEQ_E();
            IEQ1_D ieq1D = new IEQ1_D(); IEQ2_D ieq2D = new IEQ2_D();
            string query = "select * from al.oieq where DocEntry=" + DocEntry;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                dr.Read();
                if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { o.DocEntryPer = dr.GetInt32(1); }
                if (!dr.IsDBNull(2)) { o.Nombre = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { o.WhsCode = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { o.Piso = dr.GetInt32(4); }
                if (!dr.IsDBNull(5)) { o.Estado = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { o.Tipo = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { o.Propietario = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { o.Controlados = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { o.FechaRegistro = dr.GetDateTime(9).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(10)) { o.HoraRegistro = dr.GetTimeSpan(10).ToString(); }

                o.DetMiembros = ieq1D.buscarDetallesMiembros(o.DocEntry);
                o.DetFabricantes = ieq2D.buscarDetallesFabricantes(o.DocEntry);
                dr.Close();

            }
            catch { }
            return o;
        }
        public List<OIEQ_E> buscarEquipoUsrPer(Usuario_E user, OIPE_E per, OIEQ_E obj)
        {
            List<OIEQ_E> lista = new List<OIEQ_E>();
            string fil = "";
            if (obj != null)
            {
                fil += " and t0.Tipo='" + obj.Tipo + "'";
            }
            string query = "select t0.DocEntry from al.oieq t0" +
                            " inner join al.ieq1 t1 on t1.DocEntry = t0.DocEntry" +
                            " where t0.DocEntryPer = " + per.DocEntry + " and t1.Id = '" + user.Prefijo + user.Id + "' and t0.Estado='Abierto' " + fil +
                            " order by t0.DocEntryPer desc";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    OIEQ_E o = buscarEquipos(dr.GetInt32(0));
                    lista.Add(o);
                }

                dr.Close();
            }
            catch { }
            return lista;
        }
        public List<OIEQ_E> buscarPertenenciaEquipo(Usuario_E user, OIPE_E per, string equ)
        {
            List<OIEQ_E> lista = new List<OIEQ_E>();

            string query = "select t0.DocEntry from al.oieq t0 inner join al.ieq1 t1 on t1.DocEntry = t0.DocEntry" +
                               " where t0.DocEntryPer = " + per.DocEntry + "  and t0.Tipo = '" + equ + "' and t1.Nombre = '" + user.Nombres + " " + user.Apellidos + "' and t0.Estado = 'Abierto' order by t0.DocEntryPer desc";
            try
            {
                OIEQ_E o = new OIEQ_E();
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                dr.Read();
                if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                lista.Add(o);
                dr.Close();
            }
            catch { throw new Exception("Error en ingreso no tiene accesos"); }
            return lista;
        }
        public int separarEquipo(OIEQ_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIEQ", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "AS");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@DocEntryPer", obj.DocEntryPer);
                    cmd.Parameters.AddWithValue("@Tipo", obj.Tipo);
                    cmd.Parameters.AddWithValue("@Propietario", obj.Propietario);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    //post transacciones
                    SqlCommand cmd2 = new SqlCommand("POST_TRANSACCIONES", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "OIEQ");
                    cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@DocEntry"].Value);
                    cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@DocEntry"].Value);
                    cmd2.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: " + e.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return status;
        }
        public int registrarNuevoEquipo(OIEQ_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIEQ", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UA");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("@WhsCode", obj.WhsCode);
                    cmd.Parameters.AddWithValue("@Piso", obj.Piso);
                    cmd.Parameters.AddWithValue("@Estado", obj.Estado);
                    cmd.Parameters.AddWithValue("@Controlados", obj.Controlados);

                    SqlParameter tbDet = new SqlParameter("@TPIEQ1", SqlDbType.Structured);
                    tbDet.Value = IEQ1_E.tbDetalle(obj.DetMiembros);
                    tbDet.TypeName = "al.TPIEQ1";
                    cmd.Parameters.AddWithValue("@TPIEQ1", tbDet.Value);

                    SqlParameter tbDet2 = new SqlParameter("@TPIEQ2", SqlDbType.Structured);
                    tbDet2.Value = IEQ2_E.tbDetalle(obj.DetFabricantes);
                    tbDet2.TypeName = "al.TPIEQ2";
                    cmd.Parameters.AddWithValue("@TPIEQ2", tbDet2.Value);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return status;
        }
        public int editarEquipo(OIEQ_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIEQ", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UED");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@DocEntryPer", obj.DocEntryPer);
                    cmd.Parameters.AddWithValue("@Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("@WhsCode", obj.WhsCode);
                    cmd.Parameters.AddWithValue("@Piso", obj.Piso);
                    cmd.Parameters.AddWithValue("@Estado", obj.Estado);
                    cmd.Parameters.AddWithValue("@Tipo", obj.Tipo);
                    cmd.Parameters.AddWithValue("@Controlados", obj.Controlados);

                    SqlParameter tbDet = new SqlParameter("@TPIEQ1", SqlDbType.Structured);
                    tbDet.Value = IEQ1_E.tbDetalle(obj.DetMiembros);
                    tbDet.TypeName = "al.TPIEQ1";
                    cmd.Parameters.AddWithValue("@TPIEQ1", tbDet.Value);

                    SqlParameter tbDet2 = new SqlParameter("@TPIEQ2", SqlDbType.Structured);
                    tbDet2.Value = IEQ2_E.tbDetalle(obj.DetFabricantes);
                    tbDet2.TypeName = "al.TPIEQ2";
                    cmd.Parameters.AddWithValue("@TPIEQ2", tbDet2.Value);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en edicion: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en edicion y conexion: " + e2.Message); }
            return status;
        }
        public int eliminarEquipo(int DocEntry)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIEQ", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "DEL");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en eliminacion: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en eliminacion y conexion: " + e2.Message); }
            return status;
        }
    }
}