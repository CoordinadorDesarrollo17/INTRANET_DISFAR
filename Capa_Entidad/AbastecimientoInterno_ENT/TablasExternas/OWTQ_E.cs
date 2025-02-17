using System.Collections.Generic;

namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasExternas
{
    public class OWTQ_E
    {
        public int DocEntry {  get; set; }
        public int DocNum {  get; set; }
        public string DocDate {  get; set; }
        public string CardCode {  get; set; }
        public string CardName {  get; set; }
        public string NroGuia {  get; set; }
        public string OperarioResponsable {  get; set; }
        public string MotivoTraslado {  get; set; }
        public List<WTQ1_E> Detalle {  get; set; }
    }
}
