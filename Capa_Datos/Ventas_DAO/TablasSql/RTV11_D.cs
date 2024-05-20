using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Capa_Entidad.Operaciones_ENT.TablasSql;
using System.Text;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System.Data;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class RTV11_D
    {
        DBHelper db = new DBHelper();
		readonly Utilitarios uti = new Utilitarios();

		public List<RTV11_E> ObtenerPickers(int docEntry)
        {
            List<RTV11_E> lista = new List<RTV11_E>();
			CC_ORTV_D cc_ortvD = new CC_ORTV_D();

			List<CC_ORTV_E> opCC = cc_ortvD.ListarCC_ORTV(docEntry, "FIN PICKING", false);

			if (opCC != null && opCC.Count >= 1)
			{
				lista.Add(new RTV11_E { Operario = opCC[0].Operario });
			}
			
			using (SqlConnection cn = new SqlConnection(uti.cadSql))
			{
				StringBuilder sb = new StringBuilder();

				sb.Append("SELECT Operario FROM vt.RTV11 WHERE DocEntry = @DocEntry");
				string query = sb.ToString();

				SqlCommand cmd = new SqlCommand(query, cn);         // prepara
				cn.Open();

				try
				{
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.AddWithValue("@DocEntry", docEntry);
					SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

					if (dr.HasRows)
					{
						while (dr.Read())
						{
							RTV11_E datos = new RTV11_E();
							if (!dr.IsDBNull(0)) { datos.Operario = dr.GetString(0); }

							lista.Add(datos);
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


        public List<string> BuscarOperariosSacando(int DocEntry)
        {
            List<string> lista = new List<string>();
            string query = "SELECT Operario FROM vt.RTV11 WHERE DocEntry=@DocEntry";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string>() { "@DocEntry" }, DocEntry);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (dr.GetString(0).Length > 12) { lista.Add(dr.GetString(0).Substring(0, 12)); }
                        else { lista.Add(dr.GetString(0)); }
                    }
                }

                dr.Close();
            }
            catch { }
            return lista;
        }

    }
}
