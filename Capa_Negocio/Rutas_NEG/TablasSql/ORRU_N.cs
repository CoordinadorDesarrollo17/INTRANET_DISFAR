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
        public int CrearOrdenDeRuta(ORRU_E o)
        {
            validarDatosOrdenDeRuta(o);
            return orruD.CrearOrdenDeRuta(o);
        }
        public int EditarEncabezadoOrdenDeRuta(ORRU_E o)
        {
            validarDatosEncabezadoRuta(o);
            return orruD.EditarOrdenDeRuta(o);
        }
        public int AnularOrdenDeRuta(int DocEntry, string OpRegistro)
        {
            ORRU_E o = orruD.obtenerOrdenDeRuta(DocEntry);
            if (!o.Estado.Equals("CREADO")) { throw new Exception("Solo puede anular en Estado Creado"); }
            if (!o.TipoRuta.Equals("TA")) { throw new Exception("Solo puede anular ruta TA"); }
            return orruD.AnularOrdenDeRuta(DocEntry, OpRegistro);
        }
        public ORRU_E obtenerOrdenDeRuta(int DocEntry)
        {
            return orruD.obtenerOrdenDeRuta(DocEntry);
        }
        public void validarDatosOrdenDeRuta(ORRU_E o)
        {
            ORTV_N ortvN = new ORTV_N();

            if (string.IsNullOrEmpty(o.TipoRuta)) { throw new Exception("No lleno tipo de ruta encabezado"); }
            //Validacion solo para agencia courier
            if (o.TipoRuta == "AC")
            {
                if (string.IsNullOrEmpty(o.TransDesc)) { throw new Exception("Debe ingresar chofer"); }
                if (string.IsNullOrEmpty(o.Agencia)) { throw new Exception("Debe ingresar agencia"); }
                if (string.IsNullOrEmpty(o.RucAgencia)) { throw new Exception("No existe ruc de agencia"); }
            }
            //otros: (se selecciona los valores no se ingresan en input
            else
            {
                if (string.IsNullOrEmpty(o.TransCod)) { throw new Exception("Debe elegir un chofer"); }
                if (string.IsNullOrEmpty(o.VehiculoCod)) { throw new Exception("Debe elegir un vehiculo"); }
                if (string.IsNullOrEmpty(o.CopilDesc)) { throw new Exception("El documento debe tener copiloto 1"); }

            }
            //Validaciones para cualquier tipo de ruta
            if (string.IsNullOrEmpty(o.Placa)) { throw new Exception("El documento debe tener placa"); }
            if (o.FechaCont == null) { throw new Exception("No eligió FechaContabilizacion"); }

            //validaciones para tipos de ruta distinto a courier y agencia
            if (o.TipoRuta != "VG" && o.TipoRuta != "AC" && (string.IsNullOrEmpty(o.AlmOrigenCod) || string.IsNullOrEmpty(o.AlmOrigenDesc)))
            {
                throw new Exception("No eligió almacén origen");
            }
            if (o.TipoRuta != "VG" && o.TipoRuta != "AC" && (string.IsNullOrEmpty(o.AlmDestinoCod) || string.IsNullOrEmpty(o.AlmDestinoDesc)))
            {
                throw new Exception("No eligió almacén destino");
            }
            if (o.TiempoPac == null) { throw new Exception("Debe haber tiempo pactado"); }
            //Validaciones para todo tipo de ruta menos TA Transferencia entre almacenes
            if (!o.TipoRuta.Equals("TA"))
            {
                if (o.DetRRU0.Count <= 0) { throw new Exception("Debe haber detalles de documento"); }
                foreach (RRU0_E d in o.DetRRU0)
                {
                    if (d.DocEntryTicket > 0)
                    {
                        ORTV_E t = ortvN.obtenerTicket(d.DocEntryTicket);
                        if (string.IsNullOrEmpty(t.EstadoFacturacion) || t.EstadoFacturacion.Equals("PENDIENTE")) { throw new Exception("El ticket en linea " + d.Linea + " no tiene guías emitida, retirar de la tabla."); }
                        if (string.IsNullOrEmpty(d.Guias))
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
                        if (t.LugarDestino == "Agencia" || t.LugarDestino == "Agencia Courier" || t.LugarDestino == "Domicilio")
                        {
                            if (t.TipoVenta == "Normal")
                            {
                                if (t.EstadoPago != "PAGADO") { throw new Exception("El ticket normal de Agencia  o Domicilio debe estar pagado en linea " + d.Linea); }
                            }
                        }
                        //buscar vinculados solo en caso de Domicilio para validar que todos son enviados juntos en la misma hoja de ruta.
                        if (t.LugarDestino == "Domicilio")
                        {
                            var listTicketsVinculados = ortvN.BuscarVinculados(t.DocEntry, t.DocNum);
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
                //if (o.DetRRU0 != null && (o.TipoRuta == "VD" || o.TipoRuta == "VA" || o.TipoRuta == "VC" || o.TipoRuta == "VG"))
                //{
                //    TEMP_RRU01_N tempDoc = new TEMP_RRU01_N();
                //    RRU01_N rru01Neg = new RRU01_N();
                //    List<TEMP_RRU01_E> tempPri = new List<TEMP_RRU01_E>();
                //    //foreach (var det in o.DetRRU0)
                //    //{
                //    //    //List<TEMP_RRU01_E> res = tempDoc.Obtener(det.DocEntryTicket);
                //    //    //List<string> list = new List<string>();
                //    //    //foreach (var e in res)
                //    //    //{
                //    //    //    if (e.Impreso == 0)
                //    //    //    {
                //    //    //        //lista de correlativos que no se imprimieron segun tabla temporal
                //    //    //        list.Add(e.U_SYP_MDTD + "-" + e.U_SYP_MDSD + "-" + e.U_SYP_MDCD);
                //    //    //    }
                //    //    //    else { tempPri.Add(e); }
                //    //    //}
                //    //    //if (list.Count > 0)
                //    //    //{
                //    //    //    for (int i = 0; i < list.Count; i++)
                //    //    //    {
                //    //    //        //busca el elemento en la tabla top 1 para saber si fue impreso antes
                //    //    //        RRU01_E rru01E = rru01Neg.BuscarCorrelativo(list[i]);
                //    //    //       if (rru01E.Id == 0)
                //    //    //        {
                //    //    //            throw new Exception("El doc :" + list[i] + " no se ha impreso. No puede crear hoja de ruta sin antes imprimir todos los comprobantes.");
                //    //    //        }

                //    //    //    }
                //    //    //}

                //    //}
                //    o.DetRRU01 = RRU01_E.compararTemp(tempPri);
                //}
            }
            //Validaciones exclusivas para Transferencia entre almacenes ( tambien son parte de las hojas de ruta )
            else
            {
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
            //Solo edita los datos de encabezado de la hoja de ruta
            ORRU_E orruE = obtenerOrdenDeRuta(o.DocEntry);
            if (orruE.Estado != "CREADO") { throw new Exception("Solo puede editar un documento creado"); }
            if (string.IsNullOrEmpty(o.TipoRuta)) { throw new Exception("No lleno tipo de ruta encabezado"); }
            if (string.IsNullOrEmpty(o.Placa)) { throw new Exception("El documento debe tener placa"); }
            if (o.FechaCont == null) { throw new Exception("No eligió fecha de contabilizacion"); }

            //validaciones para tipos de ruta distinto a courier y agencia
            if (o.TipoRuta != "VG" && o.TipoRuta != "AC" && (string.IsNullOrEmpty(o.AlmOrigenCod) || string.IsNullOrEmpty(o.AlmOrigenDesc)))
            {
                throw new Exception("No eligió almacén origen");
            }
            if (o.TipoRuta != "VG" && o.TipoRuta != "AC" && (string.IsNullOrEmpty(o.AlmDestinoCod) || string.IsNullOrEmpty(o.AlmDestinoDesc)))
            {
                throw new Exception("No eligió almacén destino");
            }

            if (o.TiempoPac == null) { throw new Exception("Debe haber tiempo pactado"); }


            //Validacion para casos de Agencia Courier ( datos ingresados por input)
            if (o.TipoRuta == "AC")
            {
                if (string.IsNullOrEmpty(o.Agencia)) { throw new Exception("Debe ingresar agencia"); }
                if (string.IsNullOrEmpty(o.RucAgencia)) { throw new Exception("Debe ingresar ruc de agencia"); }
            }
            //Todos los casos distintos, donde se escoge los valores de un desplegable
            else
            {
                if (string.IsNullOrEmpty(o.TransCod)) { throw new Exception("Debe elegir un chofer"); }
                if (string.IsNullOrEmpty(o.VehiculoCod)) { throw new Exception("Debe elegir un vehiculo"); }
                if (string.IsNullOrEmpty(o.CopilDesc)) { throw new Exception("El documento debe tener copiloto 1"); }
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
                    if (o.TipoRuta != "VG" && o.TipoRuta != "AC")
                    {
                        if (!(a.TempI1 >= 15 && a.TempI1 <= 25)) { throw new Exception("Temp1 Inicial no cumple con el rango valido"); }
                        if (!(a.HumedI1 >= 50 && a.HumedI1 <= 80)) { throw new Exception("Humed1 Inicial no cumple con el rango valido"); }
                        if (!(a.TempI2 >= 15 && a.TempI2 <= 25)) { throw new Exception("Temp2 Inicial no cumple con el rango valido"); }
                        if (!(a.HumedI2 >= 50 && a.HumedI2 <= 80)) { throw new Exception("Humed2 Inicial no cumple con el rango valido"); }
                    }
                }
            }
            if (o.DetRRU1 != null && o.DetRRU1.Count > 0)
            {
                foreach (RRU1_E a in o.DetRRU1.Where(x => x.Estado != "LIBERADO" && x.Estado != "ANULADO"))
                {
                    if (a.Estado != "PREENVIO") { throw new Exception("Todos los tickets no estan como PREENVIO"); }
                    if (!(a.TempI1 >= 15 && a.TempI1 <= 25)) { throw new Exception("Temp1 Inicial no cumple con el rango valido"); }
                    if (!(a.HumedI1 >= 50 && a.HumedI1 <= 80)) { throw new Exception("Humed1 Inicial no cumple con el rango valido"); }
                    if (!(a.TempI2 >= 15 && a.TempI2 <= 25)) { throw new Exception("Temp2 Inicial no cumple con el rango valido"); }
                    if (!(a.HumedI2 >= 50 && a.HumedI2 <= 80)) { throw new Exception("Humed2 Inicial no cumple con el rango valido"); }
                }
            }
            orruD.IniciarReparto(o);
        }
        public void TerminarReparto(ORRU_E o)
        {
            //En este punto ya no valida el rango de temperaturas o humed para ninguno de los dos tipos de rutas.
            //El rango se valida en RRU0 Y RRU1

            if (o.Estado != "ENVIADO") { throw new Exception("El documento no esta enviado"); }
            if (o.DetRRU0 != null)
            {
                foreach (RRU0_E a in o.DetRRU0.Where(x => x.Estado != "LIBERADO" && x.Estado != "ANULADO"))
                {
                    if (a.Estado != "ENTREGADO") { throw new Exception("El ticket " + a.DocNumTicket + " no esta ENTREGADO"); }
                }
            }
            if (o.DetRRU1 != null)
            {
                foreach (RRU1_E a in o.DetRRU1.Where(x => x.Estado != "LIBERADO"))
                {
                    if (a.Estado != "ENTREGADO") { throw new Exception("Todos los tickets no estan como ENTREGADO"); }
                }
            }

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
        public List<ORRU_E.RptRutas> ReporteHojasCargo(ORRU_E o)
        {
            return orruD.ReporteHojasCargo(o);
        }
        public List<ORRU_E.RptRutasDet> ReporteHojasCargoDet(ORRU_E o)
        {
            return orruD.ReporteHojasCargoDet(o);
        }
        public List<RptPesaje_E> ListarRptPesaje(FiltroRptPesaje datosFiltro)
        {
            return orruD.ListarRptPesaje(datosFiltro);
        }
    }
}