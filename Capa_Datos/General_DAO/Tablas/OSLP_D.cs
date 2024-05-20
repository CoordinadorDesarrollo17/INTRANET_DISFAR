using Capa_Entidad.General_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sap.Data.Hana;
using System.Data;

namespace Capa_Datos.General_DAO.Tablas
{
    public class OSLP_D
    {
        Utilitarios uti = new Utilitarios();DBHelper db = new DBHelper();
        public List<OSLP_E> listadoOslp(string Memo)
        {
            List<OSLP_E> lista = new List<OSLP_E>();
            string query = "select \"SlpCode\" from "+uti.schemaHana+"OSLP where \"Memo\" like '"+Memo+"%' order by \"SlpName\"";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while(hdr.Read())
                {
                    OSLP_E o = new OSLP_E();
                    o = obtenerOslp(hdr.GetInt32(0));
                    lista.Add(o);
                }
                hdr.Close();
            }catch { }
            return lista;
        }
        public OSLP_E obtenerOslp(int SlpCode)
        {
            OSLP_E o = new OSLP_E();
            string query = "select \"SlpCode\",\"SlpName\",\"Memo\" from " + uti.schemaHana + "OSLP where \"SlpCode\"=" + SlpCode;
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                hdr.Read();
                o.SlpCode = hdr.GetInt32(0);
                if (!hdr.IsDBNull(1)) { o.SlpName = hdr.GetString(1); }
                if (!hdr.IsDBNull(2)) { o.Memo = hdr.GetString(2); }
                hdr.Close();
            }
            catch { }
            return o;
        }
    }
}
