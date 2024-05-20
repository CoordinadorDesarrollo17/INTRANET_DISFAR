using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.General_ENT.TablasSql
{
    public class OUR1_E
    {
        public int Id { get; set; }
        public int IdCourier { get; set; }
        public int Ubigeo { get; set; }
        public string Distrito { get; set; }
        public string Provincia { get; set; }
        public string Departamento { get; set; }
        public string Calle { get; set; }

        /********************************************/
        public string NombreAgencia { get; set; }
    }
}
