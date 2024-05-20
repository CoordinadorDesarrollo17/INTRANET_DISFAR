using Capa_Entidad.Seguridad_ENT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Capa_Datos.Seguridad_DAO
{
	public class USR2_D
	{
		//intentos rol usuario
		Utilitarios uti = new Utilitarios();
		public USR2_E buscarIntenRolUsu(int DocEntry, int IdOperacion)
		{
			USR2_E obj = new USR2_E();
			string query = "select * from dbo.USR2 where DocEntry=@DocEntry and IdOperacion=@IdOperacion";

			using (SqlConnection cn = new SqlConnection(uti.cadSql))
			{
				cn.Open();
				try
				{
					SqlCommand cmd = new SqlCommand(query, cn);
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
					cmd.Parameters.AddWithValue("@IdOperacion", IdOperacion);
					SqlDataReader dr = cmd.ExecuteReader();

					dr.Read();
					if (!dr.IsDBNull(0)) { obj.DocEntry = dr.GetInt32(0); }
					if (!dr.IsDBNull(1)) { obj.IdOperacion = dr.GetInt32(1); }
					if (!dr.IsDBNull(2)) { obj.Intentos = dr.GetInt32(2); }
					if (!dr.IsDBNull(3)) { obj.UsosDia = dr.GetInt32(3); }
					if (!dr.IsDBNull(4)) { obj.Dia = dr.GetDateTime(4); }
					dr.Close();
				}
				catch (Exception e)
				{
					throw new Exception(e.Message);
				}

				cn.Close();
			}


			return obj;
		}
		public int reiniciarUsos(int DocEntry, int IdOperacion)
		{
			int status = -1;
			string query = "update usr2 set UsosDia=Intentos,Dia=getdate() where DocEntry=@DocEntry and IdOperacion=@IdOperacion";
			SqlConnection cn = new SqlConnection(uti.cadSql);
			try
			{
				cn.Open();
				SqlTransaction tran = cn.BeginTransaction("transaccion1");
				try
				{
					SqlCommand cmd = new SqlCommand(query, cn);
					cmd.Transaction = tran; cmd.CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
					cmd.Parameters.AddWithValue("@IdOperacion", IdOperacion);
					status = cmd.ExecuteNonQuery();
					tran.Commit();
				}
				catch { tran.Rollback(); throw new Exception(""); }
				cn.Close();
			}
			catch { cn.Close(); status = -1; }
			return status;
		}
		public void UsarIntento(int DocEntry, int IdOperacion)
		{
			string query = "update usr2 set UsosDia=UsosDia-1 where DocEntry=@DocEntry and IdOperacion=@IdOperacion";
			SqlConnection cn = new SqlConnection(uti.cadSql);
			try
			{
				cn.Open();
				SqlTransaction tran = cn.BeginTransaction("transaccion1");
				try
				{
					SqlCommand cmd = new SqlCommand(query, cn);
					cmd.Transaction = tran; cmd.CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
					cmd.Parameters.AddWithValue("@IdOperacion", IdOperacion);
					cmd.ExecuteNonQuery();
					tran.Commit();
				}
				catch { tran.Rollback(); throw new Exception(""); }
				cn.Close();
			}
			catch { cn.Close(); throw new Exception("Error al registrar intento"); }
		}
	}
}
