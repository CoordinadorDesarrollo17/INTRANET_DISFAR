using Capa_Entidad.TI_Sistemas_ENT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.TI_Sistemas_DAO
{
    public class OTSO_D
    {
        readonly Utilitarios uti = new Utilitarios(); readonly DBHelper db = new DBHelper();
        public List<OTSO_E> listarTicketsSoporte(OTSO_E obj)
        {
            List<OTSO_E> lista = new List<OTSO_E>();
            string query;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                if (obj != null)
                {
                    query = "select top 10 DocEntry,Titulo,Estado,OpAsignado,Prioridad,TiempoRegistro,Propietario," +
                       "TiempoAtencion,Area from ti.otso where DocEntry>0 ";
                    if (obj.DocEntry > 0) { query += " and DocEntry=" + obj.DocEntry; }
                    if (obj.Titulo != null) { query += " and Titulo like '%" + obj.Titulo + "%'"; }
                    if (obj.Estado != null) { query += " and Estado='" + obj.Estado + "'"; }
                    if (obj.OpAsignado != null) { query += " and OpAsignado ='" + obj.OpAsignado + "'"; }
                    if (obj.Prioridad != null) { query += " and Prioridad='" + obj.Prioridad + "'"; }
                    if (obj.Area != null) { query += " and Area like '%" + obj.Area + "%'"; }
                    query += " order by 1 DESC";
                }
                else
                {
                    query = "select top 10 DocEntry,Titulo,Estado,OpAsignado,Prioridad,TiempoRegistro,Propietario," +
                           "TiempoAtencion,Area from ti.otso where DocEntry>0 order by 1 DESC";
                }
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    OTSO_E o = new OTSO_E() { };
                    o.DocEntry = dr.GetInt32(0);
                    if (!dr.IsDBNull(1)) { o.Titulo = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.Estado = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.OpAsignado = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.Prioridad = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { o.TiempoRegistro = dr.GetDateTime(5); }
                    if (!dr.IsDBNull(6)) { o.Propietario = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { o.TiempoAtencion = dr.GetDateTime(7); }
                    if (!dr.IsDBNull(8)) { o.Area = dr.GetString(8); }

                    lista.Add(o);
                }
                dr.Close();
                cn.Close();
            }
            catch { cn.Close(); }

            return lista;
        }
        public int ticketsVencidos()
        {
            string query; int num;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            query = "select count(*) from ti.otso where Estado='CREADO' AND CONVERT(varchar, TiempoRegistro,1) < CONVERT(varchar, GETDATE(),1) ";
            cn.Open();
            SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            num = dr.GetInt32(0);
            dr.Close();
            cn.Close();
            return num;
        }
        public int ticketsUrgentes()
        {
            string query; int num;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            query = "select count(*) from ti.otso where Prioridad='URGENTE' and Estado !='ATENDIDO' ";
            cn.Open();
            SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            num = dr.GetInt32(0);
            dr.Close();
            cn.Close();
            return num;
        }
        public int ticketsNoAtendidos()
        {
            string query; int num;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            query = "select count(*) from ti.otso where Estado='CREADO'  AND cast(TiempoRegistro as date) = CAST(GETDATE() AS date) ";
            cn.Open();
            SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            num = dr.GetInt32(0);
            dr.Close();
            cn.Close();
            return num;

        }
        public int ticketsAtendidos()
        {
            string query; int num;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            query = "select count(*) from ti.otso where Estado='ATENDIDO' ";
            cn.Open();
            SqlCommand cmd = new SqlCommand(query, cn) { CommandType = CommandType.Text };
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            num = dr.GetInt32(0);
            dr.Close();
            cn.Close();
            return num;

        }
        public string buscarRutaArchivo(int DocEntry)
        {
            OTSO_E o = new OTSO_E();
            string query = "select Adjunto from TI.OTSO where DocEntry =" + DocEntry;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                dr.Read();
                if (!dr.IsDBNull(0)) { o.Adjunto = dr.GetString(0); }
                dr.Close();

            }
            catch { }
            return o.Adjunto;
        }
        public OTSO_E buscarTicketSoporte(int DocEntry)
        {
            OTSO_E o = new OTSO_E();
            string query = "select * from ti.otso where DocEntry=@DocEntry";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string>() { "@DocEntry" }, DocEntry);
                dr.Read();

                o.DocEntry = dr.GetInt32(0);
                if (!dr.IsDBNull(1)) { o.Estado = dr.GetString(1); }
                if (!dr.IsDBNull(2)) { o.Titulo = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { o.Descripcion = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { o.Prioridad = dr.GetString(4); }
                if (!dr.IsDBNull(5)) { o.Propietario = dr.GetString(5); }
                if (!dr.IsDBNull(6)) { o.Contacto = dr.GetString(6); }
                if (!dr.IsDBNull(7)) { o.Area = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { o.Sede = dr.GetString(8); }
                if (!dr.IsDBNull(9)) { o.Asistencia = dr.GetString(9); }
                if (!dr.IsDBNull(10)) { o.Solucion = dr.GetString(10); }
                if (!dr.IsDBNull(11)) { o.Tipo = dr.GetString(11); }
                if (!dr.IsDBNull(12)) { o.Categoria = dr.GetString(12); }
                if (!dr.IsDBNull(13)) { o.Subcategoria = dr.GetString(13); }
                if (!dr.IsDBNull(14)) { o.Adjunto = dr.GetString(14); }
                if (!dr.IsDBNull(15)) { o.TiempoRegistro = dr.GetDateTime(15); }
                if (!dr.IsDBNull(16)) { o.OpAsignacion = dr.GetString(16); }
                if (!dr.IsDBNull(17)) { o.IdAsignado = dr.GetString(17); }
                if (!dr.IsDBNull(18)) { o.OpAsignado = dr.GetString(18); }
                if (!dr.IsDBNull(19)) { o.TiempoAsignacion = dr.GetDateTime(19); }
                if (!dr.IsDBNull(20)) { o.OpAtencion = dr.GetString(20); }
                if (!dr.IsDBNull(21)) { o.TiempoAtencion = dr.GetDateTime(21); }
                dr.Close();
            }
            catch { }
            return o;
        }
        public int registrarTicketSoporte(OTSO_E obj)
        {
            int status = -1;
            string rutaDirectorio = uti.directorioFileServer + "TI_Sistemas";
            if (obj.Archivo != null) { obj.Adjunto = obj.Archivo.FileName; }

            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("ti.MANT_OTSO", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "A");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Titulo", obj.Titulo);
                    cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("@Prioridad", obj.Prioridad);
                    cmd.Parameters.AddWithValue("@Propietario", obj.Propietario);
                    cmd.Parameters.AddWithValue("@Contacto", obj.Contacto);
                    cmd.Parameters.AddWithValue("@Area", obj.Area);
                    cmd.Parameters.AddWithValue("@Sede", obj.Sede);
                    cmd.Parameters.AddWithValue("@Asistencia", obj.Asistencia);
                    cmd.Parameters.AddWithValue("@Solucion", obj.Solucion);
                    cmd.Parameters.AddWithValue("@Tipo", obj.Tipo);
                    cmd.Parameters.AddWithValue("@Categoria", obj.Categoria);
                    cmd.Parameters.AddWithValue("@Subcategoria", obj.Subcategoria);
                    cmd.Parameters.AddWithValue("@Adjunto", obj.Adjunto);
                    /*cmd.Parameters.AddWithValue("@OpAsignacion", obj.OpAsignacion);
                    cmd.Parameters.AddWithValue("@IdAsignado", obj.IdAsignado);
                    cmd.Parameters.AddWithValue("@OpAsignado", obj.OpAsignado);
                    cmd.Parameters.AddWithValue("@TiempoAsignacion", obj.TiempoAsignacion);
                    cmd.Parameters.AddWithValue("@OpAtencion", obj.OpAtencion);
                    cmd.Parameters.AddWithValue("@TiempoAtencion", obj.TiempoAtencion);*/


                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());
                    //post transacciones
                    SqlCommand cmd2 = new SqlCommand("POST_TRANSACCIONES", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "OTSO");
                    cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@DocEntry"].Value);
                    cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@DocEntry"].Value);
                    cmd2.ExecuteNonQuery();
                    if (obj.Archivo != null)
                    {
                        Directory.CreateDirectory(rutaDirectorio + @"\" + status);
                        obj.Archivo.SaveAs(rutaDirectorio + @"\" + status + @"\" + obj.Adjunto);
                    }
                    tran.Commit();
                }
                catch  { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return status;
        }
        public int editarTicketSoporte(OTSO_E obj)
        {
            int status = -1;
            string rutaDirectorio = uti.directorioFileServer + "TI_Sistemas";
            if (obj.Archivo != null) { obj.Adjunto = obj.Archivo.FileName; }
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("ti.MANT_OTSO", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "U");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("@Prioridad", obj.Prioridad);
                    cmd.Parameters.AddWithValue("@Contacto", obj.Contacto);
                    cmd.Parameters.AddWithValue("@Area", obj.Area);
                    cmd.Parameters.AddWithValue("@Sede", obj.Sede);
                    cmd.Parameters.AddWithValue("@Asistencia", obj.Asistencia);
                    cmd.Parameters.AddWithValue("@Tipo", obj.Tipo);
                    cmd.Parameters.AddWithValue("@Categoria", obj.Categoria);
                    cmd.Parameters.AddWithValue("@Subcategoria", obj.Subcategoria);
                    cmd.Parameters.AddWithValue("@Adjunto", obj.Adjunto);

                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@DocEntry"].Value.ToString());

                    if (obj.Archivo != null)
                    {
                        Directory.CreateDirectory(rutaDirectorio + @"\" + status);
                        obj.Archivo.SaveAs(rutaDirectorio + @"\" + status + @"\" + obj.Adjunto);
                    }
                    tran.Commit();
                }
                catch { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: "); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); status = 0; throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return status;
        }
        public int asignarTicketSoporte(OTSO_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("ti.MANT_OTSO", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "AS");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@OpAsignado", obj.OpAsignado);
                    cmd.Parameters.AddWithValue("@OpAsignacion", obj.OpAsignacion);
                    cmd.Parameters.AddWithValue("@IdAsignado", obj.IdAsignado);


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
        public int atenderTicketSoporte(OTSO_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("ti.MANT_OTSO", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "AT");
                    cmd.Parameters.AddWithValue("@DocEntry", obj.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Solucion", obj.Solucion);
                    cmd.Parameters.AddWithValue("@OpAtencion", obj.OpAtencion);

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

        public int obtenerEstadoOperario(string Id)
        {
            int c = 0;
            string query = "select count(*) from ti.OTSO where Estado='ASIGNADO' and IdAsignado ='" + Id + "'";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                dr.Read();
                if (!dr.IsDBNull(0)) { c = dr.GetInt32(0); }
                dr.Close();

            }
            catch { }
            return c;
        }

    }
}

