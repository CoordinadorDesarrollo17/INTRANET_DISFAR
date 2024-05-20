using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.ReportesHana
{
    public class AnlVent1_E
    {
        // analisis de ventas1 para rotaciones mensuales de stock en cajas
        public string CardCode { get; set; }
        public string RazonSocial { get; set; }
        public decimal ImporteVentas { get; set; }
        public string FirmName { get; set; }
        public string ItemCode { get; set; }
        public string Articulo { get; set; }
        public decimal CantidadCj { get; set; }
        public decimal PrecioIgvCj { get; set; }
        public string Departamento { get; set; }
        public string Vendedor { get; set; }
        public decimal Ene { get; set; }
        public decimal Feb { get; set; }
        public decimal Mar { get; set; }
        public decimal Abr { get; set; }
        public decimal May { get; set; }
        public decimal Jun { get; set; }
        public decimal Jul { get; set; }
        public decimal Ago { get; set; }
        public decimal Set { get; set; }
        public decimal Oct { get; set; }
        public decimal Nov { get; set; }
        public decimal Dic { get; set; }
    }
}
