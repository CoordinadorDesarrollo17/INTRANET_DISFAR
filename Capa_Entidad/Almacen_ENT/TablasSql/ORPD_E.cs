using System.Collections.Generic;

namespace Capa_Entidad.Almacen_ENT.TablasSql
{
    public class ORPD_E
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string FechaDevolucion { get; set; }
        public string Correlativo { get; set; }
        public string WhsCode { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Estado { get; set; }
        public bool RetiroMercado { get; set; }
        public string Correo { get; set; }
        public string TiempoCorreoEnviado { get; set; }
        public string Comentario { get; set; }
        public bool SinEM { get; set; }
        public long? ODOCSId { get; set; }       // Para liberaciones (Dirección Técnica)

        /********************* CAMPOS EXTRAS ********************/
        public string HoraDevolucion { get; set; }
        public string Operario { get; set; }                    // Para la tabla CC_ORPD
        public string FechaOperacion { get; set; }      // Para la tabla CC_ORPD
        public List<RPD1_E> DetalleDevolucion { get; set; }
    }

}
