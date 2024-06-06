using Capa_Datos;
using Capa_Datos.Ventas_DAO.TablasSql;
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
using System.Linq;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
    public class ORTV_N
    {
        ORTV_D ticketV = new ORTV_D(); CC_ORTV_D ccTicket = new CC_ORTV_D();
        public object t { get; private set; }
        //public string generaTablaTicketsVenta(ORTV_E filtro, int days, string[] estados)
        //{
        //    return ticketV.generaTablaTicketsVenta(filtro, days, estados);
        //}
        public List<ORTV_E> listarTicketsParaRepartos(ORTV_E filtro, string[] estados, out int cantidadTicketsNoEnviados)
        {
            return ticketV.listarTicketsParaRepartos(filtro, estados, out cantidadTicketsNoEnviados);
        }
        public List<ORTV_E> listarTicketsRepartosNoEnviados(ORTV_E filtro, string[] estados)
        {
            return ticketV.listarTicketsRepartosNoEnviados(filtro, estados);
        }
        public List<ORTV_E> listarTicketsVenta(Usuario_E user, ORTV_E t)
        {
            return ticketV.listarTicketsVenta(user, t);
        }
        public List<string> BuscarVinculados(int DocEntry, int DocNum)
        {
            return ticketV.BuscarVinculados(DocEntry, DocNum);
        }
        public List<Rpt_TicketVenta_E> listarTicketsAgencia()
        {
            return ticketV.listarTicketsAgencia();
        }
        public List<ORTV_E> listarTicketsSeparados(int Id)
        {
            return ticketV.listarTicketsSeparados(Id);
        }
        public ORTV_E separarTicket(Usuario_E u)
        {
            return ticketV.separarTicket(u);
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
                        var TkPrincipal = obtenerTicket(Convert.ToInt32(det7.DocNumVinc - 2000000000));
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
            return ticketV.registrarTicket(ticket);
        }
        public void validarDatosTicket(ORTV_E ticket, int IdRol)
        {
            OREG_N oregN = new OREG_N(); OCLR_N oclrN = new OCLR_N();
            if (ticket.Det2 == null || ticket.Det2.Count() == 0) { throw new Exception("NO PUEDE REGISTRAR CON DETALLES VACIOS"); }
            else
            {
                foreach (RTV2_E d in ticket.Det2)
                {
                    if (d.Verificar == "on" && string.IsNullOrEmpty(d.TipoComprobante)) { throw new Exception("DEBE LLENAR TIPO DE COMPROBANTE EN DETALLES LINEA: " + d.Linea); }
                }
            }
            if (string.IsNullOrEmpty(ticket.FechaSapTicket)) { throw new Exception("NO ELIGIÓ FECHA TICKET"); }
            if (ticket.DocNum <= 0) { throw new Exception("NO SELECCIONÓ UN NRO. DE TICKET"); }
            if (string.IsNullOrEmpty(ticket.CardCode) || string.IsNullOrEmpty(ticket.CardName)) { throw new Exception("NO SELECCIONÓ CLIENTE"); }
            if (string.IsNullOrEmpty(ticket.Embalaje)) { throw new Exception("NO SE SELECCIONO TIPO EMBALAJE"); }
            if (string.IsNullOrEmpty(ticket.TipoVenta)) { throw new Exception("NO SELECCIONÓ TIPO DE VENTA"); }
            if (!string.IsNullOrEmpty(ticket.WhsCodeLog) && ticket.WhsCodeLog.Equals("01") && string.IsNullOrEmpty(ticket.FormaPago)) { throw new Exception("NO SELECCIONÓ FORMA DE PAGO"); }
            if (string.IsNullOrEmpty(ticket.LugarDestino)) { throw new Exception("NO SELECCIONÓ LUGAR DESTINO"); }
            else
            {
                if (ticket.LugarDestino.Equals("Agencia") || ticket.LugarDestino.Equals("Agencia Courier"))
                {
                    if (string.IsNullOrEmpty(ticket.EnvioAgencia)) { throw new Exception("DEBE SELECCIONAR MODO DE ENVIO"); }
                    if (ticket.EnvioAgencia.Equals("Oficina de agencia") && string.IsNullOrEmpty(ticket.Referencia)) { throw new Exception("DEBE LLENAR REFERENCIA OBLIGATORIAMENTE"); }
                    if (ticket.EnvioAgencia.Equals("Domicilio de cliente") && !string.IsNullOrEmpty(ticket.Referencia)) { throw new Exception("NO DEBE LLENAR REFERENCIA"); }
                    if (string.IsNullOrEmpty(ticket.Agencia)) { throw new Exception("DEBE LLENAR AGENCIA "); }
                    if (string.IsNullOrEmpty(ticket.DirDestino)) { throw new Exception("DEBE LLENAR DIRECCIONDESTINO"); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].NombrePer)) { throw new Exception("DEBE LLENAR NOMBRE"); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].TipoDocPer)) { throw new Exception("DEBE LLENAR TIPO DE DOCUMENTO PER"); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].DocPer)) { throw new Exception("DEBE LLENAR EL NRODOCUMENTO PER"); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].TelfPer)) { throw new Exception("DEBE LLENAR TELEFONO"); }
                    if (ticket.Embalaje != "CP") { throw new Exception("EMBALAJE DEBE SER CAJA PROVINCIA"); }
                    /*
                     * SE QUITA VALIDACIÓN 01-06-23 POR OBSERVACIONES SOBRE COBEFAR V2
                     * if (ticket.EnvioAgencia == "Oficina de agencia" && (ticket.Det3[1].Ubigeo == null || string.IsNullOrEmpty(ticket.Det3[1].Calle) || string.IsNullOrEmpty(ticket.Agencia)))
                    { throw new Exception("DEBE LLENAR DIRECCIONDESTINO2 Y AGENCIA"); }*/
                    string lugEn = string.Empty;
                    foreach (RTV2_E d in ticket.Det2.Where(x => x.Verificar == "on"))
                    {
                        if (lugEn != string.Empty)
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
                    string lugEn = string.Empty;
                    foreach (RTV2_E d in ticket.Det2.Where(x => x.Verificar == "on"))
                    {
                        if (lugEn != string.Empty)
                        {
                            if (!lugEn.Equals("ALMACÉN N°6")) { throw new Exception("LUGAR DE ENTREGA DEBE SER ALM06"); }
                            if (d.LugarDeEntrega.Equals(lugEn)) { }
                            else { throw new Exception("NO COINCIDEN LOS LUGARES DE ENTREGA EN DETALLE"); }
                        }
                        else
                        {
                            lugEn = d.LugarDeEntrega;
                            if (!lugEn.Equals("ALMACÉN N°6")) { throw new Exception("LUGAR DE ENTREGA DEBE SER ALM06"); }
                        }
                    }
                }
                else if (ticket.LugarDestino.Equals("Domicilio"))
                {
                    if (string.IsNullOrEmpty(ticket.Zona)) { throw new Exception("DEBE EXISTIR UNA ZONA"); }
                    if (string.IsNullOrEmpty(ticket.DirDestino)) { throw new Exception("DEBE LLENAR DIRECCION DESTINO"); }
                    if (ticket.Det3 != null && ticket.Det3.Count >= 2 && !string.IsNullOrEmpty(ticket.Det3[1].Calle) && ticket.Det3[1].Calle.Length > 200) { throw new Exception("DirDestino2 EXCEDE EL LÍMITE DE CARACTERES PERMITIDOS"); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].NombrePer)) { throw new Exception("DEBE LLENAR NOMBRE"); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].TipoDocPer)) { throw new Exception("DEBE LLENAR TIPO DE DOCUMENTO PER"); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].DocPer)) { throw new Exception("DEBE LLENAR EL NRODOCUMENTO PER"); }
                    if (string.IsNullOrEmpty(ticket.Det1[0].TelfPer)) { throw new Exception("DEBE LLENAR TELEFONO"); }
                    string lugEn = string.Empty;
                    foreach (RTV2_E d in ticket.Det2.Where(x => x.Verificar == "on"))
                    {
                        if (lugEn != string.Empty)
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
                    string lugEn = string.Empty;
                    foreach (RTV2_E d in ticket.Det2.Where(x => x.Verificar == "on"))
                    {
                        if (lugEn != string.Empty)
                        {
                            if (!lugEn.Equals("ALMACÉN N°1")) { throw new Exception("LUGAR DE ENTREGA DEBE SER ALM01"); }
                            if (d.LugarDeEntrega.Equals(lugEn)) { }
                            else { throw new Exception("NO COINCIDEN LOS LUGARES DE ENTREGA EN DETALLE"); }
                        }
                        else
                        {
                            lugEn = d.LugarDeEntrega;
                            if (!lugEn.Equals("ALMACÉN N°1")) { throw new Exception("LUGAR DE ENTREGA DEBE SER ALM01"); }
                        }
                    }
                }
            }
            if (ticket.Det3 != null && ticket.Det3.Count >= 1)
            {
                if (ticket.Det3[0].Ubigeo > 0)
                {
                    if (string.IsNullOrEmpty(ticket.Det3[0].Calle)) { throw new Exception("DEBE LLENAR DIRECCION DESTINO"); }
                }
            }

            if (ticket.MontoTotal <= 0) { throw new Exception("NO PUEDE REGISTAR MONTO TOTAL EN 0 o negativo"); }
            if (ticket.MontoFinal <= 0) { throw new Exception("NO SE PUEDE REGISTRAR MONTOFINAL EN 0 o negativo"); }
            if (ticket.TiempoEntrega.ToString() == "" || ticket.TiempoEntrega == null) { throw new Exception("DEBE LLENAR TIEMPO DE ENTREGA"); }

            if (ticket.Det1 != null && ticket.Det1.Count > 0)
            {
                if (!string.IsNullOrEmpty(ticket.Det1[0].TipoDocPer) && ticket.Det1[0].TipoDocPer.Equals("DNI") && (ticket.Det1[0].DocPer.Length != 8)) { throw new Exception("EL DNIPER DEBE TENER 8 DIGITOS"); }
                if (!string.IsNullOrEmpty(ticket.Det1[0].TipoDocPer) && ticket.Det1[0].TipoDocPer.Equals("RUC") && (ticket.Det1[0].DocPer.Length != 11)) { throw new Exception("EL RUCPER DEBE TENER 11 DÍGITOS"); }
                if (!string.IsNullOrEmpty(ticket.Det1[0].TipoDocPer) && ticket.Det1[0].TipoDocPer.Equals("CE") && (ticket.Det1[0].DocPer.Length != 9)) { throw new Exception("EL CARNE EXTRANJERIAPER DEBE TENER 9 DÍGITOS"); }
                if (!string.IsNullOrEmpty(ticket.Det1[0].TelfPer) && ticket.Det1[0].TelfPer.Length != 9) { throw new Exception("EL TELEFONO DEBE TENER 9 DÍGITOS"); }

                if (string.IsNullOrEmpty(ticket.Det1[0].TipoDocPer) &&
                string.IsNullOrEmpty(ticket.Det1[0].DocPer) &&
                string.IsNullOrEmpty(ticket.Det1[0].TelfPer) &&
                string.IsNullOrEmpty(ticket.Det1[0].NombrePer)) { ticket.Det1 = null; }
            }

            if (ticket.Flete < 0) { throw new Exception("NO PUEDE REGISTAR FLETE NEGATIVO"); }
            if (ticket.DeudaCliente < 0) { throw new Exception("NO PUEDE REGISTAR DeudaCliente NEGATIVO"); }
            if (ticket.GastoEnvio < 0) { throw new Exception("NO PUEDE REGISTAR GastoEnvio NEGATIVO"); }
            if (ticket.DeudaEmpresa < 0) { throw new Exception("NO PUEDE REGISTAR DeudaEmpresa NEGATIVO"); }
            if (ticket.DescuentoNC < 0) { throw new Exception("NO PUEDE REGISTAR DescuentoNC NEGATIVO"); }
            if (ticket.MontoTotal != CalcularMontos(ticket).MontoTotal) { throw new Exception("EL CALCULO DEL MONTOTOTAL TUVO UN ERROR"); }
            if (ticket.MontoFinal != CalcularMontos(ticket).MontoFinal) { throw new Exception("EL CALCULO DEL MONTOFINAL TUVO UN ERROR"); }
            if (IdRol == 0)
            {
                if (ticket.Det5 != null && ticket.Det5.Count > 0)
                {
                    int idReg = ticket.Det5[0].IdReg;
                    if (idReg > 0)
                    {
                        OREG_E objRegalo = oregN.buscarRegalo(idReg);
                        if (ticket.Det5[0].RegCant <= 0) { throw new Exception("Debe llenar cantidades para el regalo"); }
                        else if (!oclrN.comprobarDispCliReg(new CLR1_E { IdReg = ticket.Det5[0].IdReg, CardCode = ticket.CardCode, Cantidad = ticket.Det5[0].RegCant }))
                        { throw new Exception("El cliente no tiene saldo regalo disponible"); }
                        if (objRegalo.Estado == "Inactivo") { throw new Exception("No puede dar regalos inactivos"); }
                        if (objRegalo.StockDisp < ticket.Det5[0].RegCant) { throw new Exception("No hay Stock disponible del regalo"); }
                    }
                    else if (ticket.Det5[0].RegCant > 0) { throw new Exception("Debe llenar cantidad de regalo"); }
                }
            }
        }
        public ORTV_E obtenerTicket(int DocEntry)
        {
            return ticketV.obtenerTicket(DocEntry);
        }
        public int editarTicket(int DocEntry, ORTV_E ticket)
        {
            ORTV_E t = ticketV.obtenerTicket(DocEntry);
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
                        var TkPrincipal = obtenerTicket(Convert.ToInt32(det7.DocNumVinc - 2000000000));
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
            return ticketV.editarTicket(DocEntry, ticket);
        }
        public int editarVisibilidadTicket(int DocEntry)
        {
            return ticketV.editarVisibilidadTicket(DocEntry);
        }
        public int registrarImpresionTicket(int DocEntry,string Operario)
        {
            return ticketV.registrarImpresionTicket(DocEntry,Operario);
        }
        
        public int cancelarTicket(int DocEntry, string Operario, int IdRol)
        {
            ORTV_E t = ticketV.obtenerTicket(DocEntry);
            bool continuarCancelarTicket = false;
            // Cuando el ticket esta en "SEPARADO" o "ABIERTO" solo lo podran cancelar Rol (Sup Ventas,Op Ventas)
            if (t.Estado.Equals("SEPARADO") || t.Estado.Equals("ABIERTO"))
            {
                if (IdRol == 6 || IdRol == 7) { continuarCancelarTicket = true; }
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

            if (continuarCancelarTicket == false) { throw new Exception("NO SE PUEDE CANCELAR EL TICKET N° " + t.DocNum + " POR SU ESTADO " + t.Estado); }
            if (t.EstadoPago != null && t.EstadoPago.Equals("PAGADO")) { throw new Exception("NO PUEDE CANCELAR UN TICKET PAGADO N°" + t.DocNum); }
            if (t.Estado.Equals("CANCELADO")) { throw new Exception("EL TICKET YA SE ENCUENTRA CANCELADO N°" + t.DocNum); }
            if (t.Estado.Equals("ENTREGADO")) { throw new Exception("EL TICKET YA SE ENCUENTRA ENTREGADO N°" + t.DocNum + " NO SE PUEDE CANCELAR"); }
            return ticketV.cancelarTicket(DocEntry, t.Estado, Operario);
        }
        /*
         * Editar Ticket parámetros ilimitados
         */
        public int EditarTicketDesdeSeguimiento(Dictionary<string, Object> datos, string Request)
        {
            ORTV_E ticket = obtenerTicket(Convert.ToInt32(datos["docEntryTicket"]));

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

            return ticketV.EditarTicketDesdeSeguimiento(datos);
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
                    /*if (!string.IsNullOrEmpty(t.Operario) && (t.Operario == "05" || t.Operario == "07"))
					{
                        if (listaEstados.FirstOrDefault().Operacion != "INICIO PICKING" && listaEstados.FirstOrDefault().Operacion != "ANULAR FIN EMPACAR" )
                        {
                            throw new Exception("Solo puedes ANULAR INICIO PICKING  a un ticket con ultimo flujo INICIO PICKING O ANULAR FIN EMPACAR");
                        }
                    }
					else { */

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
                    if (listaEstados.FirstOrDefault().Operacion != "INICIO VERIFICAR" && listaEstados.FirstOrDefault().Operacion != "ANULAR FIN VERIFICAR" && listaEstados.FirstOrDefault().Operacion != "ANULAR INICIO EMPACAR")
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
                var to = obtenerTicket(t.DocEntry);
                if (!t.Estado.Equals("ENTREGADO")) { throw new Exception("Solo puedes ANULARENTREGADO a un ticket ENTREGADO"); }
                if (to.Det5 != null && to.Det5.Count >= 1)
                {
                    if ((to.Det5[0].IdReg > 0 || to.Det5[0].RegCant > 0) && to.Det5[0].RegEstado == "PENDIENTE") { throw new Exception("Solo puedes ANULAR ENTREGA a un ticket que no tenga regalo pendiente"); }
                }
            }
            return ticketV.editarSeguimientoTicket(Estado, DocEntry, t);
        }
        public int emitirGuia(int DocEntry, Usuario_E u)
        {
            ORTV_E t = obtenerTicket(DocEntry);
            if (t.Estado.Equals("CANCELADO") || t.Estado.Equals("ANULADO")) { throw new Exception("No puede emitir guia en este ticket."); }
            if (!t.EstadoFacturacion.Equals("PENDIENTE")) { throw new Exception("El ticket no puede emitir guias."); }
            return ticketV.emitirGuia(DocEntry, u);
        }
        public int revertirGuiasTicket(int DocEntry, String operario)
        {
            ORTV_E t = obtenerTicket(DocEntry);
            if (!t.EstadoFacturacion.Equals("GRE EMITIDA")) { throw new Exception("No se puede revertir el proceso de guias"); }
            return ticketV.revertirGuiasTicket(DocEntry, operario);
        }
        public int facturarTicket(int DocEntry, Usuario_E u)
        {
            Utilitarios uti = new Utilitarios(); Capa_Datos.Ventas_DAO.Tablas.OINV_D oinvD = new Capa_Datos.Ventas_DAO.Tablas.OINV_D();
            ORTV_E t = obtenerTicket(DocEntry);
            //validamos que existan facturas o boletas
            List<int> OrdenesSap = new List<int>();
            foreach (var ordr in t.Det2)
            {
                string query = $"SELECT \"DocEntry\" FROM {uti.schemaHana}ordr WHERE \"DocNum\" = '{ordr.NroSap}' AND \"CANCELED\" = 'N'";
                HanaConnection cn = new HanaConnection(uti.cadHana);
                try
                {
                    cn.Open();
                    HanaCommand cmd = new HanaCommand(query, cn);
                    cmd.CommandType = System.Data.CommandType.Text;
                    HanaDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    if (!dr.IsDBNull(0)) { OrdenesSap.Add(dr.GetInt32(0)); }
                    dr.Close();
                    cn.Close();
                }
                catch { cn.Close(); }
            }
            List<OINV_E> ComprobantesVinculados = new List<OINV_E>();
            foreach (int DocEntryO in OrdenesSap)
            {
                List<OINV_E> comprobantesPorOrden = oinvD.listadoComprobantesPorOrdr(DocEntryO);
                ComprobantesVinculados.AddRange(comprobantesPorOrden);
            }
            if (ComprobantesVinculados.Count == 0) { throw new Exception("Este ticket no tiene facturas o boletas relacionadas desde SAP"); }
            if (t.Estado.Equals("CANCELADO") || t.Estado.Equals("ANULADO")) { throw new Exception("No puede facturar en este ticket."); }
            if (!t.EstadoFacturacion.Equals("GRE EMITIDA")) { throw new Exception("El ticket no tiene guías emitidas"); }
            return ticketV.facturarTicket(DocEntry, u);
        }
        public int revertirFacturarTicket(int DocEntry, String operario)
        {
            ORTV_E t = obtenerTicket(DocEntry);
            if (!t.EstadoFacturacion.Equals("FACTURADO")) { throw new Exception("No se puede revertir el proceso de facturado"); }
            return ticketV.revertirFacturarTicket(DocEntry, operario);
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
            return ticketV.ListarRptAnalisisTickets(datosFiltro);
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
        //ATENCION AL CLIENTES
        public List<ORTV_E> ListarTicketsParaAtencion()
        {
            return ticketV.ListarTicketsParaAtencion();
        }
        //*********CALCULOS****************//
        public ORTV_E CalcularMontos(ORTV_E t)
        {
            return ticketV.CalcularMontos(t);
        }
        public List<Capa_Entidad.ReportesDigemid_ENT.OrdenDeVenta_E> obtenerOrdenDeVenta(int DocNum)
        {
            ReportesDigemid_NEG.DocumentosDig_N dN = new ReportesDigemid_NEG.DocumentosDig_N();
            return dN.ConsultarOrdenDeVenta(DocNum);
        }
        public string generaInfoListaClientes(string Fecha)
        {
            return ticketV.generaInfoListaClientes(Fecha);
        }
        public string generaInfoListaDirDestinos(string CardCode)
        {
            return ticketV.generaInfoListaDirDestinos(CardCode);
        }
        public string generaInfoListaOrdenesDeVenta(string fecha, string cardCode, int docNum)
        {
            return ticketV.generaInfoListaOrdenesDeVenta(fecha, cardCode, docNum);
        }
        public string generaInfoListaNotasDeCreditoV(string CardCode)
        {
            return ticketV.generaInfoListaNotasDeCreditoV(CardCode);
        }
        public string GuiasTicket(int DocEntry)
        {
            return ticketV.GuiasTicket(DocEntry);
        }

        public Tickets buscarTicket(int DocEntry)
        {
            return ticketV.buscarTicket(DocEntry);
        }
        public ORTV_E CalcularPesoTotal(ORTV_E t)
        {
            return ticketV.CalcularPesoTotal(t);
        }
        public List<TEMP_RRU01_E> GuiasRemisionSap(int DocEntry)
        {
            return ticketV.GuiasRemisionSap(DocEntry);
        }
        public List<Tickets> buscarVariosTickets(int[] arrDocNum)
        {
            return ticketV.buscarVariosTickets(arrDocNum);
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
                    ticketV.entregarMasivoTicket(DocEntry, ticket);
                }
            }
            else
            {
                if (ticket.Estado == "ENVIADO" && entregadoConRegalo == 0)
                {
                    ticketV.entregarMasivoTicket(DocEntry, ticket);
                }
            }

            return ticket;
        }

        public void editarTicketSup(int DocEntry, int idRol, ORTV_E ticket)
        {
            ORTV_E t = ticketV.obtenerTicket(DocEntry);
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
                        var TkPrincipal = obtenerTicket(Convert.ToInt32(det7.DocNumVinc - 2000000000));
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
            ticketV.editarTicketSup(DocEntry, ticket);
        }
        public List<RTV4_E> obtenerDet4Ticket(int DocEntry, int DocNum = 0)
        {
            return ticketV.obtenerDet4Ticket(DocEntry, DocNum);
        }
        }
}