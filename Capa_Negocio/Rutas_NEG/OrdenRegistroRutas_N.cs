using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.Ventas_ENT;
using Capa_Datos.Rutas_DAO;
using Capa_Entidad.Rutas_ENT;
using Capa_Entidad.Almacen_ENT;
using Capa_Negocio.Ventas_NEG;
using Capa_Entidad.Almacen_ENT.Tablas;

namespace Capa_Negocio.Rutas_NEG
{
    public class OrdenRegistroRutas_N
    {
        OrdenRegistroRutas_D orruD = new OrdenRegistroRutas_D();
        public List<OrdenRegistroRutas_E> listarOrru(OrdenRegistroRutas_E o)
        {
            return orruD.listarOrru(o);
        }
        public OrdenRegistroRutas_E inicializarOrdenDeRuta()
        {
            return orruD.inicializarOrdenDeRuta();
        }
        public List<OrdenRegistroRutas_E> listaTranportistas()
        {
            return orruD.listaTranportistas();
        }
        public List<OrdenRegistroRutas_E> listaPlacas()
        {
            return orruD.listaPlacas();
        }
        public List<OrdenRegistroRutas_E> listaCopilotos()
        {
            return orruD.listaCopilotos();
        }
        public List<OrdenRegistroRutas_E> listaOrigenesDestinos()
        {
            return orruD.listaOrigenesDestinos();
        }
        public List<TicketVenta_E> listaSocios(string FechaCont)
        {
            return orruD.listaSocios(FechaCont);
        }
        public List<OWTQ_E> listarGuiasTraslado()
        {
            return orruD.listarGuiasTraslado();
        }
        public string GuiasTicket(int DocEntry)
        {
            return orruD.GuiasTicket(DocEntry);
        }
        public int CrearOrdenDeRuta(OrdenRegistroRutas_E o)
        {
            validarDatosOrdenDeRuta(o);
            return orruD.CrearOrdenDeRuta(o);
        }
        public int EditarOrdenDeRuta(OrdenRegistroRutas_E o)
        {
            OrdenRegistroRutas_E or = obtenerOrdenDeRuta(o.DocEntry);
            if (or.Estado.Equals("ANULADO")) { throw new Exception("No se puede editar Ruta Anulada"); }
            return orruD.EditarOrdenDeRuta(o);
        }
        public int AnularOrdenDeRuta(int DocEntry)
        {
            OrdenRegistroRutas_E o = orruD.obtenerOrdenDeRuta(DocEntry);
            if (o.Estado.Equals("ANULADO")) { throw new Exception("La ruta ya se encuentra Anulado"); }
            foreach(DetalleRegistroRutas_E d in o.ListaDetalleRegistroRutas)
            {
                if (!d.Ticket.EstadoPedido.Equals("ENVIADO")) { throw new Exception("Solo puedes anularRuta a tickets ENVIADOS: " + d.Ticket.DocNum); }
            }
            return orruD.AnularOrdenDeRuta(DocEntry);
        }
        public OrdenRegistroRutas_E obtenerOrdenDeRuta(int DocEntry)
        {
            return orruD.obtenerOrdenDeRuta(DocEntry);
        }
        public void validarDatosOrdenDeRuta(OrdenRegistroRutas_E o)
        {
            TicketVenta_N ortvN = new TicketVenta_N();
            bool esVacio(string dato)
            {
                if (dato == null || dato.ToString().Equals("")) { return true; } else { return false; }
            }
            if (esVacio(o.TipoRuta)) { throw new Exception("No lleno Tipo de ruta encabezado"); }
            if (o.TransCod <= 0 || esVacio(o.TransDesc)) { throw new Exception("No eligio transportista"); }
            if (o.PlacaCod<=0||esVacio(o.PlacaDesc)) { throw new Exception("No eligio placa"); }
            if (o.CopilCod<=0||esVacio(o.CopilDesc)) { throw new Exception("No eligio copiloto1"); }
            if (esVacio(o.FechaCont)) { throw new Exception("No eligio FechaContabilizacion"); }
            if (esVacio(o.AlmOrigenCod)||esVacio(o.AlmOrigenDesc)) { throw new Exception("No eligio almacen origen"); }
            if (esVacio(o.AlmDestinoCod)||esVacio(o.AlmDestinoDesc)) { throw new Exception("No eligio almacen destino"); }
            if (esVacio(o.HoraI)) { throw new Exception("No lleno Hora inicio"); }
            //if (esVacio(o.HoraT)) { throw new Exception("No lleno Hora termino"); }
            if (o.TotalCajas<=0) { throw new Exception("El total de cajas debe der mayor a 0"); }
            if(!o.TipoRuta.Equals("TA"))
            {
                foreach (DetalleRegistroRutas_E d in o.ListaDetalleRegistroRutas)
                {
                    if (d.DocEntryTicket > 0)
                    {
                        TicketVenta_E t = ortvN.obtenerTicket(d.DocEntryTicket);
                        if (esVacio(d.Guias)) { throw new Exception("El ticket no tienes guia en linea " + d.Linea); }
                        if (t.Cajas <= 0) { throw new Exception("El ticket no tiene cajas en linea " + d.Linea); }
                        if (esVacio(d.Verificado)) { throw new Exception("No verifico el ticket en linea " + d.Linea); }
                        if (t.EstadoPedido != "EMPACADO") { throw new Exception("Solo Puedes enviar ticket en estado EMPACADO " + d.Linea); }
                        if (t.TipoVenta == "Normal") 
                        {
                            if(t.LugarDestino=="Agencia" || t.LugarDestino=="Domicilio")
                            {
                                if (t.EstadoPago != "PAGADO") { throw new Exception("El ticket normal de Agencia  o Domicilio debe estar pagado en linea " + d.Linea); }
                            }                            
                        }  
                    }
                }
            }
            else
            {
                foreach(RRU1_E r in o.RRU1)
                {
                    if(r.Guia!=null && r.Guia.Length>0)
                    {
                        if (r.Cajas <= 0) { throw new Exception("La guia no tiene cajas en linea " + r.Linea); }
                        if(r.ListaRRU12!=null && r.ListaRRU12.Count>0)
                        {
                            foreach (RRU12_E r12 in r.ListaRRU12)
                            {
                                if (r12.Cajas <= 0) { throw new Exception("La guia no tiene cajasMaster en posicion " + r12.BaseLinea + "," + r12.Linea); }
                            }
                        }
                        if (!(r.Cajas == r.ListaRRU12.Sum(y => y.Cajas))) { throw new Exception("El total de cajasMaster no coincide en linea " + r.Linea); }
                    }                    
                }
                if(!(o.TotalCajas== o.RRU1.Sum(x => x.Cajas))) { throw new Exception("El total de cajas no coincide en los subtotales"); }
            }
        }
        public OrdenRegistroRutas_E obtenerOrdenDeRutaTicket(int DocEntryTicket)
        {
            return orruD.obtenerOrdenDeRutaTicket(DocEntryTicket);
        }
        //calculos
        public int calcularTotalCajas(List<TicketVenta_E> l)
        {
            return orruD.calcularTotalCajas(l);
        }
    }
}
