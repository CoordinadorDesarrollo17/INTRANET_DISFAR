namespace Capa_Entidad.RecursosHumanos_ENT.TablasSQL
{
    public class EMPL1_E
    {
        public int IdEMPL1 { get; set; }
        public int IdOEMPL { get; set; }
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

        // Campos que no son de la tabla
        public string NombreDepartamento { get; set; }
        public string NombreArea { get; set; }
        public string NombreCargo { get; set; }
        public string NumeroCorporativo { get; set; }
        public string NroDocumento { get; set; }
    }
}
