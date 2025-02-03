using Capa_Entidad.Almacen_ENT.Tablas;
using System;
using System.Collections.Generic;
using Sap.Data.Hana;
using Capa_Entidad.DireccionTecnica_ENT.TablasSql;
using System.Data.SqlClient;
using System.Text;

namespace Capa_Datos.Almacen_DAO.Tablas
{
	public class OITM_D
	{
		readonly Utilitarios uti = new Utilitarios(); DBHelper db = new DBHelper();
		public List<OITM_E> Listar(OITM_E fil)
		{
			List<OITM_E> lista = new List<OITM_E>();
			string filtros = string.Empty;
			if (fil != null)
			{
				if (!string.IsNullOrWhiteSpace(fil.ItemCode)) { filtros += " and upper(\"ItemCode\")like upper('%" + fil.ItemCode + "%')"; }
				if (!string.IsNullOrWhiteSpace(fil.ItemName)) { filtros += " and upper(\"ItemName\") like upper('%" + fil.ItemName + "%')"; }
				if (!string.IsNullOrWhiteSpace(fil.UserText)) { filtros += " and upper(\"UserText\") like upper('%" + fil.UserText + "%')"; }
				if (!string.IsNullOrWhiteSpace(fil.U_COB_EST_SKU)) { filtros += " and \"U_COB_EST_SKU\"='"+ fil.U_COB_EST_SKU + "'"; }
				if (!string.IsNullOrWhiteSpace(fil.validFor)) { filtros += " and \"validFor\"='" + fil.validFor+"'"; }
				if (fil.OnHand > 0.00M) { filtros += " and \"OnHand\" like '%" + fil.OnHand + "%'"; }
				if (fil.FirmCode > 0) { filtros += " and \"FirmCode\"=" + fil.FirmCode; }
				if (fil.ItmsGrpCod > 0) { filtros += " and \"ItmsGrpCod\"=" + fil.ItmsGrpCod; }
			}
			string query = "SELECT TOP 500 \"ItemCode\" FROM " + uti.schemaHana + "OITM " +
			"WHERE \"ItemCode\" is not null " + filtros + " ORDER BY \"ItemName\"";
			try
			{
				HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
				while (hdr.Read())
				{
					OITM_E pd = Obtener(hdr.GetString(0));
					lista.Add(pd);
				}
				hdr.Close();
			}
			catch { }
			return lista;
		}
		public string datalistArticulosLabo(OITM_E fil)
		{
			string lista = string.Empty;
			try
			{
				foreach (OITM_E o in Listar(fil))
				{
					lista += "<option ItemCode=\"" + o.ItemCode + "\" value=\"" + o.ItemName + "\"></option>";
				}
			}
			catch { }
			return lista;
		}
		public string datalistArticulos(OITM_E fil)
		{
			string lista = "<datalist id=\"ListaProductos\" >";
			try
			{
				foreach (OITM_E o in Listar(fil))
				{
                    lista += "<option ItemCode=\"" + o.ItemCode + "\" value=\"" + o.ItemName.Replace("\x022", "&quot;") + "\"></option>";

                }
            }
			catch { }
			lista += "</datalist>";
			return lista;
		}
		public OITM_E Obtener(string ItemCode)
		{
			OITM_E o = null;
			string query = "select \"ItemCode\",\"ItemName\",\"FrgnName\",\"ItmsGrpCod\",\"BuyUnitMsr\",(select top 1 x.\"Price\" from " + uti.schemaHana + "itm1 x where x.\"ItemCode\"='" + ItemCode + "'and x.\"PriceList\"=1)" +
				",(select top 1 y.\"FirmName\" from " + uti.schemaHana + "omrc y where y.\"FirmCode\"=\"FirmCode\"),\"FirmCode\"" +
				",\"AsstStatus\",\"UserText\",\"OnHand\",\"U_SYP_FAMILIA\",\"U_SYP_DFAM\",\"U_SYP_SUBFAMILIA\",\"U_SYP_DSFAM\"" +
				",\"U_SYP_FORM\",\"U_SYP_FFSIMP\",\"U_SYP_FFDET\",\"U_SYP_FORPR\",\"U_SYP_FABRICANTE\",\"U_SYP_TCONTROLADO\",\"NumInBuy\" " +
				" from " + uti.schemaHana + "oitm where \"ItemCode\"='" + ItemCode + "'";

			try
			{
				HanaDataReader hdr = db.HanaExecuteReaderNoSp(query);
				hdr.Read();
				o = new OITM_E();
				if (!hdr.IsDBNull(0)) { o.ItemCode = hdr.GetString(0); }
				if (!hdr.IsDBNull(1)) { o.ItemName = hdr.GetString(1); }
				if (!hdr.IsDBNull(2)) { o.FrgnName = hdr.GetString(2); }
				if (!hdr.IsDBNull(3)) { o.ItmsGrpCod = hdr.GetInt32(3); }
				if (!hdr.IsDBNull(4)) { o.BuyUnitMsr = hdr.GetString(4); }
				if (!hdr.IsDBNull(5)) { o.Price = hdr.GetDecimal(5); }
				if (!hdr.IsDBNull(6)) { o.FirmName = hdr.GetString(6); }
				if (!hdr.IsDBNull(7)) { o.FirmCode = hdr.GetInt32(7); }
				if (!hdr.IsDBNull(8)) { o.AsstStatus = hdr.GetString(8); }
				if (!hdr.IsDBNull(9)) { o.UserText = hdr.GetString(9); }
				if (!hdr.IsDBNull(10)) { o.OnHand = Math.Round(hdr.GetDecimal(10), 0); }
				if (!hdr.IsDBNull(11)) { o.U_SYP_FAMILIA = hdr.GetString(11); }
				if (!hdr.IsDBNull(12)) { o.U_SYP_DFAM = hdr.GetString(12); }
				if (!hdr.IsDBNull(13)) { o.U_SYP_SUBFAMILIA = hdr.GetString(13); }
				if (!hdr.IsDBNull(14)) { o.U_SYP_DSFAM = hdr.GetString(14); }
				if (!hdr.IsDBNull(15)) { o.U_SYP_FORM = hdr.GetString(15); }
				if (!hdr.IsDBNull(16)) { o.U_SYP_FFSIMP = hdr.GetString(16); }
				if (!hdr.IsDBNull(17)) { o.U_SYP_FFDET = hdr.GetString(17); }
				if (!hdr.IsDBNull(18)) { o.U_SYP_FORPR = hdr.GetString(18); }
				if (!hdr.IsDBNull(19)) { o.U_SYP_FABRICANTE = hdr.GetString(19); }
				if (!hdr.IsDBNull(20)) { o.U_SYP_TCONTROLADO = hdr.GetString(20); }
				if (!hdr.IsDBNull(21)) { o.NumInBuy = hdr.GetDecimal(21); }
				hdr.Close();
			}
			catch (Exception e) { throw new Exception(e.Message); }
			return o;
		}		
    }
}