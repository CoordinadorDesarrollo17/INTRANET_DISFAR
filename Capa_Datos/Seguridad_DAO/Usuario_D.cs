using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Capa_Entidad.Seguridad_ENT;
using DocumentFormat.OpenXml.Office.Word;

namespace Capa_Datos
{
    public class Usuario_D
    {
        Utilitarios uti = new Utilitarios();
        DBHelper db = new DBHelper();

        public string Perfiles(int IdRol)
        {
            Dictionary<int, string> roles = new Dictionary<int, string>
                {
                    { 2, $" AND IdRol in({IdRol},3)" },								// SDT, DT
					{ 4, $" AND IdRol in({IdRol},5,50,51,52,53,54)" },			// SALM, RECEP,PIK, ALM, ENC, DESPACHO, FACT
					{ 6, $" AND IdRol in({IdRol},7)" },								// SVENTAS, VENTAS
					{ 8, $" AND IdRol in({IdRol},9)" },								// SCAJA, CAJA
					{ 10, $" AND IdRol in({IdRol})" },								// COMPRAS
					{ 11, $" AND IdRol in({IdRol})" },								// SPATC
					{ 55, $" AND IdRol in({IdRol})" }								// REPA
				};

            return roles[IdRol];
        }

        public Usuario_E buscarUsuario(int DocEntry)
        {
            Usuario_E u = new Usuario_E();
            String select = "DocEntry, Prefijo, Id, Nombres, Apellidos, Email, IdRol, Activo, CONVERT(varchar, FechaRegistro, 103) AS FechaRegistro, HoraRegistro, OpRegistro, WhsCode, CodigoSap,ClaveEmail";
            string query = $"select {select} from dbo.OUSR where DocEntry=@DocEntry";

            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    if (!dr.IsDBNull(0)) { u.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { u.Prefijo = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { u.Id = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { u.Nombres = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { u.Apellidos = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { u.Email = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { u.IdRol = dr.GetInt32(6); }
                    if (!dr.IsDBNull(7)) { u.Activo = dr.GetInt32(7); }
                    if (!dr.IsDBNull(8)) { u.FechaRegistro = dr.GetString(8); }
                    if (!dr.IsDBNull(9)) { u.HoraRegistro = dr.GetTimeSpan(9).ToString(); }
                    if (!dr.IsDBNull(10)) { u.OperarioRegistro = dr.GetString(10); }
                    if (!dr.IsDBNull(11)) { u.WhsCode = dr.GetString(11); }
                    if (!dr.IsDBNull(12)) { u.CodigoSap = dr.GetInt32(12); }
                    if (!dr.IsDBNull(13)) { u.ClaveEmail = dr.GetString(13); }
                }
                dr.Close();
                cn.Close();
            }
            catch (Exception e) { cn.Close(); throw new Exception(e.Message); }
            return u;
        }

        public Usuario_E buscarUsuarioSesion(string user, string pass)
        {
            Usuario_E u = null;
            SqlConnection cn = new SqlConnection(uti.cadSql);

            string query = @"
                            SELECT
                                DocEntry, Prefijo, Id, Nombres, Apellidos, Email, IdRol, Activo, FechaRegistro, HoraRegistro, OpRegistro, WhsCode, CodigoSap,ClaveEmail
                            FROM OUSR 
                            WHERE Activo = 1 AND CONCAT(Prefijo,Id) = @user AND CONVERT(VARCHAR(MAX), DECRYPTBYPASSPHRASE('pwC0B3F@R', Password)) = @pass";

            try
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@user", user);
                    cmd.Parameters.AddWithValue("@pass", pass);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            u = new Usuario_E();

                            if (!dr.IsDBNull(0)) { u.DocEntry = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { u.Prefijo = dr.GetString(1); }
                            if (!dr.IsDBNull(2)) { u.Id = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { u.Nombres = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { u.Apellidos = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { u.Email = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { u.IdRol = dr.GetInt32(6); }
                            if (!dr.IsDBNull(7)) { u.Activo = dr.GetInt32(7); }
                            if (!dr.IsDBNull(8)) { u.FechaRegistro = dr.GetDateTime(8).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(9)) { u.HoraRegistro = dr.GetTimeSpan(9).ToString(); }
                            if (!dr.IsDBNull(10)) { u.OperarioRegistro = dr.GetString(10); }
                            if (!dr.IsDBNull(11)) { u.WhsCode = dr.GetString(11); }
                            if (!dr.IsDBNull(12)) { u.CodigoSap = dr.GetInt32(12); }
                            if (!dr.IsDBNull(13)) { u.ClaveEmail = dr.GetString(13); }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return u;
        }

        public List<Usuario_E> ListaUsuarios(Usuario_E filtro)
        {
            List<Usuario_E> lista = new List<Usuario_E>();
            string fil = string.Empty;

            if (filtro != null)
            {
                if (filtro.DocEntry > 0) { fil += $" AND DocEntry LIKE '%{filtro.DocEntry}%'"; }
                if (!string.IsNullOrEmpty(filtro.FechaRegistro)) { fil += $" AND FechaRegistro = '{filtro.FechaRegistro}'"; }
                if (filtro.Nombres != null) { fil += $" AND CONCAT(Nombres,' ',Apellidos) LIKE '%{filtro.Nombres}%'"; }
                if (filtro.IdRol >= 2)
                {
                    fil += Perfiles(filtro.IdRol);
                }

                if (!string.IsNullOrEmpty(filtro.Prefijo))
                {
                    if (filtro.Prefijo.Equals("ALM")) { fil += $" AND Prefijo in ('ALM','PIK','DESPACHO','RECEP')"; }
                    else { fil += $" AND Prefijo = '{filtro.Prefijo}'"; }
                }
            }

            string select = "DocEntry, Prefijo, Id, Nombres, Apellidos, Email, IdRol, Activo, CONVERT(varchar, FechaRegistro, 103) AS FechaRegistro, HoraRegistro, OpRegistro, WhsCode, CodigoSap";
            string query = $"SELECT {select} FROM dbo.OUSR WHERE IdRol >= 1 " + fil + " ORDER BY DocEntry desc ";

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand(query, cn);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Usuario_E u = new Usuario_E();
                        if (!dr.IsDBNull(0)) { u.DocEntry = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { u.Prefijo = dr.GetString(1); }
                        if (!dr.IsDBNull(2)) { u.Id = dr.GetString(2); }
                        if (!dr.IsDBNull(3)) { u.Nombres = dr.GetString(3); }
                        if (!dr.IsDBNull(4)) { u.Apellidos = dr.GetString(4); }
                        if (!dr.IsDBNull(5)) { u.Email = dr.GetString(5); }
                        if (!dr.IsDBNull(6)) { u.IdRol = dr.GetInt32(6); }
                        if (!dr.IsDBNull(7)) { u.Activo = dr.GetInt32(7); }
                        if (!dr.IsDBNull(8)) { u.FechaRegistro = dr.GetString(8); }
                        if (!dr.IsDBNull(9)) { u.HoraRegistro = dr.GetTimeSpan(9).ToString(); }
                        if (!dr.IsDBNull(10)) { u.OperarioRegistro = dr.GetString(10); }
                        if (!dr.IsDBNull(11)) { u.WhsCode = dr.GetString(11); }
                        if (!dr.IsDBNull(12)) { u.CodigoSap = dr.GetInt32(12); }
                        lista.Add(u);
                    }
                    dr.Close();
                    cn.Close();
                }
                catch { cn.Close(); }
            }

            return lista;
        }
        public List<Usuario_E> listaUsuariosPermisos(Usuario_E filtro, int idRol)
        {
            List<Usuario_E> lista = new List<Usuario_E>();
            string condWhere = string.Empty;

            if (filtro != null)
            {
                if (filtro.DocEntry > 0) { condWhere += $" AND DocEntry LIKE '%{filtro.DocEntry}%'"; }
                if (filtro.Nombres != null) { condWhere += $" AND CONCAT(Nombres, ' ', Apellidos) LIKE '%{filtro.Nombres}%'"; }
            }

            if (idRol >= 2)
            {
                Dictionary<int, string> roles = new Dictionary<int, string>
                {
                    { 2, $" AND IdRol in({idRol},3)" },						// SDT, DT
					{ 4, $" AND IdRol in({idRol},5,50,51,52,53,54)" },		// SALM, RECEP, ALM, ENC, DESPACHO, FACT
					{ 6, $" AND IdRol in({idRol},7)" },						// SVENTAS, VENTAS
					{ 8, $" AND IdRol in({idRol},9)" },						// SCAJA, CAJA
					{ 10, $" AND IdRol in({idRol})" },							// COMPRAS
					{ 11, $" AND IdRol in({idRol})" }							// SPATC
				};

                condWhere += roles[idRol];
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = @"
                                            SELECT
                                                USU.DocEntry, USU.Prefijo, USU.Id, USU.Nombres, USU.Apellidos, USU.Email, USU.Password, USU.IdRol, USU.Activo, CONVERT(varchar, USU.FechaRegistro, 103) AS FechaRegistro, USU.HoraRegistro, USU.OpRegistro, USU.WhsCode, ROL.Nombre
                                            FROM
                                                OUSR USU
                                            INNER JOIN
                                                OROL ROL ON ROL.Id = USU.IdRol
                                            WHERE
                                                1 = 1 " + condWhere + @" ORDER BY DocEntry DESC";
                SqlCommand cmd = new SqlCommand(query, cn);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Usuario_E u = new Usuario_E();
                            u.DocEntry = dr.GetInt32(0);
                            if (!dr.IsDBNull(1)) { u.Prefijo = dr.GetString(1); }
                            if (!dr.IsDBNull(2)) { u.Id = dr.GetString(2); }
                            u.Nombres = dr.GetString(3);
                            u.Apellidos = dr.GetString(4);
                            if (!dr.IsDBNull(5)) { u.Email = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { u.Password = ""; }
                            if (!dr.IsDBNull(7)) { u.IdRol = dr.GetInt32(7); }
                            if (!dr.IsDBNull(8)) { u.Activo = dr.GetInt32(8); }
                            if (!dr.IsDBNull(9)) { u.FechaRegistro = dr.GetString(9); }
                            if (!dr.IsDBNull(10)) { u.HoraRegistro = dr.GetTimeSpan(10).ToString(); }
                            if (!dr.IsDBNull(11)) { u.OperarioRegistro = dr.GetString(11); }
                            if (!dr.IsDBNull(12)) { u.WhsCode = dr.GetString(12); }
                            if (!dr.IsDBNull(13)) { u.DescripcionRol = dr.GetString(13); }

                            lista.Add(u);
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

        public string CrearUsuario(Usuario_E usu, string opRegistro)
        {
            string msj = string.Empty;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_OUSR", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "A");
                    cmd.Parameters.AddWithValue("@Prefijo", usu.Prefijo);
                    cmd.Parameters.AddWithValue("@Id", usu.Id);
                    cmd.Parameters.AddWithValue("@Nombres", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(usu.Nombres.Trim().ToLower()));
                    cmd.Parameters.AddWithValue("@Apellidos", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(usu.Apellidos.Trim().ToLower()));
                    cmd.Parameters.AddWithValue("@Email ", usu.Email);
                    cmd.Parameters.AddWithValue("@Password ", usu.Password);
                    cmd.Parameters.AddWithValue("@IdRol ", usu.IdRol);
                    cmd.Parameters.AddWithValue("@Activo ", 1);
                    cmd.Parameters.AddWithValue("@OperarioRegistro", opRegistro);
                    cmd.Parameters.AddWithValue("@WhsCode", usu.WhsCode);
                    cmd.Parameters.AddWithValue("@CodigoSap", usu.CodigoSap);
                    cmd.ExecuteNonQuery();
                    msj = "Usuario creado satisfactoriamente";
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }

            return msj;
        }

        public string editarUsuario(Usuario_E u)
        {
            string msj = string.Empty;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("dbo.MANT_OUSR", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "U");         // UPDATE
                    cmd.Parameters.AddWithValue("@DocEntry", u.DocEntry);
                    cmd.Parameters.AddWithValue("@Email", u.Email);
                    cmd.Parameters.AddWithValue("@WhsCode", u.WhsCode);
                    cmd.Parameters.AddWithValue("@CodigoSap", u.CodigoSap);
                    cmd.ExecuteNonQuery();
                    msj = "Se editó el usuario seleccionado correctamente.";
                }
                catch (SqlException e) { msj = e.Message; }
            }

            return msj;
        }

        public string eliminarUsuario(Usuario_E usu)
        {
            string msj = string.Empty;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("dbo.MANT_OUSR", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "D");
                    cmd.Parameters.AddWithValue("@DocEntry", usu.DocEntry).Direction = ParameterDirection.InputOutput;
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    msj = e.Message;
                }
                catch (Exception e2)
                {
                    throw new Exception("Error : " + e2.Message);
                }
            }

            return msj;
        }

        public Dictionary<string, string> generarId(int idRol)
        {
            Dictionary<string, string> accesoUsuario = new Dictionary<string, string>();
            if (idRol > 0)
            {
                //string id = "";
                //string query = "SELECT (@Prefijo + (CONVERT(VARCHAR,  (max(cast( SUBSTRING(id,LEN(@Prefijo)+1,4) as int))+1,1))))  " +
                //                     "FROM dbo.OUSR where idRol = @idRol and Prefijo like @Prefijo+'%' ";

                String query = "SELECT CONVERT(varchar, MAX(Id*1)+1) FROM dbo.OUSR where idRol = @idRol and Prefijo like @Prefijo+'%'";
                string query1 = "SELECT UPPER(PrefijoId) FROM dbo.OROL WHERE id=@idRol";
                try
                {
                    string prefijo = "";
                    SqlDataReader dr = db.ExecuteReaderNoSp(query1, new List<string> { "@idRol" }, idRol);
                    dr.Read();
                    prefijo = dr.GetString(0);
                    accesoUsuario.Add("prefijo", prefijo);
                    dr.Close();

                    SqlDataReader dr2 = db.ExecuteReaderNoSp(query, new List<string> { "@idRol", "@Prefijo" }, idRol, prefijo);
                    if (dr2.HasRows)
                    {
                        dr2.Read();
                        if (!dr2.IsDBNull(0))
                        {
                            accesoUsuario.Add("id", dr2.GetString(0));
                        }
                        else
                        {
                            accesoUsuario.Add("id", "1");
                        }

                    }

                    dr2.Close();
                }
                catch (SqlException e) { throw new Exception(e.Message); }
            }

            return accesoUsuario;
        }
        public Usuario_E BuscarDocEntryUsuario(string Usuario)
        {
            Usuario_E u = new Usuario_E();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT DocEntry, Nombres, Apellidos FROM dbo.OUSR where CONCAT(Prefijo, Id) = @Usuario";
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@Usuario", Usuario);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        if (!dr.IsDBNull(0)) { u.DocEntry = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { u.Nombres = dr.GetString(1); }
                        if (!dr.IsDBNull(2)) { u.Apellidos = dr.GetString(2); }
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

            return u;
        }

        public int BuscarUsuarioRol(string nombres, string apellidos, int idRol)
        {
            int result = 0;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT COUNT(DocEntry) AS TotalUsuarios FROM dbo.OUSR WHERE Nombres = @nombres AND Apellidos = @apellidos AND IdROl = @idRol";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@nombres", nombres);
                cmd.Parameters.AddWithValue("@apellidos", apellidos);
                cmd.Parameters.AddWithValue("@idRol", idRol);

                cn.Open();
                result = (int?)cmd.ExecuteScalar() ?? 0;
                cn.Close();
            }

            return result;
        }
    }
}