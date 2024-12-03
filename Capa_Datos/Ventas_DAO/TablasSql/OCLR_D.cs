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
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
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
        public void registrarClienteRegalo(OCLR_E obj)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("transaccion1");
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_OCLR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "A");
                    cmd.Parameters.AddWithValue("@CardCode", obj.CardCode);
                    cmd.Parameters.AddWithValue("@CardName", obj.CardName);
                    cmd.Parameters.AddWithValue("@NroOpe", obj.NroOpe);

                    SqlParameter tbDet = new SqlParameter("@TPCLR1", SqlDbType.Structured);
                    tbDet.Value = CLR1_E.tbDetalle(obj.Det);

                    tbDet.TypeName = "dbo.TPCLR1";
                    cmd.Parameters.AddWithValue("@TPCLR1", tbDet.Value);
                    decimal cantidad = 0;
                    int i = 0;
                    foreach (CLR1_E detObj in obj.Det)
                    {
                        decimal detOCant = detObj.Cantidad ?? default(decimal);
                        cantidad = cantidad + detOCant;
                        i++;
                    }
                    cmd.Parameters.AddWithValue("@Saldo", cantidad);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: " + e.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }
        }
        public void editarClienteRegalo(OCLR_E obj)
        {
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlTransaction tran = cn.BeginTransaction();
                try
                {
                    SqlCommand cmd = new SqlCommand("vt.MANT_OCLR", cn);
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TipoMantenimiento", "U");
                    cmd.Parameters.AddWithValue("@CardCode", obj.CardCode);

                    SqlParameter tbDet = new SqlParameter("@TPCLR1", SqlDbType.Structured);
                    tbDet.Value = CLR1_E.tbDetalle(obj.Det);

                    tbDet.TypeName = "dbo.TPCLR1";
                    cmd.Parameters.AddWithValue("@TPCLR1", tbDet.Value);
                    decimal cantidad = 0;
                    int i = 0;
                    foreach (CLR1_E detObj in obj.Det)
                    {
                        decimal detOCant = detObj.Cantidad ?? default(decimal);
                        cantidad = cantidad + detOCant;
                        i++;
                    }
                    cmd.Parameters.AddWithValue("@Saldo", cantidad);

                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception e) { tran.Rollback(); cn.Close(); throw new Exception("Error en creacion: " + e.Message); }
                cn.Close();
            }
            catch (Exception e2) { cn.Close(); throw new Exception("Error en creacion y conexion: " + e2.Message); }
        }
        public void CompromisoClienteRegalo(CLR1_E obj)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                string query = "UPDATE vt.CLR1 SET Cantidad=Cantidad-@Cantidad WHERE CardCode=@CardCode AND IdReg=@IdReg";

                cn.Open();
                SqlTransaction tran = cn.BeginTransaction("ACTUALIZAR CLR1");

                try
                {
                    SqlCommand cmd= new SqlCommand(query, cn, tran);         // prepara
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@CardCode", obj.CardCode);
                    cmd.Parameters.AddWithValue("@IdReg", obj.IdReg);
                    cmd.Parameters.AddWithValue("@Cantidad", obj.Cantidad);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception e) { tran.Rollback(); throw new Exception(e.Message); }

                cn.Close();
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
        // borrar
        public void migrarClientesReg()
        {
            string query = "select \"CardCode\", \"CardName\" , sum(\"DocTotal\") " +
                " from " + uti.schemaHana + "oinv where \"CANCELED\" = 'N' " +
                " and \"DocDate\" between  '2021-01-01' and '2021-12-31' " +
                "group by \"CardCode\",\"CardName\" " +
                "having sum(\"DocTotal\")>1000 order by 3";
            try
            {
                HanaDataReader dr = db.HanaExecuteReaderNoSp(query);
                while (dr.Read())
                {

                }
                dr.Close();
            }
            catch { }
        }
    }
}