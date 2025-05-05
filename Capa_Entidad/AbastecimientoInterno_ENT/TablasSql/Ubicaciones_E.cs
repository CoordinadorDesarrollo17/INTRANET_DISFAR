using System.Collections.Generic;

namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class Ubicaciones_E
    {
        public int Id { get; set; }
        public string Almacen { get; set; }
        public string CodigoUbicacion { get; set; }

        // Datos adicionales que no están en la tabla SQL
        public string NombreOperarioAccion { get; set; }
        public int CantidadProductos{ get; set; }

        // Relación de múltiples ubicaciones si es necesario
        public List<UbicacionesLotes_E> UbicacionesLotes { get; set; }

    }
}

