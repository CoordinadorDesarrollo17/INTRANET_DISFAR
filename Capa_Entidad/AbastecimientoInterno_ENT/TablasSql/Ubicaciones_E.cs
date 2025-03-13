using System.Collections.Generic;

namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class Ubicaciones_E
    {
        public int Id { get; set; }
        public string Almacen { get; set; }
        public string CodigoUbicacion { get; set; }

        // Datos adicionales que no están en la tabla SQL
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string NombreOperarioAccion { get; set; }
        public string UbiSistema { get; set; }
        public int CantidadUbicaciones { get; set; }

        // Picking
        public int StockMinAbastecimiento { get; set; }
        public int StockMinVenta { get; set; }
        public string Clasificacion { get; set; }

        // Relación de múltiples ubicaciones si es necesario
        public List<Ubicaciones_E> Ubicaciones { get; set; }

    }
}

