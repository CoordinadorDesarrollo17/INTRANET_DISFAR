using Capa_Entidad.AbastecimientoInterno_ENT.Interfaces;
using System.Collections.Generic;
namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class SolicitudTraslado_E : ITraslado
    {
        public int Id { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string NroGuia { get; set; }
        public string OperarioResponsable { get; set; }
        public string MotivoTraslado { get; set; }
        public string Estado { get; set; }
        public List<DetSolicitudTraslado_E> Detalle { get; set; }
    }
}
