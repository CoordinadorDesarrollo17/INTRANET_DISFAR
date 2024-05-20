using Capa_Entidad.Ventas_ENT.Reportes;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
	public class OEP_D
	{
		readonly Utilitarios uti = new Utilitarios();

		public string RegistrarErroresPicking(OEP_E cabecera, List<EP1_E> detalleErroresPicking)
		{
			string result;

			using (SqlConnection cn = new SqlConnection(uti.cadSql))
			{
				cn.Open();
				SqlTransaction tran = cn.BeginTransaction();

				try
				{
					SqlCommand cmd = new SqlCommand("vt.MANT_OEP", cn);
					cmd.Transaction = tran;
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@Accion", "INS");
					cmd.Parameters.AddWithValue("@DocEntryTicket", cabecera.DocEntryTicket);
					cmd.Parameters.AddWithValue("@DocNumTicket", cabecera.DocNumTicket);
					cmd.Parameters.AddWithValue("@OpRegistro", cabecera.OpRegistro);

					if (detalleErroresPicking != null && detalleErroresPicking.Count >= 1)
					{
						SqlParameter tbDetalle = new SqlParameter("@TPEP1", SqlDbType.Structured);
						tbDetalle.Value = EP1_E.TbDetalle(detalleErroresPicking, 0);
						tbDetalle.TypeName = "vt.TPEP1";
						cmd.Parameters.AddWithValue("@TPEP1", tbDetalle.Value);
					}

					cmd.ExecuteNonQuery();
					result = "Errores de picking registrados correctamente";

					tran.Commit();
				}
				catch (Exception ex2)
				{
					result = "Error al registrar errores de picking"; tran.Rollback();
					throw new Exception("Error en registro: " + ex2.Message);
				}
				finally
				{
					cn.Close();
				}
			}

			return result;
		}

		public OEP_E ObtenerDatosErroresPicking(int docEntryTicket)
		{
			OEP_E result = null;

			using (SqlConnection cn = new SqlConnection(uti.cadSql))
			{
				StringBuilder sb = new StringBuilder();

				sb.Append("SELECT EP.IdOEP, EP.DocEntryTicket, EP.DocNumTicket, EP.Estado, EP.OpRegistro, CONVERT(varchar, EP.FechaRegistro, 103), CONVERT(varchar, EP.HoraRegistro, 8)");
				sb.Append(" FROM vt.OEP EP");
				sb.Append($" WHERE EP.Estado = '1' AND EP.DocEntryTicket = @DocEntryTicket");

				string query = sb.ToString();

				SqlCommand cmd = new SqlCommand(query, cn);         // prepara
				cmd.Parameters.AddWithValue("@DocEntryTicket", docEntryTicket);
				cn.Open();

				try
				{
					SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

					if (dr.HasRows)
					{
						result = new OEP_E();
						while (dr.Read())
						{
							if (!dr.IsDBNull(0)) { result.IdOEP = dr.GetInt32(0); }
							if (!dr.IsDBNull(1)) { result.DocEntryTicket = dr.GetInt32(1); }
							if (!dr.IsDBNull(2)) { result.DocNumTicket = dr.GetInt32(2); }
							if (!dr.IsDBNull(3)) { result.Estado = dr.GetString(3); }
							if (!dr.IsDBNull(4)) { result.OpRegistro = dr.GetString(4); }
							if (!dr.IsDBNull(5)) { result.FechaRegistro = dr.GetString(5); }
							if (!dr.IsDBNull(6)) { result.HoraRegistro = dr.GetString(6); }
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

		public List<RptErroresPicking_E> ExportarReporteErroresPicking(RptFiltrosErroresPicking_E filtros)
		{
			List<RptErroresPicking_E> lista = new List<RptErroresPicking_E>();
			string condWhere = string.Empty;

			if (filtros != null)
			{
				if (!string.IsNullOrEmpty(filtros.FechaTicketDesde) && !string.IsNullOrEmpty(filtros.FechaTicketHasta))
				{
					condWhere += $" AND VT.FechaSapTicket BETWEEN '{filtros.FechaTicketDesde}' AND '{filtros.FechaTicketHasta}'";
				}

				if (filtros.DocNumTicket >= 1)
				{
					condWhere += $" AND EP.DocNumTicket = '{filtros.DocNumTicket}'";
				}

				if (!string.IsNullOrEmpty(filtros.Estado))
				{
					condWhere += $" AND EP.Estado = '{filtros.Estado}'";
				}
			}

			using (SqlConnection cn = new SqlConnection(uti.cadSql))
			{
				StringBuilder sb = new StringBuilder();

				sb.Append("SELECT EP.DocNumTicket, CONVERT(varchar, VT.FechaSapTicket, 103), VT.Estado, DET.CodigoProducto, DET.DescripcionProducto, TP.Descripcion, DET.PickerResponsable, CASE WHEN EP.Estado = '1' THEN 'ACTIVO' ELSE 'INACTIVO' END, EP.OpRegistro, CONVERT(varchar, EP.FechaRegistro, 103), CONVERT(varchar, EP.HoraRegistro, 8)");
				sb.Append(" FROM vt.OEP EP");
				sb.Append(" INNER JOIN vt.ORTV VT ON VT.DocEntry = EP.DocEntryTicket ");
				sb.Append(" INNER JOIN vt.EP1 DET ON DET.IdOEP = EP.IdOEP");
				sb.Append(" INNER JOIN vt.TipoErroresPicking TP ON TP.IdTipoErrorPicking = DET.IdTipoErrorPicking");
				sb.Append($" WHERE EP.IdOEP > 0 {condWhere}");

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
							RptErroresPicking_E rpt = new RptErroresPicking_E();
							if (!dr.IsDBNull(0)) { rpt.DocNumTicket = dr.GetInt32(0); }
							if (!dr.IsDBNull(1)) { rpt.FechaTicket = dr.GetString(1); }
							if (!dr.IsDBNull(2)) { rpt.EstadoTicket = dr.GetString(2); }
							if (!dr.IsDBNull(3)) { rpt.CodigoProducto = dr.GetString(3); }
							if (!dr.IsDBNull(4)) { rpt.DescripcionProducto = dr.GetString(4); }
							if (!dr.IsDBNull(5)) { rpt.TipoError = dr.GetString(5); }
							if (!dr.IsDBNull(6)) { rpt.PickerResponsable = dr.GetString(6); }
							if (!dr.IsDBNull(7)) { rpt.Estado = dr.GetString(7); }
							if (!dr.IsDBNull(8)) { rpt.OpRegistro = dr.GetString(8); }
							if (!dr.IsDBNull(9)) { rpt.FechaRegistro = dr.GetString(9); }
							if (!dr.IsDBNull(10)) { rpt.HoraRegistro = dr.GetString(10); }

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
