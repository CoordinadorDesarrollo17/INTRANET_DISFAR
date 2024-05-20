using Capa_Entidad.Compras_ENT.Tablas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sap.Data.Hana;

namespace Capa_Datos.Compras_DAO.Tablas
{
    public class PCH1_D
    {
        DBHelper db = new DBHelper();Utilitarios uti = new Utilitarios();
        public List<PCH1_E> listarDetallesPch1(int DocEntry)
        {
            List<PCH1_E> lista = new List<PCH1_E>();
            string query = "select \"DocEntry\",\"LineNum\",\"ItemCode\",\"Dscription\",\"Quantity\",\"WhsCode\",\"UomCode\" " +
                "from "+uti.schemaHana+"pch1 where \"DocEntry\"=" + DocEntry;
            try
            {
                HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
                while (hdr.Read())
                {
                    PCH1_E p = new PCH1_E();
                    if (!hdr.IsDBNull(0)) { p.DocEntry = hdr.GetInt32(0); }
                    if (!hdr.IsDBNull(1)) { p.LineNum = hdr.GetInt32(1); }
                    if (!hdr.IsDBNull(2)) { p.ItemCode = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { p.Dscription = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { p.Quantity = Math.Round(hdr.GetDecimal(4),0); }
                    if (!hdr.IsDBNull(5)) { p.WhsCode = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { p.UomCode = hdr.GetString(6); }
                    lista.Add(p);
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
    }
}
