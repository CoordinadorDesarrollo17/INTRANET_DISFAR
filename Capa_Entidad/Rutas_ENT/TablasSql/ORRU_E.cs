using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Capa_Entidad.Rutas_ENT.TablasSql
{
    public class ORRU_E
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string TipoRuta { get; set; }
        public string TransCod { get; set; }
        public string TransDesc { get; set; }
        public string VehiculoCod { get; set; }
        public string Placa { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string CopilDesc { get; set; }
        public string Copil2Desc { get; set; }
        public string Copil3Desc { get; set; }
        public string Copil4Desc { get; set; }
        public DateTime? FechaCont { get; set; }
        public DateTime? FechaDoc { get; set; }
        public string AlmOrigenCod { get; set; }
        [DisplayName("Almacen de Origen")]
        public string AlmOrigenDesc { get; set; }
        public string AlmOrigenDesc2 { get; set; }
        public string AlmDestinoCod { get; set; }
        [DisplayName("Almacen de Destino")]
        public string AlmDestinoDesc { get; set; }
        public string AlmDestinoDesc2 { get; set; }
        public string Propietario { get; set; }
        public string HoraRegistro { get; set; }
        public DateTime? TiempoPac { get; set; }
        public string TiempoIniEn { get; set; }
        public string TiempoTerEn { get; set; }
        public string Estado { get; set; }
        public string Observaciones { get; set; }
        public string Operario { get; set; }
        public string OpInicio { get; set; }
        public string OpTermino { get; set; }
        public string Agencia { get; set; }
        [DisplayName("Ruc de Agencia")]
        public string RucAgencia { get; set; }
        public string Origen { get; set; }
        public List<RRU0_E> DetRRU0 { get; set; }
        public List<RRU01_E> DetRRU01 { get; set; }
        public List<RRU1_E> DetRRU1 { get; set; }
        public List<RRU11_E> DetRRU11 { get; set; }
        public int TotCajas()
        {
            int tot = 0;
            if (DetRRU0 != null)
            {
                tot += DetRRU0.Where(x => x.Estado != "LIBERADO").Sum(x => x.Cajas);
            }
            if (DetRRU1 != null)
            {
                tot += DetRRU1.Where(x => x.Estado != "LIBERADO").Sum(x => x.Cajas);
            }
            return tot;
        }
        /* campos no de la tabla */
        public string SerieT1 { get; set; }
        public string SerieT2 { get; set; }
        public int TotalCajas { get; set; }
        public string AlmIni { get; set; }
        public string AlmFin { get; set; }
        public string FecConIni { get; set; }
        public string FecConFin { get; set; }
        public string FechaRegistroDesde { get; set; }
        public string FechaRegistroHasta{ get; set; }
        public decimal MontoTotalIni { get; set; }
        public decimal MontoTotalFin { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public class RptRutas
        {
            public string TransDesc { get; set; }
            public string TipoRuta { get; set; }
            public string CopilDesc { get; set; }
            public string Copil2Desc { get; set; }
            public string Copil3Desc { get; set; }
            //public string Copil4Desc { get; set; }
            public string Placa { get; set; }
            public string AlmOrigenDesc { get; set; }
            public string AlmDestinoDesc { get; set; }
            public string Propietario { get; set; }
            //public string Observaciones { get; set; }
            public int DocNum { get; set; }
            //public string FechaCont { get; set; }
            public string FechaDoc { get; set; }
            public string Hora { get; set; }
            public string Estado { get; set; }
            public int Linea { get; set; }
            public string CardCode { get; set; }
            public string CardName { get; set; }
            public int DocNumTicket { get; set; }
            public string Guias { get; set; }
            public int Cajas { get; set; }
            //public string Observaciones2 { get; set; }
            public string DirDestino { get; set; }
            public string Distrito1 { get; set; }
            public string Provincia1 { get; set; }
            public string Departamento1 { get; set; }
            //public string DirDestino2 { get; set; }
            //public string Distrito2 { get; set; }
            //public string Provincia2 { get; set; }
            //public string Departamento2 { get; set; }
            public decimal MontoTotal { get; set; }
            public decimal MontoFinal { get; set; }
            public string Agencia { get; set; }
            public decimal Flete { get; set; }
            public decimal GastoEnvio { get; set; }
            public string TipoVenta { get; set; }
            public string ZonaVenta { get; set; }
            public string FechaEntregaVenta { get; set; }
            public string HoraEntregaVenta { get; set; }
            public decimal PesoTotalVenta { get; set; }
            public string FormaPagoVenta { get; set; }
            public string TipoPagoRepartoContraEntrega { get; set; }
            public string FechaInicioReparto { get; set; }
            public string HoraInicioReparto { get; set; }
            public string FechaFinReparto { get; set; }
            public string HoraFinReparto { get; set; }
            public string ComentarioLiberado { get; set; }
            public decimal MontoRecibidoDepositoContraEntrega { get; set; }
            public decimal MontoRecibidoEfectivoContraEntrega { get; set; }
            public string FechaPagoVenta { get; set; }
            public string HoraPagoVenta { get; set; }
            public string FechaEntregaReparto { get; set; }
            public string HoraEntregaReparto { get; set; }
            public string FechaPreenvio { get; set; }
            public string FechaEnviado { get; set; }
            public string NombreRecoge { get; set; }
        }
        public class RptRutasT
        {
            public string TipoRuta { get; set; }
            public int DocNum { get; set; }
            public string FechaDoc { get; set; }
            [DisplayName("Almacén de Origen")]
            public string AlmOrigenDesc { get; set; }

            [DisplayName("Almacén de Destino")]
            public string AlmDestinoDesc { get; set; }

            [DisplayName("Transportista")]
            public string TransDesc { get; set; }
            public string Placa { get; set; }
            public string Guia { get; set; }
            public int NroSap { get; set; }
            public string Estado { get; set; }
        }
        public class RptRutasDet
        {
            public string TransDesc { get; set; }
            public string TipoRuta { get; set; }
            public string CopilDesc { get; set; }
            public string Copil2Desc { get; set; }
            public string Copil3Desc { get; set; }
            public string Copil4Desc { get; set; }
            public string Placa { get; set; }
            public string AlmOrigenDesc { get; set; }
            public string AlmDestinoDesc { get; set; }
            public string Propietario { get; set; }
            public string Observaciones { get; set; }
            public int DocNum { get; set; }
            public string FechaCont { get; set; }
            public string FechaDoc { get; set; }
            public string Hora { get; set; }
            public string Estado { get; set; }
            public int Linea { get; set; }
            public string CardCode { get; set; }
            public string CardName { get; set; }
            public int DocNumTicket { get; set; }
            public string Guias { get; set; }
            public int Cajas { get; set; }
            public string Observaciones2 { get; set; }
            public string DirDestino { get; set; }
            public string Distrito1 { get; set; }
            public string Provincia1 { get; set; }
            public string Departamento1 { get; set; }
            public string DirDestino2 { get; set; }
            public string Distrito2 { get; set; }
            public string Provincia2 { get; set; }
            public string Departamento2 { get; set; }
            public decimal MontoTotal { get; set; }
            public decimal MontoFinal { get; set; }
            public decimal Flete { get; set; }
            public decimal GastoEnvio { get; set; }
            public string TipoVenta { get; set; }
            public int LineaPeso { get; set; }
            public decimal Peso { get; set; }
            public string UniMed { get; set; }
            public decimal PrecioEnv { get; set; }
        }

        public class OrdenDevolucionHana
        {
            public int DocEntry { get; set; }
            public int DocNum { get; set; }
            public string CardCode { get; set; }
            public string CardName { get; set; }
            public string Address { get; set; }
            public string Comments { get; set; }
            public string JrnlMemo { get; set; }
            public string NumAtCard { get; set; }      // Guía concatenada
            public string TipoMotivo { get; set; }     // U_SYP_MDMT
            public string Agencia { get; set; }        // U_COB_LUGAREN
            public string NumDocConductor { get; set; }// U_BPP_NUDOCCOND
            public string Conductor { get; set; }      // U_SYP_MDFN
            public string Placa { get; set; }          // U_SYP_MDVC
            public int Bultos { get; set; }
            public decimal DocTotal { get; set; }
        }
    }
}