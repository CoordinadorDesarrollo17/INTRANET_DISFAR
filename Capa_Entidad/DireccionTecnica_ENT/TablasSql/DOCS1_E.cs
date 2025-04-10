namespace Capa_Entidad.TablasSql
{
    public class DOCS1_E
    {
        public int Id { get; set; }
        public int ODCOSId { get; set; }
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
        public int CantidadAprobados { get; set; }
        public int CantidadBaja { get; set; }
        public int CantidadDevolucion { get; set; }
        public int CantidadTotal { get; set; }
        public int Liberado { get; set; }
        public int Transferido { get; set; }
    }
}