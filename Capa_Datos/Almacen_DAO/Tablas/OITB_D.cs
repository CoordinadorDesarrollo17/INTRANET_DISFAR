using Capa_Entidad.Almacen_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sap.Data.Hana;

namespace Capa_Datos.Almacen_DAO.Tablas
{
    public class OITB_D
    {
        DBHelper db = new DBHelper(); Utilitarios uti = new Utilitarios();
        public List<OITB_E> listarGrupoArticulos()
        {
            List<OITB_E> lista = new List<OITB_E>();
            string query = "select top 50 \"ItmsGrpCod\",\"ItmsGrpNam\" from "+uti.schemaHana+"oitb order by 1";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    OITB_E o = new OITB_E();
                    if (!hdr.IsDBNull(0)) { o.ItmsGrpCod = hdr.GetInt32(0); }
                    if (!hdr.IsDBNull(1)) { o.ItmsGrpNam = hdr.GetString(1); }
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
    }
}
