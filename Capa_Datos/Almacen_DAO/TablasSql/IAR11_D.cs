using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class IAR11_D
    {
        DBHelper db = new DBHelper();
        public List<IAR11_E> buscarDetApoyos(int DocEntry, int Fase)
        {
            List<IAR11_E> lista = new List<IAR11_E>();
            string query = "select * from al.iar11 where DocEntry=" + DocEntry + " and Fase=" + Fase;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    IAR11_E obj = new IAR11_E();
                    if (!dr.IsDBNull(0)) { obj.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { obj.Fase = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { obj.Linea = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { obj.Id = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { obj.Nombre = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { obj.ObsApoyo = dr.GetString(5); }
                    lista.Add(obj);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
    }
}