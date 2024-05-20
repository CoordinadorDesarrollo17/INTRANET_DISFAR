
using System.Collections.Generic;

namespace Capa_Entidad.General_ENT.TablasSql
{
    public class Firmas_E
    {
        public int DocEntry { get; set; }
        public int DocEntryUsuario { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string RutaFirma { get; set; }
        /******************************* C A M P O S   Q U E   N O   S O N   D E   L A   T A B L A *******************************/
        public List<int> ListaDocEntryUsuario { get; set; }
        public int IdRolUsuario { get; set; }
    }
}