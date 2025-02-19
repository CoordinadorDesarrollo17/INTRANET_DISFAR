using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.AbastecimientoInterno_ENT.TablasSql
{
    public class KardexAbastecimiento_E
    {
        public int Id { get; set; }

        [Display(Name = "SKU")]
        public string ItemCode { get; set; }

        [Display(Name = "Descripción")]
        public string ItemName { get; set; }
        public string Almacen { get; set; }
        public string RucProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public string Sentido { get; set; }
        public string Tabla { get; set; }
        public string Referencia { get; set; }
        public int Cantidad { get; set; }
        public int Imputado { get; set; }
        public string Operario { get; set; }
        public DateTime TiempoRegistro { get; set; }
    }
}
