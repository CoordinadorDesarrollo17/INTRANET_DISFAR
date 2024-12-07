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
					},tran);

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

            // Creamos una nueva conexión
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                // Abrimos la conexión
                cn.Open();

                // Si no se pasa una transacción, creamos una nueva
                SqlTransaction transaction = tran ?? cn.BeginTransaction();

                try
                {
                    // Ejecutamos el comando usando la transacción proporcionada o creada
                    using (SqlCommand cmd = new SqlCommand("vt.MANT_OREG", cn, transaction))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TipoMantenimiento", "US");            // Actualizar stock disponible
                        cmd.Parameters.AddWithValue("@Id", reg.Id).Direction = ParameterDirection.InputOutput;
                        cmd.Parameters.AddWithValue("@StockDisp", reg.StockDisp);

                        cmd.ExecuteNonQuery();
                    }

                    otrcD.registrarTransaccion(obj, transaction);

                    status = true;

                    // Si se creó una nueva transacción dentro del método, la confirmamos aquí
                    if (tran == null)
                    {
                        transaction.Commit();
                    }
                }
                catch (Exception e)
                {
                    if (tran == null)
                    {
                        transaction.Rollback();
                    }

                    status = false;
                    throw new Exception("Error en creación de compromiso de stock: " + e.Message, e);
                }
            }
        }


        public void CompromisosStock(List<ORTV_E> listaTickets, SqlTransaction tran)
        {
            OTRC_D otrcD = new OTRC_D();
            OCLR_D oclrD = new OCLR_D();
            bool status = false;

            try
            {
				foreach (var ticket in listaTickets)
				{
					if ((ticket.Estado == "SEPARADO" || ticket.Estado == "ABIERTO") && ticket.Det5 != null && ticket.Det5.Count > 0)
					{
						if (ticket.Det5[0].RegCant > 0)
						{
							// Comprobar si el cliente tiene saldo suficiente, solo si esta ingresando un valor positivo,
							// si es negativo se entiende que esta devolviendo el comprometido
							if (!oclrD.ComprobarDispCliReg(new CLR1_E()
							{
								CardCode = ticket.CardCode,
								IdReg = ticket.Det5[0].IdReg,
								Cantidad = ticket.Det5[0].RegCant
							}))
							{
								throw new Exception("El cliente no tiene saldo suficiente.");
							}
						}
						using (SqlCommand cmd = new SqlCommand("vt.MANT_OREG", tran.Connection, tran))
						{
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.CommandTimeout = 120;
							cmd.Parameters.AddWithValue("@TipoMantenimiento", "USC");
							cmd.Parameters.AddWithValue("@Id", ticket.Det5[0].IdReg).Direction = ParameterDirection.InputOutput;
							cmd.Parameters.AddWithValue("@StockComp", ticket.Det5[0].RegCant);

							cmd.ExecuteNonQuery();
						}

						// Registrar la transacción de stock
						otrcD.registrarTransaccion(new OTRC_E()
						{
							IdReg = ticket.Det5[0].IdReg,
							RegName = ticket.Det5[0].RegCate + " " + ticket.Det5[0].RegTipo,
							CardCode = ticket.CardCode,
							CardName = ticket.CardName,
							Sentido = "Asignacion",
							Detalle = ticket.DocNum.ToString(),
							Imputado = ticket.Det5[0].RegCant,
							Operario = ticket.OpRegistro
						}, tran);

						// Registrar el compromiso con el cliente
						oclrD.CompromisoClienteRegalo(new CLR1_E()
						{
							CardCode = ticket.CardCode,
							IdReg = ticket.Det5[0].IdReg,
							Cantidad = ticket.Det5[0].RegCant
						}, tran);



						status = true;
					}
				}
            }
            catch (Exception e)
            {
                status = false;
                throw new Exception("Error en creación de compromiso de stock: " + e.Message, e);
            }
        }

    }
}
