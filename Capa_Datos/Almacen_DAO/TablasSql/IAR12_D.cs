using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class IAR12_D
    {
        DBHelper db = new DBHelper();
        public List<IAR12_E> buscarDetContab(int DocEntry, int Fase)
        {
            List<IAR12_E> lista = new List<IAR12_E>();
            string query = "select * from al.iar12 where DocEntry=" + DocEntry + " and Fase=" + Fase;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    IAR12_E obj = new IAR12_E();
                    if (!dr.IsDBNull(0)) { obj.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { obj.Fase = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { obj.Linea = dr.GetInt32(2); }
                    if (!dr.IsDBNull(3)) { obj.BatchNum = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { obj.ExpDate = dr.GetDateTime(4).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(5)) { obj.QuantityCajas = dr.GetDecimal(5); }
                    if (!dr.IsDBNull(6)) { obj.QuantityPiezas = dr.GetDecimal(6); }
                    if (!dr.IsDBNull(7)) { obj.ObsLote = dr.GetString(7); }
                    lista.Add(obj);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
    }
}