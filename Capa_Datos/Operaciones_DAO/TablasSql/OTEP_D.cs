using Capa_Entidad.Operaciones_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Capa_Datos.Operaciones_DAO.TablasSql
{
	public class OTEP_D
	{
		readonly Utilitarios uti = new Utilitarios();

		public List<OTEP_E> ListarTiposErroresPicking()
		{
			List<OTEP_E> lista = new List<OTEP_E>();

			using (SqlConnection cn = new SqlConnection(uti.cadSql))
			{
				StringBuilder sb = new StringBuilder();

				sb.Append("SELECT OTEP.IdTipoErrorPicking, OTEP.Descripcion, OTEP.Estado, CONVERT(varchar, OTEP.FechaRegistro, 103)");
				sb.Append(" FROM vt.TipoErroresPicking OTEP");

				string query = sb.ToString();

				SqlCommand cmd = new SqlCommand(query, cn);         // prepara
				cn.Open();

				try
				{
					SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

					if (dr.HasRows)
					{
						while (dr.Read())
						{
							OTEP_E rpt = new OTEP_E();
							if (!dr.IsDBNull(0)) { rpt.IdTipoErrorPicking = dr.GetInt32(0); }
							if (!dr.IsDBNull(1)) { rpt.Descripcion = dr.GetString(1); }
							if (!dr.IsDBNull(2)) { rpt.Estado = dr.GetString(2); }
							if (!dr.IsDBNull(3)) { rpt.FechaRegistro = dr.GetString(3); }

							lista.Add(rpt);
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
