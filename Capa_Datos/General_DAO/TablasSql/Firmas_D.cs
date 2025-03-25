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
        public List<Firmas_E> ListarFirmas(string condicion, Dictionary<string, object> parametros)
        {
            List<Firmas_E> lista = new List<Firmas_E>();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cn;
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("SELECT FIR.[DocEntry], FIR.[DocEntryUsuario], FIR.[Nombres], FIR.[Apellidos], FIR.[RutaFirma], FIR.[TipoFirma], FIR.[CodigoAlmacen], FIR.[Almacen], USU.[IdRol]");
                sb.AppendLine("FROM Firmas FIR");
                sb.AppendLine("INNER JOIN OUSR USU ON USU.[DocEntry] = FIR.[DocEntryUsuario]");
                sb.AppendLine("WHERE FIR.[Nombres] != ''");
                sb.AppendLine(condicion);

                cmd.CommandText = sb.ToString();

                // Agregamos los parámetros dinámicamente
                foreach (var param in parametros)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value);
                }

                try
                {
                    cn.Open();
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
                            if (!dr.IsDBNull(5)) { md.TipoFirma = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { md.CodigoAlmacen = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { md.Almacen = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { md.IdRolUsuario = dr.GetInt32(8); }

                            lista.Add(md);
                        }
                    }

                    dr.Close();
                }
                catch (Exception ex)
                {
                    LogHelper.RegistrarError(ex, "Ubicaciones_D - ListarUbicaciones");
                    throw;
                }
            }

            return lista;
        }
    }
}