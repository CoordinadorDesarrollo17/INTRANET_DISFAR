using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Sap.Data.Hana;

namespace Capa_Datos.SocioNegocios_DAO.TablasExternas
{
    public class OCPR_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public List<OCPR_E> listarContactosVentasSocio(string CardCode)
        {
            List<OCPR_E> lista = new List<OCPR_E>();
            string query = "select \"CntctCode\",\"CardCode\",\"Name\",\"FirstName\",\"LastName\",\"Address\" from " + uti.schemaHana+"ocpr "+
                           " where \"CardCode\" = '"+CardCode+"'";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while(hdr.Read())
                {
                    OCPR_E o = new OCPR_E();
                    if (!hdr.IsDBNull(0)) { o.CntctCode = hdr.GetInt32(0); }
                    if (!hdr.IsDBNull(1)) { o.CardCode = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { o.Name = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { o.FirstName = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { o.LastName = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { o.Address = hdr.GetString(5); }
                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
    }
}
