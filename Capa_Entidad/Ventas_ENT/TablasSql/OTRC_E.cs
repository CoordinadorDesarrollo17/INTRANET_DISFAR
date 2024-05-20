using System;

namespace Capa_Entidad.Ventas_ENT.TablasSql
{
    public class OTRC_E
    {
        public int Id { get; set; }
        public int IdReg { get; set; }
        public string RegName { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Sentido { get; set; }
        public string Detalle { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Imputado { get; set; }
        public string Operario { get; set; }
        public string FechaRegistro { get; set; }
        public string HoraRegistro { get; set; }
        // campos no de la tabla
        public DateTime TiempoIni { get; set; }
        public DateTime TiempoFin { get; set; }
        public class RptTransacciones
        {
            public int Id { get; set; }
            public int IdReg { get; set; }
            public string RegName { get; set; }
            public string CardCode { get; set; }
            public string CardName { get; set; }
            public string Sentido { get; set; }
            public string Detalle { get; set; }
            public decimal Cantidad { get; set; }
            public decimal Imputado { get; set; }
            public string Operario { get; set; }
            public string FechaRegistro { get; set; }
            public string HoraRegistro { get; set; }
        }
    }
}