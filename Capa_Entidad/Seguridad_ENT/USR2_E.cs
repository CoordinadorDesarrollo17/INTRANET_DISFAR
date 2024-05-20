using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Seguridad_ENT
{
    public class USR2_E
    {
        //intentos de operaciones por dia de cada usuario
        public int DocEntry { get; set; }
        public int IdOperacion { get; set; }
        public int Intentos { get; set; }
        public int UsosDia { get; set; }
        public DateTime Dia { get; set; }
    }
}
