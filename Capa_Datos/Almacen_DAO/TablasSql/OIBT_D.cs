using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;
using Sap.Data.Hana;
using System.Data.SqlClient;
using System.Data;
using System.Timers;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
	public class OIBT_D
	{
		Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public static Timer aTimer;
        public static int secondsCount = 0;
        //stock de lotes
        public List<OIBT_E> ListarArticulosLotes(OIBT_E filtro = null)
		{
			List<OIBT_E> lista = new List<OIBT_E>();
			string fil = string.Empty,query = string.Empty;
			if (filtro == null)
			{
				query = "select top 50 \"ItemCode\",\"BatchNum\",\"WhsCode\",\"ItemName\",TO_CHAR(\"ExpDate\", 'YYYY-MM-DD'), \"Quantity\" " +
				" from " + uti.schemaHana + "OIBT where \"ItemCode\" is not null order by \"ItemCode\"";
			}
			else
			{
				if (filtro.Quantity > 0) { fil += " and \"Quantity\">0"; }
				if (!string.IsNullOrEmpty(filtro.WhsCode)) { fil += " and \"WhsCode\"='" + filtro.WhsCode + "'"; }
				if (!string.IsNullOrEmpty(filtro.ItemCode)) { fil += " and \"ItemCode\"='" + filtro.ItemCode + "'"; }
				if (!string.IsNullOrEmpty(filtro.BatchNum)) { fil += " and \"BatchNum\"='" + filtro.BatchNum + "'"; }
				query = "select top 500 \"ItemCode\",\"BatchNum\",\"WhsCode\",\"ItemName\", TO_CHAR(\"ExpDate\", 'YYYY-MM-DD'), \"Quantity\" " +
				" from " + uti.schemaHana + "OIBT where \"ItemCode\" is not null " + fil + " order by \"ItemCode\"";
			}
			try
			{
				HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
				while (hdr.Read())
				{
					OIBT_E o = new OIBT_E();
					if (!hdr.IsDBNull(0)) { o.ItemCode = hdr.GetString(0); }
					if (!hdr.IsDBNull(1)) { o.BatchNum = hdr.GetString(1); }
					if (!hdr.IsDBNull(2)) { o.WhsCode = hdr.GetString(2); }
					if (!hdr.IsDBNull(3)) { o.ItemName = hdr.GetString(3); }
					if (!hdr.IsDBNull(4)) { o.ExpDate = hdr.GetString(4); }
					if (!hdr.IsDBNull(5)) { o.Quantity = hdr.GetDecimal(5); }
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
			List<OIBT_E> lista = new List<OIBT_E>();
			SqlConnection cn = new SqlConnection(uti.cadSql);
			try
			{
				cn.Open();

				try
				{
					SqlCommand cmd = new SqlCommand("DELETE FROM al.OIBT", cn);
					cmd.CommandType = CommandType.Text;
					cmd.ExecuteNonQuery();
				}
				catch (Exception e) { cn.Close(); throw new Exception("Error en eliminacion: " + e.Message); }
				cn.Close();
			}
			catch (Exception e2) { cn.Close(); throw new Exception("Error en eliminacion y conexion: " + e2.Message); }

			string query1 = "insert into al.OIBT values(@Laboratorio,@ItemCode,@ItemName,@BatchNum,@ExpDate,@Quantity,@WhsCode, @Price);";

			string query2 = "select top 10 " +
									"y.\"U_SYP_DESC\" AS \"LABORATORIO\"," +
									"x.\"ItemCode\" AS \"COD PRODUCTO\"," +
									"x.\"ItemName\" AS \"NOM PRODUCTO\"," +
									"x.\"BatchNum\" AS \"LOTE\"," +
									"TO_CHAR(x.\"ExpDate\", 'YYYY-MM-DD') AS \"FECHA VENC\"," +
									"x.\"Quantity\" AS \"STOCK\"," +
									"x.\"WhsCode\" AS \"ALM UBICACION\"," +
									"(SELECT i1.\"Price\" FROM " + uti.schemaHana + "ITM1 i1 WHERE i1.\"ItemCode\" =  x.\"ItemCode\" AND i1.\"PriceList\" = 1 ) AS \"PRECIO\"" +
									" from " + uti.schemaHana + "OIBT x" +
									" INNER JOIN " + uti.schemaHana + "OITM z ON x.\"ItemCode\" = z.\"ItemCode\"" +
									" INNER JOIN " + uti.schemaHana + "OMRC y ON z.\"FirmCode\" = y.\"FirmCode\"" +
									" where x.\"ItemCode\" is not null and x.\"Quantity\" > 0 order by x.\"ItemCode\", x.\"ExpDate\" asc";

			string query3 = "UPDATE al.OIBT SET Quantity = @Quantity, ExpDate = @ExpDate, BatchNum = @BatchNum WHERE ItemCode = @ItemCode AND WhsCode = @WhsCode";

			try
			{
				HanaDataReader hdr = db.HanaExecuteReaderNoSp(query2);
				while (hdr.Read())
				{
					OIBT_E o = new OIBT_E();
					if (!hdr.IsDBNull(0)) { o.Laboratorio = hdr.GetString(0); }
					if (!hdr.IsDBNull(1)) { o.ItemCode = hdr.GetString(1); }
					if (!hdr.IsDBNull(2)) { o.ItemName = hdr.GetString(2); }
					if (!hdr.IsDBNull(3)) { o.BatchNum = hdr.GetString(3); }
					if (!hdr.IsDBNull(4)) { o.ExpDate = hdr.GetString(4); }
					if (!hdr.IsDBNull(5)) { o.Quantity = hdr.GetDecimal(5); }
					if (!hdr.IsDBNull(6)) { o.WhsCode = hdr.GetString(6); }
					if (!hdr.IsDBNull(7)) { o.Price = hdr.GetDecimal(7); }
					lista.Add(o);
				}
				hdr.Close();
			}
			catch { }


			var listaItemCode = new List<string>();

			foreach (var obj in lista)
			{

				try
				{
					cn.Open();
					SqlTransaction tran = cn.BeginTransaction();
					try
					{
						if (!listaItemCode.Contains(obj.ItemCode))
						{
							listaItemCode.Add(obj.ItemCode);

							SqlCommand cmd = new SqlCommand(query1, cn);
							cmd.Transaction = tran;
							cmd.CommandType = CommandType.Text;
							cmd.Parameters.AddWithValue("@Laboratorio", obj.Laboratorio);
							cmd.Parameters.AddWithValue("@ItemCode", obj.ItemCode);
							cmd.Parameters.AddWithValue("@ItemName", obj.ItemName);
							cmd.Parameters.AddWithValue("@BatchNum", obj.BatchNum);
							cmd.Parameters.AddWithValue("@ExpDate", obj.ExpDate);
							cmd.Parameters.AddWithValue("@Quantity", obj.Quantity);
							cmd.Parameters.AddWithValue("@WhsCode", obj.WhsCode);
							cmd.Parameters.AddWithValue("@Price", obj.Price);
							cmd.ExecuteNonQuery();
						}
						else
						{
							// Se sumarán las cantidades cuando los ItemCode sean iguales y solo se mostrará el ExpDate próximo a vencer							
							var result = BuscarArticulo(obj);

							// Para actualizar el registro, el artículo debe coincidir con el WhsCode, caso contrario se procederá a insertar ya que los artículos no son del mismo almacén
							if (result != null && result.Count >= 1)
							{
								SqlCommand cmd = new SqlCommand(query3, cn);
								cmd.Transaction = tran;
								cmd.CommandType = CommandType.Text;

								DateTime fechaVencInsert = Convert.ToDateTime(obj.ExpDate);
								DateTime fechaVencResult = Convert.ToDateTime(result[0].ExpDate);

								// Predomina el que tiene la fecha más próxima a vencer
								if (fechaVencInsert >= fechaVencResult)
								{
									cmd.Parameters.AddWithValue("@ExpDate", obj.ExpDate);
									cmd.Parameters.AddWithValue("@BatchNum", obj.BatchNum);
								}
								else
								{
									cmd.Parameters.AddWithValue("@ExpDate", result[0].ExpDate);
									cmd.Parameters.AddWithValue("@BatchNum", result[0].BatchNum);
								}

								cmd.Parameters.AddWithValue("@ItemCode", obj.ItemCode);
								cmd.Parameters.AddWithValue("@Quantity", obj.Quantity + result[0].Quantity);
								cmd.Parameters.AddWithValue("@WhsCode", obj.WhsCode);
								cmd.ExecuteNonQuery();
							}
							else
							{
								SqlCommand cmd = new SqlCommand(query1, cn);
								cmd.Transaction = tran;
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.AddWithValue("@Laboratorio", obj.Laboratorio);
								cmd.Parameters.AddWithValue("@ItemCode", obj.ItemCode);
								cmd.Parameters.AddWithValue("@ItemName", obj.ItemName);
								cmd.Parameters.AddWithValue("@BatchNum", obj.BatchNum);
								cmd.Parameters.AddWithValue("@ExpDate", obj.ExpDate);
								cmd.Parameters.AddWithValue("@Quantity", obj.Quantity);
								cmd.Parameters.AddWithValue("@WhsCode", obj.WhsCode);
								cmd.Parameters.AddWithValue("@Price", obj.Price);
								cmd.ExecuteNonQuery();
							}
							
						}
						
						tran.Commit();
					}
					catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error en migracion: " + e.Message); }
					cn.Close();
				}
				catch (Exception e2) { cn.Close(); throw new Exception("Error en migracion y conexion: " + e2.Message); }

			}
			return status;
		}
		public List<OIBT_E> BuscarArticulo(OIBT_E articulo)
		{
			List<OIBT_E> lista = new List<OIBT_E>();

			using (SqlConnection cn = new SqlConnection(uti.cadSql))
			{
				string query = $"SELECT Id, Laboratorio, ItemCode, ItemName, BatchNum, ExpDate, Quantity, WhsCode, Price FROM al.OIBT WHERE WhsCode = '{articulo.WhsCode}' AND ItemName LIKE '%{articulo.ItemName}%'";

				SqlCommand cmd = new SqlCommand(query, cn);         // prepara
				cn.Open();

				try
				{
					SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

					if (dr.HasRows)
					{
						while (dr.Read())
						{
							OIBT_E oibt = new OIBT_E();

							if (!dr.IsDBNull(0)) { oibt.Id = dr.GetInt32(0); }
							if (!dr.IsDBNull(1)) { oibt.Laboratorio = dr.GetString(1); }
							if (!dr.IsDBNull(2)) { oibt.ItemCode = dr.GetString(2); }
							if (!dr.IsDBNull(3)) { oibt.ItemName = dr.GetString(3); }
							if (!dr.IsDBNull(4)) { oibt.BatchNum = dr.GetString(4); }
							if (!dr.IsDBNull(5)) { oibt.ExpDate = dr.GetString(5); }
							if (!dr.IsDBNull(6)) { oibt.Quantity = dr.GetDecimal(6); }
							if (!dr.IsDBNull(7)) { oibt.WhsCode = dr.GetString(7); }
							if (!dr.IsDBNull(8)) { oibt.Price = dr.GetDecimal(8); }

							lista.Add(oibt);
						}
					}

					dr.Close();
				}
				catch (Exception e)
				{
					throw new Exception(e.Message);
				}

				cn.Close();
			}

			return lista;
		}
		//PROYECTO DEVOLUCIONES- ANTONY
        public int TemporizarMigrarArticulos()
        {
            // Create a timer with a two second interval.
            aTimer = new Timer(1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            return 1;
        }
        public static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            /*Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
							  e.SignalTime);*/
            secondsCount++;

            // 30 minutos -> 1800000
            if (secondsCount.Equals(1800000))
            {
                aTimer.Stop();
                OIBT_D oibtD = new OIBT_D();
                oibtD.Migrar();
            }
        }
        public bool VerificarMigracionArticulos()
        {
            bool ejecutandoMigracion = false;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT COUNT(Id) FROM al.OIBT";

                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        dr.Read();
                        if (dr.GetInt32(0) >= 1 && dr.GetInt32(0) <= 1000)
                        {
                            ejecutandoMigracion = true;
                        }
                    }

                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cn.Close();
                }
            }

            return ejecutandoMigracion;
        }
        public List<OIBT_E> ListarLaboratorios()
        {

            List<OIBT_E> lista = new List<OIBT_E>();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = $"SELECT DISTINCT Laboratorio FROM al.OIBT ORDER BY Laboratorio ASC";

                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            OIBT_E oibt = new OIBT_E();

                            if (!dr.IsDBNull(0)) { oibt.Laboratorio = dr.GetString(0); }

                            lista.Add(oibt);
                        }
                    }

                    dr.Close();
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
                finally
                {
                    cn.Close();
                }
            }

            return lista;
        }
    }
}