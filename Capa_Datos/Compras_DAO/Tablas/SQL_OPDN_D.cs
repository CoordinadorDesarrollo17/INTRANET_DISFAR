using Capa_Entidad.Compras_ENT.Tablas;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Compras_DAO.Tablas
{
    public class SQL_OPDN_D
    {
        DBHelper db = new DBHelper();
        public SQL_OPDN_E buscarSqlEntradaDeMercancias(int DocEntry)
        {
            SQL_OPDN_E s = new SQL_OPDN_E();
            string query = "select * from cc.SQL_OPDN where DocEntry=" + DocEntry;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                dr.Read();
                if (!dr.IsDBNull(0)) { s.DocEntry = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { s.ObjType = dr.GetInt32(1); }
                if (!dr.IsDBNull(2)) { s.Estado = dr.GetString(2); }
				if (!dr.IsDBNull(3)) { s.FechaRealizacion = dr.GetDateTime(3).ToString("yyyy-MM-dd"); }
				if (!dr.IsDBNull(4)) { s.FechaRealizacion = dr.GetTimeSpan(4).ToString(); }
				if (!dr.IsDBNull(5)) { s.OpRealizacion = dr.GetString(5); }
				if (!dr.IsDBNull(6)) { s.Observaciones = dr.GetString(6); }

                dr.Close();
            }
            catch { }
            return s;
        }
        public int realizarSqlEntradaDeMercancias(SQL_OPDN_E s)
        {
            int status = -1;
            string query = "insert into cc.SQL_OPDN values (" + s.DocEntry+",'"+s.ObjType+ "','REALIZADO',(select convert(varchar,getdate(),23)), (select convert(char(5),getdate(),108)), '" +
                s.OpRealizacion+"','"+s.Observaciones+"')";
            try
            {
                db.ExecuteNonQueryTrxNoSp(query);
            }
            catch { throw new Exception("La entrada ya se encuentra realizada"); }
            return status;
        }
        public int eliminarSqlEntradaDeMercancias(int DocEntry)
        {
            int status = -1;
            string query = "delete from cc.SQL_OPDN where DocEntry=" + DocEntry;
            try
            {
                db.ExecuteNonQueryTrxNoSp(query);
            }
            catch { throw new Exception("ocurrio un error en la anulacion"); }
            return status;
        }
        public List<SQL_OPDN_E> listarSqlEntradaDeMercancias(SQL_OPDN_E fil)
        {
            List<SQL_OPDN_E> lista = new List<SQL_OPDN_E>();
            string filtros = "";
            if(fil!=null)
            {
                if (fil.Estado != null) { filtros += " and Estado='" + fil.Estado + "'"; }
            }
            string query = "select top 50 * from cc.SQL_OPDN where DocEntry>0 " + filtros;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while(dr.Read())
                {
                    SQL_OPDN_E s = new SQL_OPDN_E();
                    if (!dr.IsDBNull(0)) { s.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { s.ObjType = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { s.Estado = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { s.FechaRealizacion = dr.GetDateTime(3).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(4)) { s.FechaRealizacion = dr.GetTimeSpan(4).ToString(); }
                    if (!dr.IsDBNull(5)) { s.OpRealizacion = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { s.Observaciones = dr.GetString(6); }
                    lista.Add(s);
                }
                dr.Close();
            }
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
			return lista;
        }
    }
}
