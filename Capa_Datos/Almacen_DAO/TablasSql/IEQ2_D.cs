using System.Collections.Generic;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System.Data.SqlClient;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class IEQ2_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public List<IEQ2_E> buscarDetallesFabricantes(int DocEntry)
        {
            List<IEQ2_E> lista = new List<IEQ2_E>();
            string query = "select * from al.ieq2 where DocEntry=" + DocEntry;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    IEQ2_E o = new IEQ2_E();
                    if (!dr.IsDBNull(0)) { o.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { o.Linea = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { o.FirmCode = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { o.U_SYP_DESC = dr.GetString(3); }

                    lista.Add(o);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
    }
}