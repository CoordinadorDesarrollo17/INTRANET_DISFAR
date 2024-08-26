using Capa_Entidad.Seguridad_ENT;

namespace Capa_Entidad
{
    public class AccessoHelper_E
    {
        public int OpeID { get; set; }
        public string action { get; set; }
        public string controllerDestino { get; set; }
        public Usuario_E usuario { get; set; }
        public string userHostAddress { get; set; }
        public string userHostName { get; set; }
    }
}