using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class OIPE_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper(); IPE1_D ipe1D = new IPE1_D();
        IPE2_D ipe2D = new IPE2_D();
        public List<OIPE_E> ListarPeriodosInventario(OIPE_E filtro)
        {
            List<OIPE_E> lista = new List<OIPE_E>();
            string fil = string.Empty;
            if (filtro != null)
            {
                if (filtro.DocEntry > 0) { fil += " and DocEntry=" + filtro.DocEntry; }
                if (filtro.Descripcion != null) { fil += " and Descripcion like '%" + filtro.Descripcion + "%'"; }
                if (filtro.FecIni != null) { fil += " and FecIni = '" + filtro.FecIni + "'"; }
                if (filtro.FecFin != null) { fil += " and FecFin = '" + filtro.FecFin + "'"; }
                if (!string.IsNullOrWhiteSpace(filtro.FechaRegistro)) { fil += " and FechaRegistro = '" + filtro.FechaRegistro + "'"; }
                if (filtro.Estado != null) { fil += " and Estado like '%" + filtro.Estado + "%'"; }
            }
            string query = "select top 50 DocEntry,Descripcion,FecIni,FecFin,FechaRegistro,HoraRegistro,Estado from al.oipe " +
                "where DocEntry>0 " + fil + " order by DocEntry desc";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    OIPE_E o = new OIPE_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.Descripcion = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.FecIni = dr.GetDateTime(2).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(3)) { o.FecFin = dr.GetDateTime(3).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(4)) { o.FechaRegistro = dr.GetDateTime(4).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(5)) { o.HoraRegistro = dr.GetTimeSpan(5).ToString(); }
                    if (!dr.IsDBNull(6)) { o.Estado = dr.GetString(6); }
                    o.DetAlmacenes = ipe1D.buscarDetallesAlmacenes(o.DocEntry);
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
        public int Registrar(OIPE_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIPE", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "A");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion.TrimEnd());
                    cmd.Parameters.AddWithValue("@FecIni", Convert.ToDateTime(obj.FecIni));
                    cmd.Parameters.AddWithValue("@FecFin", Convert.ToDateTime(obj.FecFin));
                    cmd.Parameters.AddWithValue("@Observaciones", obj.Observaciones);
                    cmd.Parameters.AddWithValue("@Propietario", obj.Propietario);

                    SqlParameter tbDet = new SqlParameter("@TPIPE1", SqlDbType.Structured);
                    tbDet.Value = IPE1_E.tbDetalle(obj.DetAlmacenes);
                    tbDet.TypeName = "al.TPIPE1";
                    cmd.Parameters.AddWithValue("@TPIPE1", tbDet.Value);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    //post transacciones
                    SqlCommand cmd2 = new SqlCommand("dbo.POST_TRANSACCIONES", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "OIPE");
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
        public OIPE_E Buscar(int DocEntry, bool ConArticulos)
        {
            OIPE_E o = new OIPE_E();
            string query = "select * from al.oipe where DocEntry=" + DocEntry;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                dr.Read();
                if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { o.Descripcion = dr.GetString(1); }
                if (!dr.IsDBNull(2)) { o.FecIni = dr.GetDateTime(2).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(3)) { o.FecFin = dr.GetDateTime(3).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(4)) { o.Observaciones = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { o.Propietario = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { o.FechaRegistro = dr.GetDateTime(6).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(7)) { o.HoraRegistro = dr.GetTimeSpan(7).ToString(); }
                if (!dr.IsDBNull(8)) { o.Estado = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { o.OpCarga = dr.GetString(9); }
                if (!dr.IsDBNull(10)) { o.FechaCarga = dr.GetDateTime(10).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(11)) { o.HoraCarga = dr.GetTimeSpan(11).ToString(); }
                if (!dr.IsDBNull(12)) { o.EstadoDatos = dr.GetString(12); }

                o.DetAlmacenes = ipe1D.buscarDetallesAlmacenes(o.DocEntry);
                if (ConArticulos) { o.DetArticulos = ipe2D.buscarDetallesArticulos(o.DocEntry); }
                dr.Close();
            }
            catch { }
            return o;
        }
        public int Seleccionar(OIPE_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIPE", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "USL");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Operario", obj.Operario);
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                    OIPE_E.PeriodoSeleccionado = Buscar(status, false);
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en seleccion: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en seleccion y conexion: " + e2.Message); }
            return status;
        }
        public int Editar(OIPE_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIPE", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UED");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("@FecIni", Convert.ToDateTime(obj.FecIni));
                    cmd.Parameters.AddWithValue("@FecFin", Convert.ToDateTime(obj.FecFin));
                    cmd.Parameters.AddWithValue("@Observaciones", obj.Observaciones);
                    cmd.Parameters.AddWithValue("@Operario", obj.Operario);
                    SqlParameter tbDet = new SqlParameter("@TPIPE1", SqlDbType.Structured);
                    tbDet.Value = IPE1_E.tbDetalle(obj.DetAlmacenes);
                    tbDet.TypeName = "al.TPIPE1";
                    cmd.Parameters.AddWithValue("@TPIPE1", tbDet.Value);

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
        public int Cerrar(OIPE_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIPE", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UCE");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Operario", obj.Operario);
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en cierre: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en cierre y conexion: " + e2.Message); }
            return status;
        }
        public int RevertirCerrar(int DocEntry, string Operario)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIPE", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "URC");
                    cmd.Parameters.AddWithValue("@DocEntry", DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Operario", Operario);
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en  revertir cierre: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en revertir cierre y conexion: " + e2.Message); }
            return status;
        }
        public int MigrarArticulos(OIPE_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                obj.DetArticulos = ipe2D.listarArticulosLotes(Buscar(obj.DocEntry, false),
                    new IPE2_E { Quantity = 1 });
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIPE", cn)
                    {
                        Transaction = tran,
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UMA");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Operario", obj.Operario);
                    SqlParameter tbDet = new SqlParameter("@TPIPE2", SqlDbType.Structured);
                    tbDet.Value = IPE2_E.tbDetalle(obj.DetArticulos);
                    tbDet.TypeName = "al.TPIPE2";
                    cmd.Parameters.AddWithValue("@TPIPE2", tbDet.Value);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en migracion: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en migracion y conexion: " + e2.Message); }
            return status;
        }
        public int CargarArticulosMigrados(OIPE_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_OIPE", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "UCD");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@OpCarga", obj.OpCarga);
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en carga: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en carga y conexion: " + e2.Message); }
            return status;
        }
    }
}