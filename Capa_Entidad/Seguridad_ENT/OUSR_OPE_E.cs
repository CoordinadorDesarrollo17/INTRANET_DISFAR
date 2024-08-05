namespace Capa_Entidad.Seguridad_ENT
{
    public class OUSR_OPE_E
    {
        public int Id { get; set; }
        public int UsrDocEntry { get; set; }
        public int OpeID { get; set; }

        /********** C A M P O S   Q U E   N O   S O N   D E   L A   T A B L A **********/
        public string OpeNombre { get; set; }
        public int OpeIdModulo{ get; set; }
        public int Grup_OpeID { get; set; }
        public string Grup_OpeControlador { get; set; }
    }
}