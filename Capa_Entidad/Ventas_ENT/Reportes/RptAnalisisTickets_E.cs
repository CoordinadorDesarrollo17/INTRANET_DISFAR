using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.Reportes
{
    public class RptAnalisisTickets_E
    {
        [DisplayName("NRO TICKET")]
        public int DocNum { get; set; }

        [DisplayName("FECHA TICKET")]
        public string FechaSapTicket { get; set; }

        [DisplayName("CLIENTE")]
        public string CardName { get; set; }

        [DisplayName("MONTO TOTAL")]
        public decimal MontoTotal { get; set; }

        [DisplayName("VENDEDOR")]
        public string Vendedor { get; set; }

        [DisplayName("ESTADO PEDIDO")]
        public string Estado { get; set; }                                  // USADO COMO FILTRO

        [DisplayName("FECHA ABIERTO")]
        public string FechaAbierto { get; set; }

        [DisplayName("HORA ABIERTO")]
        public string HoraAbierto { get; set; }

        [DisplayName("FECHA RECIBIDO")]
        public string FechaRecibido { get; set; }

        [DisplayName("HORA RECIBIDO")]
        public string HoraRecibido { get; set; }

        [DisplayName("OP RECIBIDO")]
        public string OpRecibido { get; set; }

        /***** P I C K I N G *****/
        [DisplayName("FECHA INICIO PICKING")]
        public string FechaInicioPicking { get; set; }

        [DisplayName("HORA INICIO PICKING")]
        public string HoraInicioPicking { get; set; }

        [DisplayName("OP INICIO PICKING")]
        public string OpInicioPicking { get; set; }

        [DisplayName("FECHA FIN PICKING")]
        public string FechaFinPicking { get; set; }

        [DisplayName("HORA FIN PICKING")]
        public string HoraFinPicking { get; set; }

        [DisplayName("OP PICKING 1")]
        public string OpFinPicking1 { get; set; }

        [DisplayName("OP PICKING 2")]
        public string OpFinPicking2 { get; set; }

        [DisplayName("OP PICKING 3")]
        public string OpFinPicking3 { get; set; }

        /***** V E R I F I C A R *****/
        [DisplayName("FECHA INICIO VERIFICACIÓN")]
        public string FechaInicioVerificar { get; set; }

        [DisplayName("HORA INICIO VERIFICACIÓN")]
        public string HoraInicioVerificar { get; set; }

        [DisplayName("OP INICIO VERIFICACIÓN")]
        public string OpInicioVerificar { get; set; }

        [DisplayName("FECHA FIN VERIFICACIÓN")]
        public string FechaFinVerificar { get; set; }

        [DisplayName("HORA FIN VERIFICACIÓN")]
        public string HoraFinVerificar { get; set; }

        [DisplayName("OP FIN VERIFICACIÓN 1")]
        public string OpFinVerificador1 { get; set; }

        [DisplayName("OP FIN VERIFICACIÓN 2")]
        public string OpFinVerificador2 { get; set; }

        [DisplayName("OP FIN VERIFICACIÓN 3")]
        public string OpFinVerificador3 { get; set; }

        /***** E M P A C A R *****/
        [DisplayName("FECHA INICIO EMPACADO")]
        public string FechaInicioEmpacar { get; set; }

        [DisplayName("HORA INICIO EMPACADO")]
        public string HoraInicioEmpacar { get; set; }

        [DisplayName("OP INICIO EMPACADO")]
        public string OpInicioEmpacar { get; set; }

        [DisplayName("FECHA FIN EMPACADO")]
        public string FechaFinEmpacar { get; set; }

        [DisplayName("HORA FIN EMPACADO")]
        public string HoraFinEmpacar { get; set; }

        [DisplayName("OP FIN EMPACADO")]
        public string OpFinEmpacar1 { get; set; }

        [DisplayName("OP FIN EMPACADO 2")]
        public string OpFinEmpacar2 { get; set; }

        [DisplayName("OP FIN EMPACADO 3")]
        public string OpFinEmpacar3 { get; set; }
        /***************/

        [DisplayName("LUGAR DESTINO")]
        public string LugarDestino { get; set; }                        // USADO COMO FILTRO

        [DisplayName("NRO DE VENTAS")]
        public string NroVentas { get; set; }

        [DisplayName("TOTAL NRO VENTAS")]
        public int TotalNroVentas { get; set; }

        [DisplayName("CAJAS")]
        public int Cajas { get; set; }

        [DisplayName("ESTADO PAGO")]
        public string EstadoPago { get; set; }
        [DisplayName("FORMA PAGO")]
        public string FormaPago { get; set; }

        [DisplayName("ESTADO FACTURACION")]
        public string EstadoFacturacion { get; set; }

        [DisplayName("FECHA FACTURACION")]
        public string FechaFacturacion { get; set; }

        [DisplayName("HORA FACTURACION")]
        public string HoraFacturacion { get; set; }

        [DisplayName("OP FACTURACION")]
        public string OpFacturacion { get; set; }
        [DisplayName("FECHA ENTREGA")]
        public string FechaEntrega { get; set; }

        [DisplayName("HORA ENTREGA")]
        public string HoraEntrega { get; set; }

        [DisplayName("OP ENTREGADO")]
        public string OpEntrega { get; set; }

        [DisplayName("FECHA ANULACION")]
        public string FechaAnulacion { get; set; }

        [DisplayName("HORA ANULACION")]
        public string HoraAnulacion { get; set; }

        [DisplayName("OP ANULACION")]
        public string OpAnulacion { get; set; }

        [DisplayName("FECHA CANCELACION")]
        public string FechaCancelacion { get; set; }

        [DisplayName("HORA CANCELACION")]
        public string HoraCancelacion { get; set; }

        [DisplayName("OP CANCELACION")]
        public string OpCancelacion { get; set; }

        [DisplayName("FECHA ESTIMADA ENTREGA")]
        public string FechaTiempoEntrega { get; set; }

        [DisplayName("HORA ESTIMADA ENTREGA")]
        public string HoraTiempoEntrega { get; set; }

        [DisplayName("LUGAR ENTREGA")]
        public string LugarDeEntrega { get; set; }

        [DisplayName("NRO MESA")]
        public int NroMesa { get; set; }

        [DisplayName("ALMACEN SALIDA")]
        public string AlmacenSalida { get; set; }               // USADO COMO FILTRO
        public string Comentario { get; set; } 
        public string ComentarioFac { get; set; }
        public string ZonaVenta { get; set; }               
        public string DirDestinoVenta { get; set; }
        public string Calle2 { get; set; }
        public string Distrito2 { get; set; }
        public string Provincia2 { get; set; }
        public string Departamento2 { get; set; }
        public decimal PesoTotalPedido { get; set; }               
        public string FechaPagoTicket { get; set; }               
        public string HoraPagoTicket { get; set; }
        [DisplayName("Tipo Venta")]
        public string TipoVenta { get; set; }
    }

    public class RptFiltrosAnalisisTickets_E : RptAnalisisTickets_E
    {
        public string AlmIni { get; set; }
        public string AlmFin { get; set; }
        public string TipoOperario { get; set; }
        public string Operario { get; set; }
        public string FecIni { get; set; }
        public string FecFin { get; set; }
        public string CardCode { get; set; }
        public decimal MontoFinalIni { get; set; }
        public decimal MontoFinalFin { get; set; }
        public string FormaPagoFil { get; set; }
    }

}