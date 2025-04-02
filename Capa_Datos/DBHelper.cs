using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Sap.Data.Hana;

namespace Capa_Datos
{
    public class DBHelper
    {
        Utilitarios uti = new Utilitarios();
        public SqlDataReader ExecuteReaderNoSp(string query,List<string> npara=null, params object[] Parametros)
        {
            SqlConnection cnx = new SqlConnection(uti.cadSql);
            cnx.Open();
            SqlCommand cmd = new SqlCommand(query, cnx);
            cmd.CommandType = CommandType.Text;
            if(npara!=null)
            {
                foreach (string p in npara)
                {
                    SqlParameter par = new SqlParameter(p, null);
                    cmd.Parameters.Add(par);
                }
                if (Parametros.Length > 0)
                    LlenarParametrosNoSp(cmd, Parametros);
            }    
            SqlDataReader lector = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return lector;
        }
        public SqlDataReader ExecuteReader(string NombreSP, params object[] Parametros)
        {
            SqlConnection cnx = new SqlConnection(uti.cadSql);
            cnx.Open();
            SqlCommand cmd = new SqlCommand(NombreSP, cnx);
            cmd.CommandType = CommandType.StoredProcedure;
            if (Parametros.Length > 0)
                LlenarParametros(cmd, Parametros);
            SqlDataReader lector = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return lector;
        }
        public void ExecuteNonQuery(string NombreSP, params object[] Parametros)
        {
            SqlConnection cnx = new SqlConnection(uti.cadSql);
            cnx.Open();
            //
            SqlCommand cmd = new SqlCommand(NombreSP, cnx);
            cmd.CommandType = CommandType.StoredProcedure;
            if (Parametros.Length > 0)
                LlenarParametros(cmd, Parametros);

            cmd.ExecuteNonQuery();

            cnx.Close();
        }
        public void ExecuteNonQueryTrx(string NombreSP, params object[] Parametros)
        {
            SqlConnection cnx = new SqlConnection(uti.cadSql);
            cnx.Open();
            SqlTransaction trx = cnx.BeginTransaction();
            try
            {
                // especificamos que la transaccion sera llevada a cabo por SqlCommand
                SqlCommand cmd = new SqlCommand(NombreSP, cnx, trx);
                cmd.CommandType = CommandType.StoredProcedure;
                if (Parametros.Length > 0)
                    LlenarParametros(cmd, Parametros);

                cmd.ExecuteNonQuery();
                // si no hay ningun ERROR, entonces CONFIRMAMOS las OPERACIONES
                trx.Commit();
                cnx.Close();
            }
            catch (Exception ex)
            {
                trx.Rollback(); // cancela las operaciones
                throw new Exception(ex.Message);
            }
        }
        public void ExecuteNonQueryTrxNoSp(string query, List<string> npara = null, params object[] Parametros)
        {
            SqlConnection cnx = new SqlConnection(uti.cadSql);
            cnx.Open();
            SqlTransaction trx = cnx.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand(query, cnx, trx);
                cmd.CommandType = CommandType.Text;

                if (npara != null)
                {
                    foreach (string p in npara)
                    {
                        SqlParameter par = new SqlParameter(p, null);
                        cmd.Parameters.Add(par);
                    }
                    if (Parametros.Length > 0)
                        LlenarParametrosNoSp(cmd, Parametros);
                }
                cmd.ExecuteNonQuery();
                trx.Commit();
                //
                cnx.Close();
            }
            catch (Exception ex)
            {
                trx.Rollback(); // cancela las operaciones
                throw new Exception(ex.Message);
            }
        }
        public object ExecuteScalar(string NombreSP, params object[] Parametros)
        {
            SqlConnection cnx = new SqlConnection(uti.cadSql);
            cnx.Open();
            SqlCommand cmd = new SqlCommand(NombreSP, cnx);
            cmd.CommandType = CommandType.StoredProcedure;

            if (Parametros.Length > 0)
                LlenarParametros(cmd, Parametros);

            object rpta = cmd.ExecuteScalar();

            cnx.Close();

