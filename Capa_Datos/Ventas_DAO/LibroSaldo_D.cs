using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.Ventas_ENT;
using System.Data.SqlClient;
using System.Data;

namespace Capa_Datos.Ventas_DAO
{
    public class LibroSaldo_D
    {
        DBHelper db = new DBHelper();
        public List<LibroSaldo_E> listarLibrosSaldo(LibroSaldo_E li)
        {
            List<LibroSaldo_E> lista = new List<LibroSaldo_E>();
            string query;
            if(li==null){query = "select CardCode from olds order by CardName";}
            else
            {
                query = "select CardCode from olds where CardCode is not null ";
                if (li.CardName != null) { query += " and CardName like '%"+li.CardName+"%'"; }
                if (li.NroOpe > 0) { query += " and NroOpe >=" + li.NroOpe; }
                if (li.SaldoActual > 0) { query += " and SaldoActual > 0.00"; }
                if (li.SaldoActual < 0) { query += " and SaldoActual < 0.00"; }
                query += " order by CardName";
            }            
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while(dr.Read())
                {
                    LibroSaldo_E l = obtenerLibroSaldo(dr.GetString(0));
                    lista.Add(l);
                } 
                dr.Close();
            }catch { }
            return lista;
        }
        public LibroSaldo_E obtenerLibroSaldo(string CardCode)
        {
            LibroSaldo_E l = new LibroSaldo_E();
            string query = "select * from olds where CardCode=@CardCode";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query,new List<string> { "@CardCode"},CardCode);
                dr.Read();
                l.CardCode = dr.GetString(0);
                l.CardName = dr.GetString(1);
                if (!dr.IsDBNull(2)) { l.NroOpe = dr.GetInt32(2); }
                if (!dr.IsDBNull(3)) { l.SaldoActual = dr.GetDecimal(3); }
                l.Det = obtenerDetLibroSaldo(l.CardCode);
                dr.Close();
            }
            catch { }
            return l;
        }
        public List<DetLibroSaldo_E> obtenerDetLibroSaldo(string CardCode)
        {
            List<DetLibroSaldo_E> lista = new List<DetLibroSaldo_E>();
            string query = "select * from lds1 where CardCode=@CardCode order by Linea";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query,new List<string> {"@CardCode" },CardCode);
                while(dr.Read())
                {
                    DetLibroSaldo_E d = new DetLibroSaldo_E();
                    d.CardCode = CardCode;
                    d.Linea = dr.GetInt32(1);
                    if (!dr.IsDBNull(2)) { d.FechaOpe = dr.GetDateTime(2).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(3)) { d.Operacion = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { d.DetOpe = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { d.Ingreso = dr.GetDecimal(5); }
                    if (!dr.IsDBNull(6)) { d.Egreso = dr.GetDecimal(6); }
                    if (!dr.IsDBNull(7)) { d.Saldo = dr.GetDecimal(7); }
                    if (!dr.IsDBNull(8)) { d.FechaReg = dr.GetDateTime(8).ToString(); }
                    if (!dr.IsDBNull(9)) { d.OperarioReg = dr.GetString(9); }
                    lista.Add(d);
                }
                dr.Close();
            }catch { }
            return lista;
        }
        public int crearLibroSaldo(LibroSaldo_E l)
        {
            int status = 0;
            try
            {
                db.ExecuteNonQuery("MANT_OLDS", "AC",l.CardCode,l.CardName,0,0.00M);
                status = 1;
            }catch { status = -1; }
            return status;
        }
        // detalles
        public int agregarDetLibroSaldo(DetLibroSaldo_E d)
        {
            int status = 0;
            try
            {
                db.ExecuteNonQuery("MANT_OLDS", "AD",d.CardCode,null,null,null,d.Linea,
                    d.FechaOpe,d.Operacion,d.DetOpe,d.Ingreso,d.Egreso,null,null,d.OperarioReg);
                status = 1;
            }
            catch (Exception e) { status = -1; throw new Exception(e.Message); }
            return status;
        }
    }
}
