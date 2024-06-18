using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class ORPD_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<ORPD_E> ListarDevoluciones(ORPD_E Filtros)
        {
            List<ORPD_E> lista = new List<ORPD_E>();
            string condWhere = string.Empty, join = string.Empty;

            if (Filtros != null)
            {
                if (Filtros.DocEntry >= 1)
                {
                    condWhere += $" AND DEV.DocEntry = '{Filtros.DocEntry}' ";
                }

                if (Filtros.DocNum >= 1)
                {
                    condWhere += $" AND DEV.DocNum = '{Filtros.DocNum}' ";
                }

                if (!string.IsNullOrEmpty(Filtros.CardName))
                {
                    condWhere += $" AND DEV.CardName LIKE '%{Filtros.CardName}%' ";
                }

                if (!string.IsNullOrEmpty(Filtros.Estado))
                {
                    condWhere += $" AND DEV.Estado = '{Filtros.Estado}' ";
                }
                if (!string.IsNullOrEmpty(Filtros.WhsCode))
                {
                    condWhere += $" AND DEV.WhsCode = '{Filtros.WhsCode}' ";
                }
                if (Filtros.DetalleDevolucion != null && Filtros.DetalleDevolucion.Count >= 1 && Filtros.DetalleDevolucion[0].RefFactura != null)
                {
                    join += " INNER JOIN al.RPD1 DET on DET.DocEntry = DEV.DocEntry";
                    condWhere += $" AND DET.RefFactura = '{Filtros.DetalleDevolucion[0].RefFactura}' ";
                }
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = $"SELECT DISTINCT TOP 100 DEV.DocEntry, DEV.DocNum, CONVERT(varchar,DEV.FechaDevolucion,103) AS 'FechaDevolucion',CONVERT(varchar, DEV.HoraDevolucion,8), DEV.Correlativo, DEV.WhsCode, DEV.CardCode, DEV.CardName, DEV.Estado, DEV.RetiroMercado, DEV.Correo, DEV.TiempoCorreoEnviado , DEV.Comentario FROM al.ORPD DEV {join} WHERE 1=1 {condWhere} ORDER BY DEV.DocEntry DESC";
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                                                                    //cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            ORPD_E dev = new ORPD_E();
                            CC_ORPD_D cC_ORPD_D = new CC_ORPD_D();
                            if (!dr.IsDBNull(0)) { dev.DocEntry = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { dev.DocNum = dr.GetInt32(1); }
                            if (!dr.IsDBNull(2)) { dev.FechaDevolucion = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { dev.HoraDevolucion = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { dev.Correlativo = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { dev.WhsCode = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { dev.CardCode = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { dev.CardName = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { dev.Estado = dr.GetString(8); }
                            if (!dr.IsDBNull(9)) { dev.RetiroMercado = dr.GetBoolean(9); }
                            if (!dr.IsDBNull(10)) { dev.Correo = dr.GetString(10); }
                            if (!dr.IsDBNull(11)) { dev.TiempoCorreoEnviado = dr.GetDateTime(11).ToString("yyyy-MM-dd"); }
                            if (!dr.IsDBNull(12)) { dev.Comentario = dr.GetString(12); }
                            /*** C O N T R O L  D E  C A M B I O S ***/
                            CC_ORPD_E ccDev = new CC_ORPD_E
                            {
                                DocEntry = dev.DocEntry,
                                Operacion = "REGISTRAR"
                            };

                            var listaCC = cC_ORPD_D.ListarCC_ORPD(ccDev);

                            if (listaCC != null && listaCC.Count() >= 1)
                            {
                                dev.Operario = listaCC[0].Operario;
                                dev.FechaOperacion = listaCC[0].FechaOperacion;     // Registro

                                lista.Add(dev);     // solo se agregará a la lista los que cumplen con los filtros enviados
                            }

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

        public ORPD_E ObtenerDevolucion(int DocEntry)
        {
            ORPD_E dev = new ORPD_E();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "SELECT DocEntry, DocNum, CONVERT(varchar,FechaDevolucion,103), CONVERT(varchar,HoraDevolucion,8 ),Correlativo, WhsCode, CardCode, CardName, Estado, RetiroMercado, Correo, TiempoCorreoEnviado, SinEM FROM al.ORPD WHERE DocEntry = @DocEntry";
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta
                    dr.Read();

                    CC_ORPD_D cC_ORPD_D = new CC_ORPD_D();
                    RPD1_D rpd1_D = new RPD1_D();

                    if (!dr.IsDBNull(0)) { dev.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { dev.DocNum = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { dev.FechaDevolucion = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { dev.HoraDevolucion = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { dev.Correlativo = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { dev.WhsCode = dr.GetString(5); }
                    if (!dr.IsDBNull(6)) { dev.CardCode = dr.GetString(6); }
                    if (!dr.IsDBNull(7)) { dev.CardName = dr.GetString(7); }
                    if (!dr.IsDBNull(8)) { dev.Estado = dr.GetString(8); }
                    if (!dr.IsDBNull(9)) { dev.RetiroMercado = dr.GetBoolean(9); }
                    if (!dr.IsDBNull(10)) { dev.Correo = dr.GetString(10); }
                    if (!dr.IsDBNull(11)) { dev.TiempoCorreoEnviado = Convert.ToDateTime(dr.GetDateTime(11)).ToString("dd/MM/yyyy HH:mm:ss.fff"); }
                    if (!dr.IsDBNull(12)) { dev.SinEM = dr.GetBoolean(12); }

                    dev.DetalleDevolucion = rpd1_D.ListarDetalleDevolucion(dev.DocEntry);
                    /******** C O N T R O L  D E  C A M B I O S ********/
                    CC_ORPD_E ccDev = new CC_ORPD_E
                    {
                        DocEntry = dev.DocEntry,
                        Operacion = "REGISTRAR"
                    };
                    var listaCC = cC_ORPD_D.ListarCC_ORPD(ccDev);

                    if (listaCC != null)
                    {
                        dev.Operario = listaCC[0].Operario;
                        dev.FechaOperacion = listaCC[0].FechaOperacion;
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

            return dev;
        }

        public int RegistrarDevolucion(ORPD_E Devolucion, List<RPD1_E> DetalleDevolucion)
        {
            int status = -1;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_ORPD", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "A");
                    cmd.Parameters.AddWithValue("@CardCode", Devolucion.CardCode);
                    cmd.Parameters.AddWithValue("@CardName", Devolucion.CardName);
                    cmd.Parameters.AddWithValue("@WhsCode", Devolucion.WhsCode);
                    cmd.Parameters.AddWithValue("@FechaDevolucion", Devolucion.FechaDevolucion);
                    cmd.Parameters.AddWithValue("@RetiroMercado", Devolucion.RetiroMercado);
                    cmd.Parameters.AddWithValue("@TiempoCorreoEnviado", Devolucion.TiempoCorreoEnviado);
                    cmd.Parameters.AddWithValue("@Correo", Devolucion.Correo);
                    cmd.Parameters.AddWithValue("@SinEM", Devolucion.SinEM);
                    cmd.Parameters.AddWithValue("@Operario", Devolucion.Operario);
                    cmd.Parameters.Add("@DocEntry", SqlDbType.Int);
                    cmd.Parameters["@DocEntry"].Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@DocNum", SqlDbType.Int);
                    cmd.Parameters["@DocNum"].Direction = ParameterDirection.Output;

                    if (DetalleDevolucion != null && DetalleDevolucion.Count >= 1)
                    {
                        SqlParameter tbDetDevolucion = new SqlParameter("@TPRPD1", SqlDbType.Structured);
                        tbDetDevolucion.Value = RPD1_E.TbDetalle(DetalleDevolucion, 0);
                        tbDetDevolucion.TypeName = "al.TPRPD1";
                        cmd.Parameters.AddWithValue("@TPRPD1", tbDetDevolucion.Value);
                    }

                    cmd.ExecuteNonQuery();
                    status = Convert.ToInt32(cmd.Parameters["@DocEntry"].Value.ToString());

                    SqlCommand cmd2 = new SqlCommand("dbo.POST_TRANSACCIONES", cn)
                    {
                        Transaction = tran,
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd2.Parameters.AddWithValue("@Tipo", "A");
                    cmd2.Parameters.AddWithValue("@Tabla", "ORPD");
                    cmd2.Parameters.AddWithValue("@DocNum", cmd.Parameters["@DocNum"].Value);
                    cmd2.Parameters.AddWithValue("@DocEntry", cmd.Parameters["@DocEntry"].Value);
                    cmd2.ExecuteNonQuery();

                    tran.Commit();
                }
                catch (Exception ex2)
                {
                    status = 0; tran.Rollback();
                    throw new Exception("Error en registro: " + ex2.Message);
                }
                finally
                {
                    cn.Close();
                }
            }

            return status;
        }

        public int EditarDevolucion(ORPD_E Devolucion, List<RPD1_E> DetalleDevolucion)
        {
            int status = -1;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_ORPD", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "U");
                    cmd.Parameters.AddWithValue("@Operario", Devolucion.Operario);
                    cmd.Parameters.AddWithValue("@DocEntry", Devolucion.DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", Devolucion.DocNum);

                    // datos de ordenes de venta
                    if (DetalleDevolucion != null && DetalleDevolucion.Count >= 1)
                    {
                        SqlParameter tbDetDevolucion = new SqlParameter("@TPRPD1", SqlDbType.Structured);
                        tbDetDevolucion.Value = RPD1_E.TbDetalle(DetalleDevolucion, Devolucion.DocEntry);
                        tbDetDevolucion.TypeName = "al.TPRPD1";
                        cmd.Parameters.AddWithValue("@TPRPD1", tbDetDevolucion.Value);
                    }

                    cmd.ExecuteNonQuery();
                    //if (DetalleDevolucion != null && DetalleDevolucion.Count >= 1)
                    //{
                    //	try
                    //	{
                    //		foreach (var det in DetalleDevolucion)
                    //		{
                    //			SqlCommand cmd4 = new SqlCommand("al.MANT_RPD1", cn)
                    //			{
                    //				Transaction = tran,
                    //				CommandType = CommandType.StoredProcedure
                    //			};

                    //			cmd4.Parameters.AddWithValue("@Accion", "INS");
                    //			cmd4.Parameters.AddWithValue("@DocEntry", det.DocEntry);
                    //			cmd4.Parameters.AddWithValue("@Linea", det.ItemCode);
                    //			cmd4.Parameters.AddWithValue("@ItemCode", det.ItemCode);
                    //			cmd4.Parameters.AddWithValue("@ItemName", det.ItemName);
                    //			cmd4.Parameters.AddWithValue("@FirmCode", det.FirmCode);
                    //			cmd4.Parameters.AddWithValue("@BatchNum", det.BatchNum);
                    //			cmd4.Parameters.AddWithValue("@ExpDate", det.ExpDate);
                    //			cmd4.Parameters.AddWithValue("@Quantity", det.Quantity);
                    //			cmd4.Parameters.AddWithValue("@Motivo", det.Motivo);
                    //			cmd4.Parameters.AddWithValue("@RefFactura", det.RefFactura);
                    //			cmd4.Parameters.AddWithValue("@Observacion", det.Observacion);
                    //			cmd4.ExecuteNonQuery();
                    //		}
                    //	}
                    //	catch (Exception ex)
                    //	{
                    //		tran.Rollback();
                    //		throw new Exception("Error al insertar detalle: " + ex.Message);
                    //	}
                    //}

                    tran.Commit();
                }
                catch (Exception ex2)
                {
                    status = 0; tran.Rollback();
                    throw new Exception("Error en registro: " + ex2.Message);
                }
                finally
                {
                    cn.Close();
                }
            }

            return status;
        }
        /*
		 *  Los tipos de mantenimiento para el método CambiarEstadoDevolucion() son los siguientes:
		 *  'R': CAMBIAR A ESTADO RECOGIDO 
		 *  'T': CAMBIAR A ESTADO TERMINAR 
		 *  'AA': CAMBIAR A ESTADO ANULAR 
		 *  'RR': CAMBIAR A ESTADO REVERTIR RECOGIDO 
		 *  'EC': SOLO GRABA CORREO PROVEEDOR 
		 */
        public int CambiarEstadoDevolucion(ORPD_E devolucion, string tipoMantenimiento)
        {
            int status = -1;
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand("al.MANT_ORPD", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@TipoMantenimiento", tipoMantenimiento);
                    cmd.Parameters.AddWithValue("@Operario", devolucion.Operario);
                    cmd.Parameters.AddWithValue("@DocEntry", devolucion.DocEntry);
                    cmd.Parameters.AddWithValue("@DocNum", devolucion.DocNum);

                    // R: CAMBIAR A ESTADO RECOGIDO 
                    if (!string.IsNullOrEmpty(tipoMantenimiento) && tipoMantenimiento.Equals("R"))
                    {
                        cmd.Parameters.AddWithValue("@WhsCode", devolucion.WhsCode);
                    }
                    // AA: CAMBIAR A ESTADO ANULADO 
                    if (!string.IsNullOrEmpty(tipoMantenimiento) && tipoMantenimiento.Equals("AA"))
                    {
                        cmd.Parameters.AddWithValue("@Comentario", devolucion.Comentario);
                    }
                    // EC: ENVIAR CORREO 
                    if (!string.IsNullOrEmpty(tipoMantenimiento) && tipoMantenimiento.Equals("EC"))
                    {
                        cmd.Parameters.AddWithValue("@Correo", devolucion.Correo);
                    }

                    cmd.ExecuteNonQuery();
                    status = Convert.ToInt32(cmd.Parameters["@DocEntry"].Value.ToString());
                    tran.Commit();
                }
                catch (Exception ex2)
                {
                    status = 0; tran.Rollback();
                    throw new Exception("Error en registro: " + ex2.Message);
                }
                finally
                {
                    cn.Close();
                }
            }

            return status;
        }

        public List<Capa_Entidad.Almacen_ENT.ReportesSql.RptHistoricoDevoluciones_E> ExportarExcelDevoluciones(Capa_Entidad.Almacen_ENT.ReportesSql.RptFiltrosHistoricoDevoluciones_E Filtros)
        {
            List<Capa_Entidad.Almacen_ENT.ReportesSql.RptHistoricoDevoluciones_E> lista = new List<Capa_Entidad.Almacen_ENT.ReportesSql.RptHistoricoDevoluciones_E>();
            string condWhere = string.Empty;

            if (Filtros != null)
            {
                if (!string.IsNullOrEmpty(Filtros.FechaDesde) && !string.IsNullOrEmpty(Filtros.FechaHasta))
                {
                    condWhere += $" AND (SELECT TOP 1 FechaOperacion FROM al.cc_ORPD WHERE DocEntry = DEV.DocEntry AND Operacion = 'REGISTRAR') BETWEEN '{Filtros.FechaDesde}' AND '{Filtros.FechaHasta}'";
                }

                if (!string.IsNullOrEmpty(Filtros.RefFactura))
                {
                    condWhere += $" AND DET.RefFactura LIKE '%{Filtros.RefFactura}%'";
                }

                if (!string.IsNullOrEmpty(Filtros.CardName))
                {
                    condWhere += $" AND DEV.CardName LIKE '%{Filtros.CardName}%' ";
                }
                if (Filtros.DocNum > 0)
                {
                    condWhere += $" AND DEV.DocNum = {Filtros.DocNum} ";
                }
                if (!string.IsNullOrEmpty(Filtros.Estado))
                {
                    condWhere += $" AND DEV.Estado = '{Filtros.Estado}' ";
                }

                if (!string.IsNullOrEmpty(Filtros.ItemName))
                {
                    condWhere += $" AND DET.ItemName LIKE '%{Filtros.ItemName}%' ";
                }

                if (!string.IsNullOrEmpty(Filtros.WhsCode))
                {
                    condWhere += $" AND DEV.WhsCode = '{Filtros.WhsCode}' ";
                }
            }

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT DEV.DocNum,DEV.Correlativo, (SELECT TOP 1 CONVERT(varchar,FechaOperacion ,103) FROM al.CC_ORPD WHERE DocEntry = DEV.DocEntry AND Operacion = 'REGISTRAR'), DEV.CardCode, DEV.CardName, CONVERT(varchar,DEV.FechaDevolucion,103) AS 'FechaDevolucion',  DEV.Estado, DET.ItemCode, DET.ItemName, DEV.WhsCode, DET.BatchNum, CONVERT(varchar,DET.ExpDate,103), DET.BuyUnitMsr, DET.Quantity,  DET.RefFactura, MD.Descripcion, SUB.Descripcion, DET.Observacion, DEV.Comentario,");
                sb.Append(" (SELECT TOP 1 Operario FROM al.cc_ORPD WHERE DocEntry = DEV.DocEntry AND Operacion = 'REGISTRAR')");
                sb.Append(" FROM al.ORPD DEV");
                sb.Append(" INNER JOIN al.RPD1 DET ON DET.DocEntry = DEV.DocEntry");
                sb.Append(" INNER JOIN al.MotivosDevoluciones MD ON MD.IdMotivo = DET.Motivo");
                sb.Append(" LEFT JOIN al.SubmotivosDevoluciones SUB ON SUB.IdSubmotivo = DET.Submotivo");
                sb.Append($" WHERE DEV.DocEntry > 0 {condWhere} ORDER BY DEV.DocEntry DESC");

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
                            Capa_Entidad.Almacen_ENT.ReportesSql.RptHistoricoDevoluciones_E dev = new Capa_Entidad.Almacen_ENT.ReportesSql.RptHistoricoDevoluciones_E();
                            if (!dr.IsDBNull(0)) { dev.DocNum = dr.GetInt32(0); }
                            if (!dr.IsDBNull(1)) { dev.Correlativo = dr.GetString(1); }
                            if (!dr.IsDBNull(2)) { dev.FechaCreacion = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { dev.CardCode = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { dev.CardName = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { dev.FechaDevolucion = dr.GetString(5); }
                            if (!dr.IsDBNull(6)) { dev.Estado = dr.GetString(6); }
                            if (!dr.IsDBNull(7)) { dev.ItemCode = dr.GetString(7); }
                            if (!dr.IsDBNull(8)) { dev.ItemName = dr.GetString(8); }
                            if (!dr.IsDBNull(9)) { dev.WhsCode = dr.GetString(9); }
                            if (!dr.IsDBNull(10)) { dev.BatchNum = dr.GetString(10); }
                            if (!dr.IsDBNull(11)) { dev.ExpDate = dr.GetString(11); }
                            if (!dr.IsDBNull(12)) { dev.BuyUnitMsr = dr.GetString(12); }
                            if (!dr.IsDBNull(13)) { dev.Quantity = dr.GetDecimal(13); }
                            if (!dr.IsDBNull(14)) { dev.RefFactura = dr.GetString(14); }
                            if (!dr.IsDBNull(15)) { dev.Descripcion = dr.GetString(15); }
                            if (!dr.IsDBNull(16)) { dev.Submotivo = dr.GetString(16); }
                            if (!dr.IsDBNull(17)) { dev.Observacion = dr.GetString(17); }
                            if (!dr.IsDBNull(18)) { dev.Comentario = dr.GetString(18); }
                            if (!dr.IsDBNull(19)) { dev.OpCreacion = dr.GetString(19); }
                            lista.Add(dev);

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

        public List<Capa_Entidad.Almacen_ENT.ReportesSql.RptCorreoDevolucion_E> RptCorreoDevolucion(int DocEntry)
        {
            List<Capa_Entidad.Almacen_ENT.ReportesSql.RptCorreoDevolucion_E> listdev = new List<Capa_Entidad.Almacen_ENT.ReportesSql.RptCorreoDevolucion_E>();

            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "select ISNULL(substring(RefFactura,4,4),'') , ISNULL(substring(RefFactura,9,8),''),ItemName,BatchNum,convert(varchar(10),ExpDate),Quantity,(SELECT Descripcion FROM AL.SubmotivosDevoluciones where IdSubmotivo=al.rpd1.Submotivo)  from al.rpd1 where DocEntry = @DocEntry";
                SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                cmd.Parameters.AddWithValue("@DocEntry", DocEntry);
                cn.Open();

                try
                {
                    SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            Capa_Entidad.Almacen_ENT.ReportesSql.RptCorreoDevolucion_E dev = new Capa_Entidad.Almacen_ENT.ReportesSql.RptCorreoDevolucion_E();
                            if (!dr.IsDBNull(0)) { dev.SerieFactura = dr.GetString(0); }
                            if (!dr.IsDBNull(1)) { dev.CorrelativoFactura = dr.GetString(1); }
                            if (!dr.IsDBNull(2)) { dev.ItemName = dr.GetString(2); }
                            if (!dr.IsDBNull(3)) { dev.BatchNum = dr.GetString(3); }
                            if (!dr.IsDBNull(4)) { dev.ExpDate = dr.GetString(4); }
                            if (!dr.IsDBNull(5)) { dev.Quantity = dr.GetDecimal(5); }
                            if (!dr.IsDBNull(6)) { dev.SubMotivoDescripcion = dr.GetString(6); }
                            listdev.Add(dev);
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

            return listdev;
        }

        public bool VerificarExistenciaDevolucion(Capa_Entidad.Almacen_ENT.ReportesSql.RptFiltrosHistoricoDevoluciones_E filtros)
        {
            bool status = false;
            string condWhere = string.Empty, join = string.Empty;

            if (filtros != null)
            {
                if (!string.IsNullOrEmpty(filtros.FechaDesde) && !string.IsNullOrEmpty(filtros.FechaHasta))
                {
                    condWhere += $" AND DEV.FechaDevolucion BETWEEN '{filtros.FechaDesde}' AND '{filtros.FechaHasta}'";
                }

                if (!string.IsNullOrEmpty(filtros.Estado))
                {
                    condWhere += $" AND DEV.Estado = '{filtros.Estado}' ";
                }

                if (!string.IsNullOrEmpty(filtros.RefFactura))
                {
                    join += " INNER JOIN al.RPD1 DET on DET.DocEntry = DEV.DocEntry";
                    condWhere += $" AND DET.RefFactura = '{filtros.RefFactura}' ";
                }

                if (!string.IsNullOrEmpty(filtros.WhsCode))
                {
                    condWhere += $" AND DEV.WhsCode = '{filtros.WhsCode}' ";
                }

                if (filtros.DocNum >= 1)
                {
                    condWhere += $" AND DEV.DocNum = '{filtros.DocNum}' ";
                }

                if (!string.IsNullOrEmpty(filtros.CardName))
                {
                    condWhere += $" AND DEV.CardName LIKE '%{filtros.CardName}%' ";
                }

                if (!string.IsNullOrEmpty(filtros.ItemName))
                {
                    condWhere += $" AND DET.ItemName LIKE '%{filtros.ItemName}%' ";
                }
            }

            if (!string.IsNullOrEmpty(condWhere))
            {
                using (SqlConnection cn = new SqlConnection(uti.cadSql))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("SELECT TOP 1 DEV.DocNum");
                    sb.Append(" FROM al.ORPD DEV");
                    sb.Append($" {join} WHERE DEV.DocEntry > 0 {condWhere}");

                    string query = sb.ToString();

                    SqlCommand cmd = new SqlCommand(query, cn);         // prepara
                    cn.Open();

                    try
                    {
                        SqlDataReader dr = cmd.ExecuteReader();             // ejecuta

                        if (dr.HasRows)
                        {
                            status = true;
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
            }

            return status;
        }
    }
}
