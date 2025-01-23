using System.ComponentModel.DataAnnotations;

namespace Capa_Entidad.Repartos_ENT.TablasHana
{
    public class SYP_CONDUC_E
    {
        [Display(Name="Codigo")]
        public string Code { get; set; }
        [Display(Name = "DocEntry")]
        public int DocEntry { get; set; }
        [Display(Name = "Cancelado")]
        public string Canceled { get; set; }
        [Display(Name = "Fecha de creación")]
        public string CreateDate { get; set; }
        [Display(Name = "Fecha de modificación")]
        public string UpdateDate { get; set; }
        [Display(Name = "Licencia")]
        public string U_SYP_CHLI { get; set; }
        [Display(Name = "Nombre")]
        public string U_SYP_CHNO { get; set; }
        [Display(Name = "Zona de reparto")]
        public string U_SYP_DIS { get; set; }
        [Display(Name = "Dni")]
        public string U_SYP_DNI { get; set; }

    }
}
