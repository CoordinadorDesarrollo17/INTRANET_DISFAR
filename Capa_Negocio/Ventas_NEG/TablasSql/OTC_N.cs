using Capa_Datos;
using Capa_Datos.Caja_DAO;
using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Caja_ENT;
using Capa_Entidad.Ventas_ENT.Reportes;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
    public class OTC_N
    {
        OTC_D otcD = new OTC_D();
        ORTV_D ticketV = new ORTV_D();

        public int PagarTicket(int DocEntry, ORTV_E ticket, int idRol, int idOTC)
        {
            // Para los usuarios que no son tipo "manager"
            if (!idRol.Equals(1))
            {
                if (ticket.CodSapCajero <= 0) { throw new Exception("NO ELIGIÓ CAJERO O CAJERO SIN CÓDIGO SAP"); }
                if (ticket.Cajero == null || ticket.Cajero.Equals("")) { throw new Exception("NO ELIGIÓ CAJERO"); }
            }

            var datosTicket = ticketV.obtenerTicket(DocEntry);

            if (ticket.MontoFinal != ticket.MontoRecibido) { throw new Exception("NO SE PAGO YA QUE LOS MONTOS NO COINCIDEN"); }
            if (datosTicket.Estado.Equals("CANCELADO")) { throw new Exception("NO PUEDE PAGAR UN TICKET CANCELADO"); }
            if (datosTicket.EstadoPago.Equals("PAGADO")) { throw new Exception("EL TICKET YA SE ENCUENTRA PAGADO"); }

            int docNumTicket = ticketV.pagarTicket(DocEntry, ticket);

            // Para tickets a cuadrar (PAGO EFECTIVO)
            if (docNumTicket > 0)
            {
                OTC_D otcD = new OTC_D();
                var resultOTC = otcD.ObtenerDatosTicketACuadrar(DocEntry, idOTC);

                if (resultOTC != null)
                {
                    OTC_E tc = new OTC_E() { IdOTC = resultOTC.IdOTC, Estado = "CUADRADO", PersonaEntrega = ticket.Cajero };
                    otcD.CambiarEstadoTicketACuadrar(tc, "CUADRAR");
                }
            }

            return docNumTicket;
        }

        public int AnularPagoTicket(int DocEntry)
        {
            ORTV_E t = ticketV.obtenerTicket(DocEntry);
            if (t.EstadoPago != null && !t.EstadoPago.Equals("PAGADO")) { throw new Exception("EL TICKET SE ENCUENTRA PENDIENTE"); }
            if (t.Estado.Equals("ENTREGADO")) { throw new Exception("NO PUEDES ANULARPAGO DE TICKET ENTREGADO"); }
            if (t.Estado.Equals("ENVIADO")) { throw new Exception("NO PUEDES ANULARPAGO DE TICKET ENVIADO"); }
            if (t.EstadoGasto == "CONFIRMADO") { throw new Exception("No puedes Anular Pago de EstadoGasto confirmado"); }
            return ticketV.anularPagoTicket(DocEntry);
        }

        public void ConfGastEnvio(ORTV_E o)
        {
            ORTV_E ortvE = ticketV.obtenerTicket(o.DocEntry);
            if (ortvE.EstadoPago != "PAGADO") { throw new Exception("Solo puedes confirmar ticket Pagado"); }
            if (o.PagoEnv <= 0) { throw new Exception("Pago envio no puede ser negativo o cero"); }
            if (ortvE.EstadoGasto == "CONFIRMADO") { throw new Exception("Error: El ticket ya esta confirmado"); }
            o.CardCode = ortvE.CardCode; o.GastoEnvio = ortvE.GastoEnvio; o.DocNum = ortvE.DocNum;
            if (o.DetOpe == null) { o.DetOpe = ""; }
            ticketV.confGastEnvio(o);
        }

        public OTC_E ObtenerDatosTicketACuadrar(int docEntryTicket, int idOTC)
        {
            return otcD.ObtenerDatosTicketACuadrar(docEntryTicket, idOTC);
        }

        protected string ValidarAdjunto(List<HttpPostedFileBase> adjuntos)
        {
            if (adjuntos == null || adjuntos.Count == 0)
            {
                return "Es obligatorio adjuntar imagen";
            }

            string[] formatosValidos = { "image/jpg", "image/png", "image/jpeg" };
            const int maxSizeBytes = 15485760; // 15 MB

            foreach (var img in adjuntos)
            {
                // Verificar que el archivo no esté vacío
                if (img.ContentLength <= 0)
                {
                    return "Error: Archivo vacío o no válido.";
                }

                // Verificar que el tipo de contenido sea válido
                if (!formatosValidos.Contains(img.ContentType))
                {
                    return "Error: Tipo de archivo no permitido. Solo se permiten archivos de imagen en formato .jpg, .png, o .jpeg.";
                }

                // Verificar el tamaño del archivo
                if (img.ContentLength > maxSizeBytes)
                {
                    return "Error: El tamaño del archivo excede el límite permitido de 15 MB.";
                }
            }

            return string.Empty;
        }

        public string ValidarTipoPago(OTC_E tc, decimal montoFinalTicket)
        {
            string mensajeMontoIncorrecto = "Monto ingresado no coincide con el monto a cobrar";

            switch (tc.TipoPago)
            {
                case "PCE":
                    if (montoFinalTicket.Equals(tc.MontoRecibidoEfectivo) && tc.MontoRecibidoDeposito.Equals(0))
                    {
                        return string.Empty;
                    }
                    break;

                case "PCD":
                    if (montoFinalTicket.Equals(tc.MontoRecibidoDeposito) && tc.MontoRecibidoEfectivo.Equals(0))
                    {
                        string mensaje = ValidarAdjunto(tc.ImgAdjunta);

                        // Validación correcta cuando el método ValidarAdjunto() no retorna ningún mensaje de error
                        return string.IsNullOrEmpty(mensaje) ? string.Empty : mensaje;
                    }
                    break;

                case "PMI":
                    decimal totalRecibido = (decimal)tc.MontoRecibidoEfectivo + (decimal)tc.MontoRecibidoDeposito;

                    if ((decimal)tc.MontoRecibidoDeposito == montoFinalTicket)
                    {
                        return "Para el monto ingresado cambiar el tipo de pago a PAGO DEPÓSITO";
                    }

                    if ((decimal)tc.MontoRecibidoEfectivo == montoFinalTicket)
                    {
                        return "Para el monto ingresado cambiar el tipo de pago a PAGO EFECTIVO";
                    }

                    if ((decimal)tc.MontoRecibidoDeposito > montoFinalTicket || (decimal)tc.MontoRecibidoEfectivo > montoFinalTicket)
                    {
                        return mensajeMontoIncorrecto;
                    }

                    if (totalRecibido > 0 && totalRecibido <= montoFinalTicket)
                    {
                        if (tc.MontoRecibidoEfectivo >= 0 && tc.MontoRecibidoEfectivo <= montoFinalTicket &&
                            tc.MontoRecibidoDeposito >= 0 && tc.MontoRecibidoDeposito <= montoFinalTicket)
                        {
                            // Cuando recibimos algún monto en depósito, es obligatorio adjuntar imagen
                            if (tc.MontoRecibidoDeposito > 0)
                            {
                                string mensaje = ValidarAdjunto(tc.ImgAdjunta);

                                // Validación errónea cuando el método ValidarAdjunto() retorna ningún mensaje de error
                                return string.IsNullOrEmpty(mensaje) ? string.Empty : mensaje;
                            }
                            return string.Empty;
                        }
                    }
                    break;

                case "SP":
                    if (tc.MontoRecibidoEfectivo.Equals(0) && tc.MontoRecibidoDeposito.Equals(0))
                    {
                        return string.Empty;
                    }
                    break;

                default:
                    return "Tipo de pago no válido";
            }

            return mensajeMontoIncorrecto;
        }

        // Autorización excepcional: Para gestionar solicitudes fuera de horario laboral de caja
        public bool AutorizarEntregaExcepcional()
        {
            const int lunes = 1;
            const int viernes = 5;
            const int sabado = 6;
            bool autorizacionExcepcional = false;

            // Obtener el día de la semana y la hora actual
            int dia = (int)DateTime.Now.DayOfWeek;
            int horas = (int)DateTime.Now.Hour;
            int minutos = (int)DateTime.Now.Minute;

            // Lunes a Viernes a partir de las 06:30 PM o Sábados a partir de las 01:00 PM
            if ((dia >= lunes && dia <= viernes && horas >= 18 && minutos > 30) || (dia == sabado && horas >= 13 && minutos > 0))
            {
                autorizacionExcepcional = true;
            }

            return autorizacionExcepcional;
        }

        public string ValidarRegistroTC(OTC_E tc)
        {
            string msj = string.Empty;

            // Obtenemos los datos ingresados por el usuario REPA
            var datosTC = otcD.ObtenerDatosTicketACuadrar((int)tc.DocEntryTicket, (int)tc.IdOTC);

            // Solo cuando no exista registro o cuando la solicitud ha sido rechazada (usuario REPA puede volver a enviar una nueva solicitud)
            if (datosTC == null || (datosTC.Estado != null && datosTC.Estado.Equals("RECHAZADO")))
            {
                string[] tiposVentaValidos = { "ContraEntrega" };
                var result = ticketV.obtenerTicket((int)tc.DocEntryTicket);

                if (result != null && tc.DocEntryTicket.Equals(result.DocEntry) && tiposVentaValidos.Contains(result.TipoVenta))
                {
                    tc.TipoVenta = result.TipoVenta;
                    tc.DocNumTicket = result.DocNum;

                    if (AutorizarEntregaExcepcional())
                    {
                        if (string.IsNullOrEmpty(tc.AutorizacionExcepcional)) { msj = "Es obligatorio justificar la autorización de la entrega"; }
                    }

                    if(string.IsNullOrEmpty(msj)) { msj = ValidarTipoPago(tc, result.MontoFinal); }

                    // se valida en los casos que el textarea no sea visible en el FormData (JS)
                    if (!string.IsNullOrEmpty(tc.AutorizacionExcepcional) && tc.AutorizacionExcepcional.Equals("undefined")) { tc.AutorizacionExcepcional = string.Empty; }

                    // Registrar solo si no existe mensaje de error en las validaciones anteriores
                    if (string.IsNullOrEmpty(msj)) { otcD.RegistrarTicketACuadrar(tc); msj = "Se solicitó la VALIDACIÓN"; }
                }
                else { msj = "No puede solicitar validación para este tipo de venta"; }
            }
            else { msj = $"Tiene una solicitud en estado {datosTC.Estado} para el N° Ticket {datosTC.DocNumTicket}"; }

            return msj;
        }

        public List<OTC_E> ListarTicketsACuadrar(OTC_E tc)
        {
            return otcD.ListarTicketsACuadrar(tc);
        }

        public List<ORTV_E> ListarTicketsVenta(ORTV_E filtro1)
        {
            return otcD.ListarTicketsVenta(filtro1);
        }

        public string CambiarEstadoTicketACuadrar(OTC_E tc, string operacion, string area = "")
        {
            if (tc == null || tc.IdOTC <= 0)
            {
                return "No se realizó ninguna acción";
            }

            string[] estadosValidos = { "PENDIENTE", "VALIDADO", "AUTORIZADO", "RECHAZADO", "CUADRADO" };
            if (string.IsNullOrEmpty(tc.Estado) || !estadosValidos.Contains(tc.Estado))
            {
                return "Estado no válido, verificar ticket a cuadrar";
            }

            var datosTC = otcD.ObtenerDatosTicketACuadrar((int)tc.DocEntryTicket, (int)tc.IdOTC);

            if (datosTC == null || string.IsNullOrEmpty(datosTC.Estado))
            {
                return "Verificar el estado de la solicitud. Reportarlo a SISTEMAS";
            }

            string[] operacionesValidas = { "VALIDAR", "AUTORIZAR", "RECHAZAR" };
            if (operacionesValidas.Contains(operacion) &&  datosTC.Estado.Equals("RECHAZADO"))
            {
                return "La solicitud ya se encuentra RECHAZADA";
            }

            if (operacion.Equals("VALIDAR") && datosTC.Estado.Equals("VALIDADO"))
            {
                return "La solicitud ya se encuentra VALIDADA";
            }

            if (operacion.Equals("AUTORIZAR") && datosTC.Estado.Equals("AUTORIZADO"))
            {
                return "La solicitud ya se encuentra AUTORIZADA";
            }

            if (operacion.Equals("VALIDAR") && (datosTC.TipoPago.Equals("PCD") || datosTC.TipoPago.Equals("PMI")) && tc.ComentarioCaja.Trim().Length == 0)
            {
                return "Es obligatorio agregar el motivo de VALIDACIÓN";
            }

            var datosTicket = new ORTV_D().obtenerTicket((int)tc.DocEntryTicket);
            if (datosTicket == null)
            {
                return "Verificar datos del ticket. Reportarlo a SISTEMAS";
            }

            if (operacion.Equals("AUTORIZAR") && datosTC.TipoPago.Equals("PMI") && (datosTC.MontoRecibidoDeposito + datosTC.MontoRecibidoEfectivo < datosTicket.MontoFinal) && datosTC.MontoRecibidoDeposito > 0 && !datosTC.Estado.Equals("VALIDADO"))
            {
                return "Para AUTORIZAR esta solicitud, primero debe ser VALIDADA por CAJA";
            }

            if (operacion.Equals("AUTORIZAR") && datosTC.TipoPago.Equals("PMI") && (datosTC.MontoRecibidoDeposito + datosTC.MontoRecibidoEfectivo < datosTicket.MontoFinal) && tc.ComentarioVentas.Trim().Length == 0)
            {
                return "Es obligatorio agregar el motivo de AUTORIZACIÓN";
            }

            if (operacion.Equals("AUTORIZAR") && datosTC.TipoPago.Equals("SP") && tc.ComentarioVentas.Trim().Length == 0 && tc.FechaCompromisoPago == null)
            {
                return "Es obligatorio seleccionar una fecha de compromiso de pago ";
            }

            return otcD.CambiarEstadoTicketACuadrar(tc, operacion, area);
        }

        public string AgregarPagosParciales(List<OPP_E> pp, int docEntryTicket)
        {
            if (pp == null || pp.Count == 0)
            {
                return "Debe ingresar un monto para proceder con el pago parcial";
            }

            string mensaje = otcD.AgregarPagosParciales(pp);

            if (mensaje != "Pagos registrados correctamente")
            {
                return mensaje;
            }

            OPP_D ppD = new OPP_D();
            var totalPagos = ppD.ObtenerTotalPagos(pp[0].IdOTC);

            var datosTicket = new ORTV_D().obtenerTicket(docEntryTicket);
            var datosCaja = otcD.ObtenerDatosTicketACuadrar(docEntryTicket, pp[0].IdOTC);

            // Si el cliente cumplió con el compromiso de pago restante del ticket
            if (totalPagos == datosTicket.MontoFinal && datosCaja != null)
            {
                var pagosParciales = ppD.ObtenerDatosPagosParciales(pp[0].IdOTC);

                // Si la fecha del último pago parcial es mayor a la fecha del compromiso de pago que generó el cliente
                // Entonces su comportamiento es negativo (MOROSO), caso contrario se mantiene positivo (PUNTUAL)
                string comportamiento = Convert.ToDateTime(pagosParciales[0].FechaRegistro) > Convert.ToDateTime(datosCaja.FechaCompromisoPago) ? "MOROSO" : "PUNTUAL";
                otcD.ActualizarComportamientoPago(pp[0].IdOTC, comportamiento);
            }


            return mensaje;
        }

        public string ConsultarNuevasSolicitudesTC()
        {
            var result = otcD.ConsultarNuevasSolicitudesTC();

            return (result > 0) ? $"Cantidad de solicitudes a VALIDAR: {result}" : string.Empty;
        }

        public List<OTC_E> ObtenerSolicitudesAutorizar()
        {
            return otcD.ObtenerSolicitudesAutorizar();
        }

        public List<RptTicketACuadrar_E> ExportarExcelTicketsACuadrar(ORTV_E tc)
        {
            return otcD.ExportarExcelTicketsACuadrar(tc);
        }

        public List<string> ObtenerComprobantePagoEfectivo(int docNumTicket, string fechaOperacion)
        {
            List<string> arrImg = new List<String>();

            // Se utiliza Path.Combine() para concatenar de manera segura el directorio base y la fecha de operación.
            string rutaDirectorio = Path.Combine(new Utilitarios().directorioFileServer + @"\Repartos\ComprobantesPagoEfectivo", fechaOperacion);
            string patronBusqueda = $"{docNumTicket}_*";

            if (Directory.Exists(rutaDirectorio))
            {
                string[] archivosEncontrados = Directory.GetFiles(rutaDirectorio, patronBusqueda);

                foreach (var archivo in archivosEncontrados)
                {
                    // Se utiliza Path.GetExtension() para obtener la extensión del archivo de manera segura.
                    string extension = Path.GetExtension(archivo).ToLower();

                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    {
                        byte[] img = File.ReadAllBytes(archivo);
                        string base64 = Convert.ToBase64String(img);
                        arrImg.Add($"data:image/{extension};base64,{base64}");
                    }
                }

            }

            return arrImg;
        }
    }
}