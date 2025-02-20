

namespace Capa_Entidad.AbastecimientoInterno_ENT.Interfaces
{
    public interface ITraslado
    {
        int Id { get; }
        int DocEntry { get; }
        int DocNum { get; }
        string DocDate { get; }
        string CardCode { get; }
        string CardName { get; }
        string NroGuia { get; }
        string OperarioResponsableSAP { get; }
        string MotivoTraslado { get; }
        string Estado { get; }
        //Datos que no son de la tabla

    }
}
