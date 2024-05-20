using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class ORDR_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<ORDR_E> ListarPedidosOnline(ORDR_E pedido)
        {
            string condWhere = string.Empty;
            string selectTop = "TOP 100";
            bool verDetPed = false;

            List<ORDR_E> lista = new List<ORDR_E>();

            if (pedido != null && pedido.Id > 0)
            {
                condWhere += $" AND Id = {pedido.Id}";
                selectTop = "TOP 1";
                verDetPed = true;
            }

            if (pedido != null && !string.IsNullOrEmpty(pedido.CardName))
            {
                condWhere += $" AND Cardname LIKE '%{pedido.CardName}%'";
            }

            if (pedido != null && !string.IsNullOrEmpty(pedido.Vendedor))
            {
                condWhere += $" AND Vendedor LIKE '%{pedido.Vendedor}%'";
            }

            if (pedido != null && !string.IsNullOrEmpty(pedido.Estado))
            {
                condWhere += $" AND Estado = '{pedido.Estado}'";
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = $"SELECT {selectTop} Id, DocEntryTicket, DocNumTicket, CardCode, CardName, Estado, WhsCode, Vendedor, Comentario, CONVERT(varchar, FechaCreacion, 103), HoraCreacion, VendedorRecibido, CONVERT(varchar, FechaRecibido, 103), HoraRecibido, VendedorCancelado, CONVERT(varchar, FechaCancelado, 103), HoraCancelado FROM vt.ORDR WHERE Id>0 {condWhere} ORDER BY Id DESC";

                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ORDR_E ordr = new ORDR_E();

                            if (!dr.IsDBNull(0)) { ordr.Id = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { ordr.DocEntryTicket = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { ordr.DocNumTicket = dr.GetInt32(2); }
                            if (!dr.IsDBNull(3)) { ordr.CardCode = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { ordr.CardName = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { ordr.Estado = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { ordr.WhsCode = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { ordr.Vendedor = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { ordr.Comentario = dr.GetString(8); }
                            if (!dr.IsDBNull(9)) { ordr.FechaCreacion = dr.GetString(9); }
                            if (!dr.IsDBNull(10)) { ordr.HoraCreacion = dr.GetTimeSpan(10).ToString(); }
                            if (!dr.IsDBNull(11)) { ordr.VendedorRecibido = dr.GetString(11); }
                            if (!dr.IsDBNull(12)) { ordr.FechaRecibido = dr.GetString(12); }
                            if (!dr.IsDBNull(13)) { ordr.HoraRecibido = dr.GetTimeSpan(13).ToString(); }
                            if (!dr.IsDBNull(14)) { ordr.VendedorCancelado = dr.GetString(14); }
                            if (!dr.IsDBNull(15)) { ordr.FechaCancelado = dr.GetString(15); }
                            if (!dr.IsDBNull(16)) { ordr.HoraCancelado = dr.GetTimeSpan(16).ToString(); }

                            if (verDetPed)
                            {
                                ordr.DetallePedido = ListarDetallePedidoOnline(dr.GetInt32(0));
                            }

                            lista.Add(ordr);
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

        public List<RDR1_E> ListarDetallePedidoOnline(int IdORDR)
        {
            List<RDR1_E> listaDetPed = new List<RDR1_E>();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string queryDetPed = $"SELECT ItemCode, ItemName, BatchNum, ExpDate, UMVenta, UndMed, Quantity AS 'Stock', Cantidad, Price, (Price*Cantidad) AS PrecioxCantidad FROM vt.RDR1 WHERE IdORDR = @IdORDR";
                SqlCommand cmd = new SqlCommand(queryDetPed, cn);         // prepara
                cmd.Parameters.AddWithValue("@IdORDR", IdORDR);

                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();                               // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            RDR1_E rdr1 = new RDR1_E();

                            if (!dr.IsDBNull(0)) { rdr1.ItemCode = dr.GetString(0); }
                            if (!dr.IsDBNull(1)) { rdr1.ItemName = dr.GetString(1); }
                            if (!dr.IsDBNull(2)) { rdr1.BatchNum = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { rdr1.ExpDate = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { rdr1.UMVenta = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { rdr1.UndMed = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { rdr1.Quantity = dr.GetDecimal(6); }
                            if (!dr.IsDBNull(7)) { rdr1.Cantidad = dr.GetInt32(7); }
                            if (!dr.IsDBNull(8)) { rdr1.Price = dr.GetDecimal(8); }
                            if (!dr.IsDBNull(9)) { rdr1.PrecioxCantidad = dr.GetDecimal(9); }

                            listaDetPed.Add(rdr1);
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

            return listaDetPed;
        }

        public int RegistrarPedidoOnline(ORDR_E pedido, List<Capa_Entidad.Almacen_ENT.TablasSql.OIBT_E> DetallePedido)
        {
            int status = -1;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_ORDR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (pedido.EstadoInicialCreacion == "GENERAR")
                    {
                        cmd.Parameters.AddWithValue("@Accion", "INS");
                    }
                    else if (pedido.EstadoInicialCreacion == "BORRADOR")
                    {
                        cmd.Parameters.AddWithValue("@Accion", "BOR");
                    }

                    cmd.Parameters.AddWithValue("@CardCode", pedido.CardCode);
                    cmd.Parameters.AddWithValue("@CardName", pedido.CardName);
                    cmd.Parameters.AddWithValue("@WhsCode", pedido.WhsCode);
                    cmd.Parameters.AddWithValue("@CodSapVendedor", pedido.CodSapVendedor);
                    cmd.Parameters.AddWithValue("@Vendedor", pedido.Vendedor);
                    cmd.Parameters.AddWithValue("@Comentario", pedido.Comentario);
                    cmd.Parameters.Add("@Id", SqlDbType.Int);
                    cmd.Parameters["@Id"].Direction = ParameterDirection.Output;

                    if (pedido.EstadoInicialCreacion == "GENERAR")
                    {
                        cmd.Parameters.Add("@DocEntryTicket", SqlDbType.Int);
                        cmd.Parameters["@DocEntryTicket"].Direction = ParameterDirection.Output;

                        cmd.Parameters.Add("@DocNumTicket", SqlDbType.Int);
                        cmd.Parameters["@DocNumTicket"].Direction = ParameterDirection.Output;
                    }

                    cmd.ExecuteNonQuery();
                    status = Convert.ToInt32(cmd.Parameters["@Id"].Value.ToString());           // (select DocEntry from GENE where Tabla='ORDR')

                    SqlCommand cmd2 = new SqlCommand("dbo.POST_TRANSACCIONES", cn);
                    cmd2.Transaction = tran;
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "ORDR");
                    cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@Id"].Value);
                    cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@Id"].Value);
                    cmd2.ExecuteNonQuery();

                    if (pedido.EstadoInicialCreacion == "GENERAR")
                    {
                        SqlCommand cmd3 = new SqlCommand("dbo.POST_TRANSACCIONES", cn);
                        cmd3.Transaction = tran;
                        cmd3.CommandType = CommandType.StoredProcedure;
                        cmd3.Parameters.AddWithValue("@Tipo", "A");
                        cmd3.Parameters.AddWithValue("@Tabla", "ORTV");
                        cmd3.Parameters.AddWithValue("@DocNum", Convert.ToInt32(cmd.Parameters["@DocNumTicket"].Value.ToString()));
                        cmd3.Parameters.AddWithValue("@DocEntry", Convert.ToInt32(cmd.Parameters["@DocEntryTicket"].Value.ToString()));
                        cmd3.ExecuteNonQuery();
                    }


                    if (DetallePedido != null && DetallePedido.Count >= 1)
                    {
                        foreach (var det in DetallePedido)
                        {
                            string query2 = $"INSERT INTO vt.RDR1 VALUES (@IdORDR, @ItemCode, @ItemName, @BatchNum, @ExpDate, @UMVenta, @UndMed, @Quantity, @Cantidad, @Price)";

                            cmd2.Transaction = tran;
                            try
                            {
                                SqlCommand cmd4 = new SqlCommand(query2, cn, tran);
                                cmd4.Parameters.AddWithValue("@IdORDR", status);
                                cmd4.Parameters.AddWithValue("@ItemCode", det.ItemCode);
                                cmd4.Parameters.AddWithValue("@ItemName", det.ItemName);
                                cmd4.Parameters.AddWithValue("@BatchNum", det.BatchNum);
                                cmd4.Parameters.AddWithValue("@ExpDate", det.ExpDate);
                                cmd4.Parameters.AddWithValue("@UMVenta", det.UMVenta);
                                cmd4.Parameters.AddWithValue("@UndMed", det.UndMed);
                                cmd4.Parameters.AddWithValue("@Quantity", det.Quantity);
                                cmd4.Parameters.AddWithValue("@Cantidad", det.Cantidad);
                                cmd4.Parameters.AddWithValue("@Price", det.Price);
                                cmd4.ExecuteNonQuery();
                            }
                            catch (Exception e1)
                            {
                                tran.Rollback();
                                throw new Exception("Error en registro: " + e1.Message);
                            }
                        }
                    }


                    tran.Commit();
                }
                catch (Exception ex)
                {
                    status = 0; tran.Rollback();
                    throw new Exception("Error en registro: " + ex.Message);
                }
                finally
                {
                    cn.Close();
                }
            }

            return status;
        }

        public int EditarPedidoOnline(int idORDR, List<Capa_Entidad.Almacen_ENT.TablasSql.OIBT_E> DetallePedido)
        {
            int status = 0;

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_ORDR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Accion", "UPD");
                    cmd.Parameters.AddWithValue("@Id", idORDR);
                    cmd.Parameters.Add("@DocEntryTicket", SqlDbType.Int);
                    cmd.Parameters["@DocEntryTicket"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@DocNumTicket", SqlDbType.Int);
                    cmd.Parameters["@DocNumTicket"].Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();


                    SqlCommand cmd3 = new SqlCommand("dbo.POST_TRANSACCIONES", cn);
                    cmd3.Transaction = tran;
                    cmd3.CommandType = CommandType.StoredProcedure;
                    cmd3.Parameters.AddWithValue("@Tipo", "A");
                    cmd3.Parameters.AddWithValue("@Tabla", "ORTV");
                    cmd3.Parameters.AddWithValue("@DocNum", Convert.ToInt32(cmd.Parameters["@DocNumTicket"].Value.ToString()));
                    cmd3.Parameters.AddWithValue("@DocEntry", Convert.ToInt32(cmd.Parameters["@DocEntryTicket"].Value.ToString()));
                    cmd3.ExecuteNonQuery();

                    string query4 = $"DELETE FROM vt.RDR1 WHERE IdORDR = {idORDR}";

                    try
                    {
                        SqlCommand cmd4 = new SqlCommand(query4, cn, tran);
                        cmd4.ExecuteNonQuery();
                    }
                    catch (Exception e1)
                    {
                        tran.Rollback();
                        throw new Exception("Error en registro: " + e1.Message);
                    }

                    if (DetallePedido != null && DetallePedido.Count >= 1)
                    {
                        foreach (var det in DetallePedido)
                        {
                            string query = "INSERT INTO vt.RDR1 VALUES (@IdORDR, @ItemCode, @ItemName, @BatchNum, @ExpDate, @UmVenta, @UndMed, @Quantity, @Cantidad, @Price)";

                            try
                            {
                                SqlCommand cmd2 = new SqlCommand(query, cn, tran);
                                cmd2.Parameters.AddWithValue("@IdORDR", idORDR);
                                cmd2.Parameters.AddWithValue("@ItemCode", det.ItemCode);
                                cmd2.Parameters.AddWithValue("@ItemName", det.ItemName);
                                cmd2.Parameters.AddWithValue("@BatchNum", det.BatchNum);
                                cmd2.Parameters.AddWithValue("@ExpDate", det.ExpDate);
                                cmd2.Parameters.AddWithValue("@UmVenta", det.UMVenta);
                                cmd2.Parameters.AddWithValue("@UndMed", det.UndMed);
                                cmd2.Parameters.AddWithValue("@Quantity", det.Quantity);
                                cmd2.Parameters.AddWithValue("@Cantidad", det.Cantidad);
                                cmd2.Parameters.AddWithValue("@Price", det.Price);
                                cmd2.ExecuteNonQuery();
                                status = 1;
                            }
                            catch (Exception e1)
                            {
                                tran.Rollback();
                                throw new Exception("Error en registro: " + e1.Message);
                            }
                        }
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    status = 0; tran.Rollback();
                    throw new Exception("Error en registro: " + ex.Message);
                }
                finally
                {
                    cn.Close();
                }

            }

            return status;
        }

        public string CambiarEstadoPedidoOnline(ORDR_E Pedido, string Accion)
        {
            string mensaje = string.Empty;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_ORDR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Accion", Accion);

                    if (Accion.Equals("REC"))
                    {
                        cmd.Parameters.AddWithValue("@VendedorRecibido", Pedido.VendedorRecibido);
                    }
                    else if (Accion.Equals("CAN"))
                    {
                        cmd.Parameters.AddWithValue("@DocEntryTicket", Pedido.DocEntryTicket);
                        cmd.Parameters.AddWithValue("@VendedorCancelado", Pedido.VendedorCancelado);
                    }

                    cmd.Parameters.AddWithValue("@Estado", Pedido.Estado);
                    cmd.Parameters.AddWithValue("@Id", Pedido.Id);

                    cmd.ExecuteNonQuery();
                    mensaje = $"Cambio de estado del pedido con éxito";

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    mensaje = "Lo sentimos, hubo un error al cambiar de estado"; tran.Rollback();
                    throw new Exception("Error en registro: " + ex.Message);
                }
                finally
                {
                    cn.Close();
                }
            }

            return mensaje;
        }
    }
}
