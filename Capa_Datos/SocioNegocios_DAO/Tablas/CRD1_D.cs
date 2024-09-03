using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.SocioNegocios_DAO.Tablas
{
    public class CRD1_D
    {
        Utilitarios uti = new Utilitarios();
        public string BuscarZonaPedido(string shipToCode, string cardCode)
        {
            string zona = null;
            string query = $@"select ""Address2"" from {uti.schemaHana}CRD1 where ""CardCode"" ='{cardCode}' and ""Address"" ='{shipToCode}'";
            try
            {
                using (var hcn = new HanaConnection(uti.cadHana))
                {
                    
                    hcn.Open();
                    using (var command = new HanaCommand(query, hcn))
                    {
                        using (var hdr = command.ExecuteReader())
                        {
                            if (hdr.Read() && !hdr.IsDBNull(0))
                            {
                                zona = hdr.GetString(0);
                            }
                        }
                    }
                }
            }
            catch { }
            return zona;
        }
        
    }
}
