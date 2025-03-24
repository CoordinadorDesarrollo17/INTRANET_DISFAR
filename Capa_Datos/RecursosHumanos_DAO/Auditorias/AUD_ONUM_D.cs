using Capa_Entidad.RecursosHumanos_ENT.Auditorias;
using Capa_Entidad.RecursosHumanos_ENT.TablasSQL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Capa_Datos.RecursosHumanos_DAO.Auditorias
{
    public class AUD_ONUM_D
    {
        readonly Utilitarios uti = new Utilitarios();
        public void RegistrarAuditoria(AUD_ONUM_E datos)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_AUDITORIAS", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", "AUD_ONUM");
                    cmd.Parameters.AddWithValue("@IdNumero", datos.IdNumero);
                    cmd.Parameters.AddWithValue("@Campo", datos.Campo);
                    cmd.Parameters.AddWithValue("@ValorAnterior", datos.ValorAnterior);
                    cmd.Parameters.AddWithValue("@ValorActual", datos.ValorActual);
                    cmd.Parameters.AddWithValue("@RegistradoPor", datos.RegistradoPor);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    RegistrarError(ex, "AUD_ONUM_D - RegistrarAuditoria");
                }
            }
        }
        public List<AUD_ONUM_E> AuditarNumero(AUD_ONUM_E filtros)
        {
            List<AUD_ONUM_E> lista = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = cn;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("SELECT AUD_NU.IdAUD_ONUM, AUD_NU.IdNumero, AUD_NU.Campo, AUD_NU.ValorAnterior, AUD_NU.ValorActual, AUD_NU.RegistradoPor, CONVERT(varchar, AUD_NU.FechaRegistro, 103), CONVERT(varchar, AUD_NU.HoraRegistro, 108),");
                    sb.Append(" CONCAT(USU.NOMBRES, ' ', USU.APELLIDOS)");
                    sb.Append(" FROM dbo.AUD_ONUM AUD_NU");
                    sb.Append(" INNER JOIN dbo.OUSR USU ON USU.DocEntry = AUD_NU.RegistradoPor");
                    sb.Append(" WHERE 1 = 1");
                    if (filtros != null)
                    {
                        if (filtros.IdNumero > 0)
                        {
                            sb.Append(" AND AUD_NU.IdNumero = @IdNumero");
                            cmd.Parameters.AddWithValue("@IdNumero", filtros.IdNumero);
                        }
                    }
                    //sb.Append($" ORDER BY ---- DESC");    DESCOMENTAR LÍNEA SI SE DESEA ORDENAR POR ALGÚN CAMPO EN ESPECIAL
                    cmd.CommandText = sb.ToString();
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            lista = new List<AUD_ONUM_E>();
                            while (dr.Read())
                            {
                                AUD_ONUM_E obj = new AUD_ONUM_E();
                                if (!dr.IsDBNull(0)) { obj.IdAUD_ONUM = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.IdNumero = dr.GetInt32(1); }
                                if (!dr.IsDBNull(2)) { obj.Campo = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.ValorAnterior = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.ValorActual = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.RegistradoPor = dr.GetInt32(5); }
                                if (!dr.IsDBNull(6)) { obj.FechaRegistro = dr.GetString(6); }
                                if (!dr.IsDBNull(7)) { obj.HoraRegistro = dr.GetString(7); }
                                if (!dr.IsDBNull(8)) { obj.NomApeRegistradoPor = dr.GetString(8); }
                                lista.Add(obj);
                            }
                        }
                    }
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                RegistrarError(ex, "AUD_ONUM_D - AuditarNumero");
            }
            return lista;
        }
        private void RegistrarError(Exception ex, string nombreArchivo)
        {
            File.AppendAllText(uti.directorioLogs + nombreArchivo + ".txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");
        }
    }
}
