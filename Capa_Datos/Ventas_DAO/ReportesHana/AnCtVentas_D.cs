using Capa_Entidad.Ventas_ENT.Formularios;
using Capa_Entidad.Ventas_ENT.ReportesHana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sap.Data.Hana;
using System.Data;
using System.Data.SqlClient;
using Capa_Datos.Ventas_DAO.TablasSql;

namespace Capa_Datos.Ventas_DAO.ReportesHana
{
    public class AnCtVentas_D
    {
        Utilitarios uti = new Utilitarios();
        USR1_D usr1D = new USR1_D();
        public List<AnCtVentas_E> rptAnCtVentas(FrmAnCtVentas_E frm)
        {
            List<AnCtVentas_E> lista = new List<AnCtVentas_E>();
         
            string query = "select  T0.\"CardCode\",T0.\"CardName\",Dayofmonth(T1.\"DocDate\") as \"Dia\"" +
                ",sum(T1.\"DocTotal\"-T1.\"VatSum\") as \"TotVendCliDia\"" +
                ",(select sum(x.\"BaseAmnt\") " +
                    "from " + uti.schemaHana + "orin x where x.\"CANCELED\" = 'N' and x.\"SlpCode\" = T1.\"SlpCode\"" +
                    " and x.\"DocDate\" between '" + frm.FecIni + "' and '" + frm.FecFin + "' " +") as \"TotNcCliDia\" " +
                ",T1.\"SlpCode\"" +
                " from " + uti.schemaHana + "ocrd T0 " +
                " inner join "+uti.schemaHana+"ordr T1 on T1.\"CardCode\" = T0.\"CardCode\" and T1.\"CANCELED\" = 'N' and T1.\"DocDate\" between '"+frm.FecIni+ "' and '" + frm.FecFin + "' " +
                                        "and T1.\"DocStatus\"='C' and T1.\"SlpCode\" = " +frm.CodigoSap+
                
                " where T0.\"CardType\" = 'C'" +
                " group by T0.\"CardCode\",T0.\"CardName\",Dayofmonth(T1.\"DocDate\"),T1.\"SlpCode\"" +
                " order by T1.\"SlpCode\" desc,Dayofmonth(T1.\"DocDate\"),T0.\"CardName\" ";
            AnCtVentas_E aux = datosTkRpt(frm);
           // aux.Cuota = usr1D.buscarUsr1(frm.DocEntry, frm.ÝearU, frm.MonthU).Cuota;
            aux.Cuota = usr1D.buscarUsr1(frm.DocEntry, Convert.ToDateTime(frm.FecIni).Year, Convert.ToDateTime(frm.FecIni).Month).Cuota;
            HanaConnection hcn = new HanaConnection(uti.cadHana);
            try
            {
                hcn.Open();
                HanaCommand hcmd = new HanaCommand(query, hcn);
                hcmd.CommandType = CommandType.Text;
                hcmd.Parameters.AddWithValue("FecIni", frm.FecIni);
                hcmd.Parameters.AddWithValue("FecFin", frm.FecFin);
                hcmd.Parameters.AddWithValue("SlpCode", frm.CodigoSap);
                HanaDataReader hdr = hcmd.ExecuteReader();
                while(hdr.Read())
                {
                    AnCtVentas_E obj = new AnCtVentas_E(); ;
                    if (!hdr.IsDBNull(0)) { obj.CardCode = hdr.GetString(0); }
                    if (!hdr.IsDBNull(1)) { obj.CardName = hdr.GetString(1); }
                    if (!hdr.IsDBNull(2)) { obj.Dia = hdr.GetInt32(2); }
                    if (!hdr.IsDBNull(3)) { obj.TotVendCliDia = hdr.GetDecimal(3); }
                    if (!hdr.IsDBNull(4)) { obj.TotNcCliDia = hdr.GetDecimal(4); }
                    obj.DocEntry = frm.DocEntry;
                    obj.nombre = frm.nombre;
                    obj.PromTicket = aux.PromTicket;
                    obj.TotPagTk = aux.TotPagTk;
                    obj.TotAnTk = aux.TotAnTk;
                    obj.TotPendTk = aux.TotPendTk;
                    obj.Cuota = aux.Cuota;
                    lista.Add(obj);
                }
                hdr.Close();
                hcn.Close();
            }
            catch{ hcn.Close(); }
            return lista;
        }
        public AnCtVentas_E datosTkRpt(FrmAnCtVentas_E frm)
        {
            AnCtVentas_E obj = new AnCtVentas_E();
            string query1 = "select isnull(avg(MontoTotal),0) from ortv" +
                            " where PropietarioCod=@PropietarioCod and FechaTicket between @FecIni and @FecFin";
            string query2 = "select isnull(count(*),0) from ortv" +
                            " where PropietarioCod=@PropietarioCod and FechaTicket between @FecIni and @FecFin" +
                            " and EstadoPago = 'PAGADO'";
            string query3 = "select isnull(count(*),0) from ortv" +
                            " where PropietarioCod=@PropietarioCod and FechaTicket between @FecIni and @FecFin" +
                            " and EstadoPago = 'PENDIENTE'";
            string query4 = "select isnull(count(*),0) from ortv" +
                            " where PropietarioCod=@PropietarioCod and FechaTicket between @FecIni and @FecFin" +
                            " and EstadoPedido = 'ANULADO'";
            SqlConnection cn = new SqlConnection(uti.cadSql);
            try
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand(query1, cn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@PropietarioCod", frm.CodigoSap);
                cmd.Parameters.AddWithValue("@FecIni", frm.FecIni);
                cmd.Parameters.AddWithValue("@FecFin", frm.FecFin);
                obj.PromTicket = (decimal)cmd.ExecuteScalar();
                SqlCommand cmd2 = new SqlCommand(query2, cn);
                cmd2.CommandType = CommandType.Text;
                cmd2.Parameters.AddWithValue("@PropietarioCod", frm.CodigoSap);
                cmd2.Parameters.AddWithValue("@FecIni", frm.FecIni);
                cmd2.Parameters.AddWithValue("@FecFin", frm.FecFin);
                obj.TotPagTk = (int)cmd2.ExecuteScalar();
                SqlCommand cmd3 = new SqlCommand(query3, cn);
                cmd3.CommandType = CommandType.Text;
                cmd3.Parameters.AddWithValue("@PropietarioCod", frm.CodigoSap);
                cmd3.Parameters.AddWithValue("@FecIni", frm.FecIni);
                cmd3.Parameters.AddWithValue("@FecFin", frm.FecFin);
                obj.TotPendTk = (int)cmd3.ExecuteScalar();
                SqlCommand cmd4 = new SqlCommand(query4, cn);
                cmd4.CommandType = CommandType.Text;
                cmd4.Parameters.AddWithValue("@PropietarioCod", frm.CodigoSap);
                cmd4.Parameters.AddWithValue("@FecIni", frm.FecIni);
                cmd4.Parameters.AddWithValue("@FecFin", frm.FecFin);
                obj.TotAnTk = (int)cmd4.ExecuteScalar();
                cn.Close();
            }
            catch { cn.Close(); }
            return obj;
        }
    }
}
