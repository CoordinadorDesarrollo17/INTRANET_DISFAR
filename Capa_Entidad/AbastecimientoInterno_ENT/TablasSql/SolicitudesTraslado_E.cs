using System.Collections.Generic;

namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class SolicitudesTraslado_E 
    {
        public int Id { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocDate { get; set; }
        public string NroGuia { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string OperarioResponsableSAP { get; set; }
        public string OperarioRegistra { get; set; }
        public string MotivoTraslado { get; set; }
        public string Estado { get; set; }
        public Dictionary<string,  DetalleSolicitudesTraslado_E> Detalle { get; set; }

        public string TipoDocumento { get; set; }

    }
}
