using System;
using System.Collections.Generic;

namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class Requerimientos_E
    {
        public int IdentificadorExcel { get; set; }
        public int Id { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string TipoAbastecimiento { get; set; }
        public string Comentario { get; set; }
        public DateTime TiempoRegistro { get; set; }
        public string OperarioRegistra { get; set; }
        public string Zona { get; set; }
        public int Aprobado { get; set; }
        public List<DetalleRequerimientos_E> Detalle { get; set; }
    }
}