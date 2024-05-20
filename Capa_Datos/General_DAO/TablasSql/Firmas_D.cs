using Capa_Entidad.General_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;


namespace Capa_Datos.General_DAO.TablasSql
{
    public class Firmas_D
    {
        readonly Utilitarios uti = new Utilitarios();
        public List<Firmas_E> ListarFirmas(Firmas_E filtros)
        {
            List<Firmas_E> lista = new List<Firmas_E>();
            string condWhere = string.Empty;

            if (filtros != null)
            {
                if (filtros.DocEntryUsuario >= 1)
                {
                    condWhere += $" AND FIR.DocEntryUsuario = '{filtros.DocEntryUsuario}'";
                }

                if (filtros.ListaDocEntryUsuario != null && filtros.ListaDocEntryUsuario.Count >= 1)
                {
                    condWhere += $" AND FIR.DocEntryUsuario IN ({string.Join(",", filtros.ListaDocEntryUsuario)})";
                }

                if (!string.IsNullOrEmpty(filtros.Nombres) || !string.IsNullOrEmpty(filtros.Apellidos))
                {
                    condWhere += $" AND CONCAT(FIR.Nombres, ' ', FIR.Apellidos) LIKE '%{filtros.Nombres}%'";
                }
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT FIR.DocEntry, FIR.DocEntryUsuario, FIR.Nombres, FIR.Apellidos, FIR.RutaFirma, USU.IdRol")
                    .Append(" FROM Firmas FIR")
                    .Append(" INNER JOIN OUSR USU ON USU.DocEntry = FIR.DocEntryUsuario")
                    .Append($" WHERE FIR.Nombres != '' {condWhere}");

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
                            Firmas_E md = new Firmas_E();
                            if (!dr.IsDBNull(0)) { md.DocEntry = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { md.DocEntryUsuario = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { md.Nombres = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { md.Apellidos = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { md.RutaFirma = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { md.IdRolUsuario = dr.GetInt32(5); }
                            lista.Add(md);
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
