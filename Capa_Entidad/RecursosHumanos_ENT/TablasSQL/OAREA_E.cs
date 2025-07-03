namespace Capa_Entidad.RecursosHumanos_ENT.TablasSQL
{
    public class OAREA_E
    {
        public int Id { get; set; } // primary key
        public int Codigo { get; set; }
        public int IdDepartamento { get; set; }

        //entre Codigo y IdDepartamento se genera el codigo segun sap Ejem 17.1
        public string Nombre { get; set; }

        public string Estado { get; set; }
        public string FechaRegistro { get; set; }

        // Campos que no son de la tabla
        public string DescripcionEstado { get; set; }

        public string DepartamentoNombre { get; set; }
    }
}