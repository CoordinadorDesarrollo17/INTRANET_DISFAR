using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class OTRC_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        //Revisado
        public void registrarTransaccion(OTRC_E o, SqlTransaction tran)
        {
            try { 
                using (SqlCommand cmd = new SqlCommand("vt.MANT_OTRC", tran.Connection, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "A");
                    cmd.Parameters.AddWithValue("@IdReg", o.IdReg);
                    cmd.Parameters.AddWithValue("@RegName", o.RegName);
                    cmd.Parameters.AddWithValue("@CardCode", o.CardCode);
                    cmd.Parameters.AddWithValue("@CardName", o.CardName);
                    cmd.Parameters.AddWithValue("@Sentido", o.Sentido);
                    cmd.Parameters.AddWithValue("@Detalle", o.Detalle);
                    cmd.Parameters.AddWithValue("@Cantidad", o.Cantidad);
                    cmd.Parameters.AddWithValue("@Imputado", o.Imputado);
                    cmd.Parameters.AddWithValue("@Operario", o.Operario);
                
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error en registro: " + e.Message);
            }
        }

        public void registrarTransaccionDataTable(DataTable tablaDatos2, SqlTransaction tran)
        {
            try
            {

                using (SqlCommand cmd = new SqlCommand("vt.MANT_OTRC", tran.Connection, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120;

                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "AA");
                    SqlParameter param = cmd.Parameters.AddWithValue("@TablaDatos", tablaDatos2);
                    param.SqlDbType = SqlDbType.Structured;

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error en registro: " + e.Message);
            }
        }

        public List<OTRC_E.RptTransacciones> listarTransacciones(OTRC_E filtro)
        {
            List<OTRC_E.RptTransacciones> lista = new List<OTRC_E.RptTransacciones>();
            string fil = "";
            string query = "";
            string top = " top 50";
            if (filtro != null)
            {
                if (filtro.IdReg > 0) { fil += " and IdReg=" + filtro.IdReg; }
                if (filtro.TiempoIni.Year > 1800 && filtro.TiempoFin.Year > 1800)
                { fil += " and FechaRegistro between '" + filtro.TiempoIni.ToString("yyyy-MM-dd") + "' and '" + filtro.TiempoFin.ToString("yyyy-MM-dd") + "'"; top = ""; }
            }
            query = "select " + top + " Id,IdReg,RegName,CardCode, CardName, Sentido,Detalle,Cantidad,Imputado,Operario,FechaRegistro,HoraRegistro from vt.OTRC where Id>0 " + fil + " order by Id desc";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    OTRC_E.RptTransacciones o = new OTRC_E.RptTransacciones();
                    if (!dr.IsDBNull(0)) { o.Id = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.IdReg = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { o.RegName = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.CardCode = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.CardName = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { o.Sentido = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { o.Detalle = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { o.Cantidad = dr.GetDecimal(7); }
                    if (!dr.IsDBNull(8)) { o.Imputado = dr.GetDecimal(8); }
                    if (!dr.IsDBNull(9)) { o.Operario = dr.GetString(9); }
                    if (!dr.IsDBNull(10)) { o.FechaRegistro = dr.GetDateTime(10).ToString("dd/MM/yyyy"); }
                    if (!dr.IsDBNull(11)) { o.HoraRegistro = dr.GetTimeSpan(11).ToString(); }
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
    }
}