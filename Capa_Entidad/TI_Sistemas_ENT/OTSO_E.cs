using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Capa_Entidad.TI_Sistemas_ENT
{
    public class OTSO_E

    {
        public int DocEntry { get; set; }
        public string Estado { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Prioridad { get; set; }
        public string Propietario { get; set; }
        public string Contacto { get; set; }
        public string Area { get; set; }
        public string Sede { get; set; }
        public string Asistencia { get; set; }
        public string Tipo { get; set; }
        public string Subcategoria { get; set; }
        public string Categoria { get; set; }
        public string Solucion { get; set; }
        public string Adjunto { get; set; }
        //
        public HttpPostedFileBase Archivo { get; set; }
        public DateTime TiempoRegistro { get; set; }
        public string OpAsignacion { get; set; }
        public string IdAsignado { get; set; }
        public string OpAsignado { get; set; }
        public DateTime TiempoAsignacion { get; set; }
        public string OpAtencion { get; set; }
        public DateTime TiempoAtencion { get; set; }
        // campos que no son de la tabla
        public string Operario { get; set; }


    }
}
