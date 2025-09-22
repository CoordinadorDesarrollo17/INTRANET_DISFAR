using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Rutas_ENT.ReportesSql
{
    public class Rpt_TempHumed_E
    {
        public string Placa { get; set; }
        public string Serie { get; set; }
        public string Fecha { get; set; }
        public int Documento { get; set; }
        //Campo para docentry o guias
        public string Codigo { get; set; }
        public string HoraSalida { get; set; }
        public string HoraLlegada { get; set; }
        public decimal TempI { get; set; }
        public int HumedI { get; set; }
        public decimal TempF { get; set; }
        public int HumedF { get; set; }
        public string Encargado { get; set; }
        public string Verificado { get; set; }
        /************ C A M P O S   Q U E   N O   S O N   D E   L A   T A B L A ************/
        public string TransCod { get; set; }
        // Nuevos campos para identificar el registro a actualizar (si existen en el origen de datos)
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
    }
}
