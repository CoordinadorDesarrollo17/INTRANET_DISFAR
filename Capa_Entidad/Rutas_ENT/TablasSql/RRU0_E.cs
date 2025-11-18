using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System.Web;
using System.ComponentModel;

namespace Capa_Entidad.Rutas_ENT.TablasSql
{
	public class RRU0_E
	{
		public int DocEntry { get; set; }
		public int Linea { get; set; }
		public int DocEntryTicket { get; set; }
		[DisplayName("Num Ticket")]
		public int DocNumTicket { get; set; }
		public string Socio { get; set; }
		public string Guias { get; set; }
        public string ConducYPlaca { get; set; } // ✅ AGREGAR ESTA PROPIEDAD

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
		public string EstadoPago { get; set; }
		public string EstadoTC { get; set; }					// Tickets a cuadrar
		public string TipoPagoTC { get; set; }					// Tickets a cuadrar
		public decimal MontoRecibidoEfectivo { get; set; }		// Tickets a cuadrar
		public int IdOTC { get; set; }							// Tickets a cuadrar
		public decimal MontoRecibidoDeposito { get; set; }			// Tickets a cuadrar

		public static List<RRU0_E> listaFinalDetalles(List<RRU0_E> dt)
		{
			List<RRU0_E> lista = new List<RRU0_E>();
			int linea = 1;
			foreach (RRU0_E reg in dt)
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
		/*
        public static object veriftiemEnt(DateTime Tiem)
        {
            if (Tiem.Year == 1) { return null; }
            else return Tiem;
        }*/
		public static DataTable tbDetalle(List<RRU0_E> dt)
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


            foreach (RRU0_E reg in listaFinalDetalles(dt))
			{
				tb.Rows.Add(reg.DocEntry, reg.Linea, reg.DocEntryTicket, reg.DocNumTicket, reg.Socio, reg.Guias, reg.Verificado,
					reg.Cajas, reg.Observaciones, reg.MontoFinal, reg.Envio, reg.Direcciones, reg.Estado, reg.TempI1, reg.HumedI1,
					reg.TempI2, reg.HumedI2, reg.TempF1, reg.HumedF1, reg.TempF2, reg.HumedF2, reg.OpEntrega, reg.FechaEntrega,
					reg.HoraEntrega, reg.ConducYPlaca);
			}
			return tb;
		}
	}
}