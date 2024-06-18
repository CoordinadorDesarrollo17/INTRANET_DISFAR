using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Entidad.Almacen_ENT.Tablas
{
    public class OITM_E
    {
        // datos maestros de articulo
        [DisplayName("Numero de Articulo")]
        public string ItemCode { get; set; }
        [DisplayName("Descripcion")]
        public string ItemName { get; set; }
        [DisplayName("Nombre Extranjero")]
        public string FrgnName { get; set; }
        [DisplayName("Grupo de Articulos")]
        public int ItmsGrpCod { get; set; }
        [DisplayName("Grupo unid. Med.")]
        public string BuyUnitMsr { get; set; }
        [DisplayName("Precio por Unidad")]
        public decimal Price { get; set; }
        [DisplayName("Linea Laboratorio")]
        public string FirmName { get; set; }
        [DisplayName("Estado")]
        public int FirmCode { get; set; }
        public string AsstStatus { get; set; }
        public string validFor { get; set; }
        public string U_COB_EST_SKU { get; set; }
        [DisplayName("Observacion")]
        public string UserText { get; set; }
        [DisplayName("En Stock")]
        public decimal OnHand { get; set; }
        [DisplayName("Familia-Codigo")]
        public string U_SYP_FAMILIA { get; set; }
        [DisplayName("Familia-Descripcion")]
        public string U_SYP_DFAM { get; set; }
        [DisplayName("Subfamilia-Codigo")]
        public string U_SYP_SUBFAMILIA { get; set; }
        [DisplayName("SubFamilia-Descripcion")]
        public string U_SYP_DSFAM { get; set; }
        [DisplayName("Forma Farmaceutica")]
        public string U_SYP_FORM { get; set; }
        [DisplayName("Desc.Form.Simpl")]
        public string U_SYP_FFSIMP { get; set; }
        [DisplayName("Desc.Form.Det")]
        public string U_SYP_FFDET { get; set; }
        [DisplayName("FormaPresentacion")]
        public string U_SYP_FORPR { get; set; }
        [DisplayName("Fabricante")]
        public string U_SYP_FABRICANTE { get; set; }
        [DisplayName("TipoDeControlado")]
        public string U_SYP_TCONTROLADO { get; set; }
        public decimal NumInBuy { get; set; }
        //
        public string Status { get; set; }
    }
}
