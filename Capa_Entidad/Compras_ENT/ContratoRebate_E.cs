using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Capa_Entidad.Compras_ENT
{
    public class ContratoRebate_E
    {
        [DisplayName("Nro Contrato")]
        public int id { get; set; }
        [Required(ErrorMessage ="Debe elegir Tipo")]
        public string Tipo { get; set; }
        [DisplayName("Socio")]
        [Required(ErrorMessage = "Debe elegir Socio")]
        public string SocioDesc { get; set; }
        [Required(ErrorMessage = "Debe elegir SocioCod")]
        public string SocioCod { get; set; }
        [DisplayName("Titulo del contrato")]
        [Required(ErrorMessage = "Debe LLenar Titulo")]
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        [DisplayName("PeriodoInicio")]
        [Required(ErrorMessage = "Debe elegir PeriodoInicio")]
        public string PerValIni { get; set; }
        [DisplayName("PeriodoFin")]
        [Required(ErrorMessage = "Debe elegir PeriodoFin")]
        public string PerValFin { get; set; }
        [DisplayName("MontoObjetivo")]
        public decimal ObjPer { get; set; }
        [DisplayName("Encargado de Compras")]
        public string EncCompras { get; set; }
        [DisplayName("Representante del Socio")]
        public string EncSocio { get; set; }
        public string Estado { get; set; }
        [DisplayName("Texto Confirmacion")]
        public string ConfDesc { get; set; }
        public List<DetContratoRebate_E> Det { get; set; }
    }
}
