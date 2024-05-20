using Capa_Entidad.Almacen_ENT.TablasSql;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class IPE1_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public List<IPE1_E> buscarDetallesAlmacenes(int DocEntry)
        {
            List<IPE1_E> lista = new List<IPE1_E>();
            string query = "select * from al.ipe1 where DocEntry=" + DocEntry;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    IPE1_E o = new IPE1_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.WhsCode = dr.GetString(1); }
                    if (!dr.IsDBNull(2)) { o.WhsName = dr.GetString(2); }
                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
    }
}