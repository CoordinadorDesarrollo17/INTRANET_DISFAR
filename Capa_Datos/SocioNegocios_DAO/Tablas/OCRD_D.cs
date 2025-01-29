using Capa_Entidad.SocioNegocios_ENT.Tablas;
using System;
using System.Collections.Generic;
using Sap.Data.Hana;
using System.Data.SqlClient;
using System.Data;

namespace Capa_Datos.SocioNegocios_DAO.Tablas
{
    public class OCRD_D
    {
        DBHelper db = new DBHelper();
        Utilitarios uti = new Utilitarios();
        public List<OCRD_E> listarSociosDeNegocios(OCRD_E filtro)
        {
            List<OCRD_E> lista = new List<OCRD_E>();
            string fil = string.Empty;
            if(filtro!=null)
            {
                if (!string.IsNullOrWhiteSpace(filtro.CardName)) { fil += " and T0.\"CardName\" like '%" + filtro.CardName + "%'"; }
                if (!string.IsNullOrWhiteSpace(filtro.CardType)) { fil += " and T0.\"CardType\"='"+filtro.CardType+"'"; }
            }
            string query = "select T0.\"CardCode\",T0.\"CardName\",T0.\"CardType\",T0.\"GroupCode\",T0.\"Phone1\" " +
                           " from " + uti.schemaHana + "OCRD T0 where T0.\"CardCode\" is not null "+
                           fil+" order by T0.\"CardName\"";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    OCRD_E o = new OCRD_E();
                    if (!hdr.IsDBNull(0)) { o.CardCode = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { o.CardName = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { o.CardType = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { o.GroupCode = hdr.GetInt32(3); }
                    if (!hdr.IsDBNull(4)) { o.Phone1 = hdr.GetString(4); }
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public List<OCRD_E> listarSociosConContactos()
        {
            List<OCRD_E> lista = new List<OCRD_E>();
            string query = "select T0.\"CardCode\",T0.\"CardName\",T0.\"CardType\",T0.\"GroupCode\" "+
                           " from "+uti.schemaHana+"OCRD T0 inner join "+uti.schemaHana+"OCPR T1 on T1.\"CardCode\" = T0.\"CardCode\" "+
                           " group by T0.\"CardCode\",T0.\"CardName\",T0.\"CardType\",T0.\"GroupCode\" ";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while(hdr.Read())
                {
                    OCRD_E o = new OCRD_E();
                    if (!hdr.IsDBNull(0)) { o.CardCode = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { o.CardName = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { o.CardType = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { o.GroupCode = hdr.GetInt32(3); }
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
		public int Migrar()
		{
			int status = 0;
			List<OCRD_E> lista = new List<OCRD_E>();
			SqlConnection cn = new SqlConnection(uti.cadSql);
			try
			{
				cn.Open();

				try
				{
                    SqlCommand cmd = new SqlCommand("DELETE FROM dbo.OCRD WHERE CreateDate >= convert(char(10),getdate(),126)", cn); 
					cmd.CommandType = CommandType.Text;
					cmd.ExecuteNonQuery();
				}
				catch (Exception e) { cn.Close(); throw new Exception("Error en eliminacion: " + e.Message); }
				cn.Close();
			}
			catch (Exception e2) { cn.Close(); throw new Exception("Error en eliminacion y conexion: " + e2.Message); }
            string query1 = $@"INSERT INTO dbo.OCRD values(@CardCode, @CardName, @Address, @CreateDate);";
            string query2 = $@"SELECT 
                                x.""CardCode"" AS ""RUC"", 
                                x.""CardName"" AS ""NOMBRE"", 
                                x.""Address"" AS ""DIRECCION FISCAL"", 
                                TO_VARCHAR(x.""CreateDate"", 'YYYY-MM-DD') AS ""FECHA CREACION""
                            FROM {uti.schemaHana}OCRD x
                            WHERE TO_VARCHAR(x.""CreateDate"", 'YYYY-MM-DD') >= TO_VARCHAR(CURRENT_DATE, 'YYYY-MM-DD')
                            ORDER BY x.""CreateDate"" ASC";

            // DESCOMENTAR CUANDO SE QUIERA TRAER TODA LA TABLA COMPLETA SIN RESTRICCIONES "x.\"CardCode\" AS \"RUC\", x.\"CardName\" AS \"NOMBRE\", x.\"Address\" AS \"DIRECCION FISCAL\", TO_VARCHAR (x.\"CreateDate\", 'YYYY-MM-DD')  FROM " + uti.schemaHana + "OCRD x ORDER BY x.\"CreateDate\" ASC";
            try
            {
				HanaDataReader hdr = db.HanaExecuteReaderNoSp(query2);
				while (hdr.Read())
				{
					OCRD_E o = new OCRD_E();
					if (!hdr.IsDBNull(0)) { o.CardCode = hdr.GetString(0); }
					if (!hdr.IsDBNull(1)) { o.CardName = hdr.GetString(1); }
					if (!hdr.IsDBNull(2)) { o.Address = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { o.CreateDate = hdr.GetDateTime(3).ToString("yyyy-MM-dd"); }
                    lista.Add(o);
				}
				hdr.Close();
			}
			catch { }

			foreach (var obj in lista)
			{

				try
				{
					cn.Open();
					SqlTransaction tran = cn.BeginTransaction();
					try
					{
						SqlCommand cmd = new SqlCommand(query1, cn);
						cmd.Transaction = tran;
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.AddWithValue("@CardCode", obj.CardCode);
						cmd.Parameters.AddWithValue("@CardName", obj.CardName);
                        cmd.Parameters.AddWithValue("@Address", !string.IsNullOrWhiteSpace(obj.Address) ? obj.Address : "-");
                        cmd.Parameters.AddWithValue("@CreateDate", obj.CreateDate);
                        cmd.ExecuteNonQuery();
                        tran.Commit();
					}
					catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error en migracion: " + e.Message); }
					cn.Close();
				}
				catch (Exception e2) { cn.Close(); throw new Exception("Error en migracion y conexion: " + e2.Message); }

			}
			return status;
		}
		

	}
}
