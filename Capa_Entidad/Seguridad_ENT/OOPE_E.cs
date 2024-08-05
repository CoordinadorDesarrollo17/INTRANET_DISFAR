namespace Capa_Entidad.Seguridad_ENT
{
    public class OOPE_E
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public int idModulo { get; set; }
        public int Grup_OpeID { get; set; }

        /******* C A M P O S   Q U E   N O   S O N   D E   L A   T A B L A *******/
        public string Controlador { get; set; }
    }
}