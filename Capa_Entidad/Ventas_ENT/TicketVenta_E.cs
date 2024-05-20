using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Capa_Entidad.Rutas_ENT;

namespace Capa_Entidad.Ventas_ENT
{
    public class TicketVenta_E
    {
        [DisplayName("Nro Interno")]
        public int DocEntry { get; set; }
        [DisplayName("Nro Ticket")]
        public int DocNum { get; set; }
		[DisplayName("FechaTk")]
		public string FechaTicket { get; set; }
		public string CardCode { get; set; }
		[DisplayName("Cliente")]
		public string CardName { get; set; }
		[DisplayName("HoraTk")]
		public string HoraTicket { get; set; }
		[DisplayName("Tipo de Venta")]
		public string TipoVenta { get; set; }
		[DisplayName("LgDestino")]
		public string LugarDestino { get; set; }
		public string Agencia { get; set; }
		public string DirDestino1 { get; set; }
		public string DirDestino2 { get; set; }
		[DisplayName("Nombre1")]
		public string NombrePer1 { get; set; }
		public string TipoDocPer1 { get; set; }
		public string DocPer1 { get; set; }
		public string TelfPer1 { get; set; }
		[DisplayName("Nombre2")]
		public string NombrePer2 { get; set; }
		public string TipoDocPer2 { get; set; }
		public string DocPer2 { get; set; }
		public string TelfPer2 { get; set; }
		public string Correo { get; set; }
		public string Observaciones { get; set; }
		public string Protocolo { get; set; }
		public string Embalaje { get; set; }
		public int PropietarioCod { get; set; }
		[DisplayName("Propietario")]
		public string PropietarioDesc { get; set; }
		public string Comentario { get; set; }
		public decimal MontoTotal { get; set; }
		public decimal Flete { get; set; }
		public decimal GastoEnvio { get; set; }
		public decimal DescuentoNC { get; set; }
		public decimal DeudaCliente { get; set; }
		public decimal DeudaEmpresa { get; set; }
		public decimal MontoFinal { get; set; }
		public string NroVentas { get; set; }
		public string FormaPago { get; set; }
		public decimal MontoRecibido { get; set; }
		[DisplayName("EstPago")]
		public string EstadoPago { get; set; }
		public string FechaPago { get; set; }
		public string HoraPago { get; set; }
		public int CajeroCod { get; set; }
		public string CajeroDesc { get; set; }
		[DisplayName("Estado")]
		public string EstadoPedido { get; set; }
		public string FechaSeparo { get; set; }
		public string HoraSeparo { get; set; }
		public string OpSeparo { get; set; }
		public string FechaSacar { get; set; }
		public string HoraSacar { get; set; }
		public string OpSacar { get; set; }
		public string FechaRecibir { get; set; }
		public string HoraRecibir { get; set; }
		public string OpRecibir { get; set; }
		public string FechaSacando { get; set; }
		public string HoraSacando { get; set; }
		[DisplayName("Sacador")]
		public string OpSacando { get; set; }
		public string FechaEmpacado { get; set; }
		public string HoraEmpacado { get; set; }
		public string OpEmpacado { get; set; }
		public string FechaEnvio { get; set; }
		public string HoraEnvio { get; set; }
		public string OpEnvio { get; set; }
		public string FechaEntrega { get; set; }
		public string HoraEntrega { get; set; }
		public string OpEntrega { get; set; }
		public string FechaAnulacion { get; set; }
		public string HoraAnulacion { get; set; }
		public string OpAnulacion { get; set; }
		public string UserId { get; set; }
		public int Cajas { get; set; }
		public string Observaciones2 { get; set; }
		public string FechaNC { get; set; }
		public string EstadoFacturacion { get; set; }
		public string FechaFacturacion { get; set; }
		public string HoraFacturacion { get; set; }
		public string OpFacturacion { get; set; }
		public string EstadoProtocolo { get; set; }
		public string FechaProtocolo { get; set; }
		public string HoraProtocolo { get; set; }
		public string OpProtocolo { get; set; }
		public string TiempoEntrega { get; set; }
		public string Guias { get; set; }
		public int NroMesa { get; set; }
		public string OpSacando2 { get; set; }
		public string OpChequeador { get; set; }
		public string OpEmpacado2 { get; set; }
		[DisplayName("SocioVentas")]
		public string CardCodeScVt { get; set; }
		public int CntctCode { get; set; }
		[DisplayName("VendedorSocio")]
		public string NameScVt { get; set; }
		public string FirstLastNameScVt { get; set; }
		public string AddressScVt { get; set; }
		public int IdReg { get; set; }
		public string RegTipo { get; set; }
		public string RegCate { get; set; }
		public decimal RegCant { get; set; }
		public string RegEstado { get; set; }
		//detalles
		public List<DetTicketVenta_E> Det { get; set; }
		public List<DetTicketVenta2_E>Det2 { get; set; }
		//nc
		// Campos no de la tabla
		public string VendedorSap { get; set; }
		public string LugEntrega { get; set; }
		public OrdenRegistroRutas_E orru { get; set; }
		//operarios sacando
		public string sacador2 { get; set; }
		public string sacador3 { get; set; }
		public string sacador4 { get; set; }
		//operarios chequeador
		public string chequeador1 { get; set; }
		public string chequeador2 { get; set; }
		public string chequeador3 { get; set; }
		//operarios encajador
		public string encajador2 { get; set; }
		public string encajador3 { get; set; }
	}
}
