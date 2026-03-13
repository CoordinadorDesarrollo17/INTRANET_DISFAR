using Capa_Datos.Rutas_DAO.TablasSql;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.Rutas_ENT.ReportesSql;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Capa_Negocio.Ventas_NEG.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Capa_Negocio.Ventas_NEG.Tablas;
using Capa_Entidad.Ventas_ENT.Tablas;

namespace Capa_Negocio.Rutas_NEG.TablasSql
{
    public class ORRU_N
    {
        ORRU_D orruD = new ORRU_D();

        public List<ORRU_E> Listar(ORRU_E o)
        {
            return orruD.Listar(o);
        }
        public List<OWTQ_E> listarGuiasTraslado(string Origen)
        {
            return orruD.listarGuiasTraslado(Origen);
        }
        public int NuevaHojaDeReparto(ORRU_E o)
        {
            validarNuevaHojaDeRepartoOTransferencia(o);
            return orruD.NuevaHojaDeReparto(o);
        }
        public int EditarHojaDeReparto(ORRU_E o)
        {
            validarDatosEncabezadoRuta(o);
            return orruD.EditarHojaDeReparto(o);
        }
        public int AnularOrdenDeRuta(int DocEntry, string OpRegistro)
        {
            ORRU_E o = orruD.obtenerOrdenDeRuta(DocEntry);
            if (!o.Estado.Equals("CREADO")) { throw new Exception("Solo puede anular en Estado Creado"); }
            
            // ✅ PERMITIR ANULAR TANTO TRANSFERENCIAS (TA) COMO DEVOLUCIONES (DE)
            if (!o.TipoRuta.Equals("TA") && !o.TipoRuta.Equals("DE")) 
            { 
                throw new Exception("Solo puede anular rutas de tipo Transferencia (TA) o Devolución (DE)"); 
            }
            
            return orruD.AnularOrdenDeRuta(DocEntry, OpRegistro);
        }
        public ORRU_E obtenerOrdenDeRuta(int DocEntry)
        {
            return orruD.obtenerOrdenDeRuta(DocEntry);
        }
        public void validarNuevaHojaDeRepartoOTransferencia(ORRU_E o)
        {
            if (string.IsNullOrWhiteSpace(o.TipoRuta)) { throw new Exception("No lleno tipo de ruta encabezado"); }
            //Validacion solo para agencia courier
            if (o.TipoRuta != "TA" && o.TipoRuta != "VG")
            {
                if (string.IsNullOrWhiteSpace(o.TransDesc)) { throw new Exception("Debe elegir un conductor"); }
                if (string.IsNullOrWhiteSpace(o.VehiculoCod)) { throw new Exception("Debe elegir un vehiculo"); }
                if (string.IsNullOrWhiteSpace(o.CopilDesc)) { throw new Exception("El documento debe tener copiloto 1"); }
            }
            //if (o.TipoRuta == "VG")
            //{
            //    if (string.IsNullOrWhiteSpace(o.ProvDesc)) { throw new Exception("Debe elegir un conductor o proveedor de envío"); }
            //}
            //Validaciones para cualquier tipo de ruta
            if (o.FechaCont == null) { throw new Exception("No eligió FechaContabilizacion"); }

            //validaciones para tipos de ruta distinto a courier y agencia
            if (o.TipoRuta != "VG" && o.TipoRuta != "AC" && o.TipoRuta != "DE" && (string.IsNullOrWhiteSpace(o.AlmOrigenCod) || string.IsNullOrWhiteSpace(o.AlmOrigenDesc)))
            {
                throw new Exception("No eligió almacén origen");
            }
            if (o.TipoRuta != "VG" && o.TipoRuta != "AC" && (string.IsNullOrWhiteSpace(o.AlmDestinoCod) || string.IsNullOrWhiteSpace(o.AlmDestinoDesc)))
            {
                throw new Exception("No eligió almacén destino");
            }
            if (o.TipoRuta != "DE" && o.TiempoPac == null) { throw new Exception("Debe haber tiempo pactado"); }

            // VALIDACIONES ESPECÍFICAS PARA SOLICITUD DE DEVOLUCIÓN (DE)
            if (o.TipoRuta == "DE")
            {
                // Validar que exista al menos un detalle (órdenes de devolución)
                if (o.DetRRU0 == null || o.DetRRU0.Count <= 0)
                {
                    throw new Exception("Debe agregar al menos una orden de devolución al detalle");
                }

                // Validar almacén destino específico para devoluciones
                if (string.IsNullOrWhiteSpace(o.AlmDestinoCod) || string.IsNullOrWhiteSpace(o.AlmDestinoDesc))
                {
                    throw new Exception("Debe seleccionar un almacén destino para la devolución");
                }

                // Validar cada detalle de devolución
                foreach (RRU0_E d in o.DetRRU0)
                {
                    if (d.DocEntryTicket <= 0)
                    {
                        throw new Exception($"La orden en línea {d.Linea} no tiene un DocEntry válido");
                    }

                    if (string.IsNullOrWhiteSpace(d.DocNumTicket.ToString()) || d.DocNumTicket <= 0)
                    {
                        throw new Exception($"La orden en línea {d.Linea} no tiene un número de ticket válido");
                    }

                    // ✅ VALIDAR CAJAS (que viene como Bultos desde la vista)
                    if (d.Cajas <= 0)
                    {
                        throw new Exception($"La orden {d.DocNumTicket} en línea {d.Linea} debe tener al menos 1 bulto/caja");
                    }

                    // Validar que tenga guías
                    if (string.IsNullOrWhiteSpace(d.Guias))
                    {
                        throw new Exception($"La orden {d.DocNumTicket} en línea {d.Linea} no tiene guías asignadas");
                    }

                    // Validar formato conductor/placa si aplica
                    if (!string.IsNullOrWhiteSpace(d.ConducYPlaca) && d.ConducYPlaca.Split('/').Length < 2)
                    {
                        throw new Exception($"La orden {d.DocNumTicket} en línea {d.Linea} tiene formato incorrecto de Conductor/Placa");
                    }
                }
            }

            //Validaciones para todo tipo de ruta menos TA Transferencia entre almacenes
            if (!o.TipoRuta.Equals("TA") && !o.TipoRuta.Equals("DE"))
            {
                if (o.DetRRU0.Count <= 0) { throw new Exception("Debe haber detalles de documento"); }
                foreach (RRU0_E d in o.DetRRU0)
                {
                    if (d.DocEntryTicket > 0)
                    {
                        ORTV_E t = new Capa_Negocio.Ventas_NEG.TablasSql.ORTV_N().ObtenerDatosCompletosTicket(d.DocEntryTicket);
                        if (string.IsNullOrWhiteSpace(t.EstadoFacturacion) || t.EstadoFacturacion.Equals("PENDIENTE")) { throw new Exception("El ticket en linea " + d.Linea + " no tiene guías emitida, retirar de la tabla."); }
                        if (string.IsNullOrWhiteSpace(d.Guias))
                        {
                            throw new Exception("El ticket no tiene guias en linea " + d.Linea);
                        }
                        else
                        {
                            if (!Regex.IsMatch(d.Guias, @"\d"))
                            {
                                throw new Exception("El ticket no tiene guias en linea " + d.Linea);
                            }
                        }
                        if (o.TipoRuta == "AC")
                        {
                            if (t.Estado != "PESADO") { throw new Exception("Solo puedes agregar tickets en estado PESADO " + d.Linea); }
                        }
                        else
                        {
                            if (t.Estado != "EMPACADO" && t.Estado != "PESADO") { throw new Exception("Ticket en linea " + d.Linea + " no puede ser agregado, estado diferente a EMPACADO o PESADO"); }
                        }
                        if (t.LugarDestino == "EXTERNO" || t.LugarDestino == "Agencia Courier" || t.LugarDestino == "LOCAL")
                        {
                            if (t.TipoVenta == "Normal")
                            {
                                if (t.EstadoPago != "PAGADO") { throw new Exception("El ticket normal de Externo  o Local debe estar pagado en linea " + d.Linea); }
                            }
                        }
                        //buscar vinculados solo en caso de Domicilio para validar que todos son enviados juntos en la misma hoja de ruta.
                        if (t.LugarDestino == "LOCAL")
                        {
                            var listTicketsVinculados = new Capa_Negocio.Ventas_NEG.TablasSql.ORTV_N().BuscarVinculados(t.DocEntry, t.DocNum);
                            foreach (string DocNumVinculado in listTicketsVinculados)
                            {
                                //buscamos que el ticket vinculado se encuentre en la hoja de ruta creandose
                                var ticketEncontrado = o.DetRRU0.FirstOrDefault(detalle => Convert.ToString(detalle.DocNumTicket) == DocNumVinculado);
                                if (ticketEncontrado == null)
                                {
                                    throw new Exception("El ticket: " + t.DocNum + " no ha agregado a su ticket vinculado: " + DocNumVinculado + ". Esto es necesario para crear la hoja de ruta.");
                                }
                            }
                        }

                        //Incrusta la cantidad de cajas correcta de acuerdo al ticket actualizado
                        d.Cajas = t.Cajas;
                    }
                }
            }
            //Validaciones exclusivas para Transferencia entre almacenes ( tambien son parte de las hojas de ruta )
            else if(o.TipoRuta.Equals("TA"))
            {
                o.DetRRU1 = o.DetRRU1.Where(det => det.Guia != null).ToList();
                foreach (RRU1_E r in o.DetRRU1)
                {
                    if (r.Guia != null && r.Guia.Length > 0)
                    {
                        bool ExistePza = false;
                        if (r.ListaRRU11 != null && r.ListaRRU11.Count > 0)
                        {
                            foreach (RRU11_E r12 in r.ListaRRU11)
                            {
                                if (r12.UnitMed.Equals("PZA")) { ExistePza = true; }
                                if (!r12.UnitMed.Equals("PZA") && r12.Cajas <= 0)
                                {
                                    throw new Exception("La guia no tiene cajasMaster en posición :" + r12.Linea);
                                }
                            }
                        }
                        if (ExistePza == false)
                        {
                            if (!(r.Cajas == r.ListaRRU11.Sum(y => y.Cajas))) { throw new Exception("El total de cajasMaster no coincide en línea: " + r.Linea); }
                            if (r.Cajas <= 0) { throw new Exception("La guia no tiene cajas en línea: " + r.Linea); }
                        }
                        else if (ExistePza == true)
                        {
                            if (r.Cajas <= 0) { r.Cajas = 1; }
                        }
                    }
                }

            }
        }
        public void validarDatosEncabezadoRuta(ORRU_E o)
        {
            // Solo edita los datos de encabezado de la hoja de ruta
            ORRU_E orruE = obtenerOrdenDeRuta(o.DocEntry);
            if (orruE.Estado != "CREADO") { throw new Exception("Solo puede editar un documento creado"); }
            if (string.IsNullOrWhiteSpace(o.TipoRuta)) { throw new Exception("No lleno tipo de ruta encabezado"); }

            if (o.FechaCont == null) { throw new Exception("No eligió fecha de contabilizacion"); }

            // Validaciones para tipos de ruta distinto a agencia ("VG")
            // CAMBIO 1: Excluir "DE" de la validación de Almacén Origen
            if (o.TipoRuta != "VG" && o.TipoRuta != "DE" && (string.IsNullOrWhiteSpace(o.AlmOrigenCod) || string.IsNullOrWhiteSpace(o.AlmOrigenDesc)))
            {
                throw new Exception("No eligió almacén origen");
            }

            // El almacén destino se mantiene obligatorio (a donde se devuelve la mercadería)
            if (o.TipoRuta != "VG" && (string.IsNullOrWhiteSpace(o.AlmDestinoCod) || string.IsNullOrWhiteSpace(o.AlmDestinoDesc)))
            {
                throw new Exception("No eligió almacén destino");
            }

            // CAMBIO 2: Tiempo pactado obligatorio SOLO si NO es "DE"
            if (o.TipoRuta != "DE" && o.TiempoPac == null)
            {
                throw new Exception("Debe haber tiempo pactado");
            }

            if (o.TipoRuta != "TA" && o.TipoRuta != "VG")
            {
                if (string.IsNullOrWhiteSpace(o.Placa)) { throw new Exception("El documento debe tener placa"); }
                // Todos los casos distintos, donde se escoge los valores de un desplegable
                if (string.IsNullOrWhiteSpace(o.TransDesc)) { throw new Exception("Debe elegir un conductor"); }
                if (string.IsNullOrWhiteSpace(o.VehiculoCod)) { throw new Exception("Debe elegir un vehiculo"); }
                if (string.IsNullOrWhiteSpace(o.CopilDesc)) { throw new Exception("El documento debe tener copiloto 1"); }
            }
        }
        public ORRU_E obtenerOrdenDeRutaTicket(int DocEntryTicket)
        {
            return orruD.obtenerOrdenDeRutaTicket(DocEntryTicket);
        }
        public void IniciarReparto(ORRU_E o)
        {
            if (o.Estado != "CREADO") { throw new Exception("El reparto debe estar CREADO"); }
            if (o.DetRRU0 != null && o.DetRRU0.Count > 0)
            {
                foreach (RRU0_E a in o.DetRRU0.Where(x => x.Estado != "LIBERADO"))
                {
                    if (a.Ticket?.EstadoFacturacion != "FACTURADO") { throw new Exception($"El ticket {a.DocNumTicket} no se encuentra FACTURADO"); }
                    if (a.Estado != "PREENVIO") { throw new Exception("El ticket " + a.DocNumTicket + " no esta en PREENVIO"); }
                }
            }
            if (o.DetRRU1 != null && o.DetRRU1.Count > 0)
            {
                foreach (RRU1_E a in o.DetRRU1.Where(x => x.Estado != "LIBERADO" && x.Estado != "ANULADO"))
                {
                    if (a.Estado != "PREENVIO") { throw new Exception("Todos los tickets no estan como PREENVIO"); }
                    //if (!(a.TempI1 >= 15 && a.TempI1 <= 25)) { throw new Exception("Temp1 Inicial no cumple con el rango valido (mayor o igual a 15 y menor o igual a 25)"); }
                    //if (!(a.TempI2 >= 15 && a.TempI2 <= 25)) { throw new Exception("Temp2 Inicial no cumple con el rango valido (mayor o igual a 15 y menor o igual a 25)"); }
                }
            }
            orruD.IniciarReparto(o);
        }
        public void TerminarReparto(ORRU_E o)
        {
            // 1. VALIDACIÓN DE ESTADO SEGÚN TIPO DE RUTA
            if (o.TipoRuta == "DE")
            {
                // Para Devoluciones, el estado previo debe ser "DEVUELTO" (luego del botón recibir)
                if (o.Estado != "DEVUELTO")
                {
                    throw new Exception("La devolución debe estar recibida (Estado DEVUELTO) antes de terminar.");
                }
            }
            else
            {
                // Para Rutas Normales (VC, VD, etc), el estado debe ser "ENVIADO"
                if (o.Estado != "ENVIADO")
                {
                    throw new Exception("El documento no está en estado ENVIADO.");
                }
            }

            // 2. VALIDACIÓN DE TICKETS (SOLO PARA RUTAS NORMALES)
            // Si es DE, nos saltamos todo este bloque if
            if (o.TipoRuta != "DE")
            {
                if (o.DetRRU0 != null)
                {
                    foreach (RRU0_E a in o.DetRRU0.Where(x => x.Estado != "LIBERADO" && x.Estado != "ANULADO"))
                    {
                        if (a.Estado != "ENTREGADO")
                        {
                            throw new Exception("El ticket " + a.DocNumTicket + " no esta ENTREGADO");
                        }
                    }
                }

                if (o.DetRRU1 != null)
                {
                    foreach (RRU1_E a in o.DetRRU1.Where(x => x.Estado != "LIBERADO"))
                    {
                        if (a.Estado != "ENTREGADO")
                        {
                            throw new Exception("Todos los tickets no estan como ENTREGADO");
                        }
                    }
                }
            }

            // 3. EJECUTAR EL CIERRE EN LA BASE DE DATOS
            // (Llama al SP con 'UTR' que pone el estado TERMINADO)
            orruD.TerminarReparto(o);
        }
        public string infoListaProductosOWTQ(string guia, int linea, string Origen)
        {
            return orruD.infoListaProductosOWTQ(guia, linea, Origen);
        }
        public List<Rpt_TempHumed_E> RptTempHumed(string Placa, string FechaTerEn, string Serie)
        {
            return orruD.RptTempHumed(Placa, FechaTerEn, Serie);
        }
        public List<ORRU_E.RptRutas> ReporteHojasRuta(ORRU_E o)
        {
            return orruD.ReporteHojasRuta(o);
        }

