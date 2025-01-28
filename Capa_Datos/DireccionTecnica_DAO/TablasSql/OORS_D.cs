using Capa_Entidad.DireccionTecnica_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace Capa_Datos.DireccionTecnica_DAO.TablasSql
{
    public class OORS_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<OORS_E> ObtenerDatosObsRS(string registroSanitario, string codArticulo, string codLaboratorio)
        {
            List<OORS_E> result = null;

            StringBuilder query = new StringBuilder("SELECT OBS.IdOORS, OBS.CodLaboratorio, OBS.CodArticulo, OBS.RegistroSanitario, OBS.Descripcion, OBS.RegistradoPor, CONVERT(varchar, OBS.FechaRegistro, 103), CONVERT(varchar, OBS.HoraRegistro, 8) FROM dt.OORS OBS WHERE 1 = 1");

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cn;

                if (!string.IsNullOrWhiteSpace(registroSanitario))
                {
                    query.Append(" AND OBS.RegistroSanitario = @registroSanitario");
                    cmd.Parameters.AddWithValue("@registroSanitario", registroSanitario);
                }

                if (!string.IsNullOrWhiteSpace(codArticulo))
                {
                    query.Append(" AND CodArticulo = @codArticulo");
                    cmd.Parameters.AddWithValue("@codArticulo", codArticulo);
                }

                if (!string.IsNullOrWhiteSpace(codLaboratorio))
                {
                    query.Append(" AND OBS.CodLaboratorio = @codLaboratorio");
                    cmd.Parameters.AddWithValue("@codLaboratorio", codLaboratorio);
                }

                cmd.CommandText = query.ToString();

                cn.Open();

                try
                {
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            result = new List<OORS_E>();

                            while (dr.Read())
                            {
                                OORS_E obj = new OORS_E();

                                if (!dr.IsDBNull(0)) { obj.IdOORS = dr.GetInt32(0); }
                                if (!dr.IsDBNull(1)) { obj.CodLaboratorio = dr.GetInt32(1); }
                                if (!dr.IsDBNull(2)) { obj.CodArticulo = dr.GetString(2); }
                                if (!dr.IsDBNull(3)) { obj.RegistroSanitario = dr.GetString(3); }
                                if (!dr.IsDBNull(4)) { obj.Descripcion = dr.GetString(4); }
                                if (!dr.IsDBNull(5)) { obj.RegistradoPor = dr.GetString(5); }
                                if (!dr.IsDBNull(6)) { obj.FechaRegistro = dr.GetString(6); }
                                if (!dr.IsDBNull(7)) { obj.HoraRegistro = dr.GetString(7); }

                                result.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            return result;
        }

        protected int ObtenerIdOORS(string registroSanitario, string codArticulo)
        {
            int result = 0;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT TOP 1 IdOORS FROM dt.OORS WHERE CodArticulo = @codArticulo AND RegistroSanitario = @registroSanitario";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@registroSanitario", registroSanitario);
                cmd.Parameters.AddWithValue("@codArticulo", codArticulo);

                cn.Open();
                result = (int?)cmd.ExecuteScalar() ?? 0;
                cn.Close();
            }

            return result;
        }

        public List<OORS_E> ListarRegistrosSanitarios(OORS_E rs = null)
        {
            DBHelper db = new DBHelper();
            List<OORS_E> lista = null;
            string condWhere = string.Empty;

            if (rs != null)
            {
                if (!string.IsNullOrWhiteSpace(rs.RegistroSanitario)) { condWhere += $" AND BTN.\"MnfSerial\" = '{rs.RegistroSanitario}'"; }
                if (!string.IsNullOrWhiteSpace(rs.CodArticulo)) { condWhere += $" AND ITM.\"ItemCode\" = '{rs.CodArticulo}'"; }
                if (!string.IsNullOrWhiteSpace(rs.DescArticulo)) { condWhere += $" AND ITM.\"ItemName\" LIKE '%{rs.DescArticulo.ToUpper()}%'"; }
                if (!string.IsNullOrWhiteSpace(rs.FechaVenc)) { condWhere += $" AND TO_CHAR(ITM.\"U_COB_FECH_RS\", 'YYYY-MM-DD') = '{rs.FechaVenc}'"; }
                if (!string.IsNullOrWhiteSpace(rs.Estado)) { condWhere += $" AND ITM.\"U_COB_ESTRS\" = '{rs.Estado}'"; }
            }

            string query = @" SELECT TOP 50 
                                                    BTN.""MnfSerial"", 
                                                    ITM.""ItemCode"", 
                                                    ITM.""FirmCode"", 
                                                    ITM.""ItemName"", 
                                                    TO_CHAR(ITM.""U_COB_FECH_RS"", 'YYYY-MM-DD'), 
                                                    ITM.""U_COB_OREGS"", 
                                                    EST.""Name"" 
                                                FROM 
                                                    " + uti.schemaHana + @"OIBT IBT 
                                                INNER JOIN 
                                                    " + uti.schemaHana + @"OITM ITM ON ITM.""ItemCode"" = IBT.""ItemCode""
                                                LEFT JOIN 
                                                    " + uti.schemaHana + @"OBTN BTN ON BTN.""ItemCode"" = ITM.""ItemCode""
                                                LEFT JOIN " + uti.schemaHana + @"""@COB_ESTA_RS"" EST ON EST.""Code"" = ITM.""U_COB_ESTRS""
                                               WHERE 
                                                    BTN.""MnfSerial"" != '' AND IBT.""Quantity"" > 0 " + condWhere + @"
                                                GROUP BY 
                                                    BTN.""MnfSerial"", 
                                                    ITM.""ItemCode"", 
                                                    ITM.""FirmCode"", 
                                                    ITM.""ItemName"", 
                                                    TO_CHAR(ITM.""U_COB_FECH_RS"", 'YYYY-MM-DD'), 
                                                    ITM.""U_COB_OREGS"", 
                                                    EST.""Name""";

            try
            {
                using (HanaDataReader hdr = db.HanaExecuteReaderNoSp(query))
                {
                    lista = new List<OORS_E>();

                    while (hdr.Read())
                    {
                        OORS_E obj = new OORS_E();
                        if (!hdr.IsDBNull(0)) { obj.RegistroSanitario = hdr.GetString(0); }
                        if (!hdr.IsDBNull(1)) { obj.CodArticulo = hdr.GetString(1); }
                        if (!hdr.IsDBNull(2)) { obj.CodLaboratorio = hdr.GetInt32(2); }
                        if (!hdr.IsDBNull(3)) { obj.DescArticulo = hdr.GetString(3); }
                        if (!hdr.IsDBNull(4)) { obj.FechaVenc = hdr.GetString(4); }
                        if (!hdr.IsDBNull(5)) { obj.Comentario = hdr.GetString(5); }
                        if (!hdr.IsDBNull(6)) { obj.Estado = hdr.GetString(6); }
                        obj.IdOORS = ObtenerIdOORS(hdr.GetString(0), hdr.GetString(1));

                        lista.Add(obj);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return lista;
        }

        public string RegistrarObservacion(OORS_E rs)
        {
            string result;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd2 = new SqlCommand("dt.MANT_OORS", cn);
                    cmd2.CommandType = CommandType.StoredProcedure;

                    cmd2.Parameters.AddWithValue("@Accion", "INS");
                    cmd2.Parameters.AddWithValue("@CodArticulo", rs.CodArticulo);
                    cmd2.Parameters.AddWithValue("@CodLaboratorio", rs.CodLaboratorio);
                    cmd2.Parameters.AddWithValue("@RegistroSanitario", rs.RegistroSanitario);
                    cmd2.Parameters.AddWithValue("@Descripcion", rs.Descripcion);
                    cmd2.Parameters.AddWithValue("@RegistradoPor", rs.RegistradoPor);

                    cmd2.ExecuteNonQuery();

                    result = "Se registró la observación satisfactoriamente";
                }
                catch (Exception ex)
                {
                    result = "Error al registrar observación: " + ex.Message;
                    throw new Exception("Error en registro: " + ex.Message);
                }
            }

            return result;
        }

        public List<string> ConsultarRegistrosSanitariosExpirados()
        {
            DBHelper db = new DBHelper();
            List<string> lista = null;

            string query = @"SELECT TOP 100 
                      ITM.""ItemCode"", 
                      TO_CHAR(ITM.""U_COB_FECH_RS"", 'YYYY-MM-DD'), 
                      DAYS_BETWEEN(CURRENT_DATE, TO_CHAR(ITM.""U_COB_FECH_RS"", 'YYYY-MM-DD'))
                FROM 
                      " + uti.schemaHana + @"OITM ITM 
                      LEFT JOIN " + uti.schemaHana + @"OBTN BTN ON BTN.""ItemCode"" = ITM.""ItemCode""
                WHERE 
                      BTN.""MnfSerial"" != '' AND 
                      ITM.""U_COB_FECH_RS"" != '' AND 
                      DAYS_BETWEEN(CURRENT_DATE, TO_CHAR(ITM.""U_COB_FECH_RS"", 'YYYY-MM-DD')) IN (0, 5, 15) AND 
                      ITM.""U_COB_ESTRS"" IN ('01', '02', '06')
                GROUP BY 
                      ITM.""ItemCode"", 
                      TO_CHAR(ITM.""U_COB_FECH_RS"", 'YYYY-MM-DD')";

            try
            {
                using (HanaDataReader hdr = db.HanaExecuteReaderNoSp(query))
                {
                    lista = new List<string>();

                    while (hdr.Read())
                    {
                        if (!hdr.IsDBNull(0))
                        {
                            string codigoArticulo = hdr.GetString(0);
                            string diasParaVencer = hdr.GetString(2);

                            string mensaje = $"Cód. Artículo: {codigoArticulo} vencerá en {diasParaVencer} {(Convert.ToInt32(diasParaVencer) > 1 ? "días" : "día")}";

                            lista.Add(mensaje);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return lista;
        }
    }
}