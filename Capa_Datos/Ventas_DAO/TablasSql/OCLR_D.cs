using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sap.Data.Hana;
namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class OCLR_D
    {
        Utilitarios uti = new Utilitarios(); 
        DBHelper db = new DBHelper();
        CLR1_D clr1D = new CLR1_D();
        public List<OCLR_E> listadoRegaloCliente(OCLR_E filtro)
        {
            List<OCLR_E> lista = new List<OCLR_E>();
            string fil = "";
            if (filtro != null)
            {
                if (filtro.CardCode != null) { fil += " and CardCode like'%" + filtro.CardCode + "%'"; }
                if (filtro.CardName != null) { fil += " and CardName like'%" + filtro.CardName + "%'"; }
                if (filtro.NroOpe > 0) { fil += " and NroOpe ='" + filtro.NroOpe + "'"; }
                if (((int)filtro.Saldo) > 0) { fil += " and Saldo = " + filtro.Saldo + ""; }
            }

            string query = "select CardCode,CardName,NroOpe,Saldo from vt.OCLR where 1=1 " + fil;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    OCLR_E o = new OCLR_E();
                    if (!dr.IsDBNull(0)) { o.CardCode = dr.GetString(0); }
                    if (!dr.IsDBNull(1)) { o.CardName = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.NroOpe = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { o.Saldo = dr.GetDecimal(3); }
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
        public OCLR_E buscarClienteRegalo(string CardCode)
        {
            OCLR_E o = new OCLR_E();
            o.Det = new List<CLR1_E>();
            string query = "select CardCode,CardName,NroOpe,Saldo from vt.OCLR where CardCode=@CardCode";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string>() { "@CardCode" }, CardCode);
                dr.Read();
                if (!dr.IsDBNull(0)) { o.CardCode = dr.GetString(0); }
                if (!dr.IsDBNull(1)) { o.CardName = dr.GetString(1); }
                if (!dr.IsDBNull(2)) { o.NroOpe = dr.GetInt32(2); }
                if (!dr.IsDBNull(3)) { o.Saldo = dr.GetDecimal(3); }
                o.Det = clr1D.listarDetallesCli(CardCode);
                dr.Close();
            }
            catch { }
            return o;
        }
        //Revisado
        public void registrarClienteRegalo(OCLR_E obj)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlTransaction tran = cn.BeginTransaction("TRANSACCION DE INSERT EN OCLR"))
                    {
                        try
                        {
                            SqlCommand cmd = new SqlCommand("vt.MANT_OCLR", cn, tran)
                            {
                                CommandType = CommandType.StoredProcedure
                            };

                            cmd.Parameters.AddWithValue("@TipoMantenimiento", "A");
                            cmd.Parameters.AddWithValue("@CardCode", obj.CardCode);
                            cmd.Parameters.AddWithValue("@CardName", obj.CardName);
                            cmd.Parameters.AddWithValue("@NroOpe", obj.NroOpe);

                            SqlParameter tbDet = new SqlParameter("@TPCLR1", SqlDbType.Structured)
                            {
                                Value = CLR1_E.GenerarDataTable(obj.Det),
                                TypeName = "dbo.TPCLR1"
                            };
                            cmd.Parameters.AddWithValue("@TPCLR1", tbDet.Value);

                            decimal saldo = CalcularSaldo(obj.Det);
                            cmd.Parameters.AddWithValue("@Saldo", saldo);

                            cmd.ExecuteNonQuery();
                            tran.Commit();
                        }
                        catch (Exception e)
                        {
                            tran.Rollback();
                            throw new Exception("Error en creación de cliente regalo: " + e.Message);
                        }
                    }
                }
                catch (Exception e2)
                {
                    throw new Exception("Error en creación y conexión: " + e2.Message);
                }
            }
        }
        //Agregado recientemente para registrar y editar
        private decimal CalcularSaldo(IEnumerable<CLR1_E> detalles)
        {
            decimal cantidadTotal = 0;
            foreach (CLR1_E detObj in detalles)
            {
                cantidadTotal += detObj.Cantidad ?? 0;
            }
            return cantidadTotal;
        }
        //Revisado
        public void editarClienteRegalo(OCLR_E obj)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                try
                {
                    cn.Open();
                    using (SqlTransaction tran = cn.BeginTransaction("TRANSACCION DE UPDATE EN OCLR"))
                    {
                        try
                        {
                            SqlCommand cmd = new SqlCommand("vt.MANT_OCLR", cn, tran)
                            {
                                CommandType = CommandType.StoredProcedure
                            };

                            cmd.Parameters.AddWithValue("@TipoMantenimiento", "U");
                            cmd.Parameters.AddWithValue("@CardCode", obj.CardCode);

                            SqlParameter tbDet = new SqlParameter("@TPCLR1", SqlDbType.Structured)
                            {
                                Value = CLR1_E.GenerarDataTable(obj.Det),
                                TypeName = "dbo.TPCLR1"
                            };
                            cmd.Parameters.AddWithValue("@TPCLR1", tbDet.Value);

                            decimal saldo = CalcularSaldo(obj.Det);
                            cmd.Parameters.AddWithValue("@Saldo", saldo);

                            cmd.ExecuteNonQuery();
                            tran.Commit();
                        }
                        catch (Exception e)
                        {
                            tran.Rollback();
                            throw new Exception("Error en actualización de cliente regalo: " + e.Message);
                        }
                    }
                }
                catch (Exception e2)
                {
                    throw new Exception("Error en conexión o transacción: " + e2.Message);
                }
            }
        }
        //Revisado
        //public void CompromisoClienteRegalo(CLR1_E obj, SqlTransaction tran)
        //{
        //    string query = "UPDATE vt.CLR1 SET Cantidad = Cantidad - @Cantidad WHERE CardCode = @CardCode AND IdReg = @IdReg; UPDATE vt.OCLR SET Saldo=Saldo - @Cantidad  WHERE CardCode = @CardCode ;";

        //    try
        //    {
        //        using (SqlCommand cmd = new SqlCommand(query, tran.Connection, tran)) 
        //        {
        //            cmd.CommandType = CommandType.Text;
        //            cmd.CommandTimeout = 120;
        //            cmd.Parameters.AddWithValue("@CardCode", obj.CardCode);
        //            cmd.Parameters.AddWithValue("@IdReg", obj.IdReg);
        //            cmd.Parameters.AddWithValue("@Cantidad", obj.Cantidad);

        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception($"Error al procesar compromiso para el cliente con CardCode: {obj.CardCode} y IdReg: {obj.IdReg}. Detalle: {e.Message}", e);
        //    }
        //}
        public void CompromisoClienteRegaloDataTable(DataTable tablaDatos3, SqlTransaction tran)
        {
            string query = @"
        UPDATE vt.CLR1 
        SET Cantidad = Cantidad - @Cantidad 
        WHERE CardCode = @CardCode AND IdReg = @IdReg;
        
        UPDATE vt.OCLR 
        SET Saldo = Saldo - @Cantidad 
        WHERE CardCode = @CardCode;";

            try
            {
                using (SqlCommand cmd = new SqlCommand(query, tran.Connection, tran))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 120;

                    // Añadimos los parámetros pero sin valores iniciales
                    cmd.Parameters.Add("@CardCode", SqlDbType.NVarChar, 50);
                    cmd.Parameters.Add("@IdReg", SqlDbType.Int);
                    cmd.Parameters.Add("@Cantidad", SqlDbType.Decimal);

                    // Iterar sobre las filas del DataTable
                    foreach (DataRow row in tablaDatos3.Rows)
                    {
                        // Establecer valores de los parámetros desde la fila
                        cmd.Parameters["@CardCode"].Value = row["CardCode"];
                        cmd.Parameters["@IdReg"].Value = row["IdReg"];
                        cmd.Parameters["@Cantidad"].Value = row["Cantidad"];

                        // Ejecutar el comando
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error al procesar compromisos para los clientes. Detalle: " + e.Message, e);
            }
        }

        public bool ComprobarDispCliReg(CLR1_E obj)
        {
            bool r = false;
            int idReg = obj.IdReg ?? default(int);
            CLR1_E auxCliReg = clr1D.buscarDetCliReg(obj.CardCode, idReg);
            if (auxCliReg.Cantidad >= obj.Cantidad)
            {
                r = true;
            }
            return r;
        }

    }
}