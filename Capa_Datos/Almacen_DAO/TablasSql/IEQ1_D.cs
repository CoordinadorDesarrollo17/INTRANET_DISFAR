using System.Collections.Generic;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System.Data.SqlClient;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class IEQ1_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public List<IEQ1_E> buscarDetallesMiembros(int DocEntry)
        {
            List<IEQ1_E> lista = new List<IEQ1_E>();
            string query = "select * from al.ieq1 where DocEntry=" + DocEntry;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    IEQ1_E o = new IEQ1_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.Linea = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { o.Id = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { o.Nombre = dr.GetString(3); }

                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
    }
}