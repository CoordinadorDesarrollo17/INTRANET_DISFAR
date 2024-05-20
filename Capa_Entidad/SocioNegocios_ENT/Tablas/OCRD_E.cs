using System.Collections.Generic;

namespace Capa_Entidad.SocioNegocios_ENT.Tablas
{
    // maestro de socio de negocios
    public class OCRD_E
    {
        public string Address { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public string Phone1 { get; set; }
        public int GroupCode { get; set; }
        public string CreateDate { get; set; }
        public string TipoCliente { get; set; }			// Devolución de mercancías
        //detalles
        public List<OCPR_E> PersonasContacto { get; set; }
    }
}
