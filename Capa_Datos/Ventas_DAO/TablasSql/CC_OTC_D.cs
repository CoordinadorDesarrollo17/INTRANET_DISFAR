using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
	public class CC_OTC_D
	{
		readonly Utilitarios uti = new Utilitarios();

		public CC_OTC_E ObtenerDatosCC_OTC(int idOTC, string operacion)
		{
			CC_OTC_E result = null;

			using (SqlConnection cn = new SqlConnection(uti.cadSql))
			{
				StringBuilder sb = new StringBuilder();

				sb.Append("SELECT TOP 1 IdCC_OTC, IdOTC, Operacion, Operario, FechaOperacion, CONVERT(varchar, HoraOperacion, 8)");
				sb.Append(" FROM cj.CC_OTC");
				sb.Append(" WHERE IdOTC = @IdOTC AND Operacion = @Operacion");

				string query = sb.ToString();

				SqlCommand cmd = new SqlCommand(query, cn);         // prepara
				cmd.Parameters.AddWithValue("@IdOTC", idOTC);
				cmd.Parameters.AddWithValue("@Operacion", operacion);
				cn.Open();

				try
				{
					SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

					if (dr.HasRows)
					{
						result = new CC_OTC_E();
						while (dr.Read())
						{
							if (!dr.IsDBNull(0)) { result.IdCC_OTC = dr.GetInt32(0); }
							if (!dr.IsDBNull(1)) { result.IdOTC = dr.GetInt32(1); }
							if (!dr.IsDBNull(2)) { result.Operacion = dr.GetString(2); }
							if (!dr.IsDBNull(3)) { result.Operario = dr.GetString(3); }
							if (!dr.IsDBNull(4)) { result.FechaOperacion = dr.GetDateTime(4).ToString("yyyy-MM-dd"); }
							if (!dr.IsDBNull(5)) { result.HoraOperacion = dr.GetString(5); }
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

			return result;
		}

		public Dictionary<string, CC_OTC_E> ObtenerFechaCobroFechaCuadre(int idOTC)
		{
			Dictionary<string, CC_OTC_E> lista = null;

			using (SqlConnection cn = new SqlConnection(uti.cadSql))
			{
				StringBuilder sb = new StringBuilder();

				sb.Append("SELECT IdCC_OTC, IdOTC, Operacion, Operario, FechaOperacion, CONVERT(varchar, HoraOperacion, 8)");
				sb.Append(" FROM cj.CC_OTC");
				sb.Append(" WHERE IdOTC = @IdOTC AND Operacion IN ('REGISTRAR', 'CUADRAR')");

				string query = sb.ToString();

				SqlCommand cmd = new SqlCommand(query, cn);         // prepara
				cmd.Parameters.AddWithValue("@IdOTC", idOTC);
				cn.Open();

				try
				{
					SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

					if (dr.HasRows)
					{
						lista = new Dictionary<string, CC_OTC_E>();
						CC_OTC_E result = new CC_OTC_E();

						while (dr.Read())
						{
							if (!dr.IsDBNull(0)) { result.IdCC_OTC = dr.GetInt32(0); }
							if (!dr.IsDBNull(1)) { result.IdOTC = dr.GetInt32(1); }
							if (!dr.IsDBNull(2)) { result.Operacion = dr.GetString(2); }
							if (!dr.IsDBNull(3)) { result.Operario = dr.GetString(3); }
							if (!dr.IsDBNull(4)) { result.FechaOperacion = dr.GetDateTime(4).ToString("yyyy-MM-dd"); }
							if (!dr.IsDBNull(5)) { result.HoraOperacion = dr.GetString(5); }

							lista.Add(result.Operacion, result);
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
