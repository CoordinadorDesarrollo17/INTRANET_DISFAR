namespace Capa_Entidad.RecursosHumanos_ENT.TablasSQL
{
    public class EMPL1_E
    {
        public int Id { get; set; }
        public int EmpleadoID { get; set; }
        public string TipoContrato { get; set; }
        public string FechaContratacion { get; set; }
        public decimal Salario { get; set; }
        public string FechaCese { get; set; }
        public int SedeID { get; set; }
        public int DepartamentoID { get; set; }
        public int AreaID{ get; set; }
        public int CargoID { get; set; }
        public string TurnoTrabajo { get; set; }
        public string Discapacidad { get; set; }
        public string CondicionLaboral { get; set; }
        // Campos que se deben normalizar en otra tabla maestra
        public string CorreoCorporativo { get; set; }
        public int NumeroCorporativoID { get; set; }
        public string AnexoCorporativo { get; set; }
        

        // Campos que no son de la tabla
        public string NombreDepartamento { get; set; }
        public string NombreArea { get; set; }
        public string NombreCargo { get; set; }
        public string NumeroCorporativo { get; set; }
        public string NroDocumento { get; set; }
        public string UsuarioOperacion { get; set; }
    }
}
