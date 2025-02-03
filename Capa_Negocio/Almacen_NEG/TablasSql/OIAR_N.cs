using Capa_Datos.Almacen_DAO.TablasSql;
using Capa_Entidad.Seguridad_ENT;
using Capa_Entidad.Almacen_ENT.TablasSql;
using System;
using System.Collections.Generic;

namespace Capa_Negocio.Almacen_NEG.TablasSql
{
    public class OIAR_N
    {
        OIAR_D oiarD = new OIAR_D(); OIPE_D oipeD = new OIPE_D(); OIEQ_E oieqE = new OIEQ_E(); OIEQ_N oieqN = new OIEQ_N();

        public List<OIAR_E> Listar(OIAR_E filtro, Usuario_E user, string tipo)
        {
            if (OIPE_E.PeriodoSeleccionado == null || OIPE_E.PeriodoSeleccionado.DocEntry == 0) { throw new Exception("No hay periodo Seleccionado"); }
            filtro.DocEntryPer = OIPE_E.PeriodoSeleccionado.DocEntry;
            validarOperacionesDelPeriodo(filtro.DocEntryPer);
            return oiarD.Listar(filtro, user, tipo);
        }
        public int Registrar(OIAR_E obj)
        {
            validarNuevoConteo(obj);
            return oiarD.Registrar(obj);
        }
        public OIAR_E Buscar(int DocEntry)
        {
            return oiarD.Buscar(DocEntry);
        }
        public int IniciarConteo(OIAR_E obj)
        {
            validarInicioConteo(obj);
            return oiarD.IniciarConteo(obj);
        }
        public int RevertirIniciarConteo(OIAR_E obj)
        {
            validarRevertirInicioConteo(obj);
            return oiarD.RevertirIniciarConteo(obj.DocEntry);
        }
        public int TerminarConteo(OIAR_E obj)
        {
            validarTerminoConteo(obj);
            return oiarD.TerminarConteo(obj);
        }
        public int RevertirTerminoConteo(OIAR_E obj)
        {
            validarRevertirTerminoConteo(obj);
            return oiarD.RevertirTerminoConteo(obj.DocEntry);
        }
        public int IniciarReconteo(OIAR_E obj)
        {
            validarInicioReconteo(obj);
            return oiarD.IniciarReconteo(obj);
        }
        public int RevertirIniciarReconteo(OIAR_E obj)
        {
            validarRevertirInicioReconteo(obj);
            return oiarD.RevertirIniciarReconteo(obj.DocEntry);
        }
        public int TerminarReconteo(OIAR_E obj)
        {
            validarTerminoReconteo(obj);
            return oiarD.TerminarReconteo(obj);
        }
        public int RevertirTerminoReconteo(OIAR_E obj)
        {
            validarRevertirTerminoReconteo(obj);
            return oiarD.RevertirTerminoReconteo(obj.DocEntry);
        }
        public int IniciarAnalisisConteo(OIAR_E obj)
        {
            validarInicioAnalisisConteo(obj);
            return oiarD.IniciarAnalisisConteo(obj);
        }
        public int RevertirIniciarAnalisisConteo(OIAR_E obj)
        {
            validarRevertirInicioAnalisisConteo(obj);
            return oiarD.RevertirIniciarAnalisisConteo(obj.DocEntry);
        }
        public int TerminarAnalisisConteo(OIAR_E obj)
        {
            validarTerminoAnalisisConteo(obj);
            return oiarD.TerminarAnalisisConteo(obj);
        }
        public int RevertirTerminoAnalisisConteo(OIAR_E obj)
        {
            validarRevertirTerminoAnalisisConteo(obj);
            return oiarD.RevertirTerminoAnalisisConteo(obj.DocEntry);
        }
        // metodos para validaciones
        public void validarNuevoConteo(OIAR_E obj)
        {
            validarOperacionesDelPeriodo(obj.DocEntryPer);
            if (obj.DocEntryEqu == 0) { throw new Exception("Seleccione un equipo de trabajo"); }
            if (string.IsNullOrWhiteSpace(obj.ItemCode)) { throw new Exception("Seleccione un laboratorio y articulo"); }
            if (obj.FirmCode == 0)
            {
                obj.FirmCode = obj.FirmCode2;
            }
        }
        public void validarOperacionesDelPeriodo(int DocEntryPer)
        {
            if (OIPE_E.PeriodoSeleccionado == null || OIPE_E.PeriodoSeleccionado.DocEntry == 0) { throw new Exception(" No hay periodo seleccionado"); }
            else
            {
                OIPE_E periodoSel = oipeD.Buscar(OIPE_E.PeriodoSeleccionado.DocEntry, false);
                if (periodoSel.DocEntry != DocEntryPer) { throw new Exception("No se puede realizar operacion en un periodo que no este seleccionado"); }
                if (periodoSel.Estado == "Cerrado")
                { throw new Exception(" No se pueden hacer operaciones en periodo Cerrado"); }
                else if (periodoSel.EstadoDatos != "Cargado") { throw new Exception("No se puede realizar operacion en un periodo que no tenga datos Cargados"); }
            }
        }
        public void validarVistaReportes(int DocEntryPer)
        {
            if (OIPE_E.PeriodoSeleccionado == null || OIPE_E.PeriodoSeleccionado.DocEntry == 0) { throw new Exception(" No hay periodo seleccionado"); }
            else
            {
                OIPE_E periodoSel = oipeD.Buscar(OIPE_E.PeriodoSeleccionado.DocEntry, false);
                if (periodoSel.DocEntry != DocEntryPer) { throw new Exception("No se puede realizar operacion en un periodo que no este seleccionado"); }
                else if (periodoSel.EstadoDatos != "Cargado") { throw new Exception("No se puede realizar operacion en un periodo que no tenga datos Cargados"); }
            }
        }
        public void validarInicioConteo(OIAR_E obj)
        {
            OIAR_E bean = Buscar(obj.DocEntry);
            validarOperacionesDelPeriodo(bean.DocEntryPer);
            if (bean.Estado != "Pendiente") { throw new Exception("Solo se puede iniciar un conteo en estado Pendiente"); }
        }
        public void validarRevertirInicioConteo(OIAR_E obj)
        {
            OIAR_E bean = Buscar(obj.DocEntry);
            validarOperacionesDelPeriodo(bean.DocEntryPer);
            if (bean.Estado != "EnConteo") { throw new Exception("Solo se puede revertir si estado esta EnConteo"); }
            if (bean.DetFases[0].Operario != obj.DetFases[0].Operario) { throw new Exception("Solo " + bean.DetFases[0].Operario + " puede revertir su inicio de conteo"); }
        }
        public void validarTerminoConteo(OIAR_E obj)
        {
            OIAR_E bean = Buscar(obj.DocEntry);
            validarOperacionesDelPeriodo(bean.DocEntryPer);
            if (obj.DetFases == null || obj.DetFases.Count == 0) { throw new Exception(" Hubo un error al grabar los detalles"); }
            if (obj.DetFases[0].DetContab == null || obj.DetFases[0].DetContab.Count == 0) { throw new Exception("La tabla no contiene ningun lote"); }
            int i = 0;
            foreach (IAR12_E iar12 in obj.DetFases[0].DetContab)
            {
                if (string.IsNullOrWhiteSpace(iar12.BatchNum)) { throw new Exception(" El lote en la linea " + (i + 1) + " no puede estar vacia"); }
                if (string.IsNullOrWhiteSpace(iar12.ExpDate)) { throw new Exception(" La Fecha Venc. en la linea " + (i + 1) + " no puede estar vacia"); }
                if (iar12.QuantityCajas < 0) { throw new Exception(" El stock en cajas no puede ser negativo en linea " + (i + 1)); }
                else if (iar12.QuantityCajas == 0)
                {
                    if (iar12.QuantityPiezas <= 0) { throw new Exception(" El stock en piezas debe tener al menos 1 pieza en linea " + (i + 1)); }
                }
                if (iar12.QuantityPiezas < 0) { throw new Exception(" El stock en piezas no puede ser negativo en linea " + (i + 1)); }
                i++;
            }
            if (bean.Estado != "EnConteo") { throw new Exception(" Solo se puede terminar el conteo en estado EnConteo"); }
            if (bean.DetFases[0].Operario != obj.DetFases[0].Operario) { throw new Exception("Solo " + bean.DetFases[0].Operario + " puede terminar su conteo"); }
        }
        public void validarRevertirTerminoConteo(OIAR_E obj)
        {
            OIAR_E bean = Buscar(obj.DocEntry);
            validarOperacionesDelPeriodo(bean.DocEntryPer);
            if (bean.Estado != "Contado") { throw new Exception("Solo se puede revertir si estado esta Contado"); }
            if (bean.DetFases[1].Operario != obj.DetFases[0].Operario) { throw new Exception("Solo " + bean.DetFases[1].Operario + " puede revertir su termino de conteo"); }
        }
        public void validarInicioReconteo(OIAR_E obj)
        {
            OIAR_E bean = Buscar(obj.DocEntry);
            validarOperacionesDelPeriodo(bean.DocEntryPer);
            if (bean.Estado != "Contado") { throw new Exception("Solo se puede iniciar un reconteo en estado Contado"); }
        }
        public void validarRevertirInicioReconteo(OIAR_E obj)
        {
            OIAR_E bean = Buscar(obj.DocEntry);
            validarOperacionesDelPeriodo(bean.DocEntryPer);
            if (bean.Estado != "EnReconteo") { throw new Exception("Solo se puede revertir si estado esta EnReconteo"); }
            if (bean.DetFases[2].Operario != obj.DetFases[0].Operario) { throw new Exception("Solo " + bean.DetFases[2].Operario + " puede revertir su inicio de reconteo"); }
        }
        public void validarTerminoReconteo(OIAR_E obj)
        {
            OIAR_E bean = Buscar(obj.DocEntry);
            validarOperacionesDelPeriodo(bean.DocEntryPer);
            if (obj.DetFases == null || obj.DetFases.Count == 0) { throw new Exception(" Hubo un error al grabar los detalles"); }
            if (obj.DetFases[0].DetContab == null || obj.DetFases[0].DetContab.Count == 0) { throw new Exception("La tabla no contiene ningun lote"); }
            int i = 0;
            foreach (IAR12_E iar12 in obj.DetFases[0].DetContab)
            {
                if (string.IsNullOrWhiteSpace(iar12.BatchNum)) { throw new Exception(" El lote en la linea " + (i + 1) + " no puede estar vacia"); }
                if (string.IsNullOrWhiteSpace(iar12.ExpDate)) { throw new Exception(" La Fecha Venc. en la linea " + (i + 1) + " no puede estar vacia"); }
                if (iar12.QuantityCajas < 0) { throw new Exception(" El stock en cajas no puede ser negativo en linea " + (i + 1)); }
                else if (iar12.QuantityCajas == 0)
                {
                    if (iar12.QuantityPiezas <= 0) { throw new Exception(" El stock en piezas debe tener al menos 1 pieza en linea " + (i + 1)); }
                }
                if (iar12.QuantityPiezas < 0) { throw new Exception(" El stock en piezas no puede ser negativo en linea " + (i + 1)); }
                i++;
            }
            if (bean.Estado != "EnReconteo") { throw new Exception(" Solo se puede terminar el reconteo en estado EnReconteo"); }
            if (bean.DetFases[2].Operario != obj.DetFases[0].Operario) { throw new Exception("Solo " + bean.DetFases[2].Operario + " puede terminar su reconteo"); }
            // obj.DetFases[0].TiempoFase = DateTime.Now;
        }
        public void validarRevertirTerminoReconteo(OIAR_E obj)
        {
            OIAR_E bean = Buscar(obj.DocEntry);
            validarOperacionesDelPeriodo(bean.DocEntryPer);
            if (bean.Estado != "ReContado") { throw new Exception("Solo se puede revertir si estado esta ReContado"); }
            if (bean.DetFases[3].Operario != obj.DetFases[0].Operario) { throw new Exception("Solo " + bean.DetFases[3].Operario + " puede revertir su termino de reconteo"); }
        }
        public void validarInicioAnalisisConteo(OIAR_E obj)
        {
            OIAR_E bean = Buscar(obj.DocEntry);
            validarOperacionesDelPeriodo(bean.DocEntryPer);
            if (bean.Estado != "ReContado") { throw new Exception("Solo se puede iniciar un analisis de conteo en estado ReContado"); }
        }
        public void validarRevertirInicioAnalisisConteo(OIAR_E obj)
        {
            OIAR_E bean = Buscar(obj.DocEntry);
            validarOperacionesDelPeriodo(bean.DocEntryPer);
            if (bean.Estado != "EnAnalisis") { throw new Exception("Solo se puede revertir si estado esta EnAnalisis"); }
            if (bean.DetFases[4].Operario != obj.DetFases[0].Operario) { throw new Exception("Solo " + bean.DetFases[4].Operario + " puede revertir su inicio de analisis"); }
        }
        public void validarTerminoAnalisisConteo(OIAR_E obj)
        {
            OIAR_E bean = Buscar(obj.DocEntry);
            validarOperacionesDelPeriodo(bean.DocEntryPer);
            if (obj.DetFases == null || obj.DetFases.Count == 0) { throw new Exception(" Hubo un error al grabar los detalles"); }
            if (obj.DetFases[0].DetContab == null || obj.DetFases[0].DetContab.Count == 0) { throw new Exception("La tabla no contiene ningun lote"); }
            int i = 0;
            foreach (IAR12_E iar12 in obj.DetFases[0].DetContab)
            {
                if (string.IsNullOrWhiteSpace(iar12.BatchNum)) { throw new Exception(" El lote en la linea " + (i + 1) + " no puede estar vacia"); }
                if (string.IsNullOrWhiteSpace(iar12.ExpDate)) { throw new Exception(" La Fecha Venc. en la linea " + (i + 1) + " no puede estar vacia"); }
                if (iar12.QuantityCajas < 0) { throw new Exception(" El stock en cajas no puede ser negativo en linea " + (i + 1)); }
                else if (iar12.QuantityCajas == 0)
                {
                    if (iar12.QuantityPiezas <= 0) { throw new Exception(" El stock en piezas debe tener al menos 1 pieza en linea " + (i + 1)); }
                }
                if (iar12.QuantityPiezas < 0) { throw new Exception(" El stock en piezas no puede ser negativo en linea " + (i + 1)); }
                i++;
            }
            if (bean.Estado != "EnAnalisis") { throw new Exception(" Solo se puede terminar el analisis en estado EnAnalisis"); }
            if (bean.DetFases[4].Operario != obj.DetFases[0].Operario) { throw new Exception("Solo " + bean.DetFases[4].Operario + " puede terminar su analisis"); }
        }
        public void validarRevertirTerminoAnalisisConteo(OIAR_E obj)
        {
            OIAR_E bean = Buscar(obj.DocEntry);
            validarOperacionesDelPeriodo(bean.DocEntryPer);
            if (bean.Estado != "Analizado") { throw new Exception("Solo se puede revertir si estado esta Analizado"); }
            if (bean.DetFases[5].Operario != obj.DetFases[0].Operario) { throw new Exception("Solo " + bean.DetFases[5].Operario + " puede revertir su termino analisis"); }
        }

    }
}