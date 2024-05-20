using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Entidad.Ventas_ENT;
using Capa_Entidad.Ventas_ENT.Tablas;

namespace Capa_Datos.Ventas_DAO
{
    public class FormTicketVenta_D
    {
        TicketVenta_D ticketD = new TicketVenta_D();
        public string generaInfoListaClientes(string Fecha)
        {
            string info = "<datalist id='ListaClientes'>";             
                 foreach (OCRD_E x in ticketD.listarClientes(Fecha))
                 {
                     info+="<option CardCode='"+x.CardCode+"' value='"+x.CardName+"'></option>";
                 }
            info += "</datalist>";
            return info;
        }
        public string generaInfoListaDirDestinos(string CardCode)
        {
            string info = "<datalist id='ListaDirDestino'>";
            foreach (string x in ticketD.listarDirDestinos(CardCode))
            {
                info += "<option value='" + x + "'>"+x+"</option>";
            }
            info += "</datalist>";
            return info;
        }
        public string generaInfoListaOrdenesDeVenta(string Fecha, string CardCode)
        {
            string info = "<thead><tr style='background-color:darkgreen'><th>Verificar</th><th>#</th><th>Monto</th>" +
                              "<th>Nro SAP</th><th>TipoComprobante</th><th>Vendedor</th>"+
                              "<th>LugarDeEntrega</th><th>AlmacenSalida</th><th>Obs</th></tr></thead><tbody class='table-light table-responsive-sm'>";
            int linea = 1;
            List<Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E> lista = ticketD.listarOrdenesdeVentaFinales(Fecha, CardCode);
            foreach(Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E o in lista)
            {
                info += "<tr><td align='center'><input id='Verificar" + linea+"' name='Det[" + (linea-1)+ "].Verificar' type='checkbox' onclick=\"validacionVerificarMontos()\" /></td>" +
                    "<td><input id='Linea"+linea+"' name='Det[" + (linea-1)+"].Linea' type='text' value='"+linea+"' readonly size=3 /></td>" +
                    "<td><input id='Monto"+linea+"' name='Det[" + (linea-1)+"].Monto' type='text' value='"+ String.Format("{0:0.00}",o.DocTotal)+"' readonly size=10 /></td>" +
                    "<td><input id='NroSap"+linea+"' name='Det[" + (linea-1)+"].NroSap' type='text' value='"+o.DocNum+"' readonly size=8 /></td>" +
                    "<td><select id='TipoComprobante"+linea+"' name='Det[" + (linea-1)+ "].TipoComprobante'><option value=''>Seleccione</option><option value='Factura'>Factura</option><option value='Boleta'>Boleta</option><option value='F/B'>F/B</option></select></td>" +
                    "<td><input id='Vendedor"+linea+"' name='Det[" + (linea-1)+"].Vendedor' type='text' value='"+o.SlpName+"' readonly size=10 /></td>" +
                    "<td><input id='LugarDeEntrega"+linea+"' name='Det[" + (linea-1)+ "].LugarDeEntrega' type='text' value='"+o.LugarDeEntrega+ "' readonly size=12 /></td>" +
                    "<td><input id='AlmacenSalida" + linea + "' name='Det[" + (linea - 1) + "].AlmacenSalida' type='text' value='" + o.AlmacenSalida + "' readonly size=10 /></td>" +
                    "<td><input id='Observaciones" +linea+"' name='Det[" + (linea-1)+ "].Observaciones' type='text' size=30 /></td></tr>";
                linea++;
            }
            info += "</tbody>";
            return info;
        }
        public string generaInfoListaCorreosCliente(string CardCode)
        {
            string info = "<datalist id='ListaCorreos'>";
            foreach(string x in ticketD.listarCorreosCliente(CardCode))
            {
                info += "<option value='"+x+"'>" + x + "</option>";
            }
            info += "</datalist>";
            return info;
        }
        public string generaInfoListaNotasDeCreditoV(string CardCode)
        {
            string info = "<tr><td><strong>Ver</strong></td><td><strong>#</strong></td><td><strong>Monto</strong></td>" +
                              "<td><strong>Fecha</strong></td><td><strong>Nro SAP</strong></td></tr>";
            int linea = 1;
            List<ORIN_E> lista = ticketD.listarNotasDeCreditoV(CardCode);
            if (lista.Count == 0) { return ""; }
            foreach(ORIN_E n in lista)
            {
                info += "<tr>" +
                            "<td><input type='checkbox' name='Det2["+(linea-1)+"].Verificar' style=\"width:10px;\" onclick=\"validacionVerificarMontos()\"></td>" +
                            "<td><input type='text' name='Det2["+(linea-1)+"].Linea' style=\"width:40px;\" value='" + linea+"' readonly></td>" +
                            "<td><input type='text' name='Det2["+(linea-1)+"].Nc.DocTotal' style=\"width:90px;\" value='" + String.Format("{0:0.00}", n.DocTotal) + "' readonly></td>"+
                            "<td><input type='text' name='Det2["+(linea-1)+"].Nc.DocDate' style=\"width:100px;\" value='" + n.DocDate+"' readonly></td>" +
                            "<td><input type='text' name='Det2["+(linea-1)+"].Nc.DocNum' style=\"width:100px;\" value='" + n.DocNum+"' readonly></td>" +
                        "</tr>";
                linea++;
            }
            return info;
        }
    }
}
