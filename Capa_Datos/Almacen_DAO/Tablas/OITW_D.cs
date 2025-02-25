using Capa_Entidad.Almacen_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sap.Data.Hana;
using Capa_Entidad.Almacen_ENT.TablasSql;
using DocumentFormat.OpenXml.Vml;
using System.Windows.Forms;

namespace Capa_Datos.Almacen_DAO.Tablas
{
    public class OITW_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public List<OITW_E> ListarDetArticulosInv(OITW_E obj)
        {
            List<OITW_E> lista = new List<OITW_E>();
            string query = $@"
        SELECT 
            T1.""WhsCode"" AS ""Almacen"",
            T1.""ItemCode"" AS ""SKU"",
            T1.""OnHand"" AS ""StockDisponible"",
            T1.""OnOrder"" AS ""StockEnOrden"",
            T1.""IsCommited"" AS ""StockComprometido"",
            (T1.""OnHand"" - T1.""IsCommited"") AS ""StockLibre""
        FROM {uti.schemaHana}OITW T1
        WHERE T1.""ItemCode"" = '{obj.ItemCode}'
        ORDER BY T1.""WhsCode""";

            try
            {
                using (HanaDataReader hdr = db.HanaExecuteReaderNoSp(query))
                {
                    while (hdr.Read())
                    {
                        OITW_E o = new OITW_E
                        {
                            ItemCode = hdr.IsDBNull(1) ? string.Empty : hdr.GetString(1),
                            WhsCode = hdr.IsDBNull(0) ? string.Empty : hdr.GetString(0),
                            OnHand = hdr.IsDBNull(2) ? 0 : Math.Round(hdr.GetDecimal(2), 0),
                            OnOrder = hdr.IsDBNull(3) ? 0 : Math.Round(hdr.GetDecimal(3), 0),
                            IsCommited = hdr.IsDBNull(4) ? 0 : Math.Round(hdr.GetDecimal(4), 0)
                        };
                        lista.Add(o);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error en ListarDetArticulosInv");
            }

            return lista;
        }

    }
}
