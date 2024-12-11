using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using Capa_Entidad;
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
                    { 2, $" AND IdRol in({IdRol},3)" },								            // SDT, DT
					{ 4, $" AND IdRol in({IdRol},5,50,51,52,53,54)" },			// SALM, RECEP,PIK, ALM, ENC, DESPACHO, FACT
					{ 6, $" AND IdRol in({IdRol},7)" },								            // SVENTAS, VENTAS
					{ 8, $" AND IdRol in({IdRol},9)" },								            // SCAJA, CAJA
					{ 10, $" AND IdRol in({IdRol})" },								            // COMPRAS
					{ 11, $" AND IdRol in({IdRol})" },								            // SPATC
					{ 55, $" AND IdRol in({IdRol})" }								            // REPA
				};

            return roles[IdRol];
        }
        public Usuario_E buscarUsuario(int DocEntry)
        {
            Usuario_E u = new Usuario_E();
            String select = "DocEntry, Prefijo, Id, Nombres, Apellidos, Email, CONVERT(VARCHAR(MAX), DECRYPTBYPASSPHRASE('pwC0B3F@R', Password)) AS Password, IdRol, Activo, CONVERT(varchar, FechaRegistro, 103) AS FechaRegistro, HoraRegistro, OpRegistro, WhsCode, CodigoSap,ClaveEmail";
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
                    if (!dr.IsDBNull(6)) { u.Password = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { u.IdRol = dr.GetInt32(7); }
                    if (!dr.IsDBNull(8)) { u.Activo = dr.GetInt32(8); }
                    if (!dr.IsDBNull(9)) { u.FechaRegistro = dr.GetString(9); }
                    if (!dr.IsDBNull(10)) { u.HoraRegistro = dr.GetTimeSpan(10).ToString(); }
                    if (!dr.IsDBNull(11)) { u.OperarioRegistro = dr.GetString(11); }
                    if (!dr.IsDBNull(12)) { u.WhsCode = dr.GetString(12); }
                    if (!dr.IsDBNull(13)) { u.CodigoSap = dr.GetInt32(13); }
                    if (!dr.IsDBNull(14)) { u.ClaveEmail = dr.GetString(14); }
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
  

            string query = @"SELECT *
        FROM VT.OTRC
        WHERE Sentido = 'Asignacion' AND DETALLE IN (
            '2000351467', '2000351863', '2000351869', '2000351874', '2000351962', 
            '2000351978', '2000352014', '2000352290', '2000352307', '2000352382', 
            '2000352477', '2000352494', '2000353066', '2000353084', '2000353199', 
            '2000353279', '2000353395', '2000353424', '2000353497', '2000353606', 
            '2000353633', '2000353646', '2000353659', '2000353687', '2000353707', 
            '2000353709', '2000353722', '2000353723', '2000353732', '2000353735', 
            '2000353742', '2000353745', '2000353746', '2000353758', '2000353766', 
            '2000353769', '2000353772', '2000353774', '2000353781', '2000353783', 
            '2000353786', '2000353799', '2000353803', '2000353808', '2000353812', 
            '2000353816', '2000353818', '2000353822', '2000353823', '2000353827', 
            '2000353832', '2000353842', '2000353848', '2000353855', '2000353857', 
            '2000353871', '2000353875', '2000353879', '2000353890', '2000353895', 
            '2000353907', '2000353920', '2000353926', '2000353935', '2000353939', 
            '2000353940', '2000353947', '2000353956', '2000353970', '2000354020', 
            '2000354076', '2000354141',      
        ) 
        ORDER BY DETALLE DESC, Sentido ASC;";

            string insertQuery = @"
INSERT INTO VT.OTRC (IdReg, RegName, CardCode, CardName, Sentido, Detalle, Cantidad, Imputado, Operario, FechaRegistro, HoraRegistro)
VALUES (@IdReg, @RegName, @CardCode, @CardName, @Sentido, @Detalle, @Cantidad, @Imputado, @Operario, @FechaRegistro, @HoraRegistro);";


            // Crear conexión
            using (SqlConnection connection = new SqlConnection(uti.cadSql))
            {
                connection.Open();

                // Ejecutar la consulta para obtener los registros
                SqlCommand selectCommand = new SqlCommand(query, connection);
                SqlDataReader reader = selectCommand.ExecuteReader();

                // Preparar la consulta para insertar los registros con 'Imputado' negativo
                SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                insertCommand.Parameters.Add(new SqlParameter("@IdReg", SqlDbType.Int));
                insertCommand.Parameters.Add(new SqlParameter("@RegName", SqlDbType.VarChar));
                insertCommand.Parameters.Add(new SqlParameter("@CardCode", SqlDbType.VarChar));
                insertCommand.Parameters.Add(new SqlParameter("@CardName", SqlDbType.VarChar));
                insertCommand.Parameters.Add(new SqlParameter("@Sentido", SqlDbType.VarChar));
                insertCommand.Parameters.Add(new SqlParameter("@Detalle", SqlDbType.VarChar));
                insertCommand.Parameters.Add(new SqlParameter("@Cantidad", SqlDbType.Decimal));
                insertCommand.Parameters.Add(new SqlParameter("@Imputado", SqlDbType.Decimal));
                insertCommand.Parameters.Add(new SqlParameter("@Operario", SqlDbType.VarChar));
                insertCommand.Parameters.Add(new SqlParameter("@FechaRegistro", SqlDbType.Date));
                insertCommand.Parameters.Add(new SqlParameter("@HoraRegistro", SqlDbType.Time));


                // Iterar sobre los resultados y insertar en la base de datos
                while (reader.Read())
                {
                    // Obtener los valores de cada registro
                    int idReg = Convert.ToInt32(reader["IdReg"]);
                    string regname = reader["RegName"].ToString();
                    string carcode = reader["CardCode"].ToString();
                    string cardname = reader["CardName"].ToString();
                    string sentido = reader["Sentido"].ToString();
                    string detalle = reader["Detalle"].ToString();
                   
                    decimal cantidad = Convert.ToDecimal(reader["Cantidad"]);
                    decimal imputado = Convert.ToDecimal(reader["Imputado"]);
                    string operario = reader["Operario"].ToString();
                    DateTime fechaRegistro = Convert.ToDateTime(reader["FechaRegistro"]);
                    TimeSpan horaRegistro = TimeSpan.Parse(reader["HoraRegistro"].ToString());

                    // Ajustar el valor de 'Imputado' a negativo
                    imputado = -imputado;
                    if (string.IsNullOrEmpty(operario))
                    {
                        operario = "Alisson Karina Romero Cabrera";
                    }


                    // Asignar los valores a los parámetros del comando INSERT
                    insertCommand.Parameters["@IdReg"].Value = idReg;
                    insertCommand.Parameters["@RegName"].Value = regname; // Asegúrate de asignar el valor correcto para RegName
                    insertCommand.Parameters["@CardCode"].Value = carcode; // Asigna el valor correcto para CardCode
                    insertCommand.Parameters["@CardName"].Value = cardname; // Asigna el valor correcto para CardName
                    insertCommand.Parameters["@Sentido"].Value = sentido;
                    insertCommand.Parameters["@Detalle"].Value = detalle;
                    insertCommand.Parameters["@Cantidad"].Value = cantidad;
                    insertCommand.Parameters["@Imputado"].Value = imputado;
                    insertCommand.Parameters["@Operario"].Value = operario;
                    insertCommand.Parameters["@FechaRegistro"].Value = fechaRegistro;
                    insertCommand.Parameters["@HoraRegistro"].Value = horaRegistro;

                    insertCommand.ExecuteNonQuery();
                }
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
                if (filtro.Activo != null) { fil += $" AND Activo={filtro.Activo}"; }
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

            string select = "DocEntry, Prefijo, Id, Nombres, Apellidos, Email, IdRol, Activo, CONVERT(varchar, FechaRegistro, 103) AS FechaRegistro, HoraRegistro, OpRegistro, WhsCode, CodigoSap, DATEDIFF(day, FechaUltimoIngreso, GETDATE()) AS DiferenciaDias ";
            string query = $"SELECT {select} FROM dbo.OUSR WHERE IdRol >= 1 " + fil + " AND Activo=1 ORDER BY DocEntry desc ";

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
                        if (!dr.IsDBNull(13)) { u.DiferenciaDias = dr.GetInt32(13); }
                        lista.Add(u);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al ejecutar consulta: " + ex.Message);
                }
                finally
                {
                    if (cn.State != ConnectionState.Closed)
                    {
                        cn.Close();
                    }
                }
            }


            return lista;
        }
        public List<Usuario_E> listaUsuariosPermisos(Usuario_E filtro, int idRol)
        {
            List<Usuario_E> lista = new List<Usuario_E>();
            string condWhere = string.Empty;

            if (filtro != null)
            {
                if (filtro.DocEntry > 0) { condWhere += $" AND DocEntry = '{filtro.DocEntry}'"; }
                if (filtro.Nombres != null) { condWhere += $" AND CONCAT(Nombres, ' ', Apellidos) LIKE '%{filtro.Nombres}%'"; }
                if (filtro.Activo > 0) { condWhere += $" AND Activo=1"; }
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
                                                USU.DocEntry, USU.Prefijo, USU.Id, USU.Nombres, USU.Apellidos, USU.Email, USU.Password, USU.IdRol, USU.Activo, CONVERT(varchar, USU.FechaRegistro, 103) AS FechaRegistro, USU.HoraRegistro, USU.OpRegistro, USU.WhsCode, ROL.Nombre, USU.CodigoSap
                                            FROM
                                                OUSR USU
                                            INNER JOIN
                                                OROL ROL ON ROL.Id = USU.IdRol
                                            WHERE
                                                1 = 1 " + condWhere + @" ORDER BY USU.Nombres";
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
                            if (!dr.IsDBNull(14)) { u.CodigoSap = dr.GetInt32(14); }

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
        public Helper_E CrearUsuario(Usuario_E usu, string opRegistro)
        {
            string mensaje = string.Empty;
            int docEntry = 0;

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
                    cmd.Parameters.AddWithValue("@EmpleadoID", usu.EmpleadoID);

                    // Agregar parámetro de salida para DocEntry
                    SqlParameter outputDocEntry = new SqlParameter("@DocEntry", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputDocEntry);

                    cmd.ExecuteNonQuery();

                    // Recuperar el valor de DocEntry
                    docEntry = (int)outputDocEntry.Value;

                    mensaje = $"Usuario: {usu.Prefijo}{usu.Id} y Contraseña: {usu.Password} creados";
                }
                catch (Exception ex)
                {
                    RegistrarError(ex, "Usuario_D - CrearUsuario");
                    mensaje = "Ocurrió un error al registrar usuario. Por favor, comunicarse con SISTEMAS.";
                }
            }

            return new Helper_E { DocEntry = docEntry, Mensaje= mensaje };
        }
        public string EditarUsuario(Usuario_E datos)
        {
            string msj = string.Empty;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();

                try
                {
                    SqlCommand cmd = new SqlCommand("dbo.MANT_OUSR", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "U");         // UPDATE
                    cmd.Parameters.AddWithValue("@DocEntry", datos.DocEntry);
                    cmd.Parameters.AddWithValue("@Email", datos.Email);
                    cmd.Parameters.AddWithValue("@Password", datos.Password);
                    cmd.Parameters.AddWithValue("@WhsCode", datos.WhsCode);
                    cmd.Parameters.AddWithValue("@CodigoSap", datos.CodigoSap);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    RegistrarError(ex, "Usuario_D - EditarUsuario");
                    msj = "Ocurrió un error al registrar usuario. Por favor, comunicarse con SISTEMAS.";
                }
            }

            return msj;
        }
        public string Inactivar(Usuario_E usu)
        {
            string msj = string.Empty;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("dbo.MANT_OUSR", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "INC");
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
        public string Activar(Usuario_E usu)
        {
            string msj = string.Empty;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand("dbo.MANT_OUSR", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "ACT");
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

        private void RegistrarError(Exception ex, string nombreArchivo)
        {
            File.AppendAllText(uti.directorioLogs + nombreArchivo + ".txt", $"{DateTime.Now}: {ex.Message}\n {ex.StackTrace}\n");
        }
    }
}