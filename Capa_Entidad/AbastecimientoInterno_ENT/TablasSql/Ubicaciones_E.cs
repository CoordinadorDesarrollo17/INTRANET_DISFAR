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

        // Datos adicionales que no están en la tabla SQL
        public string NombreOperarioAccion { get; set; }
        public string UbiSistema { get; set; }
        public int CantidadUbicaciones { get; set; }
        
        // Relación de múltiples ubicaciones si es necesario
        public List<Ubicaciones_E> Ubicaciones { get; set; }
    }

    // Picking
    public class UbicacionesPicking_E : Ubicaciones_E
    {
        public int StockMinAbastecimiento { get; set; }
        public int StockMinVenta { get; set; }
    }

    // Reserva
    public class UbicacionesReserva_E : Ubicaciones_E
    {
        public int UbicacionId { get; set; } // Relación con UbicacionesPicking_E
        public string BatchNum { get; set; }
        public decimal Quantity { get; set; }
    }
}
