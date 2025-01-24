using Capa_Entidad.ReportesDigemid_ENT;
using System.Collections.Generic;

namespace Capa_Entidad.Ventas_ENT
{
    public class PaginacionImpresion_E
    {
        public List<OrdenDeVenta_E> Pagina { get; set; } // Lista de elementos en esta página
        public bool EsUltima { get; set; }
    }
}
