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
            string condWhere = string.Empty;
            if (obj != null)
            {
                if (!string.IsNullOrEmpty(obj.WhsCode))
                {
                    condWhere += $@"AND T1.""WhsCode"" = '{obj.WhsCode}'";
                }

                if (!string.IsNullOrEmpty(obj.ItemCode))
                {
                    condWhere += $@"AND T1.""ItemCode"" = '{obj.ItemCode}'";
                }
            }

            List<OITW_E> lista = new List<OITW_E>();

            string query = $@"
            SELECT 
                T1.""ItemCode"" AS ""SKU"",
                T2.""ItemName"" AS ""SKUDescripcion"",
                T1.""WhsCode"" AS ""Almacen"",
                T1.""OnHand"" AS ""StockDisponible"",
                T1.""OnOrder"" AS ""StockEnOrden"",
                T1.""IsCommited"" AS ""StockComprometido"",
                (T1.""OnHand"" - T1.""IsCommited"") AS ""StockLibrePiezas"",
                (T1.""OnHand"" - T1.""IsCommited"")/T2.""NumInBuy"" AS ""StockLibreUnidades""
            FROM {uti.schemaHana}OITW T1
            INNER JOIN {uti.schemaHana}OITM T2 ON T2.""ItemCode"" = T1.""ItemCode""
            WHERE 1 = 1
            {condWhere}
            ORDER BY T1.""WhsCode""";

            try
            {
                using (HanaDataReader hdr = db.HanaExecuteReaderNoSp(query))
                {
                    while (hdr.Read())
                    {
                        OITW_E o = new OITW_E();
                        o.ItemCode = hdr.IsDBNull(0) ? string.Empty : hdr.GetString(0);
                        o.ItemName = hdr.IsDBNull(1) ? string.Empty : hdr.GetString(1);
                        o.WhsCode = hdr.IsDBNull(2) ? string.Empty : hdr.GetString(2);
                        o.OnHand = hdr.IsDBNull(3) ? 0 : Math.Round(hdr.GetDecimal(3), 0);
                        o.OnOrder = hdr.IsDBNull(4) ? 0 : Math.Round(hdr.GetDecimal(4), 0);
                        o.IsCommited = hdr.IsDBNull(5) ? 0 : Math.Round(hdr.GetDecimal(5), 0);
                        o.StockLibrePiezas = hdr.IsDBNull(6) ? 0 : Math.Round(hdr.GetDecimal(6), 0);
                        o.StockLibreUnidades = hdr.IsDBNull(7) ? 0 : hdr.GetDecimal(7);
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

        public int ObtenerStockSKUPorAlmacen(string condicion, Dictionary<string, object> parametros)
        {
            var stock = 0;

            string query = $@"
            SELECT SUM(""OnHand"")
            FROM {uti.schemaHana}OITW
            {condicion}";

            try
            {
                using (HanaDataReader hdr = db.HanaExecuteReaderNoSp(query))
                {
                    while (hdr.Read())
                    {
                        stock = hdr.IsDBNull(0) ? 0 : Convert.ToInt32(hdr.GetDecimal(0));
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.RegistrarError(ex, "Error en inesperado en OITW_D - ObtenerStockSKUPorAlmacen()");
            }

            return stock;
        }

    }
}
