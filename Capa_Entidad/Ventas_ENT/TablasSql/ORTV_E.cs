using Capa_Entidad.Rutas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class ORTV_E
    {
        [DisplayName("Nro Interno")] public int DocEntry { get; set; }
        [DisplayName("Nro Ticket")] public int DocNum { get; set; }
        public string CardCode { get; set; }
        [DisplayName("Cliente")] public string CardName { get; set; }
        public string Estado { get; set; }
        [DisplayName("Tipo de Venta")] public string TipoVenta { get; set; }
        [DisplayName("Destino")] public string LugarDestino { get; set; }
        public string DirDestino { get; set; }
        public string Referencia { get; set; }
        public string Agencia { get; set; }
        [DisplayName("Envio Agencia")] public string EnvioAgencia { get; set; }
        public string Embalaje { get; set; }
        public int CodSapVendedor { get; set; }
        public string Vendedor { get; set; }
        [DisplayName("Monto Total")] public decimal MontoTotal { get; set; }
        public decimal Flete { get; set; }
        public decimal GastoEnvio { get; set; }
        [DisplayName("EstGasto")]
        public string EstadoGasto { get; set; }
        public decimal PagoEnv { get; set; }
        public string ClaveEnv { get; set; }
        public DateTime? TiempoEntrega { get; set; }
        public decimal DescuentoNC { get; set; }
        public decimal DeudaCliente { get; set; }
        public decimal DeudaEmpresa { get; set; }
        public decimal MontoFinal { get; set; }
        public string FormaPago { get; set; }
        public decimal MontoRecibido { get; set; }
        [DisplayName("EstPago")] public string EstadoPago { get; set; }
        public string FechaPago { get; set; }
        public string HoraPago { get; set; }
        public int CodSapCajero { get; set; }
        public string Cajero { get; set; }
        public string Comentario { get; set; }
        public int Cajas { get; set; }
        public int CajasNuevo { get; set; }
        public int NroMesa { get; set; }
        public int NroMesaNuevo { get; set; }
        public string FechaNC { get; set; }
        public string EstadoFacturacion { get; set; }
        public string FechaFacturacion { get; set; }
        public string HoraFacturacion { get; set; }
        public string OpFacturacion { get; set; }
        public string Observaciones { get; set; }
        public string Observaciones2 { get; set; }
        public string Observaciones3 { get; set; }
        [DisplayName("Fecha Registro")] public string FechaRegistro { get; set; }
        [DisplayName("FechaTk")] public string FechaSapTicket { get; set; }
        [DisplayName("Hora Registro")] public string HoraRegistro { get; set; }
        public string NroVentas { get; set; }
        public string OpRegistro { get; set; }
        public int RolSupervisor { get; set; }
        public string AlmProcedencia { get; set; }
        public string Zona { get; set; }
        public int Notificado { get; set; }
        public string Visible { get; set; }
        public string Presupuesto { get; set; }
        public string Guias { get; set; }

        /********** AnularTicketVenta **********/
        public string OpAnulacion { get; set; }
        public string FechaAnulacion { get; set; }
        public string HoraAnulacion { get; set; }
        /*****************************************/

        public List<RTV1_E> Det1 { get; set; }                  // Detalle persona de recojo
        public List<RTV2_E> Det2 { get; set; }                  // Detalle ordenes de venta
        public List<RTV3_E> Det3 { get; set; }                  // Detalle direcciones
        public List<RTV4_E> Det4 { get; set; }                  // Detalle notas de credito
        public List<RTV5_E> Det5 { get; set; }                  // Detalle de regalos
        public List<RTV6_E> Det6 { get; set; }                  // Detalle pesaje
        public List<RTV7_E> Det7 { get; set; }                  // Detalle tickets vinculados
        public List<RTV11_E> Det11 { get; set; }              // Detalle más operarios sacando ticket
        public List<RTV12_E> Det12 { get; set; }              // Detalle operarios chequeadores ticket
        public List<RTV13_E> Det13 { get; set; }              // Detalle más operarios empacando ticket

        // Campos no de la tabla
        public string VendedorSap { get; set; }
        public string LugEntrega { get; set; }
        public string DetOpe { get; set; }
        public decimal PesoTotal { get; set; }
        public int Impreso { get; set; }
        public string OpImpresion { get; set; }
        

        /**** LISTADO TICKETS RECEPCIÓN ****/
        public string HoraRecibir { get; set; }
        public string FechaAbierto { get; set; }
        public string HoraAbierto { get; set; }
        /*****************************************/

        /**** PICKEANDO TICKETS ****/
        [DisplayName("Picker")] public string OpSacando { get; set; }
        public List<string> OpSacandoApoyo { get; set; }                  // Aquí se guardará la concatenación del OpSacador2, OpSacador3
        public string FechaSacando { get; set; }
        public string HoraSacando { get; set; }

        // Operarios sacando
        public string sacador2 { get; set; }
        public string sacador3 { get; set; }
        /*****************************************/

        /** EMPACANDO TICKETS **/
        [DisplayName("Empacador")] public string OpEmpacado { get; set; }
        public List<string> OpEmpacadoApoyo { get; set; }                  // Aquí se guardará la concatenación del OpEmpacado2 y OpEmpacado3 
        public string FechaEmpacado { get; set; }
        public string HoraEmpacado { get; set; }
        // Operarios verificador
        public string empacador2 { get; set; }
        public string empacador3 { get; set; }

        /** VERIFICANDO TICKETS **/
        [DisplayName("Verificador")] public string OpVerificado { get; set; }
        public List<string> OpVerificadoApoyo { get; set; }                  // Aquí se guardará la concatenación del OpVerificador2 y OpEmpacado3
        public string FechaVerificado { get; set; }
        public string HoraVerificado { get; set; }

        // Operarios verificador
        public string verificador2 { get; set; }
        public string verificador3 { get; set; }
        /***************/

        public string Operario { get; set; }
        public string FechaOperacion { get; set; }
        public string HoraOperacion { get; set; }
        public ORRU_E orru { get; set; }

        // Campos que no son de la tabla BD
        public string Mensaje { get; set; }
        public string NombreVista { get; set; }
        public string ultimoCCEstado { get; set; }
        public bool aptoIniVerificar { get; set; }
        public bool aptoFinVerificar { get; set; }
        public bool hayIniPicking { get; set; }
        public bool hayIniVerificar { get; set; }
        public bool hayIniEmpacar { get; set; }
        public bool hayFinPicking { get; set; }
        public bool hayFinVerificar { get; set; }
        public bool hayFinEmpacar { get; set; }
        //exclusivo para seguimiento de ticket venta
        public bool hayRecibir { get; set; }
        public bool hayEnviar { get; set; }
        public bool hayEntregar { get; set; }
        public string WhsCodeLog { get; set; }
        public string Vinculados { get; set; }
        //guarda cantida de tickets sin enviar en almacen de repartos
        public int TicketsNoEnviados { get; set; }
        public string EstadoContraEntrega { get; set; }     // Estado de la tabla OTC
        public string TipoPago { get; set; }                // TipoPago de la tabla OTC
        public int IdOTC { get; set; }                      // Para saber el Id de Tickets a Cuadrar
        public bool zonaDistinta { get; set; }                      // Para saber el Id de Tickets a Cuadrar
        public ORTV_E()
        {
            hayIniPicking = false;
            hayIniVerificar = false;
            hayIniEmpacar = false;
            hayFinPicking = false;
            hayFinVerificar = false;
            hayFinEmpacar = false;
            hayRecibir = false;
            hayEnviar = false;  
            hayEntregar = false;
            aptoFinVerificar = false;
            aptoIniVerificar=false;
            zonaDistinta = false;
        }
    }

    public class ReporteRegalos
    {
        public int DocNum { get; set; }
        [DisplayName("Fecha SAP Ticket")]
        public string FechaSapTicket { get; set; }
        [DisplayName("Ruc")]
        public string CardCode { get; set; }
        [DisplayName("Razón Social")]
        public string CardName { get; set; }
        public int Linea { get; set; }
        [DisplayName("Nombre del Regalo")]
        public string NombreRegalo { get; set; }
        public int Cantidad { get; set; }
        [DisplayName("Estado de Entrega del Regalo")]
        public string EstadoRegalo { get; set; }
        [DisplayName("Estado de Ticket")]
        public string Estado { get; set; }
        [DisplayName("Lugar Destino")]
        public string LugarDestino { get; set; }
        [DisplayName("Vendedor")]
        public string Vendedor { get; set; }
    }

    public class Tickets
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string CardName { get; set; }
        public string CardCode { get; set; }
        public string Estado { get; set; }
        public string Operario { get; set; }
        // Campos que no son de la tabla BD
        public string Mensaje { get; set; }
        //detalles de regalos
        public List<RTV5_E> Det5 { get; set; }
        public static ORTV_E ORTV_EntregaMasiva(Tickets t)
        {
            ORTV_E result = new ORTV_E();
            result.DocEntry = t.DocEntry;
            result.DocNum = t.DocNum;
            result.CardCode = t.CardCode;
            result.CardName = t.CardName;
            result.Estado = t.Estado;
            result.Operario = t.Operario;
            result.Det5 = t.Det5;

            return result;
        }
    }

}
