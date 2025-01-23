using Capa_Datos.Almacen_DAO.Tablas;
using Capa_Entidad.Almacen_ENT.Tablas;
using Capa_Entidad.ComprobantesContables_ENT;
using Capa_Entidad.Rutas_ENT.TablasSql;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;

namespace Capa_Negocio.Almacen_NEG.Tablas
{
    public class OWTR_N
    {
        OWTR_D oD = new OWTR_D();
        public List<OWTR_E> listadoTransferenciasStock(OWTR_E fil)
        {
            return oD.listadoTransferenciasStock(fil);
        }
        public List<Guia_Remision_E> buscarGuiaRemisionSap(string NumAtCard)
        {
            return oD.buscarGuiaRemisionSap(NumAtCard);
        }
        public string GuiasTicketTransferencia(int DocNum, string WhsCode,string CardCode)
        {
            return oD.GuiasTicketTransferencia(DocNum, WhsCode,CardCode);
        }
        public string CalcularPdfsActaRecepcion(string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN)
        {
            return oD.CalcularPdfsActaRecepcion(Fecha, U_SYP_STATUS, U_COB_LUGAREN);
        }
        public string CalcularPdfsActaDespachoOWTR(string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN)
        {
            return oD.CalcularPdfsActaDespachoOWTR(Fecha, U_SYP_STATUS, U_COB_LUGAREN);
        }
        public List<(string, int)> DetalleCalculadoraPdf(string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN)
        {
            
            return oD.DetalleCalculadoraPdf(Fecha,U_SYP_STATUS,U_COB_LUGAREN);
        }
        public List<(string, int)> DetalleCalculadoraPdfOWTR(string Fecha, string U_SYP_STATUS, string U_COB_LUGAREN)
        {

            return oD.DetalleCalculadoraPdfOWTR(Fecha, U_SYP_STATUS, U_COB_LUGAREN);
        }
        
    }
}
