using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.AtencionCliente_ENT.ReportesSql
{
    public class Rpt_Regalos
    {
        [DisplayName("N° TICKET VENTA")] public string DocNum { get; set; }
        [DisplayName("FECHA TICKET")] public string FechaSapTicket { get; set; }
        [DisplayName("CLIENTE")] public string CardName { get; set; }
        [DisplayName("ESTADO VENTA")] public string EstadoVt { get; set; }
        [DisplayName("N° SOLICITUD AT")] public string DocNumAT { get; set; }
        [DisplayName("ESTADO SOLICITUD AT")] public string Estado { get; set; }
    }
}
