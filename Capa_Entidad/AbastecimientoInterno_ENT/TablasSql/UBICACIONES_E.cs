using System.Collections.Generic;

namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class Ubicaciones_E
    {
        public int Id { get; set; }
        public string Almacen { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CodigoUbicacion { get; set; }
        public string BatchNum { get; set; }
        public decimal QuantityUnidades { get; set; }

        // Campos que no son de la tabla
        public string NombreOperarioAccion { get; set; }
        public string UbiSistema { get; set; }
        public List<Ubicaciones_E> Ubicaciones { get; set; }
        public int CantidadUbicaciones { get; set; }
        public int StockMinAbastecimiento { get; set; }
        public int StockMinVenta { get; set; }
    }
}