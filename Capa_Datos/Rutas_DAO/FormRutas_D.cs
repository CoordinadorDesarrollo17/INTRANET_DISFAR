using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sap.Data.Hana;
using System.Data;
using System.Data.SqlClient;
using Capa_Entidad.Rutas_ENT;
using Capa_Entidad.Ventas_ENT;

namespace Capa_Datos.Rutas_DAO
{
    public class FormRutas_D
    {
        OrdenRegistroRutas_D orruD = new OrdenRegistroRutas_D();
        public string infoListaSocios(string FechaCont)
        {
            string info = "";
            foreach(TicketVenta_E t in orruD.listaSocios(FechaCont))
            {
                info += "<option value='" + t.CardName + "' CardCode='" + t.CardCode + "'></option>";
            }
            return info;
        }
        public string infoListaTicketsVenta(string FechaCont,string CardCode)
        {
            string info = "<option value=\"0\">Seleccione</option>";
            foreach(TicketVenta_E t in orruD.listarTicketsVenta(FechaCont,CardCode))
            {
                info += "<option value='" + t.DocEntry + "'>" + t.DocNum + "</option>";
            }
            return info;
        }
        public string infoListaProductosOWTQ(string guia,int linea)
        {
            if (guia == null || guia == "") { return ""; }
            string info = "<table border='1' class='table-secondary' style='font-size:11px;'>";
            info += "<tr style='background-color:darkgreen'>" +
                        "<th></th><th>#</th><th>Producto</th><th>Lote</th><th>Cant.</th><th>Lab.</th><th>Unid.Med.</th><th>CajasMaster</th>" +
                    "</tr>";
            int i = 0;
            foreach (RRU12_E r in orruD.listaProductosOWTQ(guia))
            {
                info += "<tr>" +
                            "<td><input name='RRU1["+linea+"].ListaRRU12["+i+"].BaseLinea' type='text' value='"+(linea+1)+"' style='width:25px;' readonly></td>"+
                            "<td><input name='RRU1["+linea+"].ListaRRU12["+i+"].Linea' id='ldrr"+linea+"rru"+i+"' type='text' value='"+(i+1)+"' style='width:25px;' readonly></td>" +
                            "<td><input name='RRU1["+linea+"].ListaRRU12["+i+"].ItemName' type='text' value='" + r.ItemName+"' style='width:200px;' readonly>" +
                                "<input name='RRU1["+linea+"].ListaRRU12["+i+"].ItemCode' type='hidden' value='" + r.ItemCode+"' style='width:200px;' readonly>"+
                            "</td>" +
                            "<td><input name='RRU1["+linea+"].ListaRRU12["+i+"].Lote' type='text' value='" + r.Lote+"' style='width:80px;' readonly></td>" +
                            "<td><input name='RRU1["+linea+"].ListaRRU12["+i+"].CantidadL' type='text' value='"+Math.Round(r.CantidadL, 0)+"' style='width:60px;' readonly></td>" +
                            "<td><input name='RRU1["+linea+"].ListaRRU12["+i+ "].LaboDesc' type='text' value='" + r.LaboDesc+"' style='width:50px;' readonly>" +
                                    "<input name='RRU1["+linea+"].ListaRRU12["+i+"].LaboCod' type='hidden' value='" + r.LaboCod+ "' style='width:50px;' readonly>"+
                            "</td>" +
                            "<td><input name='RRU1["+linea+"].ListaRRU12["+i+"].UnitMed' type='text' value='"+r.UnitMed+"' style='width:50px;' readonly>" +
                                "<input name='RRU1["+linea+"].ListaRRU12["+i+"].CantUnitMed' type='hidden' value='"+r.CantUnitMed+"' style='width:50px;' readonly>" +
                            "</td>" +
                            "<td><input name='RRU1["+linea+"].ListaRRU12["+i+"].Cajas' id='ldrr"+linea+"rruC"+i+"' type='number' style='width:50px;' onchange='actualizarCajas();'></td>" +
                        "</tr>";
                i++;
            }
            info += "</table>";
            return info;
        }

        /**********************************************/
    }
}
