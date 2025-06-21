using System.Collections.Generic;

namespace Capa_Entidad.ReportesDigemid_ENT
{
    public class OVItemCodeDetalle_E
    {
        public string Codigo { get; set; }
        public string Producto { get; set; }
        public string Laboratorio { get; set; }
        public decimal TotalUnidadesVendidas { get; set; }
        public string Comentarios { get; set; }
        public List<OVLoteDetalle_E> LoteDetalle { get; set; }
        public string[] Ubicaciones { get; set; }
    }
}
