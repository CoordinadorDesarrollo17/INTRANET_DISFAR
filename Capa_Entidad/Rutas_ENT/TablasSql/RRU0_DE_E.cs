using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Capa_Entidad.Rutas_ENT.TablasSql
{
    public class RRU0_DE_E
    {
        public int DocEntry { get; set; }
        public int Linea { get; set; }
        public int DocEntryTicket { get; set; }
        [DisplayName("Num Ticket")]
        public int DocNumTicket { get; set; }
        public string Socio { get; set; }
        public string Guias { get; set; }
        public string ConducYPlaca { get; set; }
        public string Verificado { get; set; }
        public int Cajas { get; set; }
        public string Observaciones { get; set; }
        public decimal MontoFinal { get; set; }
        public decimal Envio { get; set; }
        public string Direcciones { get; set; }
        public string Estado { get; set; }
        public decimal TempI1 { get; set; }
        public decimal TempI2 { get; set; }
        public decimal HumedI1 { get; set; }
        public decimal HumedI2 { get; set; }
        public decimal TempF1 { get; set; }
        public decimal TempF2 { get; set; }
        public decimal HumedF1 { get; set; }
        public decimal HumedF2 { get; set; }
        public string OpEntrega { get; set; }
        public string FechaEntrega { get; set; }
        public string HoraEntrega { get; set; }
        public ORTV_E Ticket { get; set; }
        public string ComentarioLiberado { get; set; }

        /****************** C A M P O S   Q U E   N O   S O N   D E   L A   T A B L A ******************/
        public string Operario { get; set; }
        public HttpPostedFileBase Archivo { get; set; }
        public string TipoVenta { get; set; }
        public string FormaPago { get; set; }
        public string EstadoPago { get; set; }
        public string EstadoTC { get; set; }                    // Tickets a cuadrar
        public string TipoPagoTC { get; set; }                  // Tickets a cuadrar
        public decimal MontoRecibidoEfectivo { get; set; }      // Tickets a cuadrar
        public int IdOTC { get; set; }                          // Tickets a cuadrar
        public decimal MontoRecibidoDeposito { get; set; }          // Tickets a cuadrar
        public string EnvioAgencia { get; set; }
        public string CardCode { get; set; }
        public string DirDestino { get; set; }
        public int TotalCajasDevolucion { get; set; }  // ✅ NUEVO: suma de cajas de RRU0_DE

        public static List<RRU0_DE_E> listaFinalDetallesDE(List<RRU0_DE_E> dt)
        {
            List<RRU0_DE_E> lista = new List<RRU0_DE_E>();
            int linea = 1;
            foreach (RRU0_DE_E reg in dt)
            {
                if (reg.DocEntryTicket > 0)
                {
                    reg.Linea = linea;
                    lista.Add(reg);
                    linea++;
                }
            }
            return lista;
        }

        public static DataTable tbDetalleDE(List<RRU0_DE_E> dt)
        {
            DataTable tb = new DataTable();
            tb.Columns.Add("DocEntry", typeof(int));
            tb.Columns.Add("Linea", typeof(int));
            tb.Columns.Add("DocEntryTicket", typeof(int));
            tb.Columns.Add("DocNumTicket", typeof(int));
            tb.Columns.Add("Socio", typeof(string));
            tb.Columns.Add("Guias", typeof(string));
            tb.Columns.Add("Verificado", typeof(string));
            tb.Columns.Add("Cajas", typeof(int));
            tb.Columns.Add("Observaciones", typeof(string));
            tb.Columns.Add("MontoFinal", typeof(decimal));
            tb.Columns.Add("Envio", typeof(decimal));
            tb.Columns.Add("Direcciones", typeof(string));
            tb.Columns.Add("Estado", typeof(string));
            tb.Columns.Add("TempI1", typeof(decimal));
            tb.Columns.Add("HumedI1", typeof(decimal));
            tb.Columns.Add("TempI2", typeof(decimal));
            tb.Columns.Add("HumedI2", typeof(decimal));
            tb.Columns.Add("TempF1", typeof(decimal));
            tb.Columns.Add("HumedF1", typeof(decimal));
            tb.Columns.Add("TempF2", typeof(decimal));
            tb.Columns.Add("HumedF2", typeof(decimal));
            tb.Columns.Add("OpEntrega", typeof(string));
            tb.Columns.Add("FechaEntrega", typeof(string));
            tb.Columns.Add("HoraEntrega", typeof(string));
            tb.Columns.Add("ConducYPlaca", typeof(string));
            tb.Columns.Add("EnvioAgencia", typeof(string));

            foreach (RRU0_DE_E reg in listaFinalDetallesDE(dt))
            {
                DataRow row = tb.NewRow();
                row["DocEntry"] = reg.DocEntry;
                row["Linea"] = reg.Linea;
                row["DocEntryTicket"] = reg.DocEntryTicket;
                row["DocNumTicket"] = reg.DocNumTicket;
                row["Socio"] = string.IsNullOrEmpty(reg.Socio) ? DBNull.Value : (object)reg.Socio;
                row["Guias"] = string.IsNullOrEmpty(reg.Guias) ? DBNull.Value : (object)reg.Guias;
                row["Verificado"] = string.IsNullOrEmpty(reg.Verificado) ? DBNull.Value : (object)reg.Verificado;
                row["Cajas"] = reg.Cajas;
                row["Observaciones"] = string.IsNullOrEmpty(reg.Observaciones) ? DBNull.Value : (object)reg.Observaciones;
                row["MontoFinal"] = reg.MontoFinal;
                row["Envio"] = reg.Envio;
                row["Direcciones"] = string.IsNullOrEmpty(reg.Direcciones) ? DBNull.Value : (object)reg.Direcciones;
                row["Estado"] = string.IsNullOrEmpty(reg.Estado) ? (object)"PREENVIO" : (object)reg.Estado;
                row["TempI1"] = reg.TempI1;
                row["HumedI1"] = reg.HumedI1;
                row["TempI2"] = reg.TempI2;
                row["HumedI2"] = reg.HumedI2;
                row["TempF1"] = reg.TempF1;
                row["HumedF1"] = reg.HumedF1;
                row["TempF2"] = reg.TempF2;
                row["HumedF2"] = reg.HumedF2;
                row["OpEntrega"] = string.IsNullOrEmpty(reg.OpEntrega) ? DBNull.Value : (object)reg.OpEntrega;
                row["FechaEntrega"] = string.IsNullOrEmpty(reg.FechaEntrega) ? DBNull.Value : (object)reg.FechaEntrega;
                row["HoraEntrega"] = string.IsNullOrEmpty(reg.HoraEntrega) ? DBNull.Value : (object)reg.HoraEntrega;
                row["ConducYPlaca"] = string.IsNullOrEmpty(reg.ConducYPlaca) ? DBNull.Value : (object)reg.ConducYPlaca;
                row["EnvioAgencia"] = string.IsNullOrEmpty(reg.EnvioAgencia) ? DBNull.Value : (object)reg.EnvioAgencia;

                tb.Rows.Add(row);
            }

            return tb;
        }
    }
}
