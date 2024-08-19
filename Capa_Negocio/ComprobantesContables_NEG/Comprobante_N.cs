using Capa_Datos.ComprobantesContables_ENT;
using Capa_Entidad.ComprobantesContables_ENT;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.ComprobantesContables_NEG
{
    public class Comprobante_N
    {
        Comprobante_D compD = new Comprobante_D();
        public List<Comprobante_E> ObtenerEncabezadoGuias(int DocEntry) //Metodo para traer datos principales de guia remision 
        { return compD.ObtenerEncabezadoGuias(DocEntry); }
        public List<Comprobante_E> ObtenerEncabezadoGuiasTransferencia(int DocNum,string WhsCode) //Metodo para traer datos principales de guia remision en transferencias ( casos centro y arriola) 
        { return compD.ObtenerEncabezadoGuiasTransferencia(DocNum,WhsCode); }
        public List<Comprobante_E> ObtenerEncabezadoFacturas(int DocEntryOrden,string LugarDestino)
        {return compD.ObtenerEncabezadoFacturas(DocEntryOrden,LugarDestino);}
        public List<Comprobante_E> ObtenerEncabezadoNotaCredito(List<RTV4_E> NotasCredito, string FacturasConcatenadas)
        { return compD.ObtenerEncabezadoNotaCredito(NotasCredito, FacturasConcatenadas); }
        public List<Comprobante_E> ObtenerEncabezadoNotaDebito(int DocNum, string FacturasConcatenadas)
        { return compD.ObtenerEncabezadoNotaDebito(DocNum,FacturasConcatenadas); }
    }
}
