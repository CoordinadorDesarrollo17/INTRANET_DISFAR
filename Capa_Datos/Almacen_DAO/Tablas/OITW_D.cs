using Capa_Entidad.Almacen_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sap.Data.Hana;
using Capa_Entidad.Almacen_ENT.TablasSql;

namespace Capa_Datos.Almacen_DAO.Tablas
{
    public class OITW_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public List<OITW_E> listarDetArticulosInv(string ItemCode)
        {
            List<OITW_E> lista = new List<OITW_E>();
            string query = "select \"ItemCode\",\"WhsCode\",\"OnHand\",\"IsCommited\",\"OnOrder\" " +
                "from " + uti.schemaHana+"oitw where \"ItemCode\"='"+ItemCode+ "' order by \"WhsCode\"";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while(hdr.Read())
                {
                    OITW_E o = new OITW_E();
                    if (!hdr.IsDBNull(0)) { o.ItemCode = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { o.WhsCode = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { o.OnHand = Math.Round(hdr.GetDecimal(2),0); }
                    if (!hdr.IsDBNull(3)) { o.IsCommited = Math.Round(hdr.GetDecimal(3)); }
                    if (!hdr.IsDBNull(4)) { o.OnOrder = Math.Round(hdr.GetDecimal(4)); }
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
    }
}
