using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.SocioNegocios_ENT.Tablas;

namespace Capa_Entidad.Compras_ENT
{
    public class LineaProduccion_E
    {
        public int id { get; set; }
        [Required(ErrorMessage ="Debe elegir el fabricante")]
        public OMRC_E Fabricante { get; set; }
        [Required(ErrorMessage ="Debe llenar descripcion")]
        public string Descripcion { get; set; }
        public OCRD_E Proveedor { get; set; }
        [DisplayName("PersonaDeContacto")]
        public string PerContacto1 { get; set; }
        [DisplayName("Telefono")]
        public string TelefPerContacto1 { get; set; }
        [DisplayName("Email")]
        public string EmailPerContacto1 { get; set; }
        public string TiempoCreacion { get; set; }
        [Required(ErrorMessage ="Debe elegir productos")]
        public List<DetLineaProduccion_E> Det { get; set; }
        public List<DetLineaProduccion_E> Det2 { get; set; } // auxiliar para otro laboratorio
        //metodos
        public List<DetLineaProduccion_E> FabricarDetalles(List<DetLineaProduccion_E> Det, List<DetLineaProduccion_E> Det2)
        {
            List<DetLineaProduccion_E> lista = Det;
            if(Det2!=null)
            {
                foreach (DetLineaProduccion_E d in Det2)
                {
                    lista.Add(d);
                }
            }
            return lista;
        }

    }
}
