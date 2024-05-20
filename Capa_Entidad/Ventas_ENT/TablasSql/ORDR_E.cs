using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class ORDR_E
    {
        public int Id { get; set; }
        public int DocEntryTicket { get; set; }
        public int DocNumTicket { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Estado { get; set; }
        public string WhsCode { get; set; }
        public int CodSapVendedor { get; set; }
        public string Vendedor { get; set; }
        public string Comentario { get; set; }
        public string FechaCreacion { get; set; }
        public string HoraCreacion { get; set; }
        public string VendedorRecibido { get; set; }
        public string FechaRecibido { get; set; }
        public string HoraRecibido { get; set; }
        public string VendedorCancelado { get; set; }
        public string FechaCancelado { get; set; }
        public string HoraCancelado { get; set; }
        // Se usa para saber si el pedido está siendo generado/creado o es un borrador
        // En el caso de borrador, aplica también cuando otro usuario está migrando la data y el usuario actual no pierda su registro
        public string EstadoInicialCreacion { get; set; }

        // Campos que no son de la tabla
        public List<RDR1_E> DetallePedido { get; set; }
    }
}
