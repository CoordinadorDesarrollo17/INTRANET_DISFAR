using Capa_Datos.Rutas_DAO.TablasSql;
using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Capa_Negocio.Ventas_NEG.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Capa_Negocio.Rutas_NEG.TablasSql
{
    public class RRU0_N
    {
        RRU0_D rru0D = new RRU0_D(); ORRU_N orruN = new ORRU_N(); RRU01_N rru01N = new RRU01_N();

        private void AnularTicketACuadrar(int docEntryTicket, int docNumTicket, int docEntryORRU, string opEntrega, int idOTC)
        {
            if (docNumTicket > 0)
            {
                OTC_D otcD = new OTC_D();
                var resultOTC = otcD.ObtenerDatosTicketACuadrar(docEntryTicket, idOTC);

                if (resultOTC != null)
                {
                    OTC_E tc = new OTC_E() { IdOTC = resultOTC.IdOTC, Estado = "ANULADO", PersonaEntrega = opEntrega };
                    otcD.CambiarEstadoTicketACuadrar(tc, "ANULAR", "D");
                }
            }
        }

        public void LiberarRRU0(RRU0_E obj)
        {
            ORRU_E orruE = orruN.obtenerOrdenDeRuta(obj.DocEntry);
            RRU0_E datosRRU0 = rru0D.buscarRRU0(obj.DocEntry, obj.Linea);

            if (orruE.Estado.Equals("TERMINADO")) { throw new Exception("No puede liberar documento TERMINADO"); }
            if (datosRRU0.Estado != "PREENVIO" && datosRRU0.Estado != "ENVIADO") { throw new Exception("Solo puede liberar ticket en PREENVIO o ENVIADO"); }

            datosRRU0.Operario = obj.Operario;
            // Se asigna el "ComentarioLiberado" del POST al objeto "datosRRU0"
            datosRRU0.ComentarioLiberado = obj.ComentarioLiberado;

            rru0D.liberarRRU0(datosRRU0);

            //pasar a liberado el detalle RRU01
            if (orruE.DetRRU01 != null && orruE.DetRRU01.Count > 0)
            {
                foreach (var i in orruE.DetRRU01.Where(x => x.DocEntryTicket == obj.DocEntryTicket))
                {
                    rru01N.Liberar(i.Id, i.DocEntryORRU);
                }
            }

            // Para tickets a cuadrar (PAGO EFECTIVO)
            AnularTicketACuadrar(obj.DocEntryTicket, obj.DocNumTicket, obj.DocEntry, obj.OpEntrega, obj.IdOTC);
        }

        public void AnularRRU0(RRU0_E obj)
        {
            ORRU_E orruE = orruN.obtenerOrdenDeRuta(obj.DocEntry);
            if (orruE.Estado != "TERMINADO") { throw new Exception("No puede anular documento"); }
            RRU0_E o = rru0D.buscarRRU0(obj.DocEntry, obj.Linea);
            o.Operario = obj.Operario;
            if (o.Estado != "ENTREGADO") { throw new Exception("Solo puede anular ticket en estado entregado"); }
            rru0D.anularRRU0(o);
        }
        public void AgregarRRU0(RRU0_E o)
        {
            ORRU_E orruE = orruN.obtenerOrdenDeRuta(o.DocEntry);
            if (orruE.Estado != "CREADO") { throw new Exception("Solo puede agregar a documento CREADO"); }
            ORTV_N ortvN = new ORTV_N();
            ORTV_E t = ortvN.ObtenerDatosCompletosTicket(o.DocEntryTicket);
            if (t.LugarDestino.Equals("Agencia Courier") && orruE.TipoRuta.Equals("AC")) { if (t.Estado != "PESADO") { throw new Exception("El ticket debe estar Pesado"); } }
            else
            {
                if (t.Estado != "EMPACADO" && t.Estado != "PESADO") { throw new Exception("El ticket debe estar Empacado o Pesado"); }
            }
            if (t.LugarDestino.Equals("Agencia") || t.LugarDestino.Equals("Agencia Courier") || t.LugarDestino.Equals("Domicilio"))
            {
                if (t.TipoVenta == "Normal")
                {
                    if (t.EstadoPago != "PAGADO") { throw new Exception("El ticket normal de Agencia  o Domicilio debe estar pagado "); }
                }
            }
            if (string.IsNullOrWhiteSpace(t.EstadoFacturacion) || t.EstadoFacturacion.Equals("PENDIENTE")) { throw new Exception("El ticket no tiene guía emitida, retirar de la tabla."); }

            o.DocNumTicket = t.DocNum; o.Cajas = t.Cajas; o.Envio = t.GastoEnvio; o.MontoFinal = t.MontoFinal; o.Observaciones = t.Observaciones; o.Socio = t.CardName; o.Verificado = "on";

            if (t.Det3 != null)
            {
                if (t.Det3.Count >= 2)
                {
                    o.Direcciones = t.DirDestino + t.Det3[1].Calle;
                }
                else
                {
                    o.Direcciones = t.DirDestino;
                }
            }

            if (string.IsNullOrWhiteSpace(o.Guias))
            {
                throw new Exception("El ticket no tiene guias en linea " + o.Linea);
            }
            else
            {
                if (!Regex.IsMatch(o.Guias, @"\d"))
                {
                    throw new Exception("El ticket no tiene guias en linea " + o.Linea);
                }
            }
            rru0D.agregarRRU0(o);
        }
        public RRU0_E BuscarRRU0(int DocEntry, int Linea)
        {
            return rru0D.buscarRRU0(DocEntry, Linea);
        }

        public void ValidarEntDetReparto(RRU0_E o)
        {
            ORTV_N ortvN = new ORTV_N();
            ORRU_E orruE = orruN.obtenerOrdenDeRuta(o.DocEntry);
            RRU0_E rru0E = BuscarRRU0(o.DocEntry, o.Linea);

            ORTV_E ortvE = ortvN.ObtenerDatosCompletosTicket(rru0E.DocEntryTicket);
            if (orruE.Estado != "ENVIADO") { throw new Exception("El reparto no se encuentra ENVIADO"); }
            if (rru0E.Estado != "ENVIADO") { throw new Exception("Solo puedes entregar detalle Enviado"); }
            if (ortvE.Det5 != null && ortvE.Det5.Count > 0 && ortvE.Det5[0].RegCant > 0 && ortvE.LugarDestino == "Domicilio")
            {
                if (o.Ticket.Det5[0].RegEstado != "Entregado") { throw new Exception("Debe entregar regalo"); }
            }
            if (orruE.TipoRuta != "VG" && orruE.TipoRuta != "AC" && orruE.TipoRuta != "DE")
            {
                if (!(o.TempF1 >= 15 && o.TempF1 <= 25)) { throw new Exception("Temp1 final no cumple con el rango valido (mayor o igual a 15 y menor o igual a 25)"); }
                if (!(o.TempF2 >= 15 && o.TempF2 <= 25)) { throw new Exception("Temp2 final no cumple con el rango valido (mayor o igual a 15 y menor o igual a 25)"); }
            }
            if (orruE.TipoRuta == "VG")
            {
                if (o.Archivo == null)
                {
                    throw new Exception("Debe subir foto Evidencia");
                }
            }
            if (o.Archivo != null)
            {
                if (!(o.Archivo.ContentType == "image/gif" ||
                    o.Archivo.ContentType == "image/png" ||
                    o.Archivo.ContentType == "image/jpeg"))
                {
                    throw new Exception("Debe elegir un archivo valido .gif,.png,.jpeg");
                }
                if (o.Archivo.ContentLength > 15485760) { throw new Exception("No puedes cargar un archivo superior a 15Mb"); }
            }
            o.DocEntryTicket = rru0E.DocEntryTicket;
        }

        public void EntregarRRU0(RRU0_E o)
        {
            ValidarEntDetReparto(o);
            rru0D.entregarRRU0(o);
        }

        public void EntregaMasivaDetRep(RRU0_E o)
        {
            if (!(o.TempF1 >= 15 && o.TempF1 <= 25)) { throw new Exception("Temp1 final no cumple con el rango valido (mayor o igual a 15 y menor o igual a 25)"); }
            if (!(o.TempF2 >= 15 && o.TempF2 <= 25)) { throw new Exception("Temp2 final no cumple con el rango valido (mayor o igual a 15 y menor o igual a 25)"); }

            ORRU_E orruE = orruN.obtenerOrdenDeRuta(o.DocEntry);
            if (orruE.Estado != "ENVIADO") { throw new Exception("El reparto debe estar enviado"); }
            if (!(orruE.TipoRuta == "VC" || orruE.TipoRuta == "VA")) { throw new Exception("No se puede entregar masivamente en caso de rutas centro o arriola"); }
            foreach (RRU0_E r in orruE.DetRRU0.Where(x => x.Estado == "ENVIADO"))
            {
                r.TempF1 = o.TempF1;
                r.TempF2 = o.TempF2;
                r.OpEntrega = o.OpEntrega;
                rru0D.entregarRRU0(r);
            }
        }

        public List<MotivoLiberacion> ListarMotivosLiberacion()
        {
            return rru0D.ListarMotivosLiberacion(); 
        }
    }
}