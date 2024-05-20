using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Capa_Entidad.Ventas_ENT.TablasSql;

namespace Capa_Datos.Ventas_DAO.TablasSql
{
    public class RTV13_D
    {
        DBHelper db = new DBHelper();
        public List<RTV13_E> BuscarRTV13(int DocEntry)
        {
            List<RTV13_E> lista = new List<RTV13_E>();
            string query = "SELECT * FROM vt.RTV13 WHERE DocEntry=@DocEntry";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string>() { "@DocEntry" }, DocEntry);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        RTV13_E objRTV13 = new RTV13_E();

                        if (!dr.IsDBNull(0)) { objRTV13.DocEntry = dr.GetInt32(0); }
                        if (!dr.IsDBNull(1)) { objRTV13.Linea = dr.GetInt32(1); }
                        if (!dr.IsDBNull(2)) { objRTV13.Operario = dr.GetString(2); }

                        lista.Add(objRTV13  );
                    }
                }
                
                dr.Close();
            }
            catch { }
            return lista;
        }


        public List<string> BuscarOperariosEmpacando(int DocEntry)
        {
            List<string> lista = new List<string>();
            string query = "SELECT Operario FROM vt.RTV13 WHERE DocEntry=@DocEntry";
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query, new List<string>() { "@DocEntry" }, DocEntry);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        if (dr.GetString(0).Length > 12) { lista.Add(dr.GetString(0).Substring(0, 12)); }
                        else { lista.Add(dr.GetString(0)); }
                    }
                }

                dr.Close();
            }
            catch { }
            return lista;
        }

    }
}
