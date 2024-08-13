using Capa_Datos.Ventas_DAO.Tablas;
using Capa_Entidad.ComprobantesContables_ENT;
using Capa_Entidad.ReportesDigemid_ENT.Reportes;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Capa_Entidad.Ventas_ENT.Tablas;
using System.Collections.Generic;

namespace Capa_Negocio.Ventas_NEG.Tablas
{
    public class OINV_N
    {
        OINV_D oD = new OINV_D();
        public List<OINV_E> listadoFacturasDeVenta(OINV_E fil)
        {
            return oD.listadoFacturasDeVenta(fil);
        }
        public List<OINV_E> listadoBoletasDeVenta(OINV_E fil)
        {
            return oD.listadoBoletasDeVenta(fil);
        }
        public List<OINV_E> listadoComprobantesPorOrdr(int DocEntryOrden)
        {
            return oD.listadoComprobantesPorOrdr(DocEntryOrden);
        }
        public string CalcularPdfsActaDespachoOINV(string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN, string TipoComprobante)
        {
            return oD.CalcularPdfsActaDespachoOINV(Fecha, U_SYP_STATUS, U_COB_LUGAREN, TipoComprobante);
        }
        public List<TEMP_RRU01_E> NotasDebitoSap(string FBConcatenadas)
        {
            return oD.NotasDebitoSap(FBConcatenadas);
        }
        public List<ComprobanteDePago_E> buscarFacturaBoletaSap(string NumAtCard)
        {
            return oD.buscarFacturaBoletaSap(NumAtCard);
        }
        public List<NotaCreditoDebito_E> buscarNotaDebitoSap(string NumAtCard)
        {
            return oD.buscarNotaDebitoSap(NumAtCard);
        }
        public List<(string, int)> DetalleCalculadoraPdfOINV(string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN, string TipoComprobante)
        {
            return oD.DetalleCalculadoraPdfOINV(Fecha, U_SYP_STATUS, U_COB_LUGAREN, TipoComprobante);
        }
    }
}
