using System.Collections.Generic;
using System.ComponentModel;
using System.Web;

namespace Capa_Entidad.AtencionCliente_ENT.TablasSql
{
    public class OSAT_E
    {
        [DisplayName("Cod")] public int DocEntry { get; set; }
        [DisplayName("N° Sol.")] public string DocNum { get; set; }
        public string Tipo { get; set; }
        [DisplayName("N° Ticket")] public int DocNumTicket { get; set; }
        public int DocEntryTicket { get; set; }
        public string Estado { get; set; }
        public string Factor { get; set; }
        public string Contacto { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string DireccionRecojo { get; set; }
        public string FechaRegistro { get; set; }
        public string HoraRegistro { get; set; }
        public string OpRegistro { get; set; }
        public string RutaArchivo { get; set; }
        public string UrlArchivo { get; set; }
        public string Resultado { get; set; }
        public string Solucion { get; set; }
        public string TipoSolucion { get; set; }
        public string FechaFacturacion { get; set; }
        public string FechaAtencion { get; set; }
        public string TipoError { get; set; }
        public string Problema { get; set; }
        public string TipoVenta { get; set; }   
        public string CanalVenta { get; set; }
        public int NotiCliente { get; set; }
        public int DiasRetraso { get; set; }
        public string ErrAlmOtrCom { get; set; }
        public string TicketSolucion { get; set; }

        // Campos que no son de la tabla RTV1
        public string TiempoAtencion { get; set; }
        public string DocFact { get; set; }
        public string TiempoRegistro { get; set; }

        // Se usa para filtrar query sobre el botón Reclamos en CreaTicketVenta
        public string TipoSolicitudCreaTicketVenta { get; set; }
        public bool? SoloSinTicketSolucion { get; set; }

        public List<SAT1_E> Det { get; set; }
        public Dictionary<string, string> DetORTV { get; set; }
        public List<HttpPostedFileBase> Archivo { get; set; }

        // Se usa para el proceso de Notificar Cliente
        public string CardName { get; set; }
        public string CardCode { get; set; }
        public int TicketsAbiertos { get; set; }
        public string FechaSapTicket { get; set; }
        public int DocNumVt { get; set; }
        public string EstadoVt { get; set; }
        public string Vendedor { get; set; }

        public string enlistarDetSolicitudes()
        {
            string enlista = "";
            if (Det != null)
            {
                foreach (SAT1_E i in Det)
                {
                    enlista += "" + i.ComprobanteFin + ",\n";
                }
            }

            return enlista;
        }

        public bool alMenos1Nc()
        {
            foreach (SAT1_E i in Det)
            {
                if (i.TareaFact != null) { return true; }
            }
            return false;
        }
    }
}
