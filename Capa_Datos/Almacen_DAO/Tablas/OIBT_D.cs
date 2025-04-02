using Capa_Entidad.Almacen_ENT.Tablas;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Almacen_DAO.Tablas
{
    public class OIBT_D
    {
        readonly Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public List<OIBT_E> ListarArticulosLotes(OIBT_E filtro = null, bool joinOITM = false, string limite = "500")
        {
            List<OIBT_E> lista = new List<OIBT_E>();
            string fil = string.Empty, query = string.Empty, select, innerJoin = string.Empty;
            if (filtro == null)
            {
                query = "select top 50 \"ItemCode\",\"BatchNum\",\"WhsCode\",\"ItemName\",TO_CHAR(\"ExpDate\", 'YYYY-MM-DD'), \"Quantity\" " +
                " from " + uti.schemaHana + "OIBT where \"ItemCode\" is not null order by \"ItemCode\"";
            }
            else
            {
                if (filtro.Quantity > 0) { fil += " and T1.\"Quantity\">0"; }
                if (!string.IsNullOrWhiteSpace(filtro.WhsCode)) { fil += " and T1.\"WhsCode\"='" + filtro.WhsCode + "'"; }
                if (!string.IsNullOrWhiteSpace(filtro.ItemCode)) { fil += " and T1.\"ItemCode\"='" + filtro.ItemCode + "'"; }
                if (!string.IsNullOrWhiteSpace(filtro.BatchNum)) { fil += " and T1.\"BatchNum\"='" + filtro.BatchNum + "'"; }

                select = "T1.\"ItemCode\", T1.\"BatchNum\", T1.\"WhsCode\", T1.\"ItemName\", TO_CHAR(T1.\"ExpDate\", 'YYYY-MM-DD'), T1.\"Quantity\"";

                if (joinOITM)
                {
                    select += $", T2.\"FirmCode\", (SELECT \"FirmName\" FROM {uti.schemaHana}OMRC WHERE \"FirmCode\" = T2.\"FirmCode\"), T2.\"BuyUnitMsr\", T2.\"NumInBuy\"";
                    innerJoin = $" INNER JOIN {uti.schemaHana}OITM T2 ON T2.\"ItemCode\" = T1.\"ItemCode\" ";
                }

                query = $"SELECT TOP {limite} {select} from {uti.schemaHana}OIBT T1 {innerJoin} WHERE T1.\"ItemCode\" IS NOT NULL {fil} order by T1.\"ItemCode\"";
            }


			try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    OIBT_E o = new OIBT_E();
                    if (!hdr.IsDBNull(0)) { o.ItemCode = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { o.BatchNum = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { o.WhsCode = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { o.ItemName = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { o.ExpDate = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { o.Quantity = hdr.GetDecimal(5); }

                    if (joinOITM)
                    {
                        if (!hdr.IsDBNull(6)) { o.FirmCode = hdr.GetInt32(6); }
                        if (!hdr.IsDBNull(7)) { o.FirmName = hdr.GetString(7); }
                        if (!hdr.IsDBNull(8)) { o.BuyUnitMsr = hdr.GetString(8); }
                        if (!hdr.IsDBNull(9)) { o.NumInBuy = hdr.GetDecimal(9); }
                    }

                    lista.Add(o);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
    }
}
