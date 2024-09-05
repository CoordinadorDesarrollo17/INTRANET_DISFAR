using Capa_Datos;
using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.ComprobantesContables_ENT;
using Capa_Entidad.Rutas_ENT.ReportesSql;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.Ventas_ENT.Reportes;
using Capa_Entidad.Ventas_ENT.Tablas;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls.WebParts;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
    public class ORTV_N
    {
        ORTV_D tkD = new ORTV_D(); CC_ORTV_D ccTicket = new CC_ORTV_D();
        public object t { get; private set; }
        public List<ORTV_E> listarTicketsParaRepartos(ORTV_E filtro, string[] estados, out int cantidadTicketsNoEnviados)
        {
            return tkD.listarTicketsParaRepartos(filtro, estados, out cantidadTicketsNoEnviados);
        }
        public List<ORTV_E> listarTicketsRepartosNoEnviados(ORTV_E filtro, string[] estados)
        {
            return tkD.listarTicketsRepartosNoEnviados(filtro, estados);
        }
        public List<string> BuscarVinculados(int DocEntry, int DocNum)
        {
            return tkD.BuscarVinculados(DocEntry, DocNum);
        }
        public List<Rpt_TicketVenta_E> listarTicketsAgencia()
        {
            return tkD.listarTicketsAgencia();
        }
        public List<ORTV_E> listarTicketsSeparados(int Id)
        {
            return tkD.listarTicketsSeparados(Id);
        }
        public ORTV_E separarTicket(Usuario_E u)
        {
            return tkD.separarTicket(u);
        }
        public int registrarTicket(ORTV_E ticket)
        {
            validarDatosTicket(ticket, 0);
            if (!ticket.Estado.Equals("SEPARADO")) { throw new Exception("EL TICKET DEBE TENER ESTADO SEPARADO"); }
            if (ticket.Observaciones2 == "SI")
            {
                if (ticket.Det7 != null && ticket.Det7.Count > 0)
                {
                    foreach (var det7 in ticket.Det7)
                    {
                        var TkPrincipal = ObtenerDatosCompletosTicket(DocEntryTicket((int)det7.DocNumVinc));
                        if (TkPrincipal.Estado != "ABIERTO" && TkPrincipal.Estado != "RECIBIDO" && TkPrincipal.Estado != "PICKEANDO" && TkPrincipal.Estado != "PICKEADO" && TkPrincipal.Estado != "VERIFICANDO" && TkPrincipal.Estado != "VERIFICADO" && TkPrincipal.Estado != "EMPACANDO" && TkPrincipal.Estado != "EMPACADO" && TkPrincipal.Estado != "PESADO")
                        {
                            throw new Exception("El ticket que quiere vincular en linea " + det7.Linea + " se encuentra fuera de un estado modificable.");
                        }
                        if (string.IsNullOrEmpty(det7.CardCode) || string.IsNullOrEmpty(det7.CardName) || det7.DocNumVinc == 0 || det7.MontoFinal == 0)
                        {
                            throw new Exception("El ticket vinculado en la linea " + det7.Linea + " no cumple con los datos requeridos.");
                        }
                    }
                }
                else { throw new Exception("Debe vincular tickets"); }
            }
            return tkD.registrarTicket(ticket);
        }
        public void validarDatosTicket(ORTV_E ticket, int IdRol)
        {
            OREG_N oregN = new OREG_N();
            OCLR_N oclrN = new OCLR_N();
            if (ticket.Det2 == null || ticket.Det2.Count() == 0) { throw new Exception("No puede registrar con detalles vacíos."); }
            else
            {
                foreach (RTV2_E d in ticket.Det2)
                {
                    if (d.Verificar == "on" && string.IsNullOrEmpty(d.TipoComprobante)) { throw new Exception("Debe llenar el tipo de comprobante en la línea: " + d.Linea); }
                }
            }
            if (string.IsNullOrEmpty(ticket.FechaSapTicket)) { throw new Exception("No eligió la fecha del ticket."); }
            if (ticket.DocNum <= 0) { throw new Exception("No seleccionó un número de ticket."); }
            if (string.IsNullOrEmpty(ticket.CardCode) || string.IsNullOrEmpty(ticket.CardName)) { throw new Exception("No seleccionó un cliente."); }
            if (string.IsNullOrEmpty(ticket.Embalaje)) { throw new Exception("No seleccionó el tipo de embalaje."); }
            if (string.IsNullOrEmpty(ticket.TipoVenta)) { throw new Exception("No seleccionó el tipo de venta."); }
            if (!string.IsNullOrEmpty(ticket.WhsCodeLog) && ticket.WhsCodeLog.Equals("01") && string.IsNullOrEmpty(ticket.FormaPago)) { throw new Exception("No seleccionó la forma de pago."); }
            if (string.IsNullOrEmpty(ticket.LugarDestino)) { throw new Exception("No seleccionó el lugar de destino."); }
            else
            {
                if (ticket.LugarDestino.Equals("Agencia") || ticket.LugarDestino.Equals("Agencia Courier"))
                {
                    if (string.IsNullOrEmpty(ticket.EnvioAgencia)) { throw new Exception("Debe seleccionar el modo de envío."); }
                    if (ticket.EnvioAgencia.Equals("Oficina de agencia") && string.IsNullOrEmpty(ticket.Referencia)) { throw new Exception("Debe llenar la referencia obligatoriamente."); }
                    if (ticket.EnvioAgencia.Equals("Domicilio de cliente") && !string.IsNullOrEmpty(ticket.Referencia)) { throw new Exception("No debe llenar la referencia."); }
                    if (string.IsNullOrEmpty(ticket.Agencia)) { throw new Exception("Debe llenar la agencia."); }
                    if (string.IsNullOrEmpty(ticket.DirDestino)) { throw new Exception("Debe llenar la dirección de destino."); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].NombrePer)) { throw new Exception("Debe llenar el nombre."); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].TipoDocPer)) { throw new Exception("Debe llenar el tipo de documento personal."); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].DocPer)) { throw new Exception("Debe llenar el número de documento personal."); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].TelfPer)) { throw new Exception("Debe llenar el teléfono."); }
                    if (ticket.Embalaje != "CP") { throw new Exception("El embalaje debe ser Caja Provincia."); }

                    string lugEn = string.Empty;
                    foreach (RTV2_E d in ticket.Det2.Where(x => x.Verificar == "on"))
                    {
                        if (lugEn != string.Empty)
                        {
                            if (!(lugEn.Equals("ALMACÉN N°3") || lugEn.Equals("ALMACÉN FALTANTES") || lugEn.Equals("ALMACÉN N°7"))) { throw new Exception("El lugar de entrega debe ser válido."); }
                            if (!d.LugarDeEntrega.Equals(lugEn) && !d.LugarDeEntrega.Equals("ALMACÉN FALTANTES")) { throw new Exception("No coinciden los lugares de entrega en el detalle."); }
                        }
                        else if (!d.LugarDeEntrega.Equals("ALMACÉN FALTANTES"))
                        {
                            lugEn = d.LugarDeEntrega;
                            if (!(lugEn.Equals("ALMACÉN N°3") || lugEn.Equals("ALMACÉN FALTANTES") || lugEn.Equals("ALMACÉN N°7"))) { throw new Exception("El lugar de entrega debe ser válido."); }
                        }
                    }
                }
                else if (ticket.LugarDestino.Equals("Arriola"))
                {
                    string lugEn = string.Empty;
                    foreach (RTV2_E d in ticket.Det2.Where(x => x.Verificar == "on"))
                    {
                        if (lugEn != string.Empty)
                        {
                            if (!lugEn.Equals("ALMACÉN N°6") && !lugEn.Equals("ALMACÉN FALTANTES")) { throw new Exception("El lugar de entrega debe ser 'ALMACÉN N°6' o 'ALMACÉN FALTANTES'."); }
                            if (!d.LugarDeEntrega.Equals(lugEn) && !d.LugarDeEntrega.Equals("ALMACÉN FALTANTES")) { throw new Exception("No coinciden los lugares de entrega en el detalle."); }
                        }
                        else if (!d.LugarDeEntrega.Equals("ALMACÉN FALTANTES"))
                        {
                            lugEn = d.LugarDeEntrega;
                            if (!lugEn.Equals("ALMACÉN N°6") && !lugEn.Equals("ALMACÉN FALTANTES")) { throw new Exception("El lugar de entrega debe ser 'ALMACÉN N°6' o 'ALMACÉN FALTANTES'."); }
                        }
                    }
                }
                else if (ticket.LugarDestino.Equals("Domicilio"))
                {
                    if (string.IsNullOrEmpty(ticket.Zona)) { throw new Exception("Debe existir una zona."); }
                    if (string.IsNullOrEmpty(ticket.DirDestino)) { throw new Exception("Debe llenar la dirección de destino."); }
                    if (ticket.Det3 != null && ticket.Det3.Count >= 2 && !string.IsNullOrEmpty(ticket.Det3[1].Calle) && ticket.Det3[1].Calle.Length > 200) { throw new Exception("La dirección de destino excede el límite de 200 caracteres."); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].NombrePer)) { throw new Exception("Debe llenar el nombre."); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].TipoDocPer)) { throw new Exception("Debe llenar el tipo de documento personal."); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].DocPer)) { throw new Exception("Debe llenar el número de documento personal."); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].TelfPer)) { throw new Exception("Debe llenar el teléfono."); }
                    string lugEn = string.Empty;
                    foreach (RTV2_E d in ticket.Det2.Where(x => x.Verificar == "on"))
                    {
                        if (lugEn != string.Empty)
                        {
                            if (!(lugEn.Equals("ALMACÉN N°3") || lugEn.Equals("ALMACÉN FALTANTES") || lugEn.Equals("ALMACÉN N°7"))) { throw new Exception("El lugar de entrega no es válido."); }

                            if (!d.LugarDeEntrega.Equals(lugEn) && !d.LugarDeEntrega.Equals("ALMACÉN FALTANTES")) { throw new Exception("No coinciden los lugares de entrega en el detalle."); }

                        }
                        else if (!d.LugarDeEntrega.Equals("ALMACÉN FALTANTES"))
                        {
                                lugEn = d.LugarDeEntrega;
                                if (!(lugEn.Equals("ALMACÉN N°3") || lugEn.Equals("ALMACÉN FALTANTES") || lugEn.Equals("ALMACÉN N°7"))) { throw new Exception("El lugar de entrega no es válido."); }
                        }
                    }
                }
                else if (ticket.LugarDestino.Equals("Centro"))
                {
                    string lugEn = string.Empty;
                    foreach (RTV2_E d in ticket.Det2.Where(x => x.Verificar == "on"))
                    {
                        if (lugEn != string.Empty)
                        {
                            if (!lugEn.Equals("ALMACÉN N°1") && !lugEn.Equals("ALMACÉN FALTANTES")) { throw new Exception("El lugar de entrega solo debe ser 'ALMACÉN N°1' o 'ALMACÉN FALTANTES'."); }
                            if (!d.LugarDeEntrega.Equals(lugEn) && !d.LugarDeEntrega.Equals("ALMACÉN FALTANTES")) { throw new Exception("No coinciden los lugares de entrega en el detalle."); }
                        }
                        else if (!d.LugarDeEntrega.Equals("ALMACÉN FALTANTES"))
                        {
                            lugEn = d.LugarDeEntrega;
                            if (!lugEn.Equals("ALMACÉN N°1") && !lugEn.Equals("ALMACÉN FALTANTES")) { throw new Exception("El lugar de entrega solo debe ser 'ALMACÉN N°1' o 'ALMACÉN FALTANTES'."); }
                        }
                    }
                }
            }
            if (ticket.Det3 != null && ticket.Det3.Count >= 1)
            {
                if (ticket.Det3[0].Ubigeo > 0)
                {
                    if (string.IsNullOrEmpty(ticket.Det3[0].Calle)) { throw new Exception("Debe llenar la dirección de destino."); }
                }
            }

            if (ticket.MontoTotal <= 0) { throw new Exception("No puede registrar un monto total en cero o negativo."); }
            if (ticket.MontoFinal <= 0) { throw new Exception("No se puede registrar un monto final en cero o negativo."); }


            if (string.IsNullOrEmpty(ticket.TiempoEntrega.ToString()) || ticket.TiempoEntrega == null) { throw new Exception("Debe llenar el tiempo de entrega."); }

            if (ticket.Det1 != null && ticket.Det1.Count > 0)
            {
                if (!string.IsNullOrEmpty(ticket.Det1[0].TipoDocPer) && ticket.Det1[0].TipoDocPer.Equals("DNI") && (ticket.Det1[0].DocPer.Length != 8)) { throw new Exception("El DNI debe tener 8 dígitos."); }
                if (!string.IsNullOrEmpty(ticket.Det1[0].TipoDocPer) && ticket.Det1[0].TipoDocPer.Equals("RUC") && (ticket.Det1[0].DocPer.Length != 11)) { throw new Exception("El RUC debe tener 11 dígitos."); }
                if (!string.IsNullOrEmpty(ticket.Det1[0].TipoDocPer) && ticket.Det1[0].TipoDocPer.Equals("CE") && (ticket.Det1[0].DocPer.Length != 9)) { throw new Exception("El Carné de Extranjería debe tener 9 dígitos."); }
                if (!string.IsNullOrEmpty(ticket.Det1[0].TelfPer) && ticket.Det1[0].TelfPer.Length != 9) { throw new Exception("El teléfono debe tener 9 dígitos."); }

                if (string.IsNullOrEmpty(ticket.Det1[0].TipoDocPer) &&
                    string.IsNullOrEmpty(ticket.Det1[0].DocPer) &&
                    string.IsNullOrEmpty(ticket.Det1[0].TelfPer) &&
                    string.IsNullOrEmpty(ticket.Det1[0].NombrePer)) { ticket.Det1 = null; }
            }

            if (ticket.Flete < 0) { throw new Exception("El flete no puede ser negativo."); }
            if (ticket.DeudaCliente < 0) { throw new Exception("No puede registrar una deuda del cliente negativa."); }
            if (ticket.GastoEnvio < 0) { throw new Exception("No puede registrar un gasto de envío negativo."); }
            if (ticket.DeudaEmpresa < 0) { throw new Exception("No puede registrar una deuda de la empresa negativa."); }
            if (ticket.DescuentoNC < 0) { throw new Exception("No puede registrar un descuento negativo."); }
            if (ticket.MontoTotal != CalcularMontos(ticket).MontoTotal) { throw new Exception("El cálculo del monto total tuvo un error."); }
            if (ticket.MontoFinal != CalcularMontos(ticket).MontoFinal) { throw new Exception("El cálculo del monto final tuvo un error."); }
            if (IdRol == 0)
            {
                if (ticket.Det5 != null && ticket.Det5.Count > 0)
                {
                    int idReg = ticket.Det5[0].IdReg;
                    if (idReg > 0)
                    {
                        OREG_E objRegalo = oregN.buscarRegalo(idReg);
                        if (ticket.Det5[0].RegCant <= 0) { throw new Exception("Debe llenar las cantidades para el regalo."); }
                        else if (!oclrN.comprobarDispCliReg(new CLR1_E { IdReg = ticket.Det5[0].IdReg, CardCode = ticket.CardCode, Cantidad = ticket.Det5[0].RegCant }))
                        { throw new Exception("El cliente no tiene saldo de regalo disponible."); }
                        if (objRegalo.Estado == "Inactivo") { throw new Exception("No puede dar regalos inactivos."); }
                        if (objRegalo.StockDisp < ticket.Det5[0].RegCant) { throw new Exception("No hay stock disponible del regalo."); }
                    }
                    else if (ticket.Det5[0].RegCant > 0) { throw new Exception("Debe llenar la cantidad del regalo."); }
                }
            }
        }
        public int editarTicket(int DocEntry, ORTV_E ticket)
        {
            ORTV_E t = tkD.ObtenerDatosCompletosTicket(DocEntry);
            validarDatosTicket(ticket, 0);
            if (t.EstadoPago != null && t.EstadoPago.Equals("PAGADO")) { throw new Exception("NO PUEDE EDITAR UN TICKET PAGADO"); }
            if (!t.Estado.Equals("ABIERTO")) { throw new Exception("NO PUEDE EDITAR UN TICKET " + t.Estado); }
            if (ticket.GastoEnvio > 0) { ticket.EstadoGasto = "PENDIENTE"; }
            else { ticket.EstadoGasto = null; }
            //vinculacion
            if (ticket.Observaciones2 == "SI")
            {
                if (ticket.Det7 != null && ticket.Det7.Count > 0)
                {
                    foreach (var det7 in ticket.Det7)
                    {
                        var DocEntryPrincipal = DocEntryTicket((int)det7.DocNumVinc);
                        var TkPrincipal = ObtenerDatosCompletosTicket(DocEntryPrincipal);
                        if (TkPrincipal.Estado != "ABIERTO" && TkPrincipal.Estado != "RECIBIDO" && TkPrincipal.Estado != "PICKEANDO" && TkPrincipal.Estado != "PICKEADO" && TkPrincipal.Estado != "VERIFICANDO" && TkPrincipal.Estado != "VERIFICADO" && TkPrincipal.Estado != "EMPACANDO" && TkPrincipal.Estado != "EMPACADO" && TkPrincipal.Estado != "PESADO")
                        {
                            throw new Exception("El ticket que quiere vincular en linea " + det7.Linea + " se encuentra fuera de un estado modificable. (EMPACADO).");
                        }
                        if (string.IsNullOrEmpty(det7.CardCode) || string.IsNullOrEmpty(det7.CardName) || det7.DocNumVinc == 0 || det7.MontoFinal == 0)
                        {
                            throw new Exception("El ticket vinculado en la linea " + det7.Linea + " no cumple con los datos requeridos.");
                        }
                    }
                }
                else { throw new Exception("Debe vincular tickets"); }
            }
            return tkD.editarTicket(DocEntry, ticket);
        }
        public int editarVisibilidadTicket(int DocEntry)
        {
            return tkD.editarVisibilidadTicket(DocEntry);
        }
        public int registrarImpresionTicket(int DocEntry, string Operario)
        {
            return tkD.registrarImpresionTicket(DocEntry, Operario);
        }
        public int cancelarTicket(int DocEntry, string Operario, int IdRol)
        {
            ORTV_E t = tkD.ObtenerDatosCompletosTicket(DocEntry);
            bool continuarCancelarTicket = false;
            // Cuando el ticket esta en "SEPARADO" o "ABIERTO" solo lo podran cancelar Rol (Sup Ventas,Op Ventas)
            if (t.Estado.Equals("SEPARADO") || t.Estado.Equals("ABIERTO"))
            {
                if (IdRol == 6 || IdRol == 7 || IdRol == 12) { continuarCancelarTicket = true; }
            }
            // Cuando el ticket esta en "RECIBIDO" o "PICKEANDO" O "VERIFICANDO" O "EMPACANDO" solo lo podran cancelar Rol (Op Recepcion)
            else if (t.Estado.Equals("RECIBIDO") || t.Estado.Equals("PICKEANDO") || t.Estado.Equals("VERIFICANDO") || t.Estado.Equals("EMPACANDO"))
            {
                if (IdRol == 5) { continuarCancelarTicket = true; }
            }
            // Cuando el ticket esta en "EMPACADO" o "PESADO" O "PREENVIO" O "ENVIADO" solo lo podran cancelar Rol (Sup Atencion al cliente)
            else if (t.Estado.Equals("EMPACADO") || t.Estado.Equals("PESADO") || t.Estado.Equals("PREENVIO") || t.Estado.Equals("ENVIADO"))
            {
                if (t.Estado.Equals("PREENVIO") || t.Estado.Equals("ENVIADO"))
                {
                    //si el ticket es de centro y arriola podra cancelarse cuando este como preenvio y enviado
                    if (t.LugarDestino.Equals("Centro") || t.LugarDestino.Equals("Arriola")) { if (IdRol == 11) { continuarCancelarTicket = true; } }
                }
                else { if (IdRol == 11) { continuarCancelarTicket = true; } }
            }
            if (IdRol == 1) { continuarCancelarTicket = true; }
            if (!continuarCancelarTicket) { throw new Exception("NO SE PUEDE CANCELAR EL TICKET N° " + t.DocNum + " POR SU ESTADO " + t.Estado); }
            if (t.Estado.Equals("CANCELADO")) { throw new Exception("EL TICKET YA SE ENCUENTRA CANCELADO N°" + t.DocNum); }
            if (t.Estado.Equals("ENTREGADO")) { throw new Exception("EL TICKET YA SE ENCUENTRA ENTREGADO N°" + t.DocNum + " NO SE PUEDE CANCELAR"); }
            return tkD.cancelarTicket(DocEntry, t.Estado, Operario);
        }
        /*
         * Editar Ticket parámetros ilimitados
         */
        public int EditarTicketDesdeSeguimiento(Dictionary<string, Object> datos, string Request)
        {
            ORTV_E ticket = ObtenerDatosCompletosTicket(Convert.ToInt32(datos["docEntryTicket"]));

            if (datos["estadoTicket"].Equals("RECIBIDO") && Request.Equals("CAMBIAR A RECIBIDO"))
            {
                if (ticket.Estado.Equals("RECIBIDO")) { throw new Exception("El ticket ya se encuentra RECIBIDO."); }
                if (ticket.Estado.Equals("CANCELADO")) { throw new Exception("No puedes recibir un ticket CANCELADO"); }
                if (!ticket.Estado.Equals("ABIERTO")) { throw new Exception("Solo puedes recibir un ticket en ABIERTO, debes revertir o continuar el proceso"); }
            }
            else if (datos["estadoTicket"].Equals("ANULARRECIBIDO"))
            {
                if (!ticket.Estado.Equals("RECIBIDO")) { throw new Exception("Solo puedes AnulaRecibido a un ticket Recibido"); }
            }

            else if (datos["estadoTicket"].Equals("ENTREGADO") && Request.Equals("CAMBIAR A ENTREGADO"))
            {
                if (ticket.Estado.Equals("ENTREGADO")) { throw new Exception("El ticket ya se encuentra  ENTREGADO"); }
                if (!ticket.Estado.Equals("EMPACADO") && !ticket.Estado.Equals("PESADO") && !ticket.Estado.Equals("ENVIADO"))
                {
                    throw new Exception("El ticket debe estar en EMPACADO, PESADO o ENVIADO, debes revertir o continuar el proceso");
                }
                if (!ticket.EstadoFacturacion.Equals("FACTURADO")) { throw new Exception("Solo puedes entregar ticket facturado"); }
                if (ticket.Det5[0].IdReg > 0) { if (ticket.Det5[0].RegEstado != "Entregado") { throw new Exception("Debe Entregar el regalo"); } }
            }
            else if (datos["estadoTicket"].Equals("ANULARENTREGADO"))
            {
                if (!ticket.Estado.Equals("ENTREGADO")) { throw new Exception("Solo puedes ANULARENTREGADO a un ticket ENTREGADO"); }
                if (ticket.Det5.Count >= 1)
                {
                    if ((ticket.Det5[0].IdReg > 0 || ticket.Det5[0].RegCant > 0) && ticket.Det5[0].RegEstado == "PENDIENTE") { throw new Exception("Solo puedes ANULAR ENTREGA a un ticket que no tenga regalo pendiente"); }
                }
            }
            else if (datos["estadoTicket"].ToString() == "EMPACADO" && Request.Equals("ENVIAR DATOS"))
            {
                if (!ticket.Estado.Equals("EMPACADO"))
                {
                    throw new Exception("El ticket debe estar en EMPACADO para cambiar datos de este proceso");
                }
            }

            return tkD.EditarTicketDesdeSeguimiento(datos);
        }
        public int editarSeguimientoTicket(string Estado, int DocEntry, ORTV_E t)
        {
            if (Estado.Equals("RECIBIDO"))
            {
                if (t.Estado.Equals("RECIBIDO")) { throw new Exception("El ticket ya se encuentra RECIBIDO."); }
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
                if (!t.Estado.Equals("ABIERTO")) { throw new Exception("Solo puedes recibir un ticket en ABIERTO, debes revertir o continuar el proceso"); }
            }
            else if (Estado.Equals("ANULARRECIBIDO"))
            {
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
                if (!t.Estado.Equals("RECIBIDO")) { throw new Exception("Solo puedes ANULAR RECIBIR a un ticket RECIBIDO"); }
            }
            else if (Estado.Equals("INICIO PICKING"))
            {
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
                if (t.Estado.Equals("PICKEANDO")) { throw new Exception("El ticket ya se encuentra PICKEANDO"); }
                if (!t.Estado.Equals("RECIBIDO")) { throw new Exception("Solo puedes poner PICKEANDO a Ticket en RECIBIDO, debes revertir o continuar el proceso"); }
            }
            else if (Estado.Equals("ANULAR INICIO PICKING"))
            {
                if (t.RolSupervisor != 4 && t.RolSupervisor != 11 && t.RolSupervisor != 1 && t.RolSupervisor != 5) { throw new Exception("No tiene permisos para revertir procesos"); }
                List<CC_ORTV_E> listaEstados = ccTicket.ListarCC_ORTV(DocEntry, null, true);
                if (listaEstados.Count > 0)
                {
                    if (listaEstados.FirstOrDefault().Operacion != "INICIO PICKING" && listaEstados.FirstOrDefault().Operacion != "ANULAR FIN PICKING" && listaEstados.FirstOrDefault().Operacion != "ANULAR INICIO VERIFICAR")
                    {
                        throw new Exception("Solo puedes ANULAR INICIO PICKING  a un ticket con ultimo flujo INICIO PICKING O ANULAR FIN PICKING o ANULAR INICIO VERIFICAR");
                    }
                }
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
            }
            else if (Estado.Equals("FIN PICKING"))
            {
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
                List<CC_ORTV_E> listaEstados = ccTicket.ListarCC_ORTV(DocEntry, null, true);
                if (listaEstados.Count > 0) { if (listaEstados.FirstOrDefault().Operacion == "FIN PICKING") { throw new Exception("El ticket ya FINALIZO PICKING"); } }
                if (!t.Estado.Equals("PICKEANDO")) { throw new Exception("Solo puedes poner FIN PICKING a Ticket en PICKEANDO, debes revertir o continuar el proceso"); }
            }
            else if (Estado.Equals("ANULAR FIN PICKING"))
            {
                if (t.RolSupervisor != 4 && t.RolSupervisor != 11 && t.RolSupervisor != 1 && t.RolSupervisor != 5) { throw new Exception("No tiene permisos para revertir procesos"); }
                List<CC_ORTV_E> listaEstados = ccTicket.ListarCC_ORTV(DocEntry, null, true);
                if (listaEstados.Count > 0)
                {
                    if (listaEstados.FirstOrDefault().Operacion != "FIN PICKING" && listaEstados.FirstOrDefault().Operacion != "ANULAR INICIO VERIFICAR")
                    {
                        throw new Exception("Solo puedes ANULAR FIN PICKING  a un ticket con ultimo flujo FIN PICKING O ANULAR INICIO VERIFICAR");
                    }
                }
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
            }
            else if (Estado.Equals("INICIO VERIFICAR"))
            {
                List<CC_ORTV_E> listaEstados = ccTicket.ListarCC_ORTV(DocEntry, null, true);
                if (listaEstados.Count > 0)
                {
                    if (listaEstados.FirstOrDefault().Operacion != "INICIO PICKING" &&
                        listaEstados.FirstOrDefault().Operacion != "FIN PICKING" &&
                        listaEstados.FirstOrDefault().Operacion != "ANULAR INICIO VERIFICAR" &&
                        listaEstados.FirstOrDefault().Operacion != "ANULAR FIN PICKING" &&
                        t.Estado != "PICKEANDO")
                    { throw new Exception("El ticket no esta apto para INICIAR VERIFICAR, revise los flujos"); }
                    if (listaEstados.FirstOrDefault().Operacion == "INICIO VERIFICAR" ||
                        listaEstados.FirstOrDefault().Operacion == "FIN VERIFICAR" ||
                        listaEstados.FirstOrDefault().Operacion == "INICIO EMPACAR" ||
                        listaEstados.FirstOrDefault().Operacion == "FIN EMPACAR"
                        ) { throw new Exception("El ticket ya paso proceso INICIAR VERIFICAR"); }
                }
                if (t.Estado.Equals("EMPACANDO") || t.Estado.Equals("EMPACADO") || t.Estado.Equals("PESADO") || t.Estado.Equals("PREENVIO")
                    || t.Estado.Equals("ENVIADO") || t.Estado.Equals("ENTREGADO")) { throw new Exception("El ticket esta en un estado posterior a VERIFICANDO"); }
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
            }
            else if (Estado.Equals("ANULAR INICIO VERIFICAR"))
            {
                if (t.RolSupervisor != 4 && t.RolSupervisor != 11 && t.RolSupervisor != 1 && t.RolSupervisor != 5) { throw new Exception("No tiene permisos para revertir procesos"); }
                List<CC_ORTV_E> listaEstados = ccTicket.ListarCC_ORTV(DocEntry, null, true);
                if (listaEstados.Count > 0)
                {   
                    if (listaEstados.FirstOrDefault().Operacion != "ANULAR FIN PICKING" && listaEstados.FirstOrDefault().Operacion != "INICIO VERIFICAR" && listaEstados.FirstOrDefault().Operacion != "ANULAR FIN VERIFICAR" && listaEstados.FirstOrDefault().Operacion != "ANULAR INICIO EMPACAR")
                    { throw new Exception("Solo puedes ANULAR INICIO VERIFICAR a un ticket con ultimo flujo INICIO VERIFICAR O ANULAR FIN VERIFICAR"); }
                }
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
            }
            else if (Estado.Equals("FIN VERIFICAR"))
            {
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
                List<CC_ORTV_E> listaEstados = ccTicket.ListarCC_ORTV(DocEntry, null, true);
                if (listaEstados.Count > 0) { if (listaEstados.FirstOrDefault().Operacion == "ANULAR FIN PICKING") { throw new Exception("El ticket no es apto para FINALIZAR VERIFICAR"); } }
                if (listaEstados.Count > 0) { if (listaEstados.FirstOrDefault().Operacion == "FIN VERIFICAR") { throw new Exception("El ticket ya FINALIZO VERIFICAR"); } }
                if (t.Det12 == null || t.Det12[0].Operario == string.Empty && (t.Det12[1].Operario == string.Empty || t.Det12[2].Operario == string.Empty))
                {
                    throw new Exception("Debe llenar el verificador");
                }
            }
            else if (Estado.Equals("ANULAR FIN VERIFICAR"))
            {
                if (t.RolSupervisor != 4 && t.RolSupervisor != 11 && t.RolSupervisor != 1 && t.RolSupervisor != 5) { throw new Exception("No tiene permisos para revertir procesos"); }
                List<CC_ORTV_E> listaEstados = ccTicket.ListarCC_ORTV(DocEntry, null, true);
                if (listaEstados.Count > 0)
                {
                    if (listaEstados.FirstOrDefault().Operacion != "FIN VERIFICAR" && listaEstados.FirstOrDefault().Operacion != "ANULAR INICIO EMPACAR")
                    {
                        throw new Exception("Solo puedes ANULAR FIN VERIFICAR  a un ticket con ultimo flujo FIN VERIFICAR O ANULAR INICIO EMPACAR");
                    }

                }
            }
            else if (Estado.Equals("INICIO EMPACAR"))
            {
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
                if (t.Estado.Equals("EMPACANDO")) { throw new Exception("El ticket ya se encuentra EMPACANDO"); }
                List<CC_ORTV_E> listaEstados = ccTicket.ListarCC_ORTV(DocEntry, null, true);
                if (listaEstados.Count > 0)
                {
                    bool aptoIniEmpacar = false;
                    // Revisamos si hay INICIO VERIFICAR
                    List<CC_ORTV_E> ticketIniVerificar = ccTicket.ListarCC_ORTV(DocEntry, "INICIO VERIFICAR");
                    // Revisamos si hay ANULAR INICIO VERIFICAR
                    List<CC_ORTV_E> ticketAnularIniVerificar = ccTicket.ListarCC_ORTV(DocEntry, "ANULAR INICIO VERIFICAR");
                    List<CC_ORTV_E> listaVerif = new List<CC_ORTV_E>() { ticketIniVerificar[0], ticketAnularIniVerificar[0] };
                    var listaVerifOr = listaVerif.OrderByDescending(x => x.Id);
                    if (listaVerifOr.FirstOrDefault().Operacion == "ANULAR INICIO VERIFICAR") { aptoIniEmpacar = false; }
                    else if (listaVerifOr.FirstOrDefault().Operacion == "INICIO VERIFICAR") { aptoIniEmpacar = true; }

                    if ((listaEstados.FirstOrDefault().Operacion == "INICIO VERIFICAR" ||
                          listaEstados.FirstOrDefault().Operacion == "FIN VERIFICAR" ||
                          listaEstados.FirstOrDefault().Operacion == "ANULAR INICIO EMPACAR" ||
                          listaEstados.FirstOrDefault().Operacion == "FIN PICKING") && aptoIniEmpacar == false)
                    { throw new Exception("El ticket no se encuentra apto para INICIAR EMPACAR, revise su flujo"); }
                }
                if (t.Estado != "PICKEANDO" && t.Estado != "VERIFICANDO") { throw new Exception("El ticket no se encuentra apto para INICIAR EMPACAR, revise su flujo"); }
            }
            else if (Estado.Equals("ANULAR INICIO EMPACAR"))
            {
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
                if (t.RolSupervisor != 4 && t.RolSupervisor != 11 && t.RolSupervisor != 1 && t.RolSupervisor != 5) { throw new Exception("No tiene permisos para revertir procesos"); }
                List<CC_ORTV_E> listaEstados = ccTicket.ListarCC_ORTV(DocEntry, null, true);
                if (listaEstados.Count > 0)
                {
                    if (listaEstados.FirstOrDefault().Operacion != "INICIO EMPACAR" && listaEstados.FirstOrDefault().Operacion != "ANULAR FIN EMPACAR")
                    {
                        throw new Exception("Solo puedes ANULAR INICIO EMPACAR  a un ticket con ultimo flujo INICIO EMPACAR O ANULAR FIN EMPACAR");
                    }
                }
            }
            else if (Estado.Equals("FIN EMPACAR"))
            {
                if (t.LugarDestino == "Arriola" || t.LugarDestino == "Centro") { if (string.IsNullOrEmpty(t.AlmProcedencia)) { throw new Exception("Debe seleccionar Alm procedencia"); } }
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
                if (t.Estado.Equals("EMPACADO")) { throw new Exception("El ticket ya se encuentra  EMPACADO"); }
                List<CC_ORTV_E> listaEstados = ccTicket.ListarCC_ORTV(DocEntry, null, true);
                if (listaEstados.Count > 0)
                {
                    if (!string.IsNullOrEmpty(t.Operario) && (t.Operario == "05" || t.Operario == "07"))
                    {
                        if (listaEstados.FirstOrDefault().Operacion != "INICIO PICKING" && listaEstados.FirstOrDefault().Operacion != "ANULAR FIN PICKING")
                        { throw new Exception("El ticket no cumple con las condiciones para pasar a EMPACADO"); }
                    }
                    else
                    {
                        if (listaEstados.FirstOrDefault().Operacion != "INICIO EMPACAR" && listaEstados.FirstOrDefault().Operacion != "FIN VERIFICAR" && listaEstados.FirstOrDefault().Operacion != "ANULAR FIN EMPACAR")
                        { throw new Exception("El ticket no cumple con las condiciones para pasar a EMPACADO"); }
                    }
                }
                if (t.Cajas <= 0) { throw new Exception("Debe ingresar el nro de cajas para empacar"); }
                if (t.NroMesa <= 0) { throw new Exception("Debe ingresar el nro de Mesa para empacar"); }
            }
            else if (Estado.Equals("ANULAR FIN EMPACAR"))
            {
                if (t.RolSupervisor != 4 && t.RolSupervisor != 11 && t.RolSupervisor != 1 && t.RolSupervisor != 5) { throw new Exception("No tiene permisos para revertir procesos"); }
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
                if (!t.Estado.Equals("EMPACADO")) { throw new Exception("Solo puedes ANULAR FIN EMPACAR  a un ticket EMPACADO"); }
            }
            else if (Estado.Equals("ENVIADO"))
            {
                if (t.Estado.Equals("CANCELADO")) { throw new Exception("El ticket esta CANCELADO"); }
                if (t.Estado.Equals("ENVIADO")) { throw new Exception("El ticket ya se encuentra  ENVIADO"); }
                if (!t.Estado.Equals("EMPACADO")) { throw new Exception("Solo puedes poner ENVIADO a Ticket en EMPACADO ,debes revertir o continuar el proceso"); }
            }
            else if (Estado.Equals("ANULARENVIADO"))
            {
                if (!t.Estado.Equals("ENVIADO")) { throw new Exception("Solo puedes ANULARENVIADO a un ticket ENVIADO"); }
            }
            else if (Estado.Equals("PESADO"))
            {
                if (t.LugarDestino != "Agencia Courier" && t.LugarDestino != "Agencia") { throw new Exception("El ticket no puede ser pesado por tipo de lugar destino"); }
                if (t.Estado.Equals("PESADO")) { throw new Exception("El ticket ya se encuentra  PESADO"); }
                if (!t.Estado.Equals("EMPACADO")) { throw new Exception("Solo puedes poner PESADO a Ticket en EMPACADO ,debes revertir o continuar el proceso"); }
                if (t.Det6 == null) { throw new Exception("El ticket debe contar con pesos registrados"); }
                if (t.Det6 != null)
                {
                    foreach (var item in t.Det6)
                    {
                        if (item.Peso <= 0 || item.Verificar != "on")
                        {
                            throw new Exception("Verifique los pesos capturados");
                        }
                    }
                }
            }
            else if (Estado.Equals("ANULARPESADO"))
            {
                if (!t.Estado.Equals("PESADO")) { throw new Exception("Solo puedes ANULARPESADO a un ticket PESADO"); }
            }
            else if (Estado.Equals("ENTREGADO"))
            {

                if (t.Estado.Equals("ENTREGADO")) { throw new Exception("El ticket ya se encuentra  ENTREGADO"); }
                if (!t.Estado.Equals("EMPACADO") && !t.Estado.Equals("PESADO") && !t.Estado.Equals("ENVIADO"))
                {

                    throw new Exception("El ticket debe estar en EMPACADO, PESADO o ENVIADO, debes revertir o continuar el proceso");
                }


                if (!t.EstadoFacturacion.Equals("FACTURADO")) { throw new Exception("Solo puedes entregar ticket facturado"); }
                if (t.Det5 != null && t.Det5.Count >= 1)
                {
                    if (t.Det5[0].IdReg > 0) { if (t.Det5[0].RegEstado != "Entregado") { throw new Exception("Debe Entregar el regalo"); } }
                }
            }
            else if (Estado.Equals("ANULARENTREGADO"))
            {
                var to = ObtenerDatosCompletosTicket(t.DocEntry);
                if (!t.Estado.Equals("ENTREGADO")) { throw new Exception("Solo puedes ANULARENTREGADO a un ticket ENTREGADO"); }
                if (to.Det5 != null && to.Det5.Count >= 1)
                {
                    if ((to.Det5[0].IdReg > 0 || to.Det5[0].RegCant > 0) && to.Det5[0].RegEstado == "PENDIENTE") { throw new Exception("Solo puedes ANULAR ENTREGA a un ticket que no tenga regalo pendiente"); }
                }
            }
            return tkD.editarSeguimientoTicket(Estado, DocEntry, t);
        }
        public int emitirGuia(int DocEntry, Usuario_E u)
        {
            ORTV_E t = ObtenerDatosCompletosTicket(DocEntry);
            if (t.Estado.Equals("CANCELADO") || t.Estado.Equals("ANULADO")) { throw new Exception("No puede emitir guia en este ticket."); }
            if (!t.EstadoFacturacion.Equals("PENDIENTE")) { throw new Exception("El ticket no puede emitir guias."); }
            return tkD.emitirGuia(DocEntry, u);
        }
        public int revertirGuiasTicket(int DocEntry, String operario)
        {
            ORTV_E t = ObtenerDatosCompletosTicket(DocEntry);
            if (!t.EstadoFacturacion.Equals("GRE EMITIDA")) { throw new Exception("No se puede revertir el proceso de guias"); }
            return tkD.revertirGuiasTicket(DocEntry, operario);
        }
        public int facturarTicket(int DocEntry, Usuario_E u)
        {
            Capa_Datos.Ventas_DAO.Tablas.OINV_D oinvD = new Capa_Datos.Ventas_DAO.Tablas.OINV_D();
            Capa_Negocio.ComprobantesContables_NEG.Comprobante_N compN = new Capa_Negocio.ComprobantesContables_NEG.Comprobante_N();
            ORTV_E t = ObtenerDatosCompletosTicket(DocEntry);
            //validamos que existan facturas o boletas
            List<int> OrdenesSap = compN.ObtenerDocEntryOV(t.Det2,true);

            List<OINV_E> ComprobantesVinculados = new List<OINV_E>();
            foreach (int DocEntryO in OrdenesSap)
            {   
                List<OINV_E> comprobantesPorOrden = oinvD.listadoComprobantesPorOrdr(DocEntryO);
                ComprobantesVinculados.AddRange(comprobantesPorOrden);
            }
            if (ComprobantesVinculados.Count == 0) { throw new Exception("Este ticket no tiene facturas o boletas relacionadas desde SAP"); }
            //valida que el campo Max1099 de facturas o boletas encontradas sumen el monto total a pagar del ticket // El dato Max1099 cubre los anticipos 
            if (ComprobantesVinculados.Sum(x => x.Max1099) != t.MontoTotal) { throw new Exception("Los documentos emitidos no suman lo total a pagar por el cliente"); }

            //validamos que las guias esten completas, excluyendo 
            if (t.LugarDestino == "Centro" || t.LugarDestino == "Arriola")
            {
                //Valida cantidad de guias igual a cantidad de OV
                int cantidadOrdenes = OrdenesSap.Count;
                int cantidadGuias = compN.ObtenerEncabezadoGuiasTransferencia(t).Count();
                if (cantidadGuias != cantidadOrdenes)
                {
                    throw new Exception("Cantidad de guías emitidas con ordenes de venta no coincide.");
                }
            }
            else
            { //Valida monto de entrega igual a monto de factura
                decimal sumEntregas = compN.ObtenerEncabezadoGuiasPorEntrega(OrdenesSap).Sum(x => x.DocTotal); // Trae Dato Max1099 de entrega lo inserta en variable DocTotal
                decimal sumFacturas = ComprobantesVinculados.Sum(x => x.Max1099);
                if (sumFacturas != sumEntregas) { throw new Exception("Montos no coinciden"); }
            }

            if (t.Estado.Equals("CANCELADO") || t.Estado.Equals("ANULADO")) { throw new Exception("No puede facturar en este ticket."); }
            if (!t.EstadoFacturacion.Equals("GRE EMITIDA")) { throw new Exception("El ticket no tiene guías emitidas"); }
            return tkD.facturarTicket(DocEntry, u);
        }
        public int revertirFacturarTicket(int DocEntry, String operario)
        {
            ORTV_E t = ObtenerDatosCompletosTicket(DocEntry);
            if (!t.EstadoFacturacion.Equals("FACTURADO")) { throw new Exception("No se puede revertir el proceso de facturado"); }
            return tkD.revertirFacturarTicket(DocEntry, operario);
        }
        public int recibirTicket(int DocEntry, ORTV_E t)
        {
            return editarSeguimientoTicket("RECIBIDO", DocEntry, t);
        }
        public int anularRecibirTicket(int DocEntry, ORTV_E t)
        {
            return editarSeguimientoTicket("ANULARRECIBIDO", DocEntry, t);
        }
        public List<RptAnalisisTickets_E> ListarRptAnalisisTickets(RptFiltrosAnalisisTickets_E datosFiltro)
        {
            return tkD.ListarRptAnalisisTickets(datosFiltro);
        }
        public DataTable tbRptAnalisisVentas(Capa_Entidad.Ventas_ENT.Formularios.FrmAnalisisVentas_E obj)
        {
            if (!((obj.AlmIni == null && obj.AlmFin == null) || (obj.AlmIni != null && obj.AlmFin != null)))
            {
                throw new Exception("Debe elegir los lugares de entrega o dejarlos vacios");
            }
            if (!(obj.FecIni != null && obj.FecFin != null)) { throw new Exception("Debe completar las 2 fechas"); }
            return tkD.tbRptAnalisisVentas(obj);
        }
        //ATENCION AL CLIENTES
        public List<ORTV_E> ListarTicketsParaAtencion()
        {
            return tkD.ListarTicketsParaAtencion();
        }
        //*********CALCULOS****************//
        public ORTV_E CalcularMontos(ORTV_E t)
        {
            return tkD.CalcularMontos(t);
        }
        public List<Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E> obtenerOrdenDeVenta(int DocNum)
        {
            ReportesDigemid_NEG.DocumentosDig_N dN = new ReportesDigemid_NEG.DocumentosDig_N();
            return dN.ConsultarOrdenDeVenta(DocNum);
        }
        public string generaInfoListaClientes(string Fecha)
        {
            return tkD.generaInfoListaClientes(Fecha);
        }
        public string generaInfoListaDirDestinos(string CardCode)
        {
            return tkD.generaInfoListaDirDestinos(CardCode);
        }
        
        public string generaInfoListaNotasDeCreditoV(string CardCode)
        {
            return tkD.generaInfoListaNotasDeCreditoV(CardCode);
        }
        public string GuiasTicket(int DocEntry)
        {
            return tkD.GuiasTicket(DocEntry);
        }
        public Tickets buscarTicket(int DocEntry)
        {
            return tkD.buscarTicket(DocEntry);
        }
        public ORTV_E CalcularPesoTotal(ORTV_E t)
        {
            return tkD.CalcularPesoTotal(t);
        }
        public List<Tickets> buscarVariosTickets(int[] arrDocNum)
        {
            return tkD.buscarVariosTickets(arrDocNum);
        }
        public Tickets entregarMasivoTicket(int DocEntry, string OpEntrega, int entregadoConRegalo)
        {
            // Antes de entregar los tickets, buscaremos si tiene alguna actualización de datos
            var ticket = buscarTicket(DocEntry);
            ticket.Operario = OpEntrega;

            // Si tiene un cambio de @Estado y @RegCant
            if (ticket.Det5 != null && ticket.Det5.Count() > 0)
            {
                if (ticket.Estado.Equals("ENVIADO") && (ticket.Det5[0].RegCant >= 1 && ticket.Det5[0].RegEstado != "Entregado" || (entregadoConRegalo == 1)))
                {
                    tkD.entregarMasivoTicket(DocEntry, ticket);
                }
            }
            else
            {
                if (ticket.Estado == "ENVIADO" && entregadoConRegalo == 0)
                {
                    tkD.entregarMasivoTicket(DocEntry, ticket);
                }
            }

            return ticket;
        }
        
        public List<RTV4_E> obtenerDet4Ticket(int DocEntry, int DocNum = 0)
        {
            return tkD.obtenerDet4Ticket(DocEntry, DocNum);
        }






        // Reformulando metodos
        public (string Persona, string documento) obtenerPersonaRecojoParaGuia(int docNum)
        {
            return tkD.obtenerPersonaRecojoParaGuia(docNum);
        }
        public void editarTicketSup(int DocEntry, int idRol, ORTV_E ticket)
        {
            ORTV_E t = tkD.ObtenerDatosCompletosTicket(DocEntry);
            validarDatosTicket(ticket, idRol);
            switch (t.Estado)
            {
                case "ENTREGADO":
                case "ANULADO":
                case "CANCELADO":
                    throw new Exception("NO PUEDE EDITAR UN TICKET EN ESTADO ENTREGADO,ANULADO O CANCELADO");
                    break;
            }

            if (idRol != 1 && idRol != 6) { throw new Exception("NO TIENE PERMISOS PARA LA EDICION EXTRAORDINARIA"); }
            //validar tickets vinculados
            if (ticket.Observaciones2 == "SI")
            {
                if (ticket.Det7 != null && ticket.Det7.Count > 0)
                {
                    foreach (var det7 in ticket.Det7)
                    {
                        int docEntry = DocEntryTicket(Convert.ToInt32(det7.DocNumVinc));
                        var TkPrincipal = ObtenerDatosCompletosTicket(docEntry);
                        if (TkPrincipal.Estado != "ABIERTO" && TkPrincipal.Estado != "RECIBIDO" && TkPrincipal.Estado != "PICKEANDO" && TkPrincipal.Estado != "PICKEADO" && TkPrincipal.Estado != "VERIFICANDO" && TkPrincipal.Estado != "VERIFICADO" && TkPrincipal.Estado != "EMPACANDO" && TkPrincipal.Estado != "EMPACADO" && TkPrincipal.Estado != "PESADO")
                        {
                            throw new Exception("El ticket que quiere vincular en linea " + det7.Linea + " se encuentra fuera de un estado modificable.");
                        }
                        if (string.IsNullOrEmpty(det7.CardCode) || string.IsNullOrEmpty(det7.CardName) || det7.DocNumVinc == 0 || det7.MontoFinal == 0)
                        {
                            throw new Exception("El ticket vinculado en la linea " + det7.Linea + " no cumple con los datos requeridos.");
                        }
                    }
                }
                else { throw new Exception("Debe vincular tickets"); }
            }
            tkD.editarTicketSup(DocEntry, ticket);
        }
        public (string HtmlContent, string TipoVenta) generaInfoListaOrdenesDeVenta(string fecha, string cardCode, int docNum)
        {
            return tkD.generaInfoListaOrdenesDeVenta(fecha, cardCode, docNum);
        }
        public string EstadoTicket(int docEntry)
        { return tkD.EstadoTicket(docEntry); }
        public int DocNumTicket(int docEntry)
        { return tkD.DocNumTicket(docEntry); }
        public int DocNumTicketLike(int docNumLike)
        { return tkD.DocNumTicketLike(docNumLike); }
        public int DocEntryTicket(int docNum)
        { return tkD.DocEntryTicket(docNum); }
        public List<ORTV_E> ListarTicketsAreaVenta(Usuario_E user, ORTV_E t)
        { return tkD.ListarTicketsAreaVenta(user, t); }
        public int CantidadTicketsFacturacion(string estadoFacturacion) //Trae la cantidad de tickets PENDIENTES o GRE EMITIDA para vista de facturaciòn
        {return tkD.CantidadTicketsFacturacion(estadoFacturacion);}
        public ORTV_E ObtenerDatosCompletosTicket(int DocEntry)
        {return tkD.ObtenerDatosCompletosTicket(DocEntry);}
        public ORTV_E ObtenerTicketFacturacion(int docEntry)// Trae datos especificos para un ticket en controller facturacion
        { return tkD.ObtenerTicketFacturacion(docEntry); }
        public ORTV_E ObtenerTicketVenta(int docEntry)// Trae datos especificos para un ticket con Det2 y Det3 ( usa vinculacion )
        { return tkD.ObtenerTicketVenta(docEntry); }
        public ORTV_E ObtenerReferenciaEstadosTicket(ORTV_E ticket)
        { return tkD.ObtenerReferenciaEstadosTicket(ticket);}
        public ORTV_E ObtenerDatosTicketParaDocumentos(int docEntry)
        { return tkD.ObtenerDatosTicketParaDocumentos(docEntry);}
        public ORTV_E ObtenerTicketRotulado(int docEntry)
        { return tkD.ObtenerTicketRotulado(docEntry); }
        public ORTV_E ObtenerTicketTacoEmpaque(int docEntry)
        { return tkD.ObtenerTicketTacoEmpaque(docEntry); }
        public List<ORTV_E> ListarTicketsAreaFacturacion(Usuario_E user, ORTV_E t)
        { return tkD.ListarTicketsAreaFacturacion(user, t); }
        public List<ORTV_E> ListarTicketsAreaRecepcion(Usuario_E user, ORTV_E t)
        { return tkD.ListarTicketsAreaRecepcion(user, t); }
        public List<ORTV_E> ListarTicketsAreaAlmacén(Usuario_E user, ORTV_E t)
        { return tkD.ListarTicketsAreaAlmacén(user, t); }
        public List<ORTV_E> ListarTicketsAreaDespacho(Usuario_E user, ORTV_E t)
        { return tkD.ListarTicketsAreaDespacho(user, t); }
        public List<RTV2_E> obtenerDet2Ticket(int DocEntry)
        {
            return tkD.obtenerDet2Ticket(DocEntry);
        }

    }
}