namespace Capa_Entidad.Seguridad_ENT
{
    public class Usuario_E
    {
        public int DocEntry { get; set; }
        public string Prefijo { get; set; }
        public string Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int IdRol { get; set; }
        public int? Activo { get; set; }
        public string FechaRegistro { get; set; }
        public string HoraRegistro { get; set; }
        public string OperarioRegistro { get; set; }
        public string WhsCode { get; set; }
        public int CodigoSap { get; set; }
        public string ClaveEmail { get; set; }
        public string Usuario { get; set; }

        /******************** C A M P O S   Q U E   N O   S O N   D E   L A   T A B L A ********************/
        public string DescripcionRol { get; set; }
        public int DiferenciaDias { get; set; }         // Campo calculado para ver dias sin conexion
        public string Password2 { get; set; }
        public int EmpleadoID { get; set; }
        public string EmpleadoCobefar { get; set; }     // Campo para selección SI/NO
    }
    public class Credenciales_E
    { 
    public string correo { get; set; }
    public string password { get; set; }
    
    }
}