namespace Capa_Entidad.RecursosHumanos_ENT.TablasSQL
{
    public class HIST_OEMPL_E
    {
        public int IdOEMPL { get; set; }
        public string EstadoCivil { get; set; }
        public string Direccion { get; set; }
        public int Ubigeo { get; set; }
        public string ReferenciaDomicilio { get; set; }
        public string CorreoElectronico { get; set; }
        public string Celular { get; set; }
        public string NombreContactoEmergencia { get; set; }
        public string CelularContactoEmergencia { get; set; }
        public string TipoContrato { get; set; }
        public string FechaContratacion { get; set; }
        public decimal Salario { get; set; }
        public string FechaCese { get; set; }
        public int IdSede { get; set; }
        public int IdDepartamento { get; set; }
        public int IdArea { get; set; }
        public int IdCargo { get; set; }
        public int IdNumeroCorporativo { get; set; }
        public string AnexoCorporativo { get; set; }
        public string CorreoCorporativo { get; set; }
        public string TurnoTrabajo { get; set; }
        public string Discapacidad { get; set; }
        public int RegistradoPor { get; set; }
        public string FechaRegistro { get; set; }
        public string HoraRegistro { get; set; }
    }
}
