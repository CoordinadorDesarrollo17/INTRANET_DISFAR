using Capa_Entidad.Ventas_ENT.ReportesHana;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Ventas_DAO.ReportesHana
{
    public class VentSkuCliDias_D
    {
        Utilitarios uti = new Utilitarios();
        public List<VentSkuCliDias_E> RptVentSkuCliDias(DateTime Fecha, string ItemCodeIni, string ItemCodeFin
            , string CardCodeIni, string CardCodeFin)
        {
            List<VentSkuCliDias_E> lista = new List<VentSkuCliDias_E>();
            string query = "";
			query = "SELECT " +
					"T1.\"ItemCode\" " +
					",(select \"ItemName\" from " + uti.schemaHana + "oitm where \"ItemCode\" = T1.\"ItemCode\") as \"Item\" " +
					",T0.\"CardCode\" " +
					",(select \"CardName\" from " + uti.schemaHana + "ocrd where \"CardCode\" = T0.\"CardCode\") as \"Cli\" " +
					",(select sum(u1.\"LineTotal\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=t0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',0)) as \"TotDia1\" " +
					",(select sum(u1.\"LineTotal\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=t0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',1)) as \"TotDia2\" " +
					",(select sum(u1.\"LineTotal\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=t0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',2)) as \"TotDia3\" " +
					",(select sum(u1.\"LineTotal\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=t0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',3)) as \"TotDia4\" " +
					",(select sum(u1.\"LineTotal\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=t0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',4)) as \"TotDia5\" " +
					",(select sum(u1.\"LineTotal\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=t0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',5)) as \"TotDia6\" " +
					",(select sum(u1.\"LineTotal\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=t0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',6)) as \"TotDia7\" " +
					",(select sum(u1.\"Quantity\"*u1.\"NumPerMsr\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=T0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',0)) as \"Cant1\" " +
					",(select sum(u1.\"Quantity\"*u1.\"NumPerMsr\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=T0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',1)) as \"Cant2\" " +
					",(select sum(u1.\"Quantity\"*u1.\"NumPerMsr\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=T0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',2)) as \"Cant3\" " +
					",(select sum(u1.\"Quantity\"*u1.\"NumPerMsr\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=T0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',3)) as \"Cant4\" " +
					",(select sum(u1.\"Quantity\"*u1.\"NumPerMsr\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=T0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',4)) as \"Cant5\" " +
					",(select sum(u1.\"Quantity\"*u1.\"NumPerMsr\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=T0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',5)) as \"Cant6\" " +
					",(select sum(u1.\"Quantity\"*u1.\"NumPerMsr\") " +
						"from " + uti.schemaHana + "oinv u0 " +
						"inner join " + uti.schemaHana + "inv1 u1 on u1.\"DocEntry\"=u0.\"DocEntry\" and u1.\"ItemCode\"=T1.\"ItemCode\" " +
						"where u0.\"CANCELED\"='N' and u0.\"CardCode\"=T0.\"CardCode\" " +
							  "and u0.\"DocDate\"=add_days('" + Fecha.ToString("yyyy-MM-dd") + "',6)) as \"Cant7\" " +
					"FROM " + uti.schemaHana + "OINV T0 " +
					"INNER JOIN " + uti.schemaHana + "INV1 T1 ON T1.\"DocEntry\" = T0.\"DocEntry\" AND T1.\"ItemCode\" is not null " +
					"WHERE T0.\"CANCELED\"='N' and T0.\"DocDate\" between '" + Fecha.ToString("yyyy-MM-dd") + "' and add_days('" + Fecha.ToString("yyyy-MM-dd") + "',7) " +
						  "and T0.\"CardCode\" between '" + CardCodeIni + "' and '" + CardCodeFin + "' " +
					"GROUP BY T1.\"ItemCode\",T0.\"CardCode\" " +
					"ORDER BY T1.\"ItemCode\",T0.\"CardCode\" ";
			HanaConnection hcn = new HanaConnection(uti.cadHana);
			try
			{
				hcn.Open();
				HanaCommand cmd = new HanaCommand(query, hcn);
				HanaDataReader hdr = cmd.ExecuteReader();
				while (hdr.Read())
				{
					VentSkuCliDias_E obj = new VentSkuCliDias_E();
					if (!hdr.IsDBNull(0)) { obj.ItemCode = hdr.GetString(0); }
					if (!hdr.IsDBNull(1)) { obj.ItemName = hdr.GetString(1); }
					if (!hdr.IsDBNull(2)) { obj.CardCode = hdr.GetString(2); }
					if (!hdr.IsDBNull(3)) { obj.CardName = hdr.GetString(3); }
					if (!hdr.IsDBNull(4)) { obj.TotDia1 = hdr.GetDecimal(4); }
					if (!hdr.IsDBNull(5)) { obj.TotDia2 = hdr.GetDecimal(5); }
					if (!hdr.IsDBNull(6)) { obj.TotDia3 = hdr.GetDecimal(6); }
					if (!hdr.IsDBNull(7)) { obj.TotDia4 = hdr.GetDecimal(7); }
					if (!hdr.IsDBNull(8)) { obj.TotDia5 = hdr.GetDecimal(8); }
					if (!hdr.IsDBNull(9)) { obj.TotDia6 = hdr.GetDecimal(9); }
					if (!hdr.IsDBNull(10)) { obj.TotDia7 = hdr.GetDecimal(10); }
					if (!hdr.IsDBNull(11)) { obj.CantDia1 = hdr.GetDecimal(11); }
					if (!hdr.IsDBNull(12)) { obj.CantDia2 = hdr.GetDecimal(12); }
					if (!hdr.IsDBNull(13)) { obj.CantDia3 = hdr.GetDecimal(13); }
					if (!hdr.IsDBNull(14)) { obj.CantDia4 = hdr.GetDecimal(14); }
					if (!hdr.IsDBNull(15)) { obj.CantDia5 = hdr.GetDecimal(15); }
					if (!hdr.IsDBNull(16)) { obj.CantDia6 = hdr.GetDecimal(16); }
					if (!hdr.IsDBNull(17)) { obj.CantDia7 = hdr.GetDecimal(17); }
					obj.CalcTotal();
					lista.Add(obj);
				}
				hdr.Close();
			}
			catch { }
			finally { hcn.Close(); }
			return lista;
        }
    }
}
