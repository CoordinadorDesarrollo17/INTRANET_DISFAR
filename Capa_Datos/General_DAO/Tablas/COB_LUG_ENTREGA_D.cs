using Capa_Entidad.General_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sap.Data.Hana;

namespace Capa_Datos.General_DAO.Tablas
{
    public class COB_LUG_ENTREGA_D
    {
        DBHelper db = new DBHelper();Utilitarios uti = new Utilitarios();
        public List<COB_LUG_ENTREGA_E> listadoLugaresDeEntrega()
        {
            List<COB_LUG_ENTREGA_E> lista = new List<COB_LUG_ENTREGA_E>();
            string query = "select top 50 * from "+uti.schemaHana+"\"@COB_LUG_ENTREGA\"";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    COB_LUG_ENTREGA_E c = new COB_LUG_ENTREGA_E();
                    c.Code = hdr.GetString(0);
                    if (!hdr.IsDBNull(1)) { c.Name = hdr.GetString(1); }
                    lista.Add(c);
                }
                hdr.Close();
            }catch { }
            return lista;
        }
    }
}
