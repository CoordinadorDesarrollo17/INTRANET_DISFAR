using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Compras_ENT.Tablas
{
    public class OPDN_E
    {
        //entrada de mercancias
        public int DocEntry { get; set; }
        [DisplayName("NroDocumento")]
        public int DocNum { get; set; }
        [DisplayName("FechaCont")]
        public string DocDate { get; set; }
        [DisplayName("NombreProv.")]
        public string CardName { get; set; }
        [DisplayName("Guia")]
        public string NumAtCard { get; set; }
        [DisplayName("Total")]
        public decimal DocTotal { get; set; }
        [DisplayName("EstadoDoc")]
        public string U_SYP_STATUS { get; set; }
        // no esta en la tabla
        public string Almacen { get; set; }
        public SQL_OPDN_E sqlopdn { get; set; }
    }
}
