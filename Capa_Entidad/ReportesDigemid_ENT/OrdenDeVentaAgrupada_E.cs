using System.Collections.Generic;

namespace Capa_Entidad.ReportesDigemid_ENT
{
    public class OrdenDeVentaAgrupada_E
    {
        public string NombreBd { get; set; }
        public int DocNum { get; set; }
        public string Fecha { get; set; }
        public string CardName { get; set; }
        public string RucCliente { get; set; }
        public string SlpName { get; set; }
        public List<OVItemCodeDetalle_E> ItemCodeDetalle { get; set; }
        public string Almacen { get; set; }
        public decimal DocTotal { get; set; }
    }
}