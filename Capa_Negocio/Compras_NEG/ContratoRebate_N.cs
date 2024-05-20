using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa_Datos.Compras_DAO;
using Capa_Entidad.Compras_ENT;
using Capa_Entidad.Compras_ENT.Reportes;

namespace Capa_Negocio.Compras_NEG
{
    public class ContratoRebate_N
    {
        ContratoRebate_D crD = new ContratoRebate_D();
        public List<ContratoRebate_E> listarContratosRebate(ContratoRebate_E c)
        {
            return crD.listarContratosRebate(c);
        }
        public ContratoRebate_E obtenerContratoRebate(int id)
        {
            return crD.obtenerContratoRebate(id);
        }
        public int EliminarContratoRebate(int id)
        {
            return crD.EliminarContratoRebate(id);
        }
        //reportes
        public CuadreContrato_E GenerarCuadreContrato(int id) // conexion a hana
        {
            return crD.GenerarCuadreContrato(id);
        }
        public List<ResumenRebate_E> listarResumenCuadreContrato(DetCuadreContrato_E filtro,int año)
        {
            return crD.listarResumenCuadreContrato(filtro,año);
        }
        //infos
        public string infoListarSocios(string Tipo)
        {
            return crD.infoListarSocios(Tipo);
        }
        public string infoListarDetallesContratoRebate(ContratoRebate_E c)
        {
            return crD.infoListarDetallesContratoRebate(c);
        }
        public string infoListarEspDetCR2(string PeriodoRebate, int Linea, string SubTipo, string CardCode, int idLp, string FecIni, string FecFin)
        {
            return crD.infoListarEspDetCR2(PeriodoRebate, Linea, SubTipo, CardCode, idLp, FecIni, FecFin);
        }
        public int CrearContratoRebate(ContratoRebate_E c)
        {
            validarContrato(c);
            return crD.CrearContratoRebate(c);
        }
        public int EditarContratoRebate(string TipoMantenimiento, ContratoRebate_E c)
        {
            return crD.EditarContratoRebate(TipoMantenimiento, c);
        }
        public void validarContrato(ContratoRebate_E c)
        {
            if(c.SocioDesc==null || c.SocioCod == null) { throw new Exception(" Los datos del Socio incompletos"); }
            if (c.Titulo == null) { throw new Exception("Debe llenar el titulo"); }
            if (c.Tipo == null) { throw new Exception("Debe elegir tipo de contrato"); }
            if (c.PerValIni == null) { throw new Exception("Debe llenar perInicial"); }
            if (c.PerValIni == null) { throw new Exception("Debe llenar perFinal"); }
            if (c.Det == null) { throw new Exception("Debe llenar detalles"); }
            else
            {
                if (c.Det[0].PeriodoRebate == null) { throw new Exception("Debe llenar al menos 1 periodo"); }
                if (c.Det[0].EspDet2 == null) { throw new Exception("No lleno especificaciones de linea de produccion 1"); }
            }
        }
    }
}
