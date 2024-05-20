using Capa_Datos.Ventas_DAO.TablasSql;
using Capa_Entidad.Ventas_ENT;
using Capa_Entidad.Ventas_ENT.TablasSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa_Negocio.Ventas_NEG.TablasSql
{
    public class OREG_N
    {
        OREG_D oregD = new OREG_D();
        private bool cadenaVacia(string cad)
        {
            if (cad == null || cad.Replace(" ", "").Length == 0) { return true; }
            else return false;
        }
        public List<OREG_E> listaRegalos(OREG_E filtro)
        {
            return oregD.listaRegalos(filtro);
        }
        public void registrarNuevoRegalo(OREG_E obj)
        {
            validarNuevoRegalo(obj);
            oregD.registrarNuevoRegalo(obj);
        }
        public void validarNuevoRegalo(OREG_E obj)
        {
            if (cadenaVacia(obj.Categoria)) { throw new Exception("Debe llenar categoria"); }
            if (cadenaVacia(obj.Tipo)) { throw new Exception("Debe llenar tipo"); }
            if (obj.Estado != "Activo" && obj.Estado != "Inactivo") { throw new Exception("Debe seleccionar un estado"); }
            if (obj.StockTotal <= 0) { throw new Exception("Debe ingresar un stock valido"); }

        }
        public OREG_E buscarRegalo(int id)
        {
            return oregD.buscarRegalo(id);
        }
        public int inactivarRegalo(OREG_E obj)
        {
            validarInactivarRegalo(obj.Id);
            return oregD.inactivarRegalo(obj);
        }
        public int revertirInactivarRegalo(OREG_E obj)
        {
            validarRevertirInactivar(obj.Id);
            return oregD.revertirInactivarRegalo(obj);
        }
        public void registrarGestionStock(OREG_E obj, OTRC_E obj2)
        {
            validarGestionStock(obj, obj2);
            if (obj2.Sentido == "Salida") { obj2.Cantidad = -1 * obj2.Cantidad; }
            obj.StockDisp = obj2.Cantidad;
            oregD.registrarGestionStockCrud(obj, obj2);
        }
        public void validarGestionStock(OREG_E obj, OTRC_E obj2)
        {
            OREG_E oReg = oregD.buscarRegalo(obj2.IdReg);
            if (obj2.Sentido == null) { throw new Exception("Debe elegir una accion"); }
            if (obj2.Detalle == null) { throw new Exception("Debe llenar el detalle"); }
            if (obj2.Cantidad < 0) { throw new Exception("La cantidad debe ser un numero positivo"); }
        }
        public void validarInactivarRegalo(int id)
        {
            OREG_E obj = buscarRegalo(id);
            if (obj.Estado != "Activo") { throw new Exception("Solo se puede inactivar si el estado es Activo"); }

        }
        public void validarRevertirInactivar(int id)
        {
            OREG_E obj = buscarRegalo(id);
            if (obj.Estado == "Activo") { throw new Exception("Solo se puede revertir si el estado es Inactivo"); }
        }

    }
}
