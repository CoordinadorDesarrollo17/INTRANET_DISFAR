using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace Capa_Entidad.Almacen_ENT.TablasSql
{
    public class OIPE_E
    {
        [DisplayName("N#")]
        public int DocEntry { get; set; }
        public string Descripcion { get; set; }
        public string FecIni { get; set; }
        public string FecFin { get; set; }
        public string Observaciones { get; set; }
        public string Propietario { get; set; }
        public string FechaRegistro { get; set; }
        public string HoraRegistro { get; set; }
        public string Estado { get; set; }
        public string OpCarga { get; set; }
        public string FechaCarga { get; set; }
        public string HoraCarga { get; set; }
        public string EstadoDatos { get; set; }
        public string Operario { get; set; }
        // campo no dela tabla
        public List<IPE1_E> DetAlmacenes { get; set; }
        public List<IPE2_E> DetArticulos { get; set; }
        public static OIPE_E PeriodoSeleccionado { get; set; }
        // metodos
        public string enlistarDetAlmacenes()
        {
            string enlista = "";
            foreach (IPE1_E i in DetAlmacenes)
            {
                enlista += "'" + i.WhsCode + "'" + ",";
            }
            return enlista;
        }

    }
}