using Capa_Entidad.Seguridad_ENT;

namespace Capa_Entidad.Ventas_ENT.Formularios
{
    public class FrmAnCtVentas_E
    {
        public bool Encarg { get; set; }
        public int ÝearU { get; set; }
        public int MonthU { get; set; }
        public int DayU { get; set; }
        public int CodigoSap { get; set; }
        public int DocEntry { get; set; }
        public string nombre { get; set; }
        public string FecIni { get; set; }
        public string FecFin { get; set; }
        public Usuario_E user { get; set; }
        public string Usuario { get; set; }
	}
}
