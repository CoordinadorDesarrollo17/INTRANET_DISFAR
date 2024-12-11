using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Capa_Entidad.Ventas_ENT;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Drawing;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class OREG_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public List<OREG_E> listaRegalos(OREG_E filtro)
        {
            List<OREG_E> lista = new List<OREG_E>();
            string fil = "";
            if (filtro != null)
            {
                if (filtro.Id != 0) { fil += $" AND Id = {filtro.Id}"; }
                if (filtro.Categoria != null) { fil += " and Categoria like'%" + filtro.Categoria + "%'"; }
                if (filtro.Tipo != null) { fil += " and Tipo like'" + filtro.Tipo + "%'"; }
                if (filtro.Estado != null) { fil += " and Estado  = '" + filtro.Estado + "'"; }
            }
            string query = "SELECT top 50 Id,Categoria,Tipo,Estado,StockTotal,StockDisp,StockComp,OpRegistro,FechaRegistro,HoraRegistro FROM" +
                " vt.OREG where Id>0 " + fil + " order by Id desc";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    OREG_E o = new OREG_E();
                    if (!dr.IsDBNull(0)) { o.Id = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.Categoria = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.Tipo = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.Estado = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { o.StockTotal = dr.GetDecimal(4); }
                    if (!dr.IsDBNull(5)) { o.StockDisp = dr.GetDecimal(5); }
                    if (!dr.IsDBNull(6)) { o.StockComp = dr.GetDecimal(6); }
                    if (!dr.IsDBNull(7)) { o.OpRegistro = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { o.FechaRegistro = dr.GetDateTime(8).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(9)) { o.HoraRegistro = dr.GetTimeSpan(9).ToString(); }
                    lista.Add(o);
                }
                dr.Close();
            }
            catch (Exception e) { throw new Exception("Error: " + e.Message); }
            return lista;
        }
        public void registrarNuevoRegalo(OREG_E obj)
        {
            OTRC_D otrcD = new OTRC_D();
            SqlConnection cn = new SqlConnection(uti.cadSql);
            int auxId;
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_OREG", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "A");
                    cmd.Parameters.AddWithValue("@Id", obj.Id).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@Categoria", obj.Categoria);
                    cmd.Parameters.AddWithValue("@Tipo", obj.Tipo);
                    cmd.Parameters.AddWithValue("@Estado", obj.Estado);
                    cmd.Parameters.AddWithValue("@StockTotal", obj.StockTotal);
                    cmd.Parameters.AddWithValue("@StockDisp", obj.StockDisp);
                    cmd.Parameters.AddWithValue("@StockComp", obj.StockComp);
                    cmd.Parameters.AddWithValue("@OpRegistro", obj.OpRegistro);

                    cmd.ExecuteNonQuery();

                    auxId = (int)cmd.Parameters["@Id"].Value;
                    //post transacciones
                    SqlCommand cmd2 = new SqlCommand("POST_TRANSACCIONES", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "OREG");
                    cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@Id"].Value);
                    cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@Id"].Value);
                    cmd2.ExecuteNonQuery();

                    //Revisado
                    otrcD.registrarTransaccion(new OTRC_E()
                    {
                        IdReg = auxId,
                        RegName = obj.Categoria + " " + obj.Tipo,
                        Sentido = "Entrada",
                        Detalle = "Saldo Inicial",
                        Cantidad = obj.StockTotal,
                        Operario = obj.OpRegistro
                    }, tran);

                    tran.Commit();
                }
                catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: " + e.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }

        }
        public OREG_E buscarRegalo(int id)
        {
            OREG_E o = new OREG_E();
            string query = "select Id,Categoria,Tipo,Estado,StockTotal,StockDisp, StockComp,OpRegistro,FechaRegistro,HoraRegistro from vt.OREG where Id=@Id";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string>() { "@Id" }, id);
                dr.Read();
                if (!dr.IsDBNull(0)) { o.Id = dr.GetInt32(0); }
                if (!dr.IsDBNull(1)) { o.Categoria = dr.GetString(1); }
                if (!dr.IsDBNull(2)) { o.Tipo = dr.GetString(2); }
                if (!dr.IsDBNull(3)) { o.Estado = dr.GetString(3); }
                if (!dr.IsDBNull(4)) { o.StockTotal = dr.GetDecimal(4); }
                if (!dr.IsDBNull(5)) { o.StockDisp = dr.GetDecimal(5); }
                if (!dr.IsDBNull(6)) { o.StockComp = dr.GetDecimal(6); }
                if (!dr.IsDBNull(7)) { o.OpRegistro = dr.GetString(7); }
                if (!dr.IsDBNull(8)) { o.FechaRegistro = dr.GetDateTime(8).ToString("yyyy-MM-dd"); }
                if (!dr.IsDBNull(9)) { o.HoraRegistro = dr.GetDateTime(9).ToString("HH:mm:ss"); }
                dr.Close();
            }
            catch { }
            return o;
        }
        public int inactivarRegalo(OREG_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_OREG", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "IR");
                    cmd.Parameters.AddWithValue("@Id", obj.Id);
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@Id"].Value.ToString());
                    tran.Commit();
                }
                catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: " + e.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return status;
        }
        public int revertirInactivarRegalo(OREG_E obj)
        {
            int status = -1;
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_OREG", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "RIR");
                    cmd.Parameters.AddWithValue("@Id", obj.Id);
                    cmd.ExecuteNonQuery();
                    status = int.Parse(cmd.Parameters["@Id"].Value.ToString());
                    tran.Commit();
                }
                catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: " + e.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }
            return status;
        }

        //revisado
        public void RegistrarGestionStock(OREG_E reg, OTRC_E obj, SqlTransaction tran = null)
        {
            bool status = false;
            OTRC_D otrcD = new OTRC_D();

            // Verificamos si se ha proporcionado una transacción
            if (tran != null)
            {
                // Si la transacción no es nula, utilizamos la conexión asociada a esta transacción
                using (SqlCommand cmd = new SqlCommand("vt.MANT_OREG", tran.Connection, tran))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Agregamos los parámetros necesarios
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "US"); // Actualizar stock disponible
                    cmd.Parameters.AddWithValue("@Id", reg.Id).Direction = ParameterDirection.InputOutput;
                    cmd.Parameters.AddWithValue("@StockDisp", reg.StockDisp);

                    try
                    {
                        // Ejecutamos el comando usando la transacción proporcionada
                        cmd.ExecuteNonQuery();

                        // Registrar transacción de stock
                        otrcD.registrarTransaccion(obj, tran);

                        status = true;  // Indicamos que todo salió bien
                    }
                    catch (Exception e)
                    {
                        // En caso de error, hacemos rollback
                        tran.Rollback();
                        status = false;
                        throw new Exception("Error en creación de compromiso de stock: " + e.Message, e);
                    }
                }
            }
            else
            {
                // Si no se pasa una transacción, creamos una nueva conexión y transacción
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    cn.Open();
                    SqlTransaction transaction = cn.BeginTransaction();

                    try
                    {
                        // Ejecutamos el comando usando la nueva transacción
                        using (SqlCommand cmd = new SqlCommand("vt.MANT_OREG", cn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Agregamos los parámetros necesarios
                            cmd.Parameters.AddWithValue("@TipoMantenimiento", "US"); // Actualizar stock disponible
                            cmd.Parameters.AddWithValue("@Id", reg.Id).Direction = ParameterDirection.InputOutput;
                            cmd.Parameters.AddWithValue("@StockDisp", reg.StockDisp);

                            cmd.ExecuteNonQuery();
                        }

                        // Registrar transacción de stock
                        otrcD.registrarTransaccion(obj, transaction);

                        status = true;  // Indicamos que todo salió bien

                        // Confirmamos la transacción si fue creada dentro del método
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        // Hacemos rollback si ocurrió un error
                        transaction.Rollback();
                        status = false;
                        throw new Exception("Error en creación de compromiso de stock: " + e.Message, e);
                    }
                }
            }
        }
        public void RegistroComprometidos(DataTable tablaDatos, SqlTransaction tran = null)
        {
            using (SqlCommand cmd = new SqlCommand("vt.MANT_OREG", tran.Connection, tran))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 120;

                cmd.Parameters.AddWithValue("@TipoMantenimiento", "USCX");
                cmd.Parameters.AddWithValue("@Id", 0).Direction = ParameterDirection.InputOutput;
                SqlParameter param = cmd.Parameters.AddWithValue("@TablaDatos", tablaDatos);
                param.SqlDbType = SqlDbType.Structured;

                cmd.ExecuteNonQuery();
            }
        }

        public void CompromisosStock(List<ORTV_E> listaTickets, SqlTransaction tran)
        {
            OTRC_D otrcD = new OTRC_D();
            OCLR_D oclrD = new OCLR_D();
            bool status = false;

            try
            {
                DataTable tablaDatos = new DataTable();
                tablaDatos.Columns.Add("IdReg", typeof(int));
                tablaDatos.Columns.Add("StockComp", typeof(decimal));


                DataTable tablaDatos2 = new DataTable();
                tablaDatos2.Columns.Add("IdReg", typeof(int));
                tablaDatos2.Columns.Add("RegName", typeof(string));
                tablaDatos2.Columns.Add("CardCode", typeof(string));
                tablaDatos2.Columns.Add("CardName", typeof(string));
                tablaDatos2.Columns.Add("Sentido", typeof(string)); //Asignacion
                tablaDatos2.Columns.Add("Detalle", typeof(string));
                tablaDatos2.Columns.Add("Cantidad", typeof(decimal));
                tablaDatos2.Columns.Add("Imputado", typeof(decimal));
                tablaDatos2.Columns.Add("Operario", typeof(string));


                DataTable tablaDatos3 = new DataTable();
                tablaDatos3.Columns.Add("CardCode", typeof(string));
                tablaDatos3.Columns.Add("IdReg", typeof(int));
                tablaDatos3.Columns.Add("Cantidad", typeof(decimal));

                decimal cantidadDevolviendo = 0;
                foreach (var ticket in listaTickets)
                {
                    if (ticket.Det5.Any())
                    {
                        foreach (var regalo in ticket.Det5)
                        {
                            if (regalo.RegCant < 0) { cantidadDevolviendo = regalo.RegCant; }
                            else {
                                // Comprobar si el cliente tiene saldo suficiente considerando si se ha devuelvo en esta misma transaccion
                                if (!oclrD.ComprobarDispCliReg(new CLR1_E()
                                {
                                    CardCode = ticket.CardCode,
                                    IdReg = regalo.IdReg,
                                    Cantidad =  cantidadDevolviendo + regalo.RegCant
                                }))
                                {
                                    throw new Exception("El cliente no tiene saldo suficiente.");
                                }
                             }

                            tablaDatos.Rows.Add(regalo.IdReg,
                                regalo.RegCant);

                            tablaDatos2.Rows.Add(
                                regalo.IdReg,
                                regalo.RegCate + " " + regalo.RegTipo,
                                ticket.CardCode,
                                ticket.CardName,
                                "Asignacion",
                                ticket.DocNum.ToString(),
                                0,
                                regalo.RegCant,
                                ticket.Vendedor
                                );

                            tablaDatos3.Rows.Add(
                                ticket.CardCode,
                                regalo.IdReg,
                                regalo.RegCant
                                );

                        }
                    }
                }


                RegistroComprometidos(tablaDatos, tran);

                // Registrar la transacción de stock
                otrcD.registrarTransaccionDataTable(tablaDatos2, tran);

                // Registrar el compromiso con el cliente
                oclrD.CompromisoClienteRegaloDataTable(tablaDatos3, tran);

                status = true;

            }
            catch (Exception e)
            {
                status = false;
                throw new Exception("Error en creación de compromiso de stock: " + e.Message, e);
            }
        }

    }
}
