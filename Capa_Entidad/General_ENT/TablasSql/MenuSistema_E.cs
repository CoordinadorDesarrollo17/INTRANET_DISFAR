using System.Collections.Generic;

namespace Capa_Entidad.General_ENT.TablasSql
{
    public class MenuSistema_E
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Nivel { get; set; }
        public int? SuperiorId { get; set; }
        public string Ruta { get; set; }
        public string Icono { get; set; }
        public int Orden { get; set; }
        public List<MenuSistema_E> SubMenus { get; set; } = new List<MenuSistema_E>();
    }
}