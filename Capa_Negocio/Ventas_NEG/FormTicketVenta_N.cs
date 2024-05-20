using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.Ventas_DAO;

namespace Capa_Negocio.Ventas_NEG
{
    public class FormTicketVenta_N
    {
        FormTicketVenta_D formTicketD = new FormTicketVenta_D();
        public string generaInfoListaClientes(string Fecha)
        {
            return formTicketD.generaInfoListaClientes(Fecha);
        }
        public string generaInfoListaDirDestinos(string CardCode)
        {
            return formTicketD.generaInfoListaDirDestinos(CardCode);
        }
        public string generaInfoListaCorreosCliente(string CardCode)
        {
            return formTicketD.generaInfoListaCorreosCliente(CardCode);
        }
        public string generaInfoListaOrdenesDeVenta(string Fecha, string CardCode)
        {
            return formTicketD.generaInfoListaOrdenesDeVenta(Fecha, CardCode);
        }
        public string generaInfoListaNotasDeCreditoV(string CardCode)
        {
            return formTicketD.generaInfoListaNotasDeCreditoV(CardCode);
        }
    }
}
