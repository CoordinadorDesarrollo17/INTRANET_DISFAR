using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.Ventas_DAO;
using Capa_Entidad.Ventas_ENT;
using Capa_Entidad;
using System.Data;
using Capa_Entidad.SocioNegocios_ENT.Tablas;
using Capa_Entidad.Rutas_ENT.ReportesSql;
using Capa_Datos;

namespace Capa_Negocio.Ventas_NEG
{
    public class TicketVenta_N
    {
        TicketVenta_D ticketV = new TicketVenta_D(); Usuario_D uD = new Usuario_D();
        public List<OCRD_E> listarClientes(string Fecha)
        {
            return ticketV.listarClientes(Fecha);
        }
        // sql
        public List<TicketVenta_E> listarTicketsVenta(Usuario_E user, TicketVenta_E t)
        {
            return ticketV.listarTicketsVenta(user, t);
        }
        public int  enviarTicketAgencia(int DocEntry, TicketVenta_E t)
        {
            return ticketV.enviarTicketAgencia(DocEntry,t);
        }
        
        public List<Rpt_TicketVenta_E> listarTicketsAgencia()
        {
            return ticketV.listarTicketsAgencia();
        }
        public List<TicketVenta_E> listarTicketsSeparados(string Id)
        {
            return ticketV.listarTicketsSeparados(Id);
        }
        public TicketVenta_E separarTicket(Usuario_E u)
        {
            return ticketV.separarTicket(u);
        }
        public int registrarTicket(TicketVenta_E ticket)
        {
            validarDatosTicket(ticket);
            if (!ticket.EstadoPedido.Equals("SEPARADO")) { throw new Exception("EL TICKET DEBE TENER ESTADO SEPARADO"); }
            return ticketV.registrarTicket(ticket);
        }
        public void validarDatosTicket(TicketVenta_E ticket)
        {
            TablasSql.OREG_N oregN = new TablasSql.OREG_N();
            TablasSql.OCLR_N oclrN = new TablasSql.OCLR_N();
            bool esVacio(string dato)
            {
                if (dato == null || dato.ToString().Equals("")) { return true; } else { return false; }
            }
            // validacion de detalles
            if (ticket.Det == null || ticket.Det.Count() == 0) { throw new Exception("NO PUEDE REGISTRAR CON DETALLES VACIOS"); }
            else
            {
                foreach (DetTicketVenta_E d in ticket.Det)
                {
                    if (d.Verificar == "on" && esVacio(d.TipoComprobante)) { throw new Exception("DEBE LLENAR TIPODECOMPROBANTE EN DETALLES LINEA: " + d.Linea); }
                }
            }
            //validacion cabecera
            if (esVacio(ticket.FechaTicket)) { throw new Exception("NO ELIGIÓ FECHA TICKET"); }
            if (ticket.DocNum <= 0) { throw new Exception("NO SE SELECCIONO UN NRO. DE TICKET"); }
            if (esVacio(ticket.CardCode) || esVacio(ticket.CardName)) { throw new Exception("NO SE SELECCIONO CLIENTE"); }
            if (esVacio(ticket.TipoVenta)) { throw new Exception("NO SE SELECCIONO TIPO DE VENTA"); }
            if (esVacio(ticket.LugarDestino)) { throw new Exception("NO SE SELECCIONO LUGAR DESTINO"); }
            else
            {
                if (ticket.LugarDestino.Equals("Agencia"))
                {
                    if (esVacio(ticket.Agencia)) { throw new Exception("DEBE LLENAR AGENCIA "); }
                    if (esVacio(ticket.DirDestino1)) { throw new Exception("DEBE LLENAR DIRECIONDESTINO1 "); }
                    if (esVacio(ticket.NombrePer1)) { throw new Exception("DEBE LLENAR NOMBRE1 "); }
                    if (esVacio(ticket.TipoDocPer1)) { throw new Exception("DEBE LLENAR TIPO DE DOCUMENTO PER1 "); }
                    if (esVacio(ticket.DocPer1)) { throw new Exception("DEBE LLENAR EL NRODOCUMENTO PER1"); }
                    if (esVacio(ticket.TelfPer1)) { throw new Exception("DEBE LLENAR TELEFONO1 "); }
                    if (ticket.Embalaje != "CP") { throw new Exception("EMBALAJE DEBE SER CAJA PROVINCIA"); }
                    string lugEn = "";
                    foreach (DetTicketVenta_E d in ticket.Det.Where(x => x.Verificar == "on"))
                    {
                        if (lugEn != "")
                        {
                            if (!(lugEn.Equals("ALMACÉN N°3") || lugEn.Equals("ALMACÉN N°5") || lugEn.Equals("ALMACÉN N°7"))) { throw new Exception("LUGAR DE ENTREGA DEBE SER ALM03 O ALM05 O ALM07"); }
                            if (d.LugarDeEntrega.Equals(lugEn)) { }
                            else { throw new Exception("NO COINCIDEN LOS LUGARES DE ENTREGA EN DETALLE"); }
                        }
                        else
                        {
                            lugEn = d.LugarDeEntrega;
                            if (!(lugEn.Equals("ALMACÉN N°3") || lugEn.Equals("ALMACÉN N°5") || lugEn.Equals("ALMACÉN N°7"))) { throw new Exception("LUGAR DE ENTREGA DEBE SER ALM03 O ALM05 O ALM07"); }
                        }
                    }
                }
                else if (ticket.LugarDestino.Equals("Arriola"))
                {
                    string lugEn = "";
                    foreach (DetTicketVenta_E d in ticket.Det.Where(x => x.Verificar == "on"))
                    {
                        if (lugEn != "")
                        {
                            if (!(lugEn.Equals("ALMACÉN N°6"))) { throw new Exception("LUGAR DE ENTREGA DEBE SER ALM06"); }
                            if (d.LugarDeEntrega.Equals(lugEn)) { }
                            else { throw new Exception("NO COINCIDEN LOS LUGARES DE ENTREGA EN DETALLE"); }
                        }
                        else
                        {
                            lugEn = d.LugarDeEntrega;
                            if (!(lugEn.Equals("ALMACÉN N°6"))) { throw new Exception("LUGAR DE ENTREGA DEBE SER ALM06"); }
                        }
                    }
                }
                else if (ticket.LugarDestino.Equals("Domicilio"))
                {
                    if (esVacio(ticket.DirDestino1)) { throw new Exception("DEBE LLENAR DIRECIONDESTINO1 "); }
                    if (esVacio(ticket.NombrePer1)) { throw new Exception("DEBE LLENAR NOMBRE1 "); }
                    if (esVacio(ticket.TipoDocPer1)) { throw new Exception("DEBE LLENAR TIPO DE DOCUMENTO PER1 "); }
                    if (esVacio(ticket.DocPer1)) { throw new Exception("DEBE LLENAR EL NRODOCUMENTO PER1"); }
                    if (esVacio(ticket.TelfPer1)) { throw new Exception("DEBE LLENAR TELEFONO1 "); }
                    string lugEn = "";
                    foreach (DetTicketVenta_E d in ticket.Det.Where(x => x.Verificar == "on"))
                    {
                        if (lugEn != "")
                        {
                            if (!(lugEn.Equals("ALMACÉN N°3") || lugEn.Equals("ALMACÉN N°5") || lugEn.Equals("ALMACÉN N°7"))) { throw new Exception("LUGAR DE ENTREGA DEBE SER ALM03 O ALM05 O ALM7"); }
                            if (d.LugarDeEntrega.Equals(lugEn)) { }
                            else { throw new Exception("NO COINCIDEN LOS LUGARES DE ENTREGA EN DETALLE"); }
                        }
                        else
                        {
                            lugEn = d.LugarDeEntrega;
                            if (!(lugEn.Equals("ALMACÉN N°3") || lugEn.Equals("ALMACÉN N°5") || lugEn.Equals("ALMACÉN N°7"))) { throw new Exception("LUGAR DE ENTREGA DEBE SER ALM03 O ALM05 O ALM07"); }
                        }
                    }
                }
                else if (ticket.LugarDestino.Equals("Centro"))
                {
                    string lugEn = "";
                    foreach (DetTicketVenta_E d in ticket.Det.Where(x => x.Verificar == "on"))
                    {
                        if (lugEn != "")
                        {
                            if (!(lugEn.Equals("ALMACÉN N°1"))) { throw new Exception("LUGAR DE ENTREGA DEBE SER ALM01"); }
                            if (d.LugarDeEntrega.Equals(lugEn)) { }
                            else { throw new Exception("NO COINCIDEN LOS LUGARES DE ENTREGA EN DETALLE"); }
                        }
                        else
                        {
                            lugEn = d.LugarDeEntrega;
                            if (!(lugEn.Equals("ALMACÉN N°1"))) { throw new Exception("LUGAR DE ENTREGA DEBE SER ALM01"); }
                        }
                    }
                }
            }
            if (!esVacio(ticket.Protocolo) && (ticket.Protocolo.Equals("Virtual")) && (esVacio(ticket.Correo))) { throw new Exception("NO ESCRIBIO CORREO"); }
            if (esVacio(ticket.Embalaje)) { throw new Exception("NO SE SELECCIONO TIPO EMBALAJE"); }
            if (ticket.MontoTotal <= 0) { throw new Exception("NO PUEDE REGISTAR MONTO TOTAL EN 0 o negativo"); }
            if (ticket.MontoFinal <= 0) { throw new Exception("NO SE PUEDE REGISTRAR MONTOFINAL EN 0 o negativo"); }
            if (esVacio(ticket.TiempoEntrega)) { throw new Exception("DEBE LLENAR TIEMPOENTREGA"); }
            // validacion de tipo de contenido
            if (!esVacio(ticket.TipoDocPer1) && (ticket.TipoDocPer1.Equals("DNI")) && (ticket.DocPer1.Length != 8)) { throw new Exception("EL DNIPER1 DEBE TENER 8 DIGITOS"); }
            if (!esVacio(ticket.TipoDocPer1) && (ticket.TipoDocPer1.Equals("RUC")) && (ticket.DocPer1.Length != 11)) { throw new Exception("EL RUCPER1 DEBE TENER 11 DIGITOS"); }
            if (!esVacio(ticket.TipoDocPer1) && (ticket.TipoDocPer1.Equals("CE")) && (ticket.DocPer1.Length != 9)) { throw new Exception("EL CARNE EXTRANJERIAPER1 DEBE TENER 9 DIGITOS"); }
            if (!esVacio(ticket.TelfPer1) && (ticket.TelfPer1.Length > 9)) { throw new Exception("EL TELEFONO1 DEBE TENER MAXIMO 9 DIGITOS"); }
            if (!esVacio(ticket.TipoDocPer2) && (ticket.TipoDocPer2.Equals("DNI")) && (ticket.DocPer2.Length != 8)) { throw new Exception("EL DNIPER2 DEBE TENER 8 DIGITOS"); }
            if (!esVacio(ticket.TipoDocPer2) && (ticket.TipoDocPer2.Equals("RUC")) && (ticket.DocPer2.Length != 11)) { throw new Exception("EL RUCPER2 DEBE TENER 11 DIGITOS"); }
            if (!esVacio(ticket.TipoDocPer2) && (ticket.TipoDocPer2.Equals("CE")) && (ticket.DocPer2.Length != 9)) { throw new Exception("EL CARNE EXTRANJERIAPER2 DEBE TENER 9 DIGITOS"); }
            if (!esVacio(ticket.TelfPer2) && (ticket.TelfPer2.Length > 9)) { throw new Exception("EL TELEFONO2 DEBE TENER MAXIMO 9 DIGITOS"); }
            if (ticket.Flete < 0) { throw new Exception("NO PUEDE REGISTAR FLETE NEGATIVO"); }
            if (ticket.DeudaCliente < 0) { throw new Exception("NO PUEDE REGISTAR DeudaCliente NEGATIVO"); }
            if (ticket.GastoEnvio < 0) { throw new Exception("NO PUEDE REGISTAR GastoEnvio NEGATIVO"); }
            if (ticket.DeudaEmpresa < 0) { throw new Exception("NO PUEDE REGISTAR DeudaEmpresa NEGATIVO"); }
            if (ticket.DescuentoNC < 0) { throw new Exception("NO PUEDE REGISTAR DescuentoNC NEGATIVO"); }
            // validacion de calculos
            if (ticket.MontoTotal != CalcularMontos(ticket).MontoTotal) { throw new Exception("EL CALCULO DEL MONTOTOTAL TUVO UN ERROR"); }
            if (ticket.MontoFinal != CalcularMontos(ticket).MontoFinal) { throw new Exception("EL CALCULO DEL MONTOFINAL TUVO UN ERROR"); }
            if (ticket.IdReg > 0) {
                if (ticket.RegCant <= 0) { throw new Exception("Debe llenar cantidades para el regalo"); }
                else if(!oclrN.comprobarDispCliReg(new Capa_Entidad.Ventas_ENT.TablasSql.CLR1_E { IdReg=ticket.IdReg,CardCode=ticket.CardCode,Cantidad=ticket.RegCant }))
                { throw new Exception("El cliente no tiene saldo regalo disponible"); }
                if (oregN.buscarRegalo(ticket.IdReg).Estado == "Inactivo") { throw new Exception("No puede dar regalos inactivos"); }
                if (oregN.buscarRegalo(ticket.IdReg).StockDisp < ticket.RegCant) { throw new Exception("No hay Stock disponible del regalo"); }
            }
            else if (ticket.RegCant>0) { throw new Exception("Debe elegir un regalo"); }
        }
        public TicketVenta_E obtenerTicket(int DocEntry)
        {
            return ticketV.obtenerTicket(DocEntry);
        }
        public int editarTicket(int DocEntry, TicketVenta_E ticket)
        {
            //condiciones previas 
            TicketVenta_E t = ticketV.obtenerTicket(DocEntry);
            validarDatosTicket(ticket);
            if (t.EstadoPago != null && t.EstadoPago.Equals("PAGADO")) { throw new Exception("NO PUEDE EDITAR UN TICKET PAGADO"); }
            if (!t.EstadoPedido.Equals("ABIERTO")) { throw new Exception("NO PUEDE EDITAR UN TICKET " + t.EstadoPedido); }
            return ticketV.editarTicket(DocEntry, ticket);
        }
        public int pagarTicket(int DocEntry, TicketVenta_E ticket)
        {
            if (ticket.CajeroCod <= 0) { throw new Exception("NO ELIGIÓ CAJERO  O CAJERO SIN CODIGO"); }
            if (ticket.CajeroDesc == null || ticket.CajeroDesc.Equals("")) { throw new Exception("NO ELIGIÓ CAJERO DESC"); }
            if (ticket.MontoFinal != ticket.MontoRecibido) { throw new Exception("NO SE PAGO YA QUE LOS MONTOS NO COINCIDEN"); }
            if (ticketV.obtenerTicket(DocEntry).EstadoPedido.Equals("ANULADO")) { throw new Exception("NO PUEDE PAGAR UN TICKET ANULADO"); }
            if (ticketV.obtenerTicket(DocEntry).EstadoPago.Equals("PAGADO")) { throw new Exception("EL TICKET YA SE ENCUENTRA PAGADO"); }
            return ticketV.pagarTicket(DocEntry, ticket);
        }
        public int anularPagoTicket(int DocEntry)
        {
            TicketVenta_E t = ticketV.obtenerTicket(DocEntry);
            if (t.EstadoPago != null && !t.EstadoPago.Equals("PAGADO")) { throw new Exception("EL TICKET SE ENCUENTRA PENDIENTE"); }
            if (t.EstadoPedido.Equals("ENTREGADO")) { throw new Exception("NO PUEDES ANULARPAGO DE TICKET ENTREGADO"); }
            if (t.EstadoPedido.Equals("ENVIADO")) { throw new Exception("NO PUEDES ANULARPAGO DE TICKET ENVIADO"); }
            return ticketV.anularPagoTicket(DocEntry);
        }
        public int anularTicket(int DocEntry, TicketVenta_E ticket)
        {
            TicketVenta_E t = ticketV.obtenerTicket(DocEntry);
            if (t.EstadoPago != null && t.EstadoPago.Equals("PAGADO")) { throw new Exception("NO PUEDE ANULAR UN TICKET PAGADO"); }
            if (t.EstadoPedido.Equals("ANULADO")) { throw new Exception("EL TICKET YA SE ENCUENTRA ANULADO"); }
            if (t.EstadoPedido.Equals("CANCELADO")) { throw new Exception("EL TICKET YA SE ENCUENTRA CANCELADO, NO ES POSIBLE ANULAR"); }
            if (!(t.EstadoPedido.Equals("SEPARADO") || t.EstadoPedido.Equals("ABIERTO"))) { throw new Exception("EL TICKET DEBE ESTAR SEPARADO O ABIERTO"); }
            return ticketV.anularTicket(DocEntry, ticket);
        }
        public int cancelarTicket(int DocEntry, TicketVenta_E ticket)
        {
            TicketVenta_E t = ticketV.obtenerTicket(DocEntry);
            if (t.EstadoPago != null && t.EstadoPago.Equals("PAGADO")) { throw new Exception("NO PUEDE CANCELAR UN TICKET PAGADO N°"+t.DocNum); }
            if (t.EstadoPedido.Equals("CANCELADO")) { throw new Exception("EL TICKET YA SE ENCUENTRA CANCELADO N°" + t.DocNum); }
            if (t.EstadoPedido.Equals("ANULADO")) { throw new Exception("NO PUEDE CANCELAR UN TICKET ANULADO N°" + t.DocNum); }
            return ticketV.cancelarTicket(DocEntry, ticket);
        }
        public int editarSeguimientoTicket(string Estado, int DocEntry, TicketVenta_E t)
        {
            if (Estado.Equals("RECIBIDO"))
            {
                if (t.EstadoPedido.Equals("RECIBIDO")) { throw new Exception("El ticket ya se encuentra  Recibido"); }
                if (t.EstadoPedido.Equals("ANULADO")) { throw new Exception("No puedes recibir Ticket anulado"); }
                if (!t.EstadoPedido.Equals("ABIERTO")) { throw new Exception("Solo puedes recibir Ticket en ABIERTO ,debes revertir o continuar el proceso"); }
            }
            else if (Estado.Equals("ANULARRECIBIDO"))
            {
                if (!t.EstadoPedido.Equals("RECIBIDO")) { throw new Exception("Solo puedes AnulaRecibido a un ticket Recibido"); }
            }
            else if (Estado.Equals("SACANDO"))
            {
                if (t.EstadoPedido.Equals("SACANDO")) { throw new Exception("El ticket ya se encuentra  SACANDO"); }
                if (!t.EstadoPedido.Equals("RECIBIDO")) { throw new Exception("Solo puedes poner SACANDO a Ticket en RECIBIDO ,debes revertir o continuar el proceso"); }
                if (t.sacador2 != null)
                {
                    if (uD.buscarUsuarioxNombre(t.sacador2) == 0) { throw new Exception("Sacador 2 ERROR"); }
                }
                if (t.sacador3 != null)
                {
                    if (uD.buscarUsuarioxNombre(t.sacador3) == 0) { throw new Exception("Sacador 3 ERROR"); }
                }
                if (t.sacador4 != null)
                {
                    if (uD.buscarUsuarioxNombre(t.sacador4) == 0) { throw new Exception("Sacador 4 ERROR"); }
                }
            }
            else if (Estado.Equals("ANULARSACANDO"))
            {
                if (!t.EstadoPedido.Equals("SACANDO")) { throw new Exception("Solo puedes ANULARSACANDO a un ticket SACANDO"); }
            }
            else if (Estado.Equals("EMPACADO"))
            {
                if (t.EstadoPedido.Equals("EMPACADO")) { throw new Exception("El ticket ya se encuentra  EMPACADO"); }
                if (!t.EstadoPedido.Equals("SACANDO")) { throw new Exception("Solo puedes poner EMPACADO a Ticket en SACANDO ,debes revertir o continuar el proceso"); }
                if (t.Cajas <= 0) { throw new Exception("Debe ingresar el nro de cajas para empacar"); }
                if (t.NroMesa <= 0) { throw new Exception("Debe ingresar el nro de Mesa para empacar"); }
                /*if (t.OpChequeador == null || t.OpChequeador == "") { throw new Exception("Debe llenar el Chequeador"); }*/
                if (t.chequeador1 == null || t.chequeador1 == "" && t.chequeador2 == null || t.chequeador2 == "" && t.chequeador3 == null || t.chequeador3 == "") 
                { throw new Exception("Debe llenar el Chequeador"); }
                if (t.chequeador1 != null)
                {
                    if (uD.buscarUsuarioxNombre(t.chequeador1) == 0) { throw new Exception("Chequeador 1 ERROR"); }
                }
                if (t.chequeador2 != null)
                {
                    if (uD.buscarUsuarioxNombre(t.chequeador2) == 0) { throw new Exception("Chequeador 2 ERROR"); }
                }
                if (t.chequeador3 != null)
                {
                    if (uD.buscarUsuarioxNombre(t.chequeador3) == 0) { throw new Exception("Chequeador 3 ERROR"); }
                }
                if (t.encajador2 != null)
                {
                    if (uD.buscarUsuarioxNombre(t.encajador2) == 0) { throw new Exception("Encajador 2 ERROR"); }
                }
                if (t.encajador3 != null)
                {
                    if (uD.buscarUsuarioxNombre(t.encajador3) == 0) { throw new Exception("Encajador 3 ERROR"); }
                }
            }
            else if (Estado.Equals("ANULAREMPACADO"))
            {
                if (!t.EstadoPedido.Equals("EMPACADO")) { throw new Exception("Solo puedes ANULAREMPACADO a un ticket EMPACADO"); }
            }
            else if (Estado.Equals("ENVIADO"))
            {
                if (t.EstadoPedido.Equals("ENVIADO")) { throw new Exception("El ticket ya se encuentra  ENVIADO"); }
                if (!t.EstadoPedido.Equals("EMPACADO")) { throw new Exception("Solo puedes poner ENVIADO a Ticket en EMPACADO ,debes revertir o continuar el proceso"); }
            }
            else if (Estado.Equals("ANULARENVIADO"))
            {
                if (!t.EstadoPedido.Equals("ENVIADO")) { throw new Exception("Solo puedes ANULARENVIADO a un ticket ENVIADO"); }
            }
            else if (Estado.Equals("ENTREGADO"))
            {
                if (t.EstadoPedido.Equals("ENTREGADO")) { throw new Exception("El ticket ya se encuentra  ENTREGADO"); }
                if (!t.EstadoPedido.Equals("EMPACADO") && !t.EstadoPedido.Equals("ENVIADO")) { throw new Exception("Solo puedes poner ENTREGADO a Ticket en EMPACADO o ENVIADO ,debes revertir o continuar el proceso"); }
                if (!t.EstadoFacturacion.Equals("FACTURADO")) { throw new Exception("Solo puedes entregar ticket facturado"); }
                if (t.IdReg > 0) { if (t.RegEstado != "Entregado") { throw new Exception("Debe Entregar el regalo"); } }
            }
            else if (Estado.Equals("ANULARENTREGADO"))
            {
                if (!t.EstadoPedido.Equals("ENTREGADO")) { throw new Exception("Solo puedes ANULARENTREGADO a un ticket ENTREGADO"); }
            }
            return ticketV.editarSeguimientoTicket(Estado, DocEntry, t);
        }
        public int facturarTicket(int DocEntry, Usuario_E u)
        {
            TicketVenta_E t = obtenerTicket(DocEntry);
            if (t.EstadoPedido.Equals("SEPARADO")) { throw new Exception("No puede facturar ticket separado"); }
            if (t.EstadoPedido.Equals("ANULADO")) { throw new Exception("No puede facturar ticket anulado"); }
            if (t.EstadoFacturacion != null && t.EstadoFacturacion.Equals("FACTURADO")) { throw new Exception("El ticket ya se encuentra facturado"); }
            return ticketV.facturarTicket(DocEntry, u);
        }
        public int anularFacturarTicket(int DocEntry)
        {
            TicketVenta_E t = obtenerTicket(DocEntry);
            if (t.EstadoFacturacion != null && !t.EstadoFacturacion.Equals("FACTURADO")) { throw new Exception("No se puede revertir el proceso de facturado"); }
            return ticketV.anularFacturarTicket(DocEntry);
        }
        public int recibirTicket(int DocEntry, TicketVenta_E t)
        {
            return editarSeguimientoTicket("RECIBIDO", DocEntry, t);
        }
        public int anularRecibirTicket(int DocEntry, TicketVenta_E t)
        {
            return editarSeguimientoTicket("ANULARRECIBIDO", DocEntry, t);
        }
        public int enviarProtocoloTicket(int DocEntry, Usuario_E u)
        {
            TicketVenta_E t = obtenerTicket(DocEntry);
            if (t.EstadoProtocolo != null && t.EstadoProtocolo.Equals("REALIZADO")) { throw new Exception("El protocolo ya fue REALIZADO"); }
            return ticketV.enviarProtocoloTicket(DocEntry, u);
        }
        public int anularEnviarProtocoloTicket(int DocEntry)
        {
            TicketVenta_E t = obtenerTicket(DocEntry);
            if (t.EstadoProtocolo == null || t.EstadoProtocolo.Equals("PENDIENTE") || t.EstadoProtocolo.Equals("")) { throw new Exception("Solo puedes anular un protocolo ENVIADO"); }
            return ticketV.anularEnviarProtocoloTicket(DocEntry);
        }
        public DataTable tbRptAnalisisVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnalisisVentas_E obj)
        {
            if (!((obj.AlmIni == null && obj.AlmFin == null) || (obj.AlmIni != null && obj.AlmFin != null)))
            {
                throw new Exception("Debe elegir los lugares de entrega o dejarlos vacios");
            }
            if (!(obj.FecIni != null && obj.FecFin != null)) { throw new Exception("Debe completar las 2 fechas"); }
            return ticketV.tbRptAnalisisVentas(obj);
        }
        //ATENCION ALCLIENTES
        public List<TicketVenta_E> listarTicketsParaAtencion()
        {
            return ticketV.listarTicketsParaAtencion();
        }
        //*********CALCULOS****************
        public TicketVenta_E CalcularMontos(TicketVenta_E t)
        {
            return ticketV.CalcularMontos(t);
        } 
        //protocolo
        public List<Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E> obtenerOrdenDeVenta(int DocNum)
        {
            ReportesDigemid_NEG.DocumentosDig_N dN = new ReportesDigemid_NEG.DocumentosDig_N();
            return dN.ConsultarOrdenDeVenta(DocNum);
        }
    }
}
