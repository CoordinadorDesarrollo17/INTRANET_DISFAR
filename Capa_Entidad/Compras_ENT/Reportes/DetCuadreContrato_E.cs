using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Compras_ENT.Reportes
{
    public class DetCuadreContrato_E
    {
        public string Descripcion { get; set; }
        public string U_SYP_DESC { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string SubTipo { get; set; }
        public string EstadoDet { get; set; }
        public List<EspCuadreContrato_E> Esp { get; set; }
    }
}
