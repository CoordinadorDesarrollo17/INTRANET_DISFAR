using Capa_Datos.AtencionCliente_DAO.TablasSql;
using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.AtencionCliente_ENT.ReportesSql;
using Capa_Entidad.AtencionCliente_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Capa_Negocio.AtencionCliente_NEG.TablasSql
{
    public class OSAT_N
    {
        OSAT_D osatD = new OSAT_D();

        public List<OSAT_E> ListarSolicitudes(OSAT_E filtro, bool fact)
        {
            return osatD.ListarSolicitudes(filtro, false, fact);
        }
        public List<OSAT_E> ListarSolicitudes2(OSAT_E filtro, bool fact)
        {
            return osatD.ListarSolicitudes(filtro, true, fact);
        }
        public List<Rpt_OSAT_E> ListarSolicitudesExcel(OSAT_E filtro)
        {
            return osatD.ListarSolicitudesExcel(filtro);
        }
        public string registrarNuevaSolicitud(OSAT_E obj)
        {
            validarNuevaSolicitud(obj);
            return osatD.registrarNuevaSolicitud(obj);
        }
        public OSAT_E buscarSolicitud(int DocEntry)
        {
            var solicitud = osatD.buscarSolicitud(DocEntry);

            return solicitud;
        }
        public string anularSolicitud(OSAT_E obj)
        {
            validarAnularSolicitud(obj);
            return osatD.anularSolicitud(obj);
        }
        public string editarSolicitud(OSAT_E obj)
        {
            validarEditarSolicitud(obj);
            return osatD.editarSolicitud(obj);
        }
        public string procesarSolicitud(OSAT_E obj)
        {
            validarProcesarSolicitud(obj);
            return osatD.procesarSolicitud(obj);
        }
        public string revertirProcesarSolicitud(OSAT_E obj)
        {
            validarRevertirProcesarSolicitud(obj);
            return osatD.revertirProcesarSolicitud(obj);
        }
        public string atenderSolicitud(OSAT_E obj)
        {
            validarAtenderSolicitud(obj);
            return osatD.atenderSolicitud(obj);
        }
        public string revertirAtenderSolicitud(OSAT_E obj)
        {
            validarRevertirAtenderSolicitud(obj);
            return osatD.revertirAtenderSolicitud(obj);
        }
        public string culminarSolicitud(OSAT_E obj)
        {
            validarCulminarSolicitud(obj);
            return osatD.culminarSolicitud(obj);
        }
        public string revertirCulminarSolicitud(OSAT_E obj)
        {
            validarRevertirCulminarSolicitud(obj);
            return osatD.revertirCulminarSolicitud(obj);
        }
        public string obtenerNroSolicitud(string Tipo)
        {
            return osatD.ObtenerNroSolicitud(Tipo);
        }
        public OSAT_E BuscarDatosTicket(int DocNumTicket)
        {
            return osatD.BuscarDatosTicket(DocNumTicket);
        }
        public Dictionary<int, string> BuscarAdjuntosOSAT(int DocEntry, int Linea)
        {
            return osatD.BuscarAdjuntosOSAT(DocEntry, Linea);
        }
        public List<OSAT_E> obtenerNotificadoCliente()
        {
            return osatD.obtenerNotificadoCliente();
        }
        public List<OSAT_E> obtenerNotificadoClienteDetalle(string cardName)
        {
            return osatD.obtenerNotificadoClienteDetalle(cardName);
        }
        public void ActualizarTicketSolucion(List<string> docNums, string ticketSolucion)
        {
            new OSAT_D().ActualizarTicketSolucion(docNums, ticketSolucion);
        }
        public List<Rpt_Regalos> ListarRegalosAplicados()
        {
            return osatD.ListarRegalosAplicados();
        }
        //validaciones
        public void validarNuevaSolicitud(OSAT_E obj)
        {
            ORTV_D ortv_D = new ORTV_D();
            ORTV_E ticket = ortv_D.ObtenerDatosCompletosTicket(obj.DocEntryTicket);

            if (obj.Tipo == null) { throw new Exception("De Elegir un tipo de atencion"); }
            else if (obj.Tipo == "Devolucion")
            {
                if (obj.DireccionRecojo == null && (!ticket.LugarDestino.Equals("Arriola") && !ticket.LugarDestino.Equals("Centro"))) { throw new Exception("Debe llenar direccion de recojo"); }
            }
            if (obj.Contacto == null && (!ticket.LugarDestino.Equals("Arriola") && !ticket.LugarDestino.Equals("Centro"))) { throw new Exception("Debe llenar el contacto"); }
            if (obj.Telefono == null && (!ticket.LugarDestino.Equals("Arriola") && !ticket.LugarDestino.Equals("Centro"))) { throw new Exception("Debe llenar el telefono"); }
            if (obj.DocNumTicket <= 0) { throw new Exception("De registrar un Nro de Ticket"); }
            if (obj.Det == null || obj.Det.Count == 0) { throw new Exception("La solicitud debe tener al menos 1 articulo"); }
            if (string.IsNullOrWhiteSpace(obj.Problema)) { throw new Exception("Debe elegir un problema"); }

            if (Convert.ToDateTime(ticket.FechaRegistro) >= Convert.ToDateTime("2024-03-04") && !string.IsNullOrWhiteSpace(obj.Tipo) && !obj.Tipo.Equals("Reclamo"))
            {
                if (string.IsNullOrWhiteSpace(obj.TipoVenta)) { throw new Exception("Debe elegir un tipo de venta"); }
                if (string.IsNullOrWhiteSpace(obj.CanalVenta)) { throw new Exception("Debe elegir un canal de venta"); }
            }

            foreach (SAT1_E o in obj.Det)
            {
                if (o.unitMsrF == null) { throw new Exception("Debe elegir unidad de medida en linea " + o.Linea); }
                if (o.QuantityF <= 0) { throw new Exception("Debe llenar una cantidad valida y positiva en linea " + o.Linea); }
                if (o.LineTotalF <= 0) { throw new Exception("El total de linea debe ser positivo en linea " + o.Linea); }
                if (o.unitMsrF == "P")
                {
                    if (Math.Round(o.LineTotalF, 2) != Math.Round((o.PriceAfVAT * o.QuantityF / o.NumPerMsr), 2)) { throw new Exception("Hubo un error al calcular el totalLinea en linea " + o.Linea); }
                }
                else if (o.unitMsrF == "C")
                {
                    if (Math.Round(o.LineTotalF, 2) != Math.Round((o.PriceAfVAT * o.QuantityF), 2)) { throw new Exception("Hubo un error al calcular el totalLinea en linea " + o.Linea); }
                }
                if (string.IsNullOrWhiteSpace(o.Problema)) { throw new Exception("Debe elegir un problema"); }
                if (string.IsNullOrWhiteSpace(o.TipoError)) { throw new Exception("Debe elegir un tipo de error"); }
                if (o.LineTotalF > (o.PriceAfVAT * o.Quantity)) { throw new Exception("La cantidad devuelta no puede superar a lo vendido en linea" + o.Linea); }
            }
            if (obj.Archivo != null)
            {
                if (obj.Archivo.Count >= 1 && obj.Archivo[0] != null)
                {
                    foreach (var arch in obj.Archivo)
                    {
                        if (!(arch.ContentType == "application/pdf" || arch.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" ||
                            arch.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" ||
                            arch.ContentType == "application/x-zip-compressed" ||
                            arch.ContentType == "application/zip" || arch.ContentType == "image/png" || arch.ContentType == "image/jpeg" || arch.ContentType == "image/jpg" ||
                            arch.ContentType == "application/octet-stream"))
                        {
                            throw new Exception("Formatos válidos: .png, .jpeg, .jpg, .zip, .rar, .docx, .xslx, .pdf" + arch.ContentType);
                        }
                        if (arch.ContentLength > 10485760) { throw new Exception("No puedes cargar un archivo superior a 10 Mb"); }
                    }
                }


            }
        }
        public void validarAnularSolicitud(OSAT_E obj)
        {
            OSAT_E bean = buscarSolicitud(obj.DocEntry);
            if (bean.Estado != "Registrado") { throw new Exception("Solo se puede anular en estado Registrado"); }
            if (obj.DocEntry == 0) { throw new Exception("Error al identificar el nro de solicitud"); }
        }
        public void validarEditarSolicitud(OSAT_E obj)
        {
            OSAT_E bean = buscarSolicitud(obj.DocEntry);
            if (bean.Estado == "Anulado") { throw new Exception("No se pueden hacer operaciones en solicitud en estado Anulado"); }
            if (bean.Estado != "Registrado") { throw new Exception("Solo se puede editar en estado Registrado"); }
            if (obj.DocEntry == 0) { throw new Exception("Error al identificar el nro de solicitud"); }
            if (obj.Det == null || obj.Det.Count == 0) { throw new Exception("La solicitud debe tener al menos 1 articulo"); }
            if (Convert.ToDateTime(bean.FechaRegistro) >= Convert.ToDateTime("2024-03-04") && !string.IsNullOrWhiteSpace(obj.Tipo) && !obj.Tipo.Equals("Reclamo"))
            {
                if (string.IsNullOrWhiteSpace(obj.TipoVenta)) { throw new Exception("Debe elegir un tipo de venta"); }
                if (string.IsNullOrWhiteSpace(obj.CanalVenta)) { throw new Exception("Debe elegir un canal de venta"); }
            }

            foreach (SAT1_E o in obj.Det)
            {
                if (o.unitMsrF == null) { throw new Exception("Debe elegir unidad de medida en linea " + o.Linea); }
                if (o.QuantityF <= 0) { throw new Exception("Debe llenar una cantidad valida y positiva en linea " + o.Linea); }
                if (o.LineTotalF <= 0) { throw new Exception("El total de linea debe ser positivo en linea " + o.Linea); }
                if (o.unitMsrF == "P")
                {
                    if (Math.Round(o.LineTotalF, 2) != Math.Round((o.PriceAfVAT * o.QuantityF / o.NumPerMsr), 2)) { throw new Exception("Hubo un error al calcular el totalLinea en linea " + o.Linea); }
                }
                else if (o.unitMsrF == "C")
                {
                    if (Math.Round(o.LineTotalF, 2) != Math.Round((o.PriceAfVAT * o.QuantityF), 2)) { throw new Exception("Hubo un error al calcular el totalLinea en linea " + o.Linea); }
                }
                if (bean.Tipo == "Reclamo")
                {
                    if (string.IsNullOrWhiteSpace(o.Problema)) { throw new Exception("Debe elegir un problema"); }
                    if (string.IsNullOrWhiteSpace(o.TipoError)) { throw new Exception("Debe elegir un tipo de error"); }
                }
                if (o.LineTotalF > (o.PriceAfVAT * o.Quantity)) { throw new Exception("La cantidad devuelta no puede superar a lo vendido en linea" + o.Linea); }
            }

        }
        public void validarProcesarSolicitud(OSAT_E obj)
        {
            OSAT_E bean = buscarSolicitud(obj.DocEntry);
            if (bean.Estado == "Anulado") { throw new Exception("No se pueden hacer operaciones en solicitud en estado Anulado"); }
            if (bean.Estado != "Registrado") { throw new Exception("Solo se puede procesar en estado Registrado"); }
            if (obj.DocEntry == 0) { throw new Exception("Error al identificar el nro de solicitud"); }
            if (string.IsNullOrWhiteSpace(obj.Factor)) { throw new Exception("De Elegir un factor"); }

            if (Convert.ToDateTime(bean.FechaRegistro) >= Convert.ToDateTime("2024-03-04") && !string.IsNullOrWhiteSpace(obj.Tipo) && !obj.Tipo.Equals("Reclamo"))
            {
                if (string.IsNullOrWhiteSpace(obj.TipoVenta)) { throw new Exception("Debe elegir un tipo de venta"); }
                if (string.IsNullOrWhiteSpace(obj.CanalVenta)) { throw new Exception("Debe elegir un canal de venta"); }
            }

            if (obj.Det == null || obj.Det.Count == 0) { throw new Exception("La solicitud debe tener al menos 1 articulo"); }
            foreach (SAT1_E o in obj.Det)
            {
                if (o.unitMsrF == null) { throw new Exception("Debe elegir unidad de medida en linea " + o.Linea); }
                if (o.QuantityF <= 0) { throw new Exception("Debe llenar una cantidad valida y positiva en linea " + o.Linea); }
                if (o.LineTotalF <= 0) { throw new Exception("El total de linea debe ser positivo en linea " + o.Linea); }
                if (o.unitMsrF == "P")
                {
                    if (Math.Round(o.LineTotalF, 2) != Math.Round((o.PriceAfVAT * o.QuantityF / o.NumPerMsr), 2)) { throw new Exception("Hubo un error al calcular el totalLinea en linea " + o.Linea); }
                }
                else if (o.unitMsrF == "C")
                {
                    if (Math.Round(o.LineTotalF, 2) != Math.Round((o.PriceAfVAT * o.QuantityF), 2)) { throw new Exception("Hubo un error al calcular el totalLinea en linea " + o.Linea); }
                }
                if (o.LineTotalF > (o.PriceAfVAT * o.Quantity)) { throw new Exception("La cantidad devuelta no puede superar a lo vendido en linea" + o.Linea); }
                if (string.IsNullOrWhiteSpace(o.TipoError)) { throw new Exception("Debe elejir un tipo de error en linea " + o.Linea); }
                if (bean.Tipo == "Reclamo")
                {
                    if (string.IsNullOrWhiteSpace(o.Problema)) { throw new Exception("Debe elegir un problema en linea " + o.Linea); }
                }
            }
            if (obj.Archivo != null)
            {
                /*if (!(obj.Archivo.ContentType == "application/pdf" ||
                    obj.Archivo.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" ||
                    obj.Archivo.ContentType == "application/x-zip-compressed" ||
                    obj.Archivo.ContentType == "application/zip" ||
                    obj.Archivo.ContentType == "application/octet-stream"))
                {
                    throw new Exception("Debe elegir un archivo valido .zip,.rar,.docx,.pdf");
                }
                if (obj.Archivo.ContentLength > 10485760) { throw new Exception("No puedes cargar un archivo superior a 10Mb"); }*/
            }
        }
        public void validarRevertirProcesarSolicitud(OSAT_E obj)
        {
            OSAT_E bean = buscarSolicitud(obj.DocEntry);
            if (bean.Estado == "Anulado") { throw new Exception("No se pueden hacer operaciones en solicitud en estado Anulado"); }
            if (obj.Estado != "Proceso") { throw new Exception("Solo se puede revertir en estado Proceso"); }
            if (obj.DocEntry == 0) { throw new Exception("Error al identificar el nro de solicitud"); }
        }
        public void validarAtenderSolicitud(OSAT_E obj)
        {
            if (!string.IsNullOrWhiteSpace(obj.ErrAlmOtrCom) && obj.Det != null)
            {
                foreach (var o in obj.Det)
                {
                    if (o.ErrorAlmacen == "OTROS")
                    {
                        o.ErrAlmOtrCom = obj.ErrAlmOtrCom;
                    }
                }
            }
            OSAT_E bean = buscarSolicitud(obj.DocEntry);
            if (bean.Estado == "Anulado") { throw new Exception("No se pueden hacer operaciones en solicitud en estado Anulado"); }
            if (bean.Estado != "Proceso") { throw new Exception("Solo se puede atender en estado Proceso"); }
            if (obj.DocEntry == 0) { throw new Exception("Error al identificar el nro de solicitud"); }
            if (string.IsNullOrWhiteSpace(obj.Factor)) { throw new Exception("Debe seleccionar un factor"); }
            if (string.IsNullOrWhiteSpace(obj.Resultado)) { throw new Exception("Debe seleccionar un resultado"); }
            if (string.IsNullOrWhiteSpace(obj.TipoSolucion)) { throw new Exception("Debe Elegir un tipo de solucion"); }
            if (string.IsNullOrWhiteSpace(obj.Solucion)) { throw new Exception("Debe registrar una solucion"); }
            if (Convert.ToDateTime(bean.FechaRegistro) >= Convert.ToDateTime("2024-03-04") && !string.IsNullOrWhiteSpace(obj.Tipo) && !obj.Tipo.Equals("Reclamo"))
            {
                if (string.IsNullOrWhiteSpace(obj.TipoVenta)) { throw new Exception("Debe elegir un tipo de venta"); }
                if (string.IsNullOrWhiteSpace(obj.CanalVenta)) { throw new Exception("Debe elegir un canal de venta"); }
            }

            foreach (SAT1_E o in obj.Det)
            {
                if (o.TareaFact == "N.C.")
                {
                    if (string.IsNullOrWhiteSpace(o.ComprobanteVinc)) { throw new Exception("El campo comprobante debe ser ingresado si hay una NC vinculada linea:" + o.Linea); }
                    if (string.IsNullOrWhiteSpace(o.AlmTransf)) { throw new Exception("El campo almacen debe ser seleccionado si hay una NC vinculada linea:" + o.Linea); }
                }

                if (o.NuevoPrecioArticulo != null && o.NuevoPrecioArticulo <= 0)
                    throw new Exception("El nuevo precio de artículo debe ser mayor a cero en la línea: " + o.Linea);

                if (!string.IsNullOrWhiteSpace(o.TipoError) && o.TipoError.Equals("ErrorAlmacen") && !string.IsNullOrWhiteSpace(obj.Resultado) && obj.Resultado.Equals("Procede") && string.IsNullOrWhiteSpace(o.ErrorAlmacen))
                    throw new Exception("Debe elegir un error de almacén");

                if (!string.IsNullOrWhiteSpace(o.TipoError) && o.TipoError.Equals("ErrorAlmacen") && !string.IsNullOrWhiteSpace(obj.Resultado) && obj.Resultado.Equals("Procede") && o.ErrorAlmacen == "OTROS" && string.IsNullOrWhiteSpace(o.ErrAlmOtrCom))

                    throw new Exception("Debe ingresar un comentario para 'Otros' en error de almacén en la línea " + o.Linea);

            }
          
        }
        public void validarRevertirAtenderSolicitud(OSAT_E obj)
        {
            OSAT_E bean = buscarSolicitud(obj.DocEntry);
            if (bean.Estado == "Anulado") { throw new Exception("No se pueden hacer operaciones en solicitud en estado Anulado"); }
            if (bean.Estado != "Atendido") { throw new Exception("Solo se puede revertir atencion si estado es Atendido"); }
            if (obj.DocEntry == 0) { throw new Exception("Error al identificar el nro de solicitud"); }

        }
        public void validarCulminarSolicitud(OSAT_E obj)
        {
            OSAT_E bean = buscarSolicitud(obj.DocEntry);
            if (bean.Estado == "Anulado") { throw new Exception("No se pueden hacer operaciones en solicitud en estado Anulado"); }
            if (bean.Estado != "Atendido") { throw new Exception("Solo se puede culminar en estado Atendido"); }
            if (obj.DocEntry == 0) { throw new Exception("Error al identificar el nro de solicitud"); }
            if (Convert.ToDateTime(bean.FechaRegistro) >= Convert.ToDateTime("2024-03-04") && !string.IsNullOrWhiteSpace(obj.Tipo) && !obj.Tipo.Equals("Reclamo"))
            {
                if (string.IsNullOrWhiteSpace(obj.TipoVenta)) { throw new Exception("Debe elegir un tipo de venta"); }
                if (string.IsNullOrWhiteSpace(obj.CanalVenta)) { throw new Exception("Debe elegir un canal de venta"); }
            }

            foreach (SAT1_E o in obj.Det)
            {
                if (o.TareaFact == "N.C.")
                {
                    if (string.IsNullOrWhiteSpace(o.ComprobanteFin)) { throw new Exception("El campo documento debe ser ingresado si hay una NC vinculada linea:" + o.Linea); }
                }

                if (!string.IsNullOrWhiteSpace(bean.TipoSolucion) && bean.TipoSolucion.Equals("NotaDeCredito") && (o.NCSAP <= 1 || o.NCSAP == null)) { throw new Exception("NC SAP ingresada no es válida"); }
            }
        }
        public void validarRevertirCulminarSolicitud(OSAT_E obj)
        {
            OSAT_E bean = buscarSolicitud(obj.DocEntry);
            if (bean.Estado == "Anulado") { throw new Exception("No se pueden hacer operaciones en solicitud en estado Anulado"); }
            if (bean.Estado != "Culminado") { throw new Exception("Solo se puede revertir culminacion si estado es Culminado"); }
            if (obj.DocEntry == 0) { throw new Exception("Error al identificar el nro de solicitud"); }

        }
    }
}