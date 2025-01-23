using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Rutas_ENT.ReportesSql;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;

namespace Capa_Datos.Almacen_DAO.Tablas
{
    //OBJETO OPDN REPRESENTA ENTRADA DE MERCANCIAS
    public class OPDN_D
    {
        Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
        public List<OPDN_E> Listar(OPDN_E filtro)
        {
            string SP = string.Empty; List<OPDN_E> lista = new List<OPDN_E>();

            try
            {
                if (filtro != null)
                {
                    // CASO DEV07
                    if (!string.IsNullOrEmpty(filtro.U_COB_LUGAREN) && filtro.U_COB_LUGAREN.Equals("DEV07"))
                    {
                        SP = $"{uti.schemaHana}\"COBE_LIST_TRANS_LOTES_DEV07\"";
                    }
                    // CASO 06 RETIRO MERCADO
                    else if (!string.IsNullOrEmpty(filtro.U_COB_LUGAREN) && filtro.U_COB_LUGAREN.Equals("06"))
                    {
                        SP = $"{uti.schemaHana}\"COBE_LIST_TRANS_LOTES_06\"";
                    }
                    // CASOS NORMALES CON ENTRADA DE MERCADERIA
                    else
                    {
                        SP = $"{uti.schemaHana}\"COBE_LIST_ENTRADA_MERCANCIA\"";

                    }
                }
                HanaDataReader hdr = db.HanaExecuteReaderSp(SP, filtro.DocNum, ((object)filtro.DocDate) ?? String.Empty, ((object)filtro.ItemCode) ?? String.Empty, ((object)filtro.BatchNum) ?? String.Empty, ((object)filtro.CardCode) ?? String.Empty, ((object)filtro.U_COB_LUGAREN) ?? String.Empty, ((object)filtro.NumAtCard) ?? String.Empty);

                if (!string.IsNullOrEmpty(filtro.U_COB_LUGAREN) && filtro.Quantity > 0 && (filtro.U_COB_LUGAREN.Equals("06") || filtro.U_COB_LUGAREN.Equals("DEV07")))
                {
                    decimal sumQuantity = 0;
                    while (sumQuantity < filtro.Quantity && hdr.Read())
                    {
                        OPDN_E o = new OPDN_E();
                        if (!hdr.IsDBNull(0)) { o.DocNum = hdr.GetInt32(0); }
                        if (!hdr.IsDBNull(1)) { o.DocDate = hdr.GetString(1); }
                        if (!hdr.IsDBNull(2)) { o.CardCode = hdr.GetString(2); }
                        if (!hdr.IsDBNull(3)) { o.CardName = hdr.GetString(3); }
                        if (!hdr.IsDBNull(4)) { o.Address = hdr.GetString(4); }
                        if (!hdr.IsDBNull(5)) { o.U_COB_LUGAREN = hdr.GetString(5); }
                        if (!hdr.IsDBNull(6)) { o.FirmCode = hdr.GetString(6); }
                        if (!hdr.IsDBNull(7)) { o.FirmName = hdr.GetString(7); }
                        if (!hdr.IsDBNull(8)) { o.ItemCode = hdr.GetString(8); }
                        if (!hdr.IsDBNull(9)) { o.Dscription = hdr.GetString(9); }
                        if (!hdr.IsDBNull(10)) { o.Quantity = hdr.GetDecimal(10); }//cantidad que viene desde la entrada de mercancia
                        if (!hdr.IsDBNull(11)) { o.NumInBuy = hdr.GetDecimal(11); }//valor que tiene esa unidad de medida
                        if (!hdr.IsDBNull(12)) { o.BuyUnitMsr = hdr.GetString(12); }//definicion de la unidad de medida
                        if (!hdr.IsDBNull(13)) { o.BatchNum = hdr.GetString(13); }
                        if (!hdr.IsDBNull(14)) { o.ExpDate = hdr.GetString(14); }
                        if (!hdr.IsDBNull(15)) { o.NumAtCard = hdr.GetString(15); }
                        if (!hdr.IsDBNull(16)) { o.NumAtCard = hdr.GetString(16); }
                        if (!hdr.IsDBNull(17)) { o.QuantityOIBT = hdr.GetDecimal(17); }//cantidad que viene desde la devolucion oibt
                        sumQuantity += o.Quantity * o.NumInBuy;
                        lista.Add(o);
                    }
                    hdr.Close();
                }
                else
                {
                    while (hdr.Read())
                    {
                        OPDN_E o = new OPDN_E();
                        if (!hdr.IsDBNull(0)) { o.DocNum = hdr.GetInt32(0); }
                        if (!hdr.IsDBNull(1)) { o.DocDate = hdr.GetString(1); }
                        if (!hdr.IsDBNull(2)) { o.CardCode = hdr.GetString(2); }
                        if (!hdr.IsDBNull(3)) { o.CardName = hdr.GetString(3); }
                        if (!hdr.IsDBNull(4)) { o.Address = hdr.GetString(4); }
                        if (!hdr.IsDBNull(5)) { o.U_COB_LUGAREN = hdr.GetString(5); }
                        if (!hdr.IsDBNull(6)) { o.FirmCode = hdr.GetString(6); }
                        if (!hdr.IsDBNull(7)) { o.FirmName = hdr.GetString(7); }
                        if (!hdr.IsDBNull(8)) { o.ItemCode = hdr.GetString(8); }
                        if (!hdr.IsDBNull(9)) { o.Dscription = hdr.GetString(9); }
                        if (!hdr.IsDBNull(10)) { o.Quantity = hdr.GetDecimal(10); }//cantidad que viene desde la entrada de mercancia
                        if (!hdr.IsDBNull(11)) { o.NumInBuy = hdr.GetDecimal(11); }
                        if (!hdr.IsDBNull(12)) { o.BuyUnitMsr = hdr.GetString(12); }
                        if (!hdr.IsDBNull(13)) { o.BatchNum = hdr.GetString(13); }
                        if (!hdr.IsDBNull(14)) { o.ExpDate = hdr.GetString(14); }
                        if (!hdr.IsDBNull(15)) { o.NumAtCard = hdr.GetString(15); }
                        if (!hdr.IsDBNull(16)) { o.NumAtCard = hdr.GetString(16); }
                        if (!hdr.IsDBNull(17)) { o.QuantityOIBT = hdr.GetDecimal(17); }//cantidad que viene desde la devolucion oibt
                        lista.Add(o);
                    }
                    hdr.Close();
                }
            }
            catch { }
            return lista;
        }



    }
}
