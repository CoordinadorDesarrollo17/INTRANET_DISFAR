
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
        public string TipoFirma { get; set; }
        public string CodigoAlmacen { get; set; }
        public string Almacen { get; set; }

        // CAMPOS EXTRAS
        public List<int> ListaDocEntryUsuario { get; set; }
        public int IdRolUsuario { get; set; }
    }
}