using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Ventas_ENT.Tablas
{
    public class ORDR_E
    {
        //tabla de orden de venta; cabecera
        public int DocEntry { get; set; }
        [DisplayName("NroVenta")]
        public int DocNum { get; set; }
        public string CANCELED { get; set; }
        [DisplayName("FechaCont")]
        public string DocDate { get; set; } 
        [DisplayName("Cliente")]
        public string CardCode { get; set; }
        [DisplayName("Nombre")]
        public string CardName { get; set; }
        [DisplayName("Total")]
        public decimal DocTotal { get; set; }
        [DisplayName("VendedorCod")]
        public int SlpCode { get; set; }
        [DisplayName("Comentario")]
        public string Comments { get; set; }
        [DisplayName("Estado")]
        public string U_SYP_STATUS { get; set; }
        [DisplayName("LugarDeEntrega")]
        public string U_COB_LUGAREN { get; set; }
        // campos para reportes ajenos a la tabla
        public List<OINV_E> ComprobantesVinculados { get; set; }
        //constructor
        public ORDR_E()
        {
            ComprobantesVinculados = new List<OINV_E>();
        }
    }
}
