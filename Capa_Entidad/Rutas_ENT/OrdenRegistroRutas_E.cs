using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Rutas_ENT
{
    public class OrdenRegistroRutas_E
    {
        public int DocEntry { get; set; }
        public string DocSerie { get; set; }
        public int DocNum { get; set; }
        public string TipoRuta { get; set; }
        public int TransCod { get; set; }
        public string TransDesc { get; set; }
        public int PlacaCod { get; set; }
        public string PlacaDesc { get; set; }
        public int CopilCod { get; set; }
        public string CopilDesc { get; set; }
        public int Copil2Cod { get; set; }
        public string Copil2Desc { get; set; }
        public int Copil3Cod { get; set; }
        public string Copil3Desc { get; set; }
        public int Copil4Cod { get; set; }
        public string Copil4Desc { get; set; }
        public string FechaCont { get; set; }
        public string FechaDoc { get; set; }
        public string AlmOrigenCod { get; set; }
        public string AlmOrigenDesc { get; set; }
        public string AlmOrigenDesc2 { get; set; }
        public string AlmDestinoCod { get; set; }
        public string AlmDestinoDesc { get; set; }
        public string AlmDestinoDesc2 { get; set; }
        public int PropietarioCod { get; set; }
        public string PropietarioDesc { get; set; }
        public string HoraI { get; set; }
        public string HoraT { get; set; }
        public string Estado { get; set; }
        public int TotalCajas { get; set; }
        public string Observaciones { get; set; }

        public List<DetalleRegistroRutas_E> ListaDetalleRegistroRutas { get; set; }
        public List<RRU1_E> RRU1 { get; set; }
    }
}
