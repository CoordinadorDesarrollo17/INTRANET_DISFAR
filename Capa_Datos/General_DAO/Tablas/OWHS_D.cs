using Capa_Entidad.General_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sap.Data.Hana;

namespace Capa_Datos.General_DAO.Tablas
{
    public class OWHS_D
    {
        DBHelper db = new DBHelper();Utilitarios uti = new Utilitarios();
        public List<OWHS_E> ListarAlmacenes(string alms)
        {
            List<OWHS_E> lista = new List<OWHS_E>();
            string query = "";
            if(alms==null)
            {
                query = "SELECT T0.\"WhsCode\",T0.\"WhsName\" FROM " + uti.schemaHana + "OWHS T0" +
                " WHERE T0.\"WhsCode\" NOT IN('00','05','06','07','08','CUAR07', 'DEV05') ORDER BY T0.\"WhsCode\"";
            }
            else if(alms=="todos")
            {
                query = "SELECT T0.\"WhsCode\",T0.\"WhsName\" FROM " + uti.schemaHana + "OWHS T0" +
                "  WHERE T0.\"WhsCode\" NOT IN('05', 'DEV05') ORDER BY T0.\"WhsCode\"";
            }
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    OWHS_E alm = new OWHS_E();
                    alm.WhsCode = hdr.GetString(0);
                    alm.WhsName = hdr.GetString(1);
                    lista.Add(alm);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
        public OWHS_E buscarAlmacen(string WhsCode)
        {
            OWHS_E alm=null;
            string query = "select \"WhsCode\",\"WhsName\" from " + uti.schemaHana+ "owhs where \"WhsCode\"='"+WhsCode+"'";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                hdr.Read();
                    alm = new OWHS_E();
                    alm.WhsCode = hdr.GetString(0);
                    alm.WhsName = hdr.GetString(1);
                hdr.Close();
            }
            catch { }
            return alm; 
        }
    }
}
