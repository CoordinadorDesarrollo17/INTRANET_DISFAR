using Capa_Entidad.DireccionTecnica_ENT.TablasExternas;
using Capa_Entidad.DireccionTecnica_ENT.TablasSql;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.DireccionTecnica_DAO.TablasExternas
{
    public class COB_ESTA_RS_D
    {
        readonly Utilitarios uti = new Utilitarios();

        public List<COB_ESTA_RS_E> ListarEstadoRegistrosSanitarios()
        {
            DBHelper db = new DBHelper();
            List<COB_ESTA_RS_E> lista = null;

            string query = @"SELECT TOP 50 ""Code"", ""Name"" FROM " + uti.schemaHana + @"""@COB_ESTA_RS""";

            try
            {
                using (HanaDataReader hdr = db.HanaExecuteReaderNoSp(query))
                {
                    lista = new List<COB_ESTA_RS_E>();

                    while (hdr.Read())
                    {
                        COB_ESTA_RS_E obj = new COB_ESTA_RS_E();
                        if (!hdr.IsDBNull(0)) { obj.Code = hdr.GetString(0); }
                        if (!hdr.IsDBNull(1)) { obj.Name = hdr.GetString(1); }

                        lista.Add(obj);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return lista;
        }
    }
}
