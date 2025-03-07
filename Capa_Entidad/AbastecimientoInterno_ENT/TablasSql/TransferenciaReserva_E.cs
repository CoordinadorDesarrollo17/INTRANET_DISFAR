using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class TransferenciaReserva_E
    {
        public int IdentificadorExcel { get; set; }
        public int Id { get; set; }
        public int SolicitudTrasladoId { get; set; }
        public int SolicitudTrasladoDocNum { get; set; } 
        public string DocDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string NroGuia { get; set; }
        public string OperarioRegistra { get; set; }
        public List<DetalleTransferenciaReserva_E> Detalle { get; set; }
    }
}
