using System;
using System.Collections.Generic;
using Capa_Entidad.Rutas_ENT.TablasSql;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Capa_Entidad.Seguridad_ENT;

namespace Capa_Datos.Rutas_DAO.TablasSql
{
	public class RRU11_D
	{
		readonly Utilitarios uti = new Utilitarios();
		public List<RRU11_E> ListarRRU11(int docEntry, int linea = 0)
		{
			List<RRU11_E> lista = new List<RRU11_E>();

			using (SqlConnection cn = new SqlConnection(uti.cadSql))
			{
				string query = "SELECT * FROM al.RRU11 WHERE BaseEntry = @BaseEntry";

				if (linea != 0)
				{
					query += " AND Baselinea = @Baselinea";
				}

				SqlCommand cmd = new SqlCommand(query, cn);         // prepara
				cmd.Parameters.AddWithValue("@BaseEntry", docEntry);
				if (linea != 0)
				{
					cmd.Parameters.AddWithValue("@Baselinea", linea);
				}
				cn.Open();

				try
				{
					SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

					if (dr.HasRows)
					{
						while (dr.Read())
						{
							RRU11_E rru11 = new RRU11_E();
							rru11.BaseEntry = dr.GetInt32(0);
							rru11.BaseLinea = dr.GetInt32(1);
							if (!dr.IsDBNull(2)) { rru11.Linea = dr.GetInt32(2); }
							if (!dr.IsDBNull(3)) { rru11.ItemCode = dr.GetString(3); }
							if (!dr.IsDBNull(4)) { rru11.ItemName = dr.GetString(4); }
							if (!dr.IsDBNull(5)) { rru11.Lote = dr.GetString(5); }
							if (!dr.IsDBNull(6)) { rru11.CantidadL = dr.GetDecimal(6); }
							if (!dr.IsDBNull(7)) { rru11.LaboCod = dr.GetInt32(7); }
							if (!dr.IsDBNull(8)) { rru11.LaboDesc = dr.GetString(8); }
							if (!dr.IsDBNull(9)) { rru11.UnitMed = dr.GetString(9); }
							if (!dr.IsDBNull(10)) { rru11.CantUnitMed = dr.GetInt32(10); }
							if (!dr.IsDBNull(11)) { rru11.Cajas = dr.GetInt32(11); }
							lista.Add(rru11);
						}
					}
					/*else
					{
						RRU11_E rru11 = new RRU11_E { Id = 0, DocEntry = 0, Operacion = operacion, Operario = "", FechaOperacion = "", HoraOperacion = "" };
						lista.Add(rru11);
					}*/

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

		public void EditarDetalleOrdenRuta(int BaseEntry, int BaseLinea, int[] DetRRU11)
		{
			using (SqlConnection cn = new SqlConnection(uti.cadSql))
			{
				int linea = 1;
				foreach (var nuevoValorCaja in DetRRU11)
				{
					if (nuevoValorCaja >= 1)
					{
						string query = $"UPDATE al.RRU11 SET Cajas = {nuevoValorCaja} WHERE BaseEntry = @BaseEntry AND BaseLinea = @BaseLinea AND Linea = @Linea";

						cn.Open();
						SqlTransaction tran = cn.BeginTransaction("EDITAR DETALLE OR");
						try
						{
							SqlCommand cmd = new SqlCommand(query, cn, tran);         // prepara
							cmd.Parameters.AddWithValue("@BaseEntry", BaseEntry);
							cmd.Parameters.AddWithValue("@BaseLinea", BaseLinea);
							cmd.Parameters.AddWithValue("@Linea", linea);

							cmd.ExecuteNonQuery();
							tran.Commit();
						}
						catch (Exception e) { tran.Rollback(); throw new Exception("Error: " + e.Message); }
						cn.Close();
						++linea;
					}
					
				}
				
			}
		}
	}
}
