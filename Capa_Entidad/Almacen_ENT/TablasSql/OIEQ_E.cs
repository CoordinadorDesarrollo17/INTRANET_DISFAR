using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.TablasSql
{
    public class OIEQ_E
    {
        public int DocEntry { get; set; }
        public int DocEntryPer { get; set; }
        public string Nombre { get; set; }
        public string WhsCode { get; set; }
        public int Piso { get; set; }
        public string Estado { get; set; }
        public string FechaRegistro { get; set; }
        public string HoraRegistro { get; set; }
        public string Tipo { get; set; }
        public string Propietario { get; set; }
        public string Controlados { get; set; }
        //campos no de la tabla
        public string DescripcionPeriodo { get; set; }
        public List<IEQ1_E> DetMiembros { get; set; }
        public List<IEQ2_E> DetFabricantes { get; set; }
        //constructor
        public OIEQ_E()
        {
            DetFabricantes = new List<IEQ2_E>();
            DetMiembros = new List<IEQ1_E>();
        }

    }
}