        public List<ORRU_E.RptRutasT> ReporteTransferencias(ORRU_E o)
        {
            return orruD.ReporteTransferencias(o);
        }
        public List<RptPesaje_E> ListarRptPesaje(FiltroRptPesaje datosFiltro)
        {
            return orruD.ListarRptPesaje(datosFiltro);
        }
        // Nuevo: actualizar hora de llegada (HoraEntrega) en RRU0 o RRU1
        public bool ActualizarHoraLlegada(int docEntry, int docNum, string nuevaHora)
        {
            if (string.IsNullOrWhiteSpace(nuevaHora) || !Regex.IsMatch(nuevaHora, @"^\d{2}:\d{2}(:\d{2})?$"))
                throw new Exception("Formato de hora inválido (HH:mm o HH:mm:ss)");
            if (nuevaHora.Length == 5) nuevaHora += ":00"; // normalizar
            return orruD.ActualizarHoraLlegada(docEntry, docNum, nuevaHora);
        }
        public List<ORRU_E.OrdenDevolucionHana> ListarOrdenesDevolucionDesdeHana(string cardCode, string fecha)
        {
            return orruD.ListarOrdenesDevolucionDesdeHana(cardCode, fecha);
        }

        public List<dynamic> ListarClientesPorFechaDesdeHana(string fecha)
        {
            return new ORRU_D().ListarClientesPorFechaDesdeHana(fecha);
        }

