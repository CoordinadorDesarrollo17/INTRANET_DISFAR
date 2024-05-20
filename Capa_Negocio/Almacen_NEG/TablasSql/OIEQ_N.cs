using Capa_Datos.Almacen_DAO.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;

namespace Capa_Negocio.Almacen_NEG.TablasSql
{
    public class OIEQ_N
    {
        OIEQ_D oD = new OIEQ_D();
        private bool cadenaVacia(string cad)
        {
            if (cad == null || cad.Replace(" ", "").Length == 0) { return true; }
            else return false;
        }
        public List<OIEQ_E> listarEquipos(OIEQ_E filtro)
        {
            if (OIPE_E.PeriodoSeleccionado == null || OIPE_E.PeriodoSeleccionado.DocEntry == 0) { throw new Exception("No hay periodo Seleccionado"); }
            filtro.DocEntryPer = OIPE_E.PeriodoSeleccionado.DocEntry;
            return oD.listarEquipos(filtro);
        }
        public int registrarNuevoEquipo(OIEQ_E obj)
        {
            validarEquipo(obj);
            return oD.registrarNuevoEquipo(obj);
        }
        public int editarEquipo(OIEQ_E obj)
        {
            validarEquipo(obj);
            return oD.editarEquipo(obj);
        }
        public int eliminarEquipo(int DocEntry)
        {
            OIEQ_E oieqE = buscarEquipos(DocEntry);
            OIPE_N oipeN = new OIPE_N();
            OIPE_E oipeE = oipeN.Buscar(oieqE.DocEntryPer);
            if (oipeE.Estado == "Cerrado") { throw new Exception("No se puede hacer operaciones en periodo cerrado"); }
            // validar que ya conteo en ese equipo
            return oD.eliminarEquipo(DocEntry);
        }
        public OIEQ_E buscarEquipos(int DocEntry)
        {
            return oD.buscarEquipos(DocEntry);
        }
        public List<OIEQ_E> buscarEquipoUsrPer(Usuario_E user, OIPE_E per, OIEQ_E obj = null)
        {
            if (user == null) { return new List<OIEQ_E>(); }
            if (per == null) { return new List<OIEQ_E>(); }
            return oD.buscarEquipoUsrPer(user, per, obj);
        }
        public List<OIEQ_E> buscarPertenenciaEquipo(Usuario_E user, OIPE_E per, string equ)
        {
            if (user == null) { return new List<OIEQ_E>(); }
            if (per == null) { return new List<OIEQ_E>(); }
            if (equ == null) { return new List<OIEQ_E>(); }
            return oD.buscarPertenenciaEquipo(user, per, equ);
        }
        public int separarEquipo(OIEQ_E obj)
        {
            return oD.separarEquipo(obj);
        }
        // metodos
        public void validarEquipo(OIEQ_E obj)
        {
            OIPE_N oipeN = new OIPE_N();
            OIPE_E oipeE = oipeN.Buscar(obj.DocEntryPer);
            if (oipeE.Estado == "Cerrado") { throw new Exception("No se puede hacer operaciones en periodo cerrado"); }

            if (cadenaVacia(obj.Nombre) || obj.Nombre.Length > 50) { throw new Exception("Debe llenar el nombre como maximo 50 caracteres"); }
            if (cadenaVacia(obj.Tipo)) { throw new Exception("Debe seleccionar un tipo "); }
            if (cadenaVacia(obj.Estado)) { throw new Exception("Debe seleccionar un Estado "); }
            if (obj.DocEntryPer <= 0) { throw new Exception("Debe seleccionar un Periodo "); }
            if (cadenaVacia(obj.WhsCode)) { throw new Exception("Debe seleccionar un Almacen "); }
            if (obj.Piso <= 0) { throw new Exception("Debe llenar un Piso con numero mayor a 0 "); }
            if (obj.DetMiembros == null || obj.DetMiembros.Count <= 0) { throw new Exception("Debe llenar por lo menos un miembro al equipo"); }
            if (obj.DetFabricantes == null || obj.DetFabricantes.Count <= 0) { throw new Exception("Debe llenar por lo menos un Laboratorio al equipo"); }
            if (cadenaVacia(obj.Controlados)) { throw new Exception("Debe seleccionar controlados "); }
        }
    }
}