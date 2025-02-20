using Capa_Entidad.AbastecimientoInterno_ENT.Interfaces;
using System.Collections.Generic;

namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasExternas
{
    public class OWTQ_E : ITraslado
    {
        public int Id { get; set; }
        public int DocEntry {  get; set; }
        public int DocNum {  get; set; }
        public string DocDate {  get; set; }
        public string CardCode {  get; set; }
        public string CardName {  get; set; }
        public string NroGuia {  get; set; }
        public string OperarioResponsableSAP {  get; set; }
        public string MotivoTraslado {  get; set; }
        public string Estado { get; set; } = "PENDIENTE";
        public Dictionary<string , WTQ1_E>  Detalle {  get; set; }
    }
}