            return rpta;
        }
        public object ExecuteScalarNoSp(string NombreSP)
        {
            SqlConnection cnx = new SqlConnection(uti.cadSql);
            object rpta;
            try
            {
                cnx.Open();
                SqlCommand cmd = new SqlCommand(NombreSP, cnx);
                cmd.CommandType = CommandType.Text;
                rpta = cmd.ExecuteScalar();
                cnx.Close();
            }
            catch(Exception e) { cnx.Close(); throw new Exception(e.Message); }
            return rpta;
        }
        private void LlenarParametros(SqlCommand comando, params object[] parametros)
        {
            int indice = 0;
            int totalParam = parametros.Length;
            SqlCommandBuilder.DeriveParameters(comando);
            foreach (SqlParameter item in comando.Parameters)
            {
                if (item.ParameterName != "@RETURN_VALUE")
                {
                    item.Value = parametros[indice];
                    indice++;
                }
                if (totalParam==(indice)) { return; }
            }
        }
        private void LlenarParametrosNoSp(SqlCommand comando, params object[] parametros)
        {
            int indice = 0;
            //SqlCommandBuilder.DeriveParameters(comando);
            foreach (SqlParameter item in comando.Parameters)
            {
                if (item.ParameterName != "@RETURN_VALUE")
                {
                    item.Value = parametros[indice];
                    indice++;
                }
            }
        }
        //para hana sql
        public HanaDataReader HanaExecuteReaderSp(string query, params object[] Parametros)
        {
            HanaConnection cnx = new HanaConnection(uti.cadHana);
            cnx.Open();
            HanaCommand hcmd = new HanaCommand(query, cnx);
            hcmd.CommandType = CommandType.StoredProcedure;
            if (Parametros.Length > 0)
                HanaLlenarParametros(hcmd, Parametros);
            HanaDataReader lector = hcmd.ExecuteReader(CommandBehavior.CloseConnection);
            return lector;
        }
        public HanaDataReader HanaExecuteReaderNoSp(string query, params object[] Parametros)
        {
            HanaConnection cnx = new HanaConnection(uti.cadHana);
            cnx.Open();
            HanaCommand hcmd = new HanaCommand(query, cnx);
            hcmd.CommandType = CommandType.Text;
            if (Parametros.Length > 0)
                HanaLlenarParametros(hcmd, Parametros);
            HanaDataReader lector = hcmd.ExecuteReader(CommandBehavior.CloseConnection);
            return lector;
        }
        private void HanaLlenarParametros(HanaCommand comando, params object[] parametros)
        {
            int indice = 0;
            HanaCommandBuilder.DeriveParameters(comando);
            //
            foreach (HanaParameter item in comando.Parameters)
            {
                if (item.ParameterName != "@RETURN_VALUE")
                {
                    item.Value = parametros[indice];
                    indice++;
                }
            }
        }
        public void HanaExecuteNonQueryTrx(string query)
        {
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            hcn.Open();
            HanaTransaction trx = hcn.BeginTransaction();
            try
            {
                HanaCommand cmd = new HanaCommand(query, hcn, trx);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                trx.Commit();
                hcn.Close();
            }
            catch(Exception ex) {
                trx.Rollback();
                hcn.Close();
                throw new Exception(ex.Message);
            }
        }
        //
        public string statusBD()
        {
            string status = "";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                cn.Open();
                status = "SQLSERVER 2019: " + uti.BDsql;
                cn.Close();
            }
            catch (Exception e) { status = "HUBO UN ERROR DE CONEXION A SQL" + e.Message; cn.Close(); }
            try
            {
                hcn.Open();
                status += " Y BASE DE SAP : " + uti.schemaHana;
                hcn.Close();
            }
            catch (Exception e2) { status += "HUBO UN ERROR DE CONEXION A HANA" + e2.Message; hcn.Close(); }
            return status;
        }

        public int ContarRegistros(string query, SqlParameter[] parametros)
        {
            using (SqlConnection cn = new SqlConnection(uti.cadSql))
            {
                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddRange(parametros);
                    cn.Open();
                    return (int)cmd.ExecuteScalar();
                }
            }
        }
    }
}
