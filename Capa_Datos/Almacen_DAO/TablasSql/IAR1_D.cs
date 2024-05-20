using Capa_Entidad.Almacen_ENT.TablasSql;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Capa_Datos.Almacen_DAO.TablasSql
{
    public class IAR1_D
    {
        DBHelper db = new DBHelper(); IAR11_D iar11D = new IAR11_D(); IAR12_D iar12D = new IAR12_D();
        public List<IAR1_E> buscarDetFases(int DocEntry)
        {
            List<IAR1_E> lista = new List<IAR1_E>();
            string query = "select * from al.iar1 where DocEntry=" + DocEntry;
            try
            {
                SqlDataReader dr = db.ExecuteReaderNoSp(query);
                while (dr.Read())
                {
                    IAR1_E obj = new IAR1_E();
                    if (!dr.IsDBNull(0)) { obj.DocEntry = dr.GetInt32(0); }
                    if (!dr.IsDBNull(1)) { obj.Fase = dr.GetInt32(1); }
                    if (!dr.IsDBNull(2)) { obj.NombreFase = dr.GetString(2); }
                    if (!dr.IsDBNull(3)) { obj.TipoOperario = dr.GetString(3); }
                    if (!dr.IsDBNull(4)) { obj.Operario = dr.GetString(4); }
                    if (!dr.IsDBNull(5)) { obj.FechaFase = dr.GetDateTime(5).ToString("yyyy-MM-dd"); }
                    if (!dr.IsDBNull(6)) { obj.HoraFase = dr.GetTimeSpan(6).ToString(); }
                    if (!dr.IsDBNull(7)) { obj.Observacion = dr.GetString(7); }
                    obj.DetApoyos = iar11D.buscarDetApoyos(obj.DocEntry, obj.Fase);
                    obj.DetContab = iar12D.buscarDetContab(obj.DocEntry, obj.Fase);
                    lista.Add(obj);
                }
                dr.Close();
            }
            catch { }
            return lista;
        }
    }
}