namespace Capa_Entidad.DireccionTecnica_ENT.Reportes.Liberaciones
{
    public class RptTransferencias_E
    {
        // Cabecera ODOCS
        public string TipoDocumento { get; set; }
        public long DocEntry { get; set; }
        public long DocNum { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Guia { get; set; }
        public string ComprobanteVinculado { get; set; }
        public string FechaContabilizacion { get; set; }
        public string FechaInicioTraslado { get; set; }
        public string Estado { get; set; }

        // Detalle DOCS1
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Lote { get; set; }
        public string FechaVencimiento { get; set; }
        public string RegistroSanitario { get; set; }
        public string Fabricante { get; set; }
        public string CondicionAlmTrans { get; set; }
        public string Almacen { get; set; }
        public string CertificadoAnalisis { get; set; }
        public string ComentarioOrganoleptico { get; set; }
        public int CantidadAprobados{ get; set; }
        public int CantidadBaja{ get; set; }
        public int CantidadDevolucion { get; set; }
        public int CantidadTotal{ get; set; }
        public string Liberado{ get; set; }
        public string Transferido { get; set; }
    }
}
