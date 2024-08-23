namespace Capa_Entidad.RecursosHumanos_ENT.TablasSQL
{
    public class OEMPL_E
    {
        public int Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string FechaNacimiento { get; set; }
        public string EstadoCivil { get; set; }
        public string Genero { get; set; }
        public string Direccion { get; set; }
        public int UbigeoID { get; set; }
        public string ReferenciaDomicilio { get; set; }
        public string Nacionalidad { get; set; }
        public string CorreoElectronico { get; set; }
        public string Celular { get; set; }
        public string LicenciaConducir { get; set; }
        public string NombreContactoEmergencia { get; set; }
        public string CelularContactoEmergencia { get; set; }
        public string PrefijoId { get; set; }
        public string Estado { get; set; }

        // Campos que no son de la tabla
        public string DescripcionEstado { get; set; }
        public string NombresApellidos { get; set; }
        public EMPL1_E DatosLaborales { get; set; }
        public int PaginacionResultados { get; set; }
        public string MostrarNotificacionCambio { get; set; }
    }
}