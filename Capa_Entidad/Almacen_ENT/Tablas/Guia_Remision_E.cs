namespace Capa_Entidad.Almacen_ENT.Tablas
{
    public class Guia_Remision_E
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string ElaboradoPor { get; set; }
        public string NombreBD { get; set; }
        public string DireccionBD { get; set; }
        public string RucBD { get; set; }
        public string TelBD { get; set; }
        public string DocDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string NumAtCard { get; set; }
        public string DirCliente { get; set; }
        public string DirSalida { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CantidadL { get; set; }
        public decimal QUMVta { get; set; }
        public string UnidadMedidaLote { get; set; }
        public string UnidadMedidaLote2 { get; set; }
        public decimal UndPesoLinea { get; set; }
        public string DocOrigen { get; set; }
        public string TextoPermanente { get; set; }
        public int Bulto { get; set; }
        public string RegSanit { get; set; }
        public string NomTransportista { get; set; }
        public string RucTransportista { get; set; }
        public string UniMedida { get; set; }
        public string ItemCode { get; set; }
        public string DescripcionArticulo { get; set; }
        public string DirAlmacen { get; set; }
        public string NumAlmacen { get; set; }
        public string DistritoAlmacen { get; set; }
        public string ProvinciaAlmacen { get; set; }
        public string DepartamentoAlmacen { get; set; }
        public string DirLlegada { get; set; }
        public string DirProveedor { get; set; }
        public string Laboratorio { get; set; }
        public string LoteNum { get; set; }
        public string VctoLote { get; set; }
        public string Motivo { get; set; }
        public int LineaOrden { get; set; }
        public string FechaTrasl { get; set; }
        public string Texto { get; set; }
        public string Motivo_Trasl { get; set; }
        public string Modalidad_Trasl { get; set; }
        public decimal PesoTotal { get; set; }
        public string Conductor { get; set; }
        public string DNI_Conduc { get; set; }
        public string Licencia { get; set; }
        public string Marca { get; set; }
        public string Placa { get; set; }
        public string CertiInscrip { get; set; }
        public string TipoComprobantePago { get; set; }
        public string NroComprobantePago { get; set; }
        public string Um { get; set; }
        // metodos del layout
        public decimal cantVta()
        {
            if (LoteNum == null || LoteNum == "") { return Cantidad; }
            else
            {
                if ((CantidadL / QUMVta) < 1) { return CantidadL; }
                else { return CantidadL / QUMVta; }
            }
        }
        public string UM()
        {
            if ((CantidadL / QUMVta) < 1) { return "PZA"; }
            else { return Um; }
        }
    }
}
