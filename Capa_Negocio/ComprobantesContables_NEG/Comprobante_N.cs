using Capa_Datos.ComprobantesContables_ENT;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.ComprobantesContables_ENT;
using Capa_Entidad.ReportesDigemid_ENT.Reportes;
using Capa_Entidad.Ventas_ENT.TablasSql;
using Sap.Data.Hana;
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
        public List<int> ObtenerDocEntryOV(List<RTV2_E> det2List,bool excluirCero)
        {
            return compD.ObtenerDocEntryOV(det2List, excluirCero);
        }
        public List<Guia_Remision_E> ObtenerDetalleGuia(string NumAtCard,string Tabla)
        {return compD.ObtenerDetalleGuia(NumAtCard, Tabla);}
        public List<ComprobanteDePago_E> ObtenerDetalleFactura(string NumAtCard)
        { return compD.ObtenerDetalleFactura(NumAtCard); }
        public List<Comprobante_E> ObtenerEncabezadoGuiasPorEntrega(List<int> listDocEntrySap) //Metodo para traer datos principales de guia remision en caso de Domicilio y Agencia
        { return compD.ObtenerEncabezadoGuiasPorEntrega(listDocEntrySap); }
        public List<Comprobante_E> ObtenerEncabezadoGuiasTransferencia(ORTV_E obj) //Metodo para traer datos principales de guia remision en transferencias ( casos centro y arriola) 
        { return compD.ObtenerEncabezadoGuiasTransferencia(obj); }
        public List<Comprobante_E> ObtenerEncabezadoFacturas(int DocEntryOrden,string LugarDestino)
        {return compD.ObtenerEncabezadoFacturas(DocEntryOrden,LugarDestino);}
        public List<Comprobante_E> ObtenerEncabezadoNotaCredito(List<RTV4_E> NotasCredito, string FacturasConcatenadas)
        { return compD.ObtenerEncabezadoNotaCredito(NotasCredito, FacturasConcatenadas); }
        public List<Comprobante_E> ObtenerEncabezadoNotaDebito(string FacturasConcatenadas)
        { return compD.ObtenerEncabezadoNotaDebito(FacturasConcatenadas); }
    }
}
