using Capa_Entidad.Ventas_ENT.ReportesHana;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Ventas_DAO.ReportesHana
{
    public class VentCliDias_D
    {
        Utilitarios uti = new Utilitarios();
        public List<VentCliDias_E> RptVentCliDias(DateTime Fecha, string CardCodeIni, string CardCodeFin)
        {
            List<VentCliDias_E> lista = new List<VentCliDias_E>();
            string query = "select T0.\"CardCode\",T0.\"CardName\"" +
                            ",(select sum(\"DocTotal\") from "+uti.schemaHana+"oinv" + 
	                            " where \"CANCELED\"='N' and \"CardCode\"=T0.\"CardCode\" and \"DocDate\" ='"+Fecha.ToString("yyyy-MM-dd")+"' ) as Dia1"+
                            ",(select sum(\"DocTotal\") from "+uti.schemaHana+"oinv" +
	                            " where \"CANCELED\"='N' and \"CardCode\"=T0.\"CardCode\" and \"DocDate\" =add_days('"+Fecha.ToString("yyyy-MM-dd")+"',1)) as Dia2"+
                            ",(select sum(\"DocTotal\") from " + uti.schemaHana + "oinv" +
                                   " where \"CANCELED\"='N' and \"CardCode\"=T0.\"CardCode\" and \"DocDate\" =add_days('"+Fecha.ToString("yyyy-MM-dd")+"',2)) as Dia3" +
                            ",(select sum(\"DocTotal\") from " + uti.schemaHana + "oinv" +
                                   " where \"CANCELED\"='N' and \"CardCode\"=T0.\"CardCode\" and \"DocDate\" =add_days('"+Fecha.ToString("yyyy-MM-dd")+"',3)) as Dia4" +
                            ",(select sum(\"DocTotal\") from " + uti.schemaHana + "oinv" +
                                   " where \"CANCELED\"='N' and \"CardCode\"=T0.\"CardCode\" and \"DocDate\" =add_days('"+Fecha.ToString("yyyy-MM-dd")+"',4)) as Dia5" +
                            ",(select sum(\"DocTotal\") from " + uti.schemaHana + "oinv" +
                                   " where \"CANCELED\"='N' and \"CardCode\"=T0.\"CardCode\" and \"DocDate\" =add_days('"+Fecha.ToString("yyyy-MM-dd")+"',5)) as Dia6" +
                            ",(select sum(\"DocTotal\") from " + uti.schemaHana + "oinv" +
                                   " where \"CANCELED\"='N' and \"CardCode\"=T0.\"CardCode\" and \"DocDate\" =add_days('"+Fecha.ToString("yyyy-MM-dd")+"',6)) as Dia7" +
                            " from "+uti.schemaHana+"ocrd T0 where T0.\"CardType\"='C' and T0.\"CardCode\" between '"+CardCodeIni+"' and '"+CardCodeFin+"'";
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand cmd = new HanaCommand(query, hcn);
                HanaDataReader hdr = cmd.ExecuteReader();
                while (hdr.Read())
                {
                    VentCliDias_E obj = new VentCliDias_E();
                    if (!hdr.IsDBNull(0)) { obj.CardCode = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { obj.CardName = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { obj.TotDia1 = hdr.GetDecimal(2); }
                    if (!hdr.IsDBNull(3)) { obj.TotDia2 = hdr.GetDecimal(3); }
                    if (!hdr.IsDBNull(4)) { obj.TotDia3 = hdr.GetDecimal(4); }
                    if (!hdr.IsDBNull(5)) { obj.TotDia4 = hdr.GetDecimal(5); }
                    if (!hdr.IsDBNull(6)) { obj.TotDia5 = hdr.GetDecimal(6); }
                    if (!hdr.IsDBNull(7)) { obj.TotDia6 = hdr.GetDecimal(7); }
                    if (!hdr.IsDBNull(8)) { obj.TotDia7 = hdr.GetDecimal(8); }
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
