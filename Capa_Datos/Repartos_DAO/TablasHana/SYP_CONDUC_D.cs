using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Repartos_DAO.TablasHana
{
    public class SYP_CONDUC_D
    {
        private readonly Utilitarios uti = new Utilitarios();
        private readonly DBHelper db = new DBHelper();
        public List<string> listar(){

             var lista = new List<string>();
              string query = $@"
            SELECT
                ""U_SYP_CHNO"" AS ""Nombres""
            FROM 
                 {uti.schemaHana}""@SYP_CONDUC"" ";
            try
            {
                using (var hdr = db.HanaExecuteReaderNoSp(query))
                {
                    while (hdr.Read())
                    {
                        if (!hdr.IsDBNull(0))
                        {
                            lista.Add(hdr.GetString(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }

            return lista;
        }
    }
}