        public void RecibirDevolucion(int DocEntry, string opRegistro)
        {
            // 1. Obtener la ruta completa
            ORRU_E ruta = obtenerOrdenDeRuta(DocEntry);

          // 2. Validar que sea una devolución
  if (ruta.TipoRuta != "DE")
            {
    throw new Exception("Solo se pueden recibir devoluciones (TipoRuta = DE)");
    }

// 3. Validar estado previo
  if (ruta.Estado != "ENVIADO")
    {
  throw new Exception("La devolución debe estar en estado ENVIADO para poder recibirse");
            }

         // 4. Cambiar estado a "DEVUELTO" usando el SP con URD
          orruD.RecibirDevolucion(DocEntry, opRegistro);
        }

        public void TerminarDevolucion(int DocEntry, string opRegistro)
        {
 // 1. Obtener la ruta completa
         ORRU_E ruta = obtenerOrdenDeRuta(DocEntry);

     // 2. Validar que sea una devolución
       if (ruta.TipoRuta != "DE")
    {
    throw new Exception("Solo se pueden terminar devoluciones (TipoRuta = DE)");
            }

       // 3. Validar estado previo
            if (ruta.Estado != "DEVUELTO")
            {
         throw new Exception("La devolución debe estar RECIBIDA (Estado DEVUELTO) para poder terminarla");
    }

            // 4. Asignar operario y terminar
 ruta.Operario = opRegistro;
TerminarReparto(ruta);
        }
        public List<ORRU_E.RptRutasExcel> ObtenerRptRutasExcel(int docEntry)
        {
            var ReporteExcel = orruD.ObtenerRptRutasExcel(docEntry);

            var ordrN = new Capa_Negocio.Ventas_NEG.Tablas.ORDR_N();
            var negtik = new Capa_Negocio.Ventas_NEG.TablasSql.ORTV_N();
            var oinvNeg = new OINV_N();

            foreach (var item in ReporteExcel)
            {
                var obj = negtik.ObtenerDatosCompletosTicket(item.DocEntry);
                List<string> FB = new List<string>();

                if (obj?.Det2 != null)
                {
                    foreach (var orden in obj.Det2)
                    {
                        var Ordenes = ordrN.listadoOrdenesDeVenta(
                            new Capa_Entidad.Ventas_ENT.Tablas.ORDR_E { DocNum = orden.NroSap }, true);

                        if (Ordenes.Count > 0)
                        {
                            foreach (var x in Ordenes[0].ComprobantesVinculados)
                            {
                                FB.Add(x.NumAtCard);
                            }
                        }
                    }
                }

                List<OINV_E> lista = new List<OINV_E>();
                foreach (var o in FB)
                {
                    if (!string.IsNullOrWhiteSpace(o) && o.Contains("F"))
                    {
                        var factura = oinvNeg.listadoFacturasDeVenta(
                            new OINV_E { NumAtCard = o }).FirstOrDefault();
                        if (factura != null) lista.Add(factura);
                    }
                    else if (!string.IsNullOrWhiteSpace(o) && o.Contains("B"))
                    {
                        var boleta = oinvNeg.listadoBoletasDeVenta(
                            new OINV_E { NumAtCard = o }).FirstOrDefault();
                        if (boleta != null) lista.Add(boleta);
                    }
                }

                // Agrupa y asigna los números de factura/boleta al campo Factura
                var facturas = lista
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.NumAtCard))
                    .GroupBy(x => x.NumAtCard)
                    .Select(g => g.First().NumAtCard)
                    .ToList();

                item.Factura = string.Join(", ", facturas);
            }

            return ReporteExcel;
        }

        public List<dynamic> ObtenerRptRutasExcelGuiaProveedor(int docEntry) { return new ORRU_D().ObtenerRptRutasExcelGuiaProveedor(docEntry); }
    }
}