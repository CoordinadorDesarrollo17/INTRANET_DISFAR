using System.ComponentModel.DataAnnotations;

namespace Capa_Entidad.Repartos_ENT.TablasHana
{
    public class SYP_VEHICU_E
    {
        [Display(Name="Codigo") ]
        public string Code { get; set; }
        [Display(Name = "DocEntry")]
        public int DocEntry { get; set; }
        [Display(Name = "Cancelado")]
        public string Canceled { get; set; }
        [Display(Name = "Fecha de creación")]
        public string CreateDate { get; set; }
        [Display(Name = "Fecha de modificación")]
        public string UpdateDate { get; set; }
        [Display(Name = "Marca de Vehículo")]
        public string U_SYP_VEMA { get; set; }
        [Display(Name = "Modelo de Vehículo")]
        public string U_SYP_VEMO { get; set; }
        [Display(Name = "Año de Vehículo")]
        public string U_SYP_VEAN { get; set; }
        [Display(Name = "Placa de Vehículo")]
        public string U_SYP_VEPL { get; set; }
        [Display(Name = "Chofer")]
        public string U_SYP_CHOF { get; set; } /*enlaza con U_SYP_CHLI en SYP_CONDUC_E*/

    }
}
