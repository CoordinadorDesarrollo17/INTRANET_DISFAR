using Capa_Entidad.Almacen_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sap.Data.Hana;

namespace Capa_Datos.Almacen_DAO.Tablas
{
    public class OMRC_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public List<OMRC_E> listarFabricantes()
        {
            List<OMRC_E> lista = new List<OMRC_E>();
            string query = "SELECT \"FirmCode\",\"U_SYP_DESC\",\"FirmName\" FROM " + uti.schemaHana + "OMRC WHERE \"U_SYP_DESC\" IS NOT NULL ORDER BY 2";
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    OMRC_E lb = new OMRC_E();
                    lb.FirmCode = hdr.GetInt32(0);
                    lb.U_SYP_DESC = hdr.GetString(1);
                    if (!hdr.IsDBNull(2)) {lb.FirmName= hdr.GetString(2); }
                    lista.Add(lb);
                }
                hdr.Close();
            }
            catch {}
            return lista;
        }
    }
}
