using Capa_Entidad.Almacen_ENT.Tablas;
using DocumentFormat.OpenXml.Office.Word;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2013.Excel;

namespace Capa_Datos.Almacen_DAO.Tablas
{
    public class ORPD_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
       /* public int BuscarDocNum(Capa_Entidad.Almacen_ENT.TablasSql.ORPD_E filtro)
        {
            int DocNum = 0;
            try
            {
                if (filtro.DetalleDevolucion != null && filtro.DetalleDevolucion.Count >= 1)
                {

                    HanaParameter tbDetDevolucion = new HanaParameter("SQLRPD1", HanaDbType.TableType);
                    tbDetDevolucion.Value = Capa_Entidad.Almacen_ENT.TablasSql.RPD1_E.TbDetalleFU(filtro.DetalleDevolucion);

                    HanaDataReader hdr = db.HanaExecuteReaderSp($"{uti.schemaHana}\"COBE_BUSCAR_DOCNUM\"", ((object)filtro.CardCode) ?? String.Empty, ((object)filtro.FechaDevolucion) ?? String.Empty, tbDetDevolucion.Value);
                    while (hdr.Read())
                    {
                        DocNum = hdr.GetInt32(0);

                    }
                    hdr.Close();
                }
            }
            catch { }
            return DocNum;
        }*/
        public List<ORPD_E> BuscarDevolucion(string DocDate, string CardCode,string RefFactura)
        {
            List<ORPD_E> lista = new List<ORPD_E>();
            try
            {

                HanaDataReader hdr = db.HanaExecuteReaderSp($"{uti.schemaHana}\"COBE_LIST_DEV_PROVEEDOR\"", ((object)DocDate) ?? String.Empty, ((object)CardCode) ?? String.Empty,((object)RefFactura) ?? String.Empty);

                while (hdr.Read())
                {
                    ORPD_E o = new ORPD_E();
                    if (!hdr.IsDBNull(0)) { o.DocNum = hdr.GetInt32(0); }
                    if (!hdr.IsDBNull(1)) { o.DocDate = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { o.CardCode = hdr.GetString(2); }
                    if (!hdr.IsDBNull(3)) { o.CardName = hdr.GetString(3); }
                    if (!hdr.IsDBNull(4)) { o.ItemCode = hdr.GetString(4); }
                    if (!hdr.IsDBNull(5)) { o.Dscription = hdr.GetString(5); }
                    if (!hdr.IsDBNull(6)) { o.WhsCode = hdr.GetString(6); }
                    if (!hdr.IsDBNull(7)) { o.FirmCode = hdr.GetInt32(7); }
                    if (!hdr.IsDBNull(8)) { o.BatchNum = hdr.GetString(8); }
                    if (!hdr.IsDBNull(9)) { o.ExpDate = hdr.GetString(9); }
                    if (!hdr.IsDBNull(10)) { o.Quantity = hdr.GetDecimal(10); }
                    if (!hdr.IsDBNull(11)) { o.NumInBuy = hdr.GetDecimal(11); }
                    if (!hdr.IsDBNull(12)) { o.BuyUnitMsr = hdr.GetString(12); }
                    if (!hdr.IsDBNull(13)) { o.RefFactura = hdr.GetString(13); }
                    lista.Add(o);

                    
                }
                hdr.Close();
            }
            catch { }
            return lista;
        }
    }
        
    
}
