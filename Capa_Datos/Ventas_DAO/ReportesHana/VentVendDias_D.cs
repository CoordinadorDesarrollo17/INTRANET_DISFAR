using Capa_Entidad.Ventas_ENT.ReportesHana;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Datos.Ventas_DAO.ReportesHana
{
    public class VentVendDias_D
    {
        Utilitarios uti = new Utilitarios();
        public List<VentVendDias_E> RptVentVendDias(DateTime Fecha)
        {
            List<VentVendDias_E> lista = new List<VentVendDias_E>();
            string query = "SELECT "+
                            " T0.\"SlpName\""+
                            " ,(select sum(\"DocTotal\") from "+uti.schemaHana+"oinv where \"SlpCode\"=T0.\"SlpCode\" and \"CANCELED\"='N' and \"DocDate\"=add_days('"+Fecha.ToString("yyyy-MM-dd")+"',0)) as \"Dia1\" "+
                            " ,(select sum(\"DocTotal\") from "+uti.schemaHana+"oinv where \"SlpCode\"=T0.\"SlpCode\" and \"CANCELED\"='N' and \"DocDate\"=add_days('"+Fecha.ToString("yyyy-MM-dd")+"',1)) as \"Dia2\" "+
                            " ,(select sum(\"DocTotal\") from "+uti.schemaHana+"oinv where \"SlpCode\"=T0.\"SlpCode\" and \"CANCELED\"='N' and \"DocDate\"=add_days('"+Fecha.ToString("yyyy-MM-dd")+"',2)) as \"Dia3\" "+
                            " ,(select sum(\"DocTotal\") from "+uti.schemaHana+"oinv where \"SlpCode\"=T0.\"SlpCode\" and \"CANCELED\"='N' and \"DocDate\"=add_days('"+Fecha.ToString("yyyy-MM-dd")+"',3)) as \"Dia4\" "+
                            " ,(select sum(\"DocTotal\") from "+uti.schemaHana+"oinv where \"SlpCode\"=T0.\"SlpCode\" and \"CANCELED\"='N' and \"DocDate\"=add_days('"+Fecha.ToString("yyyy-MM-dd")+"',4)) as \"Dia5\" "+
                            " ,(select sum(\"DocTotal\") from "+uti.schemaHana+"oinv where \"SlpCode\"=T0.\"SlpCode\" and \"CANCELED\"='N' and \"DocDate\"=add_days('"+Fecha.ToString("yyyy-MM-dd")+"',5)) as \"Dia6\" "+
                            " ,(select sum(\"DocTotal\") from "+uti.schemaHana+"oinv where \"SlpCode\"=T0.\"SlpCode\" and \"CANCELED\"='N' and \"DocDate\"=add_days('"+Fecha.ToString("yyyy-MM-dd")+"',6)) as \"Dia7\" "+
                            " FROM "+uti.schemaHana+"oslp T0"+ 
                            " where T0.\"Memo\"='VENTAS' order by 1";
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand cmd = new HanaCommand(query, hcn);
                HanaDataReader hdr = cmd.ExecuteReader();
                while (hdr.Read())
                {
                    VentVendDias_E obj = new VentVendDias_E();
                    if (!hdr.IsDBNull(0)) { obj.Vendedor = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { obj.TotDia1 = hdr.GetDecimal(1); }
                    if (!hdr.IsDBNull(2)) { obj.TotDia2 = hdr.GetDecimal(2); }
                    if (!hdr.IsDBNull(3)) { obj.TotDia3 = hdr.GetDecimal(3); }
                    if (!hdr.IsDBNull(4)) { obj.TotDia4 = hdr.GetDecimal(4); }
                    if (!hdr.IsDBNull(5)) { obj.TotDia5 = hdr.GetDecimal(5); }
                    if (!hdr.IsDBNull(6)) { obj.TotDia6 = hdr.GetDecimal(6); }
                    if (!hdr.IsDBNull(7)) { obj.TotDia7 = hdr.GetDecimal(7); }
                    obj.CalcTotal();
                    lista.Add(obj);
                }
                hdr.Close();
            }
            catch{ }
            finally { hcn.Close(); }
            return lista;
        }
    }
}